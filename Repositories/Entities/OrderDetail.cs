using System;
using System.Collections.Generic;

namespace Repositories.Entities
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; } = 0;

        public virtual Order? Order { get; set; } 
        public virtual Product? Product { get; set; }
    }
}
