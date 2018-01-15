<%@ Page Title="Configuração de folgas das peças" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPecaModelo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadPecaModelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Código Modelo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição Modelo" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="170px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                                    </td>
                        <td>
                            <asp:Label ID="Grupo" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" 
                                DataSourceID="odsGrupoModelo" DataTextField="Descricao" 
                                DataValueField="IdGrupoModelo">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                        <td style='<%= Glass.Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ? "" : "display: none" %>'>
                            <asp:Label ID="Label1" runat="server" Text="Cod.Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style='<%= Glass.Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ? "" : "display: none" %>'>
                            <asp:TextBox ID="txtCodCliente" runat="server" Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td style='<%= Glass.Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ? "" : "display: none" %>'>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                    </tr>                  
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkInsercaoRapidaFolga" runat="server" Text="Inserção Rápida de Folga"
                                AutoPostBack="true" OnCheckedChanged="chkInsercaoRapidaFolga_CheckedChanged" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPecaProjMod" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    DataKeyNames="IdPecaProjMod" DataSourceID="odsPecaProjMod"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# string.IsNullOrEmpty(txtCodCliente.Text) %>'>
                                    <img border="0" src="../../Images/Edit.gif"></img></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" 
                                    Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Modelo" SortExpression="CodModelo">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("CodModelo") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodModelo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipoSigla">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("DescrTipoSigla") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrTipoSigla") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item" SortExpression="Item">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Item") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Item") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--Inicio Visualização de Folga--%>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 03MM" SortExpression="Largura03MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura03MM" runat="server" Text='<%# Bind("Largura03MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura03MM" runat="server" Text='<%# Bind("Largura03MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 03MM" SortExpression="Altura03MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura03MM" runat="server" Text='<%# Bind("Altura03MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura03MM" runat="server" Text='<%# Bind("Altura03MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 04MM" SortExpression="Largura04MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura04MM" runat="server" Text='<%# Bind("Largura04MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura04MM" runat="server" Text='<%# Bind("Largura04MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 04MM" SortExpression="Altura04MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura04MM" runat="server" Text='<%# Bind("Altura04MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura04MM" runat="server" Text='<%# Bind("Altura04MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 05MM" SortExpression="Largura05MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura05MM" runat="server" Text='<%# Bind("Largura05MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura05MM" runat="server" Text='<%# Bind("Largura05MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 05MM" SortExpression="Altura05MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura05MM" runat="server" Text='<%# Bind("Altura05MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura05MM" runat="server" Text='<%# Bind("Altura05MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 06MM" SortExpression="Largura06MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura06MM" runat="server" Text='<%# Bind("Largura06MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura06MM" runat="server" Text='<%# Bind("Largura06MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 06MM" SortExpression="Altura06MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura06MM" runat="server" Text='<%# Bind("Altura06MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura06MM" runat="server" Text='<%# Bind("Altura06MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 08MM" SortExpression="Largura08MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura08MM" runat="server" Text='<%# Bind("Largura08MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura08MM" runat="server" Text='<%# Bind("Largura08MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 08MM" SortExpression="Altura08MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura08MM" runat="server" Text='<%# Bind("Altura08MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura08MM" runat="server" Text='<%# Bind("Altura08MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 10MM" SortExpression="Largura10MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura10MM" runat="server" Text='<%# Bind("Largura10MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura10MM" runat="server" Text='<%# Bind("Largura10MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 10MM" SortExpression="Altura10MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura10MM" runat="server" Text='<%# Bind("Altura10MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura10MM" runat="server" Text='<%# Bind("Altura10MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 12MM" SortExpression="Largura12MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura12MM" runat="server" Text='<%# Bind("Largura12MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura12MM" runat="server" Text='<%# Bind("Largura12MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 12MM" SortExpression="Altura12MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura12MM" runat="server" Text='<%# Bind("Altura12MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura12MM" runat="server" Text='<%# Bind("Altura12MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--Fim Visualização de Folga--%>
                        <%--Inicio Inserção Rápida de Folga--%>
                        <asp:TemplateField HeaderText="Largura" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLarguraInsercaoRapida" runat="server" Text='<%# Bind("Largura") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAlturaInsercaoRapida" runat="server" Text='<%# Bind("Altura") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 03MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura03MMInsercaoRapida" runat="server" Text='<%# Bind("Largura03MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 03MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura03MMInsercaoRapida" runat="server" Text='<%# Bind("Altura03MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 04MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura04MMInsercaoRapida" runat="server" Text='<%# Bind("Largura04MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 04MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura04MMInsercaoRapida" runat="server" Text='<%# Bind("Altura04MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 05MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura05MMInsercaoRapida" runat="server" Text='<%# Bind("Largura05MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 05MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura05MMInsercaoRapida" runat="server" Text='<%# Bind("Altura05MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 06MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura06MMInsercaoRapida" runat="server" Text='<%# Bind("Largura06MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 06MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura06MMInsercaoRapida" runat="server" Text='<%# Bind("Altura06MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 08MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura08MMInsercaoRapida" runat="server" Text='<%# Bind("Largura08MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 08MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura08MMInsercaoRapida" runat="server" Text='<%# Bind("Altura08MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 10MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura10MMInsercaoRapida" runat="server" Text='<%# Bind("Largura10MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 10MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura10MMInsercaoRapida" runat="server" Text='<%# Bind("Altura10MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura 12MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLargura12MMInsercaoRapida" runat="server" Text='<%# Bind("Largura12MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura 12MM" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAltura12MMInsercaoRapida" runat="server" Text='<%# Bind("Altura12MM") %>'
                                    onkeypress="return soNumeros(event, true, false)" Width="50px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--Fim Inserção Rápida de Folga--%>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ID="Image1" runat="server" Height="170px" 
                                    ImageUrl='<%# Eval("ModeloPath") %>' 
                                    Visible='<%# (bool)((int)Eval("Tipo")==1) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ID="Image2" runat="server" Height="170px" 
                                    ImageUrl='<%# Eval("ModeloProjetoPath") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" Value='<%# Eval("IdPecaProjMod") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button runat="server" ID="btnSalvarInsercaoRapida" Text="Salvar" Width="100"
                    Visible="false" OnClick="btnSalvarInsercaoRapida_Click" OnClientClick="bloquearPagina(); desbloquearPagina(false);" />
                <div id="blanket" style="display: none; position: fixed; left: 0px; top: 0px; z-index: 99999;
                    width: 100%; height: 100%">
                    <iframe frameborder="0" style="position: absolute; left: 0px; top: 0px; width: 100%;
                        height: 100%"></iframe>
                    <div style="width: 100%; height: 100%; opacity: 0.8; background-color: white; position: absolute;
                        left: 0; top: 0">
                    </div>
                    <div style="text-align: center; top: 40%; position: relative">
                        <span>
                            <img src="<%= this.ResolveClientUrl("~/Images/Load.gif") %>" height="96px" />
                            <br />
                            <span style="font-size: xx-large">Aguarde </span>
                            <br />
                            <span style="font-size: medium">Processando suas informações... </span></span>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPecaProjMod" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.PecaProjetoModelo" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.PecaProjetoModeloDAO" UpdateMethod="UpdateFolga">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupoModelo" Name="idGrupoModelo" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodCliente" Name="idCliente" 
                            PropertyName="Text" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
        
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" 
        SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.GrupoModeloDAO">
    </colo:VirtualObjectDataSource>
        
            </td>
        </tr>
    </table>
</asp:Content>

