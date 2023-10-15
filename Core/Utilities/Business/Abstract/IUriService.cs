using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Business.Abstract
{
    public interface IUriService
    {
        Uri GetPageUri(int pNumber, int pSize, string route);
    }

}
