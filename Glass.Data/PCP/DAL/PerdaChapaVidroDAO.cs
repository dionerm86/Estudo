using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PerdaChapaVidroDAO : BaseDAO<PerdaChapaVidro, PerdaChapaVidroDAO>
	{
        //private PerdaChapaVidroDAO() { }

        private static readonly object _cancelar = new object();
        private static readonly object _marcaPerdaChapaVidro = new object();

        #region Recupera Listagem

        private string sqlPerdaChapa(uint idPerdaChapaVidro, string idsProdImpressao, uint idTipoPeda, uint idSubTipoPerda,
            string dataIni, string dataFim, string numEtiqueta, bool cancelado, bool selecionar)
        {
            string campos = selecionar ? @"
                pcv.*, pi.idNf, pi.IdprodNf, pi.idImpressao, pnf.Idprod, pi.posicaoProd, pi.ItemEtiqueta, pi.qtdeProd,
                CONCAT(p.CodInterno, ' - ', p.Descricao) as DescrProd, tp.Descricao as TipoPerda, stp.Descricao as SubtipoPerda, pi.numEtiqueta,
                f.Nome as FuncPerda" : "count(*)";

            string sql = @"
                SELECT " + campos + @"
                FROM perda_chapa_vidro pcv
                    LEFT JOIN produto_impressao pi ON (pcv.idProdImpressao = pi.idProdImpressao)
                    LEFT JOIN produtos_nf pnf ON (pi.idprodnf = pnf.idprodnf)
                    LEFT JOIN produto P ON (pnf.IdProd = p.idProd)
                    LEFT JOIN tipo_perda tp ON (pcv.idTipoPerda = tp.idTipoPerda)
                    LEFT JOIN subtipo_perda stp ON (pcv.idSubtipoPerda = stp.idSubtipoPerda)
                    LEFT JOIN funcionario f ON (pcv.IdFuncPerda = f.IdFunc)
                WHERE 1";

            if (cancelado)
                sql += " and pcv.cancelado=true";

            if (idPerdaChapaVidro > 0)
                sql += " and pcv.idPerdaChapaVidro= " + idPerdaChapaVidro;

            if (!string.IsNullOrEmpty(idsProdImpressao))
                sql += " and pcv.idProdImpressao in (" + idsProdImpressao + ")";

            if (idTipoPeda > 0)
                sql += " and pcv.idTipoPerda=" + idTipoPeda;

            if (idSubTipoPerda > 0)
                sql += " and pcv.idSubTipoPerda=" + idSubTipoPerda;

            if (!string.IsNullOrEmpty(dataIni))
                sql += " and pcv.dataPerda >=?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " and pcv.dataPerda <=?dataFim";

            if (!string.IsNullOrEmpty(numEtiqueta))
            {
                string[] dadosEtiqueta = numEtiqueta.Split('-', '.', '/');

                sql += " AND pi.IDNF = " + dadosEtiqueta[0].Substring(1) + @"
                        AND pi.POSICAOPROD = " + dadosEtiqueta[1] + @"
                        AND pi.ITEMETIQUETA = " + dadosEtiqueta[2] + @"
                        AND pi.QTDEPROD= " + dadosEtiqueta[3];
            }

            return sql;
        }

        /// <summary>
        /// Recupera a lista de perdas da chapa de vidro
        /// </summary>
        /// <param name="idPerdaChapaVidro"></param>
        /// <param name="idImpressaoProduto"></param>
        /// <param name="idTipoPeda"></param>
        /// <param name="idSubTipoPerda"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="numEtiqueta"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<PerdaChapaVidro> GetListPerdaChapaVidro(uint idPerdaChapaVidro, string idsProdImpressao, uint idTipoPeda, uint idSubTipoPerda,
            string dataIni, string dataFim, string numEtiqueta, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "IdPerdaChapaVidro DESC";

            return LoadDataWithSortExpression(sqlPerdaChapa(idPerdaChapaVidro, idsProdImpressao, idTipoPeda, idSubTipoPerda,
            dataIni, dataFim, numEtiqueta, false, true), sortExpression, startRow, pageSize, GetParam(dataIni, dataFim));

        }

        public int GetListPerdaChapaVidroCount(uint idPerdaChapaVidro, string idsProdImpressao, uint idTipoPeda, uint idSubTipoPerda,
           string dataIni, string dataFim, string numEtiqueta)
        {

            return objPersistence.ExecuteSqlQueryCount(sqlPerdaChapa(idPerdaChapaVidro, idsProdImpressao, idTipoPeda, idSubTipoPerda,
            dataIni, dataFim, numEtiqueta, false, false), GetParam(dataIni, dataFim));

        }

        public PerdaChapaVidro GetPerdaChapaVidro(GDASession sessao, uint idPerdaChapaVidro)
        {
            return objPersistence.LoadOneData(sessao, sqlPerdaChapa(idPerdaChapaVidro, null, 0, 0, null, null, null, false, true));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:00")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Validações

        /// <summary>
        /// Verifica se a etiqueta é valida.
        /// </summary>
        public string ValidaEtiqueta(GDASession session, string numEtiqueta)
        {
            try
            {
                if (string.IsNullOrEmpty(numEtiqueta))
                    return "Informe o número da etiqueta";

                if (!ProdutoImpressaoDAO.Instance.IsChapaVidro(session, numEtiqueta))
                    return "O produto da etiqueta informada não é do sub-grupo Chapas de Vidro";

                string[] dadosEtiqueta = numEtiqueta.Split('-', '.', '/');

                if (dadosEtiqueta[0][0].ToString().ToUpper() != "N")
                    return "A etiqueta informada não é de uma nota fiscal.";

                if (!NotaFiscalDAO.Instance.Exists(session, dadosEtiqueta[0].Substring(1).StrParaUint()))
                    return "A nota fiscal da etiqueta informada não existe.";

                if(!ProdutoImpressaoDAO.Instance.EstaImpressa(session, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal))
                    return "Esta etiqueta não foi impressa.";

                uint idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(session, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);

                if (ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(session, idProdImpressao))
                    return "Não é possivel marcar perda em uma etiqueta que já houve leitura";

                if(dadosEtiqueta[3].StrParaInt() > dadosEtiqueta[3].StrParaInt())
                    return ("Etiqueta não pode ser do tipo '1-1.2/1'. Identificador da peça maior que a quantidade de produtos. Etiqueta: " + numEtiqueta);

                return "ok";

            }
            catch (Exception ex)
            {
                return "Falha na verificação da etiqueta. " + ex.Message;
            }
        }

        /// <summary>
        /// Verifica se a perda da etiqueta esta cancelada
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool IsCancelada(string numEtiqueta)
        {
            try
            {
                string[] dadosEtiqueta = numEtiqueta.Split('-', '.', '/');

                string sql = @"
                                SELECT COUNT(*) 
                                FROM perda_chapa_vidro pcv
                                INNER JOIN produto_impressao pi on (pcv.IDPRODIMPRESSAO = pi.IDPRODIMPRESSAO)
                                WHERE pi.IDNF = " + dadosEtiqueta[0].Substring(1) + @"
                                      AND pi.POSICAOPROD = " + dadosEtiqueta[1] + @"
                                      AND pi.ITEMETIQUETA = " + dadosEtiqueta[2] + @"
                                      AND pi.QTDEPROD= " + dadosEtiqueta[3] + @"
                                      AND pcv.CANCELADO = true";

                return objPersistence.ExecuteSqlQueryCount(sql) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// verifica se a etiqueta foi marcada perda
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool IsPerda(string numEtiqueta)
        {
            return IsPerda(null, numEtiqueta);
        }

        /// <summary>
        /// verifica se a etiqueta foi marcada perda
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool IsPerda(GDASession sessao, string numEtiqueta)
        {
            try
            {
                string[] dadosEtiqueta = numEtiqueta.Split('-', '.', '/');

                if (!char.IsNumber(dadosEtiqueta[0][0]))
                {
                    if (dadosEtiqueta[0].Length <= 1)
                        throw new Exception($"Etiqueta {numEtiqueta} fora de padrão");

                    dadosEtiqueta[0] = dadosEtiqueta[0].Substring(1);
                }

                string sql = @"
                                SELECT COUNT(*) 
                                FROM perda_chapa_vidro pcv
                                INNER JOIN produto_impressao pi on (pcv.IDPRODIMPRESSAO = pi.IDPRODIMPRESSAO)
                                WHERE pi.IDNF = " + dadosEtiqueta[0] + @"
                                      AND pi.POSICAOPROD = " + dadosEtiqueta[1] + @"
                                      AND pi.ITEMETIQUETA = " + dadosEtiqueta[2] + @"
                                      AND pi.QTDEPROD= " + dadosEtiqueta[3] + @"
                                      AND (pcv.CANCELADO = false OR pcv.CANCELADO is null)";

                return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Marca Perda na Chapa de Vidro

        /// <summary>
        /// Marca a perda em uma chapa pelo número da etiqueta
        /// </summary>
        public PerdaChapaVidro MarcaPerdaChapaVidroComTransacao(string numEtiqueta, uint idTipoPerda, uint? idSubTipoPerda, string obs)
        {
            lock(_marcaPerdaChapaVidro)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var retorno = MarcaPerdaChapaVidro(transaction, numEtiqueta, idTipoPerda, idSubTipoPerda, obs);
                        
                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Marca a perda em uma chapa pelo número da etiqueta
        /// </summary>
        public PerdaChapaVidro MarcaPerdaChapaVidro(GDASession sessao, string numEtiqueta, uint idTipoPerda, uint? idSubTipoPerda, string obs)
        {
            string valida = ValidaEtiqueta(sessao, numEtiqueta);

            if (valida != "ok")
                throw new Exception(valida);

            if(IsPerda(sessao, numEtiqueta))
                throw new Exception("Já foi marcado perda na etiqueta informada");

            if (idTipoPerda < 1)
                throw new Exception("O tipo de perda deve ser informado.");

            if(Configuracoes.ProducaoConfig.ObrigarMotivoPerda && string.IsNullOrWhiteSpace(obs))
                throw new Exception("O motivo da perda deve ser informado.");

            PerdaChapaVidro pcv = new PerdaChapaVidro();
            pcv.IdProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);
            pcv.IdTipoPerda = idTipoPerda;
            pcv.IdSubTipoPerda = idSubTipoPerda.GetValueOrDefault();
            pcv.DataPerda = DateTime.Now;
            pcv.IdFuncPerda = UserInfo.GetUserInfo.CodUser;
            pcv.Obs = obs;

            uint idPerdaChapaVidro = Insert(sessao, pcv);

            pcv = GetPerdaChapaVidro(sessao, idPerdaChapaVidro);

            var idNf = ProdutosNfDAO.Instance.ObtemIdNf(sessao, pcv.IdProdNf.Value);

            if (idNf == 0)
                throw new Exception("Não foi possível recuperar a nota fiscal.");

            var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf);

            if (idLoja == 0)
                throw new Exception("Não foi possível recuperar a loja da nota fiscal.");

            MovEstoqueDAO.Instance.BaixaEstoquePerdaChapa(sessao, Convert.ToUInt32(pcv.IdProd), pcv.IdProdNf.Value, idLoja, idPerdaChapaVidro);

            //Movimenta o estoque da materia-prima
            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPerdaChapaVidro(sessao, (int)pcv.IdProd, (int)pcv.IdProdNf.Value, (int)idPerdaChapaVidro);

            return pcv;
        }

        public uint MarcaPerdaRetalho(GDASession sessao, string numEtiqueta, uint idProdImpressao, uint idTipoPerda, uint? idSubTipoPerda, string obs)
        {
            try
            {
                if (Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda && !string.IsNullOrEmpty(obs))
                    throw new Exception("O motivo da perda deve ser informado.");

                PerdaChapaVidro pcv = new PerdaChapaVidro();
                pcv.IdProdImpressao = idProdImpressao;
                pcv.IdTipoPerda = idTipoPerda;
                pcv.IdSubTipoPerda = idSubTipoPerda.GetValueOrDefault(0);
                pcv.DataPerda = DateTime.Now;
                pcv.IdFuncPerda = UserInfo.GetUserInfo.CodUser;
                pcv.Obs = obs;

                var idPerdaChapaVidro = Insert(sessao, pcv);

                return idPerdaChapaVidro;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Obtem Campos

        /// <summary>
        /// Obtem o número da etiqueta da perda
        /// </summary>
        /// <param name="idPerdaChapaVidro"></param>
        /// <returns></returns>
        public string ObtemNumEtiqueta(uint idPerdaChapaVidro)
        {
            string sql = @"
                SELECT pi.NUMETIQUETA
                FROM perda_chapa_vidro pcv
                INNER JOIN produto_impressao pi ON (pcv.IDPRODIMPRESSAO = pi.IDPRODIMPRESSAO)
                WHERE pcv.IDPERDACHAPAVIDRO = " + idPerdaChapaVidro;

            object retorno = objPersistence.ExecuteScalar(sql);

            return retorno != null ? retorno.ToString() : "";
        }

        #endregion

        #region Cancelamento

        /// <summary>
        /// Cancela uma perda de chapa de vidro.
        /// </summary>
        /// <param name="perdaChapaVidro">A perda de chapa de vidro que será cancelada.</param>
        public void Cancelar(PerdaChapaVidro perdaChapaVidro)
        {
            using (var session = new GDATransaction())
            {
                try
                {
                    session.BeginTransaction();

                    this.Cancelar(session, perdaChapaVidro);

                    session.Commit();
                    session.Close();
                }
                catch
                {
                    session.Rollback();
                    session.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Cancela uma perda de chapa de vidro.
        /// </summary>
        /// <param name="session">A sessão atual.</param>
        /// <param name="perdaChapaVidro">A perda de chapa de vidro que será cancelada.</param>
        public void Cancelar(GDASession session, PerdaChapaVidro perdaChapaVidro)
        {
            lock (_cancelar)
            {
                var pcv = this.GetPerdaChapaVidro(session, perdaChapaVidro.IdPerdaChapaVidro);

                var idNf = ProdutosNfDAO.Instance.ObtemIdNf(session, pcv.IdProdNf.Value);

                if (idNf == 0)
                {
                    throw new Exception("Não foi possível recuperar a nota fiscal.");
                }

                var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(session, idNf);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja da nota fiscal.");
                }

                // Credita o estoque da chapa
                MovEstoqueDAO.Instance.CreditaEstoquePerdaChapa(session, pcv.IdProd, pcv.IdProdNf.Value, idLoja, pcv.IdPerdaChapaVidro);

                // Marca a perda como cancelada.
                this.objPersistence.ExecuteCommand(session, "UPDATE perda_chapa_vidro SET cancelado = 1 WHERE IdPerdaChapaVidro = " + perdaChapaVidro.IdPerdaChapaVidro);

                LogCancelamentoDAO.Instance.LogPerdaChapaVidro(session, pcv);
            }
        }

        #endregion
    }
}
