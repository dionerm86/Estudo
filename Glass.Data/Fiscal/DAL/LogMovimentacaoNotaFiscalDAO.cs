using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class LogMovimentacaoNotaFiscalDAO : BaseDAO<LogMovimentacaoNotaFiscal, LogMovimentacaoNotaFiscalDAO>
    {
        /// <summary>
        /// Retorna os logs da nota passada
        /// </summary>
        public List<LogMovimentacaoNotaFiscal> ObtemLogsNotaFiscal(uint idNf)
        {
            var sql = string.Format(@"SELECT * FROM 
                            (SELECT nf.NUMERONFE, NULL AS DESCRICAOPROD, lmnf.MENSAGEMLOG, lmnf.DataCad
                                FROM nota_fiscal nf
                                INNER JOIN log_movimentacao_nota_fiscal lmnf ON lmnf.IDNF=nf.IDNF
                                where nf.idnf = {0}
                            UNION 
                                SELECT NULL AS NUMERONFE, p.DESCRICAO AS DESCRICAOPROD, lmnf.MENSAGEMLOG, lmnf.DataCad
                                FROM  produtos_nf pnf 
                                INNER JOIN produto p ON pnf.IDPROD=p.IDPROD
                                INNER JOIN log_movimentacao_nota_fiscal lmnf ON lmnf.IDNF=pnf.IDNF AND lmnf.IDPRODNF=pnf.IDPRODNF
                                where pnf.IDNF= {0}) tmp
                            ORDER BY DESCRICAOPROD", idNf);

            var retorno = objPersistence.LoadData(sql).ToList();

            return retorno;
        }

        /// <summary>
        /// Deleta os registros da nota 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idNf"></param>
        public void DeleteFromNf(GDASession session, uint idNf)
        {
            objPersistence.ExecuteCommand(session, "DELETE FROM log_movimentacao_nota_fiscal WHERE idNf = " + idNf);
        }
    }
}
