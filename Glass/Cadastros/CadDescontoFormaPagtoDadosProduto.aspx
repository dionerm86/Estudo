<%@ Page Title="Cadastro Desconto por Forma Pagto. e Dados Produto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadDescontoFormaPagtoDadosProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDescontoFormaPagtoDadosProduto" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        var codCartao = CadDescontoFormaPagtoDadosProduto.GetCartaoCod().value;

        function loadSubgrupos(idGrupo) {
            try {
                var drpSubgrupo = FindControl("drpSubgrupo", "select");

                // Remove as opções do DropDownList dos subgrupos
                while (drpSubgrupo.options.length > 0)
                    drpSubgrupo.remove(0);

                // Recupera e insere as opções
                var subgrupos = CadDescontoFormaPagtoDadosProduto.GetSubgrupos(idGrupo).value.split('|');
                for (i = 0; i < subgrupos.length; i++) {
                    var dados = subgrupos[i].split(';');
                    var opcao = document.createElement("option");
                    opcao.value = dados[0];
                    opcao.text = dados[1];

                    drpSubgrupo.options.add(opcao);
                }

                drpSubgrupo.selectedIndex = 0;
                if (FindControl("hdfIdSubgrupo", "input") != null)
                    FindControl("hdfIdSubgrupo", "input").value = "";
            }
            catch (err) {
            }
        }

        function validar()
        {
            if (FindControl("drpSubgrupo", "select").value != "" || FindControl("drpSubgrupo", "select").value != "0")
                FindControl("hdfIdSubgrupo", "input").value = FindControl("drpSubgrupo", "select").value;

            if (FindControl("drpTipoPagto", "select").value == "1")
            {
                if (FindControl("hdfNumParcelas", "input").value == "" || FindControl("hdfDias", "input").value == "")
                {
                    alert("Cadastre os dias das parcelas.");
                    return false;
                }
            }
        
            return true;
        }

        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        if (drpTipoVenda != null) {
            tipoVendaChange(drpTipoVenda, false);
        }

        // Evento acionado ao trocar o tipo de venda (à vista/à prazo)
        function tipoVendaChange(control, calcParcelas) {
            if (control == null)
                return;

            //formaPagtoVisibility();
            formaPagtoChanged();
        }

        // Controla a visibilidade da forma de pagto, escondendo quando
        // o pedido for a vista e exibindo quando o pedido for a prazo
        function formaPagtoVisibility() {

            var control = FindControl("drpTipoVenda", "select");
            var formaPagto = FindControl("drpFormaPagto", "select");

            if (control == null || formaPagto == null)
                return;

            // Se for à vista, obra ou se estiver vazio esconde a forma de pagamento
            if (control.value == 0 || control.value == 1 || control.value == 5 || control.value == 6) {
                formaPagto.selectedIndex = 0;
                formaPagto.style.display = "none";
            }
            else
                formaPagto.style.display = "";
        }

        // Evento acionado quando a forma de pagamento é alterada
        function formaPagtoChanged() {
            var formaPagto = FindControl("drpFormaPagto", "select");
            if (formaPagto == null)
                return;

            FindControl("drpTipoCartao", "select").style.display = formaPagto.value == codCartao ? "" : "none";

            if (formaPagto.value != codCartao)
                FindControl("drpTipoCartao", "select").selectedIndex = 0;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvDescontoFormaPagtoDadosProduto" runat="server" SkinID="defaultDetailsView"
                    DataSourceID="odsDescontoFormaPagtoDadosProduto" DataKeyNames="IdDescontoFormaPagamentoDadosProduto"
                    OnItemInserted="dtvDescontoFormaPagtoDadosProduto_ItemInserted" OnItemUpdated="dtvDescontoFormaPagtoDadosProduto_ItemUpdated">
                    <Fields>
                        <asp:TemplateField HeaderText="Tipo Venda" SortExpression="TipoVenda">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTipoVenda" runat="server" DataSourceID="odsTipoVenda" 
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoVenda") %>'
                                    onchange="tipoVendaChange(this, true);">
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoVenda" runat="server" DataSourceID="odsTipoVenda" 
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoVenda") %>'
                                    onchange="tipoVendaChange(this, true);">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagamento" SortExpression="IdFormaPagto">
                            <InsertItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="true" DataSourceID="odsFormaPagto"
                                                DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'
                                                onchange="formaPagtoChanged();">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="true" DataSourceID="odsFormaPagto"
                                                DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'
                                                onchange="formaPagtoChanged();">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parcela" SortExpression="IdParcela">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descricao"
                                    DataValueField="IdParcela" SelectedValue='<%# Bind("IdParcela") %>' AppendDataBoundItems="true">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descricao"
                                    DataValueField="IdParcela" SelectedValue='<%# Bind("IdParcela") %>' AppendDataBoundItems="true">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo do Produto" SortExpression="IdGrupoProd">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpGrupoProd" runat="server" DataSourceID="odsGrupoProd"
                                    onchange="loadSubgrupos(this.value)" DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                    SelectedValue='<%# Bind("IdGrupoProd") %>' OnDataBound="drpGrupoProd_DataBound">
                                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpGrupoProd" runat="server" onchange="loadSubgrupos(this.value)"
                                    DataSourceID="odsGrupoProd" DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                    SelectedValue='<%# Bind("IdGrupoProd") %>' OnDataBound="drpGrupoProd_DataBound">
                                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Subgrupo do Produto" SortExpression="IdSubgrupoProd">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSubgrupo" runat="server" onchange="alteraVisibilidade(this.value)">
                                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfIdSubgrupo" runat="server" Value='<%# Bind("IdSubgrupoProd") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSubgrupo" runat="server" onchange="alteraVisibilidade(this.value)"
                                    SelectedValue='<%# Eval("IdSubgrupoProd") %>'>
                                    <asp:ListItem Value="" Text="Nenhum"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfIdSubgrupo" runat="server" Value='<%# Bind("IdSubgrupoProd") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desconto (%)" SortExpression="Desconto">
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDesconto" runat="server" Text='<%# Bind("Desconto") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDesconto" runat="server" Text='<%# Bind("Desconto") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="if (!validar()) return false;"
                                    Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!validar()) return false;"
                                    Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle CssClass="dtvHeader" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource ID="odsDescontoFormaPagtoDadosProduto" Culture="pt-BR" runat="server"
                    TypeName="Glass.Data.DAL.DescontoFormaPagamentoDadosProdutoDAO"
                    DataObjectTypeName="Glass.Data.Model.DescontoFormaPagamentoDadosProduto"
                    InsertMethod="Insert" SelectMethod="GetElement" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idDescontoFormaPagamentoDadosProduto" QueryStringField="idDescontoFormaPagamentoDadosProduto" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    SelectMethod="ObtemGruposProduto" TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfTipoConsulta" runat="server" Value="2" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetForControleSelecionar"
                    TypeName="Glass.Data.DAL.ParcelasDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfTipoConsulta" Name="tipo" PropertyName="Value"
                            Type="Object" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="GetCredito"
                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
