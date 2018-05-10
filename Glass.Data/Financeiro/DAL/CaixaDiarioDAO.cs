using System;
using System.Linq;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CaixaDiarioDAO : BaseCadastroDAO<CaixaDiario, CaixaDiarioDAO>
    {
        //private CaixaDiarioDAO() { }

        private static readonly object _movimentarCaixaLock = new object();

        #region Movimenta caixa

        /// <summary>
        /// Movimentação geral
        /// </summary>
        public uint MovCaixa(uint idLoja, uint? idCliente, int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo)
        {
            return MovCaixa(null, idLoja, idCliente, tipoMov, valorMov, juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo, null);
        }

        /// <summary>
        /// Movimentação geral
        /// </summary>
        public uint MovCaixa(GDASession session, uint idLoja, uint? idCliente, int tipoMov, decimal valorMov, decimal juros,
            uint idConta, string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo, int? idCaixaDiarioEstorno)
        {
            return MovimentaCaixa(session, idLoja, idCliente, null, null, null, null, null, null, null, null, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, formaSaida, obs, mudarSaldo, null, null, null, null, idCaixaDiarioEstorno);
        }

        public uint MovCxCredito(uint idLoja, uint idConta, int tipoMov, int formaEntrada, decimal valor, string obs)
        {
            return MovimentaCaixa(idLoja, null, null, null, null, null, null, null, null, null, tipoMov, valor, 0, idConta, null, formaEntrada, obs, true);
        }

        /// <summary>
        /// Movimentação proveniente de pedido
        /// </summary>
        public uint MovCxPedido(GDASession sessao, uint idLoja, uint? idCliente, uint idPedido, int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, idPedido, null, null, null, null, null, null, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, 0, null, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// Movimentação proveniente de pedido
        /// </summary>
        public uint MovCxPedido(GDASession sessao, uint idLoja, uint? idCliente, uint idPedido, int tipoMov, decimal valorMov, decimal juros,
            uint idConta, string numAutConstrucard, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, idPedido, null, null, null, null, null, null, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, 0, obs, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Movimentação proveniente de conta a receber
        /// </summary>
        public uint MovCxContaRec(uint idLoja, uint? idCliente, uint? idPedido, uint? idLiberarPedido, uint idContaR,
            int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo)
        {
            return MovCxContaRec(null, idLoja, idCliente, idPedido, idLiberarPedido, idContaR, tipoMov, valorMov, juros,
                idConta, numAutConstrucard, formaSaida, obs, mudarSaldo);
        }

        /// <summary>
        /// Movimentação proveniente de conta a receber
        /// </summary>
        public uint MovCxContaRec(GDASession sessao, uint idLoja, uint? idCliente, uint? idPedido, uint? idLiberarPedido, uint idContaR,
            int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, idPedido, null, idContaR, null, idLiberarPedido, null, null, tipoMov, valorMov,
                juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// Movimentação proveniente de acerto
        /// </summary>
        public uint MovCxAcerto(GDASession sessao, uint idLoja, uint? idCliente, uint idAcerto, int tipoMov, decimal valorMov,
            decimal juros, uint idConta, string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, null, null, null, idAcerto, null, null, null, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, formaSaida, obs, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// Movimentação proveniente de liberação de pedidos
        /// </summary>
        public uint MovCxLiberarPedido(GDASession sessao, uint idLoja, uint? idCliente, uint idLiberarPedido, int tipoMov, decimal valorMov,
            decimal juros, uint idConta, string numAutConstrucard, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, null, null, null, null, idLiberarPedido, null, null, tipoMov, valorMov, juros,
                idConta, numAutConstrucard, 0, obs, mudarSaldo, null, null, 0);
        }

        public uint MovCxObra(GDASession sessao, uint idLoja, uint idCliente, uint idObra, int tipoMov, decimal valorMov, decimal juros, uint idConta,
            string numAutConstrucard, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, null, null, null, null, null, idObra, null, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, 0, obs, mudarSaldo, null, null, 0);
        }

        public uint MovCxTrocaDev(GDASession sessao, uint idLoja, uint idCliente, uint idTrocaDev, uint? idPedido, int tipoMov, decimal valorMov,
            decimal juros, uint idConta, string numAutConstrucard, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, idPedido, null, null, null, null, null, idTrocaDev, tipoMov, valorMov, juros, idConta,
                numAutConstrucard, 0, obs, mudarSaldo, null, null, 0);
        }

        public uint MovCxSinal(GDASession sessao, uint idLoja, uint idCliente, uint idSinal, int tipoMov, decimal valorMov, decimal juros, uint idConta,
            string numAutConstrucard, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, null, null, idSinal, null, null, null, null, null, tipoMov, valorMov, juros,
                idConta, numAutConstrucard, 0, obs, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// Movimenta o caixa diário da loja passada
        /// </summary>
        private uint MovimentaCaixa(uint idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idContaR, uint? idAcerto,
            uint? idLiberarPedido, uint? idObra, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, uint idConta,
            string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo)
        {
            return MovimentaCaixa(null, idLoja, idCliente, idFornec, idPedido, idSinal, idContaR, idAcerto, idLiberarPedido,
                idObra, idTrocaDevolucao, tipoMov, valorMov, juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo, null, null, 0);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idLoja, uint? idCheque, uint? idDeposito, uint? idAcertoCheque, uint? idCliente, uint? idFornec, uint idConta, int tipoMov,
            decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, string obs)
        {
            return MovCxAcertoCheque(sessao, idLoja, idCheque, idDeposito, idAcertoCheque, idCliente, idFornec, idConta, tipoMov,
                valorMov, juros, numAutConstrucard, mudarSaldo, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idLoja, uint? idCheque, uint? idDeposito, uint? idAcertoCheque, uint? idCliente, uint? idFornec, uint idConta, int tipoMov,
            decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, idFornec, null, null, null, null, null, null, null, tipoMov, valorMov,
                 juros, idConta, numAutConstrucard, 0, obs, mudarSaldo, idCheque, idAcertoCheque, 0, contadorDataUnica);
        }

        public uint MovCxDevolucaoPagto(GDASession sessao, uint idLoja, uint idDevolucaoPagto, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, string obs)
        {
            return MovCxDevolucaoPagto(sessao, idLoja, idDevolucaoPagto, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, obs, null);
        }

        public uint MovCxDevolucaoPagto(GDASession sessao, uint idLoja, uint idDevolucaoPagto, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, 0, null, null, null, null, null, null, null, tipoMov, valorMov,
                 juros, idConta, numAutConstrucard, 0, obs, mudarSaldo, 0, 0, idDevolucaoPagto, contadorDataUnica);
        }

        public uint MovCxCartaoNaoIdentificado(GDASession sessao, uint idLoja, uint idCartaoNaoIdentificado, uint idConta, int tipoMov,
            decimal valorMov, decimal juros, bool mudarSaldo, string obs)
        {
            return MovimentaCaixa(sessao, idLoja, null, 0, null, null, null, null, null, null, null, tipoMov, valorMov,
                 juros, idConta, null, 0, obs, mudarSaldo, 0, 0, idCartaoNaoIdentificado, null);
        }

        /// <summary>
        /// Chamado 18364: Variável criada para impedir que a movimentação seja inserida no mesmo segundo que outra
        /// só será resolvido em definitivo criando trigger na versão Migração
        /// </summary>
        private static DateTime ultimaMovimentacao = DateTime.Now;

        /// <summary>
        /// Movimenta o caixa diário da loja passada
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idContaR, uint? idAcerto,
            uint? idLiberarPedido, uint? idObra, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, uint idConta,
            string numAutConstrucard, int formaSaida, string obs, bool mudarSaldo, uint? idCheque, uint? idAcertoCheque, uint? idDevolucaoPagto)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, idFornec, idPedido, idSinal, idContaR, idAcerto, idLiberarPedido,
                idObra, idTrocaDevolucao, tipoMov, valorMov, juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo,
                idCheque, idAcertoCheque, idDevolucaoPagto, null);
        }

        /// <summary>
        /// Movimenta o caixa diário da loja passada
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idContaR, uint? idAcerto,
            uint? idLiberarPedido, uint? idObra, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard,
            int formaSaida, string obs, bool mudarSaldo, uint? idCheque, uint? idAcertoCheque, uint? idDevolucaoPagto, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, idFornec, idPedido, idSinal, idContaR, idAcerto, idLiberarPedido, idObra, idTrocaDevolucao, tipoMov,
                valorMov, juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo, idCheque, idAcertoCheque, idDevolucaoPagto, contadorDataUnica, null);
        }

        /// <summary>
        /// Movimenta o caixa diário da loja passada
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idContaR, uint? idAcerto,
            uint? idLiberarPedido, uint? idObra, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard,
            int formaSaida, string obs, bool mudarSaldo, uint? idCheque, uint? idAcertoCheque, uint? idDevolucaoPagto, int? contadorDataUnica, int? idCaixaDiarioEstorno)
        {
            return MovimentaCaixa(sessao, idLoja, idCliente, idFornec, idPedido, idSinal, idContaR, idAcerto, idLiberarPedido, idObra,
                idTrocaDevolucao, tipoMov, valorMov, juros, idConta, numAutConstrucard, formaSaida, obs, mudarSaldo, idCheque,
                idAcertoCheque, idDevolucaoPagto, null, contadorDataUnica, idCaixaDiarioEstorno);
        }

        /// <summary>
        /// Movimenta o caixa diário da loja passada
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idContaR, uint? idAcerto,
            uint? idLiberarPedido, uint? idObra, uint? idTrocaDevolucao, int tipoMov, decimal valorMov, decimal juros, uint idConta, string numAutConstrucard,
            int formaSaida, string obs, bool mudarSaldo, uint? idCheque, uint? idAcertoCheque, uint? idDevolucaoPagto, uint? idCartaoNaoIdentificado,
            int? contadorDataUnica, int? idCaixaDiarioEstorno)
        {
            lock (_movimentarCaixaLock)
            {
                // Chamado 18364: Impede que a movimentação seja inserida no mesmo segundo que outra.
                // Só será resolvido em definitivo criando trigger na versão Migração
                if (Math.Abs(FuncoesData.DateDiff(DateInterval.Second, ultimaMovimentacao, DateTime.Now)) <= 1)
                {
                    Thread.Sleep(2500);
                }

                ultimaMovimentacao = DateTime.Now;

                // Se for utilização de crédito, não gera movimentação no caixa
                if (new List<uint>(UtilsPlanoConta.GetLstCredito(3)).Contains(idConta))
                {
                    mudarSaldo = false;
                }

                CaixaDiario caixaDiario = new CaixaDiario();
                caixaDiario.IdLoja = idLoja;
                caixaDiario.IdCliente = idCliente;
                caixaDiario.IdFornec = idFornec == 0 ? null : idFornec;
                caixaDiario.IdPedido = idPedido;
                caixaDiario.IdSinal = idSinal;
                caixaDiario.IdAcerto = idAcerto;
                caixaDiario.IdContaR = idContaR;
                caixaDiario.IdLiberarPedido = idLiberarPedido;
                caixaDiario.IdObra = idObra;
                caixaDiario.IdTrocaDevolucao = idTrocaDevolucao;
                caixaDiario.TipoMov = tipoMov;
                caixaDiario.Valor = valorMov;
                caixaDiario.Juros = juros;
                caixaDiario.IdConta = idConta;
                caixaDiario.NumAutConstrucard = numAutConstrucard;
                caixaDiario.Obs = obs;
                caixaDiario.IdCheque = idCheque;
                caixaDiario.IdAcertoCheque = idAcertoCheque;
                caixaDiario.IdDevolucaoPagto = idDevolucaoPagto;
                caixaDiario.IdCaixaDiarioEstorno = idCaixaDiarioEstorno;
                caixaDiario.IdCartaoNaoIdentificado = idCartaoNaoIdentificado;
                caixaDiario.MudarSaldo = mudarSaldo;

                if (formaSaida > 0)
                {
                    caixaDiario.FormaSaida = formaSaida;
                }

                var idCaixaDiario = Insert(sessao, caixaDiario);

                /* Chamado 29589. */
                if (contadorDataUnica.HasValue)
                {
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_diario SET DataUnica=CONCAT(DataUnica, '_', {0}) WHERE IdCaixaDiario={1};", contadorDataUnica, idCaixaDiario));
                }

                return idCaixaDiario;
            }
        }

        #endregion

        #region Retorna movimentação

        /// <summary>
        /// Retorna uma única movimentação
        /// </summary>
        public CaixaDiario GetMovimentacao(GDASession session, int idCxDiario)
        {
            var sql = @"
                Select c.*, f.Nome as DescrUsuCad, p.Descricao as DescrPlanoConta, l.NomeFantasia as NomeLoja 
                From caixa_diario c 
                    Left Join funcionario f On c.UsuCad=f.IdFunc
                    Left Join loja l On c.IdLoja=l.IdLoja 
                    Left Join plano_contas p On c.IdConta=p.IdConta 
                Where c.idCaixaDiario=" + idCxDiario;

            var lstMov = objPersistence.LoadData(session, sql).ToList();

            return lstMov.Count > 0 ? lstMov[0] : null;
        }

        #endregion

        #region Recupera o saldo do caixa diário

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo do caixa diário
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetSaldoByLoja(uint idLoja)
        {
            return GetSaldoByLoja(null, idLoja);
        }

        /// <summary>
        /// Recupera o saldo do caixa diário
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetSaldoByLoja(GDASession sessao, uint idLoja)
        {
            return GetSaldoByLoja(sessao, idLoja, DateTime.Now);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo do caixa diário
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="data">O dia que será usado para busca.</param>
        /// <returns></returns>
        public decimal GetSaldoByLoja(uint idLoja, DateTime data)
        {
            return GetSaldoByLoja(null, idLoja, data);
        }

        /// <summary>
        /// Recupera o saldo de lançamentos avulsos do caixa diario pelo período informado
        /// </summary>
        /// <param name="dataSaldo"></param>
        /// <returns></returns>
        public decimal GetSaldoLancAvulsos(DateTime? dataIni, DateTime? dataFim)
        {
            if (dataIni == null && dataFim == null)
                return 0;
            
            string sql = @"
                Select Sum(valor) from caixa_diario 
                Where tipoMov = 2
                    And idacerto is null and idcheque is null and idpedido is null and idLiberarPedido is null and idcontar is null 
                    And idObra is null and idCheque is null and idFornec is null and idTrocaDevolucao is null and idSinal is null 
                    And idConta not in (" + (int)UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral) + "," +
                    (int)UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeralParaDiario) + ")";

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (dataIni != null)
            {
                sql += "and datacad >= ?dataIni ";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni.Value.ToString("dd/MM/yyyy 00:00:00"))));
            }
            if (dataFim != null)
            {
                sql += "and datacad <= ?dataFim ";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim.Value.ToString("dd/MM/yyyy 23:59:59"))));
            }

            return ExecuteScalar<uint>(sql, lstParam.ToArray());
        }

        /// <summary>
        /// Recupera o saldo do caixa diário
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="data">O dia que será usado para busca.</param>
        /// <returns></returns>
        public decimal GetSaldoByLoja(GDASession sessao, uint idLoja, DateTime data)
        {
            string sql = "Select idCaixaDiario From caixa_diario Where IdLoja=" + idLoja + @"
                and date(DataCad)=date(?data) Order By IdCaixaDiario {0} limit 1";

            GDAParameter d = new GDAParameter("?data", data);

            uint idCaixaDiarioUlt = ExecuteScalar<uint>(sessao, String.Format(sql, "Desc"), d);

            if (idCaixaDiarioUlt == 0)
                return 0;

            uint idCaixaDiarioPrim = ExecuteScalar<uint>(sessao, String.Format(sql, "Asc"), d);

            return ObtemValorCampo<decimal>(sessao, "saldo", "idCaixaDiario=" + Math.Max(idCaixaDiarioPrim, idCaixaDiarioUlt));
        }

        /// <summary>
        /// Retorna o saldo/entrada/saída por forma de pagamento e período
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <param name="idFunc"></param>
        /// <param name="data"></param>
        /// <param name="tipo">1-Saldo, 2-Entrada, 3-Saída, 4-Saída (Apenas estorno), 5-Retiradas (Apenas retiradas, sem estorno e sem transf. Cx Geral), 6-Entrada (Sem saldo remanescente)</param>
        /// <returns></returns>
        public decimal GetSaldoByFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagto, uint tipoCartao, uint idLoja, uint idFunc, DateTime data, int tipo)
        {
            return GetSaldoByFormaPagto(null, formaPagto, tipoCartao, idLoja, idFunc, data, tipo);
        }

        /// <summary>
        /// Retorna o saldo/entrada/saída por forma de pagamento e período
        /// </summary>
        /// <param name="session"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="idLoja"></param>
        /// <param name="idFunc"></param>
        /// <param name="data"></param>
        /// <param name="tipo">1-Saldo, 2-Entrada, 3-Saída, 4-Saída (Apenas estorno), 5-Retiradas (Apenas retiradas, sem estorno e sem transf. Cx Geral), 6-Entrada (Sem saldo remanescente)</param>
        /// <returns></returns>
        public decimal GetSaldoByFormaPagto(GDASession session, Glass.Data.Model.Pagto.FormaPagto formaPagto, uint tipoCartao, uint idLoja, uint idFunc, DateTime data, int tipo)
        {
            // FOI RETIRADO A SOMA DO JUROS COM O VALOR DA MOVIMENTAÇÃO PARA NÃO SOMAR JUROS DUAS VEZES,
            // UMA VEZ QUE O JUROS DE RECEBIMENTO, POR EXEMPLO, JÁ CRIA UMA NOVA MOVIMENTAÇÃO.

            var filtroFunc = String.Empty;
            if (idFunc > 0)
                filtroFunc += " And usuCad=" + idFunc;

            string sqlEntrada = @"Coalesce((Select Sum(valor) From caixa_diario Where idconta <>569 and tipomov=1 and " +
                (tipo == 6 ? "idConta<>" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.SaldoRemanescente) + " and " : "") + @"
                (idConta in(" + UtilsPlanoConta.GetLstEntradaByFormaPagto(formaPagto, tipoCartao, true) + ") " +
                (formaPagto == Glass.Data.Model.Pagto.FormaPagto.Dinheiro ? " Or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro :
                formaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ? " Or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio : String.Empty) +
                ") And dataCad>=?dtIni And dataCad<=?dtFim And idLoja=" + idLoja + filtroFunc + "), 0)";

            string sqlSaida = "Coalesce((Select Sum(valor) From caixa_diario Where tipomov=2 and (idConta in (" +
                UtilsPlanoConta.GetLstSaidaByFormaPagto(formaPagto, tipoCartao, tipo != 4 ? 1 : 2) + ") " +
                (formaPagto == Glass.Data.Model.Pagto.FormaPagto.Dinheiro && tipo != 4 && tipo != 5 ? " Or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro :
                formaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeProprio && tipo != 4 && tipo != 5 ? " Or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio : String.Empty) +
                ") And dataCad>=?dtIni And dataCad<=?dtFim And idLoja=" + idLoja + filtroFunc + "), 0)";

            string sqlRetiradas = "Coalesce((Select Sum(valor) From caixa_diario Where tipomov=2 and (idConta not in (" +
                UtilsPlanoConta.GetLstSaidaByFormaPagto(formaPagto, tipoCartao, 1) + ") " +
                (formaPagto == Glass.Data.Model.Pagto.FormaPagto.Dinheiro ? " And formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro :
                formaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ? " And formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio : String.Empty) +
                ") And dataCad>=?dtIni And dataCad<=?dtFim And idLoja=" + idLoja + filtroFunc + "), 0)";

            string sql =
                tipo == 1 ? "Select " + sqlEntrada + "-" + sqlSaida + " as Total" :
                tipo == 2 || tipo == 6 ? "Select " + sqlEntrada + " as Total" :
                tipo == 3 || tipo == 4 ? "Select " + sqlSaida + " as Total" :
                tipo == 5 ? "Select " + sqlRetiradas + " as Total" : String.Empty;

            return ExecuteScalar<decimal>(session, sql, GetParams(data));
        }

        private GDAParameter[] GetParams(DateTime data)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(data.ToString("dd/MM/yyyy 00:00"))));
            lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(data.ToString("dd/MM/yyyy 23:59"))));

            return lstParam.ToArray();
        }

        #endregion

        #region Busca o registro da tabela que contém o sinal do pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca o registro da tabela que contém o sinal do pedido
        /// </summary>
        public CaixaDiario GetPedidoSinal(uint idPedido)
        {
            return GetPedidoSinal(null, idPedido);
        }

        /// <summary>
        /// Busca o registro da tabela que contém o sinal do pedido
        /// </summary>
        public CaixaDiario GetPedidoSinal(GDASession sessao, uint idPedido)
        {
            var caixa = new CaixaDiario();

            var sql = @"
                Select c.*, f.Nome as DescrUsuCad 
                From caixa_diario c 
                    Left Join funcionario f On c.UsuCad=f.IdFunc 
                Where idSinal in (Select idSinal From pedido Where idPedido=" + idPedido + @") 
                    And idConta In (" + UtilsPlanoConta.ContasSinalPedido() + ")";

            var lst = objPersistence.LoadData(sessao, sql).ToList();

            if (lst.Count > 0)
                return lst[0];

            // Se não houver registro de sinal no caixa diário, procura no caixa geral
            CaixaGeral caixaGeral = CaixaGeralDAO.Instance.GetPedidoSinal(sessao, idPedido);

            if (caixaGeral != null)
            {
                caixa.DescrUsuCad = caixaGeral.DescrUsuCad;
                caixa.Valor = caixaGeral.ValorMov;
                caixa.DataCad = caixaGeral.DataMov;

                return caixa;
            }

            // Se não houver nenhum registro também no caixa geral, procura nas movimentações bancárias
            MovBanco movBanco = MovBancoDAO.Instance.GetPedidoSinal(sessao, idPedido);

            if (movBanco != null)
            {
                caixa.DescrUsuCad = movBanco.DescrUsuCad;
                caixa.Valor = movBanco.ValorMov;
                caixa.DataCad = movBanco.DataMov;

                return caixa;
            }

            return caixa;
        }

        #endregion

        #region Movimentações que serão usadas no estorno

        /// <summary>
        /// Retorna lista com movimentações que serão estornadas
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idSinal"></param>
        /// <param name="tipo">1-À Vista, 2-Entrada</param>
        /// <returns></returns>
        public CaixaDiario[] GetListForEstorno(uint? idPedido, uint? idLiberarPedido, uint? idSinal, int tipo)
        {
            return GetListForEstorno(null, idPedido, idLiberarPedido, idSinal, tipo);
        }

        /// <summary>
        /// Retorna lista com movimentações que serão estornadas
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idSinal"></param>
        /// <param name="tipo">1-À Vista, 2-Entrada</param>
        /// <returns></returns>
        public CaixaDiario[] GetListForEstorno(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint? idSinal, int tipo)
        {
            string idsConta = String.Empty;

            if (tipo == 1) // À Vista
                idsConta += UtilsPlanoConta.ContasAVista() + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado);
            else if (tipo == 2) // Entrada
                idsConta += UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ContasEstornoSinalPedido() + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoEntradaGerado);

            // Busca as movimentações que este pedido possa ter feito no caixa diário
            string sql = "Select * From caixa_diario Where idConta In (" + idsConta + ") ";

            if (idPedido > 0)
                sql += " And idPedido=" + idPedido;

            if (idLiberarPedido > 0)
                sql += " And idLiberarPedido=" + idLiberarPedido;

            if (idSinal > 0)
                sql += " And idSinal=" + idSinal;

            sql += " Order By idCaixaDiario Desc";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Verifica se a loja que o pedido foi recebido está com o caixa aberto

        /// <summary>
        /// Verifica se a loja que o pedido foi recebido está com o caixa aberto
        /// </summary>
        public bool CaixaFechadoPedido(uint idPedido)
        {
            return CaixaFechadoPedido(null, idPedido);
        }

        /// <summary>
        /// Verifica se a loja que o pedido foi recebido está com o caixa aberto
        /// </summary>
        public bool CaixaFechadoPedido(GDASession session, uint idPedido)
        {
            string sql = "Select * From caixa_diario Where idConta In ($idsConta) And idPedido=" + idPedido;

            // Busca as movimentações que este pedido possa ter feito no caixa diário
            List<CaixaDiario> lstAVista = objPersistence.LoadData(session, sql.Replace("$idsConta", UtilsPlanoConta.ContasAVista()));

            string idsMov = String.Empty;

            // Estorna valores à vista
            foreach (CaixaDiario c in lstAVista)
                if (CaixaFechado(session, c.IdLoja))
                    return true;

            return false;
        }

        #endregion

        #region Busca crédito utilizado em um pedido

        /// <summary>
        /// Busca crédito utilizado em um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoPedido(uint idPedido, bool aVista, bool sinal, bool recPrazo)
        {
            string idConta = String.Empty;

            if (aVista)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + ",";

            if (sinal)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + ",";

            if (recPrazo)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) + ",";

            idConta = idConta.Trim(',');

            string sql = "Select Round(Sum(Valor), 2) From caixa_diario Where idConta In (" + idConta + ") And idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito utilizado em uma liberação de pedido

        /// <summary>
        /// Busca crédito utilizado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoLiberarPedido(uint idLiberarPedido)
        {
            return GetCreditoLiberarPedido(idLiberarPedido, false);
        }

        /// <summary>
        /// Busca crédito utilizado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoLiberarPedido(uint idLiberarPedido, bool apenasEntrada)
        {
            string idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + (!apenasEntrada ? "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito) : String.Empty);

            string sql = "Select Round(Sum(Valor), 2) From caixa_diario Where idConta In (" + idConta + ") And idLiberarPedido=" + idLiberarPedido;
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito gerado em um pedido

        /// <summary>
        /// Busca crédito gerado em um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoGeradoPedido(uint idPedido, bool aVista, bool sinal, bool recPrazo)
        {
            string idConta = String.Empty;

            if (aVista)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + ",";

            if (sinal)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) + ",";

            if (recPrazo)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado) + ",";

            idConta = idConta.Trim(',');

            string sql = "Select Round(Sum(Valor), 2) From caixa_diario Where idConta In (" + idConta + ") And idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito gerado/usado em uma liberação de pedido

        /// <summary>
        /// Busca crédito gerado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoGeradoLiberarPedido(uint idLiberarPedido)
        {
            string idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado).ToString();

            string sql = "Select Round(Sum(Valor), 2) From caixa_diario Where idConta In (" + idConta + ") And idLiberarPedido=" + idLiberarPedido;
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Retorna o id da loja do caixa no qual o pedido passado foi confirmado

        /// <summary>
        /// Retorna o id da loja do caixa no qual o pedido passado foi confirmado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint GetLojaFromPedido(uint idPedido)
        {
            return ObtemValorCampo<uint>("idLoja", "idPedido=" + idPedido);
        }

        #endregion

        #region Retorna o id da loja do caixa no qual a liberação do pedido foi feita

        /// <summary>
        /// Retorna o id da loja do caixa no qual a liberação do pedido foi feita
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public uint GetLojaFromLiberarPedido(uint idLiberarPedido)
        {
            return ObtemValorCampo<uint>("idLoja", "idLiberarPedido=" + idLiberarPedido);
        }

        #endregion

        #region Fecha o caixa diário

        /// <summary>
        /// Fecha o caixa diário
        /// </summary>
        public void FechaCaixa(uint idLoja, decimal valorTransf, DateTime dataFechamento, bool fechamentoAtrasado)
        {
            using (var transaction = new GDATransaction())
            {
                CaixaDiario cxDiario = new CaixaDiario();

                try
                {
                    transaction.BeginTransaction();

                    if (CaixaFechado(transaction, idLoja))
                        throw new Exception("O caixa já foi fechado.");

                    uint idCxDiario = 0;
                    uint idCxDiarioSaldoRemanescente = 0;
                    bool existeMovCxDiario = ExisteMovimentacoes(transaction, idLoja);
                    var contadorDataUnica = 0;

                    // Busca as movimentações para ter acesso ao total em dinheiro e cheque
                    cxDiario = GetForRpt(transaction, idLoja, 0, dataFechamento)[0];

                    // Busca o saldo acumulado no dia ou do dia que o caixa não foi fechado
                    decimal saldo = !fechamentoAtrasado ? GetSaldoByLoja(transaction, idLoja) : GetSaldoDiaAnterior(transaction, idLoja, DateTime.Now);

                    // Se houver apenas movimentação de restante de saldo, recupera o mesmo
                    decimal saldoAnterior = 0;
                    if (saldo == 0 && GetMovimentacoes(transaction, idLoja, 0, DateTime.Now).Length <= 1)
                    {
                        saldo = GetSaldoDiaAnterior(transaction, idLoja, DateTime.Now);
                        saldoAnterior = saldo;
                    }

                    if (saldo < 0)
                        throw new Exception("Não é possível fechar o caixa com saldo negativo.");

                    if (valorTransf != saldo)
                    {
                        // Verifica se o valor a ser transferido é menor que o saldo disponível
                        if (saldo < valorTransf)
                            throw new Exception("O valor a ser transferido não pode ser maior que o saldo do caixa.");

                        // Verifica se o valor que será deixado no caixa é menor ou igual ao valor de dinheiro no caixa
                        if ((saldo - valorTransf) > cxDiario.TotalDinheiro + saldoAnterior)
                            throw new Exception("O valor a ser transferido deve ser no mínimo de " +
                                (saldo - cxDiario.TotalDinheiro + saldoAnterior).ToString("C") + ".");
                    }

                    // Insere registro no caixa diário indicando que o caixa foi fechado
                    CaixaDiario caixa = new CaixaDiario();
                    caixa.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);
                    caixa.IdLoja = idLoja;
                    caixa.Valor = valorTransf;
                    caixa.Saldo = saldo - valorTransf;
                    caixa.TipoMov = 2;
                    caixa.DataCad = dataFechamento;
                    caixa.Usucad = UserInfo.GetUserInfo.CodUser;
                    /* Chamado 16661.
                     * Deve chamar o insert base para não alterar a data de cadastro da movimentação. */
                    idCxDiario = base.InsertBase(transaction, caixa);

                    // Se for fechamento de caixa atrasado e o valor transferido for menor que o saldo daquele dia,
                    // gera uma movimentação "saldo remanescente"
                    if (fechamentoAtrasado && (saldo - valorTransf) > 0)
                    {
                        CaixaDiario caixaSaldoRem = new CaixaDiario();
                        caixaSaldoRem.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.SaldoRemanescente);
                        caixaSaldoRem.IdLoja = idLoja;
                        caixaSaldoRem.Valor = saldo - valorTransf;
                        caixaSaldoRem.Saldo = caixaSaldoRem.Valor;
                        caixaSaldoRem.TipoMov = 1;
                        caixaSaldoRem.DataCad = DateTime.Now;
                        caixaSaldoRem.Usucad = UserInfo.GetUserInfo.CodUser;
                        /* Chamado 16661.
                         * Deve chamar o insert base para não alterar a data de cadastro da movimentação. */
                        idCxDiarioSaldoRemanescente = base.InsertBase(transaction, caixaSaldoRem);
                    }

                    if (valorTransf > 0)
                    {
                        decimal valorTransfTemp = valorTransf;
                        decimal valor;

                        // Insere registros no caixa geral passando o valor que foi fechado no caixa diario

                        // Transfere valor em cheque para o caixa geral
                        valor = Math.Min(valorTransfTemp, cxDiario.TotalCheques);
                        if (valor > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque), 1, 0,
                                valor, 0, null, true, null, dataFechamento);
                            valorTransfTemp -= valor;
                        }

                        // Transfere o valor em cartão para o caixa geral
                        if (cxDiario.TotalCartao > 0)
                        {
                            // Transfere o total de cartão de CRÉDITO para o caixa geral
                            valor = cxDiario.Cartoes.Where(f => f.Tipo == TipoCartaoEnum.Credito).Sum(f => f.Valor);

                            if (valor > 0)
                            {
                                CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCartao), 1, 0, valor, 0,
                                    null, false, "Cartão de crédito", dataFechamento, false, contadorDataUnica++);
                                valorTransfTemp -= valor;
                            }

                            // Transfere o total de cartão de DÉBITO para o caixa geral
                            valor = cxDiario.TotalCartao - valor;
                            if (valor > 0)
                            {
                                CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCartao), 1, 0, valor, 0,
                                    null, false, "Cartão de débito", dataFechamento, false, contadorDataUnica++);
                                valorTransfTemp -= valor;
                            }
                        }

                        // Transfere o valor em depósito para o caixa geral
                        if (cxDiario.TotalDeposito > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDeposito), 1, 0,
                                cxDiario.TotalDeposito, 0, null, false, null, dataFechamento);
                            valorTransfTemp -= cxDiario.TotalDeposito;
                        }

                        // Transfere o valor em construcard para o caixa geral
                        if (cxDiario.TotalConstrucard > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioConstrucard), 1, 0,
                                cxDiario.TotalConstrucard, 0, null, false, null, dataFechamento);
                            valorTransfTemp -= cxDiario.TotalConstrucard;
                        }

                        // Transfere o valor em boleto para o caixa geral
                        if (cxDiario.TotalBoleto > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioBoleto), 1, 0,
                                cxDiario.TotalBoleto, 0, null, false, null, dataFechamento);
                            valorTransfTemp -= cxDiario.TotalBoleto;
                        }

                        // Transfere o valor em permuta para o caixa geral (Não precisa subtrair a permuta de valorTransfTemp, pois este valor não é somado no saldo)
                        if (cxDiario.TotalPermuta > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioPermuta), 1, 0,
                                cxDiario.TotalPermuta, 0, null, false, null, dataFechamento);
                        }

                        // Transfere o valor em dinheiro para o caixa geral, inseri "- (saldo - valorTransf)" para que
                        // o valor do saldo remanescente não seja transferido para o caixa geral
                        valor = Math.Min(valorTransfTemp, cxDiario.TotalDinheiro - (saldo - valorTransf));

                        // Caso o total em dinheiro do caixa de hoje esteja vazio, tenha valor em dinheiro para ser transferido e não
                        // tenha nenhuma movimentação no caixa hoje, força a transferir o valor restante
                        if (cxDiario.TotalDinheiro == 0 && valorTransfTemp > 0 && !existeMovCxDiario)
                            valor = valorTransfTemp;

                        if (valor > 0)
                        {
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, idLoja, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro), 1, 0,
                                valor, 0, null, true, null, dataFechamento);
                            valorTransfTemp -= valor;
                        }
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FechaCaixa - IdLoja: {0} - ValorTransf: {1} - DataFechamento: {2} - FechamentoAtrasado: {3} - " +
                        "TotalBoleto: {4} - TotalBoletoBancoBrasil: {5} - TotalBoletoLumen: {6} - TotalBoletoOutros: {7} - TotalBoletoSantander: {8} - TotalCartao: {9} - " +
                        "TotalCheques: {10} - TotalConstrucard: {11} - TotalDeposito: {12} - TotalDinheiro: {13} - TotalEstornoDinheiro: {14} - TotalMasterCredito: {15} - " +
                        "TotalMasterDebito: {16} - TotalOutrosCredito: {17} - TotalOutrosDebito: {18} - TotalPermuta: {19} - TotalRecDinheiro: {20} - TotalSaidaCheque: {21} - " +
                        "TotalSaidaDinheiro: {22} - TotalTransfCxGeralDinheiro: {23} - TotalVisaCredito: {24} - TotalVisaDebito: {25}",
                        idLoja, valorTransf, dataFechamento, fechamentoAtrasado, cxDiario.TotalBoleto, cxDiario.TotalBoletoBancoBrasil, cxDiario.TotalBoletoLumen,
                        cxDiario.TotalBoletoOutros, cxDiario.TotalBoletoSantander, cxDiario.TotalCartao, cxDiario.TotalCheques, cxDiario.TotalConstrucard,
                        cxDiario.TotalDeposito, cxDiario.TotalDinheiro, cxDiario.TotalEstornoDinheiro, cxDiario.TotalMasterCredito, cxDiario.TotalMasterDebito,
                        cxDiario.TotalOutrosCredito, cxDiario.TotalOutrosDebito, cxDiario.TotalPermuta, cxDiario.TotalRecDinheiro, cxDiario.TotalSaidaCheque,
                        cxDiario.TotalSaidaDinheiro, cxDiario.TotalTransfCxGeralDinheiro, cxDiario.TotalVisaCredito, cxDiario.TotalVisaDebito), ex);
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg(ex.Message, ex));
                }
            }
        }

        #endregion

        #region Reabre o caixa diário

        /// <summary>
        /// Reabre o caixa diário.
        /// </summary>
        /// <param name="idLoja"></param>
        public void ReabrirCaixa(uint idLoja)
        {
            if (!CaixaFechado(idLoja))
                throw new Exception("O caixa não está fechado.");

            try
            {
                // Apaga a movimentação de transferência para o caixa geral
                objPersistence.ExecuteCommand("delete from caixa_diario where date(dataCad)=date(now()) and idLoja=" +
                    idLoja + " and idConta=" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral));

                // Planos de contas usadas no SQL
                string contas = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCartao) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDeposito) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioConstrucard) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioBoleto) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioPermuta);

                // Variável com o filtro das consultas SQL
                string where = string.Format("date(dataMov)=date(now()) and idLoja={0} and idConta in ({1}) AND (LancManual IS NULL OR LancManual = 0)", idLoja, contas);

                // Ajusta o saldo das movimentações do caixa geral
                string sql = @"select (select saldo from caixa_geral where idCaixaGeral < all (
                    select idCaixaGeral from caixa_geral where " + where + @") order by idCaixaGeral desc limit 1)-(
                    select saldo from caixa_geral where " + where + " limit 1)";

                float valorMov = ExecuteScalar<float>(sql);
                objPersistence.ExecuteCommand("update caixa_geral set saldo=saldo-?valor where idCaixaGeral > all (" +
                    "select idCaixaGeral from (select idCaixaGeral from caixa_geral where " + where + ") as temp)",
                    new GDAParameter("?valor", valorMov));

                // Remove as movimentações do caixa geral
                objPersistence.ExecuteCommand("delete from caixa_geral where " + where);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao reabrir caixa diário.", ex));
            }
        }

        #endregion

        #region Busca as movimentações de hoje da loja passada

        /// <summary>
        /// Busca as movimentações de hoje da loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public CaixaDiario[] GetMovimentacoes(uint idLoja, uint idFunc, DateTime data)
        {
            return GetMovimentacoes(null, idLoja, idFunc, data);
        }

        /// <summary>
        /// Busca as movimentações de hoje da loja passada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idLoja"></param>
        /// <param name="idFunc"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public CaixaDiario[] GetMovimentacoes(GDASession session, uint idLoja, uint idFunc, DateTime data)
        {
            string sql = @"
                Select c.*, f.Nome as DescrUsuCad, CONCAT(p.Descricao, COALESCE(IF(COALESCE(p.descricao, '') <> '', CONCAT(' (', c.Obs ,')'), ''),'')) as DescrPlanoConta, l.NomeFantasia as NomeLoja, cli.Nome as NomeCliente,
                    coalesce(forn.RazaoSocial, forn.NomeFantasia) as NomeFornecedor
                From caixa_diario c 
                    Left Join funcionario f On c.UsuCad=f.IdFunc 
                    Left Join loja l On c.IdLoja=l.IdLoja 
                    Left Join cliente cli On cli.id_Cli=c.idCliente
                    Left Join fornecedor forn On forn.idFornec=c.idFornec
                    Left Join plano_contas p On c.IdConta=p.IdConta 
                Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(?data) 
                    And MONTH(c.DataCad)=MONTH(?data) 
                    And YEAR(c.DataCad)=YEAR(?data) 
                    And c.IdLoja=" + idLoja;

            if (idFunc > 0)
                sql += " and c.usuCad=" + idFunc + " And c.idConta<>" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);
            
            sql += " Order by c.idCaixaDiario";

            GDAParameter param = new GDAParameter("?data", data);

            List<CaixaDiario> lstCx = objPersistence.LoadData(session, sql, new GDAParameter[] { param });

            RecalculaSaldos(session, idFunc, ref lstCx);

            // Se não houver movimentações no caixa de hoje, busca o saldo remanescente do dia anterior
            if (lstCx.Count == 0)
            {
                decimal saldoDiaAnterior = GetSaldoDiaAnterior(session, idLoja, data);

                if (saldoDiaAnterior > 0)
                {
                    CaixaDiario cx = new CaixaDiario();
                    cx.DescrPlanoConta = "Saldo Remanescente";
                    cx.DataCad = DateTime.Now;
                    cx.Saldo = saldoDiaAnterior;
                    cx.Valor = saldoDiaAnterior;
                    cx.TipoMov = 1;
                    lstCx.Add(cx);
                }
            }

            return lstCx.ToArray();
        }

        /// <summary>
        /// Recalcula os saldos caso seja usado filtro por funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="lstCx"></param>
        private void RecalculaSaldos(uint idFunc, ref List<CaixaDiario> lstCx)
        {
            RecalculaSaldos(null, idFunc, ref lstCx);
        }

        /// <summary>
        /// Recalcula os saldos caso seja usado filtro por funcionário
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idFunc"></param>
        /// <param name="lstCx"></param>
        private void RecalculaSaldos(GDASession session, uint idFunc, ref List<CaixaDiario> lstCx)
        {
            // Se houver filtro por funcionário, calcula o saldo de 
            // cada operação realizada por ele e estornos, uma vez que o que está salvo no BD considera todas 
            // as movimentações do caixa diário
            if (idFunc > 0)
            {
                decimal saldo = 0;

                for (int i = 0; i < lstCx.Count; i++)
                {
                    if (i == 0)
                    {
                        CaixaDiario movAnterior = BuscaMovAnterior(session, lstCx[0].IdCaixaDiario);

                        if (movAnterior != null && movAnterior.Saldo == lstCx[0].Saldo && lstCx[0].IdConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.SaldoRemanescente))
                            lstCx[0].Saldo = 0;
                        else
                        {
                            lstCx[0].Saldo = lstCx[0].Valor * (lstCx[0].TipoMov == 1 ? 1 : -1);
                            saldo = lstCx[0].Saldo;
                        }

                        continue;
                    }

                    // Só altera se o saldo tiver sido alterado
                    if (lstCx[i].Saldo != ObtemSaldoMovAnterior(session, lstCx[i].IdCaixaDiario))
                    {
                        if (lstCx[i].TipoMov == 1)
                            saldo += lstCx[i].Valor;
                        else
                            saldo -= lstCx[i].Valor;
                    }

                    lstCx[i].Saldo = saldo;
                }
            }
        }

        /// <summary>
        /// Verifica se existe alguma movimentação neste dia
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ExisteMovimentacao(uint idLoja, DateTime data)
        {
            string sql = @"
                Select Count(*) > 0
                From caixa_diario c 
                Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(?data) 
                    And MONTH(c.DataCad)=MONTH(?data) 
                    And YEAR(c.DataCad)=YEAR(?data) 
                    And c.IdLoja=" + idLoja;

            return ExecuteScalar<bool>(sql, new GDAParameter("?data", data));
        }

        #endregion

        #region Busca as movimentações de hoje da loja passada com os totais em cheque/dinheiro/cartão/crédito

        /// <summary>
        /// Busca as movimentações de hoje da loja passada com os totais em cheque/dinheiro/cartão/crédito
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public CaixaDiario[] GetForRpt(uint idLoja, uint idFunc, DateTime data)
        {
            return GetForRpt(null, idLoja, idFunc, data);
        }

        /// <summary>
        /// Busca as movimentações de hoje da loja passada com os totais em cheque/dinheiro/cartão/crédito
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public CaixaDiario[] GetForRpt(GDASession session, uint idLoja, uint idFunc, DateTime data)
        {
            string sql = @"
                Select c.*, f.Nome as DescrUsuCad, p.Descricao as DescrPlanoConta, l.NomeFantasia as NomeLoja, cli.Nome as NomeCliente,
                    coalesce(forn.RazaoSocial, forn.NomeFantasia) as NomeFornecedor
                From caixa_diario c 
                    Left Join funcionario f On c.UsuCad=f.IdFunc 
                    Left Join loja l On c.IdLoja=l.IdLoja 
                    Left Join cliente cli On cli.id_Cli=c.idCliente
                    Left Join fornecedor forn On forn.idFornec=c.idFornec
                    Left Join plano_contas p On c.IdConta=p.IdConta 
                Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(?data)
                    And MONTH(c.DataCad)=MONTH(?data) 
                    And YEAR(c.DataCad)=YEAR(?data) 
                    And c.IdLoja=" + idLoja;

            var filtroFunc = String.Empty;
            if (idFunc > 0)
                filtroFunc = " and c.usuCad=" + idFunc + " And c.idConta<>" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);

            sql += filtroFunc + " Order by c.idCaixaDiario";

            GDAParameter param = new GDAParameter("?data", data);

            List<CaixaDiario> lst = objPersistence.LoadData(session, sql, param);

            if (lst.Count == 0)
                lst.Add(new CaixaDiario());

            decimal creditoGerado = ExecuteScalar<decimal>(session, "Select Sum(c.valor) From caixa_diario c " +
                "Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(?data) And MONTH(c.DataCad)=MONTH(?data) And YEAR(c.DataCad)=YEAR(?data) " +
                "And idConta In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado) + ") " +
                "And c.IdLoja=" + idLoja + filtroFunc, param);

            decimal estornoCreditoGerado = ExecuteScalar<decimal>(session, "Select Sum(c.valor) From caixa_diario c " +
                "Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(?data) And MONTH(c.DataCad)=MONTH(?data) And YEAR(c.DataCad)=YEAR(?data) " +
                "And idConta In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoVendaGerado) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoRecPrazoGerado) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraEstorno) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoEntradaGerado) + ") " +
                "And c.IdLoja=" + idLoja + filtroFunc, param);

            decimal totalTransfCxGeralDinheiro = ExecuteScalar<decimal>(session, "Select Sum(c.valorMov) From caixa_geral c " +
                "Where DAYOFMONTH(c.DataMovBanco)=DAYOFMONTH(?data) And MONTH(c.DataMovBanco)=MONTH(?data) And YEAR(c.DataMovBanco)=YEAR(?data) " +
                "And idConta In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) + ") " +
                "And c.IdLoja=" + idLoja + filtroFunc, param);

            RecalculaSaldos(session, idFunc, ref lst);

            var estornoCheque = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, idLoja, idFunc, data, 4);

            lst[0].TotalDinheiro = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, idLoja, idFunc, data, 1);
            lst[0].TotalRecDinheiro = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, idLoja, idFunc, data, 6);
            lst[0].TotalEstornoDinheiro = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, idLoja, idFunc, data, 4);
            lst[0].TotalSaidaDinheiro = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, idLoja, idFunc, data, 5);
            lst[0].TotalTransfCxGeralDinheiro = totalTransfCxGeralDinheiro;

            /* Chamado 23396, 23428. */
            //lst[0].TotalCheques = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, idLoja, idFunc, data, 1) - estornoCheque;
            lst[0].TotalCheques = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, idLoja, idFunc, data, 1);
            lst[0].TotalCartao = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Cartao, 0, idLoja, idFunc, data, 1);
            lst[0].CreditoGerado = creditoGerado - estornoCreditoGerado;
            lst[0].CreditoRecebido = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Credito, 0, idLoja, idFunc, data, 1);
            lst[0].CreditoVisible = true;

            var cartoes = UtilsPlanoConta.ContasCartoes;

            foreach (var c in cartoes)
                c.Valor = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Cartao, (uint)c.IdTipoCartao, idLoja, idFunc, data, 1);

            lst[0].Cartoes = cartoes;

            lst[0].TotalSaidaCheque = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, idLoja, idFunc, data, 3) - estornoCheque;
            lst[0].TotalBoleto = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Boleto, 0, idLoja, idFunc, data, 1);
            lst[0].TotalBoletoBancoBrasil = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Boleto, (uint)Utils.TipoBoleto.BancoBrasil, idLoja, idFunc, data, 1);
            lst[0].TotalBoletoLumen = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Boleto, (uint)Utils.TipoBoleto.Lumen, idLoja, idFunc, data, 1);
            lst[0].TotalBoletoOutros = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Boleto, (uint)Utils.TipoBoleto.Outros, idLoja, idFunc, data, 1);
            lst[0].TotalBoletoSantander = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Boleto, (uint)Utils.TipoBoleto.Santander, idLoja, idFunc, data, 1);
            lst[0].TotalDeposito = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Deposito, 0, idLoja, idFunc, data, 1); ;
            lst[0].TotalConstrucard = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Construcard, 0, idLoja, idFunc, data, 1);
            lst[0].TotalPermuta = GetSaldoByFormaPagto(session, Glass.Data.Model.Pagto.FormaPagto.Permuta, 0, idLoja, idFunc, data, 1);

            return lst.ToArray();
        }

        #endregion

        #region Retorna movimentações relacionadas ao acerto passado

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public IList<CaixaDiario> GetByAcerto(GDASession sessao, uint idAcerto)
        {
            string sql = "Select * From caixa_diario where idAcerto=" + idAcerto + " And idConta Not In (" +
                FinanceiroConfig.PlanoContaJurosCartao + ") Order By idCaixaDiario Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a um acerto de cheques

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public IList<CaixaDiario> GetByAcertoCheque(uint idAcertoCheque)
        {
            return GetByAcertoCheque(null, idAcertoCheque);
        }

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public IList<CaixaDiario> GetByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            string sql = "Select * From caixa_diario where idAcertoCheque=" + idAcertoCheque + " Order By idCaixaDiario Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Retorna movimentações relacionadas à liberação passada

        /// <summary>
        /// Retorna movimentações relacionadas à liberação passada
        /// </summary>
        public CaixaDiario[] GetByLiberacao(GDASession session, uint idLiberarPedido)
        {
            string sql = "Select * From caixa_diario where idAcerto=" + idLiberarPedido + " And idConta Not In (" +
                FinanceiroConfig.PlanoContaJurosCartao + ") Order By idCaixaDiario Desc";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um sinal de pedido

        /// <summary>
        /// Busca movimentações relacionadas a um sinal de pedido
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public IList<CaixaDiario> GetBySinal(uint idPedido)
        {
            string sql = @"
                Select * From caixa_diario
                Where idPedido=" + idPedido + @" 
                    And idConta in (" + UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ResumoDiarioContasCreditoGerado() + @")
                Order By idCaixaDiario Desc";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Retorna movimentações relacionadas à uma conta recebida

        /// <summary>
        /// Retorna as últimas movimentações relacionadas à uma conta recebida
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public CaixaDiario[] GetByContaRec(GDASession sessao, uint idContaR)
        {
            string sql = "Select * From caixa_diario where idContaR=" + idContaR + " Order By idCaixaDiario desc";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a uma obra

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public CaixaDiario[] GetByObra(uint idObra)
        {
            return GetByObra(null, idObra);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public CaixaDiario[] GetByObra(GDASession sessao, uint idObra)
        {
            string sql = "Select * From caixa_diario where idObra=" + idObra + " And idConta In (" + UtilsPlanoConta.ContasAVista() +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado) + ") Order By idCaixaDiario Desc";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Estorno retirada/crédito

        private object EstornarCreditoOuRetiradaLock = new object();

        #region Recupera a movimentação de crédito/retirada

        /// <summary>
        /// Recupera a movimentação de crédito/retirada.
        /// </summary>
        public void RecuperarMovimentacaoCreditoOuRetirada(GDASession session, int idCaixaDiario, out CaixaDiario movimentacao)
        {
            if (idCaixaDiario == 0)
                throw new Exception("Esta movimentação não existe.");

            movimentacao = GetMovimentacao(session, idCaixaDiario);

            if (movimentacao == null)
                throw new Exception("Esta movimentação não existe.");

            if (DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy")) != DateTime.Parse(movimentacao.DataCad.ToString("dd/MM/yyyy")))
                throw new Exception("Esta movimentação não foi feita hoje.");
        }

        #endregion

        #region Valida o estorno do crédito/retirada

        /// <summary>
        /// Valida o estorno da movimentação.
        /// </summary>
        public void ValidarEstornoCreditoOuRetirada(int idCaixaDiario, int formaSaida)
        {
            if (formaSaida == 0)
                throw new Exception("Só é possível estornar valores lançados manualmente no caixa.");
        }

        #endregion

        #region Verifica se o estorno do crédito/retirada já foi efetuado

        /// <summary>
        /// Verifica se o estorno da movimentação já foi efetuado.
        /// </summary>
        public void VerificarEstornoCreditoOuRetiradaJaEfetuadoComTransacao(int idCaixaDiario, out CaixaDiario movimentacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    VerificarEstornoCreditoOuRetiradaJaEfetuado(transaction, idCaixaDiario, out movimentacao);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Verifica se o estorno da movimentação já foi efetuado.
        /// </summary>
        public void VerificarEstornoCreditoOuRetiradaJaEfetuado(GDASession session, int idCaixaDiario, out CaixaDiario movimentacao)
        {
            RecuperarMovimentacaoCreditoOuRetirada(session, idCaixaDiario, out movimentacao);

            var podeEstornar =
                ExecuteScalar<int>(session,
                    string.Format("SELECT COUNT(*) FROM caixa_diario WHERE IdCaixaDiarioEstorno={0}", idCaixaDiario),
                    new GDAParameter("?data", DateTime.Now)) == 0;

            if (!podeEstornar)
                throw new Exception(string.Format("{0} já foi {1}.",
                    movimentacao.TipoMov == 1 ? "Este crédito" : "Esta retirada",
                    movimentacao.TipoMov == 1 ? "estornado" : "estornada"));
        }

        #endregion

        #region Estorna a movimentação de crédito/retirada

        /// <summary>
        /// Valida o estorno da movimentação.
        /// </summary>
        public void EstornarCreditoOuRetirada(int idCaixaDiario, string obs)
        {
            lock(EstornarCreditoOuRetiradaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CaixaDiario movimentacao;
                        VerificarEstornoCreditoOuRetiradaJaEfetuado(transaction, idCaixaDiario, out movimentacao);
                        var idContaCaixaGeral =
                            movimentacao.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfParaCxGeralDinheiro) ?
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) :
                                movimentacao.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfParaCxGeralCheque) ?
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque) : 0;
                        var observacao = string.Format("ESTORNO{0}", string.IsNullOrEmpty(obs) ? string.Empty : string.Format(". {0}", obs));

                        // Realiza estorno.
                        MovCaixa(transaction, movimentacao.IdLoja, null, movimentacao.TipoMov == 1 ? 2 : 1, movimentacao.Valor, 0, movimentacao.IdConta,
                            null, movimentacao.FormaSaida.Value, observacao, true, (int)movimentacao.IdCaixaDiario);

                        if (idContaCaixaGeral > 0)
                            CaixaGeralDAO.Instance.MovCxGeral(transaction, movimentacao.IdLoja, null, null, idContaCaixaGeral,
                                movimentacao.TipoMov, movimentacao.FormaSaida.Value, movimentacao.Valor, 0, null, true, observacao, null);

                        if (movimentacao.IdCheque > 0)
                            ChequesDAO.Instance.UpdateSituacao(transaction, movimentacao.IdCheque.Value, Cheques.SituacaoCheque.EmAberto);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Exclui movimentações por PKs

        /// <summary>
        /// Exclui movimentações com as PKs passadas
        /// </summary>
        /// <param name="pks"></param>
        public void DeleteByPKs(string pks)
        {
            if (String.IsNullOrEmpty(pks))
                return;

            string sql = "Delete From caixa_diario Where idCaixaDiario In (" + pks.TrimEnd(',') + ")";

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Verifica se o caixa foi/está fechado

        /// <summary>
        /// Verifica se há movimentações no caixa da loja passada hoje
        /// </summary>
        public bool ExisteMovimentacoes(uint idLoja)
        {
            return ExisteMovimentacoes(null, idLoja);
        }

        /// <summary>
        /// Verifica se há movimentações no caixa da loja passada hoje
        /// </summary>
        public bool ExisteMovimentacoes(GDASession session, uint idLoja)
        {
            // Verifica se há alguma movimentação no cx diario feita hoje
            return objPersistence.ExecuteSqlQueryCount(session, "Select count(*) From caixa_diario c " +
                "Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(current_date) And MONTH(c.DataCad)=MONTH(current_date) " +
                "And YEAR(c.DataCad)=YEAR(current_date) And c.IdLoja=" + idLoja) > 0;
        }

        /// <summary>
        /// Verifica se o caixa já foi fechado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns>True-Caixa Fechado</returns>
        public bool CaixaFechado(uint idLoja)
        {
            return CaixaFechado(null, idLoja);
        }

        /// <summary>
        /// Verifica se o caixa já foi fechado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idLoja"></param>
        /// <returns>True-Caixa Fechado</returns>
        public bool CaixaFechado(GDASession session, uint idLoja)
        {
            // Verifica se o caixa já está fechado
            string sql = "Select count(*) From caixa_diario c " +
                "Where DAYOFMONTH(c.DataCad)=DAYOFMONTH(current_date) " +
                "And MONTH(c.DataCad)=MONTH(current_date) " +
                "And YEAR(c.DataCad)=YEAR(current_date) " +
                "And c.IdLoja=" + idLoja + " And c.IdConta=" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;

        }

        /// <summary>
        /// (Sobrecarga) Verifica se o caixa foi fechado no dia anterior
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns>True-Caixa Fechado</returns>
        public bool CaixaFechadoDiaAnterior(uint idLoja)
        {
            return CaixaFechadoDiaAnterior(null, idLoja);
        }

        /// <summary>
        /// (Sobrecarga) Verifica se o caixa foi fechado no dia anterior
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idLoja"></param>
        /// <returns>True-Caixa Fechado</returns>
        public bool CaixaFechadoDiaAnterior(GDASession sessao, uint idLoja)
        {
            return CaixaFechadoDiaAnterior(sessao, idLoja, ExisteMovimentacoes(idLoja));
        }

        /// <summary>
        /// Verifica se o caixa foi fechado no dia anterior
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="existeMovimentacoes">true se houver movimentações no caixa hoje</param>
        /// <returns>True-Caixa Fechado</returns>
        public bool CaixaFechadoDiaAnterior(uint idLoja, bool existeMovimentacoes)
        {
            return CaixaFechadoDiaAnterior(null, idLoja, existeMovimentacoes);
        }

        /// <summary>
        /// Verifica se o caixa foi fechado no dia anterior
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idLoja"></param>
        /// <returns>True-Caixa Fechado</returns>
        /// <param name="existeMovimentacoes">true se houver movimentações no caixa hoje</param>
        public bool CaixaFechadoDiaAnterior(GDASession sessao, uint idLoja, bool existeMovimentacoes)
        {
            // Se houver movimentações no caixa diário da loja passada hoje, o caixa é dado como fechado
            if (existeMovimentacoes)
                return true;

            // Busca a última movimentação feita no caixa diário anterior à hoje
            uint? obj = ExecuteScalar<uint?>(sessao, "select idConta from caixa_diario where idloja=" + idLoja +
                " order by idcaixadiario desc limit 1");

            if (obj == null)
                return true;

            // Verifica se a última movimentação foi de transf. p/ cx geral, se não tiver sido,
            // significa que o caixa não foi fechado no dia anterior
            return obj == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);
        }

        #endregion

        #region Busca movimentação anterior

        /// <summary>
        /// Busca movimentação anterior
        /// </summary>
        /// <param name="idCaixaDiario"></param>
        /// <returns></returns>
        public CaixaDiario BuscaMovAnterior(uint idCaixaDiario)
        {
            return BuscaMovAnterior(null, idCaixaDiario);
        }

        /// <summary>
        /// Busca movimentação anterior
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCaixaDiario"></param>
        /// <returns></returns>
        public CaixaDiario BuscaMovAnterior(GDASession session, uint idCaixaDiario)
        {
            if (idCaixaDiario == 0)
                return null;
            
            idCaixaDiario = idCaixaDiario - 1;

            while (idCaixaDiario > 0 && objPersistence.ExecuteSqlQueryCount(
                "Select Count(*) From caixa_diario where idCaixaDiario=" + idCaixaDiario) == 0)
                idCaixaDiario--;

            if (idCaixaDiario == 0)
                return null;

            return GetElementByPrimaryKey(idCaixaDiario);
        }

        /// <summary>
        /// Busca o saldo da movimentação anterior
        /// </summary>
        /// <param name="idCaixaDiario"></param>
        /// <returns></returns>
        public decimal ObtemSaldoMovAnterior(uint idCaixaDiario)
        {
            return ObtemSaldoMovAnterior(null, idCaixaDiario);
        }

        /// <summary>
        /// Busca o saldo da movimentação anterior
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCaixaDiario"></param>
        /// <returns></returns>
        public decimal ObtemSaldoMovAnterior(GDASession session, uint idCaixaDiario)
        {
            idCaixaDiario = idCaixaDiario - 1;

            while (idCaixaDiario > 0 && objPersistence.ExecuteSqlQueryCount(
                "Select Count(*) From caixa_diario where idCaixaDiario=" + idCaixaDiario) == 0)
                idCaixaDiario--;

            if (idCaixaDiario == 0)
                return 0;

            return ObtemValorCampo<decimal>("saldo", "idCaixaDiario=" + idCaixaDiario);
        }

        #endregion

        #region Retorna saldo deixado no dia anterior

        /// <summary>
        /// Retorna saldo deixado no dia anterior (Tendo em vista que o caixa do dia anterior foi fechado)
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetSaldoDiaAnterior(uint idLoja)
        {
            return GetSaldoDiaAnterior(null, idLoja);
        }

        /// <summary>
        /// Retorna saldo deixado no dia anterior (Tendo em vista que o caixa do dia anterior foi fechado)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetSaldoDiaAnterior(GDASession session, uint idLoja)
        {
            return GetSaldoDiaAnterior(session, idLoja, null);
        }

        /// <summary>
        /// Retorna saldo deixado no dia anterior
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="data">Será considerado o dia antetior à data informada.</param>
        /// <returns></returns>
        public decimal GetSaldoDiaAnterior(uint idLoja, DateTime? data)
        {
            return GetSaldoDiaAnterior(null, idLoja, data);
        }

        /// <summary>
        /// Retorna saldo deixado no dia anterior
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idLoja"></param>
        /// <param name="data">Será considerado o dia antetior à data informada.</param>
        /// <returns></returns>
        public decimal GetSaldoDiaAnterior(GDASession session, uint idLoja, DateTime? data)
        {
            var sql = @"
                SELECT cd.Saldo FROM caixa_diario cd
                WHERE cd.IdLoja=" + idLoja;

            /* Chamado 15938.
             * O saldo atual estava sendo buscado nas datas onde não haviam movimentações. */
            if (data != null)
                sql += " AND DATE(cd.DataCad)<?data";

            sql += " ORDER BY cd.IdCaixaDiario DESC LIMIT 1";

            // Busca a última movimentação feita no caixa diário anterior à hoje
            return ExecuteScalar<decimal>(session, sql, new GDAParameter("?data", data));
        }

        #endregion

        #region Retorna o último dia que o caixa não foi fechado

        /// <summary>
        /// Retorna o último dia que o caixa não foi fechado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public DateTime GetDataCaixaAberto(uint idLoja)
        {
            // Busca a última movimentação feita no caixa diário anterior à hoje
            return ExecuteScalar<DateTime>("select dataCad from caixa_diario where idloja=" + idLoja +
                " order by idcaixadiario desc limit 1");
        }

        #endregion

        #region Recupera as movimentações por cliente

        /// <summary>
        /// Retorna as movimentações por cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="planosConta">Os planos de conta usados como filtro. Pode ser omitido para não filtrar pelos planos de conta.</param>
        /// <returns></returns>
        public IList<CaixaDiario> GetByCliente(uint idCliente, DateTime inicio, DateTime fim, params uint[] planosConta)
        {
            string sql = @"select * from caixa_diario where idCliente=?idCli and DataCad >= ?inicio and DataCad <= ?fim";

            if (planosConta.Length > 0)
            {
                sql += " and idConta in (";

                foreach (uint p in planosConta)
                    sql += "," + p;

                sql = sql.Replace("(,", "(") + ")";
            }

            return objPersistence.LoadData(sql + " order by dataCad desc", new GDAParameter("?idCli", idCliente),
                new GDAParameter("?inicio", inicio), new GDAParameter("?fim", DateTime.Parse(fim.ToShortDateString() + " 23:59"))).ToList();
        }

        #endregion

        #region Existe movimentação por pedido?

        /// <summary>
        /// Existe movimentação por pedido?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsByPedido(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from caixa_diario where idPedido=" + idPedido +
                " and idConta not in (" + UtilsPlanoConta.ListaEstornosAPrazo() + "," + UtilsPlanoConta.ListaEstornosAVista() + "," +
                UtilsPlanoConta.ListaEstornosSinalPedido() + ")") > 0;
        }

        #endregion

        #region Transferir Cx. Geral

        /// <summary>
        /// Efetua transferência de valor do caixa diário para o caixa geral.
        /// </summary>
        public void TransferirCxGeral(decimal valor, int formaSaida, string obs)
        {
            lock (_movimentarCaixaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        uint idCxDiario;
                        var idLoja = UserInfo.GetUserInfo.IdLoja;
                        var saldo = GetSaldoByLoja(transaction, idLoja);
                        var saldoCaixaDiarioDinheiro = GetSaldoByFormaPagto(transaction, Pagto.FormaPagto.Dinheiro, 0, idLoja, 0, DateTime.Now, 1);
                        var saldoCaixaDiarioCheque = GetSaldoByFormaPagto(transaction, Pagto.FormaPagto.ChequeProprio, 0, idLoja, 0, DateTime.Now, 1);

                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                            throw new Exception("Erro\tApenas funcionário Caixa Diário pode efetuar transferência para o Caixa Geral.");

                        /* Chamado 66573. */
                        if (!CaixaFechadoDiaAnterior(transaction, idLoja))
                            throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                        // Verifica se há saldo para realizar a transferência desejada
                        if (formaSaida == (int)CaixaDiario.FormaSaidaEnum.Dinheiro && saldoCaixaDiarioDinheiro < valor)
                            throw new Exception(string.Format("Não há saldo suficiente para efetuar essa saída. Saldo em dinheiro: {0}.", saldoCaixaDiarioDinheiro.ToString("C")));
                        else if (formaSaida == (int)CaixaDiario.FormaSaidaEnum.Cheque && saldoCaixaDiarioCheque < valor)
                            throw new Exception(string.Format("Não há saldo suficiente para efetuar essa saí­da. Saldo em cheque: {0}.", saldoCaixaDiarioCheque.ToString("C")));

                        if (valor > saldo)
                            throw new Exception("Valor a ser transferido é maior que o saldo disponível no caixa.");

                        // Movimenta caixa geral
                        var idCaixaGeral = CaixaGeralDAO.Instance.MovCxGeral(transaction, null, null, null,
                            UtilsPlanoConta.GetPlanoConta(formaSaida == 1 ? UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro : UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque), 1, formaSaida,
                            valor, 0, null, true, null, DateTime.Now, true, null);

                        if (idCaixaGeral == 0)
                            throw new Exception("Movimentação não foi creditada no caixa geral.");

                        var caixaGeral = CaixaGeralDAO.Instance.GetElementByPrimaryKey(transaction, idCaixaGeral);

                        if (!CaixaGeralDAO.Instance.Exists(transaction, caixaGeral) || caixaGeral.ValorMov != valor)
                            throw new Exception("Movimentação não foi creditada no caixa geral.");

                        var caixa = new CaixaDiario();
                        caixa.IdLoja = idLoja;
                        caixa.TipoMov = 2;
                        caixa.FormaSaida = formaSaida;
                        caixa.IdConta = UtilsPlanoConta.GetPlanoConta(caixa.FormaSaida == 1 ?
                            UtilsPlanoConta.PlanoContas.TransfParaCxGeralDinheiro : UtilsPlanoConta.PlanoContas.TransfParaCxGeralCheque);
                        caixa.Valor = valor;
                        caixa.Saldo = saldo - valor;
                        caixa.Obs = obs;
                        idCxDiario = Insert(transaction, caixa);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("TransferirCxGeral - Valor {0} - FormaSaida {1} - Obs {2} - Funcionario {3}",
                            valor, formaSaida, obs, UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0), ex);

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se existe alguma movimentação feita com o valor passado nos últimos segundos
        /// </summary>
        public bool ExisteMovimentacaoRecente(GDASession session, uint idLoja, uint idConta, decimal valor)
        {
            return ExecuteScalar<bool>(session, "Select Count(*)>0 From caixa_diario Where Valor=?valor and dataCad>=?dataCad and idLoja=?idLoja and idConta=?idConta",
                new GDAParameter("?valor", valor), new GDAParameter("?dataCad", DateTime.Now.AddSeconds(-30)),
                new GDAParameter("?idConta", idConta), new GDAParameter("?idLoja", idLoja));
        }

        #endregion

        #region Retirar Cx. Diário

        /// <summary>
        /// Efetua transferência de valor do caixa diário para o caixa geral.
        /// </summary>
        public void RetirarValor(int idLoja, decimal valor, int? idCheque, int idConta, int formaSaida, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var valorMov = valor;
                    // Busca o saldo do caixa diário, se o saldo for 0 e não houver movimentações no caixa hoje, retorna o saldo do dia anterior
                    var saldo = GetSaldoByLoja(transaction, (uint)idLoja);

                    if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                    {
                        throw new Exception("Apenas funcionário Caixa pode efetuar retirada do caixa.");
                    }
                    
                    if (saldo == 0 && GetMovimentacoes(transaction, (uint)idLoja, 0, DateTime.Now).Length <= 1)
                    {
                        saldo = GetSaldoDiaAnterior(transaction, (uint)idLoja);
                    }

                    // Verifica se o caixa possui saldo para realizar esta retirada
                    if (saldo - valorMov < 0)
                    {
                        throw new Exception("Não há saldo suficiente para realizar esta retirada.");
                    }

                    if (ExisteMovimentacaoRecente(transaction, (uint)idLoja, (uint)idConta, valor))
                    {
                        throw new Exception("Foi feita uma movimentação idêntica nos últimos segundos, verifique se a operação foi efetuada no caixa diário.");
                    }

                    MovCaixa(transaction, (uint)idLoja, null, 2, valorMov, 0, (uint)idConta, null, formaSaida, obs, true, null);

                    if (idCheque > 0)
                    {
                        ChequesDAO.Instance.UpdateSituacao(transaction, (uint)idCheque, Cheques.SituacaoCheque.Compensado);
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("TransferirCxGeral - Valor {0} - FormaSaida {1} - Obs {2} - Funcionario {3}",
                        valor, formaSaida, obs, UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0), ex);

                    throw;
                }
            }
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// APAGAR: Depois da migração
        /// </summary>
        public override uint Insert(CaixaDiario objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession sessao, CaixaDiario objInsert)
        {
            // Verifica se o caixa já foi fechado.
            if (CaixaFechado(sessao, objInsert.IdLoja))
            {
                throw new Exception("O caixa já foi fechado.");
            }

            // Se não houver movimentação feita no caixa de hoje, verifica se o caixa do dia anterior foi fechado e recupera o saldo do dia anterior.
            if (!ExisteMovimentacoes(sessao, objInsert.IdLoja))
            {
                // Verifica se o caixa foi fechado no dia anterior.
                if (!CaixaFechadoDiaAnterior(sessao, objInsert.IdLoja, false))
                {
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");
                }

                // Recupera saldo de dinheiro deixado no dia anterior, se não houver movimentações hoje.
                var saldoRemanescente = GetSaldoDiaAnterior(sessao, objInsert.IdLoja);
                // Insere movimentação com o saldo remanescente, se houver.
                var caixaDiario = new CaixaDiario();

                caixaDiario.IdLoja = objInsert.IdLoja;
                caixaDiario.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.SaldoRemanescente);
                caixaDiario.TipoMov = 1;
                caixaDiario.Saldo = saldoRemanescente;
                caixaDiario.Valor = saldoRemanescente;
                caixaDiario.DataCad = DateTime.Now;

                base.Insert(sessao, caixaDiario);
                
                objInsert.Saldo += saldoRemanescente;
            }

            // Não permite movimentar o caixa de forma que o mesmo fique negativo (esta validação deve ficar aqui, após a inserção de saldo acima).
            if (objInsert.Saldo < 0)
            {
                throw new Exception("Não há saldo suficiente no caixa para esta movimentação.");
            }

            var formaPato = UtilsPlanoConta.GetFormaPagtoByIdConta(objInsert.IdConta);

            // Se for saída de dinheiro, verifica se há saldo em dinheiro suficiente.
            if (objInsert.TipoMov == 2 && 
               (objInsert.FormaSaida == 1 || formaPato.IdFormaPagto == (uint)Pagto.FormaPagto.Dinheiro) && 
               objInsert.Valor > GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Dinheiro, 0, objInsert.IdLoja, 0, DateTime.Now, 1))
            {
                throw new Exception("Não há saldo de dinheiro suficiente para realizar esta retirada.");
            }

            // Se for saída de cheque, verifica se há saldo em cheque suficiente.
            if (objInsert.TipoMov == 2 && 
               (objInsert.FormaSaida == 2 || formaPato.IdFormaPagto == (uint)Pagto.FormaPagto.ChequeProprio) && 
               objInsert.Valor > GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.ChequeProprio, 0, objInsert.IdLoja, 0, DateTime.Now, 1))
            {
                throw new Exception("Não há saldo de cheque suficiente para realizar esta retirada.");
            }

            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            return base.Insert(sessao, objInsert);
        }

        #endregion

        #region Busca movimentações relacionadas a uma devolução de pagamento

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public CaixaDiario[] GetByDevolucaoPagto(uint idDevolucaoPagto)
        {
            return GetByDevolucaoPagto(null, idDevolucaoPagto);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public CaixaDiario[] GetByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            string sql = "Select * From caixa_diario where idDevolucaoPagto=" + idDevolucaoPagto + " And idConta In (" + UtilsPlanoConta.ContasDevolucaoPagto() +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + ") Order By idCaixaDiario Desc";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações por data

        /// <summary>
        /// Obtém as movimentações do caixa geral pela sua data de cadastro.
        /// Método utilizado principalmente para ajustar a tabela pagto_contas_receber.
        /// </summary>
        public IList<CaixaDiario> ObterMovimentacoesPorData(string dataInicio, string dataFim)
        {
            var parametros = new List<GDAParameter>();

            var sql = "SELECT * FROM caixa_diario cd WHERE 1 ";

            if (!string.IsNullOrEmpty(dataInicio))
            {
                sql += " AND cd.DataCad >= ?dataIni";
                parametros.Add(new GDAParameter("?dataIni", DateTime.Parse(dataInicio + " 00:00:00")));
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " AND cd.DataCad <= ?dataFim";
                parametros.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
            }

            return objPersistence.LoadData(sql, parametros.ToArray()).ToList();
        }

        #endregion

        /// <summary>
        /// Associa um cartão não identificado ao caixa geral
        /// </summary>
        public void AssociarCaixaDiarioIdCartaoNaoIdentificado(GDASession sessao, uint idCxDiario, uint idCartaoNaoIdentificado)
        {
            objPersistence.ExecuteCommand(sessao,
                    string.Format("UPDATE caixa_diario SET IdCartaoNaoIdentificado={0} WHERE IdCaixaDiario={1};", idCartaoNaoIdentificado, idCxDiario));
        }

        /// <summary>
        /// recupera movimentações a partir do cartão não identificado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCartaoNaoIdentificado"></param>
        /// <returns></returns>
        public IList<CaixaDiario> ObterMovimentacoesPorCartaoNaoIdentificado(GDASession sessao, int idCartaoNaoIdentificado)
        {
            string sql = @"Select * From caixa_diario where idPedido=null and idLiberarPedido=null and idAcerto=null and idContaR=null and idObra=null and idSinal=null and
                              idTrocaDevolucao=null and idDevolucaoPagto=null and idAcertoCheque=null and IdCartaoNaoIdentificado=" + idCartaoNaoIdentificado;

            return objPersistence.LoadData(sessao, sql).ToList();
        }
    }
}
