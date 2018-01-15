using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class ConfiguracaoLojaDAO : BaseDAO<ConfiguracaoLoja, ConfiguracaoLojaDAO>
	{
        //private ConfiguracaoLojaDAO() { }

        /// <summary>
        /// Método que retorna o SQL para retorno de itens específicos.
        /// </summary>
        /// <param name="selecionar"></param>
        /// <param name="itens"></param>
        /// <returns></returns>
        private string Sql(uint idLoja, Config.ConfigEnum[] itens, bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";
            string where = "";
            foreach (Config.ConfigEnum item in itens)
                where += ", " + (int)item;

            return "select " + campos + " from config_loja cl left join config c on (cl.idConfig=c.idConfig) " +
                "where idConfig in (" + where.Substring(2) + ") and ((idLoja is null and (c.usarLoja=false or c.usarLoja is null)) or " +
                "(idLoja=" + idLoja + " and c.usarLoja=true))";
        }

        private ConfiguracaoLoja GetItem(Config.ConfigEnum item, uint idLoja, bool usarLoja)
        {
            List<ConfiguracaoLoja> retorno = null;

            retorno = objPersistence.LoadData("select * from config_loja where idConfig=" + (int)item +
                " and idLoja" + (usarLoja ? "=" + idLoja : " is null"));

            if ((retorno == null || retorno.Count == 0 || retorno[0].IdConfig == 0) && usarLoja)
                return GetItem(item, idLoja, false);
            else
            {
                if (retorno == null || retorno.Count == 0)
                    return null;

                return retorno[0];
            }
        }

        public ConfiguracaoLoja GetItem(Config.ConfigEnum item, uint idLoja)
        {
            bool usarLoja = ConfiguracaoDAO.Instance.ObtemValorCampo<bool>("usarLoja", "idConfig=" + (uint)item);
            return GetItem(item, idLoja, usarLoja);
        }

        public IList<ConfiguracaoLoja> GetItens(uint idLoja, params Config.ConfigEnum[] itens)
        {
            return objPersistence.LoadData(Sql(idLoja, itens, true)).ToList();
        }

        public bool ExisteConfig(ConfiguracaoLoja config)
        {
            string sql = "Select Count(*) From config_loja Where idConfig=" + config.IdConfig;

            if (config.IdLoja != null)
                sql += " and idLoja=" + config.IdLoja;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override uint Insert(ConfiguracaoLoja objInsert)
        {
            uint id = base.Insert(objInsert);
            if (id > 0)
            {
                var vazio = new ConfiguracaoLoja()
                {
                    IdConfigLoja = id,
                    IdConfig = objInsert.IdConfig,
                    IdLoja = objInsert.IdLoja
                };

                LogAlteracaoDAO.Instance.LogConfigLoja(vazio, LogAlteracaoDAO.SequenciaObjeto.Atual);
            }

            return id;
        }

        public override int Update(ConfiguracaoLoja objUpdate)
        {
            LogAlteracaoDAO.Instance.LogConfigLoja(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
            return base.Update(objUpdate);
        }
	}
}