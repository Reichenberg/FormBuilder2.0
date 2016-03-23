using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Team1_DynamicForms.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Redirect a logged in user to the appropriate view
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if(User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "SuperAdmin");
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
    }
}