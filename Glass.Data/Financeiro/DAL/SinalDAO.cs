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

        /// <summary>
        /// Busca a situação do sinal.
        /// </summary>
        public Sinal.SituacaoEnum ObterSituacao(GDASession session, int idSinal)
        {
            return ObtemValorCampo<Sinal.SituacaoEnum>(session, "Situacao", string.Format("IdSinal={0}", idSinal));
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

        #region Validação dos pedidos para recebimento de sinal/pagamento antecipado
        
        /// <summary>
        /// Valida os pedidos para o pagamento.
        /// </summary>
        public void ValidaSinalPedidos(string idsPedidos, bool isSinal)
        {
            var pedidos = PedidoDAO.Instance.GetByString(null, idsPedidos);
            var situacoes = new List<Pedido.SituacaoPedido> { Pedido.SituacaoPedido.Conferido, Pedido.SituacaoPedido.AtivoConferencia, Pedido.SituacaoPedido.EmConferencia };

            // Se for empresa de confirmação ou se a empresa estiver configurada para receber sinal de pedido ativo,
            // então o recebimento de sinal de pedido ativo é liberado.
            if (!PedidoConfig.LiberarPedido)
            {
                situacoes.Add(Pedido.SituacaoPedido.Ativo);
            }

            // Se a empresa for liberação então o pedido deve estar conferido para receber o sinal.
            if (PedidoConfig.LiberarPedido)
            {
                situacoes.Add(Pedido.SituacaoPedido.ConfirmadoLiberacao);
                situacoes.Add(Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro);
            }

            // Valida os pedidos.
            foreach (var pedido in pedidos)
            {
                if (pedido.Situacao == Pedido.SituacaoPedido.Confirmado || pedido.Situacao == Pedido.SituacaoPedido.LiberadoParcialmente)
                {
                    throw new Exception(string.Format("O pedido {0} já está {1}.", pedido.IdPedido, PedidoConfig.LiberarPedido ? "liberado" : "confirmado"));
                }

                if (isSinal && pedido.RecebeuSinal)
                {
                    throw new Exception("Este pedido já possui sinal recebido.");
                }
                else if (!isSinal && pedido.IdPagamentoAntecipado.GetValueOrDefault(0) > 0)
                {
                    throw new Exception("Este pedido já possui pagamento antecipado recebido.");
                }

                // Não permite receber sinal de pedidos garantia e reposição.
                if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                {
                    throw new Exception("Não é permitido receber sinal de pedidos de garantia.");
                }
                else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
                {
                    throw new Exception("Não é permitido receber sinal de pedidos de reposição.");
                }
                /* Chamado 16925. */
                else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                {
                    throw new Exception("Não é permitido receber sinal de pedidos de obra.");
                }
                else if (isSinal && !PedidoDAO.Instance.TemSinalReceber(pedido.IdPedido))
                {
                    throw new Exception("Esse pedido não tem sinal a receber.");
                }
                // Se a empresa libera pedidos: só deixa receber o sinal do pedido se o pedido estiver conferido ou confirmado, ou (de acordo com configuração interna) pedido ativo
                // Senão: só deixa receber o sinal do pedido se estiver ativo ou conferido
                // (Antes validava se estava confirmado, porém este bloqueio não pode ser feito pois poderia dar conflito com outra regra
                // existente no sistem de só permitir confirmar pedido se o sinal do mesmo tiver sido recebido)
                else if ((isSinal || FinanceiroConfig.Sinal.BloquearRecebimentoPagtoAntecipadoPedidoAtivo) && !situacoes.Contains(pedido.Situacao))
                {
                    throw new Exception(string.Format("O pedido {0} não está conferido{1}.", pedido.IdPedido, PedidoConfig.LiberarPedido ? " ou confirmado" : " ou ativo"));
                }
                // Verifica se o cliente está ativo.
                else if (ClienteDAO.Instance.GetSituacao(null, pedido.IdCli) != (int)SituacaoCliente.Ativo)
                {
                    throw new Exception("O cliente desse pedido está inativo.");
                }
                // Verifica se o pedido possui funcionário.
                else if (string.IsNullOrWhiteSpace(PedidoDAO.Instance.ObtemNomeFuncResp(null, pedido.IdPedido)))
                {
                    throw new Exception("Este pedido não possui nenhum funcionário associado ao mesmo.");
                }
            }
        }

        #endregion

        #region Atualização de sinal/pagamento antecipado recebido no pedido

        /// <summary>
        /// Marca pedido como tendo recebido sinal
        /// </summary>
        private void RecebeuSinal(GDASession sessao, int idSinal, IEnumerable<int> idsPedido, bool isSinal, string numAutConstrucard)
        {
            var filtro = string.Empty;

            if (!string.IsNullOrEmpty(numAutConstrucard))
            {
                filtro += "p.NumAutConstrucard=?numAut, ";
            }

            filtro = !isSinal ?
                string.Format("p.IdPagamentoAntecipado={0}, p.ValorPagamentoAntecipado=IF(COALESCE(pe.Total,0) > 0, pe.Total, p.Total) - IF(p.IdSinal > 0, p.ValorEntrada, 0)", idSinal) :
                string.Format("p.IdSinal={0}", idSinal);

            objPersistence.ExecuteCommand(sessao, string.Format(@"UPDATE pedido p LEFT JOIN pedido_espelho pe ON (p.IdPedido=pe.IdPedido) SET {0} Where p.idPedido In ({1})",
                filtro, string.Join(",", idsPedido)), new GDAParameter("?numAut", numAutConstrucard));
        }

        #endregion

        #region Sinal/pagamento antecipado do pedido

        /// <summary>
        /// Efetua o recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public string ReceberSinalPagamentoAntecipado(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento,
            IEnumerable<int> idsPedido, IEnumerable<int> idsTipoCartao, bool isSinal, string numeroAutorizacaoConstrucard, IEnumerable<string> numerosAutorizacaoCartao, string observacao,
            IEnumerable<int> quantidadesParcelaCartao, IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var idSinal = CriarPreRecebimentoSinalPagamentoAntecipado(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsPedido, idsTipoCartao, isSinal, numeroAutorizacaoConstrucard,
                        numerosAutorizacaoCartao, observacao, quantidadesParcelaCartao, valoresRecebimento);

                    var retorno = FinalizarPreRecebimentoSinalPagamentoAntecipado(transaction, idSinal);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("ReceberSinal - IDs pedido: {0}.", string.Join(", ", idsPedido)), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao efetuar o recebimento.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public int CriarPreRecebimentoSinalPagamentoAntecipadoComTransacao(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento,
            bool descontarComissao, bool gerarCredito, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsPedido, IEnumerable<int> idsTipoCartao, bool isSinal, string numeroAutorizacaoConstrucard,
            IEnumerable<string> numerosAutorizacaoCartao, string observacao, IEnumerable<int> quantidadesParcelaCartao, IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = CriarPreRecebimentoSinalPagamentoAntecipado(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsPedido, idsTipoCartao, isSinal, numeroAutorizacaoConstrucard,
                        numerosAutorizacaoCartao, observacao, quantidadesParcelaCartao, valoresRecebimento);

                    TransacaoCapptaTefDAO.Instance.Insert(transaction, new TransacaoCapptaTef()
                    {
                        IdReferencia = retorno,
                        TipoRecebimento = UtilsFinanceiro.TipoReceb.SinalPedido
                    });

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarPreRecebimentoSinalComTransacao - IDs pedido: {0}.", string.Join(", ", idsPedido)), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao efetuar o recebimento.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public int CriarPreRecebimentoSinalPagamentoAntecipado(GDASession session, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento,
            bool descontarComissao, bool gerarCredito, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsPedido, IEnumerable<int> idsTipoCartao, bool isSinal, string numeroAutorizacaoConstrucard,
            IEnumerable<string> numerosAutorizacaoCartao, string observacao, IEnumerable<int> quantidadesParcelaCartao, IEnumerable<decimal> valoresRecebimento)
        {
            #region Declaração de variáveis

            var usuarioLogado = UserInfo.GetUserInfo;
            var pedidos = PedidoDAO.Instance.GetByString(session, string.Join(",", idsPedido));
            var sinal = new Sinal(pedidos[0].IdCli);
            var contadorPagamento = 1;
            var idLoja = Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas ? (int?)pedidos.ElementAtOrDefault(0)?.IdLoja ?? 0 : (int)usuarioLogado.IdLoja;
            decimal totalPagar = 0;
            decimal totalPago = 0;

            #endregion

            #region Cálculo dos totais do sinal/pagamento antecipado

            totalPago = valoresRecebimento.Sum(f => f) + (creditoUtilizado > 0 ? creditoUtilizado : 0);

            if (descontarComissao)
            {
                totalPago += UtilsFinanceiro.GetValorComissao(session, string.Join(",", idsPedido), "Pedido");
            }

            // Ignora os juros dos cartões ao calcular o valor pago/a pagar.
            totalPago -= UtilsFinanceiro.GetJurosCartoes(session, (uint)idLoja, valoresRecebimento.ToArray(), idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

            // Se for recebimento de sinal o total a ser pago será o valor de entrada do pedido,
            // Se for pagamento antecipado e o valor do sinal não tenha sido recebido então deve ser considerado o total do pedido, porém,
            // Se for pagamento antecipado e o valor do sinal tiver sido recebido então o valor a ser pago deve ser o total do pediod menos o valor recebido.
            totalPagar += pedidos.Sum(f => isSinal ? f.ValorEntrada : (f.RecebeuSinal ? f.TotalPedidoFluxo - f.ValorEntrada : f.TotalPedidoFluxo));

            #endregion

            #region Atualização dos dados do sinal/pagamento antecipado

            sinal.DataCad = DateTime.Now;
            sinal.UsuCad = usuarioLogado.CodUser;
            sinal.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(session, pedidos[0].IdCli);
            sinal.IsPagtoAntecipado = !isSinal;
            sinal.Obs = observacao;
            sinal.DataRecebimento = dataRecebimento;
            sinal.TotalPagar = Math.Max(totalPagar, !gerarCredito ? totalPago : 0);
            sinal.TotalPago = totalPago;
            sinal.IdLojaRecebimento = idLoja;
            sinal.DescontarComissao = descontarComissao;
            sinal.RecebimentoCaixaDiario = caixaDiario;
            sinal.RecebimentoGerarCredito = gerarCredito;
            sinal.NumAutConstrucard = numeroAutorizacaoConstrucard;

            #endregion

            #region Validações do pré recebimento de sinal/pagamento antecipado

            ValidarRecebimentoSinalPagamentoAntecipado(session, dadosChequesRecebimento, idsCartaoNaoIdentificado, idsContaBanco, idsFormaPagamento, pedidos, sinal);

            #endregion

            #region Inserção do sinal/pagamento antecipado

            sinal.Situacao = (int)Sinal.SituacaoEnum.Processando;
            sinal.IdSinal = Insert(session, sinal);

            #endregion

            #region Inserção dos dados do recebimento do sinal/pagamento antecipado

            ChequesSinalDAO.Instance.InserirPelaString(session, sinal, dadosChequesRecebimento);

            // Caso tenha  sido utilizado crédito é necessário salvar na tabela pagto_sinal o valor recebido do mesmo, com o idFormaPagto igual à zero.
            for (var i = 0; i <= valoresRecebimento.Count(); i++)
            {
                if (i < valoresRecebimento.Count())
                {
                    if (valoresRecebimento.ElementAtOrDefault(i) == 0)
                    {
                        continue;
                    }
                }
                // Se não tiver utilizado crédito e todos as formas de pagamento tiverem sido salvas, sai do loop.
                else if (creditoUtilizado == 0)
                {
                    break;
                }

                if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAtOrDefault(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoSinal = new PagtoSinal();
                        pagamentoSinal.IdSinal = sinal.IdSinal;
                        pagamentoSinal.NumFormaPagto = contadorPagamento++;
                        // Caso todas as forma de pagamento tiverem sido salvas, salva o valor utilizado de crédito.
                        pagamentoSinal.ValorPagto = cartaoNaoIdentificado.Valor;
                        pagamentoSinal.IdFormaPagto = idsFormaPagamento.Count() > i ? (uint)idsFormaPagamento.ElementAt(i) : (uint)Pagto.FormaPagto.Credito;
                        pagamentoSinal.IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco;
                        pagamentoSinal.IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado;
                        pagamentoSinal.IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao;
                        pagamentoSinal.NumAutCartao = cartaoNaoIdentificado.NumAutCartao;

                        PagtoSinalDAO.Instance.Insert(session, pagamentoSinal);
                    }
                }
                else
                {
                    var pagamentoSinal = new PagtoSinal();
                    pagamentoSinal.IdSinal = sinal.IdSinal;
                    pagamentoSinal.NumFormaPagto = contadorPagamento++;
                    // Caso todas as forma de pagamento tiverem sido salvas, salva o valor utilizado de crédito.
                    pagamentoSinal.ValorPagto = valoresRecebimento.Count() > i ? valoresRecebimento.ElementAt(i) : creditoUtilizado;
                    pagamentoSinal.IdFormaPagto = idsFormaPagamento.Count() > i ? (uint)idsFormaPagamento.ElementAt(i) : (uint)Pagto.FormaPagto.Credito;
                    pagamentoSinal.IdContaBanco = valoresRecebimento.Count() == i ? null : idsContaBanco.ElementAtOrDefault(i) > 0 ? (uint?)idsContaBanco.ElementAt(i) :
                    // Se for depósito não identificado, preenche a conta bancária no pagto sinal com o banco do depósito.
                        idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (uint?)DepositoNaoIdentificadoDAO.Instance.ObtemIdContaBanco(session, (int)idsDepositoNaoIdentificado.ElementAt(i)) : null;
                    pagamentoSinal.IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (int?)idsDepositoNaoIdentificado.ElementAt(i) : null;
                    pagamentoSinal.QuantidadeParcelaCartao = quantidadesParcelaCartao.ElementAtOrDefault(i) > 0 ? (int?)quantidadesParcelaCartao.ElementAt(i) : null;
                    pagamentoSinal.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint?)idsTipoCartao.ElementAt(i) : null;
                    pagamentoSinal.NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAtOrDefault(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null;

                    PagtoSinalDAO.Instance.Insert(session, pagamentoSinal);
                }
            }

            #endregion

            #region Atualização do dados do pedido

            // Marca pedido como tendo recebido sinal e NumAutConstrucard se houver.
            RecebeuSinal(session, (int)sinal.IdSinal, idsPedido, isSinal, numeroAutorizacaoConstrucard);

            #endregion

            return (int)sinal.IdSinal;
        }

        /// <summary>
        /// Valida o recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public void ValidarRecebimentoSinalPagamentoAntecipado(GDASession session, IEnumerable<string> dadosChequesRecebimento, IEnumerable<int> idsCartaoNaoIdentificado,
            IEnumerable<int> idsContaBanco, IEnumerable<int> idsFormaPagamento, IEnumerable<Pedido> pedidos, Sinal sinal)
        {
            #region Declaração de variáveis

            var tipoRecebimento = sinal.IsPagtoAntecipado ? "Pagamento antecipado" : "Sinal";
            var totalPedidos = pedidos.Sum(f => f.TotalPedidoFluxo);

            #endregion

            #region Validações de permissão

            // Se não for caixa diário, financeiro ou se não tiver permissão para efetuar pagamento antecipado então bloqueia.
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception(string.Format("Você não tem permissão para receber {0}.", tipoRecebimento.ToLower()));
            }

            // Apenas administrador, financeiro geral e financeiro pagto podem gerar comissões.
            if (sinal.DescontarComissao.GetValueOrDefault() && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
            {
                throw new Exception("Você não tem permissão para gerar comissões");
            }

            // Chamados 17870, 38407 e Chamado 39027.
            if (pedidos.Count() > 1 && Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas && FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                throw new Exception(string.Format("Não é possível receber o {0} de mais de um pedido por vez, pois, o controle de comissão de contas recebidas está habilitado.",
                    tipoRecebimento.ToLower()));
            }

            #endregion

            #region Validações dos dados dos pedidos

            foreach (var pedido in pedidos)
            {
                if (Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas && pedido.IdLoja != pedidos.ElementAtOrDefault(0)?.IdLoja)
                {
                    throw new Exception(string.Format("Não é possivel receber o {0} de pedidos de lojas diferentes", tipoRecebimento));
                }

                if (!PedidoConfig.LiberarPedido && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.AVista)
                {
                    throw new Exception(string.Format("O tipo de venda do pedido {0} é à vista, para receber um sinal do mesmo altere o tipo de venda para à prazo.", pedido.IdPedido));
                }
                
                if (!sinal.IsPagtoAntecipado && pedido.IdSinal > 0)
                {
                    throw new Exception(string.Format("O sinal do pedido {0} já foi recebido.", pedido.IdPedido));
                }

                if (sinal.IsPagtoAntecipado && pedido.IdPagamentoAntecipado > 0)
                {
                    throw new Exception(string.Format("O pedido {0} já possui um pagamento antecipado.", pedido.IdPedido));
                }

                if (!sinal.IsPagtoAntecipado && pedido.IdPagamentoAntecipado > 0)
                {
                    throw new Exception(string.Format("O pedido {0} possui um pagamento antecipado.", pedido.IdPedido));
                }

                if (pedido.Situacao == Pedido.SituacaoPedido.Confirmado)
                {
                    throw new Exception(string.Format("O pedido {0} já foi liberado.", pedido.IdPedido));
                }

                if (pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                {
                    throw new Exception(string.Format("Não é possível receber {0} de pedidos de produção.", sinal.IsPagtoAntecipado ? "pagto. antecipado" : "sinal"));
                }
            }

            #endregion

            #region Validações do valor do recebimento

            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção.
            if (sinal.RecebimentoGerarCredito.GetValueOrDefault() && Math.Round(sinal.TotalPago.GetValueOrDefault(), 2) < Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2))
            {
                throw new Exception(string.Format("Valor do {0} não confere com valor pago. Valor pago: {1} Valor do {2}: {3}.",
                    tipoRecebimento.ToLower(), Math.Round(sinal.TotalPago.GetValueOrDefault(), 2).ToString("C"), tipoRecebimento, Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2).ToString("C")));
            }
            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito, apenas para empresas que não liberam pedido.
            else if (!sinal.RecebimentoGerarCredito.GetValueOrDefault() && (!PedidoConfig.LiberarPedido || sinal.IsPagtoAntecipado) &&
                Math.Round(sinal.TotalPago.GetValueOrDefault(), 2) != Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2))
            {
                throw new Exception(string.Format("Valor do {0} não confere com valor pago. Valor pago: {1} Valor do {2}: {3}.",
                    tipoRecebimento.ToLower(), Math.Round(sinal.TotalPago.GetValueOrDefault(), 2).ToString("C"), tipoRecebimento, Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2).ToString("C")));
            }
            // Se o total a ser pago for menor que o valor pago, apenas para empresas que liberam pedido.
            else if (!sinal.RecebimentoGerarCredito.GetValueOrDefault() && PedidoConfig.LiberarPedido && !sinal.IsPagtoAntecipado &&
                Math.Round(sinal.TotalPago.GetValueOrDefault(), 2) < Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2))
            {
                throw new Exception(string.Format("Valor do {0} não pode ser menor que o valor pago. Valor pago: {1} Valor do {2}: {3}.",
                    tipoRecebimento.ToLower(), Math.Round(sinal.TotalPago.GetValueOrDefault(), 2).ToString("C"), tipoRecebimento, Math.Round(sinal.TotalPagar.GetValueOrDefault(), 2).ToString("C")));
            }
            else if (!sinal.RecebimentoGerarCredito.GetValueOrDefault() && sinal.TotalPago.GetValueOrDefault() > totalPedidos)
            {
                throw new Exception(string.Format("Valor do {0} não pode ser maior que o valor dos pedidos. Valor pago: {1} Valor dos pedidos: {2}",
                    tipoRecebimento.ToLower(), Math.Round(sinal.TotalPago.GetValueOrDefault(), 2).ToString("C"), Math.Round(totalPedidos, 2).ToString("C")));
            }

            #endregion

            #region Validações do recebimento

            // Verifica se o cheque para pagar o sinal foi cadastrado.
            if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray()) &&
                (dadosChequesRecebimento == null || dadosChequesRecebimento.Count() == 0))
            {
                throw new Exception(string.Format("Cadastre o(s) cheque(s) referente(s) ao {0} do pedido.", tipoRecebimento.ToLower()));
            }

            UtilsFinanceiro.ValidarRecebimento(session, sinal.RecebimentoCaixaDiario.GetValueOrDefault(), (int)sinal.IdCliente, sinal.IdLojaRecebimento.GetValueOrDefault(), idsCartaoNaoIdentificado,
                idsContaBanco, idsFormaPagamento, sinal.RecebimentoGerarCredito.GetValueOrDefault(), 0, false, UtilsFinanceiro.TipoReceb.SinalPedido, sinal.TotalPago.GetValueOrDefault(),
                sinal.TotalPagar.GetValueOrDefault());

            #endregion
        }

        /// <summary>
        /// Finaliza o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public string FinalizarPreRecebimentoSinalPagamentoAntecipadoComTransacao(int idSinal)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = FinalizarPreRecebimentoSinalPagamentoAntecipado(transaction, idSinal);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FinalizarPreRecebimentoSinalComTransacao - ID sinal: {0}.", idSinal), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao finalizar o recebimento.", ex));
                }
            }
        }

        /// <summary>
        /// Finaliza o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public string FinalizarPreRecebimentoSinalPagamentoAntecipado(GDASession session, int idSinal)
        {
            #region Declaração de variáveis

            UtilsFinanceiro.DadosRecebimento retorno = null;
            var sinal = Instance.GetElementByPrimaryKey(session, idSinal);
            var tipoRecebimento = sinal.IsPagtoAntecipado ? "Pagamento antecipado" : "Sinal";
            var pedidos = PedidoDAO.Instance.GetBySinal(session, sinal.IdSinal, sinal.IsPagtoAntecipado);
            var usuarioLogado = UserInfo.GetUserInfo;
            var totalPedidos = pedidos.Sum(f => f.TotalPedidoFluxo);
            var totalPagar = pedidos.Sum(f =>  !sinal.IsPagtoAntecipado ? f.ValorEntrada : (f.RecebeuSinal ? f.TotalPedidoFluxo - f.ValorEntrada : f.TotalPedidoFluxo));
            var totalPago = sinal.TotalPago.GetValueOrDefault();
            var dataRecebimento = sinal.DataRecebimento.GetValueOrDefault(DateTime.Now).ToString("dd/MM/yyyy");
            var idLojaRecebimento = (uint)sinal.IdLojaRecebimento.GetValueOrDefault();
            var descontarComissao = sinal.DescontarComissao.GetValueOrDefault();
            var recebimentoCaixaDiario = sinal.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoGerarCredito = sinal.RecebimentoGerarCredito.GetValueOrDefault();
            decimal saldoDevedor;
            decimal saldoCredito;
            var idsPedidoConfirmados = string.Empty;
            var idsPedidoNaoConfirmados = string.Empty;
            // Recupera os cheques que foram selecionados no momento do recebimento do sinal.
            var chequesRecebimento = ChequesSinalDAO.Instance.ObterStringChequesPeloSinal(session, (int)sinal.IdSinal);
            var pagamentosSinal = PagtoSinalDAO.Instance.GetBySinal(session, sinal.IdSinal);
            // Variáveis criadas para recuperar os dados do pagamento da liberação.
            var idsCartaoNaoIdentificado = new List<int?>();
            var idsContaBanco = new List<int?>();
            var idsDepositoNaoIdentificado = new List<int?>();
            var idsFormaPagamento = new List<int?>();
            var idsTipoCartao = new List<int?>();
            var numerosAutorizacaoCartao = new List<string>();
            var quantidadesParcelaCartao = new List<int?>();
            var valoresRecebimento = new List<decimal?>();
            decimal creditoUtilizado = 0;
            var numeroParcelaContaReceber = 0;

            #endregion

            #region Recuperação dos dados de recebimento do sinal

            if (pagamentosSinal.Any(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
            {
                foreach (var pagamentoSinal in pagamentosSinal.Where(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra)
                    .OrderBy(f => f.NumFormaPagto))
                {
                    idsCartaoNaoIdentificado.Add(pagamentoSinal.IdCartaoNaoIdentificado);
                    idsContaBanco.Add((int?)pagamentoSinal.IdContaBanco);
                    idsDepositoNaoIdentificado.Add(pagamentoSinal.IdDepositoNaoIdentificado);
                    idsFormaPagamento.Add((int?)pagamentoSinal.IdFormaPagto);
                    idsTipoCartao.Add(((int?)pagamentoSinal.IdTipoCartao));
                    numerosAutorizacaoCartao.Add(pagamentoSinal.NumAutCartao);
                    quantidadesParcelaCartao.Add(pagamentoSinal.QuantidadeParcelaCartao);
                    valoresRecebimento.Add(pagamentoSinal.ValorPagto);
                }
            }

            if (pagamentosSinal.Any(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito))
            {
                creditoUtilizado = pagamentosSinal.FirstOrDefault(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito)?.ValorPagto ?? 0;
            }

            #endregion

            #region Recebimento do sinal/pagamento antecipado

            retorno = UtilsFinanceiro.Receber(session, idLojaRecebimento, null, sinal, null, null, null, null, null, null, null, null, string.Join(",", pedidos.Select(f => f.IdPedido).ToList()),
                sinal.IdCliente, 0, null, dataRecebimento, Math.Max(totalPagar, !recebimentoGerarCredito ? totalPago : 0), totalPago, valoresRecebimento.Select(f => f.GetValueOrDefault()).ToArray(),
                idsFormaPagamento.Select(f => (uint)f.GetValueOrDefault()).ToArray(), idsContaBanco.Select(f => (uint)f.GetValueOrDefault()).ToArray(),
                idsDepositoNaoIdentificado.Select(f => (uint)f.GetValueOrDefault()).ToArray(), idsCartaoNaoIdentificado.Select(f => (uint)f.GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => (uint)f.GetValueOrDefault()).ToArray(), null, null, 0, false, recebimentoGerarCredito, creditoUtilizado, sinal.NumAutConstrucard, recebimentoCaixaDiario,
                quantidadesParcelaCartao.Select(f => (uint)f.GetValueOrDefault()).ToArray(), chequesRecebimento, descontarComissao, UtilsFinanceiro.TipoReceb.SinalPedido);

            if (retorno.ex != null)
            {
                throw retorno.ex;
            }

            #endregion

            #region Recálculo do valor da entrada de cada pedido, recalculando também suas parcelas

            if (!sinal.IsPagtoAntecipado && !recebimentoGerarCredito && totalPago > totalPagar)
            {
                // Calcula o valor e o percentual a mais de sinal que ser� rateado entre os pedidos
                var totalRatear = Math.Round(totalPago - totalPagar, 2);
                var percentualRatear = totalRatear / totalPago;
                decimal totalRateado = 0;

                for (var i = 0; i < pedidos.Length; i++)
                {
                    var pedido = PedidoDAO.Instance.GetElement(session, pedidos.OrderBy(f => f.Total).ToList()[i].IdPedido);

                    if (i < (pedidos.Length - 1))
                    {
                        // Acrescenta ao valor da entrada o percentual do rateio sobre o valor do pedido ou o restante do valor da entrada que 
                        // complete o total do pedido, o que for menor dos dois, para que o valor da entrada n�o fique maior que o valor do pedido
                        var novoValor = Math.Round(Math.Min(pedido.Total * percentualRatear, pedido.Total - pedido.ValorEntrada), 2);

                        totalRateado += novoValor;
                        pedido.ValorEntrada += novoValor;
                    }
                    else
                    {
                        pedido.ValorEntrada = Math.Round(pedido.ValorEntrada + totalRatear - totalRateado, 2);
                    }

                    PedidoDAO.Instance.RecalculaParcelas(session, ref pedido, PedidoDAO.TipoCalculoParcelas.Valor);
                    PedidoDAO.Instance.UpdateBase(session, pedido);
                }
            }

            #endregion

            #region Atualização dos dados do sinal/pagamento antecipado

            sinal.CreditoGeradoCriar = retorno.creditoGerado;
            sinal.CreditoUtilizadoCriar = creditoUtilizado;
            sinal.Situacao = (int)Sinal.SituacaoEnum.Aberto;

            Update(session, sinal);

            #endregion

            #region Geração da conta recebida referente ao recebimento do sinal
            
            for (var i = 0; i < valoresRecebimento.Count(); i++)
            {
                if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresRecebimento.ElementAtOrDefault(i) == 0)
                {
                    continue;
                }

                var contaRecebidaSinal = new ContasReceber();
                contaRecebidaSinal.IdLoja = idLojaRecebimento;
                contaRecebidaSinal.IdSinal = sinal.IdSinal;
                contaRecebidaSinal.IdCliente = pedidos[0].IdCli;
                contaRecebidaSinal.IdConta = UtilsPlanoConta.GetPlanoSinal((uint)idsFormaPagamento.ElementAtOrDefault(i));
                contaRecebidaSinal.DataVec = DateTime.Now;
                contaRecebidaSinal.ValorVec = valoresRecebimento.ElementAtOrDefault(i).GetValueOrDefault();
                contaRecebidaSinal.DataRec = DateTime.Now;
                contaRecebidaSinal.ValorRec = valoresRecebimento.ElementAtOrDefault(i).GetValueOrDefault();
                contaRecebidaSinal.Recebida = true;
                contaRecebidaSinal.UsuRec = usuarioLogado.CodUser;
                contaRecebidaSinal.NumParc = 1;
                contaRecebidaSinal.NumParcMax = 1;
                contaRecebidaSinal.Usucad = usuarioLogado.CodUser;
                contaRecebidaSinal.IdFuncComissaoRec = contaRecebidaSinal.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(session, contaRecebidaSinal.IdCliente) : null;

                var idContaR = ContasReceberDAO.Instance.InsertBase(session, contaRecebidaSinal);

                if (idsFormaPagamento.ElementAt(i) == (uint)Pagto.FormaPagto.Cartao)
                {
                    numeroParcelaContaReceber = ContasReceberDAO.Instance.AtualizarReferenciaContasCartao(session, retorno, quantidadesParcelaCartao.Select(f => f.GetValueOrDefault()),
                        numeroParcelaContaReceber, i, idContaR);
                }

                #region Salva o pagamento da conta

                if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAtOrDefault(i) == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => (uint)f.GetValueOrDefault()).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoContaReceber = new PagtoContasReceber();
                        pagamentoContaReceber.IdContaR = idContaR;
                        pagamentoContaReceber.IdFormaPagto = (uint)idsFormaPagamento.ElementAtOrDefault(i);
                        pagamentoContaReceber.ValorPagto = cartaoNaoIdentificado.Valor;
                        pagamentoContaReceber.IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco;
                        pagamentoContaReceber.IdCartaoNaoIdentificado = idsCartaoNaoIdentificado.ElementAtOrDefault(i) > 0 ? idsCartaoNaoIdentificado.ElementAt(i) : null;
                        pagamentoContaReceber.IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao;
                        pagamentoContaReceber.NumAutCartao = cartaoNaoIdentificado.NumAutCartao;

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }
                }
                else
                {
                    var pagamentoContaReceber = new PagtoContasReceber();
                    pagamentoContaReceber.IdContaR = idContaR;
                    pagamentoContaReceber.IdFormaPagto = (uint)idsFormaPagamento.ElementAtOrDefault(i);
                    pagamentoContaReceber.ValorPagto = valoresRecebimento.ElementAtOrDefault(i).GetValueOrDefault();
                    pagamentoContaReceber.IdContaBanco = idsFormaPagamento.ElementAtOrDefault(i) != (uint)Pagto.FormaPagto.Dinheiro && idsContaBanco.ElementAtOrDefault(i) > 0 ?
                        (uint?)idsContaBanco.ElementAt(i) : null;
                    pagamentoContaReceber.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint?)idsTipoCartao.ElementAt(i) : null;
                    pagamentoContaReceber.IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (uint?)idsDepositoNaoIdentificado.ElementAt(i) : null;
                    pagamentoContaReceber.QuantidadeParcelaCartao = quantidadesParcelaCartao.ElementAtOrDefault(i) > 0 ? quantidadesParcelaCartao.ElementAt(i) : null;
                    pagamentoContaReceber.NumAutCartao = numerosAutorizacaoCartao.ElementAtOrDefault(i);

                    PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                }

                #endregion
            }

            if (creditoUtilizado > 0)
            {
                var contaReceberCredito = new ContasReceber();
                contaReceberCredito.IdLoja = idLojaRecebimento;
                contaReceberCredito.IdSinal = sinal.IdSinal;
                contaReceberCredito.IdCliente = pedidos[0].IdCli;
                contaReceberCredito.IdConta = UtilsPlanoConta.GetPlanoSinal((uint)Pagto.FormaPagto.Credito);
                contaReceberCredito.DataVec = DateTime.Now;
                contaReceberCredito.ValorVec = creditoUtilizado;
                contaReceberCredito.DataRec = DateTime.Now;
                contaReceberCredito.ValorRec = creditoUtilizado;
                contaReceberCredito.Recebida = true;
                contaReceberCredito.UsuRec = usuarioLogado.CodUser;
                contaReceberCredito.NumParc = 1;
                contaReceberCredito.NumParcMax = 1;
                contaReceberCredito.Usucad = usuarioLogado.CodUser;

                var idContaR = ContasReceberDAO.Instance.InsertBase(session, contaReceberCredito);

                #region Salva o pagamento da conta

                var pagamentoContaReceber = new PagtoContasReceber();

                pagamentoContaReceber.IdContaR = idContaR;
                pagamentoContaReceber.IdFormaPagto = (uint)Pagto.FormaPagto.Credito;
                pagamentoContaReceber.ValorPagto = creditoUtilizado;

                PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);

                #endregion
            }

            #endregion

            #region Atualização dos dados dos pedidos

            foreach (var pedido in pedidos)
            {
                // Muda a situação para confirmado liberação caso seja pagto antecip. e não possua vidros para produção.
                if (PedidoConfig.LiberarPedido && !PedidoDAO.Instance.PossuiVidrosProducao(session, pedido.IdPedido))
                {
                    try { PedidoDAO.Instance.ConfirmarLiberacaoPedido(session, pedido.IdPedido.ToString(), out idsPedidoConfirmados, out idsPedidoNaoConfirmados, false); } catch { }
                }
                // Muda a situação para AtivoConferencia caso a situação atual seja Ativo ou EmConferencia
                else if (pedido.Situacao == Pedido.SituacaoPedido.Ativo || pedido.Situacao == Pedido.SituacaoPedido.EmConferencia)
                {
                    PedidoDAO.Instance.AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.AtivoConferencia);
                }

                #region Geração da comissão do pedido

                if (sinal.DescontarComissao.GetValueOrDefault())
                {
                    ComissaoDAO.Instance.GerarComissao(session, Pedido.TipoComissao.Comissionado, pedido.IdComissionado.Value, pedido.IdPedido.ToString(), pedido.DataConf.Value.ToString(),
                        pedido.DataConf.Value.ToString(), 0, null);
                }

                #endregion
            }

            #endregion

            #region Cálculo do saldo devedor

            ClienteDAO.Instance.ObterSaldoDevedor(session, sinal.IdCliente, out saldoDevedor, out saldoCredito);

            objPersistence.ExecuteCommand(session, string.Format("UPDATE sinal SET SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito WHERE IdSinal = {0}", sinal.IdSinal),
                new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

            #endregion

            #region Montagem da mensagem de retorno

            var mensagemRetorno = string.Format("{0} recebido. ", tipoRecebimento);

            if (retorno != null)
            {
                if (retorno.creditoGerado > 0)
                {
                    mensagemRetorno += string.Format("Foi gerado {0} de crédito para o cliente. ", retorno.creditoGerado.ToString("C"));
                }

                if (retorno.creditoDebitado)
                {
                    mensagemRetorno += string.Format("Foi utilizado {0} de crédito do cliente, restando {1} de crédito. ",
                        creditoUtilizado.ToString("C"), ClienteDAO.Instance.GetCredito(session, sinal.IdCliente).ToString("C"));
                }
            }

            mensagemRetorno += string.Format("\t{0}", sinal != null && sinal.IdSinal > 0 ? sinal.IdSinal : 0);

            #endregion

            return mensagemRetorno;
        }

        /// <summary>
        /// Cancela o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public void CancelarPreRecebimentoSinalPagamentoAntecipadoComTransacao(int idSinal, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarPreRecebimentoSinalPagamentoAntecipado(transaction, idSinal, motivo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarPreRecebimentoSinalComTransacao - ID sinal: {0}.", idSinal), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar o recebimento.", ex));
                }
            }
        }

        /// <summary>
        /// Cancela o pré recebimento do sinal/pagamento antecipado do pedido.
        /// </summary>
        public void CancelarPreRecebimentoSinalPagamentoAntecipado(GDASession session, int idSinal, string motivo)
        {
            #region Declaração de variáveis

            var sinal = GetSinalDetails(session, (uint)idSinal);
            var pedidos = PedidoDAO.Instance.GetBySinal(session, sinal.IdSinal, sinal.IsPagtoAntecipado);
            var tipoRecebimento = sinal.IsPagtoAntecipado ? "pagamento antecipado" : "sinal";
            var chequesSinal = ChequesDAO.Instance.GetBySinal(session, (uint)idSinal);

            #endregion

            #region Validações de cheques do sinal/pagamento antecipado

            // Verifica se algum dos cheques deste sinal já foi utilizando em um depósito.
            if (chequesSinal?.Any(f => f.IdDeposito > 0) ?? false)
            {
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram depositados, cancele ou retifique o depósito antes de cancelar este sinal.");
            }

            // Verifica se algum dos cheques deste sinal já foi utilizando em um pagamento.
            if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM pagto_cheque pc WHERE pc.IdCheque IN (SELECT c.IdCheque FROM cheques c WHERE c.IdSinal={0})", idSinal)))
            {
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em pagamentos, cancele ou retifique-os antes de cancelar este sinal.");
            }

            // Chamado 12032. Verifica se algum dos cheques deste sinal já foi utilizando em um pagamento de crédito para fornecedor.
            if (chequesSinal?.Any(f => f.IdCreditoFornecedor > 0) ?? false)
            {
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em pagamento de crédito para fornecedor, cancele ou retifique-os antes de cancelar este sinal.");
            }

            // Chamado 12032. Verifica se algum dos cheques deste sinal já foi utilizando em um sinal de compra.
            if (chequesSinal?.Any(f => f.IdSinalCompra > 0) ?? false)
            {
                throw new Exception("Um ou mais cheques recebidos neste sinal já foram utilizados em sinal de compra, cancele ou retifique-os antes de cancelar este sinal.");
            }

            #endregion

            #region Validações dos pedidos do sinal/pagamento antecipado

            // Verifica se o pedido já está confirmado.
            foreach (var pedido in pedidos)
            {
                // Verifica se o sinal/pagamento antecipado já foi cancelado.
                if (sinal.IsPagtoAntecipado ? pedido.IdPagamentoAntecipado.GetValueOrDefault() == 0 : pedido.IdSinal.GetValueOrDefault() == 0)
                {
                    throw new Exception(string.Format("O {0} do pedido {1} já foi cancelado.", tipoRecebimento.ToLower(), pedido.IdPedido));
                }

                // Caso o pedido possua pagamento antecipado o mesmo deve ser cancelado antes de cancelar o sinal.
                if (!sinal.IsPagtoAntecipado && pedidos.Any(f => f.IdPagamentoAntecipado > 0))
                {
                    throw new Exception(string.Format("Cancele o pagamento antecipado do pedido {0} antes de cancelar o sinal do mesmo.", pedido.IdPedido));
                }

                // Se a empresa libera os pedidos: não cancela o sinal se há uma conferência no PCP
                if (PedidoConfig.LiberarPedido)
                {
                    // Se o pedido possuir espelho não pode cancelar o sinal, uma vez que o sinal só pode efetuado 
                    // se o pedido estiver na situação conferido COM (se for pagamento antes da produção)
                    if (PedidoConfig.ImpedirConfirmacaoPedidoPagamento && PedidoEspelhoDAO.Instance.ExisteEspelho(session, pedido.IdPedido) && ClienteDAO.Instance.IsPagamentoAntesProducao(session, pedido.IdCli))
                    {
                        throw new Exception(string.Format("O {0} do pedido {1} não pode ser cancelado porque há uma conferência para ele.", tipoRecebimento.ToLower(), pedido.IdPedido));
                    }
                    else if (!LiberarPedidoDAO.Instance.PodeCancelarPedido(session, pedido.IdPedido))
                    {
                        throw new Exception(string.Format("O {0} do pedido {1} não pode ser cancelado porque já existe uma liberação para ele.", tipoRecebimento.ToLower(), pedido.IdPedido));
                    }
                }
            }

            #endregion

            #region Atualização dos pedidos do sinal

            // Indica nos pedidos que o sinal não foi recebido.
            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET {0}=NULL WHERE {1}{2}",
                !sinal.IsPagtoAntecipado ? "IdSinal" : "ValorPagamentoAntecipado=NULL, IdPagamentoAntecipado", !sinal.IsPagtoAntecipado ? "idSinal=" : "idPagamentoAntecipado=", idSinal));

            #endregion

            #region Log de cancelamento

            LogCancelamentoDAO.Instance.LogSinal(session, sinal, motivo, true);

            #endregion

            #region Atualização dos dados do sinal/pagamento antecipado

            sinal.ValorCreditoAoCriar = null;
            sinal.DataRecebimento = null;
            sinal.TotalPagar = null;
            sinal.TotalPago = null;
            sinal.IdLojaRecebimento = null;
            sinal.DescontarComissao = null;
            sinal.RecebimentoCaixaDiario = null;
            sinal.RecebimentoGerarCredito = null;
            sinal.NumAutConstrucard = null;
            // Atualiza no sinal os ids dos pedidos, dos cheques e os valores pagos.
            sinal.IdsPedidosR = string.Join(",", pedidos.Select(f => f.IdPedido).ToList());
            sinal.ValoresR = string.Join(",", pedidos.Select(f => (!sinal.IsPagtoAntecipado ? f.ValorEntrada : f.ValorPagamentoAntecipado).ToString("0.00").Replace(",", ".")).ToList());
            sinal.IdsChequesR = string.Join(",", chequesSinal.Select(f => f.IdCheque).ToList());
            sinal.Situacao = (int)Sinal.SituacaoEnum.Cancelado;
            Update(session, sinal);

            #endregion
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