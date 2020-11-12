using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        readonly DataContext context;
        readonly ITokenService tokenService;
        public AccountController(DataContext _context, ITokenService _tokenService)
        {
            context = _context;
            tokenService = _tokenService;

        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDtos Dtos)
        {

            Debug.Write("Logged IN");

            if (await UserExist(Dtos.UserName))
                return BadRequest("Username is taken!");

            var hmac = new HMACSHA512();


            var user = new AppUser
            {
                UserName = Dtos.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Dtos.Password)),
                Passwordsalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();


            return new UserDto
            {

                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            };
             
        } 

        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO)
        {
            Console.WriteLine("Logged IN");

            var user = await context.Users.
                SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.Passwordsalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Passowrd");
            }

            return new UserDto
            {

                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExist(string username)
        {
            return await context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }


}
