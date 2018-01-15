using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ArquivoOtimizacaoDAO : BaseDAO<ArquivoOtimizacao, ArquivoOtimizacaoDAO>
    {
        //private ArquivoOtimizacaoDAO() { }

        private string Sql(uint idFunc, string dataIni, string dataFim, int direcao, uint idPedido, string numEtiqueta, 
            out bool temFiltro, out string filtroAdicional, bool selecionar)
        {
            temFiltro = false;
            filtroAdicional = "";

            string campos = selecionar ? "ao.*, f.nome as nomeFunc" : "count(*)";
            string sql = @"
                select " + campos + @"
                from arquivo_otimizacao ao
                    left join funcionario f on (ao.idFunc=f.idFunc)
                where 1" + FILTRO_ADICIONAL;

            if (idFunc > 0)
                filtroAdicional += " and ao.idFunc=" + idFunc;

            if (!String.IsNullOrEmpty(dataIni))
                filtroAdicional += " and ao.dataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                filtroAdicional += " and ao.dataCad<=?dataFim";

            if (direcao > 0)
                filtroAdicional += " and ao.direcao=" + direcao;

            if (!String.IsNullOrEmpty(numEtiqueta))
                filtroAdicional += @" and ao.idArquivoOtimiz in (select * from (
                    select idArquivoOtimiz from etiqueta_arquivo_otimizacao where numEtiqueta=?numEtiqueta) as temp)";
            else if (idPedido > 0)
                filtroAdicional += @" and ao.idArquivoOtimiz in (select * from (
                    select idArquivoOtimiz from etiqueta_arquivo_otimizacao where numEtiqueta like ?idPedido) as temp)";

            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, uint idPedido, string numEtiqueta)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni = dataIni + " 00:00") : DateTime.Parse(dataIni))));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim = dataFim + " 23:59") : DateTime.Parse(dataFim))));

            if (!String.IsNullOrEmpty(numEtiqueta))
                lst.Add(new GDAParameter("?numEtiqueta", numEtiqueta));
            else if (idPedido > 0)
                lst.Add(new GDAParameter("?idPedido", idPedido + "-%"));

            return lst.ToArray();
        }

        public IList<ArquivoOtimizacao> GetList(uint idFunc, string dataIni, string dataFim, int direcao, uint idPedido, 
            string numEtiqueta, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro = false;
            string filtroAdicional = "";

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "ao.dataCad desc";
            return LoadDataWithSortExpression(Sql(idFunc, dataIni, dataFim, direcao, idPedido, numEtiqueta, 
                out temFiltro, out filtroAdicional, true), sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParams(dataIni, dataFim, idPedido, numEtiqueta));
        }

        public int GetCount(uint idFunc, string dataIni, string dataFim, int direcao, uint idPedido, string numEtiqueta)
        {
            bool temFiltro = false;
            string filtroAdicional = "";

            return GetCountWithInfoPaging(Sql(idFunc, dataIni, dataFim, direcao, idPedido, numEtiqueta, out temFiltro, 
                out filtroAdicional, true), temFiltro, filtroAdicional, GetParams(dataIni, dataFim, idPedido, numEtiqueta));
        }

        /// <summary>
        /// Retorna os pedidos já exportados
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public string PedidosJaExportados(string idsPedido)
        {
            // A variável idsPedido não pode ser passada como parâmetro, caso seja o group_concat retorna somente o primeiro idPedido da lista
            string sql = @"
                Select Cast(group_concat(distinct idpedido) as char) 
                From arquivo_otimizacao ao
                    Inner Join etiqueta_arquivo_otimizacao ea On (ao.idArquivoOtimiz=ea.idArquivoOtimiz) 
                Where direcao=" + (int)ArquivoOtimizacao.DirecaoEnum.Exportar + @"
                    And ea.idPedido In (" + idsPedido + ")";

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Insere um registro na tabela de arquivos otimizados e salva as etiquetas otimizadas.
        /// </summary>
        /// <param name="direcao"></param>
        /// <param name="extensaoArquivo"></param>
        /// <param name="etiquetas"></param>
        /// <param name="codArquivoOtimiz"></param>
        /// <returns></returns>
        public ArquivoOtimizacao InserirArquivoOtimizacao(ArquivoOtimizacao.DirecaoEnum direcao, 
            string extensaoArquivo, List<RelModel.Etiqueta> etiquetas, List<string> codArquivoOtimiz)
        {
            List<string> etiq = new List<string>(), forma = new List<string>();
            foreach (RelModel.Etiqueta e in etiquetas)
            {
                etiq.Add(e.NumEtiqueta);
                forma.Add(e.Forma);
            }

            return InserirArquivoOtimizacao(direcao, extensaoArquivo, etiq, forma, codArquivoOtimiz);
        }

        /// <summary>
        /// Insere um registro na tabela de arquivos otimizados e salva as etiquetas otimizadas.
        /// </summary>
        /// <param name="direcao"></param>
        /// <param name="extensaoArquivo"></param>
        /// <param name="etiquetas"></param>
        /// <param name="formas"></param>
        /// <param name="codArquivoOtimiz"></param>
        /// <returns></returns>
        public ArquivoOtimizacao InserirArquivoOtimizacao(ArquivoOtimizacao.DirecaoEnum direcao,
            string extensaoArquivo, List<string> etiquetas, List<string> formas, List<string> codArquivoOtimiz)
        {
            // Cria o registro na tabela
            ArquivoOtimizacao a = new ArquivoOtimizacao();
            a.IdFunc = UserInfo.GetUserInfo.CodUser;
            a.DataCad = DateTime.Now;
            a.Direcao = (int)direcao;
            a.ExtensaoArquivo = extensaoArquivo.TrimEnd('\"');

            // Estão ocorrendo três erros misteriosos ao chamar esse insert, "The given key was not present in the dictionary", 
            // "Probable I/O race condition..." e "Index out of range", alterei para tentar inserir 5 vezes e caso ocorra erro, salva o mesmo no banco
            var cont = 1;
            while (true)
            {
                try
                {
                    var id = Insert(a);
                    a.IdArquivoOtimizacao = id;
                    break;
                }
                catch
                {
                    Thread.Sleep(500);

                    if (cont++ == 6)
                        throw new Exception("Falha ao inserir arquivo otimização. ln.:162");
                }
            }

            // Insere as etiquetas na tabela auxiliar
            if (etiquetas != null)
                for (int i = 0; i < etiquetas.Count; i++)
                {
                    if (etiquetas[i] == null)
                        continue;

                    EtiquetaArquivoOtimizacao e = new EtiquetaArquivoOtimizacao();
                    e.IdArquivoOtimiz = a.IdArquivoOtimizacao;
                    e.NumEtiqueta = etiquetas[i];
                    e.IdPedido = Glass.Conversoes.StrParaUint(e.NumEtiqueta.Split('-')[0]);
                    e.TemArquivoOtimizacao = codArquivoOtimiz != null && formas != null && formas.Count > i &&
                        !String.IsNullOrEmpty(formas[i]) && codArquivoOtimiz.Contains(formas[i] + ".SAG");

                    // Estão ocorrendo três erros misteriosos ao chamar esse insert, "The given key was not present in the dictionary", 
                    // "Probable I/O race condition..." e "Index out of range", alterei para tentar inserir 5 vezes e caso ocorra erro, salva o mesmo no banco
                    cont = 1;
                    while (true)
                    {
                        try
                        {
                            EtiquetaArquivoOtimizacaoDAO.Instance.Insert(e);
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(500);

                            if (cont++ == 6)
                                throw new Exception("Falha ao inserir etiquetas do arq. arquivo otimização. ln.:195"); ;
                        }
                    }
                }

            return a;
        }

        public IList<GenericModel> GetFuncionarios()
        {
            string sql = "select * from funcionario where idFunc in (select * from (select idFunc from arquivo_otimizacao) as temp)";

            PersistenceObject<Funcionario> objFunc = new PersistenceObject<Funcionario>(GDA.GDASettings.GetProviderConfiguration("WebGlass"));
            var func = objFunc.LoadData(sql).ToList();

            List<GenericModel> retorno = new List<GenericModel>();
            foreach (Funcionario f in func)
                retorno.Add(new GenericModel(f.IdFunc, f.Nome));

            return retorno.ToArray();
        }

        public bool EtiquetaExportada(string numeroEtiqueta)
        {
            return EtiquetaExportada(null, numeroEtiqueta);
        }

        public bool EtiquetaExportada(GDASession session, string numeroEtiqueta)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(0, null, null, 0, 0, numeroEtiqueta, out temFiltro, out filtroAdicional, false).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            sql = sql.Replace("count(*)", "sum(ao.direcao=" + (int)ArquivoOtimizacao.DirecaoEnum.Exportar + ")-" +
                "sum(ao.direcao=" + (int)ArquivoOtimizacao.DirecaoEnum.Importar + ")");

            return ExecuteScalar<int>(session, sql, GetParams(null, null, 0, numeroEtiqueta)) > 0;
        }

        public DateTime ObtemDataUltimaExportacaoEtiqueta(GDASession session, string numeroEtiqueta)
        {
            bool temFiltro;
            string filtroAdicional,
                sql = Sql(0, null, null, (int) ArquivoOtimizacao.DirecaoEnum.Exportar,
                    0, numeroEtiqueta, out temFiltro, out filtroAdicional, true)
                    .Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadData(session, sql + "ORDER BY DataCad DESC",
                GetParams(null, null, 0, numeroEtiqueta)).ToList().Select(f => f.DataCad).FirstOrDefault();
        }

        public int NumeroEtiquetasExportadas(uint idProdPed, int qtdeImprimir)
        {
            int numeroExportadas = 0;
            foreach (var etiqueta in ImpressaoEtiquetaDAO.Instance.ObtemEtiquetasNaoImpressas(idProdPed, qtdeImprimir))
                if (EtiquetaExportada(etiqueta))
                    numeroExportadas++;

            return numeroExportadas;
        }
    }
}
