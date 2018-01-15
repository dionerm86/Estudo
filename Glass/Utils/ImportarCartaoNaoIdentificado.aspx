<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ImportarCartaoNaoIdentificado.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ImportarCartaoNaoIdentificado" Title="Importar Cartão não Identificado" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function validaCampos() {
            if (FindControl("fluArquivo", "input").value == "") {
                alert("Selecione o arquivo que será importado.");
                return false;
            }

            return true;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Selecione o arquivo
                        </td>
                        <td>
                            <asp:FileUpload ID="fluArquivo" runat="server" />
                        </td>
                    </tr>                   
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnImportarArquivo" runat="server" onclick="btnImportarArquivo_Click" Text="Importar Arquivo" OnClientClick="return validaCampos()" />   
                <br/>  <br/>  <br/>  <br/>  <br/>  <br/>  
                <asp:Button ID="btnVoltar" runat="server" onclick="Voltar_Click" Text="Voltar para Listagem"/>           
            </td>            
        </tr>
    </table>
     <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
