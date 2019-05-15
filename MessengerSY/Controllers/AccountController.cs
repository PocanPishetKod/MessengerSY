using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MessengerSY.Core.Domain;
using MessengerSY.Extensions;
using MessengerSY.Hubs;
using MessengerSY.Models.Account;
using MessengerSY.Services.JwtService;
using MessengerSY.Services.RefreshTokenService;
using MessengerSY.Services.SmsService;
using MessengerSY.Services.UserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace MessengerSY.Controllers
{
    [Produces("application/json")]
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

        [HttpGet("testgetjwtmy")]
        public string TestGetJwt()
        {
            string phonenumber = "+79951970169";
            return _jwtService.GenerateToken(phonenumber, 5, "", 2);
        }

        [HttpGet("testgetjwtmom")]
        public string TestGetJwMomt()
        {
            string phonenumber = "+79284234038";
            return _jwtService.GenerateToken(phonenumber, 8, "", 2);
        }

        [HttpGet("testgetjwtpap")]
        public string TestGetJwtPap()
        {
            string phonenumber = "+79284234037";
            return _jwtService.GenerateToken(phonenumber, 9, "", 2);
        }

        /// <summary>
        /// Отправляет код на переданный номер
        /// </summary>
        /// <param name="model">Номер телефона в формате +7**********</param>
        /// <response code="200">Запрос обработан успешно, код выслан</response>
        /// <response code="500">Невозможно отправить код по внутренним причинам</response>
        /// <response code="400">Входные данные не прошли валидацию</response>
        /// <response code="409">Пользователь с таким номеров уже зарегистрирован</response>
        /// <response code="403">Пользователь аутентифицирован</response>
        [HttpPost("registration/getcode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Registration([FromBody]PhoneNumberModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
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

            return Forbid();
        }

        [HttpPost("testreg")]
        public async Task<IActionResult> RegistrationForTests([FromBody]PhoneNumberModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                if (!await _userProfileService.IsUserProfileExists(model.PhoneNumber))
                {
                    var refrashToken = new RefreshToken()
                    {
                        Token = _refreshTokenService.GenerateRefreshToken()
                    };

                    var userProfile = new UserProfile()
                    {
                        PhoneNumber = model.PhoneNumber
                    };

                    userProfile.RefreshTokens.Add(refrashToken);

                    await _userProfileService.AddUserProfile(userProfile);

                    var jwt = _jwtService.GenerateToken(userProfile.PhoneNumber, userProfile.Id, userProfile.Nickname,
                        refrashToken.Id);

                    return Ok(new TokensModel()
                    {
                        Jwt = jwt,
                        RefreshToken = new RefreshTokenModel()
                        {
                            RefreshToken = refrashToken.Token,
                            RefreshTokenId = refrashToken.Id,
                            UserProfileId = userProfile.Id
                        }
                    });
                }

                return Conflict();
            }

            return Forbid();
        }

        [HttpPost("testauth")]
        public async Task<IActionResult> AuthForTest([FromBody] PhoneNumberModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                var userProfile = await _userProfileService.GetUserProfileByPhone(model.PhoneNumber);
                if (userProfile != null)
                {
                    var refreshToken = _refreshTokenService.GenerateRefreshToken();
                    var refreshTokenModel =
                        await _userProfileService.CreateRefreshToken(refreshToken, userProfile.Id);
                    var jwt = _jwtService.GenerateToken(model.PhoneNumber, userProfile.Id, userProfile.Nickname,
                        refreshTokenModel.Id);

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
            }

            return Forbid();
        }

        /// <summary>
        /// Регистрирует пользователя если правильно введет смс код.
        /// Сервер помнит отправленный код в течение 1 минуты.
        /// </summary>
        /// <param name="model">Телефонный номер и код из смс</param>
        /// <returns>При успешной регистрации возвращает сгенерированные jwt и refresh токены.</returns>
        /// <response code="200">Пользователь успешно зарегистрирован</response>
        /// <response code="409">Пользователь с таким номером уже зарегистрирован</response>
        /// <response code="400">Входные данные не прошли валидацию или код не правильный</response>
        /// <response code="408">Время ввода кода истекло</response>
        /// <response code="403">Пользователь аутентифицирован</response>
        [HttpPost("registration/verifyphone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        public async Task<IActionResult> Registration([FromBody]VerifyCodeModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
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

                            var jwt = _jwtService.GenerateToken(userProfile.PhoneNumber, userProfile.Id, userProfile.Nickname,
                                refreshTokenModel.Id);

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

                        return BadRequest(new {message = "Неверный код"});
                    }

                    return StatusCode(StatusCodes.Status408RequestTimeout);
                }

                return Conflict();
            }

            return Forbid();
        }

        /// <summary>
        /// Отправляет код на переданный номер
        /// </summary>
        /// <param name="model">Номер телефона в формате +7**********</param>
        /// <response code="200">Запрос обработан успешно, код выслан</response>
        /// <response code="500">Невозможно отправить код по внутренним причинам</response>
        /// <response code="400">Входные данные не прошли валидацию</response>
        /// <response code="403">Пользовательеще не зарегистрирован или аутентифицирован</response>
        [HttpPost("auth/getcode")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Authentication([FromBody]PhoneNumberModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
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
            }

            return Forbid();
        }

        /// <summary>
        /// Аутентифицирует пользователя если правильно введет смс код.
        /// Сервер помнит отправленный код в течение 1 минуты.
        /// </summary>
        /// <param name="model">Телефонный номер и код из смс</param>
        /// <returns>При успешной регистрации возвращает сгенерированные jwt и refresh токены.</returns>
        /// <response code="200">Пользователь успешно аутентифицирован</response>
        /// <response code="409">Пользователь с таким номером еще не зарегистрирован</response>
        /// <response code="400">Входные данные не прошли валидацию или код не правильный</response>
        /// <response code="408">Время ввода кода истекло</response>
        /// <response code="403">Пользовательеще не зарегистрирован или аутентифицирован</response>
        [HttpPost("auth/verifyphone")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        public async Task<IActionResult> Authentication([FromBody]VerifyCodeModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
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
                            var refreshTokenModel =
                                await _userProfileService.CreateRefreshToken(refreshToken, userProfile.Id);
                            var jwt = _jwtService.GenerateToken(model.PhoneNumber, userProfile.Id, userProfile.Nickname,
                                refreshTokenModel.Id);

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
            }

            return Forbid();
        }

        /// <summary>
        /// Выдает новый jwt при истечении времени жизни старого jwt и продливает жизнь refresh токену.
        /// Если время жизни refresh токена истекло, то нужно пройти этап аутентификации.
        /// </summary>
        /// <param name="model">Refresh токен</param>
        /// <returns>Новый jwt</returns>
        /// <response code="200">Refresh токен прошел валдацию и сгенерирован новый jwt</response>
        /// <response code="400">Refresh токен не прошел начальную валидацию</response>
        /// <response code="403">Пользователя не существует, у пользователся нет refresh токенов, refresh токен не соответствует пользователю или срок действия токена не закончился</response>
        /// <response code="409">Время жизни refresh токена истекло. Нужно пройти этап аутентификации</response>
        [HttpPost("refreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenModel model)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
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
                                    var newJwt = _jwtService.GenerateToken(userProfile.PhoneNumber, userProfile.Id, userProfile.Nickname,
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
            }

            return Forbid();
        }

        /// <summary>
        /// Деактивирует jwt, связанный с refresh токеном
        /// </summary>
        /// <param name="model">Refresh токен</param>
        /// <returns>Новый jwt</returns>
        /// <response code="200">Refresh токен прошел валдацию и jwt успешно деактивирован</response>
        /// <response code="400">Refresh токен не прошел начальную валидацию</response>
        /// <response code="403">Пользователя не существует, у пользователся нет refresh токенов или refresh токен не соответствует пользователю</response>
        [Authorize]
        [HttpPost("signout")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Signout([FromBody]RefreshTokenModel model)
        {
            if (User.GetUserProfileId() == model.UserProfileId  && User.GetRefreshTokenId() == model.RefreshTokenId)
            {
                var userProfileRefreshToken = await _userProfileService.GetRefreshTokenById(model.RefreshTokenId);
                if (userProfileRefreshToken != null)
                {
                    if (User.GetUserProfileId() == userProfileRefreshToken.UserProfileId)
                    {
                        if (_refreshTokenService.CompareTokens(model.RefreshToken, userProfileRefreshToken.Token))
                        {
                            _jwtService.BlockToken(userProfileRefreshToken.Id);
                            await _userProfileService.DeleteRefreshToken(userProfileRefreshToken);

                            return Ok();
                        }
                    }
                }
            }

            return Forbid();
        }

        /// <summary>
        /// Деактивирует все jwt пользователя и удаляет все refresh токены пользователя
        /// </summary>
        /// <param name="model">Refresh токен</param>
        /// <returns>Новый jwt</returns>
        /// <response code="200">Refresh токен прошел валдацию, все jwt деактивированы, все refresh токены удалены</response>
        /// <response code="400">Refresh токен не прошел начальную валидацию</response>
        /// <response code="403">Пользователя не существует, у пользователся нет refresh токенов или refresh токен не соответствует пользователю</response>
        [Authorize]
        [HttpPost("allsignout")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SignoutFromAllDevices([FromBody]RefreshTokenModel model)
        {
            if (User.GetUserProfileId() == model.UserProfileId && User.GetRefreshTokenId() == model.RefreshTokenId)
            {
                var userProfileRefreshToken = await _userProfileService.GetRefreshTokenById(model.RefreshTokenId);
                if (userProfileRefreshToken != null)
                {
                    if (User.GetUserProfileId() == userProfileRefreshToken.UserProfileId)
                    {
                        if (_refreshTokenService.CompareTokens(model.RefreshToken, userProfileRefreshToken.Token))
                        {
                            var userProfileRefreshTokens =
                                _userProfileService.GetUserProfileRefreshTokens(User.GetUserProfileId());
                            var refreshTokenIds = userProfileRefreshTokens.Select(refreshToken => refreshToken.Id)
                                .ToArray();

                            await _userProfileService.DeleteRefreshTokens(userProfileRefreshTokens);
                            _jwtService.BlockTokens(refreshTokenIds);

                            return Ok();
                        }
                    }
                }
            }

            return Forbid();
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