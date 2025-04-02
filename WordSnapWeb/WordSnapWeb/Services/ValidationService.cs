using System.Text.RegularExpressions;

namespace WordSnapWeb.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationResult ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new ValidationResult(false, "Ім'я користувача не може бути порожнім.");
            }

            var regex = new Regex("^[a-z0-9_]+$");

            if (!regex.IsMatch(username))
            {
                return new ValidationResult(false, "Ім'я користувача може містити лише малі літери, цифри, та нижні підкреслювання.");
            }

            return new ValidationResult(true);
        }

        public ValidationResult ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult(false, "Пошта не може бути порожньою.");
            }

            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (!regex.IsMatch(email))
            {
                return new ValidationResult(false, "Введіть справжню електронну адресу.");
            }

            return new ValidationResult(true);
        }

        public ValidationResult ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult(false, "Пароль не може бути порожнім.");
            }

            if (password.Length < 8)
            {
                return new ValidationResult(false, "Пароль повинен складатися принаймні з 8 символів.");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult(false, "Пароль повинен містити принаймні одну велику літеру.");
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult(false, "Пароль повинен містити принаймні одну малу літеру.");
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult(false, "Пароль повинен містити принаймні одну цифру.");
            }

            return new ValidationResult(true);
        }

        public class ValidationResult
        {
            public ValidationResult(bool isValid, string errorMessage = "")
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }

            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
