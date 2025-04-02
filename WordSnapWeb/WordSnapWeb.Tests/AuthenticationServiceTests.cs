using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using WordSnapWeb.Models;
using WordSnapWeb.Services;

namespace WordSnapWeb.Tests
{
    public class AuthenticationServiceTests : IDisposable
    {
        private readonly WordSnapDbContext _context;
        private readonly AuthenticationService _authService;
        private readonly Mock<IValidationService> _validationServiceMock;

        public AuthenticationServiceTests()
        {
            var options = new DbContextOptionsBuilder<WordSnapDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAuthDatabase")
                .Options;
            _context = new WordSnapDbContext(options);

            // Mock IValidationService instead of ValidationService
            _validationServiceMock = new Mock<IValidationService>();
            _validationServiceMock
                .Setup(v => v.ValidateEmail(It.IsAny<string>()))
                .Returns(new ValidationService.ValidationResult(true));
            _validationServiceMock
                .Setup(v => v.ValidateUsername(It.IsAny<string>()))
                .Returns(new ValidationService.ValidationResult(true));
            _validationServiceMock
                .Setup(v => v.ValidatePassword(It.IsAny<string>()))
                .Returns(new ValidationService.ValidationResult(true));

            _authService = new AuthenticationService(_context, _validationServiceMock.Object);

            UserSession.Instance.Logout();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task RegisterAsync_ValidData_ReturnsSuccessAndLogsInUser()
        {
            var username = "testuser";
            var email = "test@test.com";
            var password = "Password123";

            var (success, message, user) = await _authService.RegisterAsync(username, email, password);

            Assert.True(success);
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email);
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            Assert.NotNull(dbUser);
            Assert.NotNull(UserSession.Instance.CurrentUser);
            Assert.Equal(dbUser.Id, UserSession.Instance.CurrentUser.Id);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            var email = "login@test.com";
            var password = "Password123";
            var salt = "TestSalt";
            var hashedPassword = ComputeHashPassword(password, salt);
            var user = new User
            {
                Username = "loginuser",
                Email = email,
                PasswordSalt = salt,
                PasswordHash = hashedPassword,
                IsVerified = true,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var (success, message, loggedUser) = await _authService.LoginAsync(email, password);

            Assert.True(success);
            Assert.NotNull(loggedUser);
            Assert.Equal(user.Email, loggedUser.Email);
            Assert.NotNull(UserSession.Instance.CurrentUser);
            Assert.Equal(user.Id, UserSession.Instance.CurrentUser.Id);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            var email = "loginfail@test.com";
            var password = "CorrectPassword";
            var salt = "TestSaltFail";
            var hashedPassword = ComputeHashPassword(password, salt);
            var user = new User
            {
                Username = "loginfail",
                Email = email,
                PasswordSalt = salt,
                PasswordHash = hashedPassword,
                IsVerified = true,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var (success, message, loggedUser) = await _authService.LoginAsync(email, "WrongPassword");

            Assert.False(success);
            Assert.Null(loggedUser);
            Assert.Null(UserSession.Instance.CurrentUser);
        }

        private string ComputeHashPassword(string password, string salt)
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
    }
}
