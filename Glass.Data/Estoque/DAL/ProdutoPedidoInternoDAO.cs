using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoPedidoInternoDAO : BaseDAO<ProdutoPedidoInterno, ProdutoPedidoInternoDAO>
    {
        //private ProdutoPedidoInternoDAO() { }

        #region Busca padrão

        private string Sql(uint idProdPedInterno, uint idPedidoInterno, string idsPedidoInterno, bool selecionar)
        {
            string criterio = "";
            string campos = selecionar ? "ppi.*, p.codInterno, p.descricao as descrProduto, p.idGrupoProd, p.idSubgrupoProd, " +
                "'$$$' as criterio" : "count(*)";

            string sql = @"
                select " + campos + @"
                from produto_pedido_interno ppi
                    left join produto p on (ppi.idProd=p.idProd)
                where 1";

            if (idProdPedInterno > 0)
                sql += " and idProdPedInterno=" + idProdPedInterno;

            if (idPedidoInterno > 0)
            {
                sql += " and idPedidoInterno=" + idPedidoInterno;
                criterio += "Pedido Interno: " + idPedidoInterno + "    ";
            }

            if (!string.IsNullOrEmpty(idsPedidoInterno))
                sql += string.Format(" and idPedidoInterno In ({0})", idsPedidoInterno);

            sql = sql.Replace("$$$", criterio);
            return sql;
        }

        private string SqlRptProdutoPedidoInterno(int idSubGrupo, string dataInicio, string dataFim,
            int idGrupo, int idPedInterno, int idFuncCad, int idFuncReceb, bool selecionar)
        {
            string sql = @"
                Select ppi.*, p.codInterno, p.descricao as descrProduto, sum(ppi.Qtde) as QtdeSomada, sum(ppi.TotM) as TotM2,
                        round(sum((coalesce(p.CustoCompra, p.custoFabBase, 0) * coalesce(ppi.TotM, ppi.Qtde, 0))), 2) as Custo
                    From pedido_interno pi
                        Inner Join produto_pedido_interno ppi on (ppi.idPedidoInterno = pi.idPedidoInterno)
                        Inner Join produto p on(p.idProd = ppi.idProd)
                    Where 1 " +
                      (String.IsNullOrEmpty(dataInicio) ? "" : " And pi.dataPedido >= ?dataIni").ToString() +
                      (String.IsNullOrEmpty(dataFim) ? "" : " And pi.dataPedido <= ?dataFim").ToString() +
                      (idSubGrupo > 0 ? " And p.idSubGrupoProd = " + idSubGrupo.ToString() : "").ToString() +
                      (idGrupo > 0 ? " And p.idGrupoProd = " + idGrupo.ToString() : "").ToString() +
                      (idPedInterno > 0 ? " And pi.idPedidoInterno = " + idPedInterno.ToString() : "").ToString() +
                      (idFuncReceb > 0 ? " And idFunc = " + idFuncReceb.ToString() : "").ToString() +
                      (idFuncCad > 0 ? " And idFuncCad = " + idFuncCad.ToString() : "").ToString() + @"
                group by p.codInterno, p.descricao
                order by pi.dataPedido
            ";

            if (!selecionar)
                return "Select * From (" + sql + ")";

            return sql;
        }

        public ProdutoPedidoInterno GetElement(uint idProdPedInterno)
        {
            return objPersistence.LoadOneData(Sql(idProdPedInterno, 0, null, true));
        }

        public IList<ProdutoPedidoInterno> GetList(uint idPedidoInterno, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idPedidoInterno) == 0)
                return new ProdutoPedidoInterno[] { new ProdutoPedidoInterno() };
            else
                return LoadDataWithSortExpression(Sql(0, idPedidoInterno, null, true), sortExpression, startRow, pageSize);
        }

        public IList<ProdutoPedidoInterno> GetForRptProdutoInterno(string dataInicio, string dataFim,
            int idSubGrupo, int idGrupo, int idPedidoInterno, int idFuncCad, int idFuncReceb)
        {
            return objPersistence.LoadData(
                SqlRptProdutoPedidoInterno(idSubGrupo, dataInicio, dataFim, idGrupo, idPedidoInterno, idFuncCad, idFuncReceb, true), GetParam(dataInicio, dataFim)).ToList();
        }

        public int GetCount(uint idPedidoInterno)
        {
            int retorno = GetCountReal(idPedidoInterno);
            return retorno > 0 ? retorno : 1;
        }

        public int GetForRptProdutoInternoCount(string dataInicio, string dataFim,
            int idSubGrupo, int idGrupo, int idPedidoInterno, int idFuncCad, int idFuncReceb)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlRptProdutoPedidoInterno(idSubGrupo, dataInicio, dataFim, idGrupo, idPedidoInterno, idFuncCad, idFuncReceb, false));
        }

        public int GetCountReal(uint idPedidoInterno)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedidoInterno, null, false));
        }

        public IList<ProdutoPedidoInterno> GetByPedidoInterno(uint idPedidoInterno)
        {
            return GetByPedidoInterno(null, idPedidoInterno);
        }

        public IList<ProdutoPedidoInterno> GetByPedidoInterno(GDASession session, uint idPedidoInterno)
        {
            return objPersistence.LoadData(session, Sql(0, idPedidoInterno, null, true)).ToList();
        }

        public IList<ProdutoPedidoInterno> GetByPedidoInternoForRpt(string idsPedidoInterno)
        {
            return objPersistence.LoadData(Sql(0, 0, idsPedidoInterno, true)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(ProdutoPedidoInterno objInsert)
        {
            var situacao = PedidoInternoDAO.Instance.ObtemValorCampo<int>("situacao", "idPedidoInterno=" + objInsert.IdPedidoInterno);

            // Caso o pedido interno esteja confirmado ou autorizado então não pode ser atualizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.Autorizado ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            decimal custo = 0, total = 0;
            float altura = objInsert.Altura, totM = objInsert.TotM;

            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(0, (int)objInsert.IdProd, objInsert.Largura, objInsert.Qtde, 1, 0, 0, false, 2, false, ref custo, ref altura, ref totM,
                ref total, false, 0);

            objInsert.Altura = altura;
            objInsert.TotM = totM;

            return base.Insert(objInsert);
        }

        public override int Update(ProdutoPedidoInterno objUpdate)
        {
            var situacao = PedidoInternoDAO.Instance.ObtemValorCampo<int>("situacao", "idPedidoInterno=" + objUpdate.IdPedidoInterno);

            // Caso o pedido interno esteja confirmado ou autorizado então não pode ser atualizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.Autorizado ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            decimal custo = 0, total = 0;
            float altura = objUpdate.Altura, totM = 0;

            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(0, (int)objUpdate.IdProd, objUpdate.Largura, objUpdate.Qtde, 1, 0, 0, false, 2, false, ref custo, ref altura, ref totM,
                ref total, false, 0);

            objUpdate.Altura = altura;
            objUpdate.TotM = totM;

            return base.Update(objUpdate);
        }
 
        public override int Delete(ProdutoPedidoInterno objDelete)
        {
            var situacao = PedidoInternoDAO.Instance.ObtemValorCampo<int>("situacao", "idPedidoInterno=" +
                ObtemValorCampo<uint>("idPedidoInterno", "idProdPedInterno=" + objDelete.IdProdPedInterno));

            // Caso o pedido interno esteja confirmado ou autorizado então não pode ser atualizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.Autorizado ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            return base.Delete(objDelete);
        }

        #endregion

        public string GetDescricaoProduto(uint idProdPedInterno)
        {
            uint idProd = ObtemValorCampo<uint>("idProd", "idProdPedInterno=" + idProdPedInterno);
            return ProdutoDAO.Instance.ObtemDescricao((int)idProd);
        }

        public decimal ObterQtde(GDASession session, int idProdPedInterno)
        {
            return ObtemValorCampo<decimal>(session, "Qtde", string.Format("IdProdPedInterno={0}", idProdPedInterno));
        }

        /// <summary>
        /// Método para estornar os produtos ao estoque em caso de cancelamento de pedido confirmado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idProduto"></param>
        /// <param name="quantidade"></param>
        /// <returns></returns>
        public int ExtornaProdutosPedidoInterno(uint idLoja, uint idProduto, float quantidade)
        {
            string sql = "UPDATE produto_loja SET QtdEstoque=?qt WHERE IdLoja=?l AND IdProd=?p";
            return objPersistence.ExecuteCommand(sql, new GDAParameter("?qt", quantidade), new GDAParameter("?l", idLoja), new GDAParameter("?p", idProduto));
        }
    }
}
