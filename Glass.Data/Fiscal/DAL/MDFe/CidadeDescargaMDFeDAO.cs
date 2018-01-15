using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public class CidadeDescargaMDFeDAO : BaseDAO<CidadeDescargaMDFe, CidadeDescargaMDFeDAO>
    {
        #region Busca Padrão

        private string Sql(int idCidadeDescarga, int idManifestoEletronico, bool selecionar)
        {
            var campos = selecionar ? " cd.*, c.NomeCidade" : " COUNT(*)";
            var sql = @"SELECT" + campos +
                @" FROM cidade_descarga_mdfe cd
                LEFT JOIN cidade c ON (cd.IdCidade=c.IdCidade)
                WHERE 1";

            if(idCidadeDescarga > 0)
            {
                sql += " AND cd.IdCidadeDescarga=" + idCidadeDescarga;
            }

            if(idManifestoEletronico > 0)
            {
                sql += " AND cd.IdManifestoEletronico=" + idManifestoEletronico;
            }

            return sql;
        }

        public IList<CidadeDescargaMDFe> GetList(int idManifestoEletronico, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idManifestoEletronico) == 0)
                return new CidadeDescargaMDFe[] { new CidadeDescargaMDFe() };

            var sql = Sql(0, idManifestoEletronico, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal(int idManifestoEletronico)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idManifestoEletronico, false));
        }

        public int GetCount(int idManifestoEletronico)
        {
            int retorno = GetCountReal(idManifestoEletronico);
            return retorno > 0 ? retorno : 1;
        }

        #endregion

        public CidadeDescargaMDFe ObterCidadeDescargaMDFe(int idCidadeDescarga)
        {
            return objPersistence.LoadOneData(Sql(idCidadeDescarga, 0, true));
        }

        public List<CidadeDescargaMDFe> ObterCidadeDescargaPeloManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            var sql = Sql(0, idManifestoEletronico, true);

            return objPersistence.LoadData(sessao ,sql).ToList();
        }

        #region Obter valor dos campos

        public int ObterIdManifestoEletronico(GDASession sessao, int idCidadeDescarga)
        {
            return ObtemValorCampo<int>(sessao ,"IdManifestoEletronico", "IdCidadeDescarga=" + idCidadeDescarga);
        }

        #endregion

        public int CountNFesAssociadoMDFe(GDASession sessao, int idManifestoEletronico)
        {
            var quantidadeNFes = 0;
            var cidadesDescarga = ObterCidadeDescargaPeloManifestoEletronico(sessao, idManifestoEletronico);

            cidadesDescarga.ForEach(c => quantidadeNFes += c.NFesCidadeDescarga.Count());

            return quantidadeNFes;
        }

        public int CountCTesAssociadoMDFe(GDASession sessao, int idManifestoEletronico)
        {
            var quantidadeCTes = 0;
            var cidadesDescarga = ObterCidadeDescargaPeloManifestoEletronico(sessao, idManifestoEletronico);

            cidadesDescarga.ForEach(c => quantidadeCTes += c.CTesCidadeDescarga.Count());

            return quantidadeCTes;
        }

        #region Mêtodos Sobrescritos

        public int DeleteComTransacao(CidadeDescargaMDFe objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Delete(transaction, objDelete);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Delete(GDASession session, CidadeDescargaMDFe objDelete)
        {
            var situacao = ManifestoEletronicoDAO.Instance.ObterSituacaoManifestoEletronico(session, objDelete.IdManifestoEletronico);

            if (situacao != SituacaoEnum.Aberto && situacao != SituacaoEnum.FalhaEmitir)
                throw new Exception("Só é possível alterar MDF-e se ele estiver Aberto ou com Falha ao Emitir.");

            NFeCidadeDescargaMDFeDAO.Instance.DeletarPorIdCidadeDescarga(session, objDelete.IdCidadeDescarga);
            CTeCidadeDescargaMDFeDAO.Instance.DeletarPorIdCidadeDescarga(session, objDelete.IdCidadeDescarga);

            return base.Delete(session, objDelete);
        }

        #endregion
    }
}
