using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class RelacaoVendasDAO : BaseDAO<RelacaoVendas, RelacaoVendasDAO>
    {
        //private RelacaoVendasDAO() { }

        internal string Sql(string dataIni, string dataFim, int situacao, int agruparFunc, int ordenar, LoginUsuario login, bool selecionar)
        {
            bool temFiltro;
            string filtroAdicional;

            string idFunc = "", nomeFunc = "";
            switch (agruparFunc)
            {
                case 0:
                    idFunc = "idFunc";
                    nomeFunc = "nomeFunc";
                    break;
                case 1:
                    idFunc = "idFuncCliente";
                    nomeFunc = "nomeFuncCliente";
                    break;
                case 2:
                    idFunc = "idComissionado";
                    nomeFunc = "nomeComissionado";
                    break;
            }

            string sit = null;
            switch (situacao)
            {
                case 5:
                    sit = (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente;
                    break;
                case 7:
                    sit = (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "";
                    break;
                case 99:
                    sit = (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," +
                        (int)Pedido.SituacaoPedido.ConfirmadoLiberacao;
                    break;
            }

            string tipo = (int)Pedido.TipoPedidoEnum.Venda + "," + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.MaoDeObra;
            string tipoVenda = (int)Pedido.TipoVendaPedido.AVista + "," + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.Obra;

            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoFuncionario);


            string sql = PedidoDAO.Instance.SqlRptSit(0, null, 0, null, null, null, null, 0, null, sit, dataIni, dataFim, null, null, null, null, 0, 0, tipo,
                0, 0, 0, null, tipoVenda, 0, null, null, false, false, false, null, null, 0, null, null, 0, 0, null, null, null, null, false, 0, 0, true, false, false, true,
                out temFiltro, out filtroAdicional, 0, null, 0, true, 0, null,
                cliente, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario).Replace("?filtroAdicional?", filtroAdicional);

            sql = @"
                select cast(" + agruparFunc + @" as signed) as agrupar, " + idFunc + " as idFunc, " + nomeFunc + @" as nomeFunc, 
                    count(*) as NumPedidos, sum(totM) as totM2, cast(sum(total) as decimal(12,2)) as valorTotal, Criterio,
                    (select count(distinct date(dataCad)) from pedido where dataCad>=?dtIniSit and dataCad<=?dtFimSit and idFunc=p.idFunc) as NumDias
                from (" + sql + @") as p
                " + (agruparFunc == 2 ? "where idComissionado is not null" : "") + " group by " + idFunc;

            switch (ordenar)
            {
                case 0: sql += " Order By nomeFunc"; break;
                case 1: sql += " Order By nomeFunc Desc"; break;
                default: break;
            }

            return selecionar ? sql : "select count(*) from (" + sql + ") as temp";
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        public IList<RelacaoVendas> GetList(string dataIni, string dataFim, int situacao, int agruparFunc, int ordenar, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(dataIni, dataFim, situacao, agruparFunc, ordenar, UserInfo.GetUserInfo, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(string dataIni, string dataFim, int situacao, int agruparFunc, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(dataIni, dataFim, situacao, agruparFunc, ordenar, UserInfo.GetUserInfo, false), GetParams(dataIni, dataFim));
        }

        public RelacaoVendas[] GetForRpt(string dataIni, string dataFim, int situacao, int agruparFunc, int ordenar, LoginUsuario login)
        {
            return objPersistence.LoadData(Sql(dataIni, dataFim, situacao, agruparFunc, ordenar, login, true), GetParams(dataIni, dataFim)).ToList().ToArray();
        }
    }
}
