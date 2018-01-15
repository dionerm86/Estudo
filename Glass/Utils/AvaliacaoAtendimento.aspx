<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AvaliacaoAtendimento.aspx.cs" Inherits="Glass.UI.Web.Utils.AvaliacaoAtendimento"
    Title="Avaliação de atendimento" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function avaliaAtendimento(idAvaliacaoAtendimento, idChamado, aprovado) {
            var container = FindControl("ava_" + idChamado, "tr");
            var satisfacao = FindControl("drpSatisfacao", "select", container).value;
            var obs = FindControl("txtObs", "textarea", container).value;

            if(satisfacao == 0){
                alert("Informe a satisfação para avaliar o chamado!");
                return false;
            }

            if (aprovado == false && (obs == "" || obs == undefined || obs == null)) {
                alert("Informe a observação para negar o chamado!");
                return false;
            }

            if (aprovado == true)
                if (confirm('Confirma a APROVAÇÃO da resolução deste chamado?')) {
                    var retorno = AvaliacaoAtendimento.AvaliaAtendimentoAjax(idAvaliacaoAtendimento, satisfacao, obs, aprovado).value;
                    if (retorno != "")
                        alert(retorno);
                }
            if (aprovado == false)
                if (confirm('Confirma a NÃO APROVAÇÃO da resolução deste chamado?')) {
                    var retorno = AvaliacaoAtendimento.AvaliaAtendimentoAjax(idAvaliacaoAtendimento, satisfacao, obs, aprovado).value;
                    if (retorno != "")
                        alert(retorno);
                }

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Font-Size="Small" Text="Sua avaliação é muito importante para a melhoria contínua dos nossos serviços."></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Font-Size="Small" Text="Clique no ícone “Aprovar” para confirmar que o chamado foi solucionado, caso contrário, clique em “Negar”."></asp:Label>
                <br />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAvaliacaoAtendimento" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsAvaliacaoAtendimento" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum atendimento a ser avaliado!"
                    DataKeyNames="IdAvaliacaoAtendimento" OnRowCommand="grdAvaliacaoAtendimento_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="IdChamado" HeaderText="Chamado" />
                        <asp:BoundField DataField="Analista" HeaderText="Analista" />
                        <asp:BoundField DataField="DataAvaliacao" HeaderText="Cadastro" />
                        <asp:TemplateField HeaderText="Descrição" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label><br />
                                <asp:Label ID="lblResolucao" runat="server" Text='<%# "Resolução: " + (Eval("Resolucao") != null ? Eval("Resolucao") : string.Empty).ToString() %>' Font-Bold="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aprovar" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgAprovar" CommandName="Aprovar" ImageUrl="~/Images/check.gif" CommandArgument='<%# Eval("IdAvaliacaoAtendimento") %>'
                                    OnClientClick='<%# "avaliaAtendimento(" + Eval("IdAvaliacaoAtendimento") + ", " + Eval("IdChamado") + ", true)" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Negar" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgNegar" CommandName="Negar" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("IdAvaliacaoAtendimento") %>'
                                    OnClientClick='<%# "avaliaAtendimento(" + Eval("IdAvaliacaoAtendimento") + ", " + Eval("IdChamado") + ", false)" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td></tr>
                                <tr id="ava_<%# Eval("IdChamado") %>" style="border-top: none; display: normal">
                                    <td colspan="2"></td>
                                    <td colspan="10" style="padding-right: 6px" align="left">
                                        <table>
                                            <tr>
                                                <td>
                                                    <div style="font-weight: bold; font-size: 120%; padding-top: 4px">
                                                        Satisfação: 
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="drpSatisfacao" runat="server" DataSourceID="odsSatisfacaoAvaliacaoAtendimento"
                                                        AppendDataBoundItems="true" DataTextField="Descr" DataValueField="Id">
                                                        <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <div style="font-weight: bold; font-size: 120%; padding-top: 4px">
                                                        Observação Cliente: 
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Rows="3" Width="350"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAvaliacaoAtendimento" runat="server" DataObjectTypeName="Glass.Data.Model.AvaliacaoAtendimento"
                    SelectMethod="GetList" TypeName="Glass.Data.DAL.AvaliacaoAtendimentoDAO">
                </colo:VirtualObjectDataSource>
                <sync:ObjectDataSource ID="odsSatisfacaoAvaliacaoAtendimento" runat="server" SelectMethod="GetSatisfacaoAvaliacaoAtendimento"
                    TypeName="Glass.Data.Helper.DataSources">
                </sync:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="Label3" runat="server" ForeColor="Red" Text="* Nossa equipe entrará em contato em breve, caso um chamado seja negado."></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
