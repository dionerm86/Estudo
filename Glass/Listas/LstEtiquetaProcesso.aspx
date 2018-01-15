<%@ Page Title="Processos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaProcesso.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaProcesso" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno) {
            FindControl("txtAplIns", "input").value = codInterno;
            FindControl("hdfIdAplicacao", "input").value = idAplicacao;
        }

        function loadApl(codInterno) {
            if (codInterno == undefined || codInterno == "") {
                setApl("", "");
                return false;
            }

            try {
                var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

                if (response == null || response == "") {
                    alert("Falha ao buscar Aplicação. Ajax Error.");
                    setApl("", "");
                    return false
                }

                response = response.split("\t");

                if (response[0] == "Erro") {
                    alert(response[1]);
                    setApl("", "");
                    return false;
                }

                setApl(response[1], response[2]);
            }
            catch (err) {
                alert(err);
            }
        }

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
                <asp:GridView GridLines="None" ID="grdProcesso" runat="server" SkinID="gridViewEditable" 
                    DataSourceID="odsProcesso" DataKeyNames="IdProcesso" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
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
                            <HeaderStyle HorizontalAlign="Left" />
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
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aplicação" SortExpression="DescrAplicacao">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodInternoAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescricaoAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Destacar na Etiqueta?" 
                            SortExpression="DestacarEtiqueta">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkDestEtiq" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkDestEtiq" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkDestacar" runat="server" 
                                    Checked='<%# Bind("DestacarEtiqueta") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Left" />
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
                                    Checked='<%# Bind("GerarFormaInexistente") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Eval("GerarFormaInexistente") %>' Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gerar Arquivo de Mesa" 
                            SortExpression="GerarArquivoDeMesa">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkGerarArquivoDeMesa" runat="server" 
                                    Checked='<%# Bind("GerarArquivoDeMesa") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkGerarArquivoDeMesa" runat="server" 
                                    Checked='<%# Bind("GerarArquivoDeMesa") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkGerarArquivoDeMesa" runat="server" 
                                    Checked='<%# Eval("GerarArquivoDeMesa") %>' Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número dias úteis data entrega">
                            <ItemTemplate>
                                <asp:Label ID="lblDiasEntrega" runat="server" Text='<%# Eval("NumeroDiasUteisDataEntrega") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDiasEntrega" runat="server" MaxLength="10" Text='<%# Bind("NumeroDiasUteisDataEntrega") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDiasEntrega" runat="server" MaxLength="10" Text='<%# Bind("NumeroDiasUteisDataEntrega") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo">
                            <ItemTemplate>
                            <asp:Label ID="Label33" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoProcesso")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                             <asp:DropDownList ID="drpTipoProcesso" runat="server" 
                                               SelectedValue='<%# Bind("TipoProcesso") %>'>
                                    <asp:ListItem Value="" Text="" />
                                    <asp:ListItem Value="Instalacao">Instalação</asp:ListItem>
                                    <asp:ListItem Value="Caixilho">Caixilho</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoProcesso" runat="server">
                                    <asp:ListItem Value="" Text = "" />
                                    <asp:ListItem Value="Instalacao">Instalação</asp:ListItem>
                                    <asp:ListItem Value="Caixilho">Caixilho</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
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
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label> &nbsp&nbsp&nbsp
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Processo" IdRegistro='<%# (uint)(int)Eval("IdProcesso") %>' />
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
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>                
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProcesso" runat="server" 
                    DeleteMethod="ApagarEtiquetaProcesso" EnablePaging="True"
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarEtiquetaProcessos" SortParameterName="sortExpression"
                    SelectByKeysMethod="ObtemEtiquetaProcesso"
                    TypeName="Glass.Global.Negocios.IEtiquetaFluxo" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.EtiquetaProcesso"
                    UpdateMethod="SalvarEtiquetaProcesso"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
