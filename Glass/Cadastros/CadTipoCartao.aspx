<%@ Page Title="Tipo de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTipoCartao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTipoCartao" %>

<%@ Reference Control="~/Controls/ctrlLogPopup.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script>

        // -------------------------------------
        // Salva os juros de parcelas do cartão.
        // -------------------------------------
        function salvarJurosParc(botao) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idTipoCartao = FindControl("drpCartao", "select").value;
            var numParc = FindControl("txtNumParc", "input").value;
            var juros = new Array();

            var tabela = document.getElementById("<%= tblJurosParc.ClientID %>");
            for (i = 0; i < tabela.rows.length; i++) {
                var txtJuros = tabela.rows[i].cells[1].getElementsByTagName("input")[0];
                juros.push(txtJuros.value);
            }

            var resposta = CadTipoCartao.SalvarJurosParc(idLoja, idTipoCartao, numParc, juros.join(";")).value.split(';');
            alert(resposta[1]);

            if (resposta[0] == "Erro")
                alterarSalvar(botao, false);
            else
                atualizarPagina();
        }

        var planoContaControl = "";

        // ------------------------------------------------------
        // Função que salva o plano de conta selecionado no popup
        // ------------------------------------------------------
        function setPlanoConta(idConta, descricao) {
            FindControl("lbl" + planoContaControl, "span").innerHTML = descricao;
            FindControl("hdf" + planoContaControl, "input").value = idConta;

            // Habilita novamente o botão salvar da tabela de planos de conta, assim que o plano de conta for setado.
            if (FindControl("btnSalvarPlanoConta", "input") != undefined && FindControl("btnSalvarPlanoConta", "input") != null)
                FindControl("btnSalvarPlanoConta", "input").disabled = false;
        }

        // Verifica se todos os planos de conta foram informados corretamente, caso não tenham sido,
        // não permiti que o botão chame o método de salvar planos de conta.
        function verificarPlanosContaEstaoPreenchidos(controle) {
            var devolucaoPagto = FindControl("hdfDevolucaoPagto", "input");
            devolucaoPagto = devolucaoPagto != undefined && devolucaoPagto != null &&
                devolucaoPagto.value != undefined && devolucaoPagto.value != null && devolucaoPagto.value != "" ? devolucaoPagto.value : "0";

            var entrada = FindControl("hdfEntrada", "input");
            entrada = entrada != undefined && entrada != null &&
                entrada.value != undefined && entrada.value != null && entrada.value != "" ? entrada.value : "0";

            var estorno = FindControl("hdfEstorno", "input");
            estorno = estorno != undefined && estorno != null &&
                estorno.value != undefined && estorno.value != null && estorno.value != "" ? estorno.value : "0";

            var estornoChequeDev = FindControl("hdfEstornoChequeDev", "input");
            estornoChequeDev = estornoChequeDev != undefined && estornoChequeDev != null &&
                estornoChequeDev.value != undefined && estornoChequeDev.value != null && estornoChequeDev.value != "" ? estornoChequeDev.value : "0";

            var estornoDevolucaoPagto = FindControl("hdfEstornoDevolucaoPagto", "input");
            estornoDevolucaoPagto = estornoDevolucaoPagto != undefined && estornoDevolucaoPagto != null &&
                estornoDevolucaoPagto.value != undefined && estornoDevolucaoPagto.value != null && estornoDevolucaoPagto.value != "" ? estornoDevolucaoPagto.value : "0";

            var estornoEntrada = FindControl("hdfEstornoEntrada", "input");
            estornoEntrada = estornoEntrada != undefined && estornoEntrada != null &&
                estornoEntrada.value != undefined && estornoEntrada.value != null && estornoEntrada.value != "" ? estornoEntrada.value : "0";

            var estornoRecPrazo = FindControl("hdfEstornoRecPrazo", "input");
            estornoRecPrazo = estornoRecPrazo != undefined && estornoRecPrazo != null &&
                estornoRecPrazo.value != undefined && estornoRecPrazo.value != null && estornoRecPrazo.value != "" ? estornoRecPrazo.value : "0";

            var func = FindControl("hdfFunc", "input");
            func = func != undefined && func != null &&
                func.value != undefined && func.value != null && func.value != "" ? func.value : "0";

            var recChequeDev = FindControl("hdfRecChequeDev", "input");
            recChequeDev = recChequeDev != undefined && recChequeDev != null &&
                recChequeDev.value != undefined && recChequeDev.value != null && recChequeDev.value != "" ? recChequeDev.value : "0";

            var recPrazo = FindControl("hdfRecPrazo", "input");
            recPrazo = recPrazo != undefined && recPrazo != null &&
                recPrazo.value != undefined && recPrazo.value != null && recPrazo.value != "" ? recPrazo.value : "0";

            var vista = FindControl("hdfVista", "input");
            vista = vista != undefined && vista != null &&
                vista.value != undefined && vista.value != null && vista.value != "" ? vista.value : "0";

            if (devolucaoPagto == "0" || entrada == "0" || estorno == "0" ||
                estornoChequeDev == "0" || estornoDevolucaoPagto == "0" || estornoEntrada == "0" ||
                estornoRecPrazo == "0" || func == "0" || recChequeDev == "0" ||
                recPrazo == "0" || vista == "0") {
                alert('Informe todos os planos de conta.');
                controle.disabled = true;
                return false;
            }

            var todosPlanosContaInformados = devolucaoPagto + "," + entrada + "," + estorno + "," + estornoChequeDev + "," + estornoDevolucaoPagto + "," + estornoEntrada + "," +
                estornoRecPrazo + "," + func + "," + recChequeDev + "," + recPrazo + "," + vista;
            todosPlanosContaInformados = todosPlanosContaInformados.toString().split(',');

            var quantidadeOcorrenciaPlanoConta = [];
            for(var i = 0 ; i < todosPlanosContaInformados.length ; i++){
                quantidadeOcorrenciaPlanoConta[todosPlanosContaInformados[i]] = (quantidadeOcorrenciaPlanoConta[todosPlanosContaInformados[i]] || 0) + 1;
            }

            // Verifica se existem planos de conta associados a mais de um tipo.
            if (quantidadeOcorrenciaPlanoConta[devolucaoPagto] > 1 ||
                quantidadeOcorrenciaPlanoConta[entrada] > 1 ||
                quantidadeOcorrenciaPlanoConta[estorno] > 1 ||
                quantidadeOcorrenciaPlanoConta[estornoChequeDev] > 1 ||
                quantidadeOcorrenciaPlanoConta[estornoDevolucaoPagto] > 1 ||
                quantidadeOcorrenciaPlanoConta[estornoEntrada] > 1 ||
                quantidadeOcorrenciaPlanoConta[estornoRecPrazo] > 1 ||
                quantidadeOcorrenciaPlanoConta[func] > 1 ||
                quantidadeOcorrenciaPlanoConta[recChequeDev] > 1 ||
                quantidadeOcorrenciaPlanoConta[recPrazo] > 1 ||
                quantidadeOcorrenciaPlanoConta[vista] > 1) {
                alert('Não é possível informar o mesmo plano de conta em tipos diferentes.');
                controle.disabled = true;
                return false;
            }
        }

    </script>

    <div style="max-width: 800px;">
        <table>
            <tr>
                <td>Loja
                </td>
                <td>
                    <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="Name"
                        DataValueField="Id" AutoPostBack="true" OnSelectedIndexChanged="drpLoja_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Cartão
                </td>
                <td>
                    <asp:DropDownList ID="drpCartao" runat="server" DataSourceID="odsCartao"
                        DataTextField="Descricao" DataValueField="IdTipoCartao"
                        OnSelectedIndexChanged="drpTipoCartao_SelectedIndexChanged" AutoPostBack="true" OnDataBound="drpTipoCartao_DataBound"
                        Enabled="false">
                    </asp:DropDownList>

                </td>
            </tr>
            <tr>
                <td>Situação
                </td>
                <td>
                    <asp:DropDownList ID="drpSituacao" runat="server" >
                        <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                        <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btnSalvarSituacao" runat="server" Text="Salvar Situação"
                        OnClick="btnSalvarSituacao_Click"/>
                </td>
            </tr>
        </table>

        <fieldset>
            <legend>Juros de parcela de cartão</legend>
            <table>
                <tr>
                    <td>Conta bancária
                    </td>
                    <td>
                        <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                            DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="chkBloquearContaBanco" runat="server" Text="Bloquear conta bancária?" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnSalvarAssocContaBanco" runat="server" Text="Salvar"
                            OnClick="btnSalvarAssocContaBanco_Click"
                            ValidationGroup="associarCartaoConta" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <fieldset>
            <legend>Juros de parcela de cartão</legend>
            <table>
                <tr>
                    <td>&nbsp; Número de parcelas
                    </td>
                    <td>
                        <asp:TextBox ID="txtNumParc" runat="server" Width="30px" OnTextChanged="txtNumParc_TextChanged"
                            AutoPostBack="true" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center">
                        <asp:Table ID="tblJurosParc" runat="server">
                        </asp:Table>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center">
                        <asp:Button ID="btnSalvarJurosParc" runat="server" Text="Salvar"
                            OnClientClick="salvarJurosParc(this); return false"
                            ValidationGroup="jurosParcelaCartao" />
                    </td>
                </tr>
            </table>
            <asp:Label ID="Label26" runat="server" ForeColor="Red"
                Text="O percentual dos juros não é cumulativo. Apenas o percentual da parcela que corresponde ao número de parcelas geradas (i.e. o percentual da 6ª parcela em uma compra dividida em 6x) será considerado para cálculo do valor."></asp:Label>
        </fieldset>

        <fieldset id="fsPlanosConta" runat="server">
            <legend>Planos de Conta</legend>


            <table>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label12" runat="server" Text="Receb. à Vista" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblVista" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="lnkPesq" runat="server" OnClientClick="planoContaControl='Vista'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label13" runat="server" Text="Estorno de Receb. à Vista" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEstorno" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="planoContaControl='Estorno'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label1" runat="server" Text="Receb. à Prazo" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblRecPrazo" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="planoContaControl='RecPrazo'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label3" runat="server" Text="Estorno de Receb. à Prazo" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEstornoRecPrazo" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="planoContaControl='EstornoRecPrazo'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label5" runat="server" Text="Sinal" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEntrada" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="planoContaControl='Entrada'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label7" runat="server" Text="Estorno de Sinal" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEstornoEntrada" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="planoContaControl='EstornoEntrada'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label9" runat="server" Text="Receb. de Cheque Devolvido" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblRecChequeDev" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton5" runat="server" OnClientClick="planoContaControl='RecChequeDev'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label11" runat="server" Text="Estorno de Cheque Devolvido" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEstornoChequeDev" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton6" runat="server" OnClientClick="planoContaControl='EstornoChequeDev'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label15" runat="server" Text="Devolução de Pagto." Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblDevolucaoPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton7" runat="server" OnClientClick="planoContaControl='DevolucaoPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label17" runat="server" Text="Estorno de Devolução de Pagto." Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblEstornoDevolucaoPagto" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton8" runat="server" OnClientClick="planoContaControl='EstornoDevolucaoPagto'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label19" runat="server" Text="Receb. de Funcionário" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lblFunc" runat="server">Nenhum plano de conta associado.</asp:Label>
                    </td>
                    <td align="left">
                        <asp:LinkButton ID="LinkButton9" runat="server" OnClientClick="planoContaControl='Func'; openWindow(500, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                        <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:Button ID="btnSalvarPlanoConta" runat="server" Text="Salvar" OnClientClick="verificarPlanosContaEstaoPreenchidos(this);" OnClick="btnSalvarPlanoConta_Click"/></td>
                </tr>
            </table>
            <asp:HiddenField ID="hdfFunc" runat="server" />
            <asp:HiddenField ID="hdfEntrada" runat="server" />
            <asp:HiddenField ID="hdfEstorno" runat="server" />
            <asp:HiddenField ID="hdfEstornoRecPrazo" runat="server" />
            <asp:HiddenField ID="hdfEstornoEntrada" runat="server" />
            <asp:HiddenField ID="hdfEstornoChequeDev" runat="server" />
            <asp:HiddenField ID="hdfDevolucaoPagto" runat="server" />
            <asp:HiddenField ID="hdfEstornoDevolucaoPagto" runat="server" />
            <asp:HiddenField ID="hdfVista" runat="server" />
            <asp:HiddenField ID="hdfRecPrazo" runat="server" />
            <asp:HiddenField ID="hdfRecChequeDev" runat="server" />
        </fieldset>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCartao" runat="server" SelectMethod="GetOrdered"
            TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetAll"
            TypeName="Glass.Data.DAL.ContaBancoDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAssocContaBanco" runat="server"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server"
            SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
