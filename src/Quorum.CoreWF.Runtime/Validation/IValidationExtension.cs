namespace System.Activities.Validation
{
    public interface IValidationExtension
    {
        IEnumerable<ValidationError> PostValidate(Activity activity, ValidationSettings validationSettings);
    }
}
