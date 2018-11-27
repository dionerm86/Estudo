<%@ Page Title="Regras de Natureza de Operação" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstRegraNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRegraNaturezaOperacao" EnableEventValidation="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cfops/NaturezasOperacao/RegrasNaturezaOperacao/Templates/LstRegrasNaturezaOperacao.Filtro.html")
    %>
    <div id="app">
        <section>
            <regras-natureza-operacao-filtros :filtro.sync="filtro"></regras-natureza-operacao-filtros>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma regra de natureza de operação encontrada."
                :numero-registros="10" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        Loja
                    </th>
                    <th>
                        Tipo cliente
                    </th>
                    <th>
                        Grupo/Subgrupo
                    </th>
                    <th>
                        Cor/Espessura
                    </th>
                    <th>
                        UF's destino
                    </th>
                    <th>
                        Natureza operação produção
                    </th>
                    <th>
                        Natureza operação revenda
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td v-if="item.loja">{{ item.loja.nome }}</td>
                    <td v-if="item.tipoCliente">{{ item.tipoCliente.nome }}</td>
                    <td>
                        <template v-if="item.produto && item.produto.grupoProduto">
                            {{ item.produto.grupoProduto.nome }}
                        </template>
                        <template v-if="item.produto && item.produto.subgrupoProduto && item.produto.subgrupoProduto.id">
                            / {{ item.produto.subgrupoProduto.nome }}
                        </template>
                    </td>
                    <td>
                        <template v-if="item.produto && item.produto.cores && item.produto.cores.vidro">
                            {{ item.produto.cores.vidro.nome }}
                        </template>
                        <template v-else-if="item.produto && item.produto.cores && item.produto.cores.ferragem">
                            {{ item.produto.cores.ferragem.nome }}
                        </template>
                        <template v-else-if="item.produto && item.produto.cores && item.produto.cores.aluminio">
                            {{ item.produto.cores.aluminio.nome }}
                        </template>
                        <template v-if="item.produto && item.produto.espessura > 0">
                            / {{ item.produto.espessura }}mm
                        </template>
                    </td>
                    <td>{{ obterUfsDestino(item) }}</td>
                    <td>
                        <span class="form-group" v-if="item.naturezaOperacaoProducao && item.naturezaOperacaoProducao.intraestadual && item.naturezaOperacaoProducao.intraestadual.id > 0">
                            Intra.: {{ item.naturezaOperacaoProducao.intraestadual.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoProducao && item.naturezaOperacaoProducao.interestadual && item.naturezaOperacaoProducao.interestadual.id > 0">
                            Inter.: {{ item.naturezaOperacaoProducao.interestadual.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoProducao && item.naturezaOperacaoProducao.intraestadualComSt && item.naturezaOperacaoProducao.intraestadualComSt.id > 0">
                            Intra. ST: {{ item.naturezaOperacaoProducao.intraestadualComSt.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoProducao && item.naturezaOperacaoProducao.interestadualComSt && item.naturezaOperacaoProducao.interestadualComSt.id > 0">
                            Inter. ST: {{ item.naturezaOperacaoProducao.interestadualComSt.nome }}
                        </span>
                    </td>
                    <td>
                        <span class="form-group" v-if="item.naturezaOperacaoRevenda && item.naturezaOperacaoRevenda.intraestadual && item.naturezaOperacaoRevenda.intraestadual.id > 0">
                            Intra.: {{ item.naturezaOperacaoRevenda.intraestadual.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoRevenda && item.naturezaOperacaoRevenda.interestadual && item.naturezaOperacaoRevenda.interestadual.id > 0">
                            Inter.: {{ item.naturezaOperacaoRevenda.interestadual.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoRevenda && item.naturezaOperacaoRevenda.intraestadualComSt && item.naturezaOperacaoRevenda.intraestadualComSt.id > 0">
                            Intra. ST: {{ item.naturezaOperacaoRevenda.intraestadualComSt.nome }}
                        </span>
                        <span class="form-group" v-if="item.naturezaOperacaoRevenda && item.naturezaOperacaoRevenda.interestadualComSt && item.naturezaOperacaoRevenda.interestadualComSt.id > 0">
                            Inter. ST: {{ item.naturezaOperacaoRevenda.interestadualComSt.nome }}
                        </span>
                    </td>
                    <td>
                        <log-alteracao tabela="RegraNaturezaOperacao" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" required></lista-selecao-lojas>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoClienteAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCliente"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td style="white-space: nowrap">
                        <div>
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoProdutoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoProduto"
                                v-bind:ordenar="false"></lista-selecao-id-valor>
                        </div>
                        <div>
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="subgrupoProdutoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSubgrupoProduto"
                                v-bind:ordenar="false" v-bind:filtro-recuperar-itens="filtroSubgrupos"></lista-selecao-id-valor>
                        </div>
                    </td>
                    <td style="white-space: nowrap">
                        <div>
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoVidro">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corVidroAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorVidro"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoFerragem">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corFerragemAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorFerragem"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoAluminio">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corAluminioAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorAluminio"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                        </div>
                        <div v-if="regraNatureza.produto">
                            <span>
                                <label>Espessura: </label>
                                <input type="number" v-model.number="regraNatureza.produto.espessura" style="width: 50px" />
                            </span>
                        </div>
                    </td>
                    <td>
                        <lista-selecao-multipla-uf v-bind:ufs.sync="ufsDestinoAtuais"></lista-selecao-multipla-uf>
                    </td>
                    <td style="white-space: nowrap">
                        <div>
                            <span>
                                <label>Intra.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoIntraestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Intra. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoIntraestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                        <div>
                            <span>
                                <label>Inter.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoInterestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Inter. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoInterestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                    </td>
                    <td style="white-space: nowrap">
                        <div>
                            <span>
                                <label>Intra.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaIntraestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Intra. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaIntraestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                        <div>
                            <span>
                                <label>Inter.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaInterestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Inter. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaInterestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova regra de natureza de operação..." v-if="!inserindo">
                            <img src="../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" v-if="inserindo" required></lista-selecao-lojas>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoClienteAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCliente"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td style="white-space: nowrap">
                        <div>
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoProdutoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoProduto"
                                v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                        </div>
                        <div>
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="subgrupoProdutoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSubgrupoProduto"
                                v-bind:ordenar="false" v-bind:filtro-recuperar-itens="filtroSubgrupos" v-if="inserindo"></lista-selecao-id-valor>
                        </div>
                    </td>
                    <td style="white-space: nowrap">
                        <div v-if="inserindo">
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoVidro">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corVidroAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorVidro"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoFerragem">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corFerragemAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorFerragem"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                            <span v-if="regraNatureza && regraNatureza.produto && regraNatureza.produto.idGrupoProduto === configuracoes.idGrupoAluminio">
                                <label>Cor: </label>
                                <lista-selecao-id-valor v-bind:item-selecionado.sync="corAluminioAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorAluminio"
                                    v-bind:ordenar="false"></lista-selecao-id-valor>
                            </span>
                        </div>
                        <div v-if="inserindo && regraNatureza.produto">
                            <span>
                                <label>Espessura: </label>
                                <input type="number" v-model.number="regraNatureza.produto.espessura" style="width: 50px" />
                            </span>
                        </div>
                    </td>
                    <td>
                        <lista-selecao-multipla-uf v-bind:ufs.sync="ufsDestinoAtuais" v-if="inserindo"></lista-selecao-multipla-uf>
                    </td>
                    <td style="white-space: nowrap">
                        <div v-if="inserindo">
                            <span>
                                <label>Intra.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoIntraestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Intra. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoIntraestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                        <div v-if="inserindo">
                            <span>
                                <label>Inter.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoInterestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Inter. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoProducaoInterestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                    </td>
                    <td style="white-space: nowrap">
                        <div v-if="inserindo">
                            <span>
                                <label>Intra.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaIntraestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Intra. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaIntraestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                        <div v-if="inserindo">
                            <span>
                                <label>Inter.: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaInterestadualAtual" required></campo-busca-natureza-operacao>
                            </span>
                            <span>
                                <label>Inter. ST: </label>
                                <campo-busca-natureza-operacao v-bind:natureza-operacao.sync="naturezaOperacaoRevendaInterestadualComStAtual"></campo-busca-natureza-operacao>
                            </span>
                        </div>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cfops/NaturezasOperacao/RegrasNaturezaOperacao/Componentes/LstRegrasNaturezaOperacao.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cfops/NaturezasOperacao/RegrasNaturezaOperacao/Componentes/LstRegrasNaturezaOperacao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
