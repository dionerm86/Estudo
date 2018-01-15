<%@ Page Title="Cadastro de Loja" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadLoja.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadLoja" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function iniciaPesquisaCep(cep)
        {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");
            var idCidade = FindControl("hdfCidade", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null, idCidade);
        }

        function setCidade(idCidade, nomeCidade) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }

        function setEmails() {
            var idLoja = '<%= Request["idLoja"] %>';
            openWindow(500, 500, "../Utils/SetLojaEmail.aspx?idLoja=" + idLoja);
        }

        function bloquearEspeciais(e) {
            if (!((e.key >= 'a' && e.key <= 'z') || (e.key >= 'A' && e.key <= 'Z')) &&
                isNaN(parseFloat(e.key))) {
                e.returnValue = false;
            }
        }

    </script>

    <asp:DetailsView ID="dtvLoja" runat="server" AutoGenerateRows="False" DataKeyNames="IdLoja"
        DataSourceID="odsLoja" DefaultMode="Insert" GridLines="None" CellPadding="4" OnLoad="dtvLoja_Load" >
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
        <RowStyle CssClass="dtvAlternatingRow" />
        <FieldHeaderStyle Font-Bold="False" CssClass="dtvHeader" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <Fields>
            <asp:TemplateField HeaderText="Razão Social" SortExpression="RazaoSocial">
                <EditItemTemplate>
                    <asp:TextBox ID="txtRazaoSocial" runat="server" MaxLength="50" Text='<%# Bind("RazaoSocial") %>'
                        Width="300px"></asp:TextBox>
                    &nbsp;<asp:RequiredFieldValidator ID="rqdRazaoSocial" runat="server" ControlToValidate="txtRazaoSocial"
                        ErrorMessage="Informe a Razão Social" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("RazaoSocial") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Nome Fantasia" SortExpression="NomeFantasia">
                <EditItemTemplate>
                    <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="50" Text='<%# Bind("NomeFantasia") %>'
                        Width="300px"></asp:TextBox>
                    &nbsp;<asp:RequiredFieldValidator ID="rqdNomeFantasia" runat="server" ControlToValidate="txtNomeFantasia"
                        ErrorMessage="Informe o Nome Fantasia" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label11" runat="server" Text='<%# Bind("NomeFantasia") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpTipoLoja" runat="server" SelectedValue='<%# Bind("Tipo") %>'
                        DataSourceID="odsTipoLoja" DataValueField="Key" DataTextField="Translation" AppendDataBoundItems="true">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label16" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'
                        DataSourceID="odsSituacaoLoja" DataValueField="Key" DataTextField="Translation">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label22" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Endereço" SortExpression="Endereco">
                <EditItemTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:TextBox ID="txtEndereco" runat="server" MaxLength="50" Text='<%# Bind("Endereco") %>'
                                    Width="250px"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;N.º&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtNumero" runat="server" onKeyPress='bloquearEspeciais(event)' MaxLength="10" Text='<%# Bind("Numero") %>'
                                    Width="50px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Endereco") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Complemento" SortExpression="Compl">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCompl" runat="server" MaxLength="20" Text='<%# Bind("Compl") %>'
                        Width="150px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Compl") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Bairro" SortExpression="Bairro">
                <EditItemTemplate>
                    <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' MaxLength="50"
                        Width="200px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Bairro") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cidade" SortExpression="Cidade">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCidade" runat="server" Text='<%# Eval("Cidade.NomeCidade") %>' ReadOnly="true"
                        Width="200px"></asp:TextBox>
                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                    <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("Cidade.NomeCidade") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CEP" SortExpression="Cep">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCep" runat="server" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                        onkeydown="return maskCep(event, this);" MaxLength="9" Width="100px"></asp:TextBox>
                    <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                        OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Telefones" SortExpression="Telefone">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("Telefone") %>' onkeypress="return soTelefone(event)"
                        onkeydown="return maskTelefone(event, this);" MaxLength="14" Width="100px"></asp:TextBox>
                    <asp:TextBox ID="TextBox81" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                        onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Telefone2") %>'
                        Width="100px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("Telefone") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fax" SortExpression="Fax">
                <EditItemTemplate>
                    <asp:TextBox ID="txtFax" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                        onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Fax") %>' Width="100px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label15" runat="server" Text='<%# Bind("Fax") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Site" SortExpression="Site">
                <ItemTemplate>
                    <asp:Label ID="Label12" runat="server" Text='<%# Bind("Site") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" Text='<%# Bind("Site") %>'
                        Width="200px"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CNPJ" SortExpression="Cnpj">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCnpj" runat="server" MaxLength="18" Text='<%# Bind("Cnpj") %>'
                        Width="150px" onkeypress="maskCNPJ(event, this)"></asp:TextBox>
                    <asp:CustomValidator ID="cvlCnpj" runat="server" ClientValidationFunction="validarCnpj"
                        ControlToValidate="txtCnpj" ErrorMessage="CNPJ inválido."></asp:CustomValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("Cnpj") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Insc. Estadual" SortExpression="InscEst">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("InscEst") %>' MaxLength="15"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("InscEst") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Insc. Municipal" SortExpression="InscMunic">
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("InscMunic") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" MaxLength="20" Text='<%# Bind("InscMunic") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="SUFRAMA" SortExpression="Suframa">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox11" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label21" runat="server" Text='<%# Bind("Suframa") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CNAE" SortExpression="Cnae">
                <ItemTemplate>
                    <asp:Label ID="Label13" runat="server" Text='<%# Bind("Cnae") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox5" runat="server" MaxLength="15" Text='<%# Bind("Cnae") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CRT" SortExpression="Crt">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCrt" runat="server" SelectedValue='<%# Bind("Crt") %>' AppendDataBoundItems="true"
                        DataSourceID="odsCrtLoja" DataValueField="Key" DataTextField="Translation">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label14" runat="server" Text='<%# Bind("Crt") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Regime" SortExpression="RegimeLoja">
                <ItemTemplate>
                    <asp:Label ID="Label17" runat="server" Text='<%# Bind("RegimeLoja") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="drpRegimeLoja" runat="server" SelectedValue='<%# Bind("RegimeLoja") %>'
                        AppendDataBoundItems="True" DataSourceID="odsRegimeLoja" DataTextField="Translation" DataValueField="Key">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label16" runat="server" Text='<%# Bind("RegimeLoja") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tipo de Atividade" SortExpression="TipoAtividade">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpTipoAtividade" runat="server" SelectedValue='<%# Bind("TipoAtividade") %>'
                        DataSourceID="odsTipoAtividade" DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="True">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label18" runat="server" Text='<%# Bind("TipoAtividade") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Núm. Inscrição Junta Comercial" SortExpression="NIRE">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("NIRE") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("NIRE") %>'></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label19" runat="server" Text='<%# Bind("NIRE") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Data Inscrição Junta Comercial" SortExpression="DataNIRE">
                <EditItemTemplate>
                    <uc1:ctrlData ID="txtDataNIREEdit" runat="server" DataNullable='<%# Bind("DataNIRE") %>'
                        ReadOnly="ReadWrite" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <uc1:ctrlData ID="txtDataNIREInsert" runat="server" DataNullable='<%# Bind("DataNIRE") %>'
                        ReadOnly="ReadWrite" />
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label20" runat="server" Text='<%# Bind("DataNIRE") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="RNTRC" SortExpression="RNTRC">
                <EditItemTemplate>
                    <asp:TextBox ID="txtRNTRC" runat="server" Text='<%# Bind("RNTRC") %>' MaxLength="8"></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtRNTRC" runat="server" Text='<%# Bind("RNTRC") %>' MaxLength="8"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblRNTRC" runat="server" Text='<%# Bind("RNTRC") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Identificador CSC" SortExpression="IdCsc">
                <EditItemTemplate>
                    <asp:TextBox ID="txtIdCSC" runat="server" Text='<%# Bind("IdCsc") %>' MaxLength="6"></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtIdCSC" runat="server" Text='<%# Bind("IdCsc") %>' MaxLength="6"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIdCSC" runat="server" Text='<%# Bind("IdCsc") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CSC" SortExpression="CSC">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCSC" runat="server" Text='<%# Bind("CSC") %>' MaxLength="36"></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtCSC" runat="server" Text='<%# Bind("CSC") %>' MaxLength="36"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblCSC" runat="server" Text='<%# Bind("CSC") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Configurações Disponíveis Da Loja">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCorEspesura" runat="server" Checked='<%# Bind("IgnorarBloquearItensCorEspessura") %>' Text="Ignorar configuração 'Bloquear itens de cor e espessura diferentes (tipo Venda)' (Configurações > aba Pedido)" />
                    <br />
                    <asp:CheckBox ID="chkProdutosProntos" runat="server" Checked='<%# Bind("IgnorarLiberarApenasProdutosProntos") %>' Text="Ignorar configuração 'Liberar apenas produtos prontos' (Configurações > aba Liberação)" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="chkCorEspesura" runat="server" Checked='<%# Bind("IgnorarBloquearItensCorEspessura") %>' Text="Ignorar configuração 'Bloquear itens de cor e espessura diferentes (tipo Venda)' (Configurações > aba Pedido)" />
                    <br />
                    <asp:CheckBox ID="chkProdutosProntos" runat="server" Checked='<%# Bind("IgnorarLiberarApenasProdutosProntos") %>' Text="Ignorar configuração 'Liberar apenas produtos prontos' (Configurações > aba Liberação)" />
                </InsertItemTemplate>
                <ItemTemplate>
                     <asp:Label ID="lblCorEspesura" runat="server" Text='<%# Bind("IgnorarBloquearItensCorEspessura")  %>'></asp:Label>
                     <asp:Label ID="lblProdutosProntos" runat="server" Text='<%# Bind("IgnorarLiberarApenasProdutosProntos")  %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Impostos Pedido">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsPedido" runat="server" Checked='<%# Bind("CalcularIcmsPedido") %>' Text="Calcular ICMS no pedido" />
                    <br />
                    <asp:CheckBox ID="chkCalcularIpiPedido" runat="server" Checked='<%# Bind("CalcularIpiPedido") %>' Text="Calcular IPI no pedido" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsPedido" runat="server" Checked='<%# Bind("CalcularIcmsPedido") %>' Text="Calcular ICMS no pedido" />
                    <br />
                    <asp:CheckBox ID="chkCalcularIpiPedido" runat="server" Checked='<%# Bind("CalcularIpiPedido") %>' Text="Calcular IPI no pedido" />
                </InsertItemTemplate>
                <ItemTemplate>
                     <asp:Label ID="lblCalcularIcmsPedido" runat="server" Text='<%# ((bool?)Eval("CalcularIcmsPedido")).GetValueOrDefault() ? "Sim" : "Não"  %>'></asp:Label>
                     <asp:Label ID="lblCalcularIpiPedido" runat="server" Text='<%# ((bool?)Eval("CalcularIpiPedido")).GetValueOrDefault() ? "Sim" : "Não"  %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Impostos Liberação">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsLiberacao" runat="server" Checked='<%# Bind("CalcularIcmsLiberacao") %>' Text="Calcular ICMS na liberação" />
                    <br />
                    <asp:CheckBox ID="chkCalcularIpiLiberacao" runat="server" Checked='<%# Bind("CalcularIpiLiberacao") %>' Text="Calcular IPI na liberação" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsLiberacao" runat="server" Checked='<%# Bind("CalcularIcmsLiberacao") %>' Text="Calcular ICMS na liberação" />
                    <br />
                    <asp:CheckBox ID="chkCalcularIpiLiberacao" runat="server" Checked='<%# Bind("CalcularIpiLiberacao") %>' Text="Calcular IPI na liberação" />
                </InsertItemTemplate>
                <ItemTemplate>
                     <asp:Label ID="lblCalcularIcmsLiberacao" runat="server" Text='<%# ((bool?)Eval("CalcularIcmsLiberacao")).GetValueOrDefault() ? "Sim" : "Não"  %>'></asp:Label>
                     <asp:Label ID="lblCalcularIpiLiberacao" runat="server" Text='<%# ((bool?)Eval("CalcularIpiLiberacao")).GetValueOrDefault() ? "Sim" : "Não"  %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Imagens Da Loja">
                <InsertItemTemplate>
                    <asp:Label ID="Label10" runat="server" Text="Insira a loja para alterar as suas imagens."></asp:Label>
                </InsertItemTemplate>
                <EditItemTemplate>
                    <table>
                        <tr>
                            <td align="left" class="dtvHeader">
                                <asp:Label ID="Label10" runat="server" Text="Logo da loja (Colorido)"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:FileUpload ID="filImagemCor" runat="server" accept="image/png" ToolTip="Só são aceitos arquivos (.png)" />
                                <uc4:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Loja.LojaRepositorioImagens.Instance.ObtemUrl((int)Eval("IdLoja"), true) %>' />
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader">
                                <asp:Label ID="Label23" runat="server" Text="Logo da loja (Sem Cor)"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:FileUpload ID="filImagemSemCor" runat="server" accept="image/png" ToolTip="Só são aceitos arquivos (.png)" />
                                <uc4:ctrlImagemPopup ID="ctrlImagemPopup2" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Loja.LojaRepositorioImagens.Instance.ObtemUrl((int)Eval("IdLoja"), false) %>' />
                            </td>
                        </tr>
                    </table>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                    <asp:Button ID="btnEmails" runat="server" OnClientClick="setEmails(); return false;"
                        Text="eMails" CausesValidation="false" />
                    <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                        CausesValidation="false" />
                    <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdLoja"
                        Text='<%# Bind("IdLoja") %>' />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" />
                    <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                        CausesValidation="false" />
                </InsertItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>            
        </Fields>
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <InsertRowStyle HorizontalAlign="Left" />
        <EditRowStyle HorizontalAlign="Left" BackColor="White" />
        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
    </asp:DetailsView>
    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Loja"
        SelectMethod="ObtemLoja" 
        SelectByKeysMethod="ObtemLoja"
        TypeName="Glass.Global.Negocios.ILojaFluxo"
        InsertMethod="SalvarLoja" OnInserted="odsLoja_Inserted"
        UpdateMethod="SalvarLoja" OnUpdated="odsLoja_Updated"
        UpdateStrategy="GetAndUpdate">
        <SelectParameters>
            <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoLoja" runat="server" SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoLoja, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoLoja" runat="server" SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Situacao, Glass.Comum" />
            <asp:Parameter Name="groupKey" DefaultValue="fem" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCrtLoja" runat="server" SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.CrtLoja, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRegimeLoja" runat="server" SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Sync.Fiscal.Enumeracao.Loja.RegimeLoja, Sync.Fiscal.Comum" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoAtividade" runat="server" SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Sync.Fiscal.Enumeracao.Loja.TipoAtividadeContribuicoes, Sync.Fiscal.Comum" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
