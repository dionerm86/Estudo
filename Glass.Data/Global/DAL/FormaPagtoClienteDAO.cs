using System;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class FormaPagtoClienteDAO : BaseDAO<FormaPagtoCliente, FormaPagtoClienteDAO>
    {
        //private FormaPagtoClienteDAO() { }

        public void DeleteByCliente(int idCliente)
        {
            string sql = "delete from formapagto_cliente where idCliente=" + idCliente;
            objPersistence.ExecuteCommand(sql);
        }

        public string ObtemDescricao(int idCliente)
        {
            string sql = @"select distinct fp.descricao from formapagto fp
                left join formapagto_cliente c on (fp.idFormaPagto=c.idFormaPagto and c.idCliente=" + idCliente + @")
                where c.idFormaPagto is null and !coalesce(apenasSistema, false)";

            var descricao = ExecuteMultipleScalar<string>(sql);
            return String.Join(", ", descricao.OrderBy(x => x).ToArray());
        }

        public void AtualizaLog(int idCliente, string descricaoAnterior)
        {
            string descricao = ObtemDescricao(idCliente);
            LogAlteracaoDAO.Instance.LogFormasPagtoNaoUsar(idCliente, descricaoAnterior, descricao);
        }
    }
}
