<%@ Page Title="Cadastro de Veículos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadVeiculos.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadVeiculos" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvVeiculos" runat="server" AutoGenerateRows="False" DataSourceID="odsVeiculos"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" CellPadding="4"
                    ForeColor="#333333">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="Black" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Placa" SortExpression="Placa">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPlaca" runat="server" MaxLength="8" Text='<%# Bind("Placa") %>'
                                    Width="100px" Enabled="false"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdPlaca" runat="server" ControlToValidate="txtPlaca"
                                    ErrorMessage="Preencha o campo Placa">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPlaca" runat="server" MaxLength="8" Text='<%# Bind("Placa") %>'
                                    Width="100px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdPlaca" runat="server" ControlToValidate="txtPlaca"
                                    ErrorMessage="Preencha o campo Placa">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Placa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Modelo" SortExpression="Modelo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtModelo" runat="server" MaxLength="20" Text='<%# Bind("Modelo") %>'
                                    Width="200px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtModelo"
                                    ErrorMessage="Preencha o campo Modelo">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtModelo" runat="server" MaxLength="20" Text='<%# Bind("Modelo") %>'
                                    Width="200px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtModelo"
                                    ErrorMessage="Preencha o campo Modelo">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Modelo") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ano Fabr." SortExpression="Anofab">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAnoFabr" runat="server" MaxLength="4" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Anofab") %>' Width="100px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAnoFabr"
                                    ErrorMessage="Preencha o campo Ano Fabr.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtAnoFabr" runat="server" MaxLength="4" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Anofab") %>' Width="100px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAnoFabr"
                                    ErrorMessage="Preencha o campo Ano Fabr.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Anofab") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cor" SortExpression="Cor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCor" runat="server" MaxLength="20" Text='<%# Bind("Cor") %>'
                                    Width="150px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtCor"
                                    ErrorMessage="Preencha o campo Cor">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCor" runat="server" MaxLength="20" Text='<%# Bind("Cor") %>'
                                    Width="150px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtCor"
                                    ErrorMessage="Preencha o campo Cor">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Cor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Km Inicial" SortExpression="Kminicial">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtKmInicial" runat="server" MaxLength="9" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Kminicial") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtKmInicial"
                                    ErrorMessage="Preencha o campo Km Inicial.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtKmInicial" runat="server" MaxLength="9" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Kminicial") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtKmInicial"
                                    ErrorMessage="Preencha o campo Km Inicial.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Kminicial") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor IPVA" SortExpression="Valoripva">
                            <EditItemTemplate>
                                <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("Valoripva") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("Valoripva") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Valoripva") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Renavam" SortExpression="Renavam">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtRenavam" runat="server" MaxLength="11" Text='<%# Bind("Renavam") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtRenavam"
                                    ErrorMessage="Preencha o campo Renavam.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtRenavam" runat="server" MaxLength="11" Text='<%# Bind("Renavam") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtRenavam"
                                    ErrorMessage="Preencha o campo Renavam.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Renavam") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tara" SortExpression="Tara">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTara" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Tara") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtTara"
                                    ErrorMessage="Preencha o campo Tara.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtTara" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Tara") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtTara"
                                    ErrorMessage="Preencha o campo Tara.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Tara") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Capacidade Kg" SortExpression="CapacidadeKg">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCapacidadeKg" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("CapacidadeKg") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtCapacidadeKg"
                                    ErrorMessage="Preencha o campo Capacidade KG.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCapacidadeKg" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("CapacidadeKg") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtCapacidadeKg"
                                    ErrorMessage="Preencha o campo Capacidade KG.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CapacidadeKg") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Capacidade M³" SortExpression="CapacidadeM3">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCapacidadeM3" runat="server" MaxLength="3" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("CapacidadeM3") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtCapacidadeM3"
                                    ErrorMessage="Preencha o campo Capacidade M³.">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCapacidadeM3" runat="server" MaxLength="3" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("CapacidadeM3") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtCapacidadeM3"
                                    ErrorMessage="Preencha o campo Capacidade M³.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CapacidadeM3") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Proprietário" SortExpression="TipoProprietario">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoProp" runat="server" SelectedValue='<%# Bind("TipoProprietario") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Próprio" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Terceiro" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoProp" ControlToValidate="ddlTipoProp" runat="server"
                                    ErrorMessage="Campo Tipo Proprietário deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoProp" runat="server" SelectedValue='<%# Bind("TipoProprietario") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Próprio" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Terceiro" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoProp" ControlToValidate="ddlTipoProp" runat="server"
                                    ErrorMessage="Campo Tipo Proprietário deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("TipoProprietario") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Veículo" SortExpression="TipoVeiculo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoVeiculo" runat="server" SelectedValue='<%# Bind("TipoVeiculo") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Tração" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Reboque" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoVeiculo" ControlToValidate="ddlTipoVeiculo" runat="server"
                                    ErrorMessage="Campo Tipo Veículo deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoVeiculo" runat="server" SelectedValue='<%# Bind("TipoVeiculo") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Tração" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Reboque" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoVeiculo" ControlToValidate="ddlTipoVeiculo" runat="server"
                                    ErrorMessage="Campo Tipo Veículo deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("TipoVeiculo") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Rodado" SortExpression="TipoRodado">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoRodado" runat="server" Width="150" SelectedValue='<%# Bind("TipoRodado") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Truck" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Toco" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Cavalo Mecânico" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="VAN" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Utilitário" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="Outros" Value="6"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoRodado" ControlToValidate="ddlTipoRodado" runat="server"
                                    ErrorMessage="Campo Tipo Rodado deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoRodado" runat="server" Width="150" SelectedValue='<%# Bind("TipoRodado") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Truck" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Toco" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Cavalo Mecânico" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="VAN" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Utilitário" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="Outros" Value="6"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoRodado" ControlToValidate="ddlTipoRodado" runat="server"
                                    ErrorMessage="Campo Tipo Rodado deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("TipoRodado") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Carroceria" SortExpression="TipoCarroceria">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTipoCarroceria" runat="server" Width="150" SelectedValue='<%# Bind("TipoCarroceria") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Não Aplicável" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Aberta" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Fechada/Baú" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Granelera" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Porta Container" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Sider" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoCarroceria" ControlToValidate="ddlTipoCarroceria"
                                    runat="server" ErrorMessage="Campo Tipo Carroceria deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlTipoCarroceria" runat="server" Width="150" SelectedValue='<%# Bind("TipoCarroceria") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="Não Aplicável" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Aberta" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Fechada/Baú" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Granelera" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Porta Container" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Sider" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpTipoCarroceria" ControlToValidate="ddlTipoCarroceria"
                                    runat="server" ErrorMessage="Campo Tipo Carroceria deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("TipoCarroceria") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="UF Licenciado" SortExpression="UfLicenc">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlUfLicenc" runat="server" SelectedValue='<%# Bind("UfLicenc") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="AC" Value="AC" />
                                    <asp:ListItem Text="AL" Value="AL" />
                                    <asp:ListItem Text="AM" Value="AM" />
                                    <asp:ListItem Text="AP" Value="AP" />
                                    <asp:ListItem Text="BA" Value="BA" />
                                    <asp:ListItem Text="CE" Value="CE" />
                                    <asp:ListItem Text="DF" Value="DF" />
                                    <asp:ListItem Text="ES" Value="ES" />
                                    <asp:ListItem Text="GO" Value="GO" />
                                    <asp:ListItem Text="MA" Value="MA" />
                                    <asp:ListItem Text="MG" Value="MG" />
                                    <asp:ListItem Text="MS" Value="MS" />
                                    <asp:ListItem Text="MT" Value="MT" />
                                    <asp:ListItem Text="PA" Value="PA" />
                                    <asp:ListItem Text="PB" Value="PB" />
                                    <asp:ListItem Text="PE" Value="PE" />
                                    <asp:ListItem Text="PI" Value="PI" />
                                    <asp:ListItem Text="PR" Value="PR" />
                                    <asp:ListItem Text="RJ" Value="RJ" />
                                    <asp:ListItem Text="RN" Value="RN" />
                                    <asp:ListItem Text="RO" Value="RO" />
                                    <asp:ListItem Text="RR" Value="RR" />
                                    <asp:ListItem Text="RS" Value="RS" />
                                    <asp:ListItem Text="SC" Value="SC" />
                                    <asp:ListItem Text="SE" Value="SE" />
                                    <asp:ListItem Text="SP" Value="SP" />
                                    <asp:ListItem Text="TO" Value="TO" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpUfLicenc" ControlToValidate="ddlUfLicenc" runat="server"
                                    ErrorMessage="Campo UF Licenciado deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlUfLicenc" runat="server" SelectedValue='<%# Bind("UfLicenc") %>'>
                                    <asp:ListItem Text="Selecione" Value="selecione"></asp:ListItem>
                                    <asp:ListItem Text="AC" Value="AC" />
                                    <asp:ListItem Text="AL" Value="AL" />
                                    <asp:ListItem Text="AM" Value="AM" />
                                    <asp:ListItem Text="AP" Value="AP" />
                                    <asp:ListItem Text="BA" Value="BA" />
                                    <asp:ListItem Text="CE" Value="CE" />
                                    <asp:ListItem Text="DF" Value="DF" />
                                    <asp:ListItem Text="ES" Value="ES" />
                                    <asp:ListItem Text="GO" Value="GO" />
                                    <asp:ListItem Text="MA" Value="MA" />
                                    <asp:ListItem Text="MG" Value="MG" />
                                    <asp:ListItem Text="MS" Value="MS" />
                                    <asp:ListItem Text="MT" Value="MT" />
                                    <asp:ListItem Text="PA" Value="PA" />
                                    <asp:ListItem Text="PB" Value="PB" />
                                    <asp:ListItem Text="PE" Value="PE" />
                                    <asp:ListItem Text="PI" Value="PI" />
                                    <asp:ListItem Text="PR" Value="PR" />
                                    <asp:ListItem Text="RJ" Value="RJ" />
                                    <asp:ListItem Text="RN" Value="RN" />
                                    <asp:ListItem Text="RO" Value="RO" />
                                    <asp:ListItem Text="RR" Value="RR" />
                                    <asp:ListItem Text="RS" Value="RS" />
                                    <asp:ListItem Text="SC" Value="SC" />
                                    <asp:ListItem Text="SE" Value="SE" />
                                    <asp:ListItem Text="SP" Value="SP" />
                                    <asp:ListItem Text="TO" Value="TO" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="cvdrpUfLicenc" ControlToValidate="ddlUfLicenc" runat="server"
                                    ErrorMessage="Campo UF Licenciado deve ser selecionado." ValueToCompare="selecione"
                                    Operator="NotEqual">*</asp:CompareValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("UfLicenc") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Transportador">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                    SelectedValue='<%# Bind("IdTransportador") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                    SelectedValue='<%# Bind("IdTransportador") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="Button2" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="Placa"
                                    Text='<%# Bind("Placa") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="Button1" runat="server" CommandName="Insert" Text="Inserir" />
                                <asp:Button ID="Button2" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" />
                            </InsertItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Wrap="True" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                </asp:DetailsView>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                    ShowSummary="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVeiculos" runat="server" 
                    SelectMethod="ObtemVeiculo"
                    TypeName="Glass.Global.Negocios.IVeiculoFluxo" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Veiculo"
                    InsertMethod="SalvarVeiculo" 
                    UpdateMethod="SalvarVeiculo" 
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="placa" QueryStringField="placa" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
                    SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
