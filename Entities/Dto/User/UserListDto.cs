using Core.Entities;
using Entities.Dto.OperationClaim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.User
{
    public class UserListDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public List<OperationClaimListDto> Roles { get; set; }
        public bool Status { get; set; }

    }
}
