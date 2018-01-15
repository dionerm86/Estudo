using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ChartDREDAO))]
    public class ChartDRE
    {
        #region Propriedades

        public uint IdLoja { get; set; }

        public string NomeLoja { get; set; }

        public decimal Total { get; set; }

        public string Periodo { get; set; }

        #endregion
    }

    public class ChartDREImagem
    {
        public byte[] Buffer { get; set; }
    }
}