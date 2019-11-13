using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Memberships.Extensions;

namespace Memberships.Controllers
{
    public class RegisterCodeController : Controller
    {
        // GET: RegisterCode
        public async Task<ActionResult> Register(string code)
        {
            if (Request.IsAuthenticated)
            {
                var userId = HttpContext.GetUserId();
                var registered = await SubscriptionExtensions.RegisterUserSubscriptionCode(userId, code);

                if(!registered) throw new ArgumentException();

                return PartialView("_RegisterCodePartial");
            }
            return View();
        }
    }
}