<%@ Page Title="Cancelamento de Retirada Caixa Geral" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadEstornoCxGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEstornoCxGeral" %>

<%@ Register src="../Controls/ctrlTextBoxFloat.ascx" tagname="ctrlTextBoxFloat" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Cód. Movimentação:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodMov" runat="server" MaxLength="10" Width="70px"
                                            onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="lnkBuscarRetirada" runat="server" 
                                            ImageUrl="~/Images/Pesquisar.gif" ToolTip="Busca retirada" 
                                            onclick="lnkBuscarRetirada_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblMovimentacao" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Motivo"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" 
                                TextMode="MultiLine" Width="450px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnEstornar" runat="server" onclick="btnEstornar_Click" 
                    Text="Cancelar" />
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas" TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

