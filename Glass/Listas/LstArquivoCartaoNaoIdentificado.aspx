<%@ Page Title="Arquivos de Cartões não identificados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstArquivoCartaoNaoIdentificado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoCartaoNaoIdentificado" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlCancLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server"> 

    <script type="text/javascript">

        function openMotivoCanc(idArquivoCartaoNaoIdentificado) {
            openWindow(150, 450, "../Utils/SetMotivoCancArquivoCartaoNaoIdentificado.aspx?idCartaoNaoIdentificado=" + idArquivoCartaoNaoIdentificado);
            return false;
        }

        function openRpt(exportarExcel) {            
            var situacao = FindControl("drpSituacao", "select").value;
            var dataCadIni = FindControl("ctrlData1_txtData", "input").value;
            var dataCadFim = FindControl("ctrlData2_txtData", "input").value;
            var nomeFunc = FindControl("txtNomeFunc", "input").value;                                  

            var query = "../Relatorios/RelBase.aspx?Rel=ListaArquivoCartaoNaoIdentificado"                       
                        + "&situacao=" + situacao                        
                        + "&dataCadIni=" + dataCadIni
                        + "&dataCadFim=" + dataCadFim                       
                        + "&nomeFunc" + nomeFunc
                        + "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, query);

            return false;
        }        

    </script>

    <table>
        <tr>
            <td align="center">               
                <table>
                    <tr>                 
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                                <asp:ListItem Value="1" Text="Ativo" />
                                <asp:ListItem Value="2" Text="Cancelado" />
                            </asp:DropDownList>
                        </td>   
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td> 
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Período Importação"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlData1" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlData2" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>  
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Nome Func. Cad."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFunc" runat="server" Width="160px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>                   
                    </tr>
                </table>              
            </td>
        </tr>      
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCartaoNaoIdentificado" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdArquivoCartaoNaoIdentificado"
                    DataSourceID="odsArquivoCartaoNaoIdentificado" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum cartão não identificado encontrado.">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>                               
                                <asp:ImageButton ID="imbCancelar" runat="server" ToolTip="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "return openMotivoCanc(" + Eval("IdArquivoCartaoNaoIdentificado") + ");" %>'
                                    Visible='<%# Eval("PodeCancelar") %>' />
                                 <asp:ImageButton ID="imbDownLoad" runat="server" ImageUrl="~/Images/disk.gif" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/Download.ashx?filePath=~/Upload/ArquivosCNI/" + Eval("NomeArquivo") + "&fileName=" + Eval("NomeArquivo") + "\"); return false" %>'                                                                           
                                    ToolTip="Download do Arquivo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdArquivoCartaoNaoIdentificado" HeaderText="Cód. Arquivo" SortExpression="IdArquivoCartaoNaoIdentificado" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data de Importação" SortExpression="DataCad" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" />
                        <asp:BoundField DataField="NomeFuncCad" HeaderText="Func. Cad" SortExpression="Funccad"  DataFormatString="{0:C}" />   
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlCancLogPopup ID="ctrlLogCancPopup1" runat="server" Tabela="ArquivoCartaoNaoIdentificado" IdRegistro='<%# Convert.ToUInt32(Eval("IdArquivoCartaoNaoIdentificado")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>                                                              
                    </Columns>
                        
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0"
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton><br />
                <br />
                <asp:Label ID="Label9" runat="server" ForeColor="#FF0000" Text="Arquivos de Cartões não identificados que possuem itens utilizados não podem ser cancelados."></asp:Label>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArquivoCartaoNaoIdentificado" runat="server" EnablePaging="True"
                    SelectMethod="PesquisarArquivosCartaoNaoIdentificado"
                    TypeName="Glass.Financeiro.Negocios.IArquivoCartaoNaoIdentificadoFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlData1" Name="dataImportIni" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlData2" Name="dataImportFim" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="txtNomeFunc" Name="funcCad" PropertyName="Text" />                      
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
