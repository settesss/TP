namespace TablePlugin.Model
{
    /// <summary>
    /// Описывает валидатор.
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// Проверяет на корректность значения параметров, связанных с полкой.
        /// </summary>
        /// <param name="shelfValue">Значение параметра.</param>
        /// <param name="shelfMaxValue">Максимальное значение параметра.</param>
        /// <param name="error">Текст ошибки.</param>
        /// <returns>True, если строка
        /// пустая или false, если есть ошибки.</returns>
        public static bool ValidateShelfValue(
            double shelfValue,
            double shelfMaxValue,
            out string error)
        {
            error = string.Empty;

            if (shelfValue > shelfMaxValue)
            {
                error = "Неверное значение для параметра полки";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Вычисляет максимальное значение у расстояния от пола до полки.
        /// </summary>
        /// <param name="tableHeight">Высота стола.</param>
        /// <param name="legSize">Размер ножки.</param>
        /// <param name="shelfHeight">Высота полки.</param>
        /// <returns>Максимальное значение расстояния от пола до полки.</returns>
        public static double CalculateShelfFloorDistanceMax(
            double tableHeight,
            double legSize,
            double shelfHeight)
        {
            return tableHeight - (2 * legSize) - shelfHeight;
        }

        /// <summary>
        /// Вычисляет максимальное значение у параметров полки.
        /// </summary>
        /// <param name="tableSize">Параметр стола, который соответствует параметру полки.
        /// Например, длина стола - длине полки.</param>
        /// <param name="legSize">Размер ножки.</param>
        /// <returns>Максимальное значение параметра полки.</returns>
        public static double CalculateShelfMaxValue(double tableSize, double legSize)
        {
            return tableSize - (2 * legSize);
        }

        /// <summary>
        /// Проверяет на корректность введенное значение параметра.
        /// </summary>
        /// <param name="parameter">Параметр с текущим значением.</param>
        /// <param name="error">Строка с ошибкой.</param>
        /// <returns>True, если строка
        /// пустая или false, если есть ошибки.</returns>
        public bool Validate(Parameter parameter, out string error)
        {
            error = string.Empty;

            if (parameter.Value < parameter.MinValue)
            {
                error = " меньше минимального допустимого значения";
            }
            else if (parameter.Value > parameter.MaxValue)
            {
                error = " больше максимального допустимого значения";
            }

            return string.IsNullOrEmpty(error);
        }
    }
}
