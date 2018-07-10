<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCliente.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCliente" Title="Clientes" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Clientes/Templates/LstClientes.Filtro.html")
    %>
    <div id="app">
        <cliente-filtros :filtro.sync="filtro"></cliente-filtros>
        <section v-if="configuracoes.cadastrarCliente">
            <a :href="obterLinkInserirCliente()">
                Inserir Cliente
            </a>
        </section>
        <section>
            <lista-paginada :funcao-recuperar-itens="atualizarClientes" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum cliente encontrado">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('endereco')">Endereço</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telCont')">Tel. cont.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telCel')">Celular</a>
                    </th>
                    <th v-if="configuracoes.cadastrarCliente">
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('email')">Email</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('ultCompra')">Ult. compra</a>
                    </th>
                    <th v-if="configuracoes.exibirTotalComprado">
                        <a href="#" @click.prevent="ordenar('totalComprado')">Total comprado</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarCliente(item)" title="Editar" v-if="configuracoes.cadastrarCliente">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Cancelar" v-if="configuracoes.cadastrarCliente">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirTabelaDescontoAcrescimo(item)" title="Descontos/Acréscimos" v-if="item.permissoes.cadastrarDescontoTabela">
                            <img border="0" src="../Images/money_delete.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos" v-if="configuracoes.anexarImagens">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <a :href="obterLinkSugestoes(item)" title="Sugestões" v-if="configuracoes.cadastrarSugestoes">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="alterarSituacao(item)" title="Ativa/Inativar" v-if="item.permissoes.alterarSituacao">
                            <img border="0" src="../Images/Inativar.gif">
                        </a>
                    </td>
                    <td>{{ item.id }} - {{ item.nome }}</td>
                    <td>{{ item.enderecoCompleto }}</td>
                    <td>{{ item.telefoneContato }}</td>
                    <td>{{ item.celular }}</td>
                    <td v-if="configuracoes.cadastrarCliente">{{ item.situacao }}</td>
                    <td>{{ item.email }}</td>
                    <td>{{ item.dataUltimaCompra | data }}</td>
                    <td v-if="configuracoes.exibirTotalComprado">{{ item.totalComprado | moeda }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Cliente" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <a href="#" @click.prevent="abrirFichaCliente(item)" title="Ficha cliente" v-if="configuracoes.imprimir">
                            <img border="0" src="../Images/printer.png">
                        </a>
                        <a :href="obterLinkPrecosTabela(item)" title="Preços de tabela" v-if="configuracoes.consultarPrecoTabela">
                            <img border="0" src="../Images/cifrao.png">
                        </a>
                        <img border="0" src="../Images/user_headset.gif" v-if="item.atendente && item.atendente.id > 0" :title="'Atendente: ' + item.atendente.nome" />
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section>
            <div>
                <span v-if="configuracoes.imprimir">
                    <a href="#" @click.prevent="abrirListaClientes(false, false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span v-if="configuracoes.imprimir">
                    <a href="#" @click.prevent="abrirListaClientes(false, true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
                <span v-if="configuracoes.alterarVendedor">
                    <a href="#" @click.prevent="abrirAlteracaoVendedor(true)">
                        Alterar vendedor
                    </a>
                </span>
                <span v-if="configuracoes.alterarRota">
                    <a href="#" @click.prevent="abrirAlteracaoRota(true)">
                        Alterar rota
                    </a>
                </span>
            </div>
            <div>
                <span v-if="configuracoes.imprimir">
                    <a href="#" @click.prevent="abrirListaClientes(true, false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha
                    </a>
                </span>
                <span v-if="configuracoes.imprimir">
                    <a href="#" @click.prevent="abrirListaClientes(true, true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar ficha para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstClientes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstClientes.js" />
        </Scripts>
    </asp:ScriptManager>


    <script type="text/javascript">

        function alteraVendedor(idVendedorNovo) {
            FindControl("hdfIdVendedorNovo", "input").value = idVendedorNovo;
            FindControl("btnAlterarVendedorCliente", "input").click();
        }

        function alterarRota(idRotaNova) {
            FindControl("hdfIdRotaNova", "input").value = idRotaNova;
            FindControl("btnAlterarRotaCliente", "input").click();
        }

        function openRpt(exportarExcel, ficha) {
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var cpfCnpj = FindControl("txtCnpj", "input").value;
            var telefone = FindControl("txtTelefone", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            var codRota = FindControl("txtRota", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var agruparVend = FindControl("chkAgruparVend", "input").checked;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataSemCompraIni = FindControl("ctrlDataSemCompraIni_txtData", "input").value;
            var dataSemCompraFim = FindControl("ctrlDataSemCompraFim_txtData", "input").value;
            var dataInativadoIni = FindControl("ctrlDataInIni_txtData", "input").value;
            var dataInativadoFim = FindControl("ctrlDataInFim_txtData", "input").value;
            var idCidade = FindControl("hdfCidade", "input").value;
            var idTipoCliente = FindControl("chkTipoCliente", "select").itens();
            var tipoFiscal = FindControl("cblTipoFiscal", "select").itens();
            var formasPagto = FindControl("cblFormasPagto", "select").itens();
            var controlTabelaDesconto = FindControl("drpTabelaDescontoAcrescimo", "select");
            var idTabelaDesconto = controlTabelaDesconto != null ? controlTabelaDesconto.value : 0;
            var apenasSemRota = FindControl("chkApenasSemRota", "input") == null ? "false" : FindControl("chkApenasSemRota", "input").checked;
            var exibirHistorico = FindControl("chkExibirHistorico", "input") != null ? FindControl("chkExibirHistorico", "input").checked : false;
            var uf = FindControl("drpUf", "select").value;

            if (nomeCli.indexOf('&') >= 0) {
                alert("O filtro Nome/Apelido do cliente não deve conter o caractere '&', pois ele é utilizado como chave para geração do relatório. Tente filtrar sem o nome do cliente ou apenas com a primeira parte do nome antes do '&'.");
                return false;
            }

            
            if (idCli == "" && nomeCli == "" && cpfCnpj == "" && telefone == "" && endereco == "" && bairro == "" &&
                situacao == "" && codRota == "" && idLoja == "" && idFunc == "" && dataCadIni == "" &&
                dataCadFim == "" && dataSemCompraIni == "" && dataSemCompraFim == "" && dataInativadoIni == "" && dataInativadoFim == "" &&
                idCidade == "" && idTipoCliente == "" && tipoFiscal == "" && formasPagto === "" && idTabelaDesconto == "" && uf == "" && !apenasSemRota) {
                if (!confirm("É recomendável aplicar um filtro. Deseja realmente prosseguir?"))
                    return false;
            }

            if (idCli == "")
                idCli = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=" + (ficha == true ? "Ficha" : "Lista") + "Clientes&dataIni=&dataFim=&Revenda=0&tipoPessoa=0&Compra=0" +
                "&idCli=" + idCli + "&nome=" + nomeCli + "&cpfCnpj=" + cpfCnpj + "&telefone=" + telefone + "&endereco=" + endereco + "&idCidade=" + idCidade +
                "&bairro=" + bairro + "&situacao=" + situacao + "&codRota=" + codRota + "&idFunc=" + idFunc + "&idLoja=" + idLoja + "&idTipoCliente=" + idTipoCliente +
                "&tipoFiscal=" + tipoFiscal + "&formasPagto=" + formasPagto + "&exportarExcel=" + exportarExcel + "&agruparVend=" + agruparVend + "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim +
                "&dataSemCompraIni=" + dataSemCompraIni + "&dataSemCompraFim=" + dataSemCompraFim + "&dataInativadoIni=" + dataInativadoIni + "&dataInativadoFim=" + dataInativadoFim +
                "&idTabelaDesconto=" + idTabelaDesconto + "&apenasSemRota=" + apenasSemRota + "&exibirHistorico=" + exibirHistorico + "&uf=" + uf);

            return false;
        }

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function setCidade(idCidade, nomeCidade) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }

        function ativarTodos() {
            return confirm("ATENÇÃO: Essa opção ativará TODOS os clientes inativos que se encaixam nos filtros especificados.\nDeseja continuar?");
        }
        
    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Cód. Cliente" AssociatedControlID="txtNumCli"></asp:Label>
                <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                    onblur="getCli(this);"></asp:TextBox>
            </span>
            <span>
                <asp:Label ID="Label12" runat="server" Text="Nome/Apelido" AssociatedControlID="txtNome"></asp:Label>
                <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                    CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" AssociatedControlID="txtCnpj"></asp:Label>
                <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label22" runat="server" Text="Loja Cliente" AssociatedControlID="drpLoja"></asp:Label>
                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                    DataTextField="Name" DataValueField="Id" AutoPostBack="True">
                    <asp:ListItem Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span>
                <asp:Label ID="Label8" runat="server" Text="Situação" AssociatedControlID="drpSituacao"></asp:Label>
                <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" DataSourceID="odsSituacaoCliente"
                    DataTextField="Translation" DataValueField="Value" OnDataBound="drpSituacao_DataBound">
                </sync:CheckBoxListDropDown>
                <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Telefone" AssociatedControlID="txtTelefone"></asp:Label>
                <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label9" runat="server" Text="Endereço" AssociatedControlID="txtEndereco"></asp:Label>
                <asp:TextBox ID="txtEndereco" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label10" runat="server" Text="Bairro" AssociatedControlID="txtBairro"></asp:Label>
                <asp:TextBox ID="txtBairro" runat="server" Width="120px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label17" runat="server" Text="Cidade" AssociatedControlID="txtCidade"></asp:Label>
                <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Width="200px" onkeypress="return false;"></asp:TextBox>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar"
                    OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
            </span>
            <span>
                <asp:Label ID="Label24" runat="server" Text="UF" AssociatedControlID="txtCidade"></asp:Label>
                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True"
                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label18" runat="server" Text="Tipo" AssociatedControlID="chkTipoCliente"></asp:Label>
                <sync:CheckBoxListDropDown ID="chkTipoCliente" runat="server" DataSourceID="odsTipoCliente"
                    DataTextField="Name" DataValueField="Id">
                </sync:CheckBoxListDropDown>

                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label20" runat="server" Text="Tipo Fiscal" AssociatedControlID="cblTipoFiscal"></asp:Label>
                <sync:checkboxlistdropdown ID="cblTipoFiscal" runat="server" Title="Selecione o tipo fiscal">
                    <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                    <asp:ListItem Value="2">Revenda</asp:ListItem>
                </sync:checkboxlistdropdown>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label13" runat="server" Text="Rota" AssociatedControlID="txtRota"></asp:Label>
                <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClick="imgPesq_Click" OnClientClick="return openRota();" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label14" runat="server" Text="Vendedor" AssociatedControlID="drpFuncionario"></asp:Label>
                <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                    DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:CheckBox ID="chkAgruparVend" runat="server" Text="Agrupar por Vendedor" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label21" runat="server" Text="Data Cad." AssociatedControlID="ctrlDataCadIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label15" runat="server" Text="Período sem comprar" AssociatedControlID="ctrlDataSemCompraIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataSemCompraIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataSemCompraFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label16" runat="server" Text="Período em que o cliente foi inativado"
                    AssociatedControlID="ctrlDataInIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataInIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataInFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div runat="server" id="tbDescAcresc">
            <span>
                <asp:Label ID="Label19" runat="server" Text="Tabela Desconto/Acréscimo" AssociatedControlID="drpTabelaDescontoAcrescimo"></asp:Label>
                <asp:DropDownList ID="drpTabelaDescontoAcrescimo" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsTabelaDescontoAcrescimo" DataTextField="Name" DataValueField="Id"
                    AutoPostBack="True">
                    <asp:ListItem Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span>
                <asp:CheckBox ID="chkApenasSemRota" runat="server" Text="Apenas clientes sem rota vinculada"
                    AutoPostBack="True" />
            </span>
             <span>
                <asp:CheckBox ID="chkExibirHistorico" runat="server" Text="Exibir Histórico" />
            </span>
        </div>
    </div>
    <asp:GridView ID="grdCli" runat="server" SkinID="defaultGridView"
        DataSourceID="odsCliente" DataKeyNames="IdCli" EmptyDataText="Nenhum cliente cadastrado"
        OnPageIndexChanged="grdCli_PageIndexChanged" OnRowCommand="grdCli_RowCommand"
        OnDataBound="grdCli_DataBound">
        <Columns>
        </Columns>
        <PagerStyle />
        <EditRowStyle />
        <AlternatingRowStyle />
    </asp:GridView>
    <div class="inserir">
        <asp:LinkButton ID="lnkAtivarTodos" runat="server" OnClick="lnkAtivarTodos_Click"
            OnClientClick="return ativarTodos();" Visible="False">Ativar Clientes Inativos</asp:LinkButton>
    </div>
    <div class="imprimir">
        <div>
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false); return false;">
                <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkAlterarVendedor" runat="server" OnClientClick="openAlterarVendedor(); return false;">Alterar Vendedor</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkAlterarRota" runat="server" OnClientClick="openAlterarRota(); return false;">Alterar Rota</asp:LinkButton>
        </div>
        <div style="margin-top: 10px">
            <asp:LinkButton ID="lnkImprimirFicha" runat="server" OnClientClick="return openRpt(false, true);">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarFicha" runat="server" OnClientClick="openRpt(true, true); return false;">
                <img border="0" src="../Images/Excel.gif" /> Exportar ficha para o Excel</asp:LinkButton>
        </div>
    </div>
    <asp:HiddenField ID="hdfCidade" runat="server" Value='' />
    <div style="display: none">
        <asp:HiddenField ID="hdfIdVendedorNovo" runat="server" />
        <asp:Button ID="btnAlterarVendedorCliente" runat="server" OnClick="btnAlterarVendedorCliente_Click" />
        <asp:HiddenField ID="hdfIdRotaNova" runat="server" />
        <asp:Button ID="btnAlterarRotaCliente" runat="server" OnClick="btnAlterarRotaCliente_Click" />
    </div>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Cliente"
        DeleteMethod="ApagarCliente" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarClientes" 
        SelectByKeysMethod="ObtemCliente"
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:Parameter Name="dataNascimentoIni" Type="DateTime" />
            <asp:Parameter Name="dataNascimentoFim" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
            <asp:Parameter Name="limite" DefaultValue="0" Type="Int32" />
            <asp:Parameter Name="tipoPessoa" DefaultValue="" Type="String" />
            <asp:Parameter Name="comCompra" DefaultValue="False" Type="Boolean" />
        </SelectParameters>
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsClienteAtualizarVendedor" runat="server" 
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="AlterarVendedorClientes">
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues"  />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="hdfIdVendedorNovo" Name="idVendedorNovo" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
        </UpdateParameters>
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsClienteAlterarRota" runat="server" 
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="AlterarRotaClientes">
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues"  />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataSemCompraFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="hdfIdRotaNova" Name="idRotaNova" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
        </UpdateParameters>
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAtivarClientesInativos" runat="server" 
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="AtivarClientesInativos">
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
        </UpdateParameters>
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsTabelaDescontoAcrescimo" runat="server"
        SelectMethod="ObtemDescritoresTabelaDescontoAcrescimo" TypeName="Glass.Global.Negocios.IClienteFluxo">
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource ID="odsUf" runat="server" SelectMethod="GetUf" 
        TypeName="Glass.Data.DAL.CidadeDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
