using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components
{
    public class FormSettings
    {
        public enum FormSettingsKeys
        {
            FormAccessIsFiltered = 0,
            ListOfUsersAbleToFileForm = 1,
            ListOfUserRolesAbleToFileForm = 2,
            ListOfUsersNotAbleToFileForm = 3,
            ListOfUserRolesNotAbleToFileForm = 4,
            FormRedirectAction = 5,
            RedirectUrlAfterFiling = 6,
            RedirectTabAfterFiling = 7,
            SendEmailToAdminAfterFormFiling = 8,
            SendEmailToUserAfterFormFiling = 9,
            EmailTemplateForAdmin = 10,
            EmailTemplateForUser = 11,
            UserCanNavigateBackInTheForm = 12,
            FilingFormUserCanResume = 13,
            FilingFormUserCanResumeForThisManyHours = 14,
            FilingFormUserCanFileMultipleTimes = 15,
        }

        public enum FormRedirectActions
        {
            RedirectToUrl = 0,
            RedirectToTab = 1,
            LastPageIsFinal = 2,
            GenericFinishedMessage = 3,
        }

        bool defaultFormAccessIsFiltered = false;
        bool defaultUserCanNavigateBackInTheForm = true;
        FormRedirectActions defaultFormRedirectActions = FormRedirectActions.GenericFinishedMessage;
        string defailtRedirectUrlAfterFIling = "";
        int defaultRedirectTabAfterFiling = -1;
        bool defaultUserCanResumeFiling = true;
        int defaultUserCanResumeInHours = 24;
        bool defaultUserCanFileMultipleTimes = false;
        string defaultListOfUsersAbleToFileForm = "";
        string defaultListOfUserRolesAbleToFileForm = "";
        string defaultListOfUsersNotAbleToFileForm = "";
        string defaultListOfUserRolesNotAbleToFileForm = "";
        
        //return what should happen after the fomr is submitted
        public FormRedirectActions GetRedirectAction(List<FQDNN_FormSetting> oSettings)
        {
            string formRedirectAction = GetSetting(oSettings, FormSettingsKeys.FormRedirectAction);
            FormRedirectActions result = defaultFormRedirectActions;

            if (!String.IsNullOrEmpty(formRedirectAction))
            {
                result = (FormRedirectActions)Enum.Parse(typeof(FormRedirectActions), formRedirectAction);
            }

            return result;
        }

        //return whether navigating back is allowed
        public bool UserCanNavigateBack(List<FQDNN_FormSetting> oSettings)
        {
            bool bUserCanNavigateBackInTheForm = defaultUserCanNavigateBackInTheForm;
            string userCanNavigateBackInTheForm = GetSetting(oSettings, FormSettings.FormSettingsKeys.UserCanNavigateBackInTheForm);
            if (!string.IsNullOrEmpty(userCanNavigateBackInTheForm))
            {
                if (!Boolean.TryParse(userCanNavigateBackInTheForm, out bUserCanNavigateBackInTheForm))
                {
                    bUserCanNavigateBackInTheForm = defaultUserCanNavigateBackInTheForm;
                }
            }
            return bUserCanNavigateBackInTheForm;
        }

        public string GetRedirectUrlAfterFiling(List<FQDNN_FormSetting> oSettings)
        {
            string result = defailtRedirectUrlAfterFIling;

            string sRedirectUrlAfterFiling = GetSetting(oSettings, FormSettings.FormSettingsKeys.RedirectUrlAfterFiling);
            if (!String.IsNullOrEmpty(sRedirectUrlAfterFiling))
            {
                result = sRedirectUrlAfterFiling;
            }

            return result;
        }

        public int GetRedirectTabAfterFiling(List<FQDNN_FormSetting> oSettings)
        {
            int result = defaultRedirectTabAfterFiling;

            string sRedirectTabAfterFiling = GetSetting(oSettings, FormSettings.FormSettingsKeys.RedirectTabAfterFiling);
            if (!String.IsNullOrEmpty(sRedirectTabAfterFiling))
            {
                if (!Int32.TryParse(sRedirectTabAfterFiling, out result))
                {
                    result = defaultRedirectTabAfterFiling;
                }
            }

            return result;
        }

        public bool GetAccessIsFiltered(List<FQDNN_FormSetting> oSettings)
        {
            bool bFormAccessIsFiltered = defaultFormAccessIsFiltered;
            string formAccessIsFiltered = GetSetting(oSettings, FormSettingsKeys.FormAccessIsFiltered);
            if (!String.IsNullOrEmpty(formAccessIsFiltered))
            {
                if (!Boolean.TryParse(formAccessIsFiltered, out bFormAccessIsFiltered))
                {
                    bFormAccessIsFiltered = defaultFormAccessIsFiltered;
                }
            }

            return bFormAccessIsFiltered;
        }

        public string GetListOfUsersAbleToFileForm(List<FQDNN_FormSetting> oSettings)
        {
            string result = defaultListOfUsersAbleToFileForm;
            string listOfUsersAbleToFileForm = GetSetting(oSettings, FormSettingsKeys.ListOfUsersAbleToFileForm);
            if (!String.IsNullOrEmpty(listOfUsersAbleToFileForm))
            {
                result = listOfUsersAbleToFileForm;
            }
            return result;
        }

        public string GetListOfUserRolesAbleToFileForm(List<FQDNN_FormSetting> oSettings)
        {
            string result = defaultListOfUserRolesAbleToFileForm;
            string listOfUserRolesAbleToFileForm = GetSetting(oSettings, FormSettingsKeys.ListOfUserRolesAbleToFileForm);
            if (!String.IsNullOrEmpty(listOfUserRolesAbleToFileForm))
            {
                result = listOfUserRolesAbleToFileForm;
            }
            return result;
        }

        public string GetListOfUsersNotAbleToFileForm(List<FQDNN_FormSetting> oSettings)
        {
            string result = defaultListOfUsersNotAbleToFileForm;
            string listOfUsersNotAbleToFileForm = GetSetting(oSettings, FormSettingsKeys.ListOfUsersNotAbleToFileForm);
            if (!String.IsNullOrEmpty(listOfUsersNotAbleToFileForm))
            {
                result = listOfUsersNotAbleToFileForm;
            }
            return result;
        }

        public string GetListOfUserRolesNotAbleToFileForm(List<FQDNN_FormSetting> oSettings)
        {
            string result = defaultListOfUserRolesNotAbleToFileForm;
            string listOfUserRolesNotAbleToFileForm = GetSetting(oSettings, FormSettingsKeys.ListOfUserRolesNotAbleToFileForm);
            if (!String.IsNullOrEmpty(listOfUserRolesNotAbleToFileForm))
            {
                result = listOfUserRolesNotAbleToFileForm;
            }
            return result;
        }

        public bool GetUserCanResumeFiling(List<FQDNN_FormSetting> oSettings)
        {
            bool bUserCanResumeFiling = defaultUserCanResumeFiling;
            string sUserCanResumeFiling = GetSetting(oSettings, FormSettingsKeys.FilingFormUserCanResume);

            if (!String.IsNullOrEmpty(sUserCanResumeFiling))
            {
                if (!Boolean.TryParse(sUserCanResumeFiling, out bUserCanResumeFiling))
                {
                    bUserCanResumeFiling = defaultUserCanResumeFiling;
                }
            }
            return bUserCanResumeFiling;
        }

        public int GetUserCanResumeInHours(List<FQDNN_FormSetting> oSettings)
        {
            int iUserCanResumeInHours = defaultUserCanResumeInHours;
            string sUserCanResumeInHours = GetSetting(oSettings, FormSettingsKeys.FilingFormUserCanResumeForThisManyHours);

            if (!String.IsNullOrEmpty(sUserCanResumeInHours))
            {
                if (!Int32.TryParse(sUserCanResumeInHours, out iUserCanResumeInHours))
                {
                    iUserCanResumeInHours = defaultUserCanResumeInHours;
                }
            }

            return iUserCanResumeInHours;
        }

        public bool GetUserCanFileMultipleTimes(List<FQDNN_FormSetting> oSettings)
        {
            bool bUserCanFileMultipleTimes = defaultUserCanFileMultipleTimes;
            string sUserCanFileMultipleTimes = GetSetting(oSettings, FormSettingsKeys.FilingFormUserCanFileMultipleTimes);

            if (!String.IsNullOrEmpty(sUserCanFileMultipleTimes))
            {
                if (!Boolean.TryParse(sUserCanFileMultipleTimes, out bUserCanFileMultipleTimes))
                {
                    bUserCanFileMultipleTimes = defaultUserCanFileMultipleTimes;
                }
            }

            return bUserCanFileMultipleTimes;
        }

        private string GetSetting(List<FQDNN_FormSetting> oSettings, FormSettingsKeys key)
        {
            string result = null;

            if (oSettings != null)
            {
                foreach (FQDNN_FormSetting Setting in oSettings)
                {
                    if (Setting.SettingKey == key.ToString())
                    {
                        result = Setting.SettingValue;
                        break;
                    }
                }
            }

            return result;
        }
    }
}