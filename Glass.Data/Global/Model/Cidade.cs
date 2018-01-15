using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CidadeDAO))]
	[PersistenceClass("cidade")]
	public class Cidade : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.ICidade
    {
        #region Propriedades

        [PersistenceProperty("IDCIDADE", PersistenceParameterType.IdentityKey)]
        public int IdCidade { get; set; }

		private string _nomeCidade;

		[PersistenceProperty("NOMECIDADE")]
		public string NomeCidade
		{
			get { return _nomeCidade != null ? _nomeCidade.ToUpper() : String.Empty; }
			set { _nomeCidade = value; }
		}

        [PersistenceProperty("CODIBGECIDADE")]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("NOMEUF")]
        public string NomeUf { get; set; }

        [PersistenceProperty("CODIBGEUF")]
        public string CodIbgeUf { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NomeCidadeString
        {
            get { return NomeCidade.Replace("'", ""); }
        }

        public string CodUfMunicipio
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        public string NomeCidadeUf
        {
            get { return _nomeCidade + "/" + NomeUf; }
        }

        #endregion

        #region ICidade Members

        int Sync.Fiscal.EFD.Entidade.ICidade.CodigoCidade
        {
            get { return IdCidade; }
        }

        string Sync.Fiscal.EFD.Entidade.ICidade.Nome
        {
            get { return NomeCidade; }
        }

        string Sync.Fiscal.EFD.Entidade.ICidade.UF
        {
            get { return NomeUf; }
        }

        string Sync.Fiscal.EFD.Entidade.ICidade.CodigoIbgeMunicipio
        {
            get { return CodUfMunicipio; }
        }

        #endregion
    }
}