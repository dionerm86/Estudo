<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstVeiculos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstVeiculos" Title="Veículos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <a :href="obterLinkInserirVeiculo()" v-if="configuracoes && configuracoes.cadastrarVeiculo">
                Inserir veículo
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum veículo encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('placa')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('modelo')">Modelo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('anoFabricacao')">Ano fabricação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cor')">Cor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('quilometragemInicial')">KM inicial</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorIpva')">Valor IPVA</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarVeiculo(item)" v-if="configuracoes && configuracoes.cadastrarVeiculo">
                            <img src="../Images/Edit.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="configuracoes && configuracoes.cadastrarVeiculo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.placa }}</td>
                    <td>{{ item.modelo }}</td>
                    <td>{{ item.anoFabricacao }}</td>
                    <td>{{ item.cor }}</td>
                    <td>{{ item.quilometragemInicial }}</td>
                    <td>{{ item.valorIpva | moeda }}</td>
                    <td>{{ item.situacao }}</td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Veiculos/Componentes/LstVeiculos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
