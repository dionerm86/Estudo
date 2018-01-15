<%@ Page Title="Gerenciamento de Fornada" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstFornada.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFornada" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">

        function openRpt(analitico, exportarExcel) {

            var idFornada = FindControl("txtIdFornada", "input").value;
            var idPedido = FindControl("txtIdPedido", "input").value;
            var dataIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var numEtiqueta = FindControl("txtNumEtiqueta", "input").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var idsCorVidro = FindControl("cbdCorVidro", "select").itens();

            var queryStr = "";
            queryStr += "&idFornada=" + idFornada;
            queryStr += "&idPedido=" + idPedido;
            queryStr += "&dataIni=" + dataIni;
            queryStr += "&dataFim=" + dataFim;
            queryStr += "&numEtiqueta=" + numEtiqueta;
            queryStr += "&espessura=" + espessura;
            queryStr += "&idsCorVidro=" + idsCorVidro;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=Fornada&analitico=" + analitico + "&exportarExcel=" + exportarExcel + queryStr);

            return false;
        }

        function exibirDados(botao, idFornada, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, 'Etiquetas da fornada: ' + idFornada, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

        function exibirPecas(botao, idFornada) {

            var linha = document.getElementById("fornada_" + idFornada);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Peças";
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdFornada" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Pedido:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Num. Etiqueta:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumEtiqueta" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label33" runat="server" ForeColor="#0066FF" Text="Espessura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="35px" MaxLength="5" onkeypress="return soNumeros(event, false, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" QueryString="espessura"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label31" runat="server" ForeColor="#0066FF" Text="Cor"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdCorVidro" runat="server" DataSourceID="odsCorVidro" Width="200"
                                DataTextField="Descricao" AppendDataBoundItems="true" DataValueField="IdCorVidro"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" Title="Selecione uma cor">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq16" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdFornada" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsFornada" DataKeyNames="IdFornada" AutoGenerateColumns="False"
                    EmptyDataText="Nenhuma fornada encontrada." AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirPecas(this, " + Eval("IdFornada") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Peças" />
                            </ItemTemplate>
                            <EditItemTemplate>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Cód" SortExpression="IdFornada" DataField="IdFornada" >
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Funcionário" SortExpression="DescrUsuCad" DataField="DescrUsuCad" >
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Data" SortExpression="DataCad" DataField="DataCad" />
                        <asp:BoundField HeaderText="Capacidade" SortExpression="Capacidade" DataField="Capacidade" DataFormatString="{0:F2} M²" />
                        <asp:TemplateField HeaderText="Lido" SortExpression="M2Lido">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("M2Lido", "{0:F2} M²") + " (" + Eval("QtdeLida") + ")" %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aproveitamento" SortExpression="Aproveitamento">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Aproveitamento") + "%" %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Etiquetas">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server">
                                    <a href="#" onclick='exibirDados(this, &#039;<%# Eval("IdFornada") %>&#039;, &#039;<%# Eval("Etiquetas") %>&#039;); return false;'>
                                        <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver etiquetas." /></a>
                                </asp:PlaceHolder>
                               <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfIdFornada" runat="server" Value='<%# Eval("IdFornada") %>' />
                                <tr id="fornada_<%# Eval("IdFornada") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="5">
                                        <br />
                                        &nbsp;
                                        <br />
                                        <br />
                                        <asp:GridView ID="grdProdutosPedidoProducao" runat="server" AutoGenerateColumns="False" DataKeyNames="IdProdPedProducao"
                                            DataSourceID="odsProdutosPedidoProducao" GridLines="None" Width="100%" class="pos" ShowFooter="True"
                                            CellPadding="0" EmptyDataText="Nenhuma peça encontrada.">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdfIdFornada" runat="server" Value='<%# Eval("IdFornada") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Produto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCodProd" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Descrição">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Etiqueta">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEtiqueta" runat="server" Text='<%# Eval("NumEtiquetaExibir") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Altura">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAltura" runat="server" Text='<%# Eval("Altura") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                 <asp:TemplateField HeaderText="Largura">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLargura" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                       <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <FooterStyle Font-Bold="true" Height="30px" />
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedidoProducao" runat="server" 
                                            DataObjectTypeName="Glass.Data.Model.ProdutoPedidoProducao"
                                            SelectMethod="ObterPecasFornada" 
                                            TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdFornada" Name="idFornada" PropertyName="Value"
                                                    Type="Int32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(true, false);"> <img alt="" border="0" src="../Images/printer.png" /> Relatório Analítico</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, true); return false;">
                                <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="return openRpt(false, false);"> <img alt="" border="0" src="../Images/printer.png" /> Relatório Sintético</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRpt(false, true); return false;">
                                <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFornada" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow"
                    SelectMethod="PesquisarFornadas" SelectCountMethod="PesquisarFornadasCount"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Data.DAL.FornadaDAO"
                    DataObjectTypeName="Glass.Data.Model.Fornada">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdFornada" Name="idFornada" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtNumEtiqueta" Name="numEtiqueta" PropertyName="Text" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataIni" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataFim" PropertyName="DataString"  />
                        <asp:ControlParameter ControlID="cbdCorVidro" Name="idsCorVidro" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>

</asp:Content>
