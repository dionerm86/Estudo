<%@ Page Title="Histórico de Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstHistoricoCliente.aspx.cs" Inherits="Glass.UI.Web.Listas.LstHistoricoCliente" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(txtIdCliente) {
            if (txtIdCliente.value == "")
                return false;
        
            var retorno = MetodosAjax.GetCli(txtIdCliente.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                txtIdCliente.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function openRpt() {
            var idCliente = FindControl("txtNumCli", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var dtIniVenc = FindControl("ctrlDataIniVenc_txtData", "input").value;
            var dtFimVenc = FindControl("ctrlDataFimVenc_txtData", "input").value;
            var dtIniRec = FindControl("ctrlDataIniRec_txtData", "input").value;
            var dtFimRec = FindControl("ctrlDataFimRec_txtData", "input").value;
            var dtIniCad = FindControl("ctrlDataIniCad_txtData", "input").value;
            var dtFimCad = FindControl("ctrlDataFimCad_txtData", "input").value;
            var vIniVenc = FindControl("txtPrecoInicialVenc", "input").value;
            var vFimVenc = FindControl("txtPrecoFinalVenc", "input").value;
            var vIniRec = FindControl("txtPrecoInicialRec", "input").value;
            var vFimRec = FindControl("txtPrecoFinalRec", "input").value;
            var emAberto = FindControl("chkEmAberto", "input").checked;
            var recEmDia = FindControl("chkRecebidasEmDia", "input").checked;
            var recComAtraso = FindControl("chkRecebidasComAtraso", "input").checked;            
            var buscarParcCartao = FindControl("chkBuscarParcCartao", "input").checked;
            var contasRenegociadas = FindControl("drpContasRenegociadas", "select").value;
            var buscaPedRepoGarantia = FindControl("chkPedGarantiaRepos", "input").checked;
            var buscarChequeDevolvido = FindControl("chkBuscarChequeDevolvido", "input").checked;
            var sort = FindControl("drpSort", "select").value;

            var queryString = (idCliente == "" ? "&idCliente=0" : "&idCliente=" + idCliente) + (idPedido == "" ? "&idPedido=0" : "&idPedido=" + idPedido) + 
                "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc + "&dtIniRec=" + dtIniRec + "&dtFimRec=" + dtFimRec + "&dtIniCad=" + dtIniCad + "&dtFimCad=" +
                dtFimCad + "&vIniVenc=" + (vIniVenc == "" ? 0 : vIniVenc) + "&vFimVenc=" + (vFimVenc == "" ? 0 : vFimVenc) + "&vIniRec=" +
                (vIniRec == "" ? 0 : vIniRec) + "&sort=" + sort + "&vFimRec=" + (vFimRec == "" ? 0 : vFimRec) + "&emAberto=" + emAberto + 
                "&recEmDia=" + recEmDia + "&recComAtraso=" + recComAtraso + "&buscarParcCartao=" + buscarParcCartao + "&contasRenegociadas=" + contasRenegociadas +
                "&buscaPedRepoGarantia=" + buscaPedRepoGarantia + "&buscarChequeDevolvido=" + buscarChequeDevolvido;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=HistoricoCliente" + queryString);

            return false;
        }

        function openRptReferencia(IdPedido, IdAcerto, IdLiberarPedido, IdObra, IdTrocaDevolucao, IdSinal, IdDevolucaoPagto, IdAcertoCheque) {
            if (IdTrocaDevolucao > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=" + IdTrocaDevolucao);
            else if (IdPedido > 0)
                openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + IdPedido + "&tipo=0");
            else if (IdAcerto > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + IdAcerto);
            else if (IdLiberarPedido > 0)
                openWindow(600, 800, "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + IdLiberarPedido);
            else if (IdObra > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Obra&idObra=" + IdObra);
            else if (IdSinal > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Sinal&IdSinal=" + IdSinal);
            else if (IdDevolucaoPagto > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DevolucaoPagto&idDevolucaoPagto=" + IdDevolucaoPagto);
            else if (IdAcertoCheque > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + IdAcertoCheque);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="if (FindControl('txtNumCli', 'input').value == '') openWindow(570, 760, '../Utils/SelCliente.aspx'); else return true; "
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <table id="tbHist" width="100%" runat="server">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label19" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataIniVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataFimVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label24" runat="server" Text="Período Cad." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataIniCad" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFimCad" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label10" runat="server" Text="Recebida entre" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataIniRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFimRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoInicialVenc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        até
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoFinalVenc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            Width="16px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label15" runat="server" Text="Valor Rec." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoInicialRec" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        até
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoFinalRec" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" Width="16px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label20" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpSort" runat="server" AutoPostBack="True">
                                            <asp:ListItem Value="1">Vencimento</asp:ListItem>
                                            <asp:ListItem Value="2">Recebimento</asp:ListItem>
                                            <asp:ListItem Value="3">Situação</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label36" runat="server" Text="Contas Renegociadas" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpContasRenegociadas" runat="server" AutoPostBack="True">
                                            <asp:ListItem Value="1">Incluir contas renegociadas</asp:ListItem>
                                            <asp:ListItem Value="2">Apenas contas renegociadas</asp:ListItem>
                                            <asp:ListItem Value="3" Selected="True">Não incluir contas renegociadas</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkPedGarantiaRepos" runat="server" Checked="False" Text="Buscar Pedidos de Reposição/Garantia" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkBuscarParcCartao" runat="server" Text="Buscar Parcelas de Cartão" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkBuscarChequeDevolvido" runat="server" Checked="True" Text="Buscar Cheque Devolvido" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chkEmAberto" runat="server" Checked="True" Text="Em aberto" ForeColor="Red" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkRecebidasEmDia" runat="server" Checked="True" Text="Recebidas em dia"
                                            ForeColor="Green" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkRecebidasComAtraso" runat="server" Checked="True" Text="Recebidas com atraso"
                                            ForeColor="Blue" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" Width="16px" />
                                    </td>
                                </tr>
                            </table>
                            <asp:Button ID="btnLimparFiltros" runat="server" OnClick="btnLimparFiltros_Click"
                                Text="Limpar Filtros" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                                DataKeyNames="IdContaR" DataSourceID="odsContasReceber" CssClass="gridStyle"
                                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                EmptyDataText="Nenhuma conta a receber/recebida encontrada." AllowPaging="True"
                                PageSize="15" AllowSorting="True" OnDataBound="grdConta_DataBound">
                                <PagerSettings PageButtonCount="20" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbReferencia" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                                OnClientClick='<%# "openRptReferencia(" +
                                                (Eval("IdPedido") == "" || Eval("IdPedido") == null ? 0 : Eval("IdPedido")) + "," +
                                                (Eval("IdAcerto") == "" || Eval("IdAcerto") == null ? 0 : Eval("IdAcerto")) + "," +
                                                (Eval("IdLiberarPedido") == "" || Eval("IdLiberarPedido") == null ? 0 : Eval("IdLiberarPedido")) + "," +
                                                (Eval("IdObra") == "" || Eval("IdObra") == null ? 0 : Eval("IdObra")) + "," +
                                                (Eval("IdTrocaDevolucao") == "" || Eval("IdTrocaDevolucao") == null ? 0 : Eval("IdTrocaDevolucao")) + "," +
                                                (Eval("IdSinal") == "" || Eval("IdSinal") == null ? 0 : Eval("IdSinal")) + "," +
                                                (Eval("IdDevolucaoPagto") == "" || Eval("IdDevolucaoPagto") == null ? 0 : Eval("IdDevolucaoPagto")) + "," +
                                                (Eval("IdAcertoCheque") == "" || Eval("IdAcertoCheque") == null ? 0 : Eval("IdAcertoCheque")) + "); return false" %>'
                                                ToolTip="Relatório Referência" />
                                            <asp:HiddenField ID="hdfColor" runat="server" Value='<%# Eval("Color") %>' />
                                            <asp:HiddenField ID="hdfTotalEmAberto" runat="server" Value='<%# Eval("TotalEmAberto", "{0:C}") %>' />
                                            <asp:HiddenField ID="hdfTotalRecEmDia" runat="server" Value='<%# Bind("TotalRecEmDia", "{0:C}") %>' />
                                            <asp:HiddenField ID="hdfTotalRecComAtraso" runat="server" Value='<%# Eval("TotalRecComAtraso", "{0:C}") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NumParcString" HeaderText="Parc." SortExpression="NumParcString" />
                                    <asp:BoundField DataField="NomeCli" HeaderText="Cliente" SortExpression="NomeCli" />
                                    <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                                    <asp:BoundField DataField="ValorVec" HeaderText="Valor" SortExpression="ValorVec"
                                        DataFormatString="{0:C}">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DataVec" DataFormatString="{0:d}" HeaderText="Data. Venc."
                                        SortExpression="DataVec" />
                                    <asp:BoundField DataField="ValorRec" DataFormatString="{0:C}" HeaderText="Valor Rec."
                                        SortExpression="ValorRec">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DataRec" HeaderText="Data Rec." SortExpression="DataRec"
                                        DataFormatString="{0:d}">
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NomeFunc" HeaderText="Recebida por" SortExpression="NomeFunc" />
                                    <asp:BoundField DataField="DestinoRec" HeaderText="Localização" SortExpression="DestinoRec" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
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
                            <table id="tbTotais" runat="server">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label21" runat="server" ForeColor="Red" Text="Total em aberto"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalAberto" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label22" runat="server" ForeColor="Green" Text="Total recebidas em dia"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalRecEmDia" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label23" runat="server" ForeColor="Blue" Text="Total recebidas com atraso"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalRecComAtraso" runat="server"></asp:Label>
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
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasReceber" runat="server" MaximumRowsParameterName="pageSize"
                                SelectMethod="GetListHist" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasReceberDAO"
                                EnablePaging="True" SelectCountMethod="GetCountHist" 
                                SortParameterName="sortExpression" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" 
                                        PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="ctrlDataIniVenc" Name="dataIniVenc" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataFimVenc" Name="dataFimVenc" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataIniRec" Name="dataIniRec" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataFimRec" Name="dataFimRec" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataIniCad" Name="dataIniCad" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataFimCad" Name="dataFimCad" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtPrecoInicialVenc" Name="vIniVenc" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoFinalVenc" Name="vFinVenc" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoInicialRec" Name="vIniRec" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoFinalRec" Name="vFinRec" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="chkEmAberto" Name="emAberto" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkRecebidasEmDia" Name="recEmDia" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkRecebidasComAtraso" Name="recComAtraso" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkBuscarParcCartao" Name="buscarParcCartao" 
                                        PropertyName="Checked" Type="Boolean" />
                                    <asp:ControlParameter ControlID="drpContasRenegociadas" Name="contasRenegociadas" PropertyName="SelectedValue"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="chkPedGarantiaRepos" Name="buscaPedRepoGarantia"
                                        PropertyName="Checked" Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkBuscarChequeDevolvido" 
                                        Name="buscarChequeDevolvido" PropertyName="Checked" Type="Boolean" />
                                    <asp:ControlParameter ControlID="drpSort" Name="sort" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
