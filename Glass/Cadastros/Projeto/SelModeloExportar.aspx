<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelModeloExportar.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Projeto.SelModeloExportar" Title="Selecione o Modelo do Projeto"
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var click = false;

        // Função chamada por qualquer modelo, quando o mesmo for selecionado
        function selModelo(lnkSelecionar)
        {
            if (click)
                return false;

            click = true;

            // Pega o id do controle que chamou esta função
            var ctrlSourceId = lnkSelecionar.id.toString().split('_')[0];

            var idProjetoModelo = FindControl(ctrlSourceId + "_hdfIdProjetoModelo", "input").value;
            var idCorVidro = FindControl(ctrlSourceId + "_drpCorVidro", "select").value;
            var idCorAluminio = FindControl(ctrlSourceId + "_drpCorAluminio", "select").value;
            var idCorFerragem = FindControl(ctrlSourceId + "_drpCorFerragem", "select").value;
            var medidaExata = FindControl(ctrlSourceId + "_chkMedidaExata", "input").checked;
            var apenasVidros = FindControl(ctrlSourceId + "_chkApenasVidro", "input").checked;

            if (idCorVidro == "")
            {
                alert("Informe a cor do vidro.");
                click = false;
                return false;
            }

            window.opener.setModelo(idProjetoModelo, idCorVidro, idCorAluminio, idCorFerragem, apenasVidros, medidaExata);
            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Grupo" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" DataSourceID="odsGrupoModelo"
                                DataTextField="Descricao" DataValueField="IdGrupoModelo" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdModelosProjeto" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsProjetoModelo"
                    GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClientClick='<%# "window.opener.setModelo(" + Eval("IdProjetoModelo") + ", \"" + Eval("Codigo") + "\", \"" + Eval("DescrGrupo") + "\", \"" + Eval("Descricao") + "\", " + Eval("Espessura") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" SortExpression="Codigo" />
                        <asp:BoundField DataField="DescrGrupo" HeaderText="Grupo" SortExpression="DescrGrupo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descriçao" SortExpression="Descricao" />
                        <asp:BoundField DataField="Espessura" HeaderText="Espessura" SortExpression="Espessura" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
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
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click">
                    <img src="../../Images/addMany.gif" border="0"> Adicionar Todos</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.GrupoModeloDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProjetoModelo" runat="server" SelectCountMethod="GetCount"
        SelectMethod="GetList" TypeName="Glass.Data.DAL.ProjetoModeloDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="drpGrupoModelo" Name="idGrupoModelo" PropertyName="SelectedValue"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
