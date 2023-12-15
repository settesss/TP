namespace TablePlugin.Connector
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Kompas6API5;
    using Kompas6Constants3D;

    /// <summary>
    /// Описывает обертку для взаимодействия с API.
    /// </summary>
    public class Wrapper
    {
        /// <summary>
        /// Экземпляр Компас-3D.
        /// </summary>
        private static KompasObject _kompasObject;

        /// <summary>
        /// Открывает Компас-3D.
        /// </summary>
        public void OpenCad()
        {
            if (_kompasObject == null)
            {
                var kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                _kompasObject = (KompasObject)Activator.CreateInstance(kompasType);
            }

            if (_kompasObject != null)
            {
                var retry = true;
                short tried = 0;
                while (retry)
                {
                    try
                    {
                        tried++;
                        _kompasObject.Visible = true;
                        retry = false;
                    }
                    catch (COMException)
                    {
                        var kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                        _kompasObject =
                            (KompasObject)Activator.CreateInstance(kompasType);

                        if (tried > 3)
                        {
                            retry = false;
                        }
                    }
                }

                _kompasObject.ActivateControllerAPI();
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Создает 3D-документ для построения детали.
        /// </summary>
        public void CreateDocument3D()
        {
            var document3D = (ksDocument3D)_kompasObject.Document3D();
            document3D.Create();
        }

        /// <summary>
        /// Создает эскиз на плоскости.
        /// </summary>
        /// <param name="sketchPlane">Плоскость для эскиза.</param>
        /// <returns>Созданный скетч.</returns>
        public (ksPart createdPart, ksEntity createdSketch) CreateSketch(
            short sketchPlane)
        {
            var document3D = (ksDocument3D)_kompasObject.ActiveDocument3D();
            var part = (ksPart)document3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketchDefinition = (ksSketchDefinition)sketch.GetDefinition();
            var plane = (ksEntity)part.GetDefaultEntity(sketchPlane);
            ksSketchDefinition.SetPlane(plane);
            sketch.Create();
            ksSketchDefinition.BeginEdit();

            return (createdPart: part, createdSketch: sketch);
        }

        /// <summary>
        /// Строит столик.
        /// </summary>
        /// <param name="rectX">Начальная точка, равная ширине стола, деленной на два.</param>
        /// <param name="rectY">Координата Y, равная высоте столика.</param>
        /// <param name="rectWidth">Размер ножки.</param>
        /// <param name="tableLength">Длина столика.</param>
        /// <param name="tableWidth">Ширина столика.</param>
        /// <param name="wheelSize">Размер колесика.</param>
        public void CreateTable(
            double rectX,
            double rectY,
            double rectWidth,
            double tableLength,
            double tableWidth,
            double wheelSize)
        {
            var halfValue = 2;
            var sketchTuple = CreateSketch((short)Obj3dType.o3d_planeXOY);
            var createdPart = sketchTuple.createdPart;
            var createdSketch = sketchTuple.createdSketch;
            var ksSketchDefinition =
                (ksSketchDefinition)createdSketch.GetDefinition();
            var planeXoz =
                (ksEntity)createdPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            var planeYoz =
                (ksEntity)createdPart.GetDefaultEntity((short)Obj3dType.o3d_planeYOZ);

            CreateRectangle(rectX, 0, rectWidth, rectWidth);

            ksSketchDefinition.EndEdit();

            var extrude = ExtrudeOperation(
                createdPart,
                createdSketch,
                rectY);

            MirrorOperation(createdPart, extrude, planeYoz);

            var planeOffset =
                CreateOffsetPlane(
                    createdPart,
                    planeXoz,
                    tableLength / halfValue);
            var mirrorEntity = MirrorOperation(createdPart, extrude, planeOffset);

            MirrorOperation(createdPart, mirrorEntity, planeYoz);

            CreateTop(rectX, rectY, rectWidth, tableLength, tableWidth);

            if (wheelSize != 0)
            {
                CreateWheels(
                    rectX,
                    rectWidth,
                    wheelSize,
                    rectWidth,
                    planeOffset,
                    planeYoz);
            }
        }

        /// <summary>
        /// Строит полку.
        /// </summary>
        /// <param name="rectX">Координата X для начальной точки полки.</param>
        /// <param name="rectY">Координата Y для высоты полки.</param>
        /// <param name="rectHeight">Высота полки.</param>
        /// <param name="tableLength">Длина столика.</param>
        /// <param name="shelfLength">Длина полки.</param>
        /// <param name="shelfWidth">Ширина полки.</param>
        /// <param name="legSize">Размер ножки.</param>
        /// <param name="bracingSize">Размер крепления.</param>
        public void CreateShelf(
            double rectX,
            double rectY,
            double rectHeight,
            double tableLength,
            double shelfLength,
            double shelfWidth,
            double legSize,
            double bracingSize)
        {
            var halfValue = 2;
            var document3D = (ksDocument3D)_kompasObject.ActiveDocument3D();
            var part = (ksPart)document3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketchDefinition = (ksSketchDefinition)sketch.GetDefinition();
            var planeXoz = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            var planeXoy = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            var offsetPlane = CreateOffsetPlane(
                part,
                planeXoz,
                (tableLength + legSize - shelfLength) / halfValue);

            ksSketchDefinition.SetPlane(offsetPlane);
            sketch.Create();
            ksSketchDefinition.BeginEdit();

            var ksSketchTopDefinition =
                (ksSketchDefinition)sketch.GetDefinition();

            CreateRectangle(-rectX, -rectY, shelfWidth, rectHeight);

            ksSketchTopDefinition.EndEdit();

            ExtrudeOperation(part, sketch, shelfLength);

            CreateBracings(
                part,
                planeXoy,
                offsetPlane,
                rectX,
                rectY,
                tableLength,
                shelfLength,
                legSize,
                bracingSize);
        }

        /// <summary>
        /// Создает прямоугольник.
        /// </summary>
        /// <param name="rectX">Координата X.</param>
        /// <param name="rectY">Координата Y.</param>
        /// <param name="rectWidth">Ширина прямоугольника.</param>
        /// <param name="rectHeight">Высота прямоугольника.</param>
        private static void CreateRectangle(
            double rectX,
            double rectY,
            double rectWidth,
            double rectHeight)
        {
            var rectangleObjType = 91;
            var rectangleParam =
                (ksRectangleParam)_kompasObject.GetParamStruct((short)rectangleObjType);
            var document2D = (ksDocument2D)_kompasObject.ActiveDocument2D();

            rectangleParam.x = rectX;
            rectangleParam.y = rectY;
            rectangleParam.ang = 0;
            rectangleParam.width = rectWidth;
            rectangleParam.height = rectHeight;
            rectangleParam.style = 1;
            document2D.ksRectangle(rectangleParam, 0);
        }

        /// <summary>
        /// Строит окружность.
        /// </summary>
        /// <param name="centerX">Координата центра окружности по оси X.</param>
        /// <param name="centerY">Координата центра окружности по оси Y.</param>
        /// <param name="radius">Радиус окружности.</param>
        private static void CreateCircle(
            double centerX,
            double centerY,
            double radius)
        {
            var document2D = (ksDocument2D)_kompasObject.ActiveDocument2D();

            document2D.ksCircle(centerX, centerY, radius, 1);
        }

        /// <summary>
        /// Отзеркаливает объект.
        /// </summary>
        /// <param name="part">Объект для получения операции.</param>
        /// <param name="entityToMirror">Объект для зеркального отображения.</param>
        /// <param name="mirrorPlane">Плоскость для зеркального отображения.</param>
        /// <returns>Зеркальный объект.</returns>
        private static ksEntity MirrorOperation(
            ksPart part,
            ksEntity entityToMirror,
            ksEntity mirrorPlane)
        {
            var mirrorEntity =
                (ksEntity)part.NewEntity((short)Obj3dType.o3d_mirrorOperation);
            var mirrorDefinition =
                (ksMirrorCopyDefinition)mirrorEntity.GetDefinition();

            mirrorDefinition.GetOperationArray().Add(entityToMirror);
            mirrorDefinition.SetPlane(mirrorPlane);
            mirrorEntity.Create();

            return mirrorEntity;
        }

        /// <summary>
        /// Выдавливает объект.
        /// </summary>
        /// <param name="part">Объект для получения операции.</param>
        /// <param name="sketch">Эскиз для выдавливания.</param>
        /// <param name="depth">Глубина выдавливания.</param>
        /// <returns>Выдавленный объект.</returns>
        private static ksEntity ExtrudeOperation(
            ksPart part,
            ksEntity sketch,
            double depth)
        {
            var entityExtrude =
                (ksEntity)part.NewEntity((short)Obj3dType.o3d_bossExtrusion);
            var extrusionDefinition =
                (ksBossExtrusionDefinition)entityExtrude.GetDefinition();

            extrusionDefinition.directionType = (short)Direction_Type.dtNormal;
            extrusionDefinition.SetSideParam(true, (short)End_Type.etBlind, depth);
            extrusionDefinition.SetSketch(sketch);
            entityExtrude.Create();

            return entityExtrude;
        }

        /// <summary>
        /// Создает смещенную плоскость.
        /// </summary>
        /// <param name="part">Объект для получения операции.</param>
        /// <param name="plane">Плоскость, относительно которой надо получить смещенную.</param>
        /// <param name="offset">Расстояние смещения.</param>
        /// <returns>Смещенная плоскость.</returns>
        private static ksEntity CreateOffsetPlane(ksPart part, ksEntity plane, double offset)
        {
            var planeOffset = (ksEntity)part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var planeOffsetDefinition = (ksPlaneOffsetDefinition)planeOffset.GetDefinition();
            planeOffsetDefinition.SetPlane(plane);
            planeOffsetDefinition.offset = offset;
            planeOffset.Create();

            return planeOffset;
        }

        /// <summary>
        /// Строит столешницу.
        /// </summary>
        /// <param name="rectX">Координата X, равная ширине стола, деленной на два.</param>
        /// <param name="rectY">Координата Y, равная высоте столика за
        /// вычетом высоты столешницы.</param>
        /// <param name="rectWidth">Размер ножки.</param>
        /// <param name="tableLength">Длина столика.</param>
        /// <param name="tableWidth">Ширина столика.</param>
        private void CreateTop(
            double rectX,
            double rectY,
            double rectWidth,
            double tableLength,
            double tableWidth)
        {
            var sketchTop = CreateSketch((short)Obj3dType.o3d_planeXOZ);
            var createdPartTop = sketchTop.createdPart;
            var createdSketchTop = sketchTop.createdSketch;
            var ksSketchTopDefinition =
                (ksSketchDefinition)createdSketchTop.GetDefinition();

            CreateRectangle(-rectX - rectWidth, -rectY, tableWidth, rectWidth);

            ksSketchTopDefinition.EndEdit();

            ExtrudeOperation(createdPartTop, createdSketchTop, tableLength);
        }

        /// <summary>
        /// Строит колесики.
        /// </summary>
        /// <param name="rectX">Координата X начала построения.</param>
        /// <param name="rectWidth">Размер ножки.</param>
        /// <param name="wheelSize">Размер колесика.</param>
        /// <param name="depth">Глубина выдавливания.</param>
        /// <param name="xozMirrorPlane">Плоскость XOZ для зеркального отображения.</param>
        /// <param name="yozMirrorPlane">Плоскость YOZ для зеркального отображения.</param>
        private void CreateWheels(
            double rectX,
            double rectWidth,
            double wheelSize,
            double depth,
            ksEntity xozMirrorPlane,
            ksEntity yozMirrorPlane)
        {
            var document3D = (ksDocument3D)_kompasObject.ActiveDocument3D();
            var part = (ksPart)document3D.GetPart((short)Part_Type.pTop_Part);
            var planeXoz =
                (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);

            var sketchWheels = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketchWheelsDefinition = (ksSketchDefinition)sketchWheels.GetDefinition();

            ksSketchWheelsDefinition.SetPlane(planeXoz);
            sketchWheels.Create();
            ksSketchWheelsDefinition.BeginEdit();

            CreateCircle(rectX + (rectWidth / 2), wheelSize, wheelSize);

            ksSketchWheelsDefinition.EndEdit();

            var extrude = ExtrudeOperation(
                part,
                sketchWheels,
                depth);

            MirrorOperation(part, extrude, yozMirrorPlane);

            var mirrorEntity = MirrorOperation(part, extrude, xozMirrorPlane);

            MirrorOperation(part, mirrorEntity, yozMirrorPlane);
        }

        /// <summary>
        /// Строит крепления для полки.
        /// </summary>
        /// <param name="part">Объект детали.</param>
        /// <param name="plane">Плоскость для создания смещенной плоскости.</param>
        /// <param name="offsetPlane">Смещенная плоскость.</param>
        /// <param name="rectX">Координата X для построения креплений.</param>
        /// <param name="rectY">Координата X для выдавливания и смещенной плоскости.</param>
        /// <param name="tableLength">Длина столика.</param>
        /// <param name="shelfLength">Длина полки.</param>
        /// <param name="legSize">Размер ножки.</param>
        /// <param name="bracingSize">Размер крепления.</param>
        private void CreateBracings(
            ksPart part,
            ksEntity plane,
            ksEntity offsetPlane,
            double rectX,
            double rectY,
            double tableLength,
            double shelfLength,
            double legSize,
            double bracingSize)
        {
            var halfValue = 2;
            var bracingSketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var ksBracingSketchDefinition = (ksSketchDefinition)bracingSketch.GetDefinition();
            var braceOffsetPlane = CreateOffsetPlane(
                part,
                plane,
                rectY);
            ksBracingSketchDefinition.SetPlane(braceOffsetPlane);
            bracingSketch.Create();
            ksBracingSketchDefinition.BeginEdit();

            var ksBraceSketchDefinition =
                (ksSketchDefinition)bracingSketch.GetDefinition();
            var planeYoz =
                (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeYOZ);

            CreateRectangle(
                -rectX,
                (tableLength + legSize - shelfLength) / halfValue,
                bracingSize,
                bracingSize);

            ksBracingSketchDefinition.EndEdit();
            ksBraceSketchDefinition.EndEdit();

            var extrude = ExtrudeOperation(
                part,
                bracingSketch,
                rectY);
            var extrudeDefinition = (ksBossExtrusionDefinition)extrude.GetDefinition();
            extrudeDefinition.SetSideParam(true, 1);
            extrude.Create();

            MirrorOperation(part, extrude, planeYoz);

            var planeOffset =
                CreateOffsetPlane(
                    part,
                    offsetPlane,
                    shelfLength / halfValue);
            var mirrorEntity = MirrorOperation(part, extrude, planeOffset);

            MirrorOperation(part, mirrorEntity, planeYoz);
        }
    }
}
