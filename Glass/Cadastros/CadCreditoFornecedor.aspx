<%@ Page Title="Gerar Crédito de Fornecedor" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadCreditoFornecedor.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCreditoFornecedor" %>

<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getFornec(id) {
            if (id.value == "")
                return;

            var retorno = MetodosAjax.GetFornec(id.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                id.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
            FindControl("drpPlanoContas", "select").value = retorno[3];
        }

        function onInsertUpdate() {

            if (FindControl("drpPlanoContas", "select").value == "") {
                alert("Informe o Plano de contas.");
                return false;
            }

            if (FindControl("txtNomeFornec", "input").value == "") {
                alert("Informe o Fornecedor.");
                return false;
            }

            return true;
        }

        // Abre popup para cadastrar cheques
        function queryStringCheques() {
            return "?origem=5";
        }

        function getUrlCheques(tipoPagto, urlPadrao) {
            return tipoPagto == 2 ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
        }
    </script>

    <table width="100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCredFornec" DataKeyNames="IdCreditoFornecedor" runat="server"
                    AutoGenerateRows="False" DataSourceID="odsCredFornec" DefaultMode="Insert" GridLines="None">
                    <Fields>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <table cellspacing="0">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Descrição
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="200" Rows="3" Width="400px"
                                                Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Fornecedor
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getFornec(this);" Text='<%# Bind("IdFornecedor") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="320px"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                OnClientClick="openWindow(590, 760, '../Utils/SelFornec.aspx'); return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Plano de Contas
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                                SelectedValue='<%# Bind("IdConta") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            <div id="a_vista">
                                                <uc2:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" ParentID="a_vista" BloquearCamposContaVazia="false"
                                                    ExibirDataRecebimento="true" ExibirComissaoComissionado="false" OnLoad="ctrlFormaPagto1_Load"
                                                    ExibirGerarCredito="false" ExibirJuros="false" ExibirRecebParcial="false" ExibirUsarCredito="false"
                                                    ExibirValorAPagar="false" ContasBanco='<%# Bind("ContasBancoPagto") %>' FormasPagto='<%# Bind("FormasPagto") %>'
                                                    FuncaoQueryStringCheques="queryStringCheques" NumAutConstrucard='<%# Bind("NumAutConstrucard") %>'
                                                    ParcelasCartao='<%# Bind("ParcelasCartaoPagto") %>' TiposCartao='<%# Bind("TiposCartaoPagto") %>'
                                                    Valores='<%# Bind("ValoresPagto") %>' CobrarJurosCartaoClientes="False" DataRecebimento='<%# Bind("DataRecebimento") %>'
                                                    EfetuarBindContaBanco="false" DatasFormasPagamento='<%# Bind("DatasPagto") %>'
                                                    ExibirCredito="False" ChequesString='<%# Bind("ChequesPagto") %>' FuncaoUrlCheques="getUrlCheques" MetodoFormasPagto="GetForPagto" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCredFornec" runat="server" InsertMethod="Insert" SelectMethod="ObterCreditoFornecedor"
                    TypeName="Glass.Data.DAL.CreditoFornecedorDAO" DataObjectTypeName="Glass.Data.Model.CreditoFornecedor"
                    OnInserted="odsCredFornec_Inserted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCreditoFornecedor" QueryStringField="id" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForRecebConta"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco1" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco2" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao1" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao2" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas" TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
