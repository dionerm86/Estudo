<%@ Page Title="Receber Sinal" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadReceberSinal.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadReceberSinal" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    
    var chamarCallback = true;
    var buscandoCliente = false;
    
    function getBotao(idPedidoBuscar)
    {
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");
        for (i = 1; i < tabela.rows.length; i += 2)
        {
            var idPedido = tabela.rows[i].cells[1].innerHTML;
            if (idPedido != idPedidoBuscar)
                continue;
            
            var inputs = tabela.rows[i].cells[0].getElementsByTagName("input");
            for (j = 0; j < inputs.length; j++)
                if (inputs[j].id.indexOf("ImageButton1") > -1)
                    return inputs[j];
        }
        
        return null;
    }

    function getPedidosAbertos()
    {
        var pedidosAbertos = new Array();
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");
        
        for (i = 1; i < tabela.rows.length; i += 2)
        {
            var idPedido = tabela.rows[i].cells[1].innerHTML;
            var aberto = getBotao(idPedido);
            aberto = aberto != null ? aberto.src.toLowerCase().indexOf("menos.gif") > -1 : false;
            
            if (aberto)
                pedidosAbertos.push(idPedido);
        }
        
        FindControl("hdfPedidosAbertos", "input").value = pedidosAbertos.join(",");
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
        
        var isSinal = FindControl("hdfIsSinal", "input").value;
        var idCliente = FindControl("hdfIdCliente", "input").value;
        var validaPedido = CadReceberSinal.ValidaPedido(idPedido, idCliente, isSinal).value.split('|');
        
        if (validaPedido[0] == "false")
        {
            alert(validaPedido[1]);
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
        
        FindControl("hdfIdsPedidosRem", "input").value += idPedido + ",";
        
        FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
        cOnClick("btnBuscarPedidos", null);
    }
    
    function exibirProdutos(botao, idPedido)
    {
        var linha = document.getElementById("produtos_" + idPedido);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
    }
    
    function buscarPedidos()
    {
        var idCliente = FindControl("txtNumCli", "input").value;
        var nomeCliente = FindControl("txtNomeCliente", "input").value;
        if (idCliente == "" && nomeCliente == "")
            return;
        
        buscandoCliente = true;
        
        if (idCliente == "")
            idCliente = "0";
            
        var idsPedidosRem = FindControl("hdfIdsPedidosRem", "input").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        
        var isSinal = FindControl("hdfIsSinal", "input").value;
        FindControl("hdfBuscarIdsPedidos", "input").value = CadReceberSinal.GetPedidosByCliente(idCliente, nomeCliente, idsPedidosRem, dataIni, dataFim, isSinal).value;
    }
    
    function selecionaTodosProdutos(check)
    {
        var tabela = check;
        while (tabela.nodeName.toLowerCase() != "table")
            tabela = tabela.parentNode;
        
        var checkBoxProdutos = tabela.getElementsByTagName("input");
        
        var i = 0;
        for (i = 0; i < checkBoxProdutos.length; i++)
        {
            if (checkBoxProdutos[i].id.indexOf("chkTodos") > -1 || checkBoxProdutos[i].disabled)
                continue;
            
            checkBoxProdutos[i].checked = check.checked;
        }
    }

    function getCli(idCli) {
        if (idCli.value == "")
            return false;

        var idCliente = idCli.value;

        var retorno = MetodosAjax.GetCli(idCliente).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return false;
        }
        
        FindControl("txtNomeCliente", "input").value = retorno[1];
        FindControl("txtNumCli", "input").value = idCliente;
        FindControl("hdfIdCliente", "input").value = idCliente;

        return false;
    }

    // Abre popup para cadastrar cheques
    function queryStringCheques(altura, largura, url) {
        return "?IdSinal=&origem=1&IdCli=" + FindControl("txtNumCli","input").value + "&tipoPagto=2";
    }

    function Confirmar(control, sinal) {
       
        if (!validate())
            return false;
        
        if (!confirm(control.value + '?'))
            return false;

        control.disabled = true;
        
        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value;
        
        // Verifica se os pedidos foram buscados
        if (idsPedidos == "") {
            alert("Busque um pedido primeiro.");
            control.disabled = false;
            return false;
        }
        
        var controle = <%= ctrlFormaPagto1.ClientID %>;
        
        var isGerarCredito = controle.GerarCredito();
        var creditoUtilizado = controle.CreditoUtilizado();
        
        if (sinal && <%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %> && !isGerarCredito)
        {
            var totalPago = creditoUtilizado;
            var pagtos = controle.Valores(false);
            for (i = 0; i < pagtos.length; i++)
                totalPago += pagtos[i];
            
            if (totalPago > parseFloat(CadReceberSinal.GetTotalPedidos(idsPedidos).value.replace(",", ".")))
                if (!confirm("O valor pago é maior que o valor a pagar.\nO valor da entrada e das parcelas dos pedidos serão recalculados.\n\nDeseja continuar?"))
                {
                    control.disabled = false;
                    return false;
                }
        }
        
        var formasPagto = controle.FormasPagamento();
        var tiposCartao = controle.TiposCartao();
        var parcCartao = controle.ParcelasCartao();
        var dataReceb = controle.DataRecebimento();
        var cxDiario = FindControl("hdfCxDiario", "input").value;
        var valores = controle.Valores();
        var contasBanco = controle.ContasBanco();
        var numAut = controle.NumeroConstrucard();
        var isDescontarComissao = controle.DescontarComissao();
        var obs = FindControl("txtObs", "textarea").value;
        var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
        var numAutCartao = controle.NumeroAutCartao();
        var CNI = controle.CartoesNaoIdentificados();
        // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
        var chequesPagto = controle.Cheques();

        var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
        var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
        var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;
        var tipoRecebimento = <%= (int)Glass.Data.Helper.UtilsFinanceiro.TipoReceb.SinalPedido %>;
        var receberCappta = utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1;
        
        retorno = CadReceberSinal.Confirmar(idsPedidos, dataReceb, formasPagto, valores, contasBanco, depositoNaoIdentificado, CNI, tiposCartao, isGerarCredito, creditoUtilizado, cxDiario, numAut,
            parcCartao, chequesPagto, isDescontarComissao, obs, sinal, numAutCartao, receberCappta.toString().toLowerCase()).value.split('\t');
        
        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            control.disabled = false;
            return false;
        }

        //Se utilizar o TEF CAPPTA e tiver selecionado pagamento com cartão à vista
        if (receberCappta) {

            //Busca os dados para autenticar na cappta
            var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

            if(dadosAutenticacaoCappta.error) {
                desbloquearPagina(true);
                alert(dadosAutenticacaoCappta.error.description);
                return false;
            }

            //Instancia do canal de recebimento
            CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msgErro, codigosAdministrativos, msgRetorno) => callbackCappta(sucesso, msgErro, codigosAdministrativos, msgRetorno));

            //Inicia o recebimento
            CapptaTef.efetuarRecebimento(retorno[1], tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcCartao);

            return false;
        }


        alert(retorno[1]);
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Sinal&idSinal=" + retorno[2]);
        redirectUrl(window.location.href);
   
        
        return false;
    }

    //Método chamado ao realizar o pagamento atraves do TEF CAPPTA
    function callbackCappta(sucesso, msgErro, codigosAdministrativos, msgRetorno) {

        desbloquearPagina(true);

        if(!sucesso) {
            alert(msgErro);
            FindControl("btnReceberPagtoAntecip", "input").disabled = false;
            return false;
        }

        var retorno = msgRetorno.split('\t');

        alert(retorno[0]);
        openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos.join(';'));
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Sinal&idSinal=" + retorno[1]);
        redirectUrl(window.location.href);
        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAdd', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addPedido(); return false;" />
                        </td>
                        <td>
                            <asp:Label ID="lblCliente" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('btnBuscarPedidos', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;"> <img border="0" src="../Images/Pesquisar.gif" /> </asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblData" runat="server" Text="Data de Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc4:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc4:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarPedidos" runat="server" Text="Buscar Pedidos" OnClick="btnBuscarPedidos_Click"
                    OnClientClick="buscarPedidos()" CausesValidation="False" />
                <asp:HiddenField ID="hdfBuscarIdsPedidos" runat="server" />
                <asp:HiddenField ID="hdfPedidosAbertos" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedidos" DataKeyNames="IdPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum pedido confirmado encontrado."
                    OnDataBound="grdPedido_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "removePedido(" + Eval("IdPedido") + "); return false;" %>'
                                    ToolTip="Remover pedido" />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdPedido") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir produtos" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("TotalPedidoFluxo") %>' />
                                <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Eval("ValorEntrada") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor Entrada"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="TextoDescontoTotal" HeaderText="Desconto" SortExpression="TextoDescontoTotal" />
                        <asp:BoundField DataField="ValorIcms" DataFormatString="{0:c}" HeaderText="Valor ICMS"
                            SortExpression="ValorIcms" />
                        <asp:BoundField DataField="TotalPedidoFluxo" HeaderText="Total" SortExpression="TotalPedidoFluxo"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescrTipoVenda").ToString() + Eval("DescricaoParcelas").ToString() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrTipoVenda") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Data Entrega"
                            SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Fast Delivery" SortExpression="FastDelivery">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("FastDelivery") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("FastDelivery") %>'
                                    Enabled="False" />
                                &nbsp;<asp:Label ID="Label13" runat="server" Text='<%# (decimal)((float)Eval("TaxaFastDelivery") / 100) * (decimal)Eval("TotalPedidoFluxo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ObsLiberacao" HeaderText="Obs. Lib." />
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="produtos_<%# Eval("IdPedido") %>" style="display: none" class="<%= GetAlternateClass() %>">
                                    <td>
                                    </td>
                                    <td colspan="11" style="padding: 0px">
                                        <asp:GridView ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                            DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido" GridLines="None" Width="100%">
                                            <Columns>
                                                <asp:BoundField DataField="CodInterno" HeaderText="Cod." SortExpression="CodInterno">
                                                    <ItemStyle Wrap="False" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="AlturaLista">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label14" runat="server" Text='<%# Bind("TotM2Liberacao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Valor ICMS" SortExpression="ValorIcms">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorIcms", "{0:c}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorIcms") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" SortExpression="TotalCalc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("TotalCalc", "{0:C}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("TotalCalc") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("QtdeDisponivelLiberacao") %>'></asp:Label>
                                                        <asp:Label ID="Label12" runat="server" Text='<%# Eval("QtdePecasVidroMaoDeObra") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("QtdeDisponivelLiberacao") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:GridView>
                                        <asp:HiddenField ID="hdfIdPedidoProdutos" runat="server" Value='<%# Eval("IdPedido") %>' />
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" SelectMethod="GetForLiberacao"
                                            TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdPedidoProdutos" Name="idPedido" PropertyName="Value"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblPedidosRem" runat="server"></asp:Label>
                <asp:ImageButton ID="imbLimparRemovidos" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                    OnClick="imbLimparRemovidos_Click" Visible="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIsSinal" runat="server" />
                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                <asp:HiddenField ID="hdfValorASerPago" runat="server" />
                <asp:HiddenField ID="hdfCxDiario" runat="server" />
                <asp:HiddenField ID="hdfIdsPedidosRem" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidos" runat="server" SelectMethod="GetForReceberSinal"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfBuscarIdsPedidos" Name="idsPedidos" PropertyName="Value"
                            Type="String" />
                        <asp:ControlParameter ControlID="hdfIsSinal" Name="isSinal" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divConfirmar" runat="server" align="center">
                    <div id="divInsertSinal" runat="server" visible="False">
                        <uc3:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load"
                            TipoModel="Pedido" ExibirJuros="false" ExibirRecebParcial="false" ExibirApenasCartaoDebito="true"
                            FuncaoQueryStringCheques="queryStringCheques" TextoValorReceb="Sinal" RecalcularCredito="true" />
                        <asp:Label ID="lblMensagemRetroativa" runat="server" Text="<br />A data de recebimento só é usada se o pagamento for para uma conta bancária.<br />"></asp:Label>
                        <br />
                        <div>
                            <div>
                                <asp:Label ID="lblObs" runat="server" Text="Obs."></asp:Label>
                                <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="300px"
                                    MaxLength="200" />
                            </div>
                        </div>
                        <div style="float: left; margin-left:39%;">
                            <uc5:ctrlLimiteTexto ID="CtrlLimiteTexto1" runat="server" IdControlToValidate="txtObs" />
                        </div>
                        <div style="clear: both;">
                        </div>
                        <br />
                        <br />
                        <asp:Button ID="btnReceberSinal" runat="server" Text="Confirmar Sinal" OnClientClick="return Confirmar(this, true);"
                            Width="120px" />
                        <asp:Button ID="btnReceberPagtoAntecip" runat="server" Text="Confirmar Pagamento Antecipado"
                            Visible="false" OnClientClick="return Confirmar(this, false);" />
                    </div>
                    <div id="divViewSinal" runat="server" visible="False">
                        <table cellpadding="0" cellspacing="0" style="width: 100%;">
                            <tr>
                                <td nowrap="nowrap" align="center">
                                    <asp:Label ID="lblViewSinal" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        if (<%= (!String.IsNullOrEmpty(hdfBuscarIdsPedidos.Value) && divInsertSinal.Visible).ToString().ToLower() %>)
            <%= ctrlFormaPagto1.ClientID %>.AdicionarIDs("<%= hdfBuscarIdsPedidos.Value %>");

        FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
