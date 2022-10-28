using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenHandler tokenHandler;

        public AuthController(IUserRepository userRepository,ITokenHandler tokenHandler)
        {
            this.userRepository = userRepository;
            this.tokenHandler = tokenHandler;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAysnc(Models.DTO.LoginRequest loginRequest)
        {
            //validate the incoming request
            //here we are using fluent validation so this no need 

            //check if user is unthenticated
            ////check username and pwd
          var user= await userRepository.AuthenticateAsync(
              loginRequest.UserName, loginRequest.Password);

            if (User!=null)
            {
             var token= await tokenHandler.CreateTokenAsync(user);
             return Ok(token);
                //Generate a jwt token
            }

            return BadRequest("Username or password is incorrect");
        }
    }
}
