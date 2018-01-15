<%@ Page Title="Chapa de vidro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadChapaVidro.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadChapaVidro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function setProduto(codInterno)
        {
            try
            {
                FindControl("txtCodInterno", "input").value = codInterno;
                loadProduto(codInterno);
            }
            catch (err) { }
        }

        function loadProduto(codInterno)
        {
            var resposta = CadChapaVidro.GetProduto(codInterno).value;
            var dadosResposta = resposta.split('|');

            if (dadosResposta[0] == "Erro")
            {
                alert(dadosResposta[1]);
                return;
            }

            FindControl("hdfIdProd", "input").value = dadosResposta[1];
            FindControl("lblProd", "span").innerHTML = dadosResposta[2];
        }

        function calcM2()
        {
            var idProd = FindControl("hdfIdProd", "input").value;
            var altura = FindControl("txtAlturaMin", "input").value;
            var largura = FindControl("txtLarguraMin", "input").value;
            var m2 = CadChapaVidro.CalcM2(idProd, altura, largura).value;

            for (numCampo = 1; numCampo <= 3; numCampo++)
            {
                var txtTotM2Min = FindControl("txtTotM2Min" + numCampo, "input");
                txtTotM2Min.value = m2;
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvChapaVidro" runat="server" AutoGenerateRows="False" CellPadding="0"
                    DataSourceID="odsChapaVidro" DefaultMode="Insert" GridLines="None" OnItemInserted="dtvChapaVidro_ItemInserted"
                    OnItemCommand="dtvChapaVidro_ItemCommand">
                    <Fields>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                                    ShowSummary="False" ValidationGroup="c" />
                            </HeaderTemplate>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Produto:
                                        </td>
                                        <td colspan="7" align="left">
                                            <asp:TextBox ID="txtCodInterno" runat="server" Columns="4" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                                onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);" Text='<%# Eval("CodInternoProd") %>'></asp:TextBox>
                                            <asp:Label ID="lblProd" runat="server"></asp:Label>
                                            <asp:ImageButton ID="imgPesqProd" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openWindow(600, 800, '../Utils/SelProd.aspx?chapa=true'); return false;" />
                                            <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Altura:
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAltura" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("Altura") %>' Width="70px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAltura"
                                                Display="Dynamic" ErrorMessage="Informe a altura." SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            Mínimo:
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAlturaMin" runat="server" onblur="calcM2()" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("AlturaMinima") %>' Width="70px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Largura:
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtLargura" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("Largura") %>' Width="70px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLargura"
                                                Display="Dynamic" ErrorMessage="Informe a largura." SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            Mínimo:
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtLarguraMin" runat="server" onblur="calcM2()" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("LarguraMinima") %>' Width="70px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Quantidade:
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtQuantidade" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("Quantidade") %>' Width="70px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtQuantidade"
                                                Display="Dynamic" ErrorMessage="Informe a quantidade." SetFocusOnError="True"
                                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Total m² Mínimo (1):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTotM2Min1" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("TotM2Minimo1") %>' Width="60px"></asp:TextBox>
                                            m² &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            Perc. de aumento Mínimo (1):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtPercAcrescimoTotM21" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("PercAcrescimoTotM21") %>' Width="60px"></asp:TextBox>
                                            %
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Total m² Mínimo (2):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTotM2Min2" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("TotM2Minimo2") %>' Width="60px"></asp:TextBox>
                                            m² &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            Perc. de aumento Mínimo (2):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtPercAcrescimoTotM22" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("PercAcrescimoTotM22") %>' Width="60px"></asp:TextBox>
                                            %
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            Total m² Mínimo (3):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTotM2Min3" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("TotM2Minimo3") %>' Width="60px"></asp:TextBox>
                                            m² &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            Perc. de aumento Mínimo (3):
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtPercAcrescimoTotM23" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("PercAcrescimoTotM23") %>' Width="60px"></asp:TextBox>
                                            %
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsChapaVidro" runat="server" DataObjectTypeName="Glass.Data.Model.ChapaVidro"
                    InsertMethod="Insert" SelectMethod="GetElementByPrimaryKey" TypeName="Glass.Data.DAL.ChapaVidroDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="key" QueryStringField="idChapaVidro" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
