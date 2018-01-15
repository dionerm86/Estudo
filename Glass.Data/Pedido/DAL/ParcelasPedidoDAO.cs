using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ParcelasPedidoDAO : BaseDAO<ParcelasPedido, ParcelasPedidoDAO>
	{
        //private ParcelasPedidoDAO() { }

        private string Sql(uint idPedido, string numParcelas, bool selecionar)
        {
            string campos = selecionar ? "p.*" : "Count(*)";

            string sql = "Select " + campos + " From parcelas_pedido p " +
                "Where 1";

            if (idPedido > 0)
                sql += " and IdPedido=" + idPedido;
            else if (!String.IsNullOrEmpty(numParcelas))
                sql += " and numParc in (" + numParcelas + ")";
            else
                sql += " and false";

            return sql;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<ParcelasPedido> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        public IList<ParcelasPedido> GetByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, Sql(idPedido, null, true) + " Order By NumParc").ToList();
        }

        public IList<ParcelasPedido> GetList(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idPedido, null, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idPedido)
        {
            return GetCount(null, idPedido);
        }

        public int GetCount(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(idPedido, null, false), null);
        }

        public IList<ParcelasPedido> GetByString(string numParcelas)
        {
            return objPersistence.LoadData(Sql(0, numParcelas, true)).ToList();
        }

        public IList<ParcelasPedido> GetForRpt(uint idPedido)
        {
            var retorno = objPersistence.LoadData(Sql(idPedido, null, true) + " Order By Data Asc").ToList();
            var lstRec = ContasReceberDAO.Instance.GetByPedido(null, idPedido, false, true);

            for (int i = 0; i < retorno.Count; i++)
            {
                if (i >= lstRec.Count)
                    break;

                retorno[i].Desconto = lstRec[i].Desconto;
            }

            return retorno;
        }

        public string GetForRecibo(uint idPedido)
        {
            string retorno = "";
            
            var parcelas = GetByPedido(idPedido);
            foreach (ParcelasPedido p in parcelas)
                retorno += ", " + p.Valor.ToString("C") + " para " + p.Data;

            return retorno.Length > 0 ? retorno.Substring(2) : "";
        }

        public string GetForRecibo(string numParcelas)
        {
            string retorno = "";

            var parcelas = GetByString(numParcelas);
            foreach (ParcelasPedido p in parcelas)
                retorno += ", " + p.Valor.ToString("C") + " com vencimento em " + p.Data.GetValueOrDefault().ToString("dd/MM/yyyy");
            
            return retorno.Length > 0 ? retorno.Substring(2) : "";
        }

        public decimal ObtemTotalPorPedido(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<decimal>("Sum(valor)", "idPedido=" + idPedido);
        }

        public decimal GetTotal(string numParcelas)
        {
            string sql = "select sum(valor) from parcelas_pedido where numParc in (" + numParcelas + ")";
            return ExecuteScalar<decimal>(sql);
        }

        public void DeleteFromPedido(uint idPedido)
        {
            DeleteFromPedido(null, idPedido);
        }

        public void DeleteFromPedido(GDASession sessao, uint idPedido)
        {
            objPersistence.ExecuteCommand(sessao, "Delete From parcelas_pedido Where IdPedido=" + idPedido);
        }
	}
}