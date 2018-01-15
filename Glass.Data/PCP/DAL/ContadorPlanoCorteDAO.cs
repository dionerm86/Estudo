using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ContadorPlanoCorteDAO : BaseDAO<ContadorPlanoCorte, ContadorPlanoCorteDAO>
    {
        private static object syncRoot = new object();

        /// <summary>
        /// Retorna o próximo valor do contador.
        /// </summary>
        /// <returns></returns>
        public uint GetNext()
        {
            lock (syncRoot)
            {
                objPersistence.ExecuteCommand("Update contador_plano_corte Set contador=contador+1");
                return ExecuteScalar<uint>("Select contador From contador_plano_corte");
            }
        }
    }
}
