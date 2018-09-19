using System;
using System.Text;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class GrupoClienteDAO : BaseDAO<GrupoCliente, GrupoClienteDAO>
	{
        //private GrupoClienteDAO() { }

        public string GetNome(uint idGrupoCliente)
        {
            return GetNome(null, idGrupoCliente);
        }

        public string GetNome(GDASession session, uint idGrupoCliente)
        {
            string sql = "select descricao from grupo_cliente where idGrupoCliente=" + idGrupoCliente;
            return objPersistence.ExecuteScalar(session, sql).ToString();
        }

        public string GetNomes(string gruposClientes)
        {
            if (gruposClientes == null)
                return String.Empty;

            uint[] grupos = Array.ConvertAll(gruposClientes.Split(','), x => Glass.Conversoes.StrParaUint(x));

            StringBuilder retorno = new StringBuilder();
            foreach (uint grupo in grupos)
                retorno.AppendFormat("{0}, ", GetNome(grupo));

            return retorno.ToString().TrimEnd(',', ' ');
        }

        public override int Update(GrupoCliente objUpdate)
        {
            LogAlteracaoDAO.Instance.LogGrupoCliente(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(GrupoCliente objDelete)
        {
            return DeleteByPrimaryKey((uint)objDelete.IdGrupoCliente);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select * From cliente where idGrupoCliente=" + Key) > 0)
                throw new Exception("Existem clientes associados à esse grupo. É necessário desassociar todos os clientes antes de excluir o grupo.");

            LogAlteracaoDAO.Instance.ApagaLogGrupoCliente(Key);
            return GDAOperations.Delete(new GrupoCliente { IdGrupoCliente = (int)Key });
        }
    }
}
