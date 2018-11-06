using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using System.Reflection;
using Glass.Data.Helper;
using GDA;
using Glass.Log;

namespace Glass.Data.DAL
{
    public sealed class LogCancelamentoDAO : BaseDAO<LogCancelamento, LogCancelamentoDAO>
    {
        //private LogCancelamentoDAO() { }

        #region Busca padrão

        private string Sql(int tabela, uint idRegistroCanc, bool exibirAdmin, string campoBuscar, string campo, uint idFuncCanc,
            string dataIni, string dataFim, string valor, bool selecionar, bool buscarVazio)
        {
            string campos = !String.IsNullOrEmpty(campoBuscar) ? campoBuscar :
                selecionar ? "l.*, f.nome as nomeFuncCanc" : "count(*)";

            string sql = @"
                select " + campos + @"
                from log_cancelamento l
                    left join funcionario f on (l.idFuncCanc=f.idFunc)
                where 1";

            if (tabela > 0)
                sql += " and l.tabela=" + tabela;

            if (idRegistroCanc > 0 || buscarVazio)
                sql += " and l.idRegistroCanc=" + idRegistroCanc;

            if (!exibirAdmin)
                sql += " and l.idFuncCanc not in (" + FuncionarioDAO.Instance.SqlAdminSync(null, true) + ")";

            if (!String.IsNullOrEmpty(campo))
                sql += " and l.campo=?campo";

            if (idFuncCanc > 0)
                sql += " and l.idFuncCanc=" + idFuncCanc;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and l.dataCanc>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and l.dataCanc<=?dataFim";

            if (!String.IsNullOrEmpty(valor))
                sql += " and l.valor like ?valor";

            return sql;
        }

