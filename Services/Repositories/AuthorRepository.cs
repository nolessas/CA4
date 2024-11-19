using BooksApi.Models;

namespace BooksApi.Services.Repositories
{
    public interface IAuthorRepository
    {
        IEnumerable<Author> Filter(string? firstName, string? lastName);
    }
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Author> Filter(string? firstName, string? lastName)
        {
            // Trim whitespace if the strings are not null
            firstName = firstName?.Trim();
            lastName = lastName?.Trim();

            var query = _context.Authors.AsQueryable();

            // If both names are provided, search for both matches (AND logic)
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(a => 
                    a.FirstName.ToLower().Contains(firstName.ToLower()) && 
                    a.LastName.ToLower().Contains(lastName.ToLower()));
            }
            // If only firstName is provided
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(a => a.FirstName.ToLower().Contains(firstName.ToLower()));
            }
            // If only lastName is provided
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(a => a.LastName.ToLower().Contains(lastName.ToLower()));
            }

            return query.ToList();
        }
    }
}
