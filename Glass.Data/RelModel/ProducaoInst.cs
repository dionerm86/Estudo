using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoInstDAO))]
    public class ProducaoInst
    {
        #region Propriedades

        [PersistenceProperty("PRODUTOS", DirectionParameter.InputOptional)]
        public string Produtos { get; set; }

        [PersistenceProperty("ALTURALARGURA", DirectionParameter.InputOptional)]
        public string AlturaLargura { get; set; }

        [PersistenceProperty("NOMEEQUIPE")]
        public string NomeEquipe { get; set; }

        [PersistenceProperty("TIPOEQUIPE")]
        public string TipoEquipe { get; set; }

        [PersistenceProperty("QTDEPECAS")]
        public decimal QtdePecas { get; set; }

        [PersistenceProperty("TOTALM2")]
        public double TotalM2 { get; set; }

        [PersistenceProperty("QTDEGARANTIA")]
        public decimal QtdeGarantia { get; set; }

        public string Criterio { get; set; }

        #endregion
    }
}