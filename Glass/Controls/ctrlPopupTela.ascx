<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlPopupTela.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlPopupTela" %>
<div id="blanket" runat="server" style="display: none; position: fixed; left: 0px; top: 0px; z-index: 99999;
    width: 100%; height: 100%">
    <div style="width: 100%; height: 100%; display: table">
        <iframe frameborder="0" style="position: absolute; left: 0px; top: 0px; width: 100%;
            height: 100%"></iframe>
        <div style="width: 100%; height: 100%; opacity: 0.75; background-color: white; position: absolute;
            left: 0; top: 0">
        </div>
        <div id="texto" runat="server" style="text-align: center; position: relative; display: table-cell; vertical-align: middle">
            <%= GetHtmlExibir() %>
        </div>
    </div>
    <asp:HiddenField ID="hdfInnerHTML" runat="server" />
</div>
<script type="text/javascript">
    <%= GetScriptVariavel() %>
</script>