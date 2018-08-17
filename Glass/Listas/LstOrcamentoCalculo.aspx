<%@ Page Title="Orçamentos Cálculo" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstOrcamentoCalculo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstOrcamentoCalculo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(idOrca) {
            openWindow(600, 800, "../Relatorios/RelOrcamento.aspx?idOrca=" + idOrca);
            return false;
        }

        function getMedidor(idMedidor) {
            if (idMedidor.value == "") {
                openWindow(500, 700, "../Utils/SelMedidor.aspx");
                return false;
            }

            var retorno = MetodosAjax.GetMedidor(idMedidor.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idMedidor.value = "";
                FindControl("txtNomeMedidor", "input").value = "";
                return false;
            }

            FindControl("txtNomeMedidor", "input").value = retorno[1];
        }

        // Função utilizada após selecionar medidor no popup, para preencher o id e o nome do mesmo
        // Nas respectivas textboxes deste form
        function setMedidor(id, nome) {
            FindControl("txtNumMedidor", "input").value = id;
            FindControl("txtNomeMedidor", "input").value = nome;
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Num. Medição"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumMedicao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesqMedidor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período Finalizada"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Medidor"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumMedidor" runat="server" Width="50px" onblur="getMedidor(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeMedidor" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getMedidor(FindControl('txtNumMedidor', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCli" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdOrcamento" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdOrcamento"
                    DataSourceID="odsOrcamento" EmptyDataText="Nenhum orçamento encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" Visible='<%# Eval("EditVisible") %>'
                                    NavigateUrl='<%# "../Cadastros/CadOrcamento.aspx?idorca=" + Eval("Idorcamento") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                &nbsp; <a href="#" onclick="openRpt('<%# Eval("Idorcamento") %>');">
                                    <img border="0" src="../Images/Relatorio.gif" title="Relatório do Orçamento" /></a>
                                <asp:PlaceHolder ID="pchFotos" runat="server" Visible='<%# Eval("IdsMedicao") != null && !string.IsNullOrWhiteSpace(Eval("IdsMedicao").ToString()) %>'>
                                    <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?tipo=medicao&id=<%# Eval("IdsMedicao") %>&#039;); return false;'>
                                        <img border="0px" src="../Images/Fotos.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Orçamento" SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="IdsMedicao" HeaderText="Medição" />
                        <asp:BoundField DataField="NomeMedidor" HeaderText="Medidor" SortExpression="NomeMedidor" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFuncAbrv" HeaderText="Funcionário" SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="TelCliente" HeaderText="Tel. Res." SortExpression="TelCliente" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" HeaderText="Data Orçamento" SortExpression="DataCad"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Sit. Orçamento" SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.Orcamento"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetOrcamentoCalculoCount" SelectMethod="GetOrcamentoCalculo"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.OrcamentoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumMedicao" Name="idMedicao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumMedidor" Name="idMedidor" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeMedidor" Name="nomeMedidor" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataFinIni" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFinFim" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNomeCli" Name="nomeCli" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
