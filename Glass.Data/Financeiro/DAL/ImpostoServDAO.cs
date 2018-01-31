using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ImpostoServDAO : BaseDAO<ImpostoServ, ImpostoServDAO>
    {
        //private ImpostoServDAO() { }

        #region Busca padrão

        private string Sql(uint idImpostoServ, string dataIni, string dataFim, float valorIni, float valorFim,
            uint idFornec, string nomeFornec, bool? contabil, int tipoPagto, bool centroCustoDivergente, bool selecionar)
        {
            string criterio = String.Empty;
            string campos = selecionar ? @"i.*, f.nomeFantasia as nomeFornec, l.nomeFantasia as nomeLoja,
                pc.descricao as descrPlanoConta, func.nome as descrUsuCad, f.tipoPagto as tipoPagtoFornec, 
                (select count(*) from contas_pagar where paga=true and idImpostoServ=i.idImpostoServ)>0 as temContaPaga,
                f.Endereco as EnderecoFornec, f.Numero as NumeroFornec, f.Compl as ComplFornec, 
                f.Bairro as BairroFornec, f.Cep as CepFornec, f.TelCont as TelContFornec, f.Fax as FaxFornec, f.email as emailFornec, 
                cid.nomeCidade as CidadeFornec, cid.nomeUf as UfFornec, '$$$' as criterio" : "count(*)";

            string sql = "select " + campos + @"
                from imposto_serv i
                    left join fornecedor f on (i.idFornec=f.idFornec)
                    left join loja l on (i.idLoja=l.idLoja)
                    left join plano_contas pc on (i.idConta=pc.idConta)
                    left join funcionario func on (i.usuCad=func.idFunc)
                    Left Join cidade cid on (f.idCidade=cid.idCidade)
                where 1";

            if (idImpostoServ > 0)
            {
                sql += " and i.idImpostoServ=" + idImpostoServ;
                criterio += "Imposto/Serv.: " + idImpostoServ + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and i.dataCad>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and i.dataCad<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            if (valorIni > 0)
            {
                sql += " and i.total>=" + valorIni.ToString().Replace(",", ".");
                criterio += "Valor (a partir de): " + valorIni.ToString("C") + "    ";
            }

            if (valorFim > 0)
            {
                sql += " and i.total<=" + valorFim.ToString().Replace(",", ".");
                criterio += "Valor (até): " + valorFim.ToString("C") + "    ";
            }

            if (idFornec > 0)
            {
                sql += " and i.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " and (f.nomeFantasia like ?nomeFornec Or f.razaoSocial like ?nomeFornec)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (contabil != null)
            {
                sql += " and i.contabil=" + contabil.Value.ToString();
                criterio += contabil.Value ? "Contábil    " : "Não contábil    ";
            }

            if (tipoPagto > 0)
            {
                sql += " and i.tipoPagto=" + tipoPagto;
                criterio += "Tipo pagto.: " + (tipoPagto == (int)ImpostoServ.TipoPagtoEnum.AVista ? "À Vista" : "À Prazo") + "    ";
            }

            if (centroCustoDivergente)
            {
                sql += " AND i.total <> (SELECT COALESCE(sum(valor), 0) FROM centro_custo_associado WHERE IdImpostoServ = i.IdImpostoServ)";
                criterio += "Imposto/Serviços Avulsos com valor do centro custo divergente.  ";
            }

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string nomeFornec)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(nomeFornec))
                lst.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            return lst.ToArray();
        }

        public IList<ImpostoServ> GetForRpt(uint idImpostoServ, string dataIni, string dataFim, float valorIni, float valorFim,
            uint idFornec, string nomeFornec, bool? contabil, int tipoPagto, bool centroCustoDivergente)
        {
            return objPersistence.LoadData(Sql(idImpostoServ, dataIni, dataFim, valorIni, valorFim, idFornec, nomeFornec,
                contabil, tipoPagto, centroCustoDivergente, true), GetParams(dataIni, dataFim, nomeFornec)).ToList();
        }

        public IList<ImpostoServ> GetList(uint idImpostoServ, string dataIni, string dataFim, float valorIni, float valorFim,
            uint idFornec, string nomeFornec, bool? contabil, int tipoPagto, bool centroCustoDivergente, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idImpostoServ, dataIni, dataFim, valorIni, valorFim, idFornec, nomeFornec, 
                contabil, tipoPagto, centroCustoDivergente, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim, nomeFornec));
        }

        public int GetCount(uint idImpostoServ, string dataIni, string dataFim, float valorIni, float valorFim,
            uint idFornec, string nomeFornec, bool? contabil, int tipoPagto, bool centroCustoDivergente)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idImpostoServ, dataIni, dataFim, valorIni, valorFim, idFornec, nomeFornec,
                contabil, tipoPagto, centroCustoDivergente, false), GetParams(dataIni, dataFim, nomeFornec));
        }

        private string GetDescrPagto(GDASession session, ImpostoServ impostoServ)
        {
            string descrPagto = String.Empty;

            if (impostoServ.TipoPagto == (int)ImpostoServ.TipoPagtoEnum.AVista)
            {
                descrPagto = "À Vista";
            }
            else
            {
                if (impostoServ.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra)
                {
                    descrPagto += "À Prazo - " + impostoServ.NumParc + " vez(es)";
                    
                    descrPagto += ": ";
                    var lstParc = ParcImpostoServDAO.Instance.GetByImpostoServ(session, impostoServ.IdImpostoServ);

                    for (int i = 0; i < lstParc.Length; i++)
                        descrPagto += lstParc[i].Valor.ToString("c") + " - " + lstParc[i].Data.ToString("d") + ",    ";

                    descrPagto = descrPagto.TrimEnd(' ', ',');
                }
                else
                {
                    descrPagto += "À Prazo - " + impostoServ.NumParc + " vez(es) de " + impostoServ.ValorParc.ToString("c");

                    if (impostoServ.DataBaseVenc != null)
                        descrPagto += "  Data Base Venc.: " + impostoServ.DataBaseVenc.Value.ToString("d");
                }
            }

            return descrPagto;
        }

        public ImpostoServ GetElement(uint idImpostoServ)
        {
            return GetElement(null, idImpostoServ);
        }

        public ImpostoServ GetElement(GDASession session, uint idImpostoServ)
        {
            List<ImpostoServ> item = objPersistence.LoadData(session, Sql(idImpostoServ, null, null, 0, 0, 0, null, null, 0, false, true)).ToList();
            if (item.Count == 0)
                return null;

            item[0].DescrPagto = GetDescrPagto(session, item[0]);
            if (item[0].TipoPagto == (int)ImpostoServ.TipoPagtoEnum.APrazo && item[0].NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra)
            {
                var parcelas = ParcImpostoServDAO.Instance.GetByImpostoServ(session, idImpostoServ);

                DateTime[] datas = new DateTime[parcelas.Length];
                decimal[] valores = new decimal[parcelas.Length];

                int i = 0;
                foreach (ParcImpostoServ p in parcelas)
                {
                    datas[i] = p.Data;
                    valores[i] = p.Valor;
                    i++;
                }

                item[0].DatasParcelas = datas;
                item[0].ValoresParcelas = valores;
            }

            return item[0];
        }

        #endregion

        #region Verifica se o imposto/serviço existe

        /// <summary>
        /// Verifica se o imposto/serviço existe.
        /// </summary>
        public override bool Exists(uint idImpostoServ)
        {
            string sql = "select count(*) from imposto_serv where idImpostoServ=" + idImpostoServ;
            return CurrentPersistenceObject.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Cancelar

        /// <summary>
        /// Cancela um lançamento de imposto/serviço.
        /// </summary>
        public void Cancelar(uint idImpostoServ)
        {
            FilaOperacoes.CancelarImpostoServ.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    ContasPagar[] cp = ContasPagarDAO.Instance.GetByImpostoServ(transaction, idImpostoServ);

                    // Verifica se há alguma conta paga
                    foreach (ContasPagar c in cp)
                        if (c.Paga)
                            throw new Exception("Há uma ou mais contas pagas para esse lançamento de imposto/serviço. Cancele os pagamentos antes de continuar.");

                    // Salva as contas removidas
                    List<ContasPagar> removidos = new List<ContasPagar>();
                    
                    // Remove as contas a pagar
                    foreach (ContasPagar c in cp)
                    {
                        ContasPagarDAO.Instance.Delete(transaction, c);
                        removidos.Add(c);
                    }

                    // Marca o imposto/serviço como cancelado
                    objPersistence.ExecuteCommand(transaction, "Update imposto_serv set situacao=" + (int)ImpostoServ.SituacaoEnum.Cancelado +
                        " Where idImpostoServ=" + idImpostoServ);

                    //// Cria o Log de cancelamento do imposto
                    LogCancelamentoDAO.Instance.LogImpostoServico(transaction, GetElement(transaction, idImpostoServ), "", true);

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
                    FilaOperacoes.CancelarImpostoServ.ProximoFila();
                }
            }
        }

        #endregion

        #region Finalizar

        /// <summary>
        /// Finaliza o lançamento de imposto/serviço e gera as contas a pagar.
        /// </summary>
        public void Finalizar(uint idImpostoServ)
        {
            FilaOperacoes.FinalizarImpostoServ.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var ImpServAtual = GetElementByPrimaryKey(transaction, idImpostoServ);
                    ImpostoServ imp = GetElementByPrimaryKey(transaction, idImpostoServ);
                    var lstParc = ParcImpostoServDAO.Instance.GetByImpostoServ(transaction, idImpostoServ);
                    List<uint> idContaPg = new List<uint>();

                    if (imp.Total <= 0)
                        throw new Exception("Não há nenhum valor a ser gerado.");

                    if (imp.Situacao == (int)ImpostoServ.SituacaoEnum.Finalizado)
                        throw new Exception("Este lançamento já foi finalizado.");

                    if (ContasPagarDAO.Instance.ExisteImpostoServ(transaction, idImpostoServ))
                        throw new Exception("Já foram geradas contas a pagar para este lançamento.");
                    
                    if (imp.TipoPagto == (int)ImpostoServ.TipoPagtoEnum.APrazo && lstParc.Sum(f => f.Valor) != imp.Total)
                        throw new Exception(string.Format("O valor da soma das parcelas ({0}) difere do total a pagar ({1}).", lstParc.Sum(f => f.Valor).ToString("C"), imp.Total.ToString("C")));

                    // Campos comuns às contas a pagar
                    ContasPagar cp = new ContasPagar();
                    cp.IdImpostoServ = idImpostoServ;
                    cp.IdLoja = imp.IdLoja;
                    cp.IdFornec = imp.IdFornec;
                    cp.IdFormaPagto = imp.IdFormaPagto;
                    cp.IdConta = imp.IdConta;
                    cp.DataCad = DateTime.Now;
                    cp.Usucad = UserInfo.GetUserInfo.CodUser;
                    cp.Contabil = imp.Contabil;
                    cp.Obs = imp.Obs;

                    // Gera as contas a pagar
                    if (imp.TipoPagto == (int)ImpostoServ.TipoPagtoEnum.AVista)
                    {
                        cp.ValorVenc = imp.Total;
                        cp.DataVenc = DateTime.Now;
                        cp.AVista = true;
                        idContaPg.Add(ContasPagarDAO.Instance.Insert(transaction, cp));
                    }
                    else
                    {
                        /* Chamado 52147. */
                        if (lstParc.Count() != imp.NumParc)
                            throw new Exception("A quantidade de parcelas é diferente da quantidade de parcelas configuradas. Edite o imposto/serviço e configure as parcelas novamente.");

                        foreach (ParcImpostoServ parc in lstParc)
                        {
                            if (parc.Data.Year == 1)
                                throw new Exception("Informe a data de todas as parcelas.");

                            cp.ValorVenc = parc.Valor;
                            cp.DataVenc = parc.Data;
                            ContasPagarDAO.Instance.Insert(transaction, cp);
                        }
                    }

                    // Marca o lançamento de imposto/serviço como finalizado
                    imp.Situacao = (int)ImpostoServ.SituacaoEnum.Finalizado;
                    base.Update(transaction, imp);

                    ContasPagarDAO.Instance.AtualizaNumParcImpostoServ(transaction, idImpostoServ);

                    LogAlteracaoDAO.Instance.LogImpostoServico(transaction, ImpServAtual, imp);

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
                    FilaOperacoes.FinalizarImpostoServ.ProximoFila();
                }
            }
        }

        #endregion

        #region Reabrir

        /// <summary>
        /// Reabre um lançamento de imposto/serviço.
        /// </summary>
        public void Reabrir(uint idImpostoServ)
        {
            FilaOperacoes.ReabrirImpostoServ.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var ImpServAtual = GetElement(transaction, idImpostoServ);
                    ImpostoServ i = GetElement(transaction, idImpostoServ);
                    if (!i.ReabrirVisible)
                        throw new Exception("Esse lançamento de imposto/serviço avulso não pode ser reaberto. Verifique se já existem contas pagas para esse item.");

                    ContasPagarDAO.Instance.DeleteByImpostoServ(transaction, idImpostoServ);
                    i.Situacao = (int)ImpostoServ.SituacaoEnum.Aberto;
                    Update(transaction, i);

                    //// Cria o Log ao reabrir o imposto
                    LogAlteracaoDAO.Instance.LogImpostoServico(transaction, ImpServAtual, i);

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
                    FilaOperacoes.ReabrirImpostoServ.ProximoFila();
                }
            }
        }

        #endregion

        #region Obtém situação do imposto/serviço

        public int ObtemSituacao(uint idImpostoServ)
        {
            return ObtemValorCampo<int>("situacao", "idImpostoServ=" + idImpostoServ);
        }

        public uint ObtemIdFornec(uint idImpostoServ)
        {
            return ObtemValorCampo<uint>("idFornec", "idImpostoServ=" + idImpostoServ);
        }

        public int ObtemTipoPagto(uint idImpostoServ)
        {
            return ObtemValorCampo<int>("tipoPagto", "idImpostoServ=" + idImpostoServ);
        }

        public int ObtemIdConta(GDASession session, int idImpostoServ)
        {
            return ObtemValorCampo<int>(session, "IdConta", "idImpostoServ=" + idImpostoServ);
        }

        #endregion

        #region Insere as parcelas do imposto/serviço avulso

        private void InsertParc(GDASession session, uint idImpostoServ, ImpostoServ imp)
        {
            ParcImpostoServDAO.Instance.DeleteByImpostoServ(session, idImpostoServ);

            if (imp.TipoPagto != (int)ImpostoServ.TipoPagtoEnum.APrazo)
                return;

            if (imp.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra && imp.DatasParcelas.Length > 0)
                for (int i = 0; i < imp.NumParc; i++)
                {
                    ParcImpostoServ parc = new ParcImpostoServ();
                    parc.IdImpostoServ = idImpostoServ;
                    parc.Data = i >= imp.DatasParcelas.Count() ? parc.Data : imp.DatasParcelas[i];
                    parc.Valor = i >= imp.ValoresParcelas.Count() ? parc.Valor : imp.ValoresParcelas[i];

                    ParcImpostoServDAO.Instance.Insert(session, parc);
                }
            else if (imp.DataBaseVenc != null)
                for (int i = 0; i < imp.NumParc; i++)
                {
                    if (imp.ValorParc == 0)
                        throw new Exception("O valor da parcela deve ser informado.");

                    ParcImpostoServ parc = new ParcImpostoServ();
                    parc.IdImpostoServ = idImpostoServ;
                    parc.Data = imp.DataBaseVenc.Value.AddMonths(i);
                    parc.Valor = imp.ValorParc;

                    ParcImpostoServDAO.Instance.Insert(session, parc);
                }
        }

        #endregion

        #region Métodos sobrescritos

        #region Insert

        public override uint Insert(ImpostoServ objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

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
            }
        }

        public override uint Insert(GDASession session, ImpostoServ objInsert)
        {
            /* Chamado 49733. */
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.InserirImpostoServicoAvulsoParaQualquerLoja) && UserInfo.GetUserInfo.IdLoja != objInsert.IdLoja)
                throw new Exception("Você não possui a permissão Cadastrar imposto/serviço avulso para qualquer loja, portanto, é permitido efetuar o lançamento somente para a loja associada ao seu cadastro.");

            objInsert.DataCad = DateTime.Now;
            objInsert.UsuCad = UserInfo.GetUserInfo.CodUser;
            objInsert.Situacao = (int)ImpostoServ.SituacaoEnum.Aberto;

            var id = base.Insert(session, objInsert);

            InsertParc(session, id, objInsert);

            return id;
        }

        #endregion

        #region Update

        public override int Update(ImpostoServ objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var ImpServAtual = GetElement(transaction, objUpdate.IdImpostoServ);

                    var retorno = Update(transaction, objUpdate);

                    //// Cria o Log ao atualizar o imposto/Serv
                    LogAlteracaoDAO.Instance.LogImpostoServico(transaction, ImpServAtual, objUpdate);

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
            }
        }

        public override int Update(GDASession session, ImpostoServ objUpdate)
        {
            /* Chamado 49733. */
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.InserirImpostoServicoAvulsoParaQualquerLoja) && UserInfo.GetUserInfo.IdLoja != objUpdate.IdLoja)
                throw new Exception("Você não possui a permissão Cadastrar imposto/serviço avulso para qualquer loja, portanto, é permitido efetuar o lançamento somente para a loja associada ao seu cadastro.");

            InsertParc(session, objUpdate.IdImpostoServ, objUpdate);

            return base.Update(session, objUpdate);
        }

        #endregion

        #region Delete

        public override int Delete(ImpostoServ objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdImpostoServ);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = DeleteByPrimaryKey(transaction, Key);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Commit();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int DeleteByPrimaryKey(GDASession session, uint Key)
        {
            InsertParc(session, Key, new ImpostoServ());

            //Deleta os anexos se houver
            if (FotosImpostoServDAO.Instance.PossuiAnexo(session, Key))
            {
                var anexos = FotosImpostoServDAO.Instance.GetByImpostoServ(session, Key);

                foreach (FotosImpostoServ anexo in anexos)
                    FotosImpostoServDAO.Instance.DeleteInstanceByPrimaryKey(session, anexo.IdFoto);
            }

            return GDAOperations.Delete(session, new ImpostoServ { IdImpostoServ = Key });
        }

        #endregion

        #endregion
    }
}
