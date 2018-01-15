<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCentroCusto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCentroCusto" Title="Centros de Custos"
    EnableEventValidation="false" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        var inicial = null;
        
        function atualizaCodigoTipo()
        {
            var codigoTipo = FindControl("drpCodigoTipo", "select");
            var idLoja = FindControl("drpLoja", "select").value;

            var atual = codigoTipo.value;
            codigoTipo.innerHTML = LstCentroCusto.GetTipos(idLoja).value;
            codigoTipo.value = atual;
        }

        function codigoTipoChanged(codigoTipo)
        {
            FindControl("hdfCodigoTipo", "input").value = codigoTipo;
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdCentroCusto" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdCentroCusto" DataSourceID="odsCentroCusto" 
                    GridLines="None" ondatabound="grdCentroCusto_DataBound" 
                    ShowFooter="True" onrowcommand="grdCentroCusto_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgEdit" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgDelete" runat="server" CommandName="Cancel" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEdit" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgDelete" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir esse plano de conta contábil?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdCentroCusto">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdCentroCusto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdCentroCusto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>' onchange="atualizaCodigoTipo()">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>' onchange="atualizaCodigoTipo()">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' 
                                    MaxLength="60" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Área" SortExpression="DescrCodigoTipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodigoTipo" runat="server" 
                                    onchange="codigoTipoChanged(this.value)" EnableViewState="False">
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfCodigoTipo" runat="server" 
                                    Value='<%# Bind("CodigoTipo") %>' />
                                <script type="text/javascript">
                                    inicial = '<%# Eval("CodigoTipo") %>';
                                </script>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCodigoTipo" runat="server" 
                                    onchange="codigoTipoChanged(this.value)" EnableViewState="False">
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfCodigoTipo" runat="server" 
                                    Value='<%# Bind("CodigoTipo") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrCodigoTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                       

                        <asp:TemplateField HeaderText="Tipo de Centro de Custo" SortExpression="Tipo">
                            <ItemTemplate>
                                <asp:Label ID="Label50" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Tipo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" SelectedValue='<%# Bind("Tipo") %>'
                                                  DataSourceID="odsTipo" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" 
                                                  DataSourceID="odsTipo" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Data Cadastro" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="CentroCusto"
                                    IdRegistro='<%# Convert.ToUInt32(Eval("IdCentroCusto")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCentroCusto" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.CentroCusto" DeleteMethod="Delete" 
                    EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.CentroCustoDAO" UpdateMethod="Update" 
                    ondeleted="odsCentroCusto_Deleted" oninserted="odsCentroCusto_Inserted" 
                    onupdated="odsCentroCusto_Updated"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO"></colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource runat="server" ID="odsTipo"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoCentroCusto, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

            </td>
        </tr>
    </table>
    <script type="text/javascript">
        atualizaCodigoTipo();
        if (inicial != null)
            FindControl("drpCodigoTipo", "select").value = inicial;
    </script>
</asp:Content>

