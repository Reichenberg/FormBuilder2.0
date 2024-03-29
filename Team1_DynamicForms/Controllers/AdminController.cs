﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Team1_DynamicForms.DataProvider;
using Team1_DynamicForms.Models;

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
            WorkflowsToApproveViewModel WFViewModel = new WorkflowsToApproveViewModel();

            WFViewModel.formsToApprove = db.GetSubmittedFormsForAdminReview();
            WFViewModel.usersWhoFilledForm = db.GetUsernamesOfSubmittedFormsForAdminReview(WFViewModel.formsToApprove);
            WFViewModel.nameOfFilledForm = db.GetNamesOfSubmittedFormsForAdminReview(WFViewModel.formsToApprove);

            return View(WFViewModel);
        }

        public ActionResult CreateForm()
        {
            return View();
        }

        /// <summary>
        /// Adds a new form to the database
        /// </summary>
        /// <param name="formName">Name of the form to add</param>
        /// <param name="formHtml">Form Html</param>
        /// <param name="workFlow">Specified workflow to be attached to the form</param>
        /// <returns>Json response indicating failure or success</returns>
        [HttpPost]
        public ActionResult AddForm(string formName, string formHtml, string workFlow)
        {

            try
            {

                formHtml = HttpUtility.UrlDecode(formHtml);
                var fail = new { Success = "False", Message = "Form Cannot Be Empty." };
                var workFlowFail = new { Success = "False", Message = "Workflow Emails Not Accounts" };
                var success = new { Success = "True", Message = "Form Successfully Added" };
                if (String.IsNullOrWhiteSpace(formHtml))
                {

                    return Json(fail);
                }

                int workFlowID = db.CreateAndAddWorkFlow(workFlow.Split(',').ToList());
                if (workFlowID < 0)
                {
                    return Json(workFlowFail);
                }
                else
                {
                string creatorAccount = User.Identity.GetUserName();

                    if (db.AddFormToDb(formName, formHtml, workFlowID, creatorAccount))
                {
                    return Json(success);
                }
                else
                {
                    return Json(fail);
                }
                }

            }
            catch(Exception e)
            {
                var exception = new { Success = "False", Message = e.Message };
                return Json(exception);
            }
           
        }

        public ActionResult Approve(int? id)
        {
            ApproveOrDenyViewModel ApproveOrDeny = new ApproveOrDenyViewModel();
            ApproveOrDeny.formData = db.GetFormDataToApproveOrDeny((int)id);
            ApproveOrDeny.accountWf = (int)id;

            return View(ApproveOrDeny);
        }

        [HttpPost]
        public ActionResult SubmitResult(int id, FormCollection collection)
        {
            if (collection["Submit"] == "Approve")
            {
                if(db.ApprovalOfForm(id) == 1)
                {
                    ViewBag.Text = "Form has been successfully approved";
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
                
            }
            else if (collection["Submit"] == "Deny")
            {
                if (db.DenialOfForm(id) == 1)
                {
                    ViewBag.Text = "Form has been successfully denied";
                    return View();
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