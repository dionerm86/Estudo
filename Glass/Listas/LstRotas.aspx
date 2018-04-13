<%@ Page Title="Histórico de Rotas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRotas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRotas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function abrirMapa()
        {
            var dataIni = FindControl("txtDataInicio", "input").value;
            var horaIni = FindControl("txtHoraInicio", "input").value;
            var dataFim = FindControl("txtDataFim", "input").value;
            var horaFim = FindControl("txtHoraFim", "input").value;

            verifica_data(dataIni);
            verifica_data(dataFim);

            var dtInicio = dataIni + " " + horaIni;
            var dtFim = dataFim + " " + horaFim;
            var idEquipe = FindControl("drpEquipe", "select").value;

            openWindow(500, 700, "../Utils/Rota.aspx?IdEquipe=" + idEquipe + "&dtInicio=" + dtInicio + "&dtFim=" + dtFim);

            return false;
        }        
    </script>

    <table border="0">
        <tr>
            <td nowrap="nowrap">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Equipe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe" runat="server" CssClass="caixatexto" DataSourceID="odsEquipe"
                                DataTextField="Nome" DataValueField="IdEquipe">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
            <td nowrap="nowrap" align="left">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Data Início" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataInicio" runat="server" CssClass="caixatexto" Columns="10"
                                OnKeyUp="mascara_data(event, this)" onfocus="this.select();" MaxLength="10"></asp:TextBox>
                            <asp:TextBox ID="txtHoraInicio" runat="server" CssClass="caixatexto" Columns="6"
                                OnKeyUp="mascara_hora(event, this)" onfocus="this.select();" MaxLength="5">08:00</asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="Data Fim" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDataFim" runat="server" CssClass="caixatexto" Columns="10" OnKeyUp="mascara_data(this)"
                                            MaxLength="10" onfocus="this.select();"></asp:TextBox>
                                        <asp:TextBox ID="txtHoraFim" runat="server" CssClass="caixatexto" Columns="6" OnKeyUp="mascara_hora(this)"
                                            MaxLength="5" onfocus="this.select();">18:00</asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <a href="#" onclick="abrirMapa();">Visualizar Rota</a><colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe"
                    runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.EquipeDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
