namespace MovieLibrary.Validation
{
    public static class ValidationUtilities
    {
        // Settings


        // Messages
        public static string NonEmptyMessage = "The field {PropertyName} must not be empty";
        public static string FieldSizeExceededMessage = "The maximum number of characters of field {PropertyName} is {MaxLength}";
        public static string NonTitledCaseMessage = "The first character of the field {PropertyName} must be uppercase";
        public static string FutureDateNotAllowedMessage = "The {PropertyName} cannot be in the future";
        public static string EarliestDateMessage(DateTime value) => "The field {PropertyName} must be greater than or equal to " + value.ToString("yyyy-MM-dd");

        // Methods
        public static bool isTitledCased(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
