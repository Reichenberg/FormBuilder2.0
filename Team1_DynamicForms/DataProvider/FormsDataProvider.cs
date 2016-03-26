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

        
        public bool createAndAddFormSubmission(int formPageId, string userId, FormCollection collection, string finished)
        {
            FormPage formPage = db.GetPage(formPageId);
            string Html = formPage.HtmlCode;
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

                    if (nextFormType < nextSelectType || nextSelectType < 0)
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
                                    newHtml = Html.Insert(insertIndex + item.ToString().Length + 1, " checked ");
                                    readyForNextItem = true;
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

                    Html = newHtml;
                }
            }

            db.AddFormSubmission(formPage.Id,userId,Html,finished);

            return true;
        }

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

        public bool UpdateUserSavedForm(int submissionWholeId, FormCollection collection, string finished)
        {
            FormPage formPage = db.GetPageFromSubmissionWhole(submissionWholeId);
            string Html = formPage.HtmlCode;
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

                    if (nextFormType < nextSelectType || nextSelectType < 0)
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
                                    newHtml = Html.Insert(insertIndex + item.ToString().Length + 1, " checked ");
                                    readyForNextItem = true;
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

                    Html = newHtml;
                }
            }

            if(db.UpdateSubmissionWhole(submissionWholeId,Html,finished))
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

        //should retrieve a wholeform for a user to view
        // the id number will be used to generate a link to the actual form page
        // wholeFormId used is the id of the form to be retrieved
        public WholeForm GetWholeFormFromDb(int wholeFormId)
        {
            try
            {
                return db.GetWholeForm(wholeFormId);
            }
            catch (Exception e)
            {
                throw (new Exception("Error retrieving form page from database."));
            }
        }

        //should retrieve a submitted form for a user to view
        // userId is used to make sure that the forms submitted will have the correct user
            // forms attached to it
        // submissionFormId is the id used to generate the link for the page
        public SubmissionWhole GetSubmittedFormFromDb(int submissionFormId, int userId)
        {
            try
            {
                return db.GetSubmittedForm(submissionFormId, userId);
            }
            catch (Exception e)
            {
                throw (new Exception("Error retrieving submitted form from database."));
            }
        }
    }
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
            try {
                workFlow = db.CreateAndInsertWorkFlow("Standard");
            }
            catch (Exception e)
            {
                throw new Exception("Error creating workflow");
            }

            try {
                if (db.AddUsersToWorkFlow(userEmails, workFlow))
                {
                    return workFlow;
                }
            }
            catch (Exception e)
            {
                throw (new Exception("Error adding user to workflow"));
            }
            return -1;
        }

        /// <summary>
        /// Adds a workFlow to an existing form
        /// </summary>
        /// <param name="formId">Id of the form being modified</param>
        /// <param name="workFlowId">Id of the workflow being added</param>
        internal void AddWorkFlowToForm(int formId, int workFlowId)
        {
            db.AddWorkFlowToForm(formId, workFlowId);
        }

        /// <summary>
        /// Gets the currently logged in account and returns the formsDB version of that account
        /// </summary>
        /// <returns></returns>
        internal Account GetCurrentAccount(string userId)
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
    }

   
}
