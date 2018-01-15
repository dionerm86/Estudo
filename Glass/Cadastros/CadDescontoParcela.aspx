<%@ Page Title="Desconto/Acréscimo em Parcela" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadDescontoParcela.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDescontoParcela" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getTipo() {
            var rblTipo = FindControl("rblTipo", "table");
            var opcoes = rblTipo.getElementsByTagName("input");

            var tipo = -1;
            for (i = 0; i < opcoes.length; i++)
                if (opcoes[i].checked) {
                tipo = opcoes[i].value;
                break;
            }

            return tipo;
        }

        function alteraTipo() {
            var tipo = getTipo();
            document.getElementById("pedidoLiberacao").style.display = tipo == 0 ? "" : "none";
            document.getElementById("diversas").style.display = tipo == 1 ? "" : "none";
        }

        var contaAlterada = false;

        function setContaReceberBusca(idContaR) {
            FindControl("hdfIdContaR", "input").value = idContaR;
            contaAlterada = true;
            cOnClick("btnBuscarConta", "input");
        }

        function setPedido(idPedido) {
            FindControl("txtNumPedido", "input").value = idPedido;
        }

        function setContaReceber(idContaR, valor) {
            FindControl("lblNumConta", "span").innerHTML = idContaR;
            FindControl("lblValor", "span").innerHTML = valor;
        }

        function aplicarDescontoAcrescimo() {
            var numConta = FindControl("lblNumConta", "span").innerHTML;
            var valor = FindControl("lblValor", "span").innerHTML;
            var desconto = FindControl("txtDesconto", "input").value;
            var acrescimo = FindControl("txtAcrescimo", "input").value;
            var motivo = FindControl("txtMotivo", "textarea").value;
            var origem = FindControl("ddlOrigem", "select").value;

            if (numConta == "") {
                alert("Selecione uma conta a receber.");
                return false;
            }

            if (desconto == "" && acrescimo == "") {
                alert("Informe o desconto ou acréscimo que será dado.");
                return false;
            }
            else if (desconto != "" && acrescimo != "") {
                alert("Informe apenas o desconto ou apenas o acréscimo.");
                return false;
            }

            if (motivo == "") {
                alert("Informe o motivo do desconto/acréscimo.");
                return false;
            }

            var btnDesconto = FindControl("btnDesconto", "input");
            btnDesconto.disabled = true;

            var response = CadDescontoParcela.AplicarDescontoAcrescimo(numConta, valor.replace("R$", "").replace(" ", "").replace(".", ""), desconto, acrescimo, origem, motivo).value;

            if (response == null) {
                alert("Falha ao descontar valor da parcela.");
                btnDesconto.disabled = false;
                return;
            }

            response = response.split('\t');

            alert(response[1]);

            btnDesconto.disabled = false;

            if (response[0] == "Erro")
                return false;

            FindControl("lblNumConta", "span").innerHTML = "";
            FindControl("lblValor", "span").innerHTML = "";
            FindControl("txtDesconto", "input").value = "";
            FindControl("txtAcrescimo", "input").value = "";
            FindControl("txtMotivo", "textarea").value = "";

            var tipo = getTipo();
            switch (tipo) {
                case "0":
                    cOnClick('btnBuscarPedido', null);
                    break;

                case "1":
                    contaAlterada = true;
                    cOnClick('btnBuscarConta', null);
                    break;
            }
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" onclick="alteraTipo()">
                    <asp:ListItem Selected="True" Value="0">Pedido/Liberação</asp:ListItem>
                    <asp:ListItem Value="1">Diversas</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="pedidoLiberacao">
                    <tr>
                        <td>
                            <asp:Label ID="lblNumPedidoLiberacao" runat="server" Text="Número do Pedido/Liberação:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedidoLiberacao" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscarPedido', null);"
                                Width="70px" ValidationGroup="pedido"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedidoLiberacao"
                                ErrorMessage="*" ValidationGroup="pedido"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelPedido.aspx?tipo=3'); return false;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:Button ID="btnBuscarPedido" runat="server" Text="Buscar Pedido" Height="26px"
                                OnClick="btnBuscarPedido_Click" ValidationGroup="pedido" />
                        </td>
                    </tr>
                </table>
                <table id="diversas">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnBuscarConta" runat="server" Text="Buscar Conta" Height="26px"
                                OnClientClick="if (!contaAlterada) { openWindow(600, 800, '../Utils/SelContaReceber.aspx?desconto=1'); return false }"
                                OnClick="btnBuscarConta_Click" />
                            <asp:HiddenField ID="hdfIdContaR" runat="server" />
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
    </table>

    <script type="text/javascript">
        alteraTipo();
    </script>

    <table id="tbDesconto" runat="server" style="width: 100%" visible="false">
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                    DataKeyNames="IdContaR" DataSourceID="odsContasReceber" EmptyDataText="Nenhuma conta a receber encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EnableViewState="false"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgOk" runat="server" AlternateText="Selecionar" ImageUrl="~/Images/ok.gif"
                                    ToolTip="Selecionar" OnClientClick='<%# "setContaReceber(\"" + Eval("IdContaR") + "\", \"" + ((decimal)Eval("ValorVec") - (decimal)Eval("Acrescimo") + (decimal)Eval("Desconto")).ToString("C") + "\"); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdContaR" HeaderText="Num. Conta" SortExpression="IdContaR" />
                         <asp:TemplateField HeaderText="Parc." SortExpression="NumParc">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdLiberarPedido" HeaderText="Liberação" SortExpression="IdLiberarPedido" />
                        <asp:BoundField DataField="NomeCli" HeaderText="Cliente" SortExpression="NomeCli" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValorVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Desconto" DataFormatString="{0:C}" HeaderText="Desconto"
                            SortExpression="Desconto" />
                        <asp:BoundField DataField="Acrescimo" DataFormatString="{0:C}" HeaderText="Acréscimo"
                            SortExpression="Acrescimo" />
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Num. Conta:" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblNumConta" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Valor:" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblValor" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table width="1px">
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Desconto:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDesconto" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td width="100%">
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" Text="Acréscimo:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtAcrescimo" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblOrigem" runat="server" Text="Origem do Desconto / Acréscimo:" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4">
                            <asp:DropDownList ID="ddlOrigem" runat="server" DataSourceID="odsOrigem" DataValueField="idOrigemTrocaDesconto"
                                DataTextField="Descricao" AppendDataBoundItems="true">
                                 <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrigem" runat="server" SelectMethod="GetList"
                                TypeName="Glass.Data.DAL.OrigemTrocaDescontoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label11" runat="server" Text="Motivo:" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4">
                            <asp:TextBox ID="txtMotivo" runat="server" MaxLength="200" Rows="2" TextMode="MultiLine"
                                Width="250px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="5">
                            <asp:Button ID="btnDesconto" runat="server" Text="Aplicar" OnClientClick="return aplicarDescontoAcrescimo();" />
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
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasReceber" runat="server"
                    MaximumRowsParameterName="" SelectMethod="GetByPedidoLiberacao" StartRowIndexParameterName=""
                    TypeName="Glass.Data.DAL.ContasReceberDAO" EnableViewState="false">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="rblTipo" Name="tipoBusca" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedidoLiberacao" Name="idPedidoLiberacao"
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfIdContaR" Name="idsContasR" PropertyName="Value"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
