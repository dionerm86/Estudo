using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class DebitoComissaoDAO : BaseDAO<DebitoComissao, DebitoComissaoDAO>
    {
        //private DebitoComissaoDAO() { }

        #region Recupera os débitos

        private IList<DebitoComissao> GetListDebitos(uint[] idsComissao, uint idFuncionario, Pedido.TipoComissao tipoComissao)
        {
            string sql = "select * from debito_comissao where 1";

            if (idsComissao != null && idsComissao.Length > 0)
            {
                string ids = "";
                foreach (uint i in idsComissao)
                    ids += i + ",";

                sql += " and idComissao in (" + ids.TrimEnd(',') + ")";
            }

            if (idFuncionario > 0)
                sql += " and idFunc=" + idFuncionario; 
            
            if (tipoComissao != Pedido.TipoComissao.Todos)
                sql += " and idComissao is null and tipoFunc=" + (int)tipoComissao;

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna os débitos que serão descontados da comissão.
        /// </summary>
        /// <param name="idFuncionario">O identificador do funcionário.</param>
        /// <param name="tipoComissao">O tipo de comissão que está sendo paga.</param>
        /// <returns></returns>
        public KeyValuePair<string, decimal> GetDebitos(uint idFuncionario, Pedido.TipoComissao tipoComissao)
        {
            List<string> ids = new List<string>();
            decimal retorno = 0;

            if (idFuncionario > 0)
                foreach (DebitoComissao d in GetListDebitos(null, idFuncionario, tipoComissao))
                {
                    ids.Add(d.IdPedido.ToString());
                    retorno += d.ValorDebito;
                }

            return new KeyValuePair<string, decimal>(String.Join(",", ids.ToArray()), retorno);
        }

        public IList<DebitoComissao> GetForRpt(uint[] idsComissao)
        {
            return GetListDebitos(idsComissao, 0, Pedido.TipoComissao.Todos);
        }

        #endregion

        #region Gerar débito de comissão

        /// <summary>
        /// Gera os débitos para os pedidos.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idFunc"></param>
        /// <param name="tipo"></param>
        /// <param name="valor"></param>
        private void GerarDebito(uint idPedido, uint? idFunc, Pedido.TipoComissao tipo, decimal valor)
        {
            GerarDebito(null, idPedido, idFunc, tipo, valor);
        }

        private void GerarDebito(GDASession session, uint idPedido, uint? idFunc, Pedido.TipoComissao tipo, decimal valor)
        {
            GerarDebito(session, idPedido, idFunc, tipo, valor, null);
        }

        /// <summary>
        /// Gera os débitos para os pedidos.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <param name="idFunc"></param>
        /// <param name="tipo"></param>
        /// <param name="valor"></param>
        private void GerarDebito(GDASession session, uint idPedido, uint? idFunc, Pedido.TipoComissao tipo, decimal valor, uint? idTrocaDevolucao)
        {
            if (idFunc == null)
                return;

            if (objPersistence.ExecuteSqlQueryCount(session, @"select count(*) from debito_comissao
                where tipoFunc=" + (int)tipo + " and idPedido=" + idPedido + 
                (idTrocaDevolucao.GetValueOrDefault() > 0 ? " and idTrocaDevolucao=" + idTrocaDevolucao : " and idTrocaDevolucao is null")) > 0)
                throw new Exception("Débito de comissão já existente para o pedido.");

            DebitoComissao novo = new DebitoComissao();
            novo.IdPedido = idPedido;
            novo.ValorDebito = valor;
            novo.Tipo = tipo;
            novo.IdFunc = idFunc.Value;
            novo.IdTrocaDevolucao = idTrocaDevolucao;

            Insert(session, novo);
        }

        /// <summary>
        /// Gera os débitos das comissões dos pedidos.
        /// </summary>
        public void GeraDebito(uint idPedido, string idsComissoes)
        {
            GeraDebito(null, idPedido, idsComissoes);
        }

        /// <summary>
        /// Gera os débitos das comissões dos pedidos.
        /// </summary>
        public void GeraDebito(GDASession session, uint idPedido, string idsComissoes)
        {
            Pedido ped;

            foreach (Comissao c in ComissaoDAO.Instance.GetByString(session, idsComissoes))
            {
                CancelaDebito(session, idPedido);

                if (c.IdFunc > 0)
                {
                    ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Funcionario);
                    GerarDebito(session, idPedido, c.IdFunc, Pedido.TipoComissao.Funcionario, (decimal)ped.ValorPagoComissao);
                }
                else if (c.IdComissionado > 0)
                {
                    ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Comissionado);
                    if (c.IdComissionado != null)
                        GerarDebito(session, idPedido, c.IdComissionado.Value, Pedido.TipoComissao.Comissionado, (decimal)ped.ValorPagoComissao);
                }
                else if (c.IdInstalador > 0)
                {
                    ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Instalador);
                    if (c.IdInstalador != null)
                        GerarDebito(session, idPedido, c.IdInstalador.Value, Pedido.TipoComissao.Instalador, (decimal)ped.ValorPagoComissao);
                }
            }
        }

        /// <summary>
        /// Atualiza o débito para um pedido.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idProdPed"></param>
        public void AtualizaDebitoPedido(GDASession session, uint idPedido)
        {
            Pedido ped;

            ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Funcionario);
            if (ped != null && ped.ValorComissaoPagar < 0)
                GerarDebito(session, idPedido, ped.IdFunc, Pedido.TipoComissao.Funcionario, -(decimal)ped.ValorComissaoPagar);

            ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Comissionado);
            if (ped != null && ped.ValorComissaoPagar < 0)
                GerarDebito(session, idPedido, ped.IdComissionado, Pedido.TipoComissao.Comissionado, -(decimal)ped.ValorComissaoPagar);

            ped = PedidoDAO.Instance.GetElementComissao(session, idPedido, Pedido.TipoComissao.Instalador);
            if (ped != null && ped.ValorComissaoPagar < 0)
                GerarDebito(session, idPedido, ped.IdInstalador, Pedido.TipoComissao.Instalador, -(decimal)ped.ValorComissaoPagar);
        }

        /// <summary>
        /// Atualiza o débito para um pedido, através dp crédito gerado pela troca/devolução.
        /// </summary>
        public void AtualizaDebitoPedidoTrocaDevolucao(GDASession session, int idPedido, uint idTrocaDevolucao)
        {
            Pedido ped;

            ped = PedidoDAO.Instance.GetElementComissao(session, (uint)idPedido, Pedido.TipoComissao.Funcionario);
            if (ped != null && ped.ValorComissaoPagarTrocaDevolucao > 0)
                GerarDebito(session, (uint)idPedido, ped.IdFunc, Pedido.TipoComissao.Funcionario, ped.ValorComissaoPagarTrocaDevolucao, idTrocaDevolucao);

            ped = PedidoDAO.Instance.GetElementComissao(session, (uint)idPedido, Pedido.TipoComissao.Comissionado);
            if (ped != null && ped.ValorComissaoPagarTrocaDevolucao > 0)
                GerarDebito(session, (uint)idPedido, ped.IdComissionado, Pedido.TipoComissao.Comissionado, ped.ValorComissaoPagarTrocaDevolucao, idTrocaDevolucao);

            ped = PedidoDAO.Instance.GetElementComissao(session, (uint)idPedido, Pedido.TipoComissao.Instalador);
            if (ped != null && ped.ValorComissaoPagarTrocaDevolucao > 0)
                GerarDebito(session, (uint)idPedido, ped.IdInstalador, Pedido.TipoComissao.Instalador, ped.ValorComissaoPagarTrocaDevolucao, idTrocaDevolucao);
        }

        #endregion

        #region Cancela os débitos

        /// <summary>
        /// Cancela os débitos de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        public void CancelaDebito(uint idPedido)
        {
            CancelaDebito(null, idPedido);
        }

        /// <summary>
        /// Cancela os débitos de um pedido.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        public void CancelaDebito(GDASession session, uint idPedido)
        {
            string sql = "delete from debito_comissao where idComissao is null and idPedido=" + idPedido;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Remove os dados da comissão da lista de débitos, informando que os débitos continuam para geração.
        /// </summary>
        /// <param name="idComissao"></param>
        public void CancelaComissao(uint idComissao)
        {
            string sql = "update debito_comissao set idComissao=null where idComissao=" + idComissao + 
                "; delete from debito_comissao where idPedido in (select idPedido from comissao_pedido where idComissao=" + idComissao + ") and idTrocaDevolucao is null";

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Verifica se o pedido pode ser cancelado.
        /// </summary>
        public bool VerificaCancelarPedido(uint idComissao, uint idPedido, Pedido.TipoComissao tipoComissao)
        {
            var sql = string.Format("SELECT COUNT(*) FROM debito_comissao WHERE IdComissao IS NOT NULL AND IdComissao<>{0} AND IdPedido={1} AND TipoFunc={2} AND (IdTrocaDevolucao IS NULL OR IdTrocaDevolucao=0)",
                idComissao, idPedido, (int)tipoComissao);

            return objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        /// <summary>
        /// Remove os dados da comissão da lista de débitos, informando que os débitos continuam para geração.
        /// </summary>
        /// <param name="idComissao"></param>
        /// <param name="idPedido"></param>
        public void CancelaPedido(uint idPedido, Pedido.TipoComissao tipoComissao)
        {
            string sql = "delete from debito_comissao where idComissao is null and idPedido=" + idPedido + 
                " and tipoFunc=" + (int)tipoComissao;

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Verifica se a troca/devolução pode ser cancelada
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool VerificaCancelarTrocaDevolucao(uint idPedido, uint idTrocaDevolucao)
        {
            string sql = "Select * From debito_comissao Where idPedido=" + idPedido + " And idTrocaDevolucao=" + idTrocaDevolucao + " And idComissao > 0";

            return objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        /// <summary>
        /// Remove os débitos da comissão pelo IdPedodo
        /// </summary>
        /// <param name="idPedido"></param>
        public void CancelarTrocaDevolucao(uint idPedido, uint idTrocaDevolucao)
        {
            string sql = "delete from debito_comissao where idPedido=" + idPedido + " And idTrocaDevolucao=" + idTrocaDevolucao;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Marca os débitos como quitados em uma comissão

        /// <summary>
        /// Indica que os débitos foram quitados em uma comissão.
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="idComissao"></param>
        /// <param name="tipoComissao"></param>
        public void MarcaComissao(string idsPedidos, uint idComissao, Pedido.TipoComissao tipoComissao)
        {
            MarcaComissao(null, idsPedidos, idComissao, tipoComissao);
        }

        /// <summary>
        /// Indica que os débitos foram quitados em uma comissão.
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="idComissao"></param>
        /// <param name="tipoComissao"></param>
        public void MarcaComissao(GDASession sessao, string idsPedidos, uint idComissao, Pedido.TipoComissao tipoComissao)
        {
            if (String.IsNullOrEmpty(idsPedidos))
                return;

            string sql = "update debito_comissao set idComissao=" + idComissao + " where idPedido in (" + idsPedidos + 
                ") and tipoFunc=" + (int)tipoComissao + " AND IdComissao IS NULL";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion
    }
}
