<%@ Page Title="" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="DadosOtimizacao.aspx.cs" Inherits="Glass.UI.Web.Otimizacao.Views.DadosOtimizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
<style>
.ellipse{
	float: left;
}
.page_navigation , .alt_page_navigation{
	padding-bottom: 10px;
	margin-left: 43.2px;
}

.page_navigation a, .alt_page_navigation a{
	padding:3px 5px;
	margin:2px;
	color:white;
	text-decoration:none;
	float: left;
	font-family: Tahoma;
	font-size: 12px;
	background: #3baae3 url('../../Style/jquery/cupertino/images/ui-bg_glass_50_3baae3_1x400.png') 50% 50% repeat-x;
}
.active_page{
	background-color:white !important;
	font-weight:bold;
	color:black !important;
}	

.content, .alt_content{
	color: black;
}

.content li, .alt_content li, .content > p{
	padding: 5px
}
</style>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-2.0.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/otimizacao/bundle.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <div id="contentOtimizacao">
        <div id="accordion">
            <h3>
                Informações do Projeto</h3>
            <div id="infoProjeto">
                <div class="tituloRed ui-widget-header">
                    Peças</div>
                <div id="listaPecas">
                    <ul runat="server" id="infoUl">
                    </ul>
                    <input type="hidden" id="areaTotalPecas" identidade="areaTotalPecas" value="0" runat="server" />
                </div>
                <div class="titulo ui-widget-header">
                </div>
                <div id="botaoOtimizar">
                    <input type="button" class="t-button" id="otimizarButton" value="Otimizar" />
                    <input type="button" class="t-button" id="limparButton" value="Limpar" />
                </div>
                <div class="titulo ui-widget-header">
                    Chapas</div>
                <div id="listaChapas">
                    <asp:GridView GridLines="None" ID="grdChapaVidro" runat="server" AutoGenerateColumns="False"
                        DataKeyNames="IdChapaVidro" DataSourceID="odsChapaVidro" CssClass="gridStyle"
                        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                        EmptyDataText="Não há chapa de vidro cadastrada.">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <input type="checkbox" runat="server" id="cbSeleciona"
                                        value='<%# Eval("IdChapaVidro") + ";" + Eval("Altura") + ";" + Eval("Largura") + ";" + Eval("Quantidade") %>'
                                        class="chapaVidroCheckBox" identidade='<%# Eval("IdChapaVidro") + "_CheckBox" %>' />
                                </ItemTemplate>
                                <HeaderTemplate>
                                    <input type="checkbox" id="selecionaTodos" />
                                </HeaderTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DescrProduto" HeaderText="Descrição" SortExpression="DescrProduto">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Quantidade" HeaderText="Estoque" SortExpression="Quantidade">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                        </Columns>
                        <PagerStyle CssClass="pgr"></PagerStyle>
                        <EditRowStyle CssClass="edit"></EditRowStyle>
                        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    </asp:GridView>
                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsChapaVidro" runat="server" SelectMethod="ObterListaChapasOtimizacao"
                        TypeName="Glass.Data.DAL.ChapaVidroDAO" >
                        <SelectParameters>
                            <asp:QueryStringParameter Name="idsPedido" QueryStringField="pedidos" Type="String" />
                            <asp:QueryStringParameter Name="idsOrcamento" QueryStringField="orcamentos" Type="String" />
                        </SelectParameters>
                    </colo:VirtualObjectDataSource>
                </div>
                <div class="titulo ui-widget-header" id="tituloDadosOtimizacao" style="display:none">
                    Dados da Otimização</div>
                <div id="dadosOtimizacao"  style="display:none">
                    
                </div>
            </div>          
            <h3>Peças Removidas</h3>
            <div id="pecasRemovidas">
            
                <div id="pecasRemovidasListBox" style="height: 400px; font-size: 10px" title="Selecione uma peça com o botão direito do mouse e clique em remover.">
                </div>
                <input type="hidden" id="hdfPecas" />
                <input type="hidden" id="ViewState" />
            </div>
        </div>
        <div id="result">
            <div id="error" style="display: none">
                <p style='color: #FF0000; font-size: 16px; padding-left: 20px; background-color: #FFFF00'>
                </p>
            </div>
            <div id="tabs" style="display: none; height: 98%; padding:5px">
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <div class="contextMenu" id="myMenu1" style="display: none">
        <ul>
            <li id="rotate">
                <img src="../../Images/rotate_context.png" alt="" />
                Rotacionar</li>
            <li id="remove">
                <img src="../../Images/remove_context.png" alt="" />
                Remover</li>
            <li id="delete">
                <img src="../../Images/delete_context.gif" alt="" />
                Excluir</li>
        </ul>
    </div>
    <div id="imagem"></div>
</asp:Content>
