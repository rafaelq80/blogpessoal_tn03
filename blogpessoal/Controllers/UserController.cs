using blogpessoal.Model;
using blogpessoal.Security;
using blogpessoal.Service;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogpessoal.Controllers
{
    [Route("~/usuarios")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IValidator<User> _userValidator;
        private readonly IAuthService _authService;

        public UserController(
            IUserService userService, 
            IValidator<User> userValidator,
            IAuthService authService
            )
        {
            _userService = userService;
            _userValidator = userValidator;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var Resposta = await _userService.GetById(id);

            if (Resposta is null)
                return NotFound();

            return Ok(Resposta);
        }

        [AllowAnonymous]
        [HttpPost("cadastrar")]
        public async Task<ActionResult> Create([FromBody] User user)
        {
            
            var validarUser = await _userValidator.ValidateAsync(user);

            if (!validarUser.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validarUser);
            }

            var Resposta = await _userService.Create(user);

            if (Resposta is null)
                return BadRequest("Usuário já está Cadastrado!");

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);

        }

        [Authorize]
        [HttpPut("atualizar")]
        public async Task<ActionResult> Update([FromBody] User user)
        {
            if (user.Id == 0)
                return BadRequest("Id da User é inválido!");

            var validarUser = await _userValidator.ValidateAsync(user);

            if (!validarUser.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validarUser);
            }

            var UserUpdate = await _userService.GetByUsuario(user.Usuario);

            if (UserUpdate is not null && UserUpdate.Id != user.Id)
                return BadRequest("O Usuário (e-mail) já está em uso por outro usuário");

            var Resposta = await _userService.Update(user);

            if (Resposta is null)
                return NotFound("User não Encontrada!");

            return Ok(Resposta);

        }

        [AllowAnonymous]
        [HttpPost("logar")]
        public async Task<ActionResult> Autenticar([FromBody] UserLogin userLogin)
        {

            var Resposta = await _authService.Autenticar(userLogin);

            if (Resposta is null)
                return Unauthorized("Usuário e/ou Senha são inválidos!");

            return Ok(Resposta);

        }
    }
}
