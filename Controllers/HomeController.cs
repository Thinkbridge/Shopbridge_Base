using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineStore.Models.Database;
using OnlineStore.Models;
using OnlineStore.Data;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UnitOfWork unit = new UnitOfWork();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
     
        public IActionResult Index(int? category, int page = 1, SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 3;
            var products = unit.ProductRepository.Get(includeProperties: "Image,Rates,Category").AsQueryable();

              // filter Based on category selected 
            if (category != null && category != 0)
            {
                products = products.Where(x => x.Category.Id == category);
            }        
               // Sorting Logic
          
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
               // Paging Logic
            var count = products.Count();
            var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            IndexViewModel model = new IndexViewModel()
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(
                    unit.CategoryRepository.Get().ToList(),
                    category
                    ),
                Products = items
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        //public IActionResult Index1([FromQuery(Name="page")]int page,
        //                            [FromQuery(Name = "sort")]string sort,
        //                            [FromQuery(Name = "filter")]string filter)
        //{
        //    ViewData["Categories"] = unit.CategoryRepository.Get();
        //    var products =  unit.ProductRepository.Get(includeProperties: "Image,Comments,Rates,Category");
        //    ViewData["prod"] = products;
        //    return View(products);
        //}
    }
}
