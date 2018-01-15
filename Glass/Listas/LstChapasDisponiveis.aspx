<%@ Page Title="Chapas Disponíveis" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstChapasDisponiveis.aspx.cs" Inherits="Glass.UI.Web.Listas.LstChapasDisponiveis" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function getProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;
            var resposta = MetodosAjax.GetProd(codInterno).value.split(';');

            if (resposta[0] == "Erro") {
                alert(resposta[1]);
                return;
            }

            FindControl("txtDescrProd", "input").value = resposta[2];
        }

        function openRpt(exportarExcel) {
            var idFornec = FindControl("txtFornecedor", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var codInternoProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var lote = FindControl("txtLote", "input").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var idCor = FindControl("cbdCorVidro", "select").itens();
            var espessura = FindControl("txtEspessura", "input").value;
            var numEtiqueta = FindControl("txtEtiqueta", "input").value;
            var numeroNfe = FindControl("txtNfe", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;


            var queryString = "&idFornec=" + idFornec +
                "&nomeFornec=" + nomeFornec +
                "&codInternoProd=" + codInternoProd +
                "&descrProd=" + descrProd +
                "&numeroNfe=" + numeroNfe +
                "&lote=" + lote +
                "&altura=" + altura +
                "&largura=" + largura +
                "&idCor=" + idCor +
                "&espessura=" + espessura +
                "&numEtiqueta=" + numEtiqueta +
                "&idLoja=" + idLoja +
                "&exportarexcel=" + exportarExcel;

            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=ChapasDisponiveis' + queryString);
            return false;
        }

    </script>

    <table>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Cor"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdCorVidro" runat="server" DataSourceID="odsCorVidro"
                                DataTextField="Descricao" AppendDataBoundItems="true" DataValueField="IdCorVidro"
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" Title="Selecione uma cor">
                                <asp:ListItem Value="0">SEM COR</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Espessura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="NF-e"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNfe" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Lote"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLote" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Altura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Largura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Produto"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="50px" onblur="getProduto()" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Etiqueta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlloja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="False"
                                MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="center">
            <td></td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grChapasDisponiveis" runat="server" AllowPaging="True"
                    AllowSorting="false" DataSourceID="odsChapasDisponiveis" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AutoGenerateColumns="False"
                    PageSize="20">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="Cor" HeaderText="Cor" SortExpression="Cor" />
                        <asp:BoundField DataField="Espessura" HeaderText="Espessura" SortExpression="Espessura" />
                        <asp:BoundField DataField="Fornecedor" HeaderText="Fornecedor" SortExpression="Fornecedor" />
                        <asp:BoundField DataField="NumeroNfe" HeaderText="NF-e" SortExpression="NumeroNfe" />
                        <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                        <asp:BoundField DataField="Produto" HeaderText="Produto" SortExpression="Produto" />
                        <asp:BoundField DataField="Etiqueta" HeaderText="Etiqueta" SortExpression="Etiqueta" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label ID="Label21" runat="server" ForeColor="Red" Text="São consideradas todas as chapas com etiquetas impressas e não cortadas/expedidas"></asp:Label>
            </td>
        </tr>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>&nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsChapasDisponiveis" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="ObtemChapasDisponiveisCount" SelectMethod="ObtemChapasDisponiveis" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ChapasDisponiveisDAO"
                    SkinID="">
                    <SelectParameters>
                        <asp:ControlParameter Name="idFornec" Type="Int32" ControlID="txtFornecedor" PropertyName="Text" />
                        <asp:ControlParameter Name="nomeFornec" Type="String" ControlID="txtNome" PropertyName="Text" />
                        <asp:ControlParameter Name="codInternoProd" Type="String" ControlID="txtCodProd" PropertyName="Text" />
                        <asp:ControlParameter Name="descrProd" Type="String" ControlID="txtDescrProd" PropertyName="Text" />
                        <asp:ControlParameter Name="numeroNfe" Type="Int32" ControlID="txtNfe" PropertyName="Text" />
                        <asp:ControlParameter Name="lote" Type="String" ControlID="txtLote" PropertyName="Text" />
                        <asp:ControlParameter Name="altura" Type="Int32" ControlID="txtAltura" PropertyName="Text" />
                        <asp:ControlParameter Name="largura" Type="Int32" ControlID="txtLargura" PropertyName="Text" />
                        <asp:ControlParameter Name="idCor" Type="String" ControlID="cbdCorVidro" PropertyName="SelectedValue" />
                        <asp:ControlParameter Name="espessura" Type="Int32" ControlID="txtEspessura" PropertyName="Text" />
                        <asp:ControlParameter Name="numEtiqueta" Type="String" ControlID="txtEtiqueta" PropertyName="Text" />
                        <asp:ControlParameter Name="idLoja" Type="Int32" ControlID="drpLoja" PropertyName="SelectedValue"   />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
