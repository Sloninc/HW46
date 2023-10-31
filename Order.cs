using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW46
{
    public class Order
    {
        public Order() { }
        public Order((int, int, int) order)
        {
            CustomerId = order.Item1;
            ProductId = order.Item2;
            Quantity = order.Item3;
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
