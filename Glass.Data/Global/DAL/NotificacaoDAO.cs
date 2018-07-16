using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public class NotificacaoDAO : BaseDAO<Notificacao, NotificacaoDAO>
    {
        /// <summary>
        /// Marca uma notificação como enviada
        /// </summary>
        /// <param name="periodo"></param>
        public void MarcarEnviada(DateTime periodo)
        {
            using (var transaction = (new GDA.GDATransaction()))
            {
                try
                {
                    transaction.BeginTransaction();

                    var sql = @"
                        UPDATE notificacao
                        SET enviada = 1
                        WHERE DataCad <= ?data";

                    objPersistence.ExecuteCommand(transaction, sql, new GDAParameter("?data", periodo));


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
        /// Obtem as notificacoes nao enviadas
        /// </summary>
        /// <returns></returns>
        public List<Notificacao> ObterNaoEnviadas()
        {
            using (var transaction = (new GDA.GDATransaction()))
            {
                try
                {
                    transaction.BeginTransaction();

                    SalvarNoticacoesSetorInoperante(transaction);
                    SalvarNoticacoesComercialInoperante(transaction);
                    SalvarNoticacoesFaturamentoInoperante(transaction);
                    SalvarNoticacoesMensagens(transaction);

                    var sql = @"SELECT * FROM notificacao WHERE enviada = 0";

                    var retorno = objPersistence.LoadData(transaction, sql).ToList();

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
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
        /// Salva as notificaçoes de setor inoperante
        /// </summary>
        public void SalvarNoticacoesSetorInoperante(GDASession session)
        {
            var sql = @"
                    SELECT CONCAT(s.IdSetor, ';', s.Descricao, ';', MAX(lp.DataLeitura), ';', s.TempoAlertaInatividade)
                    FROM Setor s
                        INNER JOIN leitura_producao lp ON (s.IdSetor = lp.IdSetor)
                    WHERE s.Situacao = 1 
                        AND s.TempoAlertaInatividade > 0 
						AND lp.DataLeitura > (NOW() - INTERVAL 20 DAY)
                        AND s.IdSetor NOT IN
                            (
                                SELECT s.IdSetor
                                FROM setor s
	                                INNER JOIN leitura_producao lp ON (s.IdSetor = lp.IdSetor)
                                WHERE TempoAlertaInatividade > 0 
									AND s.Situacao = 1
                                    AND lp.DataLeitura > (NOW() - INTERVAL 1 DAY)
	                                AND lp.DataLeitura > (NOW() - INTERVAL s.TempoAlertaInatividade MINUTE)
                            )
                    GROUP BY s.IdSetor";

            var consultaSetores = ExecuteMultipleScalar<string>(session, sql);

            var dados = consultaSetores.Select(f => new
            {
                IdSetor = f.Split(';')[0].StrParaInt(),
                Descricao = f.Split(';')[1],
                DataLeitura = f.Split(';')[2].StrParaDate().Value,
                TempoAlertaInatividade = f.Split(';')[3].StrParaInt()
            }).ToList();

            foreach (var d in dados)
            {
                var ultDataNotificacao = ObtemDataUltNotificacaoSetorInoperante(session, d.IdSetor);

                if (ultDataNotificacao == null || LeituraProducaoDAO.Instance.VerificarSetorTeveLeituraPosterior(session, d.IdSetor, ultDataNotificacao.Value))
                {
                    Insert(session, new Notificacao()
                    {
                        TipoNotificacao = TipoNotificacaoEnum.SetorInoperante,
                        Mensagem = string.Format("Setor {0} inoperante, este setor não realiza leituras há mais de {1} minutos.", d.Descricao, d.TempoAlertaInatividade),
                        IdSetor = d.IdSetor,
                        DataCad = d.DataLeitura,
                        Enviada = false
                    });
                }
            }
        }

        /// <summary>
        /// Salva as notificaçoes do comercial inoperante
        /// </summary>
        public void SalvarNoticacoesComercialInoperante(GDASession session)
        {
            var dataUltPedido = ExecuteScalar<DateTime>(session, "SELECT MAX(DataCad) FROM pedido");
            var tempoNotificacao = PedidoConfig.TempoAlertaComercialInoperante;

            if (tempoNotificacao == 0 || dataUltPedido > DateTime.Now.AddMinutes(-tempoNotificacao))
                return;

            var ultDataNotificacao = ObtemDataUltNotificacao(session, TipoNotificacaoEnum.ComercialInoperante);

                if (ultDataNotificacao == null || PedidoDAO.Instance.TeveCadPosterior(session, ultDataNotificacao.Value))
                {
                    Insert(session, new Notificacao()
                    {
                        TipoNotificacao = TipoNotificacaoEnum.ComercialInoperante,
                        Mensagem = string.Format("Comercial inoperante, comercial não emite pedidos há {0} minutos.", tempoNotificacao),
                        DataCad = dataUltPedido,
                        Enviada = false
                    });
                }
            
        }

        /// <summary>
        /// Salva as notificaçoes do faturamento inoperante
        /// </summary>
        public void SalvarNoticacoesFaturamentoInoperante(GDASession session)
        {
            var dataUltLiberacao = ExecuteScalar<DateTime>(session, "SELECT MAX(DataLiberacao) FROM liberarPedido");
            var tempoNotificacao = FinanceiroConfig.TempoAlertaFaturamentoInoperante;

            if (tempoNotificacao == 0 || dataUltLiberacao > DateTime.Now.AddMinutes(-tempoNotificacao))
                return;

            var ultDataNotificacao = ObtemDataUltNotificacao(session, TipoNotificacaoEnum.FaturamentoInoperante);

            if (ultDataNotificacao == null || LiberarPedidoDAO.Instance.TeveLiberacaoPosterior(session, ultDataNotificacao.Value))
            {
                Insert(session, new Notificacao()
                {
                    TipoNotificacao = TipoNotificacaoEnum.FaturamentoInoperante,
                    Mensagem = string.Format("Faturamento inoperante, faturamento não realiza liberações há {0} minutos.", tempoNotificacao),
                    DataCad = dataUltLiberacao,
                    Enviada = false
                });
            }

        }

        /// <summary>
        /// Salva as notificaçoes do faturamento inoperante
        /// </summary>
        public void SalvarNoticacoesMensagens(GDASession session)
        {
            var sql = @"SELECT CONCAT(m.IdMensagem, '$$', d.IdFunc, '$$', rem.nome, '$$', Assunto, '$$', Descricao, '$$', m.DataCad)
                        FROM mensagem m
	                        INNER JOIN funcionario rem ON (m.IdRemetente = rem.IdFunc)
                            INNER JOIN destinatario d ON (m.IdMensagem = d.IdMensagem)
                        WHERE d.Lida = 0 AND m.IdMensagem NOT IN 
                        (
	                        SELECT IdMensagem
	                        FROM notificacao
	                        WHERE COALESCE(IdMensagem, 0) <> 0
                        )";

            var resultado = ExecuteMultipleScalar<string>(session, sql);

            var dados = resultado.Select(f => new Notificacao()
            {
                IdMensagem = f.Split(new string[] {"$$"}, StringSplitOptions.None)[0].StrParaInt(),
                IdDestinatario = f.Split(new string[] { "$$" }, StringSplitOptions.None)[1].StrParaInt(),
                Remetente = f.Split(new string[] { "$$" }, StringSplitOptions.None)[2],
                Assunto = f.Split(new string[] { "$$" }, StringSplitOptions.None)[3],
                Mensagem = f.Split(new string[] { "$$" }, StringSplitOptions.None)[4],
                DataCad = f.Split(new string[] { "$$" }, StringSplitOptions.None)[5].StrParaDate().Value,
                TipoNotificacao = TipoNotificacaoEnum.Mensagem
            }).ToList();

            foreach (var d in dados)
                Insert(session, d);
        }

        /// <summary>
        /// Obtem a data da ultima notificação de setor inoperante
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public DateTime? ObtemDataUltNotificacaoSetorInoperante(GDASession sessao, int idSetor)
        {
            var sql = @"
                SELECT MAX(DataCad)
                FROM notificacao
                WHERE IdSetor = " + idSetor;

            return ExecuteScalar<DateTime?>(sessao, sql);
        }

        /// <summary>
        /// Obtem a data da ultima notificação de setor inoperante
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public DateTime? ObtemDataUltNotificacao(GDASession sessao, TipoNotificacaoEnum tipo)
        {
            var sql = @"
                SELECT MAX(DataCad)
                FROM notificacao
                WHERE TipoNotificacao = " + (int)tipo;

            return ExecuteScalar<DateTime?>(sessao, sql);
        }
    }
}
