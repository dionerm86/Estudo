<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlLstProdProducao.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlLstProdProducao" %>

<%@ Register Src="ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc2" %>
<%@ Register Src="ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc4" %>

<asp:GridView GridLines="None" ID="grdPecasParent" runat="server" AllowPaging="True" AutoGenerateColumns="False"
    DataKeyNames="IdProdPedProducao" DataSourceID="odsPecasParent" EmptyDataText="Nenhuma peça encontrada."
    CssClass="gridStyle"
    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    AllowSorting="True" OnDataBound="grdPecasParent_DataBound" OnLoad="grdPecasParent_Load">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/mais.gif" ToolTip="Exibir Produtos da Composição"
                    Visible='<%# Eval("IsProdutoLaminadoComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPedProducao") + "); return false"%>' />
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                 <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/arrow_undo.gif" ToolTip="Remover peça desta situação" Visible='<%# Eval("RemoverSituacaoVisible") %>'
                                    OnClientClick='<%# "voltarPeca(" + Eval("IdProdPedProducao") + "); return false;"%>' />                
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (bool)Eval("ExibirRelatorioPedido") && Request["Producao"] != "1" %>'>
                    <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, true); return false">
                        <img border="0" src="../../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, false); return false">
                    <img border="0" src="../../Images/script_go.gif" />
                </a>
                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../CadFotos.aspx?id=<%# Eval("IdPedido") %>&amp;tipo=pedido&#039;); return false;'>
                    <img border="0px" src="../../Images/Clipe.gif" /></a></asp:PlaceHolder>
                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>' />
                <asp:ImageButton ID="imgPararPecaProducao" runat="server" ImageUrl='<%# (bool)Eval("PecaParadaProducao") ? "~/Images/stop_red.png" : "~/Images/stop_blue.png" %>'
                    OnClientClick='<%# "openSetMotivPararPecaProducao(" + Eval("IdProdPedProducao") + ", " + Eval("PecaParadaProducao").ToString().ToLower() + "); return false" %>'
                    Visible='<%# Eval("ExibirPararPecaProducao") %>' Width="16" Height="16" ToolTip='<%# (bool)Eval("PecaParadaProducao") ? "Retornar peça para produção?" : "Parar peça na produção?" %>' />
                <uc4:ctrllogpopup ID="ctrlLogPopup1" runat="server" Tabela="ProdPedProducao" IdRegistro='<%# Eval("idProdPedProducao") %>' />
                <asp:HiddenField ID="hdfIdSetor" runat="server" Value='<%# Eval("IdSetor") %>' />
                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                <asp:HiddenField ID="hdfPedidoCancelado" runat="server" Value='<%# Eval("PedidoCancelado") %>' />
                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinha") %>' />
                <asp:ImageButton runat="server" ID="imgPopup" ImageUrl="~/Images/Nota.gif" Visible='<%# Eval("PecaReposta") %>'
                    OnClientClick='<%# Eval("IdProdPedProducao", "abrirDetalhesReposicao({0}); return false") %>'
                    ToolTip="Detalhes Reposição Peça" />
                <asp:ImageButton ID="imgLogEstornoCarregamento" runat="server" OnClientClick='<%# "openLogEstornoCarregamento(" + Eval("IdProdPedProducao") + "); return false" %>'
                    ImageUrl="~/Images/log_delete.jpg" ToolTip="Exibir log de estorno de carregamento" Visible='<%# Eval("EstornoCarregamentoVisible") %>' />
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProdLargAlt">
            <EditItemTemplate>
                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Label21" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                <asp:Label ID="Label23" runat="server" Font-Bold="True" Text='<%# Eval("LarguraAltura") %>'></asp:Label>
                <br />
                <asp:Label ID="Label24" runat="server" Font-Size="90%" Text='<%# Eval("DescrTipoPerdaLista") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="CodAplicacao">
            <ItemTemplate>
                <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="CodProcesso">
            <ItemTemplate>
                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <ItemTemplate>
                </td> </tr><asp:HiddenField ID="hdfPpp" runat="server" Value='<%# Eval("IdProdPedProducao") %>' />
                <tr id="ppp_<%# Eval("IdProdPedProducao") %>" style="display: none;">
                    <td colspan="37" align="center">
                        <br />

                        <asp:HiddenField runat="server" ID="hdfIdProdPedProducaoChild" Value='<%# Eval("IdProdPedProducao") %>'/>

                        <asp:GridView GridLines="None" ID="grdPecasParentChild" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                            DataKeyNames="IdProdPedProducao" DataSourceID="odsPecasParentChild" EmptyDataText="Nenhuma peça encontrada."
                            CssClass="gridStyle"
                            PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                            AllowSorting="True" OnDataBound="grdPecasParentChild_DataBound" OnLoad="grdPecasParentChild_Load" OnRowCommand="grdPecasParentChild_RowCommand">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="VoltarPeca" OnClientClick="return confirm('Confirma remoção desta peça desta situação?');"
                                            Visible='<%# Eval("RemoverSituacaoVisible") %>'>
                                    <img src="../../Images/arrow_undo.gif" border="0" title="Remover peça desta situação"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (bool)Eval("ExibirRelatorioPedido") && Request["Producao"] != "1" %>'>
                                            <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, true); return false">
                                                <img border="0" src="../../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                                        <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, false); return false">
                                            <img border="0" src="../../Images/script_go.gif" />
                                        </a>
                                        <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../CadFotos.aspx?id=<%# Eval("IdPedido") %>&amp;tipo=pedido&#039;); return false;'>
                                            <img border="0px" src="../../Images/Clipe.gif" /></a></asp:PlaceHolder>
                                        <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>' />
                                        <asp:ImageButton ID="imgPararPecaProducao" runat="server" ImageUrl='<%# (bool)Eval("PecaParadaProducao") ? "~/Images/stop_red.png" : "~/Images/stop_blue.png" %>'
                                            OnClientClick='<%# "openSetMotivPararPecaProducao(" + Eval("IdProdPedProducao") + ", " + Eval("PecaParadaProducao").ToString().ToLower() + "); return false" %>'
                                            Visible='<%# Eval("ExibirPararPecaProducao") %>' Width="16" Height="16" ToolTip='<%# (bool)Eval("PecaParadaProducao") ? "Retornar peça para produção?" : "Parar peça na produção?" %>' />
                                        <uc4:ctrllogpopup ID="ctrlLogPopup1" runat="server" Tabela="ProdPedProducao" IdRegistro='<%# Eval("idProdPedProducao") %>' />
                                        <asp:HiddenField ID="hdfIdSetor" runat="server" Value='<%# Eval("IdSetor") %>' />
                                        <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                                        <asp:HiddenField ID="hdfPedidoCancelado" runat="server" Value='<%# Eval("PedidoCancelado") %>' />
                                        <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinha") %>' />
                                        <asp:ImageButton runat="server" ID="imgPopup" ImageUrl="~/Images/Nota.gif" Visible='<%# Eval("PecaReposta") %>'
                                            OnClientClick='<%# Eval("IdProdPedProducao", "abrirDetalhesReposicao({0}); return false") %>'
                                            ToolTip="Detalhes Reposição Peça" />
                                        <asp:ImageButton ID="imgLogEstornoCarregamento" runat="server" OnClientClick='<%# "openLogEstornoCarregamento(" + Eval("IdProdPedProducao") + "); return false" %>'
                                            ImageUrl="~/Images/log_delete.jpg" ToolTip="Exibir log de estorno de carregamento" Visible='<%# Eval("EstornoCarregamentoVisible") %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Produto" SortExpression="DescrProdLargAlt">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label21" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                        <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                        <asp:Label ID="Label23" runat="server" Font-Bold="True" Text='<%# Eval("LarguraAltura") %>'></asp:Label>
                                        <br />
                                        <asp:Label ID="Label24" runat="server" Font-Size="90%" Text='<%# Eval("DescrTipoPerdaLista") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Apl." SortExpression="CodAplicacao">
                                    <ItemTemplate>
                                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Proc." SortExpression="CodProcesso">
                                    <ItemTemplate>
                                        <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pgr"></PagerStyle>
                            <EditRowStyle CssClass="edit"></EditRowStyle>
                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                        </asp:GridView>

                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecasParentChild" runat="server" SelectMethod="PesquisarProdutosProducaoFilho"
                            TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                            SelectCountMethod="PesquisarProdutosProducaoFilhoCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                            <SelectParameters>
                                <asp:ControlParameter Name="idProdPedProducaoParent" ControlID="hdfIdProdPedProducaoChild" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <br />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecasParent" runat="server" SelectMethod="PesquisarProdutosProducaoFilho"
    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
    SelectCountMethod="PesquisarProdutosProducaoFilhoCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
    DeleteMethod="VoltarPeca">
    <DeleteParameters>
        <asp:Parameter Name="idProdPedProducao" Type="UInt32" />
        <asp:Parameter Name="idCarregamento" Type="UInt32" />
        <asp:Parameter Name="salvarLog" Type="Boolean" DefaultValue="true" />
    </DeleteParameters>
    <SelectParameters>
        <asp:ControlParameter Name="idProdPedProducaoParent" ControlID="hdfIdProdPedProducao" />
    </SelectParameters>
</colo:VirtualObjectDataSource>


<asp:HiddenField runat="server" ID="hdfIdProdPedProducao" />
