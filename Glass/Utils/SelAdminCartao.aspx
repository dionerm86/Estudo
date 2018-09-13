<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelAdminCartao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelAdminCartao"
    Title="Selecione a Administradora de Cartão" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setAdminCartao(idAdminCartao, nome)
        {
            if ('<%= Request["callback"] %>' == "setForPart")
                window.opener.ctrlSelParticipante_setAdminCartao(idAdminCartao, '<%= Request["controle"] %>');
            else if ('<%= Request["callback"] %>' == "participanteFiscal")
                window.opener.ControleSelecaoParticipanteFiscal.selecionar('<%= Request["controle"] %>', idAdminCartao, nome);

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAdminCartao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsAdminCartao"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdAdminCartao" EmptyDataText="Não há Lojas cadastradas">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setAdminCartao('<%# Eval("IdAdminCartao") %>', '<%# Eval("Nome") %>'); closeWindow();">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Cnpj" HeaderText="CNPJ" SortExpression="Cnpj" />
                        <asp:BoundField DataField="Cidade" HeaderText="Cidade" SortExpression="Cidade" />
                        <asp:BoundField DataField="Uf" HeaderText="UF" SortExpression="Uf" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAdminCartao" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.DAL.AdministradoraCartaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
