using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class EstornoItemCarregamentoDAO : BaseDAO<EstornoItemCarregamento, EstornoItemCarregamentoDAO>
    {
        //private EstornoItemCarregamentoDAO() { }

        private string Sql(uint idItemCarregamento, int idProdPedProducao, bool selecionar)
        {
            var filtro = string.Empty;
            var campos = string.Format("{0}",
                selecionar ? @"eic.*, f.Nome as NomeFuncionario,
                    IF(p.IdProd IS NOT NULL, CONCAT(p.CodInterno, ' - ', p.Descricao), CONCAT('Volume: ', ic.IdVolume)) AS CodInternoDescrPeca" :
                    "COUNT(*)");

            var sql = string.Format(@"
                SELECT {0}
                FROM estorno_item_carregamento eic
                    INNER JOIN funcionario f ON (eic.UsuCad = f.IdFunc)
                    INNER JOIN item_carregamento ic ON (eic.idItemCarregamento = ic.idItemCarregamento)
                    LEFT JOIN produto p ON (ic.IdProd = p.IdProd)
                WHERE 1 {1} 
                {2}", campos, "{0}", "{1}");

            if (idItemCarregamento > 0)
                filtro += string.Format(" AND eic.IdItemCarregamento = {0}", idItemCarregamento);

            if (idProdPedProducao > 0)
                filtro += string.Format(" AND ic.IdProdPedProducao = {0}", idProdPedProducao);

            return string.Format(sql, filtro, " ORDER BY eic.IdEstorno desc");
        }

        public List<EstornoItemCarregamento> GetByIdItemCarregamento(uint idItemCarregamento)
        {
            return objPersistence.LoadData(Sql(idItemCarregamento, 0, true));
        }

        public List<EstornoItemCarregamento> PesquisarPeloIdProdPedProducao(int idProdPedProducao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, idProdPedProducao, true), sortExpression, startRow, pageSize).ToList();
        }

        public int PesquisarPeloIdProdPedProducaoCount(int idProdPedProducao)
        {
            return GetCountWithInfoPaging(Sql(0, idProdPedProducao, false));
        }

        public bool TemEstorno(uint idItemCarregamento)
        {
            return TemEstorno(idItemCarregamento, 0);
        }

        public bool TemEstorno(uint idItemCarregamento, int idProdPedProducao)
        {
            var filtro = string.Empty;
            var sql = @"
                SELECT COUNT(*) 
                FROM estorno_item_carregamento eic 
                    INNER JOIN item_carregamento ic ON (eic.IdItemCarregamento=ic.IdItemCarregamento)
                WHERE 1 {0}";

            if (idItemCarregamento > 0)
                filtro += string.Format(" AND eic.IdItemCarregamento = {0}", idItemCarregamento);

            if (idProdPedProducao > 0)
                filtro += string.Format(" AND ic.IdProdPedProducao = {0}", idProdPedProducao);

            return objPersistence.ExecuteSqlQueryCount(string.Format(sql, filtro)) > 0;
        }

        #region Métodos Sobrescritos

        public override uint Insert(GDA.GDASession session, EstornoItemCarregamento objInsert)
        {
            objInsert.DataCad = DateTime.Now;
            objInsert.UsuCad = UserInfo.GetUserInfo.CodUser;

            var idProdPedProducao = ItemCarregamentoDAO.Instance.ObtemValorCampo<uint>("IdProdPedProducao", $"IdItemCarregamento={objInsert.IdItemCarregamento}");

            if (idProdPedProducao > 0)
            {
                var prodPedProducao = ProdutoPedidoProducaoDAO.Instance.GetElementByPrimaryKey(idProdPedProducao);

                LogAlteracaoDAO.Instance.LogProdPedProducao(session, prodPedProducao, LogAlteracaoDAO.SequenciaObjeto.Atual);
            }

            return base.Insert(session, objInsert);
        }

        public override uint Insert(EstornoItemCarregamento objInsert)
        {
            return Insert(null, objInsert);
        }

        #endregion
    }
}
