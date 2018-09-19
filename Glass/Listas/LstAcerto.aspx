<%@ Page Title="Consulta de Acertos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstAcerto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAcerto" EnableViewState="false" EnableViewStateMac="false"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Acertos/Templates/LstAcertos.Filtro.html")
    %>
    <div id="app">
        <cheques-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></cheques-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum acerto encontrado." :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Acerto</a>
                    </th>
                    <th>
                        Referência
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situacao</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirCancelamento(item)" title="¨Cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoAcerto(item)" title="¨Visualizar dados do Acerto">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoNotasPromissorias(item)" title="¨Nota promissória">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="¨Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.referencia }}</td>
                    <td>
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                    </td>
                    <td>{{ item.funcionario }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.observacao }}</td>
                    <td style="white-space: nowrap">
                        <log-cancelamento tabela="Acerto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div style="color: blue">
                Os acertos em azul foram renegociados.
            </div>
            <div style="color: gold">
                Os acertos em amarelo foram enviados para Jurídico/Cartório.
            </div>
            <div>
                Os acertos enviados para Jurídico/Cartório que também foram renegociados permaneceram em azul, para diferenciar utilize o filtro de Jurídico/Cartório.
            </div>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertos(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertos(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Acertos/Componentes/LstAcertos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Acertos/Componentes/LstAcertos.js" />
        </Scripts>
    </asp:ScriptManager>
    
    
    
    
    
    
    
    
    
    
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Num. Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNotaFiscal" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrldata ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" 
                                ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrldata ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" 
                                ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto" AutoPostBack="True" OnSelectedIndexChanged="drpFormaPagto_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label37" runat="server" Text="Jurídico/Cartório" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProtestadas" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Incluir contas em jurídico/cartório</asp:ListItem>
                                <asp:ListItem Value="1">Somente contas em jurídico/cartório</asp:ListItem>
                                <asp:ListItem Value="2">Não incluir contas em jurídico/cartório</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAcerto" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdAcerto" DataSourceID="odsAcerto"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum acerto encontrado com o filtro aplicado."
                    OnPreRender="grdAcerto_PreRender" OnRowDataBound="grdAcerto_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdAcerto" HeaderText="Acerto" SortExpression="IdAcerto" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="IdPedido, IdLiberarPedido" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="Funcionario" HeaderText="Funcionário" SortExpression="Funcionario" />
                        <asp:BoundField DataField="TotalAcerto" DataFormatString="{0:C}" HeaderText="Total" SortExpression="TotalAcerto" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdAcerto") %>'
                                    Tabela="Acerto" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <div style="color: blue; text-align: center">
                    Os acertos em azul foram renegociados.
                </div>
                <div style="color: gold; text-align: center">
                    Os acertos em amarelo foram enviados para Jurídico/Cartório.
                </div>
                <div style="text-align: center">
                    Os acertos enviados para Jurídico/Cartório que também foram renegociados permaneceram em azul, para diferenciar utilize o filtro de Jurídico/Cartório.
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAcerto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetByCliListCount" SelectMethod="GetByCliList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AcertoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumAcerto" Name="numAcerto" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoBoleto" Name="idTipoBoleto" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumNotaFiscal" Name="numNotaFiscal" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpProtestadas" Name="protestadas" PropertyName="SelectedValue"/>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaConta"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
