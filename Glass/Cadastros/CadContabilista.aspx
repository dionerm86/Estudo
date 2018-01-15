<%@ Page Title="Cadastro de Contabilista" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadContabilista.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadContabilista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">

    function iniciaPesquisaCep(cep) {
        var logradouro = FindControl("txtEndereco", "input");
        var bairro = FindControl("txtBairro", "input");
        var cidade = FindControl("txtCidade", "input");
        var idCidade = FindControl("hdfCidade", "input");
        pesquisarCep(cep, null, logradouro, bairro, cidade, null, idCidade);
    }

    function onInsert() {
        if (verificaCampos() == false)
            return false;

        var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

        if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
            return false;

        return true;
    }

    function onUpdate() {
        if (verificaCampos() == false)
            return false;

        var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

        if (cpfCnpj == "") {
            alert("Informe o CPF/CNPJ.");
            return false;
        }
        else if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
            return false;

        return true;
    }

    function verificaCampos() {
        if (FindControl("txtNome", "input").value == "") {
            alert("Informe o nome.");
            return false;
        }

        if (FindControl("hdfCidade", "input").value == "") {
            alert("Informe a cidade.");
            return false;
        }

        if (FindControl("txtEndereco", "input").value == "") {
            alert("Informe o endereço.");
            return false;
        }

        if (FindControl("txtNum", "input").value == "") {
            alert("Informe o número do endereço.");
            return false;
        }

        if (FindControl("txtBairro", "input").value == "") {
            alert("Informe o bairro.");
            return false;
        }

        var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

        if (cpfCnpj == "00000000000" || cpfCnpj == "00000000000000" || cpfCnpj == "") {
            alert("Informe o CPF/CNPJ.");
            return false;
        }

        return true;
    }

    function drpTipoPessoaChanged() {
        var contatos = document.getElementById("contatos");
        contatos.style.display = getTipoPessoa() == "J" ? "" : "none";

        if (getTipoPessoa() == "J")
            FindControl("txtCpfCnpj", "input").maxLength = 18;
        else
            FindControl("txtCpfCnpj", "input").maxLength = 14;
    }

    function getTipoPessoa() {
        return FindControl("drpTipoPessoa", "select").value;
    }

    function setCidade(idCidade, nomeCidade) {
        FindControl('hdfCidade', 'input').value = idCidade;
        FindControl('txtCidade', 'input').value = nomeCidade;
    }

</script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCliente" runat="server" AutoGenerateRows="False" DataSourceID="odsContabilista"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdContabilista">
                    <Fields>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label45" runat="server" Text="Nome/Razão Social"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtNome0" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                                Width="300px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label46" runat="server" Text="Tipo Pessoa"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpTipoPessoa" runat="server" 
                                                onchange="drpTipoPessoaChanged()" SelectedValue='<%# Bind("TipoPessoa") %>'>
                                                <asp:ListItem Value="F">Pessoa Física</asp:ListItem>
                                                <asp:ListItem Value="J">Pessoa Jurídica</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label47" runat="server" Text="CPF/CNPJ"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" 
                                                onkeypress="getTipoPessoa()=='J' ? maskCNPJ(event, this) : maskCPF(event, this);" 
                                                Text='<%# Bind("CpfCnpj") %>' Width="150px"></asp:TextBox>
                                            <asp:CustomValidator ID="valCpfCnpj" runat="server" 
                                                ClientValidationFunction="validarCpfCnpj" ControlToValidate="txtCpfCnpj" 
                                                ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label70" runat="server" Text="CRC"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCrc" runat="server" MaxLength="15" 
                                                Text='<%# Bind("Crc") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label56" runat="server" Text="Situação"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpSituacao" runat="server" 
                                                SelectedValue='<%# Bind("Situacao") %>'>
                                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label51" runat="server" Text="CEP"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" 
                                                onkeydown="return maskCep(event, this);" onkeypress="return soCep(event)" 
                                                Text='<%# Bind("Cep") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" 
                                                ImageUrl="~/Images/Pesquisar.gif" 
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label49" runat="server" Text="Endereço"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" 
                                                            Text='<%# Bind("Endereco") %>' Width="230px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label ID="Label50" runat="server" Text="N.º"></asp:Label>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtNum" runat="server" Text='<%# Bind("Numero") %>' 
                                                            Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label48" runat="server" Text="Complemento"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCompl" runat="server" MaxLength="50" 
                                                Text='<%# Bind("Compl") %>' Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label52" runat="server" Text="Bairro"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" 
                                                Text='<%# Bind("Bairro") %>' Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label54" runat="server" Text="Cidade"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" ReadOnly="True" 
                                                Text='<%# Bind("NomeCidade") %>' Width="200px"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                                OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label58" runat="server" Text="Fone"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" valign="top">
                                            <asp:TextBox ID="txtFone" runat="server" MaxLength="14" 
                                                onkeydown="return maskTelefone(event, this);" 
                                                onkeypress="return soTelefone(event)" Text='<%# Bind("TelCont") %>' 
                                                Width="100px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label59" runat="server" Text="Email"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="60" 
                                                Text='<%# Bind("Email") %>' Width="160px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label38" runat="server" Text="Fax"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtFax" runat="server" MaxLength="14" 
                                                onkeydown="return maskTelefone(event, this);" 
                                                onkeypress="return soTelefone(event)" Text='<%# Bind("Fax") %>' Width="100px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            &nbsp;</td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:HiddenField ID="hdfCidade" runat="server" 
                                                Value='<%# Bind("IdCidade") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsert()) return false;" />
                                <asp:Button ID="btnCancelar0" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContabilista" runat="server" 
                    SelectMethod="GetElement" TypeName="Glass.Data.DAL.ContabilistaDAO" 
                    DataObjectTypeName="Glass.Data.Model.Contabilista" InsertMethod="Insert" 
                    UpdateMethod="Update" oninserted="odsContabilista_Inserted" 
                    onupdated="odsContabilista_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idContabilista" 
                            QueryStringField="idContabilista" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

