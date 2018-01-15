using Glass.Configuracoes;
using Glass.Data.DAL;
using System;
using System.Linq;
using System.Collections.Generic;
using Glass.Data.RelDAL;

namespace Glass.Api.Graficos.Producao.Implementacao
{
    #region Entidades

    #region Painel da Produção

    public class ProducaoDia : IProducaoDia
    {
        public int Dia { get { return DateTime.Now.Day; } }

        public decimal Meta { get; set; }

        public decimal Produzido { get; set; }
    }

    public class ProducaoPendente : IProducaoPendente
    {
        public int Dia { get; set; }

        public decimal Valor { get; set; }
    }

    public class PercentualPerdaMensal : IPercentualPerdaMensal
    {
        public string Mes { get; set; }

        public decimal TotM2Perda { get; set; }

        public decimal TotM2Produzido { get; set; }

        public decimal PorcentagemPerda { get; set; }
    }

    public class ValorPerdaMensal : IValorPerdaMensal
    {
        public string Mes { get; set; }

        public decimal Valor { get; set; }
    }

    /// <summary>
    /// Representação dos dados do Painel da Produção
    /// </summary>
    public class PainelProducao : IPainelProducao
    {
        public IProducaoDia ProducaoDia { get; set; }

        public List<IProducaoPendente> ProducaoPendente { get; set; }

        public List<IValorPerdaMensal> ValorPerdaMensal { get; set; }

        public IPercentualPerdaMensal PercentualPerdaMensal { get; set; }
    }

    #endregion

    /// <summary>
    /// Representação dos dados de Produção Diária Prevista / Realizada
    /// </summary>
    public class ProducaoDiariaPrevistaRealizada : IProducaoDiariaPrevistaRealizada
    {
        public string Data { get; set; }

        public decimal Previsto { get; set; }

        public decimal Realizado { get; set; }

        public decimal Pendente { get; set; }
    }

    /// <summary>
    /// Representação dos dados de Perda por Setor
    /// </summary>
    public class PerdaPorSetor : IPerdaPorSetor
    {
        public int IdSetor { get; set; }

        public string Setor { get; set; }

        public decimal Real { get; set; }

        public decimal Desafio { get; set; }

        public decimal Meta { get; set; }
    }

    /// <summary>
    /// Representação dos dados de Produção Diária por Setor
    /// </summary>
    public class ProducaoDiariaPorSetor : IProducaoDiariaPorSetor
    {
        public int IdSetor { get; set; }

        public decimal Produzido { get; set; }

        public string Setor { get; set; }
    }

    /// <summary>
    /// Representação dos dados de Produção do Dia
    /// </summary>
    public class ProducaoDoDia : IProducaoDoDia
    {
        public string Dia { get; set; }

