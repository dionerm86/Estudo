<%@ Page Title="Imprimir Pedido PCP" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaPedidoPcp.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPedidoPcp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">

    function openRpt() {
        // Se a empresa não seleciona ambientes para impressão, utiliza o id digitado no textbox para imprimir
        if (FindControl("btnVisualizar", "input") != null)
            FindControl("hdfNumPedido", "input").value = FindControl("txtNumPedido", "input").value;
    
        var idPedido = FindControl("hdfNumPedido", "input").value;
    
        // Verifica se o número do pedido foi informado
        if (idPedido == "") {
            alert("Informe o número do pedido.");
            return false;
        }
        
        var tipo = FindControl("rblTipo", "table");
        if (tipo != null)
        {
            tipo = tipo.getElementsByTagName("input");
            for (i = 0; i < tipo.length; i++)
                if (tipo[i].checked)
                {
                    tipo = tipo[i].value;
                    break;
                }
        }
        else
            tipo == 0;
        
        var grupos = new Array();
        var produtos = new Array();
        
        if (tipo == 1)
        {
            // Verifica os grupos selecionados
            var cblGrupos = FindControl("cblGrupoProd", "table");
            if (cblGrupos != null)
            {
                var inputs = cblGrupos.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++)
                    if (inputs[i].type.toLowerCase() == "checkbox" && inputs[i].checked)
                        grupos.push(inputs[i].parentNode.getAttribute("valor"));
                
                if (grupos.length == 0)
                {
                    alert("Informe ao menos um grupo de produto a ser exibido na visualização da impressão.");
                    return false;
                }
            }
        }
        else if (tipo == 2)
        {
            // Verifica os ambientes selecionados
            var grdAmbientes = FindControl("grdAmbientes", "table");
            if (grdAmbientes != null)
            {
                var inputs = grdAmbientes.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++)
                {
                    if (inputs[i].type.toLowerCase() != "checkbox")
                        continue;
                        
                    var isAmbiente = inputs[i].parentNode.getAttribute("ambiente") == "true";
                    if (!isAmbiente && inputs[i].checked) {
                        var vetIdProd = inputs[i].id.split('_');
                        debugger;
                        var idProdPed = FindControl(vetIdProd[0] + "_" + vetIdProd[1] + "_" + vetIdProd[2] + "_" + vetIdProd[3] + "_" + vetIdProd[4] + "_" + vetIdProd[5] + "_" + vetIdProd[6] + "_" + vetIdProd[7] + "_" + "hdfIdProdPed", "input").value;
                        var qtde = FindControl(vetIdProd[0] + "_" + vetIdProd[1] + "_" + vetIdProd[2] + "_" + vetIdProd[3] + "_" + vetIdProd[4] + "_" + vetIdProd[5] + "_" + vetIdProd[6] + "_" + vetIdProd[7] + "_" + "txtQtdeComprar", "input").value;
                        var qtdeMax = FindControl(vetIdProd[0] + "_" + vetIdProd[1] + "_" + vetIdProd[2] + "_" + vetIdProd[3] + "_" + vetIdProd[4] + "_" + vetIdProd[5] + "_" + vetIdProd[6] + "_" + vetIdProd[7] + "_" + "hdfQtdeMaxComprar", "input").value;
                        if(qtde != qtdeMax)
                            idProdPed = idProdPed + "_" + qtde;
                            
                        produtos.push(idProdPed);
                    }
                }
                
                if (produtos.length == 0)
                {
                    alert("Selecione pelo menos um produto para ser visualizado na impressão.");
                    return false;
                }
            }
        }

        // Verifica se o pedido existe e pode ser impresso
        var retorno = ListaPedidoPcp.VerificaPedido(idPedido).value.split('\t');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            return false;
        }

        var agruparProdutos = FindControl("chkAgruparProdutos", "input");
        agruparProdutos = agruparProdutos != null ? agruparProdutos.checked : false;

        openWindow(600, 800, "RelBase.aspx?rel=PedidoPcp&idPedido=" + idPedido + "&grupos=" + grupos.toString() + 
            "&produtos=" + produtos.toString() + "&init=1&agruparProdutos=" + agruparProdutos);

        redirectUrl(window.location.href);

        return false;
    }
    
    function mudaCampo(campo)
    {
        if (FindControl("btnBuscar", "input") == null)
            FindControl("hdfNumPedido", "input").value = campo.value;
    }

    function marcaDesmarcaGrupos(chk) {
        var checked = chk.checked;
        
        var vetChk = document.getElementsByTagName("input");
        for (i = 0; i < vetChk.length; i++)
            if (vetChk[i].type == "checkbox" && vetChk[i].id.toString().indexOf("GrupoProd") > 0)
                vetChk[i].checked = checked;
    }
    
    // Função executada para exibir/esconder os produtos de um ambiente
    function exibirAmbiente(check, id)
    {
        var linha = document.getElementById(id);
        linha.style.display = check.checked ? "" : "none";
        
        var inputs = linha.getElementsByTagName("input");
        for (i = 0; i < inputs.length; i++)
            if (inputs[i].type.toLowerCase() == "checkbox")
                inputs[i].checked = check.checked;
    }
    
    function alteraTipo(controle)
    {
        if (controle == null)
            return;
        
        var opcoes = controle.getElementsByTagName("input");
        for (i = 0; i < opcoes.length; i++)
            if (opcoes[i].checked)
            {
                document.getElementById("<%= grupos.ClientID %>").style.display = opcoes[i].value == "1" ? "" : "none";
                document.getElementById("<%= produtos.ClientID %>").style.display = opcoes[i].value == "2" ? "" : "none";
                break;
            }
    }
    
    function verificaQntdeComprar(txtQntde)
    {
        var qtde = parseInt(txtQntde.value, 10);
        var qtdeMax = parseInt(FindControl("hdfQtdeMaxComprar", "input", txtQntde.parentNode).value, 10);
        
        if(qtde > qtdeMax)
        {
             txtQntde.value = qtdeMax;
             alert("Quantidade máxima do produto excedida.");
        }
        
        if(qtde < 1)
        {
             txtQntde.value = qtdeMax;
             alert("Quantidade do produto não pode ser 0.");
        }   
        
       
        
        return false;
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table cellpadding="1" cellspacing="2">
                    <tr>
                        <td>
                            Número do Pedido:&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onchange="mudaCampo(this)"
                                onkeypress="return soNumeros(event, true, true);" runat="server"
                                onkeydown="if (isEnter(event)) { if (FindControl('btnVisualizar', 'input') != null) cOnClick('btnVisualizar', 'input'); else cOnClick('btnBuscar', 'input'); }" Width="100px"></asp:TextBox>
                            <asp:HiddenField ID="hdfNumPedido" runat="server" />
                        </td>
                        <td>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                                onclick="btnBuscar_Click" />
                            <asp:Button ID="btnVisualizar" runat="server" onclientclick="return openRpt();" 
                                Text="Visualizar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="linhaTipos" runat="server" visible="false">
            <td align="center">
                <table>
                    <tr>
                        <td style="font-size: 12px; font-weight: bold">
                            <asp:RadioButtonList ID="rblTipo" runat="server" 
                                RepeatDirection="Horizontal" CellSpacing="4" onclick="alteraTipo(this)">
                                <asp:ListItem Value="1" Selected="True">Grupos</asp:ListItem>
                                <asp:ListItem Value="2">Produtos</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparProdutos" runat="server" Text="Agrupar Produtos" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="grupos" runat="server" visible="false">
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            &nbsp;<asp:CheckBox ID="chkMarcarDesmarcar" runat="server" Text="Marcar/desmarcar todos" 
                                onclick="marcaDesmarcaGrupos(this);" Checked="True"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBoxList ID="cblGrupoProd" runat="server" DataSourceID="odsGrupoProd" 
                                DataTextField="Descricao" DataValueField="IdGrupoProd" RepeatColumns="3" 
                                ondatabound="cblGrupoProd_DataBound">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr id="produtos" runat="server" visible="false">
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAmbientes" runat="server" AutoGenerateColumns="False" 
                    DataSourceID="odsAmbientesPedido" ShowHeader="False" CssClass="gridStyle">
                    <Columns>
                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Ambiente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAmbiente" runat="server" Text='<%# "Ambiente: " + Eval("Ambiente") %>'
                                    onclick='<%# "exibirAmbiente(this, \"ambiente_" + Eval("IdAmbientePedido") + "\");" %>'
                                    ondatabinding="chkAmbiente_DataBinding" />
                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" 
                                    Value='<%# Eval("IdAmbientePedido") %>' />
                                <div id="ambiente_<%# Eval("IdAmbientePedido") %>" style="display: none; text-align: right">
                                    <asp:GridView GridLines="None" ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" 
                                        CellPadding="3" DataKeyNames="IdProdPed" DataSourceID="odsProdXPed" 
                                        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                        EditRowStyle-CssClass="edit" Width="90%"
                                        onrowdatabound="grdProdutosPedido_RowDataBound">
                                        <RowStyle Height="20px" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkMarcarProduto" runat="server" 
                                                        ondatabinding="chkMarcarProduto_DataBinding" />
                                                    <asp:HiddenField ID="hdfIdProdPed" runat="server" 
                                                        Value='<%# Eval("IdProdPed") %>' />
                                                    <asp:HiddenField ID="hdfComprado" runat="server" 
                                                        Value='<%# Eval("Comprado") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Wrap="False" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CodInterno" HeaderText="Cód." 
                                                SortExpression="CodInterno">
                                                <ItemStyle Wrap="True" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                                                SortExpression="DescrProduto" />
                                                <asp:TemplateField HeaderText="Qtde">
                                                <ItemTemplate>
                                                <asp:HiddenField ID="hdfQtdeMaxComprar" runat="server"
                                                    value='<%# Eval("QtdeComprar") %>' />
                                                    <asp:TextBox ID="txtQtdeComprar" runat="server" 
                                                    Text='<%# Eval("QtdeComprar") %>' Width="40"
                                                    onblur="verificaQntdeComprar(this); return false;"
                                                    onkeypress="if (isEnter(event)) verificaQntdeComprar(this); return soNumeros(event, true, true); "/>
                                                </ItemTemplate>
                                                </asp:TemplateField>
                                            <asp:BoundField DataField="AlturaLista" HeaderText="Altura" 
                                                SortExpression="AlturaLista" />
                                            <asp:BoundField DataField="Largura" HeaderText="Largura" 
                                                SortExpression="Largura" />
                                        </Columns>
                                        <PagerStyle CssClass="pgr" />
                                        <EditRowStyle CssClass="edit" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" 
                                        SelectMethod="GetByAmbiente" 
                                        TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO">
                                        <SelectParameters>
                                            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" 
                                                Type="UInt32" />
                                            <asp:ControlParameter ControlID="hdfIdAmbientePedido" Name="idAmbientePedido" 
                                                PropertyName="Value" Type="UInt32" />
                                            <asp:Parameter DefaultValue="true" Name="forPcp" Type="Boolean" />
                                        </SelectParameters>
                                    </colo:VirtualObjectDataSource>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <asp:Label ID="Label1" runat="server" 
                    Text="Os produtos exibidos em azul já foram comprados." ForeColor="Blue"></asp:Label>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbientesPedido" runat="server" 
                    SelectMethod="GetByPedido" TypeName="Glass.Data.DAL.AmbientePedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:Parameter DefaultValue="true" Name="forPcp" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr id="separador" runat="server" visible="false">
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr id="observacoes" runat="server" visible="false">
            <td align="center">
                Observações:<br />
                <asp:TextBox ID="txtObservacoes" runat="server" Rows="4" TextMode="MultiLine" 
                    Width="450px"></asp:TextBox>
            </td>
        </tr>
        <tr id="visualizar" runat="server" visible="false">
            <td align="center">
                <br />
                <asp:Button ID="btnVisualizar1" runat="server" onclientclick="return openRpt();" 
                    Text="Visualizar" />
            </td>
        </tr>
    </table>
    <script>
        FindControl("txtNumPedido", "input").focus();
        alteraTipo(document.getElementById("<%= rblTipo.ClientID %>"));
    </script>
</asp:Content>

