using System;
using Glass.Data.Model;

namespace Glass.Data.Exceptions
{
    public class ValidacaoPedidoFinanceiroException : Exception
    {
        public string IdsPedidos { get; private set; }
        public uint IdPedido { get; private set; }
        public ObservacaoFinalizacaoFinanceiro.MotivoEnum Motivo { get; private set; }

        public ValidacaoPedidoFinanceiroException(string message, uint idPedido, string idsPedidos,
            ObservacaoFinalizacaoFinanceiro.MotivoEnum motivo)
            : base(message)
        {
            IdPedido = idPedido;
            IdsPedidos = idsPedidos;
            Motivo = motivo;
        }
    }
}
