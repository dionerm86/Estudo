<%@ Page Title="Boletos Antecipados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstQuitaContaRecAntecip.aspx.cs" Inherits="Glass.UI.Web.Listas.LstQuitaContaRecAntecip" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
        function openRpt(exportarExcel) {
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var dtIniAntecip = FindControl("ctrlDataIniAntecip_txtData", "input").value;
            var dtFimAntecip = FindControl("ctrlDataFimAntecip_txtData", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;
            var sitAntecip = FindControl("drpSituacao", "select").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            //var numeroNFe = FindControl("txtNFe", "input").value;
            var agrupar = FindControl("chkAgrupar", "input").checked;

            var queryString = idCli == "" ? "&idCli=0" : "&idCli=" + idCli;
            //queryString += numeroNFe == "" ? "&numeroNFe=0" : "&numeroNFe=" + numeroNFe;
            queryString += valorInicial == "" ? "&valorInicial=0" : "&valorInicial=" + valorInicial;
            queryString += valorFinal == "" ? "&valorFinal=0" : "&valorFinal=" + valorInicial;
            queryString += sitAntecip == "" ? "&sitAntecip=0" : "&sitAntecip=" + sitAntecip;
            queryString += idContaBanco == "" ? "&idContaBanco=0" : "&idContaBanco=" + idContaBanco;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ContaRecAntecip&nomeCli=" + nomeCli + "&dataIniAntecip=" + dtIniAntecip +
                "&dataFimAntecip=" + dtFimAntecip + queryString + "&exportarExcel=" + exportarExcel + "&agrupar=" + agrupar);

            return false;
        }
        
        var clicked = false;
        function quitarBoleto(control) {
        if (clicked)
            return false;
    
        var conf = confirm("Tem certeza que deseja quitar este boleto?");

        clicked = conf;

        return conf;
    }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label24" runat="server" Text="Período Antecip." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIniAntecip" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFimAntecip" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label29" runat="server" Text="Valor Boleto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            até
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtValorFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                CausesValidation="False" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label28" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Selected="True" Value="1">Antecipado</asp:ListItem>
                                <asp:ListItem Value="2">Quitado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblPeriodo0" runat="server" Text="Conta Bancária" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                CausesValidation="False" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgrupar" runat="server" Text="Agrupar relatório por cliente" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" DataKeyNames="IdContaR"
                    DataSourceID="odsContasReceber" EmptyDataText="Nenhum boleto antecipado encontrado."
                    OnRowCommand="grdConta_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtDataQuitar" runat="server" Width="70px" OnLoad="txtDataQuitar_Load"
                                    onkeypress="return mascara_data(event, this), soNumeros(event, true, true);"
                                    MaxLength="10" Visible='<%# !(bool)Eval("Recebida") %>' ValidationGroup="c"></asp:TextBox>
                                <asp:ImageButton ID="imgDataQuitar" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    ToolTip="Alterar" OnLoad="imgDataQuitar_Load" Style="padding-right: 4px" Visible='<%# !(bool)Eval("Recebida") %>' />
                                <asp:CustomValidator ID="ctvDataQuitar" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
                                    ControlToValidate="txtDataQuitar" Display="Dynamic" ValidateEmptyText="true" ValidationGroup="c"
                                    Visible='<%# !(bool)Eval("Recebida") %>'></asp:CustomValidator>
                                <asp:Button ID="btnQuitar" runat="server" CommandName="Quitar" OnClientClick="return quitarBoleto(this);"
                                    Text="Quitar" OnDataBinding="btnQuitar_DataBinding" Visible='<%# !(bool)Eval("Recebida") %>' ValidationGroup="c"/>
                                <asp:LinkButton ID="lnkCancelar" runat="server" CommandArgument='<%# Eval("IdContaR") %>'
                                    CommandName="Cancelar" Visible='<%# (bool)Eval("Recebida") %>' OnClientClick="return confirm('Tem certeza que deseja cancelar a quitação deste boleto?');">
                                    <img src="../Images/ExcluirGrid.gif" border="0" /></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="concat(coalesce(c.IdAcerto,0), coalesce(c.IdAcertoParcial,0), coalesce(c.IdAntecipContaRec,0), coalesce(c.IdDevolucaoPagto,0), coalesce(c.IdLiberarPedido,0), coalesce(c.IdObra,0), coalesce(c.IdPedido,0), coalesce(c.IdTrocaDevolucao,0))">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação Pedido" SortExpression="IdLiberarPedido">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("IdLiberarPedido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("IdLiberarPedido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acerto" SortExpression="IdAcertoParcial">
                            <EditItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("IdAcertoParcial") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("IdAcertoParcial") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParc">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCli">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label18" runat="server" Text='<%# Bind("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DataVencPrimNeg") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Bind("DataVencPrimNeg") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Antecip." SortExpression="DataAntecip">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataAntecip") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DataAntecip", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacaoAntecip">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrSituacaoAntecip") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrSituacaoAntecip") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. NF" SortExpression="NumeroNFe">
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label23" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Bind("IdContaR") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"
                    CausesValidation="False"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasReceber" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetAntecipCount" SelectMethod="GetAntecip" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasReceberDAO"
                    DataObjectTypeName="Glass.Data.Model.ContasReceber" UpdateMethod="AtualizaObsDataVec">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniAntecip" Name="dtIniAntecip" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimAntecip" Name="dtFimAntecip" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="sitAntecip" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
