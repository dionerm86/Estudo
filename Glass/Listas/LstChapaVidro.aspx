<%@ Page Title="Chapa de Vidro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstChapaVidro.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstChapaVidro" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function setProduto(codInterno) {
            try {
                FindControl("txtCodInterno", "input").value = codInterno;
                loadProduto(codInterno);
            }
            catch (err) { }
        }

        function loadProduto(codInterno) {
            var resposta = LstChapaVidro.GetProduto(codInterno).value;
            var dadosResposta = resposta.split('|');

            if (dadosResposta[0] == "Erro") {
                alert(dadosResposta[1]);
                return;
            }

            FindControl("hdfIdProd", "input").value = dadosResposta[1];
            FindControl("lblProd", "span").innerHTML = dadosResposta[2];
        }

        function calcM2()
        {
            var idProd = FindControl("hdfIdProd", "input").value;
            var altura = FindControl("txtAlturaMin", "input").value;
            var largura = FindControl("txtLarguraMin", "input").value;
            var m2 = LstChapaVidro.CalcM2(idProd, altura, largura).value;

            for (numCampo = 1; numCampo <= 3; numCampo++)
            {
                var txtTotM2Min = FindControl("txtTotM2Min" + numCampo, "input");
                txtTotM2Min.value = m2;
            }
        }

        function openRpt() 
        {
            var codInterno = FindControl("txtCodProd", "input").value;
            var descricao = FindControl("txtDescr", "input").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;

            if (idSubgrupo == "")
                idSubgrupo = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ChapaVidro&codInterno=" + codInterno + 
                "&descricao=" + descricao + "&idSubgrupo=" + idSubgrupo);

            return false;
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;<asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            Width="200px"></asp:TextBox>
                                        <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                            DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                                        </asp:DropDownList>
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
                            <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadChapaVidro.aspx">Inserir chapa de vidro</asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdChapaVidro" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" DataKeyNames="IdChapaVidro" OnRowCommand="grdChapaVidro_RowCommand"
                                DataSourceID="odsChapaVidro" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" 
                                EmptyDataText="Não há chapa de vidro cadastrada.">
                                <Columns>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="ImageButton3" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                            <asp:ImageButton ID="ImageButton4" runat="server" CausesValidation="False" CommandName="Cancel"
                                                ImageUrl="~/Images/ExcluirGrid.gif" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                                ToolTip="Editar" />
                                            <asp:ImageButton ID="imbInativar" runat="server" CommandArgument='<%# Eval("IdChapaVidro") %>'
                                                CommandName="Inativar" ImageUrl="~/Images/Inativar.gif" OnClientClick="if (!confirm(&quot;Deseja alterar a situação dessa chapa de vidro?&quot;)) return false"
                                                ToolTip="Alterar situação"  />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Produto" SortExpression="IdProd">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtCodInterno" runat="server" Columns="4" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                                onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);" Text='<%# Eval("CodInternoProd") %>'></asp:TextBox>
                                            <asp:Label ID="lblProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelProd.aspx?chapa=true&idProd=" + Eval("IdProd") + "\"); return false;" %>' />
                                            <br />
                                            <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdDescrProduto") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantidade" SortExpression="Quantidade">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("Quantidade") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtQuantidade" Columns="5" onkeypress="return soNumeros(event, true, true)" runat="server" Text='<%# Bind("Quantidade") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Altura") %>' onkeypress="return soNumeros(event, true, true)"
                                                Columns="5"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Largura") %>' onkeypress="return soNumeros(event, true, true)"
                                                Columns="5"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura mín." SortExpression="AlturaMinima">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtAlturaMin" runat="server" onblur="calcM2()"
                                                Text='<%# Bind("AlturaMinima") %>' onkeypress="return soNumeros(event, true, true)"
                                                Columns="5"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaMinima") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largura mín." SortExpression="LarguraMinima">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtLarguraMin" runat="server" onblur="calcM2()"
                                                Text='<%# Bind("LarguraMinima") %>' onkeypress="return soNumeros(event, true, true)"
                                                Columns="5"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("LarguraMinima") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total m² mín. (1)" SortExpression="TotM2Minimo1">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtTotM2Min1" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("TotM2Minimo1") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("TotM2Minimo1") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Perc. acréscimo m² (1)" 
                                        SortExpression="PercAcrescimoTotM21">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtPercAcr1" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("PercAcrescimoTotM21") %>'></asp:TextBox>
                                            %
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" 
                                                Text='<%# Eval("PercAcrescimoTotM21") + "%" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total m² mín. (2)" SortExpression="TotM2Minimo2">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtTotM2Min2" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("TotM2Minimo2") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("TotM2Minimo2") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Perc. acréscimo m² (2)" 
                                        SortExpression="PercAcrescimoTotM22">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtPercAcr2" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("PercAcrescimoTotM22") %>'></asp:TextBox>
                                            %
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label12" runat="server" 
                                                Text='<%# Eval("PercAcrescimoTotM22") + "%" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total m² mín. (3)" SortExpression="TotM2Minimo3">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtTotM2Min3" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("TotM2Minimo3") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label15" runat="server" Text='<%# Bind("TotM2Minimo3") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Perc. acréscimo m² (3)" 
                                        SortExpression="PercAcrescimoTotM23">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtPercAcr3" runat="server" Columns="5" 
                                                onkeypress="return soNumeros(event, false, true)" 
                                                Text='<%# Bind("PercAcrescimoTotM23") %>'></asp:TextBox>
                                            %
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label16" runat="server" 
                                                Text='<%# Eval("PercAcrescimoTotM23") + "%" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Situação">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSituacao" runat="server" 
                                                Text='<%# Colosoft.Translator.Translate(((Glass.Situacao)Int32.Parse(Eval("Situacao").ToString()))).Format() %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                                IdRegistro='<%# Eval("IdChapaVidro") %>' Tabela="ChapaVidro" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle />

<EditRowStyle CssClass="edit"></EditRowStyle>

                                <AlternatingRowStyle />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsChapaVidro" runat="server" DataObjectTypeName="Glass.Data.Model.ChapaVidro"
                                DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize" 
                                SelectCountMethod="GetListCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChapaVidroDAO"
                                UpdateMethod="Update" OnDeleted="odsChapaVidro_Deleted" OnUpdated="odsChapaVidro_Updated">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtDescr" Name="produto" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                                TypeName="Glass.Data.DAL.SubgrupoProdDAO" OnLoad="odsSubgrupo_Load">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
