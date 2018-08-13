<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ConsultaEnvio.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ConsultaEnvio" Title="Consultar Envio de " %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr runat="server" id="email" visible="false">
            <td align="center">
                <table>
                    <tr>
                        <td class="subtitle1">
                            E-mail
                        </td>
                    </tr>
                    <tr>
                        <td align="center"> 
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label12" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                            DataTextField="NomeFantasia" DataValueField="IdLoja">
                                            <asp:ListItem Value="0">TODAS</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label1" runat="server" Text="Destinatário" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap" style="width: 200px">
                                        <asp:TextBox ID="txtDestinatario" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label3" runat="server" Text="Assunto" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:TextBox ID="txtAssunto" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label23" runat="server" Text="Data Cad." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Data Envio" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEnvioIni" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEnvioFim" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="grdFilaEmail" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsFilaEmail"
                                GridLines="None" EmptyDataText="Ainda não foi enviado e-mail.">
                                <Columns>
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:BoundField DataField="EmailDestinatario" HeaderText="Destinatário" SortExpression="EmailDestinatario" />
                                    <asp:BoundField DataField="Assunto" HeaderText="Assunto" SortExpression="Assunto" />
                                    <asp:BoundField DataField="Mensagem" HeaderText="Mensagem" SortExpression="Mensagem" />
                                    <asp:BoundField DataField="DataCad" HeaderText="Data Cad." SortExpression="DataCad" />
                                    <asp:BoundField DataField="DataEnvio" HeaderText="Data Envio" SortExpression="DataEnvio" />
                                    <asp:BoundField DataField="NumeroTentativas" HeaderText="Tentativas com erro" SortExpression="NumeroTentativas" />
                                </Columns>
                                <PagerStyle CssClass="pgr" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFilaEmail" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.FilaEmailDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" />
                                    <asp:ControlParameter ControlID="ctrlDataEnvioIni" Name="dataEnvioIni" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataEnvioFim" Name="dataEnvioFim" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="txtAssunto" Name="assunto" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtDestinatario" Name="destinatario" PropertyName="Text" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr runat="server" id="separador" visible="false">
            <td>
                &nbsp; 
            </td>
        </tr>
        <tr runat="server" id="sms" visible="false">
            <td align="center">
                <table>
                    <tr>
                        <td class="subtitle1">
                            SMS
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label4" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:TextBox ID="txtNomeLoja" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label5" runat="server" Text="Destinatário" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap" style="width: 200px">
                                        <asp:TextBox ID="txtDestinatarioSms" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="Data Cad." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataCadIniSms" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataCadFimSms" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="Data Envio" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEnvioIniSms" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEnvioFimSms" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="grdFilaSms" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsFilaSms" GridLines="None"
                                EmptyDataText="Ainda não foi enviado SMS.">
                                <Columns>
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:BoundField DataField="CelCliente" HeaderText="Destinatário" SortExpression="CelCliente" />
                                    <asp:BoundField DataField="Mensagem" HeaderText="Mensagem" SortExpression="Mensagem" />
                                    <asp:BoundField DataField="DataCad" HeaderText="Data Cad." SortExpression="DataCad" />
                                    <asp:BoundField DataField="DataEnvio" HeaderText="Data Envio" SortExpression="DataEnvio" />
                                    <asp:BoundField DataField="CodResultado" HeaderText="Cód. Resultado" SortExpression="CodResultado" />
                                    <asp:BoundField DataField="DescricaoResultado" HeaderText="Resultado" SortExpression="DescricaoResultado" />
                                    <asp:BoundField DataField="NumeroTentativas" HeaderText="Tentativas com erro" SortExpression="NumeroTentativas" />
                                </Columns>
                                <PagerStyle CssClass="pgr" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFilaSms" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.FilaSmsDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNomeLoja" Name="nomeLoja" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ctrlDataEnvioIniSms" Name="dataEnvioIni" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataEnvioFimSms" Name="dataEnvioFim" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataCadIniSms" Name="dataCadIni" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="ctrlDataCadFimSms" Name="dataCadFim" PropertyName="DataString" />
                                    <asp:ControlParameter ControlID="txtDestinatarioSms" Name="destinatario" PropertyName="Text" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
