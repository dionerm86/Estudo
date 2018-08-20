<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelContaReceber.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelContaReceber" Title="Contas a Receber" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        if (GetQueryString("encontroContas") == 1)
        {
            $(window).on("unload", fechaJanela);

            if (!window.parent)
                $(window).on("blur", fechaJanela);
        }

        function fechaJanela()
        {
            window.opener.redirectUrl(window.opener.location.href);
        }

        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function escondeConta(controle, idEsconder)
        {
            var hdfIdsEsconder = FindControl("hdfIdsEsconder", "input");

            if (hdfIdsEsconder.value == "")
            {
                hdfIdsEsconder.value = idEsconder;
            }
            else
            {
                hdfIdsEsconder.value += ", " + idEsconder;
            }
            
            controle.parentNode.parentNode.style.display = "none";
        }

    </script>

    <asp:HiddenField ID="hdfIdsEsconder" runat="server" />
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Liberação Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Acerto"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtAcerto" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="NF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="CT-e"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtCte" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Forma Pagto."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpFormaPagto" runat="server" DataSourceID="odsFormaPagto" AppendDataBoundItems="true"
                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label26" runat="server" Text="Troca/Dev." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtTrocaDev" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                DataTextField="NomeFantasia" DataValueField="IdLoja">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" onkeypress="return soNumeros(event, true, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkContasVinculadas" runat="server" Text="Contas Vinculadas" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkNaoBuscarReneg" runat="server" Text="Não buscar contas renegociadas" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Ordenar por"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2" Selected="true">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="3">Pedido</asp:ListItem>
                                <asp:ListItem Value="4">Valor Venc.</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>            
            <td align="center">
                <br />
                <asp:Button ID="btnFechar" runat="server" OnClientClick="closeWindow();" Text="Fechar" />
                <br />
                <br />
                <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdContaR" DataSourceID="odsContasReceber"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Nenhuma conta a receber encontrada." 
                    onrowdatabound="grdProduto_RowDataBound" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" OnLoad="PlaceHolder1_Load"><a href="#"
                                    onclick="escondeConta(this, '<%# Eval("IdContaR") %>'); window.opener.setContaReceber('<%# Eval("IdContaR") %>', '<%# Eval("IdPedido") %>', '<%# Eval("PedidosLiberacao") != null ? Eval("PedidosLiberacao").ToString().Replace("'", "") : String.Empty %>', '<%# Eval("NomeCli").ToString().Replace("'", "") %>', '<%# Eval("ValorVec", "{0:C}") %>', '<%# Eval("DataVec", "{0:d}") %>', '<%# Eval("Juros") %>', '<%# Eval("Multa") %>', '<%# Eval("ObsScript") %>', '<%# Eval("DescricaoContaContabil") %>', window);">
                                    <img src="../Images/Insert.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                                </asp:PlaceHolder>
                                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/ok.gif" OnClientClick='<%# "window.opener.setContaReceberBusca(" + Eval("IdContaR") + "); closeWindow()" %>'
                                    OnLoad="ImageButton3_Load" />                                    
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:TemplateField HeaderText="Data Cad." SortExpression="datacad">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Parc." SortExpression="NumParc">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdCliente" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValorVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("TextoJuros") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoContaContabil" HeaderText="Tipo" 
                            SortExpression="DescricaoContaContabil" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <asp:Label ID="lblCNAB" runat="server" ForeColor="#0066FF" Text="* Contas em azul possuem CNAB gerado."></asp:Label>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
            &nbsp;
            </td>
        </tr>
        <tr runat="server" id="mensagem">
            <td align="center">
                <asp:Label ID="lblMsg" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
                <asp:GridView ID="grdCheque" runat="server" AllowPaging="True" 
                    AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" 
                    CssClass="gridStyle" DataKeyNames="IdCheque" DataSourceID="odsCheques" 
                    EditRowStyle-CssClass="edit" GridLines="None" PagerStyle-CssClass="pgr">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <EditItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Eval("Num") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Eval("NumChequeComDig") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("Banco") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label23" runat="server" Text='<%# Eval("Agencia") %>' 
                                    Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtAgencia" runat="server" 
                                    onchange="FindControl('hdfAgencia', 'input').value = this.value" 
                                    Text='<%# Eval("Agencia") %>' Visible='<%# Eval("EditarAgenciaConta") %>' 
                                    Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAgencia" runat="server" 
                                    Value='<%# Bind("Agencia") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label24" runat="server" Text='<%# Eval("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <EditItemTemplate>
                                <asp:Label ID="Label25" runat="server" Text='<%# Eval("Conta") %>' 
                                    Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtConta" runat="server" 
                                    onchange="FindControl('hdfConta', 'input').value = this.value" 
                                    Text='<%# Eval("Conta") %>' Visible='<%# Eval("EditarAgenciaConta") %>' 
                                    Width="70px"></asp:TextBox>
                                <asp:HiddenField ID="hdfConta" runat="server" Value='<%# Bind("Conta") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label26" runat="server" Text='<%# Eval("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" 
                                    Text='<%# Bind("Titular") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label27" runat="server" Text='<%# Eval("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:Label ID="Label28" runat="server" Text='<%# Eval("ValorRecebido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label29" runat="server" Text='<%# Eval("ValorRecebido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <ItemTemplate>
                                <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("DataVencLista") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVencGrid" runat="server" onkeypress="return false;" 
                                    Text='<%# Bind("DataVencString") %>' Visible='<%# Eval("AlterarDataVenc") %>' 
                                    Width="70px"></asp:TextBox>
                                <asp:ImageButton ID="imgDataVencGrid" runat="server" ImageAlign="AbsMiddle" 
                                    ImageUrl="~/Images/calendario.gif" 
                                    OnClientClick="return SelecionaData('txtDataVencGrid', this)" ToolTip="Alterar" 
                                    Visible='<%# Eval("AlterarDataVenc") %>' />
                                <asp:Label ID="lblDataVenc0" runat="server" Text='<%# Eval("DataVencLista") %>' 
                                    Visible='<%# !(bool)Eval("AlterarDataVenc") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="2" 
                                    Text='<%# Bind("Obs") %>' TextMode="MultiLine" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label31" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle />
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
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> Adicionar Todas</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasReceber" 
        runat="server" MaximumRowsParameterName="pageSize"
        SelectMethod="GetForEfetuarAcerto" StartRowIndexParameterName="startRow" 
        TypeName="Glass.Data.DAL.ContasReceberDAO" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        OldValuesParameterFormatString="original_{0}" SkinID="" 
        EnablePaging="True" SelectCountMethod="GetForEfetuarAcertoCount" 
        SortParameterName="sortExpression" >
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtAcerto" Name="idAcerto" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNF" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="chkContasVinculadas" Name="contasVinculadas" PropertyName="Checked"
                Type="Boolean" />
            <asp:ControlParameter ControlID="drpTipo" Name="tipoContaContabil" 
                PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="chkNaoBuscarReneg" Name="naoBuscarReneg" PropertyName="Checked"
                Type="Boolean" />
            <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:Parameter Name="buscarContasValorZerado" Type="Boolean" DefaultValue="false" />
            <asp:ControlParameter ControlID="txtCTe" Name="numeroCTe" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtTrocaDev" Name="idTrocaDevolucao" PropertyName="Text" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaContasReceber"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCheques" runat="server" 
        DataObjectTypeName="Glass.Data.Model.Cheques" MaximumRowsParameterName="" 
        SelectMethod="GetDevolvidosPorCliente" StartRowIndexParameterName="" 
        TypeName="Glass.Data.DAL.ChequesDAO" >
        <SelectParameters>
            <asp:QueryStringParameter Name="idCliente" QueryStringField="idCli" 
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
