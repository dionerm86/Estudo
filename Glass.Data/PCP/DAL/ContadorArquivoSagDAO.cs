using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ContadorArquivoSagDAO : BaseDAO<ContadorArquivoSag, ContadorArquivoSagDAO>
    {
        //private ContadorArquivoSagDAO() { }

        private static object syncRoot = new object();

        /// <summary>
        /// Retorna o próximo valor do contador.
        /// </summary>
        /// <returns></returns>
        public uint GetNext()
        {
            lock (syncRoot)
            {
                objPersistence.ExecuteCommand("Update contador_arquivo_sag Set contador=contador+1");
                return ExecuteScalar<uint>("Select contador From contador_arquivo_sag");
            }
        }
    }
}
