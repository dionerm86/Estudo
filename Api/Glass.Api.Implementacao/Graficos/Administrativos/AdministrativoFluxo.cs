using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Api.Graficos.Administrativos.Implementacao
{
    #region Entidades

    /// <summary>
    /// Dados do DRE
    /// </summary>
    public class Dre : IDre
    {
        public string Loja { get; set; }

        public string Data { get; set; }

        public decimal Valor { get; set; }
    }

    /// <summary>
    /// Dados do ponto de equilibrio
    /// </summary>
    public class PontoEquilibrio : IPontoEquilibrio
    {
        public string Item { get; set; }

        public decimal Valor { get; set; }

        public string Percentual { get; set; }
    }

    /// <summary>
    /// Dados do tempo gasto por etapa
    /// </summary>
    public class TempoGastoPorEtapa : ITempoGastoPorEtapa
    {
        public string Data { get; set; }

        public string Dias { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados da metragem produzida
    /// </summary>
    public class MetragemProduzir : IMetragemProduzir
    {
        public string Cor { get; set; }

        public float Espessura { get; set; }

        public decimal M2 { get; set; }

        public int Qtde { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de negocio dos graficos administrativos
    /// </summary>
    public class AdministrativoFluxo : IAdministrativoFluxo
    {
        /// <summary>
        /// Obtem os dados do DRE
        /// </summary>
        /// <returns></returns>
        public List<IDre> ObtemDre()
        {
            var dtIni = new DateTime(DateTime.Now.AddMonths(-5).Year, DateTime.Now.AddMonths(-5).Month, 1);
            var dtFim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var ids = Data.DAL.LojaDAO.Instance.GetAll().Select(f => (uint)f.IdLoja).ToList();

            var dados = Data.RelDAL.ChartDREDAO.Instance.ObterDados(0, 0, 0, dtIni.ToShortDateString(), dtFim.ToShortDateString(), 0, 0, false, ids);

            var retorno = new List<IDre>();

            foreach (var lojas in dados)
            {
                foreach (var d in lojas.Value)
                {
                    retorno.Add(new Dre()
                    {
                        Loja = d.NomeLoja,
                        Data = d.Periodo,
                        Valor = d.Total
                    });
                }
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados da metragem a ser produduzida
        /// </summary>
        /// <returns></returns>
        public List<IMetragemProduzir> ObtemMetragemProduzir()
        {

            var lstProdPedEsp = Data.DAL.ProdutosPedidoEspelhoDAO.Instance.GetForRpt(0, 0,null, 0, 0, 0, 2, "7",
                null, null, null, null, null, null, null, null, null, false, false, false, null, null, null, null, null, null);

            var lst = Data.RelDAL.ProdutosPedidoEspelhoRptDAO.Instance.CopiaLista(lstProdPedEsp.ToArray(),
                             Data.RelModel.ProdutosPedidoEspelhoRpt.TipoConstrutor.ListaPedidoEspelho);

            var dados = lst.GroupBy(f => new { f.Cor, f.Espessura }).Select(x => new
            {
                Cor = x.Key.Cor,
                Espessura = x.Key.Espessura,
                M2 = x.Sum(y => y.TotM2Rpt),
                Qtde = x.Sum(y => y.QtdeRpt)
            }).ToList();

            var retorno = new List<IMetragemProduzir>();

            foreach (var d in dados)
            {
                if (string.IsNullOrEmpty(d.Cor))
                    continue;

                retorno.Add(new MetragemProduzir()
                {
                    Cor = d.Cor,
                    Espessura = d.Espessura,
                    M2 = (decimal)d.M2,
                    Qtde = (int)d.Qtde
                });
            }

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do ponto de Equilibrio
        /// </summary>
        /// <returns></returns>
        public List<IPontoEquilibrio> ObtemPontoEquilibrio()
        {
            var dtIni = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dtFim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var lista = Data.RelDAL.PontoEquilibrioDAO.Instance.GetPontoEquilibrio(dtIni.ToShortDateString(), dtFim.ToShortDateString(), null);

            var dados = new List<IPontoEquilibrio>();

            foreach (var item in lista)
            {
                dados.Add(new PontoEquilibrio()
                {
                    Item = item.Indice + " " + item.Item,
                    Percentual = item.Percentual,
                    Valor = item.Valor
                });

                if (item.subItens != null)
                {
                    var index = 0;
                    foreach (var subItem in item.subItens)
                    {
                        dados.Add(new PontoEquilibrio()
                        {
                            Item = item.Item + "." + index + " " + subItem.Item,
                            Percentual = subItem.Percentual,
                            Valor = subItem.Valor
                        });
                        index++;
                    }
                }
            }

            return dados;
        }

        /// <summary>
        /// Obtem os dados do tempo gasto por etapa
        /// </summary>
        /// <returns></returns>
        public List<ITempoGastoPorEtapa> ObtemTempoGastoPorEtapa()
        {
            var dtIni = new DateTime(DateTime.Now.AddMonths(-5).Year, DateTime.Now.AddMonths(-5).Month, 1);
            var dtFim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var lst = Data.RelDAL.ProducaoSituacaoDataDAO.Instance
                .GetForRptTempoLiberacao(dtIni.ToShortDateString(), dtFim.ToShortDateString(), 0, 0, null, 0, 0);

            var dados = lst.Where(f => f.DataLiberacao.HasValue).GroupBy(f => new { f.Data.Year, f.Data.Month })
                .Select(f => new TempoGastoPorEtapa()
                {
                    Data = f.Key.Month + "/" + f.Key.Year,
                    Dias = Math.Round(f.Average(x => x.DiferencaDataLiberacao), 3).ToString()
                }).ToList<ITempoGastoPorEtapa>();

            return dados;
        }
    }

    #endregion
}
