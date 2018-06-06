<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlModeloProjeto.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlModeloProjeto" %>


<script type="text/javascript">

   

</script>

<table cellspacing="0">

    <tr>
        <td align="center">
            <asp:Image ID="imgImagem" runat="server" ToolTip="Clique para exibir essa imagem em tamanho real" />
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Label ID="lblDescricao" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td align="center">
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" Text="Vidro"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblEspessura" runat="server" Text="Espessura"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAluminio" runat="server" Text="Aluminio"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFerragem" runat="server" Text="Ferragem"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="drpCorVidro" runat="server"
                                                    DataTextField="Descricao" DataValueField="IdCorVidro" AppendDataBoundItems="True">
                                                    <asp:ListItem Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpEspessuraVidro" runat="server">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem Text="03MM" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="04MM" Value="4"></asp:ListItem>
                                                    <asp:ListItem Text="05MM" Value="5"></asp:ListItem>
                                                    <asp:ListItem Text="06MM" Value="6"></asp:ListItem>
                                                    <asp:ListItem Text="08MM" Value="8"></asp:ListItem>
                                                    <asp:ListItem Text="10MM" Value="10"></asp:ListItem>
                                                    <asp:ListItem Text="12MM" Value="12"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpCorAluminio" runat="server"  
                                                    DataTextField="Sigla" DataValueField="IdCorAluminio" AppendDataBoundItems="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpCorFerragem" runat="server" 
                                                    DataTextField="Sigla" DataValueField="IdCorFerragem" AppendDataBoundItems="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                            <asp:CheckBox ID="chkApenasVidro" runat="server" Text="Apenas Vidros" />
                            <span id="spnMedidaExata" runat="server" style="display: none" >
                            <asp:CheckBox ID="chkMedidaExata" runat="server" Text="Medidas Exatas" />
                        </span>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:LinkButton ID="lnkCodigo" runat="server" Font-Bold="True" Font-Size="Medium" 
                ForeColor="Blue" OnClientClick="selModelo(this); return false;"></asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td align="center">
            <br />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdProjetoModelo" runat="server" />
