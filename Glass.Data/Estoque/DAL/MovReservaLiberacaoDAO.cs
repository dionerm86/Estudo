
using GDA;
using Glass.Data.Model;
using System;

namespace Glass.Data.DAL
{
    public sealed class MovReservaLiberacaoDAO : BaseDAO<MovReservaLiberacao, MovReservaLiberacaoDAO>
    {
        /// <summary>
        /// Movimenta a reserva do produto passado (apenas para log)
        /// </summary>
        public void MovimentaReserva(GDASession sessao, int idProd, int idLoja, int tipoMov, decimal qtdReserva, int? idSaidaEstoque,
            int? idLiberarPedido, int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            // Pega o valor atual da reserva
            var reservaAtual = ProdutoLojaDAO.Instance.ObtemValorCampo<decimal>(sessao, "Reserva", string.Format("idLoja={0} And idProd={1}", idLoja, idProd));

            MovReservaLiberacao mov = new MovReservaLiberacao();
            mov.IdProd = (uint)idProd;
            mov.IdFunc = Helper.UserInfo.GetUserInfo.CodUser;
            mov.IdLoja = (uint)idLoja;
            mov.DataMov = DateTime.Now;
            mov.QtdeReserva = qtdReserva;
            mov.TipoMov = tipoMov;
            mov.SaldoReserva = tipoMov == 1 ? reservaAtual + qtdReserva : reservaAtual - qtdReserva;

            // Detalhes sobre a movimentação.
            mov.IdSaidaEstoque = idSaidaEstoque;
            mov.IdLiberarPedido = idLiberarPedido;
            mov.IdPedidoEspelho = idPedidoEspelho;
            mov.IdProdPedProducao = idProdPedProducao;
            mov.IdPedido = idPedido;
            mov.IdsPedido = idsPedido;
            mov.IdProdPed = idProdPed;
            mov.ClasseMetodo = classeMetodo;

            Insert(sessao, mov);
        }

        /// <summary>
        /// Movimenta a liberação do produto passado (apenas para log)
        /// </summary>
        public void MovimentaLiberacao(GDASession sessao, int idProd, int idLoja, int tipoMov, decimal qtdLiberacao, int? idSaidaEstoque,
            int? idLiberarPedido, int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            // Pega o valor atual da liberação
            var liberacaoAtual = ProdutoLojaDAO.Instance.ObtemValorCampo<decimal>(sessao, "Liberacao", string.Format("idLoja={0} And idProd={1}", idLoja, idProd));

            MovReservaLiberacao mov = new MovReservaLiberacao();
            mov.IdProd = (uint)idProd;
            mov.IdFunc = Helper.UserInfo.GetUserInfo.CodUser;
            mov.IdLoja = (uint)idLoja;
            mov.DataMov = DateTime.Now;
            mov.QtdeLiberacao = qtdLiberacao;
            mov.TipoMov = tipoMov;
            mov.SaldoLiberacao = tipoMov == 1 ? liberacaoAtual + qtdLiberacao : liberacaoAtual - qtdLiberacao;

            // Detalhes sobre a movimentação.
            mov.IdSaidaEstoque = idSaidaEstoque;
            mov.IdLiberarPedido = idLiberarPedido;
            mov.IdPedidoEspelho = idPedidoEspelho;
            mov.IdProdPedProducao = idProdPedProducao;
            mov.IdPedido = idPedido;
            mov.IdsPedido = idsPedido;
            mov.IdProdPed = idProdPed;
            mov.ClasseMetodo = classeMetodo;

            Insert(sessao, mov);
        }
    }
}
