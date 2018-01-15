<%@ Page Title="Cadastro de Comissionado" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadComissionado.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadComissionado" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
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
    
    function onInsert() {
        if (FindControl("txtNome", "input").value == "") {
            alert("Informe o nome do Comissionado.");
            return false;
        }

        if (FindControl("hdfCidade", "input").value == "") {
            alert("Informe a cidade do Comissionado.");
            return false;
        }

        if (FindControl("txtPercentual", "input").value == "" || FindControl("txtPercentual", "input").value == "0") {
            alert("Informe o percentual de comissão deste comissionado.");
            return false;
        }
    
        var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

        if (cpfCnpj == "") {
            alert("Informe o CPF/CNPJ do Comissionado.");
            return false;
        }
        
        if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
            return false;
        else if (CadComissionado.CheckIfExists(cpfCnpj).value == "true") {
            alert("Já existe um Comissionado cadastrado com o CPF/CNPJ informado.");
            return false;
        }
        
        FindControl("txtPercentual", "input").value = FindControl("txtPercentual", "input").value.replace("%", "");
    }

    function onUpdate() {
        if (FindControl("txtNome", "input").value == "") {
            alert("Informe o nome do Comissionado.");
            return false;
        }

        if (FindControl("hdfCidade", "input").value == "") {
            alert("Informe a cidade do Comissionado.");
            return false;
        }

        if (FindControl("txtPercentual", "input").value == "" || FindControl("txtPercentual", "input").value == "0") {
            alert("Informe o percentual de comissão deste comissionado.");
            return false;
        }

        if (FindControl("txtCpfCnpj", "input").value == "") {
            alert("Informe o CPF/CNPJ do Comissionado.");
            return false;
        }
        else if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
            return false;
        
        FindControl("txtPercentual", "input").value = FindControl("txtPercentual", "input").value.replace("%", "");
    }

    function drpTipoPessoaChanged() {
        if (getTipoPessoa() == "Juridica")
            FindControl("txtCpfCnpj", "input").maxLength = 18;
        else
            FindControl("txtCpfCnpj", "input").maxLength = 14;
    }

    function getTipoPessoa() {
        return FindControl("ddlTipoPessoa", "select").value;
    }

    function setCidade(idCidade, nomeCidade) {
        FindControl('hdfCidade', 'input').value = idCidade;
        FindControl('txtCidade', 'input').value = nomeCidade;
    }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvComissionado" runat="server" DataSourceID="odsComissionado" SkinID="defaultDetailsView"
                    Height="50px" Width="125px" DataKeyNames="IdComissionado">
                    <Fields>
                        <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NomeFantasia") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("TelCont") %>' MaxLength="15"></asp:TextBox>
                                <asp:CustomValidator ID="cvlCnpj" runat="server" ClientValidationFunction="validarCnpj"
                                    ControlToValidate="txtCnpj" ErrorMessage="CNPJ inválido."></asp:CustomValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox11" runat="server" MaxLength="15" Text='<%# Bind("TelCont") %>'></asp:TextBox>
                                <asp:CustomValidator ID="cvlCnpj" runat="server" ClientValidationFunction="validarCnpj"
                                    ControlToValidate="txtCnpj" ErrorMessage="CNPJ inválido."></asp:CustomValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                    onkeypress="getTipoPessoa()=='Juridica' ? maskCNPJ(event, this) : maskCPF(event, this);"
                                    Width="150px"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                    Width="150px" onkeypress="getTipoPessoa()=='Juridica' ? maskCNPJ(event, this) : maskCPF(event, this);"></asp:TextBox>
                                <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                    ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Pessoa" SortExpression="TipoPessoa">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoPessoa" runat="server" onchange="drpTipoPessoaChanged()"
                                    SelectedValue='<%# Bind("TipoPessoa") %>'>
                                    <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoPessoa" runat="server" onchange="drpTipoPessoaChanged()"
                                    SelectedValue='<%# Bind("TipoPessoa") %>'>
                                    <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                    <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TipoPessoa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
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
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Numero") %>' MaxLength="10"
                                    Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Numero") %>' MaxLength="10"
                                    Width="100px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Compl") %>'></asp:Label>
                            </ItemTemplate>
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
                                <asp:TextBox ID="txtCidade" runat="server" Text='<%# Bind("Cidade") %>' Width="200px" Style="margin-right: 0px;"></asp:TextBox>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCidade" runat="server" Text='<%# Bind("Cidade") %>' Width="200px" Style="margin-right: 0px"></asp:TextBox>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CEP" SortExpression="Cep">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" MaxLength="9" Width="100px"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                    onkeydown="return maskCep(event, this);" Text='<%# Bind("Cep") %>' Width="100px"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Telefone" SortExpression="TelRes">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox14" runat="server" MaxLength="14" Text='<%# Bind("TelRes") %>'
                                    onkeypress="return soTelefone(event);" onkeydown="return maskTelefone(event, this);"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox14" runat="server" MaxLength="14" Text='<%# Bind("TelRes") %>'
                                    onkeypress="return soTelefone(event)" onkeydown="return maskTelefone(event, this);"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Celular" SortExpression="TelCel">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("TelCel") %>' MaxLength="14"
                                    onkeypress="return soTelefone(event)" onkeydown="return maskTelefone(event, this);"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox13" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                    onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCel") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("TelCelVend") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="40" Text='<%# Bind("Email") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="40" Text='<%# Bind("Email") %>'
                                    Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Percentual" SortExpression="Percentual">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                    Text='<%# Bind("Percentual") %>' Width="50px"></asp:TextBox>
                                %
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                    Text='<%# Bind("Percentual") %>' Width="50px"></asp:TextBox>
                                %
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Percentual") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="25" Text='<%# Bind("Banco") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="25" Text='<%# Bind("Banco") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" MaxLength="25" Text='<%# Bind("Agencia") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" MaxLength="25" Text='<%# Bind("Agencia") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="C/C" SortExpression="Conta">
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" MaxLength="40" Text='<%# Bind("Conta") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" MaxLength="40" Text='<%# Bind("Conta") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' MaxLength="200"
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="200" Text='<%# Bind("Obs") %>'
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdComissionado"
                                    Text='<%# Bind("IdComissionado") %>' />
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" 
                    SelectMethod="ObtemComissionado"
                    TypeName="Glass.Global.Negocios.IComissionadoFluxo"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Comissionado"
                    InsertMethod="SalvarComissionado" UpdateMethod="SalvarComissionado" UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idComissionado" QueryStringField="idComissionado"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        drpTipoPessoaChanged();
        FindControl('txtCidade', 'input').setAttribute('readonly', 'readonly');
    </script>

</asp:Content>
