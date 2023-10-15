using Business.Abstract;
using Business.Constants;
using Business.Constants.Enums;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.Parity;

namespace Business.Concrete
{
    public class ParityManager : IParityService
    {
        private IParityDal _parityDal;
        private ICryptoCurrencyDal _cryptoCurrencyDal;

        public ParityManager(IParityDal parityDal, ICryptoCurrencyDal cryptoCurrencyDal)
        {
            _parityDal = parityDal;
            _cryptoCurrencyDal = cryptoCurrencyDal;
        }

        public IDataResult<Parity> Add(ParityCreateDto createDto)
        {
            try
            {
                if (createDto != null)
                {
                    var parity = new Parity()
                    {
                        FeeRate = createDto.FeeRate,
                        IsActiveParity = createDto.IsActiveParity,
                        SoldCurrencyId = createDto.SoldCurrencyId,
                        ReceivedCurrencyId = createDto.ReceivedCurrencyId,
                    };

                    _parityDal.Add(parity);
                    return new SuccessDataResult<Parity>(parity, Messages.Success);
                }
                return new ErrorDataResult<Parity>(Messages.IncompletedEntry);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Parity>(Messages.UnknownError);
            }
        }

        public IResult Delete(int id)
        {
            try
            {
                var checkParity = _parityDal.Get(x => x.Id == id);
                if (checkParity != null)
                {
                    checkParity.IsActiveParity = false;
                    _parityDal.Update(checkParity);
                    return new SuccessResult(Messages.Success);
                }
                return new ErrorResult(Messages.ParityNotFound);
            }
            catch (Exception)
            {

                return new ErrorResult(Messages.UnknownError);
            }
        }


        public IDataResult<Parity> GetById(int id)
        {
            try
            {
                var parity = _parityDal.GetById(id);
                if (parity != null)
                {
                
                    return new SuccessDataResult<Parity>(parity, Messages.Success);
                }
                return new ErrorDataResult<Parity>(Messages.ParityNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Parity>(Messages.UnknownError);
            }
        }

        public IDataResult<ParityListDto> GetDtoById(int id)
        {
            try
            {
                var parity = _parityDal.GetById(id);
                if (parity != null)
                {
                    var dto = new ParityListDto()
                    {
                        Id = parity.Id,
                        IsActiveParity = parity.IsActiveParity,
                        SoldCurrencyName = _cryptoCurrencyDal.GetById(parity.SoldCurrencyId).CurrencyShortName,
                        ReceivedCurrenyName = _cryptoCurrencyDal.GetById(parity.ReceivedCurrencyId).CurrencyShortName,
                        FeeRate = parity.FeeRate
                    };

                    return new SuccessDataResult<ParityListDto>(dto, Messages.Success);
                }
                return new ErrorDataResult<ParityListDto>(Messages.ParityNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<ParityListDto>(Messages.UnknownError);
            }
        }

        public IDataResult<List<ParityListDto>> GetList()
        {
            try
            {
                var parities = _parityDal.GetList(x => x.IsActiveParity);
                if (parities != null)
                {
                    var listDto = new List<ParityListDto>();
                    foreach (var item in parities)
                    {

                        listDto.Add(new ParityListDto()
                        {
                            Id = item.Id,
                            SoldCurrencyName = _cryptoCurrencyDal.GetById(item.SoldCurrencyId).CurrencyShortName,
                            ReceivedCurrenyName = _cryptoCurrencyDal.GetById(item.ReceivedCurrencyId).CurrencyShortName,
                            FeeRate = item.FeeRate,
                            IsActiveParity = item.IsActiveParity,
                        });
                    }
                    return new SuccessDataResult<List<ParityListDto>>(listDto, Messages.Success);
                }
                return new ErrorDataResult<List<ParityListDto>>(Messages.ParityListNotFound);
            }
            catch (Exception)
            {

                return new ErrorDataResult<List<ParityListDto>>(Messages.UnknownError);
            }
        }

        public IDataResult<Parity> SetStatus(int id)
        {
            try
            {
                var parityToSetStatus = _parityDal.Get(x => x.Id == id);
                if (parityToSetStatus != null)
                {
                    parityToSetStatus.IsActiveParity = !parityToSetStatus.IsActiveParity;

                    _parityDal.Update(parityToSetStatus);
                    return new SuccessDataResult<Parity>(parityToSetStatus, Messages.Success);
                }
                return new ErrorDataResult<Parity>(Messages.ParityNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<Parity>(Messages.UnknownError);
            }


        }

        public IDataResult<Parity> Update(ParityUpdateDto updateDto)
        {
            try
            {
                if (updateDto != null)
                {

                    var parity = new Parity()
                    {
                        Id = updateDto.Id,
                        FeeRate = (decimal)updateDto.FeeRate,
                        IsActiveParity = (bool)updateDto.IsActiveParity,
                        SoldCurrencyId = (int)updateDto.SoldCurrencyId,
                        ReceivedCurrencyId = (int)updateDto.ReceivedCurrencyId,
                    };

                    _parityDal.Update(parity);
                    return new SuccessDataResult<Parity>(parity, Messages.Success);
                }
                return new ErrorDataResult<Parity>(Messages.ParityNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Parity>(Messages.UnknownError);
            }
        }
    }
}
