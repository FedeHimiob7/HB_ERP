using System;

namespace Identity.Infrastructure.Persistence.Entities
{
    public class UserPslEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }

        public Guid PslId { get; set; }
    }
}
