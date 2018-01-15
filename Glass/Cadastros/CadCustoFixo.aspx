<%@ Page Title="Cadastro de Custo Fixo" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadCustoFixo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCustoFixo" %>

<%@ Register src="../Controls/ctrlTextBoxFloat.ascx" tagname="ctrlTextBoxFloat" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
    function onInsert() {
        var fornec = FindControl("hdfFornec", "input").value;
        var descricao = FindControl("txtDescricao", "input").value;
        var diaVenc = FindControl("txtDiaVenc", "input").value;

        // Verifica se o fornecedor foi selecionado
        if (fornec == "" || fornec == null) {
            alert("Informe o Fornecedor.");
            return false;
        }

        // Verifica se a descrição foi informada
        if (descricao == "") {
            alert("Informe a descrição do Custo Fixo.");
            return false;
        }

        // Verifica se o dia venc. foi informado
        if (diaVenc == "") {
            alert("Informe o dia de vencimento do Custo Fixo.");
            return false;
        }

        // Verifica se o dia venc. é válido
        if (diaVenc > 31 || diaVenc == 0) {
            alert("Dia de vencimento inválido.");
            return false;
        }
    }

    function onUpdate() {
        var descricao = FindControl("txtDescricao", "input").value;
        var diaVenc = FindControl("txtDiaVenc", "input").value;

        // Verifica se a descrição foi informada
        if (descricao == "") {
            alert("Informe a descrição do Custo Fixo.");
            return false;
        }

        // Verifica se o dia venc. foi informado
        if (diaVenc == "") {
            alert("Informe o dia de vencimento do Custo Fixo.");
            return false;
        }

        // Verifica se o dia venc. é válido
        if (diaVenc > 31 || diaVenc == 0) {
            alert("Dia de vencimento inválido.");
            return false;
        }
    }

    function getFornec(idFornec) {
        var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNomeFornec", "input").value = "";
            FindControl("hdfFornec", "input").value = "";
            return false;
        }

        FindControl("txtNomeFornec", "input").value = retorno[1];
        FindControl("hdfFornec", "input").value = idFornec.value;
    }


</script>
    <table style="width: 100%">
        <tr>
            <td align="center" style="height: 211px">
                <asp:DetailsView ID="dtvCustoFixo" runat="server" AutoGenerateRows="False" 
                    CellPadding="4" DataSourceID="odsCustoFixo" DefaultMode="Insert" 
                    ForeColor="#333333" GridLines="None" Height="50px" Width="125px" 
                    DataKeyNames="IdCustoFixo">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="Black" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" 
                                    Text='<%# Bind("Descricao") %>' MaxLength="100" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" 
                                    Text='<%# Bind("Descricao") %>' Width="200px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornec">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("NomeFornec") %>'></asp:Label>
                                <asp:HiddenField ID="hdfFornec" runat="server" 
                                    Value='<%# Bind("IdFornec") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNumFornec" runat="server" onblur="getFornec(this);" 
                                    onkeypress="return soNumeros(event, true, true);" 
                                    Text='<%# Eval("IdFornec") %>' Width="50px"></asp:TextBox>
                                <asp:TextBox ID="txtNomeFornec" runat="server" ReadOnly="True" 
                                    Text='<%# Eval("NomeFornec") %>' Width="250px"></asp:TextBox>
                                <asp:LinkButton ID="lnkSelFornec" runat="server" 
                                    OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                <asp:HiddenField ID="hdfFornec" runat="server" 
                                    Value='<%# Bind("IdFornec") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdFornec") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Bind("IdLoja") %>' />                                
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="Name"
                                    DataValueField="Id" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdLoja") %>'>
                                </asp:DropDownList>                               
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("IdLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="IdConta">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DescrPlanoConta") %>'></asp:Label>
                                <asp:HiddenField ID="hdfPlanoConta" runat="server" 
                                    Value='<%# Bind("IdConta") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlPlanoConta" runat="server" 
                                    DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" 
                                    DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdConta") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVenc">
                            <EditItemTemplate>
                                <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" 
                                    Value='<%# Bind("ValorVenc")%>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" 
                                    Value='<%# Bind("ValorVenc")%>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValorVenc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dia Venc." SortExpression="DataVencString">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDiaVenc" runat="server" MaxLength="2" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("DiaVenc") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDiaVenc" runat="server" MaxLength="2" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("DiaVenc") %>' Width="50px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DataVencString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contábil" SortExpression="Contabil">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>' 
                                    Text="Contábil" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>' 
                                    Text="Contábil" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Contabil") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="2">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="2">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                                    Text="Atualizar" OnClientClick="return onUpdate();" />
                                <asp:Button ID="Button2" runat="server" onclick="btnCancelar_Click" 
                                    Text="Cancelar" CausesValidation="False" />            
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" OnClientClick="return onInsert();" 
                                    CommandName="Insert" Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" onclick="btnCancelar_Click" 
                                    Text="Cancelar" CausesValidation="False" />
                            </InsertItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField >
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfDataUltGerado" runat="server" Value='<%# Bind("DataUltGerado") %>' />
                                <asp:HiddenField ID="hdfDataUltPagto" runat="server" Value='<%# Bind("DataUltPagto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" 
                        Wrap="True" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle BackColor="White" HorizontalAlign="Left" />
                    <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCustoFixo" runat="server" 
                    oninserted="odsCustoFixo_Inserted" onupdated="odsCustoFixo_Updated" 
                    SelectMethod="GetElement" TypeName="Glass.Data.DAL.CustoFixoDAO" 
                    DataObjectTypeName="Glass.Data.Model.CustoFixo" InsertMethod="Insert" 
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCustoFixo" QueryStringField="idCustoFixo" 
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContasCompra" TypeName="Glass.Data.DAL.PlanoContasDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
                    SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

