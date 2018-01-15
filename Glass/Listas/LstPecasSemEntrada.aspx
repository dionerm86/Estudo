<%@ Page Title="Peças sem Entrada no Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPecasSemEntrada.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPecasSemEntrada" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function escondeColuna(numColuna)
        {
            var tabela = FindControl("grdProdutoSemEntrada", "table");
            if (tabela == null)
                return;

            for (i = 0; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].cells.length == 1 || numColuna > tabela.rows[i].cells.length)
                    continue;

                tabela.rows[i].cells[numColuna].style.display = "none";
            }
        }
        
        function getFornec(idFornec)
        {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }
        
        function openRpt()
        {
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var idCompra = FindControl("txtNumCompra", "input").value;
            var numNFe = FindControl("txtNumNFe", "input").value;
            var dataIni = FindControl("txtDataIni", "input").value;
            var dataFim = FindControl("txtDataFim", "input").value;

            idFornec = idFornec != "" ? idFornec : "0";
            idCompra = idCompra != "" ? idCompra : "0";
            numNFe = numNFe != "" ? numNFe : "0";

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PecasSemEntrada&idFornec=" + idFornec +
                "&nomeFornec=" + nomeFornec + "&idCompra=" + idCompra + "&numNFe=" + numNFe + "&dataIni=" + dataIni + "&dataFim=" + dataFim);
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="filtros">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="NFe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNFe" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="separador">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr id="lista">
            <td align="center">
                <asp:GridView ID="grdProdutoSemEntrada" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataSourceID="odsProdutoSemEntrada" GridLines="None" 
                    EmptyDataText="Não há peças que ainda não deram entrada no estoque.">
                    <Columns>
                        <asp:BoundField DataField="IdCompra" HeaderText="Compra" 
                            SortExpression="IdCompra" />
                        <asp:BoundField DataField="NumeroNFe" HeaderText="NFe" 
                            SortExpression="NumeroNFe" />
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFornec">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" 
                                    Text='<%# Eval("IdFornec") + " - " + Eval("NomeFornec") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." 
                            SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                            SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" 
                            SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" 
                            SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. M²" SortExpression="TotM" />
                        <asp:BoundField DataField="ValorUnit" HeaderText="Valor Comprado" 
                            SortExpression="ValorUnit" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" 
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="ValorBenef" HeaderText="Valor Benef." 
                            SortExpression="ValorBenef" DataFormatString="{0:c}" />
                        <asp:TemplateField HeaderText="Qtde a dar entrada" 
                            SortExpression="QtdeEntrada">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# float.Parse(Eval("Qtde").ToString()) - float.Parse(Eval("QtdeEntrada").ToString()) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" 
                    onclientclick="openRpt(); return false"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoSemEntrada" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.RelDAL.ProdutoEntradaEstoqueDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumNFe" Name="numeroNFe" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" 
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        FindControl("txtNumFornec", "input").focus();
        
        var esconderColunas = <%= EsconderColunas().ToString().ToLower() %>;
        if (esconderColunas)
        {
            escondeColuna(6); // Altura
            escondeColuna(7); // Largura
            escondeColuna(8); // Tot. M²
            escondeColuna(11); // Valor Benef.
        }
    </script>
</asp:Content>

