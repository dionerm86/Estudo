using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using System.Linq;
using Glass.Data.Helper;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class PecaProjetoModeloDAO : BaseDAO<PecaProjetoModelo, PecaProjetoModeloDAO>
    {
        //private PecaProjetoModeloDAO() { }

        #region Busca padrão

        /// <summary>
        /// Recupera dados das peè¢³ seguindo as configuraè¶¥s do cliente
        /// </summary>
        private string SqlPecasPersonalizadas()
        {
            return @"ppm.IdPecaProjMod, ppm.IdProjetoModelo, ppm.IdArquivoMesaCorte, ppm.IdAplicacao, ppm.IdProcesso, ppm.Altura, ppm.Largura,
                    IF(fpc.FolgaAlt06MM IS NOT NULL, fpc.FolgaAlt06MM, ppm.Altura06MM) AS Altura06MM, IF(fpc.FolgaLarg06MM IS NOT NULL, fpc.FolgaLarg06MM, ppm.Largura06MM) AS Largura06MM,
                    IF(fpc.FolgaAlt08MM IS NOT NULL, fpc.FolgaAlt08MM, ppm.Altura08MM) AS Altura08MM, IF(fpc.FolgaLarg08MM IS NOT NULL, fpc.FolgaLarg08MM, ppm.Largura08MM) AS Largura08MM,
                    IF(fpc.FolgaAlt10MM IS NOT NULL, fpc.FolgaAlt10MM, ppm.Altura10MM) AS Altura10MM, IF(fpc.FolgaLarg10MM IS NOT NULL, fpc.FolgaLarg10MM, ppm.Largura10MM) AS Largura10MM,
                    IF(fpc.FolgaAlt12MM IS NOT NULL, fpc.FolgaAlt12MM, ppm.Altura12MM) AS Altura12MM, IF(fpc.FolgaLarg12MM IS NOT NULL, fpc.FolgaLarg12MM, ppm.Largura12MM) AS Largura12MM,
                    ppm.Tipo, ppm.TipoArquivo, ppm.Qtde, ppm.Item, ppm.CalculoQtde, ppm.CalculoAltura, ppm.CalculoLargura, ppm.Redondo, ppm.Obs";
        }

        private string Sql(string codigo, string descricao, uint idGrupoModelo, uint? idCliente, bool selecionar)
        {
            var campos = string.Empty;

            if (idCliente.GetValueOrDefault() > 0)
                campos = string.Format("{0}, pm.Codigo as codModelo, pm.nomeFiguraAssociada as nomeFigura, ea.codInterno as codAplicacao, ep.codInterno as codProcesso", SqlPecasPersonalizadas());
            else
                campos = "ppm.*, pm.Codigo as codModelo, pm.nomeFiguraAssociada as nomeFigura, ea.codInterno as codAplicacao, ep.codInterno as codProcesso";

            var sql = "Select " + campos + @" From peca_projeto_modelo ppm";

            if (idCliente.GetValueOrDefault() > 0)
                sql += string.Format(" Left Join folga_peca_cliente fpc on (ppm.IdPecaProjMod=fpc.IdPecaProjetoModelo AND fpc.IdCliente={0})", idCliente);

            sql += @" Left Join projeto_modelo pm on (ppm.idProjetoModelo=pm.idProjetoModelo)
                     Left Join etiqueta_aplicacao ea on (ppm.idAplicacao=ea.idAplicacao)
                     Left Join etiqueta_processo ep on (ppm.idProcesso=ep.idProcesso)
                     Where pm.situacao=1";

            StringBuilder str = new StringBuilder();
            str.Append(sql);

            if (!string.IsNullOrEmpty(codigo))
                str.Append(" And pm.codigo like ?codigo");

            if (!string.IsNullOrEmpty(descricao))
                str.Append(" And pm.descricao like ?descricao");

            if (idGrupoModelo > 0 && string.IsNullOrEmpty(codigo))
                str.Append(" And pm.idGrupoModelo=" + idGrupoModelo);

            var count = string.Format("SELECT COUNT(*) FROM ({0}) TMP", str.ToString());
            var result = string.Format("SELECT * FROM ({0}) TMP", str.ToString());

            return selecionar ? result : count;
        }

        public IList<PecaProjetoModelo> GetListParceiros(string codigo, string descricao, uint idGrupoModelo, string sortExpression, int startRow, int pageSize)
        {
            if (UserInfo.GetUserInfo.IdCliente.GetValueOrDefault() == 0)
                return new List<PecaProjetoModelo>();

            string sort = string.IsNullOrEmpty(sortExpression) ? "codModelo" : sortExpression;

            // Essa ordenação é necessário para a tela de configuração de folgas, pois clicar no lápis para editar as folgas,
            // de alguma forma o MySql ordena as peças de uma outra forma aleatória, quando tenta editar um item do mesmo projeto,
            // essa ordenação força a ordenar as peças de um projeto sempre da mesma forma, para não correr o risco de altera a folga
            // de um item diferente.
            sort += ", idPecaProjMod asc";

            return LoadDataWithSortExpression(Sql(codigo, descricao, idGrupoModelo, UserInfo.GetUserInfo.IdCliente.GetValueOrDefault(), true), sort, startRow, pageSize, GetParams(codigo, descricao));
        }

        public int GetCountParceiros(string codigo, string descricao, uint idGrupoModelo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(codigo, descricao, idGrupoModelo, UserInfo.GetUserInfo.IdCliente.GetValueOrDefault(), false), GetParams(codigo, descricao));
        }

        public IList<PecaProjetoModelo> GetList(string codigo, string descricao, uint idGrupoModelo, uint? idCliente, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "codModelo" : sortExpression;

            // Essa ordenação é necessário para a tela de configuração de folgas, pois clicar no lápis para editar as folgas,
            // de alguma forma o MySql ordena as peças de uma outra forma aleatória, quando tenta editar um item do mesmo projeto,
            // essa ordenação força a ordenar as peças de um projeto sempre da mesma forma, para não correr o risco de altera a folga
            // de um item diferente.
            sort += ", idPecaProjMod asc";

            return LoadDataWithSortExpression(Sql(codigo, descricao, idGrupoModelo, idCliente, true), sort, startRow, pageSize, GetParams(codigo, descricao));
        }

        public int GetCount(string codigo, string descricao, uint idGrupoModelo, uint? idCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(codigo, descricao, idGrupoModelo, idCliente, false), GetParams(codigo, descricao));
        }

        private GDAParameter[] GetParams(string codigo, string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codigo))
                lstParam.Add(new GDAParameter("?codigo", "%" + codigo + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca para inserção

        private string SqlIns(uint idProjetoModelo, bool selecionar)
        {
            string campos = selecionar ? "ppm.*, ac.nome as CodArqMesa, ea.codInterno as codAplicacao, " +
                "ep.codInterno as codProcesso" : "Count(*)";

            string sql = @"
                Select " + campos + @" From peca_projeto_modelo ppm 
                    Left join arquivo_mesa_corte amc On (ppm.idArquivoMesaCorte=amc.idArquivoMesaCorte) 
                    Left Join arquivo_calcengine ac ON (amc.idArquivoCalcEngine=ac.idArquivoCalcEngine)
                    Left Join etiqueta_aplicacao ea on (ppm.idAplicacao=ea.idAplicacao)
                    Left Join etiqueta_processo ep on (ppm.idProcesso=ep.idProcesso)
                Where ppm.idProjetoModelo=" + idProjetoModelo;

            return sql;
        }

        public List<PecaProjetoModelo> GetListIns(uint idProjetoModelo)
        {
            if (GetCountRealIns(idProjetoModelo) == 0)
            {
                List<PecaProjetoModelo> lst = new List<PecaProjetoModelo>();
                lst.Add(new PecaProjetoModelo());
                return lst;
            }

            var dados = objPersistence.LoadData(SqlIns(idProjetoModelo, true)).ToList();

            foreach (var d in dados)
            {
                d.FlagsArqMesa = FlagArqMesaPecaProjModDAO.Instance.ObtemPorPecaProjMod((int)d.IdPecaProjMod).Select(f => f.IdFlagArqMesa).ToArray();

                if (d.FlagsArqMesa.Length>0)
                    d.FlagsArqMesaDescricao = string.Join(", ", Glass.Data.DAL.FlagArqMesaDAO.Instance.ObterDescricao(null, d.FlagsArqMesa.ToList()).ToArray());
            }

            return dados;
        }

        public int GetCountRealIns(uint idProjetoModelo)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlIns(idProjetoModelo, false), null);
        }

        public int GetCountIns(uint idProjetoModelo)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlIns(idProjetoModelo, false), null);

            return count == 0 ? 1 : count;
        }

        #endregion

        #region Busca peças de um modelo

        public string SqlModelo(uint idPecaProjMod, uint idProjetoModelo, int item, bool selecionar)
        {
            return SqlModelo(idPecaProjMod, idProjetoModelo, item, 0, selecionar);
        }

        public string SqlModelo(uint idPecaProjMod, uint idProjetoModelo, int item, int tipo, bool selecionar)
        {
            string campos = selecionar ? "ppm.*" : "Count(*)";

            string sql = "Select " + campos + " From peca_projeto_modelo ppm Where 1";

            if (idPecaProjMod > 0)
                sql += " And ppm.idPecaProjMod=" + idPecaProjMod;

            if (idProjetoModelo > 0)
                sql += " And ppm.idProjetoModelo=" + idProjetoModelo;

            if (item > 0)
                sql += " And ppm.item like '%" + item + "%'";

            if (tipo > 0)
                sql += string.Format(" And ppm.Tipo={0}", tipo);

            return sql;
        }

        /// <summary>
        /// Recupera a peè¡ seguindo as configuraè¶¥s do cliente (se houver)
        /// </summary>
        public PecaProjetoModelo GetByCliente(GDASession sessao, uint IdPecaProjMod, uint idCliente)
        {
            var sql = string.Format(@"SELECT {0} FROM peca_projeto_modelo ppm
                Left Join folga_peca_cliente fpc on (ppm.IdPecaProjMod=fpc.IdPecaProjetoModelo AND fpc.IdCliente={1})
                WHERE ppm.IdPecaProjMod={2}", SqlPecasPersonalizadas(), idCliente, IdPecaProjMod);          

            return objPersistence.LoadOneData(sessao, sql);
        }    

        public List<PecaProjetoModelo> GetByModelo(uint idProjetoModelo)
        {
            return GetByModelo(null, idProjetoModelo);
        }

        public List<PecaProjetoModelo> GetByModelo(GDASession sessao, uint idProjetoModelo)
        {
            return objPersistence.LoadData(sessao, SqlModelo(0, idProjetoModelo, 0, true) + " Order By ppm.tipo asc, ppm.IdPecaProjMod asc ");
        }

        public List<string> GetDistinctItemPecaProjetoModelo()
        {
            return ExecuteMultipleScalar<string>("SELECT DISTINCT(item) FROM peca_projeto_modelo");
        }

        public List<PecaProjetoModelo> ObtemPecaProjetoModeloParaTotalMarcacao(uint idProjetoModelo)
        {
            return
                objPersistence.LoadData(
                    string.Format("{0} {1}",
                        SqlModelo(0, idProjetoModelo, 0, 1, true),
                        " ORDER BY ppm.Tipo ASC, ppm.IdPecaProjMod ASC "));
        }

        public PecaProjetoModelo GetByItem(uint idProjetoModelo, int item)
        {
            return GetByItem(null, idProjetoModelo, item);
        }

        public PecaProjetoModelo GetByItem(GDASession session, uint idProjetoModelo, int item)
        {
            if (idProjetoModelo == 0)
                return new Model.PecaProjetoModelo();

            var lstPeca = objPersistence.LoadData(session, SqlModelo(0, idProjetoModelo, item, true)).ToList();

            /* Chamado 58078. */
            if (lstPeca.Count(f => f.Item == item.ToString()) == 1)
                return lstPeca.First(f => f.Item == item.ToString());

            // Como o sql acima pode retornar itens 1 e 11 por exemplo, foi necessário criar este método para corrigir esta situação
            // de forma que a exceção será lançada apenas se realmente estiver duplicado
            if (lstPeca.Count > 1)
            {
                for (int i = 0; i < lstPeca.Count; i++)
                    if (!new List<string>(lstPeca[i].Item.Split(' ', ',', 'e', 'E')).Contains(item.ToString()))
                    {
                        lstPeca.RemoveAt(i);
                        i--;
                    }
            }

            if (lstPeca.Count != 1)
                throw new Exception("Foram encontradas " + lstPeca.Count + " peça(s) de projeto com o item " + item);

            return lstPeca[0];
        }

        #endregion

        #region Busca Pelo Id

        public PecaProjetoModelo ObtemPeloId(uint idPecaProjetomodelo)
        {
            var pecaProjetoModelo = GetElementByPrimaryKey(idPecaProjetomodelo);

            pecaProjetoModelo.FlagsArqMesa = FlagArqMesaPecaProjModDAO.Instance.ObtemPorPecaProjMod((int)pecaProjetoModelo.IdPecaProjMod).Select(f => f.IdFlagArqMesa).ToArray();

            if (pecaProjetoModelo.FlagsArqMesa.Length > 0)
                pecaProjetoModelo.FlagsArqMesaDescricao = string.Join(", ", Glass.Data.DAL.FlagArqMesaDAO.Instance.ObterDescricao(null, pecaProjetoModelo.FlagsArqMesa.ToList()).ToArray());

            return pecaProjetoModelo;
        }

        #endregion

        #region Altera folga da peça

        public int UpdateFolga(PecaProjetoModelo peca)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var atual = GetElementByPrimaryKey(transaction, peca.IdPecaProjMod);

                    var sql = string.Format(@"
                        UPDATE peca_projeto_modelo ppm
                        SET ppm.Altura={0},
                            ppm.Largura={1},
                            ppm.Altura03MM={2},
                            ppm.Largura03MM={3},
                            ppm.Altura04MM={4},
                            ppm.Largura04MM={5},
                            ppm.Altura05MM={6},
                            ppm.Largura05MM={7},
                            ppm.Altura06MM={8},
                            ppm.Largura06MM={9},
                            ppm.Altura08MM={10},
                            ppm.Largura08MM={11},
                            ppm.Altura10MM={12},
                            ppm.Largura10MM={13},
                            ppm.Altura12MM={14},
                            ppm.Largura12MM={15}
                        WHERE ppm.IdPecaProjMod={16}", peca.Altura, peca.Largura, peca.Altura03MM, peca.Largura03MM, peca.Altura04MM,
                        peca.Largura04MM, peca.Altura05MM, peca.Largura05MM, peca.Altura06MM, peca.Largura06MM, peca.Altura08MM,
                        peca.Largura08MM, peca.Altura10MM, peca.Largura10MM, peca.Altura12MM, peca.Largura12MM, peca.IdPecaProjMod);

                    LogAlteracaoDAO.Instance.LogPecaProjetoModelo(transaction, atual, peca);

                    var retorno = objPersistence.ExecuteCommand(transaction, sql);

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

        #endregion

        #region Obtem dados da peça

        public string ObterItem(GDASession session, int idPecaProjMod)
        {
            return ObtemValorCampo<string>(session, "Item", string.Format("IdPecaProjMod={0}", idPecaProjMod));
        }

        public uint? ObtemIdArquivoMesaCorte(uint idPecaProjMod)
        {
            return ObtemValorCampo<uint?>("idArquivoMesaCorte", "idPecaProjMod=" + idPecaProjMod);
        }

        public TipoArquivoMesaCorte? ObtemTipoArquivoMesaCorte(uint idPecaProjMod)
        {
            return ObtemTipoArquivoMesaCorte(null, idPecaProjMod);
        }

        public TipoArquivoMesaCorte? ObtemTipoArquivoMesaCorte(GDASession session, uint idPecaProjMod)
        {
            return ObtemValorCampo<TipoArquivoMesaCorte?>(session, "TipoArquivo", "idPecaProjMod=" + idPecaProjMod);
        }

        public int ObtemFolgaAltura(uint idPecaProjMod)
        {
            return ObtemFolgaAltura(null, idPecaProjMod);
        }

        public int ObtemFolgaAltura(GDASession sessao, uint idPecaProjMod)
        {
            return ObtemValorCampo<int>(sessao, "altura", "idPecaProjMod=" + idPecaProjMod);
        }

        public int ObtemFolgaLargura(uint idPecaProjMod)
        {
            return ObtemFolgaLargura(null, idPecaProjMod);
        }

        public int ObtemFolgaLargura(GDASession sessao, uint idPecaProjMod)
        {
            return ObtemValorCampo<int>(sessao, "largura", "idPecaProjMod=" + idPecaProjMod);
        }

        #endregion

        #region Verifica se projeto possui apenas vidros fixos

        /// <summary>
        /// Verifica se projeto possui apenas vidros fixos
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <returns></returns>
        public bool ProjetoPossuiApenasFixos(uint idProjetoModelo)
        {
            return objPersistence.ExecuteSqlQueryCount(
                "Select Count(*) From peca_projeto_modelo Where tipo<>2 And idProjetoModelo=" + idProjetoModelo) == 0;
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(PecaProjetoModelo objInsert)
        {
            FilaOperacoes.InserirPecaProjetoModelo.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (string.IsNullOrEmpty(objInsert.Item))
                        throw new Exception("Informe o campo item.");

                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From peca_projeto_modelo Where idProjetoModelo=" + objInsert.IdProjetoModelo + " And item=?item", new GDAParameter("?item", objInsert.Item)) > 0)
                        throw new Exception("Já foi cadastrada uma peça com o item informado.");

                    var qtde = 0;

                    if (int.TryParse(objInsert.CalculoQtde, out qtde))
                        objInsert.Qtde = qtde;

                    uint retorno = base.Insert(transaction, objInsert);

                    foreach (PecaModeloBenef b in objInsert.Beneficiamentos.ToPecasProjetoModelo(retorno))
                        PecaModeloBenefDAO.Instance.Insert(transaction, b);

                    if (objInsert.FlagsArqMesa != null && objInsert.FlagsArqMesa.Length > 0)
                    {
                        foreach (var id in objInsert.FlagsArqMesa)
                            FlagArqMesaPecaProjModDAO.Instance.Insert(transaction, new FlagArqMesaPecaProjMod()
                            {
                                IdPecaProjMod = (int)retorno,
                                IdFlagArqMesa = id
                            });
                    }

                    objInsert.RefreshBeneficiamentos();

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
                    FilaOperacoes.InserirPecaProjetoModelo.ProximoFila();
                }
            }
        }

        public override int Update(PecaProjetoModelo objUpdate)
        {
            FilaOperacoes.AtualizarPecaProjetoModelo.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (string.IsNullOrEmpty(objUpdate.Item))
                        throw new Exception("Informe o campo item.");

                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From peca_projeto_modelo Where idProjetoModelo=" + objUpdate.IdProjetoModelo + " And item=?item And idPecaProjMod<>" + objUpdate.IdPecaProjMod, new GDAParameter("?item", objUpdate.Item)) > 0)
                        throw new Exception("Já foi cadastrada uma peça com o item informado.");

                    var qtde = 0;

                    if (int.TryParse(objUpdate.CalculoQtde, out qtde))
                        objUpdate.Qtde = qtde;

                    var pecaAtual = GetElementByPrimaryKey(transaction, objUpdate.IdPecaProjMod);

                    if (objUpdate.IdArquivoMesaCorte > 0)
                        objUpdate.CodArqMesa = ArquivoMesaCorteDAO.Instance.GetElement(transaction, objUpdate.IdArquivoMesaCorte.Value).Codigo;

                    if (pecaAtual.IdArquivoMesaCorte > 0)
                        pecaAtual.CodArqMesa = ArquivoMesaCorteDAO.Instance.GetElement(transaction, pecaAtual.IdArquivoMesaCorte.Value).Codigo;

                    // Obtém as FLAGs da peça atual, para o log de alterações.
                    pecaAtual.FlagsArqMesa = FlagArqMesaPecaProjModDAO.Instance.ObtemPorPecaProjMod(transaction, (int)pecaAtual.IdPecaProjMod).Select(f => f.IdFlagArqMesa).ToArray();
                    pecaAtual.FlagsArqMesaDescricao = string.Join("\n", FlagArqMesaDAO.Instance.ObterDescricao(transaction, pecaAtual.FlagsArqMesa.ToList()));
                    
                    // Obtém a descrição da FLAG da peça nova, para o log de alterações.
                    objUpdate.FlagsArqMesaDescricao = string.Join("\n", FlagArqMesaDAO.Instance.ObterDescricao(transaction, objUpdate.FlagsArqMesa.ToList()));

                    LogAlteracaoDAO.Instance.LogPecaProjetoModelo(transaction, pecaAtual, objUpdate);
                    base.Update(transaction, objUpdate);

                    PecaModeloBenefDAO.Instance.DeleteByPecaProjMod(transaction, objUpdate.IdPecaProjMod);
                    foreach (PecaModeloBenef b in objUpdate.Beneficiamentos.ToPecasProjetoModelo(objUpdate.IdPecaProjMod))
                        PecaModeloBenefDAO.Instance.Insert(transaction, b);

                    FlagArqMesaPecaProjModDAO.Instance.DeleteByPecaProjMod(transaction, (int)objUpdate.IdPecaProjMod);
                    if (objUpdate.FlagsArqMesa.Length > 0)
                        foreach (var id in objUpdate.FlagsArqMesa)
                            FlagArqMesaPecaProjModDAO.Instance.Insert(transaction, new FlagArqMesaPecaProjMod()
                            {
                                IdPecaProjMod = (int)objUpdate.IdPecaProjMod,
                                IdFlagArqMesa = id
                            });

                    objUpdate.RefreshBeneficiamentos();

                    transaction.Commit();
                    transaction.Close();

                    return 1;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw;
                }
                finally
                {
                    FilaOperacoes.AtualizarPecaProjetoModelo.ProximoFila();
                }
            }
        }

        public override int Delete(PecaProjetoModelo objDelete)
        {
            FilaOperacoes.ApagarPecaProjetoModelo.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    
                    // Flag para geração do arquivo de mesa.
                    FlagArqMesaPecaProjModDAO.Instance.DeleteByPecaProjMod(transaction,(int)objDelete.IdPecaProjMod);
                    // Beneficiamento associado à peça.
                    PecaModeloBenefDAO.Instance.DeleteByPecaProjMod(transaction, objDelete.IdPecaProjMod);
                    // Posições associadas à peça.
                    PosicaoPecaIndividualDAO.Instance.ApagarPeloIdPecaProjMod(transaction, (int)objDelete.IdPecaProjMod);
                    // Validação da peça do projeto.
                    ValidacaoPecaModeloDAO.Instance.DeleteByPecaProjMod(transaction, (int)objDelete.IdPecaProjMod);

                    /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                    // Recupera o nome completo da imagem da peça.
                    var nomeImagem = string.Format("{0}{1}_{2}.jpg", Utils.GetModelosProjetoPath, objDelete.IdProjetoModelo, objDelete.Item);
                    // Apaga a imagem que estava associada à peça.
                    if (File.Exists(nomeImagem))
                        File.Delete(nomeImagem);

                    // Recupera o nome completo da imagem da peça.
                    var codigoProjetoModelo = ProjetoModeloDAO.Instance.ObtemCodigo(transaction, objDelete.IdProjetoModelo);
                    nomeImagem = string.Format("{0}{1}§{2}.jpg", Utils.GetModelosProjetoPath, codigoProjetoModelo, objDelete.Item);
                    // Apaga a imagem que estava associada à peça.
                    if (File.Exists(nomeImagem))
                        File.Delete(nomeImagem);

                    LogCancelamentoDAO.Instance.LogPecaProjetoModelo(transaction, objDelete, objDelete.MotivoCancelamento, true);

                    var retorno = base.Delete(transaction, objDelete);

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
                    FilaOperacoes.ApagarPecaProjetoModelo.ProximoFila();
                }
            }
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey(Key));
        }

        #endregion
    }
}