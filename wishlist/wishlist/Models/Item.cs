using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class Item
    {
        public int ItemId { get; set; }
        public int ListId { get; set; }
        public string ItemName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? Bought { get; set; }

        public virtual List List { get; set; }
    }
}
