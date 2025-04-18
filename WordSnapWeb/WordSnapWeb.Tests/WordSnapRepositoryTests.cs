using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WordSnapWeb.Models;

namespace WordSnapWeb.Tests
{
    public class WordSnapRepositoryTests : IDisposable
    {
        private readonly WordSnapDbContext _context;
        private readonly WordSnapRepository _repo;
        private readonly ApplicationUser _testUser;

        public WordSnapRepositoryTests()
        {
            var opts = new DbContextOptionsBuilder<WordSnapDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new WordSnapDbContext(opts);
            _repo = new WordSnapRepository(_context);

            _testUser = new ApplicationUser
            {
                UserName = "alice",
                Email = "alice@example.com",
                EmailConfirmed = true
            };
            _context.Users.Add(_testUser);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddCardsetAsync_ShouldAddCardset_WithValidUserRef()
        {
            var cardset = new Cardset
            {
                Name = "Test Cardset",
                IsPublic = true,
                UserRef = _testUser.Id
            };

            var changed = await _repo.AddCardsetAsync(cardset);
            var fromDb = await _context.Cardsets.FirstOrDefaultAsync(cs => cs.Name == "Test Cardset");

            Assert.Equal(1, changed);
            Assert.NotNull(fromDb);
            Assert.Equal(_testUser.Id, fromDb.UserRef);
        }

        [Fact]
        public async Task DeleteCardsetAsync_ShouldDeleteCardset()
        {
            var cardset = new Cardset
            {
                Name = "ToDelete",
                IsPublic = true,
                UserRef = _testUser.Id
            };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            await _repo.DeleteCardsetAsync(cardset.Id);
            var result = await _context.Cardsets.FindAsync(cardset.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCardsetAsync_ShouldUpdateName()
        {
            var cardset = new Cardset
            {
                Name = "OldName",
                IsPublic = false,
                UserRef = _testUser.Id
            };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            cardset.Name = "NewName";
            await _repo.UpdateCardsetAsync(cardset);
            var updated = await _context.Cardsets.FindAsync(cardset.Id);

            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task SwitchCardsetPrivacyAsync_ShouldToggleIsPublic()
        {
            var cardset = new Cardset
            {
                Name = "PrivacyTest",
                IsPublic = false,
                UserRef = _testUser.Id
            };
            _context.Cardsets.Add(cardset);
            await _context.SaveChangesAsync();

            await _repo.SwitchCardsetPrivacyAsync(cardset.Id);
            var updated = await _context.Cardsets.FindAsync(cardset.Id);

            Assert.True(updated.IsPublic);
        }

        [Fact]
        public async Task GetCardsetsFromSearchAsync_ShouldReturnPublicMatches()
        {
            var cs1 = new Cardset { Name = "English", IsPublic = true, UserRef = _testUser.Id };
            var cs2 = new Cardset { Name = "Spanish", IsPublic = true, UserRef = _testUser.Id };
            var cs3 = new Cardset { Name = "Secret", IsPublic = false, UserRef = _testUser.Id };
            _context.Cardsets.AddRange(cs1, cs2, cs3);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetCardsetsFromSearchAsync("eng")).ToList();

            Assert.Single(result);
            Assert.Equal("English", result[0].Name);
        }

        [Fact]
        public async Task GetCardsetByIdAsync_ShouldIncludeCards()
        {
            var cs = new Cardset
            {
                Name = "WithCards",
                IsPublic = true,
                UserRef = _testUser.Id,
                Cards = new[]
                {
                    new Card { WordEn = "A", WordUa = "А" },
                    new Card { WordEn = "B", WordUa = "Б" }
                }.ToList()
            };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var fetched = await _repo.GetCardsetByIdAsync(cs.Id);
            Assert.Equal(2, fetched.Cards.Count);
        }

        [Fact]
        public async Task AddCardAsync_ShouldAddCard()
        {
            var card = new Card { WordEn = "Hello", WordUa = "Привіт", Comment = "Greet" };

            var changed = await _repo.AddCardAsync(card);
            var saved = await _context.Cards.FirstOrDefaultAsync(c => c.WordEn == "Hello");

            Assert.Equal(1, changed);
            Assert.NotNull(saved);
            Assert.Equal("Привіт", saved.WordUa);
        }

        [Fact]
        public async Task UpdateCardAsync_ShouldModifyCard()
        {
            var card = new Card { WordEn = "Old", WordUa = "Старе", Comment = "com" };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            card.WordEn = "New";
            await _repo.UpdateCardAsync(card);
            var updated = await _context.Cards.FindAsync(card.Id);

            Assert.Equal("New", updated.WordEn);
        }

        [Fact]
        public async Task DeleteCardAsync_ShouldRemoveCard()
        {
            var card = new Card { WordEn = "X", WordUa = "Y", Comment = "c" };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            await _repo.DeleteCardAsync(card.Id);
            var found = await _context.Cards.FindAsync(card.Id);

            Assert.Null(found);
        }

        [Fact]
        public async Task GetCardAsync_ShouldIncludeNavigation()
        {
            var cs = new Cardset { Name = "C", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var card = new Card { WordEn = "W", WordUa = "U", CardsetRef = cs.Id };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            var fetched = await _repo.GetCardAsync(card.Id);
            Assert.NotNull(fetched);
            Assert.Equal(cs.Id, fetched.CardsetRefNavigation.Id);
        }

        [Fact]
        public async Task AddCardsetToSavedLibraryAsync_ShouldAddUserscardset()
        {
            var cs = new Cardset { Name = "LibTest", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var uc = new Userscardset { UserRef = _testUser.Id, CardsetRef = cs.Id };
            var changed = await _repo.AddCardsetToSavedLibraryAsync(uc);
            var saved = await _context.Userscardsets.FirstOrDefaultAsync(u => u.UserRef == _testUser.Id);

            Assert.Equal(1, changed);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task GetUsersCardsetsLibraryAsync_ShouldReturnAll()
        {
            var cs1 = new Cardset { Name = "A", IsPublic = true, UserRef = _testUser.Id };
            var cs2 = new Cardset { Name = "B", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.AddRange(cs1, cs2);
            await _context.SaveChangesAsync();

            _context.Userscardsets.AddRange(
                new Userscardset { UserRef = _testUser.Id, CardsetRef = cs1.Id },
                new Userscardset { UserRef = _testUser.Id, CardsetRef = cs2.Id }
            );
            await _context.SaveChangesAsync();

            var list = await _repo.GetUsersCardsetsLibraryAsync(_testUser.Id);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetUserscardsetAsync_ShouldReturnOne()
        {
            var cs = new Cardset { Name = "L", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var uc = new Userscardset { UserRef = _testUser.Id, CardsetRef = cs.Id };
            _context.Userscardsets.Add(uc);
            await _context.SaveChangesAsync();

            var fetched = await _repo.GetUserscardsetAsync(_testUser.Id, cs.Id);
            Assert.Equal(_testUser.Id, fetched.UserRef);
            Assert.Equal(cs.Id, fetched.CardsetRef);
        }

        [Fact]
        public async Task DeleteUsersCardset_ShouldReturnTrue()
        {
            var cs = new Cardset { Name = "D", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var uc = new Userscardset { UserRef = _testUser.Id, CardsetRef = cs.Id };
            _context.Userscardsets.Add(uc);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteUsersCardset(uc.Id);
            Assert.True(result);
            var deleted = await _context.Userscardsets.FindAsync(uc.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task AddTestProgressAsync_ShouldAddProgress()
        {
            var cs = new Cardset { Name = "P", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var prog = new Progress
            {
                UserRef = _testUser.Id,
                CardsetRef = cs.Id,
                SuccessRate = 75.0
            };
            var changed = await _repo.AddTestProgressAsync(prog);
            var saved = await _context.Progresses.FirstOrDefaultAsync(p => p.UserRef == _testUser.Id);

            Assert.Equal(1, changed);
            Assert.Equal(75.0, saved.SuccessRate);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnProgress()
        {
            var cs = new Cardset { Name = "GP", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var prog = new Progress
            {
                UserRef = _testUser.Id,
                CardsetRef = cs.Id,
                SuccessRate = 50.0
            };
            _context.Progresses.Add(prog);
            await _context.SaveChangesAsync();

            var fetched = await _repo.GetProgress(_testUser.Id, cs.Id);
            Assert.Equal(50.0, fetched.SuccessRate);
        }

        [Fact]
        public async Task UpdateProgress_ShouldModifyProgress()
        {
            var cs = new Cardset { Name = "UP", IsPublic = true, UserRef = _testUser.Id };
            _context.Cardsets.Add(cs);
            await _context.SaveChangesAsync();

            var prog = new Progress
            {
                UserRef = _testUser.Id,
                CardsetRef = cs.Id,
                SuccessRate = 40.0
            };
            _context.Progresses.Add(prog);
            await _context.SaveChangesAsync();

            prog.SuccessRate = 95.0;
            var changed = await _repo.UpdateProgress(prog);
            var updated = await _context.Progresses.FirstOrDefaultAsync(p => p.UserRef == _testUser.Id);
            Assert.Equal(1, changed);
            Assert.Equal(95.0, updated.SuccessRate);
        }
    }

    public class AdminControllerTests
    {
        [Fact]
        public async Task BanUser_RemovesUser_AndRedirectsToUsers()
        {
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            var userMgrMock = new Mock<UserManager<ApplicationUser>>(
                storeMock.Object, null, null, null, null, null, null, null, null);

            var toBan = new ApplicationUser { Id = "u123", UserName = "victim", Email = "v@example.com" };
            userMgrMock.Setup(m => m.FindByIdAsync("u123"))
                       .ReturnsAsync(toBan);
            userMgrMock.Setup(m => m.DeleteAsync(toBan))
                       .ReturnsAsync(IdentityResult.Success)
                       .Verifiable();

            var controller = new AdminController(userMgrMock.Object);

            var result = await controller.BanUser("u123");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AdminController.Users), redirect.ActionName);
            userMgrMock.Verify();
        }
    }
}
