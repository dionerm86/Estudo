<%@ Page Title="Ajustes no Crédito/Contribuição Social" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstAjusteContribuicao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteContribuicao" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        var loading = true;
        
        function atualizaFonte()
        {
            var valor = FindControl("drpFonteAjuste", "select").value;

            FindControl("hdfFonte", "input").value = valor;

            if (!loading)
            {
                FindControl("selCodCredCont_txtDescr", "input").value = "";
                FindControl("selCodCredCont_hdfValor", "input").value = "";
            }

            FindControl("selCodCredCont_txtDescr", "input").disabled = valor == "";
            FindControl("selCodCredCont_txtDescr", "input").title = valor == "" ? "Selecione o tipo antes de continuar" : "";
            
            FindControl("selCodCredCont_imgPesq", "input").disabled = valor == "";
            FindControl("selCodCredCont_imgPesq", "input").title = valor == "" ? "Selecione o tipo antes de continuar" : "";
        }

        function atualizaAliqImposto()
        {
            var tipoImposto = FindControl("drpTipoImposto", "select").value;
            var data = FindControl("ctrlDataAjuste_txtData", "input").value;

            FindControl("hdfTipoImposto", "input").value = tipoImposto;
            FindControl("hdfData", "input").value = data;

            if (!loading)
            {
                // Recupera a função que será executada
                var funcao = FindControl("selAliqImposto_txtDescr", "input").getAttribute("onchange");

                // Altera a referência do controle de 'this' para 'FindControl'
                // ('this' referencia o objeto 'window')
                while (funcao.indexOf("this") > -1)
                    funcao = funcao.replace("this", "FindControl('selAliqImposto_txtDescr', 'input')");

                // Executa a função
                eval(funcao);
            }

            FindControl("selAliqImposto_txtDescr", "input").disabled = tipoImposto == "" || data == "";
            FindControl("selAliqImposto_txtDescr", "input").title = tipoImposto == "" || data == "" ? "Selecione o tipo de imposto e a data antes de continuar" : "";

            FindControl("selAliqImposto_imgPesq", "input").disabled = tipoImposto == "" || data == "";
            FindControl("selAliqImposto_imgPesq", "input").title = tipoImposto == "" || data == "" ? "Selecione o tipo de imposto e a data antes de continuar" : "";
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdAjusteCont" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsAjusteCont"
                    GridLines="None" OnDataBound="grdAjusteCont_DataBound" ShowFooter="True" OnRowCommand="grdAjusteCont_RowCommand"
                    DataKeyNames="IdAjusteCont">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False" CommandName="Edit"
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="FonteAjuste">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpFonteAjuste" runat="server" DataSourceID="odsFonteAjuste"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("FonteAjuste") %>'
                                    onchange="atualizaFonte()">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpFonteAjuste" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsFonteAjuste" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("FonteAjuste") %>'
                                    onchange="atualizaFonte()">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvFonteAjuste" runat="server" ControlToValidate="drpFonteAjuste"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DescrFonteAjuste") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Imposto" SortExpression="TipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoImposto") %>'
                                    onchange="atualizaAliqImposto()">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoImposto") %>'
                                    onchange="atualizaAliqImposto()">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipoImposto" runat="server" ControlToValidate="drpTipoImposto"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrTipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Ajuste" SortExpression="TipoAjuste">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoAjuste" runat="server" DataSourceID="odsTipoAjuste"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoAjuste") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoAjuste" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoAjuste"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoAjuste") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipoAjuste" runat="server" ControlToValidate="drpTipoAjuste"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrTipoAjuste") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataAjuste">
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlDataAjuste" runat="server" ValidateEmptyText="True" Data='<%# Bind("DataAjuste") %>'
                                    onchange="atualizaAliqImposto()" ReadOnly="ReadWrite" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="ctrlDataAjuste" runat="server" ValidateEmptyText="True" Data='<%# Bind("DataAjuste") %>'
                                    onchange="atualizaAliqImposto()" ReadOnly="ReadWrite" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DataAjuste", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Crédito/Contribuição" SortExpression="CodCredCont">
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="selCodCredCont" runat="server" DataSourceID="odsCodCredCont"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrCodCredCont") %>'
                                    PermitirVazio="False" TituloTela="Selecione o Crédito/Contribuição" Valor='<%# Bind("CodCredCont") %>'
                                    UsarValorRealControle="True" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelPopup ID="selCodCredCont" runat="server" DataSourceID="odsCodCredCont"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrCodCredCont") %>'
                                    PermitirVazio="False" TituloTela="Selecione o Crédito/Contribuição" Valor='<%# Bind("CodCredCont") %>'
                                    UsarValorRealControle="True" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DescrCodCredCont") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aliq. Imposto" SortExpression="AliqImposto">
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="selAliqImposto" runat="server" DataSourceID="odsAliqImposto"
                                    DataTextField="Descr" DataValueField="Descr" Descricao='<%# Eval("AliqImpostoString") %>'
                                    TextWidth="70px" TituloTela="Selecione a Alíquota de Imposto" UsarValorRealControle="True"
                                    Valor='<%# Bind("AliqImpostoString") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelPopup ID="selAliqImposto" runat="server" DataSourceID="odsAliqImposto"
                                    DataTextField="Descr" DataValueField="Descr" Descricao='<%# Eval("AliqImpostoString") %>'
                                    TextWidth="70px" TituloTela="Selecione a Alíquota de Imposto" UsarValorRealControle="True"
                                    Valor='<%# Bind("AliqImpostoString") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("AliqImposto") %>'></asp:Label>
                                %
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodigoAjuste">
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelCodigoAjuste" runat="server" DataSourceID="odsCodigoAjuste"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrCodigoAjuste") %>'
                                    ExibirIdPopup="false" PermitirVazio="False" TituloTela="Selecione o Código de Ajuste"
                                    Valor='<%# Bind("CodigoAjuste") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelCodigoAjuste" runat="server" DataSourceID="odsCodigoAjuste"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrCodigoAjuste") %>'
                                    ExibirIdPopup="false" PermitirVazio="False" TituloTela="Selecione o Código de Ajuste"
                                    Valor='<%# Bind("CodigoAjuste") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrCodigoAjuste") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorAjuste">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorAjuste" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorAjusteString") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorAjuste" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorAjusteString") %>' Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorAjuste", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Núm. Documento" SortExpression="NumeroDocumento">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumDocumento" runat="server" Text='<%# Bind("NumeroDocumento") %>'
                                    Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumDocumento" runat="server" Text='<%# Bind("NumeroDocumento") %>'
                                    Width="100px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NumeroDocumento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' TextMode="MultiLine"
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' TextMode="MultiLine"
                                    Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/ok.gif" OnClick="imgInserir_Click"
                                    Style="height: 15px" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjusteCont" runat="server" DataObjectTypeName="Glass.Data.Model.AjusteContribuicao"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.AjusteContribuicaoDAO" UpdateMethod="Update" EnablePaging="True"
                    >
                    <SelectParameters>
                        <asp:Parameter Name="codCredCont" Type="Int32" />
                        <asp:Parameter Name="dataIni" Type="String" />
                        <asp:Parameter Name="dataFim" Type="String" />
                        <asp:Parameter Name="fonteAjuste" Type="Int32" />
                        <asp:Parameter Name="tipoImposto" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImposto"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoAjuste" runat="server" SelectMethod="GetTipoAjuste"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigoAjuste" runat="server" SelectMethod="GetCodAjusteContCred"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCredCont" runat="server" SelectMethod="GetCodCredCont"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfFonte" Name="fonteAjuste" PropertyName="Value"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFonteAjuste" runat="server" SelectMethod="GetFonteAjuste"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAliqImposto" runat="server" SelectMethod="GetAliquotasPisCofins"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfTipoImposto" Name="tipoImposto" PropertyName="Value"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="hdfData" Name="data" PropertyName="Value" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfFonte" runat="server" />
                <asp:HiddenField ID="hdfTipoImposto" runat="server" />
                <asp:HiddenField ID="hdfData" runat="server" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        atualizaFonte();
        atualizaAliqImposto();
        loading = false;
    </script>

</asp:Content>
