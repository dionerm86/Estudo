<%@ Page Title="Livro de Registro de Entrada" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LivroRegistroEntrada.aspx.cs" Inherits="Glass.UI.Web.Relatorios.NFe.LivroRegistroEntrada" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../../Scripts/jquery/jquery-1.8.2.js" type="text/javascript"></script>

    <script src="../../Scripts/jquery/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function() {
            $("#lnkImprimir").click(function() {
                var loja = FindControl("drpLoja", "select").value;
                var dataInicial = FindControl("txtDataIni", "input").value;
                var dataFinal = FindControl("txtDataFim", "input").value;
                var sobNumero = FindControl("txtSobNumero", "input").value;
                var arquivado = FindControl("txtArquivado", "input").value;
                var numeroOrdem = FindControl("txtNumeroOrdem", "input").value;
                var localData = FindControl("txtLocalData", "input").value;

                var queryString = "RelBase.aspx?rel=LivroRegEntrada&loja=" + loja + "&dataInicial=" + dataInicial +
                    "&dataFinal=" + dataFinal + "&sobNumero=" + sobNumero + "&arquivado=" + arquivado +
                    "&numeroOrdem=" + numeroOrdem + "&localData=" + localData;

                openWindow(600, 800, queryString)
            });
        });
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Sob Número" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSobNumero" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Arquivado Em" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtArquivado" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Número de Ordem" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroOrdem" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Local/Data" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLocalData" runat="server" Style="width: 300px"></asp:TextBox>
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
                            <a href="#" id="lnkImprimir" title="Gerar Livro de Registro de Entrada">
                                <img alt="" border="0" src="../../Images/printer.png" />
                                Gerar Livro de Registro de Entrada</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
