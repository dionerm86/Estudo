using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoProjetoConfigDAO : BaseDAO<ProdutoProjetoConfig, ProdutoProjetoConfigDAO>
    {
        //private ProdutoProjetoConfigDAO() { }

        #region Busca produtos x cor

        /// <summary>
        /// Retorna as cores de alumínio, ferragem ou vidro que foram ou não associadas aos produtos do projeto
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <returns></returns>
        public List<ProdutoProjetoConfig> GetByProdProj(uint idProdProj)
        {
            if (idProdProj == 0)
                return null;

            // Verifica se o produto do projeto é alumínio ou ferragem
            int tipoProdProj = ProdutoProjetoDAO.Instance.ObtemValorCampo<int>("tipo", "idProdProj=" + idProdProj);

            if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Aluminio)
            {
                string sql = @"
                    Select ca.IdCorAluminio as IdCorProduto, ca.Descricao as DescrCorProduto, p.Descricao as DescrProd, 
                        p.codInterno as CodInternoProd, ppc.*  
                    From cor_aluminio ca 
                        Left Join produto_projeto_config ppc On (ppc.IdCorAluminio=ca.IdCorAluminio) 
                            And (ppc.idProdProj=" + idProdProj + @") 
                        Left Join produto p On (ppc.IdProd=p.IdProd)";

                List<ProdutoProjetoConfig> lstProdConfig = objPersistence.LoadData(sql);
                foreach (ProdutoProjetoConfig ppc in lstProdConfig)
                    ppc.IdProdProj = idProdProj;

                return lstProdConfig;
            }
            else if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Ferragem)
            {
                string sql = @"
                    Select cf.IdCorFerragem as IdCorProduto, cf.Descricao as DescrCorProduto, p.Descricao as DescrProd, 
                        p.codInterno as CodInternoProd, ppc.* 
                    From cor_ferragem cf 
                        Left Join produto_projeto_config ppc On (ppc.IdCorFerragem=cf.IdCorFerragem) 
                            And (ppc.idProdProj=" + idProdProj + @") 
                        Left Join produto p On (ppc.IdProd=p.IdProd)";

                List<ProdutoProjetoConfig> lstProdConfig = objPersistence.LoadData(sql);
                foreach (ProdutoProjetoConfig ppc in lstProdConfig)
                    ppc.IdProdProj = idProdProj;

                return lstProdConfig;
            }
            else if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Vidro)
            {
                string sql = @"
                    Select cv.IdCorVidro as IdCorProduto, cv.Descricao as DescrCorProduto, p.Descricao as DescrProd, 
                        p.codInterno as CodInternoProd, ppc.* 
                    From cor_vidro cv 
                        Left Join produto_projeto_config ppc On (ppc.IdCorVidro=cv.IdCorVidro) 
                            And (ppc.idProdProj=" + idProdProj + @") 
                        Left Join produto p On (ppc.IdProd=p.IdProd)";

                List<ProdutoProjetoConfig> lstProdConfig = objPersistence.LoadData(sql);
                foreach (ProdutoProjetoConfig ppc in lstProdConfig)
                    ppc.IdProdProj = idProdProj;

                return lstProdConfig;
            }
            else
            {
                string sql = @"
                    Select null as IDPRODPROJCONFIG, pp.idProdProj as IDPRODPROJ, null as IDCORALUMINIO, null as IDCORFERRAGEM, 
                        null as IDCORVIDRO, pp.idProd as IDPROD, null as IdCorProduto, null as DescrCorProduto, 
                        p.codInterno as CodInternoProd, p.Descricao as DescrProd 
                    From produto_projeto pp 
                        Left Join produto p On (pp.IdProd=p.IdProd) 
                    Where pp.idProdProj=" + idProdProj;

                List<ProdutoProjetoConfig> lstProdConfig = objPersistence.LoadData(sql);
                foreach (ProdutoProjetoConfig ppc in lstProdConfig)
                    ppc.IdProdProj = idProdProj;

                return lstProdConfig;
            }
        }

        public ProdutoProjetoConfig GetElement(uint idProdProjConfig)
        {
            try
            {
                ProdutoProjetoConfig item = GetElementByPrimaryKey(idProdProjConfig);
                foreach (ProdutoProjetoConfig ppc in GetByProdProj(item.IdProdProj))
                    if (ppc.IdProdProjConfig == idProdProjConfig)
                        return ppc;

                return item;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Retorna o produto selecionado na associação ProdutoProjeto x Cor

        /// <summary>
        /// Retorna o produto selecionado na associação ProdutoProjeto x Cor
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <param name="tipoProd"></param>
        /// <param name="idCorProd"></param>
        /// <returns></returns>
        public uint? GetIdProd(uint idProdProj, ProdutoProjeto.TipoProduto tipoProd, uint idCorProd)
        {
            return GetIdProd(null, idProdProj, tipoProd, idCorProd);
        }

        /// <summary>
        /// Retorna o produto selecionado na associação ProdutoProjeto x Cor
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <param name="tipoProd"></param>
        /// <param name="idCorProd"></param>
        /// <returns></returns>
        public uint? GetIdProd(GDASession sessao, uint idProdProj, ProdutoProjeto.TipoProduto tipoProd, uint idCorProd)
        {
            object objIdProd = null;

            if (tipoProd == ProdutoProjeto.TipoProduto.Aluminio)
            {
                objIdProd = objPersistence.ExecuteScalar(sessao, "Select idProd From produto_projeto_config Where idProdProj=" + idProdProj + 
                    " And idCorAluminio=" + idCorProd);
            }
            else if (tipoProd == ProdutoProjeto.TipoProduto.Ferragem)
            {
                objIdProd = objPersistence.ExecuteScalar(sessao, "Select idProd From produto_projeto_config Where idProdProj=" + idProdProj +
                    " And idCorFerragem=" + idCorProd);
            }

            if (objIdProd == null || String.IsNullOrEmpty(objIdProd.ToString()))
                return null;

            return Glass.Conversoes.StrParaUint(objIdProd.ToString());
        }

        /// <summary>
        /// Retorna o produto do tipo vidro selecionado na associação ProdutoProjeto x Cor
        /// </summary>
        public uint? GetIdProdVidro(int tipoVidro, bool boxPadrao, float espessura, uint idCorProd)
        {
            return GetIdProdVidro(null, tipoVidro, boxPadrao, espessura, idCorProd);
        }

        /// <summary>
        /// Retorna o produto do tipo vidro selecionado na associação ProdutoProjeto x Cor
        /// </summary>
        public uint? GetIdProdVidro(GDASession session, int tipoVidro, bool boxPadrao, float espessura, uint idCorProd)
        {
            var idProdProj =
                ProdutoProjetoDAO.Instance.ObtemValorCampo<int>(session, "IdProdProj",
                    String.Format("Tipo={0} AND CodInterno='{1}{2}{3}'",
                        (int)ProdutoProjeto.TipoProduto.Vidro,
                        tipoVidro == 1 ? "PT" : "FX",
                        boxPadrao ? "BP" : "",
                        espessura.ToString().PadLeft(2, '0')));

            object objIdProd =
                objPersistence.ExecuteScalar(session,
                    String.Format("Select idProd From produto_projeto_config Where idProdProj={0} And idCorVidro={1}", idProdProj,
                        idCorProd));
            
            return objIdProd != null ? objIdProd.ToString().StrParaUintNullable() : null;
        }

        /// <summary>
        /// Retorna um registro do banco com base no produto e na cor.
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <param name="tipoProd"></param>
        /// <param name="idCorProd"></param>
        /// <returns></returns>
        public ProdutoProjetoConfig GetConfig(uint idProdProj, ProdutoProjeto.TipoProduto tipoProd, uint idCorProd)
        {
            string sql = "select * from produto_projeto_config where idProdProj=" + idProdProj + " and {0}=" + idCorProd;
            sql = String.Format(sql, tipoProd == ProdutoProjeto.TipoProduto.Aluminio ? "idCorAluminio" :
                tipoProd == ProdutoProjeto.TipoProduto.Ferragem ? "idCorFerragem" :
                tipoProd == ProdutoProjeto.TipoProduto.Vidro ? "idCorVidro" : idCorProd.ToString());

            List<ProdutoProjetoConfig> itens = objPersistence.LoadData(sql);
            return itens.Count > 0 ? itens[0] : null;
        }

        #endregion

        #region Aplica configurações

        /// <summary>
        /// Aplica configurações de Cor x Produto feitas em um ProdutoProjeto
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <param name="lstIdCor"></param>
        /// <param name="lstIdProd"></param>
        public void AplicarConfig(uint idProdProj, string lstIdCor, string lstIdProd)
        {
            // Verifica se o produto do projeto é alumínio ou ferragem
            int tipoProdProj = ProdutoProjetoDAO.Instance.ObtemValorCampo<int>("tipo", "idProdProj=" + idProdProj);

            string[] vetIdCor = lstIdCor.TrimEnd(',').Split(',');
            string[] vetIdProd = lstIdProd.TrimEnd(',').Split(',');

            for (int i = 0; i < vetIdCor.Length; i++)
            {
                // Busca o item associado
                bool alterarLog = false;
                bool alterarLogProdutoProjeto = false;
                ProdutoProjetoConfig item = GetConfig(idProdProj, (ProdutoProjeto.TipoProduto)tipoProdProj, Glass.Conversoes.StrParaUint(vetIdCor[i]));

                // Se produto não tiver sido associado com esta cor, continua o loop
                if (vetIdProd.Length <= i || String.IsNullOrEmpty(vetIdProd[i]) || vetIdProd[i] == "0")
                    continue;

                if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Aluminio)
                {
                    // Verifica se esta configuração de cor já existe, atualizando o produto associado se existir
                    if (item != null)
                    {
                        objPersistence.ExecuteCommand("Update produto_projeto_config Set idProd=" + vetIdProd[i] + " Where idProdProj=" + idProdProj + " And idCorAluminio=" + vetIdCor[i]);
                        alterarLog = true;
                    }
                    else
                    {
                        // Insere configuração
                        ProdutoProjetoConfig ppc = new ProdutoProjetoConfig();
                        ppc.IdCorAluminio = Glass.Conversoes.StrParaUint(vetIdCor[i]);
                        ppc.IdProd = Glass.Conversoes.StrParaUint(vetIdProd[i]);
                        ppc.IdProdProj = idProdProj;
                        ProdutoProjetoConfigDAO.Instance.Insert(ppc);
                    }
                }
                else if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Ferragem)
                {
                    // Verifica se esta configuração de cor já existe, atualizando o produto associado se existir
                    if (item != null)
                    {
                        objPersistence.ExecuteCommand("Update produto_projeto_config Set idProd=" + vetIdProd[i] + " Where idProdProj=" + idProdProj + " And idCorFerragem=" + vetIdCor[i]);
                        alterarLog = true;
                    }
                    else
                    {
                        // Insere configuração
                        ProdutoProjetoConfig ppc = new ProdutoProjetoConfig();
                        ppc.IdCorFerragem = Glass.Conversoes.StrParaUint(vetIdCor[i]);
                        ppc.IdProd = Glass.Conversoes.StrParaUint(vetIdProd[i]);
                        ppc.IdProdProj = idProdProj;
                        ProdutoProjetoConfigDAO.Instance.Insert(ppc);
                    }
                }
                else if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Vidro)
                {
                    // Verifica se esta configuração de cor já existe, atualizando o produto associado se existir
                    if (item != null)
                    {
                        objPersistence.ExecuteCommand("Update produto_projeto_config Set idProd=" + vetIdProd[i] + " Where idProdProj=" + idProdProj + " And idCorVidro=" + vetIdCor[i]);
                        alterarLog = true;
                    }
                    else
                    {
                        // Insere configuração
                        ProdutoProjetoConfig ppc = new ProdutoProjetoConfig();
                        ppc.IdCorVidro = Glass.Conversoes.StrParaUint(vetIdCor[i]);
                        ppc.IdProd = Glass.Conversoes.StrParaUint(vetIdProd[i]);
                        ppc.IdProdProj = idProdProj;
                        ProdutoProjetoConfigDAO.Instance.Insert(ppc);
                    }
                }
                else if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Outros)
                {
                    // Atualiza o idProd deste produto_projeto
                    objPersistence.ExecuteCommand("Update produto_projeto Set idProd=" + Glass.Conversoes.StrParaUint(lstIdProd.Trim(',')) +
                        " Where idProdProj=" + idProdProj);

                    alterarLogProdutoProjeto = true;
                }
                else
                    throw new Exception("Este produto não pode ser configurado.");

                if (alterarLog)
                    LogAlteracaoDAO.Instance.LogProdutoProjetoConfig(item);
                else if (alterarLogProdutoProjeto)
                    LogAlteracaoDAO.Instance.LogProdutoProjeto(ProdutoProjetoDAO.Instance.GetElementByPrimaryKey(idProdProj));

                if (tipoProdProj == (int)ProdutoProjeto.TipoProduto.Outros)
                    break;
            }
        }

        #endregion
    }
}