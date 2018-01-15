using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ChapasDisponiveisDAO))]
    [PersistenceClass("produto_impressao")]
    public class ChapasDisponiveis
    {
        [PersistenceProperty("Cor")]
        public string Cor { get; set; }

        [PersistenceProperty("Espessura")]
        public int Espessura { get; set; }

        [PersistenceProperty("Produto")]
        public string Produto { get; set; }

        [PersistenceProperty("Fornecedor")]
        public string Fornecedor { get; set; }

        [PersistenceProperty("NumeroNfe")]
        public int NumeroNfe { get; set; }

        [PersistenceProperty("Lote")]
        public string Lote { get; set; }

        [PersistenceProperty("Etiqueta")]
        public string Etiqueta { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }
    }
}
