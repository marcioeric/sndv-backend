using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.ViewModels.Inputs.Account;

namespace WebApplication.Controllers
{
    [Route("conta")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signManager)
        {
            _userManager = userManager;
            _signManager = signManager;
        }

        [HttpPost("entrar")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel viewModel)
        {
            //Busca dentro do banco de dados se o email informado existe.
            var user = await _userManager.FindByEmailAsync(viewModel.Email.Trim().ToLower());

            //Verifica se a consulta anteriror retornou algum usuário com o email informado.
            if(user is null) 
                return BadRequest(new {Message = "Usuário não encontrado em nossa base."});
            
            //Verifica se o usuário encotrado anteriormente consegue acessar com a senha informada.
            var loginResult = await _signManager.CheckPasswordSignInAsync(user, viewModel.Password, false);

            //Código vai ser executado caso o usuário possa acessar a aplicação
            //Retorna o token para o usuário
            if(loginResult.Succeeded)
            {
                var identity = new ClaimsIdentity(
                    new GenericIdentity(viewModel.Email, "Login"),
                    new []{
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
                    }
                );

                var createdDate = DateTime.Now;
                var expirationDate = createdDate + TimeSpan.FromSeconds(3600);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor{

                });

                var accessToken = handler.WriteToken(securityToken);
            }
            else
            {
                //Código vai ser executado caso o usuário NÃO possa acessar a aplicação
                //Retorna a mensagem de erro para o usuário
                return BadRequest(new {Message = "Usuário/Senha não estão coincidem."});
            }
        }
    }
}