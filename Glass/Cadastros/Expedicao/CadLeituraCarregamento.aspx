<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadLeituraCarregamento.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Expedicao.CadLeituraCarregamento" %>

<%@ Register Src="../../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup"
    TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Leitura de Carregamento</title>

    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Carregamento.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/gridView.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/m2br.dialog.producao.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jlinq/jlinq.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/m2br.dialog.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Carregamento.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

</head>
<body>
    <!-- Chamado 12705. A tela de consulta de produção estava ficando muito pequena, pois, como a tela de carregamento não tinha dados
        visíveis, a tela ficava pequena e o popup da consulta de produção preenchia um pequeno espaço. Por isso, alteramos o estilo do
        formulário de código "form1" para preencher 100% da tela. -->
    <form id="form1" runat="server" style="width: 100%; height: 100%;">
        <div class="boxTitulo">
            <div class="boxAzul">
                <div>
                    <div class="boxLateral" style="width: 34%; padding-top: 5px;">
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="LinkButton2" runat="server" ToolTip="Carregamento" Text="Carregamento"
                                        OnClientClick="showCarregamento();">
                                    </asp:LinkButton>
                                </td>
                                <td>&nbsp;
                                </td>
                                <td>
                                    <asp:LinkButton ID="LinkBalcao" runat="server" ToolTip="Exp. Balcão" Text="Exp. Balcão"
                                        OnClick="LinkBalcao_Click" >
                                    </asp:LinkButton>
                                </td>
                                <td>&nbsp;
                                </td>
                                <td>
                                    <asp:LinkButton ID="LinkButton3" runat="server" ToolTip="Consultar Produção" Text="Consultar Produção"
                                        OnClientClick="return showConsultaProducao(false);">
                                    </asp:LinkButton>
                                </td>
                                <td>&nbsp;
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkMensagensNaoLidas" runat="server" ToolTip="Mensagens Recebidas"
                                        Visible="false" OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')">
                                            <img src='<%= ResolveUrl("~/Images/mail_received.png") %>' border="0" /></asp:LinkButton>
                                    <asp:LinkButton ID="lnkMensagens" runat="server" ToolTip="Mensagens Recebidas" Visible="False"
                                        OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')">
                                            <img src='<%= ResolveUrl("~/Images/mail.png") %>' border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="boxLateral" style="width: 33%; text-align: center;">
                        <asp:Label ID="Label1" runat="server" Text="Carregamento" CssClass="tituloAzul"></asp:Label>
                    </div>
                    <div class="boxLateral" style="width: 33%; text-align: right; padding-top: 6px;">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkLgout_Click" CausesValidation="False">
                                             <img alt="" border="0" src="../../Images/Logout.png" /></asp:LinkButton>
                        &nbsp;
                    </div>
                </div>
            </div>
        </div>
        <div id="boxCarregamento">
            <div class="boxEtiqueta">
                <div class="boxLateral" style="width: 37%">
                    <div class="bordaAzul">
                        <table>
                            <tr>
                                <td style="font-size: x-small">
                                    <asp:Label ID="Label2" runat="server" Text="Carregamento: " CssClass="subtituloAzul"></asp:Label>
                                </td>
                                <td style="font-size: x-small">
                                    <asp:Label ID="Label3" runat="server" Text="Etiqueta: " CssClass="subtituloAzul"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCodCarregamento" runat="server" Font-Size="x-Large" Width="190px"
                                        ForeColor="Green" onkeypress="if (isEnter(event)) return carregaCarregamento();"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCodEtiqueta" runat="server" Font-Size="x-Large" Width="190px"
                                        onkeypress="if (isEnter(event)) return efetuaLeitura();"></asp:TextBox>
                                    <img runat="server" src="~/Images/Help.gif" title="Para leituras de faixas de etiquteta utilizar após a barra da etiqueta (número inicial da faixa = número final da faixa).
