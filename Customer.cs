using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW46
{
    internal class Customer
    {
        public Customer() { }
        public Customer((string, string, int) valueTuple)
        {
            FirstName = valueTuple.Item1;
            LastName = valueTuple.Item2;
            Age = valueTuple.Item3;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
