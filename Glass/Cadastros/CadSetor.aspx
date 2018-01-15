<%@ Page Title="Setores da Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSetor.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSetor" %>

<%@ Register Src="../Controls/ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function exibirBenef() {
            var botao = FindControl("lnkBenef", "a");
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('benefSetor', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamentos', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    FIX, [botao, 9 - getTableWidth('benefSetor'), -41 - getTableHeight('benefSetor')]);
            }
        }

        function atualizaFornada(control, isForno) {

            var tr = control.parentElement.parentElement;
            var gerenciarFornada = FindControl("chkGerenciarFornada", "input", tr);
            var forno = FindControl("chkForno", "input", tr);

            if (!isForno && !forno.checked)
                alert('Para habilitar o gerenciamento de fornada, a opção forno deve estar marcada.');

            if (!forno.checked)
                gerenciarFornada.checked = false;
        }

        function openRptAbrirFornada() {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AberturaFornada");
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSetor" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsSetor" DataKeyNames="IdSetor" OnRowCommand="grdSetor_RowCommand" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este Setor?');"
                                    Visible='<%# ((int)Eval("IdSetor")) > 1 %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfNumSeq" runat="server" Value='<%# Bind("NumeroSequencia") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sigla" SortExpression="Sigla">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSigla" runat="server" Columns="11" MaxLength="10"
                                    Text='<%# Bind("Sigla") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSigla" runat="server" ValidationGroup="c"
                                    ControlToValidate="txtSigla" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSigla" runat="server" Columns="11" MaxLength="10"
                                    Text='<%# Bind("Sigla") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSigla" runat="server" ValidationGroup="c"
                                    ControlToValidate="txtSigla" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("Sigla") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                    Width="120px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                    Width="120px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server">
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server"
                                    DataSourceID="odsTiposSetor" DataTextField="Translation" DataValueField="Key"
                                    SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Tipo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server"
                                    DataSourceID="odsTiposSetor" DataTextField="Translation" DataValueField="Key"
                                    SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entrada Estoque" SortExpression="EntradaEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkEntradaEstoque" runat="server" Checked='<%# Bind("EntradaEstoque") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEntradaEstoque" runat="server" Checked='<%# Bind("EntradaEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkEntradaEstoque" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Impedir Avanço" SortExpression="ImpedirAvanco">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("ImpedirAvanco") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkImpedirAvanco" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("ImpedirAvanco") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Informar Rota" SortExpression="InformarRota">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkInformarRota" runat="server" Checked='<%# Eval("InformarRota") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkInformarRota" runat="server" Checked='<%# Bind("InformarRota") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkInformarRota" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Informar Cavalete" SortExpression="InformarCavalete">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkInformarCavalete" runat="server" Checked='<%# Eval("InformarCavalete") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkInformarCavalete" runat="server" Checked='<%# Bind("InformarCavalete") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkInformarCavalete" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Corte?" SortExpression="Corte">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("Corte") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("Corte") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkCorte" runat="server" Checked='<%# Bind("Corte") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forno?" SortExpression="Forno">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkForno" runat="server" Checked='<%# Bind("Forno") %>' onchange="atualizaFornada(this, true);" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkForno" runat="server" onchange="atualizaFornada(this, true);" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkForno" runat="server" Checked='<%# Bind("Forno") %>' Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Laminado?" SortExpression="Laminado">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkLaminado" runat="server" Checked='<%# Bind("Laminado") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkLaminado" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkLaminado" runat="server" Checked='<%# Bind("Laminado") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Setores Lidos" SortExpression="ExibirSetores">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Eval("ExibirSetores") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkSetoresLidos" runat="server" Checked='<%# Bind("ExibirSetores") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkSetoresLidos" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Imagem Completa" SortExpression="ExibirImagemCompleta">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Eval("ExibirImagemCompleta") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("ExibirImagemCompleta") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkImagemCompleta" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Consultar Antes?" SortExpression="ConsultarAntes">
                            <ItemTemplate>
                                <asp:CheckBox ID="cboConsultarAntesItem" runat="server" Checked='<%# Bind("ConsultarAntes") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cboConsultarAntesEdit" runat="server" Checked='<%# Bind("ConsultarAntes") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="cboConsultarAntes" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir no Relatório?" SortExpression="ExibirRelatorio">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("ExibirRelatorio") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("ExibirRelatorio") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirRelatorio" runat="server" Checked='<%# Bind("ExibirRelatorio") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cor" SortExpression="DescrCor">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Cor")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCor" runat="server" SelectedValue='<%# Bind("Cor") %>' DataSourceID="odsCorSetor"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCor" runat="server" DataSourceID="odsCorSetor"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cor Tela" SortExpression="CorTela">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCorTela" runat="server" SelectedValue='<%# Bind("CorTela") %>'
                                    DataSourceID="odsCoresTela" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCorTela" runat="server"
                                    DataSourceID="odsCoresTela" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("CorTela")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tempo Login" SortExpression="TempoLogin">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("TempoLogin") %>'></asp:Label>
                                <asp:Label ID="Label7" runat="server" Text="min"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtTempoLogin" runat="server" MaxLength="2" Text='<%# Bind("TempoLogin") %>'
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text="min"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table cellpadding="0" cellspacing="0" class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtTempoLogin" runat="server" MaxLength="2" Width="50px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text="min"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desafio Perda" SortExpression="DesafioPerda">
                            <ItemTemplate>
                                <asp:Label ID="lblDesafioPerda" runat="server" Text='<%# Bind("DesafioPerda") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDesafioPerda" runat="server" MaxLength="12" Text='<%# Bind("DesafioPerda") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDesafioPerda" runat="server" MaxLength="12" Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Meta Perda" SortExpression="MetaPerda">
                            <ItemTemplate>
                                <asp:Label ID="lblMetaPerda" runat="server" Text='<%# Bind("MetaPerda") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMetaPerda" runat="server" MaxLength="12" Text='<%# Bind("MetaPerda") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtMetaPerda" runat="server" MaxLength="12" Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Capacidade diária de produção (m²)"
                            SortExpression="CapacidadeDiaria">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCapacidadeDiaria" runat="server"
                                    Text='<%# Bind("CapacidadeDiaria") %>' Width="80px"
                                    onkeypress="return soNumeros(event, true, true)"
                                    Visible='<%# ((int)Eval("IdSetor")) > 1 %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCapacidadeDiaria" runat="server" Width="80px"
                                    onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CapacidadeDiaria") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tempo alerta inatividade (minutos)">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTempoInatividade" runat="server" Text='<%# Bind("TempoAlertaInatividade") %>' Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtTempoInatividade" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTempoInatividade" runat="server" Text='<%# Bind("TempoAlertaInatividade") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gerenciar fornada?" SortExpression="GerenciarFornada">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkGerenciarFornada" runat="server" Checked='<%# Bind("GerenciarFornada") %>' onchange="atualizaFornada(this, false);" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkGerenciarFornada" runat="server" Checked='<%# Bind("GerenciarFornada") %>' onchange="atualizaFornada(this, false);" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkGerenciarFornada" runat="server" Checked='<%# Bind("GerenciarFornada") %>' Enabled="false" />
                                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptAbrirFornada();" Visible='<%# (bool)Eval("GerenciarFornada") %>' 
                                    ToolTip="Etiqueta de abertura/fechamento de fornada"> 
                                    <img alt="" border="0" src="../Images/printer.png" />
                                </asp:LinkButton>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>' Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTempoIntxtLarguraatividade" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ignorar Capacidade diária de produção" SortExpression="IgnorarCapacidadeDiaria">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkIgnorarCapacidadeDiaria" runat="server" Checked='<%# Bind("IgnorarCapacidadeDiaria") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkIgnorarCapacidadeDiaria" runat="server" Checked='<%# Bind("IgnorarCapacidadeDiaria") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkIgnorarCapacidadeDiaria" runat="server" Checked='<%# Bind("IgnorarCapacidadeDiaria") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Permitir Leitura fora do Roteiro" SortExpression="PermitirLeituraForaRoteiro">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkPermitirLeituraForaRoteiro" runat="server" Checked='<%# Bind("PermitirLeituraForaRoteiro") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkPermitirLeituraForaRoteiro" runat="server" Checked='<%# Bind("PermitirLeituraForaRoteiro") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkPermitirLeituraForaRoteiro" runat="server" Checked='<%# Bind("PermitirLeituraForaRoteiro") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir no Painel Comercial" SortExpression="ExibirPainelComercial">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirPainelComercial" runat="server" Checked='<%# Bind("ExibirPainelComercial") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExibirPainelComercial" runat="server" Checked='<%# Bind("ExibirPainelComercial") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirPainelComercial" runat="server" Checked='<%# Bind("ExibirPainelComercial") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir no Painel da Produção" SortExpression="ExibirPainelProducao">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirPainelProducao" runat="server" Checked='<%# Bind("ExibirPainelProducao") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExibirPainelProducao" runat="server" Checked='<%# Bind("ExibirPainelProducao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirPainelProducao" runat="server" Checked='<%# Bind("ExibirPainelProducao") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(); return false;">
                                    <img border="0" src="../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="benefSetor" style="display: none">
                                    <tr>
                                        <td>
                                            <uc1:ctrlBenefSetor ID="ctrlBenefSetor1" runat="server" FuncaoExibir="exibirBenef"
                                                Beneficiamentos='<%# ((IList<Glass.PCP.Negocios.Entidades.SetorBenef>)Eval("SetorBeneficiamentos")).Select(f => (uint)f.IdBenefConfig).ToList() %>' />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(); return false;">
                                    <img border="0" src="../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="benefSetor" style="display: none">
                                    <tr>
                                        <td>
                                            <uc1:ctrlBenefSetor ID="ctrlBenefSetor1" runat="server" FuncaoExibir="exibirBenef" />
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("IdSetor") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# ((int)Eval("IdSetor")) > 1 %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" ValidationGroup="c">
                                     <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("IdSetor") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" Visible='<%# ((int)Eval("IdSetor")) > 1 %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdSetor") %>'
                                    Tabela="Setor" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Label ID="Label8" runat="server" Font-Italic="True" Text="Tempo de login 0 (zero) é ilimitado"></asp:Label>
                <colo:VirtualObjectDataSource runat="server" Culture="pt-BR" ID="odsTiposSetor"
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                            DefaultValue="Glass.Data.Model.TipoSetor, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource runat="server" Culture="pt-BR" ID="odsCoresTela"
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                            DefaultValue="Glass.Data.Model.CorTelaSetor, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorSetor" runat="server"
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                            DefaultValue="Glass.Data.Model.CorSetor, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarSetores"
                    SelectByKeysMethod="ObtemSetor"
                    SortParameterName="sortExpression"
                    DeleteMethod="ApagarSetor"
                    DeleteStrategy="GetAndDelete"
                    TypeName="Glass.PCP.Negocios.ISetorFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.Setor"
                    InsertMethod="SalvarSetor"
                    UpdateMethod="SalvarSetor"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
