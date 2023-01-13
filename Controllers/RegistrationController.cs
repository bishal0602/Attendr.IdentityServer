using Attendr.IdentityServer.Models;
using Attendr.IdentityServer.Helpers;
using Attendr.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Attendr.IdentityServer.Models.Email;

namespace Attendr.IdentityServer.Controllers
{
    [Route("account/register")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public RegistrationController(IUserRepository userRepository, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAccount([FromBody] UserRegistrationDto user)
        {
            user.Email = user.Email.Trim().ToLower();
            user.Username = user.Username.Trim().ToLower();

            if (!EmailHelper.IsEmailApproved(user.Email))
            {
                return BadRequest(new ErrorModel("Sorry, this email is not approved by the application"));
            }

            if (await _userRepository.IsAccountActive(user.Email))
            {
                return BadRequest(new ErrorModel("Account is already registered!"));
            }

            if (await _userRepository.ExistsUsernameAsync(user.Username))
            {
                return BadRequest(new ErrorModel("Username is already taken!"));
            }

            if (await _userRepository.IsEmailAlreadyInUseAsync(user.Email))
            {
                // override account if email is in use by account that is not verified
                await _userRepository.RemoveUserByEmailAsync(user.Email);
            }

            var userToAddToDb = _mapper.Map<Entities.User>(user);
            await _userRepository.CreateUserAsync(userToAddToDb);
            await _userRepository.SaveAsync();

            // Verification Mail
            var request = _httpContextAccessor.HttpContext!.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            string verificationUrl = EmailHelper.CreateVerificationUrl(baseUrl, userToAddToDb.Username, userToAddToDb.VerificationCode);

            var message = new Message(new string[] { userToAddToDb.Email }, "Verification Test", verificationUrl); // TODO: Update content and subject
            await _emailSender.SendEmailAsync(message);

            return Ok(new { message = "Account has been succesfully registered. Check your email to activate account!" });

        }
    }
}
