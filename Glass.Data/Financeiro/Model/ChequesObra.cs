using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ChequesObraDAO))]
    [PersistenceClass("cheques_obra")]
    public class ChequesObra : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCHEQUEOBRA", PersistenceParameterType.IdentityKey)]
        public int IDChequeObra { get; set; }

        [PersistenceProperty("IDOBRA")]
        public int IdObra { get; set; }

        [PersistenceProperty("IDLOJA")]
        public int IdLoja { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public int? IdContaBanco { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [PersistenceProperty("NUM")]
        public int Num { get; set; }

        [PersistenceProperty("DIGITONUM")]
        public string DigitoNum { get; set; }

        [PersistenceProperty("BANCO")]
        public string Banco { get; set; }

        [PersistenceProperty("AGENCIA")]
        public string Agencia { get; set; }

        [PersistenceProperty("CONTA")]
        public string Conta { get; set; }

        [PersistenceProperty("TITULAR")]
        public string Titular { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATAVENC")]
        public DateTime DataVenc { get; set; }

        [PersistenceProperty("ORIGEM")]
        public int Origem { get; set; }

        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion
    }
}