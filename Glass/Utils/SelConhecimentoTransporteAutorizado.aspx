<%@ Page Title="Selecione o Conhecimento de Transporte" Language="C#" MasterPageFile="~/Layout.master"
    CodeBehind="SelConhecimentoTransporteAutorizado.aspx.cs" Inherits="Glass.UI.Web.Utils.SelConhecimentoTransporteAutorizado" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setCTe(idCTe, numCTe) {
            if ("<%= Request["origem"] %>" == "mdfe") {
                var retorno = SelConhecimentoTransporteAutorizado.VerificarDisponibilidadeCTeCidadeDescargaMdfe(idNf);

                if (retorno.error != null) {
                    alert(retorno.error.description);
                    return;
                }

                var resultado = retorno.value.split('|');

                if (resultado[0] == "Erro") {
                    alert(resultado[1]);
                    return false;
                }
            }

            // idControle utilizado para saber à qual cidade descarga será associado o CTe no cadastro de MDFe
            var idControle = "<%= Request["IdControle"] %>";
            if (idControle != "") {
                window.opener.setCTeReferenciado(idControle, idCTe, numCTe);
            }
            else {
                window.opener.setCTeReferenciado(idCTe, numCTe);
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Num. CTe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCTe" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCTe" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsCTe" DataKeyNames="IdCTe" EmptyDataText="Nenhum CT-e encontrado."
                    CssClass="gridStyle" Width="100%">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdCTe" runat="server" Value='<%# Eval("IdCte") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbSetCTe" runat="server" ImageUrl="~/Images/ok.gif" ToolTip="Selecionar"
                                    OnClientClick='<%# "setCTe(" + Eval("IdCte") + "," + Eval("NumeroCte") + ");return false;" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroCTe" HeaderText="Num. CTe" SortExpression="NumeroCTe" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="TipoDocumentoCteString" HeaderText="Tipo" SortExpression="TipoDocumentoCteString" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="EmitenteCte" HeaderText="Emitente" SortExpression="EmitenteCte" />
                        <asp:BoundField DataField="DestinatarioCte" HeaderText="Destinatário" SortExpression="DestinatarioCte" />
                        <asp:BoundField DataField="DataEmissao" DataFormatString="{0:d}" HeaderText="Data Emissão" SortExpression="DataEmissao" />
                        <asp:BoundField DataField="SituacaoString" HeaderText="Situação" SortExpression="SituacaoString" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsCTe" runat="server" Culture="pt-BR" EnablePaging="true"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    TypeName="Glass.Data.DAL.CTe.ConhecimentoTransporteDAO" DataObjectTypeName="Glass.Data.Model.Cte.ConhecimentoTransporte"
                    SelectMethod="GetList">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCTe" Name="numeroCte" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" />
                        <asp:Parameter Name="situacao" Type="String" DefaultValue="2,13" />
                        <asp:Parameter Name="idCfop" Type="UInt32" />
                        <asp:Parameter Name="formaPagto" Type="Int32" />
                        <asp:Parameter Name="tipoEmissao" Type="Int32" />
                        <asp:Parameter Name="tipoCte" Type="Int32" />
                        <asp:Parameter Name="tipoServico" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmiIni" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmiFim" PropertyName="DataString" />
                        <asp:Parameter Name="idTransportador" Type="UInt32" />
                        <asp:Parameter Name="ordenar" Type="Int32" />
                        <asp:Parameter Name="tipoRemetente" Type="UInt32" />
                        <asp:Parameter Name="idRemetente" Type="UInt32" />
                        <asp:Parameter Name="tipoDestinatario" Type="UInt32" />
                        <asp:Parameter Name="idDestinatario" Type="UInt32" />
                        <asp:Parameter Name="tipoRecebedor" Type="UInt32" />
                        <asp:Parameter Name="idRecebedor" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
