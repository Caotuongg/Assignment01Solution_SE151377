using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using Repositories.Entities;
using eStoreAPI.ViewEntities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Http.Headers;
using Services;
using System.Security.Claims;

namespace eStoreClient.Controllers
{
    public class MembersController : Controller
    {
        private readonly HttpClient client = null;
        private string MemberApiUrl = "https://localhost:7021/api/Member/";
       

        public MembersController()
        {
            client = new HttpClient();


        }
        
            public IActionResult Login()
            {
                try
                {
                    if (!string.IsNullOrEmpty(TempData["LoginFail"] as string))
                    {
                        throw new Exception(TempData["LoginFail"] as string);
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewData["ErrMsg"] = ex.Message;
                    return View();
                }
            }

            // POST: Members/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(string email, string password)
            {
                HttpResponseMessage message = await client.PostAsJsonAsync(MemberApiUrl + "Login", new
                {
                    email = email,
                    password = password
                });



                string resData = await message.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                if (message.StatusCode == HttpStatusCode.NotFound || message.StatusCode == HttpStatusCode.InternalServerError)
                {
                    TempData["LoginFail"] = json["message"].ToString();
                    return RedirectToAction(nameof(Login));
                }
                if (message.StatusCode == HttpStatusCode.OK)
                {
                    var tokenJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["token"].ToString());
                    HttpContext.Session.SetString("Token", tokenJson["token"].ToString());
                    HttpContext.Session.SetString("RefreshToken", tokenJson["refreshToken"].ToString());

                    HttpContext.Session.SetString("Role", json["role"].ToString());

                    if (json["role"].ToString() == "Admin")
                    {
                        HttpContext.Session.SetString("Name", "Admin");
                        return RedirectToAction("AdminIndex", "Home");
                    }
                    else if (json["role"].ToString() == "Customer")
                    {
                        HttpContext.Session.SetString("Name", "Customer");
                        return RedirectToAction("CustomerIndex", "Home");
                    }
                }
                return RedirectToAction(nameof(ViewMember));
            }




        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Signup(Member member)
        {
            try
            {
                HttpResponseMessage reponse = await client.PostAsJsonAsync(MemberApiUrl + "AddMember", member);
                if(reponse.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("Login");
                }
                if(reponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var data = await reponse.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    throw new Exception(json["reponse"].ToString());
                }
                return RedirectToAction("Signup");
            }
            catch
            {
                return View(member);
            }
        }
        public async Task<IActionResult> ViewMember()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage message = await client.GetAsync(MemberApiUrl + "GetAll");

                string resData = await message.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                if (json["status"] == "Error")
                {
                    throw new Exception(json["Message"].ToString());
                }

                var mems = JsonConvert.DeserializeObject<List<Member>>(json["data"].ToString());
                return View(mems);
            }
            return RedirectToAction("ViewMember", "Members");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsMember(int id)
        {

            var role = HttpContext.Session.GetString("Role");
           
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            ViewData["Role"] = role;
            string url = "";
            if (role != "Admin")
            {
                url = MemberApiUrl + "GetALl";
            }
            else
            {
                url = MemberApiUrl + $"Get?id={id}";
            }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                HttpResponseMessage message = await client.GetAsync(url);

                if (message.StatusCode == HttpStatusCode.OK)
                {


                    string resData = await message.Content.ReadAsStringAsync();

                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                    var mem = JsonConvert.DeserializeObject<Member>(json["data"].ToString());

                    return View(mem);
                }
                else if (message.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                
            
            return RedirectToAction("AdminIndex", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            ViewData["Role"] = role;
            
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                HttpResponseMessage message = await client.GetAsync(MemberApiUrl + $"Get?id={id}");

                if (message.StatusCode == HttpStatusCode.OK)
                {
                 
                    string resData = await message.Content.ReadAsStringAsync();
                    var mem = ConvertMember(resData);
                    //var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                //var mem = JsonConvert.DeserializeObject<Member>(json["data"].ToString());

                    return View(mem);
                }
                else if (message.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
            
            return RedirectToAction("AdminIndex", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(int id, Member member)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            try
            {
                if (role == "Admin")
                {
                    //if (string.IsNullOrWhiteSpace(member.Country))
                    //{
                    //    throw new Exception("Country cannot be empty!!!");
                    //}
                    //if (string.IsNullOrWhiteSpace(member.City))
                    //{
                    //    throw new Exception("City cannot be empty!!!");
                    //}
                    //if (string.IsNullOrWhiteSpace(member.CompanyName))
                    //{
                    //    throw new Exception("Company Name cannot be empty!!!");
                    //}

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                    HttpResponseMessage message = await client.PutAsJsonAsync(MemberApiUrl + $"UpdateMember", member);

                    //if (message.StatusCode == HttpStatusCode.OK)
                    //{
                    //    return RedirectToAction(nameof(ViewMember));
                    //}
                    //if (message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    //{
                    //    TempData["LoginFail"] = "You are not login";
                    //    return RedirectToAction("Login", "Members");
                    //}
                     if (message.StatusCode == HttpStatusCode.InternalServerError || message.StatusCode == HttpStatusCode.NotFound)
                    {
                        string resData = await message.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);
                        throw new Exception(json["message"].ToString());
                    }
                }
                return RedirectToAction(nameof(ViewMember));
            }
            catch (Exception ex)
            {
                ViewData["ErrMsg"] = ex.Message;
                return View(member);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMember(int? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.GetAsync(MemberApiUrl + $"Get?id={id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                    var mem = JsonConvert.DeserializeObject<Member>(json["data"].ToString());
                    return View(mem);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.NotFound)
                {
                    string resData = await response.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);
                    ViewData["ErrMsg"] = json["response"];

                    return RedirectToAction(nameof(Index));
                }
                
            }
            return RedirectToAction("ViewMember", "Members");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            if (role == "Admin")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.DeleteAsync(MemberApiUrl + $"DeleteMember?id={id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(ViewMember));
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
            }
        
            return RedirectToAction(nameof(ViewMember));
        }

        private Member ConvertMember(string resData)
        {
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

            var member = JsonConvert.DeserializeObject<Member>(json["data"].ToString());

            return member ;
        }

        [HttpGet]
        public IActionResult Error()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
            {
                HttpContext.Session.Remove("Role");
                HttpContext.Session.Remove("Token");
                HttpContext.Session.Remove("RefreshToken");
                HttpContext.Session.Remove("Name");
                HttpContext.Session.Remove("Cart");
            }
            return RedirectToAction("Login");
        }
    }
}
