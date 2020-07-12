using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<long>
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public long? MentorId { get; set; }

        public virtual User Mentor { get; set; }
        public virtual List<User> Disciples { get; set; }
    }
}