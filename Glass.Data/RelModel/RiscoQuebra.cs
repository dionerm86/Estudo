using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(RiscoQuebraDAO))]
    public class RiscoQuebra
    {
        #region Propriedades

        public uint IdPedido { get; set; }

        public string NomeLoja { get; set; }

        public string CidadeData { get; set; }

        public string Cliente { get; set; }

        public string Endereco { get; set; }

        public string Telefone { get; set; }

        public string Texto { get; set; }

        #endregion
    }
}