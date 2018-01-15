using Glass.Data.Model;
using GDA;


namespace Glass.Data.DAL
{
    public class EtiquetaExportacaoDAO : BaseDAO<EtiquetaExportacao, EtiquetaExportacaoDAO>
    {
        #region Exportacao

        /// <summary>
        /// Obtem as etiquetas exportadas pelo idprodped
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idsProdPed"></param>
        /// <returns></returns>
        public EtiquetaExportacao[] ObterEtiquetasPeloIdsProdPed(GDASession sessao, uint[] idsProdPed)
        {
            var sql = string.Format(@"Select * from etiqueta_exportacao where idProdPed IN ({0})", string.Join(",", idsProdPed));
            
            var numEtiquetas = objPersistence.LoadData(sessao, sql);

            return numEtiquetas.ToList().ToArray();
        }

        /// <summary>
        /// Salva a etiqueta exportada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="etiquetaExportacao"></param>
        public void SalvarEtiquetaExportacao(GDASession session, EtiquetaExportacao etiquetaExportacao)
        {
            if (etiquetaExportacao.IdProdPed == 0)
                throw new System.Exception("Falha ao associar etiqueta ao produto pedido! O id do produdo não foi informado");

            if (string.IsNullOrEmpty(etiquetaExportacao.NumEtiqueta))
                throw new System.Exception("Falha ao associar etiqueta ao produto pedido! A etiqueta ser associada não foi informada");

            Insert(session, etiquetaExportacao);
        }

        /// <summary>
        /// Apaga as etiquetas exportadas pelo idprodped
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        public void ApagarEtiquetasPeloProdutoPedido(GDASession sessao, int idProdPed)
        {
            if (idProdPed == 0)
                throw new System.Exception("Erro ao desassociar as etiquetas do produto! O id do produto não foi informado");

            var sql = "Delete from etiqueta_exportacao where idProdPed=" + idProdPed;

            objPersistence.ExecuteScalar(sessao, sql);
        }

        #endregion
    }
}
