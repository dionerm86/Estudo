<%@ Page Title="Reposição de Peça" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadReporPeca.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.CadReporPeca" %>

<%@ Register src="../../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>
<%@ Register src="../../Controls/ctrlTipoPerda.ascx" tagname="ctrlTipoPerda" tagprefix="uc2" %>
<%@ Register src="../../Controls/ctrlRetalhoProducao.ascx" tagname="ctrlRetalhoProducao" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    function confirmar() {
        if (FindControl("drpSetor", "select").value == "") {
            alert("Informe o setor da peça na produção.");
            return false;
        }

        if (FindControl("drpTipoPerda", "select").value == "") {
            alert("Informe o tipo da perda.");
            return false;
        }

        var obrigarObs = <%= Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda.ToString().ToLower() %>;
        if (obrigarObs && FindControl("txtObs", "textarea").value == "") {
            alert("Informe o motivo da reposição.");
            return false;
        }

        return confirm("Deseja realizar uma reposição desta peça?");
    }

    function leuEtiqueta(txtNumEtiqueta) {
        if (txtNumEtiqueta == null || txtNumEtiqueta == undefined)
            return;
    
        txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
    }
    
    function imprimirRetalhos(etiqueta) {
        
        //var $controleRetalhos = $("table[id$=ctrlRetalhoProducao1_tabela]");
        //$controleRetalhos.children("tbody").children("tr").find("input:hidden[id$=hdfIdProd]").val(idProd)
        
        openWindow(500, 700, '../../Relatorios/RelEtiquetas.aspx?apenasPlano=false&numEtiqueta=' + etiqueta + '&reposicao=true');
    }
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            Número do Pedido:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);" Width="70px"
                                TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                ErrorMessage="*" ValidationGroup="idPedido" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                TabIndex="2" ValidationGroup="idPedido" style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="3">Pendente</asp:ListItem>
                                <asp:ListItem Value="4">Pronta</asp:ListItem>
                                <asp:ListItem Value="5">Entregue</asp:ListItem>
                                <asp:ListItem Value="2">Perda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imbPesq_Click" TabIndex="2" ValidationGroup="idPedido" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" Text="Setor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetorBusca" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                DataTextField="Descricao" DataValueField="IdSetor">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imbPesq_Click" TabIndex="2" ValidationGroup="idPedido" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Turno" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTurno" runat="server" DataSourceID="odsTurno" DataTextField="Descricao"
                                DataValueField="IdTurno" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imbPesq_Click" ToolTip="Pesquisar" />
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
                <table>
                    <tr>
                        <td align="left">
                            Setor da perda</td>
                        <td align="left">
                            <asp:DropDownList ID="drpSetor" runat="server" CausesValidation="True" ValidationGroup="idPedido"
                                DataSourceID="odsSetor" DataTextField="Descricao" DataValueField="IdSetor" AppendDataBoundItems="True"
                                AutoPostBack="True" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Tipo de perda
                        </td>
                        <td align="left">
                            <uc2:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" 
                                ExibirItemVazio="True" />
&nbsp;<asp:CheckBox ID="chkPerdaDefinitiva" runat="server" Text="Perda definitiva?" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblFuncPerda" runat="server" Text="Funcionário Perda"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" 
                                AppendDataBoundItems="True" DataSourceID="odsFuncPerda" DataTextField="Nome" 
                                DataValueField="IdFunc" ondatabound="drpFuncionario_DataBound">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Data da Perda</td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataPerda" runat="server" ExibirHoras="True" 
                                ReadOnly="ReadWrite" ValidateEmptyText="True" />
                        </td>
                    </tr>
                    <tr id="obsPerda">
                        <td align="left">
                            Motivo reposição
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" Columns="50" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="obsPerda">
                        <td align="left" colspan="2">
                            <uc3:ctrlRetalhoProducao ID="ctrlRetalhoProducao1" runat="server" />
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
                <table>
                    <tr>
                        <td>
                            Num. Etiqueta:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server" Width="100px" onkeypress="if (isEnter(event)) return leuEtiqueta(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                TabIndex="2" ValidationGroup="idPedido" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdPedProducao"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdProdPed" EmptyDataText="Nenhum produto encontrado."
                    OnRowCommand="grdProdutos_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:ImageButton ID="imbMarcar0" runat="server" ToolTip="Marcar peça" CommandArgument='<%# Eval("NumEtiqueta") + ";" + Eval("Altura")+ ";" + Eval("Largura") %>'
                                                CommandName="Marcar" ImageUrl="~/Images/ok.gif" OnClientClick="if (!confirmar()) return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <asp:HiddenField ID="hdfDescr" runat="server" Value='<%# Eval("DescrProduto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="TotM2" HeaderText="Tot. m²" SortExpression="TotM2" DataFormatString="{0:N}" />
                        <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="NumEtiqueta" />
                        <asp:BoundField DataField="DescrSetor" HeaderText="Setor" SortExpression="DescrSetor" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdPedProducao" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetForReposicaoPecaCount"
                    SelectMethod="GetForReposicaoPeca" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEtiqueta" Name="numEtiqueta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSetorBusca" Name="idSetor" PropertyName="SelectedValue"
                            Type="Int32" />
                             <asp:ControlParameter ControlID="ddlTurno" Name="idTurno" 
                            PropertyName="SelectedValue" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetTipoPerda"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoDePerda" runat="server" SelectMethod="GetBySetor"
                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpSetor" DefaultValue="0" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.TurnoDAO" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncPerda" runat="server" 
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.FuncionarioDAO" 
                    >
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfRetalhos1" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
