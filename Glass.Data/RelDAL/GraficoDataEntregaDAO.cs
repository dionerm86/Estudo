using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class GraficoDataEntregaDAO : BaseDAO<GraficoDataEntrega, GraficoDataEntregaDAO>
    {
        //private GraficoDataEntregaDAO() { }

        private decimal TotalM2(DateTime dataEntrega)
        {
            var campo = "Round(pp.totM" + (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "2Calc" :
                "*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + " and ap.idAmbientePedido is not null, ap.qtde, 1)") + ", 2)";

            var sql = String.Format(@"
                select cast(sum(" + campo + @") as decimal(12,2)) as totalM2
                from produtos_pedido pp
                    inner join pedido ped on (pp.idPedido=ped.idPedido)
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                    inner join produto p on (pp.idProd=p.idProd)
                    left join cliente cli ON (ped.idCli=cli.id_cli)
                    left join rota_cliente rc ON (cli.id_cli=rc.idCliente)
                where date(ped.dataEntrega)=date(?data) 
                    and !coalesce(pp.invisivel{0}, false) and pp.qtde>0
                    and ped.situacao not in ({1}) and ({2})=0 " +
                    
                (!ProducaoConfig.ConsiderarApenasPedidosEntregaDeRotaPainelComercial ? "" :
                " And ped.tipoEntrega In (" +
                Glass.Data.Helper.DataSources.Instance.GetTipoEntrega().Where(f => f.Descr == "Entrega").ElementAt<GenericModel>(0).Id + ")" +
                " And Coalesce(rc.idCliente, 0) > 0") +

                " Group By ped.dataEntrega",

                PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido",
                (int)Pedido.SituacaoPedido.Ativo + "," + (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + "," +
                    (int)Pedido.SituacaoPedido.Cancelado + "," + (int)Pedido.SituacaoPedido.AtivoConferencia,
                SubgrupoProdDAO.Instance.SqlSubgrupoRevenda("p.idGrupoProd", "p.idSubgrupoProd"));

            return ExecuteScalar<decimal>(sql, new GDAParameter("?data", dataEntrega.Date));
        }

        private decimal LimiteProducao(DateTime dataEntrega)
        {
            return CapacidadeProducaoDiariaDAO.Instance.MaximoVendasData(dataEntrega);
        }

        public GraficoDataEntrega ObtemDadosEntrega(DateTime dataEntrega)
        {
            return new GraficoDataEntrega()
            {
                DataEntrega = dataEntrega.ToString("dd/MM/yyyy"),
                TotalM2 = TotalM2(dataEntrega),
                Meta = LimiteProducao(dataEntrega)
            };
        }

        public IList<GraficoDataEntrega> ObtemDadosEntrega(DateTime dataInicial, int numeroDiasRetorno)
        {
            var retorno = new List<GraficoDataEntrega>();
            DateTime data = dataInicial.Date;

            int numeroDias = 0;
            while (numeroDias++ < numeroDiasRetorno)
            {
                retorno.Add(ObtemDadosEntrega(data));

                data = data.AddDays(1);
                while (!data.DiaUtil())
                    data = data.AddDays(1);
            }

            return retorno;
        }
    }
}
