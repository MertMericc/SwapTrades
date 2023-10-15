using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.CryptoCurrency
{
    public class CryptoUpdateDto
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortName { get; set; }
        public bool Status { get; set; }
    }
}
