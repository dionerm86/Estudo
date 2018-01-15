using GDA;
using Glass.Data.Model;
using Sync.Utils.Boleto.Bancos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Glass.Data.DAL
{
    public sealed class CartaoNaoIdentificadoDAO :  BaseDAO<Model.CartaoNaoIdentificado, CartaoNaoIdentificadoDAO>
    {
        /// <summary>
        /// Recupera o elemento pelo identificador passado
        /// </summary>
        public CartaoNaoIdentificado GetElement(GDASession sessao, uint idCartaoNaoIdentificado)
        {
            return objPersistence.LoadOneData(sessao, ("Select * FROM Cartao_Nao_Identificado WHERE IdCartaoNaoIdentificado=" + idCartaoNaoIdentificado));
        }

        /// <summary>
        /// Cancela um déposito não identificado
        /// </summary>
        public void Cancelar(GDASession sessao, uint[] idsCartaoNaoIdentificado, string motivo)
        {
            foreach (var idCartaoNaoIdentificado in idsCartaoNaoIdentificado)
            {
                //Valida para o cancelamento
                if (idCartaoNaoIdentificado == 0)
                    throw new Exception("Informe o cartão não identificado a ser cancelado.");

                if (string.IsNullOrEmpty(motivo))
                    throw new Exception("Informe o motivo do cancelamento.");

                var cni = GetElement(sessao, idCartaoNaoIdentificado);

                if (cni == null)
                    throw new Exception("CNI não encontrado.");

                if (cni.Situacao != SituacaoCartaoNaoIdentificado.Ativo)
                    throw new Exception("O cartão não identificado deve estar ativo para ser cancelado.");

                // Cancela as movimentações das tabelas caixa_diario, caixa_geral e mov_banco.
                var retornoCancelamentoRecebimento = Helper.UtilsFinanceiro.CancelaRecebimento(sessao,
                    Helper.UtilsFinanceiro.TipoReceb.CartaoNaoIdentificado, null, null, null, null, null, 0, null, null, null, cni,
                    DateTime.Now);
                
                // Lança a exceção ocorrida no cancelamento do recebimento.
                if (retornoCancelamentoRecebimento.ex != null)
                    throw retornoCancelamentoRecebimento.ex;

                // Gera log cancelamento
                LogCancelamentoDAO.Instance.LogCartaoNaoIdentificado(sessao, cni, motivo, true);
            }

            objPersistence.ExecuteCommand(sessao,
                @"UPDATE cartao_nao_identificado
                SET Situacao=" + (int)SituacaoCartaoNaoIdentificado.Cancelado + @"
                WHERE IdCartaoNaoIdentificado IN (" + string.Join(", ", idsCartaoNaoIdentificado) + ")");
        }

        /// <summary>
        /// Faz o vinculo do deposito.
        /// </summary>
        public void VincularCartaoNaoIdentificado(GDASession sessao, uint idCartaoNaoIdentificado, Pedido pedido, LiberarPedido liberarPedido, Acerto acerto,
            ContasReceber contaR, Obra obra, Sinal sinal, TrocaDevolucao trocaDevolucao, uint? idDevolucaoPagto, uint? idAcertoCheque)
        {
            int? idPedido = pedido != null && pedido.IdPedido > 0 ? (int)pedido.IdPedido : (int?)null;
            int? idLiberarPedido = liberarPedido != null ? (int)liberarPedido.IdLiberarPedido : (int?)null;
            int? idAcerto = acerto != null ? (int)acerto.IdAcerto : (int?)null;
            int? idContaR = contaR != null ? (int)contaR.IdContaR : (int?)null;
            int? idObra = obra != null ? (int)obra.IdObra : (int?)null;
            int? idSinal = sinal != null ? (int)sinal.IdSinal : (int?)null;
            int? idTrocaDevolucao = trocaDevolucao != null ? (int)trocaDevolucao.IdTrocaDevolucao : (int?)null;

            if (idCartaoNaoIdentificado == 0)
                throw new Exception("Cartão não identificado não informado.");

            if (!idPedido.HasValue && !idLiberarPedido.HasValue && !idAcerto.HasValue && !idContaR.HasValue && !idObra.HasValue && !idSinal.HasValue
                && !idTrocaDevolucao.HasValue && !idDevolucaoPagto.HasValue && !idAcertoCheque.HasValue)
                throw new Exception("Nenhuma conta para vínculo informada.");

            var cni = GetElement(sessao, idCartaoNaoIdentificado);

            if (cni == null)
                throw new Exception("Cartão não identificado não encontrado.");

            cni.IdPedido = idPedido;
            cni.IdLiberarPedido = idLiberarPedido;
            cni.IdAcerto = idAcerto;
            cni.IdContaR = idContaR;
            cni.IdObra = idObra;
            cni.IdSinal = idSinal;
            cni.IdTrocaDevolucao = idTrocaDevolucao;
            cni.IdDevolucaoPagto = (int?)idDevolucaoPagto;
            cni.IdAcertoCheque = (int?)idAcertoCheque;
            cni.Situacao = !idDevolucaoPagto.HasValue ? SituacaoCartaoNaoIdentificado.EmUso : SituacaoCartaoNaoIdentificado.Ativo;

            Update(sessao, cni);

            uint? idCliente = null;

            // Atualiza o cliente do DNI
            if (idPedido > 0)
                idCliente = pedido.IdCli;
            else if (idLiberarPedido > 0)
                idCliente = liberarPedido.IdCliente;
            else if (idAcerto > 0)
                idCliente = acerto.IdCli;
            else if (idContaR > 0)
                idCliente = contaR.IdCliente;
            else if (idObra > 0)
                idCliente = obra.IdCliente;
            else if (idSinal > 0)
                idCliente = sinal.IdCliente;
            else if (idTrocaDevolucao > 0)
                idCliente = trocaDevolucao.IdCliente;

            var contas = ContasReceberDAO.Instance.RecuperarContaspeloIdCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach(var item in contas)
            {
                item.IdCliente = idCliente.GetValueOrDefault();
                item.IdPedido = (uint?)idPedido;
                item.IdLiberarPedido = (uint?)idLiberarPedido;
                item.IdAcerto = (uint?)idAcerto;
                item.IdContaRRef = idContaR;
                item.IdObra = (uint?)idObra;
                item.IdSinal = (uint?)idSinal;
                item.IdTrocaDevolucao = (uint?)idTrocaDevolucao;
                item.IdDevolucaoPagto = idDevolucaoPagto;
                item.IdAcertoCheque = idAcertoCheque;

                ContasReceberDAO.Instance.Update(sessao, item);
            }

            var cxDiario = CaixaDiarioDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in cxDiario)
            {
                item.IdCliente = idCliente.GetValueOrDefault();
                item.IdPedido = (uint?)idPedido;
                item.IdLiberarPedido = (uint?)idLiberarPedido;
                item.IdAcerto = (uint?)idAcerto;
                item.IdContaR = (uint?)idContaR;
                item.IdObra = (uint?)idObra;
                item.IdSinal = (uint?)idSinal;
                item.IdTrocaDevolucao = (uint?)idTrocaDevolucao;
                item.IdDevolucaoPagto = idDevolucaoPagto;
                item.IdAcertoCheque = idAcertoCheque;

                CaixaDiarioDAO.Instance.Update(sessao, item);
            }

            var cxGeral = CaixaGeralDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in cxGeral)
            {
                item.IdCliente = idCliente.GetValueOrDefault();
                item.IdPedido = (uint?)idPedido;
                item.IdLiberarPedido = (uint?)idLiberarPedido;
                item.IdAcerto = (uint?)idAcerto;
                item.IdContaR = (uint?)idContaR;
                item.IdObra = (uint?)idObra;
                item.IdSinal = (uint?)idSinal;
                item.IdTrocaDevolucao = (uint?)idTrocaDevolucao;
                item.IdDevolucaoPagto = idDevolucaoPagto;
                item.IdAcertoCheque = idAcertoCheque;

                CaixaGeralDAO.Instance.Update(sessao, item);
            }

            var movs = MovBancoDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach(var item in movs)
            {
                item.IdCliente = idCliente.GetValueOrDefault();
                item.IdPedido = (uint?)idPedido;
                item.IdLiberarPedido = (uint?)idLiberarPedido;
                item.IdAcerto = (uint?)idAcerto;
                item.IdContaR = (uint?)idContaR;
                item.IdObra = (uint?)idObra;
                item.IdSinal = (uint?)idSinal;
                item.IdTrocaDevolucao = (uint?)idTrocaDevolucao;
                item.IdDevolucaoPagto = idDevolucaoPagto;
                item.IdAcertoCheque = idAcertoCheque;

                MovBancoDAO.Instance.Update(sessao, item);
            }           
        }

        /// <summary>
        /// Desvincular CNI de suas referências
        /// </summary>        
        public void DesvincularCartaoNaoIdentificado(GDASession sessao, Pedido pedido, LiberarPedido liberarPedido, Acerto acerto,
            ContasReceber contaR, Obra obra, Sinal sinal, TrocaDevolucao trocaDevolucao, DevolucaoPagto devolucaoPagamento,
            uint acertoCheque)
        {
            uint? idPedido = pedido != null ? pedido.IdPedido : (uint?)null;
            uint? idLiberarPedido = liberarPedido != null ? liberarPedido.IdLiberarPedido : (uint?)null;
            uint? idAcerto = acerto != null ? acerto.IdAcerto : (uint?)null;
            uint? idContaR = contaR != null ? contaR.IdContaR : (uint?)null;
            uint? idObra = obra != null ? obra.IdObra : (uint?)null;
            uint? idSinal = sinal != null ? sinal.IdSinal : (uint?)null;
            uint? idTrocaDevolucao = trocaDevolucao != null ? trocaDevolucao.IdTrocaDevolucao : (uint?)null;
            uint? idDevolucaoPagto = devolucaoPagamento != null ? devolucaoPagamento.IdDevolucaoPagto : (uint?)null;
            uint? idAcertoCheque = acertoCheque > 0 ? acertoCheque : (uint?)null;

            if (!idPedido.HasValue && !idLiberarPedido.HasValue && !idAcerto.HasValue && !idContaR.HasValue && !idObra.HasValue && !idSinal.HasValue
               && !idTrocaDevolucao.HasValue && !idDevolucaoPagto.HasValue && !idAcertoCheque.HasValue)
                throw new Exception("Nenhuma conta para desvincular informada.");

            string cartoes = GetIdsCartaoNaoIdentificado(sessao, idPedido, idLiberarPedido, idAcerto, idContaR, idObra, idSinal, idTrocaDevolucao,
                idDevolucaoPagto, idAcertoCheque);

            if (string.IsNullOrEmpty(cartoes))
                return;

            string sql = @"UPDATE cartao_nao_identificado
                           SET idPedido=null, idLiberarPedido=null, idAcerto=null, idContaR=null, idObra=null, idSinal=null, 
                               idTrocaDevolucao=null, idDevolucaoPagto=null, idAcertoCheque=null, situacao=" + (int)SituacaoCartaoNaoIdentificado.Ativo + @"
                           WHERE idCartaoNaoIdentificado in (" + cartoes + ")";

            objPersistence.ExecuteCommand(sessao, sql);

            sql = @"UPDATE contas_receber
                           SET idPedido=null, idLiberarPedido=null, idAcerto=null, idObra=null, idSinal=null, idCliente= 0,
                               idTrocaDevolucao=null, idDevolucaoPagto=null, idAcertoCheque=null, IdContaRRef = null
                           WHERE idCartaoNaoIdentificado in (" + cartoes + ")";

            objPersistence.ExecuteCommand(sessao, sql);

            sql = @"UPDATE mov_banco
                           SET idPedido=null, idLiberarPedido=null, idAcerto=null, idContaR=null, idObra=null, idSinal=null, idCliente= null,
                               idTrocaDevolucao=null, idDevolucaoPagto=null, idAcertoCheque=null
                           WHERE idCartaoNaoIdentificado in (" + cartoes + ")";

            objPersistence.ExecuteCommand(sessao, sql);            
        }

        /// <summary>
        /// Recupera os ids dos depositos nao identificados para desvincular
        /// </summary>
        public string GetIdsCartaoNaoIdentificado(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint? idAcerto,
            uint? idContaR, uint? idObra, uint? idSinal, uint? idTrocaDevolucao, uint? idDevolucaoPagamento, uint? idAcertoCheque)
        {
            string filtro = "";

            string sql = @"SELECT group_concat(cni.idCartaoNaoIdentificado)
                           FROM cartao_nao_identificado cni";

            if (idPedido.HasValue && idPedido.Value > 0)
                filtro += " cni.idPedido=" + idPedido.Value + " OR";

            if (idLiberarPedido.HasValue && idLiberarPedido.Value > 0)
                filtro += " cni.idLiberarPedido=" + idLiberarPedido.Value + " OR";

            if (idAcerto.HasValue && idAcerto.Value > 0)
                filtro += " cni.idAcerto=" + idAcerto.Value + " OR";

            if (idContaR.HasValue && idContaR.Value > 0)
                filtro += " cni.idContaR=" + idContaR.Value + " OR";

            if (idObra.HasValue && idObra.Value > 0)
                filtro += " cni.idObra=" + idObra.Value + " OR";

            if (idSinal.HasValue && idSinal.Value > 0)
                filtro += " cni.idSinal=" + idSinal.Value + " OR";

            if (idTrocaDevolucao.HasValue && idTrocaDevolucao.Value > 0)
                filtro += " cni.idTrocaDevolucao=" + idTrocaDevolucao.Value + " OR";

            if (idDevolucaoPagamento.HasValue && idDevolucaoPagamento.Value > 0)
                filtro += " cni.idDevolucaoPagto=" + idDevolucaoPagamento.Value + " OR";

            if (idAcertoCheque.HasValue && idAcertoCheque.Value > 0)
                filtro += " cni.idAcertoCheque=" + idAcertoCheque.Value + " OR";

            if (string.IsNullOrEmpty(filtro))
                throw new Exception("Nenhuma conta para desvincular informada.");

            sql += " WHERE " + filtro.TrimEnd('R').TrimEnd('O');

            var retorno = ExecuteScalar<string>(sessao, sql);

            return retorno;
        }

        /// <summary>
        /// Recupera os depósitos não identificados que ainda não foram utilizados.
        /// </summary>
        public CartaoNaoIdentificado[] GetNaoUtilizados()
        {
            string sql = @"select cni.*, c.nome as DescrContaBanco
                from cartao_nao_identificado cni
                    inner join conta_banco c on (cni.idContaBanco=c.idContaBanco)
                where cni.situacao=" + (int)SituacaoCartaoNaoIdentificado.Ativo;

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Recupera o tipo de cartão do deposito não identificado
        /// </summary>
        public CartaoNaoIdentificado[] ObterPeloId(uint[] idsCNI)
        {
            return objPersistence.LoadData("SELECT * FROM cartao_nao_identificado WHERE IdCartaoNaoIdentificado IN (" + string.Join(", ", idsCNI) + ")").ToArray();
        }

        /// <summary>
        /// Recupera o valor do cartão não identificado
        /// </summary>
        public decimal GetValorCartaoNaoIdentificado(uint idCartaoNaoIdentificado)
        {
            string sql = "select Valor from cartao_nao_identificado where idCartaoNaoIdentificado=" + idCartaoNaoIdentificado;
            return decimal.Parse(objPersistence.ExecuteScalar(sql).ToString());
        }

        /// <summary>
        /// Método utilizado apenas para ajustes do ExecScript
        /// </summary>
        public void AtualizarContasCNI(GDASession sessao, uint idCNI)
        {
            var cni = GetElement(sessao, idCNI);

            uint idCliente = 0;

            if (cni.IdPedido.GetValueOrDefault() > 0)
                idCliente = PedidoDAO.Instance.GetIdCliente((uint)cni.IdPedido);
            else if (cni.IdLiberarPedido.GetValueOrDefault() > 0)
                idCliente = LiberarPedidoDAO.Instance.GetIdCliente((uint)cni.IdLiberarPedido);
            else if (cni.IdAcerto.GetValueOrDefault() > 0)
                idCliente = AcertoDAO.Instance.GetAcertoDetails((uint)cni.IdAcerto).IdCli;
            else if (cni.IdContaR.GetValueOrDefault() > 0)
                idCliente = ContasReceberDAO.Instance.GetByIdContaR((uint)cni.IdContaR).IdCliente;
            else if (cni.IdObra.GetValueOrDefault() > 0)
                idCliente = (uint)ObraDAO.Instance.ObtemIdCliente(sessao, (int)cni.IdObra);
            else if (cni.IdSinal.GetValueOrDefault() > 0)
                idCliente = SinalDAO.Instance.ObtemIdCliente((uint)cni.IdSinal);
            else if (cni.IdTrocaDevolucao.GetValueOrDefault() > 0)
                idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente((uint)cni.IdTrocaDevolucao);
            else if (cni.IdDevolucaoPagto.GetValueOrDefault() > 0)
                idCliente = DevolucaoPagtoDAO.Instance.GetElement((uint)cni.IdDevolucaoPagto).IdCliente;
            else if (cni.IdAcertoCheque.GetValueOrDefault() > 0)
                idCliente = (uint)AcertoChequeDAO.Instance.GetElement((uint)cni.IdAcertoCheque).IdCliente;

            var contas = ContasReceberDAO.Instance.RecuperarContaspeloIdCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in contas)
            {
                item.IdCliente = idCliente;
                item.IdPedido = (uint?)cni.IdPedido;
                item.IdLiberarPedido = (uint?)cni.IdLiberarPedido;
                item.IdAcerto = (uint?)cni.IdAcerto;
                item.IdContaRRef = cni.IdContaR;
                item.IdObra = (uint?)cni.IdObra;
                item.IdSinal = (uint?)cni.IdSinal;
                item.IdTrocaDevolucao = (uint?)cni.IdTrocaDevolucao;
                item.IdDevolucaoPagto = (uint?)cni.IdDevolucaoPagto;
                item.IdAcertoCheque = (uint?)cni.IdAcertoCheque;
                ContasReceberDAO.Instance.Update(sessao, item);
            }

            var cxDiario = CaixaDiarioDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in cxDiario)
            {
                item.IdCliente = idCliente;
                item.IdPedido = (uint?)cni.IdPedido;
                item.IdLiberarPedido = (uint?)cni.IdLiberarPedido;
                item.IdAcerto = (uint?)cni.IdAcerto;
                item.IdContaR = (uint?)cni.IdContaR;
                item.IdObra = (uint?)cni.IdObra;
                item.IdSinal = (uint?)cni.IdSinal;
                item.IdTrocaDevolucao = (uint?)cni.IdTrocaDevolucao;
                item.IdDevolucaoPagto = (uint)cni.IdDevolucaoPagto;
                item.IdAcertoCheque = (uint)cni.IdAcertoCheque;

                CaixaDiarioDAO.Instance.Update(sessao, item);
            }

            var cxGeral = CaixaGeralDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in cxGeral)
            {
                item.IdCliente = idCliente;
                item.IdPedido = (uint?)cni.IdPedido;
                item.IdLiberarPedido = (uint?)cni.IdLiberarPedido;
                item.IdAcerto = (uint?)cni.IdAcerto;
                item.IdContaR = (uint?)cni.IdContaR;
                item.IdObra = (uint?)cni.IdObra;
                item.IdSinal = (uint?)cni.IdSinal;
                item.IdTrocaDevolucao = (uint?)cni.IdTrocaDevolucao;
                item.IdDevolucaoPagto = (uint)cni.IdDevolucaoPagto;
                item.IdAcertoCheque = (uint?)cni.IdAcertoCheque;

                CaixaGeralDAO.Instance.Update(sessao, item);
            }

            var movs = MovBancoDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cni.IdCartaoNaoIdentificado);

            foreach (var item in movs)
            {
                item.IdCliente = idCliente;
                item.IdPedido = (uint?)cni.IdPedido;
                item.IdLiberarPedido = (uint?)cni.IdLiberarPedido;
                item.IdAcerto = (uint?)cni.IdAcerto;
                item.IdContaR = (uint?)cni.IdContaR;
                item.IdObra = (uint?)cni.IdObra;
                item.IdSinal = (uint?)cni.IdSinal;
                item.IdTrocaDevolucao = (uint?)cni.IdTrocaDevolucao;
                item.IdDevolucaoPagto = (uint?)cni.IdDevolucaoPagto;
                item.IdAcertoCheque = (uint?)cni.IdAcertoCheque;

                MovBancoDAO.Instance.Update(sessao, item);
            }
        }
    }
}
