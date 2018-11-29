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
                    <input type="text" v-model="funcionario.nome" required/>
                </span>
                <span class="cabecalho">
                    <label>
                        RG
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.rg"/>
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
                        CPF
                    </label>
                </span>
                <span>
                    <campo-cpf v-bind:cpf.sync="funcionario.documentosEDadosPessoais.cpf" required></campo-cpf>
                </span>
                <span class="cabecalho" v-if="configuracoes && configuracoes.idsTiposFuncionariosComSetor && configuracoes.idsTiposFuncionariosComSetor.indexOf(tipoFuncionarioAtual.id) > -1">
                    <label for="tipo">
                        Setores
                    </label>
                </span>
                <span v-if="configuracoes && configuracoes.idsTiposFuncionariosComSetor && configuracoes.idsTiposFuncionariosComSetor.indexOf(tipoFuncionarioAtual.id) > -1">
                    <lista-selecao-multipla v-bind:ids-selecionados.sync="funcionario.idsSetores"
                    v-bind:funcao-recuperar-itens="obterItensSetor" v-bind:ordenar="false"></lista-selecao-multipla>
                </span>
                <span class="cabecalho" v-if="configuracoes && configuracoes.idsTiposFuncionariosComSetor && configuracoes.idsTiposFuncionariosComSetor.indexOf(tipoFuncionarioAtual.id) > -1">
                </span>
                <span v-if="configuracoes && configuracoes.idsTiposFuncionariosComSetor && configuracoes.idsTiposFuncionariosComSetor.indexOf(tipoFuncionarioAtual.id) > -1">
                </span>
                <span class="cabecalho">
                    <label>
                        Data Nasc.
                    </label>
                </span>
                <span>
                    <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataNascimento" required></campo-data-hora>
                </span>
                <span class="cabecalho">
                    <label>
                        Função
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.funcao"/>
                </span>
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
                        Est. Civil
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="estadoCivilAtual" :funcao-recuperar-itens="obterEstadosCivil"></lista-selecao-id-valor>
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
                        Complemento
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.endereco.complemento"/>
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
                        CEP
                    </label>
                </span>
                    <span>
                        <campo-cep v-bind:endereco.sync="funcionario.endereco" v-bind="$attrs"></campo-cep>
                    </span>
                <span class="cabecalho">
                    <label>
                        UF
                    </label>
                </span>
                <span>
                     <lista-selecao-uf :uf-Selecionada.sync="funcionario.endereco.cidade.uf" ></lista-selecao-uf>
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
                    <campo-telefone v-bind:telefone.sync="funcionario.contatos.telefoneCelular"></campo-telefone>
                </span>
                <span class="cabecalho">
                    <label>
                        Tel. Res.
                    </label>
                </span>
                <span>
                    <campo-telefone v-bind:telefone.sync="funcionario.contatos.telefoneResidencial" required></campo-telefone>
                </span>
                <span class="cabecalho">
                    <label>
                        Data Entrada
                    </label>
                </span>
                <span>
                    <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataEntrada" required></campo-data-hora>
                </span>
                <span class="cabecalho">
                    <label>
                        Tel. Cont.
                    </label>
                </span>
                <span>
                    <campo-telefone v-bind:telefone.sync="funcionario.contatos.telefoneContato"></campo-data-hora>
                </span>
                <span class="cabecalho">
                    <label>
                        Email
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.contatos.email"/>
                </span>
                <span class="cabecalho">
                    <label>
                        Data Saída
                    </label>
                </span>
                <span>
                    <campo-data-hora :data-hora.sync="funcionario.documentosEDadosPessoais.dataSaida"></campo-data-hora>
                </span>
                <span class="cabecalho">
                    <label>
                        Ramal
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.contatos.ramal"/>
                </span>
                <span class="cabecalho">
                    <label>
                        Salário
                    </label>
                </span>
                    <span>
                        <input type="number" step="0.01" min="0" v-model.number="funcionario.documentosEDadosPessoais.salario"/>
                    </span>
                <span class="cabecalho">
                    <label>
                        Situação
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual" :funcao-recuperar-itens="obterSituacoes" :incluir-item-vazio="false"></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                    <label>
                        Gratificação
                    </label>
                </span>
                    <span>
                        <input type="number" step="0.01" min="0" v-model.number="funcionario.documentosEDadosPessoais.gratificacao" />
                    </span>
                <span class="cabecalho">
                    <label>
                        Num. Carteira Trabalho
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.numeroCTPS"/>
                </span>
                <span class="cabecalho">
                    <label>
                        Aux. Alimentação
                    </label>
                </span>
                <span>
                    <input type="number" step="0.01" min="0" v-model.number="funcionario.documentosEDadosPessoais.auxilioAlimentacao" />
                </span>
                <span class="cabecalho">
                    <label>
                        Login
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.acesso.login"/>
                </span>
                <span class="cabecalho">
                    <label>
                        Núm. Pis
                    </label>
                </span>
                <span>
                    <input type="text" v-model="funcionario.documentosEDadosPessoais.numeroPis"/>
                </span>
                <span class="cabecalho" v-if="inserindo">
                    <label>
                        Senha
                    </label>
                </span>
                <span v-if="inserindo">
                    <input type="text" v-model="funcionario.acesso.senha"/>
                </span>
                <span class="cabecalho" v-if="inserindo">
                </span>
                <span v-if="inserindo">
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
                    <label for="tipo">
                        Tipo de Pedido
                    </label>
                </span>
                <span class="form-group">
                    <lista-selecao-multipla v-bind:ids-selecionados.sync="funcionario.idsTiposPedidos"
                v-bind:funcao-recuperar-itens="obterItensTipoPedido" v-bind:ordenar="false"></lista-selecao-multipla>
                </span>
                <span class="cabecalho">
                <label>
                    Núm. Dias Atrasar Pedido
                </label>
                </span>
                <span>
                    <input type="number" min="0" v-model="funcionario.numeroDiasParaAtrasarPedidos"/>
                </span>
                <span class="cabecalho">
                    <label>
                        Foto Funcionário
                    </label>
                </span>
                <span>
                    <campo-upload @arquivo-selecionado="fotoSelecionada" tipo-arquivo="image/*">
                </span>
                <span class="cabecalho">
                </span>
                <span>
                </span>
                <span class="cabecalho">
                </span>
                <span>
                        <controle-exibicao-imagem :id-item="funcionario.id" tipo-item="Funcionario"></controle-exibicao-imagem>
                </span>
                <span class="cabecalho">
                    <label>
                        Numero do PDV
                    </label>
                </span>
                <span>
                    <input type="number" min="0"   v-model="funcionario.numeroPdv" />
                </span>
                <span class="cabecalho" v-if="configuracoes && configuracoes.enviarEmailPedidoConfirmado">
                    <label>
                        Enviar Email Pedido Confirmado Vendedor
                    </label>
                </span>
                <span v-if="configuracoes && configuracoes.enviarEmailPedidoConfirmado">
                     <input type="checkbox" id="enviarEmailPedidoConfirmado" v-model="funcionario.permissoes.enviarEmailPedidoConfirmadoVendedor" />
                </span>
                <span class="cabecalho" v-if="configuracoes && configuracoes.habilitarChat">
                    <label>
                        Habilitar Chat WebGlass
                    </label>
                </span>
                <span v-if="configuracoes && configuracoes.habilitarChat">
                     <input type="checkbox" id="utilizarChat" v-model="funcionario.permissoes.utilizarChat" />
                </span>
                <span class="cabecalho" v-if="configuracoes && configuracoes.habilitarControleUsuarios">
                    <label>
                        Exibir controle de usuários
                    </label>
                </span>
                <span>
                    <span v-if="configuracoes && configuracoes.habilitarControleUsuarios">
                        <input type="checkbox" id="habilitarControleUsuarios" v-model="funcionario.permissoes.habilitarControleUsuarios" />
                    </span>
                </span>
                <span v-if="configuracoes && configuracoes.enviarEmailPedidoConfirmado">
                </span>
                <span>
                </span>
                <span>
                </span>
                <span>
                </span>
                <span class="cabecalho">
                    <label>
                        Obs
                    </label>
                </span>
                <span class="colspan3">
                    <textarea cols="3" style="margin: 2px 0px; width: 540px; height: 50px;" v-model="funcionario.observacao"></textarea>
                </span>
        </div>
            </section>
        <span class="botoes">
            <span>
            <button @click.prevent="inserirFuncionario" v-if="inserindo">
                    Inserir
            </button>
            <button @click.prevent="atualizarFuncionario" v-if="editando">
                Atualizar
            </button>
            <button @click.prevent="alterarSenha" v-if="editando">
                Alterar Senha
            </button>
            <button @click.prevent="cancelar">
                Cancelar
            </button>
            </span>
        </span>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Funcionarios/Componentes/CadFuncionario.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
