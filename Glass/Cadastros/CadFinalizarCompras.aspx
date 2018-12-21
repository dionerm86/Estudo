<%@ Page Title="Finalizar Várias Compras" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadFinalizarCompras.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFinalizarCompras" %>

<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        var checkBoxTabela = new Array();
        
        function getCheckBox()
        {
            if (checkBoxTabela.length == 0)
            {
                var tabela = FindControl("grdCompra", "table");
                for (i = 1; i < tabela.rows.length; i++)
                {
                    var checkBox = FindControl("chkFinalizar", "input", tabela.rows[i].cells[0]);
                    checkBoxTabela.push(checkBox);
                }
            }

            return checkBoxTabela;
        }
        
        function marcarTodas(marcar)
        {
            var checkBox = getCheckBox();
            for (i = 0; i < checkBox.length; i++)
                checkBox[i].checked = marcar;
        }

        function getFornec(idFornec)
        {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function validaFinalizar()
        {
            if (!validate())
                return false;
            
            var checkBox = getCheckBox();
            for (i = 0; i < checkBox.length; i++)
                if (checkBox[i].checked)
                    return true;

            alert("Selecione pelo menos uma compra para finalizar.");
            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Data" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Plano de contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPlanoConta" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContas"
                                DataTextField="DescrPlanoGrupo" DataValueField="IdConta">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdCompra" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataSourceID="odsCompra" EmptyDataText="Não há compra à prazo para finalizar"
                    GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodas" runat="server" onclick="marcarTodas(this.checked)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkFinalizar" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Cód." SortExpression="IdCompra" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="Nf" HeaderText="NF" SortExpression="Nf" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor Entrada"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="DescrPagto" HeaderText="Pagto." SortExpression="DescrPagto" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                                <table>
                    <tr>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            NF/Pedido
                        </td>
                        <td align="left" nowrap="nowrap" valign="middle">
                            <asp:TextBox ID="txtNf" runat="server" MaxLength="20"></asp:TextBox>
                        </td>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            Data entr. fábrica
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc2:ctrlData id="ctrlDataFabrica" runat="server" readonly="ReadWrite" ValidateEmptyText="true" ErrorMessage="Informe a data entr. fábrica." />
                        </td>
                    </tr>
                    <tr id="trFormaPgto">
                        <td align="left" class="dtvHeader" nowrap="nowrap" >
                            Forma Pagto.
                        </td>
                        <td align="left" nowrap="nowrap" >
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="drpFormaPagto" runat="server" onchange="formaPagtoChange(this);"
                                            DataSourceID="odsFormaPagto" DataTextField="Descricao" DataValueField="IdFormaPagto">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        &nbsp;<asp:CheckBox ID="chkBoletoChegou" runat="server" Text="Boleto Chegou" />
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            Número de parcelas
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNumParc" runat="server" OnLoad="drpNumParc_Load">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <uc1:ctrlParcelas ID="ctrlParcelas1" runat="server" ExibirValores="False" NumParcelasLinha="5"
                    OnLoad="ctrlParcelas1_Load" OnPreRender="ctrlParcelas1_PreRender" />
                <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="True" />
                <br />
                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar Compras" OnClientClick="if (!validaFinalizar()) return false"
                    OnClick="btnFinalizar_Click" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCompra" runat="server" SelectMethod="GetListFinalizarVarias"
                    TypeName="Glass.Data.DAL.CompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpPlanoConta" Name="idConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContas" runat="server" SelectMethod="GetPlanoContasCompra"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForCompra"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
