<%@ Page Title="Padrão de Geração do Arquivo Remessa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadDadosPadraoCnab.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDadosPadraoCnab" %>

<%@ Register Src="../Controls/ctrlDadosCnab.ascx" TagName="ctrlDadosCnab" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Banco: " ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="ddlBanco" runat="server" Width="200px"
                                DataSourceID="odsBancosCnab" DataTextField="Descr" DataValueField="Id"
                                AppendDataBoundItems="true" OnSelectedIndexChanged="ddlBanco_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsBancosCnab" runat="server" SelectMethod="ListaBancosCnab"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <uc1:ctrlDadosCnab runat="server" ID="ctrlDadosCnab" Visible="false" OnCtrlDadosCnabChange="ctrlDadosCnab_CtrlDadosCnabChange"/>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" Width="70px" Visible="false"/></td>
        </tr>

    </table>

</asp:Content>
