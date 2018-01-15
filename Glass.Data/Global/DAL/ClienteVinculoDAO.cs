using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class ClienteVinculoDAO : BaseDAO<ClienteVinculo, ClienteVinculoDAO>
    {
        //private ClienteVinculoDAO() { }

        #region Criar vínculo de cliente

        /// <summary>
        /// Criar vínculo de cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idClienteVinculo"></param>
        public void CriarVinculo(uint idCliente, uint idClienteVinculo)
        {
            ClienteVinculo cliVinc = new ClienteVinculo();
            cliVinc.IdCliente = idCliente;
            cliVinc.IdClienteVinculo = idClienteVinculo;

            LogAlteracao logCliente = new LogAlteracao();
            logCliente.Tabela = (int)LogAlteracao.TabelaAlteracao.Cliente;
            logCliente.DataAlt = DateTime.Now;
            logCliente.Campo = "Criado Vínculo Cliente";
            logCliente.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            logCliente.IdRegistroAlt = (int)idCliente;
            logCliente.ValorAtual = idClienteVinculo + " - " + ClienteDAO.Instance.GetNome(idClienteVinculo);
            LogAlteracaoDAO.Instance.Insert(logCliente);

            Insert(cliVinc);
        }

        #endregion

        #region Remover cliente vinculado

        /// <summary>
        /// Remover cliente vinculado
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idClienteVinculo"></param>
        public void RemoverVinculo(uint idCliente, uint idClienteVinculo)
        {
            ClienteVinculo cliVinc = new ClienteVinculo();
            cliVinc.IdCliente = idCliente;
            cliVinc.IdClienteVinculo = idClienteVinculo;

            LogAlteracao logCliente = new LogAlteracao();
            logCliente.Tabela = (int)LogAlteracao.TabelaAlteracao.Cliente;
            logCliente.DataAlt = DateTime.Now;
            logCliente.Campo = "Removido Ví­nculo Cliente";
            logCliente.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
            logCliente.IdRegistroAlt = (int)idCliente;
            logCliente.ValorAnterior = idClienteVinculo + " - " + ClienteDAO.Instance.GetNome(idClienteVinculo);
            LogAlteracaoDAO.Instance.Insert(logCliente);

            Delete(cliVinc);
        }

        #endregion

        #region Pesquisar vinculo

        /// <summary>
        /// Recupera a lista de ids de vinculos de um cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetIdsVinculados(uint idCliente)
        {
            return GetIdsVinculados(null, idCliente);
        }

        /// <summary>
        /// Recupera a lista de ids de vinculos de um cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetIdsVinculados(GDASession session, uint idCliente)
        {
            string sql = "select IDCLIENTEVINCULO from cliente_vinculo where IDCLIENTE=?idCli";
            List<uint> ids = objPersistence.LoadResult(session, sql, new GDA.GDAParameter("?idCli", idCliente)).Select(f => f.GetUInt32(0))
                       .ToList();

            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate(uint x)
                {
                    return x.ToString();
                }
            )));
        }

        #endregion
    }
}
