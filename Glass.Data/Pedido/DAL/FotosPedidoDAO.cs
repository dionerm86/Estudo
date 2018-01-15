using Glass.Data.Model;
using System.IO;
using GDA;
using System;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class FotosPedidoDAO : BaseDAO<FotosPedido, FotosPedidoDAO>
    {
        //private FotosPedidoDAO() { }

        /// <summary>
        /// Retorna todas as fotos do pedido passado
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosPedido[] GetByPedido(uint idPedido)
        {
            string sql = "Select * From fotos_pedido Where idPedido=" + idPedido;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosPedido objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            // Cria o Log de remoção do Anexo - imagem Pedido
            LogAlteracao log = new LogAlteracao();
            log.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
            log.IdRegistroAlt = (int)objDelete.IdPedido;
            log.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(LogAlteracao.TabelaAlteracao.Pedido, (int)objDelete.IdPedido);
            log.Campo = "Anexo Pedido";
            log.DataAlt = DateTime.Now;
            log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            log.ValorAnterior = string.Format("{0} - Imagem Anexada", objDelete.IdFoto);
            log.ValorAtual = string.Format("{0} - Imagem Removida", objDelete.IdFoto);
            log.Referencia = LogAlteracao.GetReferencia(log.Tabela, objDelete.IdPedido);
            LogAlteracaoDAO.Instance.Insert(log);

            return GDAOperations.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se o pedido possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idPedido)
        {
            string sql = "Select Count(*) From fotos_pedido where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
