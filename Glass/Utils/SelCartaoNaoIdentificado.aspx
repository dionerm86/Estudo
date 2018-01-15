<%@ Page Title="Selecione o Cartão não Identificado" Language="C#" AutoEventWireup="true" CodeBehind="SelCartaoNaoIdentificado.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelCartaoNaoIdentificado" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function setCartaoNaoIdentificado(idCartaoNaoIdentificado, contaBancaria, valor, tipoCartao, observacao)
        {
            var nomeTabelaCNIOpener = <%= !String.IsNullOrEmpty(Request["tabelaCNI"]) ? Request["tabelaCNI"] : "'tbCNIPagto'" %>;
            
            var nomeControleFormaPagto = <%= Request["nomeControleFormaPagto"] != null ? Request["nomeControleFormaPagto"] : "''" %>;

            window.opener.setCartaoNaoIdentificado(nomeTabelaCNIOpener, window, idCartaoNaoIdentificado, contaBancaria, valor,
                tipoCartao, observacao, nomeControleFormaPagto);            
        }
    </script>

    <table style="width: 100%;">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Cartão não identificado"></asp:Label>
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
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Tipo Cartão"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoCartao" runat="server" Title="Selecione o tipo Cartão">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                                <asp:ListItem Value="1" Text="Master Crédito" />
                                <asp:ListItem Value="3" Text="Master Débito" />
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
                    </tr>
                </table>                             
                <table>
                    <tr>
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
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="Button1" runat="server" OnClientClick="closeWindow();" Text="Fechar" />
                <br />
                <asp:GridView GridLines="None" ID="grdCNI" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdCartaoNaoIdentificado" DataSourceID="odsCartaoNaoIdentificado"
                    EmptyDataText="Nenhum cartão não identificado cadastrado." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setCartaoNaoIdentificado(<%# Eval("IdCartaoNaoIdentificado") %>, '<%# Eval("ContaBancaria") %>',  <%# Eval("Valor").ToString().Replace(".", "").Replace(",", ".") %>, '<%# Eval("TipoCartao") %>', '<%# Eval("Observacao") %>');">
                                    <img src="../Images/Insert.gif" border="0" title="Incluir CNI no pagamento" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cod." SortExpression="Num">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("IdCartaoNaoIdentificado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("ContaBancaria") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Cartão" SortExpression="TipoCartao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("TipoCartao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Estabelecimento" SortExpression="Titular">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("NumeroEstabelecimento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>       
                        <asp:TemplateField HeaderText="Últimos Dig. Cartão" SortExpression="Titular">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("UltimosDigitosCartao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>       
                        <asp:TemplateField HeaderText="Observação" SortExpression="Titular">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                       
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCartaoNaoIdentificado" runat="server" EnablePaging="True"
        SelectMethod="PesquisarCartoesNaoIdentificados"
        TypeName="Glass.Financeiro.Negocios.ICartaoNaoIdentificadoFluxo">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtIdCartaoNaoIdentificado" Name="idCartaoNaoIdentificado"  PropertyName="Text"/>
            <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicio" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtValorFinal" Name="valorFim" PropertyName="Text" />
            <asp:Parameter Name="situacao" DefaultValue="1"/>
            <asp:ControlParameter ControlID="drpTipoCartao" Name="tipoCartao" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadInicio" PropertyName="DataString" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString" />
            <asp:ControlParameter ControlID="ctrlDataVendaIni" Name="dataVendaInicio" PropertyName="DataString" />
            <asp:ControlParameter ControlID="ctrlDataVendaFim" Name="dataVendaFim" PropertyName="DataString" />
            <asp:ControlParameter ControlID="txtnumAutorizacao" Name="nAutorizacao" PropertyName="Text" />

            <asp:ControlParameter ControlID="txtNEstab" Name="numEstabelecimento" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtUltDig" Name="ultimosDigitosCartao" PropertyName="Text" />
            <asp:Parameter Name="codArquivo"  DefaultValue="" />
            <asp:Parameter Name="dataImportacao" DefaultValue="" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
