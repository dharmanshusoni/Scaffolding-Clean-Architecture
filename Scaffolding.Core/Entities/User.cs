using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffolding.Core.Entities
{
    public class User
    {
        public User()
        {
            UserId = new Guid();
        }

        [Key]
        public Guid UserId { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string LastName { get; set; }

        public bool IsOnline { get; set; }
    }
}
