<%@ Page Title="Exportar/Importar Ferragem" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ExportarImportarFerragem.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.ExportarImportarFerragem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        // Por padrão, a tela é aberta com a opção de exportação.
        // tipo 1: exportação.
        // tipo 2: importação.
        var tipo = 1;

        // Método chamado ao selecionar a opção de exportação ou importação.
        function alterarTipo()
        {
            tipo = FindControl("rblTipo", "table").getElementsByTagName("input");

            // Verifica qual tipo foi selecionado.
            for (i = 0; i < tipo.length; i++)
            {
                if (tipo[i].type == "radio" && tipo[i].checked) {
                    tipo = tipo[i].value;
                    break;
                }
            }
            
            // Esconde ou exibe o código de exportação.
            document.getElementById("exportar").style.display = tipo == 1 ? "" : "none";
            // Esconde ou exibe o código de importação.
            document.getElementById("importar").style.display = tipo == 2 ? "" : "none";
        }

        // Método chamado ao buscar uma ferragem pelo nome, através do text box da tela.
        function adicionarFerragemPeloNome()
        {
            var nomeFerragem = FindControl("txtNomeFerragem", "input");

            // Caso a tela de importação esteja habilitada, não é possível adicionar ferragens, somente fazer o upload do arquivo de exportação.
            if (tipo == 2 || nomeFerragem == null || nomeFerragem.value == "")
            {
                alert("Ferragem não encontrada. Verifique se existe alguma ferragem cadastrada, no WebGlass, com o nome informado.");
                return false;
            }

            // resposta[0]: Ok/Erro
            // resposta[1]: idFerragem
            // resposta[2]: nomeFerragem
            // resposta[3]: nomeFabricanteFerragem
            // resposta[4]: situacao
            // resposta[5]: dataAlteracao
            // Obtém os dados da ferragem, com base no nome informado no text box.
            var retornoDadosFerragem = ExportarImportarFerragem.ObterDadosFerragem(nomeFerragem.value);
            
            // Verifica se ocorreu algum erro no método ajax.
            if (retornoDadosFerragem.error != null)
            {
                // Exibe, em um alerta, a mensagem de erro do ajax.
                alert(retornoDadosFerragem.error.description);

                // Altera o foco da tela para o text box do nome da ferragem.
                nomeFerragem.value = "";
                nomeFerragem.focus();

                return false;
            }

            // Recupera somente os dados da ferragem.
            var dadosFerragem = retornoDadosFerragem.value.split(';');

            // Verifica se ocorreu uma exceção, tratada, no método ajax.
            if (dadosFerragem[0] == "Erro")
            {
                // Exibe, em um alerta, a mensagem de erro.
                alert(dadosFerragem[1]);

                // Altera o foco da tela para o text box do nome da ferragem.
                nomeFerragem.value = "";
                nomeFerragem.focus();

                return false;
            }

            // Seta os dados da ferragem, recuperada, na tela.
            setarFerragem(dadosFerragem[1], dadosFerragem[2], dadosFerragem[3], dadosFerragem[4], dadosFerragem[5]);

            // Altera o foco da tela para o text box do nome da ferragem.
            nomeFerragem.value = "";
            nomeFerragem.focus();
        }

        // Seta os dados da ferragem na tela e no hidden field de ids de ferragem.
        function setarFerragem(idFerragem, nomeFerragem, nomeFabricanteFerragem, situacao, dataAlteracao)
        {
            // Caso a tela de importação esteja habilitada, não é possível adicionar ferragens, somente fazer o upload do arquivo de exportação.
            if (tipo == 2)
            {
                return false;
            }
            
            var campoCodigos = FindControl("hdfIdsFerragem", "input");
            var nomeTabela = "tbFerragens";
            var existentes = campoCodigos.value.split(',');

            // Verifica se a ferragem já foi adicionada na tela.
            for (i = 0; i < existentes.length; i++)
            {
                if (existentes[i] == idFerragem)
                {
                    return;
                }
            }
            
            // Adiciona uma nova linha no controle tbFerragens, com os dados da ferragem recuperada. Além disso, adiciona o id da ferragem no hidden field de ids de ferragem.
            addItem(new Array(nomeFerragem, nomeFabricanteFerragem, situacao, dataAlteracao), new Array("Ferragem", "Fabricante", "Situação", "Data Alteração"),
                nomeTabela, idFerragem, campoCodigos.id, null, null, null, false);
        }

        // Valida a exportação de ferragens.
        function validarExportacao()
        {
            // Vreifica se existem ids de ferragem no hidden field.
            if (FindControl("hdfIdsFerragem", "input") == null || FindControl("hdfIdsFerragem", "input").value.length == 0)
            {
                alert("Selecione pelo menos uma ferragem para exportar.");
                return false;
            }

            return true;
        }

        // Valida a importação de ferragens.
        function validarImportacao()
        {
            // Verifica se o arquivo foi carregado no controle.
            if (FindControl("fluArquivo", "input") == null || FindControl("fluArquivo", "input").value == "")
            {
                alert("Indique o arquivo que será importado.");
                return false;
            }
            
            // Exibe a tela Aguarde.
            document.getElementById("loading").style.display = "";

            return true;
        }
        
        // Exporta as ferragens, criando um arquivo compactado.
        function exportar(idsFerragem)
        {
            // Exibe a tela Aguarde.
            document.getElementById("loading").style.display = "";
            // Chama o handler que irá gerar o arquivo de exportação e compactá-lo.
            redirectUrl("../../Handlers/ExportarFerragem.ashx?idsFerragem=" + idsFerragem);
            // Esconde a tela Aguarde.
            document.getElementById("loading").style.display = "none";
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" OnClick="alterarTipo();">
                    <asp:ListItem Selected="True" Value="1">Exportar</asp:ListItem>
                    <asp:ListItem Value="2">Importar</asp:ListItem>
                </asp:RadioButtonList>
                <br />
            </td>
        </tr>
        <tr id="exportar">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNomeFerragem" runat="server" Text="Nome da Ferragem" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFerragem" runat="server" Width="50px" onkeydown="if (isEnter(event)) { adicionarFerragemPeloNome(); return false; }"></asp:TextBox>
                            <asp:ImageButton ID="imbAdd" runat="server" OnClientClick="adicionarFerragemPeloNome(); return false;" ImageUrl="~/Images/Insert.gif" Width="16px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar ferragens" OnClientClick="openWindow(600, 800, 'SelFerragemExportar.aspx'); return false;" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdsFerragem" runat="server" />
                <br />
                <br />
                <table id="tbFerragens"></table>
                <br />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClientClick="validarExportacao();" onclick="btnExportar_Click" />
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
                <asp:CheckBox ID="chkSubstituirFerragemExistente" runat="server" Text="Substituir Ferragem Existente" Checked="true" />
                <br /><br />
                <asp:Button ID="btnImportar" runat="server" Text="Importar" onclick="btnImportar_Click" OnClientClick="if (!validarImportacao()) return false;" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <img id="loading" src="../../Images/Load.gif" style="display: none" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">

        // Método chamado para ajustar a exibição inicial da tela.
        alterarTipo();

        // Define qual controle deve ter o foco, de acordo com o tipo selecionado.
        switch (tipo)
        {
            // Exportação.
            case 1:
                FindControl("txtCodigo", "input").focus();
                break;
            // Importação.
            case 2:
                FindControl("fluArquivo", "input").focus();
                break;
        }

    </script>
</asp:Content>

