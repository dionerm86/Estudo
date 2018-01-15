using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ContaBancoDAO : BaseCadastroDAO<ContaBanco, ContaBancoDAO>
	{
        //private ContaBancoDAO() { }

        #region Movimenta conta bancária
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaPedido(uint idContaBanco, uint idConta, int idLoja, uint idPedido, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaPedido(null, idContaBanco, idConta, idLoja, idPedido, idCliente, tipoMov,
                valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaPedido(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idPedido, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, idPedido, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Associa um cartão não identificado ao caixa geral
        /// </summary>
        public void AssociarContaBancoIdCartaoNaoIdentificado(GDASession sessao, uint idMovBanco, uint idCartaoNaoIdentificado)
        {
            objPersistence.ExecuteCommand(sessao,
                    string.Format("UPDATE mov_banco SET IdCartaoNaoIdentificado={0} WHERE IdMovBanco={1};", idCartaoNaoIdentificado, idMovBanco));
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaSinal(uint idContaBanco, uint idConta, int idLoja, uint idSinal, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaSinal(null, idContaBanco, idConta, idLoja, idSinal, idCliente, tipoMov, valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaSinal(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idSinal, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, idSinal, null, null, null, null, null, null, null, null, null, null,
                null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaSinalCompra(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idSinalCompra, uint? idContaPg, uint? idFornec, int tipoMov,
            decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, idSinalCompra, null, null, null, null, null, idContaPg,
                null, null, null, null, null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaAntecipFornec(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idAntecipFornec, uint? idContaPg, uint? idFornec, int tipoMov,
            decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, null, null, idContaPg,
                null, null, null, idAntecipFornec, null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaContaR(uint idContaBanco, uint idConta, int idLoja, uint? idPedido, uint? idLiberarPedido, uint? idContaR, uint? idSinal,
            uint idCliente, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovContaContaR(null, idContaBanco, idConta, idLoja, idPedido, idLiberarPedido, idContaR, idSinal, idCliente,
                tipoMov, valorMov, juros, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaContaR(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idPedido, uint? idLiberarPedido, uint? idContaR, uint? idSinal,
            uint idCliente, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovContaContaR(sessao, idContaBanco, idConta, idLoja, idPedido, idLiberarPedido, idContaR, idSinal, idCliente, tipoMov, valorMov, juros, dataMov, null);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// Utilizado em ContasReceberDAO
        /// </summary>
        public uint MovContaContaR(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idPedido, uint? idLiberarPedido, uint? idContaR, uint? idSinal,
            uint idCliente, int tipoMov, decimal valorMov, DateTime dataMov, uint? idCartaoNaoIdentificado)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, idPedido, idSinal, null, idContaR, null, null, null, null, null, idLiberarPedido,
                null, null, null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, idCartaoNaoIdentificado, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária, com referência do ArquivoQuitacaoParcelaCartao
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="idConta"></param>
        /// <param name="idLoja"></param>
        /// <param name="idArquivoQuitacaoParcelaCartao"></param>
        /// <param name="tipoMov"></param>
        /// <param name="valorMov"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public uint MovContaContaR(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, int? idArquivoQuitacaoParcelaCartao, int tipoMov, decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, idArquivoQuitacaoParcelaCartao, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaContaR(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idPedido, uint? idLiberarPedido, uint? idContaR, uint? idSinal,
            uint idCliente, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov, string obs)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, idPedido, idSinal, null, idContaR, null, null, null, null, null, idLiberarPedido,
                null, null, null, null, null, tipoMov, valorMov, juros, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaAcerto(uint idContaBanco, uint idConta, int idLoja, uint idAcerto, uint idCliente, int tipoMov, decimal valorMov,
            decimal juros, DateTime dataMov)
        {
            return MovContaAcerto(null, idContaBanco, idConta, idLoja, idAcerto, idCliente, tipoMov, valorMov, juros, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaAcerto(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idAcerto, uint idCliente, int tipoMov, decimal valorMov,
            decimal juros, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, null, null, null, idAcerto, null, null, null, null, null, null, null,
                null, null, null, tipoMov, valorMov, juros, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaDeposito(uint idContaBanco, uint idConta, int idLoja, uint idDeposito, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaDeposito(null, idContaBanco, idConta, idLoja, idDeposito, tipoMov, valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaDeposito(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idDeposito, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, idDeposito, null, null, null, null, null, null,
                null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaCheque(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idDeposito, uint idCheque, uint? idCliente, uint? idFornec, int tipoMov, decimal valorMov,
            DateTime dataMov)
        {
            return MovContaCheque(sessao, idContaBanco, idConta, idLoja, idDeposito, null, idCheque, idCliente, idFornec, tipoMov, valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaCheque(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idDeposito, uint? idPagto, uint idCheque, uint? idCliente, uint? idFornec,
            int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, idFornec, null, null, null, null, null, idDeposito, idCheque, idPagto, null, null,
                null, null, null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        public uint MovContaAcertoCheque(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idDeposito, uint idAcertoCheque, uint? idCliente,
            int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, null, null, null, null, idDeposito, null, null, null, null, null,
                null, null, null, null, tipoMov, valorMov, 0, dataMov, null, idAcertoCheque, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequePagto(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idCheque, uint? idPagto, uint idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovContaChequePagto(sessao, null, idContaBanco, idConta, idLoja, idCheque, idPagto, idFornec, tipoMov, valorMov, juros, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequePagto(GDASession sessao, int? idAcertoCheque, uint idContaBanco, uint idConta, int idLoja, uint idCheque,
            uint? idPagto, uint idFornec, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, idCheque, idPagto, null, null, null,
                null, null, null, null, tipoMov, valorMov, juros, dataMov, null, (uint?)idAcertoCheque, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequeCreditoFornecedor(uint idContaBanco, uint idConta, int idLoja, uint idCheque, uint? idCreditoFornecedor, uint idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovContaChequeCreditoFornecedor(null, idContaBanco, idConta, idLoja, idCheque, idCreditoFornecedor, idFornec, tipoMov, valorMov, juros, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequeCreditoFornecedor(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idCheque, uint? idCreditoFornecedor, uint idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, idCheque, null, null, null, null,
                null, null, null, null, tipoMov, valorMov, juros, dataMov, null, null, null, idCreditoFornecedor, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequeSinalCompra(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idCheque, uint? idSinalCompra, uint idFornec, int tipoMov,
            decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, idSinalCompra, null, null, null, idCheque, null, null, null, null,
                null, null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaChequeAntecipFornec(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idCheque, uint? idAntecipFornec, uint idFornec, int tipoMov,
            decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, idCheque, null, null, null, null,
                null, idAntecipFornec, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaPagto(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idPagto, uint? idContaPg, uint? idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov, string obs)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, null, idPagto, idContaPg, null, null, null,
                null, null, null, tipoMov, valorMov, juros, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaCreditoFornecedor(uint idContaBanco, uint idConta, int idLoja, uint idCreditoFornecedor, uint? idContaPg, uint? idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov, string obs)
        {
            return MovContaCreditoFornecedor(null, idContaBanco, idConta, idLoja, idCreditoFornecedor, idContaPg, idFornec, tipoMov, valorMov, juros, dataMov, obs);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaCreditoFornecedor(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idCreditoFornecedor, uint? idContaPg, uint? idFornec, int tipoMov,
            decimal valorMov, decimal juros, DateTime dataMov, string obs)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, idFornec, null, null, null, null, null, null, null, null, idContaPg, null, null, null,
                null, null, null, tipoMov, valorMov, juros, dataMov, obs, null, null, idCreditoFornecedor, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaLiberarPedido(uint idContaBanco, uint idConta, int idLoja, uint idLiberarPedido, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaLiberarPedido(null, idContaBanco, idConta, idLoja, idLiberarPedido, idCliente, tipoMov, valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaLiberarPedido(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idLiberarPedido, uint? idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, null, null, null, null, null, null, null, null, idLiberarPedido,
                null, null, null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public uint MovContaObra(uint idContaBanco, uint idConta, int idLoja, uint idObra, uint idCliente, int tipoMov, decimal valorMov,
            DateTime dataMov)
        {
            return MovContaObra(null, idContaBanco, idConta, idLoja, idObra, idCliente, tipoMov, valorMov, dataMov);
        }

        public uint MovContaObra(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idObra, uint idCliente, int tipoMov, decimal valorMov, 
            DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, null, null, null, null, null, null, null, null, null, null, idObra,
                null, null, null, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        public uint MovContaTransfConta(uint idContaBanco, uint idConta, int idLoja, uint idContaBancoDest, int tipoMov, decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, idContaBancoDest,
                null, null, null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor na conta bancária
        /// </summary>
        public uint MovContaAntecip(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idAntecipContaRec, int tipoMov, decimal valorMov,
            DateTime dataMov, string obs)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, idAntecipContaRec, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, false);
        }

        /// <summary>
        /// Debita valor da conta bancária
        /// </summary>
        public uint MovContaSaida(uint idContaBanco, uint idConta, int idLoja, int tipoMov, decimal valorMov, DateTime dataMov, string obs)
        {
            return MovContaSaida(null, idContaBanco, idConta, idLoja, tipoMov, valorMov, dataMov, obs);
        }

        /// <summary>
        /// Debita valor da conta bancária
        /// </summary>
        public uint MovContaSaida(GDASession session, uint idContaBanco, uint idConta, int idLoja, int tipoMov, decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, true);
        }

        /// <summary>
        /// Credita valor na conta bancária
        /// </summary>
        public uint MovContaCredito(uint idContaBanco, uint idConta, int idLoja, int tipoMov, decimal valorMov, DateTime dataMov, string obs)
        {
            return MovimentaConta(idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, tipoMov, valorMov, 0, dataMov, obs, null, null, null, null, null, true);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaTrocaDev(uint idContaBanco, uint idConta, int idLoja, uint idTrocaDevolucao, uint? idPedido, uint idCliente,
            int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaTrocaDev(null, idContaBanco, idConta, idLoja, idTrocaDevolucao, idPedido, idCliente, tipoMov,
                valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaTrocaDev(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idTrocaDevolucao, uint? idPedido, uint idCliente,
            int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, idPedido, null, null, null, null, null, null, null, null, null, null,
                null, null, null, idTrocaDevolucao, tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaMovFunc(uint idContaBanco, uint idConta, int idLoja, uint idCliente, uint? idPedido, uint? idLiberarPedido, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaMovFunc(null, idContaBanco, idConta, idLoja, idCliente, idPedido, idLiberarPedido, tipoMov,
                valorMov, dataMov);
        }

        /// <summary>
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaMovFunc(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idCliente, uint? idPedido, uint? idLiberarPedido, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, idPedido, null, null, null, null, null, null, null, null, idLiberarPedido, null, null, null, null, null,
                tipoMov, valorMov, 0, dataMov, null, null, null, null, null, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="idConta"></param>
        /// <param name="idDevolucaoPagto"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoMov"></param>
        /// <param name="valorMov"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public uint MovContaDevolucaoPagto(uint idContaBanco, uint idConta, int idLoja, uint idDevolucaoPagto, uint idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovContaDevolucaoPagto(null, idContaBanco, idConta, idLoja, idDevolucaoPagto, idCliente, tipoMov, valorMov, dataMov);
        }

        public uint MovContaDevolucaoPagto(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint idDevolucaoPagto, uint idCliente, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, tipoMov, valorMov, 0,
                dataMov, null, null, idDevolucaoPagto, null, null, null, false);
        }

        /// <summary>
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaDepositoNaoIdentificado(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idDepositoNaoIdentificado, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                tipoMov, valorMov, 0, dataMov, null, null, null, null, idDepositoNaoIdentificado, null, false);
        }

        /// <summary>
        /// Credita/Debita valor na conta bancária.
        /// </summary>
        public uint MovContaCartaoNaoIdentificado(GDASession session, uint idContaBanco, uint idConta, int idLoja, uint idCartaoNaoIdentificado, int tipoMov, decimal valorMov, DateTime dataMov)
        {
            return MovimentaConta(session, idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                tipoMov, valorMov, 0, dataMov, null, null, null, null, null, idCartaoNaoIdentificado, null, null, false);
        }

        public uint MovContaTerifaUsoBoleto(uint idArquivoRemessa, uint idContaBanco, uint idConta, int idLoja, decimal valorMov, DateTime dataMov, int tipoMov)
        {
            return MovimentaConta(idContaBanco, idConta, idLoja, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                tipoMov, valorMov, 0, dataMov, null, null, null, null, null, idArquivoRemessa, false);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        private uint MovimentaConta(uint idContaBanco, uint idConta, int idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idContaR,
            uint? idAcerto, uint? idDeposito, uint? idCheque, uint? idPagto, uint? idContaPg, uint? idLiberarPedido, uint? idContaBancoDest,
            uint? idObra, uint? idAntecipFornec, uint? idAntecipContaRec, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov,
            string obs, uint? idAcertoCheque, uint? idDevolucaoPagto, uint? idCreditoFornecedor, uint? idDepositoNaoIdentificado, uint? idArquivoRemessa, bool lancManual)
        {
            return MovimentaConta(null, idContaBanco, idConta, idLoja, idCliente, idFornec, idPedido, idSinal, idSinalCompra, idContaR, idAcerto,
                idDeposito, idCheque, idPagto, idContaPg, idLiberarPedido, idContaBancoDest, idObra, idAntecipFornec, idAntecipContaRec, idTrocaDevolucao,
                tipoMov, valorMov, juros, dataMov, obs, idAcertoCheque, idDevolucaoPagto, idCreditoFornecedor, idDepositoNaoIdentificado, null, null, idArquivoRemessa, lancManual);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        private uint MovimentaConta(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idContaR,
            uint? idAcerto, uint? idDeposito, uint? idCheque, uint? idPagto, uint? idContaPg, uint? idLiberarPedido, uint? idContaBancoDest,
            uint? idObra, uint? idAntecipFornec, uint? idAntecipContaRec, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov,
            string obs, uint? idAcertoCheque, uint? idDevolucaoPagto, uint? idCreditoFornecedor, uint? idDepositoNaoIdentificado, uint? idArquivoRemessa, bool lancManual)
        {
            return MovimentaConta(sessao, idContaBanco, idConta, idLoja, idCliente, idFornec, idPedido, idSinal, idSinalCompra, idContaR, idAcerto,
                idDeposito, idCheque, idPagto, idContaPg, idLiberarPedido, idContaBancoDest, idObra, idAntecipFornec, idAntecipContaRec, idTrocaDevolucao,
                tipoMov, valorMov, juros, dataMov, obs, idAcertoCheque, idDevolucaoPagto, idCreditoFornecedor, idDepositoNaoIdentificado, null, null, idArquivoRemessa, lancManual);
        }

        /// <summary>
        /// Credita/Debita valor da conta bancária
        /// </summary>
        private uint MovimentaConta(GDASession sessao, uint idContaBanco, uint idConta, int idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idContaR,
            uint? idAcerto, uint? idDeposito, uint? idCheque, uint? idPagto, uint? idContaPg, uint? idLiberarPedido, uint? idContaBancoDest,
            uint? idObra, uint? idAntecipFornec, uint? idAntecipContaRec, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, DateTime dataMov,
            string obs, uint? idAcertoCheque, uint? idDevolucaoPagto, uint? idCreditoFornecedor, uint? idDepositoNaoIdentificado, uint? idCartaoNaoIdentificado,
            int? idArquivoQuitacaoParcelaCartao, uint? idArquivoRemessa, bool lancManual)
        {
            // Verifica a conciliação bancária
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(sessao, idContaBanco, dataMov);

            // Gera movimentação
            MovBanco mov = new MovBanco();
            mov.IdLoja = idLoja;
            mov.DataMov = dataMov;
            mov.DataOriginal = dataMov;
            mov.TipoMov = tipoMov;
            mov.ValorMov = valorMov;
            mov.Juros = juros;
            mov.IdContaBanco = idContaBanco;
            mov.IdCliente = idCliente;
            mov.IdFornecedor = idFornec == 0 ? null : idFornec;
            mov.IdContaBancoDest = idContaBancoDest;
            mov.IdConta = idConta;
            mov.IdContaR = idContaR;
            mov.IdAcerto = idAcerto;
            mov.IdPedido = idPedido;
            mov.IdSinal = idSinal;
            mov.IdSinalCompra = idSinalCompra;
            mov.IdDeposito = idDeposito;
            mov.IdPagto = idPagto;
            mov.IdContaPg = idContaPg;
            mov.IdCheque = idCheque;
            mov.IdLiberarPedido = idLiberarPedido;
            mov.IdObra = idObra;
            mov.IdAntecipFornec = idAntecipFornec;
            mov.IdAcertoCheque = idAcertoCheque;
            mov.IdAntecipContaRec = idAntecipContaRec;
            mov.IdTrocaDevolucao = idTrocaDevolucao;
            mov.IdDevolucaoPagto = idDevolucaoPagto;
            mov.IdCreditoFornecedor = idCreditoFornecedor;
            mov.IdDepositoNaoIdentificado = idDepositoNaoIdentificado;
            mov.IdCartaoNaoIdentificado = idCartaoNaoIdentificado;
            mov.IdArquivoQuitacaoParcelaCartao = idArquivoQuitacaoParcelaCartao;
            mov.IdArquivoRemessa = idArquivoRemessa;
            mov.Saldo = tipoMov == 1 ? MovBancoDAO.Instance.GetSaldo(sessao, idContaBanco) + valorMov : MovBancoDAO.Instance.GetSaldo(sessao, idContaBanco) - valorMov;
            mov.Obs = obs;
            mov.LancManual = lancManual;           

            return MovBancoDAO.Instance.Insert(sessao, mov);
        }

        /// <summary>
        /// Associa um cartão não identificado a movimentações bancárias
        /// </summary>
        public void AssociarMovBancoIdCartaoNaoIdentificado(GDASession sessao, uint idMovBanco, uint idCartaoNaoIdentificado)
        {
            objPersistence.ExecuteCommand(sessao,
                    string.Format("UPDATE mov_banco SET IdCartaoNaoIdentificado={0} WHERE IdMovBanco={1};", idCartaoNaoIdentificado, idMovBanco));
        }

        #endregion

        #region Listagem de contas bancárias

        public string SqlList(uint idContaBanco, bool selecionar)
        {
            string count = "select count(*) from mov_banco where idContaBanco=c.idContaBanco";
            string campos = selecionar ? "c.*, l.NomeFantasia as NomeLoja, " +
                "(" + count + ")=0 as PodeEditar" : "Count(*)";

            string sql = "Select " + campos + " From conta_banco c " + 
                "Left Join loja l On (c.idLoja=l.idLoja) " +
                "Where 1";

            if (idContaBanco > 0)
                sql += " And c.IdContaBanco=" + idContaBanco;

            return sql;
        }

        public IList<ContaBanco> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlList(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(0, false), null);
        }

        public ContaBanco GetElement(uint idContaBanco)
        {
            return objPersistence.LoadOneData(SqlList(idContaBanco, true));
        }

        public ContaBanco[] GetOrdered()
        {
            return GetOrdered(null);
        }

        public ContaBanco[] GetOrdered(string dataSaldo)
        {
            var contas = objPersistence.LoadData("Select * From conta_banco c Where Situacao=" + (int)Situacao.Ativo + 
                " Order By Nome, Agencia, Conta").ToArray();

            Array.ForEach(contas, x => x.DataSaldo = dataSaldo);
            return contas;
        }

        public ContaBanco[] ObterBancoAgrupado()
        {
            return ObterBancoAgrupado(0, 0, 0);
        }

        public ContaBanco[] ObterBancoAgrupado(uint idContaR, uint idNf, uint idLoja)
        {
            var contasBanco = GetOrdered();

            if (idContaR > 0 || idNf > 0 || idLoja > 0)
            {
                if (FinanceiroConfig.BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf)
                {
                    if (idLoja == 0)
                    {
                        if (idNf > 0)
                        {
                            idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idNf);
                        }
                        else if (idContaR > 0)
                        {
                            var contaR = ContasReceberDAO.Instance.GetElement(idContaR);

                            if (contaR != null && contaR.IdNf.GetValueOrDefault(0) > 0)
                                idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(contaR.IdNf.Value);
                        }
                    }

                    if (idLoja > 0)
                        contasBanco = contasBanco.Where(f => f.IdLoja == idLoja).ToArray();
                }

                if (FinanceiroConfig.BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf)
                {
                    var idContaBanco = 0;

                    if (idNf > 0)
                    {
                        var idCliente = (int)NotaFiscalDAO.Instance.ObtemIdCliente(idNf).GetValueOrDefault();

                        if (idCliente > 0)
                            idContaBanco = ClienteDAO.Instance.ObtemIdContaBanco(null, idCliente);
                    }
                    else if (idContaR > 0)
                    {
                        var contaR = ContasReceberDAO.Instance.GetElement(idContaR);

                        if (contaR != null && contaR.IdNf.GetValueOrDefault(0) > 0)
                        {
                            var idCliente = (int)NotaFiscalDAO.Instance.ObtemIdCliente(contaR.IdNf.Value).GetValueOrDefault();

                            if (idCliente > 0)
                                idContaBanco = ClienteDAO.Instance.ObtemIdContaBanco(null, idCliente);
                        }
                    }

                    if (idContaBanco > 0)
                        contasBanco = contasBanco.Where(f => f.IdContaBanco == idContaBanco).ToArray();
                }
            }
            
            var codigoBancos = String.Join(",", Array.ConvertAll<int, string>(Enum.GetValues(typeof(Sync.Utils.CodigoBanco))
                .Cast<int>().ToList().ToArray(), x => x.ToString()));

            return contasBanco
                .Where(f => f.CodBanco > 0 && !String.IsNullOrEmpty(f.CodConvenio) &&
                    codigoBancos.Contains(f.CodBanco.GetValueOrDefault().ToString()))
                .OrderBy(f => f.IdContaBanco).ToArray();
        }

        #endregion

        #region Retorna o Saldo

        /// <summary>
        /// Retorna o saldo somado de todas as contas bancárias
        /// </summary>
        /// <returns></returns>
        public decimal GetSaldoGeral()
        {
            return GetSaldoGeral(false, null);
        }

        /// <summary>
        /// Retorna o saldo somado de todas as contas bancárias
        /// </summary>
        /// <returns></returns>
        public decimal GetSaldoGeral(bool descontarChequesPropriosAbertos)
        {
            return GetSaldoGeral(descontarChequesPropriosAbertos, null);
        }

        /// <summary>
        /// Retorna o saldo somado de todas as contas bancárias
        /// </summary>
        /// <returns></returns>
        public decimal GetSaldoGeral(bool descontarChequesPropriosAbertos, string dataSaldo)
        {
            // Busca as contas bancárias e seus respectivos saldos
            ContaBanco[] lstBanco = ContaBancoDAO.Instance.GetOrdered();

            decimal saldoGeral = 0;

            foreach (ContaBanco cb in lstBanco)
                saldoGeral += MovBancoDAO.Instance.GetSaldo((uint)cb.IdContaBanco, dataSaldo, descontarChequesPropriosAbertos);

            return saldoGeral;
        }

        #endregion

        #region Retorna a descrição

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retoran a descrição da conta bancária.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public string GetDescricao(uint idContaBanco)
        {
            return GetDescricao(null, idContaBanco);
        }

        /// <summary>
        /// 
        /// Retoran a descrição da conta bancária.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public string GetDescricao(GDASession sessao, uint idContaBanco)
        {
            return ObtemValorCampo<string>(sessao, "concat(nome, ' Agência: ', agencia, ' Conta: ', conta)", "idContaBanco=" + idContaBanco);
        }

        #endregion

        public int? ObtemCodigoBanco(uint idContaBanco)
        {
            return ObtemValorCampo<int?>("codBanco", "idContaBanco=" + idContaBanco);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(uint idContaBanco)
        {
            return ObtemIdLoja(null, idContaBanco);
        }

        public uint ObtemIdLoja(GDASession sessao, uint idContaBanco)
        {
            return ObtemValorCampo<uint>(sessao, "idLoja", "idContaBanco=" + idContaBanco);
        }

        public string ObtemNome(uint idContaBanco)
        {
            return ObtemValorCampo<string>("nome", "idContaBanco=" + idContaBanco);
        }

        public int ObtemSituacao(uint idContaBanco)
        {
            return ObtemValorCampo<int>("situacao", "idContaBanco=" + idContaBanco);
        }

        /// <summary>
        /// Retorna o id da conta bancária a partir do número da agência e conta da mesma
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="conta"></param>
        /// <returns>idContaBanco</returns>
        public uint GetIdByAgenciaConta(string agencia, string conta)
        {
            string sql = "Select * From conta_banco Where agencia='" + agencia + "' And conta='" + conta + "'";

            List<ContaBanco> lst = objPersistence.LoadData(sql);

            if (lst.Count == 0)
                throw new Exception("A conta e a agência cadastradas no cheque não se refere a nenhuma conta bancária cadastrada no sistema.");
            else if (lst.Count > 1)
                throw new Exception("A conta e a agência cadastradas no cheque se refere a mais de uma conta bancária cadastrada no sistema.");
            else
                return (uint)lst[0].IdContaBanco;
        }

        /// <summary>
        /// Verifica se o cheque passado já foi quitado na conta passada
        /// </summary>
        /// <param name="idCheque"></param>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public bool ChequePagtoQuitado(uint idCheque, uint idContaBanco)
        {
            return ChequePagtoQuitado(null, idCheque, idContaBanco);
        }

        /// <summary>
        /// Verifica se o cheque passado já foi quitado na conta passada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCheque"></param>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public bool ChequePagtoQuitado(GDASession session, uint idCheque, uint idContaBanco)
        {
            string sql = "Select count(*) From mov_banco Where tipoMov=2 And idCheque=" + idCheque + " And idContaBanco=" + idContaBanco;

            return objPersistence.ExecuteSqlQueryCount(session, sql, null) > 1;
        }

        public override int Delete(ContaBanco objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdContaBanco);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Veririca se existe algum pagamento associado à esta conta bancária
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From pagto_pagto Where idContaBanco=" + Key) > 0)
                throw new Exception("Esta conta bancária não pode ser excluída por haver pagamentos relacionados à mesma.");

            // Veririca se existe alguma movimentação para esta conta bancária
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From mov_banco Where idContaBanco=" + Key) > 0)
                throw new Exception("Esta conta bancária não pode ser excluída por haver movimentações relacionadas à mesma.");

            // Veririca se existe alguma parcela de cartão para esta conta bancária
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From contas_receber Where idContaBanco=" + Key + " And isParcelaCartao=True") > 0)
                throw new Exception("Esta conta bancária não pode ser excluída por haver parcela(s) de cartão relacionada(s) à mesma.");

            // Veririca se existe alguma associação com esta conta bancária
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From assoc_conta_banco Where idContaBanco=" + Key) > 0)
                throw new Exception("Esta conta bancária não pode ser excluída por haver associações em configurações relacionadas à mesma.");

            return GDAOperations.Delete(new ContaBanco { IdContaBanco = (int)Key });
        }
    }
}