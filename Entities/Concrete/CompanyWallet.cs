using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class CompanyWallet : IEntity
    {
        public int Id { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }

    }
}
