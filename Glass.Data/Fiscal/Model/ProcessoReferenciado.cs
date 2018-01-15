using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProcessoReferenciadoDAO))]
    [PersistenceClass("processo_referenciado")]
    public class ProcessoReferenciado : Sync.Fiscal.EFD.Entidade.IProcessoReferenciado
    {
        #region Propriedades

        [PersistenceProperty("IDPROCREF", PersistenceParameterType.IdentityKey)]
        public uint IdProcRef { get; set; }

        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDCTE")]
        public uint? IdCte { get; set; }

        [PersistenceProperty("ORIGEM")]
        public int Origem { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        #endregion

        #region Propriedades de suporte

        public string DescrOrigem
        {
            get { return DataSourcesEFD.Instance.GetDescrOrigemProcessoRef(Origem); }
        }

        #endregion

        #region IProcessoReferenciado Members

        Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem Sync.Fiscal.EFD.Entidade.IProcessoReferenciado.Origem
        {
            get { return (Sync.Fiscal.Enumeracao.ProcessoReferenciado.Origem)Origem; }
        }

        #endregion
    }
}