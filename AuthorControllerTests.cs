using BooksApi.Controllers;
using BooksApi.Models;
using BooksApi.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AuthorControllerTests
{
    private readonly Mock<IAuthorRepository> _mockRepository;
    private readonly AuthorController _controller;

    public AuthorControllerTests()
    {
        _mockRepository = new Mock<IAuthorRepository>();
        _controller = new AuthorController(_mockRepository.Object);
    }

    [Fact]
    public void GetAuthors_WithValidName_Returns200OK()
    {
        // Arrange
        var authors = new List<Author> 
        { 
            new Author { FirstName = "John", LastName = "Smith" } 
        };
        _mockRepository.Setup(r => r.Filter(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(authors);

        // Act
        var result = _controller.GetAuthors("John") as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(authors, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void GetAuthors_WithInvalidName_Returns400BadRequest(string name)
    {
        // Act
        var result = _controller.GetAuthors(name) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void GetAuthors_WhenNoAuthorsFound_Returns200OKWithEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.Filter(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<Author>());

        // Act
        var result = _controller.GetAuthors("NonExistent") as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Empty((IEnumerable<Author>)result.Value);
    }

    [Fact]
    public void GetAuthors_WhenRepositoryThrows_Returns500InternalServerError()
    {
        // Arrange
        _mockRepository.Setup(r => r.Filter(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = _controller.GetAuthors("John") as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }
} 