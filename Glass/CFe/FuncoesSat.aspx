<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="FuncoesSat.aspx.cs" 
    Inherits="Glass.UI.Web.CFe.FuncoesSat" Title="Funções do Aparelho SAT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript" >
        function ExibeLoad(exibir)
        {
            if (exibir)
            {
                bloquearPagina();
                desbloquearPagina(false);
            }
            else
                desbloquearPagina(true);
        }
        
        function alterarCodAtivacao(botao)
        {
            if(document.getElementById('divCamposSenha').style.display == "none")
            {
                document.getElementById('divCamposSenha').style.display = "";
                botao.value = "Ocultar";
            }
            else
            {
                document.getElementById('divCamposSenha').style.display = "none";
                botao.value = "Executar";
            }
        }
        
        function trocarSenha()
        {
            try
            {               
                ExibeLoad(true);
                
                var codigoEmergencia = FindControl("chkCodEmergencia", "input").checked;
                var senhaAtual = FindControl("txtSenhaAtual", "input").value;
                var novaSenha = FindControl("txtSenhaNova", "input").value;
                var confNovaSenha = FindControl("txtSenhaNovaConfirm", "input").value;

                if(senhaAtual == "")
                {
                    ExibeLoad(false);
                    alert("Informe a senha atual do aparelho (código de ativação).");
                    return;
                }
                
                if(codigoEmergencia == false && (novaSenha == "" || confNovaSenha == ""))                
                {
                    ExibeLoad(false);
                    alert("Informe a nova senha e a confirmação da nova senha.");
                    return;
                }
                        
                if(codigoEmergencia == false && novaSenha != confNovaSenha)
                {
                    ExibeLoad(false);
                    alert("A confirmação da senha não confere com a nova senha digitada.");
                    return;
                }                
                
                if(!confirm("Deseja realmente alterar o código de ativação do aparelho?"))
                    return;                
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_FuncoesSat.ObterNumeroSessao().value;
                var codigoAtivacao = CFe_FuncoesSat.ObterCodigoAtivacao().value;
                var retorno = active.TrocarCodigoDeAtivacao(parseInt(numSessao), senhaAtual, codigoAtivacao, novaSenha, confNovaSenha).split('|');
                
                ExibeLoad(false);
                alert(retorno[2]);            
            }
            catch(e)
            {
                ExibeLoad(false);
                alert(e);
            }             
        }
        
        function atualizarSB()
        {
            try
            {
                ExibeLoad(true);
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_FuncoesSat.ObterNumeroSessao().value;
                var codigoAtivacao = CFe_FuncoesSat.ObterCodigoAtivacao().value;
                var retorno = active.AtualizarSoftwareSAT(parseInt(numSessao), codigoAtivacao).split('|');
                
                ExibeLoad(false);
                alert(retorno[2]);            
            }
            catch(e)
            {
                ExibeLoad(false);
                alert(e);
            }            
        }
        
        function bloquearAparelho(desativacao)
        {
            try
            {
                var mensagem = "Deseja realmente bloquear o aparelho?";
            
                if(desativacao)
                    mensagem = "Deseja realmente desativar o aparelho?";
                 
                if(!confirm(mensagem))
                    return;
                
                ExibeLoad(true);
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_FuncoesSat.ObterNumeroSessao().value;
                var codigoAtivacao = CFe_FuncoesSat.ObterCodigoAtivacao().value;
                var retorno = active.BloquearSAT(parseInt(numSessao), codigoAtivacao).split('|');
                
                ExibeLoad(false);
                alert(retorno[2]);            
            }
            catch(e)
            {
                ExibeLoad(false);
                alert(e);
            }              
        }
        
        function desbloquearAparelho()
        {
            try
            {
                var mensagem = "Deseja realmente desbloquear o aparelho?";         
                if(!confirm(mensagem))
                    return;
                
                ExibeLoad(true);
                
                var active = new ActiveXObject("SyncAcessaSat.acxAcessaSat");
                var numSessao = CFe_FuncoesSat.ObterNumeroSessao().value;
                var codigoAtivacao = CFe_FuncoesSat.ObterCodigoAtivacao().value;
                var retorno = active.DesbloquearSAT(parseInt(numSessao), codigoAtivacao).split('|');
                
                ExibeLoad(false);
                alert(retorno[2]);            
            }
            catch(e)
            {
                ExibeLoad(false);
                alert(e);
            } 
        }         
    </script>
    
    <div>
        <div style="margin:20px auto;">
            <div id="divAtualizarSB">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblAtualizarSB" runat="server" Text="Atualizar Software do Aparelho:" Width="215px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnAtualizarSB" runat="server" Text="Executar" OnClientClick="atualizarSB(); return false;" />
                </div>
            </div>
            <div id="divVincularSAT" style="margin-top: 10px;">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblVincular" runat="server" Text="Vincular SAT ao WebGlass:" Width="215px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnVincular" runat="server" Text="Executar" OnClientClick="vincularAparelho(); return false;" />
                </div>
            </div>
            <div id="divAlterarCodAtivacao" style="margin-top: 10px;">
                <div>
                    <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                        <asp:Label ID="lblTrocarCodAtivacao" runat="server" Text="Alterar Código de Ativação:" Width="215px" />
                    </div>
                    <div style="display: inline;">
                        <asp:Button ID="btnOcultarCamposSenha" runat="server" Text="Executar" OnClientClick="alterarCodAtivacao(this); return false;" />
                    </div>
                </div>
                <div id="divCamposSenha" style="display:none; margin-top:10px;">
                    <fieldset id="fdsTrocaSenha" title="Trocar Senha" style="width: 400px;">
                        <legend>Trocar Senha</legend>
                        <div>
                            <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                                <asp:Label ID="lblSenhaAtual" runat="server" Text="Senha Atual:" Width="150px" />
                            </div>
                            <div style="display: inline;">
                                <asp:TextBox ID="txtSenhaAtual" runat="server" TextMode="Password" />
                            </div>
                        </div>
                        <div>
                            <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                                <asp:Label ID="lblSenhaNova" runat="server" Text="Nova Senha:" Width="150px" />
                            </div>
                            <div style="display: inline;">
                                <asp:TextBox ID="txtSenhaNova" runat="server" TextMode="Password" />
                            </div>
                        </div>
                        <div>
                            <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                                <asp:Label ID="lblConfirmSenha" runat="server" Text="Repita a Nova Senha:" Width="150px" />
                            </div>
                            <div style="display: inline;">
                                <asp:TextBox ID="txtSenhaNovaConfirm" runat="server" TextMode="Password" />
                            </div>
                        </div>
                        <div style="text-align:left; margin-left: 14px;">
                            <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                                <asp:Label ID="lblCodEmergencia" runat="server" Text="Utilizar código de emergência" AssociatedControlID="chkCodEmergencia"/>
                            </div>
                            <div style="display: inline;">
                                <asp:CheckBox ID="chkCodEmergencia" runat="server" ToolTip="Utilize esta opção caso tenha esquecido o código de ativação cadastrado no aparelho SAT." />
                            </div>
                        </div>
                        <div style="margin-top:5px;">
                            <asp:Button ID="btnAlterarSenha" runat="server" Text="Salvar" OnClientClick="trocarSenha(); return false;" />
                        </div>
                    </fieldset>
                </div>
            </div>            
            <div id="divBloquearSAT" style="margin-top: 10px;">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblBloquearSAT" runat="server" Text="Bloquear Aparelho:" Width="215px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnBloqAparelho" runat="server" Text="Executar" OnClientClick="bloquearAparelho(false); return false;" />
                </div>
            </div>
            <div id="divDesbloqAparelho" style="margin-top: 10px;">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblDesbloq" runat="server" Text="Desbloquear Aparelho:" Width="215px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnDesbloqAparelho" runat="server" Text="Executar" OnClientClick="desbloquearAparelho(); return false;" />
                </div>
            </div>
            <div id="divDesativarAparelho" style="margin-top: 10px;">
                <div style="display: inline; font-size: small; color: #0066FF; text-align: right;">
                    <asp:Label ID="lblDesativar" runat="server" Text="Desativar Aparelho:" Width="215px" />
                </div>
                <div style="display: inline;">
                    <asp:Button ID="btnDesativar" runat="server" Text="Executar" OnClientClick="bloquearAparelho(true); return false;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

