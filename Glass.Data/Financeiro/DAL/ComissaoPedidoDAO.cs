using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ComissaoPedidoDAO : BaseDAO<ComissaoPedido, ComissaoPedidoDAO>
	{
        //private ComissaoPedidoDAO() { }

        /// <summary>
        /// Busca listagem dos pedidos relacionados à comissão passada
        /// </summary>
        /// <param name="idComissao"></param>
        /// <returns></returns>
        public ComissaoPedido[] GetComissaoDetalhada(uint[] idComissao)
        {
            string sql = @"
                Select cp.*, p.dataConf as DataConfPedido, p.codCliente, " + (PedidoConfig.LiberarPedido ? "lp.dataLiberacao" : "Null") + @" As DataLiberacao, ?calcTotalPedido,
                    cli.Nome as NomeCliente, cast(coalesce((select sum(valorDebito) from debito_comissao where idComissao=cp.idComissao),0) as decimal(12,2)) as totalDebitoComissao
                From comissao_pedido cp Left Join pedido p On (cp.idPedido=p.idPedido) 
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Left Join cliente cli On (p.idCli=cli.id_Cli) 
                    " + (PedidoConfig.LiberarPedido ? @"Left Join produtos_pedido pp on (p.idPedido=pp.idPedido)
                        Left Join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                        Left Join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                        Left Join liberarpedido lp On (plp.idLiberarPedido=lp.idLiberarPedido)" : "") + @"
                Where idComissao={0}";

            if (PedidoConfig.LiberarPedido)
                sql += " And plp.idProdLiberarPedido Is Not Null group by cp.idComissaoPedido";

            sql = sql.Replace("?calcTotalPedido", PedidoDAO.Instance.SqlCampoTotalLiberacao(true, "totalPedido", "p", "pe", "a", "plp"));

            var retorno = new List<ComissaoPedido>();
            foreach (uint id in idComissao)
            {
                var lstComissao = objPersistence.LoadData(String.Format(sql, id)).ToList();

                Comissao comissao = ComissaoDAO.Instance.GetElement(id);
                for (int i = 0; i < lstComissao.Count; i++)
                {
                    lstComissao[i].TotalComissao = comissao.Total;
                    lstComissao[i].NomeFuncCom = comissao.Nome;
                    lstComissao[i].IdFunc = comissao.IdFunc > 0 ? comissao.IdFunc.Value : comissao.IdComissionado > 0 ? comissao.IdComissionado.Value : comissao.IdInstalador > 0 ? comissao.IdInstalador.Value : comissao.IdGerente.Value;
                    lstComissao[i].TipoFunc = comissao.IdFunc > 0 ? 0 : comissao.IdComissionado > 0 ? 1 : comissao.IdComissionado > 0 ? 2 : 3;
                    // Salva as datas inicial e final utilizadas ao gerar a comissão dos pedidos na lista de comissões para que este período seja mostrado na impressão individual de comissão;
                    lstComissao[i].DataRefIni = comissao.DataRefIni;
                    lstComissao[i].DataRefFim = comissao.DataRefFim;
                }

                retorno.AddRange(lstComissao);
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Exclui todos os pedidos de uma comissão
        /// </summary>
        /// <param name="idComissao"></param>
        public void DeleteByComissao(uint idComissao)
        {
            string sql = "Delete From comissao_pedido Where idComissao=" + idComissao;

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Retorna o valor pago para uma comissão por pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoFunc"></param>
        /// <returns></returns>
        public float GetTotalPagoPedido(uint idPedido, int tipoFunc, uint idInstalador)
        {
            return ExecuteScalar<float>("select valor from (" + PedidoDAO.Instance.SqlTotalComissaoPago(idPedido.ToString(), (Pedido.TipoComissao)tipoFunc, idInstalador) + ") as temp");
        }

        public decimal GetTotalBaseCalcComissaoPedido(uint idPedido, int tipoFunc, uint idInstalador)
        {
            return ExecuteScalar<decimal>("select valor from (" + PedidoDAO.Instance.SqlTotalBaseCalcComissaoPago(idPedido.ToString(), (Pedido.TipoComissao)tipoFunc, idInstalador) + ") as temp");
        }

        public uint[] GetIdsPedidosByComissao(uint idComissao)
        {
            object idsPedidos = objPersistence.ExecuteScalar("select cast(group_concat(idPedido) as char) from comissao_pedido where idComissao=" + idComissao);

            if (idsPedidos == null || string.IsNullOrEmpty(idsPedidos.ToString()))
                return new uint[0];

            string[] ids = idsPedidos.ToString().Split(',');

            uint[] retorno = new uint[ids.Length];
            for (int i = 0; i < retorno.Length; i++)
            {
                uint temp;
                if (uint.TryParse(ids[i], out temp))
                    retorno[i] = temp;
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se há comissão paga para um pedido.
        /// Se houver retorna os ids das comissões.
        /// </summary>
        public string IdsComissoesPagasPedido(uint idPedido)
        {
            return IdsComissoesPagasPedido(null, idPedido);
        }

        /// <summary>
        /// Verifica se há comissão paga para um pedido.
        /// Se houver retorna os ids das comissões.
        /// </summary>
        public string IdsComissoesPagasPedido(GDASession session, uint idPedido)
        {
            string sql = "select idComissao from comissao_pedido where idPedido=" + idPedido +
                " and idComissao in (select distinct idComissao from contas_pagar where paga=true and idComissao is not null)";
            
            return GetValoresCampo(session, sql, "idComissao");
        }

        /// <summary>
        /// Verifica se há comissão gerada para um pedido.
        /// </summary>
        public bool TemComissao(uint idPedido)
        {
            return TemComissao(null, idPedido);
        }

        /// <summary>
        /// Verifica se há comissão gerada para um pedido.
        /// </summary>
        public bool TemComissao(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from comissao_pedido where idPedido=" + idPedido) > 0;
        }
    }
}