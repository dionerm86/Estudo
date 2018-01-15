using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Glass.Data.DAL
{
    public class TransacaoCapptaTefDAO : BaseCadastroDAO<TransacaoCapptaTef, TransacaoCapptaTefDAO>
    {
        #region Variaveis Locais

        //URL da API da CAPPTA
        private Uri _baseAddress = new Uri("https://integracao.cappta.com.br/payment/");

        #endregion

        #region Métodos Publicos

        /// <summary>
        /// Busca a listagem de transações
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<TransacaoCapptaTef> GetList(string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlAgrupado(true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Faz a contagem de transações
        /// </summary>
        /// <returns></returns>
        public int GetListCount()
        {
            var sql = SqlAgrupado(false);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Busca os detalhes de uma transação
        /// </summary>
        /// <param name="tipoPagto"></param>
        /// <param name="idReferencia"></param>
        /// <returns></returns>
        public IList<TransacaoCapptaTef> GetListTransacoes(Helper.UtilsFinanceiro.TipoReceb tipoPagto, int idReferencia)
        {
            var sql = "SELECT * FROM transacao_cappta_tef WHERE TipoPagamento = ?tpPagto AND IdReferencia = ?id";

            var dados = objPersistence.LoadData(sql, new GDAParameter("?tpPagto", tipoPagto), new GDAParameter("?id", idReferencia)).ToList();

            dados.ForEach(d => BuscarDados(d));

            return dados;
        }

        /// <summary>
        /// Busca os detalhes de uma transação
        /// </summary>
        /// <param name="codControle"></param>
        /// <returns></returns>
        public IList<TransacaoCapptaTef> GetListTransacoes(string codControle)
        {
            var sql = "SELECT * FROM transacao_cappta_tef WHERE CodigoControle IN ({0})";

            var dados = objPersistence.LoadData(string.Format(sql, string.Join(",", codControle.Split(';')))).ToList();

            dados.ForEach(d => BuscarDados(d));

            return dados;
        }

        /// <summary>
        /// Salva uma transação e atualiza o pagto da referencia
        /// </summary>
        /// <param name="tipoPagto"></param>
        /// <param name="id"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="admCodes"></param>
        /// <param name="customerReceipt"></param>
        /// <param name="merchantReceipt"></param>
        /// <param name="formasPagto"></param>
        public void AtualizaPagamentosCappta(Helper.UtilsFinanceiro.TipoReceb tipoPagto, int id, string checkoutGuid, string admCodes,
            string customerReceipt, string merchantReceipt, string formasPagto)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var index = 0;
                    foreach (var admCode in admCodes.Split(';'))
                    {
                        Instance.Insert(transaction, new TransacaoCapptaTef()
                        {
                            IdReferencia = id,
                            TipoPagamento = tipoPagto,
                            CheckoutGuid = checkoutGuid,
                            CodigoControle = admCode,
                            ComprovanteLoja = merchantReceipt.Split(';')[index],
                            ComprovanteCliente = customerReceipt.Split(';')[index]
                        });

                        index++;
                    }

                    var fpIndex = 1;
                    var admCodeIndex = 0;

                    foreach (var fp in formasPagto.Split(';'))
                    {
                        if (fp.StrParaInt() != (int)Pagto.FormaPagto.Cartao)
                        {
                            fpIndex++;
                            continue;
                        }

                        switch (tipoPagto)
                        {
                            case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                                PagtoLiberarPedidoDAO.Instance.AtualizarNumAutCartao(transaction, id, fpIndex, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                                PagtoAcertoDAO.Instance.AtualizarNumAutCartao(transaction, id, fpIndex, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                                PagtoAcertoChequeDAO.Instance.AtualizarNumAutCartao(transaction, id, fpIndex, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                                PagtoContasReceberDAO.Instance.AtualizarNumAutCartao(transaction, id, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                                PagtoSinalDAO.Instance.AtualizarNumAutCartao(transaction, id, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            case Helper.UtilsFinanceiro.TipoReceb.Obra:
                                PagtoObraDAO.Instance.AtualizarNumAutCartao(transaction, id, fpIndex, admCodes.Split(';')[admCodeIndex]);
                                admCodeIndex++;
                                break;
                            default:
                                break;
                        }

                        fpIndex++;
                    }

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

        /// <summary>
        /// Salva um cancelamento 
        /// </summary>
        /// <param name="tipoPagto"></param>
        /// <param name="id"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="codControle"></param>
        /// <param name="customerReceipt"></param>
        /// <param name="merchantReceipt"></param>
        public void SalvaCancelamento(Helper.UtilsFinanceiro.TipoReceb tipoPagto, int id, string checkoutGuid, string codControle, string customerReceipt, string merchantReceipt)
        {
            Instance.Insert(new TransacaoCapptaTef()
            {
                IdReferencia = id,
                TipoPagamento = tipoPagto,
                CheckoutGuid = checkoutGuid,
                CodigoControle = codControle,
                ComprovanteLoja = merchantReceipt,
                ComprovanteCliente = customerReceipt
            });

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    switch (tipoPagto)
                    {
                        case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                                CancelarLiberacao(transaction, id);
                                break;
                        case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                            if (AcertoDAO.Instance.ObtemValorCampo<Situacao>(transaction, "Situacao", "IdAcerto = " + id) == Situacao.Ativo)
                                ContasReceberDAO.Instance.CancelarAcerto(transaction, (uint)id, "Cancelamento da transação TEF", DateTime.Now, false, false);
                                break;
                        case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                            if (AcertoChequeDAO.Instance.ObtemValorCampo<Situacao>(transaction, "Situacao", "IdAcertoCheque = " + id) == Situacao.Ativo)
                                AcertoChequeDAO.Instance.CancelarAcertoCheque((uint)id, "Cancelamento da transação TEF", DateTime.Now, false, false);
                            break;
                        case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                            if (ContasReceberDAO.Instance.ObtemValorCampo<bool>(transaction, "Recebida", "idContaR = " + id))
                                ContasReceberDAO.Instance.CancelarConta((uint)id, "Cancelamento da transação TEF", DateTime.Now, false, false);
                            break;
                        case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                            if (SinalDAO.Instance.ObtemValorCampo<Situacao>(transaction, "Situacao", "IdSinal = " + id) == Situacao.Ativo)
                                SinalDAO.Instance.Cancelar(transaction, (uint)id, null, false, false, "Cancelamento da transação TEF", DateTime.Now, false, false);
                            break;
                        case Helper.UtilsFinanceiro.TipoReceb.Obra:
                            if (ObraDAO.Instance.ObtemSituacao(transaction, (uint)id) == Obra.SituacaoObra.Confirmada)
                                ObraDAO.Instance.CancelaObra((uint)id, "Cancelamento da transação TEF", DateTime.Now, false, false);
                            break;
                        default:
                            break;
                    }

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

        /// <summary>
        /// Verifica se a referencia informada possui recebimento com TEF
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoPagto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TemRecebimentoComTef(GDASession sessao, Helper.UtilsFinanceiro.TipoReceb tipoPagto, int id)
        {
            var sql = @"SELECT count(*) FROM transacao_cappta_tef WHERE IdReferencia = ?id AND TipoPagamento = ?tipoPagto";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?id", id), new GDAParameter("?tipoPagto", tipoPagto)) > 0;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Sql de busca padrão
        /// </summary>
        /// <param name="selecionar"></param>
        /// <returns></returns>
        private string SqlAgrupado(bool selecionar)
        {
            var nomeCliente = ClienteDAO.Instance.GetNomeCliente("c");
            var campos = selecionar ? "tct.*, concat(tct.IdCliente, ' - ', " + nomeCliente + @") as NomeCliente" : "COUNT(*)";

            var sql = @"
                SELECT " + campos + @"
                FROM transacao_cappta_tef tct
                    LEFT JOIN cliente c ON (tct.IdCliente = c.Id_Cli)
                ";

            if (selecionar)
                sql += "GROUP BY tct.TipoPagamento, tct.IdReferencia";

            sql += " ORDER BY tct.DataCad Desc";

            return sql;
        }

        /// <summary>
        /// Busca os dados da transação na API da cappta
        /// </summary>
        /// <param name="transacao"></param>
        private void BuscarDados(TransacaoCapptaTef transacao)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("apikey", FinanceiroConfig.CapptaAuthKey);
                wc.Encoding = ASCIIEncoding.UTF8;

                var jsonData = wc.DownloadString(string.Format("{0}/{1}/{2}", _baseAddress.ToString(), transacao.CheckoutGuid, transacao.CodigoControle));
                transacao.CopiarPropiedades(Newtonsoft.Json.JsonConvert.DeserializeObject<TransacaoCapptaTef>(jsonData));
            }
        }

        /// <summary>
        /// Cancela a liberação ao cancelar o tef
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idLiberarPedido"></param>
        private void CancelarLiberacao(GDASession sessao, int idLiberarPedido)
        {
            if (LiberarPedidoDAO.Instance.ObterSituacao(sessao, idLiberarPedido) == LiberarPedido.SituacaoLiberarPedido.Cancelado)
                return;

                var idNf = LiberarPedidoDAO.Instance.ObterIdNf(sessao, idLiberarPedido);

                if (idNf > 0)
                {
                    var situacaoNf = NotaFiscalDAO.Instance.ObtemSituacao(sessao, (uint)idNf);

                    if (situacaoNf == (int)NotaFiscal.SituacaoEnum.Autorizada)
                        NFeUtils.EnviaXML.EnviaCancelamentoEvt((uint)idNf, "Cancelamento da transação TEF");
                    else if (situacaoNf == (int)NotaFiscal.SituacaoEnum.Aberta || situacaoNf == (int)NotaFiscal.SituacaoEnum.FalhaEmitir)
                        NFeUtils.EnviaXML.EnviaInutilizacao((uint)idNf, "Cancelamento da transação TEF");
                }

            LiberarPedidoDAO.Instance.CancelarLiberacao(sessao, (uint)idLiberarPedido, "Cancelamento da transação TEF", DateTime.Now, false, false);
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Insere uma transação
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public override uint Insert(GDASession sessao, TransacaoCapptaTef objInsert)
        {
            switch (objInsert.TipoPagamento)
            {
                case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                    {
                        objInsert.IdCliente = (int)LiberarPedidoDAO.Instance.GetIdCliente(sessao, (uint)objInsert.IdReferencia);
                        objInsert.Valor = LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Total", "IdLiberarPedido = " + objInsert.IdReferencia);
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                    {
                        objInsert.IdCliente = (int)AcertoDAO.Instance.ObtemIdCliente(sessao, (uint)objInsert.IdReferencia);
                        objInsert.Valor = PagtoAcertoDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorPagto", "IdAcerto = " + objInsert.IdReferencia);
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                    {
                        objInsert.IdCliente = AcertoChequeDAO.Instance.ObtemValorCampo<int>(sessao, "IdCliente", "IdAcertoCheque = " + objInsert.IdReferencia);
                        objInsert.Valor = AcertoChequeDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorAcerto", "IdAcertoCheque = " + objInsert.IdReferencia);
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                    {
                        objInsert.IdCliente = (int)ContasReceberDAO.Instance.ObtemIdCliente((uint)objInsert.IdReferencia);
                        objInsert.Valor = ContasReceberDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorRec", "IdContaR = " + objInsert.IdReferencia);
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                    {
                        var sinal = SinalDAO.Instance.GetSinalDetails(sessao, (uint)objInsert.IdReferencia);
                        objInsert.IdCliente = (int)sinal.IdCliente;
                        objInsert.Valor = sinal.TotalSinal;
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.Obra:
                    {
                        objInsert.IdCliente = ObraDAO.Instance.ObtemIdCliente(sessao, objInsert.IdReferencia);
                        objInsert.Valor = ObraDAO.Instance.GetValorObra(sessao, (uint)objInsert.IdReferencia);
                        break;
                    }
                default:
                    break;
            }

            return base.Insert(sessao, objInsert);
        }

        #endregion
    }
}
