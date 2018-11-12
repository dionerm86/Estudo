<%@ Page Title="Naturezas de Operação: CFOP " Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstNaturezaOperacao" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma natureza de operação encontrada."
                :numero-registros="10" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        Código
                    </th>
                    <th>
                        Mensagem
                    </th>
                    <th>
                        CST ICMS
                    </th>
                    <th>
                        Percentual redução BC ICMS
                    </th>
                    <th>
                        Percentual diferimento
                    </th>
                    <th>
                        CSOSN
                    </th>
                    <th>
                        CST IPI
                    </th>
                    <th>
                        Código enquadramento IPI
                    </th>
                    <th>
                        CST PIS/COFINS
                    </th>
                    <th>
                        Calcular ICMS
                    </th>
                    <th>
                        Calcular ICMS ST
                    </th>
                    <th>
                        Calcular IPI
                    </th>
                    <th>
                        Calcular PIS
                    </th>
                    <th>
                        Calcular COFINS
                    </th>
                    <th>
                        IPI integra BC ICMS *
                    </th>
                    <th>
                        Frete integra BC IPI
                    </th>
                    <th>
                        Alterar estoque fiscal
                    </th>
                    <th>
                        Calcular DIFAL
                    </th>
                    <th>
                        Cálculo de energia elétrica
                    </th>
                    <th>
                        Debitar ICMS desonerado do total da NF
                    </th>
                    <th>
                        NCM
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes && item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <template v-if="item.codigo">
                            {{ item.codigo }}
                        </template>
                        <template v-else>
                            (Padrão)
                        </template>
                    </td>
                    <td>{{ item.mensagem }}</td>
                    <td v-if="item.dadosIcms && item.dadosIcms.cstIcms">{{ item.dadosIcms.cstIcms.nome }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.percentualReducaoBcIcms }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.percentualDiferimento }}</td>
                    <td v-if="item.dadosIcms && item.dadosIcms.csosn">{{ item.dadosIcms.csosn.nome }}</td>
                    <td v-if="item.dadosIpi && item.dadosIpi.cstIpi">{{ item.dadosIpi.cstIpi.nome }}</td>
                    <td v-if="item.dadosIpi">{{ item.dadosIpi.codigoEnquadramentoIpi }}</td>
                    <td v-if="item.dadosPisCofins && item.dadosPisCofins.cstPisCofins">{{ item.dadosPisCofins.cstPisCofins.nome }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.calcularIcms | indicaMarcado }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.calcularIcmsSt | indicaMarcado }}</td>
                    <td v-if="item.dadosIpi">{{ item.dadosIpi.calcularIpi | indicaMarcado }}</td>
                    <td v-if="item.dadosPisCofins">{{ item.dadosPisCofins.calcularPis | indicaMarcado }}</td>
                    <td v-if="item.dadosPisCofins">{{ item.dadosPisCofins.calcularCofins | indicaMarcado }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.ipiIntegraBcIcms | indicaMarcado }}</td>
                    <td v-if="item.dadosIpi">{{ item.dadosIpi.freteIntegraBcIpi | indicaMarcado }}</td>
                    <td>{{ item.alterarEstoqueFiscal | indicaMarcado }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.calcularDifal | indicaMarcado }}</td>
                    <td>{{ item.calculoDeEnergiaEletrica | indicaMarcado }}</td>
                    <td v-if="item.dadosIcms">{{ item.dadosIcms.debitarIcmsDesoneradoTotalNf | indicaMarcado }}</td>
                    <td>{{ item.ncm }}</td>
                    <td>
                        <log-alteracao tabela="NaturezaOperacao" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                        <template v-if="natureza.codigo">
                            <input type="text" v-model="natureza.codigo" maxlength="10" style="width: 60px" required />
                        </template>
                        <template v-else>
                            (Padrão)
                        </template>
                    </td>
                    <td>
                        <input type="text" v-model="natureza.mensagem" maxlength="200" style="width:230px" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstIcmsAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstIcms"
                            v-bind:ordenar="false" campo-id="codigo"></lista-selecao-id-valor>
                    </td>
                    <td style="whiteSpace: nowrap" v-if="natureza.dadosIcms">
                        <input type="number" v-model.number="natureza.dadosIcms.percentualReducaoBcIcms" maxlength="200" style="width:50px" />%
                    </td>
                    <td style="whiteSpace: nowrap" v-if="natureza.dadosIcms">
                        <input type="number" v-model.number="natureza.dadosIcms.percentualDiferimento" maxlength="200" style="width:50px" />%
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="csosnAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCsosn"
                            v-bind:ordenar="false" campo-id="codigo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstIpiAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstIpi"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td v-if="natureza.dadosIpi">
                        <input type="number" v-model.number="natureza.dadosIpi.codigoEnquadramentoIpi" maxlength="3" style="width:50px" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstPisCofinsAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstPisCofins"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td v-if="natureza.dadosIcms">
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularIcms" />
                    </td>
                    <td v-if="natureza.dadosIcms">
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularIcmsSt" />
                    </td>
                    <td v-if="natureza.dadosIpi">
                        <input type="checkbox" v-model="natureza.dadosIpi.calcularIpi" />
                    </td>
                    <td v-if="natureza.dadosPisCofins">
                        <input type="checkbox" v-model="natureza.dadosPisCofins.calcularPis" />
                    </td>
                    <td v-if="natureza.dadosPisCofins">
                        <input type="checkbox" v-model="natureza.dadosPisCofins.calcularCofins" />
                    </td>
                    <td v-if="natureza.dadosIcms">
                        <input type="checkbox" v-model="natureza.dadosIcms.ipiIntegraBcIcms" />
                    </td>
                    <td v-if="natureza.dadosIpi">
                        <input type="checkbox" v-model="natureza.dadosIpi.freteIntegraBcIpi" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.alterarEstoqueFiscal" />
                    </td>
                    <td v-if="natureza.dadosIcms">
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularDifal" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.calculoDeEnergiaEletrica" />
                    </td>
                    <td v-if="natureza.dadosIcms">
                        <input type="checkbox" v-model="natureza.dadosIcms.debitarIcmsDesoneradoTotalNf" />
                    </td>
                    <td>
                        <input type="text" v-model="natureza.ncm" maxlength="20" />
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova natureza de operação..." v-if="!inserindo">
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
                        <input type="text" v-model="natureza.codigo" maxlength="10" style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="text" v-model="natureza.mensagem" maxlength="200" style="width:230px" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstIcmsAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstIcms"
                            v-bind:ordenar="false" campo-id="codigo" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td style="whiteSpace: nowrap">
                        <input type="number" v-model.number="natureza.dadosIcms.percentualReducaoBcIcms" maxlength="200" style="width:50px" v-if="inserindo" />
                        <label v-if="inserindo">%</label>
                    </td>
                    <td style="whiteSpace: nowrap">
                        <input type="number" v-model.number="natureza.dadosIcms.percentualDiferimento" maxlength="200" style="width:50px" v-if="inserindo" />
                        <label v-if="inserindo">%</label>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="csosnAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCsosn"
                            v-bind:ordenar="false" campo-id="codigo" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstIpiAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstIpi"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="number" v-model.number="natureza.dadosIpi.codigoEnquadramentoIpi" maxlength="3" style="width:50px" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="cstPisCofinsAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCstPisCofins"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularIcms" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularIcmsSt" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIpi.calcularIpi" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosPisCofins.calcularPis" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosPisCofins.calcularCofins" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIcms.ipiIntegraBcIcms" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIpi.freteIntegraBcIpi" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.alterarEstoqueFiscal" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIcms.calcularDifal" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.calculoDeEnergiaEletrica" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="natureza.dadosIcms.debitarIcmsDesoneradoTotalNf" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="text" v-model="natureza.ncm" maxlength="20" v-if="inserindo" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section>
            <div>
                <span>
                    <label style="color: red">
                        * IPI integra BC ICMS apenas para clientes do Tipo Fiscal "Consumidor Final"
                    </label>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cfops/NaturezasOperacao/Componentes/LstNaturezasOperacao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>