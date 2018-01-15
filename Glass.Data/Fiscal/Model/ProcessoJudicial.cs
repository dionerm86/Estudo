using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProcessoJudicialDAO))]
    [PersistenceClass("processos_judiciais")]  
    public class ProcessoJudicial : Sync.Fiscal.EFD.Entidade.IProcessoJudicial
    {
        #region Propriedades

        [PersistenceProperty("IDPROCJUD", PersistenceParameterType.IdentityKey)]
        public uint IdProcJud { get; set; }

        [PersistenceProperty("NUMEROPROCESSO")]
        public string NumeroProcesso { get; set; }

        [PersistenceProperty("SECAOJUDICIARIA")]
        public string SecaoJudiciaria { get; set; }

        [PersistenceProperty("VARA")]
        public string Vara { get; set; }

        [PersistenceProperty("NATUREZA")]
        public int Natureza { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATADECISAO")]
        public DateTime DataDecisao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NaturezaString
        {
            get { return DataSourcesEFD.Instance.GetDescrNaturezaProcessoJudicial(Natureza); }
        }

        #endregion

        #region IProcessoJudicial Members

        Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza Sync.Fiscal.EFD.Entidade.IProcessoJudicial.Natureza
        {
            get { return (Sync.Fiscal.Enumeracao.ProcessoJudicial.Natureza)Natureza; }
        }

        #endregion
    }
}