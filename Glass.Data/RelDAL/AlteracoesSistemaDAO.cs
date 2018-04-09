using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class AlteracoesSistemaDAO : BaseDAO<AlteracoesSistema, AlteracoesSistemaDAO>
    {
        //private AlteracoesSistemaDAO() { }

        #region Busca padrão

        private string Sql(string tipo, string dataIni, string dataFim, int tabela, string campo, 
            uint idFunc, string campoRetorno, bool selecionar)
        {
            string campos = !String.IsNullOrEmpty(campoRetorno) ? "distinct '{0}' as tipo, " + campoRetorno + " as campoRetorno" :
                selecionar ? @"'{0}' as tipo, l.tabela, l.idRegistro{0} as idRegistro, l.campo, {1} as valor, 
                {2} as infoAdicional, l.data{0} as data, l.idFunc{0} as idFunc, l.referencia,
                f.nome as nomeFunc, l.numEvento, '$$$' as criterio" : "count(*)";

            string sql = @"(select {1}
                from log_alteracao l
                    left join funcionario f on (l.idFuncAlt=f.idFunc)
                where 1{2})
                
                {0} (select {3}
                from log_cancelamento l
                    left join funcionario f on (l.idFuncCanc=f.idFunc)
                where 1{4})";

            AlteracoesSistema temp = new AlteracoesSistema();

            string criterio = "";
            string where = "";
            
            if (!String.IsNullOrEmpty(tipo))
            {
                where += " and '{0}'=?tipo";
                temp.Tipo = tipo;
                criterio += "Tipo: " + temp.DescrTipo + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                where += " and l.data{0}>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                where += " and l.data{0}<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            if (tabela > 0 && !String.IsNullOrEmpty(tipo))
            {
                where += " and l.tabela=" + tabela;
                temp.Tabela = tabela;
                criterio += "Tabela: " + temp.NomeTabela + "    ";
            }

            if (idFunc > 0)
            {
                where += " and l.idFunc{0}=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (!String.IsNullOrEmpty(campo))
            {
                where += " and l.campo=?campo";
                criterio += "Campo: " + campo + "    ";
            }

            string valorAlt = "concat('De ''', coalesce(l.valorAnterior, ''), ''' para ''', coalesce(l.valorAtual, ''), '''')";
            string infoAdicionalCanc = "concat('Canc. Manual: ', cast(if(l.cancelamentoManual, 'Sim', 'Não') as char), " +
                "if(length(coalesce(l.motivo, '')) > 0, concat(' / Motivo: ', l.motivo), ''))";

            sql = String.Format(sql, selecionar ? "union all" : "+",
                String.Format(campos, "Alt", valorAlt, "''"), String.Format(where, "Alt"), 
                String.Format(campos, "Canc", "l.valor", infoAdicionalCanc), String.Format(where, "Canc"));

            return "select " + (!selecionar ? "" : "* from (") + 
                sql.Replace("$$$", criterio) + (!selecionar ? "" : ") as temp");
        }

        private GDAParameter[] GetParams(string tipo, string dataIni, string dataFim, string campo)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(tipo))
                lst.Add(new GDAParameter("?tipo", tipo));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(campo))
                lst.Add(new GDAParameter("?campo", campo));

            return lst.ToArray();
        }

        public IList<AlteracoesSistema> GetList(string tipo, string dataIni, string dataFim, int tabela, string campo, uint idFunc,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(tipo, dataIni, dataFim, tabela, campo, idFunc, null, true), 
                sortExpression, startRow, pageSize, GetParams(tipo, dataIni, dataFim, campo));
        }

        public int GetCount(string tipo, string dataIni, string dataFim, int tabela, string campo, uint idFunc)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipo, dataIni, dataFim, tabela, campo, idFunc, null, false),
                GetParams(tipo, dataIni, dataFim, campo));
        }

        public IList<AlteracoesSistema> GetForRpt(string tipo, string dataIni, string dataFim, int tabela, string campo, uint idFunc)
        {
            return objPersistence.LoadData(Sql(tipo, dataIni, dataFim, tabela, campo, idFunc, null, true),
                GetParams(tipo, dataIni, dataFim, campo)).ToList();
        }

        #endregion

        #region Busca de itens para filtro

        #region Métodos privados

        private class DadosFiltro<V, D> where V : IConvertible where D : IConvertible
        {
            public string Tipo;
            public V Valor;
            public D Descricao;
        }
        
        private DadosFiltro<V, D>[] GetFiltro<V, D>(string campoValor, string campoTexto, string tipo, string dataIni, string dataFim,
            int tabela, string campo, uint idFunc, bool incluirTodos, bool separarPorTipo)
            where V : IConvertible
            where D : IConvertible
        {
            campoValor = !String.IsNullOrEmpty(campoValor) ? campoValor : "''";
            campoTexto = !String.IsNullOrEmpty(campoTexto) ? campoTexto : "''";

            string campoRetorno = String.Format("concat(coalesce({0}, ''), ';', coalesce({1}, ''))", campoValor, campoTexto);
            string sql = "select " + (separarPorTipo ? "concat(tipo, ';', campoRetorno)" : "campoRetorno") + @" as campo
                from (" + 
                    Sql(tipo, dataIni, dataFim, tabela, campo, idFunc, campoRetorno, true) + @"
                ) as temp";

            string tabelas = GetValoresCampo(String.Format(sql, campoRetorno), "campo", "|", 
                GetParams(tipo, dataIni, dataFim, campo));
            
            List<DadosFiltro<V, D>> retorno = new List<DadosFiltro<V, D>>();
            if (incluirTodos)
            {
                DadosFiltro<V, D> branco = new DadosFiltro<V, D>();
                branco.Tipo = "";
                branco.Valor = default(V);
                branco.Descricao = (D)Convert.ChangeType("Todos", typeof(D));

                retorno.Add(branco);
            }

            foreach (string t in tabelas.Split('|'))
            {
                try
                {
                    string[] dadosTabela = t.Split(';');

                    DadosFiltro<V, D> novo = new DadosFiltro<V, D>();
                    if (separarPorTipo)
                    {
                        novo.Tipo = dadosTabela[0];
                        novo.Valor = (V)Convert.ChangeType(dadosTabela[1], typeof(V));
                        novo.Descricao = (D)Convert.ChangeType(dadosTabela[2], typeof(D));
                    }
                    else
                    {
                        novo.Valor = (V)Convert.ChangeType(dadosTabela[0], typeof(V));
                        novo.Descricao = (D)Convert.ChangeType(dadosTabela[1], typeof(D));
                    }

                    retorno.Add(novo);
                }
                catch { }
            }

            return retorno.ToArray();
        }

        #endregion

        /// <summary>
        /// Retorna as tabelas para um determinado filtro.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public KeyValuePair<int, string>[] GetTabelas(string tipo, string dataIni, string dataFim, string campo, uint idFunc)
        {
            if (String.IsNullOrEmpty(tipo))
                return new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(0, "Todos (selecione um Tipo)") };

            DadosFiltro<int, string>[] filtros = GetFiltro<int, string>("l.tabela", null, tipo, dataIni, dataFim, 0, 
                campo, idFunc, true, true);

            List<KeyValuePair<int, string>> retorno = new List<KeyValuePair<int, string>>();
            foreach (DadosFiltro<int, string> f in filtros)
            {
                retorno.Add(new KeyValuePair<int, string>(f.Valor, f.Valor == 0 ? "Todos" :
                    f.Tipo == "Alt" ? LogAlteracao.GetDescrTabela(f.Valor) : LogCancelamento.GetDescrTabela(f.Valor)));
            }

            retorno.Sort(new Comparison<KeyValuePair<int, string>>(
                delegate(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
                {
                    return x.Key > 0 ? x.Value.CompareTo(y.Value) : x.Key.CompareTo(y.Key);
                }
            ));

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna os funcionários para um determinado filtro.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public KeyValuePair<uint, string>[] GetFuncionarios(string tipo, string dataIni, string dataFim, int tabela, string campo)
        {
            if (String.IsNullOrEmpty(tipo))
                return new KeyValuePair<uint, string>[] { new KeyValuePair<uint, string>(0, "Todos (selecione um Tipo)") };

            DadosFiltro<uint, string>[] filtros = GetFiltro<uint, string>("f.idFunc", "f.nome", tipo, dataIni, dataFim, 
                tabela, campo, 0, true, false);

            List<KeyValuePair<uint, string>> retorno = new List<KeyValuePair<uint, string>>();
            foreach (DadosFiltro<uint, string> f in filtros)
                retorno.Add(new KeyValuePair<uint, string>(f.Valor, BibliotecaTexto.GetTwoFirstNames(f.Descricao)));

            retorno.Sort(new Comparison<KeyValuePair<uint, string>>(
                delegate(KeyValuePair<uint, string> x, KeyValuePair<uint, string> y)
                {
                    return x.Key > 0 ? x.Value.CompareTo(y.Value) : x.Key.CompareTo(y.Key);
                }
            ));

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna os campos para um determinado filtro.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public KeyValuePair<string, string>[] GetCampos(string tipo, string dataIni, string dataFim, int tabela, uint idFunc)
        {
            DadosFiltro<string, string>[] filtros = GetFiltro<string, string>("l.campo", "l.campo", tipo, dataIni, dataFim, 
                tabela, null, idFunc, false, false);

            List<KeyValuePair<string, string>> retorno = new List<KeyValuePair<string, string>>();
            foreach (DadosFiltro<string, string> f in filtros)
                retorno.Add(new KeyValuePair<string, string>(f.Valor, f.Descricao));

            retorno.Sort(new Comparison<KeyValuePair<string, string>>(
                delegate(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
                {
                    return String.Compare(x.Key, y.Key);
                }
            ));

            return retorno.ToArray();
        }

        #endregion
    }
}
