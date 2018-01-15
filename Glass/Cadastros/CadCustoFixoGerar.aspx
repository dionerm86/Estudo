<%@ Page Title="Gerar Custo Fixo" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCustoFixoGerar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCustoFixoGerar" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function validate() {
            // Verifica se mês/ano informado é válido
            if (!validaMesAno(FindControl("txtData", "input")))
                return false;

            return true;
        }

        function checkAll(checked) {
            var inputs = $('.chkSelCustoFixo input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = checked;
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPeriodo" runat="server" Text="Defina o mês/ano (mm/aaaa):" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            
                            <asp:TextBox ID="txtData" runat="server" Width="60px" MaxLength="7" onkeypress="mascara_mesAno(event, this); return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:CustomValidator ID="ctvData" runat="server" ClientValidationFunction="validate"
                                ControlToValidate="txtData" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblPeriodo0" runat="server" Text="Loja:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"
                                OnSelectedIndexChanged="drpLoja_SelectedIndexChanged" OnChange="return validate();" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return validate();" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Fornecedor:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlSelPopup ID="ctrlSelFornec" runat="server" DataSourceID="odsFornecedor"
                                DataTextField="Nome" DataValueField="IdFornec" FazerPostBackBotaoPesquisar="True"
                                ExibirIdPopup="True" TitulosColunas="Cód.|Nome" TituloTela="Selecione o Fornecedor"
                                TextWidth="250px" />
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
                <asp:GridView GridLines="None" ID="grdCustoFixo" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsCustoFixo" DataKeyNames="IdCustoFixo"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum custo fixo encontrado para o mês especificado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this.checked);"/>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSel" runat="server" CssClass="chkSelCustoFixo"/>
                                <asp:HiddenField ID="hdfIdCustoFixo" runat="server" Value='<%# Eval("IdCustoFixo") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCustoFixo" HeaderText="Cód." SortExpression="IdCustoFixo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="IdNomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="DataUltPagto" DataFormatString="{0:d}" HeaderText="Ult. Pagto"
                            SortExpression="DataUltPagto">
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Contábil" SortExpression="Contabil">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Contabil") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>' Enabled="False" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVenc">
                            <ItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Eval("ValorVenc") %>' onkeypress="return soNumeros(event, false, true);"
                                    Width="70px"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dia Venc." SortExpression="DiaVenc">
                            <ItemTemplate>
                                <asp:TextBox ID="txtDiaVenc" runat="server" MaxLength="2" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Eval("DiaVenc") %>' Width="40px"></asp:TextBox>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs">
                            <ItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="100" Width="170px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <br />
                <asp:Button ID="btnGerar" runat="server" OnClick="btnGerar_Click" OnClientClick="return confirm(&quot;Gerar contas a pagar a partir dos custos fixos selecionados?&quot;);"
                    Text="Gerar Custos Fixos" />
                <asp:HiddenField ID="hdfData" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCustoFixo" runat="server" SelectMethod="GetToGenerate"
                    TypeName="Glass.Data.DAL.CustoFixoDAO" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtData" Name="mesAno" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelFornec" Name="idFornec" PropertyName="Valor"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FornecedorDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
