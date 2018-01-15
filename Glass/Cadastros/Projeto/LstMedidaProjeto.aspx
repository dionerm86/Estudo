<%@ Page Title="Medidas de Projeto" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMedidaProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstMedidaProjeto" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricaoEdit", "input").value;

            if (descricao != null && descricao == "") {
                alert("Informe a descrição.");
                return false;
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdMedidaProjeto" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" GridLines="None" DataSourceID="odsMedidaProjeto"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" ShowFooter="True" DataKeyNames="IdMedidaProjeto"
                    PageSize="15">
                    <FooterStyle />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" Visible='<%# Eval("EditarVisible") %>'
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir esta Medida?');" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricaoEdit" runat="server" MaxLength="50" Visible='<%# Eval("EditarVisible") %>'
                                    Text='<%# Bind("Descricao") %>' Width="150px"></asp:TextBox>
                                <asp:Label ID="lblDescricao" runat="server" Visible='<%# !(bool)Eval("EditarVisible") %>'
                                    Text='<%# Eval("Descricao") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Padrão" SortExpression="ValorPadrao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorPadrao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorPadrao" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("ValorPadrao") %>' Width="60px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorPadrao" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("ValorPadrao") %>' Width="60px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Calc. Medida Exata" SortExpression="ExibirMedidaExata">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("ExibirMedidaExata") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkMedidaExata" runat="server" Checked='<%# Bind("ExibirMedidaExata") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkMedidaExata" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Calc. Apenas Ferragens e Alumínios" SortExpression="ExibirApenasFerragensAluminios">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirApenasFerragensAluminios" runat="server" Checked='<%# Eval("ExibirApenasFerragensAluminios") %>' Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExibirApenasFerragensAluminios" runat="server" Checked='<%# Bind("ExibirApenasFerragensAluminios") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirApenasFerragensAluminios" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="IdGrupoMedProj">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrGrupo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpGrupoMedProj" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsGrupoMedProj" DataTextField="Descricao" DataValueField="IdGrupoMedProj"
                                    SelectedValue='<%# Bind("IdGrupoMedProj") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpGrupoMedProj" runat="server" DataSourceID="odsGrupoMedProj"
                                    DataTextField="Descricao" DataValueField="IdGrupoMedProj">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdMedidaProjeto") %>'
                                    Tabela="MedidaProjeto" />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <HeaderStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMedidaProjeto" runat="server" DeleteMethod="Delete"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" OnDeleted="odsMedidaProjeto_Deleted"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.MedidaProjetoDAO"
                    DataObjectTypeName="Glass.Data.Model.MedidaProjeto" UpdateMethod="Update" OnUpdated="odsMedidaProjeto_Updated">
                      <SelectParameters>
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" 
                            PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoMedProj" runat="server" SelectMethod="ObtemOrdenado" TypeName="Glass.Data.DAL.GrupoMedidaProjetoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
