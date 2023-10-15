using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dto.Parity;

namespace Business.Abstract
{
    public interface IParityService
    {
        IDataResult<Parity> Add(ParityCreateDto createDto);
        IDataResult<Parity> Update(ParityUpdateDto updateDto);
        IResult Delete(int id);
        IDataResult<List<ParityListDto>> GetList();
        IDataResult<Parity> SetStatus(int id); //Gelen Id'li parity'e ait statü değişecek.
        IDataResult<ParityListDto> GetDtoById(int id);
        IDataResult<Parity> GetById(int id);
    }
}
