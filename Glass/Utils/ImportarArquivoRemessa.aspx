<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ImportarArquivoRemessa.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ImportarArquivoRemessa" Title="Importar Arquivo de Remessa" %>

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
                     <tr>
                        <td align="right">
                            Conta Bancária de Cobrança
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" 
                                AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvContaBanco" runat="server" ErrorMessage="*" 
                                ControlToValidate="drpContaBanco" Display="Dynamic" 
                                ValidationGroup="gerar"></asp:RequiredFieldValidator>
                            <img id="image" src="../Images/load.gif" style="width:16px; height:16px; display:none"; alt="" />
                        </td>
                    </tr>
                    <tr id="tipo-arquivo">
                        <td align="right">
                            Tipo de CNAB
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlTipoCnab" runat="server">
                                <asp:ListItem Text="CNAB 240" Value="1"></asp:ListItem>
                                <asp:ListItem Text="CNAB 400" Value="2" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
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
                <asp:Button ID="btnImportarArquivo" runat="server" onclick="btnImportarArquivo_Click" 
                    Text="Importar Arquivo" />
                <asp:Button ID="btnVerificarArquivo" runat="server" onclick="btnVerificarArquivo_Click"
                    Text="Verificar Arquivo" OnClientClick="return validaCampos()" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView GridLines="None" ID="grdItensCNAB" runat="server" CssClass="gridStyle" PagerStyle-CssClass="pgr" 
                    AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" Visible="false" OnRowDataBound="grdItensCNAB_RowDataBound" >
                    <Columns>
                        <asp:TemplateField HeaderText="Nº Registro">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("NumeroLinha") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agencia">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Agencia") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Conta") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nosso Numero">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("NossoNumero") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Numero Documento">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("NumeroDocumento") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Titulo">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("ValorTitulo") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Pago">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("ValorPagoPeloSacado") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Juros">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("JurosMultaEncargos") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data de Vencimento">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DataVencimento") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
        TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
</asp:Content>
