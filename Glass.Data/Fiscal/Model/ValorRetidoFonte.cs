using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ValorRetidoFonteDAO))]
    [PersistenceClass("valor_retido_fonte")]
    public class ValorRetidoFonte : Sync.Fiscal.EFD.Entidade.IValorRetidoFonte
    {
        #region Propriedades

        [PersistenceProperty("IdValorRetidoFonte", PersistenceParameterType.IdentityKey)]
        public uint IdValorRetidoFonte { get; set; }

        [PersistenceProperty("IdLoja")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("NaturezaRetencao")]
        public Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaRetencao NaturezaRetencao { get; set; }

        [PersistenceProperty("DataRetencao")]
        public DateTime DataRetencao { get; set; }

        [PersistenceProperty("BcRetencao")]
        public decimal BcRetencao { get; set; }

        [PersistenceProperty("ValorRetido")]
        public decimal ValorRetido { get; set; }

        [PersistenceProperty("CodigoReceita")]
        public string CodigoReceita { get; set; }

        [PersistenceProperty("NaturezaReceita")]
        public Sync.Fiscal.Enumeracao.ValorRetidoFonte.NaturezaReceita NaturezaReceita { get; set; }

        [PersistenceProperty("CnpjRetentora")]
        public string CnpjRetentora { get; set; }

        [PersistenceProperty("ValorPisRetido")]
        public decimal ValorPisRetido { get; set; }

        [PersistenceProperty("ValorCofinsRetido")]
        public decimal ValorCofinsRetido { get; set; }

        [PersistenceProperty("TipoDeclarante")]
        public Sync.Fiscal.Enumeracao.ValorRetidoFonte.TipoDeclarante TipoDeclarante { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NaturezaRetencaoString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrNaturezaRetencao((int?)NaturezaRetencao);
            }
        }

        public string NaturezaReceitaString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrNaturezaReceita((int?)NaturezaReceita);
            }
        }

        public string TipoDeclaranteString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrTipoDeclarante((int?)TipoDeclarante);
            }
        }

        #endregion

        #region IValorRetidoFonte Members

        int Sync.Fiscal.EFD.Entidade.IValorRetidoFonte.CodigoLoja
        {
            get { return (int)IdLoja; }
        }

        #endregion
    }
}