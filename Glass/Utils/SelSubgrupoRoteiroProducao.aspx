<%@ Page Title="Selecione o Subgrupo" Language="C#" MasterPageFile="~/Layout.master"
     AutoEventWireup="true" CodeBehind="SelSubgrupoRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelSubgrupoRoteiroProducao" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        var selecionado = false;

        function setSubgrupo(idSubgrupo) {

            if (selecionado)
                return;

            selecionado = true;

            window.opener.setSubgrupo(idSubgrupo);

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">               
                <br />
                <asp:GridView ID="grdSubgrupo" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="Id"
                    DataSourceID="odsSubgrupo" GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return setSubgrupo('<%# Eval("Id") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>                       
                        <asp:BoundField DataField="Name" HeaderText="Subgrupo" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsSubgrupo" runat="server" Culture="pt-BR" 
                     TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo" SelectMethod="ObterSubgruposClassificacaoRoteiroProducao">                  
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
