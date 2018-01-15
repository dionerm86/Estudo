<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditarImagem.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.EditarImagem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <link href="<%= ResolveUrl("~") %>Style/EdicaoImagemProjeto/modelo.css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/funcoes.js"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/jquery.1.8.2.min.js"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/jquery.ui.core.min.js"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/jquery.ui.widget.min.js"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/jquery.ui.mouse.min.js"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/jquery.ui.draggable.min.js"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~") %>Style/EdicaoImagemProjeto/wColorPicker.css" />

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/wColorPicker.js"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~") %>Style/EdicaoImagemProjeto/wPaint.css" />

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/Utils.js"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~") %>Style/Geral.css" />
    <title>WebGlass</title>
</head>
<body style="cursor: crosshair;">

    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/EdicaoImagemProjeto/wPaint.js"></script>

    <br />
    <table cellpadding="0" cellspacing="0" style="width: 100%;">
        <tbody>
            <tr>
                <td class="subtitle" id="titulo" height="42px">
                    Editar Imagem do Projeto
                </td>
            </tr>
        </tbody>
    </table>
    <br />

    <script>
        $(window).load(function() {
            // Pesquisa de dados da imagem inicial
            var altura_imagem_inicial = $('#img_inicial').css('height');
            var caminho_imagem_inicial = $('#img_inicial').attr('src');
            var largura_imagem_inicial = $('#img_inicial').css('width');

            // Antes de inicializar o editor e carregar a imagem, redimensiona o editor para o tamanho da imagem
            $('#wPaint').css('height', altura_imagem_inicial);
            $('#wPaint').css('width', largura_imagem_inicial);

            // Inicializa o editor de imagem
            $('#wPaint').wPaint();

            // Carrega a imagem inicial no editor de imagem
            var obj_imagem_inicial = new Image();
            obj_imagem_inicial.src = caminho_imagem_inicial;
            obj_imagem_inicial.onload = function() {
                $("#wPaint").wPaint("image", caminho_imagem_inicial);
            }

            // Opção para abrir nova imagem
            $('#div_open').click(function() {
                $('.box_redimensionar_area_desenho').hide();

                if ($('.box_upload_imagem').is(':visible') == false) {
                    $('.box_upload_imagem').show();
                }
                else {
                    $('.box_upload_imagem').hide();
                }
            });

            // Opção para redimensionar a área de desenho
            $('#div_resize').click(function() {
                $('.box_upload_imagem').hide();

                if ($('.box_redimensionar_area_desenho').is(':visible') == false) {
                    $('.box_redimensionar_area_desenho').show();
                }
                else {
                    $('.box_redimensionar_area_desenho').hide();
                }
            });

            // Opção para salvar as edições na imagem
            $('#div_save').click(function () {

                // Chamado 14136.
                // Incluir um texto na imagem e clicar no botão para salvar a alteração, a alteração não estava sendo salva, porque
                // a caixa de texto ainda estava habilitada. Portanto, modificamos esta parte para sempre voltar à ferramenta padrão (pincel)
                // ao clicar em salvar, pois, dessa forma todas as alterações feitas serão salvas, não terá o risco de haver uma alteração
                // em aberto na tela esperando um clique fora para ser aplicada.
                $('#div_padrao').click();

                if (confirm("Deseja salvar alterações da imagem?")) {
                    var variaveis = location.search.split("?");
                    var parametros = variaveis[1].split("&");
                    var image_data = $('#wPaint').wPaint('image');

                    var request = $.ajax({
                        type: "POST",
                        url: "EditarImagem.aspx",
                        data: { base64: image_data, ajax: "SalvarImagem", idProjetoModelo: parametros[0].split("=")[1], //início data
                            idItemProjeto: parametros[1].split("=")[1], idPecaItemProj: parametros[2].split("=")[1],
                            item: parametros[3].split("=")[1]
                        }, //fim data
                        dataType: "json",
                        async: false
                    });

                    alert(request.responseText);
                }
            });

            // Opção abrir biblioteca
            $('#btnBiblioteca').click(function() {
                if (confirm("Deseja salvar imagem e abrir biblioteca?")) {
                    var variaveis = location.search.split("?");
                    var parametros = variaveis[1].split("&");
                    var image_data = $('#wPaint').wPaint('image');

                    var _idProjetoModelo = parametros[0].split("=")[1];
                    var _idItemProjeto = parametros[1].split("=")[1];
                    var _idPecaItemProj = parametros[2].split("=")[1];
                    var _item = parametros[3].split("=")[1];

                    $.ajax({
                        type: "POST",
                        url: "EditarImagem.aspx",
                        data: { base64: image_data, ajax: "AbrirBiblioteca", idProjetoModelo: _idProjetoModelo, //início data
                            idItemProjeto: _idItemProjeto, idPecaItemProj: _idPecaItemProj,
                            item: _item
                        }, //fim data
                        dataType: "json",
                        async: false
                    });

                    openWindow(500, 700, 'DesenhaProjeto1.aspx?idProjetoModelo=' + _idProjetoModelo + '&idItemProjeto=' + _idItemProjeto +
                        '&idPecaItemProj=' + _idPecaItemProj + '&item=' + _item);

                    closeWindow();
                    return false;
                }
            });

            // Opção para upload de imagem
            $('#file_imagem').change(function() {
                fnc_enviar_imagem(document.getElementById("hidden_serial").value);
            });

            // Opções para redimensionamento da área de desenho
            $('.change_size_canvas').click(function() {
                // Tamanho deslocamento
                var tamanho_deslocamento = 20;

                // Busca os dados da área de desenho atual
                var canvas_atual = document.getElementById("canvas");
                var ctx_atual = canvas_atual.getContext('2d');
                var altura_canvas_atual = parseInt(canvas_atual.height);
                var largura_canvas_atual = parseInt(canvas_atual.width);

                // ID da opção que foi clicada para redimensionamento
                var array_info_id = $(this).attr('id').split('_');

                // Verifica qual é o lado da área do desenho que deve ser redimensionada
                switch (array_info_id[1]) {
                    case 'bottom':
                        {
                            switch (array_info_id[2]) {
                                case 'decrease':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('height', altura_canvas_atual - tamanho_deslocamento);
                                        canvas_atual.height = altura_canvas_atual - tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;

                                case 'increase':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('height', altura_canvas_atual + tamanho_deslocamento);
                                        canvas_atual.height = altura_canvas_atual + tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;
                            }
                        }
                        break;

                    case 'left':
                        {
                            switch (array_info_id[2]) {
                                case 'decrease':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('width', largura_canvas_atual - tamanho_deslocamento);
                                        canvas_atual.width = largura_canvas_atual - tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, (tamanho_deslocamento * -1), 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;

                                case 'increase':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('width', largura_canvas_atual + tamanho_deslocamento);
                                        canvas_atual.width = largura_canvas_atual + tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, tamanho_deslocamento, 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;
                            }
                        }
                        break;

                    case 'right':
                        {
                            switch (array_info_id[2]) {
                                case 'decrease':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('width', largura_canvas_atual - tamanho_deslocamento);
                                        canvas_atual.width = largura_canvas_atual - tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;

                                case 'increase':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('width', largura_canvas_atual + tamanho_deslocamento);
                                        canvas_atual.width = largura_canvas_atual + tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, 0, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;
                            }
                        }
                        break;

                    case 'top':
                        {
                            switch (array_info_id[2]) {
                                case 'decrease':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('height', altura_canvas_atual - tamanho_deslocamento);
                                        canvas_atual.height = altura_canvas_atual - tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, (tamanho_deslocamento * -1), largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;

                                case 'increase':
                                    var novo_obj_img = new Image();
                                    novo_obj_img.onload = function() {
                                        $('#wPaint').css('height', altura_canvas_atual + tamanho_deslocamento);
                                        canvas_atual.height = altura_canvas_atual + tamanho_deslocamento;
                                        ctx_atual.clearRect(0, 0, largura_canvas_atual, altura_canvas_atual);
                                        ctx_atual.beginPath();
                                        ctx_atual.rect(0, 0, canvas_atual.width, canvas_atual.height);
                                        ctx_atual.fillStyle = 'white';
                                        ctx_atual.fill();
                                        ctx_atual.drawImage(novo_obj_img, 0, tamanho_deslocamento, largura_canvas_atual, altura_canvas_atual);
                                    }
                                    novo_obj_img.src = canvas_atual.toDataURL('image/png');
                                    break;
                            }
                        }
                        break;
                }
            });

            // Funções para fazer o textarea mudar sua posição na tela		
            $.widget('ui.moveable',
					{
					    options:
						{
						    width: "16px",
						    height: "16px"
						},

					    // initialization code goes here
					    _create: function() {
					        var element = this.element;
					        var elementPos = element.position();

					        // add a handle that is draggable
					        element.before("<img id='img_change_position_text' src='../../Images/EdicaoImagemProjeto/arrow.png'>");

					        var handle = element.prev();
					        var w = this.options.width;
					        var h = this.options.height;
					        handle.css({ width: w, height: h });
					        var x = handle.width() - 7;
					        var y = handle.height() - 7;
					        handle.css({ position: "absolute", top: elementPos.top - y, left: elementPos.left - x });


					        // make the handle draggable
					        handle.draggable(
							{
							    drag: function(event, ui) {
							        // move the element relative to handle
							        element.css({ position: "absolute", top: ui.position.top + y, left: ui.position.left + x });
							        handle.css("z-index", 1);
							    }
							});
					    },

					    // destructor code here
					    destroy: function() {
					        var handle = this.element.prev();
					        // remove the handle
					        handle.remove();
					    }
					});
        });

        // Envio de uma nova imagem pelo editor
        function fnc_enviar_imagem(serial) {
            if (document.getElementById("file_imagem").value != '') {
                document.getElementById("btnSalvarImagem").click();
            }
            else {
                alert('Favor informar uma imagem.');
            }
        }

        // Muda a operação conforme a página
        function fnc_operacao_imagem(tipo, serial) {
            document.location.href = 'edicao_imagem_serial.php?tipo=' + tipo + '&serial=' + serial;
        }	
	
		</script>

    <asp:Image ID="img_inicial" runat="server" Style="display: none;" />
    <form runat="server" name="form_edicao_imagem" method="post" enctype="multipart/form-data">
    <!-- INICIO CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <input id="hidden_contexto" name="hidden_contexto" type="hidden" value=".." />
    <input id="hidden_diretorio_imagem" name="hidden_diretorio_imagem" type="hidden"
        value="../../Upload/FigurasProjeto/1007.jpg" />
    <input id="hidden_extensao_imagem" name="hidden_extensao_imagem" type="hidden" value="png" />
    <input id="hidden_imagem_alterada" type="hidden" value="" />
    <input id="hidden_serial" name="hidden_serial" type="hidden" value="77943" />
    <input id="hidden_tipo_texto_selecionado" type="hidden" value="H" />
    <!-- FIM CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <!-- INICIO CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <input id="hidden_imagem_adicional_atual_selecionada" type="hidden" />
    <input id="hidden_imagem_adicional_anterior_selecionada" type="hidden" />
    <input id="hidden_quantidade_imagens_adicionadas" type="hidden" value="0" />
    <!-- FIM CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <!-- INICIO CORPO EDITOR E OPÇÃO PARA REDIMENSIONAR ÁREA DE DESENHO !-->
    <div id="wPaint" class="box_edicao_imagem" style="height: 198px; width: 424px;">
        <canvas id="canvas" width="424" height="198px" style="position: absolute; left: 0px;
            top: 0px;"></canvas>
    </div>
    <img id="img_canvas" style="display: none;" />
    <!-- INICIO OPÇÃO PARA VOLTAR !-->
    <div style="padding-left: 450px;">
        <input type="button" value="Fechar" onclick="closeWindow();" title="Clique aqui para fechar a tela" />
        <asp:Button runat="server" Text="Voltar imagem padrão" ID="VoltarImagem" OnClick="VoltarImagem_Click" />
        <input type="button" value="Itens da Biblioteca" id="btnBiblioteca" />
    </div>
    <!-- FIM OPÇÃO PARA VOLTAR !-->
    <!-- INICIO OPÇÃO PARA UPLOAD DE IMAGEM !-->
    <br><br>
    <label class="f_13 f_blue">
        <b>Selecione uma nova imagem para o projeto:</b></label><br>
    <input ID="file_imagem" name="file_imagem" type="file" runat="server">
    <!-- FIM OPÇÃO PARA UPLOAD DE IMAGEM !-->
    <!-- INICIO OPÇÃO PARA REDIMENSIONAR A ÁREA DE DESENHO !-->
        <%--<div class="box_redimensionar_area_desenho" style="display: none;">
    	<table align="center" cellpadding="0" cellspacing="1" width="120px">
        <tbody><tr>
          <td align="center" height="26px " colspan="3">
            <img id="img_top_increase" class="change_size_canvas" 
            src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_top_increase.png" style="cursor: pointer;" title="Aumentar parte de cima" />
            <img id="img_top_decrease" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_top_decrease.png" 
            style="cursor: pointer;" title="Diminuir parte de cima" />
          </td>
        </tr>
        <tr>
          <td align="center" width="26px">
            <img id="img_left_increase" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_left_increase.png" 
            style="cursor: pointer;" title="Aumentar esquerda" />
            <br>
            <img id="img_left_decrease" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_left_decrease.png" 
            style="cursor: pointer;" title="Diminuir esquerda" />
          </td>
          <td align="center">
            <img src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_square.png">
          </td>
          <td align="center" width="26px">
            <img id="img_right_increase" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_right_increase.png" 
            style="cursor: pointer;" title="Aumentar direita" />
            <br>
            <img id="img_right_decrease" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_right_decrease.png" 
            style="cursor: pointer;" title="Diminuir direita" />
          </td>
        </tr>
        <tr>
          <td align="center" height="26px " colspan="3">
            <img id="img_bottom_increase" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_bottom_increase.png" 
            style="cursor: pointer;" title="Aumentar parte de baixo" />
            <img id="img_bottom_decrease" class="change_size_canvas" src="<%= ResolveUrl("~") %>Images/EdicaoImagemProjeto/icon_bottom_decrease.png" 
            style="cursor: pointer;" title="Diminuir parte de baixo" />
          </td>
        </tr>
    	</tbody></table>
    </div>--%>
    <!-- FIM OPÇÃO PARA REDIMENSIONAR A ÁREA DE DESENHO !-->
    <asp:Button runat="server" Text="Salvar Imagem" ID="btnSalvarImagem" OnClick="btnSalvarImagem_Click" style="display: none" />
    </form>
</body>
</html>
