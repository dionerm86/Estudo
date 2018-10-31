<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadFuncionario" Title="Cadastro de Funcionário" %>

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
                        Tipo Funcionário
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoFuncionarioAtual" :funcao-recuperar-itens="obterTiposFuncionario" required></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                    <label>
                        Função
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
                <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataNascimento" required></campo-data-hora>
                <span class="cabecalho">
                    <label>
                        Endereço
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
                    <input type="text" v-model="funcionario.endereco.cidade" required />
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
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.telefoneCelular" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Data Nasc.
                    </label>
                </span>
                <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataNascimento" required></campo-data-hora>
                <span class="cabecalho">
                    <label>
                        Situação
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual" :funcao-recuperar-itens="obterSituacoes" required></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                        <label>
                            Gratificação
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentosEDadosPessoais.gratificacao" />
                    </span>
                <span class="cabecalho">
                    <label>
                        Email
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.email" required />
            </span>
                <span class="cabecalho">
                    <label>
                        Num. Carteira Trabalho
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.numeroCarteiraTrabalho" required />
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
                        Núm. Dias Atrasar Pedido
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.numeroDiasAtrasarPedido" required />
            </span>
        <span class="cabecalho">
                    <label>
                        RG
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.rg" required />
            </span>
        <span class="cabecalho">
                    <label>
                        CPF
                    </label>
            </span>
            <span>
                <campo-cpf v-bind:cpf.sync="funcionario.documentosEDadosPessoais.cpf" required></campo-cpf>
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
                        Resgistrado
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="registrado" v-model="funcionario.documentosEDadosPessoais.registrado" />
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
                        <campo-cep v-bind:endereco.sync="funcionario.endereco.cep" v-bind="$attrs"></campo-cep>
                    </span>
        <span class="cabecalho">
                    <label>
                        UF
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="funcionario.id" required></lista-selecao-id-valor>
                </span>
        <span class="cabecalho">
                    <label>
                        Tel. Cel.
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.telefoneResidencial" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Tel. Cel.
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.telefoneContato" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Data Saída
                    </label>
                </span>
                <span>
                <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataSaida" required></campo-data-hora>
                    </span>
        <span class="cabecalho">
                        <label>
                            Salário
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentosEDadosPessoais.salario" />
                    </span>
        <span class="cabecalho">
                        <label>
                            Aux. Alimentação
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="funcionario.documentosEDadosPessoais.auxilioAlimentacao" />
                    </span>
        <span class="cabecalho">
                    <label>
                        Ramal
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.ramal" required />
            </span>
        <span class="cabecalho">
                    <label>
                        Núm. Pis
                    </label>
            </span>
            <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.numeroPis" required />
            </span>
        <span class="cabecalho">
                        <label for="tipo">
                            Tipo
                        </label>
                </span>
        <span class="form-group">
            <lista-selecao-multipla v-bind:ids-selecionados.sync="funcionario.idsTiposPedidos"
                v-bind:funcao-recuperar-itens="obterItensTipoPedido" v-bind:ordenar="false"></lista-selecao-multipla>
        </span>
        <span class="cabecalho">
                    <label>
                        Habilitar Chat WebGlass
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="habilitarChatWebglass" v-model="funcionario.documentosEDadosPessoais.registrado" />
                    </span>
                </span>
        <span class="cabecalho">
                    <label>
                        Exibir controle de usuários
                    </label>
                </span>
                <span>
                    <span>
                        <input type="checkbox" id="exibirControleUsuarios" v-model="funcionario.documentosEDadosPessoais.registrado" />
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
                </div>
        </section>
        <span class="botoes">
            <span>
            <button @click.prevent="inserirFuncionario" v-if="inserindo">
                    Inserir
                </button>
                <button @click.prevent="atualizar" v-else-if="editando">
                    Atualizar
                </button>

                <button @click.prevent="alterarSenha">
                    Alterar Senha
                </button>
                <button @click.prevent="cancelar">
                    Cancelar
                </button>

            </span>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Funcionarios/Componentes/CadFuncionario.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
