using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data;
using System.IO;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components
{
    public class TemplateGeneration
    {
        public const string NewLine = "\r\n";

        //loop through all pages/controls in the database and generate a template from it.
        public void GenerateTemplateFile(int formID)
        {
            DataAccess oDataAccess = new DataAccess();
            FQDNN_Form oForm = oDataAccess.GetFullForm(formID);
            oForm.FQDNN_FormSetting.ToList();
            string FormTemplateName = oForm.FormID.ToString();
            if (!String.IsNullOrEmpty(oForm.FormTemplate))
            {
                FormTemplateName = oForm.FormTemplate;
            }
            
            string FormTemplatePath = CommonLogic.PathToTemplates + FormTemplateName + ".ascx";

            StringBuilder sb = new StringBuilder();
            sb.Append("<%@ Control Language=\"C#\" AutoEventWireup=\"true\" Inherits=\"HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components.FormBase\" %>" + NewLine);
            foreach (FQDNN_FormPage oFormPage in oForm.FQDNN_FormPage)
            {
                sb.Append(String.Format("<!-- PAGE {0} START -->", oFormPage.PageNumber) + NewLine);
                if (oFormPage.FQDNN_FormControl.Count > 0)
                {
                    int maxColumn = oFormPage.FQDNN_FormControl.Max(c => c.Column);
                    int maxRow = oFormPage.FQDNN_FormControl.Max(c => c.Row);
                    List<FQDNN_FormControl> oControls = oFormPage.FQDNN_FormControl.ToList();
                    sb.Append(String.Format("<asp:Panel ID=\"{0}\" runat=\"server\">", oFormPage.GetControlID()) + NewLine);
                    sb.Append("<table>" + NewLine);
                    for (int currentRow = 1; currentRow <= maxRow; currentRow++)
                    {
                        sb.Append("<tr>" + NewLine);
                        for (int currentColumn = 1; currentColumn <= maxColumn; currentColumn++)
                        {
                            sb.Append("<td>" + NewLine);
                            sb.Append(GetControlMarkup(oControls, currentRow, currentColumn) + NewLine);
                            sb.Append("</td>" + NewLine);
                        }
                        sb.Append("</tr>" + NewLine);
                    }
                    sb.Append("</table>" + NewLine);
                    sb.Append(String.Format("<asp:Button ID=\"{0}\" runat=\"server\" ResourceKey=\"PreviousPage\"/>", oFormPage.GetPreviousPageControlID()) + NewLine);
                    sb.Append(String.Format("<asp:Button ID=\"{0}\" runat=\"server\" ResourceKey=\"NextPage\"/>", oFormPage.GetNextPageControlID()) + NewLine);
                    sb.Append("</asp:Panel>" + NewLine);
                }
                sb.Append(String.Format("<!-- PAGE {0} END -->", oFormPage.PageNumber) + NewLine);
            }
            sb.Append(String.Format("<asp:Button ID=\"{0}\" runat=\"server\" ResourceKey=\"Submit\"/>", oForm.GetSubmitButtonControlID()) + NewLine);
            sb.Append(String.Format("<asp:Panel ID=\"{0}\" runat=\"server\" ><asp:Label ID=\"lblGenericMessage\" ResourceKey=\"lblGenericMessage\" runat=\"server\"/></asp:Panel>", oForm.GetGenericMessageControlID()) + NewLine);
            
            StreamWriter writer = new StreamWriter(HttpContext.Current.Server.MapPath(FormTemplatePath));
            writer.Write(sb.ToString());
            writer.Close();
        }

        private string GetControlMarkup(List<FQDNN_FormControl> oControls, int currentRow, int currentColumn)
        {
            string result = "";

            foreach (FQDNN_FormControl oControl in oControls)
            {
                if (oControl.Row == currentRow && oControl.Column == currentColumn)
                {
                    result = oControl.FQDNN_ControlDefinition.Markup;
                    result = result.Replace("{ID}", oControl.GetControlID());
                    break;
                }
            }

            return result;
        }
    }
}