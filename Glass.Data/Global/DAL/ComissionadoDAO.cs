using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ComissionadoDAO : BaseCadastroDAO<Comissionado, ComissionadoDAO>
	{
        //private ComissionadoDAO() { }

        #region Retorna Comissionados

        private string Sql(uint idComissionado, string nome, int situacao, bool selecionar)
        {
            string campos = selecionar ? "c.*, if(c.idCidade is null, c.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf" : "Count(*)";

            string sql = "Select " + campos + " From comissionado c " +
                "Left Join cidade cid On (cid.idCidade=c.idCidade) Where 1";

            if (idComissionado > 0)
                sql += " And idComissionado=" + idComissionado;

            if (!String.IsNullOrEmpty(nome))
                sql += " And Nome Like ?nome";

            if (situacao > 0)
                sql += " And c.situacao=" + situacao;

            return sql;
        }

        public Comissionado GetElement(uint idComissionado)
        {
            return objPersistence.LoadOneData(Sql(idComissionado, null, 0, true));
        }

        public IList<Comissionado> GetList(string nome, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "Nome" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, nome, situacao, true), filtro, startRow, pageSize, GetParam(nome));
        }

        public int GetCount(string nome, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, nome, situacao, false), GetParam(nome));
        }

        public IList<Comissionado> GetListForRpt(string nome, int situacao)
        {
            return objPersistence.LoadData(Sql(0, nome, situacao, true)).ToList();
        }

        public IList<Comissionado> GetOrdered()
        {
            return objPersistence.LoadData(Sql(0, null, (int)Situacao.Ativo, true) + " Order By Nome").ToList();
        }

        private GDAParameter[] GetParam(string nome)
        {
            return !String.IsNullOrEmpty(nome) ? new GDAParameter[] { new GDAParameter("?nome", "%" + nome + "%") } : null;
        }

        public IList<Comissionado> GetAllOrdered()
        {
            string sql = "select * from comissionado order by nome";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Retorna dados do comissionado

        public string GetNome(uint idComissionado)
        {
            return GetNome(null, idComissionado);
        }

        public string GetNome(GDASession session, uint idComissionado)
        {
            string sql = "Select nome From comissionado Where idComissionado=" + idComissionado;

            object nome = objPersistence.ExecuteScalar(session, sql);

            return nome != null ? nome.ToString().Replace("'", "") : String.Empty;
        }

        public float ObtemPercentual(uint idComissionado)
        {
            return ObtemValorCampo<float>("percentual", "idComissionado=" + idComissionado);
        }

        #endregion

        #region Verifica se comissionado já existe

        /// <summary>
        /// Verifica se já existe um comissionado cadastrado com o CPF/CNPJ cadastrado
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public bool CheckIfExists(string cpfCnpj)
        {
            string sql = "Select Count(*) From comissionado Where " +
                "Replace(Replace(Replace(CpfCnpj, '.', ''), '-', ''), '/', '')='" + cpfCnpj + "'";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteSqlQueryCount(sql).ToString()) > 0;
        }

        #endregion

        #region Busca para Comissão

        public IList<Comissionado> GetComissionadosForComissao(string dataIni, string dataFim)
        {
            string ids = PedidoDAO.Instance.GetPedidosIdForComissao(Pedido.TipoComissao.Comissionado, 0, dataIni, dataFim);
            if (ids.Length == 0)
                return new Comissionado[0];

            string sql = "select * from comissionado where idComissionado in (" + ids + ") order by Nome";
            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Comissionado> GetComissionadosByComissao()
        {
            string sql = "select * from comissionado where idComissionado in (select distinct idComissionado from comissao where idComissionado is not null) order by Nome";
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Comissionado objInsert)
        {
            // Não permite que o nome do comissionado possua ' ou "
            objInsert.Nome = objInsert.Nome.Replace("'", "").Replace("\"", "");

            return base.Insert(objInsert);
        }

        public override int Update(Comissionado objUpdate)
        {
            // Não permite que o nome do comissionado possua ' ou "
            objUpdate.Nome = objUpdate.Nome.Replace("'", "").Replace("\"", "");

            return base.Update(objUpdate);
        }

        #endregion
    }
}