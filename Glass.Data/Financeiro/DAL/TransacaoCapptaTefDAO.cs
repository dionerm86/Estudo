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
    /// <summary>
    /// Acesso a dados da transação cappta
    /// </summary>
    public class TransacaoCapptaTefDAO : BaseCadastroDAO<TransacaoCapptaTef, TransacaoCapptaTefDAO>
    {
        #region Variaveis Locais

        //URL da API da CAPPTA
        private Uri _baseAddress = new Uri("https://integracao.cappta.com.br/payment/");

        #endregion

        #region Busca de dados

        /// <summary>
        /// Busca a listagem de transações
        /// </summary>
        public IList<TransacaoCapptaTef> GetList(string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlAgrupado(true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Faz a contagem de transações
        /// </summary>
        public int GetListCount()
        {
            var sql = SqlAgrupado(false);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Busca os detalhes de uma transação
        /// </summary>
        public IList<TransacaoCapptaTef> GetListTransacoes(Helper.UtilsFinanceiro.TipoReceb tipoRecebimento, int idReferencia)
        {
            var sql = "SELECT * FROM transacao_cappta_tef WHERE CheckoutGuid IS NOT NULL AND TipoRecebimento = ?tpReceb AND IdReferencia = ?id";

            var dados = objPersistence.LoadData(sql, new GDAParameter("?tpReceb", tipoRecebimento), new GDAParameter("?id", idReferencia)).ToList();

            dados.ForEach(d => BuscarDados(d));

            return dados;
        }

        /// <summary>
        /// Busca os detalhes de uma transação
        /// </summary>
        public IList<TransacaoCapptaTef> GetListTransacoes(string codControle)
        {
            var sql = "SELECT * FROM transacao_cappta_tef WHERE CodigoControle IN ({0})";

            var dados = objPersistence.LoadData(string.Format(sql, string.Join(",", codControle.Split(';')))).ToList();

            dados.ForEach(d => BuscarDados(d));

            return dados;
        }

        /// <summary>
        /// Sql de busca padrão
        /// </summary>
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
                sql += "GROUP BY tct.TipoRecebimento, tct.IdReferencia";

            sql += " ORDER BY tct.DataCad Desc";

            return sql;
        }

        /// <summary>
        /// Busca os dados da transação na API da cappta
        /// </summary>
        private void BuscarDados(TransacaoCapptaTef transacao)
        {
            if (string.IsNullOrWhiteSpace(transacao.CheckoutGuid))
                return;

            using (var wc = new WebClient())
            {
                wc.Headers.Add("apikey", FinanceiroConfig.CapptaAuthKey);
                wc.Encoding = ASCIIEncoding.UTF8;

                var jsonData = wc.DownloadString(string.Format("{0}/{1}/{2}", _baseAddress.ToString(), transacao.CheckoutGuid, transacao.CodigoControle));
                transacao.CopiarPropiedades(Newtonsoft.Json.JsonConvert.DeserializeObject<TransacaoCapptaTef>(jsonData));
            }
        }

        /// <summary>
        /// Verifica se a referencia informada possui recebimento com TEF
        /// </summary>
        public bool TemRecebimentoComTef(GDASession sessao, Helper.UtilsFinanceiro.TipoReceb tipoRecebimento, int id)
        {
            var sql = @"SELECT count(*) FROM transacao_cappta_tef WHERE CheckoutGuid IS NOT NULL AND IdReferencia = ?id AND tipoRecebimento = ?tipoRecebimento";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?id", id), new GDAParameter("?tipoRecebimento", tipoRecebimento)) > 0;
        }

        /// <summary>
        /// Metodo para informar se deve exibir o botão para finalizar a transação na listagem de transações
        /// </summary>
        public bool ExibirFinalizarTransacao(TransacaoCapptaTef transacao)
        {
            switch (transacao.TipoRecebimento)
            {
                case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                    {
                        return LiberarPedidoDAO.Instance.ObterSituacao(null, transacao.IdReferencia) == LiberarPedido.SituacaoLiberarPedido.Processando;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                    {
                        return AcertoDAO.Instance.ObterSituacao(null, transacao.IdReferencia) == Acerto.SituacaoEnum.Processando;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                    {
                        return AcertoChequeDAO.Instance.ObterSituacao(null, transacao.IdReferencia) == AcertoCheque.SituacaoEnum.Processando;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                    {
                        return SinalDAO.Instance.ObterSituacao(null, transacao.IdReferencia) == Sinal.SituacaoEnum.Processando;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.Obra:
                    {
                        return ObraDAO.Instance.ObtemSituacao(null, (uint)transacao.IdReferencia) == Obra.SituacaoObra.Processando;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        #endregion

        #region Finaliza transação

        /// <summary>
        /// Atualiza a transação apos o recebimento
        /// </summary>
        public void FinalizarTransacao(CapptaRetornoRecebimento dados)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (dados.Estorno)
                        FinalizarTransacaoEstorno(transaction, dados);
                    else
                        FinalizarTransacaoRecebimento(transaction, dados);

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
        /// Finaliza uma transação de recebimento
        /// </summary>
        private void FinalizarTransacaoRecebimento(GDASession session, CapptaRetornoRecebimento dados)
        {
            if (dados.Sucesso)
            {
                //Insere as transações
                foreach (var receb in dados.Recebimentos)
                {
                    //Necessario usar uma transação a parte, pois o recebimento na cappta ja foi realizado e caso ocorra algum problema na finalização no webglass
                    //o mesmo não sera cancelado na cappta, sendo assim tem que ficar registrado no webglass que o recibimento ocorreu com sucesso na cappta 
                    //apesar de ter ocorrido erro na finalização no webglass
                    Instance.InsertComTransacao(new TransacaoCapptaTef()
                    {
                        IdReferencia = dados.IdReferencia,
                        TipoRecebimento = dados.TipoRecebimento,
                        CheckoutGuid = dados.CheckoutGuid,
                        CodigoControle = receb.CodigoAdministrativo,
                        ComprovanteLoja = receb.Comprovantes.ComprovanteLoja,
                        ComprovanteCliente = receb.Comprovantes.ComprovanteCliente
                    });

                    #region Atualiza o numero de autorização do cartão nos recebimentos

                    switch (dados.TipoRecebimento)
                    {
                        case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                            {
                                PagtoLiberarPedidoDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.PagtoIndex, receb.CodigoAdministrativo);
                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                            {
                                PagtoAcertoDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.PagtoIndex, receb.CodigoAdministrativo);
                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                            {
                                PagtoAcertoChequeDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.PagtoIndex, receb.CodigoAdministrativo);
                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                            {
                                PagtoContasReceberDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.CodigoAdministrativo);
                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                            {
                                PagtoSinalDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.PagtoIndex, receb.CodigoAdministrativo);
                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.Obra:
                            {
                                PagtoObraDAO.Instance.AtualizarNumAutCartao(session, dados.IdReferencia, receb.PagtoIndex, receb.CodigoAdministrativo);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    #endregion
                }

                #region Realiza a finalização dos recebimentos

                switch (dados.TipoRecebimento)
                {
                    case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                        {
                            LiberarPedidoDAO.Instance.FinalizarPreLiberacaoAVista(session, dados.IdReferencia);
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                        {
                            dados.MensagemRetorno = ContasReceberDAO.Instance.FinalizarPreRecebimentoAcerto(session, dados.IdReferencia);
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                        {
                            ChequesDAO.Instance.FinalizarPreQuitacaoChequeDevolvido(session, dados.IdReferencia);
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                        {
                            dados.MensagemRetorno = ContasReceberDAO.Instance.FinalizarPreRecebimentoConta(session, dados.IdReferencia);
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                        {
                            dados.MensagemRetorno = SinalDAO.Instance.FinalizarPreRecebimentoSinalPagamentoAntecipado(session, dados.IdReferencia);
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Obra:
                        {
                            ObraDAO.Instance.FinalizarPrePagamentoVista(session, dados.IdReferencia);
                            break;
                        }
                    default:
                        break;
                }

                #endregion
            }
            else
            {
                #region Cancelamento do recebimento

                //Caso ocorra algum erro ao receber na cappta, seja por falta de limite ou erro cancela o pre recebimento no webglass

                switch (dados.TipoRecebimento)
                {
                    case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                        {
                            LiberarPedidoDAO.Instance.CancelarPreLiberacaoAVista(session, DateTime.Now, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                        {
                            ContasReceberDAO.Instance.CancelarPreRecebimentoAcerto(session, DateTime.Now, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                        {
                            ChequesDAO.Instance.CancelarPreQuitacaoChequeDevolvido(session, DateTime.Now, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                        {
                            ContasReceberDAO.Instance.CancelarPreRecebimentoConta(session, DateTime.Now, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                        {
                            SinalDAO.Instance.CancelarPreRecebimentoSinalPagamentoAntecipado(session, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Obra:
                        {
                            ObraDAO.Instance.CancelarPrePagamentoVista(session, dados.IdReferencia, string.Format("Falha no recebimento TEF. Motivo: {0}", dados.MensagemErro));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                #endregion
            }
        }

        /// <summary>
        /// Finaliza uma transação de estorno
        /// </summary>
        private void FinalizarTransacaoEstorno(GDASession session, CapptaRetornoRecebimento dados)
        {
            //Insere a transação de estorno
            if (dados.Sucesso)
            {
                //Necessario usar uma transação a parte, pois o recebimento na cappta ja foi realizado e caso ocorra algum problema na finalização no webglass
                //o mesmo não sera cancelado na cappta, sendo assim tem que ficar registrado no webglass que o recibimento ocorreu com sucesso na cappta 
                //apesar de ter ocorrido erro na finalização no webglass
                Instance.InsertComTransacao(new TransacaoCapptaTef()
                {
                    IdReferencia = dados.IdReferencia,
                    TipoRecebimento = dados.TipoRecebimento,
                    CheckoutGuid = dados.CheckoutGuid,
                    CodigoControle = dados.Recebimentos[0].CodigoAdministrativo,
                    ComprovanteLoja = dados.Recebimentos[0].Comprovantes.ComprovanteLoja,
                    ComprovanteCliente = dados.Recebimentos[0].Comprovantes.ComprovanteCliente
                });
            }

            #region Cancela o recebimento no webglass

            if (dados.Sucesso || dados.MensagemErro == "Falha ao realizar estorno. Transação não encontrada ou não autorizada.")
            {
                switch (dados.TipoRecebimento)
                {
                    case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                        {
                            if (LiberarPedidoDAO.Instance.ObterSituacao(session, dados.IdReferencia) == LiberarPedido.SituacaoLiberarPedido.Cancelado)
                            {
                                return;
                            }

                            var idNf = LiberarPedidoDAO.Instance.ObterIdNf(session, dados.IdReferencia);

                            if (idNf > 0)
                            {
                                var situacaoNf = NotaFiscalDAO.Instance.ObtemSituacao(session, (uint)idNf);

                                if (situacaoNf == (int)NotaFiscal.SituacaoEnum.Autorizada)
                                {
                                    NFeUtils.EnviaXML.EnviaCancelamentoEvt((uint)idNf, "Cancelamento da transação TEF");
                                }
                                else if (situacaoNf == (int)NotaFiscal.SituacaoEnum.Aberta || situacaoNf == (int)NotaFiscal.SituacaoEnum.FalhaEmitir)
                                {
                                    NFeUtils.EnviaXML.EnviaInutilizacao((uint)idNf, "Cancelamento da transação TEF");
                                }
                            }

                            //LiberarPedidoDAO.Instance.CancelarLiberacao(session, (uint)dados.IdReferencia, "Cancelamento da transação TEF", DateTime.Now, false);

                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                        {
                            if (AcertoDAO.Instance.ObtemValorCampo<Situacao>(session, "Situacao", "IdAcerto = " + dados.IdReferencia) == Situacao.Ativo)
                            {
                                //ContasReceberDAO.Instance.CancelarAcerto(session, (uint)dados.IdReferencia, "Cancelamento da transação TEF", DateTime.Now, false);
                            }

                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                        {
                            if (AcertoChequeDAO.Instance.ObtemValorCampo<Situacao>(session, "Situacao", "IdAcertoCheque = " + dados.IdReferencia) == Situacao.Ativo)
                            {
                                //AcertoChequeDAO.Instance.CancelarAcertoCheque((uint)dados.IdReferencia, "Cancelamento da transação TEF", DateTime.Now, false);
                            }

                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                        {
                            if (ContasReceberDAO.Instance.ObtemValorCampo<bool>(session, "Recebida", "idContaR = " + dados.IdReferencia))
                            {
                                //ContasReceberDAO.Instance.CancelarConta((uint)dados.IdReferencia, "Cancelamento da transação TEF", DateTime.Now, false);
                            }

                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                        {
                            if (SinalDAO.Instance.ObtemValorCampo<Situacao>(session, "Situacao", "IdSinal = " + dados.IdReferencia) == Situacao.Ativo)
                            {
                                //SinalDAO.Instance.Cancelar(session, (uint)dados.IdReferencia, null, false, false, "Cancelamento da transação TEF", DateTime.Now, false);
                            }

                            break;
                        }
                    case Helper.UtilsFinanceiro.TipoReceb.Obra:
                        {
                            var situacao = ObraDAO.Instance.ObtemSituacao(session, (uint)dados.IdReferencia);

                            if (situacao == Obra.SituacaoObra.Confirmada || situacao == Obra.SituacaoObra.Finalizada)
                            {
                                //ObraDAO.Instance.CancelaObra((uint)dados.IdReferencia, "Cancelamento da transação TEF", DateTime.Now, false);
                            }

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            #endregion
        }

        /// <summary>
        /// Finaliza uma transação que ficou na situação processando
        /// </summary>
        public void FinalizarTransacaoProcessando(Helper.UtilsFinanceiro.TipoReceb tipoRecebimento, int idReferencia)
        {
            using (var transacation = new GDATransaction())
            {
                try
                {
                    transacation.BeginTransaction();

                    var sql = "SELECT COUNT(*) > 0 FROM transacao_cappta_tef WHERE COALESCE(CodigoControle, '') <> '' AND TipoRecebimento = ?tr AND IdReferencia = ?id";
                    var temRecebimento = ExecuteScalar<bool>(transacation, sql, new GDAParameter("?tr", (int)tipoRecebimento), new GDAParameter("?id", idReferencia));

                    switch (tipoRecebimento)
                    {
                        case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                            {
                                if (temRecebimento)
                                {
                                    LiberarPedidoDAO.Instance.FinalizarPreLiberacaoAVista(transacation, idReferencia);
                                }
                                else
                                {
                                    LiberarPedidoDAO.Instance.CancelarPreLiberacaoAVista(transacation, DateTime.Now, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                            {
                                if (temRecebimento)
                                {
                                    //ContasReceberDAO.Instance.FinalizarPreRecebimentoAcerto(transacation, idReferencia);
                                }
                                else
                                {
                                    //ContasReceberDAO.Instance.CancelarPreRecebimentoAcerto(transacation, DateTime.Now, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                            {
                                if (temRecebimento)
                                {
                                    //ChequesDAO.Instance.FinalizarPreQuitacaoChequeDevolvido(transacation, idReferencia);
                                }
                                else
                                {
                                    //ChequesDAO.Instance.CancelarPreQuitacaoChequeDevolvido(transacation, DateTime.Now, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                            {
                                if (temRecebimento)
                                {
                                    //ContasReceberDAO.Instance.FinalizarPreRecebimentoConta(transacation, idReferencia);
                                }
                                else
                                {
                                    //ContasReceberDAO.Instance.CancelarPreRecebimentoConta(transacation, DateTime.Now, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                            {
                                if (temRecebimento)
                                {
                                    //SinalDAO.Instance.FinalizarPreRecebimentoSinalPagamentoAntecipado(transacation, idReferencia);
                                }
                                else
                                {
                                    //SinalDAO.Instance.CancelarPreRecebimentoSinalPagamentoAntecipado(transacation, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        case Helper.UtilsFinanceiro.TipoReceb.Obra:
                            {
                                if (temRecebimento)
                                {
                                    //ObraDAO.Instance.FinalizarPrePagamentoVista(transacation, idReferencia);
                                }
                                else
                                {
                                    //ObraDAO.Instance.CancelarPrePagamentoVista(transacation, idReferencia, "Cancelamento de liberação em situação processando.");
                                }

                                break;
                            }
                        default:
                            break;
                    }

                    transacation.Commit();
                    transacation.Close();

                }
                catch (Exception ex)
                {
                    transacation.Rollback();
                    transacation.Close();

                    ErroDAO.Instance.InserirFromException("FinalizarTransacaoProcessando", ex);
                    throw;
                }
            }
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Insere uma transação
        /// </summary>
        public uint InsertComTransacao(TransacaoCapptaTef objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("TransacaoCappta", ex);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Insere uma transação.
        /// </summary>
        public override uint Insert(GDASession sessao, TransacaoCapptaTef objInsert)
        {
            switch (objInsert.TipoRecebimento)
            {
                case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                    {
                        objInsert.IdCliente = (int)LiberarPedidoDAO.Instance.GetIdCliente(sessao, (uint)objInsert.IdReferencia);
                        objInsert.Valor = LiberarPedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Total", string.Format("IdLiberarPedido={0}", objInsert.IdReferencia));
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                    {
                        objInsert.IdCliente = (int)AcertoDAO.Instance.ObtemIdCliente(sessao, (uint)objInsert.IdReferencia);
                        objInsert.Valor = PagtoAcertoDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorPagto", string.Format("IdAcerto={0}", objInsert.IdReferencia));
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                    {
                        objInsert.IdCliente = AcertoChequeDAO.Instance.ObtemValorCampo<int>(sessao, "IdCliente", string.Format("IdAcertoCheque={0}", objInsert.IdReferencia));
                        objInsert.Valor = AcertoChequeDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorAcerto", string.Format("IdAcertoCheque={0}", objInsert.IdReferencia));
                        break;
                    }
                case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                    {
                        objInsert.IdCliente = (int)ContasReceberDAO.Instance.ObtemIdCliente((uint)objInsert.IdReferencia);
                        objInsert.Valor = ContasReceberDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorRec", string.Format("IdContaR={0}", objInsert.IdReferencia));
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
                    {
                        break;
                    }
            }

            return base.Insert(sessao, objInsert);
        }

        #endregion
    }
}
