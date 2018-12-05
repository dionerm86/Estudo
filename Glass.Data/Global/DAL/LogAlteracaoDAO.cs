using System;
using System.Collections.Generic;
using Glass.Data.Model;
using System.Reflection;
using Glass.Data.Helper;
using GDA;
using Glass.Log;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class LogAlteracaoDAO : BaseDAO<LogAlteracao, LogAlteracaoDAO>
    {
        //private LogAlteracaoDAO() { }

        public enum SequenciaObjeto
        {
            Atual,
            Novo
        }

        #region Classe de suporte

        internal class PropriedadeLog
        {
            public PropriedadeLog(PropertyInfo propriedade, LogAttribute atributo)
            {
                Propriedade = propriedade;
                Atributo = atributo;
            }

            public PropertyInfo Propriedade { get; private set; }
            public LogAttribute Atributo { get; private set; }
        }

        #endregion

        #region Busca padrão

        private string Sql(int tabela, uint idRegistroAlt, bool exibirAdmin, string campoBuscar, string campo,
            string dataIni, string dataFim, bool selecionar, bool buscarVazio)
        {
            string campos = !String.IsNullOrEmpty(campoBuscar) ? campoBuscar :
                selecionar ? "l.*, f.nome as nomeFuncAlt" : "count(*)";

            string sql = @"
                select " + campos + @"
                from log_alteracao l
                    left join funcionario f on (l.idFuncAlt=f.idFunc)
                where 1";

            if (tabela > 0)
                sql += " and l.tabela=" + tabela;

            if (idRegistroAlt > 0 || buscarVazio)
                sql += " and l.idRegistroAlt=" + idRegistroAlt;

            if (!exibirAdmin)
                sql += " and l.idFuncAlt not in (" + FuncionarioDAO.Instance.SqlAdminSync(null, true) + ")";

            if (!String.IsNullOrEmpty(campo))
                sql += " and l.campo=?campo";

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and l.dataAlt>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and l.dataAlt<=?dataFim";

            return sql;
        }

        private GDAParameter[] GetParams(string campo, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(campo))
                lst.Add(new GDAParameter("?campo", campo));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<LogAlteracao> GetList(int tabela, uint idRegistroAlt, bool exibirAdmin, string campo, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "l.dataAlt desc";
            return LoadDataWithSortExpression(Sql(tabela, idRegistroAlt, exibirAdmin, null, campo, null, null, true, false), sortExpression, startRow, pageSize,
                GetParams(campo, null, null));
        }

        public int GetCount(int tabela, uint idRegistroAlt, bool exibirAdmin, string campo)
        {
            return GetCount(tabela, idRegistroAlt, exibirAdmin, campo, false);
        }

        /// <summary>
        /// Obtém a quantidade de logs de alteração, que correspondem aos filtros informados.
        /// </summary>
        /// <param name="tabela">tabela.</param>
        /// <param name="idRegistroAlt">idRegistroAlt.</param>
        /// <param name="exibirAdmin">exibirAdmin.</param>
        /// <param name="campo">campo.</param>
        /// <param name="buscarVazio">buscarVazio.</param>
        /// <returns>Retorna a quantidade de logs de alteração, que correspondem aos filtros informados.</returns>
        public int GetCount(int tabela, uint idRegistroAlt, bool exibirAdmin, string campo, bool buscarVazio)
        {
            var sqlLogAlteracao = this.Sql(tabela, idRegistroAlt, exibirAdmin, null, campo, null, null, false, buscarVazio);
            var parametrosConsulta = this.GetParams(campo, null, null);
            var retorno = this.objPersistence.ExecuteSqlQueryCount(sqlLogAlteracao, parametrosConsulta);

            return retorno;
        }

        #endregion

        #region Verifica se há log para um registro de uma tabela

        /// <summary>
        /// Verifica se há log para um registro de uma tabela.
        /// </summary>
        public bool TemRegistro(LogAlteracao.TabelaAlteracao tabela, uint idRegistroAlt, string campo)
        {
            return GetCount((int)tabela, idRegistroAlt, UserInfo.GetUserInfo.IsAdminSync, campo, true) > 0;
        }

        #endregion

        #region Busca por período

        private void GetByEFD(ref List<LogAlteracao> itens)
        {
            if (itens == null || itens.Count == 0)
                return;

            Type tipo = GetType((LogAlteracao.TabelaAlteracao)itens[0].Tabela);
            for (int i = itens.Count - 1; i >= 0; i--)
            {
                var p = GetPropriedadeFromLog(tipo, itens[i].Campo);

                if (p == null || !p.Atributo.IsCampoEFD)
                    itens.RemoveAt(i);
            }
        }

        public LogAlteracao[] GetByItem(LogAlteracao.TabelaAlteracao tabela, uint idRegistroAlt, DateTime dataIni, DateTime dataFim, bool forEfd)
        {
            return GetByItem(tabela, idRegistroAlt, dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), forEfd);
        }

        public LogAlteracao[] GetByItem(LogAlteracao.TabelaAlteracao tabela, uint idRegistroAlt, string dataIni, string dataFim, bool forEfd)
        {
            string sql = Sql((int)tabela, idRegistroAlt, true, null, null, dataIni, dataFim, true, true) + " order by l.dataAlt desc";
            if (forEfd)
                sql = "select * from (" + sql + ") as temp group by campo, date(dataAlt) order by dataAlt desc";

            List<LogAlteracao> retorno = objPersistence.LoadData(sql, GetParams(null, dataIni, dataFim));

            if (forEfd)
                GetByEFD(ref retorno);

            return retorno.ToArray();
        }

        #endregion

        #region Busca o nome dos campos para o filtro da tela de consulta de log

        /// <summary>
        /// Busca o nome dos campos para o filtro da tela de consulta de log.
        /// </summary>
        public KeyValuePair<string, string>[] GetCampos(int tabela, uint idRegistroAlt, string campo, bool exibirAdmin)
        {
            string campos = GetValoresCampo(Sql(tabela, idRegistroAlt, exibirAdmin, "campo", campo, null,
                null, true, false), "campo", GetParams(campo, null, null)).ToString();

            List<KeyValuePair<string, string>> retorno = new List<KeyValuePair<string, string>>();

            if (!String.IsNullOrEmpty(campos))
                foreach (string c in campos.Split(','))
                    if (!String.IsNullOrEmpty(c))
                        retorno.Add(new KeyValuePair<string, string>(c, c));

            retorno.Sort(new Comparison<KeyValuePair<string, string>>(
                delegate(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
                {
                    return x.Key.CompareTo(y.Key);
                }
            ));

            return retorno.ToArray();
        }

        #endregion

        #region Busca a última data de alteração de uma tabela, registro e campo

        internal string SqlDataAlt(int tabela, string idRegistroAlt, string campo, string sufixoNomeParam, object[] valores,
            out List<GDAParameter> param, bool apenasUltima)
        {
            param = new List<GDAParameter>();

            string sql = "select " + (String.IsNullOrEmpty(idRegistroAlt) ? "idRegistroAlt, " : "") +
                "dataAlt from log_alteracao where tabela=" + tabela + " and campo=?campo" + sufixoNomeParam;

            if (!String.IsNullOrEmpty(idRegistroAlt))
                sql += " and idRegistroAlt=" + idRegistroAlt;

            param.Add(new GDAParameter("?campo" + sufixoNomeParam, campo));

            if (valores != null && valores.Length > 0)
            {
                string sqlValores = "";

                for (int i = 0; i < valores.Length; i++)
                {
                    sqlValores += "?valor" + i + sufixoNomeParam + ", ";
                    param.Add(new GDAParameter("?valor" + i + sufixoNomeParam, valores[i]));
                }

                sql += " and valorAtual in (" + sqlValores.TrimEnd(',', ' ') + ")";
            }

            if (apenasUltima)
                sql += " order by dataAlt desc limit 1";

            return sql;
        }

        /// <summary>
        /// Retorna a data da última alteração feita em um campo de um registro.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroAlt"></param>
        /// <param name="campo"></param>
        /// <returns></returns>
        public DateTime? GetDataUltimaAlt(int tabela, uint idRegistroAlt, string campo, params object[] valores)
        {
            List<GDAParameter> p;
            return ExecuteScalar<DateTime?>(SqlDataAlt(tabela, idRegistroAlt.ToString(), campo, "", valores, out p, true), p.ToArray());
        }

        #endregion

        #region Cadastro de itens

        #region Métodos de suporte

        /// <summary>
        /// Retorna o número do evento de alteração para um registro de uma tabela.
        /// </summary>
        public uint GetNumEvento(LogAlteracao.TabelaAlteracao tabela, int idRegistroAlt)
        {
            return GetNumEvento(null, tabela, idRegistroAlt);
        }

        /// <summary>
        /// Retorna o número do evento de alteração para um registro de uma tabela.
        /// </summary>
        public uint GetNumEvento(GDASession session, LogAlteracao.TabelaAlteracao tabela, int idRegistroAlt)
        {
            if (idRegistroAlt < 0)
                return 1;

            string sql = @"select coalesce(max(numEvento), 0) + 1 from log_alteracao
                where tabela=" + (int)tabela + " and idRegistroAlt=" + idRegistroAlt;

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
                atributos[0].TipoLog == TipoLog.Atualizacao) ? atributos[0] : null;
        }

        /// <summary>
        /// Recupera todas as propriedades que possuem o atributo de Log.
        /// </summary>
        /// <param name="modelo">O item que terá as propriedades retornadas.</param>
        /// <returns></returns>
        private PropriedadeLog[] GetPropriedades(object modelo)
        {
            return GetPropriedades(modelo.GetType());
        }

        /// <summary>
        /// Recupera todas as propriedades que possuem o atributo de Log.
        /// </summary>
        /// <param name="tipo">O tipo do item que terá as propriedades retornadas.</param>
        /// <returns></returns>
        private PropriedadeLog[] GetPropriedades(Type tipo)
        {
            // Recupera todas as propriedades do item
            List<PropertyInfo> lista = new List<PropertyInfo>(tipo.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            List<PropriedadeLog> retorno = new List<PropriedadeLog>();

            // Ignora as propriedades que não possuem o atributo de Log
            for (int i = 0; i < lista.Count; i++)
            {
                LogAttribute a = GetAttribute(lista[i]);
                if (a != null)
                    retorno.Add(new PropriedadeLog(lista[i], a));
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Cria o Log de alterações de um objeto com base em outro.
        /// </summary>
        /// <param name="tabela">O código da tabela que está sendo comparada.</param>
        /// <param name="id">O ID do item que está sendo alterado.</param>
        /// <param name="atual">O item atual (salvo atualmente no banco de dados).</param>
        /// <param name="novo">O item novo (que será salvo no banco de dados).</param>
        private void InserirLog(uint idFunc, LogAlteracao.TabelaAlteracao tabela, uint id, object atual, object novo)
        {
            InserirLog(null, idFunc, tabela, id, atual, novo);
        }

        /// <summary>
        /// Recupera os dados do log.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="tabela"></param>
        /// <param name="id"></param>
        /// <param name="atual"></param>
        /// <param name="novo"></param>
        /// <returns></returns>
        public IEnumerable<LogAlteracao> ObtemLogs(uint idFunc, LogAlteracao.TabelaAlteracao tabela, int id, object atual, object novo, string referencia)
        {
            uint numEvento = GetNumEvento(tabela, id);

            // Percorre todas as propriedades que usam Log
            foreach (var p in GetPropriedades(atual))
            {
                // Cria um objeto comparador
                object comparador = typeof(Comparer<>).MakeGenericType(p.Propriedade.PropertyType).GetProperty("Default").GetValue(null, null);

                // Recupera o valor que será comparado
                object valorAtual = p.Propriedade.GetValue(atual, null);
                object valorNovo = novo != null ? p.Propriedade.GetValue(novo, null) : null;

                // Compara os 2 valores
                int c = (int)comparador.GetType().GetMethod("Compare").Invoke(comparador, new object[] { valorAtual, valorNovo });

                // Verifica se houve alteração de valor
                if (c != 0)
                {
                    // Recupera os valores (usado para buscar descrição de outros itens)
                    valorAtual = p.Atributo.GetValue(p.Propriedade, atual);
                    valorNovo = p.Atributo.GetValue(p.Propriedade, novo);

                    // Cria o Log
                    LogAlteracao log = new LogAlteracao();
                    log.Tabela = (int)tabela;
                    log.IdRegistroAlt = id;
                    log.NumEvento = numEvento;
                    log.Campo = p.Atributo.Campo;
                    log.DataAlt = DateTime.Now;
                    log.IdFuncAlt = idFunc;
                    log.ValorAnterior = valorAtual != null ? valorAtual.ToString() : null;
                    log.ValorAtual = valorNovo != null ? valorNovo.ToString() : null;
                    log.Referencia = referencia ?? LogAlteracao.GetReferencia(tabela, (uint)log.IdRegistroAlt);

                    if (log.Referencia != null)
                        log.Referencia = log.Referencia.Length <= 100 ? log.Referencia : log.Referencia.Substring(0, 97) + "...";

                    yield return log;
                }
            }
        }

        /// <summary>
        /// Cria o Log de alterações de um objeto com base em outro.
        /// </summary>
        /// <param name="tabela">O código da tabela que está sendo comparada.</param>
        /// <param name="id">O ID do item que está sendo alterado.</param>
        /// <param name="atual">O item atual (salvo atualmente no banco de dados).</param>
        /// <param name="novo">O item novo (que será salvo no banco de dados).</param>
        private void InserirLog(GDASession sessao, uint idFunc, LogAlteracao.TabelaAlteracao tabela, uint id, object atual, object novo)
        {
            uint numEvento = GetNumEvento(sessao, tabela, (int)id);

            // Percorre todas as propriedades que usam Log
            foreach (var p in GetPropriedades(atual))
            {
                // Cria um objeto comparador
                object comparador = typeof(Comparer<>).MakeGenericType(p.Propriedade.PropertyType).GetProperty("Default").GetValue(null, null);

                // Recupera o valor que será comparado
                object valorAtual = p.Propriedade.GetValue(atual, null);
                object valorNovo = novo != null ? p.Propriedade.GetValue(novo, null) : null;

                // Compara os 2 valores
                int c = (int)comparador.GetType().GetMethod("Compare").Invoke(comparador, new object[] { valorAtual, valorNovo });

                // Verifica se houve alteração de valor
                if (c != 0)
                {
                    // Recupera os valores (usado para buscar descrição de outros itens)
                    valorAtual = p.Atributo.GetValue(p.Propriedade, atual);
                    valorNovo = p.Atributo.GetValue(p.Propriedade, novo);

                    // Cria o Log
                    LogAlteracao log = new LogAlteracao();
                    log.Tabela = (int)tabela;
                    log.IdRegistroAlt = (int)id;
                    log.NumEvento = numEvento;
                    log.Campo = p.Atributo.Campo;
                    log.DataAlt = DateTime.Now;
                    log.IdFuncAlt = idFunc;
                    log.ValorAnterior = valorAtual != null ? valorAtual.ToString() : null;
                    log.ValorAtual = valorNovo != null ? valorNovo.ToString() : null;
                    log.Referencia = LogAlteracao.GetReferencia(sessao, tabela, id);

                    if (log.Referencia != null)
                        log.Referencia = log.Referencia.Length <= 100 ? log.Referencia : log.Referencia.Substring(0, 97) + "...";

                    Insert(sessao, log);
                }
            }
        }

        #endregion

        /// <summary>
        /// Cria o Log de alterações para a validação de peça de projeto.
        /// </summary>
        /// <param name="validacaoPecaModelo"></param>
        public void LogValidacaoPecaModelo(ValidacaoPecaModelo validacaoPecaModelo)
        {
            var atual = ValidacaoPecaModeloDAO.Instance.GetElementByPrimaryKey((uint)validacaoPecaModelo.IdValidacaoPecaModelo);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ValidacaoPecaModelo,
                (uint)validacaoPecaModelo.IdValidacaoPecaModelo, atual, validacaoPecaModelo);
        }

        /// <summary>
        /// Cria o Log de alterações para o funcionário.
        /// </summary>
        /// <param name="func"></param>
        public void LogFuncionario(Funcionario func, SequenciaObjeto sequencia)
        {
            Funcionario outro = FuncionarioDAO.Instance.GetElement((uint)func.IdFunc);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Funcionario, (uint)func.IdFunc, outro, func);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Funcionario, (uint)func.IdFunc, func, outro);
        }

        public void LogFuncionarioSetor(int idFuncionario, string setoresRemovidos, string setoresAdicionados)
        {
            LogFuncionarioSetor(null, idFuncionario, setoresRemovidos, setoresAdicionados);
        }

        /// <summary>
        /// Cria o Log de alterações para o funcionario do Setor.
        /// </summary>
        /// <param name="transp"></param>
        public void LogFuncionarioSetor(GDASession session, int idFuncionario, string setoresRemovidos, string setoresAdicionados)
        {
            if (!string.Equals(setoresRemovidos, setoresAdicionados, StringComparison.CurrentCultureIgnoreCase))
            {
                var tabela = LogAlteracao.TabelaAlteracao.Funcionario;
                var logSetoresRemovidos = string.IsNullOrEmpty(setoresRemovidos) ? string.Empty : "Setor(es) removido(s):\n" + setoresRemovidos;
                var logSetoresAdicionados = string.IsNullOrEmpty(setoresAdicionados) ? string.Empty : "Setor(es) adicionado(s):\n" + setoresAdicionados;

                if (!string.IsNullOrEmpty(logSetoresRemovidos) || !string.IsNullOrEmpty(logSetoresAdicionados))
                {
                    var item = new LogAlteracao()
                    {
                        IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                        DataAlt = DateTime.Now,
                        Campo = "Setores Produção",
                        IdRegistroAlt = idFuncionario,
                        NumEvento = GetNumEvento(session, tabela, idFuncionario),
                        Referencia = LogAlteracao.GetReferencia(session, tabela, (uint)idFuncionario),
                        Tabela = (int)tabela,
                        ValorAnterior = logSetoresRemovidos,
                        ValorAtual = logSetoresAdicionados
                    };

                    Insert(session, item);
                }
            }
        }

        /// <summary>
        /// Cria o Log de alterações para o cliente.
        /// </summary>
        /// <param name="cli"></param>
        public void LogCliente(GDASession sessao, Cliente cli)
        {
            Cliente atual = ClienteDAO.Instance.GetElement(sessao, (uint)cli.IdCli);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Cliente, (uint)cli.IdCli, atual, cli);
        }

        /// <summary>
        /// Cria o log para alteração de situação em lote do cliente.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idFunc"></param>
        /// <param name="situacao"></param>
        /// <param name="idsCli"></param>
        public void LogSituacaoCliente(GDASession sessao, string situacao, string idsCli)
        {
            var tabela = (int)LogAlteracao.TabelaAlteracao.Cliente;

            var sql = @"
                INSERT INTO log_alteracao (Tabela, IdRegistroAlt, NumEvento, Campo, DataAlt, IdFuncAlt, ValorAnterior, ValorAtual)
                SELECT {0}, c.Id_Cli, (coalesce(max(numEvento), 0) + 1), 'Situacao', Now(), {1}, ELT(c.Situacao, 'Ativo', 'Inativo', 'Cancelado', 'Bloqueado'),  '{2}'
                FROM cliente c
	                LEFT JOIN log_alteracao la ON (tabela = {0} and idRegistroAlt = c.Id_Cli)
                WHERE c.Id_Cli IN ({3})
                GROUP by c.Id_Cli";

            sql = string.Format(sql, tabela, 0, situacao, idsCli);

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Cria o Log de alterações para o transportador.
        /// </summary>
        /// <param name="transp"></param>
        public void LogTransportador(Transportador transp)
        {
            Transportador atual = TransportadorDAO.Instance.GetElementByPrimaryKey((uint)transp.IdTransportador);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Transportador, (uint)transp.IdTransportador, atual, transp);
        }

        /// <summary>
        /// Cria o Log de alterações para o produto.
        /// </summary>
        public void LogProduto(Produto prod, SequenciaObjeto sequencia)
        {
            LogProduto(null, prod, sequencia);
        }

        /// <summary>
        /// Cria o Log de alterações para o produto.
        /// </summary>
        public void LogProduto(GDASession session, Produto prod, SequenciaObjeto sequencia)
        {
            Produto outro = ProdutoDAO.Instance.GetElementByPrimaryKey(session, (uint)prod.IdProd);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Produto, (uint)prod.IdProd, outro, prod);
            else
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Produto, (uint)prod.IdProd, prod, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para chapa de vidro.
        /// </summary>
        /// <param name="chapaVidro"></param>
        public void LogChapaVidro(ChapaVidro cv)
        {
            ChapaVidro atual = ChapaVidroDAO.Instance.GetElementByPrimaryKey(cv.IdChapaVidro);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ChapaVidro, cv.IdChapaVidro, atual, cv);
        }

        /// <summary>
        /// Cria o Log de alterações para o fornecedor.
        /// </summary>
        /// <param name="fornec"></param>
        public void LogFornecedor(Fornecedor fornec)
        {
            Fornecedor atual = FornecedorDAO.Instance.GetElementByPrimaryKey((uint)fornec.IdFornec);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Fornecedor, (uint)fornec.IdFornec, atual, fornec);
        }

        /// <summary>
        /// Cria o Log de alterações para a conta a receber.
        /// </summary>
        /// <param name="contaReceberAtual"></param>
        /// <param name="contaReceberNova"></param>
        public void LogContaReceber(ContasReceber contaReceberAtual, ContasReceber contaReceberNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ContasReceber, (uint)contaReceberAtual.IdContaR,
                contaReceberAtual, contaReceberNova);
        }

        /// <summary>
        /// Cria o Log de alterações para a conta a pagar.
        /// </summary>
        public void LogContaPagar(GDASession session, ContasPagar contaPagarAtual, ContasPagar contaPagarNova)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ContaPagar, contaPagarAtual.IdContaPg,
                contaPagarAtual, contaPagarNova);
        }

        /// <summary>
        /// Cria o Log de alterações para a configuração por loja.
        /// </summary>
        /// <param name="config"></param>
        public void LogConfigLoja(ConfiguracaoLoja config, SequenciaObjeto sequencia)
        {
            ConfiguracaoLoja outro = ConfiguracaoLojaDAO.Instance.GetElementByPrimaryKey(config.IdConfigLoja);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ConfigLoja, config.IdConfigLoja, outro, config);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ConfigLoja, config.IdConfigLoja, config, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para tipos de perda.
        /// </summary>
        /// <param name="tipoPerda"></param>
        public void LogTipoPerda(TipoPerda tipoPerda)
        {
            TipoPerda atual = TipoPerdaDAO.Instance.GetElementByPrimaryKey((uint)tipoPerda.IdTipoPerda);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.TipoPerda, (uint)tipoPerda.IdTipoPerda, atual, tipoPerda);
        }

        /// <summary>
        /// Cria o Log de alterações para o CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        public void LogCfop(Cfop cfop)
        {
            Cfop atual = CfopDAO.Instance.GetElementByPrimaryKey((uint)cfop.IdCfop);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Cfop, (uint)cfop.IdCfop, atual, cfop);
        }

        public void LogProdutoImpressao(ProdutoImpressao prodImprAtual, ProdutoImpressao prodImprNovo)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoImpressao, (uint)prodImprAtual.IdProdImpressao, prodImprAtual, prodImprNovo);
        }

        /// <summary>
        /// Cria o Log de alterações para o desconto/acréscimo por cliente.
        /// </summary>
        public void LogDescontoAcrescimoCliente(GDASession session, DescontoAcrescimoCliente descontoCli)
        {
            DescontoAcrescimoCliente novo = DescontoAcrescimoClienteDAO.Instance.GetElementByPrimaryKey(session, (uint)descontoCli.IdDesconto);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.DescontoAcrescimoCliente, (uint)descontoCli.IdDesconto, descontoCli, novo);
        }

        /// <summary>
        /// Cria o Log de alterações para o controle de usuário.
        /// </summary>
        /// <param name="funcModulo"></param>
        public void LogControleUsuario(FuncModulo funcModulo)
        {
            FuncModulo novo = FuncModuloDAO.Instance.GetByModuloFunc((uint)funcModulo.IdModulo, (uint)funcModulo.IdFunc);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ControleUsuario, funcModulo.IdLog, funcModulo, novo);
        }

        /// <summary>
        /// Cria o Log de alterações para o grupo do produto.
        /// </summary>
        /// <param name="grupoProd"></param>
        public void LogGrupoProduto(GrupoProd grupoProd)
        {
            GrupoProd atual = GrupoProdDAO.Instance.GetElementByPrimaryKey((uint)grupoProd.IdGrupoProd);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.GrupoProduto, (uint)grupoProd.IdGrupoProd, atual, grupoProd);
        }

        /// <summary>
        /// Cria o Log de alterações para o subgrupo do produto.
        /// </summary>
        /// <param name="subgrupoProd"></param>
        public void LogSubgrupoProduto(SubgrupoProd subgrupoProd)
        {
            SubgrupoProd atual = SubgrupoProdDAO.Instance.GetElementByPrimaryKey((uint)subgrupoProd.IdSubgrupoProd);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.SubgrupoProduto, (uint)subgrupoProd.IdSubgrupoProd, atual, subgrupoProd);
        }

        /// <summary>
        /// Cria o Log de alterações para a nota fiscal (alteração manual).
        /// </summary>
        /// <param name="notaFiscal"></param>
        public void LogNotaFiscal(GDASession sessao, NotaFiscal notaFiscal)
        {
            NotaFiscal atual = NotaFiscalDAO.Instance.GetElementByPrimaryKey(sessao, notaFiscal.IdNf);
            if (atual.Situacao == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros ||
                (atual.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada &&
                NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(sessao, notaFiscal.IdNf)))
            {
                InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.NotaFiscal, notaFiscal.IdNf, atual, notaFiscal);
            }
        }

        /// <summary>
        /// Cria o Log de alterações para o produto da nota fiscal (alteração manual).
        /// </summary>
        /// <param name="produtoNf"></param>
        public void LogProdutoNotaFiscal(ProdutosNf produtoNf, SequenciaObjeto sequencia)
        {
            ProdutosNf outro = ProdutosNfDAO.Instance.GetElementByPrimaryKey(produtoNf.IdProdNf);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoNotaFiscal, produtoNf.IdProdNf, outro, produtoNf);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoNotaFiscal, produtoNf.IdProdNf, produtoNf, outro);
        }

        public void LogOrcamento(Orcamento orcamento, Orcamento outro, SequenciaObjeto sequencia)
        {
            LogOrcamento(null, orcamento, outro, sequencia);
        }

        public void LogOrcamento(GDASession session, Orcamento orcamento, Orcamento outro, SequenciaObjeto sequencia)
        {
            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Orcamento, orcamento.IdOrcamento, outro, orcamento);
            else
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Orcamento, orcamento.IdOrcamento, orcamento, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para o pedido (alteração de desconto).
        /// </summary>
        /// <param name="pedido"></param>
        public void LogPedido(Pedido pedido, Pedido outro, SequenciaObjeto sequencia)
        {
            LogPedido(null, pedido, outro, sequencia);
        }

        /// <summary>
        /// Cria o Log de alterações para o pedido (alteração de desconto).
        /// </summary>
        /// <param name="pedido"></param>
        public void LogPedido(GDASession sessao, Pedido pedido, Pedido outro, SequenciaObjeto sequencia)
        {
            /* Chamado 45477. */
            if (UserInfo.GetUserInfo == null)
                throw new Exception("Não foi possível recuperar o login do usuário. Efetue o login no sistema novamente.");

            /* Chamado 62138. */
            var codUsuario = UserInfo.GetUserInfo.CodUser;
            if ((!UserInfo.GetUserInfo.IsCliente && codUsuario == 0) || (UserInfo.GetUserInfo.IsCliente && UserInfo.GetUserInfo.IdCliente == 0))
                throw new Exception("Não foi possível recuperar o login do usuário. Efetue o login no sistema novamente.");

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(sessao, codUsuario, LogAlteracao.TabelaAlteracao.Pedido, pedido.IdPedido, outro, pedido);
            else
                InserirLog(sessao, codUsuario, LogAlteracao.TabelaAlteracao.Pedido, pedido.IdPedido, pedido, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para a obra.
        /// </summary>
        public void LogObra(GDASession sessao, Obra obraAtual)
        {
            var obraNova = ObraDAO.Instance.GetElement(sessao, obraAtual.IdObra);

            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Obra, obraAtual.IdObra, obraAtual, obraNova);
        }

        /// <summary>
        /// Cria o Log de Alterações para as peças impressas do pedido.
        /// </summary>
        public void LogPedidoPecaImpressa(GDASession session, uint idFunc, int idPedido, string descricaoAntiga, string descricaoNova)
        {
            if (!string.Equals(descricaoAntiga, descricaoNova, StringComparison.CurrentCultureIgnoreCase))
            {
                var tabela = LogAlteracao.TabelaAlteracao.Pedido;
                var descricaoLog =
                    string.IsNullOrEmpty(descricaoAntiga) ?
                        "Peças impressas" :
                        string.IsNullOrEmpty(descricaoNova) ?
                            "Peças canceladas" :
                            string.Empty;

                if (!string.IsNullOrEmpty(descricaoLog))
                {
                    var item = new LogAlteracao()
                    {
                        IdFuncAlt = idFunc,
                        DataAlt = DateTime.Now,
                        Campo = "Impressão de etiquetas",
                        IdRegistroAlt = idPedido,
                        NumEvento = GetNumEvento(session, tabela, idPedido),
                        Referencia = LogAlteracao.GetReferencia(session, tabela, (uint)idPedido),
                        Tabela = (int)tabela,
                        ValorAnterior = descricaoLog,
                        ValorAtual = descricaoLog
                    };

                    Insert(session, item);
                }
            }
        }

        /// <summary>
        /// Cria o Log de alterações para o pedido espelho data fábrica
        /// </summary>
        public void LogPedidoEspelho(PedidoEspelho pedidoEspelho, SequenciaObjeto sequencia)
        {
            LogPedidoEspelho(null, pedidoEspelho, sequencia);
        }

        /// <summary>
        /// Cria o Log de alterações para o pedido espelho data fábrica
        /// </summary>
        public void LogPedidoEspelho(GDASession session, PedidoEspelho pedidoEspelho, SequenciaObjeto sequencia)
        {
            PedidoEspelho outro = PedidoEspelhoDAO.Instance.GetElement(session, pedidoEspelho.IdPedido);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PedidoEspelho, pedidoEspelho.IdPedido, outro, pedidoEspelho);
            else
                InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PedidoEspelho, pedidoEspelho.IdPedido, pedidoEspelho, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para o cheque.
        /// </summary>
        /// <param name="cheque"></param>
        public void LogCheque(Cheques cheque, SequenciaObjeto sequencia)
        {
            LogCheque(null, cheque, sequencia);
        }

        /// <summary>
        /// Cria o Log de alterações para o cheque.
        /// </summary>
        /// <param name="cheque"></param>
        public void LogCheque(GDASession sessao, Cheques cheque, SequenciaObjeto sequencia)
        {
            Cheques outro = ChequesDAO.Instance.GetElementByPrimaryKey(cheque.IdCheque);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Cheque, cheque.IdCheque, outro, cheque);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Cheque, cheque.IdCheque, cheque, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para o pagamento.
        /// </summary>
        /// <param name="pagto"></param>
        public void LogPagto(Pagto pagto)
        {
            Pagto novo = PagtoDAO.Instance.GetForLog(pagto.IdPagto);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Pagto, pagto.IdPagto, pagto, novo);
        }

        /// <summary>
        /// Cria o Log de alterações para as unidades de medida.
        /// </summary>
        /// <param name="grupoProd"></param>
        public void LogUnidadeMedida(UnidadeMedida unidadeMedida)
        {
            UnidadeMedida atual = UnidadeMedidaDAO.Instance.GetElementByPrimaryKey((uint)unidadeMedida.IdUnidadeMedida);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.UnidadeMedida, (uint)unidadeMedida.IdUnidadeMedida, atual, unidadeMedida);
        }

        /// <summary>
        /// Cria o Log de alterações para um produto no estoque.
        /// </summary>
        public void LogProdutoLoja(ProdutoLoja produtoLoja)
        {
            LogProdutoLoja(null, produtoLoja);
        }

        /// <summary>
        /// Cria o Log de alterações para um produto no estoque.
        /// </summary>
        public void LogProdutoLoja(GDASession session, ProdutoLoja produtoLoja)
        {
            ProdutoLoja novo = ProdutoLojaDAO.Instance.GetElement(session, (uint)produtoLoja.IdLoja, (uint)produtoLoja.IdProd);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoLoja, produtoLoja.IdLog, produtoLoja, novo);
        }

        /// <summary>
        /// Cria o Log de alterações para um plano de conta contábil.
        /// </summary>
        /// <param name="planoContaContabil"></param>
        /// <param name="sequencia"></param>
        public void LogPlanoContaContabil(PlanoContaContabil planoContaContabil, SequenciaObjeto sequencia)
        {
            PlanoContaContabil outro = PlanoContaContabilDAO.Instance.GetElementByPrimaryKey(planoContaContabil.IdContaContabil);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PlanoContaContabil, (uint)planoContaContabil.IdContaContabil, outro, planoContaContabil);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PlanoContaContabil, (uint)planoContaContabil.IdContaContabil, planoContaContabil, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para um centro de custos.
        /// </summary>
        /// <param name="centroCusto"></param>
        /// <param name="sequencia"></param>
        public void LogCentroCusto(CentroCusto centroCusto, SequenciaObjeto sequencia)
        {
            CentroCusto outro = CentroCustoDAO.Instance.GetElementByPrimaryKey((uint)centroCusto.IdCentroCusto);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.CentroCusto, (uint)centroCusto.IdCentroCusto, outro, centroCusto);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.CentroCusto, (uint)centroCusto.IdCentroCusto, centroCusto, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para o bem ativo imobilizado.
        /// </summary>
        /// <param name="bemAtivoImobilizado"></param>
        /// <param name="sequencia"></param>
        public void LogBemAtivoImobilizado(BemAtivoImobilizado bemAtivoImobilizado, SequenciaObjeto sequencia)
        {
            BemAtivoImobilizado outro = BemAtivoImobilizadoDAO.Instance.GetElementByPrimaryKey(bemAtivoImobilizado.IdBemAtivoImobilizado);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.BemAtivoImobilizado, bemAtivoImobilizado.IdBemAtivoImobilizado, outro, bemAtivoImobilizado);
            else
                InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.BemAtivoImobilizado, bemAtivoImobilizado.IdBemAtivoImobilizado, bemAtivoImobilizado, outro);
        }

        /// <summary>
        /// Cria o Log de alterações para a movimentação do bem ativo imobilizado.
        /// </summary>
        /// <param name="movimentacaoBemAtivoImob"></param>
        public void LogMovimentacaoBemAtivoImob(MovimentacaoBemAtivoImob movimentacaoBemAtivoImob)
        {
            MovimentacaoBemAtivoImob atual = MovimentacaoBemAtivoImobDAO.Instance.GetElement(movimentacaoBemAtivoImob.IdProdNf);
            if (atual == null)
                atual = new MovimentacaoBemAtivoImob();

            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MovBemAtivoImobilizado, movimentacaoBemAtivoImob.IdProdNf, atual, movimentacaoBemAtivoImob);
        }

        /// <summary>
        /// Cria o Log de alterações para a loja.
        /// </summary>
        /// <param name="loja"></param>
        public void LogLoja(Loja loja)
        {
            Loja atual = LojaDAO.Instance.GetElementByPrimaryKey((uint)loja.IdLoja);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Loja, (uint)loja.IdLoja, atual, loja);
        }

        /// <summary>
        /// Cria o Log de alterações para a administradora de cartão.
        /// </summary>
        /// <param name="adminCartao"></param>
        public void LogAdministradoraCartao(AdministradoraCartao adminCartao)
        {
            AdministradoraCartao atual = AdministradoraCartaoDAO.Instance.GetElementByPrimaryKey(adminCartao.IdAdminCartao);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.AdministradoraCartao, adminCartao.IdAdminCartao, atual, adminCartao);
        }

        /// <summary>
        /// Cria o Log de alterações para o controle de configuração de comissão
        /// </summary>
        public void LogComissaoConfig(ComissaoConfig comissaoConfig)
        {
            ComissaoConfig atual = ComissaoConfigDAO.Instance.GetElementByPrimaryKey(comissaoConfig.IdComissaoConfig);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ComissaoConfig, comissaoConfig.IdComissaoConfig, atual, comissaoConfig);
        }

        /// <summary>
        /// Cria o Log de alterações para o modelo de projeto.
        /// </summary>
        public void LogProjetoModelo(GDASession session, ProjetoModelo projetoModelo)
        {
            var atual = ProjetoModeloDAO.Instance.GetElement(session, projetoModelo.IdProjetoModelo);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProjetoModelo, projetoModelo.IdProjetoModelo, atual, projetoModelo);
        }

        /// <summary>
        /// Cria o Log de Alterações para a posição da peça do modelo de projeto.
        /// </summary>
        /// <param name="atual"></param>
        /// <param name="posicaoPecaModelo"></param>
        public void LogPosicaoPecaModelo(PosicaoPecaModelo atual, PosicaoPecaModelo posicaoPecaModelo)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PosicaoPecaModelo, posicaoPecaModelo.IdPosicaoPecaModelo, atual, posicaoPecaModelo);
        }

        /// <summary>
        /// Cria o Log de Alterações para a peça do modelo de projeto.
        /// </summary>
        public void LogPecaProjetoModelo(GDASession session, PecaProjetoModelo atual, PecaProjetoModelo pecaProjetoModelo)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PecaProjetoModelo, pecaProjetoModelo.IdPecaProjMod, atual, pecaProjetoModelo);
        }

        /// <summary>
        /// Cria o Log de Alterações para o material do modelo de projeto.
        /// </summary>
        /// <param name="materialProjetoModelo"></param>
        public void LogMaterialProjetoModelo(MaterialProjetoModelo materialProjetoModelo)
        {
            MaterialProjetoModelo atual = MaterialProjetoModeloDAO.Instance.GetElementByPrimaryKey(materialProjetoModelo.IdMaterProjMod);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MaterialProjetoModelo, materialProjetoModelo.IdMaterProjMod, atual, materialProjetoModelo);
        }

        /// <summary>
        /// Cria o Log de Alterações para o grupo de modelos de projeto.
        /// </summary>
        /// <param name="grupoModelo"></param>
        public void LogGrupoModelo(GrupoModelo grupoModelo)
        {
            GrupoModelo atual = GrupoModeloDAO.Instance.GetElementByPrimaryKey(grupoModelo.IdGrupoModelo);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.GrupoModelo, grupoModelo.IdGrupoModelo, atual, grupoModelo);
        }

        /// <summary>
        /// Cria o Log de Alterações para a posição da peça individual do modelo de projeto.
        /// </summary>
        /// <param name="atual"></param>
        /// <param name="posicaoPecaIndividual"></param>
        public void LogPosicaoPecaIndividual(PosicaoPecaIndividual atual, PosicaoPecaIndividual posicaoPecaIndividual)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual, posicaoPecaIndividual.IdPosPecaInd, atual, posicaoPecaIndividual);
        }

        /// <summary>
        /// Cria o Log de Alterações para o produto do projeto.
        /// </summary>
        /// <param name="produtoProjeto"></param>
        public void LogProdutoProjeto(ProdutoProjeto produtoProjeto)
        {
            ProdutoProjeto atual = ProdutoProjetoDAO.Instance.GetElementByPrimaryKey(produtoProjeto.IdProdProj);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoProjeto, produtoProjeto.IdProdProj, atual, produtoProjeto);
        }

        /// <summary>
        /// Cria o Log de Alterações para o produto vinculado do projeto.
        /// </summary>
        /// <param name="produtoProjetoConfig"></param>
        public void LogProdutoProjetoConfig(ProdutoProjetoConfig produtoProjetoConfig)
        {
            ProdutoProjetoConfig novo = ProdutoProjetoConfigDAO.Instance.GetElement(produtoProjetoConfig.IdProdProjConfig);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoProjetoConfig, produtoProjetoConfig.IdProdProjConfig, produtoProjetoConfig, novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para a medida do projeto.
        /// </summary>
        /// <param name="medidaProjeto"></param>
        public void LogMedidaProjeto(GDASession sessao, MedidaProjeto medidaProjeto)
        {
            MedidaProjeto atual = MedidaProjetoDAO.Instance.GetElementByPrimaryKey(sessao, medidaProjeto.IdMedidaProjeto);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MedidaProjeto, medidaProjeto.IdMedidaProjeto, atual, medidaProjeto);
        }

        /// <summary>
        /// Cria o Log de Alterações para o setor.
        /// </summary>
        /// <param name="setor"></param>
        public void LogSetor(Setor setor)
        {
            Setor atual = SetorDAO.Instance.GetElementByPrimaryKey((uint)setor.IdSetor);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Setor, (uint)setor.IdSetor, atual, setor);
        }

        /// <summary>
        /// Cria o Log de Alterações para o texto de pedido
        /// </summary>
        public void LogTextoImprPedido(TextoImprPedido textoImprPedido)
        {
            TextoImprPedido atual = TextoImprPedidoDAO.Instance.GetElementByPrimaryKey(textoImprPedido.IdTextoImprPedido);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.TextoImprPedido, textoImprPedido.IdTextoImprPedido, atual, textoImprPedido);
        }

        /// <summary>
        /// Cria o Log de Alterações para o tipo de cliente.
        /// </summary>
        public void LogTipoCliente(TipoCliente tipoCliente)
        {
            TipoCliente atual = TipoClienteDAO.Instance.GetElementByPrimaryKey((uint)tipoCliente.IdTipoCliente);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.TipoCliente, (uint)tipoCliente.IdTipoCliente, atual, tipoCliente);
        }

        /// <summary>
        /// Cria o Log de Alterações para o grupo de cliente.
        /// </summary>
        public void LogGrupoCliente(GrupoCliente grupoCliente)
        {
            TipoCliente atual = TipoClienteDAO.Instance.GetElementByPrimaryKey((uint)grupoCliente.IdGrupoCliente);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.GrupoCliente, (uint)grupoCliente.IdGrupoCliente, atual, grupoCliente);
        }

        /// <summary>
        /// Cria o Log de Alterações para o beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        public void LogBenefConfig(BenefConfig benefConfig)
        {
            BenefConfig atual = BenefConfigDAO.Instance.GetElement((uint)benefConfig.IdBenefConfig);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.BenefConfig, (uint)benefConfig.IdBenefConfig, atual, benefConfig);
        }

        /// <summary>
        /// Cria o Log de Alterações para o preço de beneficiamento.
        /// </summary>
        /// <param name="benefConfigPreco"></param>
        public void LogBenefConfigPreco(BenefConfigPreco benefConfigPreco)
        {
            BenefConfigPreco atual = BenefConfigPrecoDAO.Instance.GetElementByPrimaryKey((uint)benefConfigPreco.IdBenefConfigPreco);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.BenefConfigPreco, (uint)benefConfigPreco.IdBenefConfigPreco, atual, benefConfigPreco);
        }

        /// <summary>
        /// Cria o Log de Alterações para a imagem da produção.
        /// </summary>
        public void LogImagemProducao(uint idPecaItemProj, string item, string tipoAlteracao)
        {
            PecaItemProjeto novo = PecaItemProjetoDAO.Instance.GetElementExt(null, idPecaItemProj, true);
            novo.Item = item;
            novo.TipoAlteracao = tipoAlteracao;

            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ImagemProducao, novo.IdLog, new PecaItemProjeto(), novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para o subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        public void LogSubtipoPerda(SubtipoPerda subtipoPerda)
        {
            SubtipoPerda atual = SubtipoPerdaDAO.Instance.GetElementByPrimaryKey((uint)subtipoPerda.IdSubtipoPerda);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.SubtipoPerda, (uint)subtipoPerda.IdSubtipoPerda, atual, subtipoPerda);
        }

        /// <summary>
        /// Cria o Log de Alterações para a movimentação bancária.
        /// </summary>
        /// <param name="movEstoque"></param>
        public void LogMovBanco(MovBanco movBanco)
        {
            MovBanco atual = MovBancoDAO.Instance.GetElementByPrimaryKey(movBanco.IdMovBanco);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MovBanco, movBanco.IdMovBanco, atual, movBanco);
        }

        /// <summary>
        /// Cria o Log de Alterações para a movimentação de estoque.
        /// </summary>
        /// <param name="movEstoque"></param>
        public void LogMovEstoque(MovEstoque movEstoque)
        {
            MovEstoque atual = MovEstoqueDAO.Instance.GetElementByPrimaryKey(movEstoque.IdMovEstoque);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MovEstoque, movEstoque.IdMovEstoque, atual, movEstoque);
        }

        /// <summary>
        /// Cria o Log de Alterações para a movimentação de estoque fiscal.
        /// </summary>
        /// <param name="movEstoqueFiscal"></param>
        public void LogMovEstoqueFiscal(MovEstoqueFiscal movEstoqueFiscal)
        {
            MovEstoqueFiscal atual = MovEstoqueFiscalDAO.Instance.GetElementByPrimaryKey(movEstoqueFiscal.IdMovEstoqueFiscal);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MovEstoqueFiscal, movEstoqueFiscal.IdMovEstoqueFiscal, atual, movEstoqueFiscal);
        }

        /// <summary>
        /// Cria o Log de Alterações para a rota.
        /// </summary>
        /// <param name="rota"></param>
        public void LogRota(Rota rota)
        {
            Rota atual = RotaDAO.Instance.GetElementByPrimaryKey((uint)rota.IdRota);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Rota, (uint)rota.IdRota, atual, rota);
        }

        /// <summary>
        /// Cria o Log de Alterações para o sinal da compra.
        /// </summary>
        /// <param name="rota"></param>
        public void LogSinalCompra(SinalCompra SinalCompra)
        {
            SinalCompra atual = SinalCompraDAO.Instance.GetElementByPrimaryKey(SinalCompra.IdSinalCompra);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.SinalCompra, SinalCompra.IdSinalCompra, atual, SinalCompra);
        }

        /// <summary>
        /// Cria o Log de Alterações para o sinal/pagto. antecipado.
        /// </summary>
        /// <param name="sinal"></param>
        public void LogSinal(Sinal sinal)
        {
            LogSinal(null, sinal);
        }

        /// <summary>
        /// Cria o Log de Alterações para o sinal/pagto. antecipado.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="sinal"></param>
        public void LogSinal(GDASession session, Sinal sinal)
        {
            Sinal atual = SinalDAO.Instance.GetElementByPrimaryKey(session, sinal.IdSinal);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Sinal, sinal.IdSinal, atual, sinal);
        }

        /// <summary>
        /// Cria o Log de Alterações para o Encontro de Contas.
        /// </summary>
        public void LogEncontroContas(EncontroContas model)
        {
            LogEncontroContas(null, model);
        }

        /// <summary>
        /// Cria o Log de Alterações para o Encontro de Contas.
        /// </summary>
        public void LogEncontroContas(GDASession session, EncontroContas model)
        {
            EncontroContas atual = EncontroContasDAO.Instance.GetElement(session, model.IdEncontroContas);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.EncontroContas, model.IdEncontroContas, atual, model);
        }

        /// <summary>
        /// Cria o Log de Alterações para o Produto de Fornecedor.
        /// </summary>
        /// <param name="prodFornec"></param>
        public void LogProdutoFornecedor(ProdutoFornecedor prodFornec)
        {
            ProdutoFornecedor atual = ProdutoFornecedorDAO.Instance.GetElementByPrimaryKey((uint)prodFornec.IdProdFornec);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ProdutoFornecedor, (uint)prodFornec.IdProdFornec, atual, prodFornec);
        }

        /// <summary>
        /// Cria o Log de Alterações para o limite de cheques por CPF/CNPJ.
        /// </summary>
        /// <param name="limiteCheque"></param>
        public void LogLimiteChequeCpfCnpj(LimiteChequeCpfCnpj limiteCheque)
        {
            LimiteChequeCpfCnpj atual = LimiteChequeCpfCnpjDAO.Instance.GetElementByPrimaryKey(limiteCheque.IdLimiteCheque);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.LimiteChequeCpfCnpj, limiteCheque.IdLimiteCheque, atual, limiteCheque);
        }

        /// <summary>
        /// Cria o Log de Alterações para o retalho de produção.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="retalhoProducao"></param>
        /// <param name="idFunc"></param>
        public void LogRetalhoProducao(GDASession sessao, RetalhoProducao retalhoProducao, uint idFunc)
        {
            RetalhoProducao novo = RetalhoProducaoDAO.Instance.GetElementByPrimaryKey(sessao, retalhoProducao.IdRetalhoProducao);
            InserirLog(sessao, idFunc, LogAlteracao.TabelaAlteracao.RetalhoProducao, (uint)retalhoProducao.IdRetalhoProducao, retalhoProducao, novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para o retalho de produção.
        /// </summary>
        /// <param name="retalhoProducao"></param>
        public void LogRetalhoProducao( RetalhoProducao retalhoProducao, uint idFunc)
        {
            LogRetalhoProducao(null, retalhoProducao, idFunc);
        }

        /// <summary>
        /// Cria o Log de Alterações para a natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        public void LogNaturezaOperacao(NaturezaOperacao naturezaOperacao)
        {
            NaturezaOperacao atual = NaturezaOperacaoDAO.Instance.GetElementByPrimaryKey((uint)naturezaOperacao.IdNaturezaOperacao);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.NaturezaOperacao, (uint)naturezaOperacao.IdNaturezaOperacao, atual, naturezaOperacao);
        }

        /// <summary>
        /// Cria o Log de Alterações para a regra de natureza de operação.
        /// </summary>
        /// <param name="regraNaturezaOperacao"></param>
        public void LogRegraNaturezaOperacao(RegraNaturezaOperacao regraNaturezaOperacao)
        {
            RegraNaturezaOperacao atual = RegraNaturezaOperacaoDAO.Instance.GetElementByPrimaryKey((uint)regraNaturezaOperacao.IdRegraNaturezaOperacao);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.RegraNaturezaOperacao, (uint)regraNaturezaOperacao.IdRegraNaturezaOperacao, atual, regraNaturezaOperacao);
        }

        /// <summary>
        /// Cria o Log de Alterações para o roteiro de produção.
        /// </summary>
        /// <param name="roteiroProducao"></param>
        public void LogRoteiroProducao(RoteiroProducao roteiroProducao)
        {
            RoteiroProducao atual = RoteiroProducaoDAO.Instance.ObtemElemento(null, roteiroProducao.IdRoteiroProducao);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.RoteiroProducao, (uint)roteiroProducao.IdRoteiroProducao, atual, roteiroProducao);
        }

        /// <summary>
        /// Cria o Log de Alterações para a movimentação de estoque de cliente.
        /// </summary>
        /// <param name="movEstoqueCliente"></param>
        public void LogMovEstoqueCliente(MovEstoqueCliente movEstoqueCliente)
        {
            MovEstoqueCliente atual = MovEstoqueClienteDAO.Instance.GetElementByPrimaryKey(movEstoqueCliente.IdMovEstoqueCliente);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.MovEstoqueCliente, movEstoqueCliente.IdMovEstoqueCliente, atual, movEstoqueCliente);
        }

        /// <summary>
        /// Cria o Log de Alterações para o produto pedido produção.
        /// </summary>
        /// <param name="prodPedProducao"></param>
        /// <param name="sequencia"></param>
        public void LogProdPedProducao(GDASession sessao, ProdutoPedidoProducao prodPedProducao, SequenciaObjeto sequencia)
        {
            LogProdPedProducao(sessao, prodPedProducao, sequencia, UserInfo.GetUserInfo.CodUser);
        }

        /// <summary>
        /// Cria o Log de Alterações para o produto pedido produção.
        /// </summary>
        /// <param name="prodPedProducao"></param>
        /// <param name="sequencia"></param>
        /// <param name="idFuncAlt"></param>
        public void LogProdPedProducao(GDASession session, ProdutoPedidoProducao prodPedProducao, SequenciaObjeto sequencia, uint idFuncAlt)
        {
            ProdutoPedidoProducao outro = ProdutoPedidoProducaoDAO.Instance.GetElementByPrimaryKey(prodPedProducao.IdProdPedProducao);

            if (sequencia == SequenciaObjeto.Novo)
                InserirLog(idFuncAlt, LogAlteracao.TabelaAlteracao.ProdPedProducao, prodPedProducao.IdProdPedProducao, outro, prodPedProducao);
            else
                InserirLog(idFuncAlt, LogAlteracao.TabelaAlteracao.ProdPedProducao, prodPedProducao.IdProdPedProducao, prodPedProducao, outro);
        }

        /// <summary>
        /// Cria o Log de Alterações para a capacidade de produção diária.
        /// </summary>
        /// <param name="capacidadeProducao"></param>
        public void LogCapacidadeProducaoDiaria(CapacidadeProducaoDiaria capacidadeProducaoAnterior)
        {
            CapacidadeProducaoDiaria novo = CapacidadeProducaoDiariaDAO.Instance.ObtemParaLog(capacidadeProducaoAnterior.IdParaLog);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.CapacidadeProducaoDiaria, novo.IdParaLog, capacidadeProducaoAnterior, novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para a capacidade de produção diária.
        /// </summary>
        /// <param name="capacidadeProducao"></param>
        public void LogCapacidadeProducaoDiaria(CapacidadeProducaoDiaria anterior, CapacidadeProducaoDiaria novo)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.CapacidadeProducaoDiaria, novo.IdParaLog, anterior, novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para as parcelas do cliente/fornecedor.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idFornec"></param>
        /// <param name="descricaoAnterior"></param>
        /// <param name="descricaoNova"></param>
        public void LogParcelasNaoUsar(int? idCliente, int? idFornec, string descricaoAnterior, string descricaoNova)
        {
            var tabela = idCliente != null ? LogAlteracao.TabelaAlteracao.Cliente : LogAlteracao.TabelaAlteracao.Fornecedor;
            int id = idCliente ?? idFornec.Value;

            if (!String.Equals(descricaoAnterior, descricaoNova, StringComparison.CurrentCultureIgnoreCase))
            {
                var item = new LogAlteracao()
                {
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    DataAlt = DateTime.Now,
                    Campo = "Parcelas",
                    IdRegistroAlt = id,
                    NumEvento = GetNumEvento(tabela, id),
                    Referencia = LogAlteracao.GetReferencia(tabela, (uint)id),
                    Tabela = (int)tabela,
                    ValorAnterior = descricaoAnterior,
                    ValorAtual = descricaoNova
                };

                Insert(item);
            }
        }

        /// <summary>
        /// Cria o Log de Alterações para os juros de parcelaso do tipo de cartão.
        /// </summary>
        public void LogTipoCartaoJurosParcelas(GDASession session, int idTipoCartao, int idLoja, string descricaoAnterior, string descricaoNova)
        {
            var tabela = LogAlteracao.TabelaAlteracao.TipoCartao;

            if (!string.Equals(descricaoAnterior, descricaoNova, StringComparison.CurrentCultureIgnoreCase))
            {
                var item = new LogAlteracao()
                {
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    DataAlt = DateTime.Now,
                    Campo = string.Format("LOJA: {0}.", LojaDAO.Instance.GetNome((uint)idLoja)),
                    IdRegistroAlt = idTipoCartao,
                    NumEvento = GetNumEvento(session, tabela, idTipoCartao),
                    Referencia = LogAlteracao.GetReferencia(session, tabela, (uint)idTipoCartao),
                    Tabela = (int)tabela,
                    ValorAnterior = descricaoAnterior,
                    ValorAtual = descricaoNova
                };

                Insert(session, item);
            }
        }

        /// <summary>
        /// Cria o Log de Alterações para o tipo de cartão.
        /// </summary>
        public void LogTipoCartao(GDASession sessao, TipoCartaoCredito antigo, TipoCartaoCredito novo)
        {
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.TipoCartao, (uint)novo.IdTipoCartao, antigo, novo);
        }

        /// <summary>
        /// Cria o Log de Alterações para as formas de pagamento do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="descricaoAnterior"></param>
        /// <param name="descricaoNova"></param>
        public void LogFormasPagtoNaoUsar(int idCliente, string descricaoAnterior, string descricaoNova)
        {
            var tabela = LogAlteracao.TabelaAlteracao.Cliente;
            int id = idCliente;

            if (!String.Equals(descricaoAnterior, descricaoNova, StringComparison.CurrentCultureIgnoreCase))
            {
                var item = new LogAlteracao()
                {
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    DataAlt = DateTime.Now,
                    Campo = "Formas de Pagamento",
                    IdRegistroAlt = id,
                    NumEvento = GetNumEvento(tabela, id),
                    Referencia = LogAlteracao.GetReferencia(tabela, (uint)id),
                    Tabela = (int)tabela,
                    ValorAnterior = descricaoAnterior,
                    ValorAtual = descricaoNova
                };

                Insert(item);
            }
        }

        /// <summary>
        /// Cria o Log de Alterações para o controle de créditos do EFD.
        /// </summary>
        /// <param name="controleCredito"></param>
        public void LogControleCreditosEfd(ControleCreditoEfd controleCredito)
        {
            ControleCreditoEfd atual = ControleCreditoEfdDAO.Instance.GetElementByPrimaryKey(controleCredito.IdCredito);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ControleCreditosEfd, controleCredito.IdCredito, atual, controleCredito);
        }

        public void LogTrocaDev(TrocaDevolucao trocaDev)
        {
            LogTrocaDev(null, trocaDev);
        }

        public void LogTrocaDev(GDASession session, TrocaDevolucao trocaDev)
        {
            var atual = TrocaDevolucaoDAO.Instance.GetElement(session, trocaDev.IdTrocaDevolucao);
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.TrocaDev, trocaDev.IdTrocaDevolucao, atual, trocaDev);
        }

        public void LogAmbientePedido(AmbientePedido ambiente)
        {
            LogAmbientePedido(null, ambiente);
        }

        public void LogAmbientePedido(GDASession sessao, AmbientePedido ambiente)
        {
            var atual = AmbientePedidoDAO.Instance.GetElement(sessao, ambiente.IdAmbientePedido);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.AmbientePedido, ambiente.IdAmbientePedido, atual, ambiente);
        }

        public void LogCarregamento(GDASession session, Carregamento atual, Carregamento novo)
        {
            InserirLog(session, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Carregamento, atual.IdCarregamento, atual, novo);
        }

        public void LogReenvioEmailLiberacao(GDASession sessao, uint idLiberacao)
        {
            // Cria o Log
            LogAlteracao log = new LogAlteracao();
            log.Tabela = (int)LogAlteracao.TabelaAlteracao.LiberacaoReenvioEmail;
            log.IdRegistroAlt = (int)idLiberacao;
            log.NumEvento = GetNumEvento(sessao, LogAlteracao.TabelaAlteracao.LiberacaoReenvioEmail, (int)idLiberacao);
            log.Campo = "Reenvio de E-mail";
            log.DataAlt = DateTime.Now;
            log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            log.Referencia = idLiberacao.ToString();

            Insert(sessao, log);
        }

        public void LogEnvioEmailOrcamento(uint idOrcamento)
        {
            // Cria o Log
            LogAlteracao log = new LogAlteracao();
            log.Tabela = (int)LogAlteracao.TabelaAlteracao.Orcamento;
            log.IdRegistroAlt = (int)idOrcamento;
            log.NumEvento = GetNumEvento(LogAlteracao.TabelaAlteracao.Orcamento, (int)idOrcamento);
            log.Campo = "Envio de E-mail";
            log.DataAlt = DateTime.Now;
            log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            log.Referencia = idOrcamento.ToString();

            Insert(log);
        }

        public void LogPedidoEspelho(GDASession sessao, PedidoEspelho pedidoEsp)
        {
            var atual = PedidoEspelhoDAO.Instance.GetElement(sessao, pedidoEsp.IdPedido);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.PedidoEspelho, pedidoEsp.IdPedido, pedidoEsp, atual);
        }

        public void LogOrdemCarga(GDASession sessao, int idOrdemCarga, string obs)
        {
            // Cria o Log
            LogAlteracao log = new LogAlteracao();
            log.Tabela = (int)LogAlteracao.TabelaAlteracao.OrdemCarga;
            log.IdRegistroAlt = idOrdemCarga;
            log.NumEvento = GetNumEvento(LogAlteracao.TabelaAlteracao.OrdemCarga, idOrdemCarga);
            log.Campo = obs;
            log.DataAlt = DateTime.Now;
            log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            log.Referencia = idOrdemCarga.ToString();

            Insert(sessao, log);
        }

        public void LogCarregamentoOC(GDASession sessao, int idCarregamento, string obs)
        {
            // Cria o Log
            LogAlteracao log = new LogAlteracao();
            log.Tabela = (int)LogAlteracao.TabelaAlteracao.CarregamentoOC;
            log.IdRegistroAlt = idCarregamento;
            log.NumEvento = GetNumEvento(LogAlteracao.TabelaAlteracao.CarregamentoOC, idCarregamento);
            log.Campo = "Ocs Adicionadas: ";
            log.ValorAtual = obs;
            log.DataAlt = DateTime.Now;
            log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            log.Referencia = idCarregamento.ToString();

            Insert(sessao, log);
        }

        /// <summary>
        /// Cria o Log de alterações para o depósito não identificado.
        /// </summary>
        public void LogDepositoNaoIdentificado(GDASession sessao, DepositoNaoIdentificado depositoNaoIdentificado)
        {
            var novo = DepositoNaoIdentificadoDAO.Instance.GetElement(sessao, depositoNaoIdentificado.IdDepositoNaoIdentificado);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.DepositoNaoIdentificado,
                depositoNaoIdentificado.IdDepositoNaoIdentificado, depositoNaoIdentificado, novo);
        }

        public void LogConfiguracaoAresta(GDASession sessao, string configuracaoAntiga, string configuracaoNova)
        {
            #region Recupera a listagem das configurações antigas e novas

            var index = 0;
            var existeConfiguracaoAntiga = !string.IsNullOrEmpty(configuracaoAntiga);
            var existeConfiguracaoNova = !string.IsNullOrEmpty(configuracaoNova);

            // Recupera as configuracoes de aresta antigas.
            var configuracoesAntigas =
                existeConfiguracaoAntiga ?
                    configuracaoAntiga.Split('|')[0].Split(';').Select(f => new
                    {
                        IdSubgrupoProd = configuracaoAntiga.Split('|')[0].Split(';')[index].StrParaInt(),
                        IdBenefConfig = configuracaoAntiga.Split('|')[1].Split(';')[index].StrParaInt(),
                        CondEspessura = configuracaoAntiga.Split('|')[2].Split(';')[index].StrParaInt(),
                        Espessura = configuracaoAntiga.Split('|')[3].Split(';')[index].StrParaInt(),
                        Aresta = configuracaoAntiga.Split('|')[4].Split(';')[index].StrParaInt(),
                        IdProcesso = configuracaoAntiga.Split('|')[5].Split(';')[index++].StrParaInt(),
                    }).ToList() : null;

            // Recupera as configuracoes de aresta novas.
            index = 0;
            var configuracoesNovas =
                existeConfiguracaoNova ?
                    configuracaoNova.Split('|')[0].Split(';').Select(f => new
                    {
                        IdSubgrupoProd = configuracaoNova.Split('|')[0].Split(';')[index].StrParaInt(),
                        IdBenefConfig = configuracaoNova.Split('|')[1].Split(';')[index].StrParaInt(),
                        CondEspessura = configuracaoNova.Split('|')[2].Split(';')[index].StrParaInt(),
                        Espessura = configuracaoNova.Split('|')[3].Split(';')[index].StrParaInt(),
                        Aresta = configuracaoNova.Split('|')[4].Split(';')[index].StrParaInt(),
                        IdProcesso = configuracaoNova.Split('|')[5].Split(';')[index++].StrParaInt(),
                    }).ToList() : null;

            #endregion

            #region Campos utilizados na inserção do log

            // Esta variável será utilizada para definir a propriedade Campo do log de alteração.
            // Cada configuração terá um campo diferente. Ex.: Config. aresta: 1, Config. aresta: 2 etc.
            var campo = "Config. aresta: {0}";
            // Variável utilizada para salvar os valores atuais e antigos da configuração de aresta.
            var valor = "Subgrupo: {0} | Beneficiamento: {1} | Condição espessura: {2} | Espessura: {3} | Aresta: {4} | Processo: {5}";
            // Vetor com as possíveis condições de espessura.
            var condicaoEspessura = new string[] { "", "=", "<>", ">", "<", ">=", "<=" };

            // Objeto do log com os valores padrões.
            var logAresta = new LogAlteracao()
            {
                DataAlt = DateTime.Now,
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                IdRegistroAlt = 1,
                NomeFuncAlt = UserInfo.GetUserInfo.Nome,
                NumEvento = GetNumEvento(null, LogAlteracao.TabelaAlteracao.ConfiguracaoAresta, 1),
                Referencia = "Aresta",
                Tabela = (int)LogAlteracao.TabelaAlteracao.ConfiguracaoAresta,
            };

            #endregion

            #region Salva o Log da configuração da aresta

            // Atualiza o log com base nas configurações antigas.
            if (configuracoesAntigas != null)
            {
                for (var i = 0; i < configuracoesAntigas.Count; i++)
                {
                    // O valor anterior será as configurações de aresta antigas.
                    var valorAnterior = string.Format(valor, configuracoesAntigas[i].IdSubgrupoProd, configuracoesAntigas[i].IdBenefConfig,
                            condicaoEspessura[configuracoesAntigas[i].CondEspessura], configuracoesAntigas[i].Espessura,
                            configuracoesAntigas[i].Aresta, configuracoesAntigas[i].IdProcesso);

                    // Caso o valor atual da aresta não tenha sido informado, ou seja, a aresta tenha sido removida, então
                    // os valores atuais serão 0. Caso as configurações novas tenham sido informadas os valores serão salvos
                    // na variável configuracoesNovas.
                    var valorAtual =
                        configuracoesNovas != null && configuracoesNovas.Count > i ?
                            string.Format(valor, configuracoesNovas[i].IdSubgrupoProd, configuracoesNovas[i].IdBenefConfig,
                                condicaoEspessura[configuracoesNovas[i].CondEspessura], configuracoesNovas[i].Espessura,
                                configuracoesNovas[i].Aresta, configuracoesNovas[i].IdProcesso) :
                            string.Format(valor, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

                    // Salva o log da aresta somente se os valores forem diferentes.
                    if (valorAnterior != valorAtual)
                    {
                        logAresta.Campo = string.Format(campo, i + 1);
                        logAresta.ValorAnterior = valorAnterior;
                        logAresta.ValorAtual = valorAtual;

                        Insert(sessao, logAresta);
                    }
                }

                // Caso o usuário tenha inserido mais arestas, salva o log de inserção das arestas novas.
                if (configuracoesNovas.Count > configuracoesAntigas.Count)
                {
                    // Percorre as arestas novas que não geraram log.
                    for (var i = configuracoesAntigas.Count; i < configuracoesNovas.Count; i++)
                    {
                        // O valor anterior será 0 porque esta aresta não existia no sistema.
                        var valorAnterior = string.Format(valor, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                            string.Empty);

                        // Salva o valor da aresta nova.
                        var valorAtual = string.Format(valor, configuracoesNovas[i].IdSubgrupoProd, configuracoesNovas[i].IdBenefConfig,
                            condicaoEspessura[configuracoesNovas[i].CondEspessura], configuracoesNovas[i].Espessura,
                            configuracoesNovas[i].Aresta, configuracoesNovas[i].IdProcesso);

                        logAresta.Campo = string.Format(campo, i + 1);
                        logAresta.ValorAnterior = valorAnterior;
                        logAresta.ValorAtual = valorAtual;

                        Insert(sessao, logAresta);
                    }
                }
            }
            // Insere o log das arestas novas.
            else if (configuracoesNovas != null)
            {
                for (var i = 0; i < configuracoesNovas.Count; i++)
                {
                    // O valor anterior será 0 porque a aresta não existia.
                    var valorAnterior = string.Format(valor, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                        string.Empty);

                    var valorAtual = string.Format(valor, configuracoesNovas[i].IdSubgrupoProd, configuracoesNovas[i].IdBenefConfig,
                        condicaoEspessura[configuracoesNovas[i].CondEspessura], configuracoesNovas[i].Espessura,
                        configuracoesNovas[i].Aresta, configuracoesNovas[i].IdProcesso);

                    logAresta.Campo = string.Format(campo, i + 1);
                    logAresta.ValorAnterior = valorAnterior;
                    logAresta.ValorAtual = valorAtual;

                    Insert(sessao, logAresta);
                }
            }

            #endregion
        }

        /// <summary>
        /// Cria o Log de Alterações para o grupo de medidas de projeto.
        /// </summary>
        /// <param name="grupoMedidaProjeto"></param>
        public void LogGrupoMedidaProjeto(GrupoMedidaProjeto grupoMedidaProjeto)
        {
            GrupoMedidaProjeto atual = GrupoMedidaProjetoDAO.Instance.GetElementByPrimaryKey(grupoMedidaProjeto.IdGrupoMedProj);
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.GrupoMedidaProjeto, grupoMedidaProjeto.IdGrupoMedProj, atual, grupoMedidaProjeto);
        }

        /// <summary>
        /// Cria o Log de Alterações para Desconto por Forma de Pagamento e Dados do Produto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="descFormaPagtoDadosProd"></param>
        public void LogDescontoFormaPagamentoDadosProduto(GDASession sessao, DescontoFormaPagamentoDadosProduto descFormaPagtoDadosProd)
        {
            DescontoFormaPagamentoDadosProduto atual = DescontoFormaPagamentoDadosProdutoDAO.Instance.GetElementByPrimaryKey(sessao, descFormaPagtoDadosProd.IdDescontoFormaPagamentoDadosProduto);
            InserirLog(sessao, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.DescontoFormaPagamentoDadosProduto, descFormaPagtoDadosProd.IdDescontoFormaPagamentoDadosProduto, atual, descFormaPagtoDadosProd);
        }

        /// <summary>
        /// Cria o Log de alterações para a Etiqueta Processo.
        /// </summary>
        /// <param name="contaReceberAtual"></param>
        /// <param name="contaReceberNova"></param>
        public void LogEtiquetaProcesso(EtiquetaProcesso etiquetaProcessoAtual, EtiquetaProcesso etiquetaProcessoNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Processo, (uint)etiquetaProcessoAtual.IdProcesso,
                etiquetaProcessoAtual, etiquetaProcessoNova);
        }

        /// <summary>
        /// Cria o Log de alterações para a Etiqueta Aplicacao.
        /// </summary>
        /// <param name="contaReceberAtual"></param>
        /// <param name="contaReceberNova"></param>
        public void LogEtiquetaAplicacao(EtiquetaAplicacao etiquetaAplicacaoAtual, EtiquetaAplicacao etiquetaAplicacaoNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Aplicacao, (uint)etiquetaAplicacaoAtual.IdAplicacao,
                etiquetaAplicacaoAtual, etiquetaAplicacaoNova);
        }

        /// <summary>
        /// Cria o Log de alterações para a Medição
        /// </summary>
        public void LogMedicao(Medicao medicaoAtual, Medicao medicaoNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Medicao, (uint)medicaoAtual.IdMedicao,
                medicaoAtual, medicaoNova);
        }

        /// <summary>
        /// Cria o Log de alteração para ComissaoConfigGerente
        /// </summary>
        /// <param name="comissaoGerenteAtual"></param>
        /// <param name="comissaoGerenteNova"></param>
        public void LogComissaoConfigGerente(ComissaoConfigGerente comissaoGerenteAtual, ComissaoConfigGerente comissaoGerenteNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ComissaoConfigGerente, comissaoGerenteNova.IdComissaoConfigGerente,
                comissaoGerenteAtual, comissaoGerenteNova);
        }

        /// <summary>
        /// Cria o Log de alteração para Bandeira de Cartão
        /// </summary>
        /// <param name="bandeiraCartaoAtual"></param>
        /// <param name="bandeiraCartaoNova"></param>
        public void LogBandeiraCartao(GDASession transaction, BandeiraCartao bandeiraCartaoAtual, BandeiraCartao bandeiraCartaoNova)
        {
            InserirLog(transaction, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.BandeiraCartao, bandeiraCartaoAtual.IdBandeiraCartao, bandeiraCartaoAtual, bandeiraCartaoNova);
        }

        /// <summary>
        /// Cria o Log de alteração para Operadora de Cartão
        /// </summary>
        /// <param name="operadoraCartaoAtual"></param>
        /// <param name="operadoraCartaoNova"></param>
        public void LogOperadoraCartao(GDASession transaction, OperadoraCartao operadoraCartaoAtual, OperadoraCartao operadoraCartaoNova)
        {
            InserirLog(transaction, UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.OperadoraCartao, operadoraCartaoAtual.IdOperadoraCartao, operadoraCartaoAtual, operadoraCartaoNova);
        }

        /// <summary>
        /// Cria o Log de alterações para a Etiqueta Aplicacao.
        /// </summary>
        /// <param name="contaReceberAtual"></param>
        /// <param name="contaReceberNova"></param>
        public void LogCompra(Compra compraAtual, Compra compraNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.Compra, (uint)compraAtual.IdCompra,
                compraAtual, compraNova);
        }

        /// <summary>
        /// Cria o Log de alterações para o imposto/Serv
        /// </summary>
        public void LogImpostoServico(GDASession sessao, ImpostoServ impostoServAtual, ImpostoServ impostoServNova)
        {
            InserirLog(UserInfo.GetUserInfo.CodUser, LogAlteracao.TabelaAlteracao.ImpostoServico, impostoServAtual.IdImpostoServ,
                impostoServAtual, impostoServNova);
        }

        #endregion

        #region Remoção de itens

        #region Método de remoção

        /// <summary>
        /// Remove os itens de Log de alteração.
        /// </summary>
        /// <param name="tabela">A tabela que será alterada.</param>
        /// <param name="id">O ID do item que será apagado.</param>
        private void ApagaLog(LogAlteracao.TabelaAlteracao tabela, uint id)
        {
            // Não apaga mais os logs
            return;

           // objPersistence.ExecuteCommand("delete from log_alteracao where tabela=" + (int)tabela + " and idRegistroAlt=" + id);
        }

        #endregion

        /// <summary>
        /// Apaga o Log de Alterações para o funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        public void ApagaLogFuncionario(uint idFunc)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Funcionario, idFunc);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void ApagaLogCliente(uint idCliente)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Cliente, idCliente);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o produto.
        /// </summary>
        /// <param name="idProd"></param>
        public void ApagaLogProduto(uint idProd)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Produto, idProd);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        public void ApagaLogFornecedor(uint idFornec)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Fornecedor, idFornec);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a configuração por loja.
        /// </summary>
        /// <param name="idConfig"></param>
        public void ApagaLogConfigLoja(uint idConfigLoja)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ConfigLoja, idConfigLoja);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o Tipo de Perda.
        /// </summary>
        /// <param name="idCfop"></param>
        public void ApagaLogTipoPerda(uint idTipoPerda)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.TipoPerda, idTipoPerda);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        public void ApagaLogCfop(uint idCfop)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Cfop, idCfop);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o CFOP por loja.
        /// </summary>
        /// <param name="idCfopLoja"></param>
        //public void ApagaLogCfopLoja(uint idCfopLoja)
        //{
        //    ApagaLog(LogAlteracao.TabelaAlteracao.CfopLoja, idCfopLoja);
        //}

        /// <summary>
        /// Apaga o Log de Alterações para o desconto/acréscimo por cliente.
        /// </summary>
        /// <param name="idDesconto"></param>
        public void ApagaLogDescontoAcrescimoCliente(uint idDesconto)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.DescontoAcrescimoCliente, idDesconto);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o controle de usuário.
        /// </summary>
        /// <param name="idLog"></param>
        public void ApagaLogControleUsuario(uint idLog)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ControleUsuario, idLog);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o grupo do produto.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        public void ApagaLogGrupoProduto(uint idGrupoProd)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.GrupoProduto, idGrupoProd);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o subgrupo do produto.
        /// </summary>
        /// <param name="idSubgrupoProd"></param>
        public void ApagaLogSubgrupoProduto(uint idSubgrupoProd)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.SubgrupoProduto, idSubgrupoProd);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a nota fiscal.
        /// </summary>
        /// <param name="idNf"></param>
        public void ApagaLogNotaFiscal(uint idNf)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.NotaFiscal, idNf);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o produto da nota fiscal.
        /// </summary>
        /// <param name="idProdutoNf"></param>
        public void ApagaLogProdutoNotaFiscal(uint idProdutoNf)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ProdutoNotaFiscal, idProdutoNf);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        public void ApagaLogPedido(uint idPedido)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Pedido, idPedido);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o cheque.
        /// </summary>
        /// <param name="idCheque"></param>
        public void ApagaLogCheque(uint idCheque)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Cheque, idCheque);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a posição da peça do modelo de projeto.
        /// </summary>
        /// <param name="idPosicaoPecaModelo"></param>
        public void ApagaLogPosicaoPecaModelo(uint idPosicaoPecaModelo)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.PosicaoPecaModelo, idPosicaoPecaModelo);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o material do modelo de projeto.
        /// </summary>
        /// <param name="idMaterialProjetoModelo"></param>
        public void ApagaLogMaterialProjetoModelo(uint idMaterialProjetoModelo)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.MaterialProjetoModelo, idMaterialProjetoModelo);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a posição da peça individual do modelo de projeto.
        /// </summary>
        /// <param name="idPosicaoPecaIndividual"></param>
        public void ApagaLogPosicaoPecaIndividual(uint idPosicaoPecaIndividual)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual, idPosicaoPecaIndividual);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o produto do projeto.
        /// </summary>
        /// <param name="idProdutoProjeto"></param>
        public void ApagaLogProdutoProjeto(uint idProdutoProjeto)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ProdutoProjeto, idProdutoProjeto);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o produto vinculado do projeto.
        /// </summary>
        /// <param name="idProdutoProjetoConfig"></param>
        public void ApagaLogProdutoProjetoConfig(uint idProdutoProjetoConfig)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ProdutoProjetoConfig, idProdutoProjetoConfig);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a medida do projeto.
        /// </summary>
        /// <param name="idMedidaProjeto"></param>
        public void ApagaLogMedidaProjeto(uint idMedidaProjeto)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.MedidaProjeto, idMedidaProjeto);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o setor.
        /// </summary>
        /// <param name="idSetor"></param>
        public void ApagaLogSetor(uint idSetor)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Setor, idSetor);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o tipo de cliente.
        /// </summary>
        /// <param name="idTipoCliente"></param>
        public void ApagaLogTipoCliente(uint idTipoCliente)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.TipoCliente, idTipoCliente);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o tipo de cliente.
        /// </summary>
        /// <param name="idGrupoCliente"></param>
        public void ApagaLogGrupoCliente(uint idGrupoCliente)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.GrupoCliente, idGrupoCliente);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        public void ApagaLogBenefConfig(uint idBenefConfig)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.BenefConfig, idBenefConfig);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o preço de beneficiamento
        /// </summary>
        /// <param name="idBenefConfigPreco"></param>
        public void ApagaLogBenefConfigPreco(uint idBenefConfigPreco)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.BenefConfigPreco, idBenefConfigPreco);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o subtipo de perda.
        /// </summary>
        /// <param name="idSubtipoPerda"></param>
        public void ApagaLogSubtipoPerda(uint idSubtipoPerda)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.SubtipoPerda, idSubtipoPerda);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a movimentação de estoque.
        /// </summary>
        /// <param name="idMovEstoque"></param>
        public void ApagaLogMovEstoque(uint idMovEstoque)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.MovEstoque, idMovEstoque);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a movimentação de estoque fiscal.
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        public void ApagaLogMovEstoqueFiscal(uint idMovEstoqueFiscal)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.MovEstoqueFiscal, idMovEstoqueFiscal);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a rota.
        /// </summary>
        /// <param name="idRota"></param>
        public void ApagaLogRota(uint idRota)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.Rota, idRota);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o sinal da compra.
        /// </summary>
        /// <param name="idRota"></param>
        public void ApagaLogSinalCompra(uint idSinalCompra)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.SinalCompra, idSinalCompra);
        }

        /// <summary>
        /// Apaga o Log de Alterações para a movimentação de estoque de cliente.
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        public void ApagaLogMovEstoqueCliente(uint idMovEstoqueCliente)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.MovEstoqueCliente, idMovEstoqueCliente);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o produto pedido produção.
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        public void ApagaLogProdPedProducao(uint idProdPedProducao)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.ProdPedProducao, idProdPedProducao);
        }

        /// <summary>
        /// Apaga o Log de Alterações para o grupo de medida de projeto.
        /// </summary>
        /// <param name="idGrupoMedidaProjeto"></param>
        public void ApagaLogGrupoMedidaProjeto(uint idGrupoMedidaProjeto)
        {
            ApagaLog(LogAlteracao.TabelaAlteracao.GrupoMedidaProjeto, idGrupoMedidaProjeto);
        }

        #endregion

        #region Atualiza IDs de uma tabela para outro

        /// <summary>
        /// Atualiza os IDs de uma tabela.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroAtual"></param>
        /// <param name="idRegistroNovo"></param>
        public void AtualizaID(int tabela, uint idRegistroAtual, uint idRegistroNovo)
        {
            string sql = "update log_alteracao set idRegistroAlt=" + idRegistroNovo + " where tabela=" + tabela +
                " and idRegistroAlt=" + idRegistroAtual;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Recria os objetos alterados

        #region Métodos de suporte

        /// <summary>
        /// Recupera o tipo do modelo de acordo com a tabela usada pelo log.
        /// </summary>
        /// <param name="tabela">A tabela usada pelo log.</param>
        /// <returns>O tipo do objeto que é lido pelo log.</returns>
        private Type GetType(LogAlteracao.TabelaAlteracao tabela)
        {
            switch (tabela)
            {
                case LogAlteracao.TabelaAlteracao.Funcionario: return typeof(Funcionario);
                case LogAlteracao.TabelaAlteracao.Cliente: return typeof(Cliente);
                case LogAlteracao.TabelaAlteracao.Produto: return typeof(Produto);
                case LogAlteracao.TabelaAlteracao.Fornecedor: return typeof(Fornecedor);
                case LogAlteracao.TabelaAlteracao.ConfigLoja: return typeof(ConfiguracaoLoja);
                case LogAlteracao.TabelaAlteracao.Cfop: return typeof(Cfop);
                //case LogAlteracao.TabelaAlteracao.CfopLoja: return typeof(CfopLoja);
                case LogAlteracao.TabelaAlteracao.DescontoAcrescimoCliente: return typeof(DescontoAcrescimoCliente);
                case LogAlteracao.TabelaAlteracao.ControleUsuario: return typeof(FuncModulo);
                case LogAlteracao.TabelaAlteracao.GrupoProduto: return typeof(GrupoProd);
                case LogAlteracao.TabelaAlteracao.SubgrupoProduto: return typeof(SubgrupoProd);
                case LogAlteracao.TabelaAlteracao.NotaFiscal: return typeof(NotaFiscal);
                case LogAlteracao.TabelaAlteracao.ProdutoNotaFiscal: return typeof(ProdutosNf);
                case LogAlteracao.TabelaAlteracao.Pedido: return typeof(Pedido);
                case LogAlteracao.TabelaAlteracao.Cheque: return typeof(Cheques);
                case LogAlteracao.TabelaAlteracao.Pagto: return typeof(Pagto);
                case LogAlteracao.TabelaAlteracao.UnidadeMedida: return typeof(UnidadeMedida);
                case LogAlteracao.TabelaAlteracao.Transportador: return typeof(Transportador);
                case LogAlteracao.TabelaAlteracao.ProdutoLoja: return typeof(ProdutoLoja);
                case LogAlteracao.TabelaAlteracao.PlanoContaContabil: return typeof(PlanoContaContabil);
                case LogAlteracao.TabelaAlteracao.CentroCusto: return typeof(CentroCusto);
                case LogAlteracao.TabelaAlteracao.BemAtivoImobilizado: return typeof(BemAtivoImobilizado);
                case LogAlteracao.TabelaAlteracao.MovBemAtivoImobilizado: return typeof(MovimentacaoBemAtivoImob);
                case LogAlteracao.TabelaAlteracao.Loja: return typeof(Loja);
                case LogAlteracao.TabelaAlteracao.AdministradoraCartao: return typeof(AdministradoraCartao);
                case LogAlteracao.TabelaAlteracao.ProjetoModelo: return typeof(ProjetoModelo);
                case LogAlteracao.TabelaAlteracao.PosicaoPecaModelo: return typeof(PosicaoPecaModelo);
                case LogAlteracao.TabelaAlteracao.PecaProjetoModelo: return typeof(PecaProjetoModelo);
                case LogAlteracao.TabelaAlteracao.MaterialProjetoModelo: return typeof(MaterialProjetoModelo);
                case LogAlteracao.TabelaAlteracao.GrupoModelo: return typeof(GrupoModelo);
                case LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual: return typeof(PosicaoPecaIndividual);
                case LogAlteracao.TabelaAlteracao.ProdutoProjeto: return typeof(ProdutoProjeto);
                case LogAlteracao.TabelaAlteracao.ProdutoProjetoConfig: return typeof(ProdutoProjetoConfig);
                case LogAlteracao.TabelaAlteracao.MedidaProjeto: return typeof(MedidaProjeto);
                case LogAlteracao.TabelaAlteracao.Setor: return typeof(Setor);
                case LogAlteracao.TabelaAlteracao.TipoPerda: return typeof(TipoPerda);
                case LogAlteracao.TabelaAlteracao.TipoCliente: return typeof(TipoCliente);
                case LogAlteracao.TabelaAlteracao.BenefConfig: return typeof(BenefConfig);
                case LogAlteracao.TabelaAlteracao.BenefConfigPreco: return typeof(BenefConfigPreco);
                case LogAlteracao.TabelaAlteracao.PedidoEspelho: return typeof(PedidoEspelho);
                case LogAlteracao.TabelaAlteracao.ImagemProducao: return typeof(PecaItemProjeto);
                case LogAlteracao.TabelaAlteracao.MovEstoque: return typeof(MovEstoque);
                case LogAlteracao.TabelaAlteracao.MovEstoqueFiscal: return typeof(MovEstoqueFiscal);
                case LogAlteracao.TabelaAlteracao.Rota: return typeof(Rota);
                case LogAlteracao.TabelaAlteracao.Sinal: return typeof(Sinal);
                case LogAlteracao.TabelaAlteracao.SinalCompra: return typeof(SinalCompra);
                case LogAlteracao.TabelaAlteracao.RetalhoProducao: return typeof(RetalhoProducao);
                default: throw new NotImplementedException();
            }
        }

        private Type GetDAO(LogAlteracao.TabelaAlteracao tabela)
        {
            switch (tabela)
            {
                case LogAlteracao.TabelaAlteracao.Funcionario: return typeof(FuncionarioDAO);
                case LogAlteracao.TabelaAlteracao.Cliente: return typeof(ClienteDAO);
                case LogAlteracao.TabelaAlteracao.Produto: return typeof(ProdutoDAO);
                case LogAlteracao.TabelaAlteracao.Fornecedor: return typeof(FornecedorDAO);
                case LogAlteracao.TabelaAlteracao.ConfigLoja: return typeof(ConfiguracaoLojaDAO);
                case LogAlteracao.TabelaAlteracao.Cfop: return typeof(CfopDAO);
                //case LogAlteracao.TabelaAlteracao.CfopLoja: return typeof(CfopLojaDAO);
                case LogAlteracao.TabelaAlteracao.DescontoAcrescimoCliente: return typeof(DescontoAcrescimoClienteDAO);
                case LogAlteracao.TabelaAlteracao.ControleUsuario: return typeof(FuncModuloDAO);
                case LogAlteracao.TabelaAlteracao.GrupoProduto: return typeof(GrupoProdDAO);
                case LogAlteracao.TabelaAlteracao.SubgrupoProduto: return typeof(SubgrupoProdDAO);
                case LogAlteracao.TabelaAlteracao.NotaFiscal: return typeof(NotaFiscalDAO);
                case LogAlteracao.TabelaAlteracao.ProdutoNotaFiscal: return typeof(ProdutosNfDAO);
                case LogAlteracao.TabelaAlteracao.Pedido: return typeof(PedidoDAO);
                case LogAlteracao.TabelaAlteracao.Cheque: return typeof(ChequesDAO);
                case LogAlteracao.TabelaAlteracao.Pagto: return typeof(PagtoDAO);
                case LogAlteracao.TabelaAlteracao.UnidadeMedida: return typeof(UnidadeMedidaDAO);
                case LogAlteracao.TabelaAlteracao.Transportador: return typeof(TransportadorDAO);
                case LogAlteracao.TabelaAlteracao.ProdutoLoja: return typeof(ProdutoLojaDAO);
                case LogAlteracao.TabelaAlteracao.PlanoContaContabil: return typeof(PlanoContaContabilDAO);
                case LogAlteracao.TabelaAlteracao.CentroCusto: return typeof(CentroCustoDAO);
                case LogAlteracao.TabelaAlteracao.BemAtivoImobilizado: return typeof(BemAtivoImobilizadoDAO);
                case LogAlteracao.TabelaAlteracao.MovBemAtivoImobilizado: return typeof(MovimentacaoBemAtivoImobDAO);
                case LogAlteracao.TabelaAlteracao.Loja: return typeof(LojaDAO);
                case LogAlteracao.TabelaAlteracao.AdministradoraCartao: return typeof(AdministradoraCartaoDAO);
                case LogAlteracao.TabelaAlteracao.ProjetoModelo: return typeof(ProjetoModeloDAO);
                case LogAlteracao.TabelaAlteracao.PosicaoPecaModelo: return typeof(PosicaoPecaModeloDAO);
                case LogAlteracao.TabelaAlteracao.PecaProjetoModelo: return typeof(PecaProjetoModeloDAO);
                case LogAlteracao.TabelaAlteracao.MaterialProjetoModelo: return typeof(MaterialProjetoModeloDAO);
                case LogAlteracao.TabelaAlteracao.GrupoModelo: return typeof(GrupoModeloDAO);
                case LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual: return typeof(PosicaoPecaIndividualDAO);
                case LogAlteracao.TabelaAlteracao.ProdutoProjeto: return typeof(ProdutoProjetoDAO);
                case LogAlteracao.TabelaAlteracao.ProdutoProjetoConfig: return typeof(ProdutoProjetoConfigDAO);
                case LogAlteracao.TabelaAlteracao.MedidaProjeto: return typeof(MedidaProjetoDAO);
                case LogAlteracao.TabelaAlteracao.Setor: return typeof(SetorDAO);
                case LogAlteracao.TabelaAlteracao.TipoPerda: return typeof(TipoPerdaDAO);
                case LogAlteracao.TabelaAlteracao.TipoCliente: return typeof(TipoClienteDAO);
                case LogAlteracao.TabelaAlteracao.BenefConfig: return typeof(BenefConfigDAO);
                case LogAlteracao.TabelaAlteracao.BenefConfigPreco: return typeof(BenefConfigPrecoDAO);
                case LogAlteracao.TabelaAlteracao.PedidoEspelho: return typeof(PedidoEspelhoDAO);
                case LogAlteracao.TabelaAlteracao.ImagemProducao: return typeof(PecaItemProjetoDAO);
                case LogAlteracao.TabelaAlteracao.MovEstoque: return typeof(MovEstoqueDAO);
                case LogAlteracao.TabelaAlteracao.MovEstoqueFiscal: return typeof(MovEstoqueFiscalDAO);
                case LogAlteracao.TabelaAlteracao.Rota: return typeof(RotaDAO);
                case LogAlteracao.TabelaAlteracao.Sinal: return typeof(SinalDAO);
                case LogAlteracao.TabelaAlteracao.SinalCompra: return typeof(SinalCompraDAO);
                case LogAlteracao.TabelaAlteracao.RetalhoProducao: return typeof(RetalhoProducaoDAO);
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Recupera a propriedade que o log usa.
        /// </summary>
        /// <param name="tipo">O tipo do item que está sendo lido.</param>
        /// <param name="nomeCampoLog">O nome do campo no log.</param>
        /// <returns>A propriedade do log, se encontrada.</returns>
        private PropriedadeLog GetPropriedadeFromLog(Type tipo, string nomeCampoLog)
        {
            foreach (var p in GetPropriedades(tipo))
                if (p.Atributo.Campo == nomeCampoLog)
                    return p;

            return null;
        }

        /// <summary>
        /// Cria um item e define a data de cadastro, se possível.
        /// </summary>
        /// <param name="tipo">O tipo do item.</param>
        /// <param name="dataCad">A data de cadastro</param>
        /// <returns></returns>
        private T GetItem<T>(Type tipo, object itemBase, DateTime dataCad)
        {
            // Cria o item
            T item = (T)Activator.CreateInstance(tipo);

            // Recupera as propriedades do tipo
            var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);

            // Tenta definir a data de cadastro
            foreach (PropertyInfo p in propriedades)
            {
                try
                {
                    if (p.GetSetMethod() == null)
                        continue;

                    p.SetValue(item, p.GetValue(itemBase, null), null);

                    if (p.Name == "DataCad" || p.Name == "Data")
                    {
                        p.SetValue(item, dataCad, null);
                        break;
                    }
                }
                catch { }
            }

            // Retorna o item
            return item;
        }

        #endregion

        public enum TipoCampoRetorno
        {
            Anterior,
            Atual
        }

        /// <summary>
        /// Recria os objetos alterados.
        /// </summary>
        /// <param name="alteracoes"></param>
        /// <param name="forEfd">Se o retorno for para o EFD só retorna um objeto por dia.</param>
        /// <returns></returns>
        public T[] GetItems<T>(LogAlteracao[] alteracoes, TipoCampoRetorno tipoCampoRetorno, bool forEfd)
        {
            // Verifica se há alterações para retorno
            if (alteracoes == null || alteracoes.Length == 0)
                return new T[0];

            // Variável de retorno
            List<T> retorno = new List<T>();

            // Variável de controle dos campos já alterados
            List<string> camposAlterados = new List<string>();

            // Variável de controle das alterações para EFD
            List<DateTime> dataEfd = new List<DateTime>();

            // Recupera o tipo do campo
            LogAlteracao.TabelaAlteracao tabela = (LogAlteracao.TabelaAlteracao)alteracoes[0].Tabela;
            Type tipo = GetType(tabela), tipoDAO = GetDAO(tabela);
            MethodInfo metodo = tipoDAO.GetMethod("GetElementByPrimaryKey", new Type[] { typeof(int) });

            object instance = tipoDAO.GetProperty("Instance", BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).GetValue(null, null);
            object atual = metodo.Invoke(instance, new object[] { alteracoes[0].IdRegistroAlt });

            // Cria um item e salva a data da alteração
            retorno.Add(GetItem<T>(tipo, atual, alteracoes[0].DataAlt));
            DateTime dataAtual = alteracoes[0].DataAlt;

            // Salva a data na lista de controle do EFD
            if (forEfd)
                dataEfd.Add(dataAtual.Date);

            // Percorre todas as alterações
            foreach (LogAlteracao l in alteracoes)
            {
                // Verifica se o campo já foi lido ou se a data é diferente da atual
                // Indica que o objeto mudou
                if (camposAlterados.Contains(l.Campo) || dataAtual.ToString("dd/MM/yyyy HH:mm:ss") != l.DataAlt.ToString("dd/MM/yyyy HH:mm:ss"))
                {
                    if (forEfd && dataEfd.Contains(l.DataAlt.Date))
                        continue;

                    // Cria um novo item
                    retorno.Add(GetItem<T>(tipo, retorno[retorno.Count - 1], l.DataAlt));

                    // Limpa os campos e atualiza a data de comparação
                    camposAlterados.Clear();
                    dataAtual = l.DataAlt;

                    // Salva a data na lista de controle do EFD
                    if (forEfd)
                        dataEfd.Add(dataAtual.Date);
                }

                // Adiciona o campo à lista de controle
                camposAlterados.Add(l.Campo);

                // Recupera a propriedade e altera o valor dela no item
                var prop = GetPropriedadeFromLog(tipo, l.Campo);

                if (prop != null)
                {
                    try
                    {
                        object valorConv = Conversoes.ConverteValor(prop.Propriedade.PropertyType,
                            tipoCampoRetorno == TipoCampoRetorno.Anterior ? l.ValorAnterior : l.ValorAtual);

                        prop.Propriedade.SetValue(retorno[retorno.Count - 1], valorConv, null);
                    }
                    catch { }
                }
            }

            // Retorna a lista
            return retorno.ToArray();
        }

        #endregion

        #region Recupera o nome da propriedade mapeada de um campo

        /// <summary>
        /// Recupera o nome da propriedade mapeada de um campo.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="campo"></param>
        /// <returns></returns>
        internal string GetNomePropriedade(int tabela, string campo)
        {
            foreach (var p in GetPropriedades(GetType((LogAlteracao.TabelaAlteracao)tabela)))
                if (p.Atributo.Campo == campo)
                    return p.Propriedade.Name;

            return null;
        }

        #endregion

        /// <summary>
        /// Copia o log de alterações de imagens-produção da peça do pedido para a peça do pedido espelho.
        /// </summary>
        /// <param name="sessao">Transação.</param>
        /// <param name="idLogComercial">Identificador do log da peça do item do projeto associado ao produto do pedido.</param>
        /// <param name="idLogPCP">Identificador do log da peça do item do projeto associado ao produto do pedido espelho.</param>
        public void CopiarLogAlteracaoImagemProducao(GDASession sessao, int idLogComercial, int idLogPCP)
        {
            var sql = $@"
                SELECT *
                FROM log_alteracao
                WHERE Tabela = {(int)LogAlteracao.TabelaAlteracao.ImagemProducao} AND IdRegistroAlt = " + idLogComercial;
            var logAlteracao = this.objPersistence.LoadData(sessao, sql).ToList();
            foreach (var alteracao in logAlteracao)
            {
                alteracao.IdRegistroAlt = idLogPCP;
                this.Insert(sessao, alteracao);
            }
        }

        /// <summary>
        /// Copia o log de alterações de imagens da peça do pedido para a peça do pedido espelho.
        /// </summary>
        /// <param name="sessao">Transação.</param>
        /// <param name="idProdPed">Identificador do produto do pedido.</param>
        /// <param name="idProdPedEsp">Identificador do produto do pedido espelho.</param>
        public void CopiarLogImagemProdPed(GDASession sessao, int idProdPed, int idProdPedEsp)
        {
            var sql = $@"
                SELECT *
                FROM log_alteracao
                WHERE Tabela = {(int)LogAlteracao.TabelaAlteracao.ImagemProdPed} AND IdRegistroAlt = " + idProdPed;

            var logAlteracao = this.objPersistence.LoadData(sessao, sql);

            foreach (var alteracao in logAlteracao)
            {
                alteracao.IdRegistroAlt = idProdPedEsp;
                alteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.ImagemProdPedEsp;
                this.Insert(sessao, alteracao);
            }
        }
    }
}
