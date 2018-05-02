<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelProdEtiqueta.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelProdEtiqueta" Title="Impressões Pendentes" MasterPageFile="~/Layout.master" %>

<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
 
        function setProdEtiqueta(idProdPed, idAmbiente, idPedido, descrProd, codProc, codApl, qtd, qtdImpresso, altura, largura, totM, obs, numEtiqueta, calcularTotM, atualizarTotais, totM2Calc)
        {
            if (SelProdEtiqueta.PodeImprimirPedidoImportado(idPedido).value.toLowerCase() == "false") {
                alert("O pedido importado ainda não foi conferido, confira o mesmo antes de imprimir");
                return false;
            }

            if (calcularTotM) {
                if (qtd == 0 && qtdImpresso == 0) {
                    totM = 0;
                    totM2Calc = 0;
                }
                else {
                    totM = new Number((parseFloat(totM.toString().replace(',', '.')) / parseFloat(qtd)) * parseFloat((qtd - qtdImpresso))).toFixed(2).toString().replace('.', ',');
                    totM2Calc = new Number((parseFloat(totM2Calc.toString().replace(',', '.')) / parseFloat(qtd)) * parseFloat((qtd - qtdImpresso))).toFixed(2).toString().replace('.', ',');
                }
            }

            if (FindControl("chkComposicaoLaminado", "input") != null && FindControl("chkComposicaoLaminado", "input").checked == true){

                var isLamComp = SelProdEtiqueta.IsProdutoLaminadoComposicao(idProdPed);
                if (isLamComp.error != null) {
                    alert(isLamComp.error.description);
                    return false;
                }

                if (isLamComp.value == "true")
                    return false;
            }
 
            window.opener.setProdEtiqueta(idProdPed, idAmbiente, idPedido, null, null, descrProd, codProc, codApl, qtd, qtdImpresso,
                qtd - qtdImpresso, altura, largura, obs, totM, null, window, numEtiqueta != "", numEtiqueta, atualizarTotais, totM2Calc, null);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table id="tbProd" runat="server">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                            Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                            ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                            OpenOnStart="False">
                                        </sync:CheckBoxListDropDown>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                            TypeName="Glass.Data.DAL.RotaDAO">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label4" runat="server" Text="Descrição Prod." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescr" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label5" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" DataSourceID="odsCorVidro"
                                            DataTextField="Descricao" DataValueField="IdCorVidro" AutoPostBack="True">
                                            <asp:ListItem Value="0">Todas</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label3" runat="server" Text="Espessura" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtEspessura" runat="server" Width="40px" MaxLength="5" onkeypress="return soNumeros(event, false, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" VerificarSeControleDeveSerDesabilitado="false" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Data de entrega do pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                                        <asp:ImageButton ID="imgDataIni" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                            OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                                        <asp:ImageButton ID="imgDataFim" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                            OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblProcesso" runat="server" Text="Processo" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProcesso" runat="server" Width="40px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblAplicacao" runat="server" Text="Aplicação" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAplicacao" runat="server" Width="40px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text="Fast Delivery" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpFastDelivery" runat="server">
                                            <asp:ListItem Value="0">Não incluir no resultado</asp:ListItem>
                                            <asp:ListItem Value="1" Selected="True">Incluir no resultado</asp:ListItem>
                                            <asp:ListItem Value="2">Apenas Fast Delivery</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkComposicaoLaminado" runat="server" AutoPostBack="true" ForeColor="#0066FF" Text="Composição duplo/laminado " TextAlign="Left" Checked="true"></asp:CheckBox>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="Data de fábrica" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>                                        
                                        <uc1:ctrlData ID="ctrlDataFabricaIni" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFabricaFim" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="Pedidos de mão de obra" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpMaoDeObra" runat="server">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="1">Incluir no resultado</asp:ListItem>
                                        </asp:DropDownList>                                        
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="Peças Repostas" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpPecasRepostas" runat="server">
                                            <asp:ListItem Value="0">Não incluir no resultado</asp:ListItem>
                                            <asp:ListItem Value="1">Incluir no resultado</asp:ListItem>
                                            <asp:ListItem Value="2">Apenas peças repostas</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd" DataSourceID="odsProduto"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <a href="#" onclick="setProdEtiqueta('<%# GetIdProdPed(Eval("IdPedido"), Eval("IdProdPed")) %>', '<%# GetIdAmbientePedido(Eval("IdPedido"), Eval("IdAmbientePedido")) %>', '<%# Eval("IdPedido") %>', '<%# Eval("DescrProduto") %>', '<%# Eval("CodProcesso") %>', '<%# Eval("CodAplicacao") %>', '<%# ObterQtde(Eval("IdProdPed"), (bool)Eval("PecaReposta"), Eval("Qtde")) %>', '<%# (bool)Eval("PecaReposta") ? 1 : Eval("QtdImpresso") %>', '<%# Eval("Altura") %>', '<%# Eval("Largura") %>', '<%# Eval("TotM") %>', '<%# Eval("Obs") != null ? Eval("Obs").ToString().Replace("\n", " ").Replace("\t", " ").Replace("\r", " ") : "" %>', '<%# Eval("NumEtiqueta") %>', true, null, '<%# Eval("TotM2Calc") %>');">
                                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                    <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CodProcesso" HeaderText="Proc." SortExpression="CodProcesso">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CodAplicacao" HeaderText="Apl." SortExpression="CodAplicacao">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Qtde" HeaderText="Qtd." SortExpression="Qtde" />
                                    <asp:BoundField DataField="QtdImpresso" HeaderText="Qtd. Já Impresso" SortExpression="QtdImpresso" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> Adicionar Todos (Ordenando por Cor/Espessura)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" 
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCountEtiq" SelectMethod="GetListEtiq" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" 
        TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" SkinID="">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpCorVidro" Name="idCorVidro" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text" Type="Single" />
            <asp:ControlParameter ControlID="drpMaoDeObra" Name="maoDeObra" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFabricaIni" Name="dataFabricaIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFabricaFim" Name="dataFabricaFim" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="txtProcesso" Name="codProcesso" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtAplicacao" Name="codAplicacao" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpPecasRepostas" Name="pecasRepostas" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblRota" Name="idsRotas" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="drpFastDelivery" Name="fastDelivery" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorVidro" runat="server" MaximumRowsParameterName=""
        SelectMethod="GetAll" StartRowIndexParameterName=""
        TypeName="Glass.Data.DAL.CorVidroDAO" >
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfImpressao" runat="server" />
</asp:Content>
