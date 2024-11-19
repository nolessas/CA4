using AutoFixture;
using BooksApi;
using BooksApi.Models;
using BooksApi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApiTest
{
    public class AuthorFilterNullTests
    {
        private readonly Fixture _fixture;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public AuthorFilterNullTests()
        {
            _fixture = new Fixture();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData(null, "Smith")]
        [InlineData("John", null)]
        [InlineData(null, null)]
        public void Filter_ShouldHandleNullInputs(string firstName, string lastName)
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
            var result = repository.Filter(firstName, lastName).ToList();

            // Assert
            if (firstName == null && lastName == null)
            {
                Assert.Single(result); // Should return all when both null
            }
            else if (firstName == "John" || lastName == "Smith")
            {
                Assert.Single(result); // Should match on non-null value
            }
            else
            {
                Assert.Empty(result); // Should not match
            }
        }
    }
} 