<%@ Page Title="Controle de Cheques em Advogados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadChequeAdvogado.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadChequeAdvogado" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">
        var linkMarcarAdvogado = null;
        
        function marcarAdvogado()
        {
            if (linkMarcarAdvogado != null)
            {
                var funcao = linkMarcarAdvogado.href.substr(11);
                funcao = funcao.replace(/\%20/g, " ");
                
                eval(funcao);
            }
        }
    </script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Libera��o"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Num. Acerto"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" ForeColor="#0066FF" Text="Nota Fiscal"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNfe" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Num. Cheque"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCheque" runat="server" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Titular"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitular" runat="server" MaxLength="40" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Ag�ncia"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAgencia" runat="server" MaxLength="25" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Conta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtConta" runat="server" MaxLength="20" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Per�odo Venc."></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
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
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Cheques Advogados"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpAdvogados" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Apenas Advogados</asp:ListItem>
                                <asp:ListItem Value="2">N�o Advogados</asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" /></td>
                        <td>
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Situa��o"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" 
                                Title="Selecione uma situa��o">
                                <asp:ListItem Selected="true" Value="3">Devolvido</asp:ListItem>
                                <asp:ListItem Selected="true" Value="7">Protestado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            at�
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
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Pr�prio</asp:ListItem>
                                <asp:ListItem Selected="True" Value="2">Terceiros</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdCheque" DataSourceID="odsCheques"
                    EmptyDataText="Nenhum cheque compensado encontrado." PageSize="15" OnRowCommand="grdCheque_RowCommand"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" Style="margin-right: 0px">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" MaxLength="50" Text='<%# Bind("Num") %>'
                                    Width="60px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Num") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBanco" runat="server" MaxLength="25" Text='<%# Bind("Banco") %>'
                                    Width="90px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ag�ncia" SortExpression="Agencia">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAgencia0" runat="server" MaxLength="25" Text='<%# Bind("Agencia") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtConta0" runat="server" MaxLength="20" Text='<%# Bind("Conta") %>'
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular0" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'
                                    Width="170px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <uc1:ctrltextboxfloat ID="ctrlTextBoxFloat6" runat="server" Value='<%# Bind("Valor") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlDataVencGrid" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                    DataString='<%# Bind("DataVencString") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situa��o" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkAdvogado" runat="server" CommandArgument='<%# Eval("IdCheque") %>'
                                    CommandName='<%# (bool)Eval("Advogado") ? "DesmarcarAdvogado" : "MarcarAdvogado" %>' Text='<%# (bool)Eval("Advogado") ? "Desmarcar Advogado" : "Marcar Advogado" %>'
                                    OnClientClick="confirm('Tem certeza que deseja marcar/desmarcar este cheque como enviado ao Advogado?'); linkMarcarAdvogado(this); return false;"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCheques" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountAdvogado" SelectMethod="GetForAdvogado" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChequesDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNfe" Name="numeroNfe" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" DefaultValue="" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCheque" Name="numCheque" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpAdvogados" Name="advogado" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="txtTitular" Name="titular" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtAgencia" Name="agencia" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtConta" Name="conta" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
