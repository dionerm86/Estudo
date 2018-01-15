<%@ Page Title="Obrigações do ICMS Recolhido ou a Recolher - Operações Próprias" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadObrigacaoRecolhidoRecolher.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadObrigacaoRecolhidoRecolher" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td>
                <asp:DetailsView ID="dtvObrigacao" runat="server" AutoGenerateRows="False" DataSourceID="odsObrigacao"
                    DefaultMode="Insert" GridLines="None" CellPadding="4"
                    Style="margin-right: 2px" DataKeyNames="Id" OnItemInserting="dtvObrigacao_ItemInserting"
                    OnItemUpdating="dtvObrigacao_ItemUpdating">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle CssClass="dtvAlternatingRow" />
                    <FieldHeaderStyle Wrap="False" Font-Bold="False" CssClass="dtvHeader" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Imposto" SortExpression="TipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator0" runat="server" ControlToValidate="drpTipoImposto"
                                    ErrorMessage="Selecione o tipo de imposto" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator0" runat="server" ControlToValidate="drpTipoImposto"
                                    ErrorMessage="Selecione o tipo de imposto" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("TipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="UF" SortExpression="Uf">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key" 
                                    SelectedValue='<%# Bind("Uf") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvUf" runat="server" ControlToValidate="drpUf" 
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Uf") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Obrigação" SortExpression="CodigoObrigacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodigoObrigacao" runat="server" DataSourceID="odsCodObrigacao"
                                    DataTextField="Descricao" DataValueField="Codigo" SelectedValue='<%# Bind("CodigoObrigacao", "{0:000}") %>'
                                    Width="254px">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="drpCodigoObrigacao"
                                    ErrorMessage="Selecione o Código da Obrigação" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodigoObrigacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("Valor") %>' Width="250px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtValor"
                                    ErrorMessage="Informe um valor" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("Valor") %>' Width="250px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtValor"
                                    ErrorMessage="Informe um valor" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVencimento">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DataVencimento") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" Data='<%# Bind("DataVencimento") %>'
                                    ValidateEmptyText="True" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" Data='<%# Bind("DataVencimento") %>'
                                    ValidateEmptyText="True" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Receita" SortExpression="CodigoReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodigoReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodigoReceita" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("CodigoReceita") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtCodigoReceita"
                                    ErrorMessage="Selecione o Código da Receita" SetFocusOnError="True" 
                                    ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Processo" SortExpression="NumeroProcesso">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumeroProcesso" runat="server" Text='<%# Bind("NumeroProcesso") %>'
                                    Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNumeroProcesso" runat="server" Text='<%# Bind("NumeroProcesso") %>'
                                    Width="250px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NumeroProcesso") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ind. Origem" SortExpression="IndicadorOrigem">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("IndicadorOrigem") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpIndicadorOrigem" runat="server" DataSourceID="odsIndicadorOrigem"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("IndicadorOrigem") %>'
                                    Width="254px">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpIndicadorOrigem" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsIndicadorOrigem" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("IndicadorOrigem") %>'
                                    Width="254px">
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desc. Processo" SortExpression="DescricaoProcesso">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricaoProcesso" runat="server" Text='<%# Bind("DescricaoProcesso") %>'
                                    Height="45px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricaoProcesso" runat="server" Text='<%# Bind("DescricaoProcesso") %>'
                                    Height="45px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DescricaoProcesso") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desc. Complementar" SortExpression="DescricaoComplementar">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricaoComplementar" runat="server" Text='<%# Bind("DescricaoComplementar") %>'
                                    Height="45px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricaoComplementar" runat="server" Text='<%# Bind("DescricaoComplementar") %>'
                                    Height="45px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DescricaoComplementar") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mês Ref." SortExpression="MesReferencia">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMesReferencia" runat="server" OnKeypress="return mascara_periodo(event, this);"
                                    Text='<%# Bind("MesReferencia") %>' Width="250px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMesReferencia"
                                    ErrorMessage="Informe mês e ano" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtMesReferencia" runat="server" OnKeypress="return mascara_periodo(event, this);"
                                    Text='<%# Bind("MesReferencia") %>' Width="250px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMesReferencia"
                                    ErrorMessage="Informe mês e ano" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("MesReferencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle ForeColor="Black" />
                </asp:DetailsView>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObrigacao" runat="server" DataObjectTypeName="Glass.Data.Model.ObrigacaoRecolhidoRecolher"
                    DeleteMethod="Delete" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ObrigacaoRecolhidoRecolherDAO"
                    UpdateMethod="Update"  InsertMethod="Insert" OnInserted="odsObrigacao_Inserted"
                    OnUpdated="odsObrigacao_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="id" QueryStringField="id" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndicadorOrigem" runat="server" SelectMethod="GetIndProc"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodObrigacao" runat="server" SelectMethod="GetTabelaCodigoObrigacaoICMS"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUf" runat="server" SelectMethod="GetUf" 
                    TypeName="Glass.Data.DAL.CidadeDAO" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
