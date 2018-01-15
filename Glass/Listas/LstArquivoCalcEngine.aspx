<%@ Page Title="Arquivo CalcEngine" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstArquivoCalcEngine.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoCalcEngine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    
    <script type="text/javascript">

    function validar(Nome)
    {
        openWindow(600, 800, '<%= this.ResolveClientUrl("../Utils/ValidadorCalcEngine.aspx") %>?Nome=' + Nome);
    }

    </script>


    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadArquivoCalcEngine.aspx">Inserir arquivo CalcEngine</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdArquivoCalcEngine" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsArquivoCalcEngine" DataKeyNames="IdArquivoCalcEngine" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" 
                    EmptyDataText="Não há arquivos cadastrados.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server"
                                    ImageUrl="~/Images/EditarGrid.gif" NavigateUrl='<%# "~/Cadastros/CadArquivoCalcEngine.aspx?IdArquivoCalcEngine=" + Eval("IdArquivoCalcEngine") %>'></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Deletar Arquivo" CommandName="Delete"
                                    OnClientClick="if (!confirm('Deseja excluir este arquivo?')) return false;" />
                                <asp:ImageButton ID="imbValidar" runat="server" ImageUrl="~/Images/validacao.gif" ToolTip="Validar" OnClientClick='<%# string.Format("validar(\"{0}\");", Eval("Nome"))  %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="validar(0)">Validar Todos os Arquivos</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsArquivoCalcEngine" runat="server" DataObjectTypeName="Glass.Data.Model.ArquivoCalcEngine"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetListCalcEngine" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ArquivoCalcEngineDAO" SelectCountMethod="GetListCountCalcEngine" OnDeleted="odsArquivoCalcEngine_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
