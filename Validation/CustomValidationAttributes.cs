using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Validation
{
    /// <summary>
    /// Атрибут для валидации, что максимальное значение больше или равно минимальному
    /// </summary>
    public class GreaterThanOrEqualAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public GreaterThanOrEqualAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var currentValue = (IComparable)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (comparisonValue == null)
                return ValidationResult.Success;

            if (currentValue.CompareTo((IComparable)comparisonValue) < 0)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} должно быть больше или равно {_comparisonProperty}");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Атрибут для валидации года издания
    /// </summary>
    public class ValidYearAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is int year)
            {
                var currentYear = DateTime.Now.Year;
                if (year < 1900 || year > currentYear + 5)
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"Год издания должен быть между 1900 и {currentYear + 5}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
