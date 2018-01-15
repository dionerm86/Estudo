using System;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ParcelasNaoUsarDAO : BaseDAO<ParcelasNaoUsar, ParcelasNaoUsarDAO>
    {
        //private ParcelasNaoUsarDAO() { }

        public ParcelasNaoUsar[] ObterPeloCliente(int idCliente)
        {
            return ObterPeloCliente(null, idCliente);
        }

        public ParcelasNaoUsar[] ObterPeloCliente(GDASession session, int idCliente)
        {
            var sql =
                string.Format("SELECT * FROM parcelas_nao_usar WHERE IdCliente={0};",
                    idCliente);

            return objPersistence.LoadData(session, sql).ToArray();
        }

        public void DeleteByClienteFornecedor(int? idCliente, int? idFornecedor)
        {
            if (idCliente == null && idFornecedor == null)
                return;

            string sql = "delete from parcelas_nao_usar where ";
            if (idCliente != null)
                sql += "idCliente=" + idCliente.Value;
            else
                sql += "idFornec=" + idFornecedor.Value;

            objPersistence.ExecuteCommand(sql);
        }

        public string ObtemDescricao(int? idCliente, int? idFornecedor)
        {
            string sql = @"select distinct p.descricao from parcelas p
                left join parcelas_nao_usar c on (p.idParcela=c.idParcela and {0})
                where c.idParcela is null";

            if (idCliente != null)
                sql = String.Format(sql, "c.idCliente=" + idCliente.Value);
            else
                sql = String.Format(sql, "c.idFornec=" + idFornecedor.Value);

            var descricao = ExecuteMultipleScalar<string>(sql);
            return String.Join(", ", descricao.OrderBy(x => x).ToArray());
        }

        public void AtualizaLog(int? idCliente, int? idFornecedor, string descricaoAnterior)
        {
            string descricao = ObtemDescricao(idCliente, idFornecedor);
            LogAlteracaoDAO.Instance.LogParcelasNaoUsar(idCliente, idFornecedor, descricaoAnterior, descricao);
        }
    }
}
