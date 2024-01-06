using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffolding.Core.Entities
{
    public class Message
    {
        [Key]
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime MessageDate { get; set; }
        public string Content { get; set; }

        public bool IsNew { get; set; }


        public bool IsSenderDeleted { get; set; }


        public bool IsReceiverDeleted { get; set; }

    }
}
