using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class CidadeDAO : BaseDAO<Cidade, CidadeDAO>
    {
        //private CidadeDAO() { }

        private string SqlList(string uf, string cidade, bool selecionar)
        {
            string campos = selecionar ? "c.*" : "Count(*)";

            string sql = "Select " + campos + " From cidade c Where 1";

            if (!String.IsNullOrEmpty(uf) && uf != "0")
                sql += " And c.NomeUf=?uf";

            if (!String.IsNullOrEmpty(cidade))
                sql += " And c.NomeCidade Like ?cidade";

            return sql;
        }

        public IList<Cidade> GetList(string uf, string cidade, string sortExpression, int startRow, int pageSize)
        {
            return GetList(null, uf, cidade, sortExpression, startRow, pageSize);
        }

        public IList<Cidade> GetList(GDASession sessao, string uf, string cidade, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "case c.NomeCidade when 'Belo Horizonte' then 1 " +
                "when 'Contagem' then 2 when 'Betim' then 3 else 9999 end, NomeUf, NomeCidade" : sortExpression;

            return LoadDataWithSortExpression(SqlList(uf, cidade, true), filtro, startRow, pageSize, GetParam(uf, cidade));
        }

        public int GetCount(string uf, string cidade)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(uf, cidade, false), GetParam(uf, cidade));
        }

        private GDAParameter[] GetParam(string uf, string cidade)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(uf))
                lstParam.Add(new GDAParameter("?uf", uf));

            if (!String.IsNullOrEmpty(cidade))
                lstParam.Add(new GDAParameter("?cidade", "%" + cidade + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Cidade> GetByUf(string uf)
        {
            return objPersistence.LoadData(SqlList(uf, null, true), GetParam(uf, null)).ToList();
        }

        /// <summary>
        /// Retorna o nome da cidade pelo seu id
        /// </summary>
        /// <param name="idCidade"></param>
        /// <returns></returns>
        public string GetNome(uint? idCidade)
        {
            return GetNome(null, idCidade);
        }

        /// <summary>
        /// Retorna o nome da cidade pelo seu id
        /// </summary>
        /// <param name="idCidade"></param>
        /// <returns></returns>
        public string GetNome(GDASession session, uint? idCidade)
        {
            if (idCidade.GetValueOrDefault() == 0)
                return String.Empty;

            string nomeCidade = ExecuteScalar<string>(session, "Select nomeCidade From cidade Where idCidade=" + idCidade, null);

            return nomeCidade != null ? nomeCidade.Replace("'", "") : String.Empty;
        }

        public string GetNomeUf(int idCidade)
        {
            return GetNomeUf(null, (uint)idCidade);
        }

        public string GetNomeUf(GDASession sessao, uint? idCidade)
        {
            if (idCidade.GetValueOrDefault() == 0)
                return String.Empty;

            return ObtemValorCampo<string>(sessao, "nomeUf", "idCidade=" + idCidade);
        }

        /// <summary>
        /// Retorna o id da cidade pelo seu nome e UF
        /// </summary>
        /// <param name="nomeCidade"></param>
        /// <param name="uf"></param>
        /// <returns></returns>
        public uint GetCidadeByNomeUf(string nomeCidade, string uf)
        {
            string sql = "Select idCidade From cidade c Where NomeUf=?uf And c.NomeCidade Like ?cidade limit 1";

            object obj = objPersistence.ExecuteScalar(sql, new GDAParameter("?uf", uf), new GDAParameter("?cidade", nomeCidade));

            if (obj == null || obj.ToString() == String.Empty)
                return 0;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        public string GetCidadeByCodIBGE(string codCidade, string codUF)
        {
            string sql = "select idcidade from cidade where codibgecidade ='" + codCidade + "' and codibgeuf = '" + codUF + "'";
            object retorno = objPersistence.ExecuteScalar(sql);

            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        private static volatile IList<string> _uf;

        public KeyValuePair<string, string>[] GetUf()
        {
            return GetUf((GDASession)null);
        }

        public KeyValuePair<string, string>[] GetUf(GDASession session)
        {
            if (_uf == null)
            {
                _uf = ExecuteMultipleScalar<string>(session, @"select distinct nomeUf from cidade
                    order by case nomeUf when 'MG' then '' else nomeUf end");
            }

            return _uf.Select(x => new KeyValuePair<string, string>(x, x)).ToArray();
        }

        public KeyValuePair<string, string>[] GetUfLojas()
        {
            var uf = ExecuteMultipleScalar<string>(@"select distinct nomeUf from cidade
                where idCidade in (select * from (select idCidade from loja) as temp)");

            return uf.Select(x => new KeyValuePair<string, string>(x, x)).ToArray();
        }

        public bool IsCidadeExterior(uint idCidade)
        {
            string codIbgeUf = ObtemValorCampo<string>("codIbgeUf", "idCidade=" + idCidade);
            return codIbgeUf == "99";
        }

        public string ObtemCodIbgeCompleto(GDASession sessao, uint idCidade)
        {
            return ObtemValorCampo<string>(sessao, "concat(codIbgeUf, codIbgeCidade)", "idCidade=" + idCidade);
        }

        /// <summary>
        /// Recupera o codigo do ibge pelo estado
        /// </summary>
        /// <param name="uf"></param>
        /// <returns></returns>
        public string GetCodIbgeEstadoByEstado(string uf)
        {
            return ObtemValorCampo<string>("codIbgeUf", "nomeUf='" + uf + "' limit 1");
        }

        public IList<Cidade> GetList()
        {
            string sql = "select * from cidade order by NomeCidade Asc";

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Cidade> ObterUF()
        {
            return objPersistence.LoadData("SELECT * FROM cidade GROUP BY CodIbgeUf").ToList();
        }

        public IList<Cidade> ObterCidadesParaMdfe()
        {
            string sql = "select IdCidade, CONCAT(NomeCidade, ' - ', NomeUf) AS NomeCidade, CodIbgeCidade, NomeUf, CodIbgeUf from cidade order by NomeCidade Asc";
            return objPersistence.LoadData(sql).ToList();
        }

    }
}
