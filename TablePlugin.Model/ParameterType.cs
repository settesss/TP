namespace TablePlugin.Model
{
    /// <summary>
    /// Описывает названия параметров.
    /// </summary>
    public class ParameterType
    {
        /// <summary>
        /// Перечисление названий.
        /// </summary>
        public enum ParamType
        {
            /// <summary>
            /// Длина столика.
            /// </summary>
            TableLength,

            /// <summary>
            /// Ширина столика.
            /// </summary>
            TableWidth,

            /// <summary>
            /// Высота столика.
            /// </summary>
            TableHeight,

            /// <summary>
            /// Длина полки.
            /// </summary>
            ShelfLength,

            /// <summary>
            /// Ширина полки.
            /// </summary>
            ShelfWidth,

            /// <summary>
            /// Высота полки.
            /// </summary>
            ShelfHeight,

            /// <summary>
            /// Размер ножки.
            /// </summary>
            SupportSize,

            /// <summary>
            /// Расстояние от полки до пола.
            /// </summary>
            ShelfFloorDistance,

            /// <summary>
            /// Размер крепления для полки.
            /// </summary>
            BracingSize,

            /// <summary>
            /// Размер колесика.
            /// </summary>
            WheelSize
        }
    }
}
