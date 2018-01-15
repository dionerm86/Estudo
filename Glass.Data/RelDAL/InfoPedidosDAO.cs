using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class InfoPedidosDAO : BaseDAO<InfoPedidos, InfoPedidosDAO>
    {
        //private InfoPedidosDAO() { }

        #region SQL interno

        /// <summary>
        /// Retorna o filtro padrão para os produtos considerados no fast delivery/máximo de vendas.
        /// </summary>
        /// <param name="excluirVidroQtde">Os vidros calculados por qtde. são desconsiderados?</param>
        /// <returns></returns>
        internal string SqlWhere(bool excluirVidroQtde)
        {
            string sql = @" and p.idSubgrupoProd<>" + (int)Utils.SubgrupoProduto.LevesDefeitos + @"
                and (pp.Invisivel{0}=false or pp.Invisivel{0} is null)";

            if (excluirVidroQtde)
                sql = " and (s.TipoCalculo<>" + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @" || s.TipoCalculo is null) 
                    and g.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + sql;

            sql = String.Format(sql, PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido");
            return sql;
        }

        /// <summary>
        /// Retorna o SQL para consulta de Fast Delivery.
        /// </summary>
        internal string SqlFastDelivery(uint? idPedido, string dataIni, string dataFim)
        {
            return SqlFastDelivery(null, idPedido, dataIni, dataFim);
        }

        /// <summary>
        /// Retorna o SQL para consulta de Fast Delivery.
        /// </summary>
        internal string SqlFastDelivery(GDASession session, uint? idPedido, string dataIni, string dataFim)
        {
            string filtroPedido = idPedido != null ? " And pp.IdPedido<>" + idPedido : "";

            var idsPedido = String.Join(",", ExecuteMultipleScalar<string>(session,
                "Select Cast(idPedido as char) From pedido Where FastDelivery=1 and Coalesce(temperaFora,false)=false and DataEntrega>=?dataIni And DataEntrega<=?dataFim",
                GetParams(dataIni, dataFim)));

            if (String.IsNullOrEmpty(idsPedido))
                idsPedido = "0";

            filtroPedido += " And pp.idPedido In (" + idsPedido + ") ";

            string sql = @"
                select Coalesce(sum(pp.TotM), 0) as TotalFastDelivery, date(ped.DataEntrega) as Data
                from produtos_pedido pp
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                    left join produto p on (pp.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) 
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                where 1 " + filtroPedido + @" 
                    And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @" 
                    " + SqlWhere(true) + @"
                    group by date(ped.DataEntrega)";

            return sql;
        }

        /// <summary>
        /// Retorna o SQL para consulta de máximo de vendas.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        internal string SqlMaximoVendas(uint? idPedido)
        {
            string filtroPedido = idPedido != null ? "pp.IdPedido<>" + idPedido + " and " : "";
            string sql = @"
                select Coalesce(sum(pp.TotM), 0) as TotalProducao, date(ped.DataEntrega) as Data
                from produtos_pedido pp
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                    left join produto p on (pp.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) 
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                where ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @"
                    and " + filtroPedido + @"(ped.DataEntrega>=?dataIni And ped.DataEntrega<=?dataFim) 
                    and (ped.temperaFora=false or ped.temperaFora is null)
                    and ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                    " + SqlWhere(true) + @"
                    group by date(ped.DataEntrega)";

            return sql;
        }

        /// <summary>
        /// Retorna o SQL para consulta de máximo de vendas internas.
        /// </summary>
        /// <returns></returns>
        internal string SqlMaximoVendasInternas()
        {
            string sql = @"
                select Coalesce(sum(pp.TotM), 0) as TotalProducaoInterna, date(ped.DataEntrega) as Data
                from produtos_pedido pp 
                    left join produto p on (pp.idProd=p.idProd) 
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                where ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @"
                    and (ped.DataEntrega>=?dataIni And ped.DataEntrega<=?dataFim) 
                    and (ped.temperaFora=false or ped.temperaFora is null)
                    and ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                    " + SqlWhere(false) + @"
                    group by date(ped.DataEntrega)";

            return sql;
        }

        #endregion

        private string Sql(bool selecionar, string dataIni, string dataFim)
        {
            string fastDelivery = SqlFastDelivery(null, dataIni, dataFim);
            string maximoVendas = SqlMaximoVendas(null);
            string maximoVendasInternas = SqlMaximoVendasInternas();

            string sql = @"
                select Data, round(Sum(TotalFastDelivery),2) as TotalFastDelivery, round(Sum(TotalProducao),2) as TotalProducao, 
                    round(Sum(TotalProducaoInterna),2) as TotalProducaoInterna
                from (
                    select Data, TotalFastDelivery, 0 as TotalProducao, 0 as TotalProducaoInterna 
                    from (" + fastDelivery + @") as tb1
                    union select Data, 0 as TotalFastDelivery, TotalProducao, 0 as TotalProducaoInterna
                    from (" + maximoVendas + @") as tb2
                    union select Data, 0 as TotalFastDelivery, 0 as TotalProducao, TotalProducaoInterna
                    from (" + maximoVendasInternas + @") as tb3
                ) as temp
                group by Data";

            return selecionar ? sql : "select count(*) from (" + sql + ") as temp";
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        public IList<InfoPedidos> GetInfoPedidos(string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(true, dataIni, dataFim), GetParams(dataIni, dataFim)).ToList();
        }

        public InfoPedidos GetInfoPedidos(string data)
        {
            if (objPersistence.ExecuteSqlQueryCount(Sql(false, data, data), GetParams(data, data)) > 0)
                return objPersistence.LoadOneData(Sql(true, data, data), GetParams(data, data));
            else
            {
                InfoPedidos novo = new InfoPedidos();
                novo.Data = DateTime.Parse(data);
                return novo;
            }
        }
    }
}