Exemplo: Etiquetas do intervalo 1111-1.1/10 até 1111-1.10/10 podem ser lidas em faixas, digitando no campo da etiqueta 1111-1.1/2=6
as etiquetas referentes à posição 1 serão lidas do item 2 até o item 6, utilizando 1111-1.1/7=9 serão lidas dos itens 7 até 9." />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="boxDivisorLateral">
                </div>
                <div class="boxLateral" style="width: 62.9%;">
                    <div class="bordaAzul">
                        <table>
                            <tr align="center">
                                <td>
                                    <table align="center">
                                        <tr align="center">
                                            <td>
                                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/refresh.png"
                                                    OnClientClick="return AtualizaPagina();" OnClick="ImageButton2_Click"
                                                    ToolTip="Atualizar Pagina" />
                                            </td>
                                        </tr>
                                        <tr align="center">
                                            <td>
                                                <asp:Label ID="Label5" runat="server" Text="Atualizar" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <table align="center">
                                        <tr align="center">
                                            <td>
                                                <asp:Label ID="Label10" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                    onblur="getCli(this);"></asp:TextBox>
                                                <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label11" runat="server" Text="Cód OC" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtIdOc" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text="Cód Pedido" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtIdPedido" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                    OnClick="imgPesq_Click" />
                                            </td>

                                        </tr>
                                    </table>
                                    <table align="center">
                                        <tr align="center">
                                            <td>
                                                <asp:Label ID="Label46" runat="server" Text="Num. Etq." ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEtqFiltro" runat="server" Width="90px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                    OnClick="imgPesq_Click" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label47" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAltura" runat="server" Width="50px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                    OnClick="imgPesq_Click" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label48" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLargura" runat="server" Width="50px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                    OnClick="imgPesq_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="tbClienteExterno" runat="server" align="center">
                                        <tr align="center">
                                            <td>
                                                <asp:Label ID="Label8" runat="server" Text="Cliente Externo" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNumCliExterno" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                                <asp:TextBox ID="txtNomeClienteExterno" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label49" runat="server" Text="Pedido Externo" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtIdPedidoExterno" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table align="center">
                                        <tr align="center">
                                            <td>
                                                <asp:Label ID="Label14" runat="server" Text="Visualizar" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <sync:CheckBoxListDropDown ID="drpVisualizar" runat="server" Width="150px">
                                                    <asp:ListItem Value="1" Selected="True">Carregados</asp:ListItem>
                                                    <asp:ListItem Value="2" Selected="True">Não Carregados</asp:ListItem>
                                                </sync:CheckBoxListDropDown>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkAtuAutomaticamente" runat="server" Text="Atualizar página automaticamente" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr align="center">
                                            <td><asp:LinkButton runat="server" ID="lnkPedProntoSemCarregamento" OnClientClick="return exibirPedidosSemCarregamento();">Pedidos prontos sem carregamento</asp:LinkButton> </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <asp:DetailsView runat="server" ID="dtvItensCarregamento" DataSourceID="odsCarregamento"
                AutoGenerateRows="False" GridLines="None" HorizontalAlign="Center" Width="100%"
                CellPadding="0" CellSpacing="0" OnDataBound="dtvItensCarregamento_DataBound">
                <Fields>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div style="margin-top: 4px">
                                        </div>
                                        <div class="boxItens">
                                            <div class="boxLateral" style="width: 33.3%;">
                                                <div class="bordaAzul">
                                                    <table width="70%" align="center">
                                                        <tr>
                                                            <td align="center" colspan="2">
                                                                <asp:Label ID="Label26" runat="server" Text="Total Carregado" CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label6" runat="server" Text="Peças de Vidro: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("PecasCarregadas") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Green"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label8" runat="server" Text="Volumes: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("VolumesCarregados") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Green"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label12" runat="server" Text="Peso: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("PesoCarregado") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Green"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <div class="boxDivisorLateral">
                                                &nbsp;
                                            </div>
                                            <div class="boxLateral" style="width: 33.3%;">
                                                <div class="bordaAzul">
                                                    <table width="70%" align="center">
                                                        <tr>
                                                            <td align="center" colspan="2">
                                                                <asp:Label ID="Label17" runat="server" Text="Total Faltante" CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label33" runat="server" Text="Peças de Vidro: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label34" runat="server" Text='<%# Eval("PecasPendentes") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Red"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label35" runat="server" Text="Volumes: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label36" runat="server" Text='<%# Eval("VolumesPendentes") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Red"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label37" runat="server" Text="Peso: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label38" runat="server" Text='<%# Eval("PesoPendente") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Red"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <div class="boxDivisorLateral">
                                                &nbsp;
                                            </div>
                                            <div class="boxLateral" style="width: 33.2%;">
                                                <div class="bordaAzul">
                                                    <table width="70%" align="center">
                                                        <tr>
                                                            <td align="center" colspan="2">
                                                                <asp:Label ID="Label39" runat="server" Text="Total do Carregamento" CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label40" runat="server" Text="Peças de Vidro: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label41" runat="server" Text='<%# Eval("TotalPecas") %>' Font-Bold="True"
                                                                    Font-Size="Large"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label42" runat="server" Text="Volumes: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label43" runat="server" Text='<%# Eval("TotalVolumes") %>' Font-Bold="True"
                                                                    Font-Size="Large"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label44" runat="server" Text="Peso: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label45" runat="server" Text='<%# Eval("PesoTotal") %>' Font-Bold="True"
                                                                    Font-Size="Large"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <div class="boxItens">
                            <div class="boxAzul">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="grvProdutos" Width="100%" AutoGenerateColumns="False"
                                                DataSourceID="odsPecasCarregamento" CssClass="gridStyle" GridLines="None" OnPreRender="grvProdutos_PreRender"
                                                OnRowDataBound="grvProdutos_RowDataBound" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                EditRowStyle-CssClass="edit" AllowPaging="true" PageSize="8" AllowSorting="true">
                                                <PagerSettings PageButtonCount="20" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Estornar" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSelEstorno" runat="server" Visible='<%# Eval("Carregado") %>' />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hdfIdItemCarregamento" runat="server" Value='<%# Eval("IdItemCarregamento") %>' />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cliente" SortExpression="IdCliente">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label27" runat="server" Text='<%# Eval("IdNomeCliente") + (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("PedidoImportado")) ?
                                                            " ("+Eval("IdClienteExterno") + " - " + Eval("ClienteExterno") +")" : "") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="OC" SortExpression="IdOC">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label32" runat="server" Text='<%# Eval("IdOc") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label15" runat="server" Text='<%# Eval("IdPedido") + " (" + Eval("PedidoEtiqueta") + ")" +
                                                                    (Eval("IdPedidoRevenda") != null ? "(Rev. " + Eval("IdPedidoRevenda") + ")" : "").ToString() %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../CadFotos.aspx?id=<%# Eval("IdPedido") %>&amp;tipo=pedido&#039;); return false;'>
                                                                <img border="0px" src="../../Images/Clipe.gif" /></a></asp:PlaceHolder>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Pedido Cli." SortExpression="PedCli">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label55" runat="server" Text='<%# Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("PedidoImportado")) ?
                                                            Eval("IdPedidoExterno") : Eval("PedCli") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item" SortExpression="CodInterno">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label16" runat="server" Text='<%# Eval("CodInternoDescProd") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label18" runat="server" Text='<%# Eval("Altura") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label19" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="M²" SortExpression="M2">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label20" runat="server" Text='<%# Eval("M2") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>' />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Etiqueta" SortExpression="Etiqueta">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("Etiqueta") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Leitura" SortExpression="DataLeitura">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label29" runat="server" Text='<%# Eval("DataLeituraStr") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Funcionário" SortExpression="IdFuncLeitura">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label30" runat="server" Text='<%# Eval("NomeFuncLeitura") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Setores Pendentes">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("SetoresPendentes") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("LogEstornoVisible") %>'>
                                                                <a href="#" onclick='exibirEstornos(<%# Eval("IdItemCarregamento") %>); return false;'>
                                                                    <img alt="" border="0" src="../../Images/blocodenotas.png" title="Ver estornos" /></a>
                                                            </asp:PlaceHolder>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td align="left" valign="top" width="10%">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="btnEstorno" runat="server" Text="Estornar item" Height="25px" Width="150px"
                                                                        UseSubmitBehavior="false" OnClick="btnEstorno_Click" /><br />
                                                                    <br />
                                                                    <asp:Button ID="btnEstornarTodos" runat="server"
                                                                        Text="Estornar todos os itens" Height="25px" Width="150px"
                                                                        UseSubmitBehavior="false" OnClick="btnEstornarTodos_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:GridView runat="server" ID="gvrVolumes" AutoGenerateColumns="False" DataSourceID="odsVolumesCarregamento"
                                                            CssClass="gridStyle" GridLines="None" OnPreRender="gvrVolumes_PreRender" OnRowDataBound="gvrVolumes_RowDataBound"
                                                            Width="100%" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                                            AllowPaging="true" PageSize="3" AllowSorting="true">
                                                            <PagerSettings PageButtonCount="20" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Estornar" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelEstorno" runat="server" Visible='<%# Eval("Carregado") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdfIdItemCarregamento" runat="server" Value='<%# Eval("IdItemCarregamento") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Cliente" SortExpression="IdCliente">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label27" runat="server" Text='<%# Eval("IdNomeCliente") + (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("PedidoImportado")) ?
                                                                            " ("+Eval("IdClienteExterno") + " - " + Eval("ClienteExterno") +")" : "") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="OC" SortExpression="IdOC">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label32" runat="server" Text='<%# Eval("IdOc") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label22" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Pedido Cli." SortExpression="PedCli">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label55" runat="server" Text='<%# Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("PedidoImportado")) ?
                                                                            Eval("IdPedidoExterno") : Eval("PedCli") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Volume" SortExpression="IdVolume">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label23" runat="server" Text='<%# Eval("IdVolume") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Data de Fechamento" SortExpression="DataFechamento">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label24" runat="server" Text='<%# Eval("DataFechamento") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Etiqueta" SortExpression="idVolume">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label25" runat="server" Text='<%# Eval("EtiquetaVolume") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Leitura" SortExpression="DataLeitura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label21" runat="server" Text='<%# Eval("DataLeituraStr") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Funcionário" SortExpression="idFuncLeitura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label31" runat="server" Text='<%# Eval("NomeFuncLeitura") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("LogEstornoVisible") %>'>
                                                                            <a href="#" onclick='exibirEstornos(<%# Eval("IdItemCarregamento") %>); return false;'>
                                                                                <img alt="" border="0" src="../../Images/blocodenotas.png" title="Ver estornos" /></a>
                                                                        </asp:PlaceHolder>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divModuloSistema" style="display: none;">
            <div class="boxModuloSistema">
                <iframe runat="server" id="frameModuloSistema" frameborder="0" style="display: none" />
            </div>
        </div>
        <div style="display: none;">
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCarregamento" runat="server"
                TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo" SelectMethod="GetInfoCarregamentoForExpedicao">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodCarregamento" PropertyName="Text" Name="idCarregamento"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdOc" PropertyName="Text" Name="idOC" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNumCli" PropertyName="Text" Name="idCliente"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" PropertyName="Text" Name="nomeCliente"
                        Type="string" />
                    <asp:ControlParameter ControlID="txtEtqFiltro" PropertyName="Text" Name="numEtiqueta"
                        Type="string" />
                    <asp:ControlParameter ControlID="txtLargura" PropertyName="Text" Name="largura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtAltura" PropertyName="Text" Name="altura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtIdPedidoExterno" Name="idPedidoExterno" PropertyName="Text" Type="UInt32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecasCarregamento" runat="server"
                DataObjectTypeName="Glass.Data.Model.ItemCarregamento" EnablePaging="True" MaximumRowsParameterName="pageSize"
                SelectCountMethod="GetItensCarregamentoForExpedicaoCount" SelectMethod="GetItensCarregamentoForExpedicao"
                SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodCarregamento" PropertyName="Text" Name="idCarregamento"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdOc" PropertyName="Text" Name="idOC" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNumCli" PropertyName="Text" Name="idCliente"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" PropertyName="Text" Name="nomeCliente"
                        Type="string" />
                    <asp:ControlParameter ControlID="drpVisualizar" PropertyName="SelectedValue" Name="filtro"
                        Type="String" />
                    <asp:Parameter Name="volume" DefaultValue="false" DbType="Boolean" />
                    <asp:ControlParameter ControlID="txtEtqFiltro" PropertyName="Text" Name="numEtiqueta"
                        Type="string" />
                    <asp:ControlParameter ControlID="txtLargura" PropertyName="Text" Name="largura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtAltura" PropertyName="Text" Name="altura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtIdPedidoExterno" Name="idPedidoExterno" PropertyName="Text" Type="UInt32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVolumesCarregamento" runat="server"
                DataObjectTypeName="Glass.Data.Model.ItemCarregamento" EnablePaging="True" MaximumRowsParameterName="pageSize"
                SelectCountMethod="GetItensCarregamentoForExpedicaoCount" SelectMethod="GetItensCarregamentoForExpedicao"
                SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodCarregamento" PropertyName="Text" Name="idCarregamento"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdOc" PropertyName="Text" Name="idOC" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNumCli" PropertyName="Text" Name="idCliente"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" PropertyName="Text" Name="nomeCliente"
                        Type="string" />
                    <asp:ControlParameter ControlID="drpVisualizar" PropertyName="SelectedValue" Name="filtro"
                        Type="String" />
                    <asp:Parameter Name="volume" DefaultValue="true" DbType="Boolean" />
                    <asp:ControlParameter ControlID="txtEtqFiltro" PropertyName="Text" Name="numEtiqueta"
                        Type="string" />
                    <asp:ControlParameter ControlID="txtLargura" PropertyName="Text" Name="largura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtAltura" PropertyName="Text" Name="altura"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtIdPedidoExterno" Name="idPedidoExterno" PropertyName="Text" Type="UInt32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <asp:HiddenField ID="hdfTempoLogin" runat="server" />
            <asp:HiddenField ID="hdfCorTela" runat="server" />
            <audio id="sndOk" src="../../Images/ok.wav"></audio>
            <audio id="sndError" src="../../Images/error.wav"></audio>
            <asp:HiddenField ID="hdfFunc" runat="server" />
        </div>
    </form>

    <script type="text/javascript">

        function setFocus() {
            if ($('#txtCodCarregamento').val() == "")
                $('#txtCodCarregamento').focus();
            else
                $('#txtCodEtiqueta').focus();
        }


        setFocus();

        var alertaPadrao = function (titulo, msg, tipo, altura, largura) {
            $('body').append('<a href="#" id="alerta-padrao"></a>');
            $('#alerta-padrao').m2brDialog({
                draggable: true,
                texto: msg,
                tipo: tipo,
                titulo: titulo,
                altura: altura,
                largura: largura,
                botoes: {
                    1: {
                        label: 'Fechar',
                        tipo: 'fechar'
                    }

                },
                unloadCallback: function () {
                    setFocus();
                }
            });
            $('#alerta-padrao')
            .click()
            .remove();
        };

        // Conta os minutos que o usuário está logado
        var countMin = 0;

        function manterLogado() {
            try {
                countMin += 2;

                var tempoLogin = FindControl("hdfTempoLogin", "input").value;

                // Verifica se o tempo de login está dentro do tempo máximo permitido para este setor
                if (tempoLogin >= countMin)
                    MetodosAjax.ManterLogado();
                else {
                    MetodosAjax.Logout();
                    redirectUrl(window.location.href);
                }
            }
            catch (err) {
                alert(err);
            }
        }


        try {
            // Se o tempo de login for igual a 0, não precisa contar quanto tempo o usuário está logado.
            if (FindControl("hdfTempoLogin", "input").value == 0)
                setInterval("MetodosAjax.ManterLogado()", 600000);
            else
                setInterval("manterLogado()", 120000);

        }
        catch (err) {
            alert(err);
        }

    </script>

</body>
</html>
