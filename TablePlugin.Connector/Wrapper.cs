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
        public void CreateTable(
            double rectX,
            double rectY,
            double rectWidth,
            double tableLength,
            double tableWidth)
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
        public void CreateShelf(
            double rectX,
            double rectY,
            double rectHeight,
            double tableLength,
            double shelfLength,
            double shelfWidth,
            double legSize)
        {
            var document3D = (ksDocument3D)_kompasObject.ActiveDocument3D();
            var part = (ksPart)document3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketchDefinition = (ksSketchDefinition)sketch.GetDefinition();
            var plane = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            var offsetPlane = CreateOffsetPlane(
                part,
                plane,
                (tableLength + legSize - shelfLength) / 2);

            ksSketchDefinition.SetPlane(offsetPlane);
            sketch.Create();
            ksSketchDefinition.BeginEdit();

            var ksSketchTopDefinition =
                (ksSketchDefinition)sketch.GetDefinition();

            CreateRectangle(-rectX, -rectY, shelfWidth, rectHeight);

            ksSketchTopDefinition.EndEdit();

            ExtrudeOperation(part, sketch, shelfLength);
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
    }
}
