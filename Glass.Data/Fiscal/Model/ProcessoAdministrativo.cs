using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProcessoAdministrativoDAO))]
    [PersistenceClass("processos_administrativos")]
    public class ProcessoAdministrativo : Sync.Fiscal.EFD.Entidade.IProcessoAdministrativo
    {
        #region Propriedades

        [PersistenceProperty("IDPROCADM", PersistenceParameterType.IdentityKey)]
        public uint IdProcAdm { get; set; }

        [PersistenceProperty("NUMEROPROCESSO")]
        public string NumeroProcesso { get; set; }

        [PersistenceProperty("NATUREZA")]
        public int Natureza { get; set; }

        [PersistenceProperty("DATADECISAO")]
        public DateTime DataDecisao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NaturezaString
        {
            get { return DataSourcesEFD.Instance.GetDescrNaturezaProcessoAdministrativo(Natureza); }
        }

        #endregion

        #region IProcessoAdministrativo Members

        Sync.Fiscal.Enumeracao.ProcessoAdministrativo.Natureza Sync.Fiscal.EFD.Entidade.IProcessoAdministrativo.Natureza
        {
            get { return (Sync.Fiscal.Enumeracao.ProcessoAdministrativo.Natureza)Natureza; }
        }

        #endregion
    }
}