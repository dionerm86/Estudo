using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable,
    PersistenceBaseDAO(typeof(EfdCteDAO)),
    PersistenceClass("efd_cte")]
    public class EfdCte : Sync.Fiscal.EFD.Entidade.IEfdCTe
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDCONTACONTABIL")]
        public uint? IdContaContabil { get; set; }

        [PersistenceProperty("NATUREZABCCRED")]
        public int? NaturezaBcCred { get; set; }

        [PersistenceProperty("INDNATUREZAFRETE")]
        public int? IndNaturezaFrete { get; set; }

        [PersistenceProperty("CODCONT")]
        public int? CodCont { get; set; }

        [PersistenceProperty("CODCRED")]
        public int? CodCred { get; set; }

        #endregion

        #region Propriedades extendidas

        [PersistenceProperty("CODINTERNOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        #endregion

        #region IEfdCTe Members

        Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial? Sync.Fiscal.EFD.Entidade.IEfdCTe.CodCont
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial?)CodCont; }
        }

        Sync.Fiscal.Enumeracao.CodigoTipoCredito? Sync.Fiscal.EFD.Entidade.IEfdCTe.CodCred
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoTipoCredito?)CodCred; }
        }

        Sync.Fiscal.EFD.DataSources.IndNaturezaFrete? Sync.Fiscal.EFD.Entidade.IEfdCTe.IndicadorNaturezaFrete
        {
            get { return (Sync.Fiscal.EFD.DataSources.IndNaturezaFrete?)IndNaturezaFrete; }
        }

        Sync.Fiscal.EFD.DataSources.NaturezaBcCredito? Sync.Fiscal.EFD.Entidade.IEfdCTe.NaturezaBcCredito
        {
            get { return (Sync.Fiscal.EFD.DataSources.NaturezaBcCredito?)NaturezaBcCred; }
        }

        int? Sync.Fiscal.EFD.Entidade.IEfdCTe.CodigoContaContabil
        {
            get { return (int?)IdContaContabil; }
        }

        #endregion
    }
}
