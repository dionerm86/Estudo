<%@ Page Title="Cancelar Custos Fixos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCustoFixoCancelar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCustoFixoCancelar"
    EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function cancelar() {
            var cData = FindControl('txtData', 'input');

            // Verifica se a data foi informada
            if (cData.value == "") {
                alert("Informe a data de cancelamento das Contas a Pagar/Pagas de custos fixos.");
                cData.focus();
                return false;
            }

            // Confirma cancelamento de contas a pagar
            if (!confirm('Tem certeza que deseja cancelar todas as Contas a Pagar/Pagas de custos fixos do mês ' + cData.value + '?'))
                return false;

            // Verifica se mês/ano informado é válido
            if (!validaMesAno(cData))
                return false;

            // Cancela custos fixos via AJAX
            var result = CadCustoFixoCancelar.Cancelar(cData.value);

            // Se o retorno do AJAX não tiver valor, mostra mensagem de erro
            if (result == "" || result == null || result.value == null) {
                alert("Falha ao cancelar Contas a Pagar/Pagas. Erro: AJAX");
                return false;
            }

            // Se tiver ocorrido algum erro, exibe a mensagem de erro
            if (result.value != "ok") {
                alert(result.value.split('\t')[1]);
                return false;
            }

            alert("Contas a Pagar/Pagas do mês " + cData.value + " canceladas com sucesso.");

            cData.value = "";

            return false;
        }

        function validate() {
            // Verifica se mês/ano informado é válido
            if (!validaMesAno(FindControl("txtData", "input")))
                return false;

            return true;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table id="tbMesAno">
                    <tr>
                        <td>
                            <asp:Label ID="lblPeriodo" runat="server" ForeColor="#0066FF" Text="Data para cancelamento de custos fixos (mm/yyyy):"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtData" runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscar', 'input');"
                                onkeypress="mascara_mesAno(event, this); return soNumeros(event, true, true);"
                                MaxLength="7" Width="60px"></asp:TextBox>
                            <asp:CustomValidator ID="ctvData" runat="server" ClientValidationFunction="validate"
                                ControlToValidate="txtData" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar contas" />
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
        <tr runat="server" id="dados" visible="false">
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContasPagar" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsContasPagar" OnRowCommand="grdContasPagar_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandArgument='<%# Eval("IdContaPg") %>'
                                    CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja cancelar esse custo fixo?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrCustoFixo" HeaderText="Custo Fixo" SortExpression="DescrCustoFixo" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="DataVenc" HeaderText="Data Venc." SortExpression="DataVenc"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="ValorVenc" HeaderText="Valor Venc." SortExpression="ValorVenc"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="PagaString" HeaderText="Paga?" SortExpression="PagaString" />
                        <asp:BoundField DataField="DataPagto" HeaderText="Data Pagto." SortExpression="DataPagto"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="ValorPago" HeaderText="Valor Pago" SortExpression="ValorPago"
                            DataFormatString="{0:c}" />
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" SelectMethod="GetCustoFixoByMesAno"
                    TypeName="Glass.Data.DAL.ContasPagarDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtData" Name="mesAno" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar todos" OnClientClick="return cancelar();"
                    Visible="False" />
            </td>
        </tr>
    </table>
</asp:Content>
