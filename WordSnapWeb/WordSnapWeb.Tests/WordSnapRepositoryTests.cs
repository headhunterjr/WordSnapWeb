using Microsoft.EntityFrameworkCore;
using WordSnapWeb.Models;

namespace WordSnapWeb.Tests
{
    public class WordSnapRepositoryTests : IDisposable
    {
        private readonly WordSnapDbContext _context;
        private readonly WordSnapRepository _repository;

        public WordSnapRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<WordSnapDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new WordSnapDbContext(options);
            _repository = new WordSnapRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddCardsetAsync_ShouldAddCardset()
        {
            // Arrange
            var cardset = new Cardset { Name = "Test Cardset", IsPublic = true };

            // Act
            await _repository.AddCardsetAsync(cardset);
            var result = await _context.Cardsets.FirstOrDefaultAsync(cs => cs.Name == "Test Cardset");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Cardset", result.Name);
            Assert.True(result.IsPublic);
        }

        [Fact]
        public async Task DeleteCardsetAsync_ShouldDeleteCardset()
        {
            // Arrange
            var cardset = new Cardset { Name = "Test Cardset", IsPublic = true };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteCardsetAsync(cardset.Id);
            var result = await _context.Cardsets.FindAsync(cardset.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCardsetAsync_ShouldUpdateCardset()
        {
            // Arrange
            var cardset = new Cardset { Name = "Old Name", IsPublic = true };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            cardset.Name = "New Name";

            // Act
            await _repository.UpdateCardsetAsync(cardset);
            var updatedCardset = await _context.Cardsets.FindAsync(cardset.Id);

            // Assert
            Assert.NotNull(updatedCardset);
            Assert.Equal("New Name", updatedCardset.Name);
        }

        [Fact]
        public async Task SwitchCardsetPrivacyAsync_ShouldToggleIsPublic()
        {
            // Arrange
            var cardset = new Cardset { Name = "Test Cardset", IsPublic = true };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            // Act
            await _repository.SwitchCardsetPrivacyAsync(cardset.Id);
            var updatedCardset = await _context.Cardsets.FindAsync(cardset.Id);

            // Assert
            Assert.NotNull(updatedCardset);
            Assert.False(updatedCardset.IsPublic);
        }

        [Fact]
        public async Task GetCardsetsFromSearchAsync_ShouldReturnMatchingCardsets()
        {
            // Arrange
            var cardset1 = new Cardset { Name = "English Words", IsPublic = true };
            var cardset2 = new Cardset { Name = "Spanish Words", IsPublic = true };
            _context.Cardsets.AddRange(cardset1, cardset2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCardsetsFromSearchAsync("English");

            // Assert
            Assert.Single(result);
            Assert.Equal("English Words", result.First().Name);
        }

        [Fact]
        public async Task GetCardsetByIdAsync_ShouldReturnCardset()
        {
            // Arrange
            var cardset = new Cardset { Name = "Test Cardset", IsPublic = true };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCardsetByIdAsync(cardset.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Cardset", result.Name);
        }

        [Fact]
        public async Task AddCardAsync_ShouldAddCard()
        {
            // Arrange
            var card = new Card { WordUa = "Привіт", WordEn = "Hello", Comment = "Greeting" };

            // Act
            await _repository.AddCardAsync(card);
            var result = await _context.Cards.FirstOrDefaultAsync(c => c.WordUa == "Привіт");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Hello", result.WordEn);
        }

        [Fact]
        public async Task UpdateCardAsync_ShouldUpdateCard()
        {
            // Arrange
            var card = new Card { WordUa = "Старе", WordEn = "Old", Comment = "Example" };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            card.WordEn = "Updated";

            // Act
            await _repository.UpdateCardAsync(card);
            var updatedCard = await _context.Cards.FindAsync(card.Id);

            // Assert
            Assert.NotNull(updatedCard);
            Assert.Equal("Updated", updatedCard.WordEn);
        }

        [Fact]
        public async Task DeleteCardAsync_ShouldDeleteCard()
        {
            // Arrange
            var card = new Card { WordUa = "Кіт", WordEn = "Cat", Comment = "Animal" };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteCardAsync(card.Id);
            var result = await _context.Cards.FindAsync(card.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCardAsync_ShouldReturnCard()
        {
            // Arrange
            var card = new Card { WordUa = "Сонце", WordEn = "Sun", Comment = "Sky" };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCardAsync(card.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sun", result.WordEn);
        }

        [Fact]
        public async Task AddCardsetToSavedLibraryAsync_ShouldAddUsersCardset()
        {
            // Arrange
            var userscardset = new Userscardset { UserRef = 1, CardsetRef = 1 };

            // Act
            var result = await _repository.AddCardsetToSavedLibraryAsync(userscardset);
            var savedEntry = await _context.Userscardsets.FirstOrDefaultAsync(uc => uc.UserRef == 1 && uc.CardsetRef == 1);

            // Assert
            Assert.Equal(1, result);
            Assert.NotNull(savedEntry);
        }

        [Fact]
        public async Task GetUsersCardsetsLibraryAsync_ShouldReturnUserCardsets()
        {
            // Arrange
            var cardset1 = new Cardset { Name = "Cardset 1", IsPublic = true };
            var cardset2 = new Cardset { Name = "Cardset 2", IsPublic = true };
            _context.Cardsets.AddRange(cardset1, cardset2);
            await _context.SaveChangesAsync();

            var userscardset1 = new Userscardset { UserRef = 1, CardsetRef = cardset1.Id };
            var userscardset2 = new Userscardset { UserRef = 1, CardsetRef = cardset2.Id };
            _context.Userscardsets.AddRange(userscardset1, userscardset2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUsersCardsetsLibraryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserscardsetAsync_ShouldReturnUsersCardset()
        {
            // Arrange
            var userscardset = new Userscardset { UserRef = 1, CardsetRef = 1 };
            _context.Userscardsets.Add(userscardset);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserscardsetAsync(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserRef);
            Assert.Equal(1, result.CardsetRef);
        }

        [Fact]
        public async Task DeleteUsersCardset_ShouldRemoveUsersCardset()
        {
            // Arrange
            var userscardset = new Userscardset { UserRef = 1, CardsetRef = 1 };
            _context.Userscardsets.Add(userscardset);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteUsersCardset(userscardset.Id);
            var deletedEntry = await _context.Userscardsets.FindAsync(userscardset.Id);

            // Assert
            Assert.True(result);
            Assert.Null(deletedEntry);
        }

        [Fact]
        public async Task AddTestProgressAsync_ShouldAddProgress()
        {
            // Arrange
            var progress = new Progress { UserRef = 1, CardsetRef = 1, SuccessRate = 90 };

            // Act
            var result = await _repository.AddTestProgressAsync(progress);
            var savedProgress = await _context.Progresses.FirstOrDefaultAsync(p => p.UserRef == 1 && p.CardsetRef == 1);

            // Assert
            Assert.Equal(1, result);
            Assert.NotNull(savedProgress);
            Assert.Equal(90, savedProgress.SuccessRate);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnProgress()
        {
            // Arrange
            var progress = new Progress { UserRef = 1, CardsetRef = 1, SuccessRate = 80 };
            _context.Progresses.Add(progress);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProgress(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(80, result.SuccessRate);
        }

        [Fact]
        public async Task UpdateProgress_ShouldModifyProgress()
        {
            // Arrange
            var progress = new Progress { UserRef = 1, CardsetRef = 1, SuccessRate = 60 };
            _context.Progresses.Add(progress);
            await _context.SaveChangesAsync();

            progress.SuccessRate = 95;

            // Act
            var result = await _repository.UpdateProgress(progress);
            var updatedProgress = await _context.Progresses.FirstOrDefaultAsync(p => p.UserRef == 1 && p.CardsetRef == 1);

            // Assert
            Assert.Equal(1, result);
            Assert.NotNull(updatedProgress);
            Assert.Equal(95, updatedProgress.SuccessRate);
        }

    }
}