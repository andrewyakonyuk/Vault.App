using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            // if (User.Identity.IsAuthenticated)
            //    return RedirectToAction(nameof(BoardsController.Index), "Boards");

            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}