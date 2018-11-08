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









    <asp:GridView ID="grdNaturezaOperacao" runat="server" SkinID="gridViewEditable"
        DataKeyNames="IdNaturezaOperacao" DataSourceID="odsNaturezaOperacao" AutoGenerateColumns="false"
        onrowcommand="grdNaturezaOperacao_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit"
                        ImageUrl="~/Images/EditarGrid.gif" CausesValidation="false" />
                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                        ImageUrl="~/Images/ExcluirGrid.gif" CausesValidation="false"
                        onclientclick="if (!confirm(&quot;Deseja remover essa natureza de operação?&quot;)) return false;" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="imgAtualizar" runat="server" OnClientClick="FindControl('txtPercReducaoBcIcms', 'input').value = FindControl('txtPercReducaoBcIcms', 'input').value.toString().replace(',', '.');FindControl('txtPercDiferimento', 'input').value = FindControl('txtPercDiferimento', 'input').value.toString().replace(',', '.');"
                        CommandName="Update" ImageUrl="~/Images/Ok.gif" style="margin-left: 0px"/>
                    <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                        CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                    <asp:HiddenField ID="hdfCodigoCfop" runat="server" 
                        Value='<%# Bind("IdCfop") %>' />
                </EditItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Código" SortExpression="CodigoInterno">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInterno") %>'
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string)  %>'></asp:Label>
                    <asp:Label ID="Label3" runat="server" Text='(Padrão)' 
                        Visible='<%# string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCodigoInterno" runat="server" 
                        Text='<%# Bind("CodInterno") %>' MaxLength="10" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>' Columns="11"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodigoInterno" runat="server" 
                        ControlToValidate="txtCodigoInterno" Display="Dynamic" ErrorMessage="*" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:RequiredFieldValidator>
                    <asp:Label ID="Label2" runat="server" Text='(Padrão)' 
                        Visible='<%# string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:Label>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCodigoInterno" runat="server" MaxLength="10" Columns="11"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodigoInterno" runat="server" 
                        ControlToValidate="txtCodigoInterno" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" Wrap="False" />
                <ItemStyle HorizontalAlign="Center" Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Mensagem" SortExpression="Mensagem">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Mensagem") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtMensagem" runat="server" Text='<%# Bind("Mensagem") %>' 
                        MaxLength="200" Width="230px"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtMensagem" runat="server" MaxLength="200" Width="230px"></asp:TextBox>
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST ICMS" SortExpression="CstIcms">
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("CstIcms") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstIcms" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIcms" DataTextField="Value" DataValueField="Key" 
                        SelectedValue='<%# Bind("CstIcms") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstIcms" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIcms" DataTextField="Value" DataValueField="Key">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Perc. Redução BC ICMS" 
                SortExpression="PercReducaoBcIcms">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPercReducaoBcIcms" runat="server" 
                        Text='<%# Bind("PercReducaoBcIcms") %>' Width="50px"
                        onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    %
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtPercReducaoBcIcms" runat="server" 
                        onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                    %
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("PercReducaoBcIcms") %>'></asp:Label>
                    %
                </ItemTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Perc. Diferimento" 
                SortExpression="PercDiferimento">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPercDiferimento" runat="server" 
                        Text='<%# Bind("PercDiferimento") %>' Width="50px"
                        onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    %
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtPercDiferimento" runat="server" 
                        onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                    %
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPercDiferimento" runat="server" Text='<%# Bind("PercDiferimento") %>'></asp:Label>
                    %
                </ItemTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CSOSN" SortExpression="Csosn">
                <ItemTemplate>
                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("Csosn") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCsosn" runat="server" 
                        SelectedValue='<%# Bind("Csosn") %>' AppendDataBoundItems="True" 
                        DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Descr">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCsosn" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Descr">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST IPI" SortExpression="CstIpi">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstIpi" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIpi" DataTextField="Translation" DataValueField="Key" 
                        SelectedValue='<%# Bind("CstIpi") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstIpi" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIpi" DataTextField="Translation" DataValueField="Key">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("CstIpi")).Format() %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cód Enq. Ipi" SortExpression="CodEnqIpi">
                <ItemTemplate>
                    <asp:Label ID="lblCodEnqIpi" runat="server" Text='<%# Eval("CodEnqIpi") %>' Width="30px" MaxLength="3" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCodEnqIpi" runat="server" Text='<%# Bind("CodEnqIpi") %>' Width="30px" MaxLength="3" />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCodEnqIpi" runat="server" Width="30px" MaxLength="3" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST Pis/Cofins" SortExpression="CstPisCofins">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstPisCofins" runat="server" 
                        AppendDataBoundItems="True" DataSourceID="odsCstPisCofins" 
                        DataTextField="Descr" DataValueField="Id" 
                        SelectedValue='<%# Bind("CstPisCofins") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstPisCofins" runat="server" 
                        AppendDataBoundItems="True" DataSourceID="odsCstPisCofins" 
                        DataTextField="Descr" DataValueField="Id">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("CstPisCofins", "{0:00}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular ICMS" SortExpression="CalcularIcms">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox7" runat="server" Checked='<%# Bind("CalcIcms") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcms" runat="server" 
                        Checked='<%# Bind("CalcIcms") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIcms" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular ICMS-ST" 
                SortExpression="CalcularIcmsSt">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("CalcIcmsSt") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsSt" runat="server" 
                        Checked='<%# Bind("CalcIcmsSt") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsSt" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular IPI" SortExpression="CalcularIpi">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("CalcIpi") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIpi" runat="server" 
                        Checked='<%# Bind("CalcIpi") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIpi" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular PIS" SortExpression="CalcularPis">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("CalcPis") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularPis" runat="server" 
                        Checked='<%# Bind("CalcPis") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularPis" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular Cofins" SortExpression="CalcularCofins">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("CalcCofins") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularCofins" runat="server" 
                        Checked='<%# Bind("CalcCofins") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularCofins" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="IPI Integra B.C. ICMS *" 
                SortExpression="IpiIntegraBaseCalculoIcms">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server"
                        Checked='<%# Bind("IpiIntegraBcIcms") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkIpiIntegraBaseCalculoIcms" runat="server"
                        Checked='<%# Bind("IpiIntegraBcIcms") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkIpiIntegraBaseCalculoIcms" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Frete Integra B.C. IPI" 
                SortExpression="FreteIntegraBaseCalculoIpi">
                <ItemTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server"
                        Checked='<%# Bind("FreteIntegraBcIpi") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server"
                        Checked='<%# Bind("FreteIntegraBcIpi") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Alterar Estoque Fiscal" 
                SortExpression="AlterarEstoqueFiscal">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" 
                        Checked='<%# Bind("AlterarEstoqueFiscal") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" 
                        Checked='<%# Bind("AlterarEstoqueFiscal") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular Difal" 
                SortExpression="CalcularDifal">
                <ItemTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" 
                        Checked='<%# Bind("CalcularDifal") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" 
                        Checked='<%# Bind("CalcularDifal") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calc. Energia Elétrica"  SortExpression="CalcEnergiaEletrica">
                <ItemTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" 
                        Checked='<%# Bind("CalcEnergiaEletrica") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" 
                        Checked='<%# Bind("CalcEnergiaEletrica") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Debitar ICMS Desonerado do Total da NF." 
                SortExpression="DebitarIcmsDesonTotalNf">
                <ItemTemplate>
                    <asp:CheckBox ID="chkDebitarIcmsDesonTotalNf" runat="server" 
                        Checked='<%# Bind("DebitarIcmsDesonTotalNf") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox8" runat="server" 
                        Checked='<%# Bind("DebitarIcmsDesonTotalNf") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="DebitarIcmsDesonTotalNf" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="NCM" 
                SortExpression="Ncm">
                <ItemTemplate>
                    <asp:Label ID="lblNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                   <asp:TextBox ID="txtNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                        IdRegistro='<%# (uint)(int)Eval("IdNaturezaOperacao") %>' Tabela="NaturezaOperacao" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/Insert.gif" 
                        onclick="imgInserir_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Label ID="Label9" runat="server" ForeColor="Red" Text='* IPI integra BC ICMS apenas para clientes do Tipo Fiscal "Consumidor Final"'></asp:Label>
    <colo:VirtualObjectDataSource ID="odsNaturezaOperacao" runat="server" 
        DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.NaturezaOperacao" 
        DeleteMethod="ApagarNaturezaOperacao" EnablePaging="True" MaximumRowsParameterName="pageSize" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarNaturezasOperacao" 
        SelectByKeysMethod="ObtemNaturezaOperacao"
        UpdateStrategy="GetAndUpdate"
        SortParameterName="sortExpression"
        TypeName="Glass.Fiscal.Negocios.ICfopFluxo" 
        UpdateMethod="SalvarNaturezaOperacao">
        <SelectParameters>
            <asp:QueryStringParameter Name="idCfop" QueryStringField="idCfop" 
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsCstIcms" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="" MaximumRowsParameterName="" SelectMethod="GetCstIcms" SkinID="" 
        StartRowIndexParameterName="" TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCstIpi" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ProdutoCstIpi, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsCstPisCofins" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        MaximumRowsParameterName="" SelectMethod="GetCstPisCofins" 
        StartRowIndexParameterName="" TypeName="Glass.Data.EFD.DataSourcesEFD">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="exibirNumeroDescricao" 
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCsosn" runat="server" SelectMethod="GetCSOSN" TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
</asp:Content>

