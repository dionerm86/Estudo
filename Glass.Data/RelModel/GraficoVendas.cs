using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(GraficoVendasDAO))]
    public class GraficoVendas
    {
        #region Propriedades

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("TOTALVENDA")]
        public decimal TotalVenda { get; set; }

        [PersistenceProperty("NOMELOJA")]
        public string NomeLoja { get; set; }

        private string _nomeVendedor;

        [PersistenceProperty("NOMEVENDEDOR")]
        public string NomeVendedor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeVendedor); }
            set { _nomeVendedor = value; }
        }

        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NOMECLIENTE")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DATAVENDA")]
        public string DataVenda { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion
    }
}