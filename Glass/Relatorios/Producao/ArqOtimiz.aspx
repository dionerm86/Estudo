<%@ Page Title="Arquivos de Otimização" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ArqOtimiz.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ArqOtimiz" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function formatarData(data)
        {
            while (data.indexOf("/") > -1)
                data = data.replace("/", "-");

            while (data.indexOf(":") > -1)
                data = data.replace(":", "_");

            return data;
        }
        
        function download(funcionario, data, arquivo, extensao)
        {
            var nomeArquivo = funcionario + " " + formatarData(data) + extensao;
            redirectUrl("../../Handlers/Download.ashx?filePath=" + arquivo +
                "&fileName=" + nomeArquivo);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Data" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ExibirHoras="True" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            a
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ExibirHoras="True" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Direção" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDirecao" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Exportação</asp:ListItem>
                                <asp:ListItem Value="2">Importação</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="50px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Etiqueta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdArqOtimiz" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdArquivoOtimizacao"
                    DataSourceID="odsArqOtimiz" EmptyDataText="Ainda não há arquivos de otimização gerados."
                    GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "download(\"" + Eval("NomeFunc") + "\", \"" + Eval("DataCad") + "\", \"" + Eval("CaminhoArquivo") + "\", \"" + Eval("ExtensaoArquivo") + "\"); return false" %>'
                                    ToolTip="Download do arquivo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrDirecao" HeaderText="Direção" SortExpression="Direcao" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArqOtimiz" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ArquivoOtimizacaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ddlDirecao" Name="direcao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEtiqueta" Name="numEtiqueta" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetFuncionarios"
                    TypeName="Glass.Data.DAL.ArquivoOtimizacaoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
