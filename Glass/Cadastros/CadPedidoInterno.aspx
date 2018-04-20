<%@ Page Title="Cadastro de Pedido Interno" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPedidoInterno.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPedidoInterno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function loadProduto(codInterno)
        {
            if (codInterno == "")
                return false;

            try {
                var retorno = CadPedidoInterno.GetProduto(codInterno).value.split(';');
                
                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }
                else if (retorno[0] == "Prod") {
                    FindControl("hdfIdProd", "input").value = retorno[1];
                    FindControl("hdfTipoCalc", "input").value = retorno[4]; // Verifica como deve ser calculado o produto
                    
                    qtdEstoque = retorno[3]; // Pega a quantidade disponível em estoque deste produto
                    var tipoCalc = retorno[4];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txtAltura", "input");
                    var cLargura = FindControl("txtLargura", "input");
                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
                    cAltura.value = retorno[5];
                    cLargura.value = retorno[6];
                }

                FindControl("lblDescrProd", "span").innerHTML = retorno[2];
            }
            catch (err) {
                alert(err);
            }
        }
    
        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno)
        {
            try
            {
                FindControl("txtCodProd", "input").value = codInterno;
                loadProduto(codInterno);
            }
            catch (err) { }
        }
    
        function getProduto()
        {
            // Não passa o pedido interno para que não limite a seleção de produtos apenas de compra
            //openWindow(450, 700, '../Utils/SelProd.aspx?PedidoInterno=1');
            openWindow(450, 700, '../Utils/SelProd.aspx');
        }
        
        function calcM2()
        {
            var idProd = FindControl("hdfIdProd", "input").value;
            var qtde = FindControl("txtQtde", "input").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            
            if (qtde == "" || altura == "" || largura == "")
                return;
            
            FindControl("lblTotM", "input").value = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, false, 0, false).value;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvPedidoInterno" runat="server" AutoGenerateRows="False" DataKeyNames="IdPedidoInterno"
                    DataSourceID="odsPedidoInterno" GridLines="None" DefaultMode="Insert" OnItemCommand="dtvPedidoInterno_ItemCommand">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellpadding="4" cellspacing="0">
                                    <tr>
                                        <td id="pedido_titulo" align="left" class="dtvHeader">
                                            Pedido
                                        </td>
                                        <td id="pedido_numero" align="left">
                                            <asp:Label ID="lblIdPedido" runat="server" Text='<%# Bind("IdPedidoInterno") %>'
                                                Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Data
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblData" runat="server" Text='<%# Bind("DataPedido") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Situação
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                                            <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Loja
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="Name"
                                                DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>' OnLoad="drpLoja_Load" onchange="onChangeLoja(this)">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" 
                                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc" 
                                                onchange="alteraDataPedidoFunc(this.value)" 
                                                SelectedValue='<%# Bind("IdFuncCad") %>'>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblCentroCusto" runat="server" Text="Centro de Custo"></asp:Label>
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto"
                                                DataTextField="Descricao" DataValueField="IdCentroCusto" SelectedValue='<%# Bind("IdCentroCusto") %>'
                                                AppendDataBoundItems="true">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Observação</td>
                                        <td align="left" colspan="3">
                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="500" TextMode="MultiLine" 
                                                Width="339px" Height="90px" Text='<%# Bind("Observacao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                                <script type="text/javascript">
                                    if (<%= (dtvPedidoInterno.CurrentMode == DetailsViewMode.Insert).ToString().ToLower() %>)
                                    {
                                        document.getElementById("pedido_titulo").style.display = "none";
                                        document.getElementById("pedido_numero").style.display = "none";
                                    }
                                </script>

                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="left" style="font-weight: bold">
                                            Pedido
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedidoInterno") %>'
                                                Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td align="left" style="font-weight: bold">
                                            Data
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataPedido", "{0:d}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">
                                            Situação
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                                        </td>
                                        <td align="left" style="font-weight: bold">
                                            Loja
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">
                                            Funcionário
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:Label ID="lblFuncionarioCad" runat="server" Text='<%# Eval("NomeFuncCad") %>'></asp:Label>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td align="left" style="font-weight: bold">
                                            <asp:Label ID="lblCentroCusto" runat="server" Text="Centro de Custo"></asp:Label> </td>
                                        <td align="left" colspan="3">
                                            <asp:Label ID="lblDescricaoCentroCusto" runat="server" Text='<%# Eval("DescrCentroCusto") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">
                                            Observação</td>
                                        <td align="left" colspan="3">
                                            <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Observacao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    OnClick="btnCancelar_Click" Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    OnClick="btnCancelar_Click" Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedidoInterno") %>'
                                    CommandName="Finalizar" Text="Finalizar" OnClientClick="if (!confirm(&quot;Deseja finalizar esse pedido interno?&quot;)) return false" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" OnClick="btnCancelar_Click"
                                    Text="Voltar" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdProdPedInterno" DataSourceID="odsProdutoPedidoInterno"
                    ShowFooter="True" OnDataBound="grdProdutos_DataBound" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse produto?&quot;)) return false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                <asp:HiddenField ID="hdfPedidoInterno" runat="server" Value='<%# Bind("IdPedidoInterno") %>' />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                    onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                <a href="#" onclick="getProduto(); return false;">
                                    <img border="0" src="../Images/Pesquisar.gif" /></a>
                                <asp:HiddenField ID="hdfIdProd" runat="server" />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true)"
                                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true)"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>' onchange="calcM2()"
                                    Width="50px" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true)"
                                    Enabled='<%# Eval("AlturaEnabled") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true)"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' onchange="calcM2()"
                                    Width="50px" onkeypress="return soNumeros(event, true, true)" Enabled='<%# Eval("LarguraEnabled") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, true, true)"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total m²" SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblTotM" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM" runat="server"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação" SortExpression="Observacao">
                            <FooterTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Height="30px" MaxLength="100" 
                                    Text='<%# Bind("Observacao") %>' TextMode="MultiLine" Width="207px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidoInterno" runat="server" DataObjectTypeName="Glass.Data.Model.PedidoInterno"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoInternoDAO"
                    UpdateMethod="Update" OnInserted="odsPedidoInterno_Inserted" OnUpdated="odsPedidoInterno_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedidoInterno" QueryStringField="idPedidoInterno"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoPedidoInterno" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutoPedidoInterno"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoPedidoInternoDAO"
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedidoInterno" QueryStringField="idPedidoInterno"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsFuncionario" runat="server" 
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
                    Culture="pt-BR" MaximumRowsParameterName="" SelectMethod="GetOrdered" SkinID="" 
                    StartRowIndexParameterName="" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
                    SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCentroCusto" runat="server" SelectMethod="ObtemParaSelecao"
                    TypeName="Glass.Data.DAL.CentroCustoDAO" DataObjectTypeName="Glass.Data.Model.CentroCusto">
                    <SelectParameters>
                        <asp:Parameter Name="buscarEstoque" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="idLoja" Type="Int32" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
