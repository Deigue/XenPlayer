using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace XenPlayer.Utils
{
    public class BinaryTextValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Regex.IsMatch((string) value, "^[01]*$") == false)
            {
                return new ValidationResult(false, "Only 0 or 1 is allowed");
            }

            return ValidationResult.ValidResult;
        }
    }
}
