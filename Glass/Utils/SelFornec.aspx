<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelFornec.aspx.cs" Inherits="Glass.UI.Web.Utils.SelFornec"
    Title="Selecione o Fornecedor" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setFornec(idFornec, nomeFantasia, razaoSocial, cpfCnpj, idConta)
        {
            // Se for busca de fornecedor para NF-e
            if (FindControl("hdfNfe", "input").value == 1)
                window.opener.setFornecNfe(idFornec, razaoSocial, cpfCnpj);
            else if (FindControl("hdfNfe", "input").value == 2)
                window.opener.setFornecEmit(idFornec, razaoSocial, cpfCnpj, idConta);
            else if ('<%= Request["callback"] %>' == "setForPopup")
                eval("window.opener." + '<%= Request["controle"] %>').AlteraValor(idFornec, idFornec);
            else if ('<%= Request["callback"] %>' == "setForPart")
                window.opener.ctrlSelParticipante_setFornec(idFornec, '<%= Request["controle"] %>');
            else if ('<%= Request["callback"] %>' == "participanteFiscal")
                window.opener.ControleSelecaoParticipanteFiscal.selecionar('<%= Request["controle"] %>', idFornec, nomeFantasia || razaoSocial);
            else
                window.opener.setFornec(idFornec, nomeFantasia, idConta);

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panPesquisar" runat="server" BorderColor="#EDE9DA" BorderWidth="2"
                    HorizontalAlign="Left" Style="white-space: nowrap" Width="380px">
                    <span style="display: block; text-align: center; font-weight: bold; background-color: #5D7B9D;
                        color: #FFFFFF;">Pesquisar</span>
                    <div style="right: 4px; float: right; position: relative; top: 4px" align="center">
                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                            ToolTip="Pesquisar" />
                        <asp:ImageButton ID="imgExcluirFiltro" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                            OnClick="imgExcluirFiltro_Click" ToolTip="Remover filtro" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkNovoFornec" runat="server" OnClick="lnkNovoFornec_Click" ToolTip="Novo Fornecedor">
                            <img border="0" src="../Images/Insert.gif" /></asp:LinkButton>
                        <br />
                    </div>
                    <div style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px; padding-top: 4px">
                        <table border="0" cellspacing="1">
                            <tr>
                                <td align="left">
                                    Código
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtCodigo" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Nome
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtNomeFantasia" runat="server" Width="250px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    CNPJ
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                        Width="150px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <br />
                <asp:GridView GridLines="None" ID="grdFornecedor" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsFornecedor" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="A pesquisa não retornou resultados."
                    DataKeyNames="IdFornec" AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setFornec('<%# Eval("IdFornec") %>', '<%# Eval("NomeFantasia").ToString().Replace("'", "") %>', '<%# Eval("RazaoSocial") %>', '<%# Eval("CpfCnpj") %>', <%# Eval("IdConta") != null ? Eval("IdConta").ToString() : "null" %>); closeWindow();">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdFornec" HeaderText="Num" SortExpression="IdFornec" />
                        <asp:BoundField DataField="NomeFantasia" HeaderText="Nome" SortExpression="NomeFantasia" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="Telcont" HeaderText="Telefone" SortExpression="Telcont" />
                        <asp:BoundField DataField="Dtultcompra" HeaderText="Ult. Compra" SortExpression="Dtultcompra"
                            DataFormatString="{0:d}" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" DataObjectTypeName="Glass.Data.Model.Fornecedor"
                    DeleteMethod="Delete" SelectMethod="GetFilter" TypeName="Glass.Data.DAL.FornecedorDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountFilter"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="idFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNomeFantasia" Name="nome" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCnpj" Name="cnpj" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfNfe" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
