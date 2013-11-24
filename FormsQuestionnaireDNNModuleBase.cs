/*
 * PackFlash http://www.packflash.com
 * Copyright (c) 2012 Hristo Evtimov
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN
{

    /// <summary>
    /// This base class can be used to define custom properties for multiple controls. 
    /// An example module, DNNSimpleArticle (http://dnnsimplearticle.codeplex.com) uses this for an ArticleId
    /// 
    /// Because the class inherits from PortalModuleBase, properties like ModuleId, TabId, UserId, and others, 
    /// are accessible to your module's controls (that inherity from FormsQuestionnaireDNNModuleBase
    /// 
    /// </summary>

    public class FormsQuestionnaireDNNModuleBase : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        //check settings and return whether the current user is allowed to file the form
        public bool UserCanFileForm(UserInfo User, FQDNN_Form Form)
        {
            CommonLogic oCommonLogic = new CommonLogic();
            List<FQDNN_FormSetting> Settings = Form.FQDNN_FormSetting.ToList();
            FormSettings oFormSettings = new FormSettings();
            bool bFormAccessIsFiltered = oFormSettings.GetAccessIsFiltered(Settings);
            bool result = !bFormAccessIsFiltered;

            string listOfUsersAbleToFileForm = oFormSettings.GetListOfUsersAbleToFileForm(Settings);
            string listOfUserRolesAbleToFileForm = oFormSettings.GetListOfUserRolesAbleToFileForm(Settings);
            string listOfUsersNotAbleToFileForm = oFormSettings.GetListOfUsersNotAbleToFileForm(Settings);
            string listOfUserRolesNotAbleToFileForm = oFormSettings.GetListOfUserRolesNotAbleToFileForm(Settings);

            if (bFormAccessIsFiltered)
            {
                //if there are any allow rules then user is not allowed by default and then check if the user matches the rules.
                if (!String.IsNullOrEmpty(listOfUsersAbleToFileForm) || !String.IsNullOrEmpty(listOfUserRolesAbleToFileForm))
                {
                    result = false;
                    if (User != null)
                    {
                        if (!String.IsNullOrEmpty(listOfUsersAbleToFileForm))
                        {
                            if (oCommonLogic.IntIsInList(User.UserID, listOfUsersAbleToFileForm))
                            {
                                result = true;
                            }
                        }
                        if (!String.IsNullOrEmpty(listOfUserRolesAbleToFileForm))
                        {
                            if (oCommonLogic.UserIsInListOfRoles(User, listOfUserRolesAbleToFileForm))
                            {
                                result = true;
                            }
                        }
                    }
                }

                if (result == false)
                {
                    return false;
                }

                if (!String.IsNullOrEmpty(listOfUsersNotAbleToFileForm) || !String.IsNullOrEmpty(listOfUserRolesNotAbleToFileForm))
                {
                    if (User != null)
                    {
                        if (!String.IsNullOrEmpty(listOfUsersNotAbleToFileForm))
                        {
                            if (oCommonLogic.IntIsInList(User.UserID, listOfUsersNotAbleToFileForm))
                            {
                                result = false;
                            }
                        }
                        if (!String.IsNullOrEmpty(listOfUserRolesNotAbleToFileForm))
                        {
                            if (oCommonLogic.UserIsInListOfRoles(User, listOfUserRolesNotAbleToFileForm))
                            {
                                result = false;
                            }
                        }
                    }
                }
            }

            bool bUserCanFileMultipleTimes = oFormSettings.GetUserCanFileMultipleTimes(Settings);
            if (!bUserCanFileMultipleTimes)
            {
                DataAccess oDataAccess = new DataAccess();
                
                FQDNN_FormFiling oPreviousFormFiling = oDataAccess.GetLastFiling(User.UserID, oCommonLogic.GetUserIPAddress(), Form.FormID);
                
                if (oPreviousFormFiling != null)
                {
                    result = false;

                    bool bUserCanResumeFiling = oFormSettings.GetUserCanResumeFiling(Settings);
                    int iUserCanResumeInHours = oFormSettings.GetUserCanResumeInHours(Settings);

                    if (bUserCanResumeFiling)
                    {
                        DateTime Now = DateTime.Now;

                        if (Now > oPreviousFormFiling.DateCreated)
                        {
                            int hoursSincePreviousFiling = (Now - oPreviousFormFiling.DateCreated).Hours;
                            if (iUserCanResumeInHours > hoursSincePreviousFiling)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public FQDNN_FormFiling GetCurrentFormFilingFromViewState(FQDNN_Form Form)
        {
            FQDNN_FormFiling FormFiling = null;
            if (ViewState["FormFiling_" + Form.FormID.ToString() + "_" + ModuleId.ToString()] != null)
            {
                FormFiling = (FQDNN_FormFiling)ViewState["FormFiling_" + Form.FormID.ToString() + "_" + ModuleId.ToString()];
            }
            return FormFiling;
        }

        public void SetCurrentFormFilingInViewState(FQDNN_FormFiling FormFiling, FQDNN_Form Form)
        {
            ViewState["FormFiling_" + Form.FormID.ToString() + "_" + ModuleId.ToString()] = FormFiling;
        }
    }
}
