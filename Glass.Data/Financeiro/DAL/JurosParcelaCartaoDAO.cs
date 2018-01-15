using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class JurosParcelaCartaoDAO : BaseDAO<JurosParcelaCartao, JurosParcelaCartaoDAO>
    {
        //private JurosParcelaCartaoDAO() { }

        private string Sql(uint idTipoCartao, uint idLoja, int numParc, bool selecionar)
        {
            string campos = selecionar ? "idJurosParcela, idTipoCartao, idLoja, numParc, juros" : "count(*)";
            string sqlBase = "select " + campos + @"
                from juros_parcela_cartao jpc
                where idLoja{0}";

            if (idTipoCartao > 0)
                sqlBase += " and idTipoCartao=" + idTipoCartao;

            if (numParc > 0)
                sqlBase += " and numParc=" + numParc;

            string sql = String.Format(sqlBase, idLoja > 0 ? "=" + idLoja : " is null");
            if (idLoja > 0)
                sql += " union " + String.Format(sqlBase, " is null") + @"
                    and (select count(*) from juros_parcela_cartao where idTipoCartao=jpc.idTipoCartao
                    and numParc=jpc.numParc and idLoja=" + idLoja + ")=0";

            return sql;
        }

        public IList<JurosParcelaCartao> GetByTipoCartao(uint idTipoCartao, uint idLoja)
        {
            return objPersistence.LoadData(Sql(idTipoCartao, idLoja, 0, true)).ToList();
        }

        public bool TemParcela(uint idTipoCartao, uint idLoja, int numParc)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idTipoCartao, idLoja, numParc, false)) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <param name="idLoja"></param>
        /// <param name="numParc"></param>
        /// <returns></returns>
        public JurosParcelaCartao GetByTipoCartaoNumParc(uint idTipoCartao, uint idLoja, int numParc)
        {
            return GetByTipoCartaoNumParc(null, idTipoCartao, idLoja, numParc);
        }

        public JurosParcelaCartao GetByTipoCartaoNumParc(GDASession sessao, uint idTipoCartao, uint idLoja, int numParc)
        {
            List<JurosParcelaCartao> juros = objPersistence.LoadData(sessao, Sql(idTipoCartao, idLoja, numParc, true));
            return juros.Count > 0 ? juros[0] : new JurosParcelaCartao(idTipoCartao, idLoja, numParc);
        }

        public void AlteraJurosParc(uint idTipoCartao, uint idLoja, int numParc, float juros)
        {
            int linhas = objPersistence.ExecuteCommand("update juros_parcela_cartao set juros=?juros where idTipoCartao=" + idTipoCartao +
                " and idLoja=" + idLoja + " and numParc=" + numParc, new GDAParameter("?juros", juros));

            if (linhas == 0)
            {
                JurosParcelaCartao novo = new JurosParcelaCartao();
                novo.IdTipoCartao = (int)idTipoCartao;
                novo.IdLoja = idLoja;
                novo.NumParc = numParc;
                novo.Juros = juros;

                Insert(novo);
            }
        }

        public string ObtemDescricaoJurosParcelas(GDASession session, int idTipoCartao, int idLoja)
        {
            var sql =
                string.Format(
                    @"SELECT CONCAT(jpc.NumParc, 'ª - ', jpc.Juros, '%') FROM juros_parcela_cartao jpc
                    WHERE jpc.IdTipoCartao={0} AND jpc.IdLoja={1}", idTipoCartao, idLoja);

            var retorno = ExecuteMultipleScalar<string>(session, sql);

            return
                retorno != null && retorno.Count > 0 ?
                    string.Join("; ", retorno) :
                    string.Empty;
        }

        /// <summary>
        /// Remove os juros de parcela de cartão, pelo ID do cartão.
        /// </summary>
        public void ApagarPeloTipoCartaoCredito(GDASession session, int idTipoCartao)
        {
            objPersistence.ExecuteCommand(session, string.Format("DELETE FROM juros_parcela_cartao WHERE IdTipoCartao={0}", idTipoCartao));
        }
    }
}
