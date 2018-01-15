<%@ Page Title="Aplicações" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaAplicacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaAplicacao" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;
            var codInterno = FindControl(insert ? "txtCodInternoIns" : "txtCodInterno", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }

            if (codInterno == "") {
                alert("Informe o código.");
                return false;
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdAplicacao" runat="server" SkinID="gridViewEditable"
                              DataSourceID="odsAplicacao" DataKeyNames="IdAplicacao" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="10" Text='<%# Bind("CodInterno") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodInternoIns" runat="server" MaxLength="10" Text='<%# Bind("CodInterno") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="30" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="30" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Destacar na Etiqueta?" 
                            SortExpression="DestacarEtiqueta">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkDestacar" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gerar forma inexistente" 
                            SortExpression="GerarFormaInexistente">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("GerarFormaInexistente") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkGerarForma" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Eval("GerarFormaInexistente") %>' Enabled="False" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dias minimos p/ Entrega" SortExpression="DiasMinimos">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDiasMinimos" runat="server" MaxLength="10" Text='<%# Bind("DiasMinimos") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDiasMinimos" runat="server" MaxLength="10" Text='<%# Bind("DiasMinimos") %>'
                                    Width="50px"></asp:TextBox>
                                <asp:Image ID="imgConta" runat="server" ImageUrl="~/Images/Help.gif" ToolTip="Numero de dias minimos para entrega do pedido com produtos dessa aplicação."/>
                                
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LblDiasMinimos" runat="server" Text='<%# Bind("DiasMinimos") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Tipo de Pedido">
                            <ItemTemplate>
                            <asp:Label ID="LabelTipoPedido" runat="server" Text='<%# Bind("DescricaoTipoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <sync:CheckBoxListDropDown ID="drpTipoPedido" runat="server" CheckAll="False" OnLoad="drpTipoPedido_Load"
                                    ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                    OpenOnStart="False" SelectedValue='<%# Bind("TipoPedido") %>'>
                                    <asp:ListItem Value="1">Venda</asp:ListItem>
                                    <asp:ListItem Value="3">Mão-de-obra</asp:ListItem>
                                    <asp:ListItem Value="4">Produção</asp:ListItem>
                                </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <sync:CheckBoxListDropDown ID="drpTipoPedido" runat="server" CheckAll="False" OnLoad="drpTipoPedido_Load"
                                    ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                    OpenOnStart="False"  SelectedValue='<%# Bind("TipoPedido") %>'>
                                    <asp:ListItem Value="1">Venda</asp:ListItem>
                                    <asp:ListItem Value="3">Mão-de-obra</asp:ListItem>
                                    <asp:ListItem Value="4">Produção</asp:ListItem>
                                </sync:CheckBoxListDropDown>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label> &nbsp&nbsp&nbsp
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Aplicacao" IdRegistro='<%# (uint)(int)Eval("IdAplicacao") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server">
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAplicacao" runat="server" 
                    DeleteMethod="ApagarEtiquetaAplicacao" EnablePaging="True"
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize" 
                    SelectMethod="PesquisarEtiquetaAplicacoes" 
                    SelectByKeysMethod="ObtemEtiquetaAplicacao"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IEtiquetaFluxo" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.EtiquetaAplicacao"
                    UpdateMethod="SalvarEtiquetaAplicacao"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
