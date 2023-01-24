using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.ViewModel;
using OnlineStore.Data;

namespace OnlineStore.Controllers
{
    public class FilterController : Controller
    {
        UnitOfWork unit = new UnitOfWork();
        public IActionResult Index(int? category, float? lowprice, float? upprice, float? lowrate, string name, int page = 1, SortState sortOrder= SortState.NameAsc)
        {
            int pageSize = 4;
            var products = unit.ProductRepository.Get(includeProperties: "Image,Rates,Category").AsQueryable();

            if(category != null && category != 0)
            {
                products = products.Where(x => x.Category.Id == category);
            }
           
            if (!String.IsNullOrEmpty(name))
            {
                products = products.Where(x => (x.Producer + x.Model + x.Description).ToLower().Contains(name.ToLower()));
            }
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    products = products.OrderByDescending(x => x.Producer + x.Model);
                    break;          
                case SortState.CategoryAsc:
                    products = products.OrderBy(x => x.Category.Name);
                    break;
                case SortState.CategoryDesc:
                    products = products.OrderByDescending(x => x.Category.Name);
                    break;         
                default:
                    products = products.OrderBy(x => x.Producer + x.Model);
                    break;
            }

            var count = products.Count();
            var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            IndexViewModel model = new IndexViewModel()
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(
                    unit.CategoryRepository.Get().ToList(),
                    category),
                Products = items
            };

            return View(model);
        }
    }
}