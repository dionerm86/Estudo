<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadFuncionario" Title="Cadastro de Funcion�rio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <div v-if="editando || inserindo">
            <section class="edicao">
            <span class="cabecalho">
                    <label>
                        Nome
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.nome" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Tipo Funcion�rio
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoFuncionarioAtual" :funcao-recuperar-itens="obterTiposFuncionario" required></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                    <label>
                        Fun��o
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.funcao" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Data Nasc.
                    </label>
                </span>
                <campo-data-hora :data-hora.sync="funcionario.documentacaoEDadosPessoais.dataNascimento" required></campo-data-hora>
                <span class="cabecalho">
                    <label>
                        Endere�o
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.endereco.logradouro" required/>
            </span>
                <span class="cabecalho">
                    <label>
                        Bairro
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.endereco.bairro" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Cidade
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.endereco.cidade.nome" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Loja
                    </label>
                </span>
                <span>
                    <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" :exibir-todas="false" required class="colspan2"></lista-selecao-lojas>
                </span>
                <span class="cabecalho">
                    <label>
                        Tel. Cel.
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.telefoneCelular" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Data Nasc.
                    </label>
                </span>
                <campo-data-hora :data-hora.sync="funcionario.documentacaoEDadosPessoais.dataNascimento" required></campo-data-hora>
                <span class="cabecalho">
                    <label>
                        Situa��o
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual" :funcao-recuperar-itens="obterSituacoes" required></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                        <label>
                            Gratifica��o
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentacaoEDadosPessoais.gratificacao" />
                    </span>
                <span class="cabecalho">
                    <label>
                        Email
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.email" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Num. Carteira Trabalho
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.numeroCarteiraTrabalho" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Login
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.acesso.senha" required v-if="inserindo" />
            </span>
                <span class="cabecalho">
                    <label>
                        Senha
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.acesso.login" required />
            </span>
                <span class="cabecalho">
                    <label>
                        N�m. Dias Atrasar Pedido
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.numeroDiasAtrasarPedido" required />
            </span>
        </div>
        <span class="cabecalho">
                    <label>
                        RG
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.rg" required />
            </span>
        <span class="cabecalho">
                    <label>
                        CPF
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.cpf" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Est. Civil
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="estadoCivil" :funcao-recuperar-itens="obterEstadosCivil" required></lista-selecao-id-valor>
                </span>
        <span class="cabecalho">
                    <label>
                        Tipo Pedido
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="registrado" v-model="funcionario.documentacaoEDadosPessoais.registrado" />
                    </span>
                </span>
        <span class="cabecalho">
                    <label>
                        Complemento
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.endereco.complemento" required />
            </span>
        <span class="cabecalho">
                        <label>
                            CEP
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.endereco.cep" />
                    </span>
        <span class="cabecalho">
                    <label>
                        UF
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="funcionario.endereco.cidade.uf" required></lista-selecao-id-valor>
                </span>
        <span class="cabecalho">
                    <label>
                        Tel. Cel.
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.telefoneResidencial" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Tel. Cel.
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.telefoneContato" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Data Sa�da
                    </label>
                </span>
                <campo-data-hora :data-hora.sync="funcionario.documentacaoEDadosPessoais.dataSaida" required></campo-data-hora>
        <span class="cabecalho">
                        <label>
                            Sal�rio
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentacaoEDadosPessoais.salario" />
                    </span>
        <span class="cabecalho">
                        <label>
                            Aux. Alimenta��o
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentacaoEDadosPessoais.auxilioAlimentacao" />
                    </span>
        <span class="cabecalho">
                    <label>
                        Ramal
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.ramal" required />
            </span>
        <span class="cabecalho">
                    <label>
                        N�m. Pis
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentacaoEDadosPessoais.numeroPis" required />
            </span>
        <span class="cabecalho">
                        <label for="tipo">
                            Tipo
                        </label>
                </span>
        <span class="form-group">
            <lista-selecao-multipla v-bind:ids-selecionados.sync="filtroAtual.tipo"
                v-bind:funcao-recuperar-itens="obterItensTipoPedido" v-bind:ordenar="false"></lista-selecao-multipla>
        </span>
        <span class="cabecalho">
                    <label>
                        Habilitar Chat WebGlass
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="habilitarChatWebglass" v-model="funcionario.documentacaoEDadosPessoais.registrado" />
                    </span>
                </span>
        <span class="cabecalho">
                    <label>
                        Exibir controle de usu�rios
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="exibirControleUsuarios" v-model="funcionario.documentacaoEDadosPessoais.registrado" />
                    </span>
                </span>
        <span class="cabecalho">
                    <label>
                        Obs
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.observacao" required />
            </span>
        <span class="botoes">
            <button @click.prevent="inserirPedido" v-if="inserindo">
                            Inserir
                        </button>
                    <span>
                        <button @click.prevent="atualizar" v-else-if="editando">
                            Atualizar
                        </button>
                    </span>
            <span>
                        <button @click.prevent="alterarSenha">
                            Alterar Senha
                        </button>
                    </span>
            <span>
                        <button @click.prevent="cancelar">
                            Cancelar
                        </button>
                    </span>
                </span>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Funcionarios/Componentes/CadFuncionario.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
