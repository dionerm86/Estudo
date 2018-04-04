<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelNotaFiscalAutorizada.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelNotaFiscalAutorizada" Title="Selecione a Nota Fiscal" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setNf(idNf, numNf) {
            // idControle utilizado para saber à qual cidade descarga será associado a NFe no cadastro de MDFe
            var idControle = "<%= Request["IdControle"] %>";
            if (idControle != "") {
                window.opener.setNfReferenciada(idControle, idNf, numNf);
            }
            else {
                window.opener.setNfReferenciada(idNf, numNf);
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNf" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtCNPJFornecedor" runat="server" Width="170px" 
                            onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc4:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInterno"
                                DataValueField="IdCfop">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Tipo CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbxdrpTipoCFOP" runat="server" CheckAll="False" DataSourceID="odsTipoCfop"
                                DataTextField="Descricao" DataValueField="IdTipoCfop" ImageURL="~/Images/DropDown.png"
                                jqueryurl="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione os tipos de CFOP">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label8" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" Text="Total Nota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdNf" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsNf" DataKeyNames="IdNf" EmptyDataText="Nenhum Nota Fiscal encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowCommand="grdNf_RowCommand" OnRowDataBound="grdNf_RowDataBound"
                    Width="100%">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Eval("IdNf") %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinhaGrid") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='setNf(&#039;<%# Eval("IdNf") %>&#039;,&#039;<%# Eval("NumeroNFe") %>&#039;);return false;'>
                                    <img alt="Selecionar" border="0" src="../Images/ok.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Num. NF" SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="CodCfop" HeaderText="CFOP" SortExpression="CodCfop" />
                        <asp:BoundField DataField="TipoDocumentoString" HeaderText="Tipo" SortExpression="TipoDocumentoString" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="NomeEmitente" HeaderText="Emitente" SortExpression="NomeEmitente" />
                        <asp:BoundField DataField="NomeDestRem" HeaderText="Destinatário/Remetente" SortExpression="NomeDestRem" />
                        <asp:BoundField DataField="TotalNota" HeaderText="Total" SortExpression="TotalNota"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataEmissao" DataFormatString="{0:d}" HeaderText="Data Emissão"
                            SortExpression="DataEmissao" />
                        <asp:BoundField DataField="SituacaoString" HeaderText="Situação" SortExpression="SituacaoString" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNf" runat="server" DataObjectTypeName="Glass.Data.Model.NotaFiscal"
                    DeleteMethod="Delete" SelectMethod="GetListPorSituacao" TypeName="Glass.Data.DAL.NotaFiscalDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountPorSituacao"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumNf" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:Parameter Name="idPedido" Type="UInt32" />
                        <asp:Parameter Name="modelo" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:Parameter Name="idCliente" Type="UInt32" />
                        <asp:Parameter Name="nomeCliente" Type="String" />
                        <asp:Parameter Name="tipoFiscal" Type="Int32" />
                        <asp:Parameter Name="idFornec" Type="UInt32" />
                        <asp:Parameter Name="nomeFornec" Type="String" />
                        <asp:Parameter Name="codRota" Type="String" />
                        <asp:Parameter Name="tipoDoc" Type="Int32" />
                        <asp:Parameter Name="situacao" Type="String" DefaultValue="2,13" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpCfop" Name="idsCfop" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="dataEntSaiIni" Type="String" />
                        <asp:Parameter Name="dataEntSaiFim" Type="String" />
                        <asp:Parameter Name="formaPagto" Type="UInt32" />
                        <asp:Parameter Name="idsFormaPagtoNotaFiscal" Type="String" />
                        <asp:Parameter Name="tipoNf" Type="Int32" />
                        <asp:Parameter Name="finalidade" Type="Int32" />
                        <asp:Parameter Name="formaEmissao" Type="Int32" />
                        <asp:Parameter Name="infCompl" Type="String" />
                        <asp:Parameter Name="ordenar" Type="Int32" />
                        <asp:ControlParameter ControlID="cbxdrpTipoCFOP" Name="idsTiposCfop" PropertyName="SelectedValue" />
                        <asp:Parameter Name="codInternoProd" Type="String" />
                        <asp:Parameter Name="descrProd" Type="String" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtCnpjFornecedor" Name="cnpjFornecedor" PropertyName="Text" Type="String" />
                        <asp:Parameter  Name="lote" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
                    TypeName="Glass.Data.DAL.CfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCfop" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.TipoCfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormasPagto" runat="server"
                    SelectMethod="GetFormasPagtoNf" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
