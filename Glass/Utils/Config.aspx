<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="Config.aspx.cs" Inherits="Glass.UI.Web.Utils.SystemConfig" Title="Configurações" %>

<%@ Reference Control="~/Controls/ctrlLogPopup.ascx" %>
<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Reference Control="~/Controls/ctrlSelPopup.ascx" %>
<%@ Reference Control="~/Controls/ctrlData.ascx" %>
<%@ Register Src="../Controls/ctrlConfigAresta.ascx" TagName="ctrlConfigAresta" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlConfigFaixasRentabilidadeComissao.ascx" TagName="ctrlConfigFaixasRentabilidadeComissao" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlConfigFaixasRentabilidadeLiberacao.ascx" TagName="ctrlConfigFaixasRentabilidadeLiberacao" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style title="text/css">
        .aba, .painel {
            width: 880px;
        }

        .aba {
            position: relative;
            left: -9px;
            padding-bottom: 6px;
        }

            .aba span {
                padding: 6px;
                margin-right: 3px;
                cursor: pointer;
            }

        .painel {
            border: 1px solid gray;
            height: 500px;
            vertical-align: top;
            padding: 8px;
            overflow: auto;
        }
    </style>
    <script type="text/javascript">
        // Variáveis
        var bBanco = false;
        var salvando = false;
        var botaoSalvarPrecoClicado = false;

        function atualizarPagina() {
            var idLoja = FindControl("drpLoja", "select").value
            var aba = FindControl("hdfAba", "input").value;
            redirectUrl("Config.aspx?idLoja=" + idLoja + "&aba=" + aba);
        }

        // -----------------------------------
        // Função que muda a aba ativa na tela
        // -----------------------------------
        function mudaAba(nomeAba) {
            if (salvando)
                return;

            // variável que contém os identificadores das abas
            var abas = new Array("geral", "comissao", "precoProduto", "financeiro", "planoconta", "orcamento", "pedido", "projeto", "nfe", "pcp", "liberarPedido", "rentabilidade", "internas");

            // percorre todas as abas
            for (i = 0; i < abas.length; i++) {
                // recupera o título de aba atual e altera dependendo do parâmetro
                var aba = document.getElementById("aba_" + abas[i]);
                var borda = (abas[i] == nomeAba) ? "1px solid gray" : "1px solid silver";
                var bordaInferior = (abas[i] == nomeAba) ? "1px solid white" : "";
                aba.style.fontWeight = (abas[i] == nomeAba) ? "bold" : "normal";
                aba.style.borderTop = borda;
                aba.style.borderLeft = borda;
                aba.style.borderRight = borda;
                aba.style.borderBottom = bordaInferior;

                // recupera a aba atual e exibe/esconde dependendo do parâmetro
                var aba = document.getElementById(abas[i]);
                aba.style.display = (abas[i] == nomeAba) ? "block" : "none";
            }

            // altera o hiddenfield que guarda a aba atual
            document.getElementById("<%= hdfAba.ClientID %>").value = nomeAba;

            if (nomeAba == 'rentabilidade') {

            }
        }

        // ---------------------------------------------------------------------------------
        // Função responsável por habilitar/desabilitar o campo de texto do ajuste de preço.
        // ---------------------------------------------------------------------------------
        function ajustePrecoCheck(checkbox, texto, hidden) {
            texto.disabled = !checkbox.checked;
            hidden.value = checkbox.checked ? texto.value : -1;
        }

        // ------------------------------------------------------------------
        // Função responsável por alterar o preço do produto no campo hidden.
        // ------------------------------------------------------------------
        function alteraPreco(texto, hidden) {
            hidden.value = texto.value;
        }

        // --------------------------------------------------------------------------------------
        // Função responsável por garantir que todos os dados sejam inseridos no ajuste de preço.
        // --------------------------------------------------------------------------------------
        function validarPreco(controle, tipo) {
            // Chamado 12466. Caso o botão tenha sido clicado, não deve ser permitido clicá-lo novamente,
            // para evitar que o ajuste seja feito em duplicidade.
            if (botaoSalvarPrecoClicado) {
                controle.disabled = true;
                return false;
            }
            // Informa que o botão foi clicado, para que, caso seja clicado novamente, não salve as informações no banco de dados duplicadamente.
            botaoSalvarPrecoClicado = true;

            var chkAtacado = FindControl("chkAtacado" + tipo, "input");
            var chkBalcao = FindControl("chkBalcao" + tipo, "input");
            var chkObra = FindControl("chkObra" + tipo, "input");

            var txbAtacado = FindControl("txbAtacado" + tipo, "input");
            var txbBalcao = FindControl("txbBalcao" + tipo, "input");
            var txbObra = FindControl("txbObra" + tipo, "input");

            if (chkAtacado.checked && txbAtacado.value == "") {
                alert("Preencha o campo 'Atacado' ou desmarque essa opção.");
                // Informa que o botão não executou a operação de atualização das informações.
                botaoSalvarPrecoClicado = false;
                return false;
            }

            if (chkBalcao.checked && txbBalcao.value == "") {
                alert("Preencha o campo 'Balcão' ou desmarque essa opção.");
                botaoSalvarPrecoClicado = false;
                return false;
            }

            if (chkObra.checked && txbObra.value == "") {
                alert("Preencha o campo 'Obra' ou desmarque essa opção.");
                botaoSalvarPrecoClicado = false;
                return false;
            }

            if (tipo == "Benef" && FindControl("drpTipoBenef", "select").value == "0") {
                alert("Selecione um tipo de beneficiamento para fazer o reajuste.");
                botaoSalvarPrecoClicado = false;
                return false;
            }

            return true;
        }

        // ------------------------------------------------------------------
        // Função que valida os processos/aplicações escolhidos pelo usuário.
        // ------------------------------------------------------------------
        function validarProjeto() {
            var processoInst = FindControl("txtTexto_19", "input") != null ? FindControl("txtTexto_19", "input").value : null;
            var aplicacaoInst = FindControl("txtTexto_20", "input") != null ? FindControl("txtTexto_20", "input").value : null;
            var processoCx = FindControl("txtTexto_21", "input") != null ? FindControl("txtTexto_21", "input").value : null;
            var aplicacaoCx = FindControl("txtTexto_22", "input") != null ? FindControl("txtTexto_22", "input").value : null;

            if (processoInst != null && processoInst != "" && SystemConfig.ProcessoExiste(processoInst).value != "true") {
                alert("Processo da instalação não existe.");
                return false;
            }

            if (aplicacaoInst != null && aplicacaoInst != "" && SystemConfig.AplicacaoExiste(aplicacaoInst).value != "true") {
                alert("Aplicação da instalação não existe.");
                return false;
            }

            if (processoCx != null && processoCx != "" && SystemConfig.ProcessoExiste(processoCx).value != "true") {
                alert("Processo do caixilho não existe.");
                return false;
            }

            if (aplicacaoCx != null && aplicacaoCx != "" && SystemConfig.AplicacaoExiste(aplicacaoCx).value != "true") {
                alert("Aplicação do caixilho não existe.");
                return false;
            }

            return true;
        }

        // --------------------------------------------------------------------------
        // Função que indica se os campos de valor da configuração estão preenchidos.
        // --------------------------------------------------------------------------
        function isConfigVazio(tabela) {
            /*
            for (i = 0; i < tabela.rows.length; i++)
            {
            var controle = tabela.rows[i].cells[1].children[0];
            var valor = null;
            try
            {
            valor = controle.value;
            }
            catch (err)
            {
            continue;
            }

            if (valor.length == 0)
            {
            alert("O campo '" + tabela.rows[i].cells[0].innerHTML + "' não pode ser vazio.");
            return true;
            }
            }
            */

            return false;
        }

        var planoContaControl = "";

        // ------------------------------------------------------
        // Função que salva o plano de conta selecionado no popup
        // ------------------------------------------------------
        function setPlanoConta(idConta, descricao) {
            FindControl("lbl" + planoContaControl, "span").innerHTML = descricao;
            FindControl("hdf" + planoContaControl, "input").value = idConta;
        }

        // ------------------------------------------------------------
        // Função que indica se o botão de salvar pode ser pressionado.
        // ------------------------------------------------------------
        function alterarSalvar(botao, salvar) {
            botao.disabled = salvar;
            salvando = salvar;
        }

        // -------------------------------------------------------------------------------------------
        // Retorna o valor que será salvo no HiddenField para configurações de tipo "GrupoEnumMetodo".
        // -------------------------------------------------------------------------------------------
        function retornaValor(nomeCampo, tipo) {
            var valores = new Array();

            var num = 0;
            while (true) {
                var campo = FindControl(nomeCampo + "_" + num++, tipo);
                if (campo == null)
                    break;

                valores.push(campo.type.toLowerCase() != "checkbox" ? campo.value : campo.checked);
            }

            return valores.join(';');
        }

        // -----------------------------------------------
        // Função que recupera os valores da configuração.
        // -----------------------------------------------
        function getTexto(tabela) {
            var texto = new String();
            var i = new Number();

            for (i = 0; i < tabela.rows.length; i++) {
                if (tabela.rows[i].cells.length == 1) {
                    var novaTabela = tabela.rows[i].cells[0].getElementsByTagName("table")[0];
                    texto += getTexto(novaTabela, tabela.rows[i].cells.length == 2);
                }
                else {
                    var valor = null;
                    var controle = null;

                    if (tabela.rows[i].cells.length == 3)
                        controle = tabela.rows[i].cells[1].children[0];
                    else if (tabela.rows[i].cells[0].children[0].children[0] != null)
                        controle = tabela.rows[i].cells[0].children[0].children[0].children[1];
                    else
                        controle = tabela.rows[i].cells[0].children[0];

                    if (controle.nodeName.toLowerCase() == "div")
                        valor = controle.children[1].value;
                    else if (controle.type == "checkbox")
                        valor = controle.checked;
                    else
                        valor = controle.value;

                    texto += "~" + controle.id + "^" + valor;
                }
            }

            return texto;
        }

        // --------------------------------------------
        // Função que salva os valores da configuração.
        // --------------------------------------------
        function salvarConfig(botao, tipo) {
            var tabela = null;
            switch (tipo) {
                case "geral":
                    tabela = document.getElementById("<%= tblGeral.ClientID %>");
                    break;
                case "comissao":
                    tabela = document.getElementById("<%= tblComissao.ClientID %>");
                    break;
                case "financeiro":
                    tabela = document.getElementById("<%= tblFinanceiro.ClientID %>");
                    break;
                case "orcamento":
                    tabela = document.getElementById("<%= tblOrcamento.ClientID %>");
                    break;
                case "pedido":
                    tabela = document.getElementById("<%= tblPedido.ClientID %>");
                    break;
                case "projeto":
                    if (!validarProjeto()) return;
                    tabela = document.getElementById("<%= tblProjeto.ClientID %>");
                    break;
                case "nfe":
                    tabela = document.getElementById("<%= tblNFe.ClientID %>");
                    break;
                case "pcp":
                    tabela = document.getElementById("<%= tblPCP.ClientID %>");
                    break;
                case "liberarPedido":
                    tabela = document.getElementById("<%= tblLiberarPedido.ClientID %>");
                    break;
                case "rentabilidade":
                    tabela = document.getElementById("<%= tblRentabilidade.ClientID %>");
                    break;
                case "internas":
                    tabela = document.getElementById("<%= tblInternas.ClientID %>");
                    break;
            }

            if (tabela == null) {
                alert("Tipo inválido '" + tipo + "'.");
                return;
            }

            if (isConfigVazio(tabela) || !validate(tipo))
                return;

            alterarSalvar(botao, true);

            var resposta = SystemConfig.Salvar(getTexto(tabela).substr(1), FindControl("drpLoja", "select").value).value.split('|');
            alert(resposta[1]);

            if (resposta[0] == "Erro")
                alterarSalvar(botao, false);
            else
                atualizarPagina();
        }

        // -------------------------------------
        // Salva a configuracao da aresta
        // -------------------------------------
        function salvarAresta(botao) {
            alterarSalvar(botao, true);

            var idLoja = FindControl("drpLoja", "select").value;

            var resposta = SystemConfig.SalvarConfiguracaoAresta(idLoja, ObterconfigAresta()).value.split(';');

            alert(resposta[1]);

            if (resposta[0] == "Erro")
                alterarSalvar(botao, false);
            else
                atualizarPagina();
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja"
                                DataTextField="Name" DataValueField="Id" AutoPostBack="True"
                                OnSelectedIndexChanged="drpLoja_SelectedIndexChanged">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server"
                                SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <div align="left" class="aba">
                    <span id="aba_geral" onclick="mudaAba('geral')">Geral
                    </span>
                    <span id="aba_comissao" onclick="mudaAba('comissao')">Comissão
                    </span>
                    <span id="aba_precoProduto" onclick="mudaAba('precoProduto')">Ajuste de preço
                    </span>
                    <span id="aba_financeiro" onclick="mudaAba('financeiro')">Financeiro
                    </span>
                    <span id="aba_planoconta" onclick="mudaAba('planoconta')">Plano Conta
                    </span>
                    <span id="aba_orcamento" onclick="mudaAba('orcamento')">Orçamento
                    </span>
                    <span id="aba_pedido" onclick="mudaAba('pedido')">Pedido
                    </span>
                    <span id="aba_pcp" onclick="mudaAba('pcp')" runat="server" clientidmode="Static">PCP
                    </span>
                    <span id="aba_liberarPedido" onclick="mudaAba('liberarPedido')">Liberação
                    </span>
                    <span id="aba_projeto" onclick="mudaAba('projeto')">Projeto
                    </span>
                    <span id="aba_nfe" onclick="mudaAba('nfe')">Fiscal
                    </span>
                    <span id="aba_rentabilidade" onclick="mudaAba('rentabilidade')">Rentab.</span>
                    <span id="aba_internas" onclick="mudaAba('internas')" runat="server" clientidmode="Static">INTERNAS
                    </span>
                </div>
                <div class="painel">
                    <div id="geral">
                        <fieldset style="padding: 8px">
                            <legend>Telas para administradores </legend>
                            <asp:LinkButton ID="lnkUsuariosLogados" runat="server"
                                OnClick="lnkUsuariosLogados_Click" CausesValidation="False">
                             Usuários Logados</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkControleBenef" runat="server"
                                OnClick="lnkControleBenef_Click" CausesValidation="False">Controle de Beneficiamentos</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkRelatorioDinamico" runat="server"
                                OnClick="lnkRelatorioDinamico_Click" CausesValidation="False">Relatório Dinâmico</asp:LinkButton>
                            <br />
                            <br />
                            <asp:LinkButton ID="lnkPrecoBenef" runat="server"
                                PostBackUrl="~/Listas/LstBenefConfig.aspx" CausesValidation="False">Preços de Beneficiamentos</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkEstoque" runat="server" CausesValidation="False" PostBackUrl="~/Listas/LstEstoque.aspx">Lançar Estoque</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkEstoqueFiscal" runat="server" CausesValidation="False" PostBackUrl="~/Listas/LstEstoque.aspx?fiscal=1">Lançar Estoque Fiscal</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkLoginSistema" runat="server" CausesValidation="False" PostBackUrl="~/Utils/LoginSistema.aspx">Utilização do Sistema</asp:LinkButton>
                            <br />
                            <br />
                            <asp:LinkButton ID="lnkExportarImportar" runat="server" CausesValidation="False" PostBackUrl="~/Cadastros/Projeto/ExportarImportar.aspx">Exportar/Importar/Duplicar Modelo de Projeto</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExportarImportarFerragem" runat="server" CausesValidation="False" PostBackUrl="~/Cadastros/Projeto/ExportarImportarFerragem.aspx">Exportar/Importar Ferragem</asp:LinkButton>
                            <span runat="server" id="consultaEnvio">&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkConsultaEnvio" runat="server" OnClick="lnkConsultaEnvio_Click">Consultar Envio de</asp:LinkButton>
                            </span>
                            <br />
                            <br />
                            <asp:LinkButton ID="lnkIntegradores" runat="server" CausesValidation="False" PostBackUrl="~/Listas/LstIntegradores.aspx">Integradores</asp:LinkButton>
                        </fieldset>
                        <fieldset runat="server" id="configGeral">
                            <legend>Configurações gerais </legend>
                            <asp:Table ID="tblGeral" runat="server" OnLoad="tblGeral_Load">
                            </asp:Table>
                            <asp:Button ID="btnSalvarGeral" runat="server" Text="Salvar"
                                OnClientClick="salvarConfig(this, 'geral'); return false"
                                ValidationGroup="geral" />
                        </fieldset>
                    </div>
                    <div id="comissao">
                        <asp:Table ID="tblComissao" runat="server" OnLoad="tblComissao_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarComissao" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'comissao'); return false"
                            ValidationGroup="comissao" />
                         <br />  <br />
                        <fieldset>
                            <legend>
	                            Comissão Gerente
                            </legend>
                            <table>
                                <tr>
                                    <td nowrap="nowrap">
                                        Selecione o funcionário que receberá comissão de todos os pedidos
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="drpGerente" runat="server" AutoPostBack="true" DataSourceID="odsGerente" AppendDataBoundItems="true"
                                            DataTextField="Nome" DataValueField="IdFunc" OnSelectedIndexChanged="drpGerente_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGerente" runat="server" SelectMethod="GetOrdered"
                                            TypeName="Glass.Data.DAL.FuncionarioDAO">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                            <asp:DetailsView ID="dtvComissaoGerente" runat="server"  AutoGenerateRows="false"  OnDataBound="dtvComissaoGerente_DataBound"
                                DataSourceID="odsComissaoGerente" DefaultMode="Edit" GridLines="None" Width="40%">
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:HiddenField ID="hdfIdFuncGerente" runat="server" Value='<%# Bind("IdFuncionario") %>' />
                                            <asp:Label runat="server" ID="lblGerentes"></asp:Label>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                             <table width="100%" cellpadding="0" cellspacing="0" >
                                                <tr>
                                                   <td align="center" colspan="2"> Percentual de comissao por tipo de pedido</td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Venda:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbVenda" runat="server" Text='<%# Bind("PercentualVenda") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualVenda" runat="server" Tabela="ComissaoConfigGerente" IdRegistro='<%# Eval("IdComissaoConfigGerente") %>' Campo="Percentual Venda" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Revenda:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbRevenda" runat="server" Text='<%# Bind("PercentualRevenda") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualRevenda" runat="server" Tabela="ComissaoConfigGerente" IdRegistro='<%# Eval("IdComissaoConfigGerente") %>' Campo="Percentual Revenda" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Mão de Obra:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbMaoDeObra" runat="server" Text='<%# Bind("PercentualMaoDeObra") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualMaoDeObra" runat="server" Tabela="ComissaoConfigGerente" IdRegistro='<%# Eval("IdComissaoConfigGerente") %>' Campo="Percentual Mão Obra" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Mão de Obra Especial:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbMaoDeObraEspecial" runat="server" Text='<%# Bind("PercentualMaoDeObraEspecial") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualMaoDeObraEspecial" runat="server" Tabela="ComissaoConfigGerente" IdRegistro='<%# Eval("IdComissaoConfigGerente") %>' Campo="Percentual Mão Obra Especial" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" OnClick="btnSalvar_Click" Text="Salvar"
                                                ValidationGroup="comissaoGerente" />
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </fieldset>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissaoGerente" runat="server"
                            SelectMethod="GetForConfig"
                            TypeName="Glass.Data.DAL.ComissaoConfigGerenteDAO"
                            DataObjectTypeName="Glass.Data.Model.ComissaoConfigGerente">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                                <asp:ControlParameter ControlID="drpGerente" Name="idFunc" PropertyName="SelectedValue" Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <br />
                        <table>
                            <tr>
                                <td nowrap="nowrap">Selecione o funcionário:
                                </td>
                                <td>
                                    <asp:DropDownList ID="drpFunc" runat="server" AutoPostBack="True" DataSourceID="odsFunc"
                                        DataTextField="Nome" DataValueField="IdFunc" OnSelectedIndexChanged="drpFunc_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetVendedoresComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">
                            <SelectParameters>
                                <asp:Parameter DefaultValue="true" Name="incluirInstaladores" Type="Boolean" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <br />
                        <fieldset style='<%= !UsarDescontoComissao() ? "display: none": "" %>'>
                            <legend>Desconto da Comissão
                            </legend>
                            <asp:DetailsView ID="dtvDescontoComissao" runat="server" AutoGenerateRows="False"
                                DataSourceID="odsDescontoComissao" DefaultMode="Edit" GridLines="None"
                                OnLoad="dtvDescontoComissao_Load">
                                <FieldHeaderStyle HorizontalAlign="Right" />
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                            <asp:HiddenField ID="hdfIdDescontoComissaoConfig" runat="server" Value='<%# Bind("IdDescontoComissaoConfig") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">Desconto dado no<br />
                                                        pedido (%)
                                                    </td>
                                                    <td align="left" style="padding-left: 33px">Percentual de redução<br />
                                                        na comissão
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="1ª" SortExpression="FaixaUm">
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("FaixaUm") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa6" runat="server" Text='<%# Bind("FaixaUm") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa6" runat="server" Text='<%# Bind("PercFaixaUm") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("FaixaUm") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="2ª" SortExpression="FaixaDois">
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("FaixaDois") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa7" runat="server" Text='<%# Bind("FaixaDois") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa7" runat="server" Text='<%# Bind("PercFaixaDois") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("FaixaDois") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="3ª" SortExpression="FaixaTres">
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("FaixaTres") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa8" runat="server" Text='<%# Bind("FaixaTres") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa8" runat="server" Text='<%# Bind("PercFaixaTres") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("FaixaTres") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="4ª" SortExpression="FaixaQuatro">
                                        <ItemTemplate>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("FaixaQuatro") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa9" runat="server" Text='<%# Bind("FaixaQuatro") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa9" runat="server" Text='<%# Bind("PercFaixaQuatro") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("FaixaQuatro") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="5ª" SortExpression="FaixaCinco">
                                        <ItemTemplate>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("FaixaCinco") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa10" runat="server" Text='<%# Bind("FaixaCinco") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa10" runat="server" Text='<%# Bind("PercFaixaCinco") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("FaixaCinco") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" CommandName="Update" Text="Salvar"
                                                ValidationGroup="descontoComissao" />
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:DetailsView>
                        </fieldset>
                        <fieldset>
                            <legend>Comissão
                            </legend>
                            <asp:DetailsView ID="dtvComissao" runat="server" AutoGenerateRows="False" DataSourceID="odsComissao"
                                DefaultMode="Edit" GridLines="None">
                                <FieldHeaderStyle HorizontalAlign="Right" />
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                            <asp:HiddenField ID="hdfIdComissaoConfig" runat="server" Value='<%# Bind("IdComissaoConfig") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                             <table width="100%" cellpadding="0" cellspacing="0" >
                                                <tr>
                                                   <td align="center" colspan="2"> Percentual de comissao por tipo de pedido</td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Venda:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbVenda" runat="server" Text='<%# Bind("PercentualVenda") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualVenda" runat="server" Tabela="ComissaoConfig" IdRegistro='<%# Eval("IdComissaoConfig") %>' Campo="Percentual Venda" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Revenda:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbRevenda" runat="server" Text='<%# Bind("PercentualRevenda") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualRevenda" runat="server" Tabela="ComissaoConfig" IdRegistro='<%# Eval("IdComissaoConfig") %>' Campo="Percentual Revenda" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Mão de Obra:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbMaoDeObra" runat="server" Text='<%# Bind("PercentualMaoDeObra") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualMaoDeObra" runat="server" Tabela="ComissaoConfig" IdRegistro='<%# Eval("IdComissaoConfig") %>' Campo="Percentual Mão Obra" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">Mão de Obra Especial:</td>
                                                    <td align="right">
                                                         <asp:TextBox ID="txbMaoDeObraEspecial" runat="server" Text='<%# Bind("PercentualMaoDeObraEspecial") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopupPercentualMaoDeObraEspecial" runat="server" Tabela="ComissaoConfig" IdRegistro='<%# Eval("IdComissaoConfig") %>' Campo="Percentual Mão Obra Especial" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left" style='<%= ExibirFaixaValor() ? "": "display: none" %>'>
                                                        <asp:Label ID="lblFaixasValor" runat="server" Text="Faixas de valor"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblPercentualComissao" runat="server" Text="Percentual de comissão"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="1ª" SortExpression="FaixaUm">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("FaixaUm") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left" style='<%= ExibirFaixaValor() ? "": "display: none" %>'>
                                                        <asp:TextBox ID="txbFaixa1" runat="server" Text='<%# Bind("FaixaUm") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa1" runat="server" Text='<%# Bind("PercFaixaUm") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdComissaoConfig") %>' Tabela="ComissaoConfig" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("FaixaUm") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="2ª" SortExpression="FaixaDois">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("FaixaDois") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa2" runat="server" Text='<%# Bind("FaixaDois") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa2" runat="server" Text='<%# Bind("PercFaixaDois") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("FaixaDois") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="3ª" SortExpression="FaixaTres">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("FaixaTres") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa3" runat="server" Text='<%# Bind("FaixaTres") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa3" runat="server" Text='<%# Bind("PercFaixaTres") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("FaixaTres") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="4ª" SortExpression="FaixaQuatro">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("FaixaQuatro") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa4" runat="server" Text='<%# Bind("FaixaQuatro") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa4" runat="server" Text='<%# Bind("PercFaixaQuatro") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("FaixaQuatro") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="5ª" SortExpression="FaixaCinco">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("FaixaCinco") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:TextBox ID="txbFaixa5" runat="server" Text='<%# Bind("FaixaCinco") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right">
                                                        <asp:TextBox ID="txbPercFaixa5" runat="server" Text='<%# Bind("PercFaixaCinco") %>'
                                                            OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("FaixaCinco") %>'></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Percentual único" ShowHeader="False" SortExpression="PercUnico">
                                        <InsertItemTemplate>
                                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("PercUnico") %>'
                                                ToolTip="Define que caso esteja sendo utilizada mais de uma faixa de valor, a última faixa atingida será utilizada para o cálculo de toda a comissão, mas caso esta opção fique desmarcada, a comissão será calculada com o percentual definido em cada faixa de valor." />
                                        </InsertItemTemplate>
                                        <EditItemTemplate>
                                            <asp:CheckBox ID="chkPercentualUnico" runat="server" Checked='<%# Bind("PercUnico") %>'
                                                Text="Percentual único"
                                                ToolTip="Define que caso esteja sendo utilizada mais de uma faixa de valor, a última faixa atingida será utilizada para o cálculo de toda a comissão, mas caso esta opção fique desmarcada, a comissão será calculada com o percentual definido em cada faixa de valor." />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("PercUnico") %>' Enabled="false" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" CommandName="Update" Text="Salvar"
                                                ValidationGroup="comissao" />
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </fieldset>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissao" runat="server" SelectMethod="GetComissaoConfig"
                            TypeName="Glass.Data.DAL.ComissaoConfigDAO" DataObjectTypeName="Glass.Data.Model.ComissaoConfig"
                            UpdateMethod="InsertOrUpdate" OnUpdated="odsComissao_Updated">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpFunc" Name="idFunc" PropertyName="SelectedValue"
                                    Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <div id="descontoComissao" runat="server" onload="descontoComissao_Load">
                            <br />
                            <br />
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDescontoComissao" runat="server" SelectMethod="GetDescontoComissaoConfig"
                                TypeName="Glass.Data.DAL.DescontoComissaoConfigDAO" DataObjectTypeName="Glass.Data.Model.DescontoComissaoConfig"
                                UpdateMethod="InsertOrUpdate" OnUpdated="odsDescontoComissao_Updated">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="drpFunc" Name="idFunc" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </div>
                    </div>
                    <div id="precoProduto">
                        <fieldset>
                            <legend>Produtos</legend>
                            <table>
                                <tr>
                                    <td>Grupo de produtos
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlGrupoProd" runat="server" DataSourceID="odsGrupoProd" DataTextField="Descricao"
                                            DataValueField="IdGrupoProd" AutoPostBack="True" OnSelectedIndexChanged="ddlGrupoProd_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                                            TypeName="Glass.Data.DAL.GrupoProdDAO" OnDataBinding="GrupoSubgrupo_DataBinding">
                                            <SelectParameters>
                                                <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Sub-grupo
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSubgrupoProd" runat="server" DataSourceID="odsSubgrupoProd"
                                            DataTextField="Descricao" DataValueField="IdSubgrupoProd" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlSubgrupoProd_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
                                            TypeName="Glass.Data.DAL.SubgrupoProdDAO" OnDataBinding="GrupoSubgrupo_DataBinding">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="ddlGrupoProd" Name="idGrupo" PropertyName="SelectedValue"
                                                    Type="Int32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Preço base
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpPrecoBase" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpPrecoBase_SelectedIndexChanged">
                                            <asp:ListItem Value="0">Custo Forn.</asp:ListItem>
                                            <asp:ListItem Value="1">Custo Imp.</asp:ListItem>
                                            <asp:ListItem Value="2">Atacado</asp:ListItem>
                                            <asp:ListItem Value="3">Balcão</asp:ListItem>
                                            <asp:ListItem Value="4">Obra</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <p>
                                Marque o tipo de preço que será atualizado e digite a
                                <br />
                                porcentagem que será calculada em cima do preço base
                                <br />
                                para cada produto do grupo/subgrupo selecionado
                            </p>
                            <asp:DetailsView ID="dtvLogProduto" runat="server" AutoGenerateRows="False" DataSourceID="odsLogProduto"
                                DefaultMode="Insert" GridLines="None" DataKeyNames="IdLogProduto">
                                <Fields>
                                    <asp:TemplateField HeaderText="Custo Forn." SortExpression="AjusteCustoFabBase">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbCustoFabBaseProd" runat="server" Text='<%# Eval("AjusteCustoFabBase") %>'
                                                onblur="alteraPreco(this, FindControl('hdfCustoFabBaseProd', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfCustoFabBaseProd" runat="server" Value='<%# Bind("AjusteCustoFabBase") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkCustoFabBaseProd" runat="server" Checked="True" onclick="ajustePrecoCheck(this, FindControl('txbCustoFabBaseProd', 'input'), FindControl('hdfCustoFabBaseProd', 'input'))"
                                                Text="Custo Forn." />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("AjusteCustoFabBase") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Custo Imp." SortExpression="AjusteCustoCompra">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbCustoCompraProd" runat="server" Text='<%# Eval("AjusteCustoCompra") %>'
                                                onblur="alteraPreco(this, FindControl('hdfCustoCompraProd', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfCustoCompraProd" runat="server" Value='<%# Bind("AjusteCustoCompra") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkCustoCompraProd" runat="server" Checked="True" onclick="ajustePrecoCheck(this, FindControl('txbCustoCompraProd', 'input'), FindControl('hdfCustoCompraProd', 'input'))"
                                                Text="Custo Imp." />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("AjusteCustoCompra") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Atacado" SortExpression="AjusteAtacado">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("AjusteAtacado") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbAtacadoProd" runat="server" Text='<%# Eval("AjusteAtacado") %>'
                                                onblur="alteraPreco(this, FindControl('hdfAtacadoProd', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfAtacadoProd" runat="server" Value='<%# Bind("AjusteAtacado") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAtacadoProd" runat="server" Text="Atacado" onclick="ajustePrecoCheck(this, FindControl('txbAtacadoProd', 'input'), FindControl('hdfAtacadoProd', 'input'))"
                                                Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Balcão" SortExpression="AjusteBalcao">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("AjusteBalcao") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbBalcaoProd" runat="server" Text='<%# Eval("AjusteBalcao") %>'
                                                onblur="alteraPreco(this, FindControl('hdfBalcaoProd', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfBalcaoProd" runat="server" Value='<%# Bind("AjusteBalcao") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkBalcaoProd" runat="server" onclick="ajustePrecoCheck(this, FindControl('txbBalcaoProd', 'input'), FindControl('hdfBalcaoProd', 'input'))"
                                                Text="Balcão" Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Obra" SortExpression="AjusteObra">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("AjusteObra") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbObraProd" runat="server" Text='<%# Eval("AjusteObra") %>'
                                                onblur="alteraPreco(this, FindControl('hdfObraProd', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfObraProd" runat="server" Value='<%# Bind("AjusteObra") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkObraProd" runat="server" onclick="ajustePrecoCheck(this, FindControl('txbObraProd', 'input'), FindControl('hdfObraProd', 'input'))"
                                                Text="Obra" Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" CommandName="Insert" Text="Salvar"
                                                OnClientClick="return validarPreco(this, 'Prod');" ValidationGroup="precoProdutos" />
                                            <asp:HiddenField ID="hdfGrupoProd" runat="server" OnDataBinding="hdfGrupoProd_DataBinding"
                                                Value='<%# Bind("IdGrupoProd") %>' />
                                            <asp:HiddenField ID="hdfSubgrupoProd" runat="server" OnDataBinding="hdfSubgrupoProd_DataBinding"
                                                Value='<%# Bind("IdSubgrupoProd") %>' />
                                            <asp:HiddenField ID="hdfIdFunc" runat="server" OnDataBinding="hdfIdFunc_DataBinding"
                                                Value='<%# Bind("IdFunc") %>' />
                                            <asp:HiddenField ID="hdfTipoPrecoBase" runat="server" OnDataBinding="hdfTipoPrecoBase_DataBinding"
                                                Value='<%# Bind("TipoPrecoBase") %>' />
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLogProduto" runat="server" DataObjectTypeName="Glass.Data.Model.LogProduto"
                                SelectMethod="GetElementByProdGrupoSubgrupo" TypeName="Glass.Data.DAL.LogProdutoDAO"
                                InsertMethod="Insert" OnInserted="odsLogProduto_Inserted" OnInserting="odsLogProduto_Inserting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlGrupoProd" Name="grupo" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="ddlSubgrupoProd" Name="subgrupo" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <center>
                                <br />
                                Últimas modificações no preço desse grupo/sub-grupo
                                <asp:GridView ID="grdViewLogProduto" runat="server" GridLines="None" PageSize="5"
                                    AutoGenerateColumns="False" DataKeyNames="IdLogProduto" DataSourceID="odsViewLogProduto"
                                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                    EditRowStyle-CssClass="edit" EmptyDataText="Não há modificações recentes" AllowPaging="True"
                                    AllowSorting="True">
                                    <Columns>
                                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                                        <asp:BoundField DataField="DescrTipoPrecoBase" HeaderText="Preço base" SortExpression="TipoPrecoBase" />
                                        <asp:BoundField DataField="ColunaCustoFabBase" HeaderText="Custo Forn." SortExpression="AjusteCustoFabBase" />
                                        <asp:BoundField DataField="ColunaCustoCompra" HeaderText="Custo Imp." SortExpression="AjusteCustoCompra" />
                                        <asp:BoundField DataField="ColunaAtacado" HeaderText="Atacado" SortExpression="AjusteAtacado" />
                                        <asp:BoundField DataField="ColunaBalcao" HeaderText="Balcão" SortExpression="AjusteBalcao" />
                                        <asp:BoundField DataField="ColunaObra" HeaderText="Obra" SortExpression="AjusteObra" />
                                        <asp:BoundField DataField="DataAjusteString" HeaderText="Data" SortExpression="DataAjuste" />
                                    </Columns>
                                    <PagerStyle />
                                    <EditRowStyle />
                                    <AlternatingRowStyle />
                                </asp:GridView>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsViewLogProduto" runat="server" SelectMethod="GetByProdGrupoSubgrupo"
                                    TypeName="Glass.Data.DAL.LogProdutoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetByProdGrupoSubgrupoCount" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlGrupoProd" Name="grupo" PropertyName="SelectedValue"
                                            Type="UInt32" />
                                        <asp:ControlParameter ControlID="ddlSubgrupoProd" Name="subgrupo" PropertyName="SelectedValue"
                                            Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </center>
                        </fieldset>
                        <fieldset style='display: <%= Glass.Configuracoes.Geral.NaoVendeVidro() ? "none" : "" %>'>
                            <legend>Beneficiamentos</legend>
                            <table>
                                <tr>
                                    <td>Tipo de beneficiamento
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoBenef" runat="server" DataSourceID="odsTipoBenef" DataTextField="Descricao"
                                            DataValueField="IdBenefConfig" AutoPostBack="True" AppendDataBoundItems="True"
                                            OnSelectedIndexChanged="drpTipoBenef_SelectedIndexChanged">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoBenef" runat="server" SelectMethod="GetForControl"
                                            TypeName="Glass.Data.DAL.BenefConfigDAO" OnDataBinding="GrupoSubgrupo_DataBinding">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                            <p>
                                Marque o tipo de preço que será atualizado e digite a
                                <br />
                                porcentagem que será calculada em cima do preço de
                                <br />
                                custo para cada beneficiamento do tipo selecionado
                            </p>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLogBenef" runat="server" DataObjectTypeName="Glass.Data.Model.BenefConfigPreco"
                                SelectMethod="GetByIdBenefConfig" TypeName="Glass.Data.DAL.BenefConfigPrecoDAO"
                                InsertMethod="AtualizarTelaConfig" OnInserted="odsLogBenef_Inserted" OnInserting="odsLogBenef_Inserting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="drpTipoBenef" Name="idBenefConfig" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                    <asp:Parameter Name="idSubgrupoProd" Type="UInt32" />
                                    <asp:Parameter Name="idCorVidro" Type="UInt32" />
                                    <asp:Parameter Name="espessura" Type="Int32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <asp:DetailsView ID="dtvLogBenef" runat="server" AutoGenerateRows="False" DataSourceID="odsLogBenef"
                                DefaultMode="Insert" GridLines="None" DataKeyNames="IdBenefConfigPreco">
                                <Fields>
                                    <asp:TemplateField HeaderText="Atacado" SortExpression="AjusteAtacado">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("AjusteAtacado") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbAtacadoBenef" runat="server" Text='<%# Eval("AjusteAtacado") %>'
                                                onblur="alteraPreco(this, FindControl('hdfAtacadoBenef', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfAtacadoBenef" runat="server" Value='<%# Bind("AjusteAtacado") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAtacadoBenef" runat="server" Text="Atacado" onclick="ajustePrecoCheck(this, FindControl('txbAtacadoBenef', 'input'), FindControl('hdfAtacadoBenef', 'input'))"
                                                Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Balcão" SortExpression="AjusteBalcao">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("AjusteBalcao") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbBalcaoBenef" runat="server" Text='<%# Eval("AjusteBalcao") %>'
                                                onblur="alteraPreco(this, FindControl('hdfBalcaoBenef', 'input'))" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfBalcaoBenef" runat="server" Value='<%# Bind("AjusteBalcao") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkBalcaoBenef" runat="server" onclick="ajustePrecoCheck(this, FindControl('txbBalcaoBenef', 'input'), FindControl('hdfBalcaoBenef', 'input'))"
                                                Text="Balcão" Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Obra" SortExpression="AjusteObra">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("AjusteObra") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txbObraBenef" runat="server" Text='<%# Eval("AjusteObra") %>' onblur="alteraPreco(this, FindControl('hdfObraBenef', 'input'))"
                                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            %
                                            <asp:HiddenField ID="hdfObraBenef" runat="server" Value='<%# Bind("AjusteObra") %>' />
                                        </EditItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkObraBenef" runat="server" onclick="ajustePrecoCheck(this, FindControl('txbObraBenef', 'input'), FindControl('hdfObraBenef', 'input'))"
                                                Text="Obra" Checked="True" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" CommandName="Insert" Text="Salvar"
                                                OnClientClick="return validarPreco(this, 'Benef');"
                                                ValidationGroup="precoBeneficiamentos" />
                                            <asp:HiddenField ID="hdfBenef" runat="server" OnDataBinding="hdfBenef_DataBinding"
                                                Value='<%# Bind("IdBenefConfig") %>' />
                                            <asp:HiddenField ID="hdfTipoBenef" runat="server" OnDataBinding="hdfTipoBenef_DataBinding"
                                                Value='<%# Bind("IdSubgrupoProd") %>' />
                                            <%-- <asp:HiddenField ID="hdfIdFuncBenef" runat="server" OnDataBinding="hdfIdFuncBenef_DataBinding"
                                                Value='<%# Bind("IdFunc") %>' />--%>
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <center>
                                <br />
                                Últimas modificações no preço desses beneficiamentos
                                <asp:GridView ID="grdViewLogBenef" runat="server" GridLines="None" PageSize="5" AutoGenerateColumns="False"
                                    DataKeyNames="IdLogProduto" DataSourceID="odsViewLogBenef" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    EmptyDataText="Não há modificações recentes" AllowPaging="True" AllowSorting="True">
                                    <Columns>
                                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                                        <asp:BoundField DataField="DescrTipoPrecoBase" HeaderText="Preço base" SortExpression="TipoPrecoBase" />
                                        <asp:BoundField DataField="ColunaAtacado" HeaderText="Atacado" SortExpression="AjusteAtacado" />
                                        <asp:BoundField DataField="ColunaBalcao" HeaderText="Balcão" SortExpression="AjusteBalcao" />
                                        <asp:BoundField DataField="ColunaObra" HeaderText="Obra" SortExpression="AjusteObra" />
                                        <asp:BoundField DataField="DataAjusteString" HeaderText="Data" SortExpression="DataAjuste" />
                                    </Columns>
                                    <PagerStyle />
                                    <EditRowStyle />
                                    <AlternatingRowStyle />
                                </asp:GridView>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsViewLogBenef" runat="server" SelectMethod="GetByProdGrupoSubgrupo"
                                    TypeName="Glass.Data.DAL.LogProdutoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetByProdGrupoSubgrupoCount" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="0" Name="grupo" Type="UInt32" />
                                        <asp:ControlParameter ControlID="drpTipoBenef" DefaultValue="" Name="subgrupo" PropertyName="SelectedValue"
                                            Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </center>
                        </fieldset>
                    </div>
                    <div id="financeiro">
                        <asp:Table ID="tblFinanceiro" runat="server" OnLoad="tblFinanceiro_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarFinanceiro" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'financeiro'); return false"
                            ValidationGroup="financeiro" />
                        <br />
                        <br />
                        <br />

                    </div>
                    <div id="planoconta">
                        <table>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label12" runat="server" Text="Tx. Antecip." Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblTxAntecip" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq" runat="server" OnClientClick="planoContaControl='TxAntecip'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label13" runat="server" Text="Juros Antecip." Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblJurosAntecip" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="planoContaControl='JurosAntecip'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label14" runat="server" Text="IOF Antecip." Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblIOFAntecip" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq1" runat="server" OnClientClick="planoContaControl='IOFAntecip'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label15" runat="server" Text="Juros Recebimento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblJurosReceb" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq2" runat="server" OnClientClick="planoContaControl='JurosReceb'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label16" runat="server" Text="Multa Recebimento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblMultaReceb" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq3" runat="server" OnClientClick="planoContaControl='MultaReceb'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label17" runat="server" Text="Juros Pagamento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblJurosPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq4" runat="server" OnClientClick="planoContaControl='JurosPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label18" runat="server" Text="Multa Pagamento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblMultaPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="lnkPesq5" runat="server" OnClientClick="planoContaControl='MultaPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label19" runat="server" Text="Estorno Juros Recebimento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstJurosReceb" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="planoContaControl='EstJurosReceb'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label21" runat="server" Text="Estorno Multa Recebimento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstMultaReceb" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="planoContaControl='EstMultaReceb'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label23" runat="server" Text="Estorno Juros Pagamento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstJurosPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="planoContaControl='EstJurosPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label25" runat="server" Text="Estorno Multa Pagamento" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstMultaPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="planoContaControl='EstMultaPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label20" runat="server" Text="Comissão" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblComissao" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton5" runat="server" OnClientClick="planoContaControl='Comissao'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label26" runat="server" Text="Quitação Parcelas de Cartões" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblQuitacaoParcelaCartao" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton10" runat="server" OnClientClick="planoContaControl='QuitacaoParcelaCartao'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label28" runat="server" Text="Estorno Quitação Parcelas de Cartões" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstornoQuitacaoParcelaCartao" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton11" runat="server" OnClientClick="planoContaControl='EstornoQuitacaoParcelaCartao'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label22" runat="server" Text="Juros Venda Cartão" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblJurosCartao" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton6" runat="server" OnClientClick="planoContaControl='JurosCartao'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label24" runat="server" Text="Estorno Juros Venda Cartão" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstJurosCartao" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton7" runat="server" OnClientClick="planoContaControl='EstJurosCartao'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label27" runat="server" Text="Tarifa para o uso de boleto" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblTarifaUsoBoleto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton8" runat="server" OnClientClick="planoContaControl='TarifaUsoBoleto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label29" runat="server" Text="Tarifa para o uso de protesto" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblTarifaUsoProtesto" runat="server">Nenhum plano de conta associado.</asp:Label>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LinkButton9" runat="server" OnClientClick="planoContaControl='TarifaUsoProtesto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdfTxAntecip" runat="server" />
                        <asp:HiddenField ID="hdfJurosAntecip" runat="server" />
                        <asp:HiddenField ID="hdfIOFAntecip" runat="server" />
                        <asp:HiddenField ID="hdfJurosReceb" runat="server" />
                        <asp:HiddenField ID="hdfMultaReceb" runat="server" />
                        <asp:HiddenField ID="hdfJurosPagto" runat="server" />
                        <asp:HiddenField ID="hdfMultaPagto" runat="server" />
                        <asp:HiddenField ID="hdfEstJurosReceb" runat="server" />
                        <asp:HiddenField ID="hdfEstMultaReceb" runat="server" />
                        <asp:HiddenField ID="hdfEstJurosPagto" runat="server" />
                        <asp:HiddenField ID="hdfEstMultaPagto" runat="server" />
                        <asp:HiddenField ID="hdfComissao" runat="server" />
                        <asp:HiddenField ID="hdfQuitacaoParcelaCartao" runat="server" />
                        <asp:HiddenField ID="hdfEstornoQuitacaoParcelaCartao" runat="server" />
                        <asp:HiddenField ID="hdfJurosCartao" runat="server" />
                        <asp:HiddenField ID="hdfEstJurosCartao" runat="server" />
                        <asp:HiddenField ID="hdfTarifaUsoBoleto" runat="server" />
                        <asp:HiddenField ID="hdfTarifaUsoProtesto" runat="server" />
                        <br />
                        <asp:Button ID="btnSalvarPlanoConta" runat="server" Text="Salvar"
                            OnClick="btnSalvarPlanoConta_Click" ValidationGroup="planosConta" />
                    </div>
                    <div id="orcamento">
                        <asp:Table ID="tblOrcamento" runat="server" OnLoad="tblOrcamento_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarOrcamento" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'orcamento'); return false"
                            ValidationGroup="orcamento" />
                    </div>
                    <div id="pedido">
                        <asp:Table ID="tblPedido" runat="server" OnLoad="tblPedido_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarPedido" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'pedido'); return false"
                            ValidationGroup="pedido" />
                    </div>
                    <div id="liberarPedido">
                        <asp:Table ID="tblLiberarPedido" runat="server" OnLoad="tblLiberarPedido_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarLiberar" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'liberarPedido'); return false"
                            ValidationGroup="liberarPedido" />
                    </div>
                    <div id="pcp">
                        <asp:Table ID="tblPCP" runat="server" OnLoad="tblPCP_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarPCP" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'pcp'); return false" ValidationGroup="pcp" />
                        <br />
                        <br />
                        <br />
                        <fieldset>
                            <legend>Configuração da Aresta</legend>
                            <table>
                                <tr>
                                    <td>
                                        <uc1:ctrlConfigAresta runat="server" ID="ctrlConfigAresta" OnLoad="ctrlConfigAresta_Load"
                                            ClientIDMode="Static" />
                                    </td>
                                </tr>
                                <tr align="center">
                                    <td>
                                        <asp:Button ID="btnSalvarAresta" runat="server" Text="Salvar"
                                            OnClientClick="salvarAresta(this); return false;" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                    <div id="projeto">
                        <asp:Table ID="tblProjeto" runat="server" OnLoad="tblProjeto_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarProjeto" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'projeto'); return false"
                            ValidationGroup="projeto" />
                    </div>
                    <div id="nfe">
                        <asp:Table ID="tblNFe" runat="server" OnLoad="tblNFe_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarNFe" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'nfe'); return false" ValidationGroup="nfe" />
                    </div>

                    <div id="rentabilidade">
                        <asp:Table ID="tblRentabilidade" runat="server" OnLoad="tblRentabilidade_Load">
                        </asp:Table>
                        <asp:Button ID="Button1" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'rentabilidade'); return false"
                            ValidationGroup="rentabilidade" />
                         <br />  <br />

                        <table>
                            <tr>
                                <td nowrap="nowrap">Selecione o funcionário:
                                </td>
                                <td>
                                    <asp:DropDownList ID="drpFuncFaixaRentabilidadeComissao"
                                        runat="server" AutoPostBack="True" DataSourceID="odsFunc"
                                        DataTextField="Nome" DataValueField="IdFunc"
                                        OnSelectedIndexChanged="drpFuncFaixaRentabilidadeComissao_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>

                        <fieldset>
                            <legend>Faixas de Rentabilidade para Comissão
                            </legend>
                            <uc1:ctrlConfigFaixasRentabilidadeComissao ID="ctrlFaixasRentabilidadeComissao" runat="server" />
                        </fieldset>

                        <fieldset>
                            <legend>Faixas de Rentabilidade para Liberção
                            </legend>
                            <uc1:ctrlConfigFaixasRentabilidadeLiberacao ID="ctrlFaixasRentabilidadeLiberacao" runat="server" />
                        </fieldset>

                    </div>
                    <div id="internas">
                        <asp:Table ID="tblInternas" runat="server" OnLoad="tblInternas_Load">
                        </asp:Table>
                        <asp:Button ID="btnSalvarInternas" runat="server" Text="Salvar"
                            OnClientClick="salvarConfig(this, 'internas'); return false" ValidationGroup="internas" />
                    </div>
                </div>
        </tr>
    </table>
    <asp:HiddenField ID="hdfAba" runat="server" Value="geral" />

    <script type="text/javascript">
        mudaAba(document.getElementById("<%= hdfAba.ClientID %>").value);
    </script>

</asp:Content>
