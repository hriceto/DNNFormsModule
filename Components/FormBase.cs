using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data;
using System.Reflection;
using System.Web.UI.WebControls;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components
{
    public class FormBase : FormsQuestionnaireDNNModuleBase
    {
        int GenericMessagePageNumber = 1000000;

        private int PageNumber
        {
            get
            {
                int result = 1;
                if (ViewState["PageNumber_" + ModuleId.ToString()] != null)
                {
                    Int32.TryParse(ViewState["PageNumber_" + ModuleId.ToString()].ToString(), out result);
                }
                return result;
            }
            set
            {
                ViewState["PageNumber_" + ModuleId.ToString()] = value;
            }
        }

        public int TotalPages
        {
            get
            {
                int result = 1;
                if (ViewState["TotalPages_" + ModuleId.ToString()] != null)
                {
                    Int32.TryParse(ViewState["TotalPages_" + ModuleId.ToString()].ToString(), out result);
                }
                return result;
            }
            set
            {
                ViewState["TotalPages_" + ModuleId.ToString()] = value;
            }
        }

        //current form id
        public int FormID
        {
            get
            {
                int result = -1;
                if (ViewState["FormID_" + ModuleId.ToString()] != null)
                {
                    Int32.TryParse(ViewState["FormID_" + ModuleId.ToString()].ToString(), out result);
                }
                return result;
            }
            set
            {
                ViewState["FormID_" + ModuleId.ToString()] = value;
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            SetupForm();
        }

        private void SetupForm()
        {
            DataAccess oDataAccess = new DataAccess();
            FQDNN_Form oForm = oDataAccess.GetFullForm(FormID);

            FQDNN_FormPage oCurrentPage = null;

            TotalPages = oForm.FQDNN_FormPage.Count;

            //find current page
            foreach (FQDNN_FormPage oFormPage in oForm.FQDNN_FormPage)
            {
                this.FindControl(oFormPage.GetControlID()).Visible = false;

                if (oFormPage.PageNumber == PageNumber)
                {
                    oCurrentPage = oFormPage;
                }
            }

            if (oCurrentPage != null)
            {
                SetupPage(oForm, oCurrentPage);
            }

            //display or hide the submit button
            this.FindControl(oForm.GetSubmitButtonControlID()).Visible = false;
            if (PageNumber == TotalPages)
            {
                FormSettings oFormSettings = new FormSettings();
                if (oFormSettings.GetRedirectAction(oForm.FQDNN_FormSetting.ToList()) != FormSettings.FormRedirectActions.LastPageIsFinal)
                {
                    this.FindControl(oForm.GetSubmitButtonControlID()).Visible = true;
                    ((Button)this.FindControl(oForm.GetSubmitButtonControlID())).Click += SubmitForm;
                }
            }

            //display final generic message if there is not redirect or last page
            System.Web.UI.Control GenericMessageControl = this.FindControl(oForm.GetGenericMessageControlID());
            if (GenericMessageControl != null)
            {
                GenericMessageControl.Visible = false;
                if (PageNumber == GenericMessagePageNumber)
                {
                    GenericMessageControl.Visible = true;
                }
            }
        }

        private void SetupPage(FQDNN_Form oForm, FQDNN_FormPage oCurrentPage)
        {
            this.FindControl(oCurrentPage.GetControlID()).Visible = true;

            foreach (FQDNN_FormControl oPageControl in oCurrentPage.FQDNN_FormControl)
            {
                SetupPageControl(oPageControl);
            }

            //set previous and next page button's visibility
            FormSettings oFormSettings = new FormSettings();
            bool bUserCanNavigateBackInTheForm = oFormSettings.UserCanNavigateBack(oForm.FQDNN_FormSetting.ToList());

            if (bUserCanNavigateBackInTheForm)
            {
                ((Button)this.FindControl(oCurrentPage.GetPreviousPageControlID())).Click += PreviousPage;
            }
            ((Button)this.FindControl(oCurrentPage.GetNextPageControlID())).Click += NextPage;

            if (PageNumber == 1 || !bUserCanNavigateBackInTheForm)
            {
                this.FindControl(oCurrentPage.GetPreviousPageControlID()).Visible = false;
            }
            if (PageNumber == TotalPages)
            {
                this.FindControl(oCurrentPage.GetNextPageControlID()).Visible = false;
            }
        }

        private void SetupPageControl(FQDNN_FormControl oPageControl)
        {
            //TODO: MAYBE TRY TO LOAD FROM DATABASE FIRST(PREVIOUSLY ENTERED VALUES IN CASE OF PAGING) AND IF THAT DOES NOT WORK LOAD DEFAULTS.
            System.Web.UI.Control control = this.FindControl(oPageControl.GetControlID());

            if (control != null)
            {
                Type type = control.GetType();

                if (oPageControl.DataSourceType == (int)FQDNN_FormControl.DataSourceTypes.Dynamic)
                {
                    if (type.IsSubclassOf(typeof(ListControl)))
                    {
                        
                    }
                }                

                //load the default value in the control
                if (!String.IsNullOrEmpty(oPageControl.DefaultValue))
                {
                    PropertyInfo valuePropery = type.GetProperty(oPageControl.FQDNN_ControlDefinition.ValuePropertyName);
                    if (valuePropery != null)
                    {
                        object defaulValue = Convert.ChangeType(oPageControl.DefaultValue, valuePropery.PropertyType);
                        valuePropery.SetValue(control, defaulValue, null);
                    }
                }
            }
        }

        protected void PreviousPage(object sender, EventArgs e)
        {
            SavePage();
            PageNumber = PageNumber - 1;
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }
            SetupForm();
        }

        protected void NextPage(object sender, EventArgs e)
        {
            SavePage();
            PageNumber = PageNumber + 1;
            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }
            SetupForm();
        }

        protected void SubmitForm(object sender, EventArgs e)
        {
            SavePage();

            DataAccess oDataAccess = new DataAccess();
            FQDNN_Form oForm = oDataAccess.GetFullForm(FormID);

            FormSettings oFormSettings = new FormSettings();
            FormSettings.FormRedirectActions redirection = oFormSettings.GetRedirectAction(oForm.FQDNN_FormSetting.ToList());

            //perform different redirects after submitting form
            if (redirection == FormSettings.FormRedirectActions.RedirectToUrl)
            {
                string redirectUrl = oFormSettings.GetRedirectUrlAfterFiling(oForm.FQDNN_FormSetting.ToList());
                if(!String.IsNullOrEmpty(redirectUrl))
                {
                    Response.Redirect(redirectUrl);
                }
                else
                {
                    redirection = FormSettings.FormRedirectActions.GenericFinishedMessage;
                }
            }
            if (redirection == FormSettings.FormRedirectActions.RedirectToTab)
            {
                int redirectTabID = oFormSettings.GetRedirectTabAfterFiling(oForm.FQDNN_FormSetting.ToList());
                if (redirectTabID > 0)
                {
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(redirectTabID));
                }
                else
                {
                    redirection = FormSettings.FormRedirectActions.GenericFinishedMessage;
                }
            }
            if (redirection == FormSettings.FormRedirectActions.GenericFinishedMessage)
            {
                PageNumber = GenericMessagePageNumber;
                SetupForm();
            }
        }

        private void SavePage()
        {
            DataAccess oDataAccess = new DataAccess();
            FQDNN_Form oForm = oDataAccess.GetFullForm(FormID);

            FQDNN_FormPage oCurrentPage = null;

            foreach (FQDNN_FormPage oFormPage in oForm.FQDNN_FormPage)
            {
                if (oFormPage.PageNumber == PageNumber)
                {
                    oCurrentPage = oFormPage;
                    break;
                }
            }

            if (oCurrentPage != null)
            {
                FQDNN_FormFiling oFormFiling = GetCurrentFormFilingFromViewState(oForm);
                if (oFormFiling == null)
                {
                    CommonLogic oCommonLogic = new CommonLogic();
                    oFormFiling = oDataAccess.InsertNewFiling(Request.Browser.Browser, oCommonLogic.GetUserIPAddress(), false, Request.Browser.Platform, UserId, oForm.FormID);
                }

                if (oFormFiling != null)
                {
                    //loop through each control on page and save the result.
                    List<FQDNN_FormFilingRecord> oNewFilingRecords = new List<FQDNN_FormFilingRecord>();
                    foreach (FQDNN_FormControl oFormControl in oCurrentPage.FQDNN_FormControl)
                    {
                        System.Web.UI.Control control = this.FindControl(oFormControl.GetControlID());
                        if (control != null)
                        {
                            Type type = control.GetType();

                            FQDNN_FormFilingRecord oNewFilingRecord = new FQDNN_FormFilingRecord();
                            oNewFilingRecord.FormControlID = oFormControl.FormControlID;
                            oNewFilingRecord.FormPageID = oCurrentPage.FormPageID;
                            
                            PropertyInfo valuePropery = type.GetProperty(oFormControl.FQDNN_ControlDefinition.ValuePropertyName);
                            if (valuePropery != null)
                            {
                                oNewFilingRecord.Value = valuePropery.GetValue(control, null).ToString();
                            }
                            oNewFilingRecords.Add(oNewFilingRecord);
                        }
                    }
                    oDataAccess.InsertFilingRecords(oNewFilingRecords, oFormFiling.FormFilingID);
                }
            }
        }
    }
}