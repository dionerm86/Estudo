<%@ Page Title="Cadastro de Rota" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadRota.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRota" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DadosRota&exportarExcel=" + exportarExcel + "&idRota=<%= Request["idRota"] %>");
        }

        function onSave() {
            if (FindControl("txtCodigo", "input").value == "") {
                alert("Informe o código da rota.");
                return false;
            }

            return true;
        }

        function setClienteCustom(idCliente) {
            var retorno = CadRota.AssociaCliente('<%= Request["IdRota"] %>', idCliente).value;

            if (retorno == null) {
                alert("Recarregando a página para continuar o cadastro...");
                return true;
            }

            retorno = retorno.split("|");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            redirectUrl(window.location.href + "&ref" + Math.random() + "=1");
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvRota" runat="server" SkinID="defaultDetailsView"
                    DataSourceID="odsRota" ondatabound="dtvRota_DataBound" DefaultMode="Edit" EnableViewState="false"
                    OnItemUpdated="dtvRota_ItemUpdated">
                    <Fields>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodigo" runat="server" MaxLength="20" Text='<%# Bind("CodInterno") %>'
                                    Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodigo" runat="server" MaxLength="20" Text='<%# Bind("CodInterno") %>'
                                    Width="100px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>' Width="250px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Distância" SortExpression="Distancia">
                            <ItemTemplate>
                                <%# Eval("Distancia") %>km
                            </ItemTemplate>
                            <EditItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="TextBox5" runat="server" MaxLength="10" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("Distancia") %>' Width="70px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;<asp:Label ID="Label6" runat="server" Text="km"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="TextBox5" runat="server" MaxLength="10" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("Distancia") %>' Width="70px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;<asp:Label ID="Label6" runat="server" Text="km"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' MaxLength="300"
                                    Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dia da Rota" SortExpression="DiasSemana">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("DiaSemana")).Format() %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <sync:CheckBoxListDropDown ID="cblDiaSemana" runat="server" Width="300px" CheckAll="False"
                                    Title="Selecione o dia" DataSourceID="odsDiasSemana" DataTextField="Translation" DataValueField="Value"
                                    ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                    OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <sync:CheckBoxListDropDown ID="cblDiaSemana" runat="server" Width="300px" CheckAll="False"
                                    Title="Selecione o dia" DataSourceID="odsDiasSemana" DataTextField="Translation" DataValueField="Value"
                                    ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                    OpenOnStart="False">
                                    </sync:CheckBoxListDropDown>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número Mínimo de Dias p/ Entrega" SortExpression="NumeroMinimoDiasEntrega">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("NumeroMinimoDiasEntrega") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("NumeroMinimoDiasEntrega") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("NumeroMinimoDiasEntrega") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Tipo Entrega" SortExpression="TipoEntrega">
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# (bool)Eval("EntregaBalcao") == true ? "Balcão" : "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                            <asp:CheckBox ID="chkTipoEntrega" runat="server" Text="Balcão"  checked='<%# Bind("EntregaBalcao") %>' />
                            
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:CheckBox ID="chkTipoEntrega" runat="server" Text="Balcão"  Checked='<%# Bind("EntregaBalcao") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        
                        
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return onSave();"
                                    Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CommandName="Cancel" OnClick="btnVoltar_Click"
                                    Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="return onSave();"
                                    Text="Inserir" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Rota"
                    InsertMethod="SalvarRota" 
                    SelectMethod="ObtemRota" 
                    TypeName="Glass.Global.Negocios.IRotaFluxo"
                    UpdateMethod="SalvarRota" 
                    UpdateStrategy="GetAndUpdate"
                    OnInserted="odsRota_Inserted" 
                    OnUpdated="odsRota_Updated" oninserting="odsRota_Inserting" 
                    onupdating="odsRota_Updating">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idRota" QueryStringField="idRota" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                 <colo:VirtualObjectDataSource  Culture="pt-BR" runat="server" ID="odsDiasSemana"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.DiasSemana, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="subtitle1" align="center">
                <asp:Label ID="lblSubtitle" runat="server" Text="Clientes Associados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkAssociarCliente" runat="server" OnClientClick="openWindow(600, 800, '../Utils/SelCliente.aspx?rota=true&custom=1'); return false;">Associar Cliente</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdClientes" runat="server" CssClass="gridStyle" SkinID="defaultGridView"
                    DataSourceID="odsRotaCliente" EmptyDataText="Nenhum cliente associado à esta Rota." 
                    OnRowCommand="grdClientes_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDesassociar" runat="server" CommandArgument='<%# Eval("IdCliente") %>'
                                    CommandName="Excluir" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Desassociar" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                            <ItemTemplate>
                                <%# Eval("IdCliente") %> - <%# Eval("Nome") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" SortExpression="EnderecoCompleto" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" 
                                    CommandArgument='<%# Eval("IdRota") + "," + Eval("IdCliente") %>' 
                                    CommandName="Up" ImageUrl="~/Images/up.gif" />
                                <asp:ImageButton ID="imgDown" runat="server" 
                                    CommandArgument='<%# Eval("IdRota") + "," + Eval("IdCliente") %>' 
                                    CommandName="Down" ImageUrl="~/Images/down.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRotaCliente" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarClientesRota" SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IRotaFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idRota" QueryStringField="idRota" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> 
                            <img border="0" src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton>
                    </td>
                </tr>
            </td>
        </tr>
    </table>
</asp:Content>
