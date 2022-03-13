using System.Globalization;
using System.Windows.Controls;

namespace CandlesCompany.Utils.ValidationRules
{
    public class PasswordMinLength : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                return new ValidationResult(false, "Обязательное поле");
            }

            if ((value as string).Length < 8 && 1 < (value as string).Length)
            {
                return new ValidationResult(false, "Минимум 8 символов");
            }

            return ValidationResult.ValidResult;
        }
    }
}
