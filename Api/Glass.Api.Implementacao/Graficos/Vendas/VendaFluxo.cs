using Glass.Data.RelDAL;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Api.Graficos.Vendas.Implementacao
{
    #region Entidades

    /// <summary>
    /// Representação dos dados do gráico de Vendas por Cliente (Curva ABC)
    /// </summary>
    public class VendasPorClienteCurvaAbc : IVendasPorClienteCurvaAbc
    {
        public int IdCliente { get; set; }

        public string Cliente { get; set; }

        public string Mes { get; set; }

        public decimal Valor { get; set; }
    }

    /// <summary>
    /// Representação dos dados do gráico de Vendas (Curva ABC)
    /// </summary>
    public class VendasCurvaAbc : IVendasCurvaAbc
    {
        public int IdLoja { get; set; }

        public string Loja { get; set; }

        public string Mes { get; set; }

        public decimal Valor { get; set; }
    }

    public class VendasPorPedido : IVendasPorPedido
    {
        public string Data { get; set; }

        public string TipoPedido { get; set; }

        public decimal Valor { get; set; }
    }

    public class VendasPorProduto : IVendasPorProduto
    {
        public int IdProd { get; set; }

        public string Descricao { get; set; }

        public string M2 { get; set; }
    }

    /// <summary>
    /// Representação dos dados do gráico de Vendas (Curva ABC)
    /// </summary>
    public class VendasPorVendedor : IVendasPorVendedor
    {
        public int IdVendedor { get; set; }

        public string Nome { get; set; }

        public string Mes { get; set; }

        public decimal Valor { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Implementação do fluxo de negocio dos graficos de venda
    /// </summary>
    public class VendaFluxo : IVendaFluxo
    {
        /// <summary>
        /// Obtem os dados do grafico de Vendas (Curva ABC)
        /// </summary>
        /// <returns></returns>
        public List<IVendasCurvaAbc> ObtemVendasCurvaAbc()
        {
            var dt = DateTime.Now;
            var dtIni = new DateTime(dt.AddMonths(-5).Year, dt.AddMonths(-5).Month, 1);
            var dtFim = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));

            var idsLojas = LojaDAO.Instance.GetIdsLojasAtivas();

            var dados = ChartVendasDAO.Instance.GetVendasForChart(0, 0, 0, 0, null, 0, dtIni.ToShortDateString(), dtFim.ToShortDateString(), null, 1, "loja", idsLojas,
                false, true, true, true);

            var retorno = new List<IVendasCurvaAbc>();

            var dados2 = dados.Where(f => f.Value.Sum(x => x.TotalVenda) > 0).ToList();

            foreach (var l in dados2)
            {
                foreach (var d in l.Value)
                {
                    retorno.Add(new VendasCurvaAbc()
                    {
                        IdLoja = (int)l.Key,
                        Loja = d.NomeLoja,
                        Mes = d.Periodo,
                        Valor = d.TotalVenda
                    });
                }
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do gráfico de Vendas por Cliente (Curva ABC)
        /// </summary>
        /// <returns></returns>
        public List<IVendasPorClienteCurvaAbc> ObtemVendasPorClienteCurvaAbc()
        {
            var dt = DateTime.Now;

            var dados = VendasDAO.Instance.GetList(0, null, null, false, dt.AddMonths(-5).Month, dt.AddMonths(-5).Year,
               dt.Month, dt.Year, 2, null, null, null, null, 0, 0, 0, 0, 0, false, null, 0, "", null, 0, 10);

            var retorno = new List<IVendasPorClienteCurvaAbc>();

            foreach (var d in dados)
            {
                var lst = new List<IVendasPorClienteCurvaAbc>();

                for (int i = 0; i < d.MesVenda.Length; i++)
                {
                    lst.Add(new VendasPorClienteCurvaAbc()
                    {
                        IdCliente = (int)d.IdCliente,
                        Cliente = d.NomeCliente,
                        Mes = d.MesVenda[i],
                        Valor = Math.Round(d.ValorVenda[i], 2)
                    });
                }

                for (var i = DateTime.Now.AddMonths(-5).Date; i <= DateTime.Now.Date; i = i.AddMonths(1))
                {
                    var item = lst.Where(f => f.Mes == i.Month + "/" + i.Year).FirstOrDefault();

                    if (item == null)
                        retorno.Add(new VendasPorClienteCurvaAbc()
                        {
                            IdCliente = lst[0].IdCliente,
                            Cliente = lst[0].Cliente,
                            Mes = i.Month + "/" + i.Year,
                            Valor = 0
                        });
                    else
                        retorno.Add(item);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do gráfico de Vendas por Pedido
        /// </summary>
        /// <returns></returns>
        public List<IVendasPorPedido> ObtemVendasPorPedido()
        {
            var dados = PedidoDAO.Instance.PesquisarVendasPedido();

            var retorno = new List<IVendasPorPedido>();

            dados = dados.OrderBy(f => f.Item3).ThenBy(f => f.Item1).ToList();

            foreach (var d in dados)
            {
                retorno.Add(new VendasPorPedido()
                {
                    TipoPedido = d.Item2,
                    Data = d.Item3.ToShortDateString(),
                    Valor = Math.Round(d.Item4, 2)

                });
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do gráfico de Vendas por Produto
        /// </summary>
        /// <returns></returns>
        public List<IVendasPorProduto> ObtemVendasPorProduto()
        {
            var dtIni = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dtFim = new DateTime(dtIni.Year, dtIni.Month, DateTime.DaysInMonth(dtIni.Year, dtIni.Month));

            //var dtIni = new DateTime(2015, 12, 1);
            //var dtFim = new DateTime(dtIni.Year, dtIni.Month, DateTime.DaysInMonth(dtIni.Year, dtIni.Month));

            var dados = GraficoProdutosDAO.Instance.GetMaisVendidos(0, 0, 0, string.Empty, (int)NomeGrupoProd.Vidro, 0, 10,
                1, dtIni.ToShortDateString(), dtFim.ToShortDateString(), null, null, false);


            var retorno = new List<IVendasPorProduto>();

            foreach (var d in dados)
            {
                retorno.Add(new VendasPorProduto()
                {
                    IdProd = (int)d.IdProd,
                    Descricao = d.DescrProduto,
                    M2 = ((decimal)d.TotalQtde) + "m²"
                });
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do grafico de Vendas (Curva ABC)
        /// </summary>
        /// <returns></returns>
        public List<IVendasPorVendedor> ObtemVendasPorVendedor()
        {
            var dt = DateTime.Now;
            var dtIni = new DateTime(dt.AddMonths(-5).Year, dt.AddMonths(-5).Month, 1);
            var dtFim = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));

            var idsVendedores = FuncionarioDAO.Instance.GetVendedoresComVendas(0, false, dtIni.ToShortDateString(), dtFim.ToShortDateString())
                .Select(f => (uint)f.IdFunc).ToList();

            var dados = ChartVendasDAO.Instance.GetVendasForChart(0, 0, 0, 0, null, 0, dtIni.ToShortDateString(), dtFim.ToShortDateString(), null, 2, "emissor", idsVendedores,
                false, true, true, true);

            var retorno = new List<IVendasPorVendedor>();
            var lst = new List<IVendasPorVendedor>();

            foreach (var l in dados)
            {
                if (l.Key == 0)
                    continue;

                foreach (var d in l.Value)
                {
                    lst.Add(new VendasPorVendedor()
                    {
                        IdVendedor = (int)l.Key,
                        Nome = d.NomeVendedor,
                        Mes = d.Periodo,
                        Valor = d.TotalVenda
                    });
                }
            }

            var index = 0;

            foreach (var item in lst.GroupBy(f => f.IdVendedor)
                .Select(f => new { IdVendedor = f.Key, Nome = f.FirstOrDefault().Nome != null ? f.FirstOrDefault().Nome : "" ,Total = f.Sum(x => x.Valor) })
                .OrderByDescending(f => f.Total))
            {
                if (index == 5)
                    return retorno;

                for (var i = DateTime.Now.AddMonths(-5).Date; i <= DateTime.Now.Date; i = i.AddMonths(1))
                {
                    var aux = lst.Where(f => f.Mes == (i.ToString("MMMM").Substring(0, 3) + "-" + i.Year.ToString().Substring(2, 2)) &&
                    f.IdVendedor == item.IdVendedor).FirstOrDefault();

                    if (aux == null)
                        retorno.Add(new VendasPorVendedor()
                        {
                            IdVendedor = item.IdVendedor,
                            Nome = item.Nome,
                            Mes = i.ToString("MMMM").Substring(0, 3) + "-" + i.Year.ToString().Substring(2, 2),
                            Valor = 0
                        });
                    else
                        retorno.Add(aux);
                }

                index++;
            }

            return retorno;
        }
    }

    #endregion
}
