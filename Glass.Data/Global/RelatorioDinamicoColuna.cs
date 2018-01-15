using GDA;

namespace Glass.Data.Model
{

    [PersistenceClass("relatorio_dinamico_coluna")]
    public class RelatorioDinamicoColuna : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdRelatorioDinamicoColuna", PersistenceParameterType.IdentityKey)]
        public int IdRelatorioDinamicoColuna { get; set; }

        [PersistenceForeignKey(typeof(RelatorioDinamico), "IdRelatorioDinamico")]
        [PersistenceProperty("IdRelatorioDinamico")]
        public int IdRelatorioDinamico { get; set; }

        [PersistenceProperty("NomeColuna")]
        public string NomeColuna { get; set; }

        [PersistenceProperty("Alias")]
        public string Alias { get; set; }

        [PersistenceProperty("Valor")]
        public string Valor { get; set; }

        [PersistenceProperty("MetodoVisibilidade")]
        public string MetodoVisibilidade { get; set; }

        [PersistenceProperty("NumSeq")]
        public int NumSeq { get; set; }
    }
}
