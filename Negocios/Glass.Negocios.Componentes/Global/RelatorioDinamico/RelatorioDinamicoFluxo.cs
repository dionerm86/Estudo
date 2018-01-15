using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Configuracoes;
using Glass.Global.Negocios.Entidades;
using GDA;
using System;

namespace Glass.Global.Negocios.Componentes
{
    public class RelatorioDinamicoFluxo : IRelatorioDinamicoFluxo
    {
        /// <summary>
        /// Cria uma nova instancia do relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public RelatorioDinamico CriarRelatorioDinamico()
        {
            return SourceContext.Instance.Create<Entidades.RelatorioDinamico>();
        }

        /// <summary>
        /// Pesquisa os relatórios dinâmicos 
        /// </summary>
        /// <returns></returns>
        public IList<RelatorioDinamico> PesquisarRelatoriosDinamico()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RelatorioDinamico>("r")
                .Select(@"r.IdRelatorioDinamico, r.NomeRelatorio, r.ComandoSql, r.Situacao")
                .OrderBy("r.NomeRelatorio")
                .ToVirtualResult<Entidades.RelatorioDinamico>();
        }

        /// <summary>
        /// Recupera os dados da conta do banco.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public RelatorioDinamico ObterRelatorioDinamico(int idRelatorioDinamico)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RelatorioDinamico>()
                .Where("IdRelatorioDinamico=?id")
                .Add("?id", idRelatorioDinamico)
                .ProcessLazyResult<Entidades.RelatorioDinamico>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da conta bancária.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarRelatorioDinamico(RelatorioDinamico relatorioDinamico)
        {
            relatorioDinamico.Require("relatorioDinamico").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = relatorioDinamico.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da conta do banco.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarRelatorioDinamico(RelatorioDinamico relatorioDinamico)
        {
            relatorioDinamico.Require("relatorioDinamico").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = relatorioDinamico.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Cria um ícone de relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public RelatorioDinamicoIcone CriarIcone()
        {
            return SourceContext.Instance.Create<RelatorioDinamicoIcone>();
        }

        /// <summary>
        /// Cria um filtro de relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        public RelatorioDinamicoFiltro CriarFiltro()
        {
            return SourceContext.Instance.Create<RelatorioDinamicoFiltro>();
        }

        /// <summary>
        /// Obtêm dados do filtro
        /// </summary>
        /// <param name="comandoSql"></param>
        /// <returns></returns>
        public List<Tuple<string, string>> ObterFiltros(string comandoSql)
        {
            if (!ValidarComandoSql(comandoSql))
                return new List<Tuple<string, string>>();

            var query = new GDA.Sql.NativeQuery(comandoSql);

            return query
                .ToDataRecords()
                .Select(record =>
                 {
                     return new Tuple<string, string>(record.GetString(0), record.GetString(1));
                 }).ToList();
        }

        /// <summary>
        /// Não permite executar certos comandos no banco
        /// </summary>
        /// <param name="comandoSql"></param>
        /// <returns></returns>
        private static bool ValidarComandoSql(string comandoSql)
        {
            comandoSql = comandoSql.ToLower();

            if (comandoSql.Contains("insert") || comandoSql.Contains("update") ||
                comandoSql.Contains("drop") || comandoSql.Contains("delete") ||
                comandoSql.Contains("alter table"))
                return false;

            return true;
        }

        #region Montagem relatório dinâmico

        /// <summary>
        /// Monta o filtro do relatório dinâmico
        /// </summary>
        /// <param name="lstFiltro"></param>
        /// <param name="parametros"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static string MontarFiltroDinamico(List<Tuple<RelatorioDinamicoFiltro, string>> lstFiltro, List<GDAParameter> parametros)
        {
            var filtro = string.Empty;

            foreach (var item in lstFiltro)
            {
                // Se não tiver sido informado nada no filtro, passa para o próximo
                if (string.IsNullOrEmpty(item.Item2) || string.IsNullOrEmpty(item.Item2.Trim(',', ' ')))
                    continue;

                switch (item.Item1.TipoControle)
                {
                    case Data.Model.TipoControle.CaixaDeSelecao:
                        {
                            var valorFiltro = item.Item1.Opcoes;

                            // Se o filtro for um sql a ser executado
                            if (!string.IsNullOrEmpty(valorFiltro) && valorFiltro.Length > 4 && valorFiltro.Substring(0, 4).ToLower() == "sql:")
                            {
                                // Filtra o sql inserido no filtro apenas se a checkbox estiver marcada
                                if (item.Item2 == "1")
                                {
                                    valorFiltro = valorFiltro.Remove(0, 4);

                                    if (!ValidarComandoSql(valorFiltro))
                                        throw new Exception(string.Format("Filtro {0} deve ser apenas seleção de valores.", item.Item1));

                                    filtro += string.Format(" And {0}", valorFiltro);
                                }
                            }
                            else
                            {
                                // Cria o parâmetro do filtro
                                var nomeParametro = string.Format("?{0}", item.Item1.NomeColunaSql);
                                parametros.Add(new GDAParameter(nomeParametro, item.Item2));

                                // Monta o filtro
                                filtro += string.Format(" And {0}={1}", item.Item1.NomeColunaSql, nomeParametro);
                            }

                            break;
                        }
                    case Data.Model.TipoControle.Numero:
                    case Data.Model.TipoControle.ListaDeSelecao:
                        {
                            // Cria o parâmetro do filtro
                            var nomeParametro = string.Format("?{0}", item.Item1.NomeColunaSql);
                            parametros.Add(new GDAParameter(nomeParametro, item.Item2));

                            // Monta o filtro
                            filtro += string.Format(" And {0}={1}", item.Item1.NomeColunaSql, nomeParametro);

                            break;
                        }

                    case Data.Model.TipoControle.Texto:
                        {
                            // Cria o parâmetro do filtro
                            var nomeParametro = string.Format("?{0}", item.Item1.NomeColunaSql);
                            parametros.Add(new GDAParameter(nomeParametro, string.Format("%{0}%", item.Item2)));

                            // Monta o filtro
                            filtro += string.Format(" And {0} Like {1}", item.Item1.NomeColunaSql, nomeParametro);
                            break;
                        }

                    case Data.Model.TipoControle.PeriodoDeData:
                    case Data.Model.TipoControle.PeriodoDeDataHora:
                    case Data.Model.TipoControle.Data:
                    case Data.Model.TipoControle.DataHora:
                        {
                            // Pega o período inicial e final da data
                            var datas = item.Item2.Split('|');

                            // Data Inicial
                            if (!string.IsNullOrEmpty(datas[0]))
                            {
                                // Cria o parâmetro do filtro
                                var nomeParametro = string.Format("?{0}Ini", item.Item1.NomeColunaSql);
                                var dataIni = datas[0] + (item.Item1.TipoControle == Data.Model.TipoControle.PeriodoDeData || 
                                    item.Item1.TipoControle == Data.Model.TipoControle.Data ? " 00:00:00" : "");
                                parametros.Add(new GDAParameter(nomeParametro, DateTime.Parse(dataIni)));

                                // Monta o filtro
                                filtro += string.Format(" And {0}>={1}", item.Item1.NomeColunaSql, nomeParametro);
                            }

                            // Data Final
                            if (datas.Length > 1 && !string.IsNullOrEmpty(datas[1]))
                            {
                                // Cria o parâmetro do filtro
                                var nomeParametro = string.Format("?{0}Fim", item.Item1.NomeColunaSql);
                                var dataFim = datas[1] + (item.Item1.TipoControle == Data.Model.TipoControle.PeriodoDeData ? " 23:59:59" : "");
                                parametros.Add(new GDAParameter(nomeParametro, DateTime.Parse(dataFim)));

                                // Monta o filtro
                                filtro += string.Format(" And {0}<={1}", item.Item1.NomeColunaSql, nomeParametro);
                            }

                            break;
                        }

                    case Data.Model.TipoControle.MultiplaSelecao:
                        {
                            var montagenFiltro = string.Empty;
                            var cont = 1;

                            // Cria um OR para cada valor selecionado
                            foreach (var valor in item.Item2.Split('|'))
                            {
                                // Cria o parâmetro do filtro
                                var nomeParametro = string.Format("?{0}{1}", item.Item1.NomeColunaSql, cont++);
                                parametros.Add(new GDAParameter(nomeParametro, string.Format("{0}", valor)));

                                montagenFiltro += string.Format(" {0}={1} Or", item.Item1.NomeColunaSql, nomeParametro);
                            }

                            // Monta o filtro
                            filtro += string.Format(" And ({0})", montagenFiltro.Substring(0, montagenFiltro.Length - 3));
                            break;
                        }

                    case Data.Model.TipoControle.Cliente:
                        {
                            var colunas = item.Item1.NomeColunaSql.Split(',');
                            var valores = item.Item2.Split('|');

                            var colunaIdCliente = colunas[0];
                            var colunaNomeCliente = colunas[1];

                            var idCliente = valores[0].StrParaInt();
                            var nomeCliente = valores[1];

                            if (idCliente > 0)
                            {
                                var nomeParametro = string.Format("?{0}", colunaIdCliente);
                                parametros.Add(new GDAParameter(nomeParametro, idCliente));
                                filtro += string.Format(" And {0} = {1}", colunaIdCliente, nomeParametro);
                            }
                            else if (!string.IsNullOrWhiteSpace(nomeCliente))
                            {
                                var nomeParametro = string.Format("?{0}", colunaNomeCliente);
                                parametros.Add(new GDAParameter(nomeParametro, string.Format("%{0}%",nomeCliente)));
                                filtro += string.Format(" And {0} LIKE {1}", colunaNomeCliente, nomeParametro);
                            }

                            break;
                        }
                }
            }

            return filtro;
        }

        /// <summary>
        /// Monta uma pesquisa do relatório dinâmico
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> PesquisarListaDinamica(int idRelatorioDinamico, List<Tuple<RelatorioDinamicoFiltro, string>> lstFiltro, int startRow, int pageSize, out int count)
        {
            // Recupera o relatório
            var relatorio = ObterRelatorioDinamico(idRelatorioDinamico);

            // Não permite executar certos comandos no banco
            if (!ValidarComandoSql(relatorio.ComandoSql))
            {
                count = 0;
                return null;
            }

            // Monta filtros
            var parametros = new List<GDAParameter>();
            var filtro = MontarFiltroDinamico(lstFiltro, parametros);

            // Monta a ordenação
            var ordenacao = lstFiltro.Where(f => f.Item1.TipoControle == Data.Model.TipoControle.Ordenacaoo).FirstOrDefault();
            if (ordenacao != null && !string.IsNullOrEmpty(ordenacao.Item2))
                filtro += string.Format(" ORDER BY {0}", ordenacao.Item2);

            // Monta o SQL
            var comandoSql = relatorio.ComandoSql.Replace("[where]", filtro);

            //Determina se a consulta deve ser preparada antes de executada
            var prepare = false;
            if (comandoSql.StartsWith("[@prepare]"))
            {
                comandoSql = comandoSql.Substring(10);
                prepare = true;
            }

            // Cria a query com o comando sql com parâmetros se houver
            var query = new GDA.Sql.NativeQuery(comandoSql);
            if (parametros.Count > 0)
                query.Parameters.AddRange(parametros);

            using (var session = new GDASession())
            {
                if (prepare)
                {
                    var sql = query.ToDataRecords(session)
                    .Select(f => f.GetString(0)).FirstOrDefault();

                    query = new GDA.Sql.NativeQuery(sql);
                }

                //Busca a quantidade de registros.
                var queryCount = new GDA.Sql.NativeQuery(string.Format("SELECT COUNT(*) FROM ({0}) AS tmpCount", query.CommandText.TrimEnd(';')));
                queryCount.Parameters.AddRange(query.Parameters);
                count = queryCount.ToDataRecords(session).Select(f => f.GetInt32(0)).FirstOrDefault();

                return query.Skip(startRow * pageSize).Take(pageSize)
                            .ToDataRecords(session)
                            .Select(record =>
                            {
                                var dic = new Dictionary<string, string>();

                                for (int i = 0; i < record.FieldCount; i++)
                                    dic.Add(record.GetName(i), record.GetString(i));

                                return dic;
                            }).ToList();
            }
        }

        #endregion

        #region Orçamento

        /// <summary>
        /// Verifica se o orçamento pode ser editado
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public bool VerificarPodeEditarOrcamento(int idOrcamento)
        {
            return Data.DAL.OrcamentoDAO.Instance.VerificarPodeEditarOrcamento((uint)idOrcamento);
        }

        #endregion
    }
}
