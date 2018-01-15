<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoRemessa" Title="Arquivos de Remessa" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <table class="style1">
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cód" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtCod" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" 
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                        <td align="right">
                            <asp:Label ID="lblArquivoRemessa2" runat="server" Text="Num. Arq. Remessa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumArqRemessa" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label6" runat="server" Text="Tipo Remessa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                        <asp:DropDownList runat="server" ID="drpTipoRemessa">
                            <asp:ListItem Text="Selecione" Value="" Selected="True" />
                            <asp:ListItem Text="Envio" Value="0" />
                            <asp:ListItem Text="Retorno" Value="1" />
                        </asp:DropDownList>
                            </td>
                            <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="lblPeriodoCadastro" runat="server" Text="Período Cadastro " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="right">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesqPeriodoCadastro" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblIdContaBancoCli" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                 onchange="drpContaClienteChange(this);">
                                <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
                                TypeName="Glass.Data.DAL.ContaBancoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdArquivoRemessa" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdArquivoRemessa" DataSourceID="odsArquivoRemessa" 
                    EmptyDataText="Não há arquivos de remessa." GridLines="None" PageSize="30" 
                    onrowdatabound="grdArquivoRemessa_RowDataBound">
                    <Columns>
                    <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este arquivo remessa?');"
                                    Visible='<%# Eval("DeletarVisivel") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/disk.gif" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Download do Arquivo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRetificar" runat="server" ImageUrl="~/Images/retificar.png" Height="16" Width="16"
                                    onclientclick='<%# "openWindow(700, 1000,\"../Cadastros/CadRetificarArquivoRemessa.aspx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Retificar Arquivo Remessa" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdArquivoRemessa" HeaderText="Cód." 
                            SortExpression="IdArquivoRemessa" />
                        <asp:BoundField DataField="NumRemessa" HeaderText="Núm. Remessa" 
                            SortExpression="NumRemessa" />
                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" 
                            SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Cad." 
                            SortExpression="DataCad" />
                             <asp:TemplateField>
                            <ItemTemplate>
                               <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/blocodenotas.png" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?logImportacao=true&id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Log de importação" Visible='<%# Eval("LogVisivel") %>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArquivoRemessa" runat="server" 
                    EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" 
                    SelectMethod="GetList" 
                    SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" DataObjectTypeName="Glass.Data.Model.ArquivoRemessa"
                    TypeName="Glass.Data.DAL.ArquivoRemessaDAO" DeleteMethod="Delete"
                    ondeleted="odsArquivoRemessa_Deleted">
                     <SelectParameters>
                        <asp:ControlParameter ControlID="txtCod" Name="codArquivoRemessa" PropertyName="text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumArqRemessa" Name="numArqRemessa" PropertyName="text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                           <asp:ControlParameter ControlID="drpTipoRemessa" Name="tipoRemessa" PropertyName="SelectedValue"
                            Type="Int32" />
                         <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

