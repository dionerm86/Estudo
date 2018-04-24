using System;
using System.Collections.Generic;
using Glass.Data.Model;
using System.Linq;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class PedidosNotaFiscalDAO : BaseDAO<PedidosNotaFiscal, PedidosNotaFiscalDAO>
    {
        //private PedidosNotaFiscalDAO() { }

        /// <summary>
        /// Retorna os pedidos relacionados à esta nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public Pedido[] GetByNotaFiscal(uint idNf)
        {
            List<PedidosNotaFiscal> pNf = objPersistence.LoadData("select * from pedidos_nota_fiscal where idNf=" + idNf + " and idPedido is not null").ToList();
            string pedidos = "";
            foreach (PedidosNotaFiscal pedido in pNf)
                pedidos += "," + pedido.IdPedido;

            return PedidoDAO.Instance.GetByString(null, pedidos.Substring(1));
        }

        /// <summary>
        /// Retorna os ids dos pedidos relacionados à esta nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public PedidosNotaFiscal[] GetByNf(uint idNf)
        {
            return GetByNf(null, idNf);
        }

        /// <summary>
        /// Retorna os ids dos pedidos relacionados à esta nota fiscal
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public PedidosNotaFiscal[] GetByNf(GDASession sessao, uint idNf)
        {
            return objPersistence.LoadData(sessao, "Select * From pedidos_nota_fiscal Where idNf=" + idNf).ToArray();
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public PedidosNotaFiscal[] GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        public PedidosNotaFiscal[] GetByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, "select * from pedidos_nota_fiscal where idPedido=" + idPedido).ToArray();
        }

        public PedidosNotaFiscal[] GetByCarregamento(uint idCarregamento)
        {
            return objPersistence.LoadData("Select * From pedidos_nota_fiscal Where idCarregamento=" + idCarregamento).ToArray();
        }

        public PedidosNotaFiscal[] GetByLiberacaoPedido(uint idLiberarPedido)
        {
            return objPersistence.LoadData("select * from pedidos_nota_fiscal where idLiberarPedido=" + idLiberarPedido).ToArray();
        }

        public string NotasFiscaisGeradas(GDASession session, uint idPedido)
        {
            var sql = string.Format(@"SELECT CAST(GROUP_CONCAT(nf.NumeroNfe SEPARATOR ', ') AS CHAR) FROM pedidos_nota_fiscal pnf
                    LEFT JOIN nota_fiscal nf ON (pnf.IdNf=nf.IdNf) 
                WHERE pnf.IdPedido={0} AND nf.Situacao NOT IN ({1}, {2}, {3}) ORDER BY nf.NumeroNfe",
                idPedido, (int)NotaFiscal.SituacaoEnum.Cancelada, (int)NotaFiscal.SituacaoEnum.Inutilizada, (int)NotaFiscal.SituacaoEnum.Denegada);

            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null ? retorno.ToString() : null;
        }

        /// <summary>
        /// Verifica se as contas a receber podem ser separadas na autorização da NF-e
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool PodeSepararContasReceberFiscaisEReais(uint idNf)
        {
            try
            {
                uint[] idsL, idsP;
                PodeSepararContasReceberFiscaisEReais(idNf, out idsL, out idsP);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se as contas a receber podem ser separadas na autorização da NF-e
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idsLiberarPedido"></param>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        internal void PodeSepararContasReceberFiscaisEReais(uint idNf, out uint[] idsLiberarPedido, out uint[] idsPedido)
        {
            PodeSepararContasReceberFiscaisEReais(null, idNf, out idsLiberarPedido, out idsPedido);
        }

        /// <summary>
        /// Verifica se as contas a receber podem ser separadas na autorização da NF-e
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <param name="idsLiberarPedido"></param>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public void PodeSepararContasReceberFiscaisEReais(GDASession sessao, uint idNf, out uint[] idsLiberarPedido, out uint[] idsPedido)
        {
            idsLiberarPedido = null;
            idsPedido = null;

            // Garante que a NF-e tenha sido gerada a partir de liberação/pedido
            var pedidosNf = GetByNf(sessao, idNf);
            if (pedidosNf.Length == 0)
                throw new Exception("Nota fiscal não foi gerada de pedido.");

            // Só separa as contas se a NF-e foi gerada a partir de liberação
            if (PedidoConfig.LiberarPedido && pedidosNf.Count(x => x.IdLiberarPedido > 0) == 0)
                throw new Exception("A nota fiscal precisa ser gerada de liberação de pedido.");

            string idsString, nomeCampo;

            // Verifica se há alguma NF-e autorizada anteriormente que contém algum desses pedidos/liberações
            if (PedidoConfig.LiberarPedido)
            {
                idsLiberarPedido = (from item in pedidosNf
                                    where item.IdLiberarPedido > 0
                                    select item.IdLiberarPedido.Value).ToArray();

                idsString = String.Join(",", Array.ConvertAll(idsLiberarPedido.ToArray(), x => x.ToString()));
                nomeCampo = "idLiberarPedido";

                if (String.IsNullOrEmpty(idsString))
                    throw new Exception("Não foi encontrada nenhuma liberação para a nota fiscal.");
                
                /* Chamado 17788. */
                if (objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM liberarpedido WHERE " +
                    nomeCampo + " IN (" + idsString + ") AND Total>0") == 0)
                    throw new Exception("A liberação está com o valor zerado, não é possível efetuar a separação.");
            }
            else
            {
                idsPedido = (from item in pedidosNf
                             where item.IdPedido > 0
                             select item.IdPedido.Value).ToArray();

                idsString = String.Join(",", Array.ConvertAll(idsPedido.ToArray(), x => x.ToString()));
                nomeCampo = "idPedido";

                if (String.IsNullOrEmpty(idsString))
                    throw new Exception("Não foi encontrado nenhum pedido para a nota fiscal.");
            }

            // Só separa as contas se ainda não houver parcela recebida para esses pedidos/liberações
            if (objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from contas_receber where " +
                nomeCampo + " in (" + idsString + ") and !isParcelaCartao and recebida") > 0)
                throw new Exception("Existe pelo menos uma conta recebida que seria utilizada na separação.");

            // Verifica se alguma conta a receber já possui a coluna IDNF preenchida para
            // esses pedidos/liberações
            idsString = GetValoresCampo(sessao, "select idNf from pedidos_nota_fiscal where " + nomeCampo + 
                " in (" + idsString + ")", "idNf");

            if (objPersistence.ExecuteSqlQueryCount(sessao, @"select count(*) from contas_receber
                where !isParcelaCartao and idNf in (" + idsString + ") " + (FinanceiroConfig.FinanceiroRec.ImpedirSeparacaoValorSePossuirPagtoAntecip ? @"and idSinal is Null" : "")) > 0)
                throw new Exception(String.Format("Já houve uma separação de valores para pelo menos um{0} pedido desta nota fiscal.",
                    PedidoConfig.LiberarPedido ? "a liberação de" : ""));
        }
         
        #region Exclui todos os pedidos de uma nota fiscal

        /// <summary>
        /// Exclui todos os produtos de uma nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        public void DeleteByNotaFiscal(GDASession sessao, uint idNf)
        {
            string sql = "Delete From pedidos_nota_fiscal Where idNf=" + idNf;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion
    }
}
