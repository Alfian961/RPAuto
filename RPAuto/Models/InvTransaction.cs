using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPAuto.Models
{
    public partial class InvTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TransactionType { get; set; }
        public string PartNumber { get; set; }
        public int Qty { get; set; }
        public string DocNo { get; set; }

        public virtual Part PartNumberNavigation { get; set; }
    } 
}
