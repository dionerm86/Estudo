<%@ Page Title="Cadastro de Transportadora" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadTransportador.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTransportador" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function alteraTipoPessoa(controle) {
            var contatos = document.getElementById("contatos");
            contatos.style.display = controle.value == "Juridica" ? "" : "none";
        }

        function onInsert() {
            if (FindControl("txtNome", "input").value == "") {
                alert("Informe o nome da Transportadora.");
                return false;
            }

            if (FindControl("hdfCidade", "input").value == "") {
                alert("Informe cidade da Transportadora.");
                return false;
            }

            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

            if (cpfCnpj == "") {
                alert("Informe o CPF/CNPJ da Transportadora.");
                return false;
            }
            if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;
            else if (CadTransportador.CheckIfExists(cpfCnpj).value == "true") {
                alert("Já existe uma transportadora cadastrada com o CPF/CNPJ informado.");
                return false;
            }
        }

        function onUpdate() {
            if (FindControl("txtNome", "input").value == "") {
                alert("Informe o nome da Transportadora.");
                return false;
            }

            if (FindControl("txtCpfCnpj", "input").value == "") {
                alert("Informe o CPF/CNPJ da Transportadora.");
                return false;
            }
            else if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;
        }

        function setCidade(idCidade, nomeCidade) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }

        function drpTipoPessoaChanged() {
            if (getTipoPessoa() == 'Juridica')
                FindControl("txtCpfCnpj", "input").maxLength = 18;
            else
                FindControl("txtCpfCnpj", "input").maxLength = 14;
        }

        function getTipoPessoa() {
            return FindControl("drpTipoPessoa", "select").value;
        }

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

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvTransportador" runat="server" SkinID="defaultDetailsView" DataSourceID="odsTransportador"
                    Height="50px" Width="125px" DataKeyNames="IdTransportador">
                    <Fields>
                        <asp:TemplateField HeaderText="Nome Fantasia" SortExpression="NomeFantasia">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="50" Text='<%# Bind("NomeFantasia") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNomeFantasia" runat="server" MaxLength="50" Text='<%# Bind("NomeFantasia") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NomeFantasia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Razão Social" SortExpression="Nome">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Pessoa" SortExpression="TipoPessoa">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoPessoa" runat="server" SelectedValue='<%# Bind("TipoPessoa") %>'
                                    onchange="drpTipoPessoaChanged();">
                                    <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTipoPessoa" runat="server" SelectedValue='<%# Bind("TipoPessoa") %>'
                                    onchange="drpTipoPessoaChanged();">
                                    <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TipoPessoa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="Cnpj">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" onkeypress="getTipoPessoa()=='Juridica' ? maskCNPJ(event, this) : maskCPF(event, this);"
                                    Text='<%# Bind("CpfCnpj") %>' Width="150px"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" onkeypress="getTipoPessoa()=='Juridica' ? maskCNPJ(event, this) : maskCPF(event, this);"
                                    Text='<%# Bind("CpfCnpj") %>' Width="150px"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Insc. Est." SortExpression="InscEst">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtInscEst" runat="server" Text='<%# Bind("InscEst") %>' MaxLength="22"
                                    Width="120px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtInscEst" runat="server" Text='<%# Bind("InscEst") %>' MaxLength="22"
                                    Width="120px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("InscEst") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SUFRAMA" SortExpression="Suframa">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Suframa") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Placa" SortExpression="Placa">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPlaca" runat="server" Text='<%# Bind("Placa") %>' MaxLength="10"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPlaca" runat="server" Text='<%# Bind("Placa") %>' MaxLength="10"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Placa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Endereço" SortExpression="Endereco">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEndereco" runat="server" Text='<%# Bind("Endereco") %>' MaxLength="50"
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtEndereco" runat="server" Text='<%# Bind("Endereco") %>' MaxLength="50"
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Endereco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Numero">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Numero") %>' MaxLength="20"
                                    Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Numero") %>' MaxLength="20"
                                    Width="100px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Numero") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Complemento" SortExpression="Complemento">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Complemento") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtComplemento" runat="server" MaxLength="100" Text='<%# Bind("Complemento") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtComplemento" runat="server" MaxLength="100" Text='<%# Bind("Complemento") %>'
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bairro" SortExpression="Bairro">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' MaxLength="30"
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' MaxLength="30"
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Bairro") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade" SortExpression="Cidade">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCidade" runat="server" Text='<%# Eval("Cidade.NomeCidade") %>' Width="200px"
                                    ReadOnly="true" Style="margin-right: 0px"></asp:TextBox>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCidade" runat="server" Text='<%# Eval("Cidade.NomeCidade") %>' Width="200px"
                                    ReadOnly="true"></asp:TextBox>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CEP" SortExpression="Cep">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" Text='<%# Bind("Cep") %>' Width="100px"></asp:TextBox>
                                <asp:Label ID="Label112" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciarPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" Text='<%# Bind("Cep") %>' Width="100px"></asp:TextBox>
                                <asp:Label ID="Label112" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciarPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Telefone" SortExpression="Telefone">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox14" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Telefone") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox14" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("Telefone") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="E-mail" SortExpression="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contato" SortExpression="Contato">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtContato" runat="server" Text='<%# Bind("Contato") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtContato" runat="server" Text='<%# Bind("Contato") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tel. Contato" SortExpression="TelefoneContato">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTelefoneContato" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelefoneContato") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtTelefoneContato" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelefoneContato") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' Width="300px" Height="60px" TextMode="MultiLine"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' Width="300px" Height="60px" TextMode="MultiLine"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdTransp"
                                    Text='<%# Bind("IdTransportador") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsert();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Transportador"
                    InsertMethod="SalvarTransportador"
                    SelectMethod="ObtemTransportador"
                    TypeName="Glass.Global.Negocios.ITransportadorFluxo"
                    UpdateMethod="SalvarTransportador"
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTransportador" QueryStringField="idTransp" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        drpTipoPessoaChanged();
    </script>

</asp:Content>
