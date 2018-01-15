using Glass.Estoque.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Estoque.Negocios.Componentes
{
    public class MovChapaFluxo : IMovChapaFluxo
    {
        public IList<MovChapa> ObtemMovChapa(string idsCorVidro, float espessura, DateTime dataIni, DateTime dataFim)
        {
            #region Variaveis Locais

            var resultado = new List<MovChapaDetalhe>();
            var movEstoqueIni = new List<Tuple<int, int, DateTime, Int64>>();
            var chapas = new List<Tuple<int, decimal, string, string>>();

            #endregion

            #region Consultas

            var consultaMovChapaDetalhe =
                SourceContext.Instance.CreateQuery()
                .From<Data.Model.ChapaCortePeca>("ccp")
                    .InnerJoin<Data.Model.ProdutoImpressao>("ccp.IdProdImpressaoChapa = pi.IdProdImpressao", "pi")
                    .InnerJoin<Data.Model.ProdutosNf>("pi.IdProdNf = pnf.IdProdNf", "pnf")
                    .InnerJoin<Data.Model.NotaFiscal>("pnf.IdNf = nf.IdNf", "nf")
                    .InnerJoin<Data.Model.Produto>("pnf.IdProd = prod.IdProd", "prod")
                    .InnerJoin<Data.Model.CorVidro>("prod.IdCorVidro = cv.IdCorVidro", "cv")
                .GroupBy("ccp.IdProdImpressaoChapa, pnf.IdProd, Date(ccp.DataCad)")
                .OrderBy("cv.IdCorVidro, prod.Espessura, ccp.DataCad DESC")
                .Where("date(ccp.DataCad) >= ?dtIni AND date(ccp.DataCad) <= ?dtFim").Add("?dtIni", dataIni).Add("?dtFim", dataFim)
                .Select(@"ccp.IdProdImpressaoChapa, nf.IdLoja, Date(ccp.DataCad) AS DataLeitura, pnf.IdProd, cv.IdCorVidro, cv.Descricao as CorVidro,
                            prod.Espessura, ROUND(pnf.TotM / pnf.Qtde, 2) as M2Utilizado, CONCAT(prod.CodInterno, ' - ', prod.Descricao) as DescricaoProd,
                            pi.NumEtiqueta, 0 AS TemOutrasLeituras, ccp.SaidaRevenda");

            var consultaMovEstoqueIni =
               SourceContext.Instance.CreateQuery()
               .From(
                        SourceContext.Instance.CreateQuery()
                       .Select(@"mv.IdProd, mv.IdLoja, mv.DataMov, mv.SaldoQtdeMov")
                       .From<Data.Model.MovEstoque>("mv")
                       .OrderBy("mv.DataMov DESC, mv.IdMovEstoque DESC"), "tmp"
                    )
               .GroupBy("IdProd, IdLoja, date(DataMov)")
               .Select("IdProd, IdLoja, Date(DataMov) as DataMov, SaldoQtdeMov");

            var consultaChapas =
               SourceContext.Instance.CreateQuery()
               .From<Data.Model.ChapaCortePeca>("ccp1")
                   .InnerJoin<Data.Model.ProdutoImpressao>("ccp1.IdProdImpressaoPeca = pi1.IdProdImpressao", "pi1")
                   .InnerJoin<Data.Model.ProdutosPedido>("pi1.IdProdPed = pp.IdProdPedEsp", "pp")
               .Where("(pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo = 0)")
               .Select(@"ccp1.IdProdImpressaoChapa, (pp.TotM / pp.Qtde) as M2Lido, ccp1.PlanoCorte, pi1.NumEtiqueta");

            #endregion

            #region Filtros

            if (!string.IsNullOrEmpty(idsCorVidro))
                consultaMovChapaDetalhe.WhereClause.And(string.Format("cv.IdCorVidro IN ({0})", idsCorVidro));

            if (espessura > 0)
                consultaMovChapaDetalhe.WhereClause.And("prod.Espessura = ?espessura").Add("?espessura", espessura);

            #endregion

            #region Execução

            SourceContext.Instance.CreateMultiQuery()
                .Add<MovChapaDetalhe>(consultaMovChapaDetalhe, (sender, query, result) =>
                {
                    resultado.AddRange(result);
                })
                .Add(consultaMovEstoqueIni, (sender, query, result) =>
                {
                    movEstoqueIni.AddRange(result.Select(f => new Tuple<int, int, DateTime, Int64>(f[0], f[1], f[2], f[3])).ToList());
                    movEstoqueIni = movEstoqueIni.OrderByDescending(f => f.Item3).ToList();
                })
                .Add(consultaChapas, (sender, query, result) =>
                {
                    chapas.AddRange(result.Select(f => new Tuple<int, decimal, string, string>(f[0], f[1], f[2], f[3])).ToList());
                })
                .Execute();

            List<int> outrasLeituras = null;

            if (resultado.Count > 0)
            {
                var outrasLeiturasA =
                    SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ChapaCortePeca>("ccp1")
                    .Where(string.Format(@"Date(ccp1.DataCad) >= ?dtIni AND Date(ccp1.DataCad) <= ?dtFim
                        AND ccp1.IdProdImpressaoChapa IN ({0}) AND ccp1.IdProdImpressaoChapa IS NOT NULL AND ccp1.IdProdImpressaoChapa > 0",
                        string.Join(",", resultado.Select(f => f.IdProdImpressaoChapa).ToList()))).Add("?dtIni", dataIni).Add("?dtFim", dataFim)
                    .Select("ccp1.IdProdImpressaoChapa")
                    .Execute()
                    .Select(f => f.GetString(0).StrParaIntNullable().GetValueOrDefault()).ToList();

                var outrasLeiturasB =
                    SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ChapaCortePeca>("ccp2")
                    .Where(string.Format(@"(Date(ccp2.DataCad) < ?dtIni OR Date(ccp2.DataCad) > ?dtFim) AND ccp2.IdProdImpressaoChapa IN ({0})",
                        string.Join(",", outrasLeiturasA))).Add("?dtIni", dataIni).Add("?dtFim", dataFim)
                    .Select("ccp2.IdProdImpressaoChapa")
                    .Execute()
                    .Select(f => f.GetString(0).StrParaIntNullable().GetValueOrDefault()).ToList();

                outrasLeituras = outrasLeiturasA.Select(f => outrasLeiturasB.Contains(f) ? f : 0).ToList().Where(f => f > 0).ToList();
            }

            #endregion

            foreach (var r in resultado)
            {
                if (r.SaidaRevenda)
                {
                    r.M2Lido = r.M2Utilizado;
                    continue;
                }

                var chs = chapas.Where(f => f.Item1 == r.IdProdImpressaoChapa).ToList();

                r.M2Lido = chs.Sum(f => f.Item2);
                r.PlanosCorte = string.Join(", ", chs.Select(f => f.Item3).Distinct().ToArray());
                r.Etiquetas = string.Join(", ", chs.Select(f => f.Item4).ToArray());
                r.TemOutrasLeituras = outrasLeituras != null ? outrasLeituras.Count(f => f == r.IdProdImpressaoChapa) > 0 : false;
            }

            var retorno = resultado.GroupBy(f => string.Format("{0}|{1}", f.IdCorVidro, f.Espessura))
                .Select(f =>
                {
                    var first = f.First();
                    var prods = (from p in f
                                 group p by new
                                 {
                                     p.IdLoja,
                                     p.IdProd
                                 } into g
                                 select new
                                 {
                                     IdLoja = g.Key.IdLoja,
                                     IdProd = g.Key.IdProd,
                                     M2 = g.First().M2Utilizado,
                                     MinData = g.Min(x => x.DataLeitura)
                                 }).ToList();

                    Int64 qtdeIni = 0;
                    decimal m2 = 0;

                    foreach (var p in prods)
                    {
                        var qtde = movEstoqueIni
                            .Where(x => x.Item1 == p.IdProd && x.Item2 == p.IdLoja && x.Item3.Date < p.MinData.Date)
                            .Select(x => x.Item4)
                            .FirstOrDefault();

                        qtdeIni += qtde;
                        m2 += qtde * p.M2;
                    }

                    return new MovChapa()
                    {
                        IdCorVidro = first.IdCorVidro,
                        Espessura = first.Espessura,
                        CorVidro = first.CorVidro,
                        QtdeInicial = qtdeIni,
                        M2Inicial = m2,
                        QtdeUtilizado = f.Count(),
                        M2Utilizado = f.Sum(x => x.M2Utilizado),
                        M2Lido = Math.Round(f.Sum(x => x.M2Lido), 2),
                        Chapas = f.OrderByDescending(x => x.DataLeitura).ToList()
                    };
                }).ToList();


            return retorno;
        }
    }
}
