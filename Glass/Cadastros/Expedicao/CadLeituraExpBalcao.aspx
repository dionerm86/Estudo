<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadLeituraExpBalcao.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Expedicao.CadLeituraExpBalcao" %>

<%@ Register Src="../../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup"
    TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Exp. Balcão</title>

    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Carregamento.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/gridView.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/m2br.dialog.producao.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jlinq/jlinq.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/m2br.dialog.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/ExpBalcao.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="boxTitulo">
            <div class="boxAzul">
                <div>
                    <div class="boxLateral" style="width: 34%; padding-top: 5px;">
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="LinkButton2" runat="server" ToolTip="Exp. Balcão" Text="Exp. Balcão"
                                        OnClientClick="showCarregamento();"> 
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
                                        Visible="false" OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')"> <img src='<%= ResolveUrl("~/Images/mail_received.png") %>' border="0" /></asp:LinkButton>
                                    <asp:LinkButton ID="lnkMensagens" runat="server" ToolTip="Mensagens Recebidas" Visible="False"
                                        OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')"> <img src='<%= ResolveUrl("~/Images/mail.png") %>' border="0" /></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="boxLateral" style="width: 33%; text-align: center;">
                        <asp:Label ID="Label1" runat="server" Text="Exp. Balcão" CssClass="tituloAzul"></asp:Label>
                    </div>
                    <div class="boxLateral" style="width: 33%; text-align: right; padding-top: 6px;">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkLgout_Click" CausesValidation="False"> <img alt="" border="0" src="../../Images/Logout.png" /></asp:LinkButton>
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
                                    <asp:Label ID="Label2" runat="server" Text="Liberação: " CssClass="subtituloAzul"></asp:Label>
                                </td>
                                <td style="font-size: x-small">
                                    <asp:Label ID="Label3" runat="server" Text="Etiqueta: " CssClass="subtituloAzul"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCodLiberacao" runat="server" Font-Size="x-Large" Width="190px"
                                        ForeColor="Green" onkeypress="if (isEnter(event)) return carregaLiberacao();"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCodEtiqueta" runat="server" Font-Size="x-Large" Width="190px"
                                        onkeypress="if (isEnter(event)) return efetuaLeitura();"></asp:TextBox>
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
                                                <asp:Label ID="Label4" runat="server" Text="Cód Pedido" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtIdPedido" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                    OnClick="imgPesq_Click" />
                                            </td>

                                            <td>
                                                <asp:Label ID="Label14" runat="server" Text="Visualizar" ForeColor="#0066FF"></asp:Label>
                                            </td>
                                            <td>
                                                <sync:CheckBoxListDropDown ID="drpVisualizar" runat="server" Width="150px">
                                                    <asp:ListItem Value="1" Selected="True">Expedidos</asp:ListItem>
                                                    <asp:ListItem Value="2" Selected="True">Não Expedidos</asp:ListItem>
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
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <asp:DetailsView runat="server" ID="dtvExpBalcao" DataSourceID="odsExpBalcao"
                AutoGenerateRows="False" GridLines="None" HorizontalAlign="Center" Width="100%"
                CellPadding="0" CellSpacing="0" OnDataBound="dtvExpBalcao_DataBound" EnableViewState="false">
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
                                                                <asp:Label ID="Label26" runat="server" Text="Total Expedido" CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label6" runat="server" Text="Peças de Vidro: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TotalPecasExpedidas") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Green"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label8" runat="server" Text="Volumes: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("TotalVolumesExpedidos") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Green"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label12" runat="server" Text="Peso: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("PesoTotalExpedido", "{0:N2}") %>' Font-Bold="True"
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
                                                                <asp:Label ID="Label34" runat="server" Text='<%# Eval("TotalPecasPendentes") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Red"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label35" runat="server" Text="Volumes: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label36" runat="server" Text='<%# Eval("TotalVolumesPendentes") %>' Font-Bold="True"
                                                                    Font-Size="Large" ForeColor="Red"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label37" runat="server" Text="Peso: " CssClass="subtituloAzul"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label38" runat="server" Text='<%# Eval("PesoTotalPendente", "{0:N2}") %>' Font-Bold="True"
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
                                                                <asp:Label ID="Label39" runat="server" Text="Total da Liberação" CssClass="subtituloAzul"></asp:Label>
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
                                                                <asp:Label ID="Label45" runat="server" Text='<%# Eval("PesoTotal", "{0:N2}") %>' Font-Bold="True"
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
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div class="boxItens">
                                            <div class="boxAzul">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblCliente" runat="server" Text="Cliente: " Font-Bold="True" 
                                                                Font-Size="Medium" ForeColor="#6EA0D0"></asp:Label>
                                                            <asp:Label ID="lblNomeCliente" runat="server" Text='<%# string.Format("{0} - {1}", Eval("IdCliente"), Eval("NomeCliente")) %>' 
                                                                Font-Size="Large" ForeColor="#6EA0D0" Font-Bold="True" ></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <colo:CollectionDataSource runat="server" id="cdsProdutos" Items='<%# Eval("Pecas") %>'>
                                                            </colo:CollectionDataSource>
                                                            <asp:GridView runat="server" ID="grvProdutos" Width="100%" AutoGenerateColumns="False"
                                                                DataSourceID="cdsProdutos" CssClass="gridStyle" GridLines="None" OnPreRender="grvProdutos_PreRender"
                                                                OnRowDataBound="grvProdutos_RowDataBound" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                                EditRowStyle-CssClass="edit" AllowPaging="true" AllowSorting="true">
                                                                <PagerSettings PageButtonCount="20" />
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Estornar" Visible="false">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelEstorno" runat="server" Visible='<%# Eval("Expedido") %>' />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdfIdProdPedProducao" runat="server" Value='<%# Eval("IdProdPedProducao") %>' />
                                                                            <asp:HiddenField ID="hdfIdProdImpressaoChapa" runat="server" Value='<%# Eval("IdProdImpressaoChapa") %>' />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label15" runat="server" Text='<%# Eval("IdPedido") + " (" + Eval("NumEtiqueta") + ")" + 
                                                                                    (((int?)Eval("IdPedidoRevenda")).GetValueOrDefault() > 0 ? "(Rev." + Eval("IdPedidoRevenda") + ")" : "" )  %>'></asp:Label>
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
                                                                            <asp:Label ID="Label55" runat="server" Text='<%# Eval("PedCli") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Item" SortExpression="CodInterno">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label16" runat="server" Text='<%# string.Format("{0} - {1}", Eval("CodProduto"), Eval("DescProduto")) %>'></asp:Label>
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
                                                                            <asp:Label ID="Label20" runat="server" Text='<%# Eval("M2", "{0:N2}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <uc1:ctrlimagempopup id="ctrlImagemPopup1" runat="server" imageurl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>' />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Etiqueta" SortExpression="Etiqueta">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("NumEtiqueta") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Leitura" SortExpression="DataLeitura">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label29" runat="server" Text='<%# Eval("DataLeitura") %>'></asp:Label>
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
                                                                    <%--<asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("LogEstornoVisible") %>'>
                                                                <a href="#" onclick='exibirEstornos(<%# Eval("IdItemCarregamento") %>); return false;'>
                                                                    <img alt="" border="0" src="../../Images/blocodenotas.png" title="Ver estornos" /></a>
                                                            </asp:PlaceHolder>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
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
                                                                                        UseSubmitBehavior="false" OnClick="btnEstorno_Click" />
                                                                                </td>
                                                                                                                                                                <td>
                                                                                    <asp:Button ID="btnEstornoTodos" runat="server" Text="Estornar Todos os Itens" Height="25px" Width="150px"
                                                                                        UseSubmitBehavior="false" OnClick="btnEstornoTodos_Click"
                                                                                        OnClientClick="if (!confirm(&quot;Tem certeza que deseja Estornar todas as peças?&quot;)) return false;"  Visible="false"/>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td align="left" valign="top">
                                                                        <colo:CollectionDataSource runat="server" id="cdsVolumes" Items='<%# Eval("Volumes") %>'>
                                                                        </colo:CollectionDataSource>
                                                                        <asp:GridView runat="server" ID="gvrVolumes" AutoGenerateColumns="False" DataSourceID="cdsVolumes"
                                                                            CssClass="gridStyle" GridLines="None" OnPreRender="gvrVolumes_PreRender" OnRowDataBound="gvrVolumes_RowDataBound"
                                                                            Width="100%" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                                                            AllowPaging="true" PageSize="3" AllowSorting="true">
                                                                            <PagerSettings PageButtonCount="20" />
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="Estornar" Visible="false">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkSelEstorno" runat="server" Visible='<%# Eval("Expedido") %>' />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <asp:HiddenField ID="hdfIdVolume" runat="server" Value='<%# Eval("IdVolume") %>' />
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
                                                                                        <asp:Label ID="Label55" runat="server" Text='<%# Eval("PedCli") %>'></asp:Label>
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
                                                                                        <asp:Label ID="Label21" runat="server" Text='<%# Eval("DataLeitura") %>'></asp:Label>
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
                                                                                <%--<asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("LogEstornoVisible") %>'>
                                                                            <a href="#" onclick='exibirEstornos(<%# Eval("IdItemCarregamento") %>); return false;'>
                                                                                <img alt="" border="0" src="../../Images/blocodenotas.png" title="Ver estornos" /></a>
                                                                        </asp:PlaceHolder>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>--%>
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
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>

        </div>
        <div id="divModuloSistema" style="display: none;">
            <div class="boxModuloSistema">
                <iframe runat="server" id="frameModuloSistema" frameborder="0" style="display: none" />
            </div>
        </div>
        <div style="display: none;">
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsExpBalcao" runat="server"
                TypeName="Glass.PCP.Negocios.IExpedicaoFluxo" SelectMethod="BuscaParaExpBalcao">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodLiberacao" PropertyName="Text" Name="idLiberarPedido"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpVisualizar" PropertyName="SelectedValue" Name="visualizar"
                        Type="String" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>

            <asp:HiddenField ID="hdfTempoLogin" runat="server" />
            <asp:HiddenField ID="hdfCorTela" runat="server" />
            <asp:HiddenField ID="hdfEstornar" runat="server" Value="false" />
            <audio id="sndOk" src="../../Images/ok.wav"></audio>
            <audio id="sndError" src="../../Images/error.wav"></audio>
            <asp:HiddenField ID="hdfFunc" runat="server" />
        </div>
    </form>
    <script type="text/javascript">

        function setFocus() {
            if ($('#txtCodLiberacao').val() == "")
                $('#txtCodLiberacao').focus();
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
