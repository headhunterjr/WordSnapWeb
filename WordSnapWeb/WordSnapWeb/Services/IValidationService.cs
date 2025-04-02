using static WordSnapWeb.Services.ValidationService;

namespace WordSnapWeb.Services
{
    public interface IValidationService
    {
        ValidationResult ValidateUsername(string username);
        ValidationResult ValidateEmail(string email);
        ValidationResult ValidatePassword(string password);
    }
}
