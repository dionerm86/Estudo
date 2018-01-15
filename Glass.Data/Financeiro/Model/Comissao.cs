using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ComissaoDAO))]
	[PersistenceClass("comissao")]
	public class Comissao
    {
        #region Propriedades

        [PersistenceProperty("IDCOMISSAO", PersistenceParameterType.IdentityKey)]
        public uint IdComissao { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [PersistenceProperty("IDCOMISSIONADO")]
        public uint? IdComissionado { get; set; }

        [PersistenceProperty("IDINSTALADOR")]
        public uint? IdInstalador { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("DATAREFINI")]
        public DateTime DataRefIni { get; set; }

        [PersistenceProperty("DATAREFFIM")]
        public DateTime DataRefFim { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("DataRecIni")]
        public DateTime? DataRecIni { get; set; }

        [PersistenceProperty("DataRecFim")]
        public DateTime? DataRecFim { get; set; }

        [PersistenceProperty("IDGERENTE")]
        public uint? IdGerente { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("NOMEFUNCIONARIO", DirectionParameter.InputOptional)]
        public string NomeFuncionario { get; set; }

        [PersistenceProperty("NOMECOMISSIONADO", DirectionParameter.InputOptional)]
        public string NomeComissionado { get; set; }

        [PersistenceProperty("NOMEINSTALADOR", DirectionParameter.InputOptional)]
        public string NomeInstalador { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrComissao
        {
            get { return DataCad.ToString("dd/MM/yyyy") + " - " + Total.ToString("C"); }
        }

        public string Nome
        {
            get { return IdFunc > 0 ? NomeFuncionario : IdComissionado > 0 ? NomeComissionado : NomeInstalador; }
        }

        #endregion
    }
}