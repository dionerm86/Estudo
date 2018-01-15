<%@ Page Title="Cartões não identificados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstCartaoNaoIdentificado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCartaoNaoIdentificado" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlCancLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openMotivoCanc(idCartaoNaoIdentificado) {
            openWindow(150, 450, "../Utils/SetMotivoCancCartaoNaoIdentificado.aspx?idCartaoNaoIdentificado=" + idCartaoNaoIdentificado);
            return false;
        }

        function openRpt(exportarExcel) {
            var idCartaNaoIdentificado = FindControl("txtIdCartaoNaoIdentificado", "input").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var valorIni = FindControl("txtValorInicial", "input").value;
            var valorFim = FindControl("txtValorFinal", "input").value;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataVendaIni = FindControl("ctrlDataVendaIni_txtData", "input").value;
            var dataVendaFim = FindControl("ctrlDataVendaFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var tipoCartao = FindControl("drpTipoCartao", "select").value;
            var nAutorizacao = FindControl("txtnumAutorizacao", "input").value;
            var nEstab = FindControl("txtNEstab", "input").value;
            var ultimosDig = FindControl("txtUltDig", "input").value;
            var codArquivo = FindControl("txtCodArquivo", "input").value;
            var dataImport = FindControl("ctrlDataImport_txtData", "input").value;       
            

            var query = "../Relatorios/RelBase.aspx?Rel=ListaCartaoNaoIdentificado"
                        + "&idDepositoNaoIdentificado=" + idCartaNaoIdentificado
                        + "&idContaBanco=" + idContaBanco
                        + "&valorIni=" + valorIni
                        + "&valorFim=" + valorFim
                        + "&situacao=" + situacao
                        + "&tipoCartao=" + tipoCartao
                        + "&dataCadIni=" + dataCadIni
                        + "&dataCadFim=" + dataCadFim
                        + "&dataVendaIni=" + dataVendaIni
                        + "&dataVendaFim=" + dataVendaFim
                        + "&nAutorizacao=" + nAutorizacao
                        + "&numEstabelecimento=" + nEstab
                        + "&ultimosDigitosCartao=" + ultimosDig
                        + "&codArquivo=" + codArquivo
                        + "&dataImportacao=" + dataImport
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
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Cod. Cartão não identificado"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdCartaoNaoIdentificado" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                                <asp:ListItem Value="1" Text="Ativo" />
                                <asp:ListItem Value="3" Text="Em Uso" />
                                <asp:ListItem Value="2" Text="Cancelado" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Tipo Cartão"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoCartao" runat="server" Title="Selecione o tipo Cartão">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                                <asp:ListItem Value="1" Text="MasterCard Crédito" />
                                <asp:ListItem Value="3" Text="MasterCard Débito" />
                                <asp:ListItem Value="4" Text="Visa Crédito" />
                                <asp:ListItem Value="5" Text="Visa Débito" />
                                <asp:ListItem Value="6" Text="Outros Crédito" />
                                <asp:ListItem Value="7" Text="Outros Débito" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Cadastro"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td> 
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período Venda"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataVendaIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataVendaFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td> 
                    </tr>
                </table>              
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Nº de Autorização"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtnumAutorizacao" runat="server" Width="160px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Nº Estabelecimento"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNEstab" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Últimos dig. Cartão"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtUltDig" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Cod. Arquivo"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodArquivo" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Data Importação"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataImport" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"> Inserir cartão não identificado</asp:LinkButton>
                &nbsp;
                &nbsp;
                <asp:LinkButton ID="lnkImportar" runat="server" OnClick="lnkImportar_Click"> Importar cartão não identificado</asp:LinkButton>
            </td>          
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCartaoNaoIdentificado" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdCartaoNaoIdentificado"
                    DataSourceID="odsCartaoNaoIdentificado" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum cartão não identificado encontrado.">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" ImageUrl="~/Images/EditarGrid.gif"
                                    NavigateUrl='<%# "../Cadastros/CadCartaoNaoIdentificado.aspx?idcartaoNaoIdentificado=" + Eval("IdCartaoNaoIdentificado") + (!string.IsNullOrEmpty(Request["cxDiario"]) ? "&cxDiario=1": "") %>'
                                    Visible='<%# Eval("PodeEditar") %>' />
                                <asp:ImageButton ID="imbCancelar" runat="server" ToolTip="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "return openMotivoCanc(" + Eval("IdCartaoNaoIdentificado") + ");" %>'
                                    Visible='<%# Eval("PodeCancelar") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCartaoNaoIdentificado" HeaderText="Cód." SortExpression="idCartaoNaoIdentificado" />
                        <asp:BoundField DataField="ContaBancaria" HeaderText="Conta Bancária" SortExpression="DescrContaBanco" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="ValorMov"  DataFormatString="{0:C}" />                                               
                        <asp:BoundField DataField="DataVenda" HeaderText="Data da Venda" SortExpression="DataVenda" DataFormatString="{0:d}" />                        
                        <asp:BoundField DataField="TipoCartaoStr" HeaderText="Tipo Cartão" SortExpression="TipoCartao" />
                        <asp:BoundField DataField="NumAutCartao" HeaderText="Nº Autorização" SortExpression="NumAutCartao" />
                        <asp:BoundField DataField="NumeroEstabelecimento" HeaderText="Nº Estabelecimento" SortExpression="NumeroEstabelecimento" />
                        <asp:BoundField DataField="UltimosDigitosCartao" HeaderText="Últimos Dig. Cartão" SortExpression="UltimosDigitosCartao" />
                        <asp:BoundField DataField="FuncionarioCadastro" HeaderText="Func. Cad" SortExpression="FuncionarioCadastro" />
                        <asp:BoundField DataField="NumeroParcelas" HeaderText="Nº de Parcelas"  ItemStyle-HorizontalAlign="Center" />     
                        <asp:BoundField DataField="IdArquivoCartaoNaoIdentificado" HeaderText="Cod. Arquivo" SortExpression="IdArquivoCartaoNaoIdentificado" />
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data de Cadastro" SortExpression="DataCad" DataFormatString="{0:d}"/>
                        <asp:BoundField DataField="Observacao" HeaderText="Obs." SortExpression="Obs" />           
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlCancLogPopup ID="ctrlLogCancPopup2" runat="server" Tabela="CartaoNaoIdentificado" IdRegistro='<%# Convert.ToUInt32(Eval("IdCartaoNaoIdentificado")) %>' />
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
                <asp:Label ID="Label9" runat="server" ForeColor="#FF0000" Text="Cartões não identificados que foram importados não podem sem cancelados ou sofrer alterações."></asp:Label>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCartaoNaoIdentificado" runat="server" EnablePaging="True"
                    SelectMethod="PesquisarCartoesNaoIdentificados"
                    TypeName="Glass.Financeiro.Negocios.ICartaoNaoIdentificadoFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdCartaoNaoIdentificado" Name="idCartaoNaoIdentificado"  PropertyName="Text"/>
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicio" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFim" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpTipoCartao" Name="tipoCartao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadInicio" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataVendaIni" Name="dataVendaInicio" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataVendaFim" Name="dataVendaFim" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="txtnumAutorizacao" Name="nAutorizacao" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtNEstab" Name="numEstabelecimento" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtUltDig" Name="ultimosDigitosCartao" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtCodArquivo" Name="codArquivo" PropertyName="Text" />
                        <asp:ControlParameter ControlID="ctrlDataImport" Name="dataImportacao" PropertyName="DataString" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
