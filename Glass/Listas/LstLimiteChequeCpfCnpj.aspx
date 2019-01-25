<%@ Page Title="Limite de Cheques por CPF/CNPJ" Language="C#" MasterPageFile="~/Painel.master" 
    AutoEventWireup="true" CodeBehind="LstLimiteChequeCpfCnpj.aspx.cs" Inherits="Glass.UI.Web.Listas.LstLimiteChequeCpfCnpj"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cheques/LimitePorCpfCnpj/Templates/LstLimiteChequesCpfCnpj.Filtro.html")
    %>

    <div id="app">
        <limite-cheques-filtros :filtro.sync="filtro"></limite-cheques-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" :numero-registros="30" :linha-editando="numeroLinhaEdicao"
                mensagem-lista-vazia="Ainda não há CPF/CNPJ cadastrados nos cheques. Ao cadastrar, serão gerados automaticamente registros para controle do limite.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpf/cnpj')">
                            CPF/CNPJ
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('limite')">
                            Limite Configurado
                        </a>
                    </th>
                    <th>
                        Valor Utilizado do Limite
                    </th>
                    <th>
                        Valor Restante do Limite
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">
                            Obs
                        </a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <span v-if="numeroLinhaEdicao === -1">
                            <a href="#" @click.prevent="editar(item, index)">
                                <img border="0" src="../Images/EditarGrid.gif">
                            </a>
                        </span>
                    </td>
                    <td>
                        {{ item.cpfCnpj }}
                    </td>
                    <td>
                        {{ item.limite.total | moeda }}
                    </td>
                    <td>
                        {{ item.limite.utilizado | moeda }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.limite.restante | moeda }}
                    </td>
                    <td>
                        {{ item.observacao }}
                    </td>
                    <td>
                        <log-alteracao tabela="LimiteChequeCpfCnpj" :id-item="item.id" :atualizar-ao-alterar="false" 
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td>
                        <span style="white-space: nowrap">
                            <a href="#" @click.prevent="atualizar" title="Atualizar">
                                <img border="0" src="../Images/ok.gif">
                            </a>
                            <a href="#" @click.prevent="cancelar" title="Cancelar edição">
                                <img border="0" src="../Images/ExcluirGrid.gif">
                            </a>
                        </span>
                    </td>
                    <td>
                        {{ limiteChequeAtual.cpfCnpj }}
                    </td>
                    <td>
                        <template v-if="limiteCheque.limite != null">
                            <input type="number" min="0" step="any" v-model.number="limiteCheque.limite" />
                        </template>
                    </td>
                    <td>
                        <template v-if="limiteChequeAtual.limite">
                            {{ limiteChequeAtual.limite.utilizado | moeda }}
                        </template>
                    </td>
                    <td :style="{ color: limiteChequeAtual.corLinha }">
                        <template v-if="limiteChequeAtual.limite">
                            {{ limiteChequeAtual.limite.restante | moeda }}
                        </template>
                    </td>
                    <td>
                        <input type="text" v-model="limiteCheque.observacao" maxlength="300" size="50" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cheques/LimitePorCpfCnpj/Componentes/LstLimiteChequesCpfCnpj.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cheques/LimitePorCpfCnpj/Componentes/LstLimiteChequesCpfCnpj.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

