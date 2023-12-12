namespace TablePlugin.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Описывает параметры.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Валидатор.
        /// </summary>
        private readonly Validator _validator = new Validator();

        /// <summary>
        /// Словарь с параметрами.
        /// </summary>
        private Dictionary<ParameterType.ParamType, Parameter> _paramsDictionary
            = new Dictionary<ParameterType.ParamType, Parameter>();

        /// <summary>
        /// Возвращает или задает словарь с параметрами.
        /// </summary>
        public Dictionary<ParameterType.ParamType, Parameter> ParamsDictionary
        {
            get => _paramsDictionary;
            set => _paramsDictionary = value ??
                new Dictionary<ParameterType.ParamType, Parameter>();
        }

        /// <summary>
        /// Добавляет параметр в словарь.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="parameter">Параметр.</param>
        public void AddParameter(ParameterType.ParamType paramType, Parameter parameter)
        {
            if (_validator.Validate(parameter, out _))
            {
                _paramsDictionary[paramType] = parameter;
            }
        }

        /// <summary>
        /// Проверяет параметр, передавая его в <see cref="Validator"/> по имени
        /// для вывода ошибки с конкретным параметром.
        /// </summary>
        /// <param name="paramName">Имя параметра.</param>
        /// <param name="parameter">Параметр.</param>
        public void Validate(string paramName, Parameter parameter)
        {
            if (!_validator.Validate(parameter, out var error))
            {
                throw new Exception($"{paramName} {error}");
            }
        }
    }
}
