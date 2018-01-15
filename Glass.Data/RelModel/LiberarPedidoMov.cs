using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LiberarPedidoMovDAO))]
    public class LiberarPedidoMov
    {
        #region Propriedades

        public uint IdLiberarPedido { get; set; }

        public string IdsPedidos { get; set; }

        public string NomeCliente { get; set; }

        public decimal Total { get; set; }

        public string Situacao { get; set; }

        public decimal Dinheiro { get; set; }

        public decimal Cheque { get; set; }

        public decimal Prazo { get; set; }

        public decimal Boleto { get; set; }

        public decimal Deposito { get; set; }

        public decimal Cartao { get; set; }

        public decimal Outros { get; set; }

        public decimal Debito { get; set; }

        public decimal Credito { get; set; }

        public decimal Desconto { get; set; }

        public decimal Acrescimo { get; set; }

        public bool CanceladoSistema { get; set; }

        public string Criterio { get; set; }

        #endregion
    }
}