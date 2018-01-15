<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoAdicNotaFiscal.aspx.cs"
    Inherits="Glass.UI.Web.Utils.InfoAdicNotaFiscal" Title="Informações Adicionais Nota Fiscal"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table align="center">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvInfoAdicNf" runat="server" AutoGenerateRows="False" DataKeyNames="IdNf"
                    DataSourceID="odsInfoAdicNf" DefaultMode="Edit" GridLines="None" OnDataBound="dtvInfoAdicNf_DataBound"
                    OnItemUpdated="dtvInfoAdicNf_ItemUpdated" CellPadding="2">
                    <Fields>
                        <asp:TemplateField HeaderText="Tipo CT-e" SortExpression="TipoCte">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoCte" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCte"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoCte") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("TipoCte") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Classe de Consumo" SortExpression="CodClasseConsumo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodClasseConsumo" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsCodClasseConsumo" DataTextField="Descr" DataValueField="Id"
                                    SelectedValue='<%# Bind("CodClasseConsumo") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodClasseConsumo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Fornecido/Consumido" SortExpression="ValorFornecido">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorFornecido" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorFornecido") %>'></asp:TextBox>
                                <asp:Label ID="Label8" runat="server" ForeColor="Blue" Text="Inclui o valor dos pedágios"
                                    Visible='<%# Eval("IsNfTransporte") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorFornecido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor não Tributado" SortExpression="ValorNaoTributado">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorNaoTributado" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorNaoTributado") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorNaoTributado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Cobrado por Terceiros" SortExpression="ValorCobradoTerceiros">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCobradoTerceiros" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorCobradoTerceiros") %>'></asp:TextBox>
                                <asp:Label ID="Label7" runat="server" Text="Ex: Taxa de iluminação pública" ForeColor="Blue"></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ValorCobradoTerceiros") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Ligação" SortExpression="CodLigacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodLigacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsCodLigacao"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodLigacao") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("CodLigacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo de Tensão" SortExpression="CodGrupoTensao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodGrupoTensao" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsCodGrupoTensao" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodGrupoTensao") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodGrupoTensao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta Contábil" SortExpression="IdContaContabil">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoContaContabil" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsPlanoContaContabil" DataTextField="Descricao" DataValueField="IdContaContabil"
                                    SelectedValue='<%# Bind("IdContaContabil") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("IdContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Assinante" SortExpression="TipoAssinante">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoAssinante" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoAssinante" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoAssinante") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("TipoAssinante") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST" SortExpression="Cst">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpOrigCst" runat="server" SelectedValue='<%# Bind("CstOrig") %>'>
                                    <asp:ListItem>0</asp:ListItem>
                                    <asp:ListItem>1</asp:ListItem>
                                    <asp:ListItem>2</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="drpCst" runat="server" onchange="drpCst_Changed();" SelectedValue='<%# Bind("Cst") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>00</asp:ListItem>
                                    <asp:ListItem Value="10"></asp:ListItem>
                                    <asp:ListItem>20</asp:ListItem>
                                    <asp:ListItem>30</asp:ListItem>
                                    <asp:ListItem>40</asp:ListItem>
                                    <asp:ListItem>41</asp:ListItem>
                                    <asp:ListItem>50</asp:ListItem>
                                    <asp:ListItem>51</asp:ListItem>
                                    <asp:ListItem>60</asp:ListItem>
                                    <asp:ListItem>70</asp:ListItem>
                                    <asp:ListItem>90</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("CstCompleto") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CST IPI" SortExpression="CstIpi">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="drpCstIpi" runat="server" AppendDataBoundItems="True" 
                                        DataSourceID="odsCstIpi" DataTextField="Descr" DataValueField="Id" 
                                        SelectedValue='<%# Bind("CstIpi") %>'>
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label13" runat="server" Text='<%# Bind("CstIpi") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Alíq. ICMS" SortExpression="AliqICMS">
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" Text='<%# Bind("AliqICMS") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAliqIcms" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("AliqICMS") %>'></asp:TextBox>
                                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtAliqIcms"
                                    Display="Dynamic" ErrorMessage="Valor deve ser maior que 0" MaximumValue="999999"
                                    MinimumValue="1" SetFocusOnError="True" ToolTip="Valor deve ser maior que 0"
                                    ValidationGroup="c">*</asp:RangeValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtAliqIcms" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("AliqICMS") %>'></asp:TextBox>
                                <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtAliqIcms"
                                    Display="Dynamic" ErrorMessage="Valor deve ser maior que 0" MaximumValue="999999"
                                    MinimumValue="1" SetFocusOnError="True" ToolTip="Valor deve ser maior que 0"
                                    ValidationGroup="c">*</asp:RangeValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Valor Fiscal" SortExpression="CodValorFiscal">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("CodValorFiscal") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCodValorFiscal" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsCodValorFiscal" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodValorFiscal") %>'
                                    Width="350px">
                                    <asp:ListItem Text="Selecione um código" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="ddlCodValorFiscal"
                                    ErrorMessage="Selecione um código" MaximumValue="3" MinimumValue="1" SetFocusOnError="True"
                                    ToolTip="Selecione um código" ValidationGroup="c">*</asp:RangeValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlCodValorFiscal" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsCodValorFiscal" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodValorFiscal") %>'
                                    Width="350px">
                                    <asp:ListItem Text="Selecione um código" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="ddlCodValorFiscal"
                                    ErrorMessage="Selecione um código" MaximumValue="3" MinimumValue="1" SetFocusOnError="True"
                                    ToolTip="Selecione um código" ValidationGroup="c">*</asp:RangeValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST PIS/Cofins">
                            <EditItemTemplate>
                                <uc2:ctrlSelPopup ID="ctrlSelPis" runat="server" DataSourceID="odsCstPisCofins" DataTextField="Descr"
                                    DataValueField="Id" ExibirIdPopup="true" FazerPostBackBotaoPesquisar="false" TamanhoTela="Tamanho600x400"
                                    TituloTela="Selecione o CST de PIS/Cofins" TextWidth="300px" Valor='<%# Bind("CstPis") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Eval("CstPis") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza BC Crédito">
                            <EditItemTemplate>
                                <uc2:ctrlSelPopup ID="ctrlNatBcCred" runat="server" DataSourceID="odsNatBcCred" DataTextField="Descr"
                                    DataValueField="Id" ExibirIdPopup="false" FazerPostBackBotaoPesquisar="false" TamanhoTela="Tamanho600x400"
                                    TituloTela="Selecione a Natureza BC Crédito" TextWidth="300px" Valor='<%# Bind("NatBcCred") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Eval("NatBcCred") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Salvar" ValidationGroup="c" />
                                <asp:Button ID="btnFechar" runat="server" OnClientClick="closeWindow()" Text="Fechar" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoAdicNf" runat="server" DataObjectTypeName="Glass.Data.Model.InfoAdicionalNf"
        SelectMethod="GetByNf" TypeName="Glass.Data.DAL.InfoAdicionalNfDAO" UpdateMethod="InsertOrUpdate">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodClasseConsumo" runat="server" SelectMethod="GetCodClasseConsumoNf"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodLigacao" runat="server" SelectMethod="GetCodLigacaoNf"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodGrupoTensao" runat="server" SelectMethod="GetCodGrupoTensaoNf"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCte" runat="server" SelectMethod="GetTipoCte" TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
        TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoAssinante" runat="server" SelectMethod="GetTipoAssinanteNf"
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodValorFiscal" runat="server" SelectMethod="GetCodValorFiscal"
        TypeName="Glass.Data.Helper.DataSources" >
        <SelectParameters>
            <asp:QueryStringParameter Name="tipoDocumento" QueryStringField="tipo" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstIpi" runat="server" SelectMethod="GetCstIpi" 
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstPisCofins" runat="server" SelectMethod="GetCstPisCofins" 
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNatBcCred" runat="server" SelectMethod="GetNaturezaBcCredito" 
        TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
</asp:Content>
