using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(NFeCidadeDescargaMDFeDAO))]
    [PersistenceClass("nfe_cidade_descarga_mdfe")]
    public class NFeCidadeDescargaMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDCIDADEDESCARGA", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(CidadeDescargaMDFe), "IdCidadeDescarga")]
        public int IdCidadeDescarga { get; set; }

        [PersistenceProperty("IDNFE", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(NotaFiscal), "IdNf")]
        public int IdNFe { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public int NumeroNFe { get; set; }

        [PersistenceProperty("MODELO", DirectionParameter.InputOptional)]
        public string Modelo { get; set; }

        [PersistenceProperty("TIPODOCUMENTO", DirectionParameter.InputOptional)]
        public int TipoDocumento { get; set; }

        [PersistenceProperty("NOMEEMITENTE", DirectionParameter.InputOptional)]
        public string NomeEmitente { get; set; }

        [PersistenceProperty("DATAEMISSAO", DirectionParameter.InputOptional)]
        public DateTime DataEmissao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string TipoDocumentoString
        {
            get { return GetTipoDocumento(TipoDocumento); }
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
