using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("relatorio_dinamico_icone")]
    public class RelatorioDinamicoIcone : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDRELATORIODINAMICOICONE", PersistenceParameterType.IdentityKey)]
        public int IdRelatorioDinamicoIcone { get; set; }

        [PersistenceForeignKey(typeof(RelatorioDinamico), "IdRelatorioDinamico")]
        [PersistenceProperty("IDRELATORIODINAMICO")]
        public int IdRelatorioDinamico { get; set; }

        [PersistenceProperty("NOMEICONE")]
        public string NomeIcone { get; set; }

        [PersistenceProperty("FUNCAOJAVASCRIPT")]
        public string FuncaoJavaScript { get; set; }

        [PersistenceProperty("ICONE")]
        public byte[] Icone { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("MetodoVisibilidade")]
        public string MetodoVisibilidade { get; set; }

        [PersistenceProperty("MostrarFinalGrid")]
        public bool MostrarFinalGrid { get; set; }
    }
}
