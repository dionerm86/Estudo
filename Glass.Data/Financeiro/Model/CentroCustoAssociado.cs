using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    public class DetalhesCentroCustoAssociado
    {
        #region Propiedades

        public int IdCompra { get; set; }

        public int IdImpostoServ { get; set; }

        public int IdNf { get; set; }

        public int IdContaPg { get; set; }

        public int IdCte { get; set; }

        public int Identificador
        {
            get { return IdCompra > 0 ? IdCompra : IdImpostoServ > 0 ? IdImpostoServ : IdNf > 0 ? (int)NotaFiscalDAO.Instance.ObtemNumeroNf(null, (uint)IdNf) : IdContaPg; }
        }

        public string Descricao
        {
            get
            {
                return IdCompra > 0 ? "Compra" : IdImpostoServ > 0 ? "Imposto/Serviços a vulsos" : IdNf > 0 ? "Nota Fiscal" : "Conta a Pagar";
            }
        }

        public decimal ValorTotal { get; set; }

        public decimal ValorAssociacao { get; set; }

        public string DescricaoValorAssociacao
        {
            get { return "Valor total " + (IdImpostoServ > 0 || IdContaPg > 0 ? "do" : "da") + " " + Descricao; }
        }

        #endregion
    }

    [PersistenceBaseDAO(typeof(CentroCustoAssociadoDAO))]
    [PersistenceClass("centro_custo_associado")]
    public class CentroCustoAssociado
    {
        #region Propiedades

        [PersistenceProperty("IdCentroCustoAssociado", PersistenceParameterType.IdentityKey)]
        public int IdCentroCustoAssociado { get; set; }

        [PersistenceProperty("IdCentroCusto")]
        public int IdCentroCusto { get; set; }

        [PersistenceProperty("IdCompra")]
        public int? IdCompra { get; set; }

        [PersistenceProperty("IdImpostoServ")]
        public int? IdImpostoServ { get; set; }

        [PersistenceProperty("IdNf")]
        public int? IdNf { get; set; }

        [PersistenceProperty("IdContaPg")]
        public int? IdContaPg { get; set; }

        [PersistenceProperty("IdPedidoInterno")]
        public int? IdPedidoInterno { get; set; }

        [PersistenceProperty("IdConta")]
        public int IdConta { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        [PersistenceProperty("IDCTE")]
        public int? IdCte { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("DescricaoCentroCusto", DirectionParameter.InputOptional)]
        public string DescricaoCentroCusto { get; set; }

        #endregion
    }
}
