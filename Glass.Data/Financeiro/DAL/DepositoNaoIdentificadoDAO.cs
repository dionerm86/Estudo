using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class DepositoNaoIdentificadoDAO : BaseDAO<DepositoNaoIdentificado, DepositoNaoIdentificadoDAO>
    {
        //private DepositoNaoIdentificadoDAO() { }

        #region Recuperação de itens

        public string Sql(uint idDepositoNaoIdentificado, uint idContaBanco, decimal valorIni, decimal valorFim,
            string dataCadIni, string dataCadFim, string dataMovIni, string dataMovFim, int situacao, bool selecionar)
        {
            string campos = "dni.*, cb.nome as descrContaBanco, '$$$' as Criterio";
            string criterio = "";

            string sql = @"SELECT " + (selecionar ? campos : "COUNT(*)") + @"
                           FROM deposito_nao_identificado dni
                           INNER JOIN conta_banco cb ON (dni.idContaBanco = cb.idContaBanco)
                           WHERE 1";

            if (idDepositoNaoIdentificado > 0)
            {
                sql += " AND dni.idDepositoNaoIdentificado=" + idDepositoNaoIdentificado;
                criterio += "Deposito não identificado: " + idDepositoNaoIdentificado + "     ";
            }

            if (idContaBanco > 0)
            {
                sql += " AND cb.IdContaBanco=" + idContaBanco;
                criterio += "Conta bancária: " + ContaBancoDAO.Instance.GetDescricao(idContaBanco) + "  ";
            }

            if (valorIni > 0)
            {
                sql += " AND dni.valorMov>=?valorMovIni";
                criterio += "Valor mov. inicial: " + valorIni.ToString("C") + "  ";
            }

            if (valorFim > 0)
            {
                sql += " AND dni.valorMov<=?valorMovFim";
                criterio += "Valor mov. final: " + valorFim.ToString("C") + "  ";
            }

            if (!string.IsNullOrEmpty(dataMovIni))
            {
                sql += " AND dni.dataMov>=?dtMovIni";
                criterio += "Data mov. inicial: " + dataMovIni + "  ";
            }

            if (!string.IsNullOrEmpty(dataMovFim))
            {
                sql += " AND dni.dataMov<=?dtMovFim";
                criterio += "Data mov. final: " + dataMovFim + "  ";
            }

            if (!string.IsNullOrEmpty(dataCadIni))
            {
                sql += " AND dni.dataCad>=?dtCadIni";
                criterio += "Data cad. inicial: " + dataCadIni + "  ";
            }

            if (!string.IsNullOrEmpty(dataCadFim))
            {
                sql += " AND dni.dataCad<=?dtCadFim";
                criterio += "Data cad. final: " + dataCadFim + "  ";
            }

            if (situacao > 0)
            {
                sql += " AND dni.Situacao=" + situacao;
                var dni = new DepositoNaoIdentificado()
                {
                    Situacao = (DepositoNaoIdentificado.SituacaoEnum)situacao
                };

                criterio += "Situacao: " + dni.DescrSituacao + "  ";
            }

            sql += " order by idDepositoNaoIdentificado desc";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParam(string dtCadIni, string dtCadFim, string dtMovIni, string dtMovFim, decimal valorIni, decimal valorFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtCadIni))
                lstParam.Add(new GDAParameter("?dtCadIni", DateTime.Parse(dtCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtCadFim))
                lstParam.Add(new GDAParameter("?dtCadFim", DateTime.Parse(dtCadFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtMovIni))
                lstParam.Add(new GDAParameter("?dtMovIni", DateTime.Parse(dtMovIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtMovFim))
                lstParam.Add(new GDAParameter("?dtMovFim", DateTime.Parse(dtMovFim + " 23:59")));

            if (valorIni > 0)
                lstParam.Add(new GDAParameter("?valorMovIni", valorIni));

            if (valorFim > 0)
                lstParam.Add(new GDAParameter("?valorMovFim", valorFim));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o elemento pelo identificador passado
        /// </summary>
        public DepositoNaoIdentificado GetElement(uint idDepositoNaoIdentificado)
        {
            return GetElement(null, idDepositoNaoIdentificado);
        }

        /// <summary>
        /// Recupera o elemento pelo identificador passado
        /// </summary>
        /// <param name="idDepositoNaoIdentificado"></param>
        /// <returns></returns>
        public DepositoNaoIdentificado GetElement(GDASession sessao, uint idDepositoNaoIdentificado)
        {
            return objPersistence.LoadOneData(sessao, Sql(idDepositoNaoIdentificado, 0, 0, 0, null, null, null, null, 0, true));
        }

        /// <summary>
        /// Recupera lista de depósitos nao identificados.
        /// </summary>
        /// <param name="idDepositoNaoIdentificado"></param>
        /// <param name="idConta"></param>
        /// <param name="valorIni"></param>
        /// <param name="valorFim"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="dataMovIni"></param>
        /// <param name="dataMovFim"></param>
        /// <param name="situacao"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<DepositoNaoIdentificado> GetList(uint idDepositoNaoIdentificado, uint idContaBanco, decimal valorIni, decimal valorFim,
            string dataCadIni, string dataCadFim, string dataMovIni, string dataMovFim, int situacao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idDepositoNaoIdentificado, idContaBanco, valorIni, valorFim,
                dataCadIni, dataCadFim, dataMovIni, dataMovFim, situacao, true), sortExpression, startRow, pageSize,
                GetParam(dataCadIni, dataCadFim, dataMovIni, dataMovFim, valorIni, valorFim));
        }

        /// <summary>
        /// Contagem de registro da lista de depósitos não identificados.
        /// </summary>
        /// <param name="idDepositoNaoIdentificado"></param>
        /// <param name="idConta"></param>
        /// <param name="valorIni"></param>
        /// <param name="valorFim"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="dataMovIni"></param>
        /// <param name="dataMovFim"></param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public int GetListCount(uint idDepositoNaoIdentificado, uint idContaBanco, decimal valorIni, decimal valorFim,
            string dataCadIni, string dataCadFim, string dataMovIni, string dataMovFim, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idDepositoNaoIdentificado, idContaBanco, valorIni, valorFim,
                dataCadIni, dataCadFim, dataMovIni, dataMovFim, situacao, false),
                GetParam(dataCadIni, dataCadFim, dataMovIni, dataMovFim, valorIni, valorFim));
        }

        /// <summary>
        /// Recupera lista de depósitos nao identificados para relatório.
        /// </summary>
        /// <param name="idDepositoNaoIdentificado"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="valorIni"></param>
        /// <param name="valorFim"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="dataMovIni"></param>
        /// <param name="dataMovFim"></param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public DepositoNaoIdentificado[] GetListRpt(uint idDepositoNaoIdentificado, uint idContaBanco, decimal valorIni, decimal valorFim,
            string dataCadIni, string dataCadFim, string dataMovIni, string dataMovFim, int situacao)
        {
            return objPersistence.LoadData(Sql(idDepositoNaoIdentificado, idContaBanco, valorIni, valorFim,
                dataCadIni, dataCadFim, dataMovIni, dataMovFim, situacao, true),
                GetParam(dataCadIni, dataCadFim, dataMovIni, dataMovFim, valorIni, valorFim)).ToArray();
        }

        /// <summary>
        /// Recupera os ids dos depositos nao identificados para desvincular
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idAcerto"></param>
        /// <param name="idContaR"></param>
        /// <param name="idObra"></param>
        /// <param name="idSinal"></param>
        /// <param name="idTrocaDevolucao"></param>
        /// <param name="idDevolucaoPagamento"></param>
        /// <param name="idAcertoCheque"></param>
        /// <returns></returns>
        public string GetIdsForDesvincular(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint? idAcerto,
            uint? idContaR, uint? idObra, uint? idSinal, uint? idTrocaDevolucao, uint? idDevolucaoPagamento, uint? idAcertoCheque)
        {
            string filtro = "";

            string sql = @"SELECT group_concat(dni.idDepositoNaoIdentificado)
                           FROM deposito_nao_identificado dni";

            if (idPedido.HasValue && idPedido.Value > 0)
                filtro += " dni.idPedido=" + idPedido.Value + " OR";

            if (idLiberarPedido.HasValue && idLiberarPedido.Value > 0)
                filtro += " dni.idLiberarPedido=" + idLiberarPedido.Value + " OR";

            if (idAcerto.HasValue && idAcerto.Value > 0)
                filtro += " dni.idAcerto=" + idAcerto.Value + " OR";

            if (idContaR.HasValue && idContaR.Value > 0)
                filtro += " dni.idContaR=" + idContaR.Value + " OR";

            if (idObra.HasValue && idObra.Value > 0)
                filtro += " dni.idObra=" + idObra.Value + " OR";

            if (idSinal.HasValue && idSinal.Value > 0)
                filtro += " dni.idSinal=" + idSinal.Value + " OR";

            if (idTrocaDevolucao.HasValue && idTrocaDevolucao.Value > 0)
                filtro += " dni.idTrocaDevolucao=" + idTrocaDevolucao.Value + " OR";

            if (idDevolucaoPagamento.HasValue && idDevolucaoPagamento.Value > 0)
                filtro += " dni.idDevolucaoPagto=" + idDevolucaoPagamento.Value + " OR";

            if (idAcertoCheque.HasValue && idAcertoCheque.Value > 0)
                filtro += " dni.idAcertoCheque=" + idAcertoCheque.Value + " OR";

            if (string.IsNullOrEmpty(filtro))
                throw new Exception("Nenhuma conta para desvincular informada.");

            sql += " WHERE " + filtro.TrimEnd('R').TrimEnd('O');

            return ExecuteScalar<string>(sessao, sql);
        }

        #endregion

        #region Não utilizados

        /// <summary>
        /// Recupera os depósitos não identificados que ainda não foram utilizados.
        /// </summary>
        /// <returns></returns>
        public DepositoNaoIdentificado[] GetNaoUtilizados()
        {
            string sql = @"select dni.*, c.nome as descrContaBanco
                from deposito_nao_identificado dni
                    inner join conta_banco c on (dni.idContaBanco=c.idContaBanco)
                where dni.situacao=" + (int)DepositoNaoIdentificado.SituacaoEnum.Ativo;

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Verifica se a depósitos não identificados não utulizados.
        /// </summary>
        /// <returns></returns>
        public bool PossuiNaoUtilizados()
        {
            string sql = @"select count(*) from deposito_nao_identificado
                where situacao=" + (int)DepositoNaoIdentificado.SituacaoEnum.Ativo;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Obtem dados deposito

        public DepositoNaoIdentificado.SituacaoEnum ObtemSituacao(uint idDeposito)
        {
            return ObtemSituacao(null, idDeposito);
        }

        public DepositoNaoIdentificado.SituacaoEnum ObtemSituacao(GDASession session, uint idDeposito)
        {
            return (DepositoNaoIdentificado.SituacaoEnum)ObtemValorCampo<int>(session, "situacao", "idDepositoNaoIdentificado=" + idDeposito);
        }
 
        public int ObtemIdContaBanco(GDASession session, int idDeposito)
        {
            return ObtemValorCampo<int>(session, "IdContaBanco", "IdDepositoNaoIdentificado=" + idDeposito);
        }

        #endregion

        #region Vinculo do depósito não identificado

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public void VinculaDepositoNaoIdentificado(uint idDepositoNaoIdentificado, Pedido pedido, LiberarPedido liberarPedido, Acerto acerto,
            ContasReceber contaR, Obra obra, Sinal sinal, TrocaDevolucao trocaDevolucao, uint? idDevolucaoPagto, uint? idAcertoCheque)
        {
            VinculaDepositoNaoIdentificado(null, idDepositoNaoIdentificado, pedido, liberarPedido, acerto,
                contaR, obra, sinal, trocaDevolucao, idDevolucaoPagto, idAcertoCheque);
        }

        /// <summary>
        /// Faz o vinculo do deposito.
        /// </summary>
        public void VinculaDepositoNaoIdentificado(GDASession sessao, uint idDepositoNaoIdentificado, Pedido pedido, LiberarPedido liberarPedido, Acerto acerto,
            ContasReceber contaR, Obra obra, Sinal sinal, TrocaDevolucao trocaDevolucao, uint? idDevolucaoPagto, uint? idAcertoCheque)
        {
            uint? idPedido = pedido != null && pedido.IdPedido > 0 ? pedido.IdPedido : (uint?)null;
            uint? idLiberarPedido = liberarPedido != null ? liberarPedido.IdLiberarPedido : (uint?)null;
            uint? idAcerto = acerto != null ? acerto.IdAcerto : (uint?)null;
            uint? idContaR = contaR != null ? contaR.IdContaR : (uint?)null;
            uint? idObra = obra != null ? obra.IdObra : (uint?)null;
            uint? idSinal = sinal != null ? sinal.IdSinal : (uint?)null;
            uint? idTrocaDevolucao = trocaDevolucao != null ? trocaDevolucao.IdTrocaDevolucao : (uint?)null;

            if (idDepositoNaoIdentificado == 0)
                throw new Exception("Depósito não informado.");

            if(!idPedido.HasValue && !idLiberarPedido.HasValue && !idAcerto.HasValue && !idContaR.HasValue && !idObra.HasValue && !idSinal.HasValue
                && !idTrocaDevolucao.HasValue && !idDevolucaoPagto.HasValue && !idAcertoCheque.HasValue)
                throw new Exception("Nenhuma conta para vínculo informada.");

            DepositoNaoIdentificado dni = GetElement(sessao, idDepositoNaoIdentificado);

            if (dni == null)
                throw new Exception("Depósito não encontrado.");

            /* Chamado 56553. */
            if (dni.Situacao != DepositoNaoIdentificado.SituacaoEnum.Ativo)
                throw new Exception("Para que o DNI seja vinculado a um recebimento, ele deve estar na situação Ativo.");

            dni.IdPedido = idPedido;
            dni.IdLiberarPedido = idLiberarPedido;
            dni.IdAcerto = idAcerto;
            dni.IdContaR = idContaR;
            dni.IdObra = idObra;
            dni.IdSinal = idSinal;
            dni.IdTrocaDevolucao = idTrocaDevolucao;
            dni.IdDevolucaoPagto = idDevolucaoPagto;
            dni.IdAcertoCheque = idAcertoCheque;
            dni.Situacao = !idDevolucaoPagto.HasValue ? DepositoNaoIdentificado.SituacaoEnum.EmUso : DepositoNaoIdentificado.SituacaoEnum.Ativo;

            Update(sessao, dni);

            uint? idCliente = null;

            // Atualiza o cliente do DNI
            if (idPedido > 0)
                idCliente = pedido.IdCli;
            else if (idLiberarPedido > 0)
                idCliente = liberarPedido.IdCliente;
            else if (idAcerto > 0)
                idCliente = acerto.IdCli;
            else if (idContaR > 0)
                idCliente = contaR.IdCliente;
            else if (idObra > 0)
                idCliente = obra.IdCliente;
            else if (idSinal > 0)
                idCliente = sinal.IdCliente;
            else if (idTrocaDevolucao > 0)
                idCliente = trocaDevolucao.IdCliente;

            if (idCliente > 0)
                objPersistence.ExecuteCommand(sessao, "Update mov_banco Set idCliente=" + idCliente + " Where idDepositoNaoIdentificado=" + dni.IdDepositoNaoIdentificado);
        }

        public void DesvinculaDepositoNaoIdentificado(GDASession sessao, Pedido pedido, LiberarPedido liberarPedido, Acerto acerto,
            ContasReceber contaR, Obra obra, Sinal sinal, TrocaDevolucao trocaDevolucao, DevolucaoPagto devolucaoPagamento,
            uint acertoCheque)
        {
            uint? idPedido = pedido != null ? pedido.IdPedido : (uint?)null;
            uint? idLiberarPedido = liberarPedido != null ? liberarPedido.IdLiberarPedido : (uint?)null;
            uint? idAcerto = acerto != null ? acerto.IdAcerto : (uint?)null;
            uint? idContaR = contaR != null ? contaR.IdContaR : (uint?)null;
            uint? idObra = obra != null ? obra.IdObra : (uint?)null;
            uint? idSinal = sinal != null ? sinal.IdSinal : (uint?)null;
            uint? idTrocaDevolucao = trocaDevolucao != null ? trocaDevolucao.IdTrocaDevolucao : (uint?)null;
            uint? idDevolucaoPagto = devolucaoPagamento != null ? devolucaoPagamento.IdDevolucaoPagto : (uint?)null;
            uint? idAcertoCheque = acertoCheque > 0 ? acertoCheque : (uint?)null;

            if (!idPedido.HasValue && !idLiberarPedido.HasValue && !idAcerto.HasValue && !idContaR.HasValue && !idObra.HasValue && !idSinal.HasValue
               && !idTrocaDevolucao.HasValue && !idDevolucaoPagto.HasValue && !idAcertoCheque.HasValue)
                throw new Exception("Nenhuma conta para desvincular informada.");

            string depositos = GetIdsForDesvincular(sessao, idPedido, idLiberarPedido, idAcerto, idContaR, idObra, idSinal, idTrocaDevolucao,
                idDevolucaoPagto, idAcertoCheque);

            if (string.IsNullOrEmpty(depositos))
                return;

            string sql = @"UPDATE deposito_nao_identificado
                           SET idPedido=null, idLiberarPedido=null, idAcerto=null, idContaR=null, idObra=null, idSinal=null,
                               idTrocaDevolucao=null, idDevolucaoPagto=null, idAcertoCheque=null, situacao=" + (int)DepositoNaoIdentificado.SituacaoEnum.Ativo + @"
                           WHERE idDepositoNaoIdentificado in (" + depositos + ")";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Cancela depósito não identificado
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cancela um déposito não identificado
        /// </summary>
        public void Cancelar(uint idDepositoNaoIdentificado, string motivo)
        {
            FilaOperacoes.CancelarDepositoNaoIdentificado.AguardarVez();

            using (var transaction = new GDATransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    transaction.BeginTransaction();

                    Cancelar(transaction, idDepositoNaoIdentificado, motivo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.CancelarDepositoNaoIdentificado.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Cancela um déposito não identificado
        /// </summary>
        public void Cancelar(GDASession sessao, uint idDepositoNaoIdentificado, string motivo)
        {
            //Vilida para o cancelamento
            if (idDepositoNaoIdentificado == 0)
                throw new Exception("Informe o depósito não identificado a ser cancelado.");

            if (string.IsNullOrEmpty(motivo))
                throw new Exception("Informe o motivo do cancelamento.");

            if (ObtemSituacao(sessao, idDepositoNaoIdentificado)
                != DepositoNaoIdentificado.SituacaoEnum.Ativo)
                throw new Exception("O depósito não identificado deve estar ativo para ser cancelado.");

            DepositoNaoIdentificado dni = GetElement(sessao, idDepositoNaoIdentificado);
            if (dni == null)
                throw new Exception("Depósito não encontrado.");

            var idMovBanco = MovBancoDAO.Instance.GetByDepositoNaoIdentificado(sessao, idDepositoNaoIdentificado)
                .Select(c => c.IdMovBanco);

            // Apaga as mov bancárias (o saldo é corrigido na sobrecarga do método abaixo)
            foreach (uint id in idMovBanco)
                MovBancoDAO.Instance.DeleteByPrimaryKey(sessao, id);

            string sql = @"
                UPDATE deposito_nao_identificado
                SET situacao=" + (int)DepositoNaoIdentificado.SituacaoEnum.Cancelado + @"
                WHERE idDepositoNaoIdentificado=" + idDepositoNaoIdentificado;

            objPersistence.ExecuteCommand(sessao, sql);

            // Gera log cancelamento
            LogCancelamentoDAO.Instance.LogDepostioNaoIdentificado(sessao, dni, motivo, true);
        }

        #endregion

        #region Metodos sobrescritos

        public override uint Insert(DepositoNaoIdentificado objInsert)
        {
            FilaOperacoes.InserirDepositoNaoIdentificado.AguardarVez();

            using (var transaction = new GDATransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("Insert - DepositoNaoIdentificadoDAO", ex);
                    throw;
                }
                finally
                {
                    FilaOperacoes.InserirDepositoNaoIdentificado.ProximoFila();
                }
            }
        }

        public override uint Insert(GDASession session, DepositoNaoIdentificado objInsert)
        {
            objInsert.DataCad = DateTime.Now;
            objInsert.IdFuncCad = UserInfo.GetUserInfo.CodUser;
            objInsert.Situacao = DepositoNaoIdentificado.SituacaoEnum.Ativo;

            objInsert.IdDepositoNaoIdentificado = base.Insert(session, objInsert);

            //Gera a movimentação bancaria
            var idMovBanco = ContaBancoDAO.Instance.MovContaDepositoNaoIdentificado(session, objInsert.IdContaBanco,
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoNaoIdentificado), (int)UserInfo.GetUserInfo.IdLoja,
                objInsert.IdDepositoNaoIdentificado, 1, objInsert.ValorMov, objInsert.DataMov);

            if (idMovBanco == 0)
                throw new Exception("Não foi possível movimentar a conta bancária.");

            return objInsert.IdDepositoNaoIdentificado;
        }
        
        public override int Update(DepositoNaoIdentificado objUpdate)
        {
            FilaOperacoes.AtualizarDepositoNaoIdentificado.AguardarVez();

            using (var transaction = new GDATransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

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
                finally
                {
                    FilaOperacoes.AtualizarDepositoNaoIdentificado.ProximoFila();
                }
            }
        }

        public override int Update(GDASession session, DepositoNaoIdentificado objUpdate)
        {
            if (ObtemSituacao(session, objUpdate.IdDepositoNaoIdentificado) != DepositoNaoIdentificado.SituacaoEnum.Ativo)
                throw new Exception("Não é possível editar este DNI, pois, ele não está ativo.");

            DepositoNaoIdentificado objOriginal = GetElement(session, objUpdate.IdDepositoNaoIdentificado);

            bool alterouValorData = objOriginal.ValorMov != objUpdate.ValorMov || objOriginal.DataMov != objUpdate.DataMov;

            int retorno = base.Update(session, objUpdate);

            if (alterouValorData)
            {
                string idsMovs = string.Join(",", MovBancoDAO.Instance.GetByDepositoNaoIdentificado(session, objUpdate.IdDepositoNaoIdentificado)
                    .Select(c => c.IdMovBanco.ToString()).ToArray());
                
                //Gera a movimentação bancaria
                ContaBancoDAO.Instance.MovContaDepositoNaoIdentificado(session, objUpdate.IdContaBanco,
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoNaoIdentificado), (int)UserInfo.GetUserInfo.IdLoja,
                    objUpdate.IdDepositoNaoIdentificado, 1, objUpdate.ValorMov, objUpdate.DataMov);

                MovBancoDAO.Instance.DeleteByPKs(session, idsMovs, "Atualização");
            }

            LogAlteracaoDAO.Instance.LogDepositoNaoIdentificado(session, objOriginal);

            return retorno;
        }

        #endregion
    }
}
