using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Estoque.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Estoque.Negocios.Componentes
{
    public class MovChapaFluxo : IMovChapaFluxo
    {
        private class DadosEstoque
        {
            public int IdProduto { get; set; }

            public int IdLoja { get; set; }

            public DateTime DataMovimentacao { get; set; }

            public long SaldoQuantidade { get; set; }

            public int IdCorVidro { get; set; }

            public float Espessura { get; set; }

            public int Altura { get; set; }

            public int Largura { get; set; }
        }

        public IList<MovChapa> ObtemMovChapa(string idsCorVidro, float espessura, int altura, int largura, DateTime dataIni, DateTime dataFim)
        {
            #region Variaveis Locais

            var resultado = new List<MovChapaDetalhe>();
            var movEstoqueIni = new List<DadosEstoque>();
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
                    .From<Data.Model.MovEstoque>("mv")
                    .InnerJoin<Data.Model.Produto>("mv.IdProd = p.IdProd", "p")
                    .InnerJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd = sg.IdSubgrupoProd", "sg")
                    .Where($"DATE(mv.DataMov)<=DATE(?dataIni) AND sg.TipoSubgrupo={(int)TipoSubgrupoProd.ChapasVidro}")
                        .Add("?dataIni", dataIni)
                    .OrderBy("mv.DataMov DESC, mv.IdMovEstoque DESC")
                    .GroupBy("mv.IdProd")
                    .Select(@"mv.IdProd, mv.IdLoja, mv.DataMov, mv.SaldoQtdeMov, p.IdCorVidro, p.Espessura, p.Altura, p.Largura");

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

            if (altura > 0)
                consultaMovChapaDetalhe.WhereClause.And("prod.Altura = ?altura").Add("?altura", altura);

            if (largura > 0)
                consultaMovChapaDetalhe.WhereClause.And("prod.Largura = ?largura").Add("?largura", largura);

            #endregion

            #region Execução

            SourceContext.Instance.CreateMultiQuery()
                .Add<MovChapaDetalhe>(consultaMovChapaDetalhe, (sender, query, result) =>
                {
                    resultado.AddRange(result);
                })
                .Add(consultaMovEstoqueIni, (sender, query, result) =>
                {
                    movEstoqueIni.AddRange(result
                        .Select(f => new DadosEstoque
                        {
                            IdProduto = f[0],
                            IdLoja = f[1],
                            DataMovimentacao = f[2],
                            SaldoQuantidade = f[3],
                            IdCorVidro = f[4],
                            Espessura = f[5],
                            Altura = f[6],
                            Largura = f[7],
                        })
                        .OrderByDescending(f => f.DataMovimentacao)
                        .ToList());
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

                outrasLeituras = outrasLeiturasA.Select(f => outrasLeiturasB.Contains(f) ? f : 0).Where(f => f > 0).ToList();
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
                r.TemOutrasLeituras = outrasLeituras != null && outrasLeituras.Any(f => f == r.IdProdImpressaoChapa);
            }

            var retorno = resultado.GroupBy(f => string.Format("{0}|{1}", f.IdCorVidro, f.Espessura))
                .Select(f =>
                {
                    var first = f.First();

                    var estoqueAtual = ProdutoLojaDAO.Instance.ObterEstoqueAtualChapasPorCorEspessura(first.IdCorVidro, first.Espessura);
                    int quantidadeDisponivel = estoqueAtual.Item1;
                    decimal metroQuadradoDisponivel = estoqueAtual.Item2;

                    var quantidadeUtilizada = f.Count();
                    var metroQuadradoUtilizado = f.Sum(x => x.M2Utilizado);

                    return new MovChapa()
                    {
                        IdCorVidro = first.IdCorVidro,
                        Espessura = first.Espessura,
                        CorVidro = first.CorVidro,
                        QtdeDisponivel = quantidadeDisponivel,
                        M2Disponivel = metroQuadradoDisponivel,
                        QtdeUtilizado = quantidadeUtilizada,
                        M2Utilizado = metroQuadradoUtilizado,
                        M2Lido = Math.Round(f.Sum(x => x.M2Lido), 2),
                        Chapas = f.OrderByDescending(x => x.DataLeitura).ToList()
                    };
                }).ToList();


            return retorno;
        }
    }
}
