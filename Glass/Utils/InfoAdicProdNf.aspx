<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoAdicProdNf.aspx.cs" Inherits="Glass.UI.Web.Utils.InfoAdicProdNf"
    Title="Dados Adicionais - Produto da Nota Fiscal" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelParticipante.ascx" TagName="ctrlSelParticipante"
    TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript" >
        function exibirVAFRMM(controle)
        {
            if (controle == null)
                return;

            var vAFRMM = FindControl("txtVafrmm", "input");
            if (controle.value == "Maritima")
                vAFRMM.style.display = "";
            else
                vAFRMM.style.display = "none";

            vAFRMM.value = '';
        }

        function exibirUfCnpj(controle) {
            if (controle == null)
                return;

            var txtCnpj = FindControl("txtCnpj", "input");
            var drpUf = FindControl("drpUf", "select");

            if (controle.value != "ImportacaoContaPropria" && controle.value != "0") {
                txtCnpj.style.display = "";
                drpUf.style.display = "";
            }
            else {
                txtCnpj.style.display = "none";
                drpUf.style.display = "none";
            }
        }
    </script>
    <table align="center">
        <tr>
            <td class="subtitle1">
                Documento de Importação
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvProdNf" runat="server" AutoGenerateRows="False" DataSourceID="odsProdNf"
                    DataKeyNames="IdProdNf" DefaultMode="Edit" GridLines="None">
                    <Fields>
                        <asp:TemplateField HeaderText="Número" SortExpression="NumDocImp">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("NumDocImp") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("NumDocImp") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumDocImp") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data de Registro" SortExpression="DataRegDocImp">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlData1" runat="server" DataNullable='<%# Bind("DataRegDocImp") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DataRegDocImp") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataRegDocImp") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Local Desembaraço" SortExpression="LocalDesembaraco">
                            <EditItemTemplate>
                                <table class="pos" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("LocalDesembaraco") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp; UF
                                        </td>
                                        <td style="padding-left: 2px">
                                            <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="True" DataSourceID="odsUf"
                                                DataTextField="Value" DataValueField="Key" SelectedValue='<%# Bind("UfDesembaraco") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("LocalDesembaraco") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("LocalDesembaraco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Desembaraço" SortExpression="DataDesembaraco">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlData2" runat="server" DataNullable='<%# Bind("DataDesembaraco") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DataDesembaraco") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DataDesembaraco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Exportador" SortExpression="CodExportador">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Columns="25" MaxLength="60" Text='<%# Bind("CodExportador") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("CodExportador") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("CodExportador") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Base calc. Imp. Importação" SortExpression="BcIi">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Columns="10" Text='<%# Bind("BcIi") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("BcIi") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("BcIi") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Despeses Aduaneiras" SortExpression="DespAduaneira">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Columns="10" Text='<%# Bind("DespAduaneira") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("DespAduaneira") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DespAduaneira") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Imp. Importação" SortExpression="ValorIi">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Columns="10" Text='<%# Bind("ValorIi") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("ValorIi") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValorIi") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor IOF" SortExpression="ValorIof">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Columns="10" Text='<%# Bind("ValorIof") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("ValorIof") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("ValorIof") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Via de Transporte" SortExpression="tpViaTransp">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpViaTrans" runat="server" SelectedValue='<%# Bind("TpViaTransp") %>'>
                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                    <asp:ListItem Value="Maritima" Text="Marítima"></asp:ListItem>
                                    <asp:ListItem Value="Fluvial" Text="Fluvial"></asp:ListItem>
                                    <asp:ListItem Value="Lacustre" Text="Lacustre"></asp:ListItem>
                                    <asp:ListItem Value="Aerea" Text="Aerea"></asp:ListItem>
                                    <asp:ListItem Value="Postal" Text="Postal"></asp:ListItem>
                                    <asp:ListItem Value="Ferroviaria" Text="Ferroviária"></asp:ListItem>
                                    <asp:ListItem Value="Rodoviaria" Text="Rodoviária"></asp:ListItem>
                                    <asp:ListItem Value="Conduto" Text="Conduto"></asp:ListItem>
                                    <asp:ListItem Value="MeiosProprios" Text="Meios Próprios"></asp:ListItem>
                                    <asp:ListItem Value="EntradaSaidaFicta" Text="Entrada/Saída Ficta"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpViaTrans" runat="server" SelectedValue='<%# Bind("TpViaTransp") %>'>
                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                    <asp:ListItem Value="Maritima" Text="Marítima"></asp:ListItem>
                                    <asp:ListItem Value="Fluvial" Text="Fluvial"></asp:ListItem>
                                    <asp:ListItem Value="Lacustre" Text="Lacustre"></asp:ListItem>
                                    <asp:ListItem Value="Aerea" Text="Aerea"></asp:ListItem>
                                    <asp:ListItem Value="Postal" Text="Postal"></asp:ListItem>
                                    <asp:ListItem Value="Ferroviaria" Text="Ferroviária"></asp:ListItem>
                                    <asp:ListItem Value="Rodoviaria" Text="Rodoviária"></asp:ListItem>
                                    <asp:ListItem Value="Conduto" Text="Conduto"></asp:ListItem>
                                    <asp:ListItem Value="MeiosProprios" Text="Meios Próprios"></asp:ListItem>
                                    <asp:ListItem Value="EntradaSaidaFicta" Text="Entrada/Saída Ficta"></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TpViaTransp")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="VAFRMM" SortExpression="VAFRMM">
                            <EditItemTemplate>
                                <asp:TextBox  ID="txtVafrmm" runat="server" Columns="15" Text='<%# Bind("VAFRMMString") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox  ID="txtVafrmm" runat="server" Columns="15" Text='<%# Bind("VAFRMMString") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("VAFRMM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="TpIntermedio" SortExpression="TpIntermedio">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpFormaImportacao" runat="server" Width="200px"
                                    SelectedValue='<%# Bind("TpIntermedio") %>' 
                                    onchange="exibirUfCnpj(this);">
                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                    <asp:ListItem Value="ImportacaoContaPropria">Importação Por Conta Própria</asp:ListItem>
                                    <asp:ListItem Value="ImportacaoContaOrdem">Importação Por Conta e Ordem</asp:ListItem>
                                    <asp:ListItem Value="ImportacaoEncomenda">Imnportação Por Encomenda</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpFormaImportacao" runat="server" Width="200px"
                                    SelectedValue='<%# Bind("TpIntermedio") %>' 
                                    onchange="exibirUfCnpj(this);">
                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                    <asp:ListItem Value="ImportacaoContaPropria">Importação Por Conta Própria</asp:ListItem>
                                    <asp:ListItem Value="ImportacaoContaOrdem">Importação Por Conta e Ordem</asp:ListItem>
                                    <asp:ListItem Value="ImportacaoEncomenda">Imnportação Por Encomenda</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("TpIntermedio") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CnpjAdquirenteEncomendante" SortExpression="CnpjAdquirenteEncomendante">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" MaxLength="50" Width="120px"
                                    Text='<%# Bind("CnpjAdquirenteEncomendante") %>' 
                                    onkeypress="maskCNPJ(event, this)" Style="display:none"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" MaxLength="50" Width="120px"
                                    Text='<%# Bind("CnpjAdquirenteEncomendante") %>' 
                                    onkeypress="maskCNPJ(event, this)" Style="display:none"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CnpjAdquirenteEncomendante") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Uf Terceiro" SortExpression="UfTerceiro">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key" 
                                    SelectedValue='<%# Eval("UfTerceiro") %>' Style="display:none">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key" 
                                    SelectedValue='<%# Eval("UfTerceiro") %>' Style="display:none">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("UfTerceiro") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="btnFechar" runat="server" OnClientClick="closeWindow(); return false"
                                    Text="Fechar" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="subtitle1">
                Adições
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProdNfAdic" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdProdNfAdicao" DataSourceID="odsProdNfAdic" GridLines="None" ShowFooter="True"
                    OnDataBound="grdProdNfAdic_DataBound" OnRowCommand="grdProdNfAdic_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdProdNf" runat="server" Value='<%# Bind("IdProdNf") %>' />
                                <asp:HiddenField ID="hdfNumSeqAdicao" runat="server" Value='<%# Bind("NumSeqAdicao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir essa adição?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Núm." SortExpression="NumAdicao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumAdicao" runat="server" Text='<%# Bind("NumAdicao") %>' onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumAdicao" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("NumAdicao") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumAdicao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Fabricante" SortExpression="CodFabricante">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodFabric" runat="server" Text='<%# Bind("CodFabricante") %>'
                                    Columns="30" MaxLength="60"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodFabric" runat="server" Columns="30" MaxLength="60" Text='<%# Bind("CodFabricante") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodFabricante") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Desconto" SortExpression="DescAdicao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDesconto" runat="server" Text='<%# Bind("DescAdicao") %>'
                                    onkeypress="return soNumeros(event, false, true)" Columns="10"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDesconto" runat="server" Columns="10" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("DescAdicao") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescAdicao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdNf" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosNf"
                    SelectMethod="GetElement" TypeName="Glass.Data.DAL.ProdutosNfDAO" UpdateMethod="UpdateInfoAdic">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProdNf" QueryStringField="idProdNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdNfAdic" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutoNfAdicao"
                    DeleteMethod="Delete" SelectMethod="GetByProdNf" TypeName="Glass.Data.DAL.ProdutoNfAdicaoDAO"
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProdNf" QueryStringField="idProdNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUf" runat="server" DeleteMethod="GetList" SelectMethod="GetUf"
                    TypeName="Glass.Data.DAL.CidadeDAO">
                    <DeleteParameters>
                        <asp:Parameter Name="uf" Type="String" />
                        <asp:Parameter Name="cidade" Type="String" />
                        <asp:Parameter Name="sortExpression" Type="String" />
                        <asp:Parameter Name="startRow" Type="Int32" />
                        <asp:Parameter Name="pageSize" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
