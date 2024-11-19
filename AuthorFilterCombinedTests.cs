using AutoFixture;
using BooksApi;
using BooksApi.Models;
using BooksApi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApiTest
{
    public class AuthorFilterCombinedTests
    {
        private readonly Fixture _fixture;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public AuthorFilterCombinedTests()
        {
            _fixture = new Fixture();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData("John", "Smith", "JOHN", "SMITH")]
        [InlineData("JOHN", "SMITH", "john", "smith")]
        [InlineData("JoHn", "SmItH", "jOhN", "sMiTh")]
        [InlineData("john", "smith", "JOHN", "SMITH")]
        [InlineData("jo", "sm", "JOHN", "SMITH")]
        [InlineData("JOHN", "SMITH", "John James", "Smith-Jones")]
        public void Filter_ShouldBeCaseInsensitive_ForBothNames(string searchFirstName, string searchLastName, string authorFirstName, string authorLastName)
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.FirstName, authorFirstName)
                .With(x => x.LastName, authorLastName)
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter(searchFirstName, searchLastName).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(authorFirstName, result[0].FirstName);
            Assert.Equal(authorLastName, result[0].LastName);
        }

        [Fact]
        public void Filter_ShouldReturnMultipleMatches_WhenPartialNamesMatch()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var authors = new List<Author>
            {
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "John")
                    .With(x => x.LastName, "Smith")
                    .Create(),
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "John")
                    .With(x => x.LastName, "Doe")
                    .Create(),
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "Jane")
                    .With(x => x.LastName, "Smith")
                    .Create()
            };
            context.Authors.AddRange(authors);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("John", null).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, author => Assert.Equal("John", author.FirstName));
        }

        [Fact]
        public void Filter_ShouldReturnEmpty_WhenOneNameMatchesButOtherDoesNot()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.FirstName, "John")
                .With(x => x.LastName, "Smith")
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("John", "NonExistent").ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_ShouldMatchPartialNamesForBoth()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var author = _fixture.Build<Author>()
                .With(x => x.FirstName, "Jonathan")
                .With(x => x.LastName, "Smithson")
                .Create();
            context.Authors.Add(author);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("Jon", "Smi").ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Jonathan", result[0].FirstName);
            Assert.Equal("Smithson", result[0].LastName);
        }

        [Fact]
        public void Filter_ShouldMatchBothNames_WhenBothProvided()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var authors = new List<Author>
            {
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "John")
                    .With(x => x.LastName, "Smith")
                    .Create(),
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "John") 
                    .With(x => x.LastName, "Doe")
                    .Create(),
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "Jane")
                    .With(x => x.LastName, "Smith")
                    .Create()
            };
            context.Authors.AddRange(authors);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("John", "Smith").ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Smith", result[0].LastName);
        }
    }
} 