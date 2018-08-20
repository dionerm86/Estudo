<%@ Page Title="Cadastro de Sugestões / Reclamações" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSugestaoCliente.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSugestaoCliente" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    function getCli(idCli)
        {            
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfCliente", "input").value = "";
                
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("hdfCliente", "input").value = idCli.value;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblInfoNomeCli" runat="server" Text="Cliente:" ForeColor="#0066FF"></asp:Label>
                &nbsp;<asp:Label ID="lblCliente" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cód. Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblInfoCli" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Width="250px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;">
                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                            </asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;&nbsp;<asp:Label ID="Label23" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" 
                                DataSourceID="odsTipoSugestaoCliente"
                                DataTextField="Descricao" 
                                DataValueField="IdTipoSugestaoCliente" 
                                AppendDataBoundItems="True">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoSugestaoCliente" runat="server" 
                                SelectMethod="ObterTiposSugestaoCliente"
                                TypeName="Glass.Data.DAL.TipoSugestaoClienteDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" style="height: 211px">
                <asp:Label ID="Label24" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                <br />
                <asp:TextBox ID="txtDescricao" runat="server" Height="166px" TextMode="MultiLine"
                    Width="529px"></asp:TextBox>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;<asp:Button ID="btnInserir" runat="server" OnClick="btnInserir_Click" Text="Inserir"
                    OnClientClick="" />
                &nbsp;<asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblStatus" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSugestao" runat="server" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="ObtemSugestao" SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.ISugestaoFluxo">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfCliente" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
