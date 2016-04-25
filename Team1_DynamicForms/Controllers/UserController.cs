using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Team1_DynamicForms.DataProvider;
using Team1_DynamicForms.Models;

namespace Team1_DynamicForms.Controllers
{
    public class UserController : Controller
    {
        //Connection to data provider
        private FormsDataProvider db = new FormsDataProvider();

        // GET: User  
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // creates an item usint the FormsAndSubmissionsViewModel to allow the user to
                // view and access submitted and unsubmitted forms
                string userName = User.Identity.Name;


                FormsAndSubmissionsViewModel formsubmview = new FormsAndSubmissionsViewModel();

                formsubmview.submittedForms = db.GetSubmittedFormFromDb(userName);
                formsubmview.unsubmittedForms = db.GetWholeFormFromDb();
                formsubmview.Statuses = db.GetFormApprovalStatuses();
                formsubmview.FormNames = db.GetFormNamesForFormsInWorkflow();

                return View(formsubmview);
            }
            return RedirectToAction("../Account/Login");
        }

        /// <summary>
        /// Gets a new/saved form from database ans display it to the user
        /// </summary>
        /// <param name="saved">Value determines if form is a saved form or new form</param>
        /// <param name="id">Id of form/saved form to fill</param>
        /// <returns>View model containing html and id of formpage/submissionPart</returns>
        public ActionResult Fill(int? saved, int? id)
        {
            if (id == null || saved == null || saved != 0 && saved != 1)
            {
                return HttpNotFound();
            }

            FormDisplayView formCodeToDisplay = new FormDisplayView();
            if (saved == 0)
            {
                FormPage formPage = db.GetFormPageToFillFromDb((int)id);
                if (formPage == null)
                {
                    return HttpNotFound();
                }
                formCodeToDisplay.idForCode = formPage.Id;
                formCodeToDisplay.htmlOfPage = formPage.HtmlCode;
                formCodeToDisplay.saved = (int)saved;
                //TEMPORARY THIS SHOULD BE IN A VIEW MODEL FOR THE PAGE, along with the other info
                ViewBag.FormName = formPage.WholeForm.Name;
            }
            else if (saved == 1)
            {
                SubmissionPart partFilledFormPage = db.GetSubmissionPart((int)id);
                if (partFilledFormPage == null)
                {
                    return HttpNotFound();
                }

                formCodeToDisplay.idForCode = partFilledFormPage.Id;
                formCodeToDisplay.htmlOfPage = partFilledFormPage.HtmlCode;
                formCodeToDisplay.saved = (int)saved;
                //TEMPORARY THIS SHOULD BE IN A VIEW MODEL FOR THE PAGE, along with the other info
                ViewBag.FormName = partFilledFormPage.SubmissionWhole.FormSubmission.WholeForm.Name;
            }

             return View(formCodeToDisplay);
        }

        /// <summary>
        /// Submits or saves a form
        /// </summary>
        /// <param name="id">Id of formpage or submissionPart</param>
        /// <param name="collection">Form data input from user</param>
        /// <returns>Json response indicating failure or view of submission confirmation page</returns>
        [HttpPost]
        public ActionResult Submit(int saved, int id, FormCollection collection)
        {
            string userAccount = User.Identity.Name;

            if (collection["Submit"] == "Submit")
            {
                if (saved == 0)
                {
                    if (db.createAndAddFormSubmission(id, userAccount, collection, "Yes"))
                    {
                        ViewBag.Text = "Form has been successfully submitted";
                        return View();
                    }
                    else
                    {
                        return HttpNotFound();
                    } 
                }
                else if(saved == 1)
                {
                    if (db.UpdateUserSavedForm(id, collection, "Yes"))
                    {
                        ViewBag.Text = "Form has been successfully submitted";
                        return View();
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else if (collection["Submit"] == "Save Progress")
            {
                if (saved == 0)
                {
                    if (db.createAndAddFormSubmission(id, userAccount, collection, "No"))
                    {
                        ViewBag.Text = "Form has been successfully Saved";
                        return View();
                    }
                    else
                    {
                        return HttpNotFound();
                    } 
                }
                else if (saved == 1)
                {
                    if (db.UpdateUserSavedForm(id, collection, "No"))
                    {
                        ViewBag.Text = "Form has been successfully submitted";
                        return View();
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return HttpNotFound();
        }
    }
}