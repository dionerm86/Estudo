<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadFuncionario" Title="Cadastro de Funcionário" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function iniciaPesquisaCep(cep) {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null);
        }

        function onInsert() {
            if (!verificaCampos())
                return false;
        }

        function onUpdate() {
            if (!verificaCampos())
                return false;
        }

        function verificaCampos() {
            if (FindControl("txtNome", "input").value == "") {
                alert("Informe o nome do funcionário.");
                return false;
            }

            if (FindControl("ddlTipoFunc", "select").value == "") {
                alert("Informe o tipo do funcionário.");
                return false;
            }

            if (FindControl("txtEndereco", "input").value == "") {
                alert("Informe o endereço do funcionário.");
                return false;
            }

            if (FindControl("txtBairro", "input").value == "") {
                alert("Informe o bairro do funcionário.");
                return false;
            }

            if (FindControl("txtCidade", "input").value == "") {
                alert("Informe a cidade do funcionário.");
                return false;
            }

            if (FindControl("txtTelRes", "input").value == "") {
                alert("Informe o telefone residencial do funcionário.");
                return false;
            }

            if (FindControl("ctrlDataNasc_txtData", "input").value == "") {
                alert("Informe a data de nascimento do funcionário.");
                return false;
            }

            if (FindControl("ctrlDataEntr_txtData", "input").value == "") {
                alert("Informe a data de entrada do funcionário.");
                return false;
            }

            if (FindControl("drpLoja", "select").value == "") {
                alert("Informe a Loja do funcionário.");
                return false;
            }

            var cpf = FindControl("txtCpf", "input").value;

            if (cpf == "" || cpf == "00000000000") {
                alert("Informe o CPF do funcionário.");
                return false;
            }

            if (FindControl("cvlCpf", "span").style.visibility == "visible")
                return false;

            return true;
        }

        function hideShowSetor() {
            // Se o funcionário for MARCADOR PRODUÇÃO, mostra campo de setor para ele selecionar
            var display = FindControl("ddlTipoFunc", "select").value == 196 ? "inline" : "none";

            FindControl("cblSetor", "table").style.display = display;
        }
        

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvFuncionario" runat="server" AutoGenerateRows="False" DataSourceID="odsFuncionario"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdFunc" OnDataBound="dtvFuncionario_DataBound">
                    <Fields>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Nome"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                                Width="300px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label11" runat="server" Text="RG"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtRg" runat="server" MaxLength="10" Text='<%# Bind("Rg") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label32" runat="server" Text="Tipo Funcionário"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoFunc" runat="server" DataSourceID="odsTipoFunc" DataTextField="Name"
                                                DataValueField="Id" onchange="hideShowSetor();" SelectedValue='<%# Bind("IdTipoFunc") %>'
                                                AppendDataBoundItems="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                            <asp:CheckBoxList ID="cblSetorIns" runat="server" DataSource='<%# Setores %>'
                                                DataTextField="Name" DataValueField="Id" RepeatColumns="3" AutoPostBack="true"
                                                OnSelectedIndexChanged="cblSetorIns_SelectedIndexChanged">
                                            </asp:CheckBoxList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label2" runat="server" Text="CPF"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCpf" runat="server" MaxLength="14" onkeypress="maskCPF(event, this);"
                                                Text='<%# Bind("Cpf") %>' Width="150px"></asp:TextBox>
                                            <asp:CustomValidator ID="cvlCpf" runat="server" ClientValidationFunction="validarCpf"
                                                ControlToValidate="txtCpf" ErrorMessage="CPF inválido."></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label13" runat="server" Text="Data Nasc."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc3:ctrlData ID="ctrlDataNasc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataNasc") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label40" runat="server" Text="Função"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtFuncao" runat="server" MaxLength="50" Text='<%# Bind("Funcao") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label5" runat="server" Text="Endereço"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" Text='<%# Bind("Endereco") %>'
                                                Width="300px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label12" runat="server" Text="Est. Civil"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpEstadoCivil" runat="server" SelectedValue='<%# Bind("EstCivil") %>'>
                                                <asp:ListItem Value="Solteiro">Solteiro</asp:ListItem>
                                                <asp:ListItem Value="Casado">Casado</asp:ListItem>
                                                <asp:ListItem Value="Divorciado">Divorciado</asp:ListItem>
                                                <asp:ListItem Value="Desquitado">Desquitado</asp:ListItem>
                                                <asp:ListItem Value="Viúvo">Viúvo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label15" runat="server" Text="Bairro"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label6" runat="server" Text="Complemento"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCompl" runat="server" MaxLength="50" Text='<%# Bind("Compl") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label16" runat="server" Text="Cidade"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" valign="top">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="200px"></asp:TextBox>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label17" runat="server" Text="CEP"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);" Text='<%# Bind("Cep") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label7" runat="server" Text="UF"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="DropDownList2" runat="server" SelectedValue='<%# Bind("Uf") %>'
                                                Width="55px">
                                                <asp:ListItem>SP</asp:ListItem>
                                                <asp:ListItem>MG</asp:ListItem>
                                                <asp:ListItem>AC</asp:ListItem>
                                                <asp:ListItem>RJ</asp:ListItem>
                                                <asp:ListItem>AL</asp:ListItem>
                                                <asp:ListItem>AM</asp:ListItem>
                                                <asp:ListItem>AP</asp:ListItem>
                                                <asp:ListItem>BA</asp:ListItem>
                                                <asp:ListItem>CE</asp:ListItem>
                                                <asp:ListItem>DF</asp:ListItem>
                                                <asp:ListItem>ES</asp:ListItem>
                                                <asp:ListItem>GO</asp:ListItem>
                                                <asp:ListItem>MA</asp:ListItem>
                                                <asp:ListItem>MS</asp:ListItem>
                                                <asp:ListItem>MT</asp:ListItem>
                                                <asp:ListItem>PA</asp:ListItem>
                                                <asp:ListItem>PB</asp:ListItem>
                                                <asp:ListItem>PE</asp:ListItem>
                                                <asp:ListItem>PI</asp:ListItem>
                                                <asp:ListItem>PR</asp:ListItem>
                                                <asp:ListItem>RN</asp:ListItem>
                                                <asp:ListItem>RO</asp:ListItem>
                                                <asp:ListItem>RR</asp:ListItem>
                                                <asp:ListItem>RS</asp:ListItem>
                                                <asp:ListItem>SC</asp:ListItem>
                                                <asp:ListItem>SE</asp:ListItem>
                                                <asp:ListItem>TO</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label3" runat="server" Text="Loja"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja"  AppendDataBoundItems="true"
                                                DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text="Tel. Cel."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCel") %>' Width="100px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label8" runat="server" Text="Tel. Res."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                onkeypress="return soTelefone(event)" Text='<%# Bind("TelRes") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label29" runat="server" Text="Data Entrada"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc3:ctrlData ID="ctrlDataEntr" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataEnt") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label18" runat="server" Text="Tel. Cont."></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTelCont" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                onkeypress="return soTelefone(event)" Text='<%# Bind("TelCont") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label35" runat="server" Text="Email"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" Text='<%# Bind("Email") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label30" runat="server" Text="Data Saída"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc3:ctrlData ID="ctrlDataSaida" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataSaida") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label38" runat="server" Text="Ramal"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtRamal" runat="server" MaxLength="30" onkeypress="return soTelefone(event)"
                                                onkeyup="return maskTelefone(event, this);" Text='<%# Bind("Ramal") %>' Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label23" runat="server" Text="Salário"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("Salario") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label19" runat="server" Text="Situação"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label33" runat="server" Text="Gratificação"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat2" runat="server" Value='<%# Bind("Gratificacao")%>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Núm. Carteira Trabalho
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumCarteiraTrab" runat="server" MaxLength="20" Text='<%# Bind("NumCarteiraTrabalho") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label34" runat="server" Text="Aux. Alimentação"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat3" runat="server" Value='<%# Bind("AuxAlimentacao")%>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label24" runat="server" Text="Login"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtLogin" runat="server" MaxLength="20" Text='<%# Bind("Login") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Núm. PIS
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtNumPis" runat="server" MaxLength="20" Text='<%# Bind("NumPis") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label31" runat="server" Text="Senha"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" Text='<%# Bind("Senha") %>'
                                                TextMode="Password"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Registrado
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkRegistrado" runat="server" Checked='<%# Bind("Registrado") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblTipoPedido" runat="server" Text="Tipo de Pedido"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" 
                                                SelectedValue='<%# Bind("TipoPedido") %>' Title="" DataSourceID="odsTipoPedido"
                                                DataTextField="Name" DataValueField="Id">
                                            </sync:CheckBoxListDropDown>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label4" runat="server" Text="Núm. Dias Atrasar Pedido"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumDiasAtrasarPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("NumDiasAtrasarPedido") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label10" runat="server" Text="Foto Funcionário"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:FileUpload ID="filImagem" runat="server" />
                                        </td>
                                         <td align="left" class="dtvHeader">
                                           <asp:Label ID="lblNumPdv" runat="server" Text="Número do PDV"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumPdv" runat="server" MaxLength="20" Text='<%# Bind("NumeroPdv") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label14" runat="server" visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor %>" Text="Enviar email pedido finalizado"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="CheckBox1" runat="server" visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor %>" Checked='<%# Bind("EnviarEmail") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label20" runat="server" Text="Utilizar chat WebGlass"
                                                Visible='<%# Glass.Data.Helper.UserInfo.GetUserInfo != null && Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>'></asp:Label>
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:CheckBox ID="chkAbrirChamado" runat="server" Checked='<%# Bind("AbrirChamado") %>' OnLoad="CheckedAbrirChamado"
                                                Visible='<%# Glass.Data.Helper.UserInfo.GetUserInfo != null && Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>'></asp:CheckBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label37" runat="server" Text="Obs"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" colspan="3">
                                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="2" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="660px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Nome"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                                Width="300px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label11" runat="server" Text="RG"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtRg" runat="server" MaxLength="10" Text='<%# Bind("Rg") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label32" runat="server" Text="Tipo Funcionário"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoFunc" runat="server" DataSourceID="odsTipoFunc" DataTextField="Name"
                                                DataValueField="Id" onchange="hideShowSetor();" SelectedValue='<%# Bind("IdTipoFunc") %>'
                                                AppendDataBoundItems="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                            <asp:CheckBoxList ID="cblSetor" runat="server" DataSource='<%# Setores %>' DataTextField="Name"
                                                DataValueField="Id" RepeatColumns="3" AutoPostBack="true" OnSelectedIndexChanged="cblSetorIns_SelectedIndexChanged">
                                            </asp:CheckBoxList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label2" runat="server" Text="CPF"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCpf" runat="server" MaxLength="14" onkeypress="maskCPF(event, this);"
                                                Text='<%# Bind("Cpf") %>' Width="150px"></asp:TextBox>
                                            <asp:CustomValidator ID="cvlCpf" runat="server" ClientValidationFunction="validarCpf"
                                                ControlToValidate="txtCpf" ErrorMessage="CPF inválido."></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label40" runat="server" Text="Função"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtFuncao" runat="server" MaxLength="50" Text='<%# Bind("Funcao") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label12" runat="server" Text="Est. Civil"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpEstadoCivil" runat="server" SelectedValue='<%# Bind("EstCivil") %>'>
                                                <asp:ListItem Value="Solteiro">Solteiro</asp:ListItem>
                                                <asp:ListItem Value="Casado">Casado</asp:ListItem>
                                                <asp:ListItem Value="Divorciado">Divorciado</asp:ListItem>
                                                <asp:ListItem Value="Desquitado">Desquitado</asp:ListItem>
                                                <asp:ListItem Value="Viúvo">Viúvo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label13" runat="server" Text="Data Nasc."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc3:ctrlData ID="ctrlDataNasc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataNasc") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label39" runat="server" Text="Registrado"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:CheckBox ID="chkRegistrado" runat="server" Checked='<%# Bind("Registrado") %>' />                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label5" runat="server" Text="Endereço"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" Text='<%# Bind("Endereco") %>'
                                                Width="300px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label6" runat="server" Text="Complemento"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCompl" runat="server" MaxLength="50" Text='<%# Bind("Compl") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label15" runat="server" Text="Bairro"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label17" runat="server" Text="CEP"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCep" onkeypress="return soCep(event)" onkeyup="return maskCep(event, this);"
                                                runat="server" MaxLength="9" Text='<%# Bind("Cep") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label16" runat="server" Text="Cidade"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" valign="top">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="200px"></asp:TextBox>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label7" runat="server" Text="UF"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="DropDownList2" runat="server" SelectedValue='<%# Bind("Uf") %>'
                                                Width="55px">
                                                <asp:ListItem>MG</asp:ListItem>
                                                <asp:ListItem>SP</asp:ListItem>
                                                <asp:ListItem>RJ</asp:ListItem>
                                                <asp:ListItem>AC</asp:ListItem>
                                                <asp:ListItem>AL</asp:ListItem>
                                                <asp:ListItem>AM</asp:ListItem>
                                                <asp:ListItem>AP</asp:ListItem>
                                                <asp:ListItem>BA</asp:ListItem>
                                                <asp:ListItem>CE</asp:ListItem>
                                                <asp:ListItem>DF</asp:ListItem>
                                                <asp:ListItem>ES</asp:ListItem>
                                                <asp:ListItem>GO</asp:ListItem>
                                                <asp:ListItem>MA</asp:ListItem>
                                                <asp:ListItem>MS</asp:ListItem>
                                                <asp:ListItem>MT</asp:ListItem>
                                                <asp:ListItem>PA</asp:ListItem>
                                                <asp:ListItem>PB</asp:ListItem>
                                                <asp:ListItem>PE</asp:ListItem>
                                                <asp:ListItem>PI</asp:ListItem>
                                                <asp:ListItem>PR</asp:ListItem>
                                                <asp:ListItem>RN</asp:ListItem>
                                                <asp:ListItem>RO</asp:ListItem>
                                                <asp:ListItem>RR</asp:ListItem>
                                                <asp:ListItem>RS</asp:ListItem>
                                                <asp:ListItem>SC</asp:ListItem>
                                                <asp:ListItem>SE</asp:ListItem>
                                                <asp:ListItem>TO</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label3" runat="server" Text="Loja"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="Name" AppendDataBoundItems="true"
                                                DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label8" runat="server" Text="Tel. Res."></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="14" Text='<%# Bind("TelRes") %>'
                                                Width="100px" onkeypress="return soTelefone(event)" onkeydown="return maskTelefone(event, this);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text="Tel. Cel."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCel") %>' Width="100px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label18" runat="server" Text="Tel. Cont."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtTelCont" runat="server" MaxLength="14" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCont") %>' Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label29" runat="server" Text="Data Entrada"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc3:ctrlData ID="ctrlDataEntr" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataEnt") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label30" runat="server" Text="Data Saída"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc3:ctrlData ID="ctrlDataSaida" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                                DataString='<%# Bind("DataSaida") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label19" runat="server" Text="Situação"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label23" runat="server" Text="Salário"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("Salario") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label33" runat="server" Text="Gratificação"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat2" runat="server" Value='<%# Bind("Gratificacao")%>' />
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label34" runat="server" Text="Aux. Alimentação"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat3" runat="server" Value='<%# Bind("AuxAlimentacao")%>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label35" runat="server" Text="Email"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" Text='<%# Bind("Email") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label38" runat="server" Text="Ramal"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtRamal" runat="server" MaxLength="30" onkeypress="return soTelefone(event)"
                                                onkeyup="return maskTelefone(event, this);" Text='<%# Bind("Ramal") %>' Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Núm. Carteira Trabalho
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumCarteiraTrab" runat="server" MaxLength="20" Text='<%# Bind("NumCarteiraTrabalho") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Núm. PIS
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumPis" runat="server" MaxLength="20" Text='<%# Bind("NumPis") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label24" runat="server" Text="Login"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtLogin" runat="server" MaxLength="20" Text='<%# Bind("Login") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblTipoPedido" runat="server" Text="Tipo de Pedido"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" DataSourceID="odsTipoPedido"
                                                DataTextField="Name" DataValueField="Id"
                                                SelectedValue='<%# Bind("TipoPedido") %>' Title="">
                                            </sync:CheckBoxListDropDown>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label4" runat="server" Text="Núm. Dias Atrasar Pedido"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumDiasAtrasarPedido" runat="server" Text='<%# Bind("NumDiasAtrasarPedido") %>'
                                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                           <asp:Label ID="lblNumPdv" runat="server" Text="Número do PDV"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumPdv" runat="server" MaxLength="20" Text='<%# Bind("NumeroPdv") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                        </td>
                                        <td>
                                            <asp:Image ID="Image1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.ObtemUrl((int)Eval("IdFunc")) %>' Height="120px"
                                                Width="140px" visible='<%# Glass.Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.PossuiImagem((int)Eval("IdFunc")) %>'/>
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label10" runat="server" Text="Foto Funcionário"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:FileUpload ID="filImagem" runat="server" />
                                            <uc4:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.ObtemUrl((int)Eval("IdFunc")) %>' />
                                        </td>
                                    </tr>
                                    <tr >
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label14" runat="server"
                                                visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor %>" Text="Enviar email pedido finalizado"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="CheckBox1" runat="server"
                                                visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor %>" Checked='<%# Bind("EnviarEmail") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label37" runat="server" Text="Obs"></asp:Label>
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="2" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="660px"></asp:TextBox>
                                            <asp:HiddenField ID="hdfSenha" runat="server" Value='<%# Bind("Senha") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onUpdate();" />
                                <asp:Button ID="btnAlterarSenha" runat="server" Text="Alterar Senha" OnClientClick='<%# Eval("IdFunc", "openWindow(150, 300, \"../Utils/TrocarSenha.aspx?IdFunc={0}\"); return false;") %>' />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdFunc"
                                    Text='<%# Eval("IdFunc") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsert();" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" 
        SelectMethod="ObtemSetores" 
        TypeName="Glass.PCP.Negocios.ISetorFluxo">
    </colo:VirtualObjectDataSource>

    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Funcionario"
        InsertMethod="SalvarFuncionario" 
        SelectMethod="ObtemFuncionario" 
        CreateDataObjectMethod="CriarFuncionario"
        TypeName="Glass.Global.Negocios.IFuncionarioFluxo"
        UpdateMethod="SalvarFuncionario" UpdateStrategy="GetAndUpdate"
        OnInserting="odsFuncionario_Inserting" OnInserted="odsFuncionario_Inserted"
        OnUpdating="odsFuncionario_Updating" OnUpdated="odsFuncionario_Updated" >
        <SelectParameters>
            <asp:QueryStringParameter Name="idFunc" QueryStringField="idFunc" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoFunc" runat="server" 
        SelectMethod="ObtemTiposFuncionario" 
        TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
        SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPedido" runat="server" 
        SelectMethod="ObtemTiposPedido"
        TypeName="Glass.Global.UI.Web.Process.Funcionarios.CadastroFuncionario">
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">

        hideShowSetor();
        
    </script>

</asp:Content>
