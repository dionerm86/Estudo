using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class RotaDAO : BaseCadastroDAO<Rota, RotaDAO>
    {
        //private RotaDAO() { }

        #region Listagem padrão

        private string Sql(uint idRota, int situacao, bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From rota Where 1 ";

            if (idRota > 0)
                sql += " And idRota=" + idRota;

            if (situacao > 0)
                sql += " And situacao=" + situacao;

            return sql;
        }

        public Rota GetElement(uint idRota)
        {
            return objPersistence.LoadOneData(Sql(idRota, 0, true));
        }

        public IList<Rota> GetList(string sortExpression, int startRow, int pageSize)
        {
            return objPersistence.LoadDataWithSortExpression(Sql(0, 0, true), new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), null).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        public IList<Rota> ObterRotas()
        {
            return objPersistence.LoadData(Sql(0, 0, true)).ToList();
        }

        #endregion

        #region Listagem para seleção de rota

        public IList<Rota> GetListSel(string sortExpression, int startRow, int pageSize)
        {
            return objPersistence.LoadDataWithSortExpression(Sql(0, 1, true), new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), null).ToList();
        }

        public int GetCountSel()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 1, false), null);
        }

        public IList<Rota> ObtemAtivas()
        {
            return objPersistence.LoadData("SELECT * FROM rota WHERE situacao = ?sit ORDER BY Descricao", new GDAParameter("?sit", Situacao.Ativo)).ToList();
        }

        public IList<Rota> ObtemAtivasPorIdCodigo(int? id, string codigo)
        {
            var sql = $"SELECT * FROM rota WHERE situacao={(int)Situacao.Ativo}";
            var parametros = new List<GDAParameter>();

            if (id > 0)
            {
                sql += $" AND IdRota={id}";
            }

            if (!string.IsNullOrEmpty(codigo))
            {
                sql += " AND CodInterno LIKE ?codInterno";
                parametros.Add(new GDAParameter("?codInterno", $"%{codigo}%"));
            }

            return this.objPersistence.LoadData(sql + " ORDER BY Descricao", parametros.Count > 0 ? parametros.ToArray() : null).ToList();
        }

        #endregion

        #region Busca pelo código interno

        public Rota GetByCodInterno(string codInterno)
        {
            string sql = "Select * From rota Where codInterno=?codInterno And situacao=" + (int)Situacao.Ativo;

            List<Rota> lstRota = objPersistence.LoadData(sql, new GDAParameter("?codInterno", codInterno));

            if (lstRota.Count != 1)
                return null;

            return lstRota[0];
        }

        #endregion

        #region Busca pelo cliente

        public Rota GetByCliente(GDASession session, uint idCliente)
        {
            if (idCliente == 0)
                return null;

            string sql = @"
                Select * From rota Where situacao=" + (int)Situacao.Ativo + @" And idRota In (
                    Select idRota From rota_cliente
                    Where idCliente=" + idCliente + @"
                )";

            List<Rota> lstRota = objPersistence.LoadData(sql).ToList();

            if (lstRota.Count != 1)
                return null;

            return lstRota[0];
        }

        #endregion

        #region Verifica se o pedido pertence à rota

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido pertence à rota
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoPertenceARota(uint idRota, uint idPedido)
        {
            return PedidoPertenceARota(null, idRota, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido pertence à rota
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoPertenceARota(GDASession sessao, uint idRota, uint idPedido)
        {
            string sql = @"
                Select Count(*) From rota_cliente
                Where idCliente In (
                    Select idCli From pedido
                    Where idPedido=" + idPedido + @"
                )
                And idRota=" + idRota + @"
                ";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null) > 0;
        }

        /// <summary>
        /// Verifica se o cliente está associado à alguma rota
        /// </summary>
        public bool ClientePossuiRota(GDASession session, uint idCliente)
        {
            string sql = @"Select Count(*) From rota_cliente Where idCliente=" + idCliente;

            return objPersistence.ExecuteSqlQueryCount(session, sql, null) > 0;
        }

        #endregion

        #region Recupera a data da rota por cliente

        /// <summary>
        /// Verifica se tem o dia informado no enum
        /// </summary>
        public bool TemODia(DayOfWeek diaProcurar, DiasSemana dias)
        {
            var retorno = false;

            switch (diaProcurar)
            {
                case DayOfWeek.Sunday:
                    if ((dias & DiasSemana.Domingo) == DiasSemana.Domingo)
                        retorno = true;
                    break;

                case DayOfWeek.Monday:
                    if ((dias & DiasSemana.Segunda) == DiasSemana.Segunda)
                        retorno = true;
                    break;

                case DayOfWeek.Tuesday:
                    if ((dias & DiasSemana.Terca) == DiasSemana.Terca)
                        retorno = true;
                    break;

                case DayOfWeek.Wednesday:
                    if ((dias & DiasSemana.Quarta) == DiasSemana.Quarta)
                        retorno = true;
                    break;

                case DayOfWeek.Thursday:
                    if ((dias & DiasSemana.Quinta) == DiasSemana.Quinta)
                        retorno = true;
                    break;

                case DayOfWeek.Friday:
                    if ((dias & DiasSemana.Sexta) == DiasSemana.Sexta)
                        retorno = true;
                    break;

                case DayOfWeek.Saturday:
                    if ((dias & DiasSemana.Sabado) == DiasSemana.Sabado)
                        retorno = true;
                    break;
            }

            return retorno;
        }

        /// <summary>
        /// Retorna se a rota é de entrega balcão.
        /// </summary>
        /// <param name="idRota">O identificador da rota</param>
        /// <returns>Retorna se a rota é entrega balcão</returns>
        public bool ObterEntregaBalcao(int idRota)
        {
            return this.ExecuteScalar<bool>($"SELECT EntregaBalcao FROM rota WHERE idRote={idRota}");
        }

        /// <summary>
        /// Recupera a data da rota por cliente.
        /// </summary>
        public DateTime? GetDataRota(uint idCli, DateTime dataInicial, Pedido.TipoPedidoEnum? tipoPedido)
        {
            return GetDataRota(null, idCli, dataInicial, tipoPedido);
        }

        /// <summary>
        /// Recupera a data da rota por cliente.
        /// </summary>
        public DateTime? GetDataRota(GDASession session, uint idCli, DateTime dataInicial, Pedido.TipoPedidoEnum? tipoPedido)
        {
            return GetDataRota(session, idCli, dataInicial, true, tipoPedido);
        }

        /// <summary>
        /// Recupera a data da rota por cliente.
        /// </summary>
        public DateTime? GetDataRota(GDASession session, uint idCli, DateTime dataInicial, bool somarDiasUteisRota, Pedido.TipoPedidoEnum? tipoPedido)
        {
            var dataConsiderarRevenda = dataInicial.AddDays(PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoRevenda);

            Rota rota = GetByCliente(session, idCli);

            if (rota == null || (rota.DiasSemana == Model.DiasSemana.Nenhum && rota.NumeroMinimoDiasEntrega == 0))
                return null;

            if (tipoPedido == Pedido.TipoPedidoEnum.Revenda)
            {
                while(!TemODia(dataConsiderarRevenda.DayOfWeek, rota.DiasSemana) || dataConsiderarRevenda.Feriado())
                    dataConsiderarRevenda = dataConsiderarRevenda.AddDays(1);

                return dataConsiderarRevenda;
            }
                

            int numeroDias = (dataInicial - DateTime.Now).Days;

            // Calcula a rota considerando dias corridos
            if (RotaConfig.UsarDiasCorridosCalculoRota)
            {
                // A comparação do número de dias percorrido com o número mínimo de dias da rota deve ser "numeroDias++ < rota.NumeroMinimoDiasEntrega",
                // porque dessa forma caso o último dia da contagem caia no dia da rota, esta data será usada, porém da forma como estava antes,
                // "numeroDias++ <= rota.NumeroMinimoDiasEntrega", a data usada será a da outra semana.
                //while (numeroDias++ < rota.NumeroMinimoDiasEntrega || dataInicial.Feriado() ||
                /* Chamado 54042. */
                while ((somarDiasUteisRota ? numeroDias++ < rota.NumeroMinimoDiasEntrega : false) || dataInicial.Feriado() ||
                     (rota.DiasSemana != Model.DiasSemana.Nenhum && !TemODia(dataInicial.DayOfWeek, rota.DiasSemana)))
                {
                    dataInicial = dataInicial.AddDays(1);
                }
            }
            // Calcula a rota considerando dias úteis
            else
            {
                /* Chamado 54042. */
                if (somarDiasUteisRota)
                    for (var i = 0; i < rota.NumeroMinimoDiasEntrega; i++)
                    {
                        dataInicial = dataInicial.AddDays(1);

                        // Enquanto não for dia útil, continua avançando a data
                        while (!FuncoesData.DiaUtil(dataInicial))
                            dataInicial = dataInicial.AddDays(1);
                    }

                // Depois de calcular os dias mínimos da rota, verifica se a data é um dia da rota
                if (rota.DiasSemana != Model.DiasSemana.Nenhum && !TemODia(dataInicial.DayOfWeek, rota.DiasSemana))
                {
                    while (!TemODia(dataInicial.DayOfWeek, rota.DiasSemana) || dataInicial.Feriado())
                        dataInicial = dataInicial.AddDays(1);
                }
            }


            return dataInicial;
        }

        #endregion

        #region Verifica se existe alguma rota cadastrada no sistema

        /// <summary>
        /// Verifica se existe alguma rota cadastrada no sistema
        /// </summary>
        /// <returns></returns>
        public bool ExisteRota()
        {
            string sql = "Select Count(*) From rota";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Obtém campos da Rota

        /// <summary>
        /// Obtém código interno.
        /// </summary>
        /// <param name="idCliente">Id do cliente.</param>
        /// <returns></returns>
        public string ObtemCodRota(uint idCliente)
        {
            return ObtemCodRota(null, idCliente);
        }

        /// <summary>
        /// Obtém código interno.
        /// </summary>
        /// <param name="idCliente">Id do cliente.</param>
        /// <returns></returns>
        public string ObtemCodRota(GDASession sessao, uint idCliente)
        {
            string codRota = "";
            if (RotaClienteDAO.Instance.IsClienteAssociado(sessao, idCliente))
                codRota = ObtemValorCampo<string>(sessao, "codInterno", "idRota In (Select idRota From rota_cliente Where idCliente=" + idCliente + ")");

            return codRota;
        }

        /// <summary>
        /// Obtém código interno das rotas.
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <returns></returns>
        public string ObtemCodRotas(string idsRotas)
        {
            if (String.IsNullOrEmpty(idsRotas))
                return String.Empty;

            var rotas = ExecuteMultipleScalar<string>("select codInterno from rota where idRota in (" + idsRotas + ")");
            return String.Join(", ", rotas.ToArray());
        }

        /// <summary>
        /// Obtém a descrição da rota pelo cliente passado
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string ObtemDescrRota(uint idCliente)
        {
            string codRota = "";
            if (RotaClienteDAO.Instance.IsClienteAssociado(idCliente))
                codRota = ObtemValorCampo<string>("descricao", "idRota In (Select idRota From rota_cliente Where idCliente=" + idCliente + ")");

            return codRota;
        }

        /// <summary>
        /// Obtém a descrição das rotas.
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <returns></returns>
        public string ObtemDescrRotas(string idsRotas)
        {
            if (String.IsNullOrEmpty(idsRotas))
                return String.Empty;

            var rotas = ExecuteMultipleScalar<string>("select descricao from rota where idRota in (" + idsRotas + ")");
            return String.Join(", ", rotas.ToArray());
        }

        public string ObterDescricaoRota(GDASession sessao, uint idRota)
        {
            return ObtemValorCampo<string>(sessao, "Descricao", "IdRota=" + idRota);
        }

        public IList<string> ObtemRotasExternas()
        {
            return ExecuteMultipleScalar<string>("SELECT DISTINCT rotaExterna FROM pedido WHERE COALESCE(rotaExterna, '') <> ''");
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Rota objInsert)
        {
            // Verifica se já foi inserida uma rota com o código interno passado
            /*if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From rota Where codInterno=?codInterno",
                new GDAParameter("?codInterno", objInsert.CodInterno)) > 0)
                throw new Exception("O código informado já está sendo usado em outra rota.");

            return base.Insert(objInsert);*/
            throw new NotSupportedException();
        }

        public override int Update(Rota objUpdate)
        {
            /*
            // Verifica se já foi inserida uma rota com o código interno passado
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From rota Where codInterno=?codInterno And idRota<>" +
                objUpdate.IdRota, new GDAParameter("?codInterno", objUpdate.CodInterno)) > 0)
                throw new Exception("O código informado já está sendo usado em outra rota.");

            LogAlteracaoDAO.Instance.LogRota(objUpdate);
            return base.Update(objUpdate);*/
            throw new NotSupportedException();
        }

        public override int Delete(Rota objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdRota);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            /*if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From rota_cliente Where idRota=" + Key) > 0)
                throw new Exception("Esta rota não pode ser excluída pois existem clientes relacionados à mesma.");

            LogAlteracaoDAO.Instance.ApagaLogRota(Key);
            return base.DeleteByPrimaryKey(Key);*/
            throw new NotSupportedException();
        }

        public IList<Rota> GetRptRota()
        {
            return GetRptRota(null);
        }

        public IList<Rota> GetRptRota(GDASession session)
        {
            return objPersistence.LoadData(session, "Select * from rota").ToList();
        }

        #endregion
    }
}
