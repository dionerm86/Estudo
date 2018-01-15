using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(VendasMesesDAO))]
    public class VendasMeses
    {
        #region Propriedades

        public uint IdCliente { get; set; }

        public uint IdComissionado { get; set; }

        public string Mes { get; set; }

        public decimal Valor { get; set; }

        public string MediaCompraCliente { get; set; }

        public string IdNome { get; set; }

        public uint IdFuncionario { get; set; }

        public string NomeFuncionario { get; set; }

        public string DoisPrimeirosNomesVendedor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(NomeFuncionario); }
        }

        public uint IdClienteComissionado { get; set; }

        public string Nome { get; set; }

        public decimal Total { get; set; }

        public double TotM2 { get; set; }

        public int TotalItens { get; set; }

        public string Format { get; set; }

        #endregion
    }
}