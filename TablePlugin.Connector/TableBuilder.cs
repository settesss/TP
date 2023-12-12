namespace TablePlugin.Connector
{
    using TablePlugin.Model;

    /// <summary>
    /// Описывает строитель модели.
    /// </summary>
    public class TableBuilder
    {
        /// <summary>
        /// Половина значения для корректного построения.
        /// </summary>
        private const int HalfValue = 2;

        /// <summary>
        /// Конвертированное значение из сантиметров в миллиметры.
        /// </summary>
        private const int ConvertMillimeters = 10;

        /// <summary>
        /// Обертка для взаимодействия с API.
        /// </summary>
        private readonly Wrapper _wrapper = new Wrapper();

        /// <summary>
        /// Строит модель столика.
        /// </summary>
        /// <param name="parameters">Параметры для построения.</param>
        public void Build(Parameters parameters)
        {
            _wrapper.OpenCad();
            _wrapper.CreateDocument3D();

            BuildTable(parameters);
            BuildShelf(parameters);
        }

        /// <summary>
        /// Строит столик.
        /// </summary>
        /// <param name="parameters">Параметры для построения.</param>
        private void BuildTable(Parameters parameters)
        {
            var tableLength =
                parameters.ParamsDictionary[ParameterType.ParamType.TableLength].Value;
            var tableWidth =
                parameters.ParamsDictionary[ParameterType.ParamType.TableWidth].Value;
            var tableHeight =
                parameters.ParamsDictionary[ParameterType.ParamType.TableHeight].Value;
            var supportSize =
                parameters.ParamsDictionary[ParameterType.ParamType.SupportSize].Value;

            _wrapper.CreateTable(
               (tableWidth / (HalfValue * ConvertMillimeters)) -
               (supportSize / ConvertMillimeters),
               tableHeight / ConvertMillimeters,
               supportSize / ConvertMillimeters,
               tableLength / ConvertMillimeters,
               tableWidth / ConvertMillimeters);
        }

        /// <summary>
        /// Строит полку.
        /// </summary>
        /// <param name="parameters">Параметры для построения.</param>
        private void BuildShelf(Parameters parameters)
        {
            var shelfLength =
                parameters.ParamsDictionary[ParameterType.ParamType.ShelfLength].Value;
            var shelfWidth =
                parameters.ParamsDictionary[ParameterType.ParamType.ShelfWidth].Value;
            var tableLength =
                parameters.ParamsDictionary[ParameterType.ParamType.TableLength].Value;
            var supportLength =
                parameters.ParamsDictionary[ParameterType.ParamType.SupportSize].Value;
            var shelfHeight =
                parameters.ParamsDictionary[ParameterType.ParamType.ShelfHeight].Value;
            var shelfFloorDistance =
                parameters.ParamsDictionary[ParameterType.ParamType.ShelfFloorDistance].Value;
            var supportSize =
                parameters.ParamsDictionary[ParameterType.ParamType.SupportSize].Value;

            _wrapper.CreateShelf(
                shelfWidth / (HalfValue * ConvertMillimeters),
                (shelfFloorDistance + shelfHeight) / ConvertMillimeters,
                shelfHeight / ConvertMillimeters,
                (tableLength - supportLength) / ConvertMillimeters,
                shelfLength / ConvertMillimeters,
                shelfWidth / ConvertMillimeters,
                supportSize / ConvertMillimeters);
        }
    }
}
