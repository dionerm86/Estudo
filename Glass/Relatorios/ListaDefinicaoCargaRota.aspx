<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaDefinicaoCargaRota.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaDefinicaoCargaRota" Title="Relatório de Definição de Carga de Rota" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function checkAll(checked) {
            var tabela = document.getElementById("<%= grdDados.ClientID %>");
            var inputs = tabela.getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf("chkTodos") > -1 || inputs[i].type != "checkbox")
                    continue;

                inputs[i].checked = checked;
            }
        }

        function getValoresMarcados() {
            var retorno = new Array();
            var tabela = document.getElementById("<%= grdDados.ClientID %>");

            for (i = 0; i < tabela.rows.length; i++) {
                var checkbox = FindControl("chkMarcar", "input", tabela.rows[i]);
                if (checkbox == null || !checkbox.checked)
                    continue;
                else
                    retorno.push(checkbox.value);

            }

            return retorno.join(",");
        }

        var data;
        
        function openRpt(excel) {

            var table = FindControl("grdDados", "table");

            if (table.rows.length > 1) {
                var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
                data = new Object();
                data["rel"] = "DefinicaoCargaRota";
                data["rotaId"] = FindControl("drpRota", "select").value;
                data["rota"] = FindControl("drpRota", "select").options[FindControl("drpRota", "select").selectedIndex].label;
                data["dataIni"] = FindControl("txtDataIni", "input").value;
                data["dataFim"] = FindControl("txtDataFim", "input").value;
                data["indices"] = FindControl("hdfIndice", "input").value;
                data["ExportaExcel"] = excel
            }
        }

        function getPostData() {
            return data;
        }

        function exibirPecas(idCliente, situacao) {
            debugger;
            var inicio = FindControl("txtDataIni", "input").value;
            var fim = FindControl("txtDataFim", "input").value;
            var situacao = situacao === 'pendente' ? "3" : situacao === 'pronto' ? "4" : "5";
            var url = "../Cadastros/Producao/LstProducao.aspx?cliente=" + idCliente + "&inicio=" + inicio + "&fim=" + fim + "&situacao=" + situacao;
            openWindow(400, 600, url);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdDados" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado." 
                    onrowcommand="grdDados_RowCommand" onrowdatabound="grdDados_RowDataBound" 
                    ShowFooter="True">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" 
                                    Text="Totais:"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M²" SortExpression="TotalM2">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotalM2") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaTotalM" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalM" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total Peso" SortExpression="Peso">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Peso") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaTotalPeso" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalPeso" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entregue M²" SortExpression="EntregueM2">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEntregueM2" runat="server" Text='<%# Bind("EntregueM2") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaEntregue" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <a href="#" alt="Visualizar peças entregues" title="Visualizar peças entregues" onclick="exibirPecas(<%# Eval("IdCliente") %>, 'entregue')">
                                    <asp:Label ID="lblTotalEntregue" runat="server" Text='<%# Bind("EntregueM2") %>'></asp:Label>
                                </a>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pronto M²" SortExpression="ProntoM2">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ProntoM2") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaPronto" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <a href="#" alt="Visualizar peças pendentes" title="Visualizar peças prontas" onclick="exibirPecas(<%# Eval("IdCliente") %>, 'pronto')">
                                    <asp:Label ID="lblTotalPronto" runat="server" Text='<%# Bind("ProntoM2") %>'></asp:Label>
                                </a>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pendente M²" SortExpression="PendenteM2">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PendenteM2") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaPendente" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <a href="#" alt="Visualizar peças pendentes" title="Visualizar peças pendentes"  onclick="exibirPecas(<%# Eval("IdCliente") %>, 'pendente')">
                                    <asp:Label ID="lblTotalPendente" runat="server" Text='<%# Bind("PendenteM2") %>'></asp:Label>
                                </a>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Etiq. Não Imp. M²" 
                            SortExpression="EtiquetaNaoImpressaM2">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" 
                                    Text='<%# Bind("EtiquetaNaoImpressaM2") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblSomaEtiq" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalEtiq" runat="server" 
                                    Text='<%# Bind("EtiquetaNaoImpressaM2") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("Indice") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# (Glass.Conversoes.StrParaInt(Eval("Indice").ToString()) > 1) && (bool)Eval("AlteraLinha") %>' />
                                <asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("Indice") %>' Visible='<%# Eval("AlteraLinha") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle Font-Bold="True" Font-Size="Small" HorizontalAlign="Right" />
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDefinicaoCargaRota" runat="server" SelectMethod="ObterDados"
                    TypeName="Glass.Data.RelDAL.DefinicaoCargaRotaDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpRota" Name="rota" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="6">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIndice" runat="server" />
</asp:Content>
