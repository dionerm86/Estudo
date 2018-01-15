<%@ Page Title="Cadastro Arquivo Quitação Parcelas de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadArquivoQuitacaoParcelaCartao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadArquivoQuitacaoParcelaCartao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    
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
                <asp:Label ID="lblArquivo" runat="server" Text="Selecione o arquivo"></asp:Label>
                <asp:FileUpload ID="fluArquivo" runat="server" />
                &nbsp;<asp:Button ID="btnEnviarArquivo" runat="server" onclick="btnEnviarArquivo_Click" Text="Enviar Arquivo" OnClientClick="return validaCampos()" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdQuitacaoParcelaCartao" runat="server" GridLines="None" AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false"
                    CssClass="gridStyle" OnRowDataBound="grdQuitacaoParcelaCartao_RowDataBound" EmptyDataText="Nenhum registro encontrado para o arquivo.">
                    <Columns>
                        <asp:BoundField DataField="IdArquivoQuitacaoParcelaCartao" HeaderText="Cód. Arquivo" SortExpression="IdArquivoQuitacaoParcelaCartao" />
                        <asp:BoundField DataField="NumAutCartao" HeaderText="Num. Aut. Cartão" SortExpression="NumAutCartao" />
                        <asp:BoundField DataField="UltimosDigitosCartao" HeaderText="Últ. Dig. Cartão" SortExpression="UltimosDigitosCartao" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:C}"/>
                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" />

                        <asp:BoundField DataField="Parcela" HeaderText="Parcela" SortExpression="Parcela" />
                        <asp:BoundField DataField="Tarifa" HeaderText="Tarifa" SortExpression="Tarifa" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataVencimento" HeaderText="Data de Vencimento" SortExpression="DataVencimento" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DataCadastro" HeaderText="Data de Cadastro" SortExpression="DataCadastro" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="NomeFuncionarioCadastro" HeaderText="Nome Func. Cad." SortExpression="NomeFuncionarioCadastro" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <EditRowStyle CssClass="edit" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnImportarArquivo" runat="server" Text="Importar Arquivo" Visible="false" OnClick="btnImportarArquivo_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" Visible="false" OnClick="btnCancelar_Click" />
                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" Visible="false" OnClientClick="redirectUrl('../Listas/LstArquivoQuitacaoParcelaCartao.aspx'); return false" />
            </td>
        </tr>
    </table>

</asp:Content>
