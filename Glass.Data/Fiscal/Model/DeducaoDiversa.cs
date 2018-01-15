using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DeducaoDiversaDAO))]
    [PersistenceClass("deducoes_diversas")]
    public class DeducaoDiversa : Sync.Fiscal.EFD.Entidade.IDeducaoDiversa
    {
        #region Propriedades

        [PersistenceProperty("IdDeducao", PersistenceParameterType.IdentityKey)]
        public uint IdDeducao { get; set; }

        [PersistenceProperty("IdLoja")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("OrigemDeducao")]
        public Sync.Fiscal.Enumeracao.DeducaoDiversa.OrigemDeducao OrigemDeducao { get; set; }

        [PersistenceProperty("NaturezaDeducao")]
        public Sync.Fiscal.Enumeracao.DeducaoDiversa.NaturezaDeducao NaturezaDeducao { get; set; }

        [PersistenceProperty("ValorPisDeduzir")]
        public decimal ValorPisDeduzir { get; set; }

        [PersistenceProperty("ValorCofinsDeduzir")]
        public decimal ValorCofinsDeduzir { get; set; }

        [PersistenceProperty("BcDeducao")]
        public decimal BcDeducao { get; set; }

        [PersistenceProperty("CnpjDedutora")]
        public string CnpjDedutora { get; set; }

        [PersistenceProperty("InformacoesCompl")]
        public string InformacoesComplementares { get; set; }

        [PersistenceProperty("DataDeducao")]
        public DateTime DataDeducao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string OrigemDeducaoString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrOrigemDeducao((int?)OrigemDeducao);
            }
        }

        public string NaturezaDeducaoString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrNaturezaDeducao((int?)NaturezaDeducao);
            }
        }

        #endregion

        #region IDeducaoDiversa Members

        int Sync.Fiscal.EFD.Entidade.IDeducaoDiversa.CodigoLoja
        {
            get { return (int)IdLoja; }
        }

        #endregion
    }
}