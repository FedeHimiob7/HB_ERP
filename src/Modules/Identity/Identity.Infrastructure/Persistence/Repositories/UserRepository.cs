using Identity.Domain;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _db;

        public UserRepository(IdentityDbContext dbContext)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<User?> GetByIdAsync(UserId id)
        {
            var model = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            return model is null ? null : UserMapper.ToDomain(model);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var model = await _db.Users
                .FirstOrDefaultAsync(x => x.NormalizedEmail == email.ToUpper());

            return model is null ? null : UserMapper.ToDomain(model);
        }

        public async Task AddAsync(User user)
        {
            var model = UserMapper.ToModel(user);
            await _db.Users.AddAsync(model);
        }

        public async Task<List<User>> GetAllAsync()
        {
            var models = await _db.Users.ToListAsync();
            return models.Select(UserMapper.ToDomain).ToList();
        }

        public async Task UpdateAsync(User user)
        {
            var existing = await _db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == user.Id.Value);

            if (existing is null)
                throw new InvalidOperationException("User not found in database");

            UserMapper.MapToExistingEntity(user, existing);
        }
    }
}
