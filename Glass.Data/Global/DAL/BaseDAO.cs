using System;
using System.Collections.Generic;
using GDA;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Linq;

namespace Glass.Data.DAL
{
    public abstract class BaseDAO<Model, DAO> : GDA.BaseDAO<Model>, IDisposable
        where Model : new()
        where DAO : BaseDAO<Model, DAO>, new()
    {        
        #region Enumera��es

        protected enum TipoSql
        {
            Comum,
            Otimizado
        }

        #endregion

        #region Vari�veis Locais

        protected string _sortExpression;
        protected int _startRow, _pageSize;
        protected const int NUMERO_PAGINAS = 35;
        protected const string FILTRO_ADICIONAL = "?filtroAdicional?";

        protected PersistenceObject<Model> objPersistence
        {
            get { return CurrentPersistenceObject; }
        }

        #endregion

        #region Inst�ncia

        /// <summary>
        /// Inst�ncia da DAO.
        /// </summary>
        public static DAO Instance
        {
            get
            {
                //return Glass.Pool.PoolableObject<DAO>.Instance;
                return GDA.GDAOperations.GetDAO<Model, DAO>(); 
            }
        }

        #endregion

        #region M�todos internos

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// Executa um SQL e converte o retorno para um tipo espec�fico.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal T ExecuteScalar<T>(string sql, params GDAParameter[] parameters)
        {
            return ExecuteScalar<T>(null, sql, parameters);
        }

        /// <summary>
        /// Executa um SQL e converte o retorno para um tipo espec�fico.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal T ExecuteScalar<T>(GDASession sessao, string sql, params GDAParameter[] parameters)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, sql, parameters);
            return Conversoes.ConverteValor<T>(retorno);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="campo"></param>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal T ObtemValorCampo<T>(string campo, string where, params GDAParameter[] parameters)
        {
            return ObtemValorCampo<T>(null, campo, where, parameters);
        }

        internal T ObtemValorCampo<T>(GDASession sessao, string campo, string where, params GDAParameter[] parameters)
        {
            string sql = "select " + campo + " from " + objPersistence.TableNameInfo.Name + (!String.IsNullOrEmpty(where) ? " where " + where : String.Empty);
            return ExecuteScalar<T>(sessao, sql, parameters);
        }

        internal List<T> ExecuteMultipleScalar<T>(string sql, params GDAParameter[] parametros)
        {
            return ExecuteMultipleScalar<T>(null, sql, parametros);
        }

        /// <summary>
        /// Executa um SQL e retorna o valor da primeira coluna de todas as linhas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        internal List<T> ExecuteMultipleScalar<T>(GDASession session, string sql, params GDAParameter[] parametros)
        {
            return new GDA.DataAccess().LoadResult(session, sql, parametros).Select(f => Conversoes.ConverteValor<T>(f[0].GetValue())).ToList();
        }

        #endregion

        #region M�todos de busca otimizados

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, out int numeroRegistros, params GDAParameter[] parameters)
        {
            return GetSqlWithLimit(null, sql, sort, startRow, pageSize, aliasTabelaOrdenar, where, semFiltro, out numeroRegistros, parameters);
        }

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(GDASession session, string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, out int numeroRegistros, params GDAParameter[] parameters)
        {
            return GetSqlWithLimit(session, sql, sort, startRow, pageSize, aliasTabelaOrdenar, where, semFiltro, true, out numeroRegistros, parameters);
        }

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, bool utilizarSortComFiltro, out int numeroRegistros, params GDAParameter[] parameters)
        {
            return GetSqlWithLimit(null, sql, sort, startRow, pageSize, aliasTabelaOrdenar, where, semFiltro, utilizarSortComFiltro, out numeroRegistros, parameters);
        }

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(GDASession session, string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, bool utilizarSortComFiltro, out int numeroRegistros, params GDAParameter[] parameters)
        {
            bool temp;
            return GetSqlWithLimit(session, sql, sort, startRow, pageSize, aliasTabelaOrdenar, where, semFiltro, utilizarSortComFiltro, 
                out temp, out numeroRegistros, parameters);
        }

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, bool utilizarSortComFiltro, out bool otimizou, out int numeroRegistros, params GDAParameter[] parameters)
        {
            return GetSqlWithLimit(null, sql, sort, startRow, pageSize, aliasTabelaOrdenar, where, semFiltro, utilizarSortComFiltro, out otimizou, out numeroRegistros, parameters);
        }

        /// <summary>
        /// Recupera o SQL com os ids dos primeiros registros encontrados.
        /// Considera a pagina��o da grid (primeiro registro e n�mero de registros)
        /// </summary>
        protected string GetSqlWithLimit(GDASession session, string sql, string sort, int startRow, int pageSize, string aliasTabelaOrdenar,
            string where, bool semFiltro, bool utilizarSortComFiltro, out bool otimizou, out int numeroRegistros, params GDAParameter[] parameters)
        {
            // Monta a string de ordena��o (se necess�rio)
            string ordenar = (semFiltro || utilizarSortComFiltro) && !string.IsNullOrEmpty(sort) ? " order by " + sort : string.Empty;

            // Monta a string de limite (se necess�rio)
            string limitar = pageSize > 0 ? " limit " + startRow + "," + pageSize : string.Empty;

            // Recupera as cl�usulas FROM, e GROUP BY do SQL e identifica se j� h� um WHERE no comando
            bool temWhere;
            string groupBy;
            string from = From(sql, out temWhere, out groupBy);

            otimizou = false;
            numeroRegistros = 0;

            // S� otimiza consultas sem filtro e que devem ser limitadas e ordenadas
            if (semFiltro && ordenar != string.Empty && limitar != string.Empty)
            {
                try
                {
                    // Recupera a tabela principal do SQL e o seu alias
                    string alias;
                    string tabela = GetTabelaFrom(from, aliasTabelaOrdenar, sort, out alias);

                    // Recupera os campos que comp�e a chave prim�ria da tabela principal
                    bool chaveComposta;
                    string campo = GetCampoChave(session, tabela, out chaveComposta);

                    // Formata o WHERE do SQL para ser usado 
                    string whereInterno = where;
                    whereInterno = string.IsNullOrEmpty(whereInterno) || (" " + whereInterno.ToLower().Trim()).IndexOf(" and ") == 0 ? whereInterno : " and " + whereInterno.Trim();

                    // Remove o alias do WHERE formatado
                    if (!string.IsNullOrEmpty(whereInterno))
                    {
                        whereInterno = whereInterno.Replace(" " + alias + ".", " ");
                        whereInterno = whereInterno.Replace("(" + alias + ".", "(");
                        whereInterno = whereInterno.Replace("=" + alias + ".", "=");
                    }

                    // Recupera os valores dos campos chave usando o WHERE e LIMIT
                    string retorno = GetValoresCampo(session, "select " + campo + " from " + tabela + " where 1" + whereInterno + 
                        ordenar.Replace(alias + ".", string.Empty) + limitar, campo, parameters);

                    numeroRegistros = retorno.Split(',').Length;

                    // Coloca um valor padr�o para o retorno (caso nada seja encontrado)
                    retorno = !string.IsNullOrEmpty(retorno) ? retorno :
                        !chaveComposta ? "0" : "''";

                    // Formata uma chave composta (retorna todos os valores dos campos separadas por uma string)
                    if (chaveComposta)
                    {
                        if (retorno != "''")
                            retorno = "'" + retorno.Replace(",", "','") + "'";

                        campo = "concat(''''," + campo + ",'''')";
                    }

                    // Monta o SQL para retorno
                    if (string.IsNullOrEmpty(groupBy))
                    {
                        retorno = sql + (!temWhere ? " where " : " and ") + (!string.IsNullOrEmpty(alias) ? alias + "." : string.Empty) +
                            campo + " in (" + retorno + ")" + ordenar;
                    }
                    else
                    {
                        string adicionar = (!temWhere ? " where " : " and ") + (!string.IsNullOrEmpty(alias) ? alias + "." : string.Empty) +
                            campo + " in (" + retorno + ")" + groupBy + ordenar;

                        retorno = sql.Replace(groupBy, adicionar);
                    }

                    otimizou = true;
                    return retorno;
                }
                catch
                {
                    // Retorna o SQL original em caso de erro
                    if (string.IsNullOrEmpty(groupBy))
                        return sql + (!temWhere ? " where 1" : string.Empty) + where + ordenar + limitar;
                    else
                    {
                        string adicionar = (!temWhere ? " where 1" : string.Empty) + where + groupBy + ordenar + limitar;
                        return sql.Replace(groupBy, adicionar);
                    }
                }
            }
            else
            {
                // Andr�: Verifica se n�o tem where e se n�o tem order by porque na lista de feriado havia order by e o where foi colocado
                // depois desse, causando erro.
                if (string.IsNullOrEmpty(groupBy))
                    return sql + (!temWhere && !sql.ToLower().Contains("order by") ? " where 1" : string.Empty) + where + " " + ordenar + limitar;
                else
                {
                    string adicionar = (!temWhere ? " where 1" : string.Empty) + where + " " + groupBy + ordenar + limitar;
                    return sql.Replace(groupBy, adicionar);
                }
            }
        }

        protected IList<Model> LoadDataWithSortExpression(string sql, string sortExpression, int startRow, int pageSize,
            params GDAParameter[] parameters)
        {
            return LoadDataWithSortExpression(null, sql, sortExpression, startRow, pageSize, parameters);
        }

        protected IList<Model> LoadDataWithSortExpression(GDASession session, string sql, string sortExpression, int startRow, int pageSize,
            params GDAParameter[] parameters)
        {
            return LoadDataWithSortExpression(session, sql, sortExpression, startRow, pageSize, true, parameters);
        }

        /// <summary>
        /// Retorna um vetor de itens limitados a um n�mero e a partir de um registro espec�fico.
        /// </summary>
        protected IList<Model> LoadDataWithSortExpression(string sql, string sortExpression, int startRow, int pageSize,
            bool temFiltro, params GDAParameter[] parameters)
        {
            return LoadDataWithSortExpression(null, sql, sortExpression, startRow, pageSize, temFiltro, parameters);
        }

        /// <summary>
        /// Retorna um vetor de itens limitados a um n�mero e a partir de um registro espec�fico.
        /// </summary>
        protected IList<Model> LoadDataWithSortExpression(GDASession session, string sql, string sortExpression, int startRow, int pageSize,
            bool temFiltro, params GDAParameter[] parameters)
        {
            return LoadDataWithSortExpression(session, sql, sortExpression, startRow, pageSize, temFiltro, null, parameters);
        }

        /// <summary>
        /// Retorna um vetor de itens limitados a um n�mero e a partir de um registro espec�fico.
        /// </summary>
        protected IList<Model> LoadDataWithSortExpression(string sql, string sortExpression, int startRow, int pageSize,
            bool temFiltro, string where, params GDAParameter[] parameters)
        {
            return LoadDataWithSortExpression(null, sql, sortExpression, startRow, pageSize, temFiltro, where, parameters);
        }

        /// <summary>
        /// Retorna um vetor de itens limitados a um n�mero e a partir de um registro espec�fico.
        /// </summary>
        protected IList<Model> LoadDataWithSortExpression(GDASession session, string sql, string sortExpression, int startRow, int pageSize,
            bool temFiltro, string where, params GDAParameter[] parameters)
        {
            // Habilitar para n�o usar a otimiza��o
            // temFiltro = true;

            // Coloca a cl�usula where otimizada no SQL, se houver filtros que n�o permitem otimiza��o
            sql = sql.Replace(FILTRO_ADICIONAL, temFiltro ? where : string.Empty);

            // Otimiza o SQL e salva as informa��es de ordena��o e pagina��o nas vari�veis privadas
            int numeroRegistros;
            sql = GetSqlWithLimit(session, sql, sortExpression, startRow, pageSize, GetAliasTabela(sql), where, 
                !temFiltro, out numeroRegistros, parameters);

            SetInfoPaging(sortExpression, startRow, pageSize);

            return objPersistence.LoadData(session, sql, parameters).ToList();
        }

        /// <summary>
        /// Atualiza os dados da pagina��o.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        protected void SetInfoPaging(string sortExpression, int startRow, int pageSize)
        {
            _sortExpression = sortExpression;
            _startRow = startRow;
            _pageSize = pageSize;
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        /// <param name="sqlSelecionar"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected int GetCountWithInfoPaging(string sqlSelecionar, params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(sqlSelecionar, false, parameters);
        }
        
        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        /// <param name="sqlSelecionar"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected int GetCountWithInfoPaging(string sqlSelecionar, bool temFiltro, params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(sqlSelecionar, temFiltro, null, parameters);
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(string sqlSelecionar, bool temFiltro, string where, params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(null, sqlSelecionar, temFiltro, where, parameters);
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(GDASession session, string sqlSelecionar, bool temFiltro, string where, params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(session, sqlSelecionar, temFiltro, where, false, parameters);
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(string sqlSelecionar, bool temFiltro, string where, bool utilizarSortComFiltro,
            params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(null, sqlSelecionar, temFiltro, where, utilizarSortComFiltro, parameters);
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(GDASession session, string sqlSelecionar, bool temFiltro, string where, bool utilizarSortComFiltro,
            params GDAParameter[] parameters)
        {
            bool otimizou;

            // Coloca a cl�usula where otimizada no SQL, se houver filtros que n�o permitem otimiza��o
            string sql = sqlSelecionar.Replace(FILTRO_ADICIONAL, temFiltro ? where : String.Empty);

            // Otimiza o SQL, considerando NUMERO_PAGINAS vezes mais registros que o necess�rio (para garantir esse n�mero de p�ginas nas grids)
            int numeroRegistros;
            sql = GetSqlWithLimit(session, sql, _sortExpression, _startRow, _pageSize * NUMERO_PAGINAS, GetAliasTabela(sqlSelecionar),
                where, !temFiltro, utilizarSortComFiltro, out otimizou, out numeroRegistros, parameters);

            _sortExpression = null;

            if (otimizou)
                return numeroRegistros + _startRow;
            else
            {
                bool temWhere;
                string groupBy, from = From(sql, out temWhere, out groupBy);

                // Verifica se deve ser colocado um LIMIT no fim do SQL
                int indexLimit = sql.ToLower().LastIndexOf(" limit ");
                int indexFrom = sql.ToLower().IndexOf(from.ToLower());

                if ((indexLimit == -1 || indexLimit < indexFrom) && _pageSize > 0)
                    sql += " limit " + (otimizou ? 0 : _startRow) + "," + _pageSize;

                return GetCountWithInfoPaging(session, sql, otimizou ? TipoSql.Otimizado : TipoSql.Comum, parameters);
            }
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(string sqlSelecionar, TipoSql tipoSql, params GDAParameter[] parameters)
        {
            return GetCountWithInfoPaging(null, sqlSelecionar, tipoSql, parameters);
        }

        /// <summary>
        /// Retorna o n�mero de registros de um SQL de sele��o.
        /// Usado em conjunto com o m�todo LoadDataWithSortExpression, ou ap�s chamar SetInfoPaging.
        /// </summary>
        protected int GetCountWithInfoPaging(GDASession session, string sqlSelecionar, TipoSql tipoSql, params GDAParameter[] parameters)
        {
            string sql;

            // Recupera as cl�usulas FROM e GROUP BY e verifica se h� um WHERE no SQL
            bool temWhere;
            string groupBy, from = From(sqlSelecionar, out temWhere, out groupBy);

            int indexLimit = sqlSelecionar.ToLower().LastIndexOf(" limit ");
            int indexFrom = sqlSelecionar.ToLower().IndexOf(from.ToLower());

            if ((indexLimit > indexFrom && _pageSize > 0) || !String.IsNullOrEmpty(groupBy))
            {
                if (indexLimit > indexFrom && _pageSize > 0)
                {
                    // Altera o LIMIT do SQL para considerar o n�mero de p�ginas
                    sqlSelecionar = sqlSelecionar.Substring(0, indexLimit) + " limit " + (tipoSql == TipoSql.Otimizado ? 0 : _startRow) + "," +
                        (_pageSize * NUMERO_PAGINAS) + (sqlSelecionar.IndexOf(")", indexLimit) > -1 ?
                        sqlSelecionar.Substring(sqlSelecionar.IndexOf(")", indexLimit)) : String.Empty);
                }

                // Faz a contagem de registros atrav�s de uma subquery
                sql = "select count(*) + " + _startRow + @"
                    from (" + sqlSelecionar + ") as temp";
            }
            else if (tipoSql == TipoSql.Otimizado && !String.IsNullOrEmpty(groupBy))
            {
                // Recupera a tabela principal e seu alias
                string alias, tabela = GetTabelaFrom(from, GetAliasTabela(sqlSelecionar), null, out alias);

                // Recupera os campos que comp�e a chave prim�ria da tabela
                bool chaveComposta;
                string campo = GetCampoChave(session, tabela, out chaveComposta, alias);

                // Faz a contagem das chaves distintas do resultado
                sql = sqlSelecionar.Substring(indexFrom);
                sql = sql.Remove(sql.Length - groupBy.Length);
                sql = "select count(distinct " + campo + ") " + sql;
            }
            else
            {
                // Apenas faz a contagem dos registros que forem encontrados
                sql = "select count(*) " + sqlSelecionar.Substring(indexFrom);
            }

            return ExecuteScalar<int>(session, sql, parameters);
        }

        #endregion

        #region M�todos privados

        private string GetCampoChave(string nomeTabela, out bool chaveComposta)
        {
            return GetCampoChave(null, nomeTabela, out chaveComposta);
        }

        private string GetCampoChave(GDASession session, string nomeTabela, out bool chaveComposta)
        {
            return GetCampoChave(session, nomeTabela, out chaveComposta, null);
        }

        private string GetCampoChave(string nomeTabela, out bool chaveComposta, string alias)
        {
            return GetCampoChave(null, nomeTabela, out chaveComposta, alias);
        }

        private string GetCampoChave(GDASession session, string nomeTabela, out bool chaveComposta, string alias)
        {
            // Formata o alias para ser usado no SQL
            alias = "'" + (!String.IsNullOrEmpty(alias) ? alias.TrimEnd(' ', '.') + "." : "") + "'";

            // Busca os campos que comp�e a chave prim�ria da tabela
            string campo = "select cast(group_concat(concat(" + alias + @", column_name)) as char) from information_schema.key_column_usage 
                where table_schema='" + DBUtils.GetDBName + "' and table_name='" + nomeTabela + "' and constraint_name='primary'";

            // Recupera os campos
            campo = ExecuteScalar<string>(session, campo);
            chaveComposta = campo.Contains(",");

            // Formata o campo para ser usado nos SQLs
            if (chaveComposta)
                campo = "concat(''''," + campo.Replace(",", "','") + ",'''')";

            return campo;
        }

        /// <summary>
        /// Retorna o alias da tabela no SQL.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string GetAliasTabela(string sql)
        {
            // Recupera as cl�usulas FROM e GROUP BY e verifica se o SQL possui WHERE
            bool temWhere;
            string groupBy;
            string from = From(sql, out temWhere, out groupBy);

            // Faz a busca no FROM pelo nome da tabela da DAO
            string aliasTabela = null;
            string tabelaBusca = " " + objPersistence.TableNameInfo.Name + " ";

            if (from.IndexOf(tabelaBusca) > -1)
            {
                aliasTabela = from.Substring(from.IndexOf(tabelaBusca) + tabelaBusca.Length).Trim();
                aliasTabela = aliasTabela.IndexOf(' ') > -1 ? aliasTabela.Substring(0, aliasTabela.IndexOf(' ')).Trim() : null;
            }

            return aliasTabela;
        }

        private static int GetIndex(string sql, string buscar, out Dictionary<int, int> indexes)
        {
            indexes = new Dictionary<int, int>();

            // Busca todos os FROM do SQL
            int index = -1;
            while ((index = sql.ToLower().IndexOf(buscar.ToLower(), index + 1)) > -1)
            {
                int numAbrir = 0, numFechar = 0;
                bool inString = false, ignorarProximo = false;

                foreach (char c in sql.Substring(0, index))
                {
                    if (ignorarProximo)
                    {
                        ignorarProximo = false;
                        continue;
                    }

                    if (c == '(' && !inString)
                        numAbrir++;
                    else if (c == ')' && !inString)
                        numFechar++;
                    else if (inString && c == '\\')
                        ignorarProximo = true;
                    else if (c == '\'')
                        inString = !inString;
                }

                indexes.Add(index, numAbrir - numFechar);
            }

            // Recupera a posi��o do FROM com menor n�mero de par�nteses anteriores
            int[] keys = new int[indexes.Count];
            indexes.Keys.CopyTo(keys, 0);

            index = 0;
            for (int i = 1; i < keys.Length; i++)
                if (indexes[keys[i]] < indexes[keys[index]])
                    index = i;

            return keys.Length > 0 ? keys[index] : -1;
        }

        /// <summary>
        /// Recupera a cl�usula From do SQL.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="temWhere"></param>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        private string From(string sql, out bool temWhere, out string groupBy)
        {
            // Recupera o FROM principal do SQL
            Dictionary<int, int> indexFrom;
            int iFrom = GetIndex(sql, " from ", out indexFrom);
            string from = sql.Substring(iFrom);

            // Verifica se h� algum WHERE
            Dictionary<int, int> indexWhere;
            int iWhere = GetIndex(from, " where ", out indexWhere);
            iWhere = indexWhere.Count > 0 && indexWhere[iWhere] == indexFrom[iFrom] ? iWhere : -1;
            temWhere = iWhere > -1;

            // Coloca a cl�usula GROUP BY em outra vari�vel (se houver)
            Dictionary<int, int> indexGroupBy;
            int iGroupBy = GetIndex(from, "group by ", out indexGroupBy);
            iGroupBy = indexGroupBy.Count > 0 && indexGroupBy[iGroupBy] == indexFrom[iFrom] ? iGroupBy : -1;
            groupBy = iGroupBy > -1 ? " " + from.Substring(iGroupBy) : String.Empty;

            // Remove do FROM a cl�usula WHERE
            from = temWhere ? from.Substring(0, iWhere) : from.Trim();

            return from;
        }

        private string GetTabelaFrom(string from, string aliasTabelaOrdenar, string sort, out string alias)
        {
            // Verifica se h� algum alias no Sort
            string[] dadosSort = sort != null ? sort.Split('.', ' ') : new string[0];
            
            // Define o alias usado
            alias = sort != null && sort.IndexOf('.') > -1 ? dadosSort[0] : aliasTabelaOrdenar;
            if (!string.IsNullOrEmpty(alias) && alias.Contains("("))
                alias = alias.Substring(alias.IndexOf("(") + 1);

            string tabela = objPersistence.TableNameInfo.Name;

            // Busca pelo alias no SQL
            if (from.IndexOf(" " + alias + " ") > -1)
            {
                tabela = from.Substring(0, from.IndexOf(" " + alias + " ")).Trim();
                tabela = tabela.LastIndexOf(' ') > -1 ? tabela.Substring(tabela.LastIndexOf(' ')).Trim() : tabela;
            }
            else if (from.IndexOf(" " + alias + "\r\n") > -1)
            {
                tabela = from.Substring(0, from.IndexOf(" " + alias + "\r\n")).Trim();
                tabela = tabela.LastIndexOf(' ') > -1 ? tabela.Substring(tabela.LastIndexOf(' ')).Trim() : tabela;
            }

            return tabela;
        }

        /// <summary>
        /// Verifica se o campo � chave na tabela.
        /// </summary>
        /// <param name="nomeCampo"></param>
        /// <param name="nomeTabela"></param>
        /// <returns></returns>
        private bool IsCampoChave(string nomeCampo, string nomeTabela)
        {
            string sql = @"select count(*) from information_schema.key_column_usage 
                where table_schema='" + DBUtils.GetDBName + @"' and table_name='" + nomeTabela + @"'
                and column_name='" + nomeCampo + "'";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Atribui o valor da chave identidade � model.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        //private void SetID(Model item, uint id)
        //{
        //    foreach (var a in objPersistence.Keys)
        //    {
        //        if (a.ParameterType == PersistenceParameterType.IdentityKey)
        //        {
        //            a.RepresentedProperty.SetValue(item, id, null);
        //            return;
        //        }
        //    }
        //}

        #endregion

        #region M�todos p�blicos

        /// <summary>
        /// Retorna o nome da tabela no banco de dados.
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return objPersistence.TableNameInfo.Name;
        }
        
        /// <summary>
        /// Insere os dados no BD.
        /// </summary>
        /// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
        /// <returns>Identidade gerada.</returns>
        public virtual uint Insert(Model objInsert)
        {
            var cont = 1;
            uint retorno = 0;

            // M�todo criado para resolver problema no insert de refer�ncia de objeto que ocorre no mysql
            while (true)
            {
                try
                {
                    retorno = objPersistence.Insert(objInsert);
                    break;
                }
                catch
                {
                    if (cont++ > 3)
                        throw;

                    Thread.Sleep(500);
                }
            }

            return retorno;
        }

        public virtual uint Insert(GDASession session, Model objInsert)
        {
            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Atualiza os dados no BD.
        /// </summary>
        /// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
        /// <returns>N�mero de linhas afetadas.</returns>
        public virtual int Update(Model objUpdate)
        {
            return objPersistence.Update(objUpdate);
        }

        /// <summary>
        /// Remove os dados no BD.
        /// </summary>
        /// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
        /// <returns>N�mero de linhas afetadas.</returns>
        public virtual int Delete(Model objDelete)
        {
            return Delete(null, objDelete);
        }

        /// <summary>
        /// Remove os dados no BD.
        /// </summary>
        /// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
        /// <returns>N�mero de linhas afetadas.</returns>
        public virtual int Delete(GDASession session, Model objDelete)
        {
            try
            {
                return objPersistence.Delete(session, objDelete);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is MySqlException && 
                    ex.InnerException.Message.ToLower().Contains("cannot delete or update a parent row: a foreign key constraint fails"))
                    throw new Exception("N�o � poss�vel remover o item: h� outros itens vinculados a ele. Remova-os antes de continuar.", ex.InnerException);

                throw new Exception(ex.Message);
            }
        }

        public virtual int DeleteByPrimaryKey(int key)
        {
            return DeleteByPrimaryKey(null, key);
        }

        public virtual int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            return DeleteByPrimaryKey(sessao, (uint)key);
        }

        /// <summary>
        /// Remove os dados no BD baseando na chave do registro.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        /// <summary>
        /// Remove os dados no BD baseando na chave do registro.
        /// </summary>
        public virtual int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            var listKeys = objPersistence.Keys;

            if (listKeys.Count != 1)
                throw new Exception("Model possui mais de uma chave.");

            string sql =
                "Delete From " + objPersistence.TableNameInfo.Name +
                " where " + listKeys[0].Name + "=" + Key.ToString();

            try
            {
                return objPersistence.ExecuteCommand(sessao, sql);
            }
            catch
            {
                return 0;
            }
        }

        protected virtual uint GetKey(Model obj)
        {
            try
            {
                foreach (PropertyInfo p in typeof(Model).GetProperties())
                {
                    PersistencePropertyAttribute[] a = p.GetCustomAttributes(typeof(PersistencePropertyAttribute), false) as PersistencePropertyAttribute[];
                    if (a == null || a.Length == 0)
                        continue;

                    if (a[0].Name == objPersistence.Keys[0].Name)
                    {
                        var value = p.GetValue(obj, null);

                        if (value is int)
                            return (uint)(int)value;
                        else
                            return (uint)value;
                    }
                }
            }
            catch { }

            return 0;
        }

        protected virtual object[] GetKeys(Model obj)
        {
            object[] o = new object[objPersistence.Keys.Count];

            try
            {
                int i = 0;
                foreach (var key in objPersistence.Keys)
                    o[i++] = key.PropertyMapper.GetValue(obj, null);
            }
            catch { }

            return o;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Exists(Model obj)
        {
            return Exists(null, obj);
        }

        /// <summary>
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Exists(GDASession sessao, Model obj)
        {
            return Exists(sessao, GetKeys(obj));
        }

        /// <summary>
        /// /// (APAGAR: quando alterar para utilizar transa��o)
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual bool Exists(uint key)
        {
            return Exists(null, key);
        }

        /// <summary>
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool Exists(GDASession sessao, uint key)
        {
            return Exists(sessao, (object)key);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual bool Exists(params object[] key)
        {
            return Exists(null, key);
        }

        /// <summary>
        /// Verifica se o registro j� est� cadastrado com base na chave prim�ria.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool Exists(GDASession sessao, params object[] key)
        {
            string sql = "select count(*) from " + objPersistence.TableNameInfo.Name + " where 1";

            int i = 0;
            foreach (object k in key)
                sql += " and " + objPersistence.Keys[i++].Name + "=" + k;

            return ExecuteScalar<int>(sessao, sql) > 0;
        }

        public virtual void InsertOrUpdate(Model objUpdate)
        {
            InsertOrUpdate(null, objUpdate);
        }

        /// <summary>
        /// Se o registro j� existir, atualiza, caso contr�rio insere.
        /// </summary>
        /// <param name="objUpdate">Objeto contendo os dados.</param>
        public virtual void InsertOrUpdate(GDASession sessao, Model objUpdate)
        {
            if (Exists(sessao, objUpdate))
                objPersistence.Update(sessao, objUpdate);
            else
            {
                uint key = GetKey(objUpdate);
                uint id = Insert(sessao, objUpdate);
                
                if (key > 0 && id > 0)
                {
                    objPersistence.ExecuteCommand(sessao, "update " + objPersistence.TableNameInfo.Name + " set " + 
                        objPersistence.Keys[0].Name + "=" + key + " where " + objPersistence.Keys[0].Name + "=" + id);
                }
            }
        }

        /// <summary>
        /// Carrega todos os dados contidos na tabela.
        /// </summary>
        /// <returns>Todos os dados da tabela.</returns>
        public Model[] GetAll()
        {
            return GetAll(null);
        }

        /// <summary>
        /// Carrega todos os dados contidos na tabela.
        /// </summary>
        /// <returns>Todos os dados da tabela.</returns>
        public Model[] GetAll(GDASession session)
        {
            string sql = "select * from " + objPersistence.TableNameInfo.Name;

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Model GetElementByPrimaryKey(int key)
        {
            return GetElementByPrimaryKey(null, key);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Model GetElementByPrimaryKey(uint key)
        {
            return GetElementByPrimaryKey(null, key);
        }

        public Model GetElementByPrimaryKey(GDASession sessao, uint key)
        {
            return GetElementByPrimaryKey(sessao, (int)key);
        }

        public Model GetElementByPrimaryKey(GDASession sessao, int key)
        {
            if (key <= 0)
                return default(Model);

            var listKeys = objPersistence.Keys;

            if (listKeys.Count != 1)
                throw new Exception("Model possui mais de uma chave.");

            string sql = "select * from " + objPersistence.TableNameInfo.Name +
                         " where " + listKeys[0].Name + "=" + key.ToString();
            
            return objPersistence.LoadOneData(sessao, sql);
        }

        public IList<Model> GetAllForList(string sortExpression, int startRow, int pageSize)
        {
            string sql = "select * from " + objPersistence.TableNameInfo.Name;
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false);
        }

        public int GetAllForListCount()
        {
            string sql = "select * from " + objPersistence.TableNameInfo.Name;
            return GetCountWithInfoPaging(sql, false);
        }

        /// <summary>
        /// Recupera os valores de um campo formatados como string e separados por v�rgula.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="nomeCampoSql"></param>
        /// <returns></returns>
        public string GetValoresCampo(string sql, string nomeCampoSql, params GDAParameter[] parameters)
        {
            return GetValoresCampo(null, sql, nomeCampoSql, ",", parameters);
        }

        /// <summary>
        /// Recupera os valores de um campo formatados como string e separados por v�rgula.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="sql"></param>
        /// <param name="nomeCampoSql"></param>
        /// <returns></returns>
        public string GetValoresCampo(GDASession sessao, string sql, string nomeCampoSql, params GDAParameter[] parameters)
        {
            return GetValoresCampo(sessao, sql, nomeCampoSql, ",", parameters);
        }

        /// <summary>
        /// Recupera os valores de um campo formatados como string e separados por v�rgula.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="nomeCampoSql"></param>
        /// <returns></returns>
        public string GetValoresCampo(string sql, string nomeCampoSql, string separador, params GDAParameter[] parameters)
        {
            return GetValoresCampo(null, sql, nomeCampoSql, separador, parameters);
        }

        /// <summary>
        /// Recupera os valores de um campo formatados como string e separados por v�rgula.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="nomeCampoSql"></param>
        /// <returns></returns>
        public string GetValoresCampo(GDASession session, string sql, string nomeCampoSql, string separador, params GDAParameter[] parameters)
        {
            // Busca os dados
            IList<string> dados = ExecuteMultipleScalar<string>(session, "select distinct " +
                nomeCampoSql + " from (" + sql + ") as temp", parameters);

            return String.Join(separador, dados.ToArray());
        }

        #endregion

        #region IDisposable Members

        //~BaseDAO()
        //{
        //    Dispose(false);
        //}

        public void Dispose()
        {
            //Dispose(true);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (Glass.Pool.Pool.IsRegistered<DAO>())
        //        Glass.Pool.Pool.Release<DAO>(this as DAO);
        //}

        #endregion
    }
}
