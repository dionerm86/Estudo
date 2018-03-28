using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class SinalDAO : BaseDAO<Sinal, SinalDAO>
    {
        //private SinalDAO() { }

        #region Busca sinais do cliente

        private string SqlList(uint idSinal, uint idPedido, uint idCli, uint idFunc, string dataIni, string dataFim, uint idFormaPagto, 
            bool? isPagtoAntecipado, int situacao, bool selecionar)
        {
            string campos = selecionar ? @"s.*, cli.Nome as NomeCliente, l.Telefone as TelefoneLoja, f.Nome as Funcionario" : "count(distinct s.idSinal)";

            string sql = @"
                Select " + campos + @" 
                From sinal s
                    Left Join cliente cli on (s.IdCliente=cli.Id_Cli) 
                    Left Join funcionario f On (s.UsuCad=f.IdFunc)
                    Left Join Loja l on (f.Idloja = l.Idloja)
                Where 1";

            if (idSinal > 0)
                sql += " And s.IdSinal=" + idSinal;

            if (idPedido > 0)
                sql += string.Format(@" AND (IF(s.idsPedidosR IS NOT NULL AND s.idsPedidosR != '', {0} IN (s.idsPedidosR), FALSE) OR
                    s.IdSinal IN (SELECT {1} FROM pedido ped WHERE ped.IdPedido={0}))", idPedido,
                    (isPagtoAntecipado.GetValueOrDefault(false) ? "idPagamentoAntecipado" : "idSinal"));

            if (idCli > 0)
                sql += " And s.IdCliente=" + idCli;

            if (idFunc > 0)
                sql += " And s.usuCad=" + idFunc;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " And s.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " And s.DataCad<=?dataFim";

            if (isPagtoAntecipado != null && idSinal == 0)
                sql += " and (s.isPagtoAntecipado is null or s.isPagtoAntecipado=" + isPagtoAntecipado + ")";

            if (situacao > 0)
                sql += " And s.situacao=" + situacao;

            return sql;
        }

        public void PreencheTotal(ref Sinal[] lstSinal)
        {
            PreencheTotal(null, ref lstSinal);
        }

        public void PreencheTotal(GDASession session, ref Sinal[] lstSinal)
        {
            string sql = "Select cast(Sum(ps.valorPagto) as decimal(12,2)) From pagto_sinal ps Where ps.idSinal=?idSinal Group By ps.idSinal";

            foreach (Sinal s in lstSinal)
            {
                object totalSinal = objPersistence.ExecuteScalar(session, sql.Replace("?idSinal", s.IdSinal.ToString()));

                if (totalSinal != null && totalSinal.ToString() != String.Empty)
                    s.TotalSinal = totalSinal.ToString().StrParaDecimal();
            }
        }

        public void PreencheTotal(ref List<Sinal> lstSinal)
        {
            PreencheTotal(null, ref lstSinal);
        }

        public void PreencheTotal(GDASession session, ref List<Sinal> lstSinal)
        {
            string sql = "Select cast(Sum(ps.valorPagto) as decimal(12,2)) From pagto_sinal ps Where ps.idSinal=?idSinal Group By ps.idSinal";

            foreach (Sinal s in lstSinal)
            {
                object totalSinal = objPersistence.ExecuteScalar(session, sql.Replace("?idSinal", s.IdSinal.ToString()));

                if (totalSinal != null && totalSinal.ToString() != String.Empty)
                    s.TotalSinal = totalSinal.ToString().StrParaDecimal();
            }
        }

        /// <summary>
        /// Retorna todos os acertos do cliente passado
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public Sinal[] GetByCliRpt(uint idCli)
        {
            List<Sinal> lstSinal = objPersistence.LoadData(SqlList(0, 0, idCli, 0, null, null, 0, null, 0, true) + " Order By s.idSinal desc");

            PreencheTotal(ref lstSinal);

            return lstSinal.ToArray();
        }

        public List<Sinal> GetForMovLib(uint idCli, uint idFunc, string dataIni, string dataFim, int situacao, bool isPagtoAntecipado)
        {
            List<Sinal> lstSinal = objPersistence.LoadData(SqlList(0, 0, idCli, idFunc, dataIni, dataFim, 0, isPagtoAntecipado, situacao, true) +
                " Order By s.idSinal desc", GetParam(dataIni, dataFim));

            PreencheTotal(ref lstSinal);

            return lstSinal;
        }

        public Sinal[] GetList(uint idSinal, uint idPedido, uint idCli, string dataIni, string dataFim, uint idFormaPagto, 
            bool isPagtoAntecipado, int ordenacao, string sortExpression, int startRow, int pageSize)
        {
            var filtro = sortExpression;
            if (string.IsNullOrEmpty(filtro))
            {
                switch (ordenacao)
                {
                    case 0: //Nenhum
                        filtro = "s.idSinal desc";
                        break;
                    case 1: //Pedido
                        // Cada Sinal pode ter mais de um IdPedido,
                        // a ordenação só funciona no relatório que filtra por pedido.
                        break;
                    case 2: //Cliente
                        filtro = "s.IdCliente";
                        break;
                    case 3: //Data Recebimento
                        filtro = "s.DataCad";
                        break;
                    default:
                        break;
                }
            }

            var lstSinal = ((List<Sinal>)LoadDataWithSortExpression(SqlList(idSinal, idPedido, idCli, 0, dataIni, dataFim, idFormaPagto, isPagtoAntecipado, 0, true), 
                filtro, startRow, pageSize, GetParam(dataIni, dataFim))).ToArray();

            PreencheTotal(ref lstSinal);

            return lstSinal;
        }

        public int GetCount(uint idSinal, uint idPedido, uint idCli, string dataIni, string dataFim, uint idFormaPagto, bool isPagtoAntecipado, int ordenacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idSinal, idPedido, idCli, 0, dataIni, dataFim, idFormaPagto, isPagtoAntecipado, 0, false), GetParam(dataIni, dataFim));
        }

        public int GetCount(uint idSinal, uint idCli, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idSinal, 0, idCli, 0, dataIni, dataFim, 0, null, 0, false), GetParam(dataIni, dataFim));
        }

        /// <summary>
        /// Busca apenas sinais abertos que tem mais de um pedido vinculado, o motivo disso é que a retificação de sinal apenas remove um 
        /// pedido no qual seu sinal foi pago junto com outro, cancelando o recebimento desse e mantendo o recebimento do outro, se o sinal
        /// possuir apenas um pedido, bastas cancelar o sinal
        /// </summary>
        public IList<Sinal> GetForRetificar(bool pagamentoAntecipado)
        {
            string sql = SqlList(0, 0, 0, 0, null, null, 0, pagamentoAntecipado, (int)Sinal.SituacaoEnum.Aberto, true);
            var nomeReferencia = pagamentoAntecipado ? "idPagamentoAntecipado" : "idSinal";
            return objPersistence.LoadData(sql + " Order by idSinal desc").ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni = dataIni + " 00:00") : DateTime.Parse(dataIni))));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim = dataFim + " 23:59:59") : DateTime.Parse(dataFim))));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca detalhes do sinal

        /// <summary>
        /// Obtém a forma de pagamento do sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public string ObtemFormaPagto(uint idSinal)
        {
            return ObtemFormaPagto(null, idSinal);
        }

        /// <summary>
        /// Obtém a forma de pagamento do sinal
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public string ObtemFormaPagto(GDASession session, uint idSinal)
        {
            string formaPagto = String.Empty;

            // Busca movimentações relacionadas a este sinal e agrupadas pela forma de pagto
            foreach (PagtoSinal ps in PagtoSinalDAO.Instance.GetBySinal(session, idSinal))
                formaPagto += string.Format("{0}: {1:C}{2}\n", ps.DescrFormaPagto, ps.ValorPagto,
                    ps.IdContaBanco.GetValueOrDefault() > 0 ?
                        string.Format(" ({0})", ContaBancoDAO.Instance.GetDescricao(session, ps.IdContaBanco.GetValueOrDefault())) :
                        string.Empty);

            return formaPagto;
        }

        /// <summary>
        /// Retorna dados do Sinal
        /// </summary>
        /// <param name="idSinal"></param>
        public Sinal GetSinalDetails(uint idSinal)
        {
            return GetSinalDetails(null, idSinal);
        }

        /// <summary>
        /// Retorna dados do Sinal
        /// </summary>
        public Sinal GetSinalDetails(GDASession session, uint idSinal)
        {
            // Retorna o acerto, apenas um registro deverá ser retornado
            var lst = objPersistence.LoadData(session, SqlList(idSinal, 0, 0, 0, null, null, 0, null, 0, true)).ToArray();

            PreencheTotal(session, ref lst);

            if (lst.Length > 0)
            {
                lst[0].FormaPagto = ObtemFormaPagto(session, idSinal).TrimEnd('\n');
                return lst[0];
            }
            else
                return null;
        }

        /// <summary>
        /// Retorna todos os pedidos do sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<Pedido> GetPedidosInSinal(uint idSinal)
        {
            PersistenceObject<Pedido> obj = new PersistenceObject<Pedido>(GDA.GDASettings.GetProviderConfiguration("WebGlass"));

            string sql = @"
                Select p.*, c.Nome as NomeCliente, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja 
                From pedido p 
                    Left Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Left Join loja l On (p.IdLoja = l.IdLoja) 
                Where p.idSinal=" + idSinal;

            return obj.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os ids dos pedidos do sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public string ObtemIdsPedidos(uint idSinal)
        {
            var sql = "Select idPedido From pedido Where (Coalesce(idSinal, 0)=" + idSinal + " OR COALESCE(IdPagamentoAntecipado, 0)=" + idSinal + ") Group by idPedido";

            var ids = ExecuteMultipleScalar<string>(sql);

            return ids != null ? string.Join(", ", ids) : string.Empty;
        }

        /// <summary>
        /// Obtém o id do cliente
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idSinal)
        {
            return ObtemIdCliente(null, idSinal);
        }

        /// <summary>
        /// Obtém o id do cliente
        /// </summary>
        public uint ObtemIdCliente(GDASession sessao, uint idSinal)
        {
            string sql = "Select idCliente From sinal Where idSinal=" + idSinal;

            object idCliente = objPersistence.ExecuteScalar(sessao, sql);

            return idCliente != null && idCliente.ToString() != String.Empty ? idCliente.ToString().StrParaUint() : 0;
        }

        #endregion

        #region Verifica se o sinal existe

        /// <summary>
        /// Verifica se o sinal existe.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public new bool Exists(uint idSinal)
        {
            return Exists(null, idSinal);
        }

        /// <summary>
        /// Verifica se o sinal existe.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public new bool Exists(GDASession session, uint idSinal)
        {
            string sql = "select count(*) from sinal where idSinal=" + idSinal;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se o pedido possui algum sinal

        /// <summary>
        /// Verifica se o pedido possui algum sinal
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteSinal(uint idPedido)
        {
            string sql = "Select Count(*) From pedido Where idPedido=" + idPedido + " And idSinal is not null";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Atualiza Número de Autorização Construcard

        /// <summary>
        /// Atualizar o Número de Autorização Construcard
        /// </summary>
        /// <param name="idSinal"></param>
        /// <param name="numAutConstrucard"></param>
        public void AtualizaNumAutConstrucard(uint idSinal, string numAutConstrucard)
        {
            objPersistence.ExecuteCommand("update sinal set numAutConstrucard=?num where idSinal=" + idSinal,
                new GDAParameter("?num", numAutConstrucard));
        }

        #endregion

        #region Sinal do Pedido

        /// <summary>
        /// Valida os pedidos para o pagamento.
        /// </summary>
        public void ValidaSinalPedidos(string idsPedidos, bool isSinal)
        {
            Pedido[] pedidos = PedidoDAO.Instance.GetByString(null, idsPedidos);
            string tipo = isSinal ? "Sinal" : "Pagamento antecipado";

            // Lista de situações possíveis para o recebimento do sinal
            List<Pedido.SituacaoPedido> situacoes = new List<Pedido.SituacaoPedido> {
                Pedido.SituacaoPedido.Conferido, Pedido.SituacaoPedido.AtivoConferencia, Pedido.SituacaoPedido.EmConferencia
            };

            // Se for empresa de confirmação ou se a empresa estiver configurada para receber sinal de pedido ativo,
            // então o recebimento de sinal de pedido ativo é liberado.
            if (!PedidoConfig.LiberarPedido)
                situacoes.Add(Pedido.SituacaoPedido.Ativo);

            // Se a empresa for liberação então o pedido deve estar conferido para receber o sinal.
            if (PedidoConfig.LiberarPedido)
            {
                situacoes.Add(Pedido.SituacaoPedido.ConfirmadoLiberacao);
                situacoes.Add(Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro);
            }

            // Valida os pedidos
            foreach (Pedido ped in pedidos)
            {
                if (ped.Situacao == Pedido.SituacaoPedido.Confirmado || ped.Situacao == Pedido.SituacaoPedido.LiberadoParcialmente)
                    throw new Exception("O pedido " + ped.IdPedido + " já está " + (PedidoConfig.LiberarPedido ? "liberado." : "confirmado."));

                if (isSinal && ped.RecebeuSinal)
                    throw new Exception("Este pedido já possui sinal recebido.");
                else if (!isSinal && ped.IdPagamentoAntecipado.GetValueOrDefault(0) > 0)
                    throw new Exception("Este pedido já possui pagamento antecipado recebido.");

                // Não permite receber sinal de pedidos garantia e reposição
                if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                    throw new Exception("Não é permitido receber sinal de pedidos de garantia.");
                else if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
                    throw new Exception("Não é permitido receber sinal de pedidos de reposição.");
                /* Chamado 16925. */
                else if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                    throw new Exception("Não é permitido receber sinal de pedidos de obra.");

                else if (isSinal && !PedidoDAO.Instance.TemSinalReceber(ped.IdPedido))
                    throw new Exception("Esse pedido não tem sinal a receber.");

                // Se a empresa libera pedidos: só deixa receber o sinal do pedido se o pedido estiver conferido ou confirmado, ou (de acordo com configuração interna) pedido ativo
                // Senão: só deixa receber o sinal do pedido se estiver ativo ou conferido
                // (Antes validava se estava confirmado, porém este bloqueio não pode ser feito pois poderia dar conflito com outra regra
                // existente no sistem de só permitir confirmar pedido se o sinal do mesmo tiver sido recebido)
                else if ((isSinal || FinanceiroConfig.Sinal.BloquearRecebimentoPagtoAntecipadoPedidoAtivo) && !situacoes.Contains(ped.Situacao))
                    throw new Exception("O pedido " + ped.IdPedido + " não está conferido" +
                        (PedidoConfig.LiberarPedido ? " ou confirmado" : " ou ativo") + ".");

                // Verifica se o cliente está ativo
                else if (ClienteDAO.Instance.GetSituacao(ped.IdCli) != (int)SituacaoCliente.Ativo)
                    throw new Exception("O cliente desse pedido está inativo.");

                // Verifica se o pedido possui funcionário
                else if (String.IsNullOrEmpty(PedidoDAO.Instance.ObtemNomeFuncResp(null, ped.IdPedido)))
                    throw new Exception("Este pedido não possui nenhum funcionário associado ao mesmo.");
            }
        }

        private static readonly object _receberLock = new object();

        /// <summary>
        /// Recebe sinal de pedido à prazo
        /// </summary>
        public string Receber(string idsPedidos, string dataRecebido, decimal[] sinais, uint[] formasPagto,
            uint[] idContasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado,
            uint[] tiposCartao, bool gerarCredito, decimal creditoUtilizado, bool cxDiario, string numAutConstrucard,
            uint[] numParcCartoes,
            string chequesPagto, bool descontarComissao, string obs, bool isSinal, string[] numAutCartao)
        {
            lock(_receberLock)
            {
                using (var transaction = new GDATransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var tipo = isSinal ? "Sinal" : "Pagamento antecipado";

                    try
                    {
                        transaction.BeginTransaction();

                        Sinal sinal = null;
                        Pedido[] pedidos = PedidoDAO.Instance.GetByString(transaction, idsPedidos);

                        /* Chamado 36144. */
                        var totalPedidos = pedidos.Sum(f => f.TotalPedidoFluxo);
                        
                        /* Chamados 17870 e 38407. */
                        if (pedidos.Count() > 1 && Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas &&
                            /* Chamado 39027. */
                            FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                            throw new Exception(string.Format("Não é possível receber o {0} de mais de um pedido por vez, pois, o controle de comissão de contas recebidas está habilitado.", tipo));

                        // Verifica se o cheque para pagar o sinal foi cadastrado
                        if (UtilsFinanceiro.ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, formasPagto) &&
                            string.IsNullOrEmpty(chequesPagto))
                            throw new Exception("Cadastre o(s) cheque(s) referente(s) ao " + tipo.ToLower() + " do pedido.");

                        decimal totalSinal = 0;
                        foreach (decimal valor in sinais)
                            totalSinal += valor;

                        // Se for pago com crédito, soma o mesmo ao totalPago
                        if (creditoUtilizado > 0)
                            totalSinal += creditoUtilizado;

                        if (descontarComissao)
                            totalSinal += UtilsFinanceiro.GetValorComissao(transaction, idsPedidos, "Pedido");

                        // Ignora os juros dos cartões ao calcular o valor pago/a pagar
                        totalSinal -= UtilsFinanceiro.GetJurosCartoes(transaction, UserInfo.GetUserInfo.IdLoja, sinais, formasPagto,
                            tiposCartao, numParcCartoes);

                        decimal totalASerPago = 0;
                        uint idLoja = 0;

                        foreach (Pedido ped in pedidos)
                        {
                            if (Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas)
                            {
                                if (idLoja == 0)
                                    idLoja = ped.IdLoja;
                                else if (idLoja != ped.IdLoja)
                                    throw new Exception("Não é possivel fazer o recebimento com pedidos de lojas diferentes");
                            }

                            if (!PedidoConfig.LiberarPedido && ped.TipoVenda == (int)Pedido.TipoVendaPedido.AVista)
                                throw new Exception("O tipo de venda do pedido " + ped.IdPedido +
                                                    " é à vista, para receber um sinal do mesmo altere o tipo de venda para à prazo.");

                            // Verifica se já foi recebido sinal/pagto antecip.
                            if (isSinal && ped.IdSinal > 0)
                                throw new Exception("O sinal do pedido " + ped.IdPedido + " já foi recebido.");

                            if (!isSinal && ped.IdPagamentoAntecipado > 0)
                                throw new Exception("O pedido " + ped.IdPedido + " já possui um pagamento antecipado.");

                            if (isSinal && ped.IdPagamentoAntecipado > 0)
                                throw new Exception("O pedido " + ped.IdPedido + " possui um pagamento antecipado.");

                            if (ped.Situacao == Pedido.SituacaoPedido.Confirmado)
                                throw new Exception("O pedido {0} já foi liberado.");

                            if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                                throw new Exception(string.Format("Não é possível receber {0} de pedidos de produção.", isSinal ? "sinal" : "pagto. antecipado"));

                            // Se for recebimento de sinal o total a ser pago será o valor de entrada do pedido,
                            // Se for pagamento antecipado e o valor do sinal não tenha sido recebido então deve ser considerado o total do pedido, porém,
                            // Se for pagamento antecipado e o valor do sinal tiver sido recebido então o valor a ser pago deve ser o total do pediod menos o valor recebido.
                            totalASerPago += isSinal
                                ? ped.ValorEntrada
                                : (ped.RecebeuSinal ? ped.TotalPedidoFluxo - ped.ValorEntrada : ped.TotalPedidoFluxo);
                        }

                        // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção
                        if (gerarCredito && Math.Round(totalSinal, 2) < Math.Round(totalASerPago, 2))
                            throw new Exception("Valor do " + tipo.ToLower() + " não confere com valor pago. Valor pago: " +
                                                Math.Round(totalSinal, 2).ToString("C") + " Valor do " + tipo + ": " +
                                                Math.Round(totalASerPago, 2).ToString("C"));

                        // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito, apenas para empresas que não liberam pedido
                        else if (!gerarCredito && (!PedidoConfig.LiberarPedido || !isSinal) &&
                                    Math.Round(totalSinal, 2) != Math.Round(totalASerPago, 2))
                            throw new Exception("Valor do " + tipo.ToLower() + " não confere com valor pago. Valor pago: " +
                                                Math.Round(totalSinal, 2).ToString("C") + " Valor do " + tipo + ": " +
                                                Math.Round(totalASerPago, 2).ToString("C"));

                        // Se o total a ser pago for menor que o valor pago, apenas para empresas que liberam pedido
                        else if (!gerarCredito && PedidoConfig.LiberarPedido && isSinal &&
                                    Math.Round(totalSinal, 2) < Math.Round(totalASerPago, 2))
                            throw new Exception("Valor do " + tipo.ToLower() +
                                                " não pode ser menor que o valor pago. Valor pago: " +
                                                Math.Round(totalSinal, 2).ToString("C") + " Valor do " + tipo + ": " +
                                                Math.Round(totalASerPago, 2).ToString("C"));

                        else if (!gerarCredito && totalSinal > totalPedidos)
                            throw new Exception("Valor do " + tipo.ToLower() +
                                                " não pode ser maior que o valor dos pedidos. Valor pago: " +
                                                Math.Round(totalSinal, 2).ToString("C") + " Valor dos pedidos: " +
                                                Math.Round(totalPedidos, 2).ToString("C"));

                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;
                        List<uint> lstIdContaRecSinal = new List<uint>();

                        // Se não for caixa diário, financeiro ou se não tiver permissão para efetuar pagamento antecipado então bloqueia.
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                            throw new Exception("Você não tem permissão para receber " + tipo.ToLower() + ".");

                        sinal = new Sinal(pedidos[0].IdCli);
                        sinal.DataCad = DateTime.Now;
                        sinal.UsuCad = UserInfo.GetUserInfo.CodUser;
                        sinal.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, pedidos[0].IdCli);
                        sinal.IsPagtoAntecipado = !isSinal;
                        sinal.Obs = obs;

                        sinal.IdSinal = Insert(transaction, sinal);

                        retorno = UtilsFinanceiro.Receber(transaction, UserInfo.GetUserInfo.IdLoja, null, sinal, null, null, null,
                            null, null, null, null, null,
                            idsPedidos, sinal.IdCliente, 0, null, dataRecebido,
                            Math.Max(totalASerPago, !gerarCredito ? totalSinal : 0), totalSinal, sinais,
                            formasPagto, idContasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao, null, null, 0, false,
                            gerarCredito, creditoUtilizado, numAutConstrucard,
                            cxDiario, numParcCartoes, chequesPagto, descontarComissao, UtilsFinanceiro.TipoReceb.SinalPedido);

                        if (retorno.ex != null)
                            throw retorno.ex;

                        #region Salva o recebimento de sinal

                        int numPagto = 1;

                        // Caso tenha  sido utilizado crédito é necessário salvar na tabela pagto_sinal o valor recebido do mesmo, com o idFormaPagto igual à zero.
                        for (int i = 0; i <= sinais.Length; i++)
                        {
                            if (i < sinais.Length)
                            {
                                if (sinais[i] == 0)
                                    continue;
                            }
                            // Se não tiver utilizado crédito e todos as formas de pagamento tiverem sido salvas, sai do loop.
                            else if (creditoUtilizado == 0)
                                break;

                            if (formasPagto.Length > i && formasPagto[i] == (int)Data.Model.Pagto.FormaPagto.CartaoNaoIdentificado)
                            {
                                var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                                foreach (var cni in CNIs)
                                {
                                    var pagto = new PagtoSinal();
                                    pagto.IdSinal = sinal.IdSinal;
                                    pagto.NumFormaPagto = numPagto++;

                                    // Caso todas as forma de pagamento tiverem sido salvas, salva o valor utilizado de crédito.
                                    pagto.ValorPagto = cni.Valor;

                                    pagto.IdFormaPagto =
                                        formasPagto.Length > i ? formasPagto[i] :
                                            (uint)Pagto.FormaPagto.Credito;

                                    pagto.IdContaBanco = (uint)cni.IdContaBanco;
                                    pagto.IdTipoCartao = (uint)cni.TipoCartao;
                                    pagto.NumAutCartao = cni.NumAutCartao;

                                    PagtoSinalDAO.Instance.Insert(transaction, pagto);
                                }
                            }
                            else
                            {
                                var pagto = new PagtoSinal();
                                pagto.IdSinal = sinal.IdSinal;
                                pagto.NumFormaPagto = numPagto++;

                                // Caso todas as forma de pagamento tiverem sido salvas, salva o valor utilizado de crédito.
                                pagto.ValorPagto =
                                    sinais.Length > i ? sinais[i] :
                                        creditoUtilizado;

                                pagto.IdFormaPagto =
                                    formasPagto.Length > i ? formasPagto[i] :
                                        (uint)Pagto.FormaPagto.Credito;

                                pagto.IdContaBanco =
                                    sinais.Length == i ? null :
                                        idContasBanco[i] > 0 ? idContasBanco[i] :
                                            depositoNaoIdentificado[i] > 0 ? (uint?)DepositoNaoIdentificadoDAO.Instance.ObtemIdContaBanco(transaction, (int)depositoNaoIdentificado[i]) :
                                                null;

                                pagto.IdTipoCartao =
                                    tiposCartao.Length > i ? (tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null) :
                                        null;

                                pagto.NumAutCartao =
                                    numAutCartao.Length > i ? numAutCartao[i] :
                                        null;

                                // Se for depósito não identificado, preenche a conta bancária no pagto sinal com o banco do depósito
                                if (sinais.Length != i && depositoNaoIdentificado != null && depositoNaoIdentificado.Length > 0 && depositoNaoIdentificado[i] > 0 && pagto.IdContaBanco.GetValueOrDefault() == 0)
                                    pagto.IdContaBanco = DepositoNaoIdentificadoDAO.Instance.ObtemValorCampo<uint>(transaction, "IdContaBanco", "IdDepositoNaoIdentificado=" + depositoNaoIdentificado[i]);

                                PagtoSinalDAO.Instance.Insert(transaction, pagto);
                            }
                        }

                        #endregion

                        #region Recalcula o valor da entrada de cada pedido, recalculando também suas parcelas

                        if (isSinal && !gerarCredito && totalSinal > totalASerPago)
                        {
                            // Calcula o valor e o percentual a mais de sinal que ser� rateado entre os pedidos
                            decimal totalRatear = Math.Round(totalSinal - totalASerPago, 2);
                            decimal percRatear = totalRatear / totalSinal;
                            decimal totalRateado = 0;

                            for (int i = 0; i < pedidos.Length; i++)
                            {
                                Pedido ped = PedidoDAO.Instance.GetElement(transaction, pedidos.OrderBy(f => f.Total).ToList()[i].IdPedido);

                                if (i < (pedidos.Length - 1))
                                {
                                    // Acrescenta ao valor da entrada o percentual do rateio sobre o valor do pedido ou o restante do valor da entrada que 
                                    // complete o total do pedido, o que for menor dos dois, para que o valor da entrada n�o fique maior que o valor do pedido
                                    var novoValor = Math.Round(Math.Min(ped.Total * percRatear, ped.Total - ped.ValorEntrada), 2);

                                    totalRateado += novoValor;
                                    ped.ValorEntrada += novoValor;
                                }
                                else
                                    ped.ValorEntrada = Math.Round(ped.ValorEntrada + totalRatear - totalRateado, 2);

                                PedidoDAO.Instance.RecalculaParcelas(transaction, ref ped, PedidoDAO.TipoCalculoParcelas.Valor);

                                PedidoDAO.Instance.UpdateBase(transaction, ped);
                            }
                        }

                        #endregion

                        // Marca pedido como tendo recebido sinal e NumAutConstrucard se houver
                        RecebeuSinal(transaction, sinal.IdSinal, idsPedidos, numAutConstrucard, isSinal);

                        sinal.CreditoGeradoCriar = retorno.creditoGerado;
                        sinal.CreditoUtilizadoCriar = creditoUtilizado;
                        Update(transaction, sinal);

                        #region Gera conta recebida referente ao recebimento do sinal

                        var numeroParcelaContaPagar = 0;

                        for (int i = 0; i < sinais.Length; i++)
                        {
                            if (sinais[i] <= 0)
                                continue;

                            ContasReceber contaRecSinal = new ContasReceber();
                            contaRecSinal.IdLoja = Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas
                                ? idLoja
                                : UserInfo.GetUserInfo.IdLoja;
                            contaRecSinal.IdSinal = sinal.IdSinal;
                            contaRecSinal.IdCliente = pedidos[0].IdCli;
                            contaRecSinal.IdConta = UtilsPlanoConta.GetPlanoSinal(formasPagto[i]);
                            contaRecSinal.DataVec = DateTime.Now;
                            contaRecSinal.ValorVec = sinais[i];
                            contaRecSinal.DataRec = string.IsNullOrEmpty(dataRecebido) ? DateTime.Now : Convert.ToDateTime(dataRecebido);
                            contaRecSinal.ValorRec = sinais[i];
                            contaRecSinal.Recebida = true;
                            contaRecSinal.UsuRec = UserInfo.GetUserInfo.CodUser;
                            contaRecSinal.NumParc = 1;
                            contaRecSinal.NumParcMax = 1;
                            contaRecSinal.Usucad = UserInfo.GetUserInfo.CodUser;
                            contaRecSinal.IdFuncComissaoRec = contaRecSinal.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(contaRecSinal.IdCliente) : null;

                            var idContaR = ContasReceberDAO.Instance.Insert(transaction, contaRecSinal);
                            lstIdContaRecSinal.Add(idContaR);

                            if (formasPagto[i] == (uint)Pagto.FormaPagto.Cartao)
                                numeroParcelaContaPagar = ContasReceberDAO.Instance.AtualizarReferenciaContasCartao(transaction, retorno, numParcCartoes, numeroParcelaContaPagar, i, idContaR);

                            #region Salva o pagamento da conta

                            if (formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                            {
                                var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                                foreach (var cni in CNIs)
                                {
                                    var pagto = new PagtoContasReceber();

                                    pagto.IdContaR = idContaR;
                                    pagto.IdFormaPagto = formasPagto[i];
                                    pagto.ValorPagto = cni.Valor;
                                    pagto.IdContaBanco = (uint)cni.IdContaBanco;
                                    pagto.IdTipoCartao = (uint)cni.TipoCartao;
                                    pagto.NumAutCartao = cni.NumAutCartao;

                                    PagtoContasReceberDAO.Instance.Insert(transaction, pagto);
                                }
                            }
                            else
                            {
                                var pagto = new PagtoContasReceber();

                                pagto.IdContaR = idContaR;
                                pagto.IdFormaPagto = formasPagto[i];
                                pagto.ValorPagto = sinais[i];

                                pagto.IdContaBanco = formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro &&
                                                     idContasBanco[i] > 0
                                    ? (uint?)idContasBanco[i]
                                    : null;
                                pagto.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;
                                pagto.IdDepositoNaoIdentificado = depositoNaoIdentificado[i] > 0
                                    ? (uint?)depositoNaoIdentificado[i]
                                    : null;
                                pagto.NumAutCartao = numAutCartao[i];

                                PagtoContasReceberDAO.Instance.Insert(transaction, pagto);
                            }

                            #endregion
                        }

                        if (creditoUtilizado > 0)
                        {
                            ContasReceber contaRecSinal = new ContasReceber();
                            contaRecSinal.IdLoja = Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas
                                ? idLoja
                                : UserInfo.GetUserInfo.IdLoja;
                            contaRecSinal.IdSinal = sinal.IdSinal;
                            contaRecSinal.IdCliente = pedidos[0].IdCli;
                            contaRecSinal.IdConta =
                                UtilsPlanoConta.GetPlanoSinal((uint)Glass.Data.Model.Pagto.FormaPagto.Credito);
                            contaRecSinal.DataVec = DateTime.Now;
                            contaRecSinal.ValorVec = creditoUtilizado;
                            contaRecSinal.DataRec = DateTime.Now;
                            contaRecSinal.ValorRec = creditoUtilizado;
                            contaRecSinal.Recebida = true;
                            contaRecSinal.UsuRec = UserInfo.GetUserInfo.CodUser;
                            contaRecSinal.NumParc = 1;
                            contaRecSinal.NumParcMax = 1;
                            contaRecSinal.Usucad = UserInfo.GetUserInfo.CodUser;
                            contaRecSinal.IdFuncComissaoRec = contaRecSinal.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(contaRecSinal.IdCliente) : null;

                            var idContaR = ContasReceberDAO.Instance.Insert(transaction, contaRecSinal);
                            lstIdContaRecSinal.Add(idContaR);

                            #region Salva o pagamento da conta

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = idContaR;
                            pagto.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito;
                            pagto.ValorPagto = creditoUtilizado;

                            PagtoContasReceberDAO.Instance.Insert(transaction, pagto);

                            #endregion
                        }

                        #endregion

                        foreach (Pedido ped in pedidos)
                        {
                            // Muda a situação para confirmado liberação caso seja pagto antecip. e não possua vidros para produção.
                            if (PedidoConfig.LiberarPedido && !PedidoDAO.Instance.PossuiVidrosProducao(transaction, ped.IdPedido))
                            {
                                try
                                {
                                    // Não é necessário colocar transação aqui
                                    // É mecessário colocar transação neste método sim, fizemos um teste de recebimento de pagamento antecipado,
                                    // conforme informado no chamado 15463, e ocorreu o erro de "lock wait timeout...".
                                    string idsPedidoOk = String.Empty, idsPedidoErro = String.Empty;
                                    PedidoDAO.Instance.ConfirmarLiberacaoPedido(transaction, ped.IdPedido.ToString(),
                                        out idsPedidoOk, out idsPedidoErro, false);
                                }
                                catch
                                {
                                }
                            }
                            // Muda a situação para AtivoConferencia caso a situação atual seja Ativo ou EmConferencia
                            else if (ped.Situacao == Pedido.SituacaoPedido.Ativo ||
                                     ped.Situacao == Pedido.SituacaoPedido.EmConferencia)
                                PedidoDAO.Instance.AlteraSituacao(transaction, ped.IdPedido,
                                    Pedido.SituacaoPedido.AtivoConferencia);

                            #region Gera a comissão do pedido

                            if (descontarComissao)
                                ComissaoDAO.Instance.GerarComissao(transaction, Pedido.TipoComissao.Comissionado,
                                    ped.IdComissionado.Value, ped.IdPedido.ToString(), ped.DataConf.Value.ToString(),
                                    ped.DataConf.Value.ToString(), 0, null);

                            #endregion

                            // Se for pagamento antecipado, não permite que haja pedido com valorPagamentoAntecipado zerado
                            if (!isSinal && PedidoDAO.Instance.ObtemValorPagtoAntecipado(transaction, ped.IdPedido) == 0)
                                throw new Exception("Não foi possível receber o pedido " + ped.IdPedido +
                                                    ", refaça a operação.");
                        }

                        var msg = tipo + " recebido. ";

                        if (retorno != null)
                        {
                            if (retorno.creditoGerado > 0)
                                msg += "Foi gerado " + retorno.creditoGerado.ToString("C") + " de crédito para o cliente. ";

                            if (retorno.creditoDebitado)
                                msg += "Foi utilizado " + creditoUtilizado.ToString("C") +
                                       " de crédito do cliente, restando " +
                                       ClienteDAO.Instance.GetCredito(transaction, sinal.IdCliente).ToString("C") +
                                       " de crédito. ";
                        }

                        msg += "\t" + (sinal != null && sinal.IdSinal > 0 ? sinal.IdSinal : 0);

                        #region Calcula o saldo devedor

                        decimal saldoDevedor;
                        decimal saldoCredito;

                        ClienteDAO.Instance.ObterSaldoDevedor(transaction, sinal.IdCliente, out saldoDevedor, out saldoCredito);

                        var sqlUpdate = @"UPDATE sinal SET SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito WHERE IdSinal = {0}";
                        objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, sinal.IdSinal), new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                        #endregion

                        transaction.Commit();
                        transaction.Close();

                        return msg;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("SinalDAO - Receber", ex);
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber " + tipo.ToLower() + ".", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Marca pedido como tendo recebido sinal
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="isSinal">O recebimento é de sinal? (Se não for o pagamento é antecipado)</param>
        private void RecebeuSinal(GDASession sessao, uint idSinal, string idsPedidos, string numAutConstrucard, bool isSinal)
        {
            string adicional = "";
            if (!String.IsNullOrEmpty(numAutConstrucard))
                adicional += "p.numAutConstrucard=?numAut, ";

            adicional = !isSinal ? "p.idPagamentoAntecipado=" + idSinal + @", p.valorPagamentoAntecipado=If(Coalesce(pe.total,0)>0, pe.total, p.total) -
                If(p.idSinal > 0, p.valorEntrada, 0)" : "p.idSinal=" + idSinal;

            objPersistence.ExecuteCommand(sessao, "Update pedido p Left Join pedido_espelho pe On (p.idPedido=pe.idPedido) Set " + adicional +
                " Where p.idPedido In (" + idsPedidos + ")", new GDAParameter("?numAut", numAutConstrucard));
        }

        #endregion

        #region Cancelamento de sinal

        private static readonly object _cancelarLock = new object();

        /// <summary>
        /// Cancela o recebimento de sinal de um pedido.
        /// </summary>
        public void CancelarComTransacao(uint idSinal, uint? idPedido, bool confirmandoPedido, 
            bool gerarCredito, string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCreditoEstorno)
        {
            lock(_cancelarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Cancelar(transaction, idSinal, idPedido, confirmandoPedido, gerarCredito, motivo, dataEstornoBanco, cancelamentoErroTef, gerarCreditoEstorno);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Cancela o recebimento de sinal de um pedido.
        /// </summary>
        public void Cancelar(GDASession session, uint idSinal, uint? idPedido, bool confirmandoPedido, bool gerarCredito,
            string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCreditoEstorno)
        {
            Sinal sinal = GetSinalDetails(session, idSinal);
            Pedido[] pedidos = PedidoDAO.Instance.GetBySinal(session, idSinal, sinal.IsPagtoAntecipado);
            var tipo = sinal.IsPagtoAntecipado ? "pagamento antecipado" : "sinal";

            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.CancelarRecebimentos))
                throw new Exception(string.Format("Você não tem permissão para cancelar {0}, favor contactar o administrador", sinal.IsPagtoAntecipado ? "pagamento antecipado" : "sinal"));

            // Verifica se algum dos cheques deste sinal já foi utilizando em um depósito
            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From cheques c Where c.idSinal=" + idSinal + " And c.idDeposito>0"))
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram depositados, cancele ou retifique o depósito antes de cancelar este sinal.");

            // Verifica se algum dos cheques deste sinal já foi utilizando em um pagamento
            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From pagto_cheque pc Where pc.idCheque In " +
                "(Select c.idCheque From cheques c Where c.idSinal=" + idSinal + ")"))
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em pagamentos, cancele ou retifique-os antes de cancelar este sinal.");

            // Chamado 12032. Verifica se algum dos cheques deste sinal já foi utilizando em um pagamento de crédito para fornecedor.
            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From cheques c Where c.idSinal=" + idSinal + " And c.idCreditoFornecedor>0"))
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em pagamento de crédito para fornecedor," +
                    " cancele ou retifique-os antes de cancelar este sinal.");

            // Chamado 12032. Verifica se algum dos cheques deste sinal já foi utilizando em um sinal de compra.
            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From cheques c Where c.idSinal=" + idSinal + " And c.idSinalCompra>0"))
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em sinal de compra," +
                    " cancele ou retifique-os antes de cancelar este sinal.");

            // Verifica se existe alguma parcela de cartão aberta para este sinal
            if (ExecuteScalar<bool>($"SELECT COUNT(*)>0 FROM contas_receber WHERE IsParcelaCartao=true AND RECEBIDA=true AND IdSinal={ idSinal }"))
                throw new Exception("Existem parcelas de cartão quitadas geradas a partir deste sinal, cancele a quitação das mesmas antes de cancelar este sinal.");

            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From cheques c Where c.IdSinal=" + idSinal + " And Situacao > 1"))
                throw new Exception(@"Um ou mais cheques recebidos já foram utilizados em outras transações, cancele ou retifique as transações dos cheques antes de cancelar este sinal.");

            // Verifica se o pedido já está confirmado
            foreach (Pedido ped in pedidos)
            {
                if (!PedidoConfig.LiberarPedido && !confirmandoPedido && PedidoDAO.Instance.IsPedidoConfirmado(session, ped.IdPedido))
                    throw new Exception("O " + tipo.ToLower() + " deste pedido não pode ser cancelado pois ele já está confirmado.");

                // Verifica se o sinal/pagamento antecipado já foi cancelado.
                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where " + (!sinal.IsPagtoAntecipado ? "idSinal" : "idPagamentoAntecipado") +
                    " Is Not Null And idPedido=" + ped.IdPedido) == 0)
                    throw new Exception("O " + tipo.ToLower() + " do pedido " + ped.IdPedido + " já foi cancelado.");

                // Caso o pedido possua pagamento antecipado o mesmo deve ser cancelado antes de cancelar o sinal.
                if (!sinal.IsPagtoAntecipado)
                    if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where idPagamentoAntecipado > 0 And idPedido=" + ped.IdPedido) > 0)
                        throw new Exception("Cancele o pagamento antecipado do pedido " + ped.IdPedido + " antes de cancelar o sinal do mesmo.");

                // Se a empresa libera os pedidos: não cancela o sinal se há uma conferência no PCP
                if (PedidoConfig.LiberarPedido)
                {
                    // Se o pedido possuir espelho não pode cancelar o sinal, uma vez que o sinal só pode efetuado 
                    // se o pedido estiver na situação conferido COM (se for pagamento antes da produção)
                    if (PedidoConfig.ImpedirConfirmacaoPedidoPagamento &&
                        PedidoEspelhoDAO.Instance.ExisteEspelho(session, ped.IdPedido) &&
                        ClienteDAO.Instance.IsPagamentoAntesProducao(session, ped.IdCli))
                        throw new Exception("O " + tipo.ToLower() + " do pedido " + ped.IdPedido + " não pode ser cancelado porque há uma conferência para ele.");
                    else if (!LiberarPedidoDAO.Instance.PodeCancelarPedido(session, ped.IdPedido))
                        throw new Exception("O " + tipo.ToLower() + " do pedido " + ped.IdPedido + " não pode ser cancelado porque já existe uma liberação para ele.");
                }
            }

            // Salva os pedidos, valores e cheques
            string idsPedidos = "", valores = "", cheques = "";
            foreach (Pedido ped in pedidos)
            {
                idsPedidos += ped.IdPedido + ",";
                valores += (!sinal.IsPagtoAntecipado ? ped.ValorEntrada : ped.ValorPagamentoAntecipado).
                    ToString("0.00").Replace(",", ".") + ",";
            }

            foreach (Cheques c in ChequesDAO.Instance.GetBySinal(session, idSinal))
                cheques += c.IdCheque + ",";

            if (!gerarCredito)
            {
                UtilsFinanceiro.CancelaRecebimento(session, UtilsFinanceiro.TipoReceb.SinalPedido, null, sinal,
                    null, null, null, 0, null, null, null, null, dataEstornoBanco, cancelamentoErroTef, gerarCreditoEstorno);

                var contasRec = ContasReceberDAO.Instance.GetRecebidasBySinal(session, idSinal);

                //Remove o pagamento das contas recebidas
                PagtoContasReceberDAO.Instance.DeleteByIdContaR(session, string.Join(",", contasRec.Select(f => f.IdContaR.ToString()).ToArray()));

                // Indica nos pedidos que o sinal não foi recebido
                objPersistence.ExecuteCommand(session, "Update pedido Set " + (!sinal.IsPagtoAntecipado ? "idSinal" : "valorPagamentoAntecipado=Null, idPagamentoAntecipado") + "=Null Where " +
                            (!sinal.IsPagtoAntecipado ? "idSinal=" : "idPagamentoAntecipado=") + idSinal);
            }
            else
            {
                decimal valorEstornoCredito = idPedido > 0 && pedidos.Length > 1 ? PedidoDAO.Instance.ObtemValorEntrada(session, idPedido.Value) : sinal.TotalSinal;

                // Gera movimentação de crédito do total do sinal
                CaixaGeralDAO.Instance.MovCxSinal(session, idSinal, sinal.IdCliente,
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, valorEstornoCredito, 0, null, false, null, null);

                ClienteDAO.Instance.CreditaCredito(session, sinal.IdCliente, valorEstornoCredito);
            }

            LogCancelamentoDAO.Instance.LogSinal(session, sinal, motivo, true);

            // Atualiza no sinal os ids dos pedidos, dos cheques e os valores pagos
            sinal.IdsPedidosR = idsPedidos.TrimEnd(',');
            sinal.ValoresR = valores.TrimEnd(',');
            sinal.IdsChequesR = cheques.TrimEnd(',');
            sinal.Situacao = gerarCredito && idPedido > 0 && pedidos.Length > 1 ? (int)Sinal.SituacaoEnum.Aberto : (int)Sinal.SituacaoEnum.Cancelado;
            Update(session, sinal);
        }

        #endregion

        #region Retificação de sinal

        private static readonly object _retificarLock = new object();

        /// <summary>
        /// Redefine a data das movimentações bancárias geradas pelo sinal
        /// </summary>
        public void RetificarDataMovBanco(uint idSinal, DateTime novaData)
        {
            if(idSinal == 0 || novaData == null)
                throw new Exception(@"Não foi possível recuperar o sinal e/ou nova data para redefinição.");

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var sinal =  GetElementByPrimaryKey(transaction, idSinal);
                    var pagtos = PagtoSinalDAO.Instance.GetBySinal(transaction, idSinal);
                    var lstMovBanco = MovBancoDAO.Instance.GetBySinal(transaction, sinal.IdSinal);

                    if (pagtos.Any(f => 
                            f.IdFormaPagto != (uint)Pagto.FormaPagto.Deposito &&
                            f.IdFormaPagto != (uint)Pagto.FormaPagto.Boleto &&
                            f.IdFormaPagto != (uint)Pagto.FormaPagto.Cartao &&
                            f.IdFormaPagto != (uint)Pagto.FormaPagto.Construcard))
                        throw new Exception(@"Não é possível retificar a data deste pagamento 
                                              antecipado pois o mesmo possui formas de pagamento 
                                              que não geram movimentações bancárias.");

                    foreach (var item in lstMovBanco)
                    {
                        item.DataMov = novaData;
                        MovBancoDAO.Instance.AtualizaDataObs(transaction, item, false);
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw new Exception(@"Não é possível retificar a data deste pagamento antecipado.", e);
                }
            }
        }

        /// <summary>
        /// Retifica um sinal (ou pagamento antecipado), removendo alguns pedidos e gerando o crédito da diferença.
        /// </summary>
        public void RetificaSinal(uint idSinal, string idsPedidosRemover, bool cxDiario)
        {
            lock(_retificarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var isSinal = false;
                        List<Pedido> ped = null;

                        if (idsPedidosRemover == null || string.IsNullOrEmpty((idsPedidosRemover = idsPedidosRemover.Trim(',', ' '))))
                            return;

                        if (ClienteDAO.Instance.IsConsumidorFinal(transaction, ObtemIdCliente(transaction, idSinal)))
                            throw new Exception("Não é possível retificar sinal/pagto. antecipado de consumidor final, já que não é possível controlar o crédito do mesmo por não ser conhecido o cliente.");

                        // Verifica se o sinal/pagto. antecipado possui mais de um pedido associado
                        if (ExecuteScalar<bool>(transaction, "SELECT COUNT(*) <= 1 FROM pedido WHERE COALESCE(IdSinal, 0)=" + idSinal + " OR COALESCE(IdPagamentoAntecipado, 0)=" + idSinal))
                            throw new Exception("Não é possível retificar sinal ou pagto. antecipado que possua somente um pedido.");

                        // Variáveis de controle
                        isSinal = !IsPagtoAntecipado(transaction, idSinal);
                        var tipo = isSinal ? "sinal" : "pagamento antecipado";

                        // Recupera os pedidos do sinal (para restauração em caso de erro)
                        ped = new List<Pedido>(PedidoDAO.Instance.GetBySinal(transaction, idSinal, !isSinal));

                        foreach (var id in idsPedidosRemover.TrimEnd(',', ' ').Split(','))
                        {
                            var idPedidoRemover = Glass.Conversoes.StrParaUint(id);

                            // Verifica se o pedido ainda está neste sinal/pagamento antecipado
                            if (!ped.Exists(f => f.IdPedido == idPedidoRemover))
                                throw new Exception("Um ou mais pedidos já foram removidos deste " + (isSinal ? "sinal" : "pagamento antecipado") + ".");

                            // Não permite que o sinal de um pedido seja cancelado caso o pedido possua pagamento antecipado.
                            if (isSinal && PedidoDAO.Instance.ObtemIdPagamentoAntecipado(transaction, idPedidoRemover) > 0)
                                throw new Exception("Cancele o pagamento antecipado do pedido " + id + " antes de cancelar o sinal do mesmo.");

                            if (PedidoDAO.Instance.ObtemSituacao(idPedidoRemover) == Pedido.SituacaoPedido.Cancelado)
                                throw new Exception(string.Format("Não é permitido remover pedidos cancelados do sinal. Pedido {0}", idPedidoRemover));

                            // Se a empresa libera os pedidos: não cancela o sinal se há uma conferência no PCP
                            if (PedidoConfig.LiberarPedido)
                            {
                                // Se o pedido possuir espelho não pode cancelar o sinal, uma vez que o sinal só pode efetuado 
                                // se o pedido estiver na situação conferido COM (se for pagamento antes da produção)
                                if (PedidoConfig.ImpedirConfirmacaoPedidoPagamento && PedidoEspelhoDAO.Instance.ExisteEspelho(transaction, idPedidoRemover) &&
                                    ClienteDAO.Instance.IsPagamentoAntesProducao(transaction, ped[0].IdCli))
                                    throw new Exception("O " + tipo.ToLower() + " do pedido " + idPedidoRemover + " não pode ser cancelado porque há uma conferência para ele.");
                                else if (!LiberarPedidoDAO.Instance.PodeCancelarPedido(transaction, idPedidoRemover))
                                    throw new Exception("O " + tipo.ToLower() + " do pedido " + idPedidoRemover + " não pode ser cancelado porque já existe uma liberação para ele.");
                            }
                        }
                        // Recupera o valor do crédito que será gerado para o cliente
                        decimal valorCredito = ExecuteScalar<decimal>(transaction, "Select Sum(" + (isSinal ? "valorEntrada" : "valorPagamentoAntecipado") +
                            ") From pedido Where idPedido In (" + idsPedidosRemover + ") And " + (isSinal ? "idSinal=" : "idPagamentoAntecipado=") + idSinal);

                        // Remove a referência do sinal dos pedidos removidos
                        objPersistence.ExecuteCommand(transaction, "Update pedido Set " + (isSinal ? "idSinal=Null" : "idPagamentoAntecipado=Null, valorPagamentoAntecipado=Null") +
                            " Where idPedido In (" + idsPedidosRemover + ") And " + (isSinal ? "idSinal=" : "idPagamentoAntecipado=") + idSinal);

                        // Recupera o cliente e variáveis de controle adicionais
                        var idCliente = ObtemIdCliente(transaction, idSinal);

                        // Credita o valor dos pedidos já pagos ao crédito do cliente
                        ClienteDAO.Instance.CreditaCredito(transaction, idCliente, valorCredito);

                        if (cxDiario)
                        {
                            CaixaDiarioDAO.Instance.MovCxSinal(transaction, UserInfo.GetUserInfo.IdLoja, idCliente, idSinal, 1, valorCredito, 0,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado),
                                null, "Retificação do " + (isSinal ? "Sinal" : "Pagto. Antecipado"), false);
                        }
                        else
                        {
                            // Faz uma movimentação no caixa geral para que o crédito entre na movimentação de crédito do cliente
                            CaixaGeralDAO.Instance.MovCxSinal(transaction, idSinal, idCliente,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado),
                                1, valorCredito, 0, null, false, null, "Retificação do " + (isSinal ? "Sinal" : "Pagto. Antecipado"));
                        }


                        Dictionary<int, decimal> valoresPago = new Dictionary<int, decimal>();
                        string obs = "";

                        // Recupera a observação original do sinal (para restauração).
                        obs = ObtemValorCampo<string>(transaction, "obs", "idSinal=" + idSinal);
                        string dados = "Pedidos Removidos: " + idsPedidosRemover + " / Valor de Crédito Gerado: " + valorCredito.ToString("C");

                        // Salva os dados da retificação na observação do sinal.
                        objPersistence.ExecuteCommand(transaction, "Update sinal Set obs=Trim(Concat(Coalesce(obs, ''), ?obs)) Where idSinal=" + idSinal,
                            new GDAParameter("?obs", " - " + dados + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));

                        decimal valorRetificar = valorCredito;

                        foreach (PagtoSinal ps in PagtoSinalDAO.Instance.GetBySinal(transaction, idSinal))
                        {
                            // Recupera o valor pago do sinal (para restauração).
                            if (!valoresPago.ContainsKey(ps.NumFormaPagto))
                                valoresPago.Add(ps.NumFormaPagto, 0);

                            valoresPago[ps.NumFormaPagto] += ps.ValorPagto;

                            if (valorRetificar > ps.ValorPagto)
                                // Atualiza o valor na tabela pagto_Sinal.
                                objPersistence.ExecuteCommand(transaction, "Update pagto_sinal Set valorPagto=valorPagto-" + ps.ValorPagto.ToString().Replace(',', '.') +
                                    " Where idSinal=" + idSinal + " And numFormaPagto=" + ps.NumFormaPagto);
                            else
                            {
                                // Atualiza o valor na tabela pagto_Sinal.
                                objPersistence.ExecuteCommand(transaction, "Update pagto_sinal Set valorPagto=valorPagto-" + valorRetificar.ToString().Replace(',', '.') +
                                    " Where idSinal=" + idSinal + " And numFormaPagto=" + ps.NumFormaPagto);

                                valorRetificar -= valorRetificar;
                                break;
                            }

                            valorRetificar -= ps.ValorPagto;
                        }

                        #region Calcula o saldo devedor

                        decimal saldoDevedor;
                        decimal saldoCredito;

                        ClienteDAO.Instance.ObterSaldoDevedor(transaction, idCliente, out saldoDevedor, out saldoCredito);

                        var sqlUpdate = @"UPDATE sinal SET SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito WHERE IdSinal = {0}";
                        objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, idSinal), new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                        #endregion

                        // Cria um log com a alteração.
                        Sinal s = GetElementByPrimaryKey(transaction, idSinal);
                        s.DadosRetificar = dados;
                        LogAlteracaoDAO.Instance.LogSinal(transaction, s);
                        
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

        #region Verifica se é pagamento antecipado

        /// <summary>
        /// Verifica se é pagamento antecipado.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public bool IsPagtoAntecipado(uint idSinal)
        {
            return IsPagtoAntecipado(null, idSinal);
        }

        /// <summary>
        /// Verifica se é pagamento antecipado.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public bool IsPagtoAntecipado(GDASession session, uint idSinal)
        {
            string sql = "SELECT COUNT(*) FROM sinal WHERE COALESCE(IsPagtoAntecipado, FALSE)=TRUE AND IdSinal=" + idSinal;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Recupera a string para referência nas models

        /// <summary>
        /// Recupera a string para referência nas models.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        internal string GetReferencia(uint idSinal)
        {
            return (!IsPagtoAntecipado(idSinal) ? "Sinal: " : "Pagto. Antecipado: ") + idSinal;
        }

        #endregion
    }
}