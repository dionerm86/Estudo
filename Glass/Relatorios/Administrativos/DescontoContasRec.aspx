<%@ Page Title="Desconto/Acréscimo em Parcelas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="DescontoContasRec.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.DescontoContasRec" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function limparVenc() {
            FindControl("ctrlDataIni_txtData", "input").value = "";
            FindControl("ctrlDataFim_txtData", "input").value = "";
        }

        function limparDesc() {
            FindControl("ctrlDataDescIni_txtData", "input").value = "";
            FindControl("ctrlDataDescFim_txtData", "input").value = "";
        }

        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtIdPedido", "input");
            idPedido = idPedido != null ? (idPedido.value.length > 0 ? idPedido.value : "0") : "0";
            
            var idLiberarPedido = FindControl("txtIdLiberarPedido", "input");
            idLiberarPedido = idLiberarPedido != null ? (idLiberarPedido.value.length > 0 ? idLiberarPedido.value : "0") : "0";

            var idLoja = FindControl("drpLoja", "select");
            idLoja = idLoja != null ? idLoja.value : "0";
            
            var idFunc = FindControl("drpFuncionario", "select").value;
            var valorIniAcres = FindControl("txtValorIniAcres", "input").value;
            valorIniAcres = valorIniAcres != "" ? valorIniAcres : "0";
            
            var valorFimAcres = FindControl("txtValorFimAcres", "input").value;
            valorFimAcres = valorFimAcres != "" ? valorFimAcres : "0";
            
            var valorIniDesc = FindControl("txtValorIniDesc", "input").value;
            valorIniDesc = valorIniDesc != "" ? valorIniDesc : "0";
            
            var valorFimDesc = FindControl("txtValorFimDesc", "input").value;
            valorFimDesc = valorFimDesc != "" ? valorFimDesc : "0";

            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataDescIni = FindControl("ctrlDataDescIni_txtData", "input").value;
            var dataDescFim = FindControl("ctrlDataDescFim_txtData", "input").value;
            
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var ddlOrigem = FindControl("ddlOrigem", "select");
            var idOrigemDesconto = ddlOrigem != null ? ddlOrigem.value : "";

            openWindow(600, 800, "RelBase.aspx?rel=DescParc&idPedido=" + idPedido + "&idLiberarPedido=" + idLiberarPedido + "&idLoja=" + idLoja + "&idFunc=" + idFunc +
                "&valorIniAcres=" + valorIniAcres + "&valorFimAcres=" + valorFimAcres + "&valorIniDesc=" + valorIniDesc + "&valorFimDesc=" + valorFimDesc +
                "&idCliente=" + idCli + "&nomeCli=" + nomeCli + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&dataDescIni=" + dataDescIni + "&dataDescFim=" + dataDescFim +
                "&idOrigemDesconto=" + idOrigemDesconto + "&exportarExcel=" + exportarExcel);

            return false;
        }
        
        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = DescontoContasRec.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPedido" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesqPed', 'input')"></asp:TextBox>
                            <asp:ImageButton ID="imgPesqPed" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLiberarPedido" runat="server" ForeColor="#0066FF" Text="Liberação"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdLiberarPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesqLib', 'input')"></asp:TextBox>
                            <asp:ImageButton ID="imgPesqLib" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Vendedor (Assoc. Pedido)"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cód. Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Nome/Apelido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período Venc."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                            <asp:ImageButton ID="imgLimparVenc" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="limparVenc(); return false" ToolTip="Limpar período venc." />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Data do desconto/acréscimo"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataDescIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataDescFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                            <asp:ImageButton ID="imgLimparDesc" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="limparDesc(); return false" ToolTip="Limpar data desconto" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                    <td>
                            <asp:Label ID="Label9" runat="server" Text="Origem Desconto/Acréscimo" ForeColor="#0066FF"
                                OnLoad="ddlOrigem_Load"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOrigem" runat="server" DataSourceID="odsOrigem" DataTextField="Descricao"
                                DataValueField="idOrigemTrocaDesconto" OnLoad="ddlOrigem_Load" AppendDataBoundItems=true>
                                <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource ID="odsOrigem" runat="server" Culture="pt-BR" SelectMethod="GetList"
                                TypeName="Glass.Data.DAL.OrigemTrocaDescontoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" OnLoad="ddlOrigem_Load"/>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Acresc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorIniAcres" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFimAcres" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Valor Desc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorIniDesc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFimDesc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
    </td> </tr>
    <tr>
        <td align="center">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:GridView GridLines="None" ID="grdDescParc" runat="server" AllowPaging="True"
                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsDescParc" CssClass="gridStyle"
                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                EmptyDataText="Nenhuma parcela com desconto encontrada.">
                <Columns>
                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                    <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                    <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdCli" />
                    <asp:BoundField DataField="NomeFunc" HeaderText="Func." SortExpression="NomeFunc" />
                    <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                    <asp:BoundField DataField="DescrTipoEntrega" HeaderText="Tipo Entrega" SortExpression="TipoEntrega" />
                    <asp:BoundField DataField="ValorVec" DataFormatString="{0:C}" HeaderText="Valor"
                        SortExpression="ValorVec">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataVec" DataFormatString="{0:d}" HeaderText="Data Venc."
                        SortExpression="DataVec" />
                    <asp:BoundField DataField="ValorRec" DataFormatString="{0:C}" HeaderText="Valor Rec."
                        SortExpression="ValorRec">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataRec" DataFormatString="{0:d}" HeaderText="Data Rec."
                        SortExpression="DataRec" />
                    <asp:BoundField DataField="Desconto" DataFormatString="{0:C}" HeaderText="Valor Desc."
                        SortExpression="Desconto">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Acrescimo" DataFormatString="{0:c}" HeaderText="Valor Acrésc."
                        SortExpression="Acrescimo"></asp:BoundField>
                    <asp:BoundField DataField="DataDescAcresc" DataFormatString="{0:d}" HeaderText="Data Desc./Acrésc."
                        SortExpression="DataDescAcresc" />
                    <asp:BoundField DataField="NomeFuncDesc" HeaderText="Resp. Desc./Acrésc." SortExpression="NomeFuncDesc" />
                    <asp:BoundField DataField="MotivoDescontoAcresc" HeaderText="Motivo Desc./Acrésc."
                        SortExpression="MotivoDescontoAcresc" />
                        <asp:BoundField DataField="DescrOrigemDescontoAcrescimo" HeaderText="Origem" SortExpression="DescrOrigemDescontoAcrescimo" />
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDescParc" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                SelectCountMethod="GetCountContaComDesconto" SelectMethod="GetListContaComDesconto"
                SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasReceberDAO"
                >
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtIdLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtValorIniAcres" Name="valorIniAcres" 
                        PropertyName="Text" Type="Decimal" />
                    <asp:ControlParameter ControlID="txtValorFimAcres" Name="valorFimAcres" 
                        PropertyName="Text" Type="Decimal" />
                    <asp:ControlParameter ControlID="txtValorIniDesc" Name="valorIniDesc" 
                        PropertyName="Text" Type="Decimal" />
                    <asp:ControlParameter ControlID="txtValorFimDesc" Name="valorFimDesc" 
                        PropertyName="Text" Type="Decimal" />
                    <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataDescIni" Name="dataDescIni" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataDescFim" Name="dataDescFim" PropertyName="DataString"
                        Type="String" />
                        <asp:ControlParameter ControlID="ddlOrigem" Name="idOrigemDesconto" PropertyName="Text"
                            Type="Uint32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
                TypeName="Glass.Data.DAL.FuncionarioDAO">
            </colo:VirtualObjectDataSource>
        </td>
    </tr>
    <tr>
        <td align="center">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td align="center">
            <br />
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                    <img alt="" border="0" src="../../Images/printer.png" />  Imprimir</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td align="center">
            &nbsp;
        </td>
    </tr>
    </table>
</asp:Content>
