using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ContadorRecepcaoEventoDAO : BaseDAO<ContadorRecepcaoEvento, ContadorRecepcaoEventoDAO>
    {
        //private ContadorRecepcaoEventoDAO() { }

        private static object syncRoot = new object();

        /// <summary>
        /// Retorna o próximo valor do contador.
        /// </summary>
        /// <returns></returns>
        public uint GetNext()
        {
            lock (syncRoot)
            {
                objPersistence.ExecuteCommand("Update contador_recepcao_evento Set contador=contador+1");
                return ExecuteScalar<uint>("Select contador From contador_recepcao_evento");
            }
        }
    }
}
