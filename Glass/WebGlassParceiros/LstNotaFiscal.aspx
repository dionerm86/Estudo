<%@ Page Title="Notas Fiscais" Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="LstNotaFiscal.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.LstNotaFiscal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptDanfe(idNf) {
            openWindow(600, 800, "RelBase.aspx?rel=Danfe&idNf=" + idNf);
            return false;
        }

        function salvarNota(idNf) {
            redirectUrl('<%= this.ResolveClientUrl("../Handlers/NotaXml.ashx") %>?idNf=' + idNf);
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
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label8" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdNf" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsNf" DataKeyNames="IdNf" EmptyDataText="Nenhum Nota Fiscal encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("PrintDanfeVisible") %>'>
                                    <a href="#" onclick="openRptDanfe('<%# Eval("IdNf") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" border="0" /></a></asp:PlaceHolder>
                                <asp:LinkButton ID="lnkSalvarXmlNota" runat="server" Visible='<%# Eval("PrintDanfeVisible") %>'
                                    OnClientClick='<%# "salvarNota(\"" + Eval("IdNf") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/disk.gif" title="Salvar arquivo da nota fiscal" /></asp:LinkButton>
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinhaGrid") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Num. NF" SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="TipoDocumentoString" HeaderText="Tipo" SortExpression="TipoDocumentoString" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="NomeEmitente" HeaderText="Emitente" SortExpression="NomeEmitente" />
                        <asp:BoundField DataField="NomeDestRem" HeaderText="Destinatário/Remetente" SortExpression="NomeDestRem" />
                        <asp:BoundField DataField="TotalNota" HeaderText="Total" SortExpression="TotalNota"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataEmissao" DataFormatString="{0:d}" HeaderText="Data Emissão"
                            SortExpression="DataEmissao" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="SituacaoString">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" ForeColor='<%# Eval("CorSituacao") %>' Text='<%# Bind("SituacaoString") %>'
                                    Style='<%# (int)Eval("Situacao") == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros ? "position: relative; bottom: 3px": "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("SituacaoString") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNf" runat="server" DataObjectTypeName="Glass.Data.Model.NotaFiscal"
                    DeleteMethod="Delete" SelectMethod="GetListAcessoExterno" TypeName="Glass.Data.DAL.NotaFiscalDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetAcessoExternoCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumNf" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
