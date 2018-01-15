using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ChartVendasDAO))]
    public class ChartVendas
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

        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NOMECLIENTE")]
        public string NomeCliente { get; set; }

        private string _nomeVendedor;

        [PersistenceProperty("NOMEVENDEDOR")]
        public string NomeVendedor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeVendedor); }
            set { _nomeVendedor = value; }
        }

        [PersistenceProperty("TIPOPEDIDO")]
        public int TipoPedido { get; set; }

        [PersistenceProperty("PERIODO", DirectionParameter.InputOptional)]
        public string Periodo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public int Agrupar { get; set; }

        public string NomeRpt
        {
            get
            {
                switch (Agrupar)
                {
                    case 0:
                        return "Empresa";
                    case 1:
                        return NomeLoja;
                    case 2:
                        return NomeVendedor;
                    case 4:
                        return TipoPedido == 1 ? "Venda" : TipoPedido == 2 ? "Revenda" : TipoPedido == 3 ? "Mão de obra" : "Produção";
                    default:
                        return "Venda";
                }
            }
        }

        #endregion
    }

    public class ChartVendasImagem
    {
        public byte[] Buffer { get; set; }
    }
}