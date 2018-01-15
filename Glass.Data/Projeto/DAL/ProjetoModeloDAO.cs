using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.IO;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ProjetoModeloDAO : BaseDAO<ProjetoModelo, ProjetoModeloDAO>
    {
        //private ProjetoModeloDAO() { }

        #region Busca para edição

        private string Sql(uint idProjetoModelo, string codigo, string descricao, uint idGrupoModelo, int situacao, bool selecionar)
        {
            string campos = selecionar ? "p.*, g.Descricao as DescrGrupo" : "Count(*)";

            string sql = "Select " + campos + @" From projeto_modelo p 
                Inner Join grupo_modelo g On (p.idGrupoModelo=g.idGrupoModelo) Where 1";

            if (idProjetoModelo > 0)
                sql += " And p.idProjetoModelo=" + idProjetoModelo;

            if (!String.IsNullOrEmpty(codigo))
                sql += " And p.codigo=?codigo";
            
            return sql;
        }

        public ProjetoModelo GetElement(uint idProjetoModelo)
        {
            return GetElement(null, idProjetoModelo);
        }

        public ProjetoModelo GetElement(GDASession session, uint idProjetoModelo)
        {
            var projMod = objPersistence.LoadOneData(session, Sql(idProjetoModelo, null, null, 0, 0, true));

            if (IsConfiguravel(session, idProjetoModelo) || projMod.TipoMedidasInst == 0)
            {
                var lstMedidaProjMod = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(session, idProjetoModelo, false);
                var medidas = string.Empty;

                foreach (var mpm in lstMedidaProjMod)
                    medidas += mpm.IdMedidaProjeto + ",";

                projMod.MedidasProjMod = medidas.TrimEnd(',');
            }

            return projMod;
        }

        public ProjetoModelo GetByCodigo(string codigo)
        {
            var itens = objPersistence.LoadData(Sql(0, codigo, null, 0, 0, true), new GDAParameter("?codigo", codigo)).ToList();
            var projMod = itens.Count > 0 ? itens[0] : null;

            if (projMod != null)
                if (IsConfiguravel(projMod.IdProjetoModelo) || projMod.TipoMedidasInst == 0)
                {
                    List<MedidaProjetoModelo> lstMedidaProjMod = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(projMod.IdProjetoModelo, false);

                    string medidas = string.Empty;

                    foreach (MedidaProjetoModelo mpm in lstMedidaProjMod)
                        medidas += mpm.IdMedidaProjeto + ",";

                    projMod.MedidasProjMod = medidas.TrimEnd(',');
                }

            return projMod;
        }

        #endregion

        #region Busca de Modelos

        private string SqlBusca(string codigo, string descricao, uint idGrupoModelo, int situacao, bool selecionar)
        {
            string campos = selecionar ? "p.*, g.Descricao as DescrGrupo" : "Count(*)";

            string sql = "Select " + campos + @" From projeto_modelo p 
                Inner Join grupo_modelo g On (p.idGrupoModelo=g.idGrupoModelo) Where 1";

            if (!String.IsNullOrEmpty(codigo))
                sql += " And p.codigo like ?codigo";

            if (!String.IsNullOrEmpty(descricao))
                sql += " And p.descricao like ?descricao";

            if (idGrupoModelo > 0 && String.IsNullOrEmpty(codigo))
                sql += " And p.idGrupoModelo=" + idGrupoModelo;

            if (situacao > 0)
                sql += " And p.situacao=" + situacao;

            var usuario = UserInfo.GetUserInfo;

            /* Chamado 23727. */
            if (!usuario.IsAdminSync)
                sql += " AND g.Descricao NOT LIKE '%INATIVO%'";

            return sql;
        }

        public List<ProjetoModelo> GetList(string codigo, string descricao, uint idGrupoModelo)
        {
            return GetList(codigo, descricao, idGrupoModelo, 1);
        }

        public List<ProjetoModelo> GetList(string codigo, string descricao, uint idGrupoModelo, int situacao)
        {
            string sort = " Order By p.codigo";

            return objPersistence.LoadData(SqlBusca(codigo, descricao, idGrupoModelo, situacao, true) + sort, GetParams(codigo, descricao));
        }

        public IList<ProjetoModelo> GetList(string codigo, string descricao, uint idGrupoModelo, int situacao, string sortExpression, int startRow, int pageSize)
        {
            var sort = string.IsNullOrEmpty(sortExpression) ? "p.codigo" : sortExpression;
            
            return LoadDataWithSortExpression(SqlBusca(codigo, descricao, idGrupoModelo, situacao, true), sort, startRow, pageSize, GetParams(codigo, descricao));
        }

        public int GetCount(string codigo, string descricao, uint idGrupoModelo, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlBusca(codigo, descricao, idGrupoModelo, situacao, false), GetParams(codigo, descricao));
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

        #region PesquisarProjetoModelo

        private string SqlPesquisarProjetoModelo(string codigo, string descricao, uint idGrupoModelo, int situacao, bool selecionar)
        {
            string campos = selecionar ? "p.*, g.Descricao as DescrGrupo" : "Count(*)";

            string sql = "Select " + campos + @" From projeto_modelo p 
                Inner Join grupo_modelo g On (p.idGrupoModelo=g.idGrupoModelo) Where 1";

            if (!String.IsNullOrEmpty(codigo))
                sql += " And p.codigo like ?codigo";

            if (!String.IsNullOrEmpty(descricao))
            {
                if (descricao.Contains('"'))
                {
                    if (descricao[0] == '"' && descricao[descricao.Length - 1] == '"')
                        sql += " And p.Descricao LIKE ?descricao";
                }
                else
                {
                    var palavras = descricao.Split(' ');
                    for (var i = 0; i < palavras.Count(); i++)
                    {
                        if (i == 0)
                            sql += " And (";
                        else
                            sql += " And";
                        sql += (string.Format(" p.Descricao LIKE ?descricao{0}", i));
                    }
                    sql += ")";
                }
            }

            if (idGrupoModelo > 0 && String.IsNullOrEmpty(codigo))
                sql += " And p.idGrupoModelo=" + idGrupoModelo;

            if (situacao > 0)
                sql += " And p.situacao=" + situacao;

            var usuario = UserInfo.GetUserInfo;

            /* Chamado 23727. */
            if (!usuario.IsAdminSync)
                sql += " AND g.Descricao NOT LIKE '%INATIVO%'";

            return sql;
        }

        public List<ProjetoModelo> PesquisarProjetoModelo(string codigo, string descricao, uint idGrupoModelo)
        {
            return PesquisarProjetoModelo(codigo, descricao, idGrupoModelo, 1);
        }

        public List<ProjetoModelo> PesquisarProjetoModelo(string codigo, string descricao, uint idGrupoModelo, int situacao)
        {
            string sort = " Order By p.codigo";

            return objPersistence.LoadData(SqlPesquisarProjetoModelo(codigo, descricao, idGrupoModelo, situacao, true) + sort, GetParamsProjetoModelo(codigo, descricao));
        }

        public IList<ProjetoModelo> PesquisarProjetoModelo(string codigo, string descricao, uint idGrupoModelo, int situacao, string sortExpression, int startRow, int pageSize)
        {
            var sort = string.IsNullOrEmpty(sortExpression) ? "p.codigo" : sortExpression;

            return LoadDataWithSortExpression(SqlPesquisarProjetoModelo(codigo, descricao, idGrupoModelo, situacao, true), sort, startRow, pageSize, GetParamsProjetoModelo(codigo, descricao));
        }

        public int PesquisarProjetoModeloCount(string codigo, string descricao, uint idGrupoModelo, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlPesquisarProjetoModelo(codigo, descricao, idGrupoModelo, situacao, false), GetParamsProjetoModelo(codigo, descricao));
        }

        private GDAParameter[] GetParamsProjetoModelo(string codigo, string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codigo))
                lstParam.Add(new GDAParameter("?codigo", "%" + codigo + "%"));

            if (!String.IsNullOrEmpty(descricao))
            {
                if (descricao.Contains('"'))
                {
                    if (descricao[0] == '"' && descricao[descricao.Length - 1] == '"')
                           lstParam.Add(new GDAParameter("?descricao", string.Format("%{0}%", descricao.Replace("\"", ""))));
                }
                else
                {
                    var palavras = descricao.Split(' ');
                    for (var i = 0; i < palavras.Count(); i++)
                           lstParam.Add(new GDAParameter(string.Format("?descricao{0}", i), string.Format("%{0}%", palavras[i])));
                }
            }

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion PesquisarProjetoModelo

        #region Atualiza texto de orçamentos

        /// <summary>
        /// Atualiza o texto do orçamento deste modelo
        /// </summary>
        /// <param name="projModelo"></param>
        /// <returns></returns>
        public int AtualizaTextoOrcamento(ProjetoModelo projModelo)
        {
            LoginUsuario login = UserInfo.GetUserInfo;

            string sql = "Update projeto_modelo set textoOrcamento=?texto, textoOrcamentoVidro=?textoVidro Where idProjetoModelo=" + 
                projModelo.IdProjetoModelo;

            return objPersistence.ExecuteCommand(sql, new GDAParameter("?texto", projModelo.TextoOrcamento),
                new GDAParameter("?textoVidro", projModelo.TextoOrcamentoVidro));
        }

        #endregion

        #region Busca ProjetoModelo do ItemProjeto

        /// <summary>
        /// Busca ProjetoModelo do ItemProjeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public ProjetoModelo GetByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select pm.* From projeto_modelo pm " +
                "Inner Join item_projeto ip On (ip.idProjetoModelo=pm.idProjetoModelo) " +
                "Where ip.idItemProjeto=" + idItemProjeto;

            return objPersistence.LoadOneData(sessao, sql);
        }

        #endregion

        #region Busca nome da figura do ProjetoModelo do ItemProjeto

        /// <summary>
        /// Busca nome da figura do ProjetoModelo do ItemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public string GetNomeFiguraByItemProjeto(uint idItemProjeto)
        {
            return GetNomeFiguraByItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Busca nome da figura do ProjetoModelo do ItemProjeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public string GetNomeFiguraByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select Coalesce(pm.nomeFigura, '') From projeto_modelo pm " +
                "Inner Join item_projeto ip On (ip.idProjetoModelo=pm.idProjetoModelo) " +
                "Where ip.idItemProjeto=" + idItemProjeto;

            var retorno = objPersistence.ExecuteScalar(sessao, sql);

            return retorno != null ? retorno.ToString() : string.Empty;
        }

        #endregion

        #region Ativa/Inativa Projeto

        /// <summary>
        /// Inativa o modelo se o mesmo estiver ativado e vice-versa
        /// </summary>
        public void AtivarInativarProjeto(uint idProjetoModelo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var projetoModelo = GetElement(transaction, idProjetoModelo);
                    projetoModelo.Situacao = projetoModelo.Situacao == (int)ProjetoModelo.SituacaoEnum.Ativo ? (int)ProjetoModelo.SituacaoEnum.Inativo : (int)ProjetoModelo.SituacaoEnum.Ativo;

                    Update(transaction, projetoModelo);

                    transaction.Commit();
                    transaction.Close();
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

        #region Atualiza nome da figura engenharia/modelo

        /// <summary>
        /// Atualiza nome da figura engenharia/modelo
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <param name="figuraEngenharia"></param>
        /// <param name="figuraModelo"></param>
        public void AtualizaNomeFigura(uint idProjetoModelo, string figuraEngenharia, string figuraModelo)
        {
            string sql = "Update projeto_modelo Set ";

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(figuraEngenharia))
            {
                sql += "nomeFiguraAssociada=?figEng,";
                lstParam.Add(new GDAParameter("?figEng", figuraEngenharia));
            }

            if (!String.IsNullOrEmpty(figuraModelo))
            {
                // Salva a altura e largura do modelo
                if (File.Exists(Utils.GetModelosProjetoPath + figuraModelo))
                    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Utils.GetModelosProjetoPath + figuraModelo))
                    {
                        sql += "alturaFigura=?alt,";
                        lstParam.Add(new GDAParameter("?alt", bmp.Height.ToString()));

                        sql += "larguraFigura=?larg,";
                        lstParam.Add(new GDAParameter("?larg", bmp.Width.ToString()));
                    }

                sql += "nomeFigura=?figMod";
                lstParam.Add(new GDAParameter("?figMod", figuraModelo));
            }

            sql = sql.TrimEnd(',') + " Where idProjetoModelo=" + idProjetoModelo;

            objPersistence.ExecuteCommand(sql, lstParam.ToArray());
        }

        #endregion

        #region Verifica se o modelo de projeto é configurável

         /// <summary>
        /// Verifica se o modelo de projeto é configurável
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        public bool IsConfiguravel(uint idProjetoModelo)
        {
            return IsConfiguravel(null, idProjetoModelo);
        }

        /// <summary>
        /// Verifica se o modelo de projeto é configurável
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        public bool IsConfiguravel(GDASession sessao, uint idProjetoModelo)
        {
            if (idProjetoModelo > 501)
                return true;

            string sql = "Select Count(*) From projeto_modelo Where idProjetoModelo=" + idProjetoModelo +
                " And tipoMedidasInst=0 And tipoCalcAluminio=0";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se o modelo de projeto é de box padrão

        /// <summary>
        /// Verifica se o modelo de projeto é de box padrão
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <returns></returns>
        public bool IsBoxPadrao(uint idProjetoModelo)
        {
            return IsBoxPadrao(null, idProjetoModelo);
        }

        /// <summary>
        /// Verifica se o modelo de projeto é de box padrão
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProjetoModelo"></param>
        /// <returns></returns>
        public bool IsBoxPadrao(GDASession sessao, uint idProjetoModelo)
        {
            string sql = @"
                Select Count(*) From projeto_modelo pm
                    Inner Join grupo_modelo g On (pm.idGrupoModelo=g.idGrupoModelo)
                Where g.boxPadrao And pm.idProjetoModelo=" + idProjetoModelo;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Salva o nome da figura associada

        /// <summary>
        /// Salva o nome da figura associada de um modelo
        /// </summary>
        public void SalvaNomeFiguraAssociada(uint idProjetoModelo, string nomeFiguraAssociada)
        {
            string sql = "Update projeto_modelo Set nomeFiguraAssociada=?nomeFigura Where idProjetoModelo=" + idProjetoModelo;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?nomeFigura", nomeFiguraAssociada));
        }

        #endregion

        #region Obtem dados do projeto modelo

        public uint ObtemId(string codigo)
        {
            return ObtemValorCampo<uint>("idProjetoModelo", "codigo=?cod", new GDAParameter("?cod", codigo));
        }

        public string ObtemCodigo(uint id)
        {
            return ObtemCodigo(null, id);
        }

        public string ObtemCodigo(GDASession sessao, uint id)
        {
            return ObtemValorCampo<string>(sessao, "codigo", "idProjetoModelo=" + id);
        }

        public int ObtemEspessura(GDASession session, uint idProjetoModelo)
        {
            return ObtemValorCampo<int>(session, "espessura", "idProjetoModelo=" + idProjetoModelo);
        }

        public uint ObtemGrupoModelo(GDASession session, uint idProjetoModelo)
        {
            return ObtemValorCampo<uint>(session, "idGrupoModelo", "idProjetoModelo=" + idProjetoModelo);
        }

        public List<ProjetoModelo> ObtemMaisUsados(int qtdeProjetos)
        {
            IList<string> ids = ExecuteMultipleScalar<string>(@"
                Select ip.idProjetoModelo 
                From item_projeto ip 
                    Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo) 
                Where pm.situacao=" + (int)ProjetoModelo.SituacaoEnum.Ativo + @" 
                Group By ip.idProjetoModelo Order By Count(*) Desc Limit " + qtdeProjetos, null);

            if (ids.Count == 0)
                return new List<ProjetoModelo>();

            return objPersistence.LoadData("Select * From projeto_modelo Where idProjetoModelo In (" + String.Join(",", ids.ToArray()) + ")", "").ToList();
        }

        public List<ProjetoModelo> ObtemMaisUsadosCliente(uint idCliente, int qtdeProjetos)
        {
            IList<string> ids = ExecuteMultipleScalar<string>(@"
                Select ip.idProjetoModelo 
                From item_projeto ip 
                    Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                    INNER JOIN pedido P ON (p.idPedido = ip.IdPedido)
                Where pm.situacao=" + (int)ProjetoModelo.SituacaoEnum.Ativo + @" 
                    AND p.idCli = " + idCliente + @"
                Group By ip.idProjetoModelo Order By Count(*) Desc Limit " + qtdeProjetos, null);

            if (ids.Count == 0)
                return new List<ProjetoModelo>();

            return objPersistence.LoadData("Select * From projeto_modelo Where idProjetoModelo In (" + String.Join(",", ids.ToArray()) + ")", "").ToList();
        }

        public string ObtemNomeFigura(uint idProjetoModelo)
        {
            return ObtemValorCampo<string>("nomeFigura", "idProjetoModelo=" + idProjetoModelo);
        }

        public int ObtemSituacao(GDASession session, uint idProjetoModelo)
        {
            return ObtemValorCampo<int>(session, "Situacao", string.Format("IdProjetoModelo={0}", idProjetoModelo));
        }

        public string ObtemIdsCorVidro(uint idProjetoModelo)
        {
            return ObtemValorCampo<string>("CorVidro", "idProjetoModelo=" + idProjetoModelo);
        }

        public IEnumerable<uint> ObtemIdsCorVidroArr(GDASession sessao, uint idProjetoModelo)
        {
            var ids = ObtemValorCampo<string>(sessao, "CorVidro", "idProjetoModelo=" + idProjetoModelo);

            if (string.IsNullOrWhiteSpace(ids))
                return new uint[0];

            return ids.Split(',').Select(f => f.StrParaUint());
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(ProjetoModelo objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProjetoModelo objInsert)
        {
            if (objInsert.Codigo.Contains('§') || objInsert.Codigo.Contains('+'))
                throw new Exception("O código do Projeto Modelo não pode conter os caracteres '§' e '+'.");

            // Verifica se já existe um modelo com o código informado
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, "Select Count(*) From projeto_modelo Where codigo=?codigo", new GDAParameter("?codigo", objInsert.Codigo)).ToString()) > 0)
                throw new Exception("Já existe um modelo cadastrado com o código informado.");

            if (Configuracoes.ProjetoConfig.ControleModeloProjeto.ApenasAdminSyncAtivarModeloProjeto)
                objInsert.Situacao = UserInfo.GetUserInfo.IsAdminSync ? 1 : 2;
            else
                objInsert.Situacao = 1;

            uint idProjetoModelo = base.Insert(session, objInsert);

            // Salva os tipos de medidas utilizadas neste modelo
            MedidaProjetoModeloDAO.Instance.SalvaMedidas(session, idProjetoModelo, objInsert.MedidasProjMod);

            return idProjetoModelo;
        }

        public override int Update(ProjetoModelo objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, ProjetoModelo objUpdate)
        {
            if (objUpdate.Codigo.Contains('§') || objUpdate.Codigo.Contains('+'))
                throw new Exception("O código do Projeto Modelo não pode conter os caracteres '§' e '+'.");

            // Verifica se já existe um modelo com o código informado.
            if (Conversoes.StrParaInt(objPersistence.ExecuteScalar(session,
                string.Format("SELECT COUNT(*) FROM projeto_modelo WHERE Codigo=?codigo AND IdProjetoModelo<>{0}", objUpdate.IdProjetoModelo),
                    new GDAParameter("?codigo", objUpdate.Codigo)).ToString()) > 0)
                throw new Exception("Já existe um modelo cadastrado com o código informado.");

            // Verifica se alguma medida foi retirada sendo que a mesma estava sendo usada no projeto
            if (MedidaProjetoModeloDAO.Instance.MedidasRetiradasEmUso(session, objUpdate.IdProjetoModelo, objUpdate.MedidasProjMod))
                throw new Exception("Algumas das medidas retiradas deste modelo de projeto já estão sendo usadas em expressões de cálculo.");

            var projModOld = GetElementByPrimaryKey(session, objUpdate.IdProjetoModelo);

            // Se o código do modelo antigo for diferente do atual, renomeia a figura do modelo e figura engenharia
            if (objUpdate.Codigo != projModOld.Codigo)
            {
                ManipulacaoImagem.RenomearImagem(Utils.GetModelosProjetoPath + projModOld.NomeFigura, Utils.GetModelosProjetoPath + objUpdate.Codigo + ".jpg");
                ManipulacaoImagem.RenomearImagem(Utils.GetModelosProjetoPath + projModOld.NomeFiguraAssociada, Utils.GetModelosProjetoPath + objUpdate.Codigo + "§E.jpg");
                objUpdate.NomeFigura = objUpdate.Codigo + ".jpg";
                objUpdate.NomeFiguraAssociada = objUpdate.Codigo + "§E.jpg";

                /* Chamado 53479. */
                foreach (var peca in PecaProjetoModeloDAO.Instance.GetByModelo(session, objUpdate.IdProjetoModelo))
                    if (peca.Tipo == 1)
                        ManipulacaoImagem.RenomearImagem(Utils.GetModelosProjetoPath + projModOld.NomeFigura.Replace(".jpg", string.Format("§{0}.jpg", peca.Item)),
                            Utils.GetModelosProjetoPath + objUpdate.Codigo + string.Format("§{0}.jpg", peca.Item));
            }

            // Salva os tipos de medidas utilizadas neste modelo
            MedidaProjetoModeloDAO.Instance.SalvaMedidas(session, objUpdate.IdProjetoModelo, objUpdate.MedidasProjMod);

            // Se o modelo de projeto não for configurável, não permite alterar alguns campos
            if (!IsConfiguravel(session, objUpdate.IdProjetoModelo))
            {
                objUpdate.TipoMedidasInst = projModOld.TipoMedidasInst;
                objUpdate.TipoCalcAluminio = projModOld.TipoCalcAluminio;
                objUpdate.TipoDesenho = projModOld.TipoDesenho;
                objUpdate.EixoPuxador = projModOld.EixoPuxador;
            }

            objUpdate.TextoOrcamento = projModOld.TextoOrcamento;
            objUpdate.TextoOrcamentoVidro = projModOld.TextoOrcamentoVidro;

            LogAlteracaoDAO.Instance.LogProjetoModelo(session, objUpdate);
            return base.Update(session, objUpdate);
        }

        #endregion
    }
}