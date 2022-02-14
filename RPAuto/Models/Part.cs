using System;
using System.Collections.Generic;

namespace RPAuto.Models
{
    public partial class Part
    {
        public Part()
        {
            InvTransaction = new HashSet<InvTransaction>();
        }

        public string PartNumber { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public int CategoryId { get; set; }
        public int Qty { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<InvTransaction> InvTransaction { get; set; }
    }
}
