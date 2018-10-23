<%@ Page Title="Setores da Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSetor.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSetor" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum setor encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Sigla</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('entradaEstoque')">Entrada estoque</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('impedirAvanco')">Impedir avanço</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('informarRota')">Informar rota</a>
                    </th>
                    <th v-if="configuracoes && configuracoes.usarControleCavalete">
                        <a href="#" @click.prevent="ordenar('informarCavalete')">Informar cavalete</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('corte')">Corte</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('forno')">Forno</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('laminado')">Laminado</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirSetoresLeituraPeca')">Exibir setores lidos na produção</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirImagemCompleta')">Exibir imagem completa</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('consultarAntesDaLeitura')">Consultar peça antes da leitura</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirNaListaERelatorio')">Exibir no relatório</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('corSetor')">Cor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('corTela')">Cor Tela</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tempoLogin')">Tempo login</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tempoAlertaInatividade')">Tempo alerta inatividade (minutos)</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('desafioPerda')">Desafio perda</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('metaPerda')">Meta perda</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('capacidadeDiaria')">Capacidade diária de produção (m²)</a>
                    </th>
                    <th v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <a href="#" @click.prevent="ordenar('gerenciarFornada')">Gerenciar fornada</a>
                        <button @click.prevent="abrirTabelaFornada()" title="Etiqueta de abertura/fechamento de fornada">
                            <img src="../Images/printer.png">
                        </button>
                    </th>
                    <th v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <a href="#" @click.prevent="ordenar('alturaMaxima')">Altura</a>
                    </th>
                    <th v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <a href="#" @click.prevent="ordenar('larguraMaxima')">Largura</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('ignorarCapacidadeDiaria')">Ignorar capacidade diária de produção</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('permitirLeituraForaRoteiro')">Permitir leitura fora do roteiro</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirPainelComercial')">Exibir no painel comercial</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirPainelProducao')">Exibir no painel da produção</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1 && item.id > 1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td v-if="item.funcoes && item.funcoes.tipo">{{ item.funcoes.tipo.nome }}</td>
                    <td v-if="item.funcoes">{{ item.funcoes.entradaEstoque | indicaMarcado }}</td>
                    <td v-if="item.restricoes">{{ item.restricoes.impedirAvanco | indicaMarcado }}</td>
                    <td v-if="item.restricoes">{{ item.restricoes.informarRota | indicaMarcado }}</td>
                    <td v-if="item.restricoes && configuracoes.usarControleCavalete">{{ item.restricoes.informarCavalete | indicaMarcado }}</td>
                    <td v-if="item.funcoes">{{ item.funcoes.corte | indicaMarcado }}</td>
                    <td v-if="item.funcoes">{{ item.funcoes.forno | indicaMarcado }}</td>
                    <td v-if="item.funcoes">{{ item.funcoes.laminado | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.setoresLeituraPeca | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.imagemCompleta | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.consultarAntesDaLeitura | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.listaERelatorio | indicaMarcado }}</td>
                    <td v-if="item.cores && item.cores.setor">{{ item.cores.setor.nome }}</td>
                    <td v-if="item.cores && item.cores.tela">{{ item.cores.tela.nome }}</td>
                    <td v-if="item.tempoLogin">{{ item.tempoLogin.maximo }}min</td>
                    <td v-if="item.tempoLogin">{{ item.tempoLogin.alertaInatividade }}min</td>
                    <td v-if="item.perda">{{ item.perda.desafio }}</td>
                    <td v-if="item.perda">{{ item.perda.meta }}</td>
                    <td v-if="item.capacidade">{{ item.capacidade.diaria }}</td>
                    <td v-if="item.funcoes && configuracoes && configuracoes.usarGerenciamentoFornada">{{ item.funcoes.gerenciarFornada | indicaMarcado }}</td>
                    <td v-if="item.capacidade && configuracoes && configuracoes.usarGerenciamentoFornada">{{ item.capacidade.alturaMaxima }}</td>
                    <td v-if="item.capacidade && configuracoes && configuracoes.usarGerenciamentoFornada">{{ item.capacidade.larguraMaxima }}</td>
                    <td v-if="item.capacidade">{{ item.capacidade.ignorarCapacidadeDiaria }}</td>
                    <td v-if="item.restricoes">{{ item.restricoes.permitirLeituraForaRoteiro | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.painelComercial | indicaMarcado }}</td>
                    <td v-if="item.exibicoes">{{ item.exibicoes.painelProducao | indicaMarcado }}</td>
                    <td style="white-space: nowrap">
                        <button @click.prevent="alterarPosicao(item, true)" v-if="!inserindo && numeroLinhaEdicao === -1 && item.id > 1">
                            <img src="../Images/up.gif">
                        </button>
                        <button @click.prevent="alterarPosicao(item, false)" v-if="!inserindo && numeroLinhaEdicao === -1 && item.id > 1">
                            <img src="../Images/down.gif">
                        </button>
                        <log-alteracao tabela="Setor" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                        <input type="text" v-model="setor.codigo" maxlength="10" style="width: 50px" required />
                    </td>
                    <td>
                        <input type="text" v-model="setor.nome" maxlength="50" style="width: 120px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.entradaEstoque" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.impedirAvanco" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.informarRota" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarControleCavalete">
                        <input type="checkbox" v-model="setor.informarCavalete" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.corte" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.forno" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.laminado" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirSetoresLeituraPeca" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirImagemCompleta" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.consultarAntesDaLeitura" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirNaListaERelatorio" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="corAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCor"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="corTelaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorTela"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.tempoLogin" style="width: 50px" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.tempoAlertaInatividade" style="width: 50px" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.desafioPerda" style="width: 50px" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.metaPerda" style="width: 50px" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.capacidadeDiaria" style="width: 50px" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="checkbox" v-model="setor.gerenciarFornada" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="number" v-model.number="setor.altura" style="width: 50px" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="number" v-model.number="setor.largura" style="width: 50px" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.ignorarCapacidadeDiaria" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.permitirLeituraForaRoteiro" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirPainelComercial" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirPainelProducao" />
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo turno..." v-if="!inserindo">
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
                        <input type="text" v-model="setor.codigo" maxlength="10" style="width: 50px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="setor.nome" maxlength="50" style="width: 120px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.entradaEstoque" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.impedirAvanco" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.informarRota" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarControleCavalete">
                        <input type="checkbox" v-model="setor.informarCavalete" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.corte" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.forno" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.laminado" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirSetoresLeituraPeca" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirImagemCompleta" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.consultarAntesDaLeitura" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirNaListaERelatorio" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="corAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCor"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="corTelaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCorTela"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.tempoLogin" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.tempoAlertaInatividade" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.desafioPerda" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.metaPerda" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="setor.capacidadeDiaria" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="checkbox" v-model="setor.gerenciarFornada" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="number" v-model.number="setor.altura" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes && configuracoes.usarGerenciamentoFornada">
                        <input type="number" v-model.number="setor.largura" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.ignorarCapacidadeDiaria" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.permitirLeituraForaRoteiro" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirPainelComercial" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="setor.exibirPainelProducao" v-if="inserindo" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Setores/Componentes/LstSetores.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
