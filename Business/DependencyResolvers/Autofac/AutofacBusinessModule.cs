using Autofac;
using Business.Abstract;
using Business.Concrete;
using Core.DataAccess;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using Dal.Abstract;
using Dal.Concrete.Context;
using Dal.Concrete.EntityFramework;
using Microsoft.AspNetCore.Http;
using Dal.Abstract;
using Dal.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Security.Jwt;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

            builder.RegisterType<EfEntityRepositoryBase<UserOperationClaim, SwapDbContext>>().As<IEntityRepository<UserOperationClaim>>();

            builder.RegisterType<EfCryptoCurrencyDal>().As<ICryptoCurrencyDal>();
            builder.RegisterType<CryptoCurrencyManager>().As<ICryptoCurrencyService>();

            builder.RegisterType<ParityManager>().As<IParityService>();
            builder.RegisterType<EfParityDal>().As<IParityDal>();

            builder.RegisterType<AuthManager>().As<IAuthService>();

            builder.RegisterType<BuyOrderManager>().As<IBuyOrderService>();
            builder.RegisterType<EfBuyOrderDal>().As<IBuyOrderDal>();

            builder.RegisterType<EfOrderStatusDal>().As<IOrderStatusDal>();

            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            builder.RegisterType<SellOrderManager>().As<ISellOrderService>();
            builder.RegisterType<EfSellOrderDal>().As<ISellOrderDal>();

            builder.RegisterType<EfOrderStatusDal>().As<IOrderStatusDal>();
            builder.RegisterType<OrderStatusManager>().As<IOrderStatusService>();

            builder.RegisterType<EfWalletDal>().As<IWalletDal>();
            builder.RegisterType<WalletManager>().As<IWalletService>();

            builder.RegisterType<CompanyWalletManager>().As<ICompanyWalletService>();
            builder.RegisterType<EfCompanyWalletDal>().As<ICompanyWalletDal>();

            builder.RegisterType<UserOperationManager>().As<IUserOperationService>();
            builder.RegisterType<OrderManager>().As<IOrderService>();

            builder.RegisterType<MarketHistoryManagerBahadir>().As<IMarketHistoryServiceBahadir>();
            builder.RegisterType<MarketHistoryDal>().As<IMarketHistoryDal>();
        }
    }
}
