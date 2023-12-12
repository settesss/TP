namespace TablePlugin.Model
{
    /// <summary>
    /// Описывает параметр.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Создает экземпляр параметра.
        /// </summary>
        /// <param name="minValue">Минимальное значение параметра.</param>
        /// <param name="maxValue">Максимальное значение параметра.</param>
        /// <param name="value">Значение параметра.</param>
        public Parameter(double value, double minValue, double maxValue)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// Возвращает или задает значение параметра.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Возвращает или задает минимальное значение параметра.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Возвращает или задает максимальное значение параметра.
        /// </summary>
        public double MaxValue { get; set; }
    }
}
