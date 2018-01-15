using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ArquivoCalcEngineDAO : BaseDAO<ArquivoCalcEngine, ArquivoCalcEngineDAO>
    {
        //private ArquivoCalcEngineDAO() { }

        #region Busca padrão

        /// <summary>
        /// Sql padrão usado para recuperar os arquivos CalcEngine.
        /// </summary>
        /// <param name="nome"></param>
        private string SqlCalcEngine(string nome, string descricao, bool selecionar, out string filtroAdicional, out bool temFiltro)
        {
            var sql = String.Empty;
            filtroAdicional = String.Empty;
            temFiltro = false;

            sql = @"
                SELECT ac.* FROM arquivo_calcengine ac
                    INNER JOIN arquivo_calcengine_variavel acv ON (ac.IdArquivoCalcEngine=acv.IdArquivoCalcEngine)
                WHERE 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(nome))
            {
                filtroAdicional += " AND ac.Nome LIKE ?Nome";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(descricao))
            {
                filtroAdicional += " AND ac.Descricao LIKE ?Descricao";
                temFiltro = true;
            }

            sql += " GROUP BY ac.IdArquivoCalcEngine ";

            return selecionar ? sql : "SELECT COUNT(*) FROM (" + sql + ") AS Temp;";
        }

        /// <summary>
        /// Recupera uma lista de arquivos CalcEngine.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        public IList<ArquivoCalcEngine> GetListCalcEngine(string nome, string descricao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            bool temFiltro;

            var sql = SqlCalcEngine(nome, descricao, true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var listArqCalcEngine = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, true, filtroAdicional, GetParamCalcEngine(nome, descricao).ToArray());

            //Recupera a lista de Flag Arquivo de Mesa e salva no Arquivo CalcEngine.
            foreach(var lace in listArqCalcEngine)
                lace.FlagsArqMesa = FlagArqMesaArqCalcEngineDAO.Instance.ObtemPorArqCalcEngine((int)lace.IdArquivoCalcEngine).Select(f => f.IdFlagArqMesa).ToArray();

            return listArqCalcEngine;
        }

        /// <summary>
        /// Recupera a quantidade de registros retornados na tela.
        /// </summary>
        /// <param name="nome"></param>
        public int GetListCountCalcEngine(string nome, string descricao)
        {
            string filtroAdicional;
            bool temFiltro;

            var sql = SqlCalcEngine(nome, descricao, true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamCalcEngine(nome, descricao).ToArray());
        }

        /// <summary>
        /// Seta os parâmetros criados no sql padrão.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        private List<GDAParameter> GetParamCalcEngine(string nome, string descricao)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nome))
                lstParam.Add(new GDAParameter("?Nome", "%" + nome + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?Descricao", "%" + descricao + "%"));

            return lstParam;
        }

        #endregion

        #region Obtém dados

        /// <summary>
        /// Retorna o arquivo do CalcEngine que será usado na mesa de corte para a peça passada
        /// </summary>
        /// <param name="idArquivoCalcEngine"></param>
        public ArquivoCalcEngine ObtemArquivoCalcEngine(uint idArquivoCalcEngine)
        {
            var arqCalcEngine = objPersistence.LoadOneData("Select ac.* From arquivo_calcengine ac Where ac.idArquivoCalcEngine=" + idArquivoCalcEngine);

            //Recupera a lista de Flag Arquivo de Mesa e salva no Arquivo CalcEngine.
            arqCalcEngine.FlagsArqMesa = FlagArqMesaArqCalcEngineDAO.Instance.ObtemPorArqCalcEngine((int)arqCalcEngine.IdArquivoCalcEngine).Select(f => f.IdFlagArqMesa).ToArray();

            return arqCalcEngine;
        }

        /// <summary>
        /// Retorna o arquivo do CalcEngine que será usado na mesa de corte para a peça passada
        /// </summary>
        /// <param name="idArquivoCalcEngine"></param>
        public int ObtemUltimoIdArquivoCalcEngine()
        {
            return (int)objPersistence.LoadOneData(@"
                SELECT ac.* From arquivo_calcengine ac
                ORDER BY ac.IdArquivoCalcEngine DESC LIMIT 1").IdArquivoCalcEngine;
        }

        /// <summary>
        /// Retorna o nome do arquivo Calc Engine do Id passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idArquivoCalcEngine"></param>
        /// <returns></returns>
        public string ObtemNomeArquivo(GDASession sessao, uint idArquivoCalcEngine)
        {
            return Instance.ObtemValorCampo<string>(sessao, "nome", "idArquivoCalcEngine=" + idArquivoCalcEngine);
        }

        #endregion

        public uint? FindByNome(uint idArquivoCalcEngine, string nome)
        {
            string trataNome = @"
                Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(nome, ' ', ''), 
                '.', ''), 'ã', 'a'), 'á', 'a'), 'â', 'a'), 'é', 'e'), 'ê', 'e'), 'í', 'i'), 'ç', 'c')";

            GDAParameter p = new GDAParameter("?nome", ArquivoCalcEngine.TrataNome(nome));
            string sql = "select count(*) from arquivo_calcengine where idArquivoCalcEngine=" + idArquivoCalcEngine + " and " + trataNome + "=?nome";
            if (objPersistence.ExecuteSqlQueryCount(sql, p) > 0)
                return idArquivoCalcEngine;

            sql = "select {0} from arquivo_calcengine where " + trataNome + "=?nome";
            if (objPersistence.ExecuteSqlQueryCount(string.Format(sql, "count(*)"), p) > 0)
                return ExecuteScalar<uint?>(string.Format(sql, "idArquivoCalcEngine"), p);

            return null;
        }

        #region Valida nome arquivo novo

        /// <summary>
        /// Médoto para validar o nome de um novo arquivo CalcEngine.
        /// </summary>
        /// <param name="nomeArquivo">Nome do arquivo a ser validado, somente o nome, não informar a extensão do arquivo.</param>
        /// <returns>Retorna True caso não exista registros, com o nome informado, nas tabelas arquivo_mesa_corte e arquivo_cancengine,
        /// e caso não exista um arquivo com este nome na pasta de Arquivos CalcEngine, caso contrário retorna False.</returns>
        public bool ValidaNomeArquivoNovo(string nomeArquivo)
        {
            // Verifica se existe algum arquivo, com o nome informado por parâmetro, na pasta onde são salvos os arquivos CalcEngine.
            var temArquivoCalcEngine = System.IO.File.Exists(Utils.GetArquivoCalcEnginePath + nomeArquivo + ".calcpackage");
            // Verifica se existe algum registro na tabela arquivo_calcengine com o nome do arquivo informado por parâmetro.
            var temRegistroCalcEngine = objPersistence.ExecuteSqlQueryCount("SELECT * FROM arquivo_calcengine WHERE NOME=?NOME",
                new GDA.GDAParameter("?NOME", nomeArquivo)) > 0;

            // Caso exista algum arquivo com o mesmo nome na pasta do sistema, então lança uma exceção.
            if (temRegistroCalcEngine)
                return false;
            // Caso exista o arquivo mas não exista o registro, então o arquivo deve ser apagado.
            else if (temArquivoCalcEngine)
                System.IO.File.Delete(Utils.GetArquivoCalcEnginePath + nomeArquivo + ".calcpackage");

            return true;
        }

        #endregion

        #region Métodos sobescritos

        public override uint Insert(Glass.Data.Model.ArquivoCalcEngine objInsert)
        {
            // Insere o arquivo CalcEngine.
            objInsert.IdArquivoCalcEngine = base.Insert(objInsert);

            // Cria uma instância do arquivo de mesa e associa a identificação do arquivo calcengine.
            var arquivoMesaCorte = new ArquivoMesaCorte();
            arquivoMesaCorte.IdArquivoCalcEngine = (int)objInsert.IdArquivoCalcEngine;
            
            // Insere um arquivo de mesa do tipo.
            ArquivoMesaCorteDAO.Instance.Insert(arquivoMesaCorte);

            var idArqCalcEngine = objInsert.IdArquivoCalcEngine;

            // Associa as Flags ao Arquivo CalcEngine.
            if (objInsert.FlagsArqMesa != null)
                foreach (var id in objInsert.FlagsArqMesa)
                    FlagArqMesaArqCalcEngineDAO.Instance.Insert(new FlagArqMesaArqCalcEngine()
                    {
                        IdArquivoCalcEngine = (int)idArqCalcEngine,
                        IdFlagArqMesa = id
                    });

            return idArqCalcEngine;
        }

        public uint InsertApenasArquivoCalcEngine(ArquivoCalcEngine objInsert)
        {
            return base.Insert(objInsert);
        }

        public override int Update(ArquivoCalcEngine objUpdate)
        {
            base.Update(objUpdate);

            // Associa as Flags ao Arquivo CalcEngine.
            FlagArqMesaArqCalcEngineDAO.Instance.DeletePorArqCalcEngine((int)objUpdate.IdArquivoCalcEngine);
            if (objUpdate.FlagsArqMesa.Length > 0)
                foreach (var id in objUpdate.FlagsArqMesa)
                    FlagArqMesaArqCalcEngineDAO.Instance.Insert(new FlagArqMesaArqCalcEngine()
                    {
                        IdArquivoCalcEngine = (int)objUpdate.IdArquivoCalcEngine,
                        IdFlagArqMesa = id
                    });

            return 1;
        }

        /// <summary>
        /// Exclui o arquivo somente se o mesmo não estiver associado a algum projeto.
        /// </summary>
        /// <param name="objDelete"></param>
        public override int Delete(ArquivoCalcEngine objDelete)
        {
            var nomeArquivo = ObtemNomeArquivo(null, objDelete.IdArquivoCalcEngine);

            // Verifica se existe algum projeto associado ao arquivo CalcEngine.
            if (objPersistence.ExecuteSqlQueryCount(@"
                SELECT COUNT(*) FROM peca_projeto_modelo ppm
                    INNER JOIN arquivo_mesa_corte amc ON (ppm.IdArquivoMesaCorte=amc.IdArquivoMesaCorte)
                    INNER JOIN arquivo_calcengine ac ON (amc.IdArquivoCalcEngine=ac.IdArquivoCalcEngine)
                WHERE ac.IdArquivoCalcEngine IS NOT NULL AND ac.IdArquivoCalcEngine=" + objDelete.IdArquivoCalcEngine + ";") > 0)
                throw new Exception("Não é possível excluir este arquivo pois existem um ou mais projetos associados a ele.");
            // Verifica se existe algum produto associado ao arquivo CalcEngine.
            if (objPersistence.ExecuteSqlQueryCount(@"
                SELECT COUNT(*) FROM produto p
                    INNER JOIN arquivo_mesa_corte amc ON (p.IdArquivoMesaCorte=amc.IdArquivoMesaCorte)
                    INNER JOIN arquivo_calcengine ac ON (amc.IdArquivoCalcEngine=ac.IdArquivoCalcEngine)
                WHERE ac.IdArquivoCalcEngine IS NOT NULL AND ac.IdArquivoCalcEngine=" + objDelete.IdArquivoCalcEngine + ";") > 0)
                throw new Exception("Não é possível excluir este arquivo pois existem um ou mais produtos associados a ele.");
            // Apaga as variáveis associadas ao arquivo CalcEngine.
            ArquivoCalcEngineVariavelDAO.Instance.DeletaPeloIdArquivoCalcEngine(objDelete.IdArquivoCalcEngine);
            // Apaga os arquivos de mesa associados ao arquivo CalcEngine.
            objPersistence.ExecuteCommand("DELETE FROM arquivo_mesa_corte WHERE IdArquivoCalcEngine=" + objDelete.IdArquivoCalcEngine);
            // Apaga o arquivo CalcEngine na pasta Upload.
            if (System.IO.File.Exists(Utils.GetArquivoCalcEnginePath + nomeArquivo + ".calcpackage"))
                System.IO.File.Delete(Utils.GetArquivoCalcEnginePath + nomeArquivo + ".calcpackage");

            FlagArqMesaArqCalcEngineDAO.Instance.DeletePorArqCalcEngine((int)objDelete.IdArquivoCalcEngine);

            return base.Delete(objDelete);
        }

        #endregion
    }
}