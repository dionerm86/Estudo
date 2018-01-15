<%@ Page Title="Retificar Boletos Antecipados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadRetificarAntecipContaRec.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarAntecipContaRec" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <script type="text/javascript">

        function checkAll(checked) {
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = !inputs[i].disabled && checked;
            }

            calculaTotal();
        }

        function verificaPodeRetificar(control) {

            var ret = CadRetificarAntecipContaRec.VerificaPodeRetificarAntecipacao(control.value);

            if (ret.error != null) {
                alert(ret.error.description);
                return false;
            }

            if (ret.value == "false") {
                alert("Não é possível retificar essa antecipação, pois ela possui contas que já foram recebidas.");
                return false;
            }

            return true;
        }

        function calculaTotal() {
            if (document.getElementById("<%= btnRetificarAntecipacao.ClientID %>") == null)
                return;

            var total = 0;
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox" && inputs[i].id.indexOf("chkAll") == -1 && inputs[i].checked) {
                    var somar = inputs[i].parentNode.getAttribute("Valor");
                    if (somar == null)
                        continue;
                    total += parseFloat(somar);
                }
            }

            var lblTotal = document.getElementById("<%= lblTotalContas.ClientID %>");
            lblTotal.innerHTML = total.toFixed(2).replace('.', ',');
            document.getElementById("<%= hdfValor.ClientID %>").value = total.toFixed(2).replace('.', ',');
        }


    </script>

    <table>
        <tr>
            <td align="center">Apenas antecipações que ainda não tiveram contas recebidas podem ser retificadas.</td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Num. Antecip."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAntecip" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);" onchange="return verificaPodeRetificar(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return verificaPodeRetificar(this);" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table id="tbRetificarAntecipacao" runat="server" clientidmode="Static">
        <tr>
            <td align="center">Selecione as contas à receber que continuarão na antecipação
                            <img src="../Images/load.gif" id="loading" style="display: none; padding-bottom: 3px" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContasRec" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsContasRec" DataKeyNames="IdContaR" EmptyDataText="Não há contas à receber para o filtro especificado."
                    PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnDataBound="grdContasRec_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this.checked);" Checked="True" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSel" runat="server" onclick="calculaTotal()"
                                    Checked="True" OnDataBinding="chkSel_DataBinding" />
                                <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Refêrencia" SortExpression="Referencia" />
                        <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdNomeCli" />
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <ItemTemplate>
                                <asp:Label ID="lblValor" runat="server" Text='<%# Bind("ValorVec") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataVec" HeaderText="Vemcimento" SortExpression="DataVec" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <HeaderStyle Wrap="False" />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td style="font-size: large" align="center">Total das Contas:
                <asp:Label ID="lblTotalContas" runat="server">0,00</asp:Label>
                <asp:HiddenField ID="hdfValor" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView runat="server" ID="dtvAntecipacao" DataSourceID="odsAntecipacao"
                    AutoGenerateRows="false" DataKeyNames="IdAntecipContaRec" GridLines="None">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td align="center">
                                            <table>
                                                <tr>
                                                    <td>Taxa:
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTaxa" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Eval("Taxa") %>'></asp:TextBox>
                                                    </td>
                                                    <td>IOF
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtIof" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Eval("Iof") %>'></asp:TextBox>
                                                    </td>
                                                    <td>Juros
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtJuros" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Eval("Juros") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="center">
                                            <table>
                                                <tr>
                                                    <td>Conta Bancária
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                                            DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="true"
                                                            SelectedValue='<%# Eval("IdContaBanco") %>'>
                                                            <asp:ListItem Value="0" Text="" Selected="True"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>Data Receb.
                                                    </td>
                                                    <td>
                                                        <uc1:ctrlData ID="ctrlDataRecebimento" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true"
                                                            Data='<%# Eval("Data") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="300px"
                                                            Text='<%# Eval("Obs") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField runat="server" ID="hdfIdAntecip" Value='<%# Eval("IdAntecipContaRec") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkEstornar" runat="server" Text="Estornar Movimentação?" /></td>
                        <td>&nbsp;&nbsp;</td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Data Estorno"></asp:Label></td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEstorno" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnRetificarAntecipacao" runat="server"
                    OnClientClick="if (!confirm('Deseja retificar essa antecipação?')) return false"
                    Text="Retificar Antecipação" OnClick="btnRetificarAntecipacao_Click" />
            </td>
        </tr>
    </table>

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasRec" runat="server"
        SelectMethod="GetByAntecipacao" TypeName="Glass.Data.DAL.ContasReceberDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumAntecip" Name="idAntecipContaRec" PropertyName="Text"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAntecipacao" runat="server"
        SelectMethod="GetElement" TypeName="Glass.Data.DAL.AntecipContaRecDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumAntecip" Name="idAntecipContaRec" PropertyName="Text"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>


    </table>

     <script type="text/javascript">
         calculaTotal();
     </script>

    </table>

</asp:Content>
