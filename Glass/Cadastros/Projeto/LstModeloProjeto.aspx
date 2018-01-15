<%@ Page Title="Modelos de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstModeloProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstModeloProjeto" %>

<%@ Register src="../../Controls/ctrlImagemPopup.ascx" tagname="ctrlImagemPopup" tagprefix="uc1" %>
<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
<script type="text/javascript">

    // Abre tela para edição da posição das peças do modelo
    function openPos(idProjetoModelo) {
        openWindow(600, 800, "CadPosicaoPecaModelo.aspx?idProjetoModelo=" + idProjetoModelo);

        return false;
    }

    function openRptImpressao(projModeloCod) {
        openWindow(600, 800, "../../Relatorios/Projeto/RelBase.aspx?Rel=ImpressaoModeloProjeto&projModeloCod=" + projModeloCod);
        return false;
    }
    
    function openRptModelos()
    {
       var cod = FindControl("txtCodigo", "input").value;
       var desc = FindControl("txtDescricao", "input").value;
       var grupo = FindControl("drpGrupoModelo", "select").value;
       var situacao = FindControl("drpSituacao", "select").value;
              
       openWindow(600, 800, "../../Relatorios/Projeto/RelBase.aspx?Rel=LstModeloProjeto&cod=" + cod + "&desc=" + desc + "&grupo=" + grupo + "&situacao=" + situacao);
    }

</script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                                    </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                                    </td>
                        <td>
                            <asp:Label ID="Grupo" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" 
                                DataSourceID="odsGrupoModelo" DataTextField="Descricao" 
                                DataValueField="IdGrupoModelo">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="1" Text="Ativo" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Inativo"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Bloqueado"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkNovoModelo" runat="server" onclick="lnkNovoModelo_Click">Novo Modelo</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdModeloProjeto" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    DataSourceID="odsProjetoModelo" 
                    PageSize="15" onrowcommand="grdModeloProjeto_RowCommand"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" 
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" Visible='<%# Eval("EditVisible") %>'
                                    NavigateUrl='<%# "CadModeloProjeto.aspx?idProjetoModelo=" + Eval("IdProjetoModelo") %>' >
                                    <img border="0" src="../../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:LinkButton ID="lnkInativar" runat="server" Visible='<%# Eval("AtivarInativarVisible") %>'
                                    CommandArgument='<%# Eval("IdProjetoModelo") %>' CommandName="Inativar" 
                                    ToolTip="Ativar/Inativar">
                                      <img border="0" src="../../Images/Inativar.gif"></img></asp:LinkButton>
                                <asp:LinkButton ID="lnkSetPos" runat="server" 
                                    OnClientClick='<%# "return openPos(\"" + Eval("IdProjetoModelo") + "\");" %>' 
                                    ToolTip="Configurar posição das informações na figura">
                                    <img src="../../Images/Coord.gif" border="0"></asp:LinkButton>
                                <asp:ImageButton ID="lnkImpressao" runat="server" 
                                    ImageUrl="~/Images/Relatorio.gif" OnClientClick='<%# "openRptImpressao(\"" + Eval("Codigo") + "\"); return false;" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." SortExpression="Codigo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" 
                            SortExpression="Descricao" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" 
                            SortExpression="Situacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdProjetoModelo") %>' 
                                    Tabela="ProjetoModelo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptModelos(); return false;">Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProjetoModelo" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="PesquisarProjetoModeloCount" 
                    SelectMethod="PesquisarProjetoModelo" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProjetoModeloDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupoModelo" Name="idGrupoModelo" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" 
                            PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>        
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" 
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.GrupoModeloDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

