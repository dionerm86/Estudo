<%@ Page Title="Fotos" Language="C#" AutoEventWireup="true" CodeBehind="CadFotos.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadFotos" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function abrirJanela(arquivo, largura, altura)
        {
            openWindow(altura, largura, <%= "\"" + ResolveClientUrl("~/Handlers/LoadImage.ashx?path=") + "\"" %> + arquivo);
        }

        function tabelaPai(controle)
        {
            var retorno = controle;
            while (retorno.tagName != "TABLE")
                retorno = (retorno.parentElement) ? retorno.parentElement : retorno.parentNode;
                
            return retorno;
        }

        function alterarLegenda(idDiv, link)
        {
            var tabela = tabelaPai(link);
            var controle = FindControl(idDiv, "div", tabela);
            var legenda = FindControl("lblLegenda", "span", tabela);
            legenda.style.display = "none";
            controle.style.display = "block";
            
            return false;
        }

        function cancelarEdicao(idDiv, link)
        {
            var pai = tabelaPai(link);
            pai = (pai.parentElement) ? pai.parentElement : pai.parentNode;
            var tabela = tabelaPai(pai);
            var controle = FindControl(idDiv, "div", tabela);
            var legenda = FindControl("lblLegenda", "span", tabela);
            legenda.style.display = "block";
            controle.style.display = "none";
            
            return false;
        }

        var numeroFotos = 10;

        function adicionarFoto()
        {
            for (a = 1; a <= numeroFotos; a++)
            {
                var arquivo = FindControl("arquivo" + a, "tr");
                if (arquivo.style.display == "none")
                {
                    arquivo.style.display = "";
                    break;
                }
                else
                    FindControl("imagens" + a, "td").style.visibility = "hidden";
            }
        }

        function removerFoto()
        {
            for (a = numeroFotos; a >= 1; a--)
            {
                var arquivo = FindControl("arquivo" + a, "tr");
                if (arquivo.style.display == "")
                {
                    arquivo.style.display = "none";
                    FindControl("fluFoto" + a, "input").value = "";
                    FindControl("txtDescricao" + a, "input").value = "";
                    
                    if (a > 1)
                        FindControl("imagens" + (a - 1), "td").style.visibility = "";
                        
                    break;
                }
            }
        }

        function validarFoto(val, args)
        {
            var numFoto = val.id.substr(7);
            if (FindControl("arquivo" + numFoto, "tr").style.display == "")
                args.IsValid = args.Value != "";
            else
                args.IsValid = true;
        }
    </script>

    <table style="width: 100%;">
        <tr id="trTitle1" runat="server">
            <td align="center" class="subtitle1">
                Novo Arquivo
            </td>
        </tr>
        <tr id="trTitle2" runat="server">
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr id="trCadastro" runat="server">
            <td align="center">
                <table cellpadding="2" cellspacing="0" runat="server" id="tblMultFlu">
                    <tr id="Tr1" runat="server">
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label21" runat="server" Text="Arquivos"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto" runat="server" multiple="true" />
                            <asp:CustomValidator ID="ctvFoto" runat="server" ErrorMessage="Arquivos: Busque um arquivo a ser salvo."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserirMult"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label22" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" ToolTip="A descrição inserida será a mesma para todos os arquivos."
                                MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnInserirMult" runat="server" Text="Inserir" OnClick="btnInserirMult_Click"
                                            ValidationGroup="inserirMult" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnFecharMult" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                                    </td>
                                </tr>
                            </table>
                            <asp:ValidationSummary ID="usuErros" runat="server" ShowMessageBox="True" ShowSummary="False"
                                ValidationGroup="inserirMult" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="2" cellspacing="0" runat="server" id="tabelaCrud">
                    <tr id="arquivo1" runat="server">
                        <td id="imagens1" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Arquivo 1"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto1" runat="server" />
                            <asp:CustomValidator ID="ctvFoto1" runat="server" ErrorMessage="Arquivo 1: Busque um arquivo a ser salvo."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto1" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label1" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao1" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo2" runat="server" style="display: none">
                        <td id="imagens2" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label3" runat="server" Text="Arquivo 2"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto2" runat="server" />
                            <asp:CustomValidator ID="ctvFoto2" runat="server" ErrorMessage="Arquivo 2: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto2" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label4" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao2" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo3" runat="server" style="display: none">
                        <td id="imagens3" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label5" runat="server" Text="Arquivo 3"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto3" runat="server" />
                            <asp:CustomValidator ID="ctvFoto3" runat="server" ErrorMessage="Arquivo 3: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto3" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label6" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao3" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo4" runat="server" style="display: none">
                        <td id="imagens4" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label7" runat="server" Text="Arquivo 4"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto4" runat="server" />
                            <asp:CustomValidator ID="ctvFoto4" runat="server" ErrorMessage="Arquivo 4: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto4" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label8" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao4" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo5" runat="server" style="display: none">
                        <td id="imagens5" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label9" runat="server" Text="Arquivo 5"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto5" runat="server" />
                            <asp:CustomValidator ID="ctvFoto5" runat="server" ErrorMessage="Arquivo 5: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto5" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label10" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao5" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo6" runat="server" style="display: none">
                        <td id="imagens6" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label11" runat="server" Text="Arquivo 6"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto6" runat="server" />
                            <asp:CustomValidator ID="ctvFoto6" runat="server" ErrorMessage="Arquivo 6: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto6" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label12" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao6" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo7" runat="server" style="display: none">
                        <td id="imagens7" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label13" runat="server" Text="Arquivo 7"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto7" runat="server" />
                            <asp:CustomValidator ID="ctvFoto7" runat="server" ErrorMessage="Arquivo 7: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto7" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label14" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao7" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo8" runat="server" style="display: none">
                        <td id="imagens8" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label15" runat="server" Text="Arquivo 8"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto8" runat="server" />
                            <asp:CustomValidator ID="ctvFoto8" runat="server" ErrorMessage="Arquivo 8: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto8" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label16" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao8" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo9" runat="server" style="display: none">
                        <td id="imagens9" align="right">
                            <img src="../Images/Insert.gif" title="Adicionar" onclick="adicionarFoto()" style="cursor: pointer" />
                            <img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()" style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label17" runat="server" Text="Arquivo 9"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto9" runat="server" />
                            <asp:CustomValidator ID="ctvFoto9" runat="server" ErrorMessage="Arquivo 9: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto9" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label18" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao9" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="arquivo10" runat="server" style="display: none">
                        <td id="imagens10" align="right">
                            &nbsp;<img src="../Images/ExcluirGrid.gif" title="Remover" onclick="removerFoto()"
                                style="cursor: pointer" />
                        </td>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label19" runat="server" Text="Arquivo 10"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:FileUpload ID="fluFoto10" runat="server" />
                            <asp:CustomValidator ID="ctvFoto10" runat="server" ErrorMessage="Arquivo 10: Busque um arquivo a ser salvo (ou remova a linha de cadastro do arquivo)."
                                ClientValidationFunction="validarFoto" ControlToValidate="fluFoto10" Display="None"
                                ValidateEmptyText="True" ValidationGroup="inserir"></asp:CustomValidator>
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="Label20" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao10" runat="server" MaxLength="200" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnInserir" runat="server" Text="Inserir" OnClick="btnInserir_Click"
                                            ValidationGroup="inserir" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                                    </td>
                                </tr>
                            </table>
                            <asp:ValidationSummary ID="vsuErros" runat="server" ShowMessageBox="True" ShowSummary="False"
                                ValidationGroup="inserir" />
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
            <td align="center" class="subtitle1">
                <%= GetSubtitle() %>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Table ID="tbFotos" runat="server">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
