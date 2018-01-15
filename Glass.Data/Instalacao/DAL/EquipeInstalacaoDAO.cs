using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class EquipeInstalacaoDAO : BaseDAO<EquipeInstalacao, EquipeInstalacaoDAO>
    {
        //private EquipeInstalacaoDAO() { }

        private string Sql(uint idOrdemInstalacao, uint idInstalacao, uint idEquipe, bool selecionar)
        {
            string campos = selecionar ? "ei.*, e.nome" : "count(*)";
            string sql = @"
                select " + campos + @"
                from equipe_instalacao ei
                    left join equipe e on (ei.idEquipe=e.idEquipe)
                where 1";

            if (idOrdemInstalacao > 0)
                sql += " and ei.idOrdemInstalacao=" + idOrdemInstalacao;

            if (idInstalacao > 0)
                sql += " and ei.idInstalacao=" + idInstalacao;

            if (idEquipe > 0)
                sql += " and ei.idEquipe=" + idEquipe;

            return sql;
        }

        public IList<EquipeInstalacao> GetByOrdemInstalacao(uint idOrdemInstalacao)
        {
            return objPersistence.LoadData(Sql(idOrdemInstalacao, 0, 0, true)).ToList();
        }

        public IList<EquipeInstalacao> GetByInstalacao(uint idInstalacao)
        {
            return objPersistence.LoadData(Sql(0, idInstalacao, 0, true)).ToList();
        }

        public IList<EquipeInstalacao> GetByEquipe(uint idEquipe)
        {
            return objPersistence.LoadData(Sql(0, 0, idEquipe, true)).ToList();
        }

        public IList<EquipeInstalacao> GetByInstalacaoEquipe(uint idInstalacao, uint idEquipe)
        {
            return objPersistence.LoadData(Sql(0, idInstalacao, idEquipe, true)).ToList();
        }
    }
}
