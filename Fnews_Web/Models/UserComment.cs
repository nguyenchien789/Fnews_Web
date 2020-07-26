using System;
using System.Collections.Generic;

namespace Fnews_Web.Models
{
    public partial class UserComment
    {
        public int Id { get; set; }
        public int? NewsId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public int? MasterCommentId { get; set; }

        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}
