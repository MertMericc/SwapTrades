using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.Claim
{
    public class ClaimListDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
