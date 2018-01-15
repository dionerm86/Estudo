using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(VeiculoDAO))]
	[PersistenceClass("veiculo")]
	public class Veiculo : ModelBaseCadastro
    {
        #region Propriedades

        private string _placa;

		[PersistenceProperty("Placa", PersistenceParameterType.Key)]
		public string Placa
		{
			get { return _placa != null ? _placa.ToUpper() : _placa; }
			set { _placa = value; }
		}

        [PersistenceProperty("Modelo")]
        public string Modelo { get; set; }

        [PersistenceProperty("AnoFab")]
        public int Anofab { get; set; }

        [PersistenceProperty("Cor")]
        public string Cor { get; set; }

        [PersistenceProperty("KmInicial")]
        public int Kminicial { get; set; }

        [PersistenceProperty("ValorIpva")]
        public float Valoripva { get; set; }

        [PersistenceProperty("CHASSI")]
        public string Chassi { get; set; }

        [PersistenceProperty("CODCORMONTADORA")]
        public string CodCorMontadora { get; set; }

        [PersistenceProperty("CODMODELORENAVAM")]
        public string CodModeloRenavam { get; set; }

        [PersistenceProperty("VALORUNITARIO")]
        public decimal ValorUnitario { get; set; }

        [PersistenceProperty("TARA")]
        public float Tara { get; set; }

        [PersistenceProperty("CAPACIDADEKG")]
        public float CapacidadeKg { get; set; }

        [PersistenceProperty("CAPACIDADEM3")]
        public float CapacidadeM3 { get; set; }

        [PersistenceProperty("TIPOPROPRIETARIO")]
        public int TipoProprietario { get; set; }

        [PersistenceProperty("TIPOVEICULO")]
        public int TipoVeiculo { get; set; }

        [PersistenceProperty("TIPORODADO")]
        public int TipoRodado { get; set; }

        [PersistenceProperty("TIPOCARROCERIA")]
        public int TipoCarroceria { get; set; }

        [PersistenceProperty("UFLICENC")]
        public string UfLicenc { get; set; }

        [PersistenceProperty("RENAVAM")]
        public string Renavam { get; set; }

        /// <summary>
        /// Ativo 1
        /// Inativo 2
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoCompleta
        {
            get 
            {
                return _placa + " " + Modelo + " " + Cor + " " + Anofab; 
            }
        }

        #endregion
    }
}