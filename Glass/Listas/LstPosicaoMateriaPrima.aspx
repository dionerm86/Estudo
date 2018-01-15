<%@ Page Title="Posição de Matéria-Prima" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstPosicaoMateriaPrima.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPosicaoMateriaPrima" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlconsultacadclisintegra"
    TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function exibirChapas(botao, idCorVidro, espessura) {
            var linha = document.getElementById("chapas_" + idCorVidro + "_" + espessura);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " chapas";
        }

        function openRpt(exportarExcel) {

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=PosicaoMateriaPrima&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Situação do Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" Title="Selecione uma situação"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False">
                                <asp:ListItem Value="4">Conferido COM</asp:ListItem>
                                <asp:ListItem Value="7">Confirmado PCP</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Tipo de Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" Title="Selecione uma opção"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False">
                                <asp:ListItem Value="4">Produção</asp:ListItem>
                                <asp:ListItem Value="1">Venda</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdRota" runat="server" Title="Selecione uma opção"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" AppendDataBoundItems="true"
                                DataSourceID="odsRota" DataTextField="CodInterno" DataValueField="IdRota">
                                <asp:ListItem Value="0">SEM ROTA</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdCorVidro" runat="server" DataSourceID="odsCorVidro"
                                DataTextField="Descricao" AppendDataBoundItems="true" DataValueField="IdCorVidro"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" Title="Selecione uma cor">
                                <asp:ListItem Value="0">SEM COR</asp:ListItem>
                            </sync:CheckBoxListDropDown>
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
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkTotM2DisponivelNegativo" runat="server" Text="M2 Disponível Negativo" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPosicaoMateriaPrima"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EmptyDataText="Nenhum produto encontrado."
                    Style="margin-right: 0px">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirChapas(this, " + Eval("IdCorVidro") + "," + Eval("Espessura").ToString().Replace(",", ".") + "); return false;" %>'
                                    Width="10px" ToolTip="Exibir chapas" Visible='<%# (decimal)Eval("TotM2Estoque") > 0 %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrCorVidro" HeaderText="Cor"
                            SortExpression="DescrCorVidro">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EspessuraString" HeaderText="Espessura"
                            SortExpression="EspessuraString">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM2" HeaderText="Total M2"
                            SortExpression="TotM2">
                            <ControlStyle Font-Bold="False" />
                            <ItemStyle Font-Bold="True" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM2ComEtiqueta" HeaderText="M2 Com Etiqueta Impressa"
                            SortExpression="TotM2ComEtiqueta">
                            <HeaderStyle BorderStyle="None" BorderWidth="10px" Wrap="True" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM2SemEtiqueta" HeaderText="M2 Sem Etiqueta Impressa"
                            SortExpression="TotM2SemEtiqueta">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM2Venda" HeaderText="M2 Pedido de Venda"
                            SortExpression="TotM2Venda">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM2Producao" HeaderText="M2 Pedido de Produção"
                            SortExpression="TotM2Producao">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="M2 em Estoque" SortExpression="TotM2Estoque">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("TotM2Estoque") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Font-Bold="True" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="M2 Disponível" SortExpression="TotM2Disponivel">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("TotM2DisponivelNovo") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Font-Bold="True" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr align="center" id="chapas_<%# Eval("IdCorVidro").ToString() + "_" + Eval("Espessura").ToString().Replace(",", ".") %>" style="display: none" class="<%= GetAlternateClass() %>">
                                    <td colspan="11">
                                        <asp:GridView ID="grdChapas" runat="server" AutoGenerateColumns="False"
                                            CellPadding="3" DataSource='<%# Eval("Chapas") %>' CssClass="gridStyle"
                                            PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                            GridLines="None" Width="100%" OnRowDataBound="grdChapas_RowDataBound" ShowFooter="True">
                                            <AlternatingRowStyle CssClass="alt" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Cod." SortExpression="CodInterno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">

                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFornecedor">

                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("NomeFornecedor") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Altura" SortExpression="Altura">

                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="Label8" runat="server" Text='Total: '></asp:Label>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtd." SortExpression="QtdeChapa">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label7" runat="server" Text='<%# Bind("QtdeChapa") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total M2" SortExpression="TotalM2Chapa">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalM2Chapa") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <PagerStyle CssClass="pgr" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <br />
                <asp:Label ID="Label21" runat="server" ForeColor="Red" Text="São consideradas todas as chapas de notas finalizadas (com ou sem etiqueta impressa)"></asp:Label>
            </td>
        </tr>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lkbRelatorio" runat="server" OnClientClick="openRpt(false); return false;"> <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>&nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>

        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPosicaoMateriaPrima"
                    runat="server" SelectMethod="GetPosMateriaPrima"
                    TypeName="Glass.Data.RelDAL.PosicaoMateriaPrimaDAO" MaximumRowsParameterName="pageSize"
                    StartRowIndexParameterName="startRow" CacheExpirationPolicy="Absolute"
                    ConflictDetection="OverwriteChanges" SkinID="">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdRota" Name="idsRota" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdTipoPedido" Name="tipoPedido" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacaoPedido" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataIniEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdCorVidro" Name="idsCorVidro" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="chkTotM2DisponivelNegativo" Name="totM2DisponivelNegativo"
                            PropertyName="Checked" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedidoPCP"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetRptRota" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
