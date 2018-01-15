<%@ Page Title="Emitir Cupom Fiscal Eletrônico - SAT-CFe" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadEmitirCFe.aspx.cs" Inherits="Glass.UI.Web.CFe.CadEmitirCFe" %>

<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
    var clientes = new Array();
    var buscandoCliente = false;
    
    function exibirOperadoraCartao(comboFrmPg, nomeDiv)
    {
        var opcao = FindControl(comboFrmPg, "select").value;
    
        if(opcao == "CartaoCredito" || opcao == "CartaoDebito")
            document.getElementById(nomeDiv).style.display = "";
        else
            document.getElementById(nomeDiv).style.display = "none";
    }
    
    function incluirFormaPag()
    {
        var numeroDiv = parseInt(FindControl("totalFormasPag", "input").value) + 1;
        
        if(numeroDiv <= 3)
        {
            var divExibir = "divPag" + numeroDiv.toString();
            var divRemoverPag = "divRemoverPag" + (numeroDiv - 1).toString();
            
            FindControl("totalFormasPag", "input").value = (parseInt(FindControl("totalFormasPag", "input").value) + 1).toString();
            
            document.getElementById(divExibir).style.display = "";
            try { document.getElementById(divRemoverPag).style.display = "none"; } catch(e) { }
        }
    }
    
    function removerPgto(divOcultar)
    {
        var numeroDiv = parseInt(FindControl("totalFormasPag", "input").value);
        
        var divRemoverPag = "divRemoverPag" + (numeroDiv - 1).toString();
        var textPag = "txtValorPago" + numeroDiv.toString();
        
        FindControl("totalFormasPag", "input").value = (parseInt(FindControl("totalFormasPag", "input").value) - 1).toString();       
        FindControl(textPag, "input").value = "";
        
        document.getElementById(divOcultar).style.display = "none";
        try{document.getElementById(divRemoverPag).style.display = "inline";}catch(e){}
    }
    
    function addPedido()
    {
        if (buscandoCliente)
            return;
        
        var idPedido = FindControl("txtNumPedido", "input").value;
        if (Trim(idPedido) == "")
        {
            alert("Selecione um pedido para continuar.");
            FindControl("txtNumPedido", "input").value = "";
            FindControl("txtNumPedido", "input").focus();
            return;
        }
        
        if (CadEmitirCFe.PedidoExiste(idPedido).value == "false")
        {
            alert("Não existe pedido com esse número.");
            FindControl("txtNumPedido", "input").value = "";
            FindControl("txtNumPedido", "input").focus();
            return;
        }
        
        if (CadEmitirCFe.IsPedidoConfirmadoLiberado(idPedido).value == "false")
        {
            alert("Esse pedido não está confirmado.");
            FindControl("txtNumPedido", "input").value = "";
            FindControl("txtNumPedido", "input").focus();
            return;
        }
        
        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
        var novosIds = new Array();
        
        novosIds.push(idPedido);
        for (i = 0; i < idsPedidos.length; i++)
            if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                novosIds.push(idsPedidos[i]);
        
        FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
        FindControl("txtNumPedido", "input").value = "";
        cOnClick("btnBuscarPedidos", null);
    }
    
    function removePedido(idPedido)
    {
        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
        var novosIds = new Array();
        
        for (i = 0; i < idsPedidos.length; i++)
            if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                novosIds.push(idsPedidos[i]);
        
        FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
        cOnClick("btnBuscarPedidos", null);
    }
          
    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCli", "input").value = "";
            return false;
        }

        FindControl("txtNomeCli", "input").value = retorno[1];
    }
    
    function cancelarCFe(botao)
    {
        if(!confirm("Deseja realmente cancelar o último cupom emitido?"))
            return false;
        
        var idLoja = FindControl("drpLoja", "select").value;
        
        botao.disabled = true;
        document.getElementById("load").style.display = "";
        
        var retorno = CadEmitirCFe.ObterDadosCancelarCupom(idLoja).value.split(';');
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            botao.disabled = false;
            document.getElementById("load").style.display = "none";
            return;
        }
        else
        {
            var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
            var numSessao = CadEmitirCFe.ObterNumeroSessao().value;
            var codigoAtivacao = CadEmitirCFe.ObterCodigoAtivacao().value;
            var chaveCupom = retorno[1];
            var seqCupom = retorno[2];
            var dadosCancel = retorno[3];
            
            var retornoSAT = active.CancelarUltimaVenda(parseInt(numSessao), codigoAtivacao, chaveCupom, dadosCancel).split('|');        
            
            CadEmitirCFe.CancelarCupom(seqCupom);
            CadEmitirCFe.SalvarArquivoCupom(retornoSAT[6], chaveCupom, true);
            
            botao.disabled = false;
            alert("Cupom fiscal cancelado com sucesso!");            
            document.getElementById("load").style.display = "none";
        }        
    }
    
    function emitirCupom(botao, testeFimAFim)
    {
        if (!validate())
            return false;

        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value;
        if(idsPedidos == null || idsPedidos == "")
        {
            alert("É necessário informar pelo menos um pedido.");
            return;
        }
        
        if(testeFimAFim)
            if(!confirm("Esta ação apenas realiza uma conexão de teste com o aparelho SAT. O cupom emitido não terá validade Fiscal. Desaja continuar?"))
                return;
        
        // Informa sobre os pedidos que já contém cupons fiscais
        var tabela = document.getElementById("<%= grdPedidos.ClientID %>");
        var pedidosNf = new Array();
        
        for (i = 1; i < tabela.rows.length; i++)
        {
            var inputs = tabela.rows[i].cells[0].getElementsByTagName("input");
            var idPedido;
            var notasGeradas;
            
            for (j = 0; j < inputs.length; j++)
                if (inputs[j].id.indexOf("hdfIdPedido") > -1)
                    idPedido = inputs[j].value;
                else if (inputs[j].id.indexOf("hdfNotasGeradas") > -1)
                    notasGeradas = inputs[j].value;
            
            if (notasGeradas.length > 0)
                pedidosNf.push(new Array(idPedido, notasGeradas));
        }
        
        if (pedidosNf.length > 0)
        {
            var pedidos = "";
            for (i = 0; i < pedidosNf.length; i++)
                pedidos += ", " + pedidosNf[i][0] + " (" + pedidosNf[i][1] + ")";
            
            if (!confirm("Os seguintes pedidos já possuem cupons fiscais gerados:\n" +
                pedidos.substr(2) + ".\n\nDeseja continuar com a emissão do cupom?"))
                return false;
        }
               
        var idNaturezaOperacao = FindControl("ctrlNaturezaOperacao_selNaturezaOperacao_hdfValor", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var valorTotal = FindControl("lblTotal", "span").innerText.replace('R$', '').replace(' ','');
        
        // Formas de Pagamento
        var valorPago = FindControl("txtValorPago", "input").value + "|" + FindControl("txtValorPago2", "input").value
                + "|" + FindControl("txtValorPago3", "input").value;        
        
        var formaPag = FindControl("cbFormaPagamento", "select").value + "|" + FindControl("cbFormaPagamento2", "select").value
               + "|" + FindControl("cbFormaPagamento3", "select").value;

        var opCartao = FindControl("cbOperadoraCartao", "select").value + "|" + FindControl("cbOperadoraCartao2", "select").value
               + "|" + FindControl("cbOperadoraCartao3", "select").value;       

        // Se a empresa calcula icms no pedido, verifica se os pedidos possuem ST e se a natureza de operação calcula ST
        if (CadEmitirCFe.CalculaIcmsPedido().value == "true") {
            var pedidosPossuemSt = CadEmitirCFe.PedidosPossuemSt(idsPedidos).value == "true";
            var calcSt = CadEmitirCFe.NaturezaOperacaoCalcSt(idNaturezaOperacao).value == "true";

            if (pedidosPossuemSt && !calcSt) {
                if (!confirm("Em alguns dos pedidos selecionados foram calculados o ICMS ST, porém você selecionou uma natureza de operação que não calcula ICMS ST, haverá diferença de valores, deseja continuar assim mesmo?"))
                    return false;
            }
        }

        if (idNaturezaOperacao == "" || idNaturezaOperacao == "0") {
            alert("Informe a natureza de operação.");
            return false;
        }

        if (idLoja == "" || idLoja == "0") {
            alert("Informe a loja.");
            return false;
        }       
        
        if(FindControl("txtValorPago", "input").value == "" || FindControl("txtValorPago", "input").value == "0")
        {
            alert("Informe o valor pago.");
            return false;
        }      

        botao.disabled = true;
        document.getElementById("load").style.display = "";
        
        var numeroSessao = CadEmitirCFe.ObterNumeroSessao().value;
        var codAtivacao = CadEmitirCFe.ObterCodigoAtivacao().value;

        var dadosVenda = CadEmitirCFe.ObterDadosEmitirCupom(idsPedidos, false, false, idNaturezaOperacao, idLoja, valorPago, formaPag, opCartao, valorTotal).value.split(';');
        
        if (dadosVenda[0] == "Erro")
        {
            alert(dadosVenda[1]);
            botao.disabled = true;
            document.getElementById("load").style.display = "none";
            return;
        }
        else
        {
            try
            {               
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                
                if(testeFimAFim)
                    var retornoSAT = active.TesteFimAFim(parseInt(numeroSessao), codAtivacao, dadosVenda[2]).split('|');
                else
                    var retornoSAT = active.EnviarDadosVenda(parseInt(numeroSessao), codAtivacao, dadosVenda[2]).split('|');
                
                if(retornoSAT.length > 5)
                {
                    var numeroCupom = CadEmitirCFe.SalvarCupom(numeroSessao, retornoSAT[8], dadosVenda[1], idLoja, idsPedidos, valorTotal, valorPago).value.split(';');
                    CadEmitirCFe.SalvarArquivoCupom(retornoSAT[6], retornoSAT[8], false);
                    
                    openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=CFeCliente&idLoja=" + idLoja + "&idCupom=" + numeroCupom[1] + "&idsPedidos=" + idsPedidos
                          + "&idNaturezaOperacao=" + idNaturezaOperacao);
                    
                }
                else
                {
                    alert(retornoSAT[3]);
                }
            }
            catch(e)
            {
                ExibeLoad(false);
                alert(e);
            }            
        
            botao.disabled = false;                         
            document.getElementById("load").style.display = "none";
            redirectUrl("CadEmitirCFe.aspx");
        }
    }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblPedido" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAddPed', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddPed" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addPedido(); return false;" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <img alt="" id="load" src="../Images/load.gif" style="display: none" />
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja">
                            </asp:DropDownList>                        
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            Natureza de Operação:
                        </td>
                        <td>
                            <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server"
                                PermitirVazio="false" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarPedidos" runat="server" Text="Buscar Pedidos" OnClick="btnBuscarPedidos_Click"
                    OnClientClick="buscarPedidos()" CausesValidation="False" style="display: none" />
                <asp:Button ID="btnCancelarUltimoCFe" runat="server" Text="Cancelar Último Cupom Emitido"
                    OnClientClick="cancelarCFe(this); return false;" CausesValidation="true" />
                <asp:Button ID="btnTesteFimAFim" runat="server" Text="Realizar Teste Fim a Fim"
                    OnClientClick="emitirCupom(this, true); return false;" CausesValidation="true" />                    
                <asp:HiddenField ID="hdfBuscarIdsPedidos" runat="server" />
                <br />
                <br />
                <table id="gerar" runat="server" visible="false">
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdPedidos" runat="server" 
                                AutoGenerateColumns="False" DataSourceID="odsPedidos"
                                DataKeyNames="IdPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Não foram encontrados pedidos confirmados para esse cliente ou com esse número."
                                EnableViewState="False" 
                                ondatabound="grdPedidos_DataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                OnClientClick='<%# "removePedido(" + Eval("IdPedido") + "); return false;" %>'
                                                ToolTip="Remover pedido" />
                                            <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                            <asp:HiddenField ID="hdfNotasGeradas" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                    <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
                                    <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                                    <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalPedido" runat="server" Text='<%# Bind("Total", "{0:C}") %>' 
                                                Visible='<%# !(bool)Eval("ExibirTotalEspelho") %>'></asp:Label>
                                            <asp:Label ID="lblTotalPedidoEsp" runat="server" 
                                                Text='<%# Bind("TotalEspelho", "{0:C}") %>' 
                                                Visible='<%# Eval("ExibirTotalEspelho") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data Pedido"
                                        SortExpression="DataPedido" />
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                            <br />
                            <div style="font-size: medium; text-align: center; margin-bottom: 10px">
                                Total: <asp:Label ID="lblTotal" runat="server" Text=""></asp:Label>
                            </div>                            
                            <asp:PlaceHolder ID="plhPagamentos" runat="server">
                                <div id="divPag1">
                                    <div style="font-size: medium; text-align: center; display:inline;">
                                        Valor Pago: <asp:TextBox ID="txtValorPago1" runat="server" onKeyPress="return soNumeros(event, false, true);" />
                                    </div>
                                    <div style="font-size: medium; text-align: center; display:inline; margin-left: 15px;">
                                        Forma de Pagamento: <asp:DropDownList ID="cbFormaPagamento1" runat="server" onChange="exibirOperadoraCartao('cbFormaPagamento1', 'divOperadoraCartao1');" />
                                    </div>
                                    <div id="divOperadoraCartao1" style="font-size: medium; text-align: center; margin-top: 10px; display: none;">
                                        Operadora Cartão: <asp:DropDownList ID="cbOperadoraCartao1" runat="server" />
                                    </div>
                                </div>
                                <div id="divPag2" style="display:none; margin-top: 15px;">
                                    <div style="font-size: medium; text-align: center; display:inline;">
                                        Valor Pago: <asp:TextBox ID="txtValorPago2" runat="server" onKeyPress="return soNumeros(event, false, true);" />
                                    </div>
                                    <div style="font-size: medium; text-align: center; display:inline; margin-left: 15px;">
                                        Forma de Pagamento: <asp:DropDownList ID="cbFormaPagamento2" runat="server" onChange="exibirOperadoraCartao('cbFormaPagamento2', 'divOperadoraCartao2');" />
                                    </div>
                                    <div id="divRemoverPag2" style="display:inline; margin-left: 10px;">
                                        <asp:LinkButton ID="lnkRemoverPag2" runat="server" OnClientClick="removerPgto('divPag2'); return false;" CausesValidation="false" Text="Remover" />
                                    </div>
                                    <div id="divOperadoraCartao2" style="font-size: medium; text-align: center; margin-top: 10px; display: none;">
                                        Operadora Cartão: <asp:DropDownList ID="cbOperadoraCartao2" runat="server" />
                                    </div>
                                </div>
                                <div id="divPag3" style="display:none; margin-top: 15px;">
                                    <div style="font-size: medium; text-align: center; display:inline;">
                                        Valor Pago: <asp:TextBox ID="txtValorPago3" runat="server" onKeyPress="return soNumeros(event, false, true);" />
                                    </div>
                                    <div style="font-size: medium; text-align: center; display:inline; margin-left: 15px;">
                                        Forma de Pagamento: <asp:DropDownList ID="cbFormaPagamento3" runat="server" onChange="exibirOperadoraCartao('cbFormaPagamento3', 'divOperadoraCartao3');" />
                                    </div>
                                    <div id="divRemoverPag3" style="display:inline; margin-left: 10px;">
                                        <asp:LinkButton ID="lnkRemoverPag3" runat="server" OnClientClick="removerPgto('divPag3'); return false;" CausesValidation="false" Text="Remover" />
                                    </div>                                    
                                    <div id="divOperadoraCartao3" style="font-size: medium; text-align: center; margin-top: 10px; display: none;">
                                        Operadora Cartão: <asp:DropDownList ID="cbOperadoraCartao3" runat="server" />
                                    </div>
                                </div>                                
                            </asp:PlaceHolder>
                            <div id="divIncluirPagamento" style="margin-top: 17px;">
                                <asp:LinkButton ID="lnkIncluirPag" runat="server" OnClientClick="incluirFormaPag(); return false;" Text="Incluir Forma de Pagamento" CausesValidation="false" />
                            </div>                            
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidos" runat="server" SelectMethod="GetForNFe" TypeName="Glass.Data.DAL.PedidoDAO"
                                OnSelected="odsPedidos_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hdfBuscarIdsPedidos" Name="idsPedidos" PropertyName="Value"
                                        Type="String" />
                                    <asp:Parameter Name="idsLiberarPedidos" Type="String" />
                                    <asp:Parameter Name="idCliente" DefaultValue="0" Type="UInt32" />
                                    <asp:Parameter Name="nomeCliente" DefaultValue="" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td colspan="5" align="center">
                                        <asp:Button ID="btnGerarNf" runat="server" OnClientClick="emitirCupom(this, false); return false;" Text="Emitir Cupom" />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" OnSelected="odsLoja_Selected" SelectMethod="GetAll"
                                            TypeName="Glass.Data.DAL.LojaDAO"></colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
    <div id="divHiddens">
        <asp:HiddenField ID="totalFormasPag" runat="server" Value="1" />
        <asp:HiddenField ID="hidSeqCupom" runat="server" Value="" />
    </div>
    
    <script type="text/javascript">
        if (!FindControl("txtNumPedido", "input").disabled)
            FindControl("txtNumPedido", "input").focus();
        else if (!FindControl("txtLiberacao", "input").disabled)
            FindControl("txtLiberacao", "input").focus();
    </script>

</asp:Content>
