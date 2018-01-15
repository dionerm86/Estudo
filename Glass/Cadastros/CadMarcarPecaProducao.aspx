<%@ Page Title="Marcar Peça Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadMarcarPecaProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMarcarPecaProducao" %>

<%@ Register Src="../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlRetalhoProducao.ascx" TagName="ctrlRetalhoProducao"
    TagPrefix="uc2" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../Scripts/jquery/jquery-1.9.0.js" type="text/javascript"></script>

    <script src="../Scripts/jquery/jquery-1.9.0.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        var setor = "";
        
        function setRota(codInterno) {
            FindControl("txtCodRota", "input").value = codInterno;
            loadRota();
        }

        function loadRota() {
            var retorno = CadMarcarPecaProducao.GetRota(FindControl("txtCodRota", "input").value).value;

            if (retorno == null) {
                alert("É necessário recarregar a página para continuar a leitura das peças.");
                return true;
            }

            retorno = retorno.split("|");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else {
                FindControl("hdfIdRota", "input").value = retorno[1];
                FindControl("lblDescrRota", "span").innerHTML = retorno[2];
            }
        }
        
        function alteraSetor(controle)
        {
            if (controle == null)
                return;
            
            for (i = 0; i < controle.options.length; i++)
                if (controle.options[i].value == controle.value)
                {
                    setor = controle.options[i].text;
                    break;
                }
            
            FindControl("chkPerda", "input").checked =
                FindControl("chkPerda", "input").checked &&
                (<%= (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != Glass.Data.Helper.DataSources.TipoReposicaoEnum.Peca).ToString().ToLower() %>).toString() == "true";

            var exibirTipoPerda = FindControl("chkPerda", "input").checked;
            document.getElementById("<%= tipoPerda.ClientID %>").style.display = exibirTipoPerda ? "" : "none";
        }
        
        function confirmar()
        {
            if (!validate())
                return false;
            
            var idRota = FindControl("hdfIdRota", "input").value;
            
            // Verifica se esta peça está sendo lida em um setor que exige que seja informada uma rota
            if (FindControl("hdfInformarRota", "input").value == "true" && 
                (idRota == "" || FindControl("txtCodRota", "input").value == ""))
            {
                alert("Informe a rota antes de marcar esta peça neste setor.");
                return false;
            }            
            
            FindControl("chkPerda", "input").checked =
                FindControl("chkPerda", "input").checked &&
                (<%= (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != Glass.Data.Helper.DataSources.TipoReposicaoEnum.Peca).ToString().ToLower() %>).toString() == "true";

            var isPerda = FindControl("chkPerda", "input").checked;

            if (FindControl("drpSetor", "select").value == "" && !isPerda) {
                alert("Informe o setor da peça na produção.");
                return false;
            }
            
            if (!isPerda)
                return confirm("Deseja marcar essa peça no setor '" + setor + "'?");
            else
            {
                // Verifica se a opção de retornar ao estoque foi marcada
                if (FindControl("drpRetornarEstoque", "select") != null && 
                    FindControl("drpRetornarEstoque", "select").value == "")
                {
                    alert("Informe se a peça deverá retornar ao estoque.");
                    return false;
                }
            
                return confirm("Deseja marcar essa peça como Perda?");
            }
        }
        
        function confirmarTodas()
        {
            if (!validate())
                return false;
            
            var idRota = FindControl("hdfIdRota", "input").value;
            
            // Verifica se esta peça está sendo lida em um setor que exige que seja informada uma rota
            if (FindControl("hdfInformarRota", "input").value == "true" && 
                (idRota == "" || FindControl("txtCodRota", "input").value == ""))
            {   
                alert("Informe a rota antes de marcar esta peça neste setor.");
                return false;
            }            
            
            FindControl("chkPerda", "input").checked =
                FindControl("chkPerda", "input").checked &&
                (<%= (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != Glass.Data.Helper.DataSources.TipoReposicaoEnum.Peca).ToString().ToLower() %>).toString() == "true";
            
            var isPerda = FindControl("chkPerda", "input").checked;
            
            if (!isPerda)
                return confirm("Deseja marcar todas as peças no setor '" + setor + "'?");
            else
            {
                // Verifica se a opção de retornar ao estoque foi marcada
                if (FindControl("drpRetornarEstoque", "select") != null &&
                    FindControl("drpRetornarEstoque", "select").value == "")
                {
                    alert("Informe se a peça deverá retornar ao estoque.");
                    return false;
                }
            
                return confirm("Deseja marcar todas as peças como Perda?");
            }
        }
        
        function validarTipoPerda(val, args)
        {
            var isPerda = document.getElementById("<%= chkPerda.ClientID %>");

            if (isPerda == null)
                return true;            
            
            isPerda.checked = isPerda.checked &&
                (<%= (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != Glass.Data.Helper.DataSources.TipoReposicaoEnum.Peca).ToString().ToLower() %>).toString() == "true";

            isPerda = isPerda != null ? isPerda.checked: true;
            args.IsValid = isPerda ? args.Value != "" : true;
        }
        
        function validarObsPerda(val, args)
        {
            var obrigarObs = <%= Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda.ToString().ToLower() %>;
            var isPerda = document.getElementById("<%= chkPerda.ClientID %>");
            
            isPerda = isPerda != null ? isPerda.checked && 
                (<%= (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != Glass.Data.Helper.DataSources.TipoReposicaoEnum.Peca).ToString().ToLower() %>).toString() : true;
            var outros = <%= (int)Glass.Data.DAL.TipoPerdaDAO.Instance.GetIDByNomeExato("Outros") %>;
            var isOutros = document.getElementById("<%= ctrlTipoPerda1.ClientID %>_drpTipoPerda").value == outros;
            args.IsValid = !obrigarObs || (isPerda && isOutros ? args.Value != "" : true);
        }
                
        function leuEtiqueta(txtNumEtiqueta) {
            if (txtNumEtiqueta == null || txtNumEtiqueta == undefined)
                return;
        
            txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
        }
        
         function imprimirRetalhos(etiqueta) {           
            openWindow(500, 700, '../Relatorios/RelEtiquetas.aspx?apenasPlano=false&numEtiqueta=' + etiqueta);
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Número do Pedido:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);" Width="70px"
                                TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                ErrorMessage="*" ValidationGroup="idPedido"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                TabIndex="2" ValidationGroup="idPedido" />
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
    </table>
    <table id="tbSaida" runat="server" style="width: 100%" visible="false">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblSubtitulo" runat="server" Text="Produtos do Pedido"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="separador">
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="situacao">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Setor da peça na produção:
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AutoPostBack="True" CausesValidation="True"
                                ValidationGroup="idPedido" DataSourceID="odsSetor" DataTextField="Descricao"
                                DataValueField="IdSetor" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged"
                                OnDataBound="drpSetor_DataBound">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPerda" runat="server" AutoPostBack="True" CausesValidation="True"
                                Text="Perda" ValidationGroup="idPedido" />
                        </td>
                    </tr>
                </table>
                <table id="tbRota" style="display: none;" runat="server">
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodRota" onblur="loadRota();" runat="server" Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblDescrRota" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkRota" runat="server" OnClientClick="openWindow(500, 700, '../Utils/SelRota.aspx'); return false;">
                                <img src="../Images/Pesquisar.gif" border="0" />
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table id="tbChapa" runat="server">
                    <tr>
                        <td style="font-weight: bold">
                            Chapa
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodChapa" runat="server" Width="120px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvChapa" runat="server" ErrorMessage="Informe a Chapa"
                                ControlToValidate="txtCodChapa" Display="None" Enabled="false"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr runat="server" id="tipoPerda">
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            Tipo de perda
                        </td>
                        <td align="left">
                            <uc1:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label4" runat="server" Text="Retornar peça ao estoque?" Visible="False"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpRetornarEstoque" runat="server" Visible="False">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Sim</asp:ListItem>
                                <asp:ListItem Value="2">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Motivo perda
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtObsPerda" runat="server" Rows="6" Columns="60" TextMode="MultiLine"></asp:TextBox>
                            <asp:CustomValidator ID="ctvObsPerda" runat="server" ClientValidationFunction="validarObsPerda"
                                ControlToValidate="txtObsPerda" Display="None" ErrorMessage="O motivo da perda não pode ser vazio."
                                ValidateEmptyText="True"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <uc2:ctrlRetalhoProducao ID="ctrlRetalhoProducao1" runat="server" />
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" 
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO" 
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
                    MaximumRowsParameterName="" SkinID="" StartRowIndexParameterName="">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="marcarPeca" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetBySetor"
                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:ValidationSummary ID="vsuSumario" runat="server" ShowMessageBox="True" ShowSummary="False"
                    DisplayMode="List" />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Num. Etiqueta:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server" Width="100px" onkeypress="if (isEnter(event)) return leuEtiqueta(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                TabIndex="2" ValidationGroup="idPedido" />
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
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdPedProducao"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdProdPed" EmptyDataText="Nenhum produto encontrado."
                    PageSize="30" OnRowCommand="grdProdutos_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:ImageButton ID="imbMarcar" runat="server" CommandName="MarcarTodas" ImageUrl="~/Images/ok.gif"
                                    OnClientClick="if (!confirmarTodas()) return false;" ToolTip="Marcar todas as peças" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:ImageButton ID="imbMarcar" runat="server" ToolTip="Marcar peça" CommandArgument='<%# Eval("NumEtiqueta") + ";" + Eval("Altura")+ ";" + Eval("Largura") %>'
                                                CommandName="Marcar" ImageUrl="~/Images/ok.gif" OnClientClick="if (!confirmar()) return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <asp:HiddenField ID="hdfDescr" runat="server" Value='<%# Eval("DescrProduto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="TotM2" HeaderText="Tot. m²" SortExpression="TotM2" DataFormatString="{0:N}" />
                        <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="NumEtiqueta" />
                        <asp:BoundField DataField="DescrSetor" HeaderText="Setor" SortExpression="DescrSetor" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdPedProducao" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetForMarcarPecaProntaCount"
                    SelectMethod="GetForMarcarPecaPronta" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtEtiqueta" Name="numEtiqueta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPerda" Name="perda" PropertyName="Checked" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfInformarRota" runat="server" />
                <asp:HiddenField ID="hdfIdRota" runat="server" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        alteraSetor(document.getElementById("<%= drpSetor.ClientID %>"));

        leuEtiqueta(FindControl("txtEtiqueta", "input"));
    </script>

</asp:Content>
