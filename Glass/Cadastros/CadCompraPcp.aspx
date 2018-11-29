<%@ Page Title="Cadastro de Compra de Mercadoria" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadCompraPcp.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadCompraPcp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    
    <style type="text/css">
        .tabela div.coluna
        {
        	width: 350px;
        	/* height: 300px; */
        	overflow: auto;
        }
        
        .tabela
        {
            padding: 0px;
            border-spacing: 0px;
        }
        
        .tabela div.coluna table
        {
            width: 99%;
        }
    </style>
    
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        
        // Variável de controle dos produtos selecionados
        var produtos = new Array();
        var linhas = new Array();

        function adicionar()
        {
            // Chamado: 85525
            var existePedidoInserido = FindControl("hdfIdsPedidos", "input").value != "";
            if (existePedidoInserido) {
                alert("Apenas um pedido poderá ser associado a compra de mercadorias");
                FindControl("txtIdPedido", "input").value = "";
                return;
            }

            var idPedido = FindControl("txtIdPedido", "input").value;
            if (idPedido == "")
            {
                alert("Digite o número do pedido.");
                return;
            }

            var resposta = CadCompraPcp.VerificaPedido(idPedido).value.split('\t');
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            addItem(new Array(idPedido), new Array("Pedido"), "tbPedidos", idPedido, "hdfIdsPedidos", null, null, "buscarAmbientes", false);

            buscarAmbientes();

            FindControl("txtIdPedido", "input").value = "";
        }
        
        function buscarAmbientes()
        {
            var tabela = document.getElementById("ambientes_pedido");
            for (a = tabela.rows.length - 1; a >= 0; a--)
                tabela.deleteRow(a);

            var idsPedidos = FindControl("hdfIdsPedidos", "input").value;
            if(idsPedidos == "")
                return;
            
            var resposta = CadCompraPcp.GetAmbientes(idsPedidos).value.split('~');
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }
            
            var colunas = 5;
            var numColuna = colunas;
            var linha;
            
            var ambientesMarcados = FindControl("hdfIdAmbientesPedido", "input").value;
            ambientesMarcados = ambientesMarcados != "" ? ambientesMarcados.split(',') : new Array();
            
            for (a = 1; a < resposta.length; a++)
            {
                if (resposta[a].split("^") == "")
                    continue;
                
                var id = resposta[a].split("^")[0];
                var descricao = resposta[a].split("^")[1];
                
                if (numColuna == colunas)
                {
                    linha = tabela.insertRow(tabela.rows.length);
                    numColuna = 0;
                }
                
                var marcado = false;
                if (ambientesMarcados.length > 0)
                {
                    for (m = 0; m < ambientesMarcados.length; m++)
                        if (ambientesMarcados[m] == id)
                        {
                            marcado = true;
                            break;
                        }
                }
                
                var celula = linha.insertCell(linha.cells.length);
                celula.innerHTML = "<input id='chkAmbiente_" + id + "' type='checkbox' " + (marcado ? "checked='checked'" : "") + " value='" + id + "' /><label for='chkAmbiente_" + id + "'>" + descricao + "</label>";
                numColuna++;
            }
        }

        function buscar()
        {
            var usarAmbientes = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.AmbientePedido.ToString().ToLower() %>;
            if (!usarAmbientes)
                return true;
            
            var tabela = document.getElementById("ambientes_pedido");
            var inputs = tabela.getElementsByTagName("input");
            
            if (inputs.length == 0)
            {
                alert("Os ambientes do pedido não foram carregados. Verifique se o pedido está cadastrado no PCP.");
                return false;
            }
            
            var marcados = new Array();
            for (b = 0; b < inputs.length; b++)
            {
                if (inputs[b].type.toLowerCase() != "checkbox")
                    continue;
                
                if (inputs[b].checked)
                    marcados.push(inputs[b].value);
            }
            
            if (marcados.length == 0)
            {
                alert("Selecione um ambiente para continuar.");
                return false;
            }
            
            FindControl("hdfIdAmbientesPedido", "input").value = marcados.join(",");
            return true;
        }
        
        function exibir(botao, texto)
        {
            var linha = document.getElementById(texto);
            var exibir = linha.style.display == "none";
            
            linha.style.display = exibir ? "" : "none";
            
            var textoAtual = exibir ? "mais" : "menos";
            var textoNovo = exibir ? "menos" : "mais";
            botao.src = botao.src.replace(textoAtual, textoNovo);
        }
        
        // Função executada para exibir/esconder os produtos de um ambiente
        function exibirAmbiente(botao, id)
        {
            exibir(botao, "ambiente_" + id);
        }
                
        // Função executada para carregar o plano de contas padrão do fornecedor
        function alteraFornecedor(idFornecedor)
        {
            var planoConta = CadCompraPcp.GetPlanoConta(idFornecedor).value;
            if (planoConta != "")
                FindControl("ddlPlanoConta", "select").value = planoConta;
        }
    
        function setPedidoEspelho(idPedido)
        {
            FindControl("txtIdPedido", "input").value = idPedido;
        }
        
        // Função executada para incluir um produto na tabela de cadastro
        function setProduto(botao, id, codigo, ambiente, produto, qtdeMax, apenasBeneficiamentos)
        {
            // Recupera as linhas que serão escondidas
            var linha = botao.parentNode.parentNode;
            var linhaBenef = linha.nextSibling;
            while (linhaBenef.nodeName.toLowerCase() != "tr")
                linhaBenef = linhaBenef.nextSibling;
            
            // Recupera os beneficiamentos
            var benef = "";
            var descrBenef = "";
            var nomeBenef = "";
            var tabela = linhaBenef.getElementsByTagName("table")[0];
            var qtdeMinBenef = 0;

            if (tabela != null)
            {
                for (i = 0; i < tabela.rows.length; i++)
                {
                    var hidden = tabela.rows[i].cells[0].getElementsByTagName("input")[1];
                    var checkbox = tabela.rows[i].cells[0].getElementsByTagName("input")[0];
                    var label = tabela.rows[i].cells[0].getElementsByTagName("label")[0];
                    var qtdeBenef = tabela.rows[i].cells[0].getElementsByTagName("input")[2];
                    
                    if (checkbox.checked)
                    {
                        benef += "," + hidden.value;
                        descrBenef += ", " + label.innerHTML;
                        
                        if (qtdeMinBenef == 0 || parseInt(qtdeBenef.value, 10) < qtdeMinBenef)
                        {
                            nomeBenef = label.innerHTML;
                            qtdeMinBenef =
                                qtdeBenef == undefined || qtdeBenef == null || qtdeBenef.value == "" ?
                                    qtdeMax : parseInt(qtdeBenef.value, 10);
                        }
                    }
                }
                
                if (benef.length > 0)
                {
                    benef = benef.substr(1);
                    descrBenef = " (" + descrBenef.substr(2) + ")";
                }
            }
            
            if (apenasBeneficiamentos)
            {
                if (benef.length == 0)
                {
                    alert("Selecione pelo menos 1 beneficiamento para o vidro.");
                    return;
                }
                
                qtdeMax = qtdeMinBenef;
            }
            else
                nomeBenef = "";

            // Cria as variáveis que serão passadas para a função de criação da tabela
            var titulos = new Array("Cód.", "Ambiente", "Produto", "Qtde", "Só benef.?");
            var celulaCodigo = "<span>" + codigo + "</span><input type='hidden' value='" + id + "' /><input type='hidden' value='" + benef + "' />" +
                "<input type='hidden' value='" + apenasBeneficiamentos + "' /><input type='hidden' value='" + nomeBenef + "' />";
            var celulaQtde = "<input type='text' value='" + qtdeMax + "' disabled='disabled' onkeypress='return soNumeros(event, false, true)' style='width: 40px' /><input type='hidden' value='" + qtdeMax + "' />";
            var itens = new Array(celulaCodigo, ambiente, produto + descrBenef, celulaQtde, apenasBeneficiamentos ? "Sim" : "Não");
            
            // Cria a linha na tabela
            addItem(itens, titulos, "tbProdutos", null, null, null, null, "removerProduto");
            
            // Adiciona o produto às variáveis de controle
            produtos.push(id);
            linhas.push(linha);
            linhas.push(linhaBenef);
            
            // Esconde as 2 linhas que representam o produto
            linha.style.display = "none";
            linhaBenef.style.display = "none";
            
            // Recupera o nome da tabela
            var tabela = linha;
            while (tabela.nodeName.toLowerCase() != "table" && tabela.id.indexOf("grdProdutosPedido") == -1)
                tabela = tabela.parentNode;
            
            // Atualiza a cor das linhas
            drawAlternateLinesEx(tabela.id, 2);
        }
        
        // Função executada ao remover um produto da tabela
        function removerProduto(linha)
        {
            // Cria duas variáveis novas
            var produtosNova = new Array();
            var linhasNova = new Array();
            
            // Recupera o id do produto
            var id = linha.cells[1].getElementsByTagName("input")[0].value;
            
            // Remove o id do produto da variável de controle e exibe as linhas novamente
            for (i = 0; i < produtos.length; i++)
                if (produtos[i] != id)
                {
                    produtosNova.push(produtos[i]);
                    linhasNova.push(linhas[i * 2]);
                    linhasNova.push(linhas[i * 2 + 1]);
                }
                else
                {
                    linhas[i * 2].style.display = "";
                    linhas[i * 2 + 1].style.display = "";
                }
            
            // Atualiza as variáveis de controle
            produtos = produtosNova;
            linhas = linhasNova;
            
            // Recupera o nome da tabela
            var tabela = linha;
            while (tabela.nodeName.toLowerCase() != "table" && tabela.id.indexOf("grdProdutosPedido") == -1)
                tabela = tabela.parentNode;
            
            // Atualiza a cor das linhas
            drawAlternateLinesEx(tabela.id, 2);
        }
        
        // Função que valida a página
        function validar()
        {
            //if (!validate())
            //    return false;
            
            // Valida o fornecedor
            if (FindControl("ddlFornecedor", "select").value == "")
            {
                alert("Selecione um fornecedor para continuar.");
                return false;
            }
            
            // Valida o plano de contas
            if (FindControl("ddlPlanoConta", "select").value == "")
            {
                alert("Selecione um plano de contas para continuar.");
                return false;
            }
            
            // Garante que haja um produto selecionado na tabela
            if (linhas.length == 0)
            {
                alert("Selecione um produto para a compra antes de continuar.");
                return false;
            }
            
            // Valida os produtos selecionados
            var tabela = document.getElementById("tbProdutos");
            for (i = 1; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].style.display == "none")
                    continue;
                
                var codigo = Trim(tabela.rows[i].cells[1].getElementsByTagName("span")[0].innerHTML);
                var qtde = tabela.rows[i].cells[4].getElementsByTagName("input")[0].value;
                var qtdeMax = tabela.rows[i].cells[4].getElementsByTagName("input")[1].value;
                var soBenef = tabela.rows[i].cells[1].getElementsByTagName("input")[2].value.toLowerCase() == "true";
                var nomeBenef = tabela.rows[i].cells[1].getElementsByTagName("input")[3].value;
                
                if (parseInt(qtde, 10) == 0)
                {
                    alert("Você deve escolher uma quantidade de itens para o produto de código '" + codigo + "'.");
                    return false;
                }
                
                if (parseInt(qtde, 10) > parseInt(qtdeMax, 10))
                {
                    if (nomeBenef == "")
                        alert("A quantidade de itens do produto de código '" + codigo + "' (" + qtde + ") é maior que a quantidade de itens a comprar do pedido (" + qtdeMax + ").");
                    else
                        alert("A quantidade máxima de itens do produto de código '" + codigo + "' deve ser " + qtdeMax + ", que é a quantidade máxima para o beneficiamento '" + nomeBenef + "'.");
                    
                    return false;
                }
            }
            
            return true;
        }
        
        // Função que cria a string usada para cadastrar os produtos da compra
        function setDadosProdutos()
        {
            if (linhas.length == 0)
                return;
            
            var valor = "";
            var tabela = document.getElementById("tbProdutos");
            
            // Cria a string para cada linha
            for (i = 1; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].style.display == "none")
                    continue;
                
                var codigo = tabela.rows[i].cells[1].getElementsByTagName("input")[0].value;
                var codigoInterno = Trim(tabela.rows[i].cells[1].getElementsByTagName("span")[0].innerHTML);
                var qtde = tabela.rows[i].cells[4].getElementsByTagName("input")[0].value;
                var benef = tabela.rows[i].cells[1].getElementsByTagName("input")[1].value;
                var apenasBeneficiamentos = tabela.rows[i].cells[1].getElementsByTagName("input")[2].value;
                
                valor += "|" + codigo + ";" + codigoInterno + ";" + qtde + ";" + benef + ";" + apenasBeneficiamentos;
            }
            
            // Altera o hidden hdfDadosProdutos
            var hdfDadosProdutos = document.getElementById("<%= hdfDadosProdutos.ClientID %>");
            hdfDadosProdutos.value = valor.length > 0 ? valor.substr(1) : "";
        }
        
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Pedido:" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtIdPedido" runat="server" Width="100px"></asp:TextBox>
                                        <a href="#" onclick="openWindow(550, 750, '../Utils/SelPedidoEspelho.aspx?finalizados=1'); return false">
                                            <img src="../Images/Pesquisar.gif" border="0" /></a>
                                        <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="adicionar(); return false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table id="tbPedidos"></table>
                            <asp:HiddenField ID="hdfIdsPedidos" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table id="ambientes_pedido">
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <br />
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar pedido" 
                                OnClientClick="return buscar()" OnClick="btnBuscar_Click" />
                            <asp:HiddenField ID="hdfIdAmbientesPedido" runat="server" />
                        </td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr id="cadastro" runat="server" visible="false">
            <td align="center">
                <asp:DetailsView ID="dtvCompra" runat="server" CellPadding="3" DefaultMode="Insert"
                    AutoGenerateRows="False" DataSourceID="odsCompra" GridLines="None" 
                    Width="600px">
                    <FieldHeaderStyle CssClass="dtvHeader" />
                    <Fields>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornec">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdFornec") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdFornec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlFornecedor" runat="server" 
                                    DataSourceID="odsFornecedor" onchange="alteraFornecedor(this.value)"
                                    DataTextField="Razaosocial" DataValueField="Idfornec" 
                                    SelectedValue='<%# Bind("IdFornec") %>' AppendDataBoundItems="True">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetOrdered"
                                    TypeName="Glass.Data.DAL.FornecedorDAO"></colo:VirtualObjectDataSource>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta" SortExpression="IdConta">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdConta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("IdConta") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                    DataTextField="DescrPlanoGrupo" DataValueField="IdConta" 
                                    SelectedValue='<%# Bind("IdConta") %>' AppendDataBoundItems="True">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContasCompra"
                                    TypeName="Glass.Data.DAL.PlanoContasDAO"></colo:VirtualObjectDataSource>
                            </InsertItemTemplate>
                            <HeaderStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="(produtosPedido)" ShowHeader="False">
                            <InsertItemTemplate>
                                <br />
                                <div style="padding: 4px; margin-top: 4px; text-align: center" class="dtvHeader">
                                    Selecione os produtos que serão usados nessa compra:
                                </div>
                                <br />
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            Disponíveis
                                        </td>
                                        <td>
                                        </td>
                                        <td align="center">
                                            Atuais
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td class="tabela" align="left">
                                            <div class="coluna">
                                                <asp:GridView ID="grdAmbientes" runat="server" AutoGenerateColumns="False" 
                                                    DataSourceID="odsAmbientes" GridLines="None" ShowHeader="False" OnLoad="grdAmbientes_Load">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                                            <EditItemTemplate>
                                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Ambiente") %>'></asp:TextBox>
                                                            </EditItemTemplate>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="imbExibir" runat="server" 
                                                                    
                                                                    onclientclick='<%# "exibirAmbiente(this, \"" + Eval("IdAmbientePedido") + "\"); return false" %>' 
                                                                    ImageUrl="~/Images/mais.gif" />
                                                                <asp:Label ID="Label4" runat="server" Font-Size="10pt" Font-Bold="True" Text="Ambiente:"></asp:Label>
                                                                <asp:Label ID="Label1" runat="server" Font-Size="10pt" Text='<%# Bind("Ambiente") %>'></asp:Label>
                                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" 
                                                                    Value='<%# Eval("IdAmbientePedido") %>' />
                                                                </td>
                                                                </tr>
                                                                <tr id="ambiente_<%# Eval("IdAmbientePedido") %>" style="display: none">
                                                                    <td align="right" style="padding-left: 12px;">
                                                                        <asp:GridView GridLines="None" ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" 
                                                                            CellPadding="3" DataKeyNames="IdProdPed" DataSourceID="odsProdXPedAmbiente" 
                                                                            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                                            EditRowStyle-CssClass="edit">
                                                                            <RowStyle Height="20px" />
                                                                            <Columns>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="imbSelecionar" runat="server" ImageUrl="~/Images/ok.gif" 
                                                                                            OnClientClick='<%#
                                                                                                string.Format("setProduto(this, {0}, \"{1}\", \"{2}\", \"{3}\", {4}, this.parentNode.parentNode.getElementsByTagName(\"input\")[3].checked); return false",
                                                                                                    Eval("IdProdPed"),
                                                                                                    (Eval("CodInterno") != null ? Eval("CodInterno") : string.Empty),
                                                                                                    (Eval("AmbientePedido") != null ? Eval("AmbientePedido").ToString().Replace("\"", "") : string.Empty),
                                                                                                    (Eval("DescrProduto") != null ? Eval("DescrProduto") : string.Empty),
                                                                                                    Eval("QtdeComprar")) %>' />
                                                                                        <input ID="Text1" style="Width: 1px; Visibility: hidden" type="text" />
                                                                                        <asp:HiddenField ID="hdfApenasBeneficiamentos" runat="server" 
                                                                                            Value='<%# Eval("QtdeComprar").ToString() == "0" %>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Wrap="False" />
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="CodInterno" HeaderText="Cód." 
                                                                                    SortExpression="CodInterno">
                                                                                    <ItemStyle Wrap="True" />
                                                                                </asp:BoundField>
                                                                                <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                                                                                    SortExpression="DescrProduto" />
                                                                                <asp:BoundField DataField="QtdeComprar" HeaderText="Qtde" 
                                                                                    SortExpression="Qtde" />
                                                                                <asp:TemplateField HeaderText="Cobrar só benef.?">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkNaoCobrarVidro" runat="server" 
                                                                                            ondatabinding="chkNaoCobrarVidro_DataBinding" />
                                                                                        </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                            </td>
                                                                                            <td colspan="5">
                                                                                                <asp:GridView ID="grdBenef" runat="server" AutoGenerateColumns="False" 
                                                                                                    CellPadding="0" DataKeyNames="IdBenefConfig" DataSourceID="odsBenef" GridLines="None" 
                                                                                                    onprerender="grdBenef_PreRender" ShowHeader="False">
                                                                                                    <Columns>
                                                                                                        <asp:TemplateField ShowHeader="False">
                                                                                                            <ItemTemplate>
                                                                                                                <asp:CheckBox ID="chkBenef" runat="server" Checked="True" 
                                                                                                                    onprerender="chkBenef_PreRender" Text='<%# Eval("DescrBenef") %>' />
                                                                                                                <asp:HiddenField ID="hdfIdBenef" runat="server" 
                                                                                                                    Value='<%# Eval("IdBenefConfig") %>' />
                                                                                                                <asp:HiddenField ID="hdfQtdeBenef" runat="server" />
                                                                                                            </ItemTemplate>
                                                                                                        </asp:TemplateField>
                                                                                                    </Columns>
                                                                                                </asp:GridView>
                                                                                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBenef" runat="server" 
                                                                                                    SelectMethod="GetForCompraPcp" 
                                                                                                    TypeName="Glass.Data.DAL.ProdutoPedidoEspelhoBenefDAO">
                                                                                                    <SelectParameters>
                                                                                                        <asp:ControlParameter ControlID="hdfIdProdPed" Name="idProdPed" 
                                                                                                            PropertyName="Value" Type="UInt32" />
                                                                                                    </SelectParameters>
                                                                                                </colo:VirtualObjectDataSource>
                                                                                                <asp:HiddenField ID="hdfIdProdPed" runat="server" 
                                                                                                    Value='<%# Eval("IdProdPed") %>' />
                                                                                                <asp:HiddenField ID="hdfQtde" runat="server" Value='<%# Eval("Qtde") %>' />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                            <PagerStyle CssClass="pgr" />
                                                                            <HeaderStyle CssClass="dtvHeader" />
                                                                            <EditRowStyle CssClass="edit" />
                                                                            <AlternatingRowStyle BackColor="#E4EFF1" />
                                                                        </asp:GridView>
                                                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPedAmbiente" runat="server" 
                                                                            SelectMethod="GetForCompra" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO">
                                                                            <SelectParameters>
                                                                                <asp:ControlParameter ControlID="txtIdPedido" Name="idPedidoEspelho" 
                                                                                    PropertyName="Text" Type="UInt32" />
                                                                                <asp:ControlParameter ControlID="hdfIdAmbientePedido" Name="idAmbientePedido" 
                                                                                    PropertyName="Value" Type="UInt32" />
                                                                                <asp:Parameter DefaultValue="0" Name="idPedido" Type="UInt32" />
                                                                            </SelectParameters>
                                                                        </colo:VirtualObjectDataSource>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbientes" runat="server" 
                                                    SelectMethod="ObterParaCompraPcpPorPedidos" 
                                                    TypeName="Glass.Data.DAL.AmbientePedidoEspelhoDAO">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="hdfIdsPedidos" Name="idsPedidos" PropertyName="Value" Type="String" />
                                                        <asp:ControlParameter ControlID="hdfIdAmbientesPedido" Name="idsAmbientes" PropertyName="Value" Type="String" />
                                                    </SelectParameters>
                                                </colo:VirtualObjectDataSource>
                                                <asp:GridView ID="grdProdutosPedido" runat="server" 
                                                    AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" CellPadding="3" 
                                                    CssClass="gridStyle" DataKeyNames="IdProdPed" DataSourceID="odsProdXPed" 
                                                    EditRowStyle-CssClass="edit" GridLines="None"
                                                    PagerStyle-CssClass="pgr">
                                                    <RowStyle Height="20px" />
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="imbSelecionar0" runat="server" ImageUrl="~/Images/ok.gif" 
                                                                    OnClientClick='<%#
                                                                        string.Format("setProduto(this, {0}, \"{1}\", \"{2}\", \"{3}\", {4}, this.parentNode.parentNode.getElementsByTagName(\"input\")[3].checked); return false",
                                                                            Eval("IdProdPed"),
                                                                            (Eval("CodInterno") != null ? Eval("CodInterno") : string.Empty),
                                                                            (Eval("AmbientePedido") != null ? Eval("AmbientePedido").ToString().Replace("\"", "") : string.Empty),
                                                                            (Eval("DescrProduto") != null ? Eval("DescrProduto") : string.Empty),
                                                                            Eval("QtdeComprar")) %>' />
                                                                <input ID="Text2" style="Width: 1px; Visibility: hidden" type="text" />
                                                                <asp:HiddenField ID="hdfApenasBeneficiamentos" runat="server" 
                                                                    Value='<%# Eval("QtdeComprar").ToString() == "0" %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Wrap="False" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." 
                                                            SortExpression="CodInterno">
                                                            <ItemStyle Wrap="True" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                                                        <asp:BoundField DataField="QtdeComprar" HeaderText="Qtde" SortExpression="Qtde" />
                                                        <asp:TemplateField HeaderText="Cobrar só benef.?">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkNaoCobrarVidro" runat="server" 
                                                                    ondatabinding="chkNaoCobrarVidro_DataBinding" />
                                                                </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                    <td colspan="4">
                                                                        <asp:GridView ID="grdBenef" runat="server" AutoGenerateColumns="False" 
                                                                            CellPadding="0" DataKeyNames="IdBenefConfig" DataSourceID="odsBenef" 
                                                                            GridLines="None" onprerender="grdBenef_PreRender" ShowHeader="False">
                                                                            <Columns>
                                                                                <asp:TemplateField ShowHeader="False">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkBenef0" runat="server" Checked="True" 
                                                                                            onprerender="chkBenef_PreRender" Text='<%# Eval("DescrBenef") %>' />
                                                                                        <asp:HiddenField ID="hdfIdBenef0" runat="server" 
                                                                                            Value='<%# Eval("IdBenefConfig") %>' />
                                                                                        <asp:HiddenField ID="hdfQtdeBenef0" runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBenef" runat="server" 
                                                                            SelectMethod="GetForCompraPcp" 
                                                                            TypeName="Glass.Data.DAL.ProdutoPedidoEspelhoBenefDAO">
                                                                            <SelectParameters>
                                                                                <asp:ControlParameter ControlID="hdfIdProdPed" Name="idProdPed" 
                                                                                    PropertyName="Value" Type="UInt32" />
                                                                            </SelectParameters>
                                                                        </colo:VirtualObjectDataSource>
                                                                        <asp:HiddenField ID="hdfIdProdPed" runat="server" 
                                                                            Value='<%# Eval("IdProdPed") %>' />
                                                                        <asp:HiddenField ID="hdfQtde" runat="server" Value='<%# Eval("Qtde") %>' />
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" />
                                                    <HeaderStyle CssClass="dtvHeader" />
                                                    <EditRowStyle CssClass="edit" />
                                                    <AlternatingRowStyle BackColor="#E4EFF1" />
                                                </asp:GridView>
                                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" 
                                                    SelectMethod="GetForCompra" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="hdfIdsPedidos" Name="idsPedidosEspelho" PropertyName="Value" Type="String" />
                                                    </SelectParameters>
                                                </colo:VirtualObjectDataSource>
                                            </div>
                                        </td>
                                        <td style="width: 20px">
                                        </td>
                                        <td class="tabela" align="right">
                                            <div class="coluna">
                                                <table id="tbProdutos" cellpadding="3" cellspacing="0">
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="(botões)" ShowHeader="False">
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" 
                                    onclientclick="if (!validar()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    OnClick="btnCancelar_Click" Text="Cancelar" />
                                <asp:HiddenField ID="hdfIdPedidoEspelho" runat="server" OnLoad="hdfIdPedidoEspelho_Load"
                                    Value='<%# Bind("IdPedidoEspelho") %>' />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCompra" runat="server" DataObjectTypeName="Glass.Data.Model.Compra"
                    InsertMethod="Insert" SelectMethod="GetElementByPrimaryKey" 
                    TypeName="Glass.Data.DAL.CompraDAO" oninserting="odsCompra_Inserting" 
                    oninserted="odsCompra_Inserted">
                    <SelectParameters>
                        <asp:Parameter Name="key" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfDadosProdutos" runat="server" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        buscarAmbientes();
    </script>
</asp:Content>
