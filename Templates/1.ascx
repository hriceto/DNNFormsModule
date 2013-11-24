<%@ Control Language="C#" AutoEventWireup="true" Inherits="HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components.FormBase" %>
<!-- PAGE 1 START -->
<asp:Panel ID="Page1" runat="server">
<table>
<tr>
<td>
<asp:TextBox ID="Name_1" runat="server"></asp:TextBox>
</td>
<td>

</td>
</tr>
<tr>
<td>
<asp:CheckBox ID="Male_2" runat="server"/>
</td>
<td>
<asp:CheckBox ID="Employed_3" runat="server"/>
</td>
</tr>
</table>
<asp:Button ID="btnPreviousPage1" runat="server" ResourceKey="PreviousPage"/>
<asp:Button ID="btnNextPage1" runat="server" ResourceKey="NextPage"/>
</asp:Panel>
<!-- PAGE 1 END -->
<!-- PAGE 2 START -->
<asp:Panel ID="Page2" runat="server">
<table>
<tr>
<td>
<asp:TextBox ID="Address_4" runat="server"></asp:TextBox>
</td>
<td>

</td>
</tr>
<tr>
<td>

</td>
<td>
<asp:DropDownList ID="State_5" runat="server"/>
</td>
</tr>
<tr>
<td>
<asp:DropDownList ID="Country_6" runat="server"/>
</td>
<td>

</td>
</tr>
</table>
<asp:Button ID="btnPreviousPage2" runat="server" ResourceKey="PreviousPage"/>
<asp:Button ID="btnNextPage2" runat="server" ResourceKey="NextPage"/>
</asp:Panel>
<!-- PAGE 2 END -->
<asp:Button ID="btnSubmit" runat="server" ResourceKey="Submit"/>
<asp:Panel ID="pnlGenericMessage" runat="server" ><asp:Label ID="lblGenericMessage" ResourceKey="lblGenericMessage" runat="server"/></asp:Panel>
