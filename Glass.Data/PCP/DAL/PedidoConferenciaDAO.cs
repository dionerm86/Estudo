using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class PedidoConferenciaDAO : BaseCadastroDAO<PedidoConferencia, PedidoConferenciaDAO>
    {
        //private PedidoConferenciaDAO() { }

        #region Listagem de pedidos em conferência

        private string SqlConferencia(uint idPedido, uint idConferente, uint idLoja, string nomeCliente, int situacao, int sitPedido, string dataConferencia, string dataFinalIni, string dataFinalFim, bool selecionar)
        {
            string criterios = String.Empty;

            string campos = selecionar ? "pc.*, conf.Nome as Conferente, vend.Nome as Vendedor, l.NomeFantasia as NomeLoja, " +
                "c.Nome as NomeCliente, p.DataEntrega, Coalesce(c.Tel_Cont, c.Tel_Res, c.Tel_Cel) as TelCli, " +
                "Concat(p.EnderecoObra, ' - ', p.BairroObra, ' - ', p.CidadeObra) as LocalObra, '$$$' as Criterio " : "Count(*)";

            string sql = @"
                Select " + campos + @" From pedido_conferencia pc
                    Left Join pedido p On (pc.IdPedido=p.IdPedido)
                    Left Join cliente c On (p.idCli=c.id_Cli)
                    Left Join funcionario conf On (pc.IdConferente=conf.IdFunc)
                    Left Join funcionario vend On (p.IdFunc=vend.IdFunc)
                    Left Join loja l On (p.IdLoja=l.IdLoja)
                Where (p.Situacao=" + (int)Pedido.SituacaoPedido.AtivoConferencia + @"
                    Or p.situacao=" + (int)Pedido.SituacaoPedido.EmConferencia + @"
                    Or pc.situacao=" + (int)PedidoConferencia.SituacaoConferencia.Finalizada + ")";

            if (idConferente > 0)
            {
                sql += " And pc.IdConferente=" + idConferente;
                criterios += "Conferente: " + FuncionarioDAO.Instance.GetNome(idConferente) + "    ";
            }

            if (!String.IsNullOrEmpty(dataConferencia))
            {
                sql += " And (pc.DataIni>=?dataConf1 && pc.DataIni<=?dataConf2)";
                criterios += "Data Conferência: " + dataConferencia + "    ";
            }

            if (!String.IsNullOrEmpty(dataFinalIni))
            {
                sql += " And pc.DataFim>=?dataFinIni";
                criterios += "Data Finalização: " + dataFinalIni;

                if (String.IsNullOrEmpty(dataFinalFim))
                    criterios += "    ";
            }

            if (!String.IsNullOrEmpty(dataFinalFim))
            {
                sql += " And pc.DataFim<=?dataFinFim";

                if (!String.IsNullOrEmpty(dataFinalIni))
                    criterios += " a " + dataFinalFim + "    ";
                else
                    criterios += "Data Finalização: até " + dataFinalFim + "    ";
            }

            if (idPedido > 0)
            {
                sql += " And pc.IdPedido=" + idPedido;
                criterios += "Num. Pedido: " + idPedido + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And p.IdLoja=" + idLoja;
                criterios += LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterios += "Cliente: " + nomeCliente;
            }

            if (situacao > 0)
            {
                sql += " And pc.situacao=" + situacao;
                criterios += "Sit. Confer.: ";
                criterios += situacao == 1 ? "Aberta" : situacao == 2 ? "Em Andamento" : situacao == 3 ? "Finalizada" : situacao == 4 ? "Cancelada" : String.Empty;
                criterios += "    ";
            }

            if (sitPedido > 0)
            {
                sql += " And p.situacao=" + sitPedido;
                criterios += "Sit. Pedido: ";
                criterios += sitPedido == 2 ? "Ativo/Em Conferência" : sitPedido == 3 ? "Em Conferência" : sitPedido == 4 ? "Conferido" : sitPedido == 5 ? "Confirmado" : sitPedido == 6 ? "Cancelado" : String.Empty;
                criterios += "    ";
            }

            // Se o usuário logado for vendedor ou aux. administrativo, busca apenas pedidos em conferência do mesmo
            LoginUsuario login = UserInfo.GetUserInfo;
            if (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor)
                sql += " And p.IdFunc=" + login.CodUser;

            return sql.Replace("$$$", criterios);
        }

        public IList<PedidoConferencia> GetForRpt(uint idPedido, uint idConferente, uint idLoja, string nomeCliente, int situacao, int sitPedido, string dataConferencia, string dataFinalIni, string dataFinalFim)
        {
            return objPersistence.LoadData(SqlConferencia(idPedido, idConferente, idLoja, nomeCliente, situacao, sitPedido, dataConferencia, dataFinalIni, dataFinalFim, true), GetParam(nomeCliente, dataConferencia, dataFinalIni, dataFinalFim)).ToList();
        }

        public IList<PedidoConferencia> GetList(uint idPedido, uint idConferente, uint idLoja, string nomeCliente, int situacao, int sitPedido, string dataConferencia, string dataFinalIni, string dataFinalFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlConferencia(idPedido, idConferente, idLoja, nomeCliente, situacao, sitPedido, dataConferencia, dataFinalIni, dataFinalFim, true), sortExpression, startRow, pageSize, GetParam(nomeCliente, dataConferencia, dataFinalIni, dataFinalFim));
        }

        public int GetCount(uint idPedido, uint idConferente, uint idLoja, string nomeCliente, int situacao, int sitPedido, string dataConferencia, string dataFinalIni, string dataFinalFim)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlConferencia(idPedido, idConferente, idLoja, nomeCliente, situacao, sitPedido, dataConferencia, dataFinalIni, dataFinalFim, false), GetParam(nomeCliente, dataConferencia, dataFinalIni, dataFinalFim));
        }

        private GDAParameter[] GetParam(string nomeCliente, string dataConferencia, string dataFinalIni, string dataFinalFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataConferencia))
            {
                lstParam.Add(new GDAParameter("?dataConf1", DateTime.Parse(dataConferencia + " 00:00")));
                lstParam.Add(new GDAParameter("?dataConf2", DateTime.Parse(dataConferencia + " 23:59")));
            }

            if (!String.IsNullOrEmpty(dataFinalIni))
                lstParam.Add(new GDAParameter("?dataFinIni", DateTime.Parse(dataFinalIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFinalFim))
                lstParam.Add(new GDAParameter("?dataFinFim", DateTime.Parse(dataFinalFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Cria um novo pedido em conferencia

        /// <summary>
        /// Cria uma conferência para o pedido, alterando a situação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="recebeuSinal"></param>
        public void NovaConferencia(uint idPedido, bool recebeuSinal)
        {
            NovaConferencia(null, idPedido, recebeuSinal);
        }

        /// <summary>
        /// Cria uma conferência para o pedido, alterando a situação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="recebeuSinal"></param>
        public void NovaConferencia(GDASession sessao, uint idPedido, bool recebeuSinal)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From pedido_conferencia where idPedido=" + idPedido) <= 0)
            {
                PedidoConferencia pedConf = new PedidoConferencia();
                pedConf.IdPedido = idPedido;
                pedConf.Situacao = (int)PedidoConferencia.SituacaoConferencia.Aberta;

                // Insere novo pedido_confer
                Insert(sessao, pedConf);

                // Se a conferencia ainda assim não tiver sido inserida, lança mensagem de erro.
                if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From pedido_conferencia where idPedido=" + idPedido) <= 0)
                    throw new Exception("Falha ao enviar Pedido para Conferência.");

                int registrosAfetados = 0;

                if (recebeuSinal)
                    // Ativo/Conferência
                    registrosAfetados = PedidoDAO.Instance.AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.AtivoConferencia);
                else
                    // Em conferência
                    registrosAfetados = PedidoDAO.Instance.AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.EmConferencia);

                // Se a atualização da situação do pedido não tiver sido realizada, lança exceção
                if (registrosAfetados <= 0)
                {
                    DeleteByPrimaryKey(sessao, idPedido);
                    throw new Exception("Falha ao enviar Pedido para Conferência.");
                }
            }
            else
            {
                if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From pedido_conferencia where idPedido=" + idPedido +
                    " And Situacao=" + (int)PedidoConferencia.SituacaoConferencia.Finalizada) > 0)
                    throw new Exception("Este pedido já foi para conferência e a mesma já foi finalizada.");

                if (recebeuSinal)
                    // Ativo/Conferência
                    PedidoDAO.Instance.AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.AtivoConferencia);
                else
                    // Em conferência
                    PedidoDAO.Instance.AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.EmConferencia);

                throw new Exception("Este pedido já foi para a conferência.");
            }
        }

        #endregion

        #region Efetuar Conferências

        /// <summary>
        /// Associa conferência de pedidos ao conferente
        /// </summary>
        /// <param name="idConferente"></param>
        /// <param name="idsPedidos"></param>
        /// <param name="dataConferencia"></param>
        public void EfetuarConferencia(uint idConferente, string idsPedidos, DateTime dataConferencia)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuConferencia.ControleConferenciaMedicao))
                throw new Exception("Apenas funcionário Supervisor Temperado pode efetuar conferências.");

            string sql = "Update pedido_conferencia set situacao=" + (int)PedidoConferencia.SituacaoConferencia.EmAndamento +
                ", idConferente=" + idConferente + ", dataIni=?dataConf Where idPedido In (" + idsPedidos + ")";

            objPersistence.ExecuteCommand(sql, new GDAParameter[] { new GDAParameter("?dataConf", dataConferencia) });
        }

        #endregion

        #region Retificar Conferências

        /// <summary>
        /// Retificar conferências já efetuadas
        /// </summary>
        /// <param name="idsRetificar"></param>
        public void RetificarConferencia(uint idConferente, string idsPedidos, DateTime dataConferencia, string idsRetificar)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuConferencia.ControleConferenciaMedicao))
                throw new Exception("Apenas funcionário Supervisor Temperado pode retificar conferências.");

            // Volta situação das conferências retificadas para seu estado anterior
            string sql = "Update pedido_conferencia set situacao=" + (int)PedidoConferencia.SituacaoConferencia.Aberta +
                ", idConferente=NULL, dataIni=NULL Where idPedido In (" + idsRetificar + ")";

            objPersistence.ExecuteCommand(sql);

            // Efetua conferências novamente
            sql = "Update pedido_conferencia set situacao=" + (int)PedidoConferencia.SituacaoConferencia.EmAndamento +
                ", idConferente=" + idConferente + ", dataIni=?dataConf Where idPedido In (" + idsPedidos + ")";

            objPersistence.ExecuteCommand(sql, new GDAParameter[] { new GDAParameter("?dataConf", dataConferencia) });
        }

        #endregion

        #region Finalizar Conferência

        /// <summary>
        /// Finaliza a conferência, alterando a situação do pedido para conferido
        /// </summary>
        /// <param name="idPedido"></param>
        public void FinalizarConferencia(uint idPedido)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuConferencia.ControleConferenciaMedicao))
                throw new Exception("Apenas funcionário Supervisor Temperado pode finalizar conferências.");

            Pedido.SituacaoPedido situacaoPedido = PedidoDAO.Instance.ObtemSituacao(null, idPedido);
            int situacaoConf = ObtemValorCampo<int>("situacao", "idPedido=" + idPedido);

            try
            {
                // Altera situação da conferência para finalizada e marca a data de término da conferência
                objPersistence.ExecuteCommand("Update pedido_conferencia set situacao=" +
                    (int)PedidoConferencia.SituacaoConferencia.Finalizada + ", DataFim=?dataFim Where idPedido=" +
                    idPedido, new GDAParameter("?dataFim", DateTime.Now));

                // Altera a situação do pedido para conferido
                objPersistence.ExecuteCommand("Update pedido Set Situacao=" + (int)Pedido.SituacaoPedido.Conferido +
                    " Where idPedido=" + idPedido);

                // Se a situação do pedido não tiver sido alterada, lança exceção
                if (PedidoDAO.Instance.ObtemSituacao(null, idPedido) != Pedido.SituacaoPedido.Conferido)
                    throw new Exception("Não foi possível atualizar a situação do pedido para conferido.");
            }
            catch (Exception ex)
            {
                // Volta situação da conferência para seu estado anterios
                objPersistence.ExecuteCommand("Update pedido_conferencia set situacao=" + situacaoConf + ", DataFim=null Where idPedido=" + idPedido);

                // Volta situação do pedido para seu estado anterior
                objPersistence.ExecuteCommand("Update pedido Set Situacao=" + (int)situacaoPedido + " Where idPedido=" + idPedido);

                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar Conferência.", ex));
            }
        }

        #endregion

        #region Obtém dados da conferência

        public int ObtemSituacao(uint idPedido)
        {
            return ObtemValorCampo<int>("situacao", "idPedido=" + idPedido);
        }

        #endregion

        #region Verifica se pedido está em conferência

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se pedido está em conferência
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsInConferencia(uint idPedido)
        {
            return IsInConferencia(null, idPedido);
        }

        /// <summary>
        /// Verifica se pedido está em conferência
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsInConferencia(GDASession sessao, uint idPedido)
        {
            string sql = "Select Count(*) From pedido_conferencia Where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null) > 0;
        }

        #endregion
    }
}