        public decimal TotalM2 { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Implementação do fluxo de negocio dos graficos de produção
    /// </summary>
    public class ProducaoFluxo : IProducaoFluxo
    {
        /// <summary>
        /// Obtem os dados do Painel da Produção
        /// </summary>
        /// <returns></returns>
        public IPainelProducao ObtemPainelProducao()
        {
            #region Variaveis

            var producaoDia = new ProducaoDia();
            var producaoPendente = new List<IProducaoPendente>();
            var valorPerdaMensal = new List<IValorPerdaMensal>();
            var percentualPerdaMensal = new PercentualPerdaMensal();
            
            #endregion

            #region Produção Dia

            // Meta de produção diária.
            if (!PCPConfig.ConsiderarMetaProducaoM2PecasPorDataFabrica)
                producaoDia.Meta = (decimal)Math.Round(PCPConfig.MetaProducaoDiaria, 2);
            else
                producaoDia.Meta = (decimal)Math.Round(ProdutoPedidoProducaoDAO.Instance.ObtemM2MetaProdDia(), 2);

            var prodPerdaDia = Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetProdPerdaDia();

            if (prodPerdaDia != null)
                producaoDia.Produzido = (decimal)prodPerdaDia.TotProdM2;

            #endregion

            #region Perda Mensal

            var dadosRelatorio = Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance
                .GetPerda(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString(), true, false);

            #region Percentual

            percentualPerdaMensal.Mes = DateTime.Now.ToString("MM/yyyy");
            percentualPerdaMensal.TotM2Perda = dadosRelatorio.ContainsKey(DateTime.Now.ToString("MMMM").ToUpper()) ?
                Math.Round((decimal)dadosRelatorio[DateTime.Now.ToString("MMMM").ToUpper()], 2) : 0;
            percentualPerdaMensal.TotM2Produzido = Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance
                .GetTotM2ProducaoMensal(DateTime.Now.Month, DateTime.Now.Year);
            percentualPerdaMensal.PorcentagemPerda = Math.Round(percentualPerdaMensal.TotM2Perda > 0 &&
                percentualPerdaMensal.TotM2Produzido > 0 ? (percentualPerdaMensal.TotM2Perda * 100 / percentualPerdaMensal.TotM2Produzido) : 0, 2);

            #endregion

            #region Valor

            // Popula os arrays de dados.
            foreach (var chave in dadosRelatorio)
            {
                valorPerdaMensal.Add(new ValorPerdaMensal()
                {
                    Mes = chave.Key,
                    Valor = Math.Round((decimal)(chave.Value > 0 ? chave.Value : 0.001), 2)
                });
            }

            #endregion

            #endregion

            #region Produção Pendente

            // Obtém os dados da produção pendente.
            var lstResultado = Data.RelDAL.PecasPendentesDAO.Instance
                .ProducaoPendente(DateTime.Now.AddDays(-9).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

            if (lstResultado.Count == 0)
                lstResultado.Add(0, 0.0);

            // Popula os arrays de dados.
            foreach (var i in lstResultado.Keys)
            {
                producaoPendente.Add(new ProducaoPendente()
                {
                    Dia = i,
                    Valor = (decimal)lstResultado[i]
                });
            }

            #endregion

            var retorno = new PainelProducao()
            {
                ProducaoDia = producaoDia,
                ValorPerdaMensal = valorPerdaMensal,
                PercentualPerdaMensal = percentualPerdaMensal,
                ProducaoPendente = producaoPendente
            };

            return retorno;
        }

        /// <summary>
        /// Obtem os dados da produção diária prevista realizada
        /// </summary>
        /// <returns></returns>
        public List<IProducaoDiariaPrevistaRealizada> ObtemProducaoDiariaPrevistaRealizada()
        {
            var dados = WebGlass.Business.ProducaoDiariaRealizada.Fluxo.
                BuscarEValidar.Instance.ObtemDadosProducaoForPainelPlanejamento();

            var retorno = dados.Select(f => new ProducaoDiariaPrevistaRealizada()
            {
                Data = f.DataFabricaStr,
                Pendente = f.TotMPendente,
                Previsto = f.TotMPrevisto,
                Realizado = f.TotMRealizado
            }).ToList<IProducaoDiariaPrevistaRealizada>();

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de Perda por Setor
        /// </summary>
        /// <returns></returns>
        public List<IPerdaPorSetor> ObtemPerdaPorSetor()
        {
            var dados = Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance
                .GetPerdaSetores(0, DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());

            var retorno = dados.Select(f => new PerdaPorSetor()
            {
                IdSetor = (int)f.IdSetor,
                Setor = f.DescricaoSetor,
                Desafio = (decimal)f.DesafioPerda,
                Meta = (decimal)f.MetaPerda,
                Real = (decimal)f.TotPerdaM2
            }).ToList<IPerdaPorSetor>();

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de Produção Diária por Setor
        /// </summary>
        /// <returns></returns>
        public List<IProducaoDiariaPorSetor> ObtemProducaoDiariaPorSetor()
        {
            //var dados = WebGlass.Business.ProducaoDiariaRealizada.Fluxo.BuscarEValidar
            //    .Instance.ObtemDadosProducaoForPainelPlanejamentoSetores(DateTime.Now);

            var dados = ProducaoDiariaRealizadaDAO.Instance.ObtemProducaoRealizadaTodosSetores();

            var retorno = dados.Select(f => new ProducaoDiariaPorSetor()
            {
                Setor = f.DescricaoSetor,
                Produzido = (decimal)f.TotRealizado
                //IdSetor = (int)f.CodigoSetor,
                //Setor = f.NomeSetor,
                //Produzido = f.M2Realizado
            }).ToList<IProducaoDiariaPorSetor>();

            return retorno;
        }

        /// <summary>
        /// Obtem os dados de Produção do Dia
        /// </summary>
        /// <returns></returns>
        public List<IProducaoDoDia> ObtemProducaoDia()
        {
            var dados = Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetProdPerdaDia();
            var producaoDoDia = new ProducaoDoDia()
            {
                Dia = dados.Dia.ToString(),
                TotalM2 = (decimal)dados.TotProdM2
            };

            return new List<IProducaoDoDia> { producaoDoDia };
        }
    }

    #endregion
}
