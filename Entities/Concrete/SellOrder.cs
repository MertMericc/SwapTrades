using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class SellOrder : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BuyerId { get; set; }
        public decimal Price { get; set; }//satışa konulan fiyat
        public decimal Amount { get; set; }
        public decimal FeePrice { get; set; }
        public int ParityId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; } //sell oluşma tarihi
        public DateTime? SoldDate { get; set; } //sell  tarihi


    }
}
