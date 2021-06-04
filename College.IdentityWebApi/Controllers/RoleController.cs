using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using College.IdentityWebApi.Domain;
using College.IdentityWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



namespace College.IdentityWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<Role> roleManager,
                            UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/<RoleController>
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Administrador", "Pode retornar" };
        }

        // GET: api/<RoleController>
        [HttpGet]
        [Authorize(Roles = "Administrador, Gerente")]
        public IEnumerable<string> GetAdministrators()
        {
            return new string[] { "Gerente e admin", "Pode retornar" };
        }

        // GET: api/<RoleController>
        [HttpGet]
        [Authorize(Roles = "Vendedor")]
        public IEnumerable<string> GetSallers()
        {
            return new string[] { "Vendedor", "Pode retornar" };
        }

        // POST api/<RoleController>
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleModel roleModel)
        {
            try
            {
                return Ok(await _roleManager.CreateAsync(new Role { Name = roleModel.Name })); 
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message} ao criar role!");
            }
        }

        // PUT api/<RoleController>/5
        [HttpPut("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRole(UpdateRoleModel updateRoleModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(updateRoleModel.Email);

                if(user != null) 
                {
                    if (updateRoleModel.Delete)
                        await _userManager.RemoveFromRoleAsync(user, updateRoleModel.Role);
                    else
                        await _userManager.AddToRoleAsync(user, updateRoleModel.Role);
                }

                return Ok("Sucesso");
            }
            catch (Exception ex)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message} ao atualizar role!");
            }
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
