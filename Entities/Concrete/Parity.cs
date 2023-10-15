using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Parity : IEntity
    {
        public int Id { get; set; }
        public int ReceivedCurrencyId { get; set; }
        public int SoldCurrencyId { get; set; }
        public bool IsActiveParity { get; set; }
        public decimal FeeRate { get; set; }

    }
}
