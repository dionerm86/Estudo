<%@ Page Title="Imprimir Vários Pedidos PCP" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaVariosPedidosPcp.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaVariosPedidosPcp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        function adicionar()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            if (idPedido == "")
            {
                alert("Digite o número do pedido.");
                return;
            }

            var resposta = ListaVariosPedidosPcp.VerificaPedido(idPedido).value.split('\t');
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            addItem(new Array(idPedido), new Array("Pedido"), "tbPedido", idPedido, "hdfIdsPedidos", null, null, null, false);
            FindControl("txtNumPedido", "input").value = "";
        }

        function openRpt()
        {
            var ids = FindControl("hdfIdsPedidos", "input").value;
            ids = ids.substr(0, ids.length - 1);

            var grupos = new Array();

            // Verifica os grupos selecionados
            var cblGrupos = FindControl("cblGrupoProd", "table");
            if (cblGrupos != null)
            {
                var inputs = cblGrupos.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++)
                    if (inputs[i].type.toLowerCase() == "checkbox" && inputs[i].checked)
                    grupos.push(inputs[i].parentNode.getAttribute("valor"));

                if (grupos.length == 0)
                {
                    alert("Informe ao menos um grupo de produto a ser exibido na visualização da impressão.");
                    return false;
                }
            }

            openWindow(600, 800, "RelBase.aspx?rel=PedidoPcp&idPedido=" + ids + "&grupos=" + grupos.toString());
            return false;
        }

        function marcaDesmarcaGrupos(chk)
        {
            var checked = chk.checked;

            var vetChk = document.getElementsByTagName("input");
            for (i = 0; i < vetChk.length; i++)
                if (vetChk[i].type == "checkbox" && vetChk[i].id.toString().indexOf("GrupoProd") > 0)
                vetChk[i].checked = checked;
        }

    </script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table cellpadding="1" cellspacing="2">
                    <tr>
                        <td>
                            Número do Pedido:&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onchange="mudaCampo(this)"
                                onkeypress="return soNumeros(event, true, true);" runat="server"
                                onkeydown="if (isEnter(event)) cOnClick('imbAdd', 'input');" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif"
                                OnClientClick="adicionar(); return false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" style="padding: 8px">
                <table id="tbPedido"></table>
                <asp:HiddenField ID="hdfIdsPedidos" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="grupos">
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            &nbsp;<asp:CheckBox ID="chkMarcarDesmarcar" runat="server" Text="Marcar/desmarcar todos" 
                                onclick="marcaDesmarcaGrupos(this);" Checked="True"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBoxList ID="cblGrupoProd" runat="server" DataSourceID="odsGrupoProd" 
                                DataTextField="Descricao" DataValueField="IdGrupoProd" RepeatColumns="3" 
                                ondatabound="cblGrupoProd_DataBound">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                Observações:<br />
                <asp:TextBox ID="txtObservacoes" runat="server" Rows="4" TextMode="MultiLine" 
                    Width="450px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="btnVisualizar" runat="server" onclientclick="return openRpt();" 
                    Text="Visualizar" />
            </td>
        </tr>
    </table>
    <script>
        FindControl("txtNumPedido", "input").focus();
    </script>
</asp:Content>

