<%@ Page Title="Criar Otimização" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadOtimizacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadOtimizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function setPedido(idPedido) {

            var buscaProdResult = CadOtimizacao.BuscarProdutos(idPedido);

            if (buscaProdResult.error != null) {
                alert(buscaProdResult.error.description);
                return;
            }

            if (buscaProdResult.value == "") {
                alert('Nenhum produto com tipo calculo ML Barra foi encontrado para o pedido ' + idPedido);
                return;
            }

            var produtos = buscaProdResult.value.split('|');
            var nomeColunasArr = new Array('Pedido', 'Produto', 'ML', 'Peso', 'Grau', '', '', '');
            var pecaOtimizadaAnteriormente = "<label style='color:red; font-weight:bold;'>* Peça otimizada anteriormente</label>";

            for (var i = 0; i < produtos.length; i++) {

                var itens = produtos[i].split(';');

                var hdfIdProd = "<input type='hidden' id='hdfIdProd' value='" + itens[6] + "'/>";
                var hdfProjEsquadria = "<input type='hidden' id='hdfProjEsquadria' value='" + itens[8] + "'/>";

                var drpGrauCorte = itens[7] != "0" ? "<select id='drpGrauCorte'>" : "<select id='drpGrauCorte'><option value='0' selected></option>";
                drpGrauCorte += itens[7] == "1" ? "<option value='1' selected>L9090</option>" : "<option value='1'>L9090</option>";
                drpGrauCorte += itens[7] == "2" ? "<option value='2' selected>L9045</option>" : "<option value='2'>L9045</option>";
                drpGrauCorte += itens[7] == "5" ? "<option value='5' selected>L9090</option>" : "<option value='5'>L4545</option>";
                drpGrauCorte += itens[7] == "6" ? "<option value='6' selected>L9045</option>" : "<option value='6'>L4590</option>";
                drpGrauCorte += itens[7] == "3" ? "<option value='3' selected>H9090</option>" : "<option value='3'>H9090</option>";
                drpGrauCorte += itens[7] == "4" ? "<option value='4' selected>H9045</option>" : "<option value='4'>H9045</option>";
                drpGrauCorte += itens[7] == "7" ? "<option value='7' selected>H9090</option>" : "<option value='7'>H4545</option>";
                drpGrauCorte += itens[7] == "8" ? "<option value='8' selected>H9045</option>" : "<option value='8'>H4590</option>";
                drpGrauCorte += "</select>";

                var itensArr = new Array(itens[0], itens[2], itens[4], itens[3], drpGrauCorte, itens[5] == "true" ? pecaOtimizadaAnteriormente : '', hdfIdProd, hdfProjEsquadria);

                addItem(itensArr, nomeColunasArr, 'lstProd', itens[1], 'hdfIdProdPed', null, null, null, false);
            }

            FindControl("btnGerarOtimizacao", "input").style.display = produtos.length > 0 ? "block" : "none";
        }

        function setOrcamento(idOrcamento) {

            var buscaProdResult = CadOtimizacao.BuscarProdutosOrcamento(idOrcamento);

            if (buscaProdResult.error != null) {
                alert(buscaProdResult.error.description);
                return;
            }

            if (buscaProdResult.value == "") {
                alert('Nenhum produto com tipo calculo ML Barra foi encontrado para o orçamento ' + idOrcamento);
                return;
            }

            var produtos = buscaProdResult.value.split('|');
            var nomeColunasArr = new Array('Orçamento', 'Produto', 'ML', 'Peso', 'Grau', '', '', '');
            var pecaOtimizadaAnteriormente = "<label style='color:red; font-weight:bold;'>* Peça otimizada anteriormente</label>";

            for (var i = 0; i < produtos.length; i++) {

                var itens = produtos[i].split(';');

                var hdfIdProd = "<input type='hidden' id='hdfIdProd' value='" + itens[6] + "'/>";
                var hdfProjEsquadria = "<input type='hidden' id='hdfProjEsquadria' value='" + itens[8] + "'/>";

                var drpGrauCorte = itens[7] != "0" ? "<select id='drpGrauCorte'>" : "<select id='drpGrauCorte'><option value='0' selected></option>";
                drpGrauCorte += itens[7] == "1" ? "<option value='1' selected>L9090</option>" : "<option value='1'>L9090</option>";
                drpGrauCorte += itens[7] == "2" ? "<option value='2' selected>L9045</option>" : "<option value='2'>L9045</option>";
                drpGrauCorte += itens[7] == "5" ? "<option value='5' selected>L9090</option>" : "<option value='5'>L4545</option>";
                drpGrauCorte += itens[7] == "6" ? "<option value='6' selected>L9045</option>" : "<option value='6'>L4590</option>";
                drpGrauCorte += itens[7] == "3" ? "<option value='3' selected>H9090</option>" : "<option value='3'>H9090</option>";
                drpGrauCorte += itens[7] == "4" ? "<option value='4' selected>H9045</option>" : "<option value='4'>H9045</option>";
                drpGrauCorte += itens[7] == "7" ? "<option value='7' selected>H9090</option>" : "<option value='7'>H4545</option>";
                drpGrauCorte += itens[7] == "8" ? "<option value='8' selected>H9045</option>" : "<option value='8'>H4590</option>";
                drpGrauCorte += "</select>";

                var itensArr = new Array(itens[0], itens[2], itens[4], itens[3], drpGrauCorte, itens[5] == "true" ? pecaOtimizadaAnteriormente : '', hdfIdProd, hdfProjEsquadria);

                addItem(itensArr, nomeColunasArr, 'lstProdOrca', itens[1], 'hdfIdProdOrca', null, null, null, false);
            }

            FindControl("btnGerarOtimizacao", "input").style.display = produtos.length > 0 ? "block" : "none";
        }

        function gerarOtimizacao() {

            var table = FindControl("lstProd", "table");

            var lstProdPed = [];
            var lstProdOrca = [];
            var lstComprimento= [];
            var lstGrau = [];
            var lstIdProd = [];

            var projEsquadria;

            for (var i = 1; i < table.rows.length; i++) {

                var row = table.rows[i];

                if (row.style.display == "none")
                    continue;

                if (projEsquadria == null)
                    projEsquadria = FindControl("hdfProjEsquadria", "input", row).value == "true";

                if (projEsquadria != (FindControl("hdfProjEsquadria", "input", row).value == "true")) {
                    alert('Não é possivel otimizar projetos de esquadria e temperado juntos. As configurações são diferentes.');
                    return false;
                }

                if (FindControl("drpGrauCorte", "select", row).value == "0") {
                    alert('Selecione o grau de todas as peças.');
                    return false;
                }

                lstProdPed.push(row.getAttribute('objid'));
                lstComprimento.push(row.cells[3].innerText.replace(',', '.'));
                lstGrau.push(FindControl("drpGrauCorte", "select", row).value);
                lstIdProd.push(FindControl("hdfIdProd", "input", row).value);
            }

            table = FindControl("lstProdOrca", "table");
                        
            for (var i = 1; i < table.rows.length; i++) {
                projEsquadria = null;
                var row = table.rows[i];

                if (row.style.display == "none")
                    continue;

                if (projEsquadria == null)
                    projEsquadria = FindControl("hdfProjEsquadria", "input", row).value == "true";

                if (projEsquadria != (FindControl("hdfProjEsquadria", "input", row).value == "true")) {
                    alert('Não é possivel otimizar projetos de esquadria e temperado juntos. As configurações são diferentes.');
                    return false;
                }

                if (FindControl("drpGrauCorte", "select", row).value == "0") {
                    alert('Selecione o grau de todas as peças.');
                    return false;
                }

                lstProdOrca.push(row.getAttribute('objid'));
                lstComprimento.push(row.cells[3].innerText.replace(',', '.'));
                lstGrau.push(FindControl("drpGrauCorte", "select", row).value);
                lstIdProd.push(FindControl("hdfIdProd", "input", row).value);
            }

            lstProdPed = lstProdPed.length == 0 ? lstProdPed[0] = "0" : lstProdPed;
            lstProdOrca = lstProdOrca.length == 0 ? lstProdOrca[0] = "0": lstProdOrca;
            var gerarOtimizacaoResult = CadOtimizacao.GerarOtimizacao(lstProdPed, lstProdOrca, lstIdProd, lstComprimento, lstGrau, projEsquadria);

            if (gerarOtimizacaoResult.error != null) {
                alert(gerarOtimizacaoResult.error.description);
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=ResultadoOtimizacao&idOtimizacao=" + gerarOtimizacaoResult.value, null, true, true);

            window.location.href = "/Listas/LstOtimizacao.aspx";
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>Pedido:
                        </td>
                        <td>
                            
                            <asp:TextBox ID="txtNumero" runat="server" Width="60px" onkeydown="if (isEnter(event)) return setPedido(this.value);"
                                onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="setPedido(FindControl('txtNumero','input').value); return false;" Width="16px" />
                        </td>
                        <td>
                            <a href="#" id="lnkBuscarPedido" onclick="<%= "openWindow(500, 700,'" + ResolveClientUrl("~/Utils/SelPedido.aspx") + "?multiSelect=1&tipo=6'); return false;" %>"
                                style="font-size: small;">Buscar Pedidos</a>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table>
                    <tr>
                        <td>Orçamento:
                        </td>
                        <td>
                            <asp:TextBox ID="txtOrcamento" runat="server" Width="60px" onkeydown="if (isEnter(event)) return setOrcamento(this.value);"
                                onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="setOrcamento(FindControl('txtOrcamento','input').value); return false;" Width="16px" />
                        </td>
                        <td>
                            <a href="#" id="lnkBuscarOrcamento" onclick="<%= "openWindow(500, 700,'" + ResolveClientUrl("~/Utils/SelOrcamento.aspx") + "?multiSelect=1&tipo=6'); return false;" %>"
                                style="font-size: small;">Buscar Orcamentos</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" id="tdLstProd" runat="server">
                <table id="lstProd">

                </table>
                <asp:HiddenField runat="server" ID="hdfIdProdPed" />
            </td>
        </tr>
                <tr>
            <td align="center" id="tdLstProdOrca" runat="server">
                <table id="lstProdOrca">

                </table>
                <asp:HiddenField runat="server" ID="hdfIdProdOrca" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <input type="button" id="btnGerarOtimizacao" value="Gerar Otimização" onclick="gerarOtimizacao(); return false;" style="display:none;"/>
            </td>
        </tr>
    </table>

</asp:Content>
