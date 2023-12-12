using eStoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Repositories.Entities;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eStoreClient.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "https://localhost:7021/api/Product/";
        private string CategoryApiUrl = "https://localhost:7021/api/Category/";

        public ProductsController()
        {
            client = new HttpClient();
            
            
        }

        private async Task<List<Category>> GetCategories()
        {
            HttpResponseMessage message = await client.GetAsync(CategoryApiUrl + "GetAll");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string resData = await message.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                var categories = JsonConvert.DeserializeObject<List<Category>>(json["data"].ToString());

                return categories;
            }
            return null;
        }


        public async Task<ActionResult> ViewProduct()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + "GetAll");
            string data = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
                
           List<Product> products = JsonSerializer.Deserialize<List<Product>>(data,options);
            return View(products);
        }

        



        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            CheckAdmin();

            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            //ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            
            try
            {
                CheckAdmin();

                var role = HttpContext.Session.GetString("Role");
                ViewData["Role"] = role;
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync(ProductApiUrl + "AddProduct", product);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    string resData = await response.Content.ReadAsStringAsync();

                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                    throw new Exception(json["response"].ToString());

                }
                return RedirectToAction("ViewProduct");
            }
            catch (Exception ex)
            {
                ViewData["ErrMsg"] = ex.Message;
                //ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }
            }

        [HttpGet]
        public async Task<ActionResult> EditProduct(int id)
        {
            CheckAdmin();

            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + $"Get?id={id}");
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string data = await response.Content.ReadAsStringAsync();
                var product = ConvertProduct(data);
                //ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return RedirectToAction(nameof(Error));
            }
            return RedirectToAction(nameof(ViewProduct));
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            CheckAdmin();

            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            try
            {
                CheckValidate(product);
                var token = HttpContext.Session.GetString("Token");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PutAsJsonAsync(ProductApiUrl + $"UpdateProduct", product);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    string resData = await response.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);
                    throw new Exception(json["response"].ToString());
                }
                return RedirectToAction(nameof(ViewProduct));
            }
            catch(Exception ex)
            {
                //ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
                ViewData["ErrMsg"] = ex.Message;
                return View(product);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            CheckAdmin();

            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + $"Get?id={id}");
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = ConvertProduct(data);
                return View(product);
            }
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction(nameof(Error));
            }
            return RedirectToAction(nameof(ViewProduct));
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            CheckAdmin();

            var token = HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.DeleteAsync(ProductApiUrl + $"DeleteProduct?id={id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(ViewProduct));
            }
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                HttpResponseMessage getProduct = await client.GetAsync(ProductApiUrl + $"Get?id={id}");
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resDataPro = await getProduct.Content.ReadAsStringAsync();

                    var product = ConvertProduct(resDataPro);

                    string resData = await response.Content.ReadAsStringAsync();

                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                    ViewData["ErrMsg"] = json["response"].ToString();

                    return View(product);
                }
            }
            return RedirectToAction(nameof(ViewProduct));
        }

        [HttpGet]
        public async Task<IActionResult> DetailsProduct(int? id)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            HttpResponseMessage message = await client.GetAsync(ProductApiUrl + $"Get?id={id}");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resData = await message.Content.ReadAsStringAsync();

                var product = ConvertProduct(resData);
                return View(product);
            }
            if (message.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction(nameof(Error));
            }
            return RedirectToAction(nameof(ViewProduct));
        }


        private Product ConvertProduct(string resData)
        {
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

            var product = JsonConvert.DeserializeObject<Product>(json["data"].ToString());

            return product;
        }

        private List<Product> ConvertProducts(string resData)
        {
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

            var products = JsonConvert.DeserializeObject<List<Product>>(json["data"].ToString());

            return products;
        }

        [HttpGet]
        public IActionResult Error()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            return View();
        }

        private void CheckAdmin()
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                RedirectToAction("Login", "Members");
            }
            if (role != "Admin")
            {
                RedirectToAction("CustomerIndex", "Home");
            }
        }

        private void CheckValidate(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new Exception("Product Name can not be empty!!!");
            }
            if (product.Weight == null)
            {
                throw new Exception("Product Weight can not be empty!!!");
            }
            //if (product.Weight <= 0)
            //{
            //    throw new Exception("Product Weight can not be empty!!!");
            //}

            if (product.UnitPrice == null)
            {
                throw new Exception("Product Price can not be empty!!!");
            }
            if (product.UnitPrice <= 0)
            {
                throw new Exception("Product Price is not valid!!!");
            }
            if (product.UnitsInStock == null)
            {
                throw new Exception("Product In Stock can not be empty!!!");
            }
            if (product.UnitsInStock < 0)
            {
                throw new Exception("Product In Stock is not valid!!!");
            }
        }

        

    }
}
