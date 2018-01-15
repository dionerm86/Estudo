<%@ Page Title="Clientes com Desconto/Acréscimo" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaClientesDescAcresc.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaClientesDescAcresc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel)
        {
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idVendedor = FindControl("drpVendedor", "select").value;
            var idGrupoProd = FindControl("drpGrupo", "select").value;
            var idSubgrupoProd = FindControl("drpSubgrupo", "select").value;
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idRota = FindControl("drpRota", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=ClienteDescAcresc&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&idGrupoProd=" + idGrupoProd + "&idSubgrupoProd=" + idSubgrupoProd + "&codInternoProd=" + codInterno +
                "&descrProd=" + descrProd + "&idRota=" + idRota + "&idLoja=" + idLoja + "&situacao=" + situacao + "&exportarExcel=" + exportarExcel);
        }
        
        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        // Carrega dados do produto com base no código do produto passado
        function setProduto()
        {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try
            {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro")
                {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch (err)
            {
                alert(err.value);
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" AutoPostBack="True" DataSourceID="odsRota"
                                DataTextField="Descricao" DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupoProd"
                                DataTextField="Descricao" DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label22" runat="server" Text="Loja Cliente" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                DataTextField="Name" DataValueField="Id">
                                <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton></td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoCliente"
                                DataTextField="Translation" DataValueField="Key" OnDataBound="drpSituacao_DataBound">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton></td>
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
                <asp:GridView ID="grdDescontoAcrescimo" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdDesconto" DataSourceID="odsDescontoAcrescimo"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="IdCliente" HeaderText="Cód." SortExpression="IdCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DescrGrupoSubgrupo" HeaderText="Grupo" ReadOnly="True"
                            SortExpression="DescrGrupoSubgrupo" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# (float)Eval("Desconto") > 0 ? ((float)Eval("Desconto")).ToString("0.##") + "%" : "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Desconto") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# (float)Eval("Acrescimo") > 0 ? ((float)Eval("Acrescimo")).ToString("0.##") + "%" : "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Acrescimo") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="AplicarBeneficiamentos" HeaderText="Aplicar nos Beneficiamentos?"
                            SortExpression="AplicarBeneficiamentos">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:CheckBoxField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;
                <a href="#" onclick="return openRpt(true);">
                    <img border="0" src="<%= ResolveUrl("~") %>Images/Excel.gif" alt="Exporta Excel" /> Exportar Excel</a>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDescontoAcrescimo" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountRpt"
                    SelectMethod="GetListRpt" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.DescontoAcrescimoClienteDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idVendedor" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Object" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <sync:ObjectDataSource ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </sync:ObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupoProd" runat="server"
                    SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoCliente" runat="server"
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoCliente, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="ObtemLojas"
                    TypeName="Glass.Global.Negocios.ILojaFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
