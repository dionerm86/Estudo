using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class ConfiguracaoDAO : BaseDAO<Configuracao, ConfiguracaoDAO>
	{
        //private ConfiguracaoDAO() { }

        /// <summary>
        /// Método que retorna o SQL para retorno de itens específicos.
        /// </summary>
        /// <param name="selecionar"></param>
        /// <param name="itens"></param>
        /// <returns></returns>
        private string Sql(bool selecionar, params Config.ConfigEnum[] itens)
        {
            string campos = selecionar ? "*" : "Count(*)";
            string idsConfig = "";
            string orderBy = "case idConfig";

            int contador = 0;
            foreach (Config.ConfigEnum item in itens)
            {
                idsConfig += ", " + (int)item;
                orderBy += " when " + (int)item + " then " + (contador++);
            }

            string where = UserInfo.GetUserInfo.IsAdminSync ? "" : " and (exibirApenasAdminSync=false or exibirApenasAdminSync is null)";

            return "select " + campos + " from config where idConfig in (" + idsConfig.Substring(2) + ")" + where +
                " order by if(length(grupo)>0, grupo, 'zzzzzzzzzz'), if(exibirApenasAdminSync, 0, 1), " + orderBy + " end";
        }

        public Configuracao GetItem(Config.ConfigEnum item)
        {
            return GetElementByPrimaryKey((uint)item);
        }

        public IList<Configuracao> GetItens(params Config.ConfigEnum[] itens)
        {
            return objPersistence.LoadData(Sql(true, itens)).ToList();
        }

        /// <summary>
        /// Retorna configurações internas
        /// </summary>
        /// <returns></returns>
        public IList<Configuracao> ObterInternas()
        {
            var sql = @"
                Select * From config 
                    Where ConfigInterna=true
                Order By Aba, Grupo";

            var lista = objPersistence.LoadData(sql).ToList();

            // Concatena a aba com o grupo, para aproveitar a estrutura atual e alterar o código o mínimo possível,
            // tendo em vista que as configurações serão removidas futuramente
            foreach (var config in lista)
                config.Grupo = string.Format("{0} - {1}", config.Aba, config.Grupo);

            return lista;
        }

        public Configuracao[] GetFilhos(Configuracao pai)
        {
            List<Configuracao> retorno = new List<Configuracao>();

            if (pai.Tipo == (int)Config.TipoConfigEnum.GrupoEnumMetodo)
                foreach (GenericModel g in ConfigDAO.Instance.GetListForConfig(pai.IdConfig))
                {
                    Configuracao c = new Configuracao();
                    c.IdConfig = pai.IdConfig;
                    c.Descricao = g.Descr;
                    c.ExibirApenasAdminSync = pai.ExibirApenasAdminSync;
                    c.NomeTipoEnum = pai.NomeTipoEnum;
                    c.NomeTipoMetodo = pai.NomeTipoMetodo;
                    c.Tipo = pai.TipoFilhos;

                    retorno.Add(c);
                }

            return retorno.ToArray();
        }

        public IList<string> ObtemTabelasBD(string nomeBanco)
        {
            return ExecuteMultipleScalar<string>(string.Format("select table_name from information_schema.tables where table_schema='{0}' And table_type<>'view'", nomeBanco));
        }

        public string ObtemDescricao(uint idConfig)
        {
            return ObtemDescricao(null, idConfig);
        }

        public string ObtemDescricao(GDASession session, uint idConfig)
        {
            return ObtemValorCampo<string>(session, "descricao", "idConfig=" + idConfig);
        }

        #region Métodos sobrescritos

        public override uint Insert(Configuracao objInsert)
        {
            return objInsert.IdConfig;
        }          
       
        #endregion
    }
}