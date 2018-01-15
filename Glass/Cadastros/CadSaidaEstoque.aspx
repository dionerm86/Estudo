<%@ Page Title="Saída de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadSaidaEstoque.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSaidaEstoque" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">
        function loadProduto(codInterno)
        {
            if (codInterno.value == "")
                return false;

            try
            {
                var retorno = MetodosAjax.GetProd(codInterno.value).value.split(';');
                
                if (retorno[0] == "Erro")
                {
                    alert(retorno[1]);
                    codInterno.value = "";
                    return false;
                }
                
                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch(err)
            {
                alert(err.value);
            }
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Número do Pedido:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);" Width="70px"
                                TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                ErrorMessage="*" Display="Dynamic" ValidationGroup="ped"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imbPesq_Click" ValidationGroup="ped" />
                        </td>
                    </tr>
                </table>
                <table runat="server" id="filtroProd">
                    <tr>
                        <td>
                            Produto:
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="loadProduto(this);" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq1', null);"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq1', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvProd" runat="server" 
                                ControlToValidate="txtDescr" Display="Dynamic" ErrorMessage="*" 
                                ValidationGroup="prod"></asp:RequiredFieldValidator>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imgPesq1_Click" ValidationGroup="prod" />
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Marcar saída na loja:"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false" MostrarTodas="false"/>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
    <table id="tbSaida" runat="server" style="width: 100%" visible="false">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblSubtitulo" runat="server" Text="Produtos do Pedido"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsProdXPed" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdProdPed"
                    EmptyDataText="Nenhum produto encontrado." PageSize="30">
                    <Columns>
                        <asp:TemplateField HeaderText="Qtde. Saída">
                            <HeaderTemplate>
                                <asp:Label ID="Label3" runat="server" Text="Label">Qtde. Saída</asp:Label></td>
                                &nbsp;<br />
                                <asp:LinkButton ID="lnkTodos" runat="server" ForeColor="blue"
                                    OnClientClick="return marcarSaida();" onclick="lnkTodos_Click">(Marcar todos)</asp:LinkButton></td>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdSaida" runat="server" onkeypress="return soNumeros(event, false, true);"
                                    Width="50px" TabIndex="3"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <asp:HiddenField ID="hdfDescr" runat="server" Value='<%# Eval("DescrProduto") %>' />
                                <asp:HiddenField ID="hdfQtde" runat="server" Value='<%# Eval("Qtde") %>' />
                                <asp:HiddenField ID="hdfQtdSaida" runat="server" Value='<%# Eval("QtdSaida") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vendido" SortExpression="ValorVendido"
                            DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Qtde") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="#33CC33" Text='<%# Eval("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Saída" SortExpression="QtdSaida">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("QtdSaida") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True" ForeColor="Red" Text='<%# Eval("QtdSaida") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnMarcarSaida" runat="server" Text="Marcar Saída" OnClick="btnMarcarSaida_Click"
                    OnClientClick="return confirm('Marcar saída dos produtos nas quantidades informadas?');"
                    TabIndex="5" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" 
                    MaximumRowsParameterName="pageSize" SelectMethod="GetForSaidaEstoque" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <table id="tbSaidaProd" runat="server" visible="false">
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutosProd" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsProduto" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdProd"
                    EmptyDataText="Nenhum produto encontrado." PageSize="30">
                    <Columns>
                        <asp:TemplateField HeaderText="Qtde. Saída">
                            <HeaderTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Label">Qtde. Saída</asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdSaida" runat="server" onkeypress="return soNumeros(event, false, true);"
                                    Width="50px" TabIndex="3"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                            SortExpression="DescrProduto" />
                        <asp:TemplateField HeaderText="Qtde" SortExpression="QtdEstoque">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("QtdEstoque") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="#33CC33" 
                                    Text='<%# Eval("DescrQtdeEstoque") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
             <td align="center">
                <div>Observação</div>
                <div>
                    <asp:TextBox runat="server" ID="txtObservacao" TextMode="MultiLine" MaxLength="500" style="margin: 2px 0px; width: 254px; height: 70px;" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnMarcarProd" runat="server" Text="Marcar Saída" 
                    onclick="btnMarcarProd_Click" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" 
                    SelectMethod="GetForRptEstoque" TypeName="Glass.Data.DAL.ProdutoLojaDAO" 
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" 
                            PropertyName="Text" Type="String" />
                        <asp:Parameter Name="idGrupoProd" Type="UInt32" />
                        <asp:Parameter Name="idSubgrupoProd" Type="UInt32" />
                        <asp:Parameter Name="exibirApenasComEstoque" Type="Boolean" />
                        <asp:Parameter Name="exibirApenasPosseTerceiros" Type="Boolean" />
                        <asp:Parameter Name="exibirApenasProdutosProjeto" Type="Boolean" />
                        <asp:Parameter Name="idCorVidro" Type="UInt32" />
                        <asp:Parameter Name="idCorFerragem" Type="UInt32" />
                        <asp:Parameter Name="idCorAluminio" Type="UInt32" />
                        <asp:Parameter Name="situacao" Type="Int32" />
                        <asp:Parameter Name="estoqueFiscal" Type="Int32" />
                        <asp:Parameter Name="aguardandoSaidaEstoque" Type="Boolean" />
                        <asp:Parameter Name="ordenacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    </asp:Content>
