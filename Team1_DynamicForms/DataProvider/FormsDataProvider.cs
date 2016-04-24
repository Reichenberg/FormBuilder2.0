//----------------------------------------------------
//File Name: 			        FormsDataProvider.cc
//Project Name: 		        Team1_DynamicForms
//----------------------------------------------------
//Creator's Name and Email: 	Chance Reichenberg	Reichenberg@goldmail.etsu.edu
//Creation Date:		        2/4/2016
//----------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Team1_DynamicForms.DataRepository;
using Team1_DynamicForms.Models;

/// <summary>
/// Handles data checking and exceptions.
/// </summary>
namespace Team1_DynamicForms.DataProvider
{
    public class FormsDataProvider
    {
        //Connection to repository
        private FormsRepository db = new FormsRepository();

        /// <summary>
        /// Adds a new account to the database.
        /// </summary>
        /// <param name="name">Account Name</param>
        /// <param name="type">Account Type</param>
        /// <param name="userId">Id of account</param>
        /// <returns></returns>
        public bool AddAccount(string name, string type, string userId)
        {
            if(db.AddAccountToDb(name,type,userId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new form to the database
        /// </summary>
        /// <param name="formName">Name of the form to add</param>
        /// <param name="formHtml">Form Html</param>
        /// <param name="workflow">Specified workflow to be attached to the form</param>
        /// <returns>Json response indicating failure or success</returns>
        public bool AddFormToDb(String formName, String formHtml, int workflow, string creatorAccount)
        {
           // try {
                if (workflow < 1)
                {
                    workflow = 1;
                }

                db.InsertFormToDb(formName, formHtml, workflow, creatorAccount);

                return true;
          //  }
          //  catch(Exception e)
           // {
           //     throw (new Exception("Error adding form to database"));
           // }
        }

        /// <summary>
        /// Gets a form for a user to fill out
        /// </summary>
        /// <param name="wholeFormId">Id of selected form to fill out</param>
        /// <returns></returns>
        public FormPage GetFormPageToFillFromDb(int wholeFormId)
        {
            try {

                return db.GetFormToFill(wholeFormId);
            }
            catch (Exception e)
            {
                throw (new Exception("Error retrieving form page from database"));
            }
        }

        
        /// <summary>
        /// Fills HTML form code with user input
        /// </summary>
        /// <param name="collection">Form input</param>
        /// <param name="Html">String of raw HTML of the form</param>
        /// <returns></returns>
        public string createFilledHtml(FormCollection collection, string Html)
        {
            string newHtml = "";
            int currentFormElement = 0;
            int nextFormType = 0;
            bool readyForNextItem = false;

            foreach (string option in collection)
            {

                if (option == "Submit")
                {
                    break;
                }

                var item = collection[option];
                readyForNextItem = false;


                while (readyForNextItem == false)
                {
                    nextFormType = Html.IndexOf("type", currentFormElement);

                    int nextSelectType = Html.IndexOf("<select", currentFormElement);

                    if ((nextFormType < nextSelectType || nextSelectType < 0) && nextFormType != -1)
                    {
                        currentFormElement = nextFormType + 4;

                        string strFormType = Html.Substring(nextFormType, 15);

                        if (strFormType.Contains("text") || strFormType.Contains("time") || strFormType.Contains("date"))
                        {
                            int insertIndex = Html.IndexOf("value=\"\"", nextFormType);
                            if (insertIndex > 0)
                            {
                                newHtml = Html.Insert(insertIndex + "value=\"".Length, item.ToString());
                                readyForNextItem = true;
                            }
                        }
                        else if (strFormType.Contains("radio"))
                        {
                            if (option.Contains("radio"))
                            {
                                int insertIndex = Html.IndexOf(item.ToString(), currentFormElement);
                                if (insertIndex > 0)
                                {
                                    int checkName = Html.IndexOf("name=\"", currentFormElement);
                                    string checkStringName = Html.Substring(checkName + "name=\"".Length, 7);

                                    if (checkStringName.Contains(option))
                                    {
                                        newHtml = Html.Insert(insertIndex + item.ToString().Length + 1, " checked ");
                                        readyForNextItem = true;
                                    }
                                }
                            }
                        }
                        else if (strFormType.Contains("checkbox"))
                        {
                            if (option.ToLower().Contains("checkbox"))
                            {
                                int insertIndex = Html.IndexOf("value=\"\"", nextFormType);
                                if (insertIndex > 0)
                                {
                                    newHtml = Html.Insert(insertIndex + "value=\"\"".Length, " checked ");
                                    readyForNextItem = true;
                                }
                            }
                        }

                    }
                    else if (nextSelectType < nextFormType || nextFormType < 0)
                    {
                        currentFormElement = nextSelectType + 7;

                        if (item.ToString() != "")
                        {
                            int insertSelectedIndex = Html.IndexOf(collection[option].ToString(), currentFormElement);
                            if (insertSelectedIndex > 0)
                            {
                                newHtml = Html.Insert(insertSelectedIndex + collection[option].ToString().Length + 1, " selected=\"selected\" ");
                                readyForNextItem = true;
                            }
                        }
                        else
                        {
                            readyForNextItem = true;
                        }
                    }

                    if (newHtml != "")
                    {
                        Html = newHtml;
                    }
                }
            }
            return newHtml;
        }

        /// <summary>
        /// Creates a new form submission to save or submit a form
        /// </summary>
        /// <param name="formPageId"></param>
        /// <param name="userId"></param>
        /// <param name="collection"></param>
        /// <param name="finished"></param>
        /// <returns></returns>
        public bool createAndAddFormSubmission(int formPageId, string userId, FormCollection collection, string finished)
        {
            FormPage formPage = db.GetPage(formPageId);
            string Html = formPage.HtmlCode;

            string newHtml = createFilledHtml(collection, Html);

            db.AddFormSubmission(formPage.Id,userId, newHtml, finished);

            return true;
        }

        /// <summary>
        /// Returns a submission part based on it's submission whole id
        /// </summary>
        /// <param name="submissionWholeId"></param>
        /// <returns></returns>
        public SubmissionPart GetSubmissionPart(int submissionWholeId)
        {
            try {
                return db.getSubmissionPart(submissionWholeId);
            }
            catch (Exception e)
            {

                throw (new Exception("Error retrieving saved form page from database"));
            }
        }

        /// <summary>
        /// Updates a form submission to be saved or submitted.
        /// </summary>
        /// <param name="submissionWholeId"></param>
        /// <param name="collection"></param>
        /// <param name="finished"></param>
        /// <returns></returns>
        public bool UpdateUserSavedForm(int submissionWholeId, FormCollection collection, string finished)
        {
            FormPage formPage = db.GetPageFromSubmissionWhole(submissionWholeId);
            string Html = formPage.HtmlCode;

            string newHtml = createFilledHtml(collection,Html);

            if(db.UpdateSubmissionWhole(submissionWholeId, newHtml, finished))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the set of all workflows for current user
        /// TODO: add ability to group workflows retrieved by type
        /// </summary>
        /// <param name="userId">The id of the user to look for</param>
        /// <returns>Workflows obtained from database</returns>
        internal WorkFlowGroupViewModel GetWorkFlows(Account user)
        {
            try
            {
                return db.GetWorkFlows(user);
            }
            catch (Exception e)
            {
                throw (new Exception("Error retrieving workflows from database"));
            }
        }

        /// <summary>
        /// Gets the names and ids of the forms created by a specific user.
        /// </summary>
        /// <param name="user">The user to look up the forms for</param>
        /// <returns></returns>
        internal WorkFlowCreateIndexViewModel GetFormNamesAndIDs(Account user)
        {
            try
            {
                WorkFlowCreateIndexViewModel viewModel = new WorkFlowCreateIndexViewModel();

                var Forms = db.GetCreatedFormsDict(user);
                viewModel.Forms = Forms.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value });

                return viewModel;
            }
            catch (Exception e)
            {
                throw (new Exception("Error retrieving Workflows for current user from database"));
            }
        }

        //should retrieve a wholeform for a user to view
        // the id number will be used to generate a link to the actual form page
        // wholeFormId used is the id of the form to be retrieved
        public List<WholeForm> GetWholeFormFromDb()
        {
            //try
            //{
                return db.GetWholeForm();
            //}
            //catch (Exception e)
            //{
            //    throw (new Exception("Error retrieving form page from database."));
            //}
        }

        //should retrieve a submitted form for a user to view
        // userId is used to make sure that the forms submitted will have the correct user
            // forms attached to it
        // submissionFormId is the id used to generate the link for the page
        public List<SubmissionWhole> GetSubmittedFormFromDb(string userName)
        {
           // try
            //{
                return db.GetSubmittedForm(userName);
            //}
            //catch (Exception e)
            //{
             //   throw (new Exception("Error retrieving submitted form from database."));
            //}
        }
   

        /// <summary>
        /// Creates and adds a new workflow to the database based on the given list of emails
        /// </summary>
        /// <param name="userEmails">The users to add to the workflow</param>
        /// <returns>new workFlows id, negative value if operation failed</returns>
        internal int CreateAndAddWorkFlow(List<string> userEmails)
        {

            //Creates workflow with empty type, TODO implement workflow types
            int workFlow;

            //Get user ids, before doing anything else, prevents junk workflows from being made
            List<int> users = db.GetUsersByEmail(userEmails);
            
            //Check that all emails have associated accounts.
            if (users.Count != userEmails.Count)
            {
                return -1;
            }
            try
            {
                workFlow = db.CreateAndInsertWorkFlow("Standard");
            }
            catch (Exception e)
            {
                throw new Exception("Error creating workflow");
            }

            //Add users and create AccountWorkflow table entries.
            try
            {
                if (db.AddUsersToWorkFlow(users, workFlow))
                {
                    return workFlow;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                throw (new Exception("Error adding user to workflow"));
            }
        }

        /// <summary>
        /// Adds a workFlow to an existing form
        /// </summary>
        /// <param name="formId">Id of the form being modified</param>
        /// <param name="workFlowId">Id of the workflow being added</param>
        /// <returns>Bool representing succes</returns>
        internal bool AddWorkFlowToForm(int formId, int workFlowId)
        {
            db.AddWorkFlowToForm(formId, workFlowId);
            return true;
        }

        /// <summary>
        /// Gets the currently logged in account and returns the formsDB version of that account
        /// </summary>
        /// <returns>Currently logged in account</returns>
        internal Account GetCurrentAccount()
        {
            return db.GetCurrentAccount();
        }

        /// <summary>
        /// Gets a form's name based on the id
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        internal string GetFormName(int formId)
        {
            return db.GetFormName(formId);
        }







        public List<AccountWorkflow> GetSubmittedFormsForAdminReview()
        {
            Account admin = GetCurrentAccount();
            return db.GetSubmittedFormsForApproval(admin);
        }

        public List<string> GetUsernamesOfSubmittedFormsForAdminReview(List<AccountWorkflow> workflows)
        {
            List<string> usernames = new List<string>();

            foreach(var form in workflows)
            {
                usernames.Add(db.GetUserWhoFilledFormFromAccountWorkflow(form.Id));
            }
            return usernames;
        }

        public List<string> GetNamesOfSubmittedFormsForAdminReview(List<AccountWorkflow> workflows)
        {
            List<string> formNames = new List<string>();

            foreach (var form in workflows)
            {
                formNames.Add(db.GetNameOFFilledFormFromAccountWorkflow(form.Id));
            }
            return formNames;
        }


        public string GetFormDataToApproveOrDeny(int accWorkflowId)
        {
            return db.GetFilledFormData(accWorkflowId);
        }


        public AccountWorkflow GetAccForkflow(int id)
        {
            return db.GetAccountForkflowFromId(id);
        }


        public int ApprovalOfForm(int id)
        {
            if (db.ApproveForm(id) == 1)
            {
                return 1;
            }
            return 0;

        }

        public int DenialOfForm(int id)
        {
            if (db.DenyForm(id) == 1)
            {
                return 1;
            }
            return 0;

        }





    }

   
}
