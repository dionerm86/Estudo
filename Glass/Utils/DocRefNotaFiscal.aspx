<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocRefNotaFiscal.aspx.cs"
    Inherits="Glass.UI.Web.Utils.DocRefNotaFiscal" Title="Processos/Documentos Referenciados"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelParticipante.ascx" TagName="ctrlSelParticipante"
    TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table align="center">
        <tr>
            <td align="center">
                Apenas para Notas Fiscais com Informações Complementares
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Label ID="Label13" runat="server" Text="Processos Referenciados" CssClass="subtitle1"></asp:Label>
                <asp:GridView ID="grdProcRef" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdProcRef" DataSourceID="odsProcRef"
                    GridLines="None" OnDataBound="grdProcRef_DataBound" OnRowCommand="grdProcRef_RowCommand"
                    ShowFooter="True">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                                <asp:HiddenField ID="hdfIdCte" runat="server" Value='<%# Bind("IdCte") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse processo referenciado?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Origem" SortExpression="Origem">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpOrigem" runat="server" DataSourceID="odsOrigemProcRef" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("Origem") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpOrigem" runat="server" DataSourceID="odsOrigemProcRef" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("Origem") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrOrigem") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número" SortExpression="Numero">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" MaxLength="15" Text='<%# Bind("Numero") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" MaxLength="15" Text='<%# Bind("Numero") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Numero") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAddProc" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAddProc_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr runat="server" id="docArrec">
            <td align="center">
                <br />
                <asp:Label ID="Label3" runat="server" Text="Documentos de Arrecadação Referenciados"
                    CssClass="subtitle1"></asp:Label>
                <asp:GridView ID="grdDocArrec" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsDocArrec" GridLines="None"
                    OnDataBound="grdDocArrec_DataBound" OnRowCommand="grdDocArrec_RowCommand" ShowFooter="True"
                    DataKeyNames="IdDocArrec">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse processo referenciado?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrCodTipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipoDocArrec" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("CodTipo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipoDocArrec" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("CodTipo") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrCodTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="UF" SortExpression="Uf">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" DataSourceID="odsUf" DataTextField="Value"
                                    DataValueField="Key" SelectedValue='<%# Bind("Uf") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" DataSourceID="odsUf" DataTextField="Value"
                                    DataValueField="Key" SelectedValue='<%# Bind("Uf") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Uf") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número" SortExpression="Numero">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" Text='<%# Bind("Numero") %>' Width="80px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" Text='<%# Bind("Numero") %>' Width="80px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Numero") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Aut. Banco" SortExpression="CodAutBanco">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodAutBanco" runat="server" Text='<%# Bind("CodAutBanco") %>'
                                    Width="80px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodAutBanco" runat="server" Text='<%# Bind("CodAutBanco") %>'
                                    Width="80px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodAutBanco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("Valor") %>' onkeypress="return soNumeros(event, false, true)"
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("Valor") %>' Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Valor", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlDataVenc" runat="server" Data='<%# Bind("DataVenc") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlData ID="ctrlDataVenc" runat="server" Data='<%# Bind("DataVenc") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Pagto." SortExpression="DataPagto">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlDataPagto" runat="server" Data='<%# Bind("DataPagto") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlData ID="ctrlDataPagto" runat="server" Data='<%# Bind("DataPagto") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DataPagto", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAddDocArrec" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClick="imgAddDocArrec_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr runat="server" id="docFisc">
            <td align="center">
                <br />
                <asp:Label ID="Label8" runat="server" Text="Documentos Fiscais Referenciados" CssClass="subtitle1"></asp:Label>
                <asp:GridView ID="grdDocFiscal" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsDocFiscal"
                    GridLines="None" OnDataBound="grdDocFiscal_DataBound" OnRowCommand="grdDocFiscal_RowCommand"
                    ShowFooter="True" DataKeyNames="IdDocFiscal">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse processo referenciado?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Partipante" SortExpression="IdCliente, IdFornec, IdLoja, IdTransp">
                            <EditItemTemplate>
                                <uc2:ctrlSelParticipante ID="ctrlSelParticipante1" runat="server" IdCliente='<%# Bind("IdCliente") %>'
                                    IdFornec='<%# Bind("IdFornec") %>' IdLoja='<%# Bind("IdLoja") %>' IdTransportador='<%# Bind("IdTransportador") %>'
                                    IdAdminCartao='<%# Bind("IdAdminCartao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlSelParticipante ID="ctrlSelParticipante1" runat="server" IdCliente='<%# Bind("IdCliente") %>'
                                    IdFornec='<%# Bind("IdFornec") %>' IdLoja='<%# Bind("IdLoja") %>' IdTransportador='<%# Bind("IdTransportador") %>'
                                    IdAdminCartao='<%# Bind("IdAdminCartao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("DescrTipoPart") %>' Font-Italic="True"></asp:Label>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrPart") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipoDocFiscal" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipoDocFiscal" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Emitente" SortExpression="Emitente">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpEmitente" runat="server" DataSourceID="odsEmitDocFiscal"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Emitente") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpEmitente" runat="server" DataSourceID="odsEmitDocFiscal"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Emitente") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DescrEmitente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Modelo" SortExpression="Modelo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtModelo" runat="server" MaxLength="10" Text='<%# Bind("Modelo") %>'
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtModelo" runat="server" MaxLength="10" Text='<%# Bind("Modelo") %>'
                                    Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Modelo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Série" SortExpression="Serie">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Text='<%# Bind("Serie") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Text='<%# Bind("Serie") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Serie") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sub-série" SortExpression="SubSerie">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSubserie" runat="server" MaxLength="3" Text='<%# Bind("SubSerie") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSubserie" runat="server" MaxLength="3" Text='<%# Bind("SubSerie") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("SubSerie") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número" SortExpression="Numero">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" Text='<%# Bind("Numero") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" Text='<%# Bind("Numero") %>' Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Numero") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Emissão" SortExpression="DataEmissao">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlDataEmissao" runat="server" Data='<%# Bind("DataEmissao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlData ID="ctrlDataEmissao" runat="server" Data='<%# Bind("DataEmissao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DataEmissao", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAddDocFiscal" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClick="imgAddDocFiscal_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    </div>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProcRef" runat="server" DataObjectTypeName="Glass.Data.Model.ProcessoReferenciado"
        DeleteMethod="Delete" EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProcessoReferenciadoDAO"
        UpdateMethod="Update">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
            <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrigemProcRef" runat="server" SelectMethod="GetOrigemProcessoRef"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDocArrec" runat="server" DataObjectTypeName="Glass.Data.Model.DocumentoArrecadacao"
        DeleteMethod="Delete" EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.DocumentoArrecadacaoDAO"
        UpdateMethod="Update">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoDocArrec" runat="server" SelectMethod="GetTipoDocumentoArrec"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUf" runat="server" SelectMethod="GetUf" TypeName="Glass.Data.DAL.CidadeDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDocFiscal" runat="server" DataObjectTypeName="Glass.Data.Model.DocumentoFiscal"
        DeleteMethod="Delete" EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.DocumentoFiscalDAO"
        UpdateMethod="Update">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoDocFiscal" runat="server" SelectMethod="GetTipoDocumentoFiscal"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEmitDocFiscal" runat="server" SelectMethod="GetEmitenteDocumentoFiscal"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTransp" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.TransportadorDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
