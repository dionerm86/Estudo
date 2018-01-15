using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class DesenhaProjeto1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.DesenhaProjeto1));
    
            // Busca o item projeto passado por query string
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idItemProjeto"]));
            ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProj.IdProjetoModelo);
    
            bool isPcp = itemProj.IdPedidoEspelho > 0;
            string nomeFigura = String.Empty;
            int item = Convert.ToInt32(Request["item"]);
    
            // Busca o produto pedido espelho para recuperar a URL da imagem
            if (isPcp)
            {
                var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByMaterItemProj(
                    MaterialItemProjetoDAO.Instance.GetMaterialByPeca(Convert.ToUInt32(Request["IdPecaItemProj"])).IdMaterItemProj).First();
    
                produtoPedidoEspelho.Item = item;
                nomeFigura = produtoPedidoEspelho.ImagemUrlSalvarItem;
            }
            else
            {
                uint idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(itemProj.IdPedido.Value,
                    MaterialItemProjetoDAO.Instance.GetMaterialByPeca(Convert.ToUInt32(Request["IdPecaItemProj"])).IdMaterItemProj);

                nomeFigura = Data.Helper.Utils.GetPecaComercialVirtualPath + idProdPed.ToString().PadLeft(10, '0') + "_" + item + ".jpg";
            }
    
            if (!IsPostBack)
            {
                if (Request["Ajax"] == null)
                {
                    // Carrega imagem na tela
                    if (File.Exists(Server.MapPath(nomeFigura)))
                        img_inicial.ImageUrl = nomeFigura;
                    else
                    {
                        // Carrega imagem na tela
                        nomeFigura = !String.IsNullOrEmpty(Request["item"]) ?
                            projMod.Codigo + "§" + Request["item"] + ".jpg" : projMod.NomeFiguraAssociada;

                        if (!File.Exists(Data.Helper.Utils.GetModelosProjetoPath + nomeFigura))
                            nomeFigura = projMod.IdProjetoModelo.ToString("0##") + "_" + Request["item"] + ".jpg";

                        img_inicial.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Data.Helper.Utils.GetModelosProjetoPath + nomeFigura;
                    }
                }
                else if (Request["Ajax"] == "SalvarImagem" || Request["Ajax"] == "AbrirBiblioteca")
                {
                    var b = new StringBuilder(Request["base64"].Split(',')[1]);
                    b.Replace("\r\n", String.Empty);
                    b.Replace(" ", String.Empty);
    
                    var imagem = Convert.FromBase64String(b.ToString());

                    ManipulacaoImagem.SalvarImagem(Server.MapPath(nomeFigura), imagem);
    
                    LogAlteracaoDAO.Instance.LogImagemProducao(Convert.ToUInt32(Request["IdPecaItemProj"]), Request["Item"], "Edição da imagem do item " + Request["item"] + ".");
                    PecaItemProjetoDAO.Instance.ImagemEditada(Convert.ToUInt32(Request["IdPecaItemProj"]), true);
                }
    
                MontaTabelaFiguras();
            }
        }
    
        /// <summary>
        /// Monta tabela com figuras que podem ser selecionada para desenho
        /// </summary>
        protected void MontaTabelaFiguras()
        {
            var contador = 1;
            var grupos = GrupoFiguraProjetoDAO.Instance.GetOrdered().Where
                (f => f.Descricao.ToLower() != "imagens em branco" &&
                f.Descricao.ToLower() != "letras horizontais" &&
                f.Descricao.ToLower() != "letras verticais" &&
                f.Descricao.ToLower() != "números horizontais" &&
                f.Descricao.ToLower() != "números verticais" &&
                f.Descricao.ToLower() != "textos");
    
            foreach (GrupoFiguraProjeto grupo in grupos)
            {
                var div = new HtmlGenericControl("div");
                div.Attributes.Add("id", "descricao_categoria_biblioteca_" + contador);
                div.Attributes.Add("class", "descricao_categoria_biblioteca");
                div.Attributes.Add("onclick", "fnc_habilita_imagem_categoria('" + contador + "','" + grupo.IdGrupoFigProj + "')");
                div.Attributes.Add("title", "Clique aqui para visualizar as imagens desta categoria");
    
                var imagemMais = new HtmlGenericControl("img");
                imagemMais.Attributes.Add("id", "img_categoria_" + contador);
                imagemMais.Attributes.Add("src", "../../Images/EdicaoImagemProjeto/mais.png");
    
                var titulo = new HtmlGenericControl("b");
                titulo.InnerText = grupo.Descricao;
    
                div.Controls.Add(imagemMais);
                div.Controls.Add(titulo);
    
                var divEspaco = new HtmlGenericControl("div");
                div.Attributes.Add("style", "clear: both");
    
                var divImagens = new HtmlGenericControl("div");
                divImagens.Attributes.Add("id", "lista_imagem_categoria_biblioteca_" + contador);
                divImagens.Attributes.Add("class", "lista_imagem_categoria_biblioteca");
                divImagens.Attributes.Add("style", "display: none");
    
                var tabela = new HtmlGenericControl("table");
                tabela.Attributes.Add("cellpadding", "0");
                tabela.Attributes.Add("cellspacing", "1");
                tabela.Attributes.Add("id", "tabela_imagem_" + contador);
    
                var linha = new HtmlGenericControl("tr");
                linha.Attributes.Add("style", "display:block");
    
                var lstFigura = FiguraProjetoDAO.Instance.GetOrdered(grupo.IdGrupoFigProj);
    
                // Monta as figuras na tabela
                foreach (var i in lstFigura)
                {
                    //var celula = new HtmlGenericControl("td");
                    //celula.Attributes.Add("align", "center");
                    //celula.Attributes.Add("height", "auto");
                    //celula.Attributes.Add("width", "60px");
                    //celula.Attributes.Add("style", "float:left");
    
                    var imagemPeca = new HtmlGenericControl("img");
                    imagemPeca.Attributes.Add("class", "img_adicional");
                    imagemPeca.Attributes.Add("src", Data.Helper.Utils.GetFullUrl(HttpContext.Current, Data.Helper.Utils.GetFigurasProjetoVirtualPath + i.IdFiguraProjeto + ".jpg"));
                    imagemPeca.Attributes.Add("border", "0");
                    imagemPeca.Attributes.Add("title", i.Descricao);
                    imagemPeca.Attributes.Add("style", "float:left; margin: 10px;");
    
                    //celula.Controls.Add(imagemPeca);
    
                    linha.Controls.Add(imagemPeca);
                }
    
                var input = new HtmlGenericControl("input");
                input.Attributes.Add("id", "hidden_quantidade_imagens_categoria_" + contador);
                input.Attributes.Add("class", "quantidade_imagens_categoria");
                input.Attributes.Add("type", "hidden");
                input.Attributes.Add("value", lstFigura.Count().ToString());
    
                var inputQtdeCategoria = new HtmlGenericControl("input");
                inputQtdeCategoria.Attributes.Add("id", "hidden_quantidade_categoria");
                inputQtdeCategoria.Attributes.Add("type", "hidden");
                inputQtdeCategoria.Attributes.Add("value", grupos.Count().ToString());
    
                tabela.Controls.Add(linha);
                divImagens.Controls.Add(tabela);
                divImagens.Controls.Add(input);
                box_biblioteca_img_adicionais.Controls.Add(div);
                box_biblioteca_img_adicionais.Controls.Add(divEspaco);
                box_biblioteca_img_adicionais.Controls.Add(divImagens);
                box_biblioteca_img_adicionais.Controls.Add(inputQtdeCategoria);
    
                contador++;
            }
        }
    
        [Ajax.AjaxMethod]
        public string test(string idGrupoFigProj)
        {
            var lstFigura = FiguraProjetoDAO.Instance.GetOrdered(Convert.ToUInt32(idGrupoFigProj));
            var listaControles = new List<HtmlGenericControl>();
            var retorno = string.Empty;
            // Monta as figuras na tabela
            foreach (var i in lstFigura)
            {
                //var celula = new HtmlGenericControl("td");
                //celula.Attributes.Add("align", "center");
                //celula.Attributes.Add("height", "auto");
                //celula.Attributes.Add("width", "60px");
                //celula.Attributes.Add("style", "float:left");
    
                var imagemPeca = new HtmlGenericControl("img");
                imagemPeca.Attributes.Add("class", "img_adicional");
                imagemPeca.Attributes.Add("src", Data.Helper.Utils.GetFullUrl(HttpContext.Current, Data.Helper.Utils.GetFigurasProjetoVirtualPath + i.IdFiguraProjeto + ".jpg"));
                imagemPeca.Attributes.Add("border", "0");
                imagemPeca.Attributes.Add("title", i.Descricao);
                imagemPeca.Attributes.Add("style", "float:left; margin: 10px;");
    
                //celula.Controls.Add(imagemPeca);
    
                listaControles.Add(imagemPeca);

                retorno += Data.Helper.Utils.GetFullUrl(HttpContext.Current, Data.Helper.Utils.GetFigurasProjetoVirtualPath + i.IdFiguraProjeto + ".jpg") + "|" + i.Descricao + "&";
            }
    
            return retorno.Substring(0, retorno.Length - 1);
        }
    }
}
