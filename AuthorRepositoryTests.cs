using AutoFixture;
using BooksApi;
using BooksApi.Models;
using BooksApi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApiTest
{
    public class AuthorRepositoryTests
    {
        private readonly Fixture _fixture;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public AuthorRepositoryTests()
        {
            _fixture = new Fixture();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData("JOHN", "john")]
        [InlineData("john", "JOHN")]
        [InlineData("JoHn", "jOhN")]
        public void Filter_ShouldBeCaseInsensitive_ForFirstName(string searchTerm, string authorName)
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.FirstName, authorName)
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter(searchTerm, null).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(authorName, result[0].FirstName);
        }

        [Theory]
        [InlineData("SMITH", "smith")]
        [InlineData("smith", "SMITH")]
        [InlineData("SmItH", "sMiTh")]
        public void Filter_ShouldBeCaseInsensitive_ForLastName(string searchTerm, string authorName)
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.LastName, authorName)
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter(null, searchTerm).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(authorName, result[0].LastName);
        }

        [Fact]
        public void Filter_ShouldReturnEmpty_WhenNoMatch()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Create<Author>();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("NonExistent", "NonExistent").ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_ShouldMatchPartialNames()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.FirstName, "Jonathan")
                .With(x => x.LastName, "Richardson")
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result1 = repository.Filter("Jon", null).ToList();
            var result2 = repository.Filter(null, "RICH").ToList();

            // Assert
            Assert.Single(result1);
            Assert.Single(result2);
            Assert.Equal("Jonathan", result1[0].FirstName);
            Assert.Equal("Richardson", result2[0].LastName);
        }
    }
} 