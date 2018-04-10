<%@ Page Title="Transações CAPPTA TEF" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstTransacaoCapptaTef.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTransacaoCapptaTef" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function exibirTransacoes(botao, idTransacao) {

            var linha = document.getElementById("transacao_" + idTransacao);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " transações";
        }

        function removerTransacao(tipoRecebimento, id, codControle) {

            if (!confirm('Deseja realmente remover esta transação?')) {
                return false;
            }

            var administrativePassword = prompt("Digite a senha administrativa: ", "senha administrativa");

            //Busca os dados para autenticar na cappta
            var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

            if (dadosAutenticacaoCappta.error) {
                desbloquearPagina(true);
                alert(dadosAutenticacaoCappta.error.description);
                return false;
            }

            //Instancia do canal de recebimento
            CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msg, codigosAdministrativos) => callbackCappta(sucesso, msg, codigosAdministrativos, id));

            //Inicia o recebimento
            CapptaTef.efetuarEstorno(id, tipoRecebimento, administrativePassword, codControle);

            return false;
        }

        function callbackCappta(sucesso, msg, codigosAdministrativos, id) {

            desbloquearPagina(true);

            if (sucesso) {
                openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos);
            } else {
                alert('Falha ao cancelar transação: ' + msg);
            }

            window.location.reload(true);
        }

        function reimprimirRecibo(codControle) {
            openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codControle + "&reimpressao=true");
            return false;
        }

        function finalizarTransacao(tipoRecebimento, IdReferencia) {
            var result = LstTransacaoCapptaTef.FinalizarTransacaoProcessando(tipoRecebimento, IdReferencia);

            if (result.error) {
                alert(result.error.description);
                return false;
            }

            alert('Transação finalizada');
            window.location.href = window.location.href;
        }

    </script>

    <table>
        <tr>
            <td align="center"></td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTransacoesCapptaTef" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataSourceID="odsTransacaoCapptaTef" EmptyDataText="Nenhuma transação encontrada." DataKeyNames="IdTransacaoCappta">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirTransacoes(this, " + Eval("IdTransacaoCappta") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Pedidos" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgFinalizarTransacao" runat="server" ImageUrl="~/Images/rotate_context.png" ToolTip="Finalizar transação em processamento?"
                                    OnClientClick='<%# "finalizarTransacao(&#39;"+ (int)Eval("TipoRecebimento") +"&#39;,&#39;"+ Eval("IdReferencia") +"&#39;); return false" %>' 
                                 Visible='<%# Eval("ExibirFinalizarTransacao") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="IdReferencia" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="IdCliente" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="UsuCad" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:c}" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> 
                               </tr>
                                <asp:HiddenField ID="hdfTipoRecebimento" runat="server" Value='<%# Eval("TipoRecebimento") %>' />
                                <asp:HiddenField ID="hdfIdReferencia" runat="server" Value='<%# Eval("IdReferencia") %>' />
                                <tr id="transacao_<%# Eval("IdTransacaoCappta") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="7">

                                        <asp:GridView GridLines="None" ID="grdTransacoes" runat="server" AllowPaging="True"
                                            AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                            AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataSourceID="odsTransacoes" EmptyDataText="Nenhuma transação encontrada." DataKeyNames="IdTransacaoCappta">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Remover transação?"
                                                            OnClientClick='<%# "removerTransacao(&#39;"+ (int)Eval("TipoRecebimento") +"&#39;,&#39;"+ Eval("IdReferencia") +"&#39;, &#39;" + Eval("CodigoControle") + "&#39;); return false" %>'
                                                            Visible='<%# Eval("ExcluirVisible") %>' />
                                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Relatorio.gif" ToolTip="Reimprimir recibo?"
                                                            OnClientClick='<%# "return reimprimirRecibo(&#39;" + Eval("CodigoControle") + "&#39;);" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="AcquirerName" HeaderText="Adquirente" SortExpression="AcquirerName" />
                                                <asp:BoundField DataField="CardBrandName" HeaderText="Bandeira" SortExpression="CardBrandName" />
                                                <asp:BoundField DataField="PaymentProductName" HeaderText="Tipo" SortExpression="PaymentProductName" />
                                                <asp:BoundField DataField="CodigoControle" HeaderText="Cód de Controle" SortExpression="CodigoControle" />
                                                <asp:BoundField DataField="AuthorizationDateTime" HeaderText="Data" SortExpression="AuthorizationDateTime" />
                                                <asp:BoundField DataField="PaymentTransactionAmount" HeaderText="Valor" SortExpression="PaymentTransactionAmount" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="PaymentTransactionInstallments" HeaderText="Parcelas" SortExpression="PaymentTransactionInstallments" />
                                            </Columns>
                                            <PagerStyle />
                                            <EditRowStyle />
                                            <AlternatingRowStyle />
                                        </asp:GridView>

                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransacoes" runat="server"
                                            SelectMethod="GetListTransacoes" TypeName="Glass.Data.DAL.TransacaoCapptaTefDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfTipoRecebimento" PropertyName="value" Name="tipoRecebimento" />
                                                <asp:ControlParameter ControlID="hdfIdReferencia" PropertyName="value" Name="idReferencia" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransacaoCapptaTef" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.TransacaoCapptaTefDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
