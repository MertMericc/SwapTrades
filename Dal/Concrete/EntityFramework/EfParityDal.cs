using Core.DataAccess.EntityFramework;
using Dal.Abstract;
using Dal.Concrete.Context;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.EntityFramework
{
    public class EfParityDal : EfEntityRepositoryBase<Parity, SwapDbContext>, IParityDal
    {
    }
}
