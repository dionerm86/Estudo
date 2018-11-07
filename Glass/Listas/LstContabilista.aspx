<%@ Page Title="Contabilista" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstContabilista.aspx.cs" 
    Inherits="Glass.UI.Web.Listas.LstContabilista" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum contabilista encontrado."
                :numero-registros="10" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('crc')">CRC</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('endereco')">Endereço</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefone')">Fone</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fax')">Fax</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('email')">Email</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.crc }}</td>
                    <td>
                        <template v-if="item.endereco">
                            {{ item.endereco.logradouro }} 
                            <template v-if="item.endereco.numero && item.endereco.numero.length">, {{ item.endereco.numero }}</template>
                            <template v-if="item.endereco.complemento && item.endereco.complemento.length"> ({{ item.endereco.complemento}}) </template>
                            {{ item.endereco.bairro }} - {{ item.endereco.cidade.nome }}/{{ item.endereco.cidade.uf }}
                        </template>
                    </td>
                    <td v-if="item.dadosContato">{{ item.dadosContato.telefone }}</td>
                    <td v-if="item.dadosContato">{{ item.dadosContato.fax }}</td>
                    <td v-if="item.dadosContato">{{ item.dadosContato.email }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
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
                        <input type="text" v-model="contabilista.nome" maxlength="50" style="width: 200px" required />
                    </td>
                    <td style="white-space: nowrap">
                        <lista-selecao-tipos-pessoa v-bind:tipo-pessoa.sync="tipoPessoaAtual" required></lista-selecao-tipos-pessoa>
                        <template v-if="contabilista.tipoPessoa == 'F'">
                            <campo-cpf v-bind:cpf.sync="contabilista.cpfCnpj" required></campo-cpf>
                        </template>
                        <template v-if="contabilista.tipoPessoa == 'J'">
                            <campo-cnpj v-bind:cnpj.sync="contabilista.cpfCnpj" required></campo-cnpj>
                        </template>
                    </td>
                    <td>
                        <input type="text" v-model="contabilista.crc" maxlength="15" style="width: 100px" />
                    </td>
                    <td>
                        <campo-endereco :endereco.sync="contabilista.endereco" :exibir-numero="true"></campo-endereco>
                    </td>
                    <td v-if="contabilista.dadosContato">
                        <campo-telefone v-bind:telefone.sync="contabilista.dadosContato.telefone"></campo-telefone>
                    </td>
                    <td v-if="contabilista.dadosContato">
                        <campo-telefone v-bind:telefone.sync="contabilista.dadosContato.fax"></campo-telefone>
                    </td>
                    <td v-if="contabilista.dadosContato">
                        <input type="text" v-model="contabilista.dadosContato.email" maxlength="60" style="width: 160px" />
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" required></lista-selecao-situacoes>
                    </td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo contabilista..." v-if="!inserindo">
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
                        <input type="text" v-model="contabilista.nome" maxlength="50" style="width: 200px" v-if="inserindo" required />
                    </td>
                    <td style="white-space: nowrap">
                        <lista-selecao-tipos-pessoa v-bind:tipo-pessoa.sync="tipoPessoaAtual" v-if="inserindo" required></lista-selecao-tipos-pessoa>
                        <template v-if="inserindo && contabilista.tipoPessoa == 'F'">
                            <campo-cpf v-bind:cpf.sync="contabilista.cpfCnpj" required></campo-cpf>
                        </template>
                        <template v-if="inserindo && contabilista.tipoPessoa == 'J'">
                            <campo-cnpj v-bind:cnpj.sync="contabilista.cpfCnpj" required></campo-cnpj>
                        </template>
                    </td>
                    <td>
                        <input type="text" v-model="contabilista.crc" maxlength="15" style="width: 100px" v-if="inserindo" />
                    </td>
                    <td>
                        <campo-endereco :endereco.sync="contabilista.endereco" :exibir-numero="true" v-if="inserindo"></campo-endereco>
                    </td>
                    <td>
                        <campo-telefone v-bind:telefone.sync="contabilista.dadosContato.telefone" v-if="inserindo && contabilista && contabilista.dadosContato"></campo-telefone>
                    </td>
                    <td>
                        <campo-telefone v-bind:telefone.sync="contabilista.dadosContato.fax" v-if="inserindo && contabilista && contabilista.dadosContato"></campo-telefone>
                    </td>
                    <td>
                        <input type="text" v-model="contabilista.dadosContato.email" maxlength="60" style="width: 160px" v-if="inserindo && contabilista && contabilista.dadosContato" />
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" v-if="inserindo && contabilista && contabilista.dadosContato" required></lista-selecao-situacoes>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Contabilistas/Componentes/LstContabilistas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

