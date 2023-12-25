using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp2
{
    public class user { 
        public string Name { get; set; }
        public string Role { get; set; }
    }

    [Route("/[controller]")]
    [ApiController]
    public class CookieController : ControllerBase
    {
        // replace those users with database users 
        public List<user> Users = new List<user>(){
            new user { Name = "aviv", Role = "admin" },
            new user { Name = "david", Role = "user" }
        };

        // cookie Settings
        public Boolean CookieExistWhenCloseBrowser = true;
        public DateTime CookieTime = DateTime.UtcNow.AddMinutes(5); // 5 mins

        [HttpPost]
        [Route("/[controller]/login")]
        public async Task<ActionResult> Login([FromForm] string name) 
        {
            var user = Users.Find(x => x.Name == name);
            if (user == null) {
                return Redirect("/");
            }

            
            // create cookie token 
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var claimsIdentity = new ClaimsIdentity(claims,"auth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var AuthenticationProperties = new AuthenticationProperties
            {
                IsPersistent = CookieExistWhenCloseBrowser,
                ExpiresUtc = CookieTime
            };
            
            //sign in user 
            await HttpContext.SignInAsync(claimsPrincipal, AuthenticationProperties);
            //redirct user 
            return Redirect("/user/");
        }
        [HttpPost]
        [Route("/[controller]/logout")]
        public async Task<ActionResult> LogOut() 
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

    }
}
