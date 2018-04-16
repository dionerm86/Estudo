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

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }
    }
}
