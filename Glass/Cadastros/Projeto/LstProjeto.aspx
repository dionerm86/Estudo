<%@ Page Title="Projetos Efetuados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstProjeto" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">

function openRpt(idPedido)
{
    openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido);
    return false;
}

function openMotivoCanc(idProjeto) {
    openWindow(150, 400, "../Utils/SetMotivoCancProj.aspx?idProjeto=" + idProjeto);
    return false;
}

function confirmDelete(numItens)
{
    if (!confirm("Tem certeza que deseja excluir este projeto?"))
        return false;
        
    if (numItens >= 10)
        if (!confirm("ESTE PROJETO TEM MAIS DE 10 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?"))
            return false;
            
    if (numItens >= 20)
        if (!confirm("ESTE PROJETO TEM MAIS DE 20 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?"))
            return false;
            
    if (numItens >= 30)
        if (!confirm("ESTE PROJETO TEM MAIS DE 30 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?"))
            return false;
    }

    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }

        FindControl("txtNome", "input").value = retorno[1];
    }

</script>
<table align="center" style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Projeto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumProjeto" runat="server" 
                                onkeypress="return soNumeros(event, true, true);" Width="60px"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" 
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkEfetuarProjeto" runat="server" 
                    onclick="lnkEfetuarProjeto_Click">Efetuar Projeto</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProjeto" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProjeto" DataKeyNames="IdProjeto" 
                    EmptyDataText="Nenhum projeto encontrado." 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" 
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>                                
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar"                                    
                                    NavigateUrl='<%# "CadProjeto.aspx?idProjeto=" + Eval("IdProjeto") + "&Parceiro=false" %>' 
                                    Visible='<%# ((bool)Eval("EditVisible")) %>'><img border="0" src="../../Images/EditarGrid.gif" /></asp:HyperLink>
                                                                    
                                <asp:LinkButton ID="lnkExcluir" CommandName="Delete" runat="server" Visible='<%# Eval("DeleteVisible") %>'
                                    OnClientClick='<%# "return confirmDelete(" + Eval("NumeroItensProjeto") + ")" %>'>
                                    <img border="0" src="../../Images/ExcluirGrid.gif" /></asp:LinkButton>  
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdProjeto" HeaderText="Num" 
                            SortExpression="IdProjeto" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" 
                            SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" 
                            SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" 
                            SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" 
                            DataFormatString="{0:C}" >
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" 
                            SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" 
                            SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProjeto" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.Projeto" DeleteMethod="Delete" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProjetoDAO" 
                    ondeleted="odsProjeto_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumProjeto" Name="idProjeto" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" 
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" 
                            PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" 
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>


