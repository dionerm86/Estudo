using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoDiariaRealizadaDAO))]
    public class ProducaoDiariaRealizada
    {
        [PersistenceProperty("IDSETOR")]
        public uint IdSetor { get; set; }

        [PersistenceProperty("TOTMPREVISTO")]
        public decimal TotMPrevisto { get; set; }

        [PersistenceProperty("TOTMREALIZADO")]
        public decimal TotMRealizado { get; set; }

        [PersistenceProperty("TOTMHOJE")]
        public decimal TotMHoje { get; set; }

        [PersistenceProperty("DataFabrica", DirectionParameter.InputOptional)]
        public DateTime DataFabrica { get; set; }

        public decimal TotMPendente
        {
            get
            {
                return TotMPrevisto - TotMRealizado;
            }
        }

        public string DataFabricaStr
        {
            get
            {
                return DataFabrica.ToShortDateString();
            }
        }
    }

    /// <summary>
    /// Classe auxiliar para carga da producao diaria por setor. 
    /// (não contém relacionamento no banco de dados)
    /// </summary>
    public class ProducaoDiariaRealizadaSetor
    {
        public string DescricaoSetor { get; set; }

        public float TotRealizado { get; set; }

        public ProducaoDiariaRealizadaSetor(string descricaoSetor)
        {
            DescricaoSetor = descricaoSetor;
        }
    }
}
