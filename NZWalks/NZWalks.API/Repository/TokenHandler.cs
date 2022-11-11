using Microsoft.IdentityModel.Tokens;
using NZWalks.API.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace NZWalks.API.Repository
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration configuration;

        public TokenHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(User user)
        {
            //create claims

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.GivenName,user.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname,user.LastName));
            claims.Add(new Claim(ClaimTypes.Email,user.EmailAddress));


            //loop into the roles of users

            user.Roles.ForEach((role) =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });

            //create a token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token= new JwtSecurityToken(
                configuration["jwt:Issuer"],
                configuration["jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);   
            
          return  new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
