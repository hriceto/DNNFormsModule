<%@ Control Language="C#" AutoEventWireup="false" Inherits="HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblForm" runat="server" /> 
 
        <asp:DropDownList ID="ddlForm" runat="server"/>
    </div>
</fieldset>