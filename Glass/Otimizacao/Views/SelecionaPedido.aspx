<%@ Page Title="Otimização - Selecione os Pedidos/Orçamentos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="SelecionaPedido.aspx.cs" Inherits="Glass.UI.Web.Otimizacao.Views.SelecionaPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">  
    <table>
        <tr>
            <td align="center">
                <table cellpadding="3">
                    <tr>
                        <td id="buscar2">
                            <table style="margin: -3px 0px">
                                <tr>
                                    <td>
                                        Pedido:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumero" runat="server" Width="60px" onkeydown="if (isEnter(event)) return obterPedido(this);"
                                            onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="obterPedido(this); return false;" Width="16px" />
                                    </td>
                                    
                                </tr>
                            </table>
                        </td>
                        <td id="buscar3">
                            <a href="#" id="lnkBuscarPedido" onclick="openWindow(500, 700,'../../Utils/SelPedido.aspx?multiSelect=1&tipo=6'); return false;" 
                            style="font-size: small;">Buscar Pedidos</a>
                        </td>
                          <td id="Td1">
                            <table style="margin: -3px 0px">
                                <tr>
                                    <td>
                                        Orçamento:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroOrcamento" runat="server" Width="60px" onkeydown="if (isEnter(event)) return obterOrcamento(this);"
                                            onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgAddOrca" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="obterOrcamento(this); return false;" Width="16px" />
                                    </td>
                                    
                                </tr>
                            </table>
                        </td>
                        <td id="Td2">
                            <a href="#" id="A1" onclick="openWindow(500, 700,'../../Utils/SelOrcamento.aspx?multiSelect=1'); return false;" 
                            style="font-size: small;">Buscar Orçamentos</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <input type="button" value="Finalizar" onclick="finalizar()" id="btnFinalizar" style="display:block" />
                <table id="lstProd" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
         <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdPedido" runat="server" />
                <asp:HiddenField ID="hdfIdOrcamento" runat="server" />
            </td>
        </tr>
    </table>   
    
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-2.0.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        
        function obterPedido(text) {
            var item = FindControl("txtNumero", "input").value;
            setPedido(item);
            text.value = "";
        }
        
        function setPedido(item) {

            var items = FindControl("hdfIdPedido", "input").value.split(',');

            for (i = 0; i < items.length; i++) {
                if (item == items[i]) {
                    alert("Pedido " + items[i] + " já incluído.");

                    return false;
                }
            }

            var response = SelecionaPedido.ObterDadosPedido(item);

            if (response == null) {
                alert("Falha ao buscar Pedido. AJAX Error.");
                return false;
            }

            response = response.value.split('\t');

            if (response[0] == "Erro") {
                alert(response[1]);
                return false;
            }

            var pedido = response[1].split(';');

            var dados = new Array(pedido[0], pedido[1], pedido[2], pedido[3], pedido[4], pedido[5], pedido[6], pedido[7], pedido[8])
            var titulo = new Array("Núm.", "Cliente", "Vendedor", "Tipo Venda", "Total", "Total M²", "Total Peças", "Data", "Situação")

            addItem(dados, titulo, 'lstProd', dados[0], 'hdfIdPedido');
            
            //Altera o caminho da imagem
            var caminhoImagem = "../../Images/ExcluirGrid.gif";
            $("#lstProd").find("img").attr("src", caminhoImagem);
        }

        function finalizar() {

            var mensagem = "";
            var valido = true;
            
            var idPedido = FindControl("hdfIdPedido", "input").value;

            if (idPedido == "" || idPedido == undefined) {
                mensagem += "Selecione um pedido.\r";
            }

            var idOrcamento = FindControl("hdfIdOrcamento", "input").value;

            if (idOrcamento == "" || idOrcamento == undefined) {
                 mensagem += "Selecione um orçamento.";
             }

             if ((idPedido == "" || idPedido == undefined) && (idOrcamento == "" || idOrcamento == undefined)) {
                 valido = false;
             }

             if (!valido) {
                 alert(mensagem);
                 return false;
             }

            redirectUrl("DadosOtimizacao.aspx?pedidos=" + idPedido.substring(0, idPedido.lastIndexOf(',')) + "&orcamentos=" + idOrcamento.substring(0, idOrcamento.lastIndexOf(',')));
        }

        function obterOrcamento(text) {
            var item = FindControl("txtNumeroOrcamento", "input").value;
            setOrcamento(item);
            text.value = "";
        }

        function setOrcamento(item) {

            var items = FindControl("hdfIdOrcamento", "input").value.split(',');

            for (i = 0; i < items.length; i++) {
                if (item == items[i]) {
                    alert("Orçamento " + items[i] + " já incluído.");

                    return false;
                }
            }

            var response = SelecionaPedido.ObterDadosOrcamento(item);

            if (response == null) {
                alert("Falha ao buscar Orçamento. AJAX Error.");
                return false;
            }

            response = response.value.split('\t');

            if (response[0] == "Erro") {
                alert(response[1]);
                return false;
            }

            var orcamento = response[1].split(';');

            var dados = new Array(orcamento[0], orcamento[1], orcamento[2], orcamento[3], orcamento[4], orcamento[5], orcamento[6], orcamento[7], orcamento[8])
            var titulo = new Array("Núm.", "Cliente", "Vendedor", "Tipo Venda", "Total", "Total M²", "Total Peças", "Data", "Situação")

            addItem(dados, titulo, 'lstProd', dados[0], 'hdfIdOrcamento');

            //Altera o caminho da imagem
            var caminhoImagem = "../../Images/ExcluirGrid.gif";
            $("#lstProd").find("img").attr("src", caminhoImagem);
        }
    </script>
</asp:Content>

