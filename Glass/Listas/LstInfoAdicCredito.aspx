<%@ Page Title="Lista de Informações Adicionais de Crédito" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstInfoAdicCredito.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInfoAdicCredito" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataIni = FindContro("txtDataIni", "input").value;
            var dataFim = FindContro("txtDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DeducaoDiversa&exportarExcel=" + exportarExcel + "&dataIni=" + dataIni + "&dataFim=" + dataFim);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cód. Crédito:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                              <uc1:ctrlSelPopup ID="ctrlSelCodCred" runat="server" DataSourceID="odsCodCred" 
                                    DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("DescrCodCred") %>' PermitirVazio="True" TextWidth="250px" 
                                    TituloTela="Selecione o Cód. Crédito" Valor='<%# Bind("CodCred") %>' 
                                  FazerPostBackBotaoPesquisar="True" />
                        </td>
                          <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo Imposto:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                              <asp:DropDownList ID="drpTipoImposto" runat="server" 
                                    DataSourceID="odsTipoImposto" 
                                    DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True">
                                  <asp:ListItem Value="0">Selecione</asp:ListItem>
                                </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" style="width: 16px" />
                        </td>
                          <td>
                            <asp:Label ID="Label3" runat="server" Text="Período:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPeriodo" runat="server" OnKeypress="return mascara_periodo(event, this);"> </asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadInfoAdicCredito.aspx">Inserir Informação Adicional de Crédito</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdInfoAdicCred" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsInfoAdicCredito"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Não há informações adicionais de crédito cadastradas." 
                    DataKeyNames="IdInfoAdicCredito">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadInfoAdicCredito.aspx?id=" + Eval("IdInfoAdicCredito") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if(!confirm('Deseja realmente excluir esse registro?')) return false;"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrCodCred" HeaderText="Cód. Créd." 
                            ReadOnly="True" SortExpression="CodCred" />
                        <asp:BoundField DataField="Periodo" HeaderText="Período" 
                            SortExpression="Periodo" />
                        <asp:BoundField DataField="DescrTipoImposto" HeaderText="Tipo Imposto" 
                            SortExpression="TipoImposto" />
                        <asp:BoundField DataField="ValorCredPerResAnt" 
                            HeaderText="Valor Créd. Per. Res. Ant." SortExpression="ValorCredPerResAnt" />
                        <asp:BoundField DataField="ValorCredDeclCompAnt" 
                            HeaderText="Valor Créd. Decl. Comp. Ant." 
                            SortExpression="ValorCredDeclCompAnt" />
                        <asp:BoundField DataField="ValorCredDescAnt" 
                            HeaderText="Valor Créd. Desc. Ant." SortExpression="ValorCredDescAnt" />
                        <asp:BoundField DataField="ValorCredPerRes" HeaderText="Valor Créd. Per. Res." 
                            SortExpression="ValorCredPerRes" />
                        <asp:BoundField DataField="ValorCredDeclComp" 
                            HeaderText="Valor Créd. Decl. Comp." SortExpression="ValorCredDeclComp" />
                        <asp:BoundField DataField="ValorCredTransf" HeaderText="Valor Créd. Transf." 
                            SortExpression="ValorCredTransf" />
                        <asp:BoundField DataField="ValorCredOutro" HeaderText="Valor Créd. Outro" 
                            SortExpression="ValorCredOutro" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoAdicCredito" runat="server" DataObjectTypeName="Glass.Data.Model.EFD.InfoAdicCredito"
                    DeleteMethod="Excluir" SelectMethod="ObterLista" 
                    TypeName="Glass.Data.DAL.InfoAdicCreditoDAO"  
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlSelCodCred" Name="codCred" 
                            PropertyName="Valor" Type="Int32" />
                        <asp:ControlParameter ControlID="txtPeriodo" Name="periodo" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoImposto" Name="tipoImposto" 
                            PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred" 
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" 
                    SelectMethod="GetTipoImposto" TypeName="Glass.Data.EFD.DataSourcesEFD" 
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    Visible="False"> <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"
                    Visible="False"><img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>

