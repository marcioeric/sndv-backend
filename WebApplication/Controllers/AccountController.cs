using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Configurations;
using WebApplication.ViewModels.Inputs.Account;

namespace WebApplication.Controllers
{
    [Route("conta")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly SigningConfigurations _signingConfigurations;

        public AccountController(UserManager<User> userManager, SignInManager<User> signManager, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations)
        {
            _userManager = userManager;
            _signManager = signManager;
            _tokenConfigurations = tokenConfigurations;
            _signingConfigurations = signingConfigurations;
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
                //Gerando a identidade do usuário
                var identity = new ClaimsIdentity(
                    new GenericIdentity(viewModel.Email, "Login"),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    }
                );

                //Calculando quando o token irá expirar baseado na data de criação dele
                var createdDate = DateTime.Now;
                var expirationDate = createdDate + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                //Configurando como o token deve ser gerado
                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = _tokenConfigurations.Issuer,
                    Audience = _tokenConfigurations.Audience,
                    SigningCredentials = _signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = createdDate,
                    Expires = expirationDate
                });

                //Gerando o token
                var accessToken = handler.WriteToken(securityToken);

                //Retornando o token para o usuário na requisição
                return Ok(new
                {
                    created = createdDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken
                });
            }
            else
            {
                //Código vai ser executado caso o usuário NÃO possa acessar a aplicação
                //Retorna a mensagem de erro para o usuário
                return BadRequest(new {Message = "Usuário/Senha não estão coincidem."});
            }
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountViewModel viewModel)
        {
            var user = new User
            {
                Email = viewModel.Email,
                UserName = viewModel.Email,
                IsEnabled = true,
                Name = viewModel.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if(result.Succeeded)
                //Caso o cadastro da conta tenha sido um sucesso, retornamos para ele 200
                return Ok("Conta cadastrada com êxito");
            else
                //Caso contrário, retornamos 400 e os erros!
                return BadRequest(result.Errors);
        }
    }
}