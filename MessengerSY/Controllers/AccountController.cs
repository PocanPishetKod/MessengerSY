using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MessengerSY.Models.Account;
using MessengerSY.Services.JwtService;
using MessengerSY.Services.RefreshTokenService;
using MessengerSY.Services.SmsService;
using MessengerSY.Services.UserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MessengerSY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMemoryCache _memoryCache;

        public AccountController(IUserProfileService userProfileService, ISmsSenderService smsSenderService,
            IJwtService jwtService, IRefreshTokenService refreshTokenService, IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _smsSenderService = smsSenderService;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Отправляет код на указанный номер
        /// </summary>
        /// <param name="model">Номер телефона в формате +7**********</param>
        /// <response code="200">Запрос обработан успешно, код выслан</response>
        /// <response code="500">Невозможно отправить код по внутренним причинам</response>
        /// <response code="400">Входные данные не прошли валидацию</response>
        /// <response code="409">Пользователь с таким номеров уже зарегистрирован</response>
        [HttpPost("registration/getcode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Registration([FromBody]PhoneNumberModel model)
        {
            if (!await _userProfileService.IsUserProfileExists(model.PhoneNumber))
            {
                var code = CodeGenerator.GenerateCode();
                if (await _smsSenderService.SendCode(model.PhoneNumber, code))
                {
                    RememberSmsCode(model.PhoneNumber, code);

                    return Ok();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Conflict();
        }

        [HttpPost("registration/verifyphone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Registration([FromBody]VerifyCodeModel model)
        {
            if (!await _userProfileService.IsUserProfileExists(model.PhoneNumber))
            {
                var result = RecallSmsCode(model.PhoneNumber, out var code);
                if (result)
                {
                    if (string.Equals(code, model.Code))
                    {
                        ForgetSmsCode(model.PhoneNumber);

                        var refreshToken = _refreshTokenService.GenerateRefreshToken();

                        (var userProfile, var refreshTokenModel) =
                            await _userProfileService.CreateUserProfile(model.PhoneNumber, refreshToken);

                        var jwt = _jwtService.GenerateToken(userProfile.PhoneNumber, userProfile.Id, refreshTokenModel.Id);

                        return Ok(new TokensModel()
                        {
                            Jwt = jwt,
                            RefreshToken = new RefreshTokenModel()
                            {
                                RefreshToken = refreshTokenModel.Token,
                                RefreshTokenId = refreshTokenModel.Id,
                                UserProfileId = userProfile.Id
                            }
                        });
                    }
                }

                return BadRequest();
            }

            return Conflict();
        }

        [HttpPost("auth/getcode")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Authentication([FromBody]PhoneNumberModel model)
        {
            if (await _userProfileService.IsUserProfileExists(model.PhoneNumber))
            {
                var code = CodeGenerator.GenerateCode();
                if (await _smsSenderService.SendCode(model.PhoneNumber, code))
                {
                    RememberSmsCode(model.PhoneNumber, code);

                    return Ok();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Forbid();
        }

        [HttpPost("auth/verifyphone")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        public async Task<IActionResult> Authentication([FromBody]VerifyCodeModel model)
        {
            var userProfile = await _userProfileService.GetUserProfileByPhone(model.PhoneNumber);
            if (userProfile != null)
            {
                var result = RecallSmsCode(model.PhoneNumber, out var code);
                if (result)
                {
                    if (string.Equals(code, model.Code))
                    {
                        ForgetSmsCode(model.PhoneNumber);

                        var refreshToken = _refreshTokenService.GenerateRefreshToken();
                        var refreshTokenModel = await _userProfileService.CreateRefreshToken(refreshToken, userProfile.Id);
                        var jwt = _jwtService.GenerateToken(model.PhoneNumber, userProfile.Id, refreshTokenModel.Id);

                        return base.Ok(new TokensModel()
                        {
                            Jwt = jwt,
                            RefreshToken = new RefreshTokenModel()
                            {
                                RefreshToken = refreshTokenModel.Token,
                                RefreshTokenId = refreshTokenModel.Id,
                                UserProfileId = userProfile.Id
                            }
                        });
                    }

                    return BadRequest();
                }

                return StatusCode(StatusCodes.Status408RequestTimeout);
            }

            return Forbid();
        }

        [HttpPost("refreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenModel model)
        {
            var userProfile = await _userProfileService.GetUserProfileById(model.UserProfileId);
            if (userProfile != null)
            {
                var userProfileRefreshToken = await _userProfileService.GetRefreshTokenById(model.RefreshTokenId);
                if (userProfileRefreshToken != null)
                {
                    if (userProfile.Id == userProfileRefreshToken.UserProfileId)
                    {
                        if (_userProfileService.CheckRefreshTokenExpirationDate(userProfileRefreshToken))
                        {
                            if (_refreshTokenService.CompareTokens(model.RefreshToken,
                                userProfileRefreshToken.Token))
                            {
                                await _userProfileService.ExtendRefreshToken(userProfileRefreshToken);
                                var newJwt = _jwtService.GenerateToken(userProfile.PhoneNumber, userProfile.Id,
                                    userProfileRefreshToken.Id);

                                return Ok(new JwtModel()
                                {
                                    Jwt = newJwt
                                });
                            }
                        }
                        else
                        {
                            return Conflict();
                        }
                    }
                }
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost("signout")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Signout([FromBody]RefreshTokenModel model)
        {
            var userProfile = await _userProfileService.GetUserProfileById(model.UserProfileId);
            if (userProfile != null)
            {
                var userProfileRefreshToken = await _userProfileService.GetRefreshTokenById(model.RefreshTokenId);
                if (userProfileRefreshToken != null)
                {
                    if (userProfile.Id == userProfileRefreshToken.UserProfileId)
                    {
                        if (_refreshTokenService.CompareTokens(model.RefreshToken, userProfileRefreshToken.Token))
                        {
                            _refreshTokenService.BlockToken(userProfileRefreshToken.Id);
                            await _userProfileService.DeleteRefreshToken(userProfileRefreshToken);

                            return Ok();
                        }
                    }
                }
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost("allsignout")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SignoutFromAllDevices([FromBody]RefreshTokenModel model)
        {
            var userProfile = await _userProfileService.GetUserProfileById(model.UserProfileId);
            if (userProfile != null)
            {
                var userProfileRefreshToken = await _userProfileService.GetRefreshTokenById(model.RefreshTokenId);
                if (userProfileRefreshToken != null)
                {
                    if (userProfile.Id == userProfileRefreshToken.UserProfileId)
                    {
                        if (_refreshTokenService.CompareTokens(model.RefreshToken, userProfileRefreshToken.Token))
                        {
                            var userProfileRefreshTokens =
                                _userProfileService.GetUserProfileRefreshTokens(userProfile.Id);
                            var refreshTokenIds = userProfileRefreshTokens.Select(refreshToken => refreshToken.Id)
                                .ToArray();

                            await _userProfileService.DeleteRefreshTokens(userProfileRefreshTokens);
                            _refreshTokenService.BlockTokens(refreshTokenIds);

                            return Ok();
                        }
                    }
                }
            }

            return Forbid();
        }

        [Authorize]
        [HttpGet("isauth")]
        public string IsAuthorize()
        {
            return DateTime.Now.ToLongTimeString();
        }

        private void RememberSmsCode(string phoneNumber, string code)
        {
            ForgetSmsCode(phoneNumber);
            _memoryCache.Set(phoneNumber, code, TimeSpan.FromMinutes(1));
        }

        private bool RecallSmsCode(string phoneNumber, out string code)
        {
            return _memoryCache.TryGetValue(phoneNumber, out code);
        }

        private void ForgetSmsCode(string phoneNumber)
        {
            _memoryCache.Remove(phoneNumber);
        }
    }
}