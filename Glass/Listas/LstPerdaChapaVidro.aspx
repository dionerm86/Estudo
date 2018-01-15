<%@ Page Title="Perda de Chapa de Vidro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPerdaChapaVidro.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPerdaChapaVidro" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cód." ForeColor="#0066FF" /></td>
                        <td>
                            <asp:TextBox ID="txtIdPerdaChapaVidro" runat="server" Width="50px"></asp:TextBox></td>
                        <td>
                            <asp:ImageButton ID="imgPesqFornComCredito" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Tipo da Perda" ForeColor="#0066FF" />
                        </td>

                        <td>
                            <asp:DropDownList runat="server" ID="drpTipoPerda" DataSourceID="odsTipoPerda" AppendDataBoundItems="true"
                                DataTextField="Descricao" DataValueField="IdTipoPerda">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Subtipo da Perda" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="drpSubtipoPerda" DataSourceID="odsSubtipoPerda" AppendDataBoundItems="true"
                                DataTextField="Descricao" DataValueField="IdSubtipoPerda">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período Perda.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Etiqueta" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumEtiqueta" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir nova perda</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPerdaChapaVidro" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdPerdaChapaVidro"
                    DataSourceID="odsPerdaChapaVidro" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit"
                    EmptyDataText="Não há perdas de chapa de vidro cadastrada." OnRowDataBound="grdPerdaChapaVidro_RowDataBound">
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# !((bool)Eval("Cancelado")) %>' OnClientClick="return confirm(&quot;Tem certeza que deseja cancelar esta perda?&quot;);" />
                                <asp:HiddenField runat="server" ID="hdfIdProd" Value='<%# Eval("IdProd") %>' />
                                <asp:HiddenField runat="server" ID="hdfIdProdNf" Value='<%# Eval("IdProdNf") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPerdaChapaVidro" HeaderText="Cód." SortExpression="IdPerdaChapaVidro" />
                        <asp:BoundField DataField="DescrProd" HeaderText="Produto" SortExpression="DescrProd" />
                        <asp:BoundField DataField="TipoPerda" HeaderText="Tipo da Perda" SortExpression="TipoPerda" />
                        <asp:BoundField DataField="SubtipoPerda" HeaderText="Subtipo da Perda" SortExpression="SubtipoPerda" />
                        <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="NumEtiqueta" />
                        <asp:BoundField DataField="DataPerda" HeaderText="Data" SortExpression="DataPerda" />
                        <asp:BoundField DataField="FuncPerda" HeaderText="Func." SortExpression="FuncPerda" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup" runat="server" Tabela="PerdaChapaVidro"
                                    IdRegistro='<%# Eval("IdPerdaChapaVidro") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center"></td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPerdaChapaVidro" runat="server" DataObjectTypeName="Glass.Data.Model.PerdaChapaVidro"
                    DeleteMethod="Cancelar" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListPerdaChapaVidroCount" SelectMethod="GetListPerdaChapaVidro" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PerdaChapaVidroDAO" OnDeleted="odsPerdaChapaVidro_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdPerdaChapaVidro" PropertyName="Text" Name="idPerdaChapaVidro" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoPerda" Name="idTipoPeda" Type="UInt32" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpSubtipoPerda" Name="idSubTipoPerda" Type="UInt32" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataIni" Type="String"  PropertyName="DataString"/>
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataFim" Type="String"  PropertyName="DataString"/>
                        <asp:ControlParameter ControlID="txtNumEtiqueta" Name="numEtiqueta" Type="String" PropertyName="Text" />
                         <asp:Parameter Name="idsProdImpressao" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetOrderedList"
                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubtipoPerda" runat="server" SelectMethod="GetListSubtipoPerda"
                    TypeName="Glass.Data.DAL.SubtipoPerdaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
