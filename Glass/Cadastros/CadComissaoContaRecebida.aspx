<%@ Page Title="Gerar Comissão de Contas Recebidas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadComissaoContaRecebida.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadComissaoContaRecebida" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script src='<%= ResolveUrl("~/Scripts/grid.js") %>' type="text/javascript"></script>
    <script src='<%= ResolveUrl("~/Scripts/jquery/jquery-ui-1.10.2.js") %>' type="text/javascript"></script>

    <script type="text/javascript">

        function buscarContas() {

            bloquearPagina();

            //Limpa a tabela
            $("#tbContas tr").remove();
            FindControl("hdfIdsContas", "input").value = "";

            var idFunc = FindControl("drpNome", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var tipoConta = FindControl("drpTipoConta", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataRecIni = FindControl("ctrlDataRecIni_txtData", "input").value;
            var dataRecFim = FindControl("ctrlDataRecFim_txtData", "input").value;

            var contas = CadComissaoContaRecebida.GetContas(idFunc, idLoja, tipoConta, dataIni, dataFim, dataRecIni, dataRecFim).value.split("\n");

            if (contas == null || contas.length == 0 || (contas.length == 1 && contas[0] == "")) {
                document.getElementById("comando").style.display = "none";
                desbloquearPagina(true);
                alert("Não há contas recebidas disponíveis para gerar comissão.");
                return false;
            }

            for (j = 0; j < contas.length; j++) {
                if (contas[j] == "")
                    continue;

                var dadosConta = contas[j].split("\t");

                addConta(dadosConta[0], dadosConta[1], dadosConta[2], dadosConta[3], dadosConta[4],
                    dadosConta[5], dadosConta[6], dadosConta[7], dadosConta[8], dadosConta[9],
                    dadosConta[10], dadosConta[11]);
            }

            desbloquearPagina(true);
        }

        function calculaTotais() {

            var totalRec = 0, totalImp = 0, totaBaseCalc = 0, TotalComissao = 0;

            $('#tbContas').find('tr').each(function (indice) {
                
                var cell = $(this).find('td');

                if (cell.length == 0 || $(this).css("display") == "none")
                    return;

                totalRec += parseFloat(cell[6].innerText.substring(3).replace(".", "").replace(",", "."));
                totalImp += parseFloat(cell[9].innerText.substring(3).replace(".", "").replace(",", "."));
                totaBaseCalc += parseFloat(cell[10].innerText.substring(3).replace(".", "").replace(",", "."));
                TotalComissao += parseFloat(cell[11].innerText.substring(3).replace(".", "").replace(",", "."));

            });

            document.getElementById("lblTotalRec").innerHTML = "R$ " + totalRec.toFixed(2).toString().replace(".", ",");
            document.getElementById("lblTotalImp").innerHTML = "R$ " + totalImp.toFixed(2).toString().replace(".", ",");
            document.getElementById("lblTotalBaseCalc").innerHTML = "R$ " + totaBaseCalc.toFixed(2).toString().replace(".", ",");
            document.getElementById("lblTotalComissao").innerHTML = "R$ " + TotalComissao.toFixed(2).toString().replace(".", ",");
        }

        function getIdsContasRecebidas() {
            var ids = FindControl("hdfIdsContas", "input").value.split(',');
            if (ids.length > 0 && ids[ids.length - 1] == "")
                ids.pop();

            return ids;
        }

        function addConta(idConta, nome, valor, dataVec, valorRec, dataRec, tipo, valorImp, baseCalc, valorComissao,
            referencia, numParcelas) {
            var ids = getIdsContasRecebidas();
            for (k = 0; k < ids.length; k++)
                if (ids[k] == idConta)
                    return;

            var titulos = new Array("Referência", "Parc.", "Cliente", "Valor", "Data Venc.", "Valor Rec.", "Data Rec.", "Tipo", "Valor Imp.", "Base de Calc.", "Valor da Comissão");
            var itens = new Array(referencia, numParcelas, nome, valor, dataVec, valorRec, dataRec, tipo, valorImp, baseCalc, valorComissao);

            addItem(itens, titulos, "tbContas", idConta, "hdfIdsContas", null, null, "permiteGerar", false);
            permiteGerar();
        }

        function permiteGerar() {

            var qtdeContas = getIdsContasRecebidas().length;

            document.getElementById("comando").style.display = qtdeContas > 0 ? "" : "none";

            if (qtdeContas > 0)
                calculaTotais();
        }

        function limpar() {
            window.location.href = window.location.href;
        }

        function gerarComissao() {

            bloquearPagina();
            

            var idsContas = FindControl("hdfIdsContas", "input").value;
            var idFunc = FindControl("drpNome", "select").value;
            var dataContaPg = FindControl("ctrlDataComissao_txtData", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataRecIni = FindControl("ctrlDataRecIni_txtData", "input").value;
            var dataRecFim = FindControl("ctrlDataRecFim_txtData", "input").value;

            var ret = CadComissaoContaRecebida.GerarComissao(idsContas, idFunc, dataContaPg, dataIni, dataFim, dataRecIni, dataRecFim);

            if (ret.error != null) {
                desbloquearPagina(true);
                alert(ret.error.description);
                return false;
            }

            desbloquearPagina(true);
            alert("Comissão gerada com sucesso");
            limpar();
        }

    </script>


    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc">
                            </asp:DropDownList>

                        </td>

                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período Cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" OnChange="dataChange();" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>

                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Período Recebimento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataRecIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" OnChange="dataChange();" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataRecFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label31" runat="server" Text="Tipo Conta:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpTipoConta" DataSourceID="odsTiposContas"
                                DataValueField="Id" DataTextField="Descr" Title="Selecione o tipo de conta">
                            </sync:CheckBoxListDropDown>
                        </td>

                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="true" />
                        </td>

                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar contas" OnClientClick="buscarContas(); return false" /></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <table id="tbContas">
                            </table>
                        </td>
                    </tr>
                </table>
                <table id="comando" style="display: none;">
                    <tr align="center">
                        <td>
                            <table cellspacing="5">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Recebido: " Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalRec" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Impostos: " Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalImp" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Base de Calculo: " Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalBaseCalc" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="Comissão: " Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalComissao" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>&nbsp;
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>Data da conta a pagar
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataComissao" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                            ValidateEmptyText="true" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <input type="button" onclick="return gerarComissao();" value="Gerar Comissão" />
                                        <input type="button" onclick="return limpar();" value="Limpar" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:HiddenField ID="hdfIdsContas" runat="server" />

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="GetVendedorForComissaoContasReceber" TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsTiposContas" runat="server" TypeName="Glass.Data.DAL.ContasReceberDAO"
        SelectMethod="ObtemTiposContas">
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">
        permiteGerar();
    </script>


</asp:Content>
