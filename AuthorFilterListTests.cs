using AutoFixture;
using BooksApi;
using BooksApi.Models;
using BooksApi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApiTest
{
    public class AuthorFilterListTests
    {
        private readonly Fixture _fixture;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public AuthorFilterListTests()
        {
            _fixture = new Fixture();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void Filter_ShouldReturnEmptyList_WhenNoMatches()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var authors = new List<Author>
            {
                _fixture.Build<Author>()
                    .With(x => x.FirstName, "John")
                    .With(x => x.LastName, "Smith")
                    .Create()
            };
            context.Authors.AddRange(authors);
            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // Act
            var result = repository.Filter("Jane", "Doe").ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_ShouldReturnAllMatches_WhenMultipleExist()
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
    }
} 