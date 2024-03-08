using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Amount { get; set; }
        public bool Contribute { get; set; }
    }
}
