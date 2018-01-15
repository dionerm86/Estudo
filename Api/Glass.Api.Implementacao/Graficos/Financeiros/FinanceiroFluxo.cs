
using System;
using System.Collections.Generic;
using Colosoft;
using System.Linq;

namespace Glass.Api.Graficos.Financeiros.Implementacao
{
    public class FinanceiroFluxo : IFinanceiroFluxo
    {
        #region Entidades

        /// <summary>
        /// Assinatura dos dados do grafico de Previsão financeira
        /// </summary>
        public class PrevisaoFinanceira : IPrevisaoFinanceira
        {
            public TipoPrevisaoFinanceiraEnum TipoPrevisao { get; set; }

            public string DescrTipoPrevisao
            {
                get
                {
                    return TipoPrevisao.Translate().Format();
                }
            }
            public decimal ValorHoje { get; set; }

            public decimal Valor30Dias { get; set; }

            public decimal Valor60Dias { get; set; }

            public decimal Valor90Dias { get; set; }

            public decimal ValorMais90Dias { get; set; }
        }

        /// <summary>
        /// Assinatura dos dados do grafico de Recebimento por Tipo
        /// </summary>
        public class RecebimentosPorTipo : IRecebimentosPorTipo
        {
            public string TipoRecebimento { get; set; }

            public decimal Valor { get; set; }
        }

        /// <summary>
        /// Assinatura dos dados do grafico de pagamentos
        /// </summary>
        public class Pagamentos: IPagamentos
        {
            public string Data { get; set; }

            public string TipoPagamento { get; set; }

            public decimal Valor { get; set; }
        }

        #endregion

        #region Fluxos

        /// <summary>
        /// Obtem os dados de previsão financeira a receber
        /// </summary>
        /// <returns></returns>
        public List<IPagamentos> ObtemPagamentos()
        {
            var dtIni = DateTime.Now.AddDays(-6).Date;
            var dtfim = DateTime.Now.Date;

            dynamic dados = Data.DAL.PagtoDAO.Instance.ObterPagamentosParaApi(dtIni, dtfim);

            var retorno = new List<IPagamentos>();

            foreach (var d in dados)
            {
                retorno.Add(new Pagamentos()
                {
                    Data = d.GetType().GetProperty("Data").GetValue(d, null),
                    TipoPagamento = d.GetType().GetProperty("FormaPagto").GetValue(d, null),
                    Valor = d.GetType().GetProperty("Valor").GetValue(d, null)
                });
            }

            return retorno;
        }

