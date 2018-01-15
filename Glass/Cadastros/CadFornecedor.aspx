<%@ Page Title="Cadastro de Fornecedor" Language="C#" MasterPageFile="~/Painel.master" EnableViewState="false"
    AutoEventWireup="true" CodeBehind="CadFornecedor.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFornecedor" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelasUsar.ascx" TagName="ctrlParcelasUsar" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .escondeUrl
        {
            display: none;
        }
    </style>

    <script type="text/javascript">

        function iniciarPesquisaCep(cep) {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");
            var idCidade = FindControl("hdfCidade", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null, idCidade);

            if (logradouro != null &&
                logradouro.value != null &&
                logradouro.value.length >= 100)
                logradouro.value = logradouro.value.toString().substring(0, 100);
        }

        function isExterior() {
            return MetodosAjax.IsCidadeExterior(FindControl("hdfCidade", "input").value).value == "true";
        }

        function validaCpfCnpj(val, args) {
            if (isExterior())
                args.IsValid = args.Value == "";
            else
                validarCpfCnpj(val, args);
        }

        function onInsert() {
            if (!verificaCampos())
                return false;

            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

            if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;
            else if (!isExterior() && CadFornecedor.CheckIfExists(cpfCnpj).value == "true") {
                alert("Já existe um fornecedor cadastrado com o CPF/CNPJ informado.");
                return false;
            }

            return true;
        }

        function onUpdate() {
            if (!verificaCampos())
                return false;
                
            if (!isExterior() && CadFornecedor.ValidaCpfCnpj(FindControl("hdfIdFornec", "input").value, FindControl("txtCpfCnpj", "input").value).value == "false") {
                alert("Este CPF/CNPJ já está cadastrado no sistema para outro fornecedor.");
                return false;
            }

            if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;

            var logradouro = FindControl("txtEndereco", "input");

            if (logradouro != null &&
                logradouro.value != null &&
                logradouro.value.length >= 100)
                logradouro.value = logradouro.value.toString().substring(0, 100);

            return true;
        }

        function verificaCampos() {
            if (FindControl("txtNomeFantasia", "input").value == "") {
                alert("Informe o nome do Fornecedor.");
                return false;
            }

            if (FindControl("hdfCidade", "input").value == "") {
                alert("Informe a cidade do Fornecedor.");
                return false;
            }

            var logradouro = FindControl("txtEndereco", "input");

            if (logradouro != null && logradouro.value == "") {
                alert("Informe o endereço do Fornecedor.");
                return false;
            }

            if (logradouro != null &&
                logradouro.value != null &&
                logradouro.value.length >= 100)
                logradouro.value = logradouro.value.toString().substring(0, 100);

            if (FindControl("txtNum", "input").value == "") {
                alert("Informe o número do endereço do Fornecedor.");
                return false;
            }

            if (FindControl("txtBairro", "input").value == "") {
                alert("Informe o bairro do Fornecedor.");
                return false;
            }

            var telefone = FindControl("txtTelefone", "input").value;
            if (telefone == "" || telefone.length < 10) {
                alert("Informe o telefone do Fornecedor.");
                return false;
            }

            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

            if (!isExterior() && cpfCnpj == "" || cpfCnpj == "000.000.000-00" || cpfCnpj == "00.000.000/0000-00") {
                alert("Informe o CPF/CNPJ do fornecedor.");
                return false;
            }

            var urlSistema = FindControl("txtUrlSistema", "input");
            var alterarUrlSistema = urlSistema != null;
            urlSistema = alterarUrlSistema ? urlSistema.value : "";

            if (urlSistema != "" && urlSistema.toUpperCase().indexOf("WEBGLASS") == -1) {
                alert("A URL do Sistema é inválida.");
                return false;
            }
            else if (alterarUrlSistema)
                FindControl("hdfUrlSistema", "input").value = urlSistema;
            
            return true;
        }

        function drpTipoPessoaChanged() {
            if (getTipoPessoa() == "J")
                FindControl("txtCpfCnpj", "input").maxLength = 18;
            else
                FindControl("txtCpfCnpj", "input").maxLength = 14;
        }

        function getTipoPessoa() {
            return FindControl("ddlTipoPessoa", "select").value;
        }

        function setCidade(idCidade, nomeCidade, uf) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
            FindControl('lblUf', 'span').innerHTML = uf;
        }
        
        function retornaPagina()
        {
            window.history.go(-1);
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvFornecedor" runat="server" AutoGenerateRows="False" DataSourceID="odsFornecedor"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" DataKeyNames="IdFornec"
                    CellPadding="4" Style="margin-right: 2px">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle CssClass="dtvAlternatingRow" />
                    <FieldHeaderStyle Wrap="False" Font-Bold="False" CssClass="dtvHeader" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Tipo Pessoa" SortExpression="TipoPessoa">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoPessoa" runat="server" SelectedValue='<%# Bind("TipoPessoa") %>'
                                    onchange="drpTipoPessoaChanged();">
                                    <asp:ListItem Value="F">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="J">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                                <asp:CheckBox ID="chkProdutorRural" runat="server" onclick="drpTipoPessoaChanged(true)"
                                    Checked='<%# Bind("ProdutorRural") %>' Text="Produtor Rural" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoPessoa" runat="server" SelectedValue='<%# Bind("TipoPessoa") %>'
                                    onchange="drpTipoPessoaChanged();">
                                    <asp:ListItem Value="F">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="J" Selected="True">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                                <asp:CheckBox ID="chkProdutorRural" runat="server" onclick="drpTipoPessoaChanged(true)"
                                    Checked='<%# Bind("ProdutorRural") %>' Text="Produtor Rural" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TipoPessoa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome Fantasia" SortExpression="NomeFantasia">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="120" Text='<%# Bind("NomeFantasia") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="120" Text='<%# Bind("NomeFantasia") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NomeFantasia") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Razão Social" SortExpression="RazaoSocial">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RazaoSocial") %>' MaxLength="120"
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RazaoSocial") %>' MaxLength="120"
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("RazaoSocial") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                    Width="150px" onkeypress="getTipoPessoa()=='J' ? maskCNPJ(event, this) : maskCPF(event, this);"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validaCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                    Width="150px" onkeypress="getTipoPessoa()=='J' ? maskCNPJ(event, this) : maskCPF(event, this);"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validaCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="RG/Insc. Est." SortExpression="RgInscEst">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtRgEsc" runat="server" MaxLength="22" Text='<%# Bind("RgInscEst") %>'
                                    Width="120px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtRgEsc" runat="server" MaxLength="22" Text='<%# Bind("RgInscEst") %>'
                                    Width="120px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("InscEst") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Documento Estrangeiro" SortExpression="PassaporteDoc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPassaporteDoc" runat="server" MaxLength="20" Text='<%# Bind("PassaporteDoc") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPassaporteDoc" runat="server" MaxLength="20" Text='<%# Bind("PassaporteDoc") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("PassaporteDoc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoFornecedor"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("Situacao") %>' OnDataBound="drpSituacao_DataBound">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoFornecedor"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("Situacao") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SUFRAMA" SortExpression="Suframa">
                            <ItemTemplate>
                                <asp:Label ID="Label19" runat="server" Text='<%# Bind("Suframa") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CRT" SortExpression="Crt">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCrt" runat="server" SelectedValue='<%# Bind("Crt") %>'>
                                    <asp:ListItem Value="RegimeNormal">Regime Normal</asp:ListItem>
                                    <asp:ListItem Value="SimplesNacional">Simples Nacional</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpCrt" runat="server" SelectedValue='<%# Bind("Crt") %>'>
                                    <asp:ListItem Value="RegimeNormal">Regime Normal</asp:ListItem>
                                    <asp:ListItem Value="SimplesNacional">Simples Nacional</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label18" runat="server" Text='<%# Bind("Crt") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Endereço" SortExpression="Endereco">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEndereco" runat="server" Text='<%# Bind("Endereco") %>' MaxLength="100"
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtEndereco" runat="server" Text='<%# Bind("Endereco") %>' MaxLength="100"
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Endereco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Numero">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNum" runat="server" Text='<%# Bind("Numero") %>' MaxLength="20"
                                    Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNum" runat="server" Text='<%# Bind("Numero") %>' MaxLength="20"
                                    Width="100px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Compl") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Complemento" SortExpression="Compl">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("Compl") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCompl" runat="server" MaxLength="100" Text='<%# Bind("Compl") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bairro" SortExpression="Bairro">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' MaxLength="50"
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' MaxLength="50"
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Bairro") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade" SortExpression="Cidade">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtCidade" runat="server" ReadOnly="true" Style="margin-right: 0px"
                                                Text='<%# Eval("Cidade.NomeCidade") %>' Width="200px"></asp:TextBox>
                                        </td>
                                        <td class="dtvHeader">
                                            UF
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUf" runat="server" Text='<%# Eval("Cidade.NomeUf") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtCidade" runat="server" ReadOnly="true" Style="margin-right: 0px"
                                                Text='<%# Eval("Cidade") %>' Width="200px"></asp:TextBox>
                                        </td>
                                        <td class="dtvHeader">
                                            UF
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUf" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Cidade") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="País" SortExpression="IdPais">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPais" runat="server" DataSourceID="odsPais" DataTextField="NomePais"
                                    DataValueField="IdPais" SelectedValue='<%# Bind("IdPais") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpPais" runat="server" DataSourceID="odsPais" DataTextField="NomePais"
                                    DataValueField="IdPais" SelectedValue='<%# Bind("IdPais") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Bind("IdPais") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CEP" SortExpression="Cep">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" MaxLength="9" Width="100px"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciarPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" Text='<%# Bind("Cep") %>' Width="100px"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciarPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Telefone" SortExpression="TelCont">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTelefone" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Telcont") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtTelefone" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Telcont") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fax" SortExpression="Fax">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Fax") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Fax") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Fax") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="60" Text='<%# Bind("Email") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="60" Text='<%# Bind("Email") %>'
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendedor" SortExpression="Vendedor">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox12" runat="server" Text='<%# Bind("Vendedor") %>' MaxLength="35"
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox12" runat="server" Text='<%# Bind("Vendedor") %>' MaxLength="35"
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("Vendedor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cel. Vend" SortExpression="TelCelVend">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("TelCelVend") %>' onkeypress="maskTelefone(event, this);"
                                    MaxLength="15"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("TelCelVend") %>' onkeypress="maskTelefone(event, this);"
                                    MaxLength="15"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("TelCelVend") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contato" SortExpression="Contato">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox32" runat="server" Text='<%# Bind("Contato") %>' MaxLength="35"
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox32" runat="server" Text='<%# Bind("Contato") %>' MaxLength="35"
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("Contato") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email Contato" SortExpression="EmailContato">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox35" runat="server" MaxLength="60" Text='<%# Bind("EmailContato") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox35" runat="server" MaxLength="60" Text='<%# Bind("EmailContato") %>'
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label63" runat="server" Text='<%# Bind("EmailContato") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tel. Contato" SortExpression="TelContato">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox33" runat="server" Text='<%# Bind("TelContato") %>' onkeypress="maskTelefone(event, this);"
                                    MaxLength="15"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox33" runat="server" Text='<%# Bind("TelContato") %>' onkeypress="maskTelefone(event, this);"
                                    MaxLength="15"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label33" runat="server" Text='<%# Bind("TelContato") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="2" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="400px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="2" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="400px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta" SortExpression="IdConta">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                    SelectedValue='<%# Bind("IdConta") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                    SelectedValue='<%# Bind("IdConta") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta Contábil" SortExpression="IdContaContabil">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoContaContabil" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil"
                                    DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpPlanoContaContabil" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil"
                                    DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label27" runat="server" Text='<%# Bind("IdContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pagto." SortExpression="TipoPagto">
                            <EditItemTemplate>
                                <uc1:ctrlParcelasUsar ID="ctrlParcelasUsar1" runat="server"  IdFornec='<%# Bind("IdFornec") %>'  BloquearPagto='<%# Bind("BloquearPagto") %>'
                                    ParcelasNaoUsar='<%# Bind("Parcelas") %>' FormaPagtoPadrao='<%# Bind("TipoPagto") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlParcelasUsar ID="ctrlParcelasUsar1" runat="server" FormaPagtoPadrao='<%# Bind("TipoPagto") %>'
                                    ParcelasNaoUsar='<%# Bind("Parcelas") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("TipoPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Url do Sistema WebGlass" SortExpression="UrlSistema">
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Bind("UrlSistema") %>' OnLoad="UrlSistema_Load"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtUrlSistema" runat="server" Text='<%# Eval("UrlSistema") %>' Width="200px"
                                    OnLoad="UrlSistema_Load"></asp:TextBox>
                                <asp:HiddenField ID="hdfUrlSistema" runat="server" Value='<%# Bind("UrlSistema") %>' />
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtUrlSistema"
                                    ErrorMessage="Informe uma URL válida." SetFocusOnError="True" ValidationExpression="((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)"
                                    ValidationGroup="c" ToolTip="Informe uma URL válida." OnLoad="UrlSistema_Load">*</asp:RegularExpressionValidator>--%>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtUrlSistema" runat="server" Text='<%# Eval("UrlSistema") %>' Width="200px"
                                    OnLoad="UrlSistema_Load"></asp:TextBox>
                                <asp:HiddenField ID="hdfUrlSistema" runat="server" Value='<%# Bind("UrlSistema") %>' />
                               <%-- <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtUrlSistema"
                                    ErrorMessage="Informe uma URL válida." SetFocusOnError="True" ValidationExpression="((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)"
                                    ValidationGroup="c" ToolTip="Informe uma URL válida." OnLoad="UrlSistema_Load">*</asp:RegularExpressionValidator>--%>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vigência tab. de preços" SortExpression="DTVIGENCIAPRECO">
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Bind("DtVigenciaPrecoString") %>' OnLoad="DataVigenciaPreco_Load"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc3:ctrlData ID="ctrlDataVigenciaPreco" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataVigenciaPrecos") %>'
                                    OnLoad="DataVigenciaPreco_Load" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc3:ctrlData ID="ctrlDataVigenciaPreco" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataVigenciaPrecos") %>'
                                    OnLoad="DataVigenciaPreco_Load" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onUpdate()) return false;" ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdFornec"
                                    Text='<%# Bind("IdFornec") %>' />
                                <asp:HiddenField ID="hdfCredito" runat="server" Value='<%# Bind("Credito") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsert()) return false;"
                                    ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle ForeColor="Black" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Fornecedor"
                    TypeName="Glass.Global.Negocios.IFornecedorFluxo"
                    InsertMethod="SalvarFornecedor" 
                    SelectMethod="ObtemFornecedor" 
                    UpdateMethod="SalvarFornecedor"
                    UpdateStrategy="GetAndUpdate" OnInserted="odsFornecedor_Inserted" OnUpdated="odsFornecedor_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idFornec" QueryStringField="idFornec" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoFornecedor" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoFornecedor, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPais" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.PaisDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>                
                <asp:HiddenField ID="hdfIdFornec" runat="server" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        drpTipoPessoaChanged();
    </script>

    <asp:ValidationSummary ID="validationSummary" runat="server" DisplayMode="List" ShowMessageBox="true"
        ShowSummary="false" ValidationGroup="c" />
</asp:Content>
