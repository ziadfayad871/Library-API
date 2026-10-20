using System.ComponentModel.DataAnnotations;

namespace rakoltodev.Validation
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        public NotInFutureAttribute() : base("The {0} field cannot be in the future.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue <= DateTime.Now;
            }
            return true;
        }
    }
}
