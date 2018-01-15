using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class CapacidadeProducaoDAO : BaseDAO<CapacidadeProducao, CapacidadeProducaoDAO>
    {
        //private CapacidadeProducaoDAO() { }

        #region SQL

        internal string ObtemIdsProdPed(uint idProdPedOriginal, DateTime dataProducao, bool incluirProntos, bool incluirRetroativos)
        {
            string idsProdPed = String.Empty;

            if (idProdPedOriginal > 0)
                idsProdPed = " and pp.idProdPed=" + idProdPedOriginal;

            else if (dataProducao != null)
            {
                string sql = @"
                    select ppe.idProdPed from produtos_pedido_espelho ppe
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido) 
                        inner join pedido p on (ppe.idPedido=p.idPedido)
                    where !coalesce(ppe.invisivelFluxo, false) and ppe.qtde>0
                        and p.situacao in (" + (int)Pedido.SituacaoPedido.Confirmado + "," +
                            (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," +
                            (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," +
                            (int)Pedido.SituacaoPedido.Conferido + @") 
                        and pe.situacao not in (" + (int)PedidoEspelho.SituacaoPedido.Cancelado + "," +
                            (int)PedidoEspelho.SituacaoPedido.Processando + ")";

                sql += " and date(pe.dataFabrica)" + (incluirRetroativos && !incluirProntos ? "<" : "") + "=date(?data)";

                if (!incluirProntos)
                    sql += " and p.situacaoProducao in (" + (int)Pedido.SituacaoProducaoEnum.NaoEntregue + "," +
                        (int)Pedido.SituacaoProducaoEnum.Pendente + ")";

                idsProdPed = GetValoresCampo(sql, "idProdPed", new GDAParameter("?data", dataProducao));

                if (!String.IsNullOrEmpty(idsProdPed))
                    idsProdPed = " and ppe.idProdPed in (" + idsProdPed + ")";
                else
                    idsProdPed = " and false";
            }

            return idsProdPed;
        }

        internal string ObtemIdsProdPedProducao(uint idProdPedOriginal, string idsProdPed)
        {
            if (idProdPedOriginal > 0)
                return " and false";

            string idsProdPedProducao = GetValoresCampo(String.Format(@"select idProdPedProducao
                from produto_pedido_producao where situacao in ({0}) {1}",

                (int)ProdutoPedidoProducao.SituacaoEnum.Producao + "," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                idsProdPed.Replace("ppe.", String.Empty)), "idProdPedProducao");

            return !String.IsNullOrEmpty(idsProdPedProducao) ? 
                " and ppp.idProdPedProducao in (" + idsProdPedProducao + ")" : 
                " and false";
        }

        internal string SqlEmProducao(uint idSetor, uint idProdPedOriginal, DateTime dataProducao, bool agruparPorPedido)
        {
            string idsProdPed = ObtemIdsProdPed(idProdPedOriginal, dataProducao, false, false);
            string idsProdPedProducao = ObtemIdsProdPedProducao(idProdPedOriginal, idsProdPed);

            string criterio = "Data de fábrica: " + dataProducao.ToString("dd/MM/yyyy");
            if (idSetor > 0)
                criterio += "    Setor: " + Utils.ObtemSetor(idSetor).Descricao;

            return String.Format(@"
                select s.idSetor, '{13}' as criterio, cast(if(coalesce(cpds.capacidade, 
                    s.capacidadeDiaria) is not null, sum(totM), 0) as decimal(12,4)) as totM {11}
                from (
                    select rps.idSetor, ppe.idPedido, if(p.tipoPedido<>{1}, ppe.totM2Calc/ppe.qtde*(ppe.qtde-count(distinct ppp.idProdPedProducao)),
                        ape.altura*ape.largura*ape.qtde/1000000*(ape.qtde-count(distinct ppp.idProdPedProducao))) as totM
                    from roteiro_producao_etiqueta rps
                        inner join produto_pedido_producao ppp on (rps.idProdPedProducao=ppp.idProdPedProducao {8})
                        inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed {7})
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido)
                        inner join pedido p on (pe.idPedido=p.idPedido)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    where ppp.situacao={0} and date(pe.dataFabrica)=date(?dataFabrica)
                    group by rps.idSetor, ppp.idProdPed

                    union all select r.idSetor, pp.idPedido, if(p.tipoPedido<>{1}, pp.totM2Calc, ap.altura*ap.largura*ap.qtde/1000000) as totM
                    from produtos_pedido pp
                        inner join pedido p on (pp.idPedido=p.idPedido)
                        left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                        inner join etiqueta_processo ep on (if(p.tipoPedido<>{1}, pp.idProcesso, ap.idProcesso)=ep.idProcesso)
                        inner join ({6}) r on (ep.idProcesso=r.idProcesso)
                    where p.tipoPedido in ({1},{2},{3},{4})
                        and p.situacaoProducao in ({5}) and date(p.dataEntrega)=date(?dataEntrega) {10}
                ) as roteiro
                    inner join setor s on (roteiro.idSetor=s.idSetor {9})
                    left join capacidade_producao_diaria_setor cpds on (date(cpds.data)=date(?dataFabrica) and cpds.idSetor=s.idSetor)
                group by s.idSetor {11} {12}",
                                   
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                (int)Pedido.TipoPedidoEnum.Venda,
                (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial,
                (int)Pedido.TipoPedidoEnum.Producao,
                (int)Pedido.SituacaoProducaoEnum.NaoEntregue,
                RoteiroProducaoEtiquetaDAO.Instance.Sql(null, null, idsProdPed.Substring(idsProdPed.StartsWith(" and ", StringComparison.CurrentCultureIgnoreCase) ? 5 : 0).Replace("pp.", "ppe."), true),

                idProdPedOriginal > 0 ? " and false" : idsProdPed,
                idsProdPedProducao,
                idSetor > 0 ? " and s.idSetor=" + idSetor : String.Empty,
                idProdPedOriginal > 0 ? idsProdPed : " and p.situacao in (" + 
                    (int)Pedido.SituacaoPedido.Conferido + "," + 
                    (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ")",
                agruparPorPedido ? ", roteiro.idPedido" : String.Empty,
                agruparPorPedido ? " having totM > 0 order by roteiro.idPedido, s.numSeq" : String.Empty,
                criterio);
        }

        internal GDAParameter[] ObtemParametros(DateTime dataFabrica, DateTime dataEntrega)
        {
            return new GDAParameter[] { 
                new GDAParameter("?dataFabrica", dataFabrica), 
                new GDAParameter("?dataEntrega", dataEntrega)
            };
        }

        private GDAParameter[] ObtemParametros(DateTime dataProducao)
        {
            DateTime dataEntrega = dataProducao;

            for (int i = 0; i < PCPConfig.Etiqueta.DiasDataFabrica; i++)
            {
                dataEntrega = dataEntrega.AddDays(1);

                while (!dataEntrega.DiaUtil())
                    dataEntrega = dataEntrega.AddDays(1);
            }

            return ObtemParametros(dataProducao, dataEntrega);
        }

        #endregion

        #region Obtém a capacidade de produção

        public IList<CapacidadeProducao> ObtemCapacidadeProducao(DateTime dataFabrica)
        {
            return ObtemCapacidadeProducao(0, dataFabrica);
        }

        public IList<CapacidadeProducao> ObtemCapacidadeProducao(uint idSetor, DateTime dataFabrica)
        {
            DateTime dataEntrega = dataFabrica;

            for (int i = 0; i < PCPConfig.Etiqueta.DiasDataFabrica; i++)
            {
                dataEntrega = dataEntrega.AddDays(1);

                while (!dataEntrega.DiaUtil())
                    dataEntrega = dataEntrega.AddDays(1);
            }

            return ObtemCapacidadeProducao(idSetor, dataFabrica, dataEntrega);
        }

        private IList<CapacidadeProducao> ObtemCapacidadeProducao(uint idSetor, DateTime dataFabrica, DateTime dataEntrega)
        {
            string sql = SqlEmProducao(idSetor, 0, dataFabrica, false);
            return objPersistence.LoadData(sql, ObtemParametros(dataFabrica, dataEntrega)).ToList();
        }

        public IList<CapacidadeProducao> ObtemCapacidadeProducao(DateTime dataFabrica, uint idPedido, bool pedidoEspelho)
        {
            return ObtemCapacidadeProducao(dataFabrica, idPedido, pedidoEspelho, 0, 0);
        }

        private IList<CapacidadeProducao> ObtemCapacidadeProducao(DateTime dataFabrica, uint idPedido, bool pedidoEspelho,
            float totM2Adicionar, uint idProcessoAdicionar)
        {
            DateTime dataEntrega = dataFabrica;

            for (int i = 0; i < PCPConfig.Etiqueta.DiasDataFabrica; i++)
            {
                dataEntrega = dataEntrega.AddDays(1);

                while (!dataEntrega.DiaUtil())
                    dataEntrega = dataEntrega.AddDays(1);
            }

            return ObtemCapacidadeProducao(dataFabrica, dataEntrega, idPedido, pedidoEspelho, totM2Adicionar, idProcessoAdicionar);
        }

        private IList<CapacidadeProducao> ObtemCapacidadeProducao(DateTime dataFabrica, DateTime dataEntrega, uint idPedido, bool pedidoEspelho,
            float totM2Adicionar, uint idProcessoAdicionar)
        {
            var capacidades = ObtemCapacidadeProducao(0, dataFabrica, dataEntrega).ToList();

            var idsProdPed = ExecuteMultipleScalar<uint>(String.Format(@"select idProdPed from produtos_pedido{0}
                where idPedido=" + idPedido, pedidoEspelho ? "_espelho" : String.Empty));

            var setoresVerificar = new List<uint>();

            foreach (var id in idsProdPed)
            {
                string sql = SqlEmProducao(0, id, dataFabrica, false);
                var cap = objPersistence.LoadData(sql, ObtemParametros(dataFabrica, dataEntrega));

                capacidades.AddRange(cap);
                setoresVerificar.AddRange(cap.Select(x => x.IdSetor));
            }

            if (totM2Adicionar > 0 && idProcessoAdicionar > 0)
            {
                int idRoteiroProducao = RoteiroProducaoDAO.Instance.ObtemValorCampo<int>("idRoteiroProducao", "idProcesso=" + idProcessoAdicionar);

                if (idRoteiroProducao > 0)
                {
                    var setores = RoteiroProducaoSetorDAO.Instance.ObtemPorRoteiroProducao(idRoteiroProducao);

                    foreach (var s in setores)
                    {
                        setoresVerificar.Add(s.IdSetor);

                        capacidades.Add(new CapacidadeProducao()
                        {
                            IdSetor = s.IdSetor,
                            TotM = (decimal)totM2Adicionar
                        });
                    }
                }
            }

            return (from c in capacidades
                    where setoresVerificar.Contains(c.IdSetor)
                    group c by c.IdSetor into g
                    select new CapacidadeProducao()
                    {
                        IdSetor = g.Key,
                        TotM = g.Sum(x => x.TotM)
                    }).ToList();
        }

        #endregion

        #region Validação das datas dos pedidos

        public void ValidaDataEntregaPedido(uint idPedido, DateTime dataEntrega, float totM2Adicionar, uint idProcessoAdicionar)
        {
            ValidaDataEntregaPedido(null, idPedido, dataEntrega, totM2Adicionar, idProcessoAdicionar);
        }

        public void ValidaDataEntregaPedido(GDASession session, uint idPedido, DateTime dataEntrega, float totM2Adicionar, uint idProcessoAdicionar)
        {
            if (Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor && 
                PedidoDAO.Instance.GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Revenda)
            {
                DateTime dataFabrica = dataEntrega;

                for (int i = 0; i < PCPConfig.Etiqueta.DiasDataFabrica; i++)
                {
                    dataFabrica = dataFabrica.AddDays(-1);

                    while (!dataFabrica.DiaUtil())
                        dataFabrica = dataFabrica.AddDays(-1);
                }

                var previsao = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(session, 0, idPedido, dataFabrica.ToShortDateString(), dataEntrega.ToShortDateString());
                var previsaoPedido = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(session, idPedido, 0, dataFabrica.ToShortDateString(), dataEntrega.ToShortDateString());

                foreach (var p in previsaoPedido)
                {
                    int capacidade = CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(session, dataFabrica, p.IdSetor);
                    var totM = p.TotM + previsao.Where(f => f.IdSetor == p.IdSetor).Sum(f => f.TotM);

                    if (capacidade > 0 && totM > capacidade)
                    {
                        throw new Exception(String.Format(@"A capacidade para o setor {0} ({1:0.##} m²) no dia {2} " +
                            "(previsão de fábrica para data de entrega {4}) foi excedida (valor que foi tentado: {3:0.## m²)}. " +
                            "Selecione outra data de entrega.",
                            Utils.ObtemSetor(p.IdSetor).Descricao,
                            capacidade,
                            dataFabrica.ToString("dd/MM/yyyy"),
                            p.TotM,
                            dataEntrega.ToString("dd/MM/yyyy")));
                    }

                }

                //var capacidades = ObtemCapacidadeProducao(dataFabrica, dataEntrega, idPedido, false, totM2Adicionar, idProcessoAdicionar);
                ////var previsto = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(dataFabrica

                //foreach (var c in capacidades)
                //{
                //    int capacidade = CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(dataFabrica, c.IdSetor);

                //    if (capacidade > 0 && c.TotM > capacidade)
                //        throw new Exception(String.Format(@"A capacidade para o setor {0} ({1:0.##} m²) no dia {2} " +
                //            "(previsão de fábrica para data de entrega {4}) foi excedida (valor que foi tentado: {3:0.## m²)}. " +
                //            "Selecione outra data de entrega.",

                //            Utils.ObtemSetor(c.IdSetor).Descricao,
                //            capacidade,
                //            dataFabrica.ToString("dd/MM/yyyy"),
                //            c.TotM,
                //            dataEntrega.ToString("dd/MM/yyyy")));
                //}
            }
        }

        public void ValidaDataFabricaPedido(uint idPedido, DateTime novaDataFabrica, DateTime dataFabricaAtual, float totM2Adicionar, uint idProcessoAdicionar)
        {
            ValidaDataFabricaPedido(null, idPedido, novaDataFabrica, dataFabricaAtual, totM2Adicionar, idProcessoAdicionar);
        }

        public void ValidaDataFabricaPedido(GDASession session, uint idPedido, DateTime novaDataFabrica, DateTime dataFabricaAtual, float totM2Adicionar, uint idProcessoAdicionar)
        {
            if (Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor && 
                PedidoDAO.Instance.GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Revenda /*&& novaDataFabrica != dataFabricaAtual*/)
            {
                //var capacidades = ObtemCapacidadeProducao(novaDataFabrica, idPedido, true, totM2Adicionar, idProcessoAdicionar);

                //foreach (var c in capacidades)
                //{
                //    int capacidade = CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(novaDataFabrica, c.IdSetor);

                //    if (capacidade > 0 && c.TotM > capacidade)
                //        throw new Exception(String.Format(@"A capacidade para o setor {0} ({1:0.##} m²) no dia {2} " +
                //            "foi excedida (valor que foi tentado: {3:0.## m²)}. Selecione outra data de fabricação.",

                //            Utils.ObtemSetor(c.IdSetor).Descricao,
                //            capacidade,
                //            novaDataFabrica.ToString("dd/MM/yyyy"),
                //            c.TotM));
                //}

                var previsao = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(session, 0, idPedido, novaDataFabrica.ToShortDateString(), false);
                var previsaoPedido = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(session, idPedido, 0, dataFabricaAtual.ToShortDateString(), true);

                foreach (var p in previsaoPedido)
                {
                    int capacidade = CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(session, novaDataFabrica, p.IdSetor);
                    var totM = p.TotM + previsao.Where(f => f.IdSetor == p.IdSetor).Sum(f => f.TotM);

                    if (capacidade > 0 && totM > capacidade)
                        throw new Exception(String.Format(@"A capacidade para o setor {0} ({1:0.##} m²) no dia {2} " +
                            "foi excedida (valor que foi tentado: {3:0.## m²)}. Selecione outra data de fabricação.",
                            Utils.ObtemSetor(p.IdSetor).Descricao,
                            capacidade,
                            novaDataFabrica.ToString("dd/MM/yyyy"),
                            p.TotM));
                }
            }
        }

        #endregion

        #region Busca para relatório de pedido/setor

        public IList<CapacidadeProducao> ObtemListaPedidosCapacidadeProducao(DateTime dataProducao, uint idSetor,
            string sortExpression, int startRow, int pageSize)
        {
            string sql = SqlEmProducao(idSetor, 0, dataProducao, true);
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, ObtemParametros(dataProducao));
        }

        public int ObtemNumeroPedidosCapacidadeProducao(DateTime dataProducao, uint idSetor)
        {
            return GetCountWithInfoPaging(SqlEmProducao(idSetor, 0, dataProducao, true),
                ObtemParametros(dataProducao));
        }

        public IList<CapacidadeProducao> ObtemRelatorioPedidosCapacidadeProducao(DateTime dataProducao, uint idSetor)
        {
            return objPersistence.LoadData(SqlEmProducao(idSetor, 0, dataProducao, true),
                ObtemParametros(dataProducao)).ToList();
        }

        #endregion
    }
}
