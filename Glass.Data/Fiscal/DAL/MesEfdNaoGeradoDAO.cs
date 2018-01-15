using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class MesEfdNaoGeradoDAO : BaseDAO<MesEfdNaoGerado, MesEfdNaoGeradoDAO>
    {
        //private MesEfdNaoGeradoDAO() { }

        private string Sql(int mes, int ano, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            return "select " + campos + " from mes_efd_nao_gerado where mes=" + 
                mes + " and ano=" + ano;
        }

        public bool MesNaoGerado(int mes, int ano)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(mes, ano, false)) > 0;
        }

        // public MesEfdNaoGerado[] GetForEFD(
    }
}
