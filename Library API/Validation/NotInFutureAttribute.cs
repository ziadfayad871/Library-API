using System.ComponentModel.DataAnnotations;

namespace Library_API.Validation
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        public NotInFutureAttribute() : base("The {0} field cannot be in the future.")
        {
        }
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue <= DateTime.UtcNow;
            }
            return true; // If the value is not a DateTime, consider it valid
        }
        // Optional: Override FormatErrorMessage to customize the error message
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}
