using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class MedicaoDAO : BaseCadastroDAO<Medicao, MedicaoDAO>
    {
        //private MedicaoDAO() { }

        #region Vari�veis locais

        private static readonly object _medicaoDefinitivaLock = new object();

        private static readonly object _medicaoLock = new object();

        #endregion

        #region Busca medi��es

        private string Sql(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja, bool selecionar)
        {
            string campos = selecionar ? "m.*, l.NomeFantasia as NomeLoja, f.Nome as NomeMedidor, vend.Nome as NomeVendedor, '$*[' as criterio" : "Count(*)";
            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From medicao m 
                    Left Join funcionario f On (m.idFuncMed=f.idfunc) 
                    Left Join funcionario vend On (m.idFunc=vend.idfunc) 
                    Left Join loja l On (m.idLoja=l.idLoja) 
                Where 1 ";

            if (idMedicao > 0)
            {
                sql += " And idMedicao=" + idMedicao;
                criterio += "Num Medicao: " + idMedicao + "    ";
            }

            if (idOrcamento > 0)
            {
                sql += string.Format(@" AND (m.IdOrcamento={0} OR m.IdMedicao IN (SELECT IdMedicao FROM orcamento WHERE IdOrcamento={0}) OR
                    m.IdMedicao IN (SELECT IdMedicaoDefinitiva FROM orcamento WHERE IdOrcamento={0}))", idOrcamento);
                criterio += "Num. Or�amento: " + idOrcamento + "    ";
            }

            if (idPedido > 0)
            {
                sql += string.Format(" AND (m.IdPedido={0} OR m.IdMedicao IN " +
                    "(SELECT IdMedicao FROM orcamento WHERE IdOrcamento IN " +
                    "(SELECT IdOrcamento FROM pedido WHERE IdPedido={0})))", idPedido);
                criterio += string.Format("Num. Pedido: {0}    ", idPedido);
            }

            if (idVendedor > 0)
            {
                sql += "And vend.idFunc=" + idVendedor;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idVendedor) + "    ";
            }

            if (situacao > 0)
            {
                sql += " And m.situacao=" + (uint)situacao;
                criterio += "Situa��o: " + GetSituacao(situacao) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And DataMedicao>=?dataIni";
                criterio += "Data In�cio: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And DataMedicao<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataEfetuar))
                sql += " And DataEfetuar>=?dataEfetuar And DataEfetuar<=?dataEfetuar1 ";

            if (!String.IsNullOrEmpty(nomeCli))
            {
                sql += " And m.NomeCliente Like ?nomeCli";
                criterio += "Cliente: " + nomeCli + "    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                sql += " And m.Bairro Like ?bairro";
                criterio += "Bairro: " + bairro + "    ";
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                sql += " And m.Endereco Like ?endereco";
                criterio += "Endere�o: " + endereco + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                sql += " And (telCliente like ?telefone Or celCliente like ?telefone)";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And m.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idMedidor > 0)
                sql += " And m.IdFuncMed=" + idMedidor;
            else if (!String.IsNullOrEmpty(nomeMedidor))
                sql += " And f.Nome Like ?nomeMedidor";

            if (!String.IsNullOrEmpty(nomeMedidor))
                criterio += "Medidor: " + nomeMedidor + "    ";

            return sql.Replace("$*[", criterio);
        }

        public IList<Medicao> GetListForRpt(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor,
            uint idVendedor, uint situacao, string dataIni, string dataFim, string dataEfetuar, string nomeCli, string bairro,
            string endereco, string telefone, uint idLoja)
        {
            var orderBy = MedicaoConfig.RelatorioListaMedicoes.OrdenarPeloIdMedicao ? " Order by idMedicao desc" : " Order By NumSeq";

            return objPersistence.LoadData(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao, dataIni,
                dataFim, dataEfetuar, nomeCli, bairro, endereco, telefone, idLoja, true) + orderBy, GetParams(dataIni, dataFim,
                dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone)).ToList();
        }

        public IList<Medicao> GetList(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            var orderBy = !String.IsNullOrEmpty(sortExpression) ? sortExpression :
                MedicaoConfig.RelatorioListaMedicoes.OrdenarPeloIdMedicao ? "idMedicao desc" : "DataMedicao desc, idMedicao desc";

            return LoadDataWithSortExpression(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao, dataIni,
                dataFim, dataEfetuar, nomeCli, bairro, endereco, telefone, idLoja, true), orderBy, startRow, pageSize,
                GetParams(dataIni, dataFim, dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone));
        }

        public int GetCount(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao,
                dataIni, dataFim, dataEfetuar, nomeCli, bairro, endereco, telefone, idLoja, false),
                GetParams(dataIni, dataFim, dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone));
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string dataEfetuar, string nomeCli, string nomeMedidor, string bairro, 
            string endereco, string telefone)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));

            if (!String.IsNullOrEmpty(dataEfetuar))
            {
                lstParam.Add(new GDAParameter("?dataEfetuar", DateTime.Parse(dataEfetuar + " 00:00")));
                lstParam.Add(new GDAParameter("?dataEfetuar1", DateTime.Parse(dataEfetuar + " 23:59")));
            }

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(nomeMedidor))
                lstParam.Add(new GDAParameter("?nomeMedidor", "%" + nomeMedidor + "%"));

            if (!String.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!String.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!String.IsNullOrEmpty(telefone))
                lstParam.Add(new GDAParameter("?telefone", "%" + telefone + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca para relat�rio individual

        /// <summary>
        /// Busca medi��o para impress�o de relat�rio padr�o �nico
        /// </summary>
        /// <param name="idMedicao"></param>
        /// <returns></returns>
        public Medicao GetForRptUnico(uint idMedicao)
        {
            string sql = "Select m.*, l.NomeFantasia as NomeLoja, if(l.idCidade is null, l.cidade, cid.NomeCidade) as cidadeLoja, " + 
                "l.endereco as enderecoLoja, l.numero as numeroLoja, l.Bairro as bairroLoja, l.Telefone as telLoja, " + 
                "f.Nome as NomeMedidor, vend.Nome as NomeVendedor From medicao m " +
                "Left Join funcionario f On (m.idFuncMed=f.idfunc) " +
                "Left Join funcionario vend On (m.idFunc=vend.idfunc) " +
                "Left Join loja l On (m.idLoja=l.idLoja) " + 
                "Left Join cidade cid On (cid.idCidade=l.idCidade) Where idMedicao=" + idMedicao;

            return objPersistence.LoadOneData(sql);
        }

        #endregion

        #region Finaliza Medi��o

        /// <summary>
        /// Finaliza a medi��o passada e gera um or�amento para a mesma, retornando o id deste
        /// </summary>
        public uint FinalizarMedicao(uint idMedicao)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var login = UserInfo.GetUserInfo;

                    if (!Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao))
                        throw new Exception("Apenas o Aux. Escrit�rio Medi��o pode finalizar medi��es.");

                    var medicao = GetMedicao(transaction, idMedicao);

                    if (medicao.Situacao == (int)Medicao.SituacaoMedicao.Finalizada)
                        throw new Exception("Medi��o j� finalizada.");

                    medicao.Situacao = (int)Medicao.SituacaoMedicao.Finalizada;
                    medicao.IdFuncConf = login.CodUser;
                    medicao.DataConf = DateTime.Now;

                    Update(transaction, medicao);
                    
                    uint idOrca = 0;

                    /* Chamado 50817. */
                    // Caso a medi��o esteja associada a um pedido ou a um or�amento, n�o gera o or�amento ou
                    // retorna o ID do or�amento com medi��o definitiva.
                    if (medicao.IdPedido.GetValueOrDefault() == 0 && medicao.IdOrcamento.GetValueOrDefault() == 0)
                    {
                        // Caso esta medi��o n�o seja uma medi��o definitiva de or�amento, gera um novo or�amento.
                        if (!medicao.IsMedicaoDefinitivaOrcamento)
                        {
                            Orcamento orca = OrcamentoDAO.Instance.GetByMedicao(transaction, idMedicao);
                            orca.Bairro = medicao.Bairro;
                            orca.Cidade = medicao.Cidade;
                            orca.Endereco = medicao.Endereco;
                            orca.IdFuncionario = medicao.IdFunc == 0 ? null : medicao.IdFunc;
                            orca.IdMedicao = medicao.IdMedicao;
                            orca.NomeCliente = medicao.NomeCliente;
                            orca.TelCliente = medicao.TelCliente;
                            orca.CelCliente = medicao.CelCliente;
                            orca.Email = medicao.EmailCliente;
                            orca.TipoEntrega = (int?)Orcamento.TipoEntregaOrcamento.Temperado;
                            orca.Validade = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;
                            orca.PrazoEntrega = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;

                            orca.FormaPagto = !string.IsNullOrEmpty(medicao.FormaPagto) ? medicao.FormaPagto :
                                OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;

                            orca.Situacao = (int)Orcamento.SituacaoOrcamento.EmAberto;

                            idOrca = orca.IdOrcamento;

                            // Se o or�amento j� existir, atualiza, sen�o insere.
                            if (orca.IdOrcamento > 0)
                                OrcamentoDAO.Instance.Update(transaction, orca);
                            else
                                idOrca = OrcamentoDAO.Instance.Insert(transaction, orca);
                        }
                        // Recupera o ID do or�amento que tem como medi��o definitiva a medi��o que est� sendo finalizada.
                        else
                            idOrca = ((uint?)OrcamentoDAO.Instance.ObterIdOrcamentoPelaMedicaoDefinitiva(transaction, (int)medicao.IdMedicao)).GetValueOrDefault();
                    }
                    // Caso a medi��o tenha sido marcada como medi��o definitiva e o or�amento tenha sido informado no cadastro dela,
                    // verifica se o or�amento n�o possui medi��o definitiva e, se n�o possuir, altera o or�amento para ser associado � essa medi��o como definitiva.
                    else if (medicao.MedicaoDefinitiva && medicao.IdOrcamento > 0)
                    {
                        idOrca = ((uint?)OrcamentoDAO.Instance.ObterIdOrcamentoPelaMedicaoDefinitiva(transaction, (int)medicao.IdMedicao)).GetValueOrDefault();
                        
                        if (idOrca == 0)
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET IdMedicaoDefinitiva={0} WHERE IdOrcamento={1}",
                                medicao.IdMedicao, medicao.IdOrcamento));
                        // Caso a medi��o definitiva do or�amento n�o seja a mesma medi��o que est� sendo finalizada,
                        // desmarca ela como medi��o definitiva.
                        else if (idOrca != medicao.IdOrcamento)
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE medicao SET MedicaoDefinitiva=0 WHERE IdMedicao={0}", medicao.IdMedicao));
                    }

                    transaction.Commit();
                    transaction.Close();

                    return idOrca;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw ex;
                }
            }
        }

        #endregion

        #region Cancela Medi��o

        /// <summary>
        /// Cancela uma medi��o.
        /// </summary>
        /// <param name="idMedicao"></param>
        public void CancelarMedicao(uint idMedicao, string obs)
        {
            try
            {
                objPersistence.ExecuteCommand("update medicao set obsMedicao=?obs, situacao=" + (int)Medicao.SituacaoMedicao.Cancelada +
                    " where idMedicao=" + idMedicao, new GDAParameter("?obs", obs));
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar medi��o.", ex));
            }
        }

        #endregion

        #region Efetua medi��es

        /// <summary>
        /// Atribui medicoes passadas ao medidor passado
        /// </summary>
        /// <param name="idMedidor"></param>
        /// <param name="idMedicoes"></param>
        /// <param name="dataEfetuar"></param>
        public void SetMedicoesForMedidor(uint idMedidor, string idMedicoes, DateTime dataEfetuar)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao))
                throw new Exception("Apenas Aux. Escrit�rio Medi��o pode efetuar medi��es.");

            string[] idsMed = idMedicoes.Split(',');
            string sqlExec = String.Empty;
            string sql = "Update medicao Set dataEfetuar=?dataEfetuar, numSeq=$numSeq, situacao=" + (int)Medicao.SituacaoMedicao.EmAndamento + 
                ", idFuncMed=" + idMedidor + " Where idMedicao=$idMed";

            GDAParameter[] param = new GDAParameter[] { new GDAParameter("?dataEfetuar", dataEfetuar) };

            // Para cada medi��o
            for (int i = 0; i < idsMed.Length; i++)
            {
                sqlExec = sql.Replace("$numSeq", (i + 1).ToString()).Replace("$idMed", idsMed[i].ToString());
                objPersistence.ExecuteCommand(sqlExec, param);
            }
        }

        #endregion

        #region Situa��o da medi��o

        /// <summary>
        /// Retorna a descri��o da situa��o da medi��o baseado no c�digo passado
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        private string GetSituacao(uint situacao)
        {
            switch (situacao)
            {
                case 1:
                    return "Aberta";
                case 2:
                    return "Em andamento";
                case 3:
                    return "Finalizada";
                case 4:
                    return "Remarcada";
                case 5:
                    return "Cancelada";
                default:
                    return "";
            }
        }

        #endregion

        #region Busca medi��o se existir

        public Medicao GetMedicao(uint idMedicao)
        {
            return GetMedicao(null, idMedicao);
        }

        public Medicao GetMedicao(GDASession session, uint idMedicao)
        {
            string sql = @"Select m.*, (select count(*) from orcamento where idMedicaoDefinitiva=m.idMedicao) > 0 as isMedicaoDefinitivaOrcamento
                From medicao m where idMedicao=" + idMedicao;

            List<Medicao> lst = objPersistence.LoadData(session, sql);

            return lst.Count == 0 ? new Medicao() : lst[0];
        }

        #endregion

        #region Busca Medidor

        public uint? GetMedidor(uint idMedicao)
        {
            return GetMedidor(null, idMedicao);
        }

        public uint? GetMedidor(GDASession session, uint idMedicao)
        {
            string sql = @"SELECT idfuncmed FROM medicao WHERE idmedicao=" + idMedicao;

            return ExecuteScalar<uint?>(session, sql);
        }

        #endregion

        #region Limite de medi��es

        public int GetNumMedicoes(DateTime dia)
        {
            return GetNumMedicoes(dia, null);
        }

        public int GetNumMedicoes(DateTime dia, uint? idMedicao)
        {
            string filtro = idMedicao != null ? "idMedicao<>" + idMedicao.Value + " and " : "";
            string sql = "select count(*) from medicao where " + filtro + "dataMedicao>=?dataIni and dataMedicao<=?dataFim " +
                "and situacao in (" + (int)Medicao.SituacaoMedicao.Aberta + "," + (int)Medicao.SituacaoMedicao.EmAndamento + "," +
                (int)Medicao.SituacaoMedicao.Remarcada + ")";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", dia.Date), new GDAParameter("?dataFim", 
                dia.Date.AddDays(1).AddMilliseconds(-1))).ToString());
        }

        public DateTime? GetMedicaoDay(Medicao medicao)
        {
            return GetMedicaoDay(medicao.DataMedicao.Value, medicao.IdMedicao); 
        }

        public DateTime? GetMedicaoDay(DateTime dataInicio, uint? idMedicao)
        {
            DateTime retorno = dataInicio;
            if (retorno < DateTime.Now)
                retorno = DateTime.Now;

            int i = 0;
            while (true)
            {
                if (IsMedicaoDay(retorno, idMedicao))
                    break;

                retorno = retorno.AddDays(1);
                if (retorno.DayOfWeek == DayOfWeek.Sunday || retorno.Feriado())
                    retorno = retorno.AddDays(1);

                i++;
            }

            return retorno;
        }

        public bool IsMedicaoDay(DateTime data, uint? idMedicao)
        {
            return GetNumMedicoes(data, idMedicao) + 1 <= OrcamentoConfig.LimiteDiarioMedicoes;
        }

        #endregion

        #region Medi��o definitiva gerada de or�amento

        /// <summary>
        /// Gera uma medi��o definitiva para um or�amento.
        /// </summary>
        /// <param name="idOrca"></param>
        public uint GerarMedicaoDefinitivaOrca(uint idOrca)
        {
            lock (_medicaoDefinitivaLock)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Orcamento orca = OrcamentoDAO.Instance.GetElementByPrimaryKey(transaction, idOrca);

                        if (orca.IdMedicaoDefinitiva.GetValueOrDefault() > 0 &&
                            objPersistence.ExecuteSqlQueryCount("SELECT IdMedicao FROM medicao WHERE Situacao=" +
                                (int)Medicao.SituacaoMedicao.Aberta + " AND IdMedicao=" + orca.IdMedicaoDefinitiva.Value) > 0)
                            throw new Exception("Este or�amento j� possui uma medi��o definitiva, cancele-a para gerar outra.");

                        Medicao medicao = new Medicao();
                        medicao.Bairro = orca.BairroObra != null ? orca.BairroObra : orca.Bairro;
                        medicao.Cidade = orca.CidadeObra != null ? orca.CidadeObra : orca.Cidade;
                        medicao.Endereco = orca.EnderecoObra != null ? orca.EnderecoObra : orca.Endereco;
                        medicao.IdFunc = orca.IdFuncionario;
                        medicao.NomeCliente = orca.NomeCliente;
                        medicao.TelCliente = orca.TelCliente;
                        medicao.EmailCliente = orca.Email;
                        medicao.Situacao = (int)Medicao.SituacaoMedicao.Aberta;
                        medicao.Turno = 1;
                        medicao.IdLoja = orca.IdLoja.GetValueOrDefault(LojaDAO.Instance.ObtemValorCampo<uint>(transaction, "idLoja", null));
                        medicao.MedicaoDefinitiva = true;
                        /* Chamado 51781. */
                        medicao.IdOrcamento = idOrca;

                        uint idMedicao = Insert(transaction, medicao);
                        
                        objPersistence.ExecuteCommand(transaction, "update orcamento set idMedicaoDefinitiva=" + idMedicao +
                            " where idOrcamento=" + idOrca);
                        
                        transaction.Commit();
                        transaction.Close();

                        return idMedicao;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se � uma medi��o definitiva gerada de or�amento.
        /// </summary>
        /// <param name="idMedicao"></param>
        /// <returns></returns>
        public bool IsMedicaoDefinitivaOrca(uint idMedicao)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from orcamento where idMedicaoDefinitiva=" + idMedicao) > 0;
        }

        #endregion

        #region Obt�m campos da medi��o

        /// <summary>
        /// Obt�m o ID do or�amento associado � medi��o.
        /// </summary>
        public int? ObterIdOrcamento(GDASession session, int idMedicao)
        {
            return ExecuteScalar<int?>(session, string.Format("SELECT IdOrcamento FROM medicao WHERE IdMedicao={0}", idMedicao));
        }

        #endregion

        #region M�todos sobrescritos

        public override uint Insert(Medicao objInsert)
        {
            return Insert((GDASession)null, objInsert);
        }

        public override uint Insert(GDASession session, Medicao objInsert)
        {
            lock (_medicaoLock)
            {
                if (objInsert.IdFunc == 0)
                    objInsert.IdFunc = null;

                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                objInsert.DataCad = DateTime.Now;

                return base.Insert(session, objInsert);
            }
        }

        public override int Update(Medicao objUpdate)
        {
            lock (_medicaoLock)
            {
                if (objUpdate.IdFunc == 0)
                    objUpdate.IdFunc = null;

                return base.Update(objUpdate);
            }
        }

        /// <summary>
        /// Altera a situa��o da medi��o para cancelada ao inv�s de excluir
        /// </summary>
        /// <param name="objDelete"></param>
        public override int Delete(Medicao objDelete)
        {
            lock (_medicaoLock)
            {
                objPersistence.ExecuteCommand("Update medicao Set Situacao=5 Where idMedicao=" + objDelete.IdMedicao);

                return 1;
            }
        }

        #endregion
    }
}