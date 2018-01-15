using System;
using System.Linq;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using System.IO;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class EditarImagem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {       
            // Busca o item projeto passado por query string
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idItemProjeto"]));
            ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProj.IdProjetoModelo);
            
            bool isPcp = itemProj.IdPedidoEspelho > 0;
            string nomeFigura = NomeFigura();
            int item = Convert.ToInt32(Request["item"]);
    
            // Busca o produto pedido espelho para recuperar a URL da imagem
            if (isPcp)
            {
                var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByMaterItemProj(
                    MaterialItemProjetoDAO.Instance.GetMaterialByPeca(Convert.ToUInt32(Request["IdPecaItemProj"])).IdMaterItemProj).First();
    
                produtoPedidoEspelho.Item = item;
                nomeFigura = produtoPedidoEspelho.ImagemUrlSalvarItem;
            }
            else if (itemProj.IdPedido.GetValueOrDefault() > 0)
            {
                uint idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(itemProj.IdPedido.Value,
                    MaterialItemProjetoDAO.Instance.GetMaterialByPeca(Convert.ToUInt32(Request["IdPecaItemProj"])).IdMaterItemProj);

                nomeFigura = Data.Helper.Utils.GetPecaComercialVirtualPath + idProdPed.ToString().PadLeft(10, '0') + "_" + item + ".jpg";
            }
    
            if (Request["Ajax"] == null)
            {
                // Se o arquivo já tiver sido editado, busca ele mesmo
                if (File.Exists(Server.MapPath(nomeFigura)))
                    img_inicial.ImageUrl = nomeFigura;
                else
                {
                    // Se o pedido não tiver sido editado ainda, busca a imagem padrão com suas devidas medidas calculadas e inseridas na imagem
                    nomeFigura = !String.IsNullOrEmpty(Request["item"]) ?
                        projMod.Codigo + "§" + Request["item"] + ".jpg" : projMod.NomeFiguraAssociada;

                    if (!File.Exists(Data.Helper.Utils.GetModelosProjetoPath + nomeFigura))
                        nomeFigura = projMod.IdProjetoModelo.ToString("0##") + "_" + Request["item"] + ".jpg";

                    img_inicial.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Data.Helper.Utils.GetModelosProjetoPath + nomeFigura
                        + "&idProjetoModelo=" + Request["IdProjetoModelo"] + "&idPecaItemProj=" + Request["IdPecaItemProj"] + "&Item=" + Request["Item"] + "&IdItemProjeto=" + Request["IdItemProjeto"];
                }
            }
            else if (Request["Ajax"] == "SalvarImagem" || Request["Ajax"] == "AbrirBiblioteca")
            {
                var b = new StringBuilder(Request["base64"].Split(',')[1]);
                b.Replace("\r\n", String.Empty);
                b.Replace(" ", String.Empty);
    
                var imagem = Convert.FromBase64String(b.ToString());

                if (!ManipulacaoImagem.SalvarImagem(Server.MapPath(nomeFigura), imagem))
                {
                    Response.Write("Falha ao salvar imagem.");
                    Response.End();
                    return;
                }
    
                LogAlteracaoDAO.Instance.LogImagemProducao(Convert.ToUInt32(Request["IdPecaItemProj"]), Request["Item"], "Edição da imagem do item " + Request["item"] + ".");
                PecaItemProjetoDAO.Instance.ImagemEditada(Convert.ToUInt32(Request["IdPecaItemProj"]), true);

                Response.Write("Imagem salva!");
                Response.End();
            }        
            else if (Request["Ajax"] == "UploadImagem")
            {
                img_inicial.ImageUrl = Request["urlUpload"];
            }        
        }

        protected void VoltarImagem_Click(object sender, EventArgs e)
        {
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
    
            File.Delete(Server.MapPath(nomeFigura));
    
            LogAlteracaoDAO.Instance.LogImagemProducao(Convert.ToUInt32(Request["IdPecaItemProj"]), Request["Item"], "Atualização da imagem do item " + Request["item"] + " para imagem padrão.");
            PecaItemProjetoDAO.Instance.ImagemEditada(Convert.ToUInt32(Request["IdPecaItemProj"]), false);
    
            Response.Redirect(Request.RawUrl);
        }

        protected string NomeFigura()
        {
            // Busca o item projeto passado por query string
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idItemProjeto"]));

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
            else if(itemProj.IdPedido.GetValueOrDefault() > 0)
            {
                uint idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(itemProj.IdPedido.Value,
                    MaterialItemProjetoDAO.Instance.GetMaterialByPeca(Convert.ToUInt32(Request["IdPecaItemProj"])).IdMaterItemProj);

                nomeFigura = Data.Helper.Utils.GetPecaComercialVirtualPath + idProdPed.ToString().PadLeft(10, '0') + "_" + item + ".jpg";
            }
            else
            {
                nomeFigura = PCPConfig.GetPecaProjetoVirtualPath + Request["IdPecaItemProj"].ToString().PadLeft(10, '0') + "_" + item + ".jpg";
            }

            return nomeFigura;
        }

        protected void btnSalvarImagem_Click(object sender, EventArgs e)
        {
            if (!ManipulacaoImagem.SalvarImagem(Server.MapPath(NomeFigura()), file_imagem.PostedFile.InputStream))
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "alert('Falha ao salvar imagem.');", true);
                return;
            }

            img_inicial.ImageUrl = NomeFigura();

            LogAlteracaoDAO.Instance.LogImagemProducao(Convert.ToUInt32(Request["IdPecaItemProj"]), Request["Item"], "Edição da imagem do item " + Request["item"] + ".");
            PecaItemProjetoDAO.Instance.ImagemEditada(Convert.ToUInt32(Request["IdPecaItemProj"]), true);

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "alert('Imagem salva!.');", true);
        }    
    }
}
