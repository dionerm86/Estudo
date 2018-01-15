<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DesenhaProjeto1.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Projeto.DesenhaProjeto1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <link rel="stylesheet" href="../../Style/EdicaoImagemProjeto/modelo.css" />
    <link rel="stylesheet" href="../../Style/EdicaoImagemProjeto/wPaint.css" />
</head>
<body ondragstart="return false">

    <script type="text/javascript" src="../../Scripts/EdicaoImagemProjeto/jquery.1.8.2.min.js"></script>

    <script type="text/javascript" src="../../Scripts/EdicaoImagemProjeto/kinetic-v4.4.3.min.js"></script>
    
    <script type="text/javascript" src="<%= ResolveUrl("~") %>Scripts/Utils.js"></script>

    <script>
			$(window).load(function()
			{
			    // Array com todas as imagens que já foram adicionadas
				var array_imagens_adicionadas = new Array();					
					
				// Busca os dados da imagem inicial
				var altura_imagem_inicial = $('#img_inicial').css('height');
				var caminho_imagem_inicial = $('#img_inicial').attr('src');
				var largura_imagem_inicial = $('#img_inicial').css('width');
				
				// Altera o tamanho da área de desenho conforme o tamanho da imagem					
				$('#box_imagem_projeto').css('height', altura_imagem_inicial);
				$('#box_imagem_projeto').css('width', largura_imagem_inicial);
							
				// Inicializa o plugin para alterar a posição das imagens
				var stage = new Kinetic.Stage(
					{
					container: 'box_imagem_projeto',
					height: parseInt(altura_imagem_inicial),
					width: parseInt(largura_imagem_inicial)
					});
				var layer = new Kinetic.Layer();				
				
				// Após carregar a imagem inicial, mostra a mesma
				var obj_imagem = new Image();
				obj_imagem.src = caminho_imagem_inicial;
				obj_imagem.onload = function ()
					{					
					// Carrega a imagem inicial no plugin	
					var imagem_inicial = new Kinetic.Image(
					  {
						image: obj_imagem,
						x: 0,
						y: 0,
						height: parseInt(altura_imagem_inicial),
						width: parseInt(largura_imagem_inicial),
						draggable: false,
						strokeEnabled: false
					  });
					
					layer.add(imagem_inicial);					
					stage.add(layer);					
					}				
				
				// Ao clicar em uma imagem da biblioteca desenha a mesma na posição 0,0
				$('.img_adicional').click(function() 
				  {
					// Cria um novo objeto do tipo imagem
					var obj_imagem = new Image();
					obj_imagem.src = $(this).attr('src');
					var height_obj_imagem = parseInt(Number(this.height));
					var width_obj_imagem = parseInt(Number(this.width));
					
					// Posição inicial em que a imagem será adicionada
					var offset_height_obj_imagem = parseInt(height_obj_imagem / 2);
					var offset_width_obj_imagem = parseInt(width_obj_imagem / 2);

					// Busca o número da última imagem adicionada
					var numero_ultima_imagem_adicionada = parseInt($('#hidden_quantidade_imagens_adicionadas').val());
					numero_ultima_imagem_adicionada++;
					$('#hidden_quantidade_imagens_adicionadas').val(numero_ultima_imagem_adicionada);
					
					// ID da nova imagem adicionada
					obj_imagem.id = 'img_adicional_'+numero_ultima_imagem_adicionada;

					// Salva o ID da imagem que foi selecionada
					$('#hidden_imagem_adicional_anterior_selecionada').val($('#hidden_imagem_adicional_atual_selecionada').val());
					$('#hidden_imagem_adicional_atual_selecionada').val(obj_imagem.id);					
					
					var imagem_adicional = new Kinetic.Image(
						{
						image: obj_imagem,
						x: 50,
						y: 50,
						height: height_obj_imagem,
						width: width_obj_imagem,
						draggable: true,
						offset: [offset_width_obj_imagem, offset_height_obj_imagem],
          	            stroke: 'red',
          	            strokeWidth: 2,
						strokeEnabled: true
						});
					layer.add(imagem_adicional);
					stage.add(layer);
					
					// Armazena em um array todas as relações de ID x objeto de todas as imagens que já foram adicionadas
					array_imagens_adicionadas[obj_imagem.id] = imagem_adicional;
					
					// Evento ao clicar em uma imagem (deixa a imagem com uma borda vermelha ao redor)	
					imagem_adicional.on('mousedown', function(evt) 
					  {
						// Tira a borda da imagem da imagem atual	
						if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
							{
							var imagem_atual = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()]; 
							imagem_atual.setStroke('');
							imagem_atual.setStrokeWidth(0);
							}							
							
						// Coloca a borda na imagem atual	
						this.setStroke('red');
						this.setStrokeWidth(2);
						
						// Salva o ID da imagem que foi selecionada
						var atual_antigo = $('#hidden_imagem_adicional_atual_selecionada').val();
						
						$('#hidden_imagem_adicional_atual_selecionada').val(this.getImage().id);
						
						if ($('#hidden_imagem_adicional_atual_selecionada').val() != atual_antigo)
						  {
							$('#hidden_imagem_adicional_anterior_selecionada').val(atual_antigo);
							}
						});					
						
					// Evento ao clicar fora de uma imagem (tira a borda vermelha ao redor)
					imagem_adicional.on('mouseout', function(evt) 
					  {
						this.setStroke('');
			      this.setStrokeWidth(0);
					  });																	
				  });	
				
				// Diminui o tamanho da imagem selecionada
				$('#div_decrease').click(function()
				 {
				 if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
					 {
					 var imagem_adicional = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];
					 var scale_x = Number(imagem_adicional.getScale().x - 0.2);
					 var scale_y = Number(imagem_adicional.getScale().y - 0.2);  	 					

					 if (scale_x > 0.2 && scale_y > 0.2)
					   {
					 	 imagem_adicional.setScale(scale_x, scale_y);
					   imagem_adicional.setStroke('red');
					   imagem_adicional.setStrokeWidth(2);				
					 	 layer.add(imagem_adicional);
					 	 stage.add(layer);
						 }
					 }
				 });
				 
				// Remove a imagem selecionada
				$('#div_delete').click(function()
				  {
					if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
					  {
						var imagem_adicional = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];
						imagem_adicional.hide();
						layer.draw();
						}
					});				 
				 
				// Aumenta o tamanho da imagem selecionada
				$('#div_increase').click(function()
				 {
				 if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
					 {
					 var imagem_adicional = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];	
					 var scale_x = Number(imagem_adicional.getScale().x + 0.2);
					 var scale_y = Number(imagem_adicional.getScale().y + 0.2);  	 					
					 
					 imagem_adicional.setScale(scale_x, scale_y);
					 imagem_adicional.setStroke('red');
					 imagem_adicional.setStrokeWidth(2);					
					 layer.add(imagem_adicional);
					 stage.add(layer);
					 }
				 });				
				
				// Rotaciona a imagem selecionada para a direita	
				$('#div_rotate_left').click(function()
					{
					if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
					  {
						var imagem_adicional = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];
						imagem_adicional.setRotation(imagem_adicional.getRotation() + (-1 * Math.PI / 2));
						imagem_adicional.setStroke('red');
						imagem_adicional.setStrokeWidth(2);						
						layer.add(imagem_adicional);
						stage.add(layer);
						}
					});				
				
				// Rotaciona a imagem selecionada para a direita	
				$('#div_rotate_right').click(function()
					{						
					if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
					  {
						var imagem_adicional = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];
						imagem_adicional.setRotation(imagem_adicional.getRotation() + (Math.PI / 2));
						imagem_adicional.setStroke('red');
						imagem_adicional.setStrokeWidth(2);						
						layer.add(imagem_adicional);
						stage.add(layer);
						}
					});	
					
					// Salva a imagem
				$('#div_save').click(function()
				  {
					if (confirm("Deseja realmente alterar a imagem atual por esta nova imagem?"))
					  { 	
					  var variaveis=location.search.split("?"); 
				      var parametros = variaveis[1].split("&");	
				      var _idProjetoModelo = parametros[0].split("=")[1];
				      var _idItemProjeto = parametros[1].split("=")[1];
				      var _idPecaItemProj = parametros[2].split("=")[1];
				      var _item = parametros[3].split("=")[1];
				      
						if ($('#hidden_imagem_adicional_atual_selecionada').val() != '')
							{
							var imagem_atual = array_imagens_adicionadas[$('#hidden_imagem_adicional_atual_selecionada').val()];
							imagem_atual.setStroke('');
							imagem_atual.setStrokeWidth(0);
							layer.add(imagem_atual);
							stage.add(layer);
							}
							
						$.ajax({
				            type: "POST",
					        url: "DesenhaProjeto1.aspx",
					        data: { base64: imagem_atual.parent.canvas.toDataURL(), ajax: "SalvarImagem", idProjetoModelo: _idProjetoModelo, //início data
					        idItemProjeto: _idItemProjeto, idPecaItemProj: _idPecaItemProj, 
					        item : _item }, //fim data
					        dataType: "json",
					        async: false					        
					    });
					    
					    location.href = "EditarImagem.aspx?idProjetoModelo=" + _idProjetoModelo + "&idItemProjeto=" 
					    + _idItemProjeto + "&idPecaItemProj=" + _idPecaItemProj + "&item=" + _item;
					   
						}					
					});																			
				});
					
					
				
			// Habilita as imagens da categoria selecionada
			function fnc_habilita_imagem_categoria(posicao, idGrupo)
			{
//			    var retorno = DesenhaProjeto1.test(idGrupo);
//			    var itens = retorno.value.split('&');			   			    
//			    
//			    if(document.getElementById('linha_imagens_'+posicao) == null)
//			    {			        
//			        var tabela = document.getElementById('tabela_imagem_'+posicao);
//			    
//			        var novaLinha = document.createElement('tr');			    			    
//			        novaLinha.setAttribute('id', 'linha_imagens_' + posicao);
//			    
//			        var imagemModelo = document.getElementById('img_modelo');
//			        var novaImagem = document.createElement('img');
//			        
//			        for(var i = 0; i < itens.length; i++)
//			        {
//			            var atributosImagem = itens[i].split('|');			        
//			            var novaImagem = document.createElement('img');

//			            novaImagem.className = 'img_adicional';
//			            novaImagem.src = atributosImagem[0];
//			            novaImagem.style.border = '0';
//			            novaImagem.title = atributosImagem[1];
//			            		  novaImagem = imagemModelo;          
//    			        
//    			        novaLinha.appendChild(novaImagem); 		
//    			        	            
//			        }    			       			    
//			        
//			        tabela.appendChild(novaLinha);
//			    }
			    if (document.getElementById("lista_imagem_categoria_biblioteca_"+posicao).style.display == 'none')
				{
				    // Oculta todas as categorias
					for (var cont = 1; cont <= parseInt(document.getElementById("hidden_quantidade_categoria").value); cont++)
					{
					    document.getElementById("img_categoria_"+cont).src = '../../Images/EdicaoImagemProjeto/mais.png';
						document.getElementById("lista_imagem_categoria_biblioteca_"+cont).style.display = 'none';
					}
						
					document.getElementById("img_categoria_"+posicao).src = '../../Images/EdicaoImagemProjeto/menos.png';
					document.getElementById("lista_imagem_categoria_biblioteca_"+posicao).style.display = '';
				}
				else
				{
					document.getElementById("img_categoria_"+posicao).src = '../../Images/EdicaoImagemProjeto/mais.png';
					document.getElementById("lista_imagem_categoria_biblioteca_"+posicao).style.display = 'none';
				}
			}		
			
			function voltar()
			{
			    var variaveis=location.search.split("?"); 
				var parametros = variaveis[1].split("&");
				
				var _idProjetoModelo = parametros[0].split("=")[1];
				var _idItemProjeto = parametros[1].split("=")[1];
				var _idPecaItemProj = parametros[2].split("=")[1];
				var _item = parametros[3].split("=")[1];
					    
				location.href = "EditarImagem.aspx?idProjetoModelo=" + _idProjetoModelo + "&idItemProjeto=" 
				+ _idItemProjeto + "&idPecaItemProj=" + _idPecaItemProj + "&item=" + _item;					   
			}	
			
		</script>
    
    
    <asp:Image ID="img_inicial" runat="server" Style="display: none;" />
    <form name="form_edicao_imagem" method="post" enctype="multipart/form-data" runat="server">
    <img class="img_adicional" id="img_modelo" />
    <table>
    <tr id="tr_modelo"></tr>
    </table>
    <label id="label_salvar_imagem" class="f_24 f_red" style="display: none; margin: 0px 0px 0px 20px;">
        <b>Aguarde salvando imagem.<br>
            <br>
        </b>
    </label>
    <!-- INICIO CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <input id="hidden_contexto" name="hidden_contexto" type="hidden" value="/ecg" />
    <input id="hidden_diretorio_imagem" name="hidden_diretorio_imagem" type="hidden"
        value="/var/www/imagensNovas/imagens_clt_003/projetosEspeciais/" />
    <input id="hidden_extensao_imagem" name="hidden_extensao_imagem" type="hidden" value="jpg" />
    <input id="hidden_imagem_alterada" type="hidden" value="1" />
    <input id="hidden_serial" name="hidden_serial" type="hidden" value="77737" />
    <input id="hidden_tipo_texto_selecionado" type="hidden" value="H" />
    <!-- FIM CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <!-- INICIO CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <input id="hidden_imagem_adicional_atual_selecionada" type="hidden" />
    <input id="hidden_imagem_adicional_anterior_selecionada" type="hidden" />
    <input id="hidden_quantidade_imagens_adicionadas" type="hidden" value="0" />
    <!-- FIM CAMPOS HIDDENS PARA EDITOR E BIBLIOTECA DE IMAGENS !-->
    <!-- INICIO MENU !-->
    <div class="_wPaint_menu_biblioteca">
        <div class="_wPaint_options">
            <div id="div_save" class="_wPaint_icon _wPaint_save" title="Salvar">
            </div>
            <div id="div_rotate_left" class="_wPaint_icon _wPaint_rotate_left" title="Girar para a esquerda a imagem selecionada">
            </div>
            <div id="div_rotate_right" class="_wPaint_icon _wPaint_rotate_right" title="Girar para a direita a imagem selecionada">
            </div>
            <div id="div_increase" class="_wPaint_icon _wPaint_increase" title="Aumentar a imagem selecionada">
            </div>
            <div id="div_decrease" class="_wPaint_icon _wPaint_decrease" title="Diminuir a imagem selecionada">
            </div>
            <div id="div_delete" class="_wPaint_icon _wPaint_delete" title="Excluir a imagem selecionada">
            </div>
        </div>
    </div>
    <!-- FIM MENU !-->
    <!-- INICIO IMAGEM DO PROJETO !-->
    <div id="box_imagem_projeto" style="height: 480px; width: 640px;">
        <div class="kineticjs-content" style="position: relative; display: inline-block;>
            <canvas width="640" height="480" style=" padding: 0px; margin: 0px; border: 0px; background-color: transparent;
                width: 640px; height: 480px; position: absolute; background-position: initial initial;
                background-repeat: initial initial;"></canvas>
        </div>
    </div>
    <!-- FIM IMAGEM DO PROJETO !-->
    <!-- INICIO BIBLIOTECA DE IMAGENS !-->
    <div id="box_biblioteca_img_adicionais" runat="server">
    </div>
    <!-- FIM BIBLIOTECA DE IMAGENS !-->
    <div style="clear: both;">
    </div>
    <br />
    <input type="button" value="Voltar para tela de edição" onclick="voltar();"
        title="Voltar para a tela de edição" />
    <input type="button" value="Fechar" onclick="closeWindow();"
        title="Clique aqui para fechar a tela" />
    <!-- INICIO BOTÕES !-->
    <!-- FIM BOTÕES !-->
    </form>
</body>
</html>
