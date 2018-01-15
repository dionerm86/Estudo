using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PosicaoMateriaPrimaChapaDAO))]
    [PersistenceClass("posicao_materia_prima_chapa")]
    public class PosicaoMateriaPrimaChapa
    {
        #region Propiedades

        [PersistenceProperty("IdProdBase")]
        public int IdProdBase { get; set; }

        [PersistenceProperty("IdCorVidro")]
        public int IdCorVidro { get; set; }

        [PersistenceProperty("Espessura")]
        public float Espessura { get; set; }

        [PersistenceProperty("CodInterno")]
        public string CodInterno { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("NomeFornecedor")]
        public string NomeFornecedor { get; set; }

        [PersistenceProperty("Altura")]
        public int Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("TotalM2Chapa")]
        public decimal TotalM2Chapa { get; set; }

        [PersistenceProperty("QtdeChapa")]
        public double QtdeChapa { get; set; }

        #endregion
    }
}
