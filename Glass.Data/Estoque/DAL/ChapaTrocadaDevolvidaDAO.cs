using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa os métodos utilizados por chapa trocada devolvida
    /// </summary>
    public class ChapaTrocadaDevolvidaDAO : BaseDAO<ChapaTrocadaDevolvida, ChapaTrocadaDevolvidaDAO>
    {
        //Verifica se a chapa está disponivel para uso
        public bool VerificarChapaDisponivel(GDASession session, string numEtiqueta)
        {
            var sql = string.Format("Select COUNT(*) From chapa_trocada_devolvida where NumEtiqueta={0} AND Situacao={1} Order by IdChapaTrocadaDevolvida desc",
                "'" + numEtiqueta + "'", (int)SituacaoChapaTrocadaDevolvida.Disponivel);

            var chapaDisponivel = objPersistence.ExecuteSqlQueryCount(session, sql);

            return chapaDisponivel > 0;
        }

        //Verifica alguma chapa das passadas não está disponivel para uso
        public bool VerificarChapaDisponivel(GDASession session, IList<int> idsProdImpressaoChapa)
        {
            var sql = string.Format("Select COUNT(*) From chapa_trocada_devolvida ctd where IdProdImpressaoChapa IN ({0}) AND Situacao IN({1}) group by IdProdImpressaoChapa Order by IdChapaTrocadaDevolvida desc",
                string.Join(",", idsProdImpressaoChapa), (int)SituacaoChapaTrocadaDevolvida.Utilizada);

            var chapasUtilizadas = objPersistence.ExecuteSqlQueryCount(session, sql);
            
            return chapasUtilizadas == 0;
        }

        /// <summary>
        /// Busca todas as etiquetas da troca e devolução que já foram utilizadas
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public string BuscarEtiquetasJaEntreguesPelaTrocaDevolucao(GDASession sessao, int idTrocaDevolucao)
        {
            var sql = string.Format("Select NumEtiqueta from chapa_trocada_devolvida where IdTrocaDevolucao={0} AND Situacao={1}", + idTrocaDevolucao, (int)SituacaoChapaTrocadaDevolvida.Utilizada);

            return string.Join(",", ExecuteMultipleScalar<string>(sessao, sql));
        }

        /// <summary>
        /// Marca a chapa trocada devolvida como utilizada (apenas a disponivel)
        /// </summary>
        public void MarcarChapaComoUtilizada(GDASession sessao, string numEtiqueta)
        {
            var sql = string.Format("UPDATE chapa_trocada_devolvida SET Situacao={0}  WHERE Situacao={1} AND NumEtiqueta={2}",
                (int)SituacaoChapaTrocadaDevolvida.Utilizada,
                (int)SituacaoChapaTrocadaDevolvida.Disponivel,
                ("'" + numEtiqueta + "'"));

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// marca a chapa trocada devolvida como disponivel (apenas a utilizada)
        /// </summary>
        public void MarcarChapaComoDisponivel(GDASession sessao, string numEtiqueta)
        {
            var sql = string.Format("UPDATE chapa_trocada_devolvida SET Situacao={0}  WHERE Situacao={1} AND NumEtiqueta={2}",
                (int)SituacaoChapaTrocadaDevolvida.Disponivel,
                (int)SituacaoChapaTrocadaDevolvida.Utilizada,
                ("'" + numEtiqueta + "'"));

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #region Métodos Sobrescritos

        /// <summary>
        /// Insere uma chapa trocada/devolvida
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertComTransacao(ChapaTrocadaDevolvida objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

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

        /// <summary>
        /// Atualiza uma chapa trocada/devolvida
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <returns></returns>
        public int UpdateComTransacao(ChapaTrocadaDevolvida objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

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
        #endregion

    }
}
