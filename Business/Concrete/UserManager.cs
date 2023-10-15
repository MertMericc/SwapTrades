using Business.Abstract;
using Business.Constants;
using Business.Constants.Enums;
using Core.DataAccess;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Dal.Abstract;
using Entities.Dto.BuyOrder;
using Entities.Dto.OperationClaim;
using Entities.Dto.SellOrder;
using Entities.Dto.User;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;
        private IEntityRepository<UserOperationClaim> _userOperationClaimRepository;
        private IUserOperationService _userOperationService;
        private IBuyOrderDal _buyOrderDal;
        private ISellOrderDal _sellOrderDal;
        private IParityService _parityService;
        private IOrderStatusService _orderStatusService;
        private ITokenHelper _tokenHelper;

        public UserManager(IUserDal userDal, IEntityRepository<UserOperationClaim> userOperationClaimRepository, IUserOperationService userOperationService, IBuyOrderDal buyOrderDal, ISellOrderDal sellOrderDal, IParityService parityService, IOrderStatusService orderStatusService, ITokenHelper tokenHelper)
        {
            _userDal = userDal;
            _userOperationClaimRepository = userOperationClaimRepository;
            _userOperationService = userOperationService;
            _buyOrderDal = buyOrderDal;
            _sellOrderDal = sellOrderDal;
            _parityService = parityService;
            _orderStatusService = orderStatusService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<User> AddWithRole(User user)
        {

            try
            {
                var userExist = _userDal.Get(u => u.Username == user.Username);
                if (userExist != null)
                {
                    return new ErrorDataResult<User>(Messages.UserAlreadyExist);
                }
                _userDal.Add(user);


                _userOperationClaimRepository.Add(new UserOperationClaim
                {
                    UserId = user.Id,
                    OperationClaimId = (int)OperationClaims.Member
                });

                _userOperationService.AddCryptoCurrencyForUser(user.Id);

                return new SuccessDataResult<User>(user, Messages.UserAdded);
            }
            catch (Exception)
            {
                return new ErrorDataResult<User>(Messages.UnknownError);
            }



        }

        public IResult Delete(int id)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == id);
                if (user == null)
                {
                    return new ErrorResult(Messages.UserNotFound);
                }
                _userDal.Delete(user);
                return new SuccessResult(Messages.UserDeleted);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.UnknownError);
            }

        }



        public IDataResult<UserListDto> GetById(int id)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == id);
                if (user == null)
                {
                    return new ErrorDataResult<UserListDto>(Messages.UserNotFound);
                }
                return new SuccessDataResult<UserListDto>(new UserListDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = GetRolesDto(user),
                });
            }
            catch (Exception)
            {
                return new ErrorDataResult<UserListDto>(Messages.UnknownError);
            }

        }

        public List<OperationClaimListDto> GetRolesDto(User user)
        {
            var roles = _userDal.GetClaims(user);
            var listDto = new List<OperationClaimListDto>();
            foreach (var role in roles)
            {
                listDto.Add(new OperationClaimListDto { Name = role.Name });
            }
            return listDto;
        }

        public IDataResult<UserHistoryDto> GetUserHistoryById(int id)
        {
            try
            {
                //Gelen id'ye ait sell ve buy historylerini getiriyoruz:
                var userBuyOrderHistory = _buyOrderDal.GetList(bo => bo.UserId == id);

                if (userBuyOrderHistory == null)
                {
                    return new ErrorDataResult<UserHistoryDto>(Messages.OrderListNotFound);

                }

                var userSellOrderHistory = _sellOrderDal.GetList(bo => bo.UserId == id);


                if (userSellOrderHistory == null)
                {
                    return new ErrorDataResult<UserHistoryDto>(Messages.OrderListNotFound);

                }

                var userDetails = _userDal.Get(x => x.Id == id);

                if (userDetails == null)
                {
                    return new ErrorDataResult<UserHistoryDto>(Messages.UserNotFound);

                }


                var buyDto = new List<BuyOrderListDto>();
                foreach (var item in userBuyOrderHistory)
                {
                    var user = GetById(item.UserId).Data;
                    var parity = _parityService.GetDtoById(item.ParityId).Data;

                    buyDto.Add(new BuyOrderListDto
                    {
                        Id = item.Id,
                        UserName = $"{user.Name} {user.Surname}",
                        Price = item.Price,
                        Amount = item.Amount,
                        FeePrice = item.FeePrice,
                        TotalPrice = (item.Price * item.Amount) + item.FeePrice,
                        ParityName = $"{parity.ReceivedCurrenyName}/{parity.SoldCurrencyName}",
                        StatusName = _orderStatusService.GetById(item.StatusId).Data.StatusName,
                        CreatedDate = item.CreatedDate,
                    });
                }

                var sellDto = new List<SellOrderListDto>();
                foreach (var item in userSellOrderHistory)
                {
                    var user = GetById(item.UserId).Data;
                    var parity = _parityService.GetDtoById(item.ParityId).Data;
                    sellDto.Add(new SellOrderListDto
                    {
                        Id = item.Id,
                        UserName = $"{user.Name} {user.Surname}",
                        Price = item.Price,
                        Amount = item.Amount,
                        FeePrice = item.FeePrice,
                        TotalPrice = (item.Price * item.Amount) + item.FeePrice,
                        ParityName = $"{parity.ReceivedCurrenyName}/{parity.SoldCurrencyName}",
                        StatusName = _orderStatusService.GetById(item.StatusId).Data.StatusName,
                        CreatedDate = item.CreatedDate
                    });
                }

                var userHistory = new UserHistoryDto()
                {
                    Id = userDetails.Id,
                    Name = userDetails.Name,
                    Surname = userDetails.Surname,
                    Email = userDetails.Email,
                    UserBuyOrdersHistory = buyDto,
                    UserSellOrdersHistory = sellDto
                };

                return new SuccessDataResult<UserHistoryDto>(userHistory, Messages.Success);

            }
            catch (Exception)
            {
                return new ErrorDataResult<UserHistoryDto>(Messages.UnknownError);
            }
        }

        public IDataResult<UserListDto> GetDetails(string token)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == _tokenHelper.GetUserClaimsFromToken(token).Id);

                if (user == null)
                {
                    return new ErrorDataResult<UserListDto>(Messages.UserNotFound);
                }

                return new SuccessDataResult<UserListDto>(new UserListDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = GetRolesDto(user),
                });
            }
            catch (Exception)
            {
                return new ErrorDataResult<UserListDto>(Messages.UnknownError);
            }

        }

        public IDataResult<List<UserListDto>> GetList(Expression<Func<User, bool>> filter = null)
        {
            try
            {
                var userList = filter == null ? _userDal.GetList() : _userDal.GetList(filter);

                if (userList == null)
                {
                    return new ErrorDataResult<List<UserListDto>>(Messages.UserListNotFound);
                }

                var dtoList = new List<UserListDto>();
                foreach (var user in userList)
                {
                    dtoList.Add(new UserListDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email,
                        Username = user.Username,
                        Roles = GetRolesDto(user),
                        Status = user.Status
                    });
                }
                return new SuccessDataResult<List<UserListDto>>(dtoList);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<UserListDto>>(Messages.UnknownError);
            }
        }

        public IDataResult<User> Get(Expression<Func<User, bool>> filter)
        {
            try
            {
                var user = _userDal.Get(filter);

                if (user == null)
                {
                    return new ErrorDataResult<User>(Messages.UserNotFound);
                }

                return new SuccessDataResult<User>(new User
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt,
                    Status = user.Status
                }, Messages.Success);
            }
            catch (Exception)
            {

                return new ErrorDataResult<User>(Messages.UnknownError);
            }
        }

        public IDataResult<UserListDto> GetDto(Expression<Func<User, bool>> filter)
        {
            try
            {
                var user = _userDal.Get(filter);

                if (user == null)
                {
                    return new ErrorDataResult<UserListDto>(Messages.UserNotFound);
                }

                return new SuccessDataResult<UserListDto>(new UserListDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = GetRolesDto(user),
                    Status = user.Status
                }, Messages.Success);
            }
            catch (Exception)
            {

                return new ErrorDataResult<UserListDto>(Messages.UnknownError);
            }
        }

        public IDataResult<User> Update(UserUpdateDto dto)
        {
            try
            {
                var userExist = _userDal.Get(u => u.Id == dto.Id);
                if (userExist == null)
                {
                    return new ErrorDataResult<User>(Messages.UserNotFound);
                }

                var user = new User()
                {
                    Id = userExist.Id,
                    Name = userExist.Name,
                    Surname = userExist.Surname,
                    Email = userExist.Email,
                    Username = userExist.Username,
                    Status = userExist.Status
                };

                _userDal.Update(user);
                return new SuccessDataResult<User>(user, Messages.UserUpdated);
            }
            catch (Exception)
            {
                return new ErrorDataResult<User>(Messages.UnknownError);
            }

        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            try
            {
                var roles = _userDal.GetClaims(user);
                var list = new List<OperationClaim>();
                foreach (var role in roles)
                {
                    list.Add(new OperationClaim { Id = role.Id, Name = role.Name });
                }
                return new SuccessDataResult<List<OperationClaim>>(list, Messages.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
