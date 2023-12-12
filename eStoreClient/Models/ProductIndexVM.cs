using Repositories.Entities;

namespace eStoreClient.Models
{
    public class ProductIndexVM
    {
        public string Search { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
