using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WordSnapWeb.Models;

namespace WordSnapWeb.Services
{
    public class AuthenticationService
    {
        private readonly WordSnapDbContext _context;
        private readonly IValidationService _validationService;

        public AuthenticationService(WordSnapDbContext context, IValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        public async Task<(bool success, string message, User? user)> LoginAsync(string email, string password)
        {
            var emailValidation = _validationService.ValidateEmail(email);
            if (!emailValidation.IsValid)
            {
                return (false, emailValidation.ErrorMessage, null);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return (false, "User with this email does not exist.", null);
            }

            var hashedPassword = HashPassword(password, user.PasswordSalt);
            if (hashedPassword != user.PasswordHash)
            {
                return (false, "Invalid password.", null);
            }

            //if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            //{
            //    return (false, "Invalid password.", null);
            //}

            UserSession.Instance.Login(user);

            return (true, "Login successful", user);
        }

        public async Task<(bool success, string message, User? user)> RegisterAsync(string username, string email, string password)
        {
            var usernameValidation = _validationService.ValidateUsername(username);
            if (!usernameValidation.IsValid)
            {
                return (false, usernameValidation.ErrorMessage, null);
            }

            var emailValidation = _validationService.ValidateEmail(email);
            if (!emailValidation.IsValid)
            {
                return (false, emailValidation.ErrorMessage, null);
            }

            var passwordValidation = _validationService.ValidatePassword(password);
            if (!passwordValidation.IsValid)
            {
                return (false, passwordValidation.ErrorMessage, null);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == username);
            if (existingUser != null)
            {
                if (existingUser.Email == email)
                {
                    return (false, "A user with this email already exists.", null);
                }
                else
                {
                    return (false, "A user with this username already exists.", null);
                }
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                IsVerified = false,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            UserSession.Instance.Login(user);

            return (true, "Registration successful", user);
        }

        public void Logout()
        {
            UserSession.Instance.Logout();
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var firstHashBytes = sha256.ComputeHash(passwordBytes);
                var firstHash = Convert.ToBase64String(firstHashBytes);

                var saltedHashBytes = Encoding.UTF8.GetBytes(firstHash + salt);
                var finalHashBytes = sha256.ComputeHash(saltedHashBytes);
                return Convert.ToBase64String(finalHashBytes);
            }
        }

        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordWithSalt = password + storedSalt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                var computedHash = Convert.ToBase64String(hashedBytes);

                return computedHash == storedHash;
            }
        }
    }
}
