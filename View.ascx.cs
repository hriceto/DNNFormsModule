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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components;
using System.Collections.Generic;
using System.Linq;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from FormsQuestionnaireDNNModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : FormsQuestionnaireDNNModuleBase, IActionable
    {

        #region Event Handlers

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// -----------------------------------------------------------------------------
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //check settings to decide which form to load
                if(Settings["FormGUID"] != null)
                {
                    DataAccess oDataAccess = new DataAccess();
                    FQDNN_Form Form = oDataAccess.GetActiveForm(Settings["FormGUID"].ToString(), new CommonLogic().GetCurrentLocale(), PortalId, PortalSettings.DefaultLanguage);
                    if (Form != null)
                    {
                        Form = oDataAccess.GetFullForm(Form.FormID);
                        FormSettings oFormSettings = new FormSettings();
                        //load the form and place it on this page if the user is allowed to file.
                        if (UserCanFileForm(UserInfo, Form))
                        {
                            string FormTemplateName = Form.FormID.ToString();
                            if (!String.IsNullOrEmpty(Form.FormTemplate))
                            {
                                FormTemplateName = Form.FormTemplate;
                            }

                            string FormTemplatePath = CommonLogic.PathToTemplates + FormTemplateName + ".ascx";
                            FormBase Template = (FormBase)LoadControl(FormTemplatePath);
                            Template.ModuleConfiguration = this.ModuleConfiguration;
                            Template.LocalResourceFile = this.LocalResourceFile;
                            Template.FormID = Form.FormID;
                            this.Controls.Add(Template);
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        //TODO: remove this
        protected void btnGenerateForm_Click(object sender, System.EventArgs e)
        {
            TemplateGeneration oTemplateGeneration = new TemplateGeneration();
            oTemplateGeneration.GenerateTemplateFile(1);
        }

        #endregion

        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(), Localization.GetString("EditModule", this.LocalResourceFile), "", "", "", EditUrl(), false, SecurityAccessLevel.Edit, true, false);
                return Actions;
            }
        }

        #endregion

    }

}
