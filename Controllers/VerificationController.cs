using Attendr.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Attendr.IdentityServer.Helpers.VerificationHelper;

namespace Attendr.IdentityServer.Controllers
{
    [Route("account/verify")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public VerificationController(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        [HttpGet]
        public async Task<IActionResult> VerifyAccount([FromQuery] string user, [FromQuery] string verify)
        {
            VerificationStatusCodes verificationStatus = await _userRepository.VerifyUserAsync(user, verify);
            return GenerateVerificationResponse(verificationStatus);
        }

    }
}
