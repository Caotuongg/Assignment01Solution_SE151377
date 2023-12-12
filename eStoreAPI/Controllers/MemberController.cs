using AutoMapper;
using eStoreAPI.ViewEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Repositories.Entities;
using Services;
using System.Security.Claims;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemberService memberService;
        private readonly IMapper mapper;

        public MemberController(IMemberService memberService, IMapper mapper, IConfiguration configuration)
        {
            this.memberService = memberService;
            this.mapper = mapper; 
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new
                {
                    Status = "Get List Success",
                    Data = memberService.GetAll()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {

            try
            {
                // Kiểm tra xác thực người dùng sau khi đăng nhập
                var role = User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login",
                    });
                }
                //User
                if (role != "Admin")
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Data = await memberService.Get(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value))
                    });
                }
                //Admin
                if (role == "Admin")
                {
                    return Ok(new
                    {
                        Status = "Get ID success",
                        Data = await memberService.Get(id)
                    });
                }
                return StatusCode(404, new
                {
                    Status = "Error",
                    Message = "Not Found",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(MemberVE memberVE)
        {
            try
            {
                //var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                //if (!string.IsNullOrEmpty(role))
                //{
                //if (role == "ADMIN" && role == "STAFF")
                //{
                var member = Validate(memberVE);
                var check = memberService.Add(member);
                return await check ? Ok(new
                {
                    Status = 1,
                    Message = "Add Member Success"
                }) : Ok(new
                {
                    Status = 0,
                    Message = "Add Member Fail"
                });
                //}
                //else
                // return   //Ok(new
                //{
                //    Status = 0,
                //    Message = "Role Denied",
                //    Data = new { }
                //});

                //else
                //{
                //    return Unauthorized();
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMember(MemberVE memberVE)
        {

            try
            {
                var role = User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role)?.Value;
                //if (!string.IsNullOrEmpty(role))
                //{
                //    return StatusCode(401, new
                //    {
                //        Status = "Error",
                //        Message = "You are not login",
                //    });
                //}
                if (role == "Admin")
                {
                    var product = Validate(memberVE);
                    var check = memberService.Update(product);
                    return await check ? Ok(new
                    {
                        Status = true,
                        Message = "Update Member Success"
                    }) : Ok(new
                    {
                        Status = false,
                        Message = "Update Member Fail"
                    });
                }
                else
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found",
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Not Login"
                    });
                }
                if(role == "Admin")
                {
                    var check = memberService.Delete(id);
                    return await check ? Ok(new
                    {
                        Status = 1,
                        Message = "Delete Member Success"
                    }) : Ok(new
                    {
                        Status = 0,
                        Message = "Delete Member Fail"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied",
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        


        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (login.Email == configuration["Admin:Email"])
                {
                    if (login.Password == configuration["Admin:Password"])
                    {
                        return Ok(new
                        {
                            Status = "Login Success",
                            Role = "Admin",
                            Info = new { },
                            Token = JWTManage.GetToken("Admin", "Admin")
                        });
                    }
                }
                var mem = await memberService.Login(login.Email, login.Password);
                if (mem == null)
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Incorect Email and Password",
                    });
                }
                //if (mem.IsDeleted)
                //{
                //    return StatusCode(404, new
                //    {
                //        Status = "Error",
                //        Message = "Your account had been deleted in the system!!!",
                //    });
                //}
                return Ok(new
                {
                    Status = "Login Success",
                    Role = "Customer",
                    Info = mem,
                    Token = JWTManage.GetToken("Customer", mem.MemberId + "")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        private Member Validate(MemberVE memberVE)
        {
            if (string.IsNullOrEmpty(memberVE.Email.Trim()))
            {
                throw new Exception("Email cannot be empty!!!");
            }
            if (string.IsNullOrEmpty(memberVE.CompanyName.Trim()))
            {
                throw new Exception("CompanyName cannot be empty!!!");
            }
            if (string.IsNullOrEmpty(memberVE.City.Trim()))
            {
                throw new Exception("City cannot be empty!!!");
            }
            if (string.IsNullOrEmpty(memberVE.Country.Trim()))
            {
                throw new Exception("Country cannot be empty!!!");
            }
            if (string.IsNullOrEmpty(memberVE.Password.Trim()))
            {
                throw new Exception("Password cannot be empty!!!");
            }

            return mapper.Map<Member>(memberVE);
        }

    }
}

