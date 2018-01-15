<%@ Page Title="Gerar Ordem de Carga" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadOrdemCarga.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadOrdemCarga" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlPopupTela.ascx" TagName="ctrlPopupTela" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .boxFlutuante
        {
            margin-left: 5px;
            margin-top: 5px;
            float: left;
            position: relative;
            width: 190px;
            background-color: #fff;
            border: 1px solid #C0C0C0;
            text-align: left;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            -webkit-box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            -moz-box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            padding: 5px;
        }
    </style>

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("txtIdPedido", "input").value = "";
        }

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function exibirOCs(botao, idCliente) {

            var linha = document.getElementById("ocs_" + idCliente);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " OC's";
        }

        function gerarOC(idCliente, idRota) {
            if (idCliente == null || idCliente == 0) {
                alert("Falha ao gerar OC. Cliente não encontrado.");
                return false;
            }

            if (idRota == null || idRota == 0) {
                alert("Falha ao gerar OC. Rota não encontrada.");
                return false;
            }

            var dtEntPedidoIni = FindControl("txtDataIni", "input").value;
            var dtEntPedidoFin = FindControl("txtDataFin", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var tipoOC = FindControl("drpTipoOC", "select").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            var chkPedidosObs = FindControl("chkPedidosObs", "input").checked;
            var chkFastDelivery = FindControl("chkFastDelivery", "input").checked;
            var codRotasExternas = FindControl("cblRotaExterna", "select").itensText();
            var idClienteExterno = FindControl("txtNumCliExterno", "input").value;
            var nomeClienteExterno = FindControl("txtNomeClienteExterno", "input").value;

            var queryString = "popup=true";
            queryString += "&idCliente=" + idCliente;
            queryString += "&idRota=" + idRota;
            queryString += "&dtEntPedidoIni=" + dtEntPedidoIni;
            queryString += "&dtEntPedidoFin=" + dtEntPedidoFin;
            queryString += "&idLoja=" + idLoja;
            queryString += "&tipoOC=" + tipoOC;
            queryString += "&situacao=" + situacao;
            queryString += "&pedidosObs=" + chkPedidosObs;
            queryString += "&fastDelivery=" + chkFastDelivery;
            queryString += "&codRotasExternas=" + codRotasExternas;
            queryString += "&idClienteExterno=" + idClienteExterno;
            queryString += "&nomeClienteExterno=" + nomeClienteExterno;

            openWindow(550, 1000, "../Cadastros/CadItensOrdemCarga.aspx?" + queryString);

            return false;
        }

        function gerarCarregamento() {

            if (!confirm("Gerar carregamento para as OCs criadas?"))
                return false;

            var idsRotas = FindControl("cblRota", "select").itens();
            var dtEntPedidoIni = FindControl("txtDataIni", "input").value;
            var dtEntPedidoFin = FindControl("txtDataFin", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;

            var queryString = "idsRotas=" + idsRotas;
            queryString += "&dtEntPedidoIni=" + dtEntPedidoIni;
            queryString += "&dtEntPedidoFin=" + dtEntPedidoFin;
            queryString += "&idLoja=" + idLoja;

            redirectUrl("../Cadastros/CadCarregamento.aspx?" + queryString);
        }

        function openRptInd(idOrdemCarga) {
            if (idOrdemCarga == null || idOrdemCarga == "") {
                alert("Informe a OC.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=OrdemCarga&idOrdemCarga=" + idOrdemCarga);
            return false;
        }

        function atualizar() {
            cOnClick("imgPesq");
        }

        function gerarOCs() {

            if (!confirm("Tem certeza que deseja gerar OC's automaticamente?"))
                return false;

            bloquearPagina();
            desbloquearPagina(false);

            var idsRotas = FindControl("cblRota", "select").itens();
            var dtEntPedidoIni = FindControl("txtDataIni", "input").value.replace("/", "-").replace("/", "-");
            var dtEntPedidoFin = FindControl("txtDataFin", "input").value.replace("/", "-").replace("/", "-");
            var idLoja = FindControl("drpLoja", "select").value;
            var tipoOC = FindControl("drpTipoOC", "select").value;
            var txtNumCli = FindControl("txtNumCli", "input").value;
            var txtNomeCliente = FindControl("txtNomeCliente", "input").value;
            var chkPedidosObs = FindControl("chkPedidosObs", "input").checked;
            var chkFastDelivery = FindControl("chkFastDelivery", "input").checked;
            var idPedido = FindControl("txtIdPedido", "input").value;
            var codRotasExternas = FindControl("cblRotaExterna", "select").itensText();
            var idClienteExterno = FindControl("txtNumCliExterno", "input").value;
            var nomeClienteExterno = FindControl("txtNomeClienteExterno", "input").value;

            var tbOcs = FindControl("grdOrdensCarga", "table");

            var cliIgnorar = "";

            for (var i = 1; i < tbOcs.rows.length; i++) {

                if (tbOcs.rows[i].cells.length > 10) {

                    var chkIgnorar = tbOcs.rows[i].cells[10].getElementsByTagName('input')[0];

                    if (chkIgnorar != null && chkIgnorar.checked)
                        cliIgnorar = tbOcs.rows[i].cells[10].getElementsByTagName('input')[1].value + ";";
                }
            }

            var retorno = CadOrdemCarga.GerarOCs(idsRotas, txtNumCli, txtNomeCliente, dtEntPedidoIni, dtEntPedidoFin, idLoja,
             tipoOC, cliIgnorar, chkPedidosObs, idPedido, codRotasExternas, idClienteExterno, nomeClienteExterno, chkFastDelivery).value.split(";");

            desbloquearPagina(true);

            if (retorno[0] == "Erro")
                alert(retorno[1]);

            atualizar();
  
            return false;
        }

        $(document).ready(function() {
            $(window).scroll(function() {
                set = ($(document).scrollTop()) + "px";
                $('#boxFloat').animate({ top: set }, { duration: 500, queue: false });
            });
        });

        function idPedidoChanged() {
            var idPedido = FindControl("txtIdPedido", "input").value;
            if (idPedido != "") {
                FindControl("txtNumCli", "input").value = "";
                FindControl("txtNomeCliente", "input").value = "";
            }
        }
        
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="200px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="ObtemAtivas"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Entrega Ped.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataFin" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Cliente: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="true" Width="250px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Situacao" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="1">Finalizado</asp:ListItem>
                                <asp:ListItem Value="2">Pendente</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Tipo OC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoOC" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpTipoOC_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Value="1">Venda</asp:ListItem>
                                <asp:ListItem Value="2">Transferência</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table ID="tbClienteExterno" runat="server">
                    <tr>
                         <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente Externo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCliExterno" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeClienteExterno" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Rota Externa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <sync:CheckBoxListDropDown ID="cblRotaExterna" runat="server" Width="200px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRotasExternas" DataTextField="Descr" DataValueField="Id"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRotasExternas" runat="server" SelectMethod="GetRotasExternas"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click"/>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Cliente do Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" Width="70px" onblur="idPedidoChanged();"></asp:TextBox>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPedidosObs" runat="server" Text="Pedidos com observação." />
                        </td>
                         <td>
                            <asp:CheckBox ID="chkFastDelivery" runat="server" Text="Fast Delivery" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellspacing="10">
                    <tr style="font-style:italic; font-weight: bold;">
                        <td style="color: red;">* Sem ordens de cargas geradas</td>
                        <td style="color: blue;">* Ordens de cargas geradas parcialmente</td>
                        <td>* Ordens de cargas geradas</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnGerarOCs" runat="server" Text="Gerar OC's" Width="150px" OnClientClick="return gerarOCs();" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdOrdensCarga" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsListagemOrdemCarga" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum pedido encontrado para o filtro informado."
                                OnRowDataBound="grdOrdensCarga_RowDataBound" OnDataBound="grdOrdensCarga_DataBound">
                                <PagerSettings PageButtonCount="20" />
                                <RowStyle HorizontalAlign="Left" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hdfPeso" runat="server" Value='<%# Bind("Peso") %>' />
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirOCs(this, " + Eval("IdCliente") + "); return false" %>'
                                                Width="10px" ToolTip="Exibir OC's" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rota">
                                        <ItemTemplate>
                                            <asp:Label ID="Label98" runat="server" Text='<%# Bind("RotaCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cidade">
                                        <ItemTemplate>
                                            <asp:Label ID="Label88" runat="server" Text='<%# Bind("CidadeCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Peso">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Peso Pend.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("PesoPendenteProducao") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total M²">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Itens">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("QtdePecasVidro") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total M² Pend.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalM2PendenteProducao") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Itens Pend.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("QtdePecaPendenteProducao") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Volumes">
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("QtdeVolumes") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Volumes Pend.">
                                        <ItemTemplate>
                                            <asp:Label ID="Label91" runat="server" Text='<%# Bind("VolumesPendentes") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ignorar bloqueios">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkIgnorarBloqueio" runat="server" />
                                            <asp:HiddenField ID="hdfIdClienteBloqueio" runat="server" Value='<%# Eval("IdCliente") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkGerarOC" runat="server" OnClientClick='<%# "return gerarOC(" + Eval("IdCliente") + "," + Eval("IdRota") + ");" %>'
                                                Visible='<%# Eval("GerarOCVisible") %>'>Gerar OC</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            </td> </tr><asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                            <tr id="ocs_<%# Eval("IdCliente") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                                <td colspan="14">
                                                    <asp:GridView ID="grdOrdemCarga" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOrdemCarga"
                                                        DataSourceID="odsOrdemCarga" GridLines="None" Width="100%" class="pos" ShowFooter="True"
                                                        CellPadding="0" EmptyDataText="Nenhuma ordem de carga encontrada." OnRowDataBound="grdOrdemCarga_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        ToolTip="Excluir OC" OnClientClick="if(!confirm('Deseja realmente excluir esta ordem de carga?')) return false;" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Cód. OC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdOrdemCarga") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Loja">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    Total :
                                                                </FooterTemplate>
                                                                <FooterStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Peso">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Peso Pendente">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("PesoPendenteProducao") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Total M²">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Itens">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("QtdePecasVidro") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Total M² Pendente">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalM2PendenteProducao") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Itens Pendentes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("QtdePecaPendenteProducao") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Volumes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("QtdeVolumes") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tipo OC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("TipoOrdemCargaStr") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Situação">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label11" runat="server" Text='<%# Bind("SituacaoStr") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                                                        ToolTip="Visualizar OC" OnClientClick='<%# "openRptInd(" + Eval("IdOrdemCarga") + "); return false;" %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle Font-Bold="true" Height="25px" />
                                                    </asp:GridView>
                                                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrdemCarga" runat="server" DataObjectTypeName="Glass.Data.Model.OrdemCarga"
                                                        DeleteMethod="Delete" SelectMethod="GetList" TypeName="WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo"
                                                        OnDeleted="odsOrdemCarga_Deleted">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="hdfIdCliente" Name="idCliente" PropertyName="Value"
                                                                Type="UInt32" />
                                                            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                                                                Type="UInt32" />
                                                            <asp:ControlParameter ControlID="txtDataIni" Name="dtEntPedidoIni" PropertyName="DataString"
                                                                Type="String" />
                                                            <asp:ControlParameter ControlID="txtDataFin" Name="dtEntPedidoFin" PropertyName="DataString"
                                                                Type="String" />
                                                            <asp:ControlParameter ControlID="drpTipoOC" Name="tipoOC" PropertyName="SelectedValue"
                                                                Type="String" />
                                                        </SelectParameters>
                                                    </colo:VirtualObjectDataSource>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt" />
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <HeaderStyle HorizontalAlign="Left" />
                                <RowStyle HorizontalAlign="Left" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsListagemOrdemCarga" runat="server"
                                SelectMethod="GetListForGerarOC" TypeName="WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtDataIni" Name="dtEntPedidoIni" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtDataFin" Name="dtEntPedidoFin" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="cblRota" Name="idsRotas" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpTipoOC" Name="tipoOC" PropertyName="SelectedValue"
                                        Type="Object" />
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCli" PropertyName="Text"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="chkPedidosObs" Name="pedidosObs" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkFastDelivery" Name="fastDelivery" PropertyName="Checked" />
                                        <asp:ControlParameter ControlID="txtIdPedido" Name="IdPedido" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="cblRotaExterna" Name="idsRotasExternas" PropertyName="SelectedItem"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td valign="top">
                            <div runat="server" id="boxFloat" class="boxFlutuante">
                                <table>
                                    <tr>
                                        <td>
                                            <b>Total:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPesoTotal" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Gerado:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPesoGerado" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Pendente Gerar:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPendenteGerar" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="display: none">
                                        <td>
                                            <b>Pendente OC's Anteriores:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPesoPendenteCarregamento" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnGerarCarregamento" runat="server" Text="Gerar Carregamento" OnClientClick="gerarCarregamento(); return false;" />
            </td>
        </tr>
    </table>
</asp:Content>
