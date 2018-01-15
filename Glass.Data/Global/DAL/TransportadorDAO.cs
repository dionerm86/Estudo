using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TransportadorDAO : BaseDAO<Transportador, TransportadorDAO>
	{
        //private TransportadorDAO() { }

        #region Listagem padrão de Transportadores

        private string Sql(uint idTransp, string nomeTransp, string cpfCnpj, bool selecionar)
        {
            string campos = selecionar ? "t.*, cid.NomeCidade as nomeCidade, cid.NomeUf as uf" : "Count(*)";

            string sql = "Select " + campos + " From transportador t " +
                "Left Join cidade cid On (cid.idCidade=t.idCidade) Where 1";

            if (idTransp > 0)
                sql += " And t.IdTransportador=" + idTransp;

            if (!String.IsNullOrEmpty(nomeTransp))
                sql += " And (t.Nome Like ?nome OR t.NomeFantasia Like ?nome)";

            if (!string.IsNullOrEmpty(cpfCnpj))
                sql += " And Replace(Replace(Replace(CpfCnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";

            return sql;
        }

        public Transportador GetElement(uint idTransp)
        {
            return objPersistence.LoadOneData(Sql(idTransp, null, null, true));
        }

        public IList<Transportador> GetList(uint idTransp, string nomeTransp, string cpjCnpj,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idTransp, nomeTransp, cpjCnpj, true), sortExpression, startRow, pageSize,
                GetParam(nomeTransp, cpjCnpj));
        }

        public IList<Transportador> GetList(uint idTransp, string nomeTransp)
        {
            return objPersistence.LoadData(Sql(idTransp, nomeTransp, null, true), GetParam(nomeTransp, null)).ToList();
        }

        public int GetCount(uint idTransp, string nomeTransp, string cpjCnpj)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idTransp, nomeTransp, cpjCnpj, false), GetParam(nomeTransp, cpjCnpj));
        }

        private GDAParameter[] GetParam(string nomeTransp, string cpfCnpj)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeTransp))
                lstParam.Add(new GDAParameter("?nome", "%" + nomeTransp + "%"));

            if (!String.IsNullOrEmpty(cpfCnpj))
                lstParam.Add(new GDAParameter("?cpfCnpj", Formatacoes.LimpaCpfCnpj(cpfCnpj)));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Transportador> GetOrdered()
        {
            return objPersistence.LoadData("Select * From transportador Order By nome").ToList();
        }

        #endregion

        #region Busca transportadoras para EFD

        /// <summary>
        /// Busca transportadoras para montar arquivo EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Transportador GetForEFD(uint idTransportador)
        {
            return objPersistence.LoadOneData(@"select t.*, cid.codIbgeUf, cid.codIbgeCidade, cid.nomeUF as UF
                from transportador t left join cidade cid on (cid.idCidade=t.idCidade)
                where t.idTransportador=" + idTransportador);
        }

        #endregion

        #region Verifica se transportador já existe

        /// <summary>
        /// Verifica se já existe um transportador cadastrado com o CNPJ cadastrado
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public bool CheckIfExists(string cpfCnpj)
        {
            string sql = "Select Count(*) From transportador Where " +
                "Replace(Replace(Replace(CpfCnpj, '.', ''), '-', ''), '/', '')='" + cpfCnpj + "' or cpfcnpj='" + cpfCnpj + "'";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteSqlQueryCount(sql).ToString()) > 0;
        }

        #endregion

        #region Retorna o nome do transportador

        /// <summary>
        /// Retorna o nome do transportador.
        /// </summary>
        public string GetNome(uint idTransportador)
        {
            return GetNome(null, idTransportador);
        }

        /// <summary>
        /// Retorna o nome do transportador.
        /// </summary>
        public string GetNome(GDASession session, uint idTransportador)
        {
            string sql = "select nome from transportador where idTransportador=" + idTransportador;
            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        #endregion

        /// <summary>
        /// Retorna o id do transportador, buscando pelo CNPJ.
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public string GetIDByCNPJ(string cnpj)
        {
            string sql = "select idTransportador from transportador where cpfcnpj='" + cnpj +"'";
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        public override int Update(Transportador objUpdate)
        {
            LogAlteracaoDAO.Instance.LogTransportador(objUpdate);

            return base.Update(objUpdate);
        }
	}
}