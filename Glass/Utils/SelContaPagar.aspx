<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelContaPagar.aspx.cs" Inherits="Glass.UI.Web.Utils.SelContaPagar"
    Title="Contas a Pagar" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

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

        function setContaPagar(idContaPg, idCompra, idCustoFixo, idImpostoServ, idFornec, nomeFornec, valorVenc, dataVenc, descrPlanoConta) {

            window.opener.setContaPagar(idContaPg, idCompra, idCustoFixo, idImpostoServ, idFornec, nomeFornec, valorVenc, dataVenc, descrPlanoConta, window);
        }
    
        function escondeConta(controle) {
            controle.parentNode.parentNode.style.display = "none";
        }

        function selFornecedor() {

            var txtNome = FindControl("txtNome", "input");
            var txtFornecedor = FindControl("txtFornecedor", "input");

            if (txtNome.value == "" && txtFornecedor.value == "") {

                openWindow(570, 760, '../Utils/SelFornec.aspx');
                return false;
            }

            cOnClick('imgPesq', null);
        }

        function setFornec(idFornec, nomeFornec) {

            FindControl("txtFornecedor", "input").value = idFornec;
            FindControl("txtNome", "input").value = nomeFornec;

            cOnClick('imgPesq', null);
        }

        function getFornec(idFornec) {

            var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');
            var chkContasVinculadas = FindControl("chkContasVinculadas", "input");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            if (chkContasVinculadas != null && chkContasVinculadas.checked == false)
                 FindControl("txtNome", "input").value = retorno[1];
        }

        function verificarPodeDigitarNomeFornecedor() {
            var txtNome = FindControl("txtNome", "input");
            var chkContasVinculadas = FindControl("chkContasVinculadas", "input");

            if (chkContasVinculadas != null && chkContasVinculadas.checked &&
                txtNome != null && txtNome.value != "")
            {
                txtNome.value = "";
                alert('Não é possível informar o nome do fornecedor caso o filtro Contras vinculadas esteja marcado, pois, ' +
                    'caso sejam buscadas as contas vinculadas somente um fornecedor deve ser informado.');

                return false;
            }
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Compra"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Imposto/Serv."></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumImpostoServ" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="NF/Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="CT-e"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCte" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                DataTextField="NomeFantasia" DataValueField="IdLoja">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onblur="verificarPodeDigitarNomeFornecedor();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="return selFornecedor();">
                                    <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                                </asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Tipo Conta"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoConta" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Comissão</asp:ListItem>
                                <asp:ListItem Value="2">Imposto/Serviço</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                 ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Height="16px" Width="16px" />
                        </td>
                        
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkCustoFixo" runat="server" OnCheckedChanged="chkCustoFixo_CheckedChanged"
                                Text="Contas de custo fixo" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkContasVinculadas" runat="server" Text="Contas vinculadas"
                                onclick="verificarPodeDigitarNomeFornecedor();" AutoPostBack="True" />
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
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdContaPg" DataSourceID="odsContasPagar"
                    EmptyDataText="Nenhuma conta a pagar encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="escondeConta(this);setContaPagar('<%# Eval("IdContaPg") %>', '<%# Eval("IdCompra") %>', '<%# Eval("IdCustoFixo") %>', '<%# Eval("IdImpostoServ") %>', '<%# Eval("IdFornec") %>', '<%# Eval("NomeFornec")!= null ? Eval("NomeFornec").ToString().Replace("'", "") : "" %>', '<%# Eval("ValorVenc", "{0:C}") %>', '<%# Eval("DataVenc", "{0:d}") %>', '<%# Eval("DescrPlanoConta") != null ? Eval("DescrPlanoConta").ToString().Replace("'", "") : "" %>');">
                                    <img alt="Selecionar" border="0" src="../Images/insert.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrContaPagar" HeaderText="Referente a" SortExpression="DescrContaPagar" />
                        <asp:BoundField DataField="ValorVenc" DataFormatString="{0:C}" HeaderText="Valor"
                            SortExpression="ValorVenc" />
                        <asp:BoundField DataField="DataVenc" DataFormatString="{0:d}" HeaderText="Vencimento"
                            SortExpression="DataVenc" />
                        <asp:BoundField DataField="FormaPagtoCompra" HeaderText="Forma Pagto." SortExpression="FormaPagtoCompra" />
                        <asp:BoundField DataField="NumBoleto" HeaderText="Cheque/Boleto" SortExpression="NumBoleto" />
                        <asp:BoundField DataField="BoletoChegouString" HeaderText="Boleto Chegou?" SortExpression="BoletoChegouString" />
                        <asp:BoundField DataField="DescricaoContaContabil" HeaderText="Tipo" 
                            SortExpression="DescricaoContaContabil" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> Adicionar Todas</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetNaoPagasCount" SelectMethod="GetNaoPagas" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" 
        TypeName="Glass.Data.DAL.ContasPagarDAO" >
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumImpostoServ" Name="idImpostoServ" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNF" Name="nf" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="txtPrecoInicial" Name="valorInicial" PropertyName="Text"
                Type="Single" />
            <asp:ControlParameter ControlID="txtPrecoFinal" Name="valorFinal" PropertyName="Text"
                Type="Single" />
            <asp:ControlParameter ControlID="drpTipo" Name="tipoContaContabil" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="drpTipoConta" Name="tipoConta" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="chkCustoFixo" Name="custoFixo" 
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text"
                            Type="Int32" />
            <asp:ControlParameter ControlID="chkContasVinculadas" Name="contasVinculadas" PropertyName="Checked"  />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>