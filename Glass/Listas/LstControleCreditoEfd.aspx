<%@ Page Title="Controle de Créditos EFD" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstControleCreditoEfd.aspx.cs" Inherits="Glass.UI.Web.Listas.LstControleCreditoEfd" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">
        function validaPeriodo(val, args) {
            var regex = /[0-9]{2}\/[0-9]{4}/;
            args.IsValid = regex.test(args.Value);
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período Geração"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <table class="pos" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="1%">
                                        <asp:DropDownList ID="drpMes" runat="server" DataSourceID="odsMes"
                                            DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>&nbsp;
                                        <asp:TextBox ID="txtAno" runat="server" MaxLength="4" Columns="4"
                                            onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Tipo de Imposto"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoImposto" runat="server"
                                AppendDataBoundItems="True" DataSourceID="odsTipoImposto" DataTextField="Descr"
                                DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Código do Crédito"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
                                DataValueField="Id" FazerPostBackBotaoPesquisar="True"
                                TextWidth="200px" TituloTela="Selecione o Tipo de Crédito" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasCreditoPositivo" runat="server"
                                Text="Apenas itens com crédito" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdControleCreditos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdCredito" DataSourceID="odsControleCreditos"
                    GridLines="None" EmptyDataText="Não há créditos para esses critérios."
                    ShowFooter="True" OnRowCommand="grdControleCreditos_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update"
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False"
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False"
                                    CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False"
                                    CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse crédito do EFD?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Período Geração"
                            SortExpression="str_to_date(concat('01/', PeriodoGeracao), '%d/%m/%Y')">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPeriodo" runat="server" Columns="7" MaxLength="7"
                                    Text='<%# Bind("PeriodoGeracao") %>' onkeypress="return soData(event)"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revPeriodo" runat="server"
                                    ControlToValidate="txtPeriodo" Display="Dynamic" ErrorMessage="*"
                                    ValidationExpression="[0-9]{2}\/[0-9]{4}"></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="rfvPeriodo" runat="server"
                                    ControlToValidate="txtPeriodo" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPeriodo" runat="server" Columns="7" MaxLength="7"
                                    onkeypress="return soData(event)"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revPeriodo" runat="server"
                                    ControlToValidate="txtPeriodo" Display="Dynamic" ErrorMessage="*"
                                    ValidationExpression="[0-9]{2}\/[0-9]{4}"></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="rfvPeriodo" runat="server"
                                    ControlToValidate="txtPeriodo" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("PeriodoGeracao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <EditItemTemplate>
                                <uc3:ctrlLoja runat="server" ID="drpLoja" SelectedValue='<%# Bind("IdLoja") %>' MostrarTodas="false"/>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc3:ctrlLoja runat="server" ID="drpLoja" SelectedValue='<%# Bind("IdLoja") %>' MostrarTodas="false"/>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label111" runat="server" Text='<%# Bind("DescrLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Imposto" SortExpression="TipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server"
                                    DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id"
                                    SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server"
                                    AppendDataBoundItems="True" DataSourceID="odsTipoImposto" DataTextField="Descr"
                                    DataValueField="Id" SelectedValue='<%# Bind("TipoImposto") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipoImposto" runat="server"
                                    ControlToValidate="drpTipoImposto" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrTipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Gerado" SortExpression="ValorGerado">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorGerado" runat="server"
                                    Text='<%# Bind("ValorGerado") %>' onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValor" runat="server"
                                    ControlToValidate="txtValorGerado" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server"
                                    Text='<%# Bind("ValorGerado", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorGerado" runat="server"
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValor" runat="server"
                                    ControlToValidate="txtValorGerado" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código do Crédito" SortExpression="CodCred">
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred"
                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="True"
                                    TextWidth="200px" TituloTela="Selecione o Tipo de Crédito"
                                    Valor='<%# Bind("CodCred") %>' Descricao='<%# Eval("DescrCodCred") %>'
                                    PermitirVazio="True" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrCodCred") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred"
                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="True"
                                    TextWidth="200px" TituloTela="Selecione o Tipo de Crédito"
                                    PermitirVazio="True" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Usado Crédito"
                            SortExpression="ValorUsadoCredito">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server"
                                    Text='<%# Eval("ValorUsadoCredito", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server"
                                    Text='<%# Bind("ValorUsadoCredito", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Restante Crédito"
                            SortExpression="ValorRestanteCredito">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server"
                                    Text='<%# Eval("ValorRestanteCredito", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server"
                                    Text='<%# Bind("ValorRestanteCredito", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClick="imgAdd_Click" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server"
                                    IdRegistro='<%# Eval("IdCredito") %>' Tabela="ControleCreditosEfd" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsControleCreditos" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ControleCreditoEfdDAO"
                    DataObjectTypeName="Glass.Data.Model.ControleCreditoEfd" DeleteMethod="Delete"
                    UpdateMethod="Update" OnDeleted="odsControleCreditos_Deleted"
                    OnUpdated="odsControleCreditos_Updated">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfPeriodo" Name="inicio" PropertyName="Value"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="drpTipoImposto" Name="tipoImposto"
                            PropertyName="SelectedValue" Type="Object" />
                        <asp:ControlParameter ControlID="selCodCred" Name="codCred"
                            PropertyName="Valor" Type="Object" />
                        <asp:ControlParameter ControlID="chkApenasCreditoPositivo" Name="apenasCreditoPositivo"
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoImposto" runat="server"
                    SelectMethod="GetTipoImposto" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMes" runat="server" SelectMethod="GetMeses"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfPeriodo" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

