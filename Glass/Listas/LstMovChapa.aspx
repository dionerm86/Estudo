<%@ Page Title="Extrato de Mov. de Chapa de Vidro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstMovChapa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovChapa" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function exibirChapas(botao, idCorVidro, espessura) {

            var linha = document.getElementById("chapa_" + idCorVidro + "_" + espessura);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Chapas";
        }

        function exibirDados(botao, titulo, etiqueta, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, titulo + ' da etiqueta: ' + etiqueta, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

        function openRpt(exibirDetalhes, exportarExcel) {
            var idsCorVidro = FindControl("cbdCorVidro", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;

            var queryString = "&idsCorVidro=" + idsCorVidro + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&espessura=" + espessura + "&altura=" + altura + "&largura=" + largura + "&exibirDetalhes=" + exibirDetalhes + "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MovChapa" + queryString);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdCorVidro" runat="server" DataSourceID="odsCorVidro"
                                DataTextField="Descricao" AppendDataBoundItems="true" DataValueField="IdCorVidro"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" Title="Selecione uma cor">
                                <asp:ListItem Value="0">SEM COR</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Espessura" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="35px" MaxLength="5" onkeypress="return soNumeros(event, false, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblAltura" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblLargura" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de Leitura" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMovChapa" runat="server"
                    AutoGenerateColumns="False" DataSourceID="odsMovChapa"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum registro encontrado." Width="1000px">
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirChapas(this, " + Eval("IdCorVidro") + "," + Eval("Espessura") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Chapas" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Cor" SortExpression="CorVidro">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CorVidro") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Espessura" SortExpression="Espessura">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Espessura", "{0} MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. Inicial" SortExpression="Inicial">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Inicial") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. Utilizada" SortExpression="Utilizado">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Utilizado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. Disponível" SortExpression="Disponivel">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Disponivel") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="M² Lido" SortExpression="M2Lido">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("M2Lido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sobra" SortExpression="Sobra">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Sobra") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="chapa_<%# Eval("IdCorVidro") %>_<%# Eval("Espessura") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="11">
                                        <br />
                                        <asp:GridView GridLines="None" ID="grdMovChapaDetalhe" runat="server"
                                            AutoGenerateColumns="False" DataSource='<%# Eval("Chapas") %>'
                                            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                            EditRowStyle-CssClass="edit" Width="100%"
                                            EmptyDataText="Nenhum chapa encontrada." OnRowDataBound="grdMovChapaDetalhe_RowDataBound">
                                            <AlternatingRowStyle CssClass="alt" />
                                            <Columns>
                                                <asp:BoundField DataField="DataLeitura" HeaderText="Data Leitura" DataFormatString="{0:d}" />
                                                <asp:BoundField DataField="NumEtiqueta" HeaderText="Etq. da Chapa" />
                                                <asp:BoundField DataField="DescricaoProd" HeaderText="Descr." />
                                                <asp:BoundField DataField="M2Utilizado" HeaderText="M²" />
                                                <asp:TemplateField HeaderText="M² Lido">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("M2Lido", "{0:n2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sobra">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Sobra", "{0:n2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Plano de Corte">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                                            <a href="#" onclick='exibirDados(this, &#039Plano de Corte&#039, &#039<%# Eval("NumEtiqueta") %>&#039,&#039;<%# Eval("PlanosCorte") != null ? ((string)Eval("PlanosCorte")).Replace("\r\n"," ") : "" %>&#039;); return false;'>
                                                                <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver Plano de Corte." /></a>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Etiquetas">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server">
                                                            <a href="#" onclick='exibirDados(this, &#039Leituras&#039, &#039<%# Eval("NumEtiqueta") %>&#039,&#039;<%# Eval("Etiquetas") != null ? ((string)Eval("Etiquetas")).Replace("\r\n"," ") : "" %>&#039;); return false;'>
                                                                <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver Etiquetas." /></a>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EditRowStyle CssClass="edit" />
                                            <PagerStyle CssClass="pgr" />
                                        </asp:GridView>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <HeaderStyle HorizontalAlign="Left" />
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <RowStyle HorizontalAlign="Left" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMovChapa" runat="server"
                    DataObjectTypeName="Glass.Estoque.Negocios.Entidades.MovChapa"
                    SelectMethod="ObtemMovChapa"
                    TypeName="Glass.Estoque.Negocios.IMovChapaFluxo"
                    EnablePaging="false">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdCorVidro" Name="idsCorVidro" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="DateTime" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="Label8" runat="server" Text="Chapas em vermelho indicam leituras em dias diferentes." Font-Italic="True" ForeColor="#FF5050"></asp:Label><br />
                <asp:Label ID="Label9" runat="server" Text="Chapas em azul indicam revenda." Font-Italic="True" ForeColor="#FF5050"></asp:Label><br />
                <asp:Label ID="Label10" runat="server" Text="A quantidade disponível é baseada na inicial menos a utilizada." Font-Italic="True" ForeColor="#FF5050"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="return openRpt(false, true);"> <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirDetalhes" runat="server" OnClientClick="return openRpt(true, false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir detalhado</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcelDetalhes" runat="server" OnClientClick="return openRpt(true, true);"> <img alt="" border="0" src="../Images/Excel.gif" /> Exportar detalhado para o Excel </asp:LinkButton>
            </td>
        </tr>
    </table>
    <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>
</asp:Content>
