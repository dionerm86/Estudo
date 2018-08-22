<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LstEtiquetaProdImp.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstEtiquetaProdImp" MasterPageFile="~/Layout.master" Title="Produtos da Impressão N.º" %>
    
<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc2" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">

        function arquivoOtimizacao(idImpressao) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var descricao = FindControl("txtDescr", "input").value;

            redirectUrl("../Handlers/ArquivoOtimizacao.ashx?idImpressao=" + idImpressao + 
                "&idPedido=" + idPedido + "&descrProd=" + descricao);
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
                                        <asp:Label ID="Label14" runat="server" Text="NFe" ForeColor="#0066FF" Visible="false"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:TextBox ID="txtNumNFe" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" Visible="false"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            Height="16px" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label4" runat="server" Text="Descrição Prod." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescr" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            Height="16px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Plano de Corte" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPlanoCorte" runat="server"
                                            Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label13" runat="server" Text="Etiqueta" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEtiqueta" runat="server"
                                            Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            ToolTip="Pesquisar" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label3" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtAltura" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            Height="16px" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label5" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtLargura" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            Height="16px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" 
                                AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" DataKeyNames="IdProdImpressao"
                                DataSourceID="odsProduto" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado."
                                PageSize="15" onrowdatabound="grdProduto_RowDataBound" 
                                onrowcommand="grdProduto_RowCommand" onload="grdProduto_Load" >
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgCancelar" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                OnClientClick='<%# Eval("IdProdImpressao", "openWindow(180, 550, \"../Utils/SetMotivoCancProdImpressao.aspx?idProdImpressao={0}\"); return false") %>'
                                                Visible='<%# Eval("CancelarVisible") %>' ToolTip="Cancelar etiqueta" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="addObs" Height="16px"
                                                ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" CausesValidation="false" 
                                                CommandArgument='<%# (uint)(int)Eval("IdProdImpressao") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False"
                                                Visible='<%# (bool)Eval("AddObsVisible") %>' ToolTip="Adicionar Observação">
                                     <img border="0" src="../Images/edit.gif" alt=""/></asp:LinkButton>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                ToolTip="Cancelar" CausesValidation="False" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdPedido") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("PedCli") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("Ambiente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="NF-e" SortExpression="NumeroNFe">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFornec">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("NomeFornec") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind((bool)Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%#Bind((bool)Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Proc." SortExpression="CodProcesso">
                                        <ItemTemplate>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Apl." SortExpression="CodAplicacao">
                                        <ItemTemplate>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("ObsEditar") %>'></asp:TextBox>
                                            <asp:HiddenField ID="hdfObs" runat="server" Value='<%# Bind("ObsEditar") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Etiqueta" 
                                        SortExpression="concat(pi.IdPedido, pi.PosicaoProd, pi.ItemEtiqueta)">
                                        <ItemTemplate>
                                            <asp:Label ID="Label12" runat="server" Text='<%# Bind("NumEtiqueta") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdProdImpressao") %>'
                                                Tabela="ProdutoImpressao" />
                                            <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdProdImpressao") %>'
                                                Tabela="ProdutoImpressao" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="Label6" runat="server" 
                                Text="Etiquetas em vermelho estão canceladas e não serão impressas" 
                                ForeColor="Red"></asp:Label>
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
        <tr id="impressao" runat="server">
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClick="lnkImprimir_Click">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimirApenasPlano" runat="server" 
                    onclick="lnkImprimirApenasPlano_Click">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir apenas planos de corte</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:PlaceHolder ID="pchArqOtimiz" runat="server">
                    <a id="lnkArqOtimizacao" href="#"
                        onclick='arquivoOtimizacao(<%= Request["IdImpressao"] %>);'>
                        <img border="0" src="../Images/blocodenotas.png" title="Arquivo de Otimização" />
                        Arquivo de Otimização</a>&nbsp;&nbsp;&nbsp;&nbsp; </asp:PlaceHolder>
                <asp:LinkButton ID="lnkImprimirCorEspessura" runat="server" OnClick="lnkImprimirCorEspessura_Click">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir produtos por cor e espessura</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCountImpressao" SelectMethod="GetListImpressao" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" 
        TypeName="Glass.Data.DAL.ProdutoImpressaoDAO" >
        <SelectParameters>
            <asp:QueryStringParameter Name="idImpressao" QueryStringField="idImpressao" Type="UInt32" />
            <asp:ControlParameter ControlID="txtPlanoCorte" Name="planoCorte" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumNFe" Name="numeroNFe" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtDescr" Name="descrProduto" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="txtEtiqueta" Name="etiqueta" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" 
                Type="Single" />
            <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" 
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>