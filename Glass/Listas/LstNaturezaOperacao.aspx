<%@ Page Title="Naturezas de Operação: CFOP " Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstNaturezaOperacao" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <asp:GridView ID="grdNaturezaOperacao" runat="server" SkinID="gridViewEditable"
        DataKeyNames="IdNaturezaOperacao" DataSourceID="odsNaturezaOperacao" AutoGenerateColumns="false"
        onrowcommand="grdNaturezaOperacao_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit"
                        ImageUrl="~/Images/EditarGrid.gif" CausesValidation="false" />
                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                        ImageUrl="~/Images/ExcluirGrid.gif" CausesValidation="false"
                        onclientclick="if (!confirm(&quot;Deseja remover essa natureza de operação?&quot;)) return false;" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="imgAtualizar" runat="server" OnClientClick="FindControl('txtPercReducaoBcIcms', 'input').value = FindControl('txtPercReducaoBcIcms', 'input').value.toString().replace(',', '.');"
                        CommandName="Update" ImageUrl="~/Images/Ok.gif" style="margin-left: 0px"/>
                    <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                        CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                    <asp:HiddenField ID="hdfCodigoCfop" runat="server" 
                        Value='<%# Bind("IdCfop") %>' />
                </EditItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Código" SortExpression="CodigoInterno">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInterno") %>'
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string)  %>'></asp:Label>
                    <asp:Label ID="Label3" runat="server" Text='(Padrão)' 
                        Visible='<%# string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCodigoInterno" runat="server" 
                        Text='<%# Bind("CodInterno") %>' MaxLength="10" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>' Columns="11"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodigoInterno" runat="server" 
                        ControlToValidate="txtCodigoInterno" Display="Dynamic" ErrorMessage="*" 
                        Visible='<%# !string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:RequiredFieldValidator>
                    <asp:Label ID="Label2" runat="server" Text='(Padrão)' 
                        Visible='<%# string.IsNullOrEmpty(Eval("CodInterno") as string) %>'></asp:Label>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCodigoInterno" runat="server" MaxLength="10" Columns="11"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodigoInterno" runat="server" 
                        ControlToValidate="txtCodigoInterno" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" Wrap="False" />
                <ItemStyle HorizontalAlign="Center" Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Mensagem" SortExpression="Mensagem">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Mensagem") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtMensagem" runat="server" Text='<%# Bind("Mensagem") %>' 
                        MaxLength="200" Width="230px"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtMensagem" runat="server" MaxLength="200" Width="230px"></asp:TextBox>
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST ICMS" SortExpression="CstIcms">
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("CstIcms") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstIcms" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIcms" DataTextField="Value" DataValueField="Key" 
                        SelectedValue='<%# Bind("CstIcms") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstIcms" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIcms" DataTextField="Value" DataValueField="Key">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Perc. Redução BC ICMS" 
                SortExpression="PercReducaoBcIcms">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPercReducaoBcIcms" runat="server" 
                        Text='<%# Bind("PercReducaoBcIcms") %>' Width="50px"
                        onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    %
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtPercReducaoBcIcms" runat="server" 
                        onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                    %
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("PercReducaoBcIcms") %>'></asp:Label>
                    %
                </ItemTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CSOSN" SortExpression="Csosn">
                <ItemTemplate>
                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("Csosn") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCsosn" runat="server" 
                        SelectedValue='<%# Bind("Csosn") %>' AppendDataBoundItems="True" 
                        DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Descr">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCsosn" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Descr">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST IPI" SortExpression="CstIpi">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstIpi" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIpi" DataTextField="Translation" DataValueField="Key" 
                        SelectedValue='<%# Bind("CstIpi") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstIpi" runat="server" AppendDataBoundItems="True" 
                        DataSourceID="odsCstIpi" DataTextField="Translation" DataValueField="Key">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("CstIpi")).Format() %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cód Enq. Ipi" SortExpression="CodEnqIpi">
                <ItemTemplate>
                    <asp:Label ID="lblCodEnqIpi" runat="server" Text='<%# Eval("CodEnqIpi") %>' Width="30px" MaxLength="3" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCodEnqIpi" runat="server" Text='<%# Bind("CodEnqIpi") %>' Width="30px" MaxLength="3" />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCodEnqIpi" runat="server" Width="30px" MaxLength="3" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CST Pis/Cofins" SortExpression="CstPisCofins">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpCstPisCofins" runat="server" 
                        AppendDataBoundItems="True" DataSourceID="odsCstPisCofins" 
                        DataTextField="Descr" DataValueField="Id" 
                        SelectedValue='<%# Bind("CstPisCofins") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpCstPisCofins" runat="server" 
                        AppendDataBoundItems="True" DataSourceID="odsCstPisCofins" 
                        DataTextField="Descr" DataValueField="Id">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("CstPisCofins", "{0:00}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular ICMS" SortExpression="CalcularIcms">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox7" runat="server" Checked='<%# Bind("CalcIcms") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcms" runat="server" 
                        Checked='<%# Bind("CalcIcms") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIcms" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular ICMS-ST" 
                SortExpression="CalcularIcmsSt">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("CalcIcmsSt") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsSt" runat="server" 
                        Checked='<%# Bind("CalcIcmsSt") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIcmsSt" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular IPI" SortExpression="CalcularIpi">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("CalcIpi") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularIpi" runat="server" 
                        Checked='<%# Bind("CalcIpi") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularIpi" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular PIS" SortExpression="CalcularPis">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("CalcPis") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularPis" runat="server" 
                        Checked='<%# Bind("CalcPis") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularPis" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular Cofins" SortExpression="CalcularCofins">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("CalcCofins") %>' 
                        Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularCofins" runat="server" 
                        Checked='<%# Bind("CalcCofins") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularCofins" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="IPI Integra B.C. ICMS *" 
                SortExpression="IpiIntegraBaseCalculoIcms">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server"
                        Checked='<%# Bind("IpiIntegraBcIcms") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkIpiIntegraBaseCalculoIcms" runat="server"
                        Checked='<%# Bind("IpiIntegraBcIcms") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkIpiIntegraBaseCalculoIcms" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Frete Integra B.C. IPI" 
                SortExpression="FreteIntegraBaseCalculoIpi">
                <ItemTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server"
                        Checked='<%# Bind("FreteIntegraBcIpi") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server"
                        Checked='<%# Bind("FreteIntegraBcIpi") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkFreteIntegraBaseCalculoIpi" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Alterar Estoque Fiscal" 
                SortExpression="AlterarEstoqueFiscal">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" 
                        Checked='<%# Bind("AlterarEstoqueFiscal") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" 
                        Checked='<%# Bind("AlterarEstoqueFiscal") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calcular Difal" 
                SortExpression="CalcularDifal">
                <ItemTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" 
                        Checked='<%# Bind("CalcularDifal") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" 
                        Checked='<%# Bind("CalcularDifal") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcularDifal" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Calc. Energia Elétrica"  SortExpression="CalcEnergiaEletrica">
                <ItemTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" 
                        Checked='<%# Bind("CalcEnergiaEletrica") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" 
                        Checked='<%# Bind("CalcEnergiaEletrica") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCalcEnergiaEletrica" runat="server" />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
             <asp:TemplateField HeaderText="NCM" 
                SortExpression="Ncm">
                <ItemTemplate>
                    <asp:Label ID="lblNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                   <asp:TextBox ID="txtNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtNcm" runat="server" Text='<%# Bind("Ncm") %>' />
                </FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                        IdRegistro='<%# (uint)(int)Eval("IdNaturezaOperacao") %>' Tabela="NaturezaOperacao" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/Insert.gif" 
                        onclick="imgInserir_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Label ID="Label9" runat="server" ForeColor="Red" Text='* IPI integra BC ICMS apenas para clientes do Tipo Fiscal "Consumidor Final"'></asp:Label>
    <colo:VirtualObjectDataSource ID="odsNaturezaOperacao" runat="server" 
        DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.NaturezaOperacao" 
        DeleteMethod="ApagarNaturezaOperacao" EnablePaging="True" MaximumRowsParameterName="pageSize" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarNaturezasOperacao" 
        SelectByKeysMethod="ObtemNaturezaOperacao"
        UpdateStrategy="GetAndUpdate"
        SortParameterName="sortExpression"
        TypeName="Glass.Fiscal.Negocios.ICfopFluxo" 
        UpdateMethod="SalvarNaturezaOperacao">
        <SelectParameters>
            <asp:QueryStringParameter Name="idCfop" QueryStringField="idCfop" 
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsCstIcms" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="" MaximumRowsParameterName="" SelectMethod="GetCstIcms" SkinID="" 
        StartRowIndexParameterName="" TypeName="Glass.Data.EFD.DataSourcesEFD">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCstIpi" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ProdutoCstIpi, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsCstPisCofins" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        MaximumRowsParameterName="" SelectMethod="GetCstPisCofins" 
        StartRowIndexParameterName="" TypeName="Glass.Data.EFD.DataSourcesEFD">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="exibirNumeroDescricao" 
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCsosn" runat="server" SelectMethod="GetCSOSN" TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
</asp:Content>

