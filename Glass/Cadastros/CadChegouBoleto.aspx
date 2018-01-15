<%@ Page Title="Chegou Boleto Parcelas Compra" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadChegouBoleto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadChegouBoleto" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
    function validaData(val, args)
    {
        args.IsValid = isDataValida(args.Value);
    }

    function getFornec(idFornec) {
        var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtFornecedor", "input").value = "";
            return false;
        }

        FindControl("txtNome", "input").value = retorno[1];

    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCompra" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="false" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="NF/Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="false" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="false" />
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
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdContaPg" DataSourceID="odsContasPagar" EmptyDataText="Nenhuma conta a pagar encontrada."
                    AllowPaging="True" AllowSorting="True" PageSize="15" OnSelectedIndexChanged="grdConta_SelectedIndexChanged"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button ID="btnChegou" runat="server" CommandName="Select" Text="Chegou" Visible='<%# !(bool)Eval("BoletoChegou") %>' />
                                <asp:Button ID="btnNaoChegou" runat="server" CommandName="Select" Text="Não Chegou"
                                    Visible='<%# Eval("BoletoChegou") %>' Width="98px" />
                                <asp:HiddenField ID="hdfIdContaPg" runat="server" Value='<%# Eval("IdContaPg") %>' />
                                <asp:HiddenField ID="hdfBoletoChegou" runat="server" Value='<%# Eval("BoletoChegou") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Compra" SortExpression="IdCompra" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="ValorVenc" DataFormatString="{0:C}" HeaderText="Valor"
                            SortExpression="ValorVenc" />
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVenc">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>' Visible='<%# (bool)Eval("BoletoChegou") %>'></asp:Label>
                                <asp:TextBox ID="txtDataVenc" runat="server" Columns="10" onkeypress="return mascara_data(event, this), soNumeros(event, true, true);"
                                    onfocus="this.select();" MaxLength="10" Text='<%# Eval("DataVenc", "{0:d}") %>'
                                    Visible='<%# !(bool)Eval("BoletoChegou") %>'>
                                </asp:TextBox>
                                <asp:CustomValidator ID="ctvData" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
                                    ControlToValidate="txtDataVenc" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BoletoChegouString" HeaderText="Boleto Chegou?" SortExpression="BoletoChegouString" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" SelectMethod="GetBoletoChegou"
                    TypeName="Glass.Data.DAL.ContasPagarDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetBoletoChegouCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCompra" Name="idCompra" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNF" Name="nf" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
