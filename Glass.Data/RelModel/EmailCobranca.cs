using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(EmailCobrancaDAO))]
    [PersistenceClass("EmailCobranca")]
    public class EmailCobranca
    {
        [PersistenceProperty("IdCliente")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NomeCli")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("EmailCobranca")]
        public string EmailCliente { get; set; }

        [PersistenceProperty("NumContasVec")]
        public decimal NumContasVec { get; set; }

        [PersistenceProperty("NumContasVecHoje")]
        public decimal NumContasVecHoje { get; set; }

        [PersistenceProperty("NumContasAVec")]
        public decimal NumContasAVec { get; set; }

        [PersistenceProperty("ValorContasVec")]
        public decimal ValorContasVec { get; set; }

        [PersistenceProperty("ValorContasVecHoje")]
        public decimal ValorContasVecHoje { get; set; }

        [PersistenceProperty("ValorContasAVec")]
        public decimal ValorContasAVec { get; set; }
    }
}
