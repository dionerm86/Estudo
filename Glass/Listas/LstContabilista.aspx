<%@ Page Title="Contabilista" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstContabilista.aspx.cs" Inherits="Glass.UI.Web.Listas.LstContabilista" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrllogpopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Contabilista</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContabilista" runat="server" 
                    AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsContabilista" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" 
                    EmptyDataText="Nenhum contabilista encontrado." 
                    DataKeyNames="IdContabilista">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadContabilista.aspx?idContabilista=" + Eval("IdContabilista") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Contabilista?&quot;);" />
                                <asp:HiddenField ID="hdfIdContabilista" runat="server" 
                                    Value='<%# Bind("IdContabilista") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" 
                            SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="Crc" HeaderText="CRC" SortExpression="Crc" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" 
                            SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="TelCont" HeaderText="Fone" 
                            SortExpression="TelCont" />
                        <asp:BoundField DataField="Fax" HeaderText="Fax" SortExpression="Fax" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource  culture="pt-BR" ID="odsContabilista" runat="server" 
                    SelectMethod="GetList" TypeName="Glass.Data.DAL.ContabilistaDAO" 
                    DataObjectTypeName="Glass.Data.Model.Contabilista" DeleteMethod="Delete" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    ondeleted="odsContabilista_Deleted" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" SelectCountMethod="GetCount">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        </table>
</asp:Content>