        private GDAParameter[] GetParams(string campo, string dataIni, string dataFim, string valor)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(campo))
                lst.Add(new GDAParameter("?campo", campo));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(valor))
                lst.Add(new GDAParameter("?valor", "%" + valor + "%"));

            return lst.ToArray();
        }

        public IList<LogCancelamento> GetList(int tabela, uint idRegistroCanc, bool exibirAdmin, string campo, uint idFuncCanc,
            string dataIni, string dataFim, string valor, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "l.dataCanc desc";
            return LoadDataWithSortExpression(Sql(tabela, idRegistroCanc, exibirAdmin, null, campo, idFuncCanc,
                dataIni, dataFim, valor, true, false), sortExpression, startRow, pageSize,
                GetParams(campo, dataIni, dataFim, valor));
        }

        public int GetCount(int tabela, uint idRegistroCanc, bool exibirAdmin, string campo, uint idFuncCanc,
            string dataIni, string dataFim, string valor)
        {
            return GetCount(tabela, idRegistroCanc, exibirAdmin, campo, idFuncCanc, dataIni, dataFim, valor, false);
        }

        public int GetCount(int tabela, uint idRegistroCanc, bool exibirAdmin, string campo, uint idFuncCanc,
            string dataIni, string dataFim, string valor, bool buscarVazio)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tabela, idRegistroCanc, exibirAdmin, null, campo, idFuncCanc, dataIni, dataFim, valor,
                false, buscarVazio), GetParams(campo, dataIni, dataFim, valor));
        }

        #endregion

        #region Busca o nome dos campos para o filtro da tela de consulta de log

        /// <summary>
        /// Busca o nome dos campos para o filtro da tela de consulta de log.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroCanc"></param>
        /// <returns></returns>
        public KeyValuePair<string, string>[] GetCampos(int tabela, uint idRegistroCanc, bool exibirAdmin)
        {
            string campos = GetValoresCampo(Sql(tabela, idRegistroCanc, exibirAdmin, "campo", null, 0,
                null, null, null, true, false), "campo").ToString();

            List<KeyValuePair<string, string>> retorno = new List<KeyValuePair<string, string>>();

            if (!String.IsNullOrEmpty(campos))
                foreach (string campo in campos.Split(','))
                    if (!String.IsNullOrEmpty(campo))
                        retorno.Add(new KeyValuePair<string, string>(campo, campo));

            retorno.Sort(new Comparison<KeyValuePair<string, string>>(
                delegate(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
                {
                    return x.Key.CompareTo(y.Key);
                }
            ));

            return retorno.ToArray();
        }

        #endregion

        #region Busca os funcionários para o filtro da tela de consulta de log

        /// <summary>
        /// Busca os funcionários para o filtro da tela de consulta de log.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroCanc"></param>
        /// <param name="exibirAdmin"></param>
        /// <returns></returns>
        public KeyValuePair<uint, string>[] GetFuncionarios(int tabela, uint idRegistroCanc, bool exibirAdmin)
        {
            string funcionarios = GetValoresCampo(Sql(tabela, idRegistroCanc, exibirAdmin, "concat(cast(idFuncCanc as " +
                "char), ';', f.nome) as campo", null, 0, null, null, null, true, false), "campo").ToString();

            List<KeyValuePair<uint, string>> retorno = new List<KeyValuePair<uint, string>>();

            if (!String.IsNullOrEmpty(funcionarios))
                foreach (string dadosFunc in funcionarios.Split(','))
                    if (!String.IsNullOrEmpty(dadosFunc))
                    {
                        string[] func = dadosFunc.Split(';');
                        retorno.Add(new KeyValuePair<uint, string>(Glass.Conversoes.StrParaUint(func[0]), func[1]));
                    }

            retorno.Sort(new Comparison<KeyValuePair<uint, string>>(
                delegate(KeyValuePair<uint, string> x, KeyValuePair<uint, string> y)
                {
                    return x.Value.CompareTo(y.Value);
                }
            ));

            return retorno.ToArray();
        }

        #endregion

        #region Cadastro de itens

        #region Métodos de suporte

        /// <summary>
        /// Retorna o número do evento de cancelamento para um registro de uma tabela.
        /// </summary>
        private uint GetNumEvento(LogCancelamento.TabelaCancelamento tabela, int idRegistroCanc)
        {
            return GetNumEvento(null, tabela, idRegistroCanc);
        }

        /// <summary>
        /// Retorna o número do evento de cancelamento para um registro de uma tabela.
        /// </summary>
        private uint GetNumEvento(GDASession session, LogCancelamento.TabelaCancelamento tabela, int idRegistroCanc)
        {
            if (idRegistroCanc < 0)
                return 1;

            string sql = @"select coalesce(max(numEvento), 0) + 1 from log_cancelamento
                where tabela=" + (int)tabela + " and idRegistroCanc=" + idRegistroCanc;

            return ExecuteScalar<uint>(session, sql);
        }

        /// <summary>
        /// Recupera o atributo de Log de uma propriedade.
        /// </summary>
        /// <param name="propriedade"></param>
        /// <returns></returns>
        private LogAttribute GetAttribute(PropertyInfo propriedade)
        {
            LogAttribute[] atributos = propriedade.GetCustomAttributes(typeof(LogAttribute), false) as LogAttribute[];
            return atributos.Length > 0 && (atributos[0].TipoLog == TipoLog.Ambos ||
                atributos[0].TipoLog == TipoLog.Cancelamento) ? atributos[0] : null;
        }

        /// <summary>
        /// Recupera todas as propriedades que possuem o atributo de Log.
        /// </summary>
        /// <param name="modelo">O item que terá as propriedades retornadas.</param>
        /// <returns></returns>
        private LogAlteracaoDAO.PropriedadeLog[] GetPropriedades(object modelo)
        {
            // Recupera todas as propriedades do item
            List<PropertyInfo> lista = new List<PropertyInfo>(modelo.GetType().GetProperties());
            List<LogAlteracaoDAO.PropriedadeLog> retorno = new List<LogAlteracaoDAO.PropriedadeLog>();

            // Remove as propriedades que não possuem o atributo de Log
            for (int i = 0; i < lista.Count; i++)
            {
                LogAttribute a = GetAttribute(lista[i]);
                if (a != null)
                    retorno.Add(new LogAlteracaoDAO.PropriedadeLog(lista[i], a));
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Cria o Log de cancelamento de um objeto.
        /// </summary>
        /// <param name="tabela">A tabela que está sendo alterada.</param>
        /// <param name="id">O ID do item que está sendo cancelado.</param>
        /// <param name="item">O item que está sendo cancelado.</param>
        /// <param name="motivo">O motivo do cancelamento.</param>
        /// <param name="manual">O cancelamento foi manual?</param>
        private void InserirLog(uint idFunc, LogCancelamento.TabelaCancelamento tabela, uint id, object item, string motivo, bool manual)
        {
            InserirLog(null, idFunc, tabela, id, item, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de cancelamento de um objeto.
        /// </summary>
        /// <param name="tabela">A tabela que está sendo alterada.</param>
        /// <param name="id">O ID do item que está sendo cancelado.</param>
        /// <param name="item">O item que está sendo cancelado.</param>
        /// <param name="motivo">O motivo do cancelamento.</param>
        /// <param name="manual">O cancelamento foi manual?</param>
        private void InserirLog(GDASession sessao, uint idFunc, LogCancelamento.TabelaCancelamento tabela, uint id, object item, string motivo, bool manual)
        {
            // Cria a variável que guarda o texto que será salvo
            StringBuilder texto = new StringBuilder();

            uint numEvento = GetNumEvento(sessao, tabela, (int)id);

            // Percorre todas as propriedades que usam Log
            foreach (var p in GetPropriedades(item))
            {
                // Recupera o valor e garante que ele seja válido
                string valor = p.Atributo.GetValue(p.Propriedade, item);
                if (String.IsNullOrEmpty(valor))
                    continue;

                // Cria o log
                LogCancelamento log = new LogCancelamento();
                log.Tabela = (int)tabela;
                log.IdRegistroCanc = (int)id;
                log.NumEvento = numEvento;
                log.Motivo = motivo;
                log.IdFuncCanc = idFunc;
                log.DataCanc = DateTime.Now;
                log.CancelamentoManual = manual;
                log.Campo = p.Atributo.Campo;
                log.Valor = valor;
                log.Referencia = LogCancelamento.GetReferencia(sessao, tabela, (uint)id);

                if (log.Referencia != null)
                    log.Referencia = log.Referencia.Length <= 100 ? log.Referencia : log.Referencia.Substring(0, 97) + "...";

                Insert(sessao, log);
            }
        }

        #endregion

        /// <summary>
        /// Recupera os registro de log
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="tabela"></param>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        /// <returns></returns>
        public IEnumerable<LogCancelamento> ObtemLogs(uint idFunc, LogCancelamento.TabelaCancelamento tabela, int id, object item, string motivo, bool manual)
        {
            uint numEvento = GetNumEvento(tabela, id);

            // Percorre todas as propriedades que usam Log
            foreach (var p in GetPropriedades(item))
            {
                // Recupera o valor e garante que ele seja válido
                string valor = p.Atributo.GetValue(p.Propriedade, item);
                if (String.IsNullOrEmpty(valor))
                    continue;

                // Cria o log
                LogCancelamento log = new LogCancelamento();
                log.Tabela = (int)tabela;
                log.IdRegistroCanc = id;
                log.NumEvento = numEvento;
                log.Motivo = motivo;
                log.IdFuncCanc = UserInfo.GetUserInfo?.CodUser ?? 0;
                log.DataCanc = DateTime.Now;
                log.CancelamentoManual = manual;
                log.Campo = p.Atributo.Campo;
                log.Valor = valor;
                log.Referencia = LogCancelamento.GetReferencia(tabela, (uint)id);

                if (log.Referencia != null)
                    log.Referencia = log.Referencia.Length <= 100 ? log.Referencia : log.Referencia.Substring(0, 97) + "...";

                yield return log;
            }
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a conta a receber.
        /// </summary>
        /// <param name="contaReceber"></param>
        /// <param name="motivo"></param>
        public void LogContaReceber(ContasReceber contaReceber, string motivo, bool manual)
        {
            LogContaReceber(null, contaReceber, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a conta a receber.
        /// </summary>
        /// <param name="contaReceber"></param>
        /// <param name="motivo"></param>
        public void LogContaReceber(GDASession sessao, ContasReceber contaReceber, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ContasReceber,
                contaReceber.IdContaR, contaReceber, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a conta a pagar.
        /// </summary>
        /// <param name="contaPagar"></param>
        /// <param name="motivo"></param>
        public void LogContaPagar(GDASession sessao, ContasPagar contaPagar, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ContasPagar,
                contaPagar.IdContaPg, contaPagar, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o texto de impressão do pedido.
        /// </summary>
        public void LogTextoPedido(GDASession sessao, TextoPedido textoPedido, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.TextoPedido,
                textoPedido.IdTextoPedido, textoPedido, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a movimentação bancária.
        /// </summary>
        /// <param name="movimentacaoBancaria"></param>
        /// <param name="motivo"></param>
        public void LogMovimentacaoBancaria(GDASession sessao, MovBanco movimentacaoBancaria, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MovimentacaoBancaria,
                movimentacaoBancaria.IdMovBanco, movimentacaoBancaria, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a movimentação bancária.
        /// </summary>
        /// <param name="movimentacaoBancaria"></param>
        /// <param name="motivo"></param>
        public void LogMovimentacaoBancaria(MovBanco movimentacaoBancaria, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MovimentacaoBancaria,
                movimentacaoBancaria.IdMovBanco, movimentacaoBancaria, motivo, manual);
        }

        public void LogImpressaoEtiquetas(uint idFunc, ImpressaoEtiqueta impressaoEtiqueta, string motivo, bool manual)
        {
            LogImpressaoEtiquetas(null, idFunc, impressaoEtiqueta, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a impressão de etiquetas.
        /// </summary>
        /// <param name="impressaoEtiqueta"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogImpressaoEtiquetas(GDASession session, uint idFunc, ImpressaoEtiqueta impressaoEtiqueta, string motivo, bool manual)
        {
            InserirLog(session, idFunc, LogCancelamento.TabelaCancelamento.ImpressaoEtiqueta,
                impressaoEtiqueta.IdImpressao, impressaoEtiqueta, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a peça do modelo de projeto.
        /// </summary>
        public void LogPecaProjetoModelo(GDASession session, PecaProjetoModelo pecaProjetoModelo, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.PecaProjetoModelo,
                pecaProjetoModelo.IdPecaProjMod, pecaProjetoModelo, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o material do modelo de projeto.
        /// </summary>
        /// <param name="materialProjetoModelo"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogMaterialProjetoModelo(MaterialProjetoModelo materialProjetoModelo, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MaterialProjetoModelo,
                materialProjetoModelo.IdMaterProjMod, materialProjetoModelo, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o caixa geral.
        /// </summary>
        public void LogCaixaGeral(GDASession session, CaixaGeral caixaGeral, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.CaixaGeral,
                caixaGeral.IdCaixaGeral, caixaGeral, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o acerto.
        /// </summary>
        /// <param name="acerto"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogAcerto(GDASession sessao, Acerto acerto, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.Acerto,
                acerto.IdAcerto, acerto, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a troca/devolução.
        /// </summary>
        /// <param name="trocaDevolucao"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogTrocaDevolucao(TrocaDevolucao trocaDevolucao, string motivo, bool manual)
        {
            LogTrocaDevolucao(null, trocaDevolucao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a troca/devolução.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trocaDevolucao"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogTrocaDevolucao(GDASession session, TrocaDevolucao trocaDevolucao, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.TrocaDevolucao,
                trocaDevolucao.IdTrocaDevolucao, trocaDevolucao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o pedido.
        /// </summary>
        public void LogPedido(Pedido pedido, string motivo, bool manual)
        {
            LogPedido(null, pedido, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o pedido.
        /// </summary>
        public void LogPedido(GDASession session, Pedido pedido, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.Pedido,
                pedido.IdPedido, pedido, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a liberação de pedido.
        /// </summary>
        /// <param name="liberacaoPedido"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogLiberarPedido(GDASession sessao, LiberarPedido liberacaoPedido, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.LiberarPedido,
                liberacaoPedido.IdLiberarPedido, liberacaoPedido, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o sinal/pagamento antecipado.
        /// </summary>
        /// <param name="sinal"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogSinal(GDASession sessao, Sinal sinal, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.Sinal,
                sinal.IdSinal, sinal, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o acerto de cheque.
        /// </summary>
        /// <param name="acertoCheque"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogAcertoCheque(AcertoCheque acertoCheque, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.AcertoCheque,
                acertoCheque.IdAcertoCheque, acertoCheque, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a obra.
        /// </summary>
        /// <param name="obra"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogObra(Obra obra, string motivo, bool manual)
        {
            LogObra(null, obra, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a obra.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="obra"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogObra(GDASession sessao, Obra obra, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.Obra,
                obra.IdObra, obra, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a devolução de pagamento.
        /// </summary>
        /// <param name="devolucaoPagamento"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogDevolucaoPagamento(GDASession sessao, DevolucaoPagto devolucaoPagamento, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.DevolucaoPagamento,
                devolucaoPagamento.IdDevolucaoPagto, devolucaoPagamento, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o crédito de fornecedor.
        /// </summary>
        /// <param name="creditoFornecedor"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogCreditoFornecedor(CreditoFornecedor creditoFornecedor, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.CreditoFornecedor,
                creditoFornecedor.IdCreditoFornecedor, creditoFornecedor, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a movimentação de estoque.
        /// </summary>
        /// <param name="movEstoque"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogMovEstoque(GDASession sessao, MovEstoque movEstoque, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MovEstoque,
                movEstoque.IdMovEstoque, movEstoque, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a movimentação de estoque fiscal.
        /// </summary>
        /// <param name="movEstoqueFiscal"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogMovEstoqueFiscal(GDASession sessao, MovEstoqueFiscal movEstoqueFiscal, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MovEstoqueFiscal,
                movEstoqueFiscal.IdMovEstoqueFiscal, movEstoqueFiscal, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para uma finalização de instalação
        /// </summary>
        /// <param name="instalacao"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogFinalizacaoInstalacao(Instalacao instalacao, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.FinalizacaoInstalacao,
                instalacao.IdInstalacao, instalacao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um produto da impressão.
        /// </summary>
        /// <param name="produtoImpressao"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogProdutoImpressao(GDASession session, ProdutoImpressao produtoImpressao, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ProdutoImpressao,
                 (uint)produtoImpressao.IdProdImpressao, produtoImpressao, motivo, manual);
        }

        public void LogRetalhoProducao(uint idFunc, RetalhoProducao retalho, string motivo, bool manual)
        {
            LogRetalhoProducao(null, idFunc, retalho, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um retalho de produção.
        /// </summary>
        /// <param name="retalho"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogRetalhoProducao(GDASession session, uint idFunc, RetalhoProducao retalho, string motivo, bool manual)
        {
            InserirLog(session, idFunc, LogCancelamento.TabelaCancelamento.RetalhoProducao,
                (uint)retalho.IdRetalhoProducao, retalho, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um sinal da compra.
        /// </summary>
        public void LogSinalCompra(GDASession session, SinalCompra sinalCompra, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.SinalCompra,
                sinalCompra.IdSinalCompra, sinalCompra, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para uma antecipação de fornecedor.
        /// </summary>
        public void LogAntecipFornec(GDASession session, AntecipacaoFornecedor antecipFornec, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.AntecipFornec,
                antecipFornec.IdAntecipFornec, antecipFornec, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um encontro de contas.
        /// </summary>
        public void LogEncontroContas(EncontroContas encontroContas, string motivo, bool manual)
        {
            LogEncontroContas(null, encontroContas, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um encontro de contas.
        /// </summary>
        public void LogEncontroContas(GDASession session, EncontroContas encontroContas, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.EncontroContas,
                encontroContas.IdEncontroContas, encontroContas, motivo, manual);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cria o Log de Cancelamento para um depósito não identificado.
        /// </summary>
        /// <param name="depositoNaoIdentificado"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogDepostioNaoIdentificado(DepositoNaoIdentificado depositoNaoIdentificado, string motivo, bool manual)
        {
            LogDepostioNaoIdentificado(null, depositoNaoIdentificado, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um depósito não identificado.
        /// </summary>
        /// <param name="depositoNaoIdentificado"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogDepostioNaoIdentificado(GDASession sessao, DepositoNaoIdentificado depositoNaoIdentificado, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.DepositoNaoIdentificado,
                depositoNaoIdentificado.IdDepositoNaoIdentificado, depositoNaoIdentificado, motivo, manual);
        }

        public void LogCartaoNaoIdentificado(GDASession sessao, CartaoNaoIdentificado cartaoNaoIdentificado, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.CartaoNaoIdentificado,
                (uint)cartaoNaoIdentificado.IdCartaoNaoIdentificado, cartaoNaoIdentificado, motivo, manual);
        }

        public void LogArquivoCartaoNaoIdentificado(GDASession sessao, ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ArquivoCartaoNaoIdentificado,
                (uint)arquivoCartaoNaoIdentificado.IdArquivoCartaoNaoIdentificado, arquivoCartaoNaoIdentificado, motivo, manual);
        }

        public void LogImpostoServ(GDASession sessao, ImpostoServ impostoServ, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ImpostoServico,
                (uint)impostoServ.IdImpostoServ, impostoServ, motivo, manual);
        }


        /// <summary>
        /// Cria o Log de Cancelamento para uma cotação de compras.
        /// </summary>
        /// <param name="cotacaoCompra"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogCotacaoCompras(CotacaoCompra cotacaoCompra, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.CotacaoCompras,
                cotacaoCompra.IdCotacaoCompra, cotacaoCompra, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para uma conciliação bancária.
        /// </summary>
        /// <param name="conciliacaoBancaria"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogConciliacaoBancaria(ConciliacaoBancaria conciliacaoBancaria, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ConciliacaoBancaria,
                conciliacaoBancaria.IdConciliacaoBancaria, conciliacaoBancaria, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para uma regra de natureza de operação.
        /// </summary>
        /// <param name="regraNaturezaOperacao"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogRegraNaturezaOperacao(RegraNaturezaOperacao regraNaturezaOperacao, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.RegraNaturezaOperacao,
                (uint)regraNaturezaOperacao.IdRegraNaturezaOperacao, regraNaturezaOperacao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a movimentação de estoque de cliente.
        /// </summary>
        /// <param name="movEstoqueCliente"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogMovEstoqueCliente(GDASession sessao, MovEstoqueCliente movEstoqueCliente, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.MovEstoqueCliente,
                movEstoqueCliente.IdMovEstoqueCliente, movEstoqueCliente, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o inventário de estoque.
        /// </summary>
        /// <param name="inventario"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogInventarioEstoque(InventarioEstoque inventario, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.InventarioEstoque,
                inventario.IdInventarioEstoque, inventario, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um controle de créditos.
        /// </summary>
        /// <param name="controleCredito"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogControleCreditos(ControleCreditoEfd controleCredito, string motivo, bool manual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ControleCreditosEfd,
                controleCredito.IdCredito, controleCredito, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para instalações.
        /// </summary>
        public void LogInstalacao(Instalacao instalacao, string motivo, bool manual)
        {
            LogInstalacao(null, instalacao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para instalações.
        /// </summary>
        public void LogInstalacao(GDASession session, Instalacao instalacao, string motivo, bool manual)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.Instalacao,
                instalacao.IdInstalacao, instalacao, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a conta a receber.
        /// </summary>
        /// <param name="contaReceber"></param>
        /// <param name="motivo"></param>
        public void LogPerdaChapaVidro(GDASession sessao, PerdaChapaVidro pcv)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.PerdaChapaVidro, pcv.IdPerdaChapaVidro, pcv, "", false);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para um deposito de cheque.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="depositoCheque"></param>
        /// <param name="motivo"></param>
        public void LogDepostioCheque(GDASession sessao, DepositoCheque depositoCheque, string motivo)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.DepositoCheque,
                depositoCheque.IdDeposito, depositoCheque, motivo, true);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o pedido da ordem de carga.
        /// </summary>
        /// <param name="acerto"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogPedidoOC(GDASession sessao, PedidoOrdemCarga pedidoOC, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.PedidoOC,
                pedidoOC.IdOrdemCarga, pedidoOC, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a ordem de carga.
        /// </summary>
        /// <param name="acerto"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogOrdemCarga(GDASession sessao, OrdemCarga oc, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.OrdemCarga,
                oc.IdCarregamento.GetValueOrDefault(0), oc, motivo, manual);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para a ordem de carga.
        /// </summary>
        public void LogMedicao(GDASession sessao, Medicao medicao, string motivo, bool manual)
        {
            uint numEvento = GetNumEvento(sessao, LogCancelamento.TabelaCancelamento.Medicao, (int)medicao.IdMedicao);
            // Cria o log
            LogCancelamento log = new LogCancelamento();
            log.Tabela = (int)LogCancelamento.TabelaCancelamento.Medicao;
            log.IdRegistroCanc = (int)medicao.IdMedicao;
            log.NumEvento = numEvento;
            log.Motivo = motivo != null && motivo.Length > 200 ? motivo.Substring(0, 200) : motivo;
            log.IdFuncCanc = UserInfo.GetUserInfo.CodUser;
            log.DataCanc = DateTime.Now;
            log.CancelamentoManual = manual;
            log.Campo = "Situacao";
            log.Valor = Medicao.SituacaoMedicao.Cancelada.ToString();
            log.Referencia = LogCancelamento.GetReferencia(sessao, LogCancelamento.TabelaCancelamento.Medicao, medicao.IdMedicao);

            Insert(sessao, log);
        }

        /// <summary>
        /// Cria o Log de Cancelamento para o imposto/serv
        /// </summary>
        /// <param name="impostoServ"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        public void LogImpostoServico(GDASession sessao, ImpostoServ impostoServ, string motivo, bool manual)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogCancelamento.TabelaCancelamento.ImpostoServico,
                impostoServ.IdImpostoServ, impostoServ, motivo, manual);
        }

        #endregion

        #region Verifica se há log para um registro de uma tabela

        /// <summary>
        /// Verifica se há log para um registro de uma tabela.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroCanc"></param>
        /// <returns></returns>
        public bool TemRegistro(LogCancelamento.TabelaCancelamento tabela, uint idRegistroCanc)
        {
            return GetCount((int)tabela, idRegistroCanc, UserInfo.GetUserInfo.IsAdminSync, null, 0, null, null, null, true) > 0;
        }

        #endregion
    }
}
