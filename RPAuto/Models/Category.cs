using System;
using System.Collections.Generic;

namespace RPAuto.Models
{
    public partial class Category
    {
        public Category()
        {
            Part = new HashSet<Part>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Part> Part { get; set; }
    }
}
