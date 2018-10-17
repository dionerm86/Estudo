using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CTeCidadeDescargaMDFeDAO))]
    [PersistenceClass("cte_cidade_descarga_mdfe")]
    public class CTeCidadeDescargaMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDCIDADEDESCARGAMDFE", PersistenceParameterType.IdentityKey)]
        public uint IdCidadeDescargaMdfe { get; set; }

        [PersistenceProperty("IDCIDADEDESCARGA")]
        [PersistenceForeignKey(typeof(CidadeDescargaMDFe), "IdCidadeDescarga")]
        public int IdCidadeDescarga { get; set; }

        [PersistenceProperty("IDCTE")]
        [PersistenceForeignKey(typeof(Cte.ConhecimentoTransporte), "IdCte")]
        public int? IdCTe { get; set; }

        [PersistenceProperty("CHAVEACESSO")]
        public string ChaveAcesso { get; set; }

        [PersistenceProperty("NUMDOCFSDA")]
        public long? NumeroDocumentoFsda { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NUMEROCTE", DirectionParameter.InputOptional)]
        public int? NumeroCTe { get; set; }

        [PersistenceProperty("MODELO", DirectionParameter.InputOptional)]
        public string Modelo { get; set; }

        [PersistenceProperty("TIPODOCUMENTOCTE", DirectionParameter.InputOptional)]
        public int TipoDocumentoCTe { get; set; }

        [PersistenceProperty("NOMEEMITENTE", DirectionParameter.InputOptional)]
        public string NomeEmitente { get; set; }

        [PersistenceProperty("DATAEMISSAO", DirectionParameter.InputOptional)]
        public DateTime? DataEmissao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string TipoDocumentoString
        {
            get { return GetTipoDocumento(TipoDocumentoCTe); }
        }

        #endregion

        #region Métodos internos estáticos

        internal static string GetTipoDocumento(int tipo)
        {
            switch (tipo)
            {
                case 1: return "Entrada";
                case 2: return "Saída";
                case 3: return "Entrada (terceiros)";
                case 4: return "Nota Fiscal de Cliente";
                default: return "";
            }
        }

        #endregion
    }
}
