namespace TablePlugin.UnitTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using TablePlugin.Model;

    /// <summary>
    /// Тесты для класса Parameters.
    /// </summary>
    [TestFixture]
    public class ParametersTests
    {
        /// <summary>
        /// Проверяет геттер ParamsDictionary.
        /// </summary>
        [Test]
        public void ParamsDictionary_GetValues_ReturnsValue()
        {
            // Arrange
            var paramFirst = new Parameter(20.1, 15.0, 20.2);
            var paramSecond = new Parameter(24, 10, 35);

            var paramTypeFirst = ParameterType.ParamType.ShelfFloorDistance;
            var paramTypeSecond = ParameterType.ParamType.TableLength;

            var expected = new Parameters
            {
                ParamsDictionary =
                {
                    { paramTypeFirst, paramFirst },
                    { paramTypeSecond, paramSecond }
                }
            };

            // Act
            var actual = expected.ParamsDictionary;

            // Assert
            Assert.AreEqual(
                expected.ParamsDictionary.Count,
                actual.Count,
                "Количество параметров должно совпадать");
            Assert.Contains(
                paramFirst,
                actual.Values,
                "Список параметров не содержит parameterFirst");
            Assert.Contains(
                paramSecond,
                actual.Values,
                "Список параметров не содержит parameterSecond");
        }

        /// <summary>
        /// Проверяет установку значения null для ParamsDictionary.
        /// </summary>
        [Test]
        public void ParamsDictionary_SetNullDictionary_ReturnsEmptyDictionary()
        {
            // Arrange
            var parameters = new Parameters();

            // Act
            parameters.ParamsDictionary = null;
            var actual = parameters.ParamsDictionary;

            // Assert
            Assert.IsNotNull(actual, "Словарь не должен быть равен null.");
            Assert.IsEmpty(actual, "Словарь должен быть пустым.");
        }

        /// <summary>
        /// Проверяет установку непустого значения для ParamsDictionary.
        /// </summary>
        [Test]
        public void ParamsDictionary_SetValueInDictionary_ValueIsSet()
        {
            // Arrange
            var parameters = new Parameters();
            var notNullValue = new Dictionary<ParameterType.ParamType, Parameter>
            {
                {
                    ParameterType.ParamType.ShelfLength,
                    new Parameter(10, 5, 20)
                },
                {
                    ParameterType.ParamType.SupportSize,
                    new Parameter(25, 10, 20)
                }
            };

            // Act
            parameters.ParamsDictionary = notNullValue;
            var actual = parameters.ParamsDictionary;

            // Assert
            Assert.AreEqual(notNullValue, actual, "Словарь не установлен правильно.");
        }

        /// <summary>
        /// Проверяет добавление параметра в словарь.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="value">Значение параметра.</param>
        /// <param name="minValue">Минимальное значение параметра.</param>
        /// <param name="maxValue">Максимальное значение параметра.</param>
        /// <param name="isValid">Ожидается ли, что параметр будет добавлен.</param>
        [Test]
        [TestCase(
            ParameterType.ParamType.ShelfLength,
            10,
            5,
            20,
            true)]
        [TestCase(
            ParameterType.ParamType.SupportSize,
            25,
            10,
            20,
            false)]
        public void AddParameter_ValidateParameter_IsValid(
            ParameterType.ParamType paramType,
            double value,
            double minValue,
            double maxValue,
            bool isValid)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(value, minValue, maxValue);

            // Act
            parameters.AddParameter(paramType, parameter);
            var actual = parameters.ParamsDictionary.TryGetValue(
                paramType,
                out var actualParameter);

            // Assert
            Assert.AreEqual(isValid, actual, "Ожидается, что параметр будет добавлен");
            if (isValid)
            {
                Assert.AreEqual(
                    value,
                    actualParameter.Value,
                    "Значение параметра не совпадает");
                Assert.AreEqual(
                    minValue,
                    actualParameter.MinValue,
                    "Минимальное значение параметра не совпадает");
                Assert.AreEqual(
                    maxValue,
                    actualParameter.MaxValue,
                    "Максимальное значение параметра не совпадает");
            }
        }

        /// <summary>
        /// Проверяет валидацию параметра.
        /// </summary>
        /// <param name="value">Значение параметра.</param>
        /// <param name="minValue">Минимальное значение параметра.</param>
        /// <param name="maxValue">Максимальное значение параметра.</param>
        /// <param name="isValid">Ожидается ли, что параметр проходит валидацию.</param>
        [Test]
        [TestCase(10, 5, 20, true)]
        [TestCase(25, 10, 20, false)]
        public void Validate_ValidateParameter_IsValid(
            double value,
            double minValue,
            double maxValue,
            bool isValid)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(value, minValue, maxValue);

            // Act & Assert
            if (isValid)
            {
                Assert.DoesNotThrow(
                    () => parameters.Validate("SupportSize", parameter));
            }
            else
            {
                var ex = Assert.Throws<Exception>(
                    () => parameters.Validate("SupportSize", parameter));
                StringAssert.Contains("SupportSize", ex.Message);
            }
        }
    }
}
