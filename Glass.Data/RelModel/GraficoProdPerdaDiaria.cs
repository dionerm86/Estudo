using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(GraficoProdPerdaDiariaDAO))]
    [PersistenceClass("producao_perda_diaria")]
    public class GraficoProdPerdaDiaria
    {
        #region Propriedades

        [PersistenceProperty("Dia")]
        public Int64 Dia { get; set; }

        [PersistenceProperty("idSetor")]
        public UInt64 IdSetor { get; set; }

        [PersistenceProperty("descricao")]
        public string DescricaoSetor { get; set; }

        public double TotProdM2
        {
            get { return (double)TotProdM2Decimal; }
            set { TotProdM2Decimal = (decimal)value; }
        }

        [PersistenceProperty("TotProdM2")]
        private decimal TotProdM2Decimal { get; set; }

        [PersistenceProperty("TotPerdaM2")]
        public double TotPerdaM2 { get; set; }

        private double _desafioPerda;

        [PersistenceProperty("desafioPerda")]
        public double DesafioPerda
        {
            get { return Math.Round(_desafioPerda, 4); }
            set { _desafioPerda = value; }
        }

        private double _metaPerda;

        [PersistenceProperty("metaPerda")]
        public double MetaPerda
        {
            get { return Math.Round(_metaPerda, 4); }
            set { _metaPerda = value; }
        }

        [PersistenceProperty("espessura")]
        public float EspessuraVidro { get; set; }

        [PersistenceProperty("corVidro")]
        public string CorVidro { get; set; }

        [PersistenceProperty("Ano", DirectionParameter.InputOptional)]
        public int Ano { get; set; }

        [PersistenceProperty("Mes", DirectionParameter.InputOptional)]
        public int Mes { get; set; }

        #endregion

        #region Propriedades de Suporte

        private double _mediaDiariaProducao;

        public double MediaDiariaProducao
        {
            get { return Math.Round(_mediaDiariaProducao, 2); }
            set { _mediaDiariaProducao = value; }
        }

        public double ProducaoAcumulada { get; set; }

        public double PerdaAcumulada { get; set; }

        public double IndicePerdaDiaria
        {
            get { return this.ProducaoAcumulada != 0 ? Math.Round(this.PerdaAcumulada / this.ProducaoAcumulada * 100, 3) : 100; }
        }

        public double IndicePerdaProducao
        {
            get
            {
                return this.TotProdM2 != 0 ? Math.Round(this.TotPerdaM2 / this.TotProdM2 * 100, 2) : 100;
            }
        }

        public string IndicePerdaProdFormat
        {
            get
            {
                return this.IndicePerdaProducao.ToString() + "%";
            }
        }

        public string EspessuraFormat
        {
            get
            {
                return this.EspessuraVidro.ToString() + "mm";
            }
        }

        #endregion
    }
}