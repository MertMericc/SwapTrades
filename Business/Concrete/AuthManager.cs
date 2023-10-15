using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;
        public AuthManager(ITokenHelper tokenHelper, IUserService userService)
        {
            _tokenHelper = tokenHelper;
            _userService = userService;
        }

        public IDataResult<UserListDto> GetByMail(string mail)
        {
            try
            {
                var user = _userService.GetDto(u => u.Email == mail).Data;
                if (user != null)
                {
                    return new SuccessDataResult<UserListDto>(user, Messages.Success);
                }
                return new ErrorDataResult<UserListDto>(Messages.UserNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<UserListDto>(Messages.UnknownError);
            }
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            try
            {
                var claims = _userService.GetClaims(user).Data;

                if (claims != null)
                {
                    var accessToken = _tokenHelper.CreateToken(user, claims);
                    return new SuccessDataResult<AccessToken>(accessToken, Messages.Success);
                }
                return new ErrorDataResult<AccessToken>(Messages.GetClaimError);

            }
            catch (Exception)
            {
                return new ErrorDataResult<AccessToken>(Messages.UnknownError);
            }



        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                var userToCheck = _userService.Get(x => x.Email == userForLoginDto.Email).Data;
                if (userToCheck == null)
                {
                    return new ErrorDataResult<User>(Messages.UserNotFound);
                }

                if (userToCheck.Status == false)
                {
                    return new ErrorDataResult<User>(Messages.UserIsNotActive);

                }

                if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
                {
                    return new ErrorDataResult<User>(Messages.PasswordsNotMatched);
                }

                return new SuccessDataResult<User>(userToCheck, Messages.Success);
            }
            catch (Exception)
            {
                return new ErrorDataResult<User>(Messages.UnknownError);
            }

        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            try
            {
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                var user = new User
                {
                    Email = userForRegisterDto.Email,
                    Name = userForRegisterDto.Name,
                    Surname = userForRegisterDto.Surname,
                    Username = userForRegisterDto.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                };

                var result = _userService.AddWithRole(user);

                if (!result.Success)
                {
                    return new ErrorDataResult<User>(result.Message);
                }

                return new SuccessDataResult<User>(user, Messages.Success);
            }
            catch (Exception)
            {
                return new ErrorDataResult<User>(Messages.UnknownError);

            }

        }

    }
}
