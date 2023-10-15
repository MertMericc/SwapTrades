using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dto.CryptoCurrency;
using System.Linq.Expressions;

namespace Business.Abstract
{
    public interface ICryptoCurrencyService
    {
        IDataResult<CryptoCurrency> GetById(int cryptoCurrencyId);
        IDataResult<List<CryptoListDto>> GetList();
        IDataResult<CryptoCurrency> Add(CryptoAddDto cryptoDto);
        IResult Delete(int cryptoCurrencyId);
        IDataResult<CryptoCurrency> Update(CryptoUpdateDto cryptoCurrencyUpdateDto);
        IDataResult<CryptoCurrency> SetStatus(int id);
        IDataResult<CryptoCurrencyHistoryDto> GetHistoryOfCrypto(string cryptoDtoName);
        IResult AddWalletToUser(int cryptoCurrencyId);
        IResult AddWalletToCompany(int cryptoCurrencyId);
        IDataResult<List<CryptoListDto>> GetListByFilter(Expression<Func<CryptoCurrency, bool>> filter = null);
        IDataResult<List<CryptoListDto>> GetActiveList();
    }
}
