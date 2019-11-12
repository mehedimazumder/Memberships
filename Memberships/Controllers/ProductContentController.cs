using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Memberships.Models;

namespace Memberships.Controllers
{
    public class ProductContentController : Controller
    {
        // GET: ProductContent
        public async Task<ActionResult> Index(int id)
        {
            var model = new ProductSectionModel
            {
                Title = "C#",
                Sections = new List<ProductSection>()
            };
            return View(model);
        }
    }
}