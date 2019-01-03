using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ItemProduzidoEFDDAO : BaseDAO<ItemProduzidoEFD, ItemProduzidoEFDDAO>
    {
        public IEnumerable<Sync.Fiscal.EFD.Entidade.IItemProduzido> ObtemItensProduzidosParaEFD(int idLoja, DateTime dataIni, DateTime dataFim)
        {
            var idsProdPedProducao = LeituraProducaoDAO.Instance.ObterIdsProdPedProducaoPeloIdSetorDataLeitura(null, 1, dataIni, dataFim);

            var sqlDadosProducao = string.Format(@"
                SELECT ppp.IdProdPedProducao, ppp.NumEtiqueta, lp.DataLeitura,
		            (lp.IdSetor = 1) as InicioProducao, (s.Corte OR s.Laminado) as Insumo,
                    COALESCE((rpe.UltimoSetor = 1 OR s.Tipo IN (3,4,6)), 0) as FinalProducao,
                    (pp.TotM2Calc / pp.Qtde) as QtdeProduzida, pp.Altura, pp.Largura, pp.IdProd
                FROM produto_pedido_producao ppp
                    INNER JOIN leitura_producao lp ON (lp.IdProdPedProducao = ppp.IdProdPedProducao)
                    INNER JOIN setor s ON (lp.IdSetor = s.IdSetor)
                    INNER JOIN produtos_pedido pp ON (ppp.IdProdPed = pp.IdProdPedEsp)
                    INNER JOIN produto prod ON (pp.IdProd=prod.IdProd)
                    INNER JOIN pedido p ON (pp.IdPedido = p.IdPedido)
                    LEFT JOIN roteiro_producao_etiqueta rpe ON (lp.IdProdPedProducao = rpe.IdProdPedProducao AND lp.IdSetor = rpe.IdSetor)
                WHERE prod.TipoMercadoria IN ({1},{2})
                    AND p.IdLoja = {3}
	                AND ppp.Situacao = 1
                    AND ppp.IdProdPedProducao IN ({0})",
                string.Join(",", idsProdPedProducao),
                (int)TipoMercadoria.ProdutoEmProcesso, (int)TipoMercadoria.ProdutoAcabado, idLoja);

            var itens = objPersistence.LoadData(sqlDadosProducao).ToList();

            var itensProcessados = new List<Sync.Fiscal.EFD.Entidade.IItemProduzido>();

            foreach (var item in itens.GroupBy(f => f.IdProdPedProducao))
            {
                var itemProcessado = new ItemProduzidoEFD();

                var aux = item.FirstOrDefault();

                if (aux == null)
                    continue;

                itemProcessado.CodigoProduto = aux.CodigoProduto;
                itemProcessado.NumEtiqueta = aux.NumEtiqueta;
                itemProcessado.QtdeProduzida = aux.QtdeProduzida;

                var inicioProd = item.FirstOrDefault(f => f.InicioProd);
                if (inicioProd != null)
                    itemProcessado.InicioProducao = inicioProd.DataLeitura;

                var finalProd = item.FirstOrDefault(f => f.FinalProd && f.DataLeitura >= dataIni && f.DataLeitura <= dataFim);
                if (finalProd != null)
                    itemProcessado.FinalProducao = finalProd.DataLeitura;

                var insumo = item.FirstOrDefault(f => f.UsoEnsumo && f.DataLeitura >= dataIni && f.DataLeitura <= dataFim);
                if (insumo != null)
                {
                    itemProcessado.Insumos = ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd((uint)insumo.CodigoProduto, true)
                    .Select(pbe => new InsumoConsumidoEFD
                    {
                        CodigoProduto = pbe.IdProdBaixa,
                        DataSaidaEstoque = insumo.InicioProducao,
                        QtdeConsumida = (decimal)(ProdutosNfDAO.Instance.ObtemQtdDanfe((uint)pbe.IdProdBaixa, (float)insumo.QtdeProduzida, 1, insumo.Altura, insumo.Largura, true, true) * pbe.Qtde)
                    })
                    .ToList();
                }

                itensProcessados.Add(itemProcessado);
            }

            return itensProcessados;
        }

        public GDAParameter[] GetParams(DateTime? dtIni, DateTime? dtFim, DateTime? dtAnteriorIni)
        {
            var parameters = new List<GDAParameter>();

            if (dtIni > DateTime.MinValue)
                parameters.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni.Value.ToShortDateString() + " 00:00:00")));

            if (dtFim > DateTime.MinValue)
                parameters.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim.Value.ToShortDateString() + " 23:59:59")));

            if (dtAnteriorIni > DateTime.MinValue)
                parameters.Add(new GDAParameter("?dtAnteriorIni", DateTime.Parse(dtAnteriorIni.Value.ToShortDateString() + " 23:59:59")));

            return parameters.ToArray();
        }
    }
}
