<%@ Page Title="Parcelas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadParcelas.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadParcelas" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
    function validar()
    {
        if (FindControl("drpTipoPagto", "select").value == "1")
        {
            if (FindControl("hdfNumParcelas", "input").value == "" || FindControl("hdfDias", "input").value == "")
            {
                alert("Cadastre os dias das parcelas.");
                return false;
            }
        }
        
        return true;
    }
    
    function adicionar(dia)
    {
        if (typeof dia == "undefined")
        {
            if (document.getElementById("dia").value == "")
                return;
            
            dia = document.getElementById("dia").value;
        }
        
        var diaAdd = document.createElement("option");
        diaAdd.text = dia;
        diaAdd.value = dia;
        
        var lista = document.getElementById("<%= dtvParcela.ClientID %>_lstDias");
        lista.options.add(diaAdd);
        
        for (i = 0; i < lista.options.length - 1; i++)
            for (j = i + 1; j < lista.options.length; j++)
            {
                var valorI = parseInt(lista.options[i].value);
                var valorJ = parseInt(lista.options[j].value);
                
                if (valorJ < valorI)
                {
                    var temp = lista.options[i];
                    lista.options[i] = lista.options[j];
                    lista.options[j] = temp;
                }
            }
        
        document.getElementById("dia").value = "";
        document.getElementById("dia").focus();
        atualizarDados();
    }
    
    function remover()
    {
        var lista = document.getElementById("<%= dtvParcela.ClientID %>_lstDias");
        lista.remove(lista.selectedIndex);
        atualizarDados();
    }
    
    function atualizarDados(numParcelas, dias)
    {
        var lista = document.getElementById("<%= dtvParcela.ClientID %>_lstDias");
        
        if (typeof numParcelas == "undefined")
            numParcelas = lista.options.length;
        
        if (typeof dias == "undefined")
        {
            dias = "";
            for (i = 0; i < lista.options.length; i++)
                dias += "," + lista.options[i].value;
            
            if (dias.length > 0)
                dias = dias.substr(1);
        }
        
        FindControl("hdfNumParcelas", "input").value = numParcelas;
        FindControl("hdfDias", "input").value = dias;
    }
    
    function alteraTipoPagto(controle)
    {
        var exibirDias = controle.value == "1";
        
        var linha = document.getElementById("diasParcelas");
        while (linha.nodeName.toLowerCase() != "tr")
            linha = linha.parentNode;
        
        linha.style.display = exibirDias ? "" : "none";
        
        var numParcelas = exibirDias ? undefined : "0";
        var dias = exibirDias ? undefined : "";
        
        atualizarDados(numParcelas, dias);
    }
    
    </script>

    <section>
        <div>
            <asp:DetailsView ID="dtvParcela" runat="server" SkinID="defaultDetailsView"
                DataSourceID="odsParcelas" DataKeyNames="IdParcela" OnLoad="dtvParcela_Load">
                <Fields>
                    <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                Width="250px"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo Pagto." SortExpression="NumParcelas">
                        <EditItemTemplate>
                            <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="alteraTipoPagto(this)"
                                SelectedValue='<%# (int)Eval("NumParcelas") == 0 ? 0 : 1 %>'>
                                <asp:ListItem Value="0">À vista</asp:ListItem>
                                <asp:ListItem Value="1">À prazo</asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="alteraTipoPagto(this)">
                                <asp:ListItem Value="0">À vista</asp:ListItem>
                                <asp:ListItem Value="1">À prazo</asp:ListItem>
                            </asp:DropDownList>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Dias">
                        <EditItemTemplate>
                            <table id="diasParcelas">
                                <tr>
                                    <td>
                                        <input type="text" id="dia" style="width: 150px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) adicionar();" />
                                    </td>
                                    <td>
                                        <img src="../Images/Insert.gif" id="add" style="cursor: pointer" onclick="adicionar()" alt="Adicionar" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:ListBox ID="lstDias" runat="server" Rows="8" Width="150px"></asp:ListBox>
                                    </td>
                                    <td valign="top">
                                        <img src="../Images/ExcluirGrid.gif" style="cursor: pointer" onclick="remover()" alt="Remover" />
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdfNumParcelas" runat="server" Value='<%# Bind("NumParcelas") %>' />
                            <asp:HiddenField ID="hdfDias" runat="server" Value='<%# Bind("Dias") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Desconto (%)" SortExpression="Desconto">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDesconto" runat="server" MaxLength="50" Text='<%# Bind("Desconto") %>'
                                Width="250px"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Desconto") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkParcelaPadrao" Text="Exibir marcado como padrão?" Checked='<%# Bind("ParcelaPadrao") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="if (!validar()) return false;"
                                Text="Atualizar" />
                            <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancelar" OnClick="btnCancelar_Click" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!validar()) return false;"
                                Text="Inserir" />
                            <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancelar" OnClick="btnCancelar_Click" />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
                <HeaderStyle CssClass="dtvHeader" />
            </asp:DetailsView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" 
                DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.Parcelas"
                InsertMethod="SalvarParcela" 
                SelectMethod="ObtemParcela" TypeName="Glass.Financeiro.Negocios.IParcelasFluxo"
                UpdateMethod="SalvarParcela"
                UpdateStrategy="GetAndUpdate">
                <SelectParameters>
                    <asp:QueryStringParameter Name="idParcela" QueryStringField="idParcela" Type="Int32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </div>
    </section>

    <script type="text/javascript">
    
    var dias = FindControl("hdfDias", "input").value.split(",");
    for (d = 0; d < dias.length; d++)
        if (dias[d].length > 0)
            adicionar(dias[d]);
    
    alteraTipoPagto(FindControl("drpTipoPagto", "select"));
    
    var idParcela = GetQueryString("idParcela");
    
    if(idParcela == null)
        FindControl("chkParcelaPadrao","input").checked = true;
    
    </script>

</asp:Content>
