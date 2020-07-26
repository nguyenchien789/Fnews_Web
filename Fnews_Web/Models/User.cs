using System;
using System.Collections.Generic;

namespace Fnews_Web.Models
{
    public partial class User
    {
        public User()
        {
            Bookmark = new HashSet<Bookmark>();
            Comment = new HashSet<UserComment>();
            Subscribe = new HashSet<Subscribe>();
            UserTag = new HashSet<UserTag>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public bool? IsAdmin { get; set; }
        public int? GroupId { get; set; }
        public bool? IsActive { get; set; }
        public string Password { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<Bookmark> Bookmark { get; set; }
        public virtual ICollection<UserComment> Comment { get; set; }
        public virtual ICollection<Subscribe> Subscribe { get; set; }
        public virtual ICollection<UserTag> UserTag { get; set; }
    }
}
