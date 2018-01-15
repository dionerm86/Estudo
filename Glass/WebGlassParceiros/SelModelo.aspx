<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelModelo.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.SelModelo"
    Title="Selecione o Modelo do Projeto" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
    var click = false;
    
    // Função chamada por qualquer modelo, quando o mesmo for selecionado
    function selModelo(lnkSelecionar) {
        if (click)
            return false;
            
        click = true;
        
        // Pega o id do controle que chamou esta função
        var ctrlSourceId = lnkSelecionar.id.toString().split('_')[2];
        
        var idProjetoModelo = FindControl(ctrlSourceId + "_hdfIdProjetoModelo", "input").value;
        var espessuraVidro = FindControl(ctrlSourceId + "_drpEspessuraVidro", "select");
        var idCorVidro = FindControl(ctrlSourceId + "_drpCorVidro", "select").value;
        var idCorAluminio = FindControl(ctrlSourceId + "_drpCorAluminio", "select") != null ? FindControl(ctrlSourceId + "_drpCorAluminio", "select").value : "";
        var idCorFerragem = FindControl(ctrlSourceId + "_drpCorFerragem", "select") != null ? FindControl(ctrlSourceId + "_drpCorFerragem", "select").value : "";
        var medidaExata = FindControl(ctrlSourceId + "_chkMedidaExata", "input").checked;
        var apenasVidros = FindControl(ctrlSourceId + "_chkApenasVidro", "input").checked;
        
        if (espessuraVidro != undefined && espessuraVidro != null) {
            if (!apenasVidros) {
                espessuraVidro.value = "";
            }
            else if (espessuraVidro.value == "") {
                alert("Informe a espessura do vidro.");
                click = false;
                return false;
            }
        }

        if (idCorVidro == "") {
            alert("Informe a cor do vidro.");
            click = false;
            return false;
        }
        
        window.opener.setModelo(idProjetoModelo, espessuraVidro == undefined || espessuraVidro == null ? "0" : espessuraVidro.value,
            idCorVidro, idCorAluminio, idCorFerragem, apenasVidros, medidaExata);
        closeWindow();
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Grupo" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" DataSourceID="odsGrupoModelo"
                                DataTextField="Descricao" DataValueField="IdGrupoModelo">
                            </asp:DropDownList>
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
            <td align="center" class="subtitle1">
                <asp:Label ID="lblMaisUsados" runat="server" Text="Projetos mais utilizados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Table ID="tbModelos" runat="server">
                </asp:Table>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.GrupoModeloDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorAluminio" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorAluminioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorFerragem" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorFerragemDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