        /// Obtem os dados de previsão financeira a receber Vencida
        /// </summary>
        /// <returns></returns>
        public List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraReceberVencida()
        {
            var dados = Data.RelDAL.PrevisaoFinanceiraDAO.Instance.GetReceber(0, DateTime.Now.ToShortDateString(), true);

            var retorno = new List<IPrevisaoFinanceira>();

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ContasReceber,
                ValorMais90Dias = dados.VencidasMais90Dias,
                Valor90Dias = dados.Vencidas90Dias,
                Valor60Dias = dados.Vencidas60Dias,
                Valor30Dias = dados.Vencidas30Dias,
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ChequeTerceiros,
                ValorMais90Dias = dados.ChequesVencidosMais90Dias,
                Valor90Dias = dados.ChequesVencidos90Dias,
                Valor60Dias = dados.ChequesVencidos60Dias,
                Valor30Dias = dados.ChequesVencidos30Dias,
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.PedidosProducao,
                ValorMais90Dias = dados.PedidosVencidosMais90Dias,
                Valor90Dias = dados.PedidosVencidos90Dias,
                Valor60Dias = dados.PedidosVencidos60Dias,
                Valor30Dias = dados.PedidosVencidos30Dias,
            });

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de previsão financeira a pagar Vencida.
        /// </summary>
        /// <returns></returns>
        public List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraPagarVencida()
        {
            var dados = Data.RelDAL.PrevisaoFinanceiraDAO.Instance.GetPagar(0, DateTime.Now.ToShortDateString(), false);

            var retorno = new List<IPrevisaoFinanceira>();

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ContasPagar,
                ValorMais90Dias = dados.VencidasMais90Dias,
                Valor90Dias = dados.Vencidas90Dias,
                Valor60Dias = dados.Vencidas60Dias,
                Valor30Dias = dados.Vencidas30Dias,
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ChequeProprios,
                ValorMais90Dias = dados.ChequesVencidosMais90Dias,
                Valor90Dias = dados.ChequesVencidos90Dias,
                Valor60Dias = dados.ChequesVencidos60Dias,
                Valor30Dias = dados.ChequesVencidos30Dias,
            });

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de previsão financeira a pagar
        /// </summary>
        /// <returns></returns>
        public List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraPagar()
        {
            var dados = Data.RelDAL.PrevisaoFinanceiraDAO.Instance.GetPagar(0, DateTime.Now.ToShortDateString(), false);

            var retorno = new List<IPrevisaoFinanceira>();

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ContasPagar,
                ValorHoje = dados.VencimentoHoje,
                Valor30Dias = dados.Vencer30Dias,
                Valor60Dias = dados.Vencer60Dias,
                Valor90Dias = dados.Vencer90Dias,
                ValorMais90Dias = dados.VencerMais90Dias
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ChequeProprios,
                ValorHoje = dados.ChequesVencimentoHoje,
                Valor30Dias = dados.ChequesVencer30Dias,
                Valor60Dias = dados.ChequesVencer60Dias,
                Valor90Dias = dados.ChequesVencer90Dias,
                ValorMais90Dias = dados.ChequesVencerMais90Dias
            });

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de recebimento por tipo
        /// </summary>
        /// <returns></returns>
        public List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraReceber()
        {
            var dados = Data.RelDAL.PrevisaoFinanceiraDAO.Instance.GetReceber(0, DateTime.Now.ToShortDateString(), true);

            var retorno = new List<IPrevisaoFinanceira>();

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ContasReceber,
                ValorHoje = dados.VencimentoHoje,
                Valor30Dias = dados.Vencer30Dias,
                Valor60Dias = dados.Vencer60Dias,
                Valor90Dias = dados.Vencer90Dias,
                ValorMais90Dias = dados.VencerMais90Dias
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.ChequeTerceiros,
                ValorHoje = dados.ChequesVencimentoHoje,
                Valor30Dias = dados.ChequesVencer30Dias,
                Valor60Dias = dados.ChequesVencer60Dias,
                Valor90Dias = dados.ChequesVencer90Dias,
                ValorMais90Dias = dados.ChequesVencerMais90Dias
            });

            retorno.Add(new PrevisaoFinanceira()
            {
                TipoPrevisao = TipoPrevisaoFinanceiraEnum.PedidosProducao,
                ValorHoje = dados.PedidosVencimentoHoje,
                Valor30Dias = dados.PedidosVencer30Dias,
                Valor60Dias = dados.PedidosVencer60Dias,
                Valor90Dias = dados.PedidosVencer90Dias,
                ValorMais90Dias = dados.PedidosVencerMais90Dias
            });

            return retorno;
        }

        /// <summary>
        /// Obtem os dados do grafico de pagamentos
        /// </summary>
        /// <returns></returns>
        public List<IRecebimentosPorTipo> ObtemRecebimentoPorTipo()
        {
            var dados = Data.RelDAL.RecebimentoDAO.Instance.GetRecebimentosTipo(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortDateString(), 0, 0);

            var retorno = dados
                .Where(f=>!f.Descricao.Contains("*") && f.Descricao.ToLower() != "total")
                .Select(f => new RecebimentosPorTipo()
                {
                    TipoRecebimento = f.Descricao,
                    Valor = f.Valor
                })
                .ToList<IRecebimentosPorTipo>();

            return retorno;
        }

        #endregion
    }
}
