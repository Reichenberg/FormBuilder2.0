using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Team1_DynamicForms.DataProvider;

namespace Team1_DynamicForms.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        //Connection to DataProvider
        private FormsDataProvider db = new FormsDataProvider();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateForm()
        {
            return View();
        }

        public ActionResult CreateFormWorkFlow()
        {
            return View();
        }

        /// <summary>
        /// Adds a new form to the database
        /// </summary>
        /// <param name="formName">Name of the form to add</param>
        /// <param name="formHtml">Form Html</param>
        /// <param name="workflow">Specified workflow to be attached to the form</param>
        /// <returns>Json response indicating failure or success</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddForm(string formName, string formHtml, int workflow)
        {
            try
            {
                var fail = new { Success = "False", Message = "Form Cannot Be Empty." };
                var success = new { Success = "True", Message = "Form Successfully Added" };
                if (String.IsNullOrWhiteSpace(formHtml))
                {

                    return Json(fail);
                }

                string creatorAccount = User.Identity.GetUserName();

                if (db.AddFormToDb(formName, formHtml, workflow, creatorAccount))
                {
                    return Json(success);
                }
                else
                {
                    return Json(fail);
                }

            }
            catch(Exception e)
            {
                var exception = new { Success = "False", Message = e.Message };
                return Json(exception);
            }
           
        }
    }
}