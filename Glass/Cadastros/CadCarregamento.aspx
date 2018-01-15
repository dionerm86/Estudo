<%@ Page Title="Gerar Carregamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCarregamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCarregamento" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .boxFlutuante
        {
            margin-left: 5px;
            margin-top: 5px;
            float: left;
            position: relative;
            width: 170px;
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
            if (idCli.value == "")
                return false;

            var idCliente = idCli.value;

            var retorno = MetodosAjax.GetCli(idCliente).value.split(';');

            var result = MetodosAjax.GetCli(idCliente);

            if (result.error != null) {
                alert(result.error.description);
                return
            }

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("txtNumCli", "input").value = idCliente;

            return false;
        }

        function addOC() {

            var idOC = FindControl("txtNumOC", "input").value;
            if (Trim(idOC) == "") {
                alert("Selecione uma OC para continuar.");
                FindControl("txtNumOC", "input").value = "";
                FindControl("txtNumOC", "input").focus();
                return false;
            }

            var validaOC = CadCarregamento.ValidaOC(idOC).value.split(';');

            if (validaOC[0] == "erro") {
                alert(validaOC[1]);
                FindControl("txtNumOC", "input").value = "";
                FindControl("txtNumOC", "input").focus();
                return false;
            }

            var idsOCs = FindControl("hdfIdsOCs", "input").value.split(',');
            var novosIds = new Array();

            novosIds.push(idOC);
            for (i = 0; i < idsOCs.length; i++)
                if (idsOCs[i] != idOC && idsOCs[i].length > 0)
                novosIds.push(idsOCs[i]);

            FindControl("hdfIdsOCs", "input").value = novosIds.join(',');
            FindControl("txtNumOC", "input").value = "";
            cOnClick("imgPesq", null);
        }

        function addCli() {

            var idCliente = FindControl("txtNumCli", "input");
            var nomeCliente = FindControl("txtNomeCliente", "input");

            if (Trim(idCliente.value) == "") {
                alert("Informe um cliente para continuar.");
                idCliente.value = "";
                nomeCliente.value = "";
                idCliente.focus();
                return false;
            }

            var idsOCs = FindControl("hdfIdsOCs", "input").value.split(',');

            FindControl("hdfIdsOCs", "input").value = CadCarregamento
                .GetIdsOCsParaCarregar(idCliente, nomeCliente, 0, 0, idsOCs).value;

            FindControl("txtNomeCliente", "input").value = "";
            FindControl("txtNumCli", "input").value = "";

            cOnClick("imgPesq", null);
        }

        function buscarOCs() {
        
            var idsLoja = FindControl("drpLoja", "select").itens();
            var idRota = FindControl("drpRota", "select").itens();

            if (idsLoja != "" || idRota != "")
            {
                FindControl("hdfIdsOCs", "input").value = CadCarregamento.GetIdsOCsParaCarregar(0, '', idsLoja, idRota, '').value;

                FindControl("txtNomeCliente", "input").value = "";
                FindControl("txtNumCli", "input").value = "";
            }
        }

        function exibirOCs(botao, idCliente) {
            var linha = document.getElementById("cliente_" + idCliente);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " OCs";
        }

        function selecionaTodosClientes(check) {

            var tbOCs = FindControl("grdOC", "table");

            if (tbOCs == null)
                return false;

            var chkClientes = tbOCs.getElementsByTagName("input");

            for (var i = 0; i < chkClientes.length; i++)
                chkClientes[i].checked = check.checked;
        }

        function selecionaTodasOCs(check, idCli) {

            var trCliente = FindControl("cliente_" + idCli, "tr");

            if (trCliente == null)
                return false;

            var chkOC = trCliente.getElementsByTagName("input");

            for (var i = 0; i < chkOC.length; i++)
                chkOC[i].checked = check.checked;

        }

        function finalizarCarregamento(idLoja, idsOCs) {

            if (idLoja == "")
            {
                alert('Nenhuma loja foi selecionada');
                return false;
            }

            var queryString = "popup=true";
            queryString += "&idLoja=" + idLoja;
            queryString += "&idsOCs=" + idsOCs;
            
            openWindow(550, 800, "../Cadastros/CadDadosCarregamento.aspx?" + queryString);

            return false;
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Num. OC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumOC" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAdd', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addOC(); return false;" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('btnBuscarOCs', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;"> <img border="0" src="../Images/Pesquisar.gif" /> </asp:LinkButton>
                        </td>
                         <td>
                            <asp:ImageButton ID="imgAddCli" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addCli(); return false;" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="buscarOCs()" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpLoja" runat="server" DataSourceID="odsLoja"
                                        DataTextField="NomeFantasia" DataValueField="IdLoja" Title="Selecione as Lojas" Width="250">
                                    </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="buscarOCs()" OnClick="imgPesq_Click"
                                    ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarOCs" runat="server" Text="Buscar OC's" OnClientClick="buscarOCs()"
                    CausesValidation="False" OnClick="btnBuscarOCs_Click" />
                <asp:HiddenField ID="hdfIdsOCs" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdOC" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsOCs" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma OC encontrada." OnDataBound="grdOC_DataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkTodosClientes" runat="server" onclick="selecionaTodosClientes(this);"
                                                Checked="True" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkTodasOCs" runat="server" Checked="True" Style="margin-left: -1px;
                                                margin-right: 1px" onclick='<%# "selecionaTodasOCs(this, " + Eval("IdCliente") + ")"%>'
                                                AutoPostBack="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirOCs(this, " + Eval("IdCliente") + "); return false" %>'
                                                Width="10px" ToolTip="Mostrar OCs" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdNomeCliente" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Peso">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPesoTotal" runat="server" Text='<%# Eval("PesoTotal") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            </td> </tr>
                                            <tr id="cliente_<%# Eval("IdCliente") %>" class="<%= GetAlternateClass() %>" style="display:none;">
                                                <td>
                                                </td>
                                                <td colspan="4" style="padding: 0px">
                                                    <asp:GridView ID="grdOcs" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                                        DataKeyNames="IdOrdemCarga" DataSource='<%# Eval("Ocs") %>' GridLines="None"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkSelOC" runat="server" Checked="True" Style="margin-left: -1px;
                                                                        margin-right: 1px" AutoPostBack="True" OnCheckedChanged="chkSelOC_CheckedChanged" />
                                                                    <asp:HiddenField ID="hdfIdOC" runat="server" Value='<%# Eval("IdOrdemCarga") %>' />
                                                                    <asp:HiddenField ID="hdfPeso" runat="server" Value='<%# Eval("Peso") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Cód. OC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIdOrdemCarga1" runat="server" Text='<%# Bind("IdOrdemCarga") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tipo da OC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTipoOrdemCargaStr1" runat="server" Text='<%# Bind("TipoOrdemCargaStr") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Peso">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPesoTotal1" runat="server" Text='<%# Eval("Peso") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOCs" runat="server" TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo"
                                SelectMethod="GetOCsForCarregamento">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hdfIdsOCs" PropertyName="Value" Name="idsOCs" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td runat="server" id="tdBoxFlutuante" valign="top">
                            <div id="boxFloat" class="boxFlutuante">
                                <table>
                                    <tr>
                                        <td>
                                            <b>Peso Total:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPeso" runat="server"></asp:Label>
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
                <asp:Button ID="btnFinalizarCarregamento" runat="server" Text="Finalizar Carregamento"
                    OnClick="btnFinalizarCarregamento_Click" Height="22px" />
            </td>
        </tr>
    </table>
</asp:Content>
