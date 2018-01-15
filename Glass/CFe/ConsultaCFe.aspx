<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="ConsultaCFe.aspx.cs" Inherits="Glass.UI.Web.CFe.ConsultaCFe" Title="Módulos de Consulta ao Aparelho SAT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    
    <object name="acxAcessaSat" style='display:none' id='acxAcessaSat' classid='CLSID:1FC0D50A-4803-4f97-94FB-2F41717F558D' codebase='AcessaSatInstall.cab#version=1,0,0,0'></object>

    <script id="jsConsultaSAT" type="text/javascript" >
        function exibirLoad(exibir)
        {
            if (exibir)
            {
                bloquearPagina();
                desbloquearPagina(false);
            }
            else
                desbloquearPagina(true);
        }
        
        function consultarSAT()
        {
            try
            {
                exibirLoad(true);
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_ConsultaCFe.ObterNumeroSessao().value;
                var retorno = active.ConsultarSAT(parseInt(numSessao)).split('|');
                
                exibirLoad(false);
                alert(retorno[2]);            
            }
            catch(e)
            {
                exibirLoad(false);
                alert(e);
            }
        }
        
        function consutarStatusOperacional()
        {
            try
            {
                exibirLoad(true);
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_ConsultaCFe.ObterNumeroSessao().value;                
                var codigoAtivacao = CFe_ConsultaCFe.ObterCodigoAtivacao().value;
                var retorno = active.ConsultarStatusOperacional(numSessao, codigoAtivacao);
                
                var lstRetorno = retorno.split('|');
                
                exibirLoad(false);
                FindControl("hidStatusOperacional", "input").value = retorno;
                
                if(lstRetorno.length > 5)                  
                {
                    return true;
                }
                else
                {
                    alert("Não foi possível obter o status operacional do aparelho.");
                    return false;
                }
            }
            catch(e)
            {
                exibirLoad(false);
                alert("Não foi possível obter o status operacional do aparelho.");
                return false;
            }
        }
    </script>
    
    <div>
        <div style="margin:20px auto;">
            <div id="divConsultarSAT">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblStatus" Text="Verificar Status SAT:" runat="server" Width="170px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnConsultarSat" OnClientClick="consultarSAT(); return false;" runat="server" Text="Consultar SAT" />
                </div>
            </div>
            <div id="divStatusOperacional" style="margin-top: 10px;">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblStatusOperacional" Text="Status Operacional SAT:" runat="server" Width="170px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnStatusOperacional" OnClientClick="return consutarStatusOperacional();" OnClick="btnStatusOperacional_Click" runat="server" Text="Consultar SAT" />
                </div>
            </div>
        </div>
        <div id="divDetailsView">
            <asp:DetailsView ID="dtvStatusOp" runat="server" AutoGenerateRows="false" DefaultMode="Edit" GridLines="None">
                <Fields>
                    <asp:TemplateField>
                        <EditItemTemplate>
                            <table>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblNumSerie" runat="server" Text="Número de série:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtNumSerie" runat="server" Text='<%# Bind("NumeroSerieSAT") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblTipoLan" runat="server" Text="Tipo LAN:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TipoLAN") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblIP" runat="server" Text="Número IP:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtAparelho" runat="server" Text='<%# Bind("IpAparelho") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblMac" runat="server" Text="Endereço MAC:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtMac" runat="server" Text='<%# Bind("MacAdress") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblSubRede" runat="server" Text="Máscara de Sub Rede:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtMascSubRede" runat="server" Text='<%# Bind("MascaraSubRede") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblGateway" runat="server" Text="Gateway:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtGateway" runat="server" Text='<%# Bind("Gateway") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblDNS1" runat="server" Text="DNS 1:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtDNS1" runat="server" Text='<%# Bind("DNS1") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblDNS2" runat="server" Text="DNS 2:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtDNS2" runat="server" Text='<%# Bind("DNS2") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblStatusRede" runat="server" Text="Status da Rede:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("StatusRede") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblNivelBateria" runat="server" Text="Nível de Bateria:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtNivelBateria" runat="server" Text='<%# Bind("NivelBateria") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblMemTotal" runat="server" Text="Memória Total:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtMemTotal" runat="server" Text='<%# Bind("MemoriaTotal") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblMemUsada" runat="server" Text="Memória Utilizada:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtMemUsada" runat="server" Text='<%# Bind("MemoriaUsada") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblDataAparelho" runat="server" Text="Data do Aparelho:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtDataAparelho" runat="server" Text='<%# Bind("DataAtualAparelho") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblVersaoSB" runat="server" Text="Versão do Software:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtVersaoSB" runat="server" Text='<%# Bind("VersaoSoftwareBasico") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblVersaoTabInf" runat="server" Text="Versão Tab. Inf.:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("VersaoLayoutTabInf") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblUltimoCFeEmit" runat="server" Text="Último CFe Emitido:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("UltimoCFeEmitido") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblPrimCfeArmaz" runat="server" Text="Primeiro CFe Armazenado:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtPrimCfeArmaz" runat="server" Text='<%# Bind("PrimeiroCFeArmazenado") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblUltCfeArmaz" runat="server" Text="Último CFe Armazenado:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtUltCfeArmaz" runat="server" Text='<%# Bind("UltimoCFeArmazenado") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblUltTransm" runat="server" Text="Última Transmissão CFe's:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtUltTrans" runat="server" Text='<%# Bind("UltimaTransmissaoSEFAZ") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblUltCom" runat="server" Text="Última Comunicação SEFAZ:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtUltiCom" runat="server" Text='<%# Bind("UltimaComunicacaoSEFAZ") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblEmCertDig" runat="server" Text="Emissão Cert. Digital:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtEmCertDig" runat="server" Text='<%# Bind("EmissaoCertDigital") %>' ReadOnly="true" />
                                    </td>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblVencCertDig" runat="server" Text="Vencimento Cert. Digital:" />
                                    </td>
                                    <td class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtVencCertDig" runat="server" Text='<%# Bind("VencimentoCertDigital") %>' ReadOnly="true" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td class="dtvHeader">
                                        <asp:Label ID="lblStatOp" runat="server" Text="Status Operacional:" />
                                    </td>
                                    <td class="dtvAlternatingRow" colspan="3">
                                        <asp:TextBox ID="txtStatOp" runat="server" Text='<%# Bind("StatusOperacional") %>' ReadOnly="true" />
                                    </td>
                                </tr>
                            </table>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </div>
    </div>
    <div id="divHiddens">
        <asp:HiddenField ID="hidStatusOperacional" runat="server" Value="" />
    </div>
    <div id="divDataSources">
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsStatusOp" runat="server" SelectMethod="ConsultarStatusOperacional" TypeName="Glass.Data.DAL.CupomFiscalDAO">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>

