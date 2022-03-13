using System;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CandlesCompany.Utils.ValidationRules
{
    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
                {
                    return new ValidationResult(false, "Обязательное поле");
                }

                MailAddress m = new MailAddress((string)value);
                return ValidationResult.ValidResult;
            }
            catch
            {
                return new ValidationResult(false, "Неверный Email");
            }
        }
    }
}
