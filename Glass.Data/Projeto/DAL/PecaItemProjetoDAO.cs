using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class PecaItemProjetoDAO : BaseDAO<PecaItemProjeto, PecaItemProjetoDAO>
	{
        //private PecaItemProjetoDAO() { }

        private List<PecaItemProjeto> GetBase(GDASession sessao, uint idItemProjeto, uint idProjetoModelo, bool pcp)
        {
            var nomeTabelaProdutosPedido = pcp ? "produtos_pedido_espelho" : "produtos_pedido";

            string sql = @"
                Select pip.*, p.Descricao as descrProduto, p.codInterno, COALESCE(ncm.ncm, p.ncm) as ncm, ea.codInterno as codAplicacao, 
                    ep.codInterno As CodProcesso, ppe.IdProdPed, ped.IdPedido
                From peca_item_projeto pip 
                    Left Join produto p On (pip.idProd=p.idProd)
                    Left Join material_item_projeto mip On (pip.idPecaItemProj=mip.idPecaItemProj)
                    Left Join etiqueta_aplicacao ea ON (mip.idAplicacao=ea.idAplicacao)
                    Left Join etiqueta_processo ep ON (mip.idProcesso=ep.idProcesso)
                    Left Join "+ nomeTabelaProdutosPedido + @" ppe On (mip.idMaterItemProj = ppe.idMaterItemProj)
                    left join pedido ped on (ppe.idPedido=ped.idPedido)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                    ) as ncm ON (ped.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                Where pip.idItemProjeto=" + idItemProjeto + @" and coalesce(mip.idItemProjeto, " + idItemProjeto + ")=" + idItemProjeto + @"
                Order By pip.tipo asc, pip.IdPecaItemProj asc";

            List<PecaItemProjeto> lstPecaItem = objPersistence.LoadData(sessao, sql);
            List<PecaProjetoModelo> lstPecaMod = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, idProjetoModelo);

            if (lstPecaItem.Count > lstPecaMod.Count)
                throw new Exception("Falha ao recuperar peça. Existe mais de um material para a mesma peça. iditemprojeto " + idItemProjeto);

            for (int i = 0; i < lstPecaItem.Count; i++)
            {
                if (lstPecaMod.Count == 0)
                {
                    lstPecaItem.RemoveAt(i);
                    continue;
                }

                lstPecaItem[i].IdPecaProjMod = lstPecaMod[i].IdPecaProjMod;
                lstPecaItem[i].Item = lstPecaMod[i].Item;
                lstPecaItem[i].IdArquivoMesaCorte = lstPecaMod[i].IdArquivoMesaCorte;
                lstPecaItem[i].TipoArquivoMesaCorte = (int?)lstPecaMod[i].TipoArquivo;
            }

            return lstPecaItem;
        }

        public List<PecaItemProjeto> GetByItemProjeto(uint idItemProjeto, uint idProjetoModelo)
        {
            return GetByItemProjeto(null, idItemProjeto, idProjetoModelo);
        }

        /// <summary>
        /// Busca peças de um item_projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<PecaItemProjeto> GetByItemProjeto(GDASession sessao, uint idItemProjeto, uint idProjetoModelo)
        {
            return GetBase(sessao, idItemProjeto, idProjetoModelo, true);
        }

        public PecaItemProjeto GetElement(uint idPecaItemProj)
        {
            return GetElement(null, idPecaItemProj);
        }

        public PecaItemProjeto GetElement(GDASession sessao, uint idPecaItemProj)
        {
            uint idItemProjeto = ExecuteScalar<uint>(sessao, "select idItemProjeto from peca_item_projeto where idPecaItemProj=" + idPecaItemProj);
            uint idProjetoModelo = ExecuteScalar<uint>(sessao, "select idProjetoModelo from item_projeto where idItemProjeto=" + idItemProjeto);

            return GetByItemProjeto(sessao, idItemProjeto, idProjetoModelo).Find(new Predicate<PecaItemProjeto>(
                delegate(PecaItemProjeto p)
                {
                    return p.IdPecaItemProj == idPecaItemProj;
                }
            ));
        }

        public PecaItemProjeto GetElementExt(GDASession session, uint idPecaItemProj, bool pcp)
        {
            uint idItemProjeto = ExecuteScalar<uint>(session, "select idItemProjeto from peca_item_projeto where idPecaItemProj=" + idPecaItemProj);
            uint idProjetoModelo = ExecuteScalar<uint>(session, "select idProjetoModelo from item_projeto where idItemProjeto=" + idItemProjeto);

            return GetBase(session, idItemProjeto, idProjetoModelo, pcp).Find(new Predicate<PecaItemProjeto>(
                delegate(PecaItemProjeto p)
                {
                    return p.IdPecaItemProj == idPecaItemProj;
                }
            ));
        }

        public PecaItemProjeto GetElementExt(uint idItemProjeto, uint idPecaProjMod)
        {
            return GetElementExt(null, idItemProjeto, idPecaProjMod);
        }

        public PecaItemProjeto GetElementExt(GDASession sessao, uint idItemProjeto, uint idPecaProjMod)
        {
            uint idProjetoModelo = ExecuteScalar<uint>(sessao, "select idProjetoModelo from item_projeto where idItemProjeto=" + idItemProjeto);

            return GetBase(sessao, idItemProjeto, idProjetoModelo, true).Find(new Predicate<PecaItemProjeto>(
                delegate(PecaItemProjeto p)
                {
                    return p.IdPecaProjMod == idPecaProjMod;
                }
            ));
        }

        /// <summary>
        /// Busca peças de um item_projeto para relatório
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<PecaItemProjeto> GetByItemProjetoRpt(uint idItemProjeto, uint idProjetoModelo)
        {
            return GetByItemProjetoRpt(idItemProjeto, idProjetoModelo, true);
        }

        /// <summary>
        /// Busca peças de um item_projeto para relatório
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<PecaItemProjeto> GetByItemProjetoRpt(uint idItemProjeto, uint idProjetoModelo,
            bool incluirInfoProdPed)
        {
            return GetByItemProjetoRpt(null, idItemProjeto, idProjetoModelo, incluirInfoProdPed);
        }
        
        /// <summary>
        /// Busca peças de um item_projeto para relatório
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<PecaItemProjeto> GetByItemProjetoRpt(GDASession sessao, uint idItemProjeto, uint idProjetoModelo)
        {
            return GetByItemProjetoRpt(sessao, idItemProjeto, idProjetoModelo, true);
        }
        
        /// <summary>
        /// Busca peças de um item_projeto para relatório
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public List<PecaItemProjeto> GetByItemProjetoRpt(GDASession sessao, uint idItemProjeto, uint idProjetoModelo,
            bool incluirInfoProdPed)
        {
            List<PecaItemProjeto> lstPecaItem = GetBase(sessao, idItemProjeto, idProjetoModelo, incluirInfoProdPed);

            List<PecaItemProjeto> lstPecaRetorno = new List<PecaItemProjeto>();
            List<MaterialProjetoBenef> lstBenef;

            // Adiciona os beneficiamentos feitos nos produtos como itens do pedido
            foreach (PecaItemProjeto pip in lstPecaItem)
            {
                MaterialItemProjeto mip = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(sessao, pip.IdPecaItemProj);

                // Verifica se o material foi marcado como redondo
                if (mip != null && mip.Redondo)
                {
                    if (!BenefConfigDAO.Instance.CobrarRedondo() && !pip.DescrProduto.ToLower().Contains("redondo"))
                        pip.DescrProduto += " REDONDO";

                    pip.Largura = 0;
                }

                lstPecaRetorno.Add(pip);

                // Se o material relacionado à peca do foreach não tiver beneficiamento, continua o loop
                if (mip == null) continue;

                // Carrega os beneficiamentos deste material, se houver
                lstBenef = MaterialProjetoBenefDAO.Instance.GetByMaterial(sessao, mip.IdMaterItemProj);

                if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                {
                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                    foreach (MaterialProjetoBenef mpb in lstBenef)
                    {
                        PecaItemProjeto peca = new PecaItemProjeto();
                        peca.IdItemProjeto = mip.IdItemProjeto;
                        peca.Qtde = mpb.Qtd > 0 ? mpb.Qtd : 1;
                        peca.DescrProduto = " " + mpb.DescrBenef +
                            Utils.MontaDescrLapBis(mpb.BisAlt, mpb.BisLarg, mpb.LapAlt, mpb.LapLarg, mpb.EspBisote, null, null, false);

                        lstPecaRetorno.Add(peca);
                    }
                }
                else
                {
                    if (lstBenef.Count > 0)
                    {
                        PecaItemProjeto peca = new PecaItemProjeto();

                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (MaterialProjetoBenef mpb in lstBenef)
                        {
                            peca.IdItemProjeto = mip.IdItemProjeto;
                            peca.Qtde = 0;
                            string textoQuantidade = (mpb.TipoCalculoBenef == (int)TipoCalculoBenef.Quantidade) ? mpb.Qtd.ToString() + " " : "";
                            peca.DescrProduto += "; " + textoQuantidade + mpb.DescrBenef +
                                Utils.MontaDescrLapBis(mpb.BisAlt, mpb.BisLarg, mpb.LapAlt, mpb.LapLarg, mpb.EspBisote, null, null, false);
                        }

                        peca.DescrProduto = " " + peca.DescrProduto.Substring(2);
                        lstPecaRetorno.Add(peca);
                    }
                }
            }

            return lstPecaRetorno;
        }

        /// <summary>
        /// Busca peças de um item projeto para a tela de totais de marcação.
        /// </summary>
        public List<PecaItemProjeto> ObtemPecaItemProjetoParaTotalMarcacao(uint idItemProjeto, uint idProjetoModelo)
        {
            var sql =
                string.Format(@"
                    SELECT pip.*, ppe.IdProdPed, ppe.IdProdPed, ppe.Qtde AS QtdeProdPed
                    FROM peca_item_projeto pip 
                        LEFT JOIN material_item_projeto mip ON (pip.IdPecaItemProj=mip.IdPecaItemProj)
                        LEFT JOIN produtos_pedido_espelho ppe ON (mip.IdMaterItemProj=ppe.IdMaterItemProj)
                    WHERE pip.Tipo=1 AND pip.IdItemProjeto={0} AND (mip.IdItemProjeto IS NULL OR mip.IdItemProjeto={0})
                    ORDER BY pip.Tipo ASC, pip.IdPecaItemProj ASC", idItemProjeto);

            var pecasItemProjeto = objPersistence.LoadData(sql).ToList();
            var pecasProjetoModelo = PecaProjetoModeloDAO.Instance.ObtemPecaProjetoModeloParaTotalMarcacao(idProjetoModelo);

            if (pecasItemProjeto.Count > pecasProjetoModelo.Count)
                throw new Exception(
                    string.Format("Falha ao recuperar peça. Existe mais de um material para a mesma peça. IdItemProjeto: {0}.",
                        idItemProjeto));

            for (int i = 0; i < pecasItemProjeto.Count; i++)
            {
                if (pecasProjetoModelo.Count == 0)
                {
                    pecasItemProjeto.RemoveAt(i);
                    continue;
                }

                pecasItemProjeto[i].IdPecaProjMod = pecasProjetoModelo[i].IdPecaProjMod;
                pecasItemProjeto[i].Item = pecasProjetoModelo[i].Item;
                pecasItemProjeto[i].IdArquivoMesaCorte = pecasProjetoModelo[i].IdArquivoMesaCorte;
                pecasItemProjeto[i].TipoArquivoMesaCorte = (int?)pecasProjetoModelo[i].TipoArquivo;
            }

            return pecasItemProjeto;
        }

        /// <summary>
        /// Busca a peça de um material item projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public PecaItemProjeto GetByMaterial(uint idMaterItemProj)
        {
            return GetByMaterial(null, idMaterItemProj);
        }

        /// <summary>
        /// Busca a peça de um material item projeto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public PecaItemProjeto GetByMaterial(GDASession sessao, uint idMaterItemProj)
        {
            uint? idPecaItemProj = ExecuteScalar<uint?>(sessao, "select idPecaItemProj from material_item_projeto where idMaterItemProj=" + idMaterItemProj);
            return idPecaItemProj > 0 ? GetElement(sessao, idPecaItemProj.Value) : null;
        }

        /// <summary>
        /// Busca a peça de um produto pedido.
        /// </summary>
        public PecaItemProjeto GetByProdPed(uint idProdPed)
        {
            return GetByProdPed(null, idProdPed);
        }

        /// <summary>
        /// Busca a peça de um produto pedido.
        /// </summary>
        public PecaItemProjeto GetByProdPed(GDASession session, uint idProdPed)
        {
            uint? idMaterItemProj = ExecuteScalar<uint?>(session, "select idMaterItemProj from produtos_pedido where idProdPed=" + idProdPed);
            return idMaterItemProj > 0 ? GetByMaterial(session, idMaterItemProj.Value) : null;
        }

        /// <summary>
        /// Busca a peça de um produto pedido espelho.
        /// </summary>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public PecaItemProjeto GetByProdPedEsp(uint idProdPedEsp)
        {
            return GetByProdPedEsp(null, idProdPedEsp);
        }

        /// <summary>
        /// Busca a peça de um produto pedido espelho.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public PecaItemProjeto GetByProdPedEsp(GDASession sessao, uint idProdPedEsp)
        {
            uint? idMaterItemProj = ExecuteScalar<uint?>(sessao, "select idMaterItemProj from produtos_pedido_espelho where idProdPed=" + idProdPedEsp);
            return idMaterItemProj > 0 ? GetByMaterial(sessao, idMaterItemProj.Value) : null;
        }

        public uint? ObtemIdPecaItemProjByIdProdPed(uint idProdPedEsp)
        {
            uint? idMaterItemProj = ExecuteScalar<uint?>("select idMaterItemProj from produtos_pedido_espelho where idProdPed=" + idProdPedEsp);
            return idMaterItemProj > 0 ? ExecuteScalar<uint?>("select idPecaItemProj from material_item_projeto where idMaterItemProj=" + idMaterItemProj) : null;
        }

        /// <summary>
        /// Retorna os itens de uma peça pelo seu produto pedido.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string[] GetItensByProdPed(uint idProdPed)
        {
            PecaItemProjeto peca = GetByProdPed(idProdPed);
            return peca != null ? UtilsProjeto.GetItensFromPeca(peca.Item) : new string[0];
        }

        /// <summary>
        /// Retorna os itens de uma peça pelo seu produto pedido espelho.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string[] GetItensByProdPedEsp(uint idProdPedEsp)
        {
            PecaItemProjeto peca = GetByProdPedEsp(idProdPedEsp);
            return peca != null ? UtilsProjeto.GetItensFromPeca(peca.Item) : new string[0];
        }

        /// <summary>
        /// Busca qtd de peças de vidro de um item_projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public int GetQtdPecaItemProjeto(uint idItemProjeto)
        {
            return GetQtdPecaItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Busca qtd de peças de vidro de um item_projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public int GetQtdPecaItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select Coalesce(Sum(pip.qtde), 0) From peca_item_projeto pip Where pip.idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString());
        }

        /// <summary>
        /// Exclui todas as peças de um item_projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void DeleteByItemProjeto(uint idItemProjeto)
        {
            DeleteByItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Exclui todas as peças de um item_projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        public void DeleteByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            PecaItemProjBenefDAO.Instance.DeleteByItemProjeto(sessao, idItemProjeto);
            string sql = "Delete From peca_item_projeto Where idItemProjeto=" + idItemProjeto;
            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Insere peças do item_projeto passado a partir da lista de peças passada, guardando o id gerado na
        /// lista lstPecaModelo
        /// </summary>
        public void InsertFromPecaModelo(ItemProjeto itemProj, ref List<PecaProjetoModelo> lstPecaModelo)
        {
            InsertFromPecaModelo(null, itemProj, ref lstPecaModelo);
        }

        /// <summary>
        /// Insere peças do item_projeto passado a partir da lista de peças passada, guardando o id gerado na
        /// lista lstPecaModelo
        /// </summary>
        public void InsertFromPecaModelo(GDASession sessao, ItemProjeto itemProj, ref List<PecaProjetoModelo> lstPecaModelo)
        {
            InsertFromPecaModelo(sessao, itemProj, ref lstPecaModelo, false);
        }

        /// <summary>
        /// Insere peças do item_projeto passado a partir da lista de peças passada, guardando o id gerado na
        /// lista lstPecaModelo
        /// </summary>
        public void InsertFromPecaModelo(GDASession sessao, ItemProjeto itemProj, ref List<PecaProjetoModelo> lstPecaModelo, bool eCommerce)
        {
            bool inserirPecas = objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From peca_item_projeto Where idItemProjeto=" + itemProj.IdItemProjeto) == 0;

            foreach (PecaProjetoModelo ppm in lstPecaModelo)
            {
                /* Chamado 48920. */
                if (eCommerce && ppm.IdPecaItemProj > 0 && ppm.Tipo != ObtemTipo(sessao, ppm.IdPecaItemProj))
                    throw new Exception("Não é possível alterar o tipo de peça do projeto pelo E-Commerce.");

                Produto prod = ProdutoDAO.Instance.GetByIdProd(sessao, ppm.IdProd);

                if (prod == null)
                    throw new Exception("Escolha um vidro para cada peça.");
                
                // Verifica se há fórmula para calcular a qtd de peças
                int qtdPeca = !String.IsNullOrEmpty(ppm.CalculoQtde) && !itemProj.MedidaExata ? (int)UtilsProjeto.CalcExpressao(sessao, ppm.CalculoQtde, itemProj, null) : ppm.Qtde;

                /* Chamado 22322. */
                if (!PCPConfig.CriarClone &&
                    !itemProj.MedidaExata &&
                    !string.IsNullOrEmpty(ppm.CalculoQtde) &&
                    itemProj.IdPedidoEspelho.GetValueOrDefault() > 0)
                    qtdPeca = ppm.Qtde;

                if (qtdPeca == 0)
                {
                    // A quantidade de peças deve ser zerada, pois pode ser que o usuário esteja tentando zerar a peça 
                    // mas como entra neste continue não estava zerando
                    if (!inserirPecas)
                        objPersistence.ExecuteCommand(sessao, "Update peca_item_projeto set qtde=0 where idPecaItemProj=" + ppm.IdPecaItemProj);

                    continue;
                }

                if (inserirPecas || (ppm.IdPecaItemProj == 0 && ppm.IdProd > 0 && ppm.Qtde > 0))
                {
                    PecaItemProjeto pip = new PecaItemProjeto();
                    pip.IdItemProjeto = itemProj.IdItemProjeto;
                    pip.IdProd = (uint)prod.IdProd;
                    pip.Altura = ppm.Altura;
                    pip.Largura = ppm.Largura;
                    pip.Qtde = qtdPeca;
                    pip.Tipo = ppm.Tipo;
                    pip.Redondo = ppm.Redondo;
                    pip.Beneficiamentos = ppm.Beneficiamentos.ToPecasItemProjeto();
                    
                    ppm.IdPecaItemProj = Insert(sessao, pip);
                }
                else
                {
                    objPersistence.ExecuteCommand(sessao, "Update peca_item_projeto set idprod=" + prod.IdProd + ", altura=" + ppm.Altura + 
                        ", largura=" + ppm.Largura + ", qtde=" + qtdPeca + ", tipo=" + ppm.Tipo + " where idPecaItemProj=" + ppm.IdPecaItemProj);
                }
            }

            // Atualiza este item_projeto com a qtd e m² Vão
            ItemProjetoDAO.Instance.AtualizaQtdM2(sessao, itemProj.IdItemProjeto, itemProj.Qtde, UtilsProjeto.CalculaAreaVao(sessao, itemProj.IdItemProjeto, itemProj.MedidaExata));
        }

        /// <summary>
        /// Verifica se a peça é redonda
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <returns></returns>
        public bool IsRedondo(uint idPecaItemProj)
        {
            return ObtemValorCampo<bool>("redondo", "idPecaItemProj=" + idPecaItemProj);
        }

        #region Verifica se peça possui associação com alguma figura

        public bool PossuiFiguraAssociada(uint idPecaItemProj)
        {
            return  PossuiFiguraAssociada(null, idPecaItemProj);
        }

        /// <summary>
        /// Verifica se peça possui associação com alguma figura
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <returns></returns>
        public bool PossuiFiguraAssociada(GDASession sessao, uint idPecaItemProj)
        {
            if (idPecaItemProj == 0)
                return false;

            string sql = "Select * From figura_peca_item_projeto Where idPecaItemProj=" + idPecaItemProj;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Obtém peça através de suas propriedades

        /// <summary>
        /// Obtém peça inserida em pedido através de propriedades da peca inserida no orçamento/projeto
        /// </summary>
        public uint? ObtemIdPecaNova(uint idPecaItemProjOld, uint idItemProjeto, string idsPecaUsados)
        {
            return ObtemIdPecaNova(null, idPecaItemProjOld, idItemProjeto, idsPecaUsados);
        }

        /// <summary>
        /// Obtém peça inserida em pedido através de propriedades da peca inserida no orçamento/projeto
        /// </summary>
        public uint? ObtemIdPecaNova(GDASession session, uint idPecaItemProjOld, uint idItemProjeto, string idsPecaUsados)
        {
            var sql = "Select idPecaItemProj From peca_item_projeto Where idItemProjeto=" + idItemProjeto +
                " And altura=" + ObtemAltura(session, idPecaItemProjOld) + " And largura=" + ObtemLargura(session, idPecaItemProjOld) + 
                " And tipo=" + ObtemTipo(session, idPecaItemProjOld) + " And qtde=" + ObtemQtde(session, idPecaItemProjOld);
            
            if (!string.IsNullOrEmpty(idsPecaUsados))
                sql += " and idPecaItemProj Not In (" + idsPecaUsados.TrimEnd(',') + ")";

            return ExecuteScalar<uint?>(session, sql + " limit 1");
        }

        #endregion

        #region Obtém as etiquetas da peça passada
                
        /// <summary>
        /// Obtém as etiquetas da peça passada
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <returns></returns>
        public string ObtemEtiquetas(uint idPecaItemProj, out PecaItemProjeto peca)
        {
            peca = GetElement(idPecaItemProj);
            return ObtemEtiquetas(peca.IdPedido, peca.IdProdPed, peca.Qtde);
        }

        /// <summary>
        /// Obtém as etiquetas da peça passada
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <param name="qtde"></param>
        /// <returns></returns>
        public string ObtemEtiquetas(uint? idPedido, uint? idProdPed, int qtde)
        {
            return ObtemEtiquetas(null, idPedido, idProdPed, qtde);
        }

        /// <summary>
        /// Obtém as etiquetas da peça passada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <param name="qtde"></param>
        /// <returns></returns>
        public string ObtemEtiquetas(GDASession session, uint? idPedido, uint? idProdPed, int qtde)
        {
            if ((idPedido == null || idPedido == 0) && (idProdPed == null || idProdPed == 0))
                return "";

            int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(session, idPedido.Value, idProdPed.Value);
            int numeroItens = qtde;

            string retorno = "";
            for (int i = 0; i < numeroItens; i++)
            {
                string etiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance.GetNumEtiqueta(idPedido.Value, pos, i + 1, numeroItens, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);
                retorno += etiqueta + ", ";
            }

            return retorno.TrimEnd(' ', ',');
        }

        #endregion
        
        #region Obtem dados de um item

        public int ObtemAltura(GDASession sessao, uint idPecaItemProj)
        {
            return ObtemValorCampo<int>(sessao, "altura", "idPecaItemProj=" + idPecaItemProj);
        }

        public int ObtemLargura(GDASession sessao, uint idPecaItemProj)
        {
            return ObtemValorCampo<int>(sessao, "largura", "idPecaItemProj=" + idPecaItemProj);
        }

        public int ObtemQtde(GDASession sessao, uint idPecaItemProj)
        {
            return ObtemValorCampo<int>(sessao, "qtde", "idPecaItemProj=" + idPecaItemProj);
        }

        public uint? ObtemIdProd(GDASession sessao, uint idPecaItemProj)
        {
            return ObtemValorCampo<uint?>(sessao, "idProd", "idPecaItemProj=" + idPecaItemProj);
        }

        public bool ObtemEditarImagemProjeto(uint idItemProj)
        {
            return ObtemValorCampo<bool>("ImagemEditada", "idItemProjeto=" + idItemProj);
        }

        public int ObtemTipo(uint idPecaItemProj)
        {
            return ObtemTipo(null, idPecaItemProj);
        }

        public int ObtemTipo(GDASession sessao, uint idPecaItemProj)
        {
            return ObtemValorCampo<int>(sessao, "tipo", "idPecaItemProj=" + idPecaItemProj);
        }

        public string ObtemItem(uint idPecaItemProj)
        {
            return ObtemItem(null, idPecaItemProj);
        }

        public string ObtemItem(GDASession sessao, uint idPecaItemProj)
        {
            uint idItemProjeto = ExecuteScalar<uint>(sessao, "select idItemProjeto from peca_item_projeto where idPecaItemProj=" + idPecaItemProj);
            uint idProjetoModelo = ExecuteScalar<uint>(sessao, "select idProjetoModelo from item_projeto where idItemProjeto=" + idItemProjeto);

            string sql = @"Select idPecaItemProj From peca_item_projeto
                Where idItemProjeto=" + idItemProjeto + " Order By tipo asc, idPecaItemProj asc";

            var lstPecaItem = objPersistence.LoadResult(sessao, sql).Select(f => f.GetUInt32(0))
                       .ToList(); ;
            var lstPecaMod = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, idProjetoModelo);

            int posicao = lstPecaItem.IndexOf(idPecaItemProj);

            if (posicao > lstPecaMod.Count - 1)
            {
                var idPedidoEspelho = ExecuteScalar<int>(sessao,
                    string.Format("SELECT IdPedidoEspelho FROM item_projeto WHERE IdItemProjeto={0}", idItemProjeto));

                throw new Exception(
                    string.Format("Projeto possui mais de uma peça para o mesmo item. Erro: IdPecaItemProj: {0} - IdPedido: {1}.",
                    idPecaItemProj, idPedidoEspelho));
            }

            return posicao > -1 ? lstPecaMod[posicao].Item : null;
        }
        
        #endregion

        #region Métodos sobrescritos

        public override uint Insert(PecaItemProjeto objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession sessao, PecaItemProjeto objInsert)
        {
            uint retorno = base.Insert(sessao, objInsert);

            foreach (PecaItemProjBenef p in objInsert.Beneficiamentos.ToPecasItemProjeto(retorno))
                PecaItemProjBenefDAO.Instance.Insert(sessao, p);

            objInsert.RefreshBeneficiamentos();
            return retorno;
        }

        public override int Update(GDASession session, PecaItemProjeto objUpdate)
        {
            base.Update(session, objUpdate);

            PecaItemProjBenefDAO.Instance.DeleteByPecaItemProj(session, objUpdate.IdPecaItemProj);
            foreach (PecaItemProjBenef p in objUpdate.Beneficiamentos.ToPecasItemProjeto(objUpdate.IdPecaItemProj))
                PecaItemProjBenefDAO.Instance.Insert(session, p);

            objUpdate.RefreshBeneficiamentos();
            return 1;
        }

        public override int Update(PecaItemProjeto objUpdate)
        {
            return Update(null, objUpdate);
        }

        /*public int ImagemEditada(uint idItemProjeto, bool imagemEditada)
        {
            return objPersistence.ExecuteCommand("Update peca_item_projeto set IMAGEMEDITADA=" + imagemEditada.ToString().ToLower() + 
                " where idItemProjeto=" + idItemProjeto);
        }*/

        /* Chamado 18532. */
        public int ImagemEditada(uint idPecaItemProjeto, bool imagemEditada)
        {
            return objPersistence.ExecuteCommand("UPDATE peca_item_projeto SET ImagemEditada=" + imagemEditada.ToString().ToLower() +
                " WHERE IdPecaItemProj=" + idPecaItemProjeto);
        }

        public override int Delete(PecaItemProjeto objDelete)
        {
            PecaItemProjBenefDAO.Instance.DeleteByPecaItemProj(null, objDelete.IdPecaItemProj);
            return base.Delete(objDelete);
        }

        #endregion

        #region CADProject

        /// <summary>
        /// Atualiza o GUID utilizado para identificar a peça no CADProject
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <param name="guid"></param>
        public void AtualizaGUID(uint idPecaItemProj, string guid)
        {
            objPersistence.ExecuteCommand("UPDATE peca_item_projeto SET guid=?guid WHERE idPecaItemProj=" + idPecaItemProj, new GDAParameter("?guid", guid));
        }

        public Guid ObterGUID(uint idPecaItemProj)
        {
            return ExecuteScalar<Guid>("SELECT guid FROM peca_item_projeto WHERE idPecaItemProj=" + idPecaItemProj);
        }

        #endregion
    }
}