using System;
using System.Text;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TipoClienteDAO : BaseDAO<TipoCliente, TipoClienteDAO>
	{
        //private TipoClienteDAO() { }

        public string GetNome(uint idTipoCliente)
        {
            return GetNome(null, idTipoCliente);
        }

        public string GetNome(GDASession session, uint idTipoCliente)
        {
            string sql = "select descricao from tipo_cliente where idTipoCliente=" + idTipoCliente;
            return objPersistence.ExecuteScalar(session, sql).ToString();
        }

        public string GetNomes(string tiposClientes)
        {
            if (tiposClientes == null)
                return String.Empty;

            uint[] tipos = Array.ConvertAll(tiposClientes.Split(','), x => Glass.Conversoes.StrParaUint(x));

            StringBuilder retorno = new StringBuilder();
            foreach (uint tipo in tipos)
                retorno.AppendFormat("{0}, ", GetNome(tipo));

            return retorno.ToString().TrimEnd(',', ' ');
        }

        /// <summary>
        /// Verifica se o cliente é de um tipo que cobra área mínima.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool CobrarAreaMinima(uint idCliente)
        {
            return CobrarAreaMinima(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente é de um tipo que cobra área mínima.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool CobrarAreaMinima(GDASession sessao, uint idCliente)
        {
            uint idTipoCliente = ExecuteScalar<uint>(sessao, "select idTipoCliente from cliente where id_Cli=" + idCliente);
            if (idTipoCliente == 0)
                return true;

            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from tipo_cliente where cobrarAreaMinima=true and idTipoCliente=" + idTipoCliente) > 0;
        }

        public override int Update(TipoCliente objUpdate)
        {
            LogAlteracaoDAO.Instance.LogTipoCliente(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(TipoCliente objDelete)
        {
            return DeleteByPrimaryKey(null, objDelete.IdTipoCliente);
        }

        public override int DeleteByPrimaryKey(GDASession session, int Key)
        {
            if (CurrentPersistenceObject.ExecuteSqlQueryCount(session, "Select * From cliente where idTipoCliente=" + Key) > 0)
                throw new Exception("Existem clientes associados à esse tipo. É necessário desassociar todos os clientes antes de excluir o tipo.");
            
            return GDAOperations.Delete(session, new TipoCliente { IdTipoCliente = Key });
        }
	}
}