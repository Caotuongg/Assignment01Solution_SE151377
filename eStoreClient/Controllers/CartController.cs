using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.Entities;

namespace eStoreClient.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration configuration;
        private string OrderApiUrl = "https://localhost:7021/api/Order/";
        private string ProductApiUrl = "https://localhost:7021/api/Product/";

        public CartController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (TempData["ErrMsg"] != null)
            {
                ViewData["ErrMsg"] = TempData["ErrMsg"].ToString();
            }
            if (!string.IsNullOrEmpty(role))
            {
                ViewData["Role"] = role;
            }
            try
            {
                var cart = new List<OrderDetail>();
                var cartString = HttpContext.Session.GetString("Cart");
                if (string.IsNullOrEmpty(cartString))
                {
                    cart = new List<OrderDetail>();
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<List<OrderDetail>>(cartString);
                }
                return View(cart);
            }
            catch (Exception ex)
            {
                ViewData["ErrMsg"] = ex.Message;
                return View(new List<OrderDetail>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCartInProduct(int id)
        {
            await AddProduct(id, 1);
            return RedirectToAction("Index", "Product");
        }
        [HttpGet]
        public async Task<IActionResult> AddCart(int id)
        {
            await AddProduct(id, 1);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> RemoveCartAsync(int id)
        {
            await AddProduct(id, -1);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            await AddProduct(id, 0);
            return RedirectToAction("Index");
        }

        private async Task AddProduct(int id, int quantity)
        {

            var product = await GetProduct(id);
            if (product != null)
            {
                var cartString = HttpContext.Session.GetString("Cart");
                var cart = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(cartString))
                {
                    cart = JsonConvert.DeserializeObject<List<OrderDetail>>(cartString);
                }
                var orderDetail = new OrderDetail();
                var existInCart = false;
                var remove = false;
                foreach (var item in cart)
                {
                    if (item.ProductId == id)
                    {
                        if (quantity == 0 || (item.Quantity == 1 && quantity < 0))
                        {
                            orderDetail = item;
                            remove = true;
                        }
                        else
                        {
                            item.Quantity = item.Quantity + quantity;
                        }
                        existInCart = true;
                    }
                }
                if (!existInCart && quantity > 0)
                {
                    cart.Add(new OrderDetail
                    {
                        ProductId = id,
                        Quantity = quantity,
                        Discount = 0,
                        
                        UnitPrice = product.UnitPrice,
                        Product = product,
                    });
                }
                if (quantity == 0 || remove)
                {
                    cart.Remove(orderDetail);
                }

                var json = JsonConvert.SerializeObject(cart);
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.SetString("Cart", json);
            }
        }

        private async Task<Product> GetProduct(int id)
        {
            HttpResponseMessage message = await _httpClient.GetAsync($"https://localhost:7021/api/Product/Get?id={id}");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resData = await message.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(resData);

                var product = JsonConvert.DeserializeObject<Product>(json["data"].ToString());
                return product;
            }
            return null;
        }
    }
}
