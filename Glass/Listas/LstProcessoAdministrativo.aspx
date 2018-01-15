<%@ Page Title="Processos Administrativos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstProcessoAdministrativo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstProcessoAdministrativo" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProcAdm" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdProcAdm" DataSourceID="odsProcessoAdm"
                    GridLines="None" ShowFooter="True" EmptyDataText="Nenhum registro encontrado"
                    OnDataBound="grdProcAdm_DataBound" OnRowUpdating="grdProcAdm_RowUpdating" OnRowCommand="grdProcAdm_RowCommand" >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" CausesValidation="False" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" CausesValidation="False" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse registro?&quot;)) return false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgSalvar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    ValidationGroup="u" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Processo" SortExpression="NumeroProcesso">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumeroProcesso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("NumeroProcesso") %>' MaxLength="20"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1"
                                    Display="Dynamic" ErrorMessage="Informe  o número do processo" SetFocusOnError="True"
                                    ToolTip="Informe  o número do processo" ValidationGroup="u">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumeroProcesso" runat="server" Text='<%# Bind("NumeroProcesso") %>'
                                    MaxLength="20"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumeroProcesso"
                                    Display="Dynamic" ErrorMessage="Informe  o número do processo" SetFocusOnError="True"
                                    ToolTip="Informe  o número do processo" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza" SortExpression="Natureza">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NaturezaString") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Natureza") %>'
                                    DataSourceID="odsNatureza" DataTextField="Descr" DataValueField="Id">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="ddlNatureza" runat="server" SelectedValue='<%# Bind("Natureza") %>'
                                    DataSourceID="odsNatureza" DataTextField="Descr" DataValueField="Id">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Decisão" SortExpression="DataDecisao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataDecisao", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlDataDecisao" ErrorMessage="*" runat="server"
                                    ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true" DataString='<%# Bind("DataDecisao") %>'/>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlData ID="ctrlDataDecisao" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                    ValidateEmptyText="true" ErrorMessage="*" />
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click"
                                    ValidationGroup="c" />
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProcessoAdm" runat="server" DataObjectTypeName="Glass.Data.Model.ProcessoAdministrativo"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" DeleteMethod="Delete" SelectMethod="GetList"
                    TypeName="Glass.Data.DAL.ProcessoAdministrativoDAO" UpdateMethod="Update" EnablePaging="True">
                    <SelectParameters>
                        <asp:Parameter Name="numeroProcesso" Type="String" />
                        <asp:Parameter Name="natureza" Type="Int32" />
                        <asp:Parameter Name="dataDecisaoIni" Type="DateTime" />
                        <asp:Parameter Name="dataDecisaoFim" Type="DateTime" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNatureza" runat="server" SelectMethod="GetNaturezaProcessoAdministrativo"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <div style="color: blue; text-align: center">
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
