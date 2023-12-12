using Repositories.Entities;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        Task<Category> Get(int id);
        IEnumerable<Category> GetAll();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;
        }

        public Task<Category> Get(int id)
        {
            try
            {
                return categoryRepository.Get(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Category> GetAll()
        {
            try
            {
                return categoryRepository.GetAll();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       
    }
}
