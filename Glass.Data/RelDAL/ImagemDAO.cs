using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.IO;
using System.Web;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class ImagemDAO : BaseDAO<Imagem, ImagemDAO>
    {
        //private ImagemDAO() { }

        public Imagem[] GetPecasAlteradas(uint idItemProjeto, float percentualImagem, PecaItemProjeto[] pecas)
        {
            return GetPecasAlteradas(null, idItemProjeto, percentualImagem, pecas);
        }

        public Imagem[] GetPecasAlteradas(GDASession sessao, uint idItemProjeto, float percentualImagem, PecaItemProjeto[] pecas)
        {
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(sessao, idItemProjeto);

            List<Imagem> retorno = new List<Imagem>();
            foreach (PecaItemProjeto p in pecas)
            {
                if (p.Tipo != 1)
                    continue;

                ProdutosPedidoEspelho ppe = !itemProj.IdPedidoEspelho.HasValue || p.IdProdPed.GetValueOrDefault() == 0 ? null :
                    ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(sessao, p.IdProdPed.Value);

                foreach (int item in Array.ConvertAll(UtilsProjeto.GetItensFromPeca(p.Item), x => Glass.Conversoes.StrParaInt(x)))
                {
                    //Se o item da peça for 99(geração dinâmica e não houver etiqueta, não gera a imagem.
                    if ((item == 90 || item == 91 || item == 92 || item == 93 || item == 94 ||
                        item == 95 || item == 96 || item == 97 || item == 98 || item == 99) && string.IsNullOrEmpty(p.Etiquetas))
                        continue;

                    if (item == 90 || item == 91 || item == 92 || item == 93 || item == 94 ||
                        item == 95 || item == 96 || item == 97 || item == 98 || item == 99)
                    {
                        string[] etiquetas = p.Etiquetas.Split(',');

                        foreach (string etq in etiquetas)
                        {
                            string imgUrl = UtilsProjeto.GetFiguraAssociadaUrl(sessao, idItemProjeto, itemProj.IdProjetoModelo, p.IdPecaItemProj, item);
                            imgUrl += "&numEtiqueta=" + etq;

                            byte[] imagem = Utils.GetImageFromRequest(HttpContext.Current, imgUrl);

                            if (imagem.Length == 0)
                                continue;

                            imagem = ManipulacaoImagem.Redimensionar(imagem, 0, 0, percentualImagem);

                            if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem2 == null)
                                retorno[retorno.Count - 1].Imagem2 = imagem;
                            else
                            {
                                Imagem nova = new Imagem();
                                nova.Chave = idItemProjeto;
                                nova.Imagem1 = imagem;
                                retorno.Add(nova);
                            }
                        }
                    }
                    else
                    {
                        string urlImagem = null;

                        if (ppe != null)
                        {
                            ppe.Item = item;
                            urlImagem = ppe.ImagemUrl;
                        }
                        else if (itemProj.IdPedido > 0)
                        {
                            MaterialItemProjeto mip = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(sessao, p.IdPecaItemProj);

                            if (mip != null)
                            {
                                // Se estiver imprimindo projeto de pedido que está no comercial,
                                // busca a imagem que pode ter sido editada do comercial
                                urlImagem = Utils.GetPecaComercialVirtualPath +
                                    ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(sessao, itemProj.IdPedido.Value,
                                    mip.IdMaterItemProj).ToString().PadLeft(10, '0') +
                                    "_" + item + ".jpg";

                                if (!File.Exists(HttpContext.Current.Server.MapPath(urlImagem)))
                                    urlImagem = null;
                            }
                        }
                        
                        if (urlImagem == null)
                            urlImagem = UtilsProjeto.GetFiguraAssociadaUrl(sessao, idItemProjeto, itemProj.IdProjetoModelo,
                                p.IdPecaItemProj, item);

                        byte[] imagem = Utils.GetImageFromRequest(HttpContext.Current, urlImagem);

                        if (imagem.Length == 0)
                            continue;

                        imagem = ManipulacaoImagem.Redimensionar(imagem, 0, 0, percentualImagem);

                        if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem2 == null)
                            retorno[retorno.Count - 1].Imagem2 = imagem;
                        else
                        {
                            Imagem nova = new Imagem();
                            nova.Chave = idItemProjeto;
                            nova.Imagem1 = imagem;
                            retorno.Add(nova);
                        }
                    }
                }
            }

            return retorno.ToArray();
        }

        public Imagem[] GetPecasModelo(uint idModeloProjeto)
        {
            PecaProjetoModelo[] pecasModelo = PecaProjetoModeloDAO.Instance.GetByModelo(idModeloProjeto).ToArray();

            List<Imagem> retorno = new List<Imagem>();

            for (int i = 0; i < pecasModelo.Length; i++)
            {
                PecaProjetoModelo peca = pecasModelo[i];

                string[] items = peca.Item.Split(' ');

                foreach (string it in items)
                {
                    /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                    var codigoProjetoModelo = ProjetoModeloDAO.Instance.ObtemCodigo(idModeloProjeto);
                    string path = Utils.GetModelosProjetoPath + codigoProjetoModelo + "§" + it + ".jpg";

                    if(!File.Exists(path))
                        path = Utils.GetModelosProjetoPath + idModeloProjeto.ToString().PadLeft(3, '0') + "_" + it + ".jpg";

                    if (File.Exists(path))
                    {
                        string pecaPath = "../../Handlers/LoadFiguraAssociada.ashx?tipoFigura=individual&tipoDesenho=0&path=" + path;

                        byte[] imagem = Utils.GetImageFromRequest(pecaPath);

                        if (imagem.Length == 0)
                            continue;

                        if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem2 == null)
                            retorno[retorno.Count - 1].Imagem2 = imagem;
                        else
                        {
                            Imagem nova = new Imagem();
                            nova.Chave = peca.IdPecaProjMod;
                            nova.Imagem1 = imagem;
                            retorno.Add(nova);
                        }
                    }
                }
            }

            return retorno.ToArray();
        }

        public Imagem[] GetProjetosModelo(string codigo, string descricao, uint idGrupoProjModelo, int situacao)
        {
            List<ProjetoModelo> lstProjModelos = ProjetoModeloDAO.Instance.GetList(codigo, descricao, idGrupoProjModelo, situacao);
            string criterio = (idGrupoProjModelo > 0 ? " Grupo: " + GrupoModeloDAO.Instance.ObtemDescricao(idGrupoProjModelo) : "  ")
                + (String.IsNullOrEmpty(codigo) ? " " : " Código: " + codigo + "  ")
                + (String.IsNullOrEmpty(descricao) ? " " : " Descrição: " + descricao + "  ");

            List<Imagem> retorno = new List<Imagem>();

            foreach (ProjetoModelo pm in lstProjModelos)
            {
                try
                {
                    string path = Utils.GetModelosProjetoPath + pm.NomeFigura;

                    if (File.Exists(path))
                    {
                        byte[] imagem = Utils.GetImageFromFile(path);
                        imagem = ManipulacaoImagem.Redimensionar(imagem, 300, 300, (float)0.75);

                        if (imagem.Length == 0)
                            continue;

                        if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem2 == null)
                        {
                            retorno[retorno.Count - 1].Imagem2 = imagem;
                            retorno[retorno.Count - 1].CodImagem2 = pm.Codigo;
                            retorno[retorno.Count - 1].DescImagem2 = pm.Descricao;
                        }
                        else if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem3 == null)
                        {
                            retorno[retorno.Count - 1].Imagem3 = imagem;
                            retorno[retorno.Count - 1].CodImagem3 = pm.Codigo;
                            retorno[retorno.Count - 1].DescImagem3 = pm.Descricao;
                        }
                        else
                        {
                            Imagem nova = new Imagem();
                            nova.Chave = pm.IdProjetoModelo;
                            nova.Imagem1 = imagem;
                            nova.CodImagem1 = pm.Codigo;
                            nova.DescImagem1 = pm.Descricao;
                            nova.Criterio = criterio;
                            retorno.Add(nova);
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao carregar imagens do projeto " + pm.Codigo + ".", ex));
                }
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Obtém a imagem engenharia do projeto, com as quotas posicionadas e calculadas de acordo com o item projeto.
        /// </summary>
        public Imagem[] ObterImagemEngenharia(int idItemProjeto, float percentualImagem)
        {
            var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(idItemProjeto);
            var retorno = new List<Imagem>();
                        
            var imgUrl = UtilsProjeto.GetFiguraAssociadaUrl((uint)idItemProjeto, itemProjeto.IdProjetoModelo);

            byte[] imagem = Utils.GetImageFromRequest(HttpContext.Current, imgUrl);

            if (imagem.Length == 0)
                return null;

            imagem = ManipulacaoImagem.Redimensionar(imagem, 0, 0, percentualImagem);

            if (retorno.Count > 0 && retorno[retorno.Count - 1].Imagem2 == null)
                retorno[retorno.Count - 1].Imagem2 = imagem;
            else
            {
                Imagem nova = new Imagem();
                nova.Chave = (uint)idItemProjeto;
                nova.Imagem1 = imagem;
                retorno.Add(nova);
            }
            
            return retorno.ToArray();
        }
    }
}
