<%@ Page Title="Custos Fixos Gerados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstCustoFixoGerados.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCustoFixoGerados" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function exibirCentroCusto(idContaPg) {

            openWindow(365, 700, '../Utils/SelCentroCusto.aspx?idContaPg=' + idContaPg);
            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cód" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtCodCustoFixo" runat="server" Width="60px" MaxLength="7" onkeypress="return soNumeros(event, true, true);"></asp:TextBox></td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" /></td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Vencimento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataVencIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataVencFin" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPeriodo0" runat="server" Text="Loja:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"
                                OnSelectedIndexChanged="drpLoja_SelectedIndexChanged" OnChange="return validate();" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return validate();" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Fornecedor:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlSelPopup ID="ctrlSelFornec" runat="server" DataSourceID="odsFornecedor"
                                DataTextField="Nome" DataValueField="IdFornec" FazerPostBackBotaoPesquisar="True"
                                ExibirIdPopup="True" TitulosColunas="Cód.|Nome" TituloTela="Selecione o Fornecedor"
                                TextWidth="250px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="190px" MaxLength="7"></asp:TextBox></td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" /></td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Referente à" ForeColor="#0066FF"></asp:Label></td>

                        <td>
                            <asp:DropDownList ID="ddlPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                AppendDataBoundItems="True" Width="290px">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" /></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkCentroCustoDivergente" runat="server" Text="Custo fixo com valor do centro custo divergente" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCustoFixo" runat="server" AllowSorting="True" AllowPaging="True"
                    AutoGenerateColumns="False" DataSourceID="odsCustoFixo" DataKeyNames="IdCustoFixo"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum custo fixo encontrado para o mês especificado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>

                        <asp:BoundField DataField="IdCustoFixo" HeaderText="Cód." SortExpression="IdCustoFixo" />
                        <asp:BoundField DataField="IdContaPg" HeaderText="Cód. Conta a Pagar" SortExpression="IdContaPg" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="IdNomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:TemplateField HeaderText="Contábil" SortExpression="Contabil">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Eval("Contabil") %>' Enabled="False" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValorContaGerada" HeaderText="Valor" SortExpression="ValorContaGerada" />
                        <asp:BoundField DataField="DataVenc" HeaderText="Vencimento" SortExpression="DataVenc" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbCentroCusto" runat="server" ImageUrl='<%# "~/Images/" + ((bool)Eval("CentroCustoCompleto") ? "cash_blue.png" : "cash_red.png") %>' Visible='<%# ExibirCentroCusto() %>'
                                    ToolTip="Exibir Centro de Custos" OnClientClick='<%# "exibirCentroCusto(" + Eval("IdContaPg") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <br />

                <asp:HiddenField ID="hdfData" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCustoFixo" runat="server" SelectMethod="GetListGerados" SelectCountMethod="GetListGeradosCount"
                    TypeName="Glass.Data.DAL.CustoFixoDAO" SortParameterName="sortExpression" EnablePaging="true" MaximumRowsParameterName="pageSize"
                    StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodCustoFixo" Name="idCustoFixo" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlSelFornec" Name="idFornec" PropertyName="Valor" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlSelFornec" Name="nomeFornec" PropertyName="Descricao" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataVencIni" Name="dataVencIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataVencFin" Name="dataVencFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ddlPlanoConta" Name="idConta" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text" Type="String" />
                         <asp:ControlParameter ControlID="chkCentroCustoDivergente" Name="centroCustoDivergente" PropertyName="Checked"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FornecedorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContasCompra"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
