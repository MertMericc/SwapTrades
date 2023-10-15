using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.Context
{
    public class SwapDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"local");
        }


        public DbSet<BuyOrder> BuyOrders { get; set; }
        public DbSet<SellOrder> SellOrders { get; set; }
        public DbSet<Parity> Parities{ get; set; }
        public DbSet<CryptoCurrency> CryptoCurrencies{ get; set; }
        public DbSet<Status> Statuses{ get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<OperationClaim> OperationClaims{ get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims{ get; set; }
        public DbSet<Wallet> Wallets{ get; set; }
        public DbSet<CompanyWallet> CompanyWallets{ get; set; }
    }
}
