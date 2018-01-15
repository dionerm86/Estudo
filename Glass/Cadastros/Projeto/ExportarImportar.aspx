<%@ Page Title="Exportar/Importar/Duplicar Modelo de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ExportarImportar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.ExportarImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        var tipo = 1;
        function alteraTipo()
        {
            tipo = FindControl("rblTipo", "table").getElementsByTagName("input");
            for (i = 0; i < tipo.length; i++)
                if (tipo[i].type == "radio" && tipo[i].checked)
                {
                    tipo = tipo[i].value;
                    break;
                }
            
            document.getElementById("exportar").style.display = tipo == 1 ? "" : "none";
            document.getElementById("importar").style.display = tipo == 2 ? "" : "none";
            document.getElementById("duplicar").style.display = tipo == 3 ? "" : "none";
        }

        function adicionar()
        {
            if (tipo == 2)
                return;

            var compNome = tipo == 1 ? "Exp" : "Dup";
            var codigo = FindControl("txtCodigo" + compNome, "input");
            var resposta = ExportarImportar.GetDadosProjetoModelo(codigo.value).value.split(';');

            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            setModelo(resposta[1], codigo.value.toUpperCase(), resposta[2], resposta[3], resposta[4]);
            codigo.value = "";
            codigo.focus();
        }

        function setModelo(idProjetoModelo, codigo, grupo, descricao, espessura)
        {
            if (tipo == 2)
                return;
            
            var compNome = tipo == 1 ? "Exp" : "Dup";
            var campoCodigos = FindControl("hdfIdsProjetosModelos" + compNome, "input");
            var nomeTabela = "tbProjetos" + compNome;
            
            var existentes = campoCodigos.value.split(',');
            for (i = 0; i < existentes.length; i++)
                if (existentes[i] == idProjetoModelo)
                    return;

            if (tipo == 1 && ExportarImportar.PodeAdicionar(idProjetoModelo).value != "true")
                return;
            
            addItem(new Array(codigo, grupo, descricao, espessura), new Array("Código", "Grupo", "Descrição", "Espessura"), nomeTabela,
                idProjetoModelo, campoCodigos.id, null, null, null, false);
        }

        function validaExportar()
        {
            if (FindControl("hdfIdsProjetosModelosExp", "input").value.length == 0)
            {
                alert("Selecione pelo menos 1 modelo de projeto para exportar.");
                return false;
            }

            return true;
        }

        function validaImportar()
        {
            if (FindControl("fluArquivo", "input").value == "")
            {
                alert("Indique o arquivo que será importado.");
                return false;
            }
            
            document.getElementById("loading").style.display = "";
            return true;
        }
        
        function exportar(idsProjetosModelo, semFolgas)
        {
            document.getElementById("loading").style.display = "";
            redirectUrl("../../Handlers/ExportarProjeto.ashx?semFolgas=" + semFolgas + "&idsProjetoModelo=" + idsProjetosModelo);
            document.getElementById("loading").style.display = "none";
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" OnClick="alteraTipo()">
                    <asp:ListItem Selected="True" Value="1">Exportar</asp:ListItem>
                    <asp:ListItem Value="2">Importar</asp:ListItem>
                    <asp:ListItem Value="3">Duplicar</asp:ListItem>
                </asp:RadioButtonList>
                <br />
            </td>
        </tr>
        <tr id="exportar">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigoExp" runat="server" Width="50px"
                                onkeydown="if (isEnter(event)) { adicionar(); return false }"></asp:TextBox>
                            <asp:ImageButton ID="imbAddExp" runat="server" OnClientClick="adicionar(); return false"
                                ImageUrl="~/Images/Insert.gif" Width="16px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnBuscarExp" runat="server" Text="Buscar modelos"
                                OnClientClick="openWindow(600, 800, 'SelModeloExportar.aspx'); return false" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdsProjetosModelosExp" runat="server" />
                <br />
                <br />
                <table id="tbProjetosExp"></table>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkExportarSemFolgas" runat="server" Text="Exportar sem folgas" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar"
                    OnClientClick="validaExportar();" onclick="btnExportar_Click" />
            </td>
        </tr>
        <tr id="importar" style="display: none">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Arquivo para importação
                        </td>
                        <td>
                            <asp:FileUpload ID="fluArquivo" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblTamanhoMaximo" runat="server" Text="Tamanho máximo do arquivo: "></asp:Label>
                <br /><br />
                <asp:Label ID="lblSubtitulo" runat="server" Text="Selecione os itens que serão importados"></asp:Label>
                <br /><br />
                <asp:CheckBox ID="cbxArquivoMesa" runat="server" Text="Arquivo de mesa" Checked="true" />
                <asp:CheckBox ID="cbxFlag" runat="server" Text="Flag" Checked="true" />
                <asp:CheckBox ID="cbxRegraValidacao" runat="server" Text="Regra de validação" Checked="true" />
                <asp:CheckBox ID="cbxFormulaExpressaoCalculo" runat="server" Text="Fórmula de expressão de cálculo" Checked="true" />
                <br /><br />
                <asp:CheckBox ID="cbxSubstituirProjetoModeloExistente" runat="server" Text="Substituir Projeto Modelo Existente" Checked="true" />
                <br /><br />
                <asp:Button ID="btnImportar" runat="server" Text="Importar" 
                    onclick="btnImportar_Click" OnClientClick="if (!validaImportar()) return false" />
            </td>
        </tr>
        <tr id="duplicar" style="display: none">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigoDup" runat="server" Width="50px"
                                onkeydown="if (isEnter(event)) { adicionar(); return false }"></asp:TextBox>
                            <asp:ImageButton ID="imbAddDup" runat="server" OnClientClick="adicionar(); return false"
                                ImageUrl="~/Images/Insert.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnBuscarDup" runat="server" Text="Buscar modelos"
                                OnClientClick="openWindow(600, 800, 'SelModeloExportar.aspx'); return false" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdsProjetosModelosDup" runat="server" />
                <br />
                <br />
                <table id="tbProjetosDup"></table>
                <br />
                <table>
                    <tr>
                        <td align="right">
                            Grupo de projeto para onde os projetos duplicados irão
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" 
                                DataSourceID="odsGrupoModelo" DataTextField="Descricao" 
                                DataValueField="IdGrupoModelo">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Texto que será acrescido ao código dos projetos duplicados
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtFinalCodigo" runat="server" Width="80px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnDuplicar" runat="server" Text="Duplicar"
                    OnClientClick="validaDuplicar();" onclick="btnDuplicar_Click" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" 
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.GrupoModeloDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <img id="loading" src="../../Images/Load.gif" style="display: none" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        alteraTipo();

        switch (tipo)
        {
            case 1:
                FindControl("txtCodigoExp", "input").focus();
                break;

            case 2:
                FindControl("fluArquivo", "input").focus();
                break;

            case 3:
                FindControl("txtCodigoDup", "input").focus();
                break;
        }
    </script>
</asp:Content>

