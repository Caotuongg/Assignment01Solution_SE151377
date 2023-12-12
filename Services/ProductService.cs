    using Repositories.Entities;
using Repositories.Repositories;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProductService
    {
        Task<Product> Get(int id);
        IEnumerable<Product> GetAll();
        Task<bool> Add(Product product);
        Task<bool> Update(Product product);
        Task<bool> Delete(int id);
        IEnumerable<Product> Search(string search);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            
        }

        public Task<bool> Add(Product product)
        {
            try
            {
                return productRepository.Add(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        

        public Task<Product> Get(int id)
        {
            try
            {
                return productRepository.Get(id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                var products = productRepository.GetAll();
                foreach (var rating in products)
                {
                    rating.Category = categoryRepository.Get(rating.CategoryId).Result;
                    rating.Category.Products = null;
                   
                }
                return products ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                
                var product = productRepository.Get(id).Result;
                if (product != null)
                {
                    
                    return productRepository.Delete(id);
                }
                else
                {
                    throw new Exception("Not Found Product");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Task<bool> Update(Product item)
        {
            try
            {
                var product = productRepository.Get(item.ProductId).Result;
                //if (productRepository.GetAll(x => x.ProductId != item.ProductId && x.ProductName == item.ProductName).Any())
                //{
                //    throw new Exception("ProductName exist");
                //}
                product.ProductName = item.ProductName;
                product.UnitPrice = item.UnitPrice;
                product.UnitsInStock = item.UnitsInStock;
                product.Weight = item.Weight;

                return productRepository.Update(product.ProductId, product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> Search(string search)
        {
            try
            {
                var list = new List<Product>();
                if(decimal.TryParse(search, out decimal price))
                {
                    var listprice = productRepository.GetAllSearch(x => x.UnitPrice == price);
                    foreach (var item in listprice)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    var listname = productRepository.GetAllSearch(x => x.ProductName.Contains(search));
                    foreach (var item in listname)
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Product CheckValidate(Product productIn, Product productDb, List<Product> products)
        {
            if (string.IsNullOrWhiteSpace(productIn.ProductName))
            {
                throw new Exception("Product Name cannot be empty");
            }
            if (productIn.UnitPrice == null || productIn.UnitPrice <= 0)
            {
                throw new Exception("Unit Price Invalid");
            }
            if (productIn.UnitsInStock == null || productIn.UnitsInStock < 0)
            {
                throw new Exception("Unit In Stock Invalid");
            }
            if (productIn.Weight == null)
            {
                throw new Exception("Weight Invalid");
            }
            if (productDb != null)
            {
                if (products.Where(x => x.ProductId != productDb.ProductId && x.ProductName == productIn.ProductName).Any())
                {
                    throw new Exception("Product Name is dupplicate");
                }
                productDb.UnitPrice = productIn.UnitPrice;
                productDb.UnitsInStock = productIn.UnitsInStock;
                productDb.Weight = productIn.Weight;
                productDb.CategoryId = productIn.CategoryId;
                productDb.ProductName = productIn.ProductName;
                return productDb;
            }
            if (productDb == null)
            {
                if (products.Where(x => x.ProductName == productIn.ProductName).Any())
                {
                    throw new Exception("Product Name is dupplicate");
                }
                
                return productIn;
            }
            return null;
        }
    }
}

