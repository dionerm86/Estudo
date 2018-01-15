<%@ Page Title="Cadastro de Medição" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadMedicao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMedicao" %>

<%@ Register src="../Controls/ctrlLinkQueryString.ascx" tagname="ctrllinkquerystring" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
    
        var limite = <%= Glass.Configuracoes.OrcamentoConfig.LimiteDiarioMedicoes %>;

        function setNumMedicoes()
        {
            if (limite == 0)
                return;
        
            var data = FindControl("txtDataMedicao", "input").value;
            var idMedicao = FindControl("hdfIdMedicao", "input");
            idMedicao = idMedicao != null ? idMedicao.value : null;
            var retorno = CadMedicao.NumMedicoesDia(data, idMedicao).value;
        
            if (retorno != null && retorno != "")
            {
                var label = FindControl("lblNumMedicoes", "span");
                label.innerHTML = "Medições para esse dia: " + retorno;
                label.style.color = retorno > limite ? "red" : "black";
            }
        }

        function iniciaPesquisaCep(cep)
        {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null);
        }

        function setDadosCliente(nome, telRes, telCel, email, endereco, bairro, cidade, cep, idCli, compl) {
            FindControl("txtCliente", "input").value = nome;
            FindControl("txtTelCliente", "input").value = telRes;
            FindControl("txtCelCliente", "input").value = telCel;
            FindControl("txtEmail", "input").value = email;
            FindControl("txtEndereco", "input").value = endereco;
            FindControl("txtBairro", "input").value = bairro;
            FindControl("txtCidade", "input").value = cidade;
            FindControl("txtCompl", "input").value = compl;
            FindControl("txtCep", "input").value = cep;
        }
    
        function validaMedidor(val, args)
        {
            var situacao = FindControl("drpSituacao", "select");
            args.IsValid = situacao == null || situacao.value == "1" || args.Value != "";
        }
    
        function alteraSituacao(situacao)
        {
            var nomeMedidor = document.getElementById("nomeMedidor");

            if (nomeMedidor != null) {
                nomeMedidor.style.display = situacao == 1 ? "none" : "";
        
                if (situacao == 1)
                    FindControl("drpMedidor", "select").value = "";
            }
        }
    
        /* Chamado 19379.
         * Impedir registro duplicado. */
        var inserindoOuAtualizando = false;
        function insertOrUpdate()
        {
            if (inserindoOuAtualizando)
                return false;

            inserindoOuAtualizando = true;
            
            alterarDisableCliente(false); 

            if (!validate()) {
                inserindoOuAtualizando = false;
                nomeCliente.disabled = true;
                return false;
            }
                 
            var data = FindControl("txtDataMedicao", "input");
            if (data.value == "")
            {
                alert("Selecione a data da medição.");
                inserindoOuAtualizando = false;

                alterarDisableCliente(<%= Glass.Configuracoes.MedicaoConfig.MedicaoApenasClienteCadastrado.ToString().ToLower() %>);
                return false;
            }
                 
            var email = FindControl("txtEmail", "input");

            if (<%= Glass.Configuracoes.MedicaoConfig.BloquearCadastroMedicaoSemEmailCliente.ToString().ToLower() %> &&
                email != null && email != undefined && email.value != null && email.value != undefined && email.value == "")
            {
                alert("Informe o e-mail do cliente.");
                inserindoOuAtualizando = false;
                return false;
            }

            if (data.value == "")
            {
                alert("Selecione a data da medição.");
                inserindoOuAtualizando = false;

                alterarDisableCliente(<%= Glass.Configuracoes.MedicaoConfig.MedicaoApenasClienteCadastrado.ToString().ToLower() %>);
                return false;
            }
        
            if (limite == 0)
                return true;
        
            var idMedicao = FindControl("hdfIdMedicao", "input");
            idMedicao = idMedicao != null ? idMedicao.value : "";
            var retorno = CadMedicao.VerificarLimite(idMedicao, data.value).value.split(';');
        
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                inserindoOuAtualizando = false;
                nomeCliente.disabled = true;
                return false;
            }
        
            if (retorno[1] == "false")
            {
                if (confirm("A data selecionada não pode ser usada porque excede o limite de medições para o dia.\n" +
                    "A próxima data disponível para a medição é " + retorno[2] + ".\nDeseja marcar essa medição para esse dia?"))
                {
                    data.value = retorno[2];
                    data.onchange();
                    return true;
                }
                else {
                    inserindoOuAtualizando = false;
                    nomeCliente.disabled = true;
                    return false;
                }
            }
        
            return true;
        }
        
        function verificarSeOrcamentoPodeSerAssociado(controle)
        {
            FindControl("hdfIdOrcamento", "input").value = FindControl("txtIdOrcamento", "input").value == null ? "" : FindControl("txtIdOrcamento", "input").value;

            if (controle == null || controle == undefined || controle.value == null || controle.value == undefined || controle.value == "")
            {
                FindControl("chkDefinitiva", "input").disabled=false;
                return false;
            }
            var retorno = CadMedicao.VerificarPodeAssociarOrcamento(controle.value);
            
            FindControl("chkDefinitiva", "input").disabled = retorno.value.split(';')[1] == "possuiMedicaoDefinitiva";

            if(FindControl("chkDefinitiva", "input").checked && retorno.value.split(';')[1] == "possuiMedicaoDefinitiva")
                FindControl("chkDefinitiva", "input").checked = retorno.value.split(';')[1] != "possuiMedicaoDefinitiva";

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }
            else if (retorno.value.split(';')[0] == "Erro"){
                alert(retorno.value.split(';')[1]);
                controle.value = "";
                return false;
            }

            var dadosOrcamento = CadMedicao.GetDadosOrcamento(FindControl("txtIdOrcamento", "input").value).value.split(";");

            if (dadosOrcamento[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            if (dadosOrcamento[1] != "") {
                getCli(dadosOrcamento[1])
            }

            return false;
        }
        
        function verificarSePedidoPodeSerAssociado(controle)
        {
            if (controle == null || controle == undefined || controle.value == null || controle.value == undefined || controle.value == "")
                return false;

            var retorno = CadMedicao.VerificarPodeAssociarPedido(controle.value);
            
            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }
            else if (retorno.value.split(';')[0] == "Erro"){
                alert(retorno.value.split(';')[1]);
                controle.value = "";
                return false;
            }

            return false;
        }

        function alterarValor()
        {
            FindControl("hdfMedicaoDef", "input").value = FindControl("chkDefinitiva", "input").checked
        }

        function getCli(idCli)
        {
            var usarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;
        
            var dados = CadMedicao.GetCli(idCli).value;
            if (dados == null || dados == "" || dados.split('|')[0] == "Erro")
            {
                idCli == "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfIdCliente", "input").value = "";
                FindControl("txtIdCliente", "input").value = "";
                        
                if (usarComissionado)
                    limparComissionado();
                
                if (dados.split('|')[0] == "Erro")
                    alert(dados.split('|')[1]);
            
                return;
            }
        
            dados = dados.split("|");
            setDadosCliente(dados[0], dados[1], dados[2], dados[3], dados[4], dados[5], dados[6], dados[7], idCli, dados[8]);
        
            var drpFuncionario = FindControl("drpFuncionario", "select");
            if (drpFuncionario != null && dados[9] != "0")
                drpFuncionario.value = dados[9];
        
            if (usarComissionado)
            {
                var comissionado = MetodosAjax.GetComissionado("", idCli).value.split(';');
                setComissionado(comissionado[0], comissionado[1], comissionado[2]);
            }
        }

        function setDadosCliente(nome, telRes, telCel, email, endereco, bairro, cidade, cep, idCliente, compl) {
            FindControl("txtNomeCliente", "input").value = nome;
            FindControl("txtTelCliente", "input").value = telRes;
            FindControl("txtCelCliente", "input").value = telCel;
            FindControl("txtEmail", "input").value = email;
            FindControl("txtEndereco", "input").value = endereco + (compl != "" && compl != null ? " (" + compl + ")" : "");
            FindControl("txtBairro", "input").value = bairro;
            FindControl("txtCidade", "input").value = cidade;
            FindControl("txtCep", "input").value = cep;

            FindControl("txtIdCliente", "input").value = idCliente;
            FindControl("hdfIdCliente", "input").value = idCliente;
        }

    </script>
    <table style="width: 100%">
        <tr>
            <td align="center">
            <asp:DetailsView ID="dtvMedicao" runat="server" AutoGenerateRows="False" 
                DataKeyNames="IdMedicao" DataSourceID="odsMedicao" DefaultMode="Insert" 
                GridLines="None" Height="50px" Width="125px" CellPadding="4">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                <RowStyle CssClass="dtvAlternatingRow" />
                <FieldHeaderStyle Font-Bold="False" CssClass="dtvHeader" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <Fields>
                    <asp:TemplateField HeaderText="Orçamento" SortExpression="IdOrcamento">
                        <ItemTemplate>
                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("IdOrcamento") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" MaxLength="7" Text='<%# Eval("IdOrcamento") %>' Width="50px"
                                onkeypress="return soNumeros(event, true, true);" onblur="verificarSeOrcamentoPodeSerAssociado(this);"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" MaxLength="7" Text='<%# Eval("IdOrcamento") %>' Width="50px"
                                onkeypress="return soNumeros(event, true, true);" onblur="verificarSeOrcamentoPodeSerAssociado(this);"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                        <ItemTemplate>
                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("IdPedido") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtIdPedido" runat="server" MaxLength="7" Text='<%# Bind("IdPedido") %>' Width="50px"
                                onkeypress="return soNumeros(event, true, true);" onblur="verificarSePedidoPodeSerAssociado(this);"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtIdPedido" runat="server" MaxLength="7" Text='<%# Bind("IdPedido") %>' Width="50px"
                                onkeypress="return soNumeros(event, true, true);" onblur="verificarSePedidoPodeSerAssociado(this);"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                        <ItemTemplate>
                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("IdCliente") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtIdCliente" runat="server" Text='<%# Eval("IdCliente") %>' onkeypress="return soNumeros(event, true, true)"
                                onblur="getCli(this.value);" Width="50px"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Text='<%# Bind("NomeCliente") %>'
                                Width="280px" MaxLength="50"></asp:TextBox>
                            <asp:ImageButton ID="imgGetCliente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="openWindow(500, 700, '../Utils/SelCliente.aspx?dadosCliente=1&tipo=orcamento'); return false;" />
                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtIdCliente" runat="server" Text='<%# Eval("IdCliente") %>' onkeypress="return soNumeros(event, true, true)"
                                onblur="getCli(this.value);" Width="50px"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Text='<%# Bind("NomeCliente") %>'
                                Width="260px" MaxLength="50"></asp:TextBox>
                            
                            <asp:ImageButton ID="imgGetCliente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="openWindow(500, 700, '../Utils/SelCliente.aspx?dadosCliente=1&tipo=orcamento'); return false;" />
                                
                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tel. Cliente" SortExpression="TelCliente">
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("TelCliente") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtTelCliente" runat="server" MaxLength="15"
                                onkeydown="return maskTelefone(event, this);" onkeypress="return soTelefone(event)" 
                                Text='<%# Bind("TelCliente") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ControlToValidate="txtTelCliente" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtTelCliente" runat="server" MaxLength="15"
                                onkeydown="return maskTelefone(event, this);" onkeypress="return soTelefone(event)" 
                                Text='<%# Bind("TelCliente") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ControlToValidate="txtTelCliente" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cel. Cliente" SortExpression="CelCliente">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCelCliente" runat="server" MaxLength="15"
                                onkeydown="return maskTelefone(event, this);" onkeypress="return soTelefone(event)" 
                                Text='<%# Bind("CelCliente") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtCelCliente" runat="server" MaxLength="15"
                                onkeydown="return maskTelefone(event, this);" onkeypress="return soTelefone(event)" 
                                Text='<%# Bind("CelCliente") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label14" runat="server" Text='<%# Bind("CelCliente") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="E-mail Cliente" SortExpression="EmailCliente">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("EmailCliente") %>' 
                                Width="200px" MaxLength="50"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("EmailCliente") %>' 
                                Width="200px" MaxLength="50"></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label20" runat="server" Text='<%# Bind("EmailCliente") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cep" SortExpression="Cep">
                        <ItemTemplate>
                            <asp:Label ID="Label21" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" 
                                onkeydown="return maskCep(event, this);" onkeypress="return soCep(event)" 
                                Text='<%# Bind("Cep") %>'></asp:TextBox>
                            <asp:ImageButton ID="imgPesquisarCep" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" 
                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Endereço" SortExpression="Endereco">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="50" 
                                Text='<%# Bind("Endereco") %>' Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ControlToValidate="txtEndereco" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("Endereco") %>'></asp:Label>
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="50" 
                                Text='<%# Bind("Endereco") %>' Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ControlToValidate="txtEndereco" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Compl." SortExpression="Compl">
                        <ItemTemplate>
                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("Compl") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCompl" MaxLength="20" runat="server" Text='<%# Bind("Compl") %>' 
                                Width="200px"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtCompl" MaxLength="20" runat="server" Text='<%# Bind("Compl") %>' 
                                Width="200px"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bairro" SortExpression="Bairro">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtBairro" runat="server" Text='<%# Bind("Bairro") %>' 
                                MaxLength="30" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                ControlToValidate="txtBairro" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("Bairro") %>'></asp:Label>
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="30" 
                                Text='<%# Bind("Bairro") %>' Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                ControlToValidate="txtBairro" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cidade" SortExpression="Cidade">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Cidade") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCidade" runat="server" Text='<%# Bind("Cidade") %>' 
                                MaxLength="30" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ControlToValidate="txtCidade" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="30" 
                                Text='<%# Bind("Cidade") %>' Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ControlToValidate="txtCidade" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Medição" SortExpression="DataMedicao">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataMedicao") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDataMedicao" runat="server" onkeypress="return false;" onchange="setNumMedicoes()"
                                Text='<%# Bind("DataMedicao") %>' Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataMedicao" runat="server" ImageAlign="AbsMiddle" 
                                ImageUrl="~/Images/calendario.gif" 
                                OnClientClick="return SelecionaData('txtDataMedicao', this)" 
                                ToolTip="Alterar" />
                            <asp:Label ID="lblNumMedicoes" runat="server"></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDataMedicao" runat="server" onkeypress="return false;" onchange="setNumMedicoes()"
                                Text='<%# Bind("DataMedicao") %>' Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataMedicao" runat="server" ImageAlign="AbsMiddle" 
                                ImageUrl="~/Images/calendario.gif" 
                                OnClientClick="return SelecionaData('txtDataMedicao', this)" 
                                ToolTip="Alterar" />
                            <asp:Label ID="lblNumMedicoes" runat="server"></asp:Label>
                        </InsertItemTemplate>
                        <HeaderStyle Wrap="False" />
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Turno" SortExpression="Turno">
                        <InsertItemTemplate>
                            <table>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="drpTurno" runat="server" DataSourceID="odsTurno" 
                                            DataTextField="Descr" DataValueField="Id" 
                                            SelectedValue='<%# Bind("Turno") %>' AppendDataBoundItems="True">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:RequiredFieldValidator ID="rqdTurno" runat="server" 
                                            ControlToValidate="drpTurno" ErrorMessage="*"></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        Hora</td>
                                    <td>
                                        <asp:TextBox ID="txtHora" runat="server" MaxLength="5" 
                                            Text='<%# Bind("Hora") %>' Width="60px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("Turno") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <table>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="drpTurno" runat="server" DataSourceID="odsTurno" 
                                            DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Turno") %>'>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        Hora</td>
                                    <td>
                                        <asp:TextBox ID="txtHora" runat="server" MaxLength="5" 
                                            Text='<%# Bind("Hora") %>' Width="60px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                        <ItemTemplate>
                            <asp:Label ID="Label16" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                SelectedValue='<%# Bind("IdLoja") %>' AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                SelectedValue='<%# Bind("IdLoja") %>' AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvLoja" runat="server" 
                                ControlToValidate="drpLoja" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vendedor" SortExpression="IdFunc">
                        <ItemTemplate>
                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("IdFunc") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="drpVendedor" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsVendedor" DataTextField="Nome" DataValueField="IdFunc" 
                                SelectedValue='<%# Bind("IdFunc") %>' 
                                Enabled='<%# Eval("DropVendedorEnabled") %>' 
                                ondatabinding="drpVendedor_DataBinding">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvVendedor" runat="server" 
                                ControlToValidate="drpVendedor" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="drpVendedorIns" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsVendedor" DataTextField="Nome" DataValueField="IdFunc" 
                                SelectedValue='<%# Bind("IdFunc") %>'>
                                <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvVendedor" runat="server" 
                                ControlToValidate="drpVendedorIns" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Contato Obra" SortExpression="ContatoObra">
                        <ItemTemplate>
                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("ContatoObra") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="50" 
                                Text='<%# Bind("ContatoObra") %>' Width="200px"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="50" 
                                Text='<%# Bind("ContatoObra") %>' Width="200px"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Situação" InsertVisible="false"
                        SortExpression="Situacao">
                        <InsertItemTemplate>
                            <span style="white-space: nowrap">
                                <asp:DropDownList ID="drpSituacao" runat="server" onchange="alteraSituacao(this.value)"
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Selected="True" Value="1">Aberta</asp:ListItem>
                                    <asp:ListItem Value="2">Em andamento</asp:ListItem>
                                </asp:DropDownList>
                                <span id="nomeMedidor">
                                    &nbsp;
                                    Medidor
                                    <asp:DropDownList ID="drpMedidor" runat="server" 
                                        DataSourceID="odsMedidores" DataTextField="Nome" DataValueField="IdFunc" 
                                        SelectedValue='<%# Bind("IdFuncMed") %>' AppendDataBoundItems="True">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="ctvMedidor" runat="server" ErrorMessage="*" ControlToValidate="drpMedidor"
                                        ClientValidationFunction="validaMedidor" Display="Dynamic" 
                                        ValidateEmptyText="True"></asp:CustomValidator>
                                </span>
                            </span>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label12" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <span style="white-space: nowrap">
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    Enabled='<%# Eval("SituacaoEnabled") == null ? false : Eval("SituacaoEnabled") %>' 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Aberta</asp:ListItem>
                                    <asp:ListItem Value="2">Em andamento</asp:ListItem>
                                    <asp:ListItem Value="4">Remarcada</asp:ListItem>
                                    <asp:ListItem Value="5">Cancelada</asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField runat="server" ID="hdfMedidor" Value='<%# Bind("IdFuncMed") %>'></asp:HiddenField>
                            </span>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Med. Definitiva" 
                        SortExpression="MedicaoDefinitiva">
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkDefinitiva" runat="server"
                            Checked='<%# Eval("MedicaoDefinitiva") %>'
                             onchange="alterarValor();" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:CheckBox ID="chkDefinitiva" runat="server" 
                                onchange="alterarValor();" />                    
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label15" runat="server" Text='<%# Eval("MedicaoDefinitiva") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtReferencia" runat="server" MaxLength="200" Rows="2" 
                                Text='<%# Bind("Referencia") %>' TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtReferencia" runat="server" MaxLength="200" Rows="2" 
                                Text='<%# Bind("Referencia") %>' TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label13" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                        <ItemTemplate>
                            <asp:Label ID="Label19" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("Valor") %>'
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Valor") %>'
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Forma pagto." SortExpression="FormaPagto">
                        <ItemTemplate>
                            <asp:Label ID="Label17" runat="server" Text='<%# Bind("FormaPagto") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtFormaPagto" runat="server" MaxLength="200" Rows="2" 
                                Text='<%# Bind("FormaPagto") %>' TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtFormaPagto" runat="server" MaxLength="200" 
                                onload="txtFormaPagto_Load" Rows="2" Text='<%# Bind("FormaPagto") %>' 
                                TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Instalação" 
                        SortExpression="DataInstalacaoString">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDataInst" runat="server" 
                                Text='<%# Bind("DataInstalacao") %>' Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataInst" runat="server" ImageAlign="AbsMiddle" 
                                ImageUrl="~/Images/calendario.gif" 
                                OnClientClick="return SelecionaData('txtDataInst', this)" ToolTip="Alterar" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDataInst" runat="server" 
                                Text='<%# Bind("DataInstalacao") %>' Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataInst" runat="server" ImageAlign="AbsMiddle" 
                                ImageUrl="~/Images/calendario.gif" 
                                OnClientClick="return SelecionaData('txtDataInst', this)" ToolTip="Alterar" />
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label18" runat="server" 
                                Text='<%# Bind("DataInstalacao") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Observação" SortExpression="ObsMedicao">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("ObsMedicao") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("ObsMedicao") %>' 
                                MaxLength="500" Rows="3" TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="3" 
                                Text='<%# Bind("ObsMedicao") %>' TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                                Text="Atualizar" 
                                onclientclick="if (!insertOrUpdate()) return false;" />
                            <asp:Button ID="btnCancelar" runat="server" onclick="btnCancelar_Click" 
                                Text="Cancelar" CausesValidation="false" />
                            <asp:HiddenField ID="hdfIdFuncMed" runat="server" 
                                Value='<%# Bind("IdFuncMed") %>' />
                            <asp:HiddenField ID="hdfDataEfetuar" runat="server" 
                                Value='<%# Bind("DataEfetuar") %>' />
                            <asp:HiddenField ID="hdfNumSeq" runat="server" Value='<%# Bind("NumSeq") %>' />
                            <asp:HiddenField ID="hdfLatitude" runat="server" 
                                Value='<%# Bind("Latitude") %>' />
                            <asp:HiddenField ID="hdfLongitude" runat="server" 
                                Value='<%# Bind("Longitude") %>' />
                            <asp:HiddenField ID="hdfIdMedicao" runat="server" 
                                Value='<%# Eval("IdMedicao") %>' />
                            <asp:HiddenField ID="hdfIdOrcamento" runat="server" 
                                Value='<%# Bind("IdOrcamento") %>' />
                            <asp:HiddenField ID="hdfMedicaoDef" runat="server" 
                                Value='<%# Bind("MedicaoDefinitiva") %>' />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                                Text="Inserir" onclientclick="if (!insertOrUpdate()) return false;" />
                            <asp:Button ID="btnCancelar0" runat="server" onclick="btnCancelar_Click" 
                                Text="Cancelar" CausesValidation="false" />
                           <asp:HiddenField ID="hdfIdOrcamento" runat="server" 
                                Value='<%# Bind("IdOrcamento") %>' />
                            <asp:HiddenField ID="hdfMedicaoDef" runat="server" 
                                Value='<%# Bind("MedicaoDefinitiva") %>' />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <InsertRowStyle HorizontalAlign="Left" />
                <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                <AlternatingRowStyle ForeColor="Black" />
            </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMedicao" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.Medicao" InsertMethod="Insert" 
                    SelectMethod="GetMedicao" TypeName="Glass.Data.DAL.MedicaoDAO" 
                    UpdateMethod="UpdateMedicao" MaximumRowsParameterName="" 
                    oninserted="odsMedicao_Inserted" onupdated="odsMedicao_Updated" 
                    StartRowIndexParameterName="">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idMedicao" QueryStringField="idMedicao" 
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" 
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" 
                    SelectMethod="GetTurnoMedicao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetListSituacao" 
                    TypeName="Glass.Data.DAL.LojaDAO" >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="situacao" Type="Int32" />
                        <asp:Parameter Name="sortExpression" Type="String" />
                        <asp:Parameter Name="startRow" Type="Int32" />
                        <asp:Parameter Name="pageSize" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMedidores" runat="server" 
                    SelectMethod="GetMedidores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <script type="text/javascript">
        if (FindControl("drpSituacao", "select") != null)
            alteraSituacao(FindControl("drpSituacao", "select").value);
    
        function alterarDisableCliente(valorBooleano)
        {
            var nomeCliente = FindControl("txtCliente", "input");
            var telCliente = FindControl("txtTelCliente","input");
            var CelCliente = FindControl("txtCelCliente", "input");
            var email = FindControl("txtEmail", "input");
            var cep = FindControl("txtCep", "input");
            var endereco = FindControl("txtEndereco", "input");
            var compl = FindControl("txtCompl", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");

            if (nomeCliente != null)
                nomeCliente.disabled = valorBooleano;
            if (telCliente != null)
                telCliente.disabled = valorBooleano;
            if (CelCliente != null)
                CelCliente.disabled = valorBooleano;
            if (email != null)
                email.disabled = valorBooleano;
            if (cep != null)
                cep.disabled = valorBooleano;
            if (endereco != null)
                endereco.disabled = valorBooleano;
            if (compl != null)
                compl.disabled = valorBooleano;
            if (bairro != null)
                bairro.disabled = valorBooleano;
            if (cidade != null)
                cidade.disabled = valorBooleano;
        }

        alterarDisableCliente(<%= Glass.Configuracoes.MedicaoConfig.MedicaoApenasClienteCadastrado.ToString().ToLower() %>);
    
        // Bloqueia ou habilita o campo Orçamento de acordo com o parâmetro informado.
        function alterarDisableOrcamento(valorBooleano)
        {
            var idOrcamento = FindControl("txtIdOrcamento", "input");

            if (idOrcamento != null)
                idOrcamento.disabled = valorBooleano == "true";

            FindControl("chkDefinitiva", "input").disabled = valorBooleano == "true";
        }

        // Verifica se a medição que está sendo editada é uma medição definitiva de algum orçamento.
        var retornoMedicaoDefinivaOrcamento = CadMedicao.VerificarMedicaoDefinitivaOrcamento(<%= !string.IsNullOrEmpty(Request["idMedicao"]) ? Request["idMedicao"] : "0" %>);
        
        if (retornoMedicaoDefinivaOrcamento.error != null) {
            // Caso ocorra algum erro, desabilita o campo Orçamento.
            alterarDisableOrcamento(true);
            alert(retornoMedicaoDefinivaOrcamento.error.description);
        }
        else if (retornoMedicaoDefinivaOrcamento.value.split(';')[0] == "Erro") {
            // Caso ocorra algum erro, desabilita o campo Orçamento.
            alterarDisableOrcamento(true);
            alert(retornoMedicaoDefinivaOrcamento.value.split(';')[1]);
        }
        else
            // Altera o disable do campo Orçamento de acordo com o retorno do Ajax.
            alterarDisableOrcamento(retornoMedicaoDefinivaOrcamento.value.split(';')[1]);
        
    </script>
</asp:Content>

