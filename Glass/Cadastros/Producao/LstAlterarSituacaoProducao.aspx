<%@ Page Title="Alterar Situação Produção nos Pedidos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstAlterarSituacaoProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstAlterarSituacaoProducao" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstAlterarSituacaoProducao.GetCli(idCli.value).value.split(';');

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
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblValorDe" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorDe" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblValorAte" runat="server" Text="à" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorAte" runat="server" Width="61px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Num. Orçamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumOrca" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="80"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar3" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="50"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar4" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período Cad.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacao"
                                DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação Prod." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoProd" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsSituacaoProd" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Altura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Largura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsPedido" DataKeyNames="IdPedido"
                    EmptyDataText="Nenhum pedido encontrado." OnRowCommand="grdPedido_RowCommand"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowDataBound="grdPedido_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    CommandName="Edit" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" ImageUrl="~/Images/Ok.gif" CommandName="Update" />
                                <asp:ImageButton ID="imgCancelar" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    CommandName="Cancel" CausesValidation="false" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num" SortExpression="IdPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("IdPedidoExibir") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Proj." SortExpression="IdProjeto">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("IdProjeto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Orça." SortExpression="IdOrcamento">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("IdOrcamento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido Cli." SortExpression="CodCliente">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("CodCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="NomeLoja">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Funcionário" SortExpression="NomeFunc">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Total", "{0:C}") %>' Visible='<%# !(bool)Eval("ExibirTotalEspelho") %>'></asp:Label>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("TotalEspelho", "{0:C}") %>'
                                    Visible='<%# Eval("ExibirTotalEspelho") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("DataPedido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entrega" SortExpression="DataEntrega">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" 
                                    Text='<%# Eval("DataEntregaExibicao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Confirm." SortExpression="DataConf">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Eval("DataConf", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pronto" SortExpression="DataPronto">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" 
                                    Text='<%# Eval("DataPronto", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação" SortExpression="DataLiberacao">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" 
                                    Text='<%# Eval("DataLiberacao", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacaoPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>' Visible='<%# Eval("DescrSituacaoPedido") != "Liberado" %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Produção" SortExpression="DescrSituacaoProducao">
                            <ItemTemplate>
                                <asp:Label ID="lblSitProd" runat="server" Text='<%# Eval("DescrSituacaoProducao") %>'
                                    OnLoad="lblSitProd_Load"></asp:Label>
                                <asp:LinkButton ID="lnkSitProd" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    CommandName="Producao" Text='<%# Eval("DescrSituacaoProducao") %>' OnLoad="lblSitProd_Load"></asp:LinkButton>
                                <asp:ImageButton ID="imbPendentePronto" runat="server" ImageUrl='<%# (int)Eval("SituacaoProducao") == 3 ? "~/Images/curtir.gif" : 
                                    (int)Eval("SituacaoProducao") == 2 ? "~/Images/não curtir.gif" : "" %>' Visible='<%# (int)Eval("SituacaoProducao") == 2 || (int)Eval("SituacaoProducao") == 3 %>'
                                    CommandName="Producao" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacaoProducao" runat="server" SelectedValue='<%# Bind("SituacaoProducao") %>'
                                    DataSourceID="odsSituacaoProd" DataTextField="Descr" DataValueField="Id">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoTipoPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" 
                                    Text='<%# Eval("DescricaoTipoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberado p/ Entrega">
                            <ItemTemplate>
                                <asp:Label ID="lblLiberarFinanc" runat="server" Text='<%# ((bool)Eval("LiberadoFinanc") ? "Sim" : "Não") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Pedido" IdRegistro='<%# Eval("IdPedido") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ID="imgPGSinal" runat="server" ImageUrl="../../Images/cifrao.png" ToolTip='<%# (bool)Eval("PagamentoAntecipado") == true && Eval("IdSinal") != null ? "Sinal e Pagamento Antecipado" :
                                    (bool)Eval("PagamentoAntecipado") == true ? "Pagamento Antecipado" : Eval("IdSinal") != null ? "Sinal" : "" %>'
                                    Visible='<%# (bool)Eval("PagamentoAntecipado") == true || Eval("IdSinal") != null ? true : false %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <sync:ObjectDataSource ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.PedidoDAO" UseDAOInstance="True" UpdateMethod="AlteraSituacaoProducaoPedido"
                    OnUpdated="odsPedido_Updated">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="idCidade" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacaoProd" Name="situacaoProd" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:QueryStringParameter DefaultValue="" Name="byVend" QueryStringField="ByVend"
                            Type="String" />
                        <asp:QueryStringParameter Name="byConf" QueryStringField="ByConf" Type="String" />
                        <asp:QueryStringParameter Name="maoObra" QueryStringField="maoObra" Type="String" />
                        <asp:Parameter Name="maoObraEspecial" Type="String" />
                        <asp:QueryStringParameter Name="producao" QueryStringField="producao" Type="String" />
                        <asp:ControlParameter ControlID="txtNumOrca" Name="idOrcamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" Type="Int32" />
                        <asp:Parameter Name="numeroDiasDiferencaProntoLib" Type="Int32" />
                        <asp:ControlParameter ControlID="txtValorDe" Name="valorDe" PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtValorAte" Name="valorAte" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:Parameter Name="dataFinIni" Type="String" />
                        <asp:Parameter Name="dataFinFim" Type="String" />
                        <asp:Parameter Name="funcFinalizacao" Type="String" />
                        <asp:Parameter Name="tipo" Type="String" />
                        <asp:Parameter Name="tipoVenda" Type="Int32" />
                        <asp:Parameter Name="fastDelivery" Type="Int32" />
                    </SelectParameters>
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsSituacaoProd" runat="server" SelectMethod="GetSituacaoProducao"
                    TypeName="Glass.Data.Helper.DataSources">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                </sync:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
