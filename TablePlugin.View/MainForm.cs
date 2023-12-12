namespace TablePlugin.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using TablePlugin.Connector;
    using TablePlugin.Model;

    /// <summary>
    /// Описывает главную форму.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Русские названия параметров.
        /// </summary>
        private static Dictionary<ParameterType.ParamType, string> _paramLocalization;

        /// <summary>
        /// Строитель.
        /// </summary>
        private readonly TableBuilder _builder = new TableBuilder();

        /// <summary>
        /// Параметры модели.
        /// </summary>
        private readonly Parameters _parameters = new Parameters();

        /// <summary>
        /// Словарь, связывающий <see cref="TextBox"/> с типом параметра.
        /// </summary>
        private readonly Dictionary<TextBox, ParameterType.ParamType> _textBoxesTypes =
            new Dictionary<TextBox, ParameterType.ParamType>();

        /// <summary>
        /// Словарь, связывающий <see cref="TextBox"/> с <see cref="Label"/> диапазона значений.
        /// </summary>
        private readonly Dictionary<TextBox, Control> _labelToTextBox =
            new Dictionary<TextBox, Control>();

        /// <summary>
        /// Словарь, связывающий <see cref="TextBox"/> с <see cref="Label"/> ошибки.
        /// </summary>
        private readonly Dictionary<TextBox, Control> _errorLabelToTextBox =
            new Dictionary<TextBox, Control>();

        /// <summary>
        /// Проверка на наличие ошибок на форме.
        /// </summary>
        private bool _exceptions;

        /// <summary>
        /// Инициализирует новый экземпляр класса  <see cref="MainForm"/>.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeParameters();

            _paramLocalization = new Dictionary<ParameterType.ParamType, string>
            {
                { ParameterType.ParamType.TableLength, "Длина столика" },
                { ParameterType.ParamType.TableWidth, "Ширина столика" },
                { ParameterType.ParamType.TableHeight, "Высота столика" },
                { ParameterType.ParamType.ShelfLength, "Длина полки" },
                { ParameterType.ParamType.ShelfWidth, "Ширина полки" },
                { ParameterType.ParamType.ShelfHeight, "Высота полки" },
                { ParameterType.ParamType.SupportSize, "Размер опоры" },
                { ParameterType.ParamType.ShelfFloorDistance, "Расстояние до пола" },
            };

            _textBoxesTypes[TableLengthTextBox] = ParameterType.ParamType.TableLength;
            _textBoxesTypes[TableWidthTextBox] = ParameterType.ParamType.TableWidth;
            _textBoxesTypes[TableHeightTextBox] = ParameterType.ParamType.TableHeight;
            _textBoxesTypes[ShelfLengthTextBox] = ParameterType.ParamType.ShelfLength;
            _textBoxesTypes[ShelfWidthTextBox] = ParameterType.ParamType.ShelfWidth;
            _textBoxesTypes[ShelfHeightTextBox] = ParameterType.ParamType.ShelfHeight;
            _textBoxesTypes[SupportSizeTextBox] = ParameterType.ParamType.SupportSize;
            _textBoxesTypes[ShelfFloorDistanceTextBox] =
                ParameterType.ParamType.ShelfFloorDistance;

            _errorLabelToTextBox[TableLengthTextBox] = TableLengthExceptionLabel;
            _errorLabelToTextBox[TableWidthTextBox] = TableWidthExceptionLabel;
            _errorLabelToTextBox[TableHeightTextBox] = TableHeightExceptionLabel;
            _errorLabelToTextBox[ShelfLengthTextBox] = ShelfLengthExceptionLabel;
            _errorLabelToTextBox[ShelfWidthTextBox] = ShelfWidthExceptionLabel;
            _errorLabelToTextBox[ShelfHeightTextBox] = ShelfHeightExceptionLabel;
            _errorLabelToTextBox[SupportSizeTextBox] = SupportSizeExceptionLabel;
            _errorLabelToTextBox[ShelfFloorDistanceTextBox] = ShelfFloorDistanceExceptionLabel;

            _labelToTextBox[TableLengthTextBox] = TableLengthRangeLabel;
            _labelToTextBox[TableWidthTextBox] = TableWidthRangeLabel;
            _labelToTextBox[TableHeightTextBox] = TableHeightRangeLabel;
            _labelToTextBox[ShelfLengthTextBox] = ShelfLengthRangeLabel;
            _labelToTextBox[ShelfWidthTextBox] = ShelfWidthRangeLabel;
            _labelToTextBox[ShelfHeightTextBox] = ShelfHeightRangeLabel;
            _labelToTextBox[SupportSizeTextBox] = SupportSizeRangeLabel;
            _labelToTextBox[ShelfFloorDistanceTextBox] = ShelfFloorDistanceRangeLabel;
        }

        /// <summary>
        /// Добавляет параметр в словарь.
        /// </summary>
        /// <param name="value">Текущее значение.</param>
        /// <param name="minValue">Минимальное значение.</param>
        /// <param name="maxValue">Максимальное значение.</param>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="dictionary">Словарь для добавления.</param>
        private void AddParameterToDict(
            string value,
            double minValue,
            double maxValue,
            ParameterType.ParamType paramType,
            Dictionary<ParameterType.ParamType, Parameter> dictionary)
        {
            if (!double.TryParse(value, out var parsedValue))
            {
                _exceptions = true;
            }

            dictionary.Add(paramType, new Parameter(parsedValue, minValue, maxValue));
        }

        /// <summary>
        /// Инициализирует и заполняет словарь параметров значениями.
        /// </summary>
        private void InitializeParameters()
        {
            try
            {
                var dictionary = new Dictionary<ParameterType.ParamType, Parameter>();

                AddParameterToDict(
                    TableLengthTextBox.Text,
                    600,
                    1200,
                    ParameterType.ParamType.TableLength,
                    dictionary);
                AddParameterToDict(
                    TableWidthTextBox.Text,
                    600,
                    1200,
                    ParameterType.ParamType.TableWidth,
                    dictionary);
                AddParameterToDict(
                    TableHeightTextBox.Text,
                    400,
                    500,
                    ParameterType.ParamType.TableHeight,
                    dictionary);
                AddParameterToDict(
                    ShelfLengthTextBox.Text,
                    300,
                    720,
                    ParameterType.ParamType.ShelfLength,
                    dictionary);
                AddParameterToDict(
                    ShelfWidthTextBox.Text,
                    300,
                    720,
                    ParameterType.ParamType.ShelfWidth,
                    dictionary);
                AddParameterToDict(
                    ShelfHeightTextBox.Text,
                    10,
                    40,
                    ParameterType.ParamType.ShelfHeight,
                    dictionary);
                AddParameterToDict(
                    SupportSizeTextBox.Text,
                    30,
                    50,
                    ParameterType.ParamType.SupportSize,
                    dictionary);
                AddParameterToDict(
                    ShelfFloorDistanceTextBox.Text,
                    30,
                    345,
                    ParameterType.ParamType.ShelfFloorDistance,
                    dictionary);

                _parameters.ParamsDictionary = dictionary;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Возвращает русскоязычный параметр.
        /// </summary>
        /// <param name="type">Тип параметра.</param>
        /// <returns>Локализованный параметр.</returns>
        private string GetLocalize(ParameterType.ParamType type)
        {
            string description;

            if (_paramLocalization.TryGetValue(type, out description))
            {
                return description;
            }

            return string.Empty;
        }

        /// <summary>
        /// Устанавливает ошибку в <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox"><see cref="TextBox"/> с ошибкой.</param>
        /// <param name="error">Сообщение об ошибке.</param>
        private void SetError(TextBox textBox, string error)
        {
            _errorLabelToTextBox[textBox].Text = error;
            textBox.BackColor = Color.FromArgb(255, 182, 193);
            _exceptions = true;
        }

        /// <summary>
        /// Очищает <see cref="TextBox"/> с корректным значением.
        /// </summary>
        /// <param name="textBox"><see cref="TextBox"/> с корректным значением.</param>
        private void ClearError(TextBox textBox)
        {
            _errorLabelToTextBox[textBox].Text = string.Empty;
            textBox.BackColor = Color.White;
            _exceptions = false;
        }

        /// <summary>
        /// Обновляет <see cref="Label"/> с диапазоном возможных значений.
        /// </summary>
        /// <param name="label"><see cref="Label"/> с диапазоном.</param>
        /// <param name="minValue">Новое минимальное значение.</param>
        /// <param name="maxValue">Новое максимальное значение.</param>
        private void UpdateRangeLabel(
            Control label,
            double minValue,
            double maxValue)
        {
            var formattedMaxValue = maxValue.ToString("F2");
            label.Text = $"{minValue} - {formattedMaxValue} мм.";
        }

        /// <summary>
        /// Проверяет зависимые параметры.
        /// </summary>
        private void CheckDependentParams()
        {
            CheckShelfParameter(
                ParameterType.ParamType.ShelfWidth,
                ShelfWidthTextBox,
                ShelfWidthRangeLabel,
                ParameterType.ParamType.TableWidth);

            CheckShelfParameter(
                ParameterType.ParamType.ShelfLength,
                ShelfLengthTextBox,
                ShelfLengthRangeLabel,
                ParameterType.ParamType.TableLength);

            CheckShelfParameter(
                ParameterType.ParamType.ShelfFloorDistance,
                ShelfFloorDistanceTextBox,
                ShelfFloorDistanceRangeLabel,
                ParameterType.ParamType.TableHeight);
        }

        /// <summary>
        /// Проверяет параметры, связанные с размерами полки и расстоянием до неё.
        /// </summary>
        /// <param name="shelfParamType">Тип параметра, связанный с полкой.</param>
        /// <param name="textBox"><see cref="TextBox"/> для вывода ошибки.</param>
        /// <param name="rangeLabel"><see cref="Label"/> с диапазоном значений для
        /// обновления.</param>
        /// <param name="tableParamType">Тип параметра, связанного со столиком.</param>
        private void CheckShelfParameter(
            ParameterType.ParamType shelfParamType,
            TextBox textBox,
            Control rangeLabel,
            ParameterType.ParamType tableParamType)
        {
            var tableSize =
                _parameters.ParamsDictionary[tableParamType].Value;
            var supportSize =
                _parameters.ParamsDictionary[ParameterType.ParamType.SupportSize].Value;
            var shelfMinValue =
                _parameters.ParamsDictionary[shelfParamType].MinValue;
            var tableHeight =
                _parameters.ParamsDictionary[ParameterType.ParamType.TableHeight].Value;
            var shelfHeight =
                _parameters.ParamsDictionary[ParameterType.ParamType.ShelfHeight].Value;
            var shelfMaxValue =
                Validator.CalculateShelfMaxValue(tableSize, supportSize);
            var maxShelfFloorDistance =
                Validator.CalculateShelfFloorDistanceMax(tableHeight, supportSize, shelfHeight);
            var shelfValue =
                _parameters.ParamsDictionary[shelfParamType].Value;

            if (shelfParamType == ParameterType.ParamType.ShelfFloorDistance)
            {
                _parameters.ParamsDictionary[shelfParamType].MaxValue = maxShelfFloorDistance;
            }
            else
            {
                _parameters.ParamsDictionary[shelfParamType].MaxValue = shelfMaxValue;
            }

            if (!Validator.ValidateShelfValue(
                shelfValue,
                _parameters.ParamsDictionary[shelfParamType].MaxValue,
                out var error))
            {
                _errorLabelToTextBox[textBox].Text = error;
                SetError(textBox, error);

                UpdateRangeLabel(
                    rangeLabel,
                    shelfMinValue,
                    _parameters.ParamsDictionary[shelfParamType].MaxValue);
                return;
            }

            UpdateRangeLabel(
                rangeLabel,
                shelfMinValue,
                _parameters.ParamsDictionary[shelfParamType].MaxValue);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;

            try
            {
                if (double.TryParse(
                    textBox.Text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var number))
                {
                    var parameterType = _textBoxesTypes[textBox];
                    var parameter = _parameters.ParamsDictionary[parameterType];

                    parameter.Value = number;

                    ClearError(textBox);

                    _parameters.Validate(GetLocalize(parameterType), parameter);

                    CheckDependentParams();
                }
                else
                {
                    SetError(textBox, "Вы ввели не целочисленное значение");
                }
            }
            catch (Exception ex)
            {
                SetError(textBox, ex.Message);
            }

            _exceptions =
                _errorLabelToTextBox.Values.Any(
                    control => !string.IsNullOrEmpty(control.Text));
        }

        private void TableBuildButton_Click(object sender, EventArgs e)
        {
            if (_exceptions == false)
            {
                try
                {
                    _builder.Build(_parameters);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(
                        "Не найден запускаемый файл.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "При запуске программы произошла ошибка.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
