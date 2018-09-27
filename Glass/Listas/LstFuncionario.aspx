<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstFuncionario" Title="Funcionários" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Funcionarios/Templates/LstFuncionarios.Filtro.html")
    %>
    <div id="app">
        <funcionario-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></funcionario-filtros>
        <section>
            <a :href="obterLinkInserirFuncionario()" v-if="configuracoes && configuracoes.alterarFuncionario">
                Inserir Funcionário
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="atualizarFuncionarios" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum funcionário encontrado">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoFunc')">Tipo Func.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfFunc')">CPF</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rgFunc')">RG</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telRes')">Tel Res</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telCel')">Cel</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space:nowrap">
                    <a :href="obterLinkEditarFuncionario(item)" title="Editar" v-if="configuracoes && configuracoes.alterarFuncionario && item.permissoes.editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir" v-if="configuracoes && configuracoes.alterarFuncionario && item.permissoes.apagar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.tipoFuncionario }}</td>
                    <td>{{ item.documentos.cpf }}</td>
                    <td>{{ item.documentos.rg }}</td>
                    <td>{{ item.contatos.telefoneResidencial }}</td>
                    <td>{{ item.contatos.telefoneCelular }}</td>
                    <td style="white-space:nowrap">
                    <controle-exibicao-imagem :id-item="item.id" tipo-item="Funcionario"></controle-exibicao-imagem>
                        <log-alteracao tabela="Funcionario" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio()" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
            </div>
        </section>
    </div>
        <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Funcionarios/Componentes/LstFuncionarios.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Funcionarios/Componentes/LstFuncionarios.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
