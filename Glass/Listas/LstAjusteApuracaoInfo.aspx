<%@ Page Title="Informações Adicionais dos Ajustes da Apuração do ICMS" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAjusteApuracaoInfo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteApuracaoInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../Scripts/jquery/jquery-1.9.0.js" type="text/javascript"></script>
    <script src="../Scripts/jquery/jquery-1.9.0.min.js" type="text/javascript"></script>
    
    <script type="text/javascript">
        $(document).ready(function() {
            $(".insert").click(function() {

                var $NumDa = $("input:text[id$=txtNumDa]").val();
                var $NumProc = $("input:text[id$=txtNumProc]").val();
                var $IndProc = $("select[id$=drpIndProc]").val();
                var $Proc = $("textarea[id$=txtProc]").val();
                var $TxtCompl = $("textarea[id$=txtCompl]").val();
                var $IdABIA = GetQueryString('id');
                var $imposto = GetQueryString('imposto');

                var postData = { "postData":
                    {  "NumDa": $NumDa, "NumProc": $NumProc, "IndProc": $IndProc, "Proc": $Proc, "TxtCompl": $TxtCompl, "IdABIA": $IdABIA, "TipoImposto":$imposto }
                };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/InserirAjusteApuracaoInfo",
                    data: JSON.stringify(postData),
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success:
                        function(result) {
                            document.location.reload(true);
                        },
                    error:
                        function(result) {
                            alert("Ocorreu um erro: " + "\r\n" + jQuery.parseJSON(result.responseText).Message);
                        }
                });
            });
        });
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdAjuste" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsAjuste" GridLines="None"
                    EmptyDataText="Nenhum registro encontrado" ShowFooter="True" 
                    DataKeyNames="Id,IdABIA,TipoImposto"
                    ondatabound="grdAjuste_DataBound" onrowcommand="grdAjuste_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False" CommandName="Edit"
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Doc. Arrecadação" SortExpression="NumDa">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumDa") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumDa" runat="server" Text='<%# Bind("NumDa") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumDa" runat="server"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Processo" SortExpression="NumProc">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("NumProc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumProc" runat="server" Text='<%# Bind("NumProc") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumProc" runat="server"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Indicador de Origem" SortExpression="DescricaoIndProc">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescricaoIndProc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpIndProc" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsIndProc" DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IndProc") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpIndProc" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsIndProc" DataTextField="Descr" DataValueField="Id">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desc. Processo" SortExpression="Proc">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Proc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtProc" runat="server" Text='<%# Bind("Proc") %>' TextMode="MultiLine" Width="227px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtProc" runat="server" TextMode="MultiLine" Width="227px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desc. Complementar" SortExpression="TxtCompl">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("TxtCompl") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCompl" runat="server" Text='<%# Bind("TxtCompl") %>' 
                                    TextMode="MultiLine" Width="227px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCompl" runat="server" TextMode="MultiLine" Width="227px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <FooterTemplate>
                                <a href="#" class="insert" style="text-decoration:none"><img style="text-decoration:none" src="../Images/ok.gif" /></a>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjuste" runat="server" DataObjectTypeName="Glass.Data.Model.AjusteApuracaoInfoAdicional"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.AjusteApuracaoInfoAdicionalDAO" EnablePaging="True"
                    UpdateMethod="Update" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idABIA" QueryStringField="id" Type="UInt32" />
                        <asp:QueryStringParameter Name="tipoImposto" QueryStringField="imposto" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndProc" runat="server" SelectMethod="GetIndProc" TypeName="Glass.Data.EFD.DataSourcesEFD"
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
        <td align="center">
        <asp:LinkButton Text="Voltar" runat="server" ID="lkbVoltar" 
                onclick="lkbVoltar_Click"></asp:LinkButton>
        </td>
        </tr>
    </table>
</asp:Content>
