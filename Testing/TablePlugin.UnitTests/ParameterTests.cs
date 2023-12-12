namespace TablePlugin.UnitTests
{
    using NUnit.Framework;
    using TablePlugin.Model;

    /// <summary>
    /// Тесты для класса Parameter.
    /// </summary>
    [TestFixture]
    public class ParameterTests
    {
        /// <summary>
        /// Проверяет, что конструктор корректно инициализирует свойства параметра.
        /// </summary>
        [Test]
        public void Parameter_ConstructObject_ObjectIsBuilt()
        {
            // Arrange
            var value = 105.2;
            var minValue = 20.2;
            var maxValue = 105.4;

            // Act
            var actual = new Parameter(value, minValue, maxValue);

            // Assert
            Assert.AreEqual(
                value,
                actual.Value,
                "Должно устанавливаться корректное Value");
            Assert.AreEqual(
                minValue,
                actual.MinValue,
                "Должно устанавливаться корректное MinValue");
            Assert.AreEqual(
                maxValue,
                actual.MaxValue,
                "Должно устанавливаться корректное MaxValue");
        }

        /// <summary>
        /// Проверяет, что свойство Value возвращает ожидаемое значение.
        /// </summary>
        [Test]
        public void Value_ReturnsExpectedValue()
        {
            // Arrange
            var parameter = new Parameter(15.0, 10.0, 20.0);
            var expected = 15.0;

            // Act
            var actual = parameter.Value;

            // Assert
            Assert.AreEqual(
                expected,
                actual,
                "Свойство Value не возвращает ожидаемое значение.");
        }

        /// <summary>
        /// Проверяет, что свойство MinValue возвращает ожидаемое значение.
        /// </summary>
        [Test]
        public void MinValue_ReturnsExpectedValue()
        {
            // Arrange
            var parameter = new Parameter(15.0, 10.0, 20.0);
            var expected = 10.0;

            // Act
            var actual = parameter.MinValue;

            // Assert
            Assert.AreEqual(
                expected,
                actual,
                "Свойство MinValue не возвращает ожидаемое значение.");
        }

        /// <summary>
        /// Проверяет, что свойство MaxValue возвращает ожидаемое значение.
        /// </summary>
        [Test]
        public void MaxValue_ReturnsExpectedValue()
        {
            // Arrange
            var parameter = new Parameter(15.0, 10.0, 20.0);
            var expected = 20.0;

            // Act
            var actual = parameter.MaxValue;

            // Assert
            Assert.AreEqual(
                expected,
                actual,
                "Свойство MaxValue не возвращает ожидаемое значение.");
        }
    }
}
