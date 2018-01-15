<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrecoFornecedor.aspx.cs"
    Inherits="Glass.UI.Web.Utils.PrecoFornecedor" Title="Preço de Produto por Fornecedor" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function getFornec()
        {
            openWindow(600, 800, "SelFornec.aspx");
        }
        
        function setFornec(idFornec)
        {
            if (idFornec == 0 || idFornec == "")
            {
                FindControl("lblFornec", "span").innerHTML = "";
                return;
            }

            FindControl("txtIdFornec", "input").value = idFornec;
            var resposta = PrecoFornecedor.GetFornec(idFornec).value.split(';');
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                FindControl("lblFornec", "span").innerHTML = "";
                return;
            }

            FindControl("lblFornec", "span").innerHTML = resposta[1];
        }

        function getProd()
        {
            openWindow(600, 800, "SelProd.aspx");
        }

        function setProduto(codInterno)
        {
            if (codInterno == "")
            {
                FindControl("hdfIdProd", "input").value = "";
                FindControl("lblProd", "span").innerHTML = "";
                return;
            }

            FindControl("txtIdProd", "input").value = codInterno;
            var resposta = PrecoFornecedor.GetProduto(codInterno).value.split(';');
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                FindControl("lblProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                return;
            }

            FindControl("hdfIdProd", "input").value = resposta[1];
            FindControl("lblProd", "span").innerHTML = resposta[2];
            FindControl("txtCustoCompra", "input").value = resposta[3];
            FindControl("txtPrazoEntrega", "input").value= "0";
        }

        function inserir()
        {
            var exigirDataVigencia = <%= grdPrecoFornec.Columns[5].Visible.ToString().ToLower() %>;
            var idFornec = <%= !String.IsNullOrEmpty(Request["idFornec"]) ? Request["idFornec"] : "FindControl('txtIdFornec', 'input').value" %>;
            var idProd = <%= !String.IsNullOrEmpty(Request["idProd"]) ? Request["idProd"] : "FindControl('hdfIdProd', 'input').value" %>;
            var dataVigencia = <%= grdPrecoFornec.Columns[5].Visible ? "FindControl('ctrlData_txtData', 'input').value" : "''" %>;
            var custo = FindControl("txtCustoCompra", "input").value;
            var codFornec = FindControl("txtCodFornec", "input").value;
            var prazoEntrega = FindControl("txtPrazoEntrega", "input").value;
            
            if (idFornec == "" || idFornec == 0)
            {
                alert("Escolha o fornecedor.");
                return false;
            }
            
            if (idProd == "" || idProd == 0)
            {
                alert("Escolha o produto.");
                return false;
            }
            
            if (exigirDataVigencia && dataVigencia == "")
            {
                alert("Escolha a data de vigência.");
                return false;
            }
            
            if(prazoEntrega == null || prazoEntrega == ""){
                alert("Informe o prazo de entrega.");
                return false;            
            }

            var resposta = PrecoFornecedor.Inserir(idFornec, idProd, custo, codFornec, prazoEntrega, dataVigencia).value.split(';');
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }
            
            redirectUrl(window.location.href);
        }
        
        function abrirDataVigencia(idFornec, idProd)
        {
            var altura = window.innerHeight;
            var largura = window.innerWidth;
            
            openWindow(altura, largura, "PrecoFornecedor.aspx?idProd=" + idProd + "&idFornec=" + idFornec);
        }
        
        function imprimir()
        {
            var idFornec = <%= !String.IsNullOrEmpty(Request["idFornec"]) ? Request["idFornec"] : "FindControl('txtIdFornec', 'input').value" %>;
            var idProd = <%= !String.IsNullOrEmpty(Request["idProd"]) ? Request["idProd"] : "FindControl('hdfIdProd', 'input').value" %>;
            var codProd = FindControl('txtCodProd', 'input').value;
            var descrProd = FindControl('txtDescr', 'input').value;
            
            var altura = window.innerHeight;
            var largura = window.innerWidth;
            
            var queryString = "../Relatorios/RelBase.aspx?rel=PrecoFornecedor";
            queryString+= "&idProd=" + idProd;
            queryString+= "&idFornec=" + idFornec;
            queryString+= "&codProd=" + codProd;
            queryString+= "&descrProd=" + descrProd;
            
            openWindow(altura, largura, queryString);
        }
        
        // Carrega dados do produto com base no código do produto passado
    function loadProduto()
    {
        var codInterno = FindControl("txtCodProd", "input").value;

        if (codInterno == "")
            return false;

            var retorno = MetodosAjax.GetProd(codInterno).value.split(';');
            
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                FindControl("txtCodProd", "input").value = "";
                return false;
            }
            
            FindControl("txtDescr", "input").value = retorno[2];
    }
        
    </script>

    <table align="center">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblTipo" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód. do produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="loadProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Descrição do produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdPrecoFornec" runat="server" SkinID="gridViewEditable"
                    DataKeyNames="IdProdFornec" DataSourceID="odsPrecoFornec" EnableViewState="False">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse preço de produto por fornecedor?&quot;)) return false" />
                                <asp:ImageButton ID="imgDataVigencia" runat="server" ImageUrl="~/Images/calendario.gif"
                                    OnClientClick='<%# "abrirDataVigencia(" + Eval("IdFornec") + ", " + Eval("IdProd") + "); return false" %>'
                                    ToolTip="Datas de vigência" Visible='<%# ExibirDataVigencia() %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFantasiaFornecedor, RazaoSocialFornecedor">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("NomeFornecedor") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtIdFornec" runat="server" Width="50px" onchange="setFornec(this.value)"></asp:TextBox>
                                <asp:ImageButton ID="imgFornec" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="getFornec(); return false" />
                                <asp:Label ID="lblFornec" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <%# Eval("NomeFornecedor") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInternoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label132" runat="server" Text='<%# Eval("CodInternoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1125" runat="server" Text='<%# Eval("CodInternoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Cód." SortExpression="CodFornec" DataField="CodFornec" />
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescricaoProduto">
                            <EditItemTemplate>
                                <%# Eval("DescricaoProduto") %>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtIdProd" runat="server" Width="50px" onchange="setProduto(this.value)"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgProd" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getProd(); return false" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblProd" runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdfIdProd" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <%# Eval("DescricaoProduto") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data de Vigência" SortExpression="DataVigencia">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataVigencia", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlData" runat="server" DataNullable='<%# Bind("DataVigencia") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="ctrlData" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Produto Fornecedor" SortExpression="CodFornec">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodFornec" runat="server" MaxLength="60" Text='<%# Bind("CodFornec") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodFornec" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodFornec") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Prazo entrega (dias)" SortExpression="PrazoEntregaDias">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPrazoEntrega" runat="server" Text='<%# Bind("PrazoEntregaDias") %>'
                                    Width="50px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPrazoEntrega" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("PrazoEntregaDias") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Custo" SortExpression="CustoCompra">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCustoCompra" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("CustoCompra") %>' Width="80px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCustoCompra" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Width="80px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CustoCompra", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="inserir(); return false" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdProdFornec") %>'
                                    Tabela="ProdutoFornecedor" />
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="imprimir(); return false"> <img src="../Images/Printer.png" border="0" alt="Imprimir" /> Imprimir </asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPrecoFornec" runat="server"
                    TypeName="Glass.Global.Negocios.IFornecedorFluxo" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.ProdutoFornecedor"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SelectMethod="PesquisarProdutosFornecedor" SortParameterName="sortExpression" 
                    SelectByKeysMethod="ObtemProdutoFornecedor"
                    DeleteMethod="ApagarProdutoFornecedor" 
                    DeleteStrategy="GetAndDelete" EnableViewState="false"
                    UpdateMethod="SalvarProdutoFornecedor" UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idFornec" QueryStringField="idFornec" Type="Int32" />
                        <asp:QueryStringParameter Name="idProd" QueryStringField="idProd" Type="Int32" />
                        <asp:ControlParameter ControlID="hdfExibirSemData" Name="exibirSemDataVigencia" PropertyName="Value"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtCodProd" PropertyName="Text" Name="codigoProduto" Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" PropertyName="Text" Name="descricaoProduto" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
          
                <asp:HiddenField ID="hdfExibirSemData" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
