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
        <section>
            <a :href="obterLinkInserirCliente()" v-if="configuracoes.cadastrarCliente">
                Inserir Cliente
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="atualizarClientes" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum cliente encontrado">
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
                    <td>{{ item.id }} - {{ item.nomeCliente }}</td>
                    <td>{{ item.cpfCnpj }}</td>
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
        <section class="links">
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
            <div>
                <span v-if="configuracoes.ativarClientes">
                    <a href="#" @click.prevent="ativarClientes(true)">
                        Ativar clientes
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
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstClientes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstClientes.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
