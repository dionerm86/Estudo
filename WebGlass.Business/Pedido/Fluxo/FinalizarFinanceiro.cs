using System;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class FinalizarFinanceiro : BaseFluxo<FinalizarFinanceiro>
    {
        private FinalizarFinanceiro() { }

        public Entidade.PedidoFinalizarFinanceiro[] ObtemItensFinalizarFinanceiro(uint idPedido, string codCliente, 
            uint idCliente, string nomeCliente, uint idOrcamento, string endereco, string bairro, string dataPedidoIni, 
            string dataPedidoFim, uint idLoja, int situacao, float alturaProd, int larguraProd, string sortExpression, 
            int startRow, int pageSize)
        {
            var itens = PedidoDAO.Instance.ObtemItensFinalizarFinanceiro(idPedido, codCliente, idCliente, nomeCliente,
                idOrcamento, endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, 
                sortExpression, startRow, pageSize).ToArray();

            return Array.ConvertAll(itens, x => new Entidade.PedidoFinalizarFinanceiro(x));
        }

        public int ObtemNumeroItensFinalizarFinanceiro(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim, uint idLoja,
            int situacao, float alturaProd, int larguraProd)
        {
            return PedidoDAO.Instance.ObtemNumeroItensFinalizarFinanceiro(idPedido, codCliente, idCliente, nomeCliente,
                idOrcamento, endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd);
        }

        public bool PodeExecutarAcao(uint idPedido, string tipo)
        {
            var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(null, idPedido);

            switch (tipo.ToLower())
            {
                case "finalizar":
                    return situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro;
                   
                case "confirmar":
                    return situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro;

                default:
                    return false;
            }
        }

        public void ExecutarAcao(uint idPedido, string tipo, string observacao, bool negado)
        {
            if (!PodeExecutarAcao(idPedido, tipo))
                throw new Exception("Não é possível executar essa ação.");

            switch (tipo)
            {
                case "finalizar":
                    FinalizarPedido(idPedido, observacao, negado);
                    break;

                case "confirmar":
                    ConfirmarPedido(idPedido, observacao, negado);
                    break;

                default:
                    throw new Exception("Ação inválida.");
            }
        }

        public void FinalizarPedido(uint idPedido, string observacao, bool negado)
        {
            if (!negado)
            {
                bool emConferencia = false;
                PedidoDAO.Instance.FinalizarPedidoComTransacao(idPedido, ref emConferencia, true);
            }
            else
            {
                PedidoDAO.Instance.AlteraSituacao(idPedido, Glass.Data.Model.Pedido.SituacaoPedido.Ativo);
            }

            ObservacaoFinalizacaoFinanceiroDAO.Instance.AtualizaItem(null, idPedido, observacao, !negado ?
                Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao :
                Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoFinalizar);
        }

        public void ConfirmarPedido(uint idPedido, string observacao, bool negado)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (!negado && Glass.Configuracoes.PedidoConfig.LiberarPedido)
                    {
                        bool enviarMensagem = false;
                        //Recalcula a data de entrega do pedido baseando-se na data de hoje e atualiza a data de entrega do pedido
                        PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(transaction, idPedido, DateTime.Now, out enviarMensagem);

                        var idRemetente = UserInfo.GetUserInfo.CodUser;
                        var idVendedorCad = (int)PedidoDAO.Instance.ObtemIdFuncCad(transaction, idPedido);
                        var dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(null, idPedido);

                        if (enviarMensagem)
                        {
                            //Envia uma mensagem para o vendedor informando que a data de entrega foi alterada
                            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                                .GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                                .EnviarMensagemVendedorAoAlterarDataEntrega((int)idRemetente, idVendedorCad, (int)idPedido, dataEntrega);
                        }

                        string idsOk, idsErro;
                        PedidoDAO.Instance.ConfirmarLiberacaoPedido(transaction, idPedido.ToString(), out idsOk, out idsErro, true, false);

                        if (PedidoDAO.Instance.IsGeradoParceiro(transaction, idPedido) &&
                            Glass.Configuracoes.ProjetoConfig.TelaCadastroParceiros.ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido)
                        {
                            var idProjeto = PedidoDAO.Instance.ObtemIdProjeto(idPedido);
                            if (ProjetoDAO.Instance.GetTipoVenda(transaction, idProjeto.GetValueOrDefault(0)) == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda)
                            {
                                // Gera o espelho do pedido.
                                PedidoEspelhoDAO.Instance.GeraEspelho(transaction, idPedido);

                                // Deixa a conferência do pedido finalizada.
                                PedidoEspelhoDAO.Instance.FinalizarPedido(transaction, idPedido);
                            }
                        }
                    }
                    else
                    {
                        PedidoDAO.Instance.AlteraSituacao(transaction, idPedido, Glass.Data.Model.Pedido.SituacaoPedido.Conferido);
                    }

                    ObservacaoFinalizacaoFinanceiroDAO.Instance.AtualizaItem(transaction, idPedido, observacao, !negado ?
                        Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao :
                        Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoConfirmar);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }
    }
}
