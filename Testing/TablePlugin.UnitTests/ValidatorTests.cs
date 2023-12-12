namespace TablePlugin.UnitTests
{
    using NUnit.Framework;
    using TablePlugin.Model;

    /// <summary>
    /// Описывает проверку валидации параметра.
    /// </summary>
    [TestFixture]
    public class ValidatorTests
    {
        /// <summary>
        /// Проверяет валидацию значения параметра, которое меньше минимального.
        /// </summary>
        /// <param name="value">Значение параметра.</param>
        /// <param name="minValue">Минимальное значение.</param>
        /// <param name="maxValue">Максимальное значение.</param>
        /// <param name="expected">Ожидаемый результат валидации.</param>
        /// <param name="expectedError">Ожидаемая ошибка валидации.</param>
        [Test]
        [TestCase(
            5,
            10,
            20,
            false,
            " меньше минимального допустимого значения")]
        [TestCase(
            25,
            10,
            20,
            false,
            " больше максимального допустимого значения")]
        [TestCase(
            15,
            10,
            20,
            true,
            "")]
        public void Validate_ParameterValue_RangeValidation(
            int value, int minValue, int maxValue, bool expected, string expectedError)
        {
            // Arrange
            var parameter = new Parameter(value, minValue, maxValue);

            // Act
            var actual = new Validator().Validate(parameter, out var error);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected ? string.Empty : expectedError, error);
        }

        /// <summary>
        /// Проверяет валидацию некорректного значения параметра полки.
        /// </summary>
        [Test]
        public void ValidateShelfValue_InvalidShelfValue_ReturnsFalse()
        {
            // Arrange
            var shelfValue = 15;
            var shelfMaxValue = 10;
            var expected = false;

            // Act
            var actual =
                Validator.ValidateShelfValue(shelfValue, shelfMaxValue, out var error);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("Неверное значение для параметра полки", error);
        }

        /// <summary>
        /// Проверяет валидацию корректного значения параметра полки.
        /// </summary>
        [Test]
        public void ValidateShelfValue_ValidShelfValue_ReturnsTrue()
        {
            // Arrange
            var shelfValue = 15;
            var shelfMaxValue = 20;
            var expected = true;

            // Act
            var actual =
                Validator.ValidateShelfValue(shelfValue, shelfMaxValue, out var error);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(string.IsNullOrEmpty(error));
        }

        /// <summary>
        /// Проверяет вычисление максимального значения расстояния от пола до полки.
        /// </summary>
        [Test]
        public void CalculateShelfFloorDistanceMax_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            var tableHeight = 100;
            var legSize = 10;
            var shelfHeight = 20;
            var expected = 60;

            // Act
            var actual =
                Validator.CalculateShelfFloorDistanceMax(tableHeight, legSize, shelfHeight);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Проверяет вычисление максимального значения параметра полки.
        /// </summary>
        [Test]
        public void CalculateShelfMaxValue_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            var tableSize = 100;
            var legSize = 20;
            double expected = 60;

            // Act
            var actual = Validator.CalculateShelfMaxValue(tableSize, legSize);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
