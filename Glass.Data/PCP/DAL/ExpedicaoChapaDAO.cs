using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ExpedicaoChapaDAO : BaseDAO<ExpedicaoChapa, ExpedicaoChapaDAO>
    {
        /// <summary>
        /// Recupera a quantidade de chapas que ja foi expedida de um pedido
        /// </summary>
        public int ObtemQuantidadeExpedida(GDASession session, int idPedido, int idProd)
        {
            string sql = @"
                SELECT COUNT(distinct ec.IdExpChapa) 
                FROM expedicao_chapa ec
                    INNER join produto_impressao pi ON (pi.IdProdImpressao = ec.IdProdImpressaoChapa)
                    INNER JOIN produtos_nf pnf ON (pnf.IdProdNf = pi.IdProdNf)
                WHERE ec.IdPedido = " + idPedido + @"
                    AND pnf.idProd = " + idProd;

            return objPersistence.ExecuteSqlQueryCount(session, sql);
        }

        /// <summary>
        /// Verifica se a etiqueta informada já deu saída na expedição
        /// </summary>
        public bool VerificaLeitura(string numEtiqueta)
        {
            return VerificaLeitura(null, numEtiqueta);
        }

        /// <summary>
        /// Verifica se a etiqueta informada já deu saída na expedição
        /// </summary>
        public bool VerificaLeitura(GDASession session, string numEtiqueta)
        {
            var sql = @"
                SELECT count(*)
                FROM expedicao_chapa ex
                    INNER JOIN produto_impressao pi ON (pi.IdProdImpressao = ex.IdProdImpressaoChapa AND !pi.cancelado)
                WHERE pi.numEtiqueta = ?etq";

            return objPersistence.ExecuteSqlQueryCount(session, sql, new GDAParameter("?etq", numEtiqueta)) > 0;
        }

        public void DeleteByIdProdImpressaoChapa(GDASession sessao, uint IdProdImpressaoChapa)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM expedicao_chapa WHERE IdProdImpressaoChapa = " + IdProdImpressaoChapa);
        }

        public int ObtemPedidoExpedicao(GDASession sessao, uint IdProdImpressaoChapa)
        {
            return ExecuteScalar<int>(sessao, "SELECT IdPedido FROM expedicao_chapa WHERE IdProdImpressaoChapa = " + IdProdImpressaoChapa);
        }
    }
}
