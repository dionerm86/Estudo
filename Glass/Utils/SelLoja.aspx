<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelLoja.aspx.cs" Inherits="Glass.UI.Web.Utils.SelLoja"
    Title="Selecione a Loja" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">

    function setLoja(idLoja, razaoSocial, cpfCnpj, nomeFantasia) {
        // Se for busca de loja para NF-e
        if (FindControl("hdfNfe", "input").value == 1)
            window.opener.setLojaNfe(idLoja, razaoSocial, cpfCnpj);
        else if (FindControl("hdfNfe", "input").value == 2)
            window.opener.setLojaDest(idLoja, razaoSocial, cpfCnpj);
        else if ('<%= Request["callback"] %>' == "setForPart")
            window.opener.ctrlSelParticipante_setLoja(idLoja, '<%= Request["controle"] %>');
        else if ('<%= Request["callback"] %>' == "participanteFiscal")
            window.opener.ControleSelecaoParticipanteFiscal.selecionar('<%= Request["controle"] %>', idLoja, nomeFantasia || razaoSocial);

        closeWindow();
    }

    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLoja" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsLoja"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdLoja"
                    EmptyDataText="Não há Lojas cadastradas">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setLoja('<%# Eval("IdLoja") %>', '<%# Eval("RazaoSocial") %>', '<%# Eval("Cnpj") %>', '<%# Eval("NomeFantasia") %>'); closeWindow();">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="RazaoSocial" HeaderText="Razão Social" SortExpression="RazaoSocial" />
                        <asp:BoundField DataField="Cnpj" HeaderText="Cnpj" SortExpression="Cnpj" />
                        <asp:BoundField DataField="DescrEndereco" HeaderText="Endereço" SortExpression="DescrEndereco" />
                        <asp:BoundField DataField="Telefone" HeaderText="Telefone" SortExpression="Telefone" />
                        <asp:BoundField DataField="InscEst" HeaderText="Insc. Est." SortExpression="InscEst" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" DataObjectTypeName="Glass.Data.Model.Loja"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.LojaDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow"></colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfNfe" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
