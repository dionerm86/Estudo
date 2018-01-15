<%@ Page Title="Parcelas de Cartão Quitadas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstParcCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstParcCartao" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
<script type="text/javascript">

    function openRpt(exportarExcel) {
        var idPedido = "", idAcerto = "", idLiberarPedido = "", dataFim = "", numCliente = "", nomeCliente = "",
            tipoEntrega = "", idAcertoCheque = "", dataIni = "";

            idPedido = FindControl("txtNumPedido", "input").value;
            idLiberarPedido = FindControl("txtNumLiberacao", "input"); idLiberarPedido = idLiberarPedido != null ? idLiberarPedido.value : "";
            idAcerto = FindControl("txtNumAcerto", "input").value;
            numCliente = FindControl("txtNumCli", "input").value;
            tipoEntrega = FindControl("drpTipoEntrega", "select").value;
            nomeCliente = FindControl("txtNome", "input").value;
            dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            idAcertoCheque = FindControl("txtAcertoCheque", "input").value;
            nCNI = FindControl("txtnumCNI", "input").value;      

        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ParcCartaoQuitadas&idPedido=" + idPedido
            + "&idLiberarPedido=" + idLiberarPedido
            + "&idAcerto=" + idAcerto
            + "&idCliente=" + numCliente
            + "&tipoEntrega=" + tipoEntrega
            + "&nome=" + nomeCliente
            + "&dataIni=" + dataIni
            + "&dataFim=" + dataFim
            + "&idAcertoCheque=" + idAcertoCheque           
            + "&nCNI" + nCNI +
            "&exportarExcel=" + exportarExcel);
    }

    function openMotivoCanc(idContaRParcCartao) {
        openWindow(180, 410, "../Utils/SetMotivoCancPagto.aspx?idContaRParcCartao=" + idContaRParcCartao);
        return false;
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap" runat="server" id="pedidoTitulo">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="pedido">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="acertoTitulo">
                            <asp:Label ID="Label15" runat="server" Text="Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="acerto">
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="liberarPedidoTitulo">
                            <asp:Label ID="Label3" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="liberarPedido">
                            <asp:TextBox ID="txtNumLiberacao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap" runat="server" id="vencDataFim">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap" runat="server" id="clienteTitulo">
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap" runat="server" id="cliente">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td runat="server" id="tipoEntregaTitulo">
                            <asp:Label ID="Label14" runat="server" Text="Tipo Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td runat="server" id="tipoEntrega">
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="true"
                                AutoPostBack="true" DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Acerto de Cheque" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap" runat="server" id="Td1">
                            <asp:TextBox ID="txtAcertoCheque" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server"  
                                ForeColor="#0066FF" Text="Nº CNI"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtnumCNI"  runat="server" 
                                Width="160px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>

                         <td>
                            <asp:Label ID="Label6" runat="server"  
                                ForeColor="#0066FF" Text="N° de autorização"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAuto"  runat="server" 
                                Width="160px" onkeypress="return soNumeros(event, true, true);" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>

                          <td>
                            <asp:Label ID="Label9" runat="server"  
                                ForeColor="#0066FF" Text="N° de Estabelecimento"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumEstabelecimento"  runat="server" 
                                Width="160px" onkeypress="return soNumeros(event, true, true);" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>

                           <td>
                            <asp:Label ID="Label11" runat="server"  
                                ForeColor="#0066FF" Text="Ult. Digito Cartão"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtUltDigCartao"  runat="server" 
                                Width="160px" onkeypress="return soNumeros(event, true, true);" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdContaR" DataSourceID="odsParcCartao"
                    EmptyDataText="Nenhuma parcela recebida encontrada.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="plhCancelar" runat="server" >
                                    <a href="#" onclick="openMotivoCanc('<%# Eval("IdContaR") %>');return false;">
                                        <img border="0" src="../Images/ExcluirGrid.gif" /></a></asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência">
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label18" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="ContaBanco">
                            <ItemTemplate>
                                <asp:Label ID="Label19" runat="server" Text='<%# Bind("ContaBanco") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Eval("ContaBanco") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCli">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="DescrPlanoConta">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrPlanoConta") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label23" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataRec" DataFormatString="{0:d}" HeaderText="Data Quitação" SortExpression="DataRec" />
                        <asp:BoundField DataField="DataVec" DataFormatString="{0:d}" HeaderText="Data Vencimento" SortExpression="DataVec" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />                        
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdContaR") %>'
                                    Tabela="ContasReceber" />
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcCartao" runat="server" SelectCountMethod="GetParcCartaoCount"
        SelectMethod="GetParcCartao" TypeName="Glass.Data.DAL.ContasReceberDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
        <DeleteParameters>
            <asp:Parameter Name="idContaR" Type="UInt32" />
        </DeleteParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumLiberacao" Name="idLiberarPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text"
                Type="UInt32" />
            <asp:Parameter Name="idLoja" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
            <asp:Parameter Name="tipoCartao" Type="UInt32" />
            <asp:ControlParameter ControlID="txtAcertoCheque" Name="idAcertoCheque" PropertyName="Text"
                Type="UInt32" />
            <asp:Parameter Name="agrupar" Type="Boolean" DefaultValue="false" />
            <asp:Parameter Name="recebidas" Type="Boolean" DefaultValue="true" />
            <asp:Parameter DefaultValue="" Name="dtCadIni" Type="String" />
            <asp:Parameter DefaultValue="" Name="dtCadFim" Type="String" />
            <asp:ControlParameter ControlID="txtnumCNI" Name="nCNI" PropertyName="Text" />  
            <asp:Parameter Name="valorIni" />
            <asp:Parameter Name="valorFim"  />
            <asp:Parameter Name="tipoRecbCartao" />      
            <asp:ControlParameter ControlID="txtNumAuto"  Name="numAutCartao" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtNumEstabelecimento"  Name="numEstabCartao" PropertyName="Text" Type="String" />
           <asp:ControlParameter ControlID="txtUltDigCartao" Name="ultDigCartao" PropertyName="Text" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
