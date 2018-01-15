using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Drawing;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class SetorDAO : BaseDAO<Setor, SetorDAO>
    {
        //private SetorDAO() { }

        #region Listagem padrão

        private string SqlList(bool agrupar, bool selecionar)
        {
            string campos = agrupar ? "s.*, cast(group_concat(sb.idBenefConfig) as char) as benefSetor, c.descricao as DescrCnc" :
                selecionar ? "s.*" : "count(*)";

            string sql = @"
                Select " + campos + @"
                From setor s 
                    Left Join cnc c On (s.idCnc=c.idCnc)
                    " + (agrupar ? "Left Join setor_benef sb On (s.idSetor=sb.idSetor)" : "") + @"
                Where 1 ";

            if (agrupar)
            {
                sql += " Group By s.idSetor";
                if (!selecionar)
                    sql = "select count(*) from (" + sql + ") as temp";
            }

            return sql;
        }

        public IList<Setor> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(true) == 0)
            {
                var lst = new List<Setor>();
                lst.Add(new Setor());
                return lst.ToArray();
            }

            return LoadDataWithSortExpression(SqlList(true, true) + " Order By numSeq", null, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return GetCountReal(false);
        }

        public int GetCountReal(bool agrupar)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(agrupar, false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false, false), null);

            return count == 0 ? 1 : count;
        }

        public Setor[] GetOrdered()
        {
            return GetOrdered(false);
         }
 
        public Setor[] GetOrdered(bool marcarPeca)
        {
            string sql = SqlList(false, true) + " AND s.Situacao=" + (int)Glass.Situacao.Ativo;
            if (marcarPeca)
                sql += " AND s.Tipo<>" + (int)TipoSetor.ExpCarregamento +
                    // Chamado 13580 (entre outros).
                    // A funcionária estava marcando as peças no setor "Impressão Etiqueta" através da tela "Marcar Peça Produção"
                    // e isso estava gerando vários erros no sistema, pois, a impressão deve ser feita através da tela de impressão de etiquetas.
                    " AND s.Descricao != 'Impr. Etiqueta'";

            sql += " ORDER BY s.NumSeq ASC";

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public Setor ObterSetorPorNome(string nome)
        {
            return ObterSetorPorNome(null, nome);
        }

        public Setor ObterSetorPorNome(GDASession sessao, string nome)
        {
            string sql = "select s.*, cast(group_concat(sb.idBenefConfig) as char) as benefSetor from setor s Left Join setor_benef sb On (s.idSetor=sb.idSetor) where descricao =?nome";

            return objPersistence.LoadOneData(sessao, sql, new GDA.GDAParameter("?nome", nome));
        }

        public string ObterIdsSetoresPorTipo(TipoSetor tipo)
        {
            return String.Join(",", ExecuteMultipleScalar<string>("Select Cast(idSetor as char) From setor Where tipo=" + (int)tipo).ToArray());
        }
        
        #endregion

        #region Troca posição do setor

        /// <summary>
        /// Troca a posição do setor passado
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="acima"></param>
        public void TrocaPosicao(uint idSetor, bool acima)
        {
            int numSeqSetor = ObtemValorCampo<int>("numSeq", "idSetor=" + idSetor);

            // Só troca de posição se houver algum setor abaixo/acima deste para ser trocado, 
            // lembrando que a posição Impr. Etiqueta não pode ser trocada
            if (numSeqSetor == 1 || (acima && numSeqSetor == 2) ||
                (!acima && Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Max(numSeq) From setor").ToString()) == numSeqSetor))
                return;
            
            // Altera a posição do setor adjacente à este
            objPersistence.ExecuteCommand("Update setor Set numSeq=numSeq" + (acima ? "+1" : "-1") +
                " Where numSeq=" + (numSeqSetor + (acima ? -1 : 1)));

            // Altera a posição deste setor
            objPersistence.ExecuteCommand("Update setor Set numSeq=numSeq" + (acima ? "-1" : "+1") +
                " Where idSetor=" + idSetor);

            // Recarrega listagem de setores
            Utils.GetSetores = SetorDAO.Instance.GetOrdered();
        }

        #endregion

        #region Retorna o nome do setor

        /// <summary>
        /// Retorna o nome do primeiro setor.
        /// </summary>
        /// <returns></returns>
        public string GetNomePrimSetor()
        {
            object retorno = objPersistence.ExecuteScalar("select descricao from setor where numSeq=2");
            return retorno != null ? retorno.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o nome dos setores informados separados por vírgula.
        /// </summary>
        /// <returns></returns>
        public string GetNomeSetores(string idsSetor)
        {
            return !String.IsNullOrEmpty(idsSetor) ?
                GetValoresCampo("select descricao from setor where idSetor in (" + idsSetor + ") order by numSeq", "descricao") :
                "";
        }

        /// <summary>
        /// Retorna o nome do setor informado.
        /// </summary>
        /// <returns></returns>
        public string ObtemNomeSetor(uint idSetor)
        {
            return GetValoresCampo("Select descricao From setor Where idSetor = " + idSetor, "descricao");
        }

        /// <summary>
        /// Retorna o num. seq. do setor informado.
        /// </summary>
        public int ObtemNumSeq(GDASession session, int idSetor)
        {
            return ObtemValorCampo<int>(session, "NumSeq", "IdSetor=" + idSetor);
        }

        #endregion

        #region Verifica se o setor marca entrada de estoque

        /// <summary>
        /// Verifica se o setor marca entrada de estoque
        /// </summary>
        public bool IsEntradaEstoque(uint idSetor)
        {
            return IsEntradaEstoque(null, idSetor);
        }

        /// <summary>
        /// Verifica se o setor marca entrada de estoque
        /// </summary>
        public bool IsEntradaEstoque(GDASession session, uint idSetor)
        {
            string sql = "Select Count(*) From setor Where entradaEstoque=true And idSetor=" + idSetor;

            return objPersistence.ExecuteSqlQueryCount(session, sql, null) > 0;
        }

        #endregion

        #region Busca setores que uma peça já passou/falta passar

        /// <summary>
        /// Obtem setores passados
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresLidos(string numEtiqueta)
        {
            uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(numEtiqueta).GetValueOrDefault();
            return ObtemSetoresLidos(idProdPedProducao);
        }
        
        /// <summary>
        /// /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem setores passados
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresLidos(uint idProdPedProducao)
        {
            return ObtemSetoresLidos(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem setores passados
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresLidos(GDASession sessao, uint idProdPedProducao)
        {
            if (idProdPedProducao == 0)
                return new List<Setor>();

            string sql = @"
                Select s.*, lprod.DataLeitura, f.nome as funcLeitura 
                From setor s
                    Inner Join (
                        Select idSetor, dataLeitura, idFuncLeitura 
                        From leitura_producao 
                        Where idProdPedProducao=" + idProdPedProducao + @"
                    ) lprod On (s.idSetor = lprod.idSetor) 
                    Inner Join funcionario f On (lprod.idFuncLeitura=f.idFunc)
                order by s.numSeq asc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Obtem setores que a peça deve passar
        /// </summary>
        public IList<Setor> ObtemSetoresRestantes(string numEtiqueta)
        {
            return ObtemSetoresRestantes(null, numEtiqueta);
        }

        /// <summary>
        /// Obtem setores que a peça deve passar
        /// </summary>
        public IList<Setor> ObtemSetoresRestantes(GDASession session, string numEtiqueta)
        {
            uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, numEtiqueta).GetValueOrDefault();
            return ObtemSetoresRestantes(session, idProdPedProducao, 0);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem setores que a peça deve passar
        /// </summary>
        public IList<Setor> ObtemSetoresRestantes(uint idProdPedProducao, uint idSetor)
        {
            return ObtemSetoresRestantes(null, idProdPedProducao, idSetor);
        }        

        /// <summary>
        /// Obtem setores que a peça deve passar
        /// </summary>
        public IList<Setor> ObtemSetoresRestantes(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            var obrigatorios = ObtemSetoresObrigatorios(sessao, idProdPedProducao).Where(x => idSetor > 0 ? 
                x.NumeroSequencia < Utils.ObtemSetor(idSetor).NumeroSequencia : true);

            var lidos = ObtemSetoresLidos(sessao, idProdPedProducao);

            var idsObrigatorios = obrigatorios.Select(x => x.IdSetor);
            var idsLidos = lidos.Select(x => x.IdSetor);

            var idsRestantes = idsObrigatorios.Except(idsLidos);

            return obrigatorios.Where(x => idsRestantes.Contains(x.IdSetor)).OrderBy(f => f.NumeroSequencia).ToList();
        }

        /// <summary>
        /// Obtém a descrição dos setores restantes para uma etiqueta ou produto de pedido.
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public string ObtemDescricaoSetoresRestantes(string etiqueta, uint? idProdPedEsp)
        {
            var etiquetas = !String.IsNullOrEmpty(etiqueta) ?
                new string[] { etiqueta } :
                idProdPedEsp > 0 ? ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByIdProdPed(idProdPedEsp.Value).Split(',') : null;

            if (etiquetas == null || etiquetas.Length == 0)
                return String.Empty;

            var setores = new List<Setor>();

            foreach (var e in etiquetas)
            {
                uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(e.Trim()).GetValueOrDefault();
                var setoresRestantes = ObtemSetoresRestantes(idProdPedProducao, 0);

                foreach (var s in setoresRestantes)
                    if (setores.Where(x => x.IdSetor == s.IdSetor).Count() == 0)
                        setores.Add(s);
            }

            return String.Join(", ", setores.OrderBy(x => x.NumeroSequencia).Select(x => x.Descricao).ToArray());
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresObrigatorios(uint idProdPedProducao)
        {
            return ObtemSetoresObrigatorios(null, idProdPedProducao);
        }

        public IList<Setor> ObtemSetoresObrigatorios(GDASession sessao, uint idProdPedProducao)
        {
            if (idProdPedProducao == 0)
                return new List<Setor>();

            string idsSetores = String.Join(",", 
                RoteiroProducaoEtiquetaDAO.Instance.ObtemSetoresEtiqueta(idProdPedProducao).
                Select(x => x.ToString()).ToArray());

            if(string.IsNullOrEmpty(idsSetores))
                return new List<Setor>();

            string sql = "select * from setor where idSetor in (" + idsSetores + ")";

            var setores = objPersistence.LoadData(sessao, sql).ToList();

            uint idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(idProdPedProducao);
            foreach (Setor s in setores)
                s.DescrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBySetor(sessao, idProdPed, s.IdSetor);

            return setores;
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém os setores não lidos anteriores ao setor passado marcados com impedir avanço
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresObrigatoriosNaoLidos(uint idProdPedProducao, uint idSetor)
        {
            return ObtemSetoresObrigatoriosNaoLidos(null, idProdPedProducao, idSetor);
        }

        /// <summary>
        /// Obtém os setores não lidos anteriores ao setor passado marcados com impedir avanço
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public IList<Setor> ObtemSetoresObrigatoriosNaoLidos(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            string sqlIsTemperado = @"
                SELECT sp.`ISVIDROTEMPERADO`
                FROM `subgrupo_prod` sp
                    LEFT JOIN produto p ON sp.`IDSUBGRUPOPROD` = p.`IDSUBGRUPOPROD`
                    LEFT JOIN `produtos_pedido_espelho` ppe ON p.`IDPROD` = ppe.`IDPROD`
                    LEFT JOIN `produto_pedido_producao` ppp ON ppe.`IDPRODPED` = ppp.`IDPRODPED`
                WHERE ppp.`IDPRODPEDPRODUCAO` = " + idProdPedProducao;

            bool isTemperado = ExecuteScalar<bool>(sessao, sqlIsTemperado);

            string sql = @"
                Select s.* From setor s
                Where idSetor not in (
                        Select idSetor From leitura_producao
                        Where idProdPedProducao=" + idProdPedProducao + @"
                    )
                    And s.situacao=" + (int)Situacao.Ativo + @"
                    And s.impedirAvanco=true
                    And s.numSeq<" + ObtemValorCampo<int>(sessao, "numSeq", "idSetor=" + idSetor);

            sql += !isTemperado ? " And forno = 0" : "";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Verifica se a peça já está impressa

        /// <summary>
        /// Verifica se a peça já está impressa.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool IsPecaImpressa(GDASession sessao, uint idProdPedProducao)
        {
            // Garante que a leitura está sendo feita em uma peça impressa
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from leitura_producao " +
                "where idProdPedProducao=" + idProdPedProducao + " and idSetor=1") > 0;
        }

        /// <summary>
        /// Verifica se a peça já está impressa.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool IsPecaImpressa(uint idProdPedProducao)
        {
            return IsPecaImpressa(null, idProdPedProducao);
        }

        #endregion

        #region Verifica se o setor passado vem depois de todos os já lidos na peça passada

        /// <summary>
        /// Verifica se o setor passado vem depois de todos os já lidos na peça passada
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool IsUltimoSetor(GDASession sessao, uint idSetor, uint idProdPedProducao)
        {
            // Alterado para não usar método por causa de possível erro
            int numSeqSetor = ObtemValorCampo<int>(sessao, "numSeq", "idSetor=" + idSetor);

            string sql = @"
                Select Count(*) From leitura_producao lp 
                    Inner Join setor s On (lp.idSetor=s.idSetor)
                Where idProdPedProducao=" + idProdPedProducao + @"
                    And s.NumSeq>" + numSeqSetor;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) == 0;
        }

        /// <summary>
        /// Verifica se o setor passado vem depois de todos os já lidos na peça passada
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool IsUltimoSetor(uint idSetor, uint idProdPedProducao)
        {
            return IsUltimoSetor(null, idSetor, idProdPedProducao);
        }

        #endregion

        #region Recupera a cor do setor de peça pronta

        /// <summary>
        /// Recupera a cor do setor de peça pronta.
        /// </summary>
        /// <returns></returns>
        public Color GetCorSetorPronto()
        {
            foreach (Setor s in Utils.GetSetores)
                if (s.Tipo == TipoSetor.Pronto)
                {
                    var property = typeof(Color).GetProperty(s.DescrCorSystem, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (property != null)
                        return (Color)property.GetValue(null, null);
                }

            return Color.Black;
        }

        #endregion

        #region Busca os setores marcados como Forno

        /// <summary>
        /// Busca os setores marcados como Forno
        /// </summary>
        /// <returns></returns>
        public string ObtemIdsSetorForno()
        {
            string obj = GetValoresCampo("select idSetor from setor where forno", "idSetor");

            if (String.IsNullOrEmpty(obj))
                obj = GetValoresCampo("Select idSetor From setor Where tipo=" + (int)TipoSetor.Pronto, "idSetor");

            return !String.IsNullOrEmpty(obj) ? obj : "0";
        }

        #endregion

        #region Busca os setores marcados como Corte

        /// <summary>
        /// Busca os setores marcados como Corte
        /// </summary>
        /// <returns></returns>
        public string ObtemIdsSetorCorte()
        {
            string sql = "Select Cast(group_concat(idSetor) as char) From setor Where Corte=true";

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj.ToString() != String.Empty ? obj.ToString() : "0";
        }

        #endregion

        #region Busca os setor marcados como Exp. Carregamento ou Entrega

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os setor marcados como Exp. Carregamento
        /// </summary>
        /// <returns></returns>
        public uint ObtemIdSetorExpCarregamento()
        {
            return ObtemIdSetorExpCarregamento(null);
        }

        /// <summary>
        /// Busca os setor marcados como Exp. Carregamento
        /// </summary>
        /// <returns></returns>
        public uint ObtemIdSetorExpCarregamento(GDASession sessao)
        {
            return ExecuteScalar<uint>(sessao, "SELECT idSetor FROM setor WHERE tipo=" + (int)TipoSetor.ExpCarregamento + " and situacao=" + (int)Glass.Situacao.Ativo);
        }

        /// <summary>
        /// Busca os setor marcado como entrega
        /// </summary>
        /// <returns></returns>
        public uint ObtemIdSetorEntrega()
        {
            return ObtemIdSetorEntrega(null);
        }

        /// <summary>
        /// Busca os setor marcado como entrega
        /// </summary>
        /// <returns></returns>
        public uint ObtemIdSetorEntrega(GDASession sessao)
        {
            return ExecuteScalar<uint>(sessao, "SELECT idSetor FROM setor WHERE tipo=" + (int)TipoSetor.Entregue + " and situacao=" + (int)Glass.Situacao.Ativo);
        }

        #endregion

        #region Busca os setores marcados como Laminado

        /// <summary>
        /// Busca os setores marcados como Laminado
        /// </summary>
        /// <returns></returns>
        public string ObtemIdsSetorLaminados()
        {
            string sql = "Select Cast(group_concat(idSetor) as char) From setor Where Laminado=true";

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj.ToString() != String.Empty ? obj.ToString() : "0";
        }

        #endregion

        #region Busca setor inoperantes

        public List<string> ObtemSetoresInoperantes(DateTime dataIni)
        {
            var sql = @"
                    SELECT s.Descricao
	                FROM setor s
                    WHERE s.Situacao = 1
		                AND s.IdSetor NOT IN 
                            (
                                SELECT IdSetor 
				                FROM leitura_producao
				                WHERE dataleitura > ?dt
                            )";

            return ExecuteMultipleScalar<string>(sql, new GDAParameter("?dt", dataIni));
        }

        #endregion

        #region Obtém dados do setor
        
        /// <summary>
        /// Verifica se o setor informado é de laminado
        /// </summary>
        public bool IsLaminado(GDASession session, uint idSetor)
        {
            return ObtemValorCampo<bool>(session, "laminado", "idSetor=" + idSetor);
        }      

        public uint? ObtemIdCnc(uint idSetor)
        {
            return ObtemValorCampo<uint?>("idCnc", "idSetor=" + idSetor);
        }

        /// <summary>
        /// Obtém o tipo do setor passado
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public TipoSetor ObtemTipoSetor(uint idSetor)
        {
            return ObtemValorCampo<TipoSetor>("tipo", "idSetor=" + idSetor);
        }

        /// <summary>
        /// Obtém a situação do setor informado.
        /// </summary>
        public Situacao ObterSituacao(GDASession session, int idSetor)
        {
            return ObtemValorCampo<Situacao>(session, "Situacao", "IdSetor=" + idSetor);
        }

        /// <summary>
        /// Recupera os ids dos setores que são mostrados no painel comercial
        /// </summary>
        /// <returns></returns>
        public List<uint> ObtemIdsSetoresPainelComercial()
        {
            return ExecuteMultipleScalar<uint>(@"
                SELECT idSetor 
                FROM Setor 
                WHERE situacao=" + (int)Glass.Situacao.Ativo + @"
                    AND ExibirPainelComercial
                ORDER BY numSeq");
        }

        /// <summary>
        /// Recupera os ids dos setores que são mostrados no painel produção
        /// </summary>
        /// <returns></returns>
        public List<uint> ObtemIdsSetoresPainelProducao()
        {
            return ExecuteMultipleScalar<uint>(@"
                SELECT idSetor 
                FROM Setor 
                WHERE situacao=" + (int)Glass.Situacao.Ativo + @"
                    AND Coalesce(ExibirPainelProducao, false)=true
                ORDER BY numSeq");
        }

        #endregion

        #region Métodos sobrescritos

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se este alguma peça foi inserida neste setor
            /*if (Glass.Conversoes.StrParaInt(CurrentPersistenceObject.ExecuteScalar("Select Count(*) From leitura_producao Where idSetor=" + Key).ToString()) > 0)
                throw new Exception("Este Setor não pode ser excluído por haver peças relacionadas ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogSetor(Key);
            int retorno = GDAOperations.Delete(new Setor { IdSetor = (int)Key });

            // Recarrega listagem de setores
            Utils.GetSetores = SetorDAO.Instance.GetOrdered();

            // Apaga os beneficiamentos
            SetorBenefDAO.Instance.DeleteBySetor((int)Key);

            return retorno;*/

            throw new NotImplementedException();
        }

        public override int Delete(Setor objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdSetor);
        }

        public override uint Insert(Setor objInsert)
        {
            // Gera um novo número de sequência para este novo setor
            /*object numSeq = objPersistence.ExecuteScalar("Select Coalesce(Max(numSeq), 0) + 1 From setor");
            objInsert.NumeroSequencia = numSeq != null && numSeq.ToString() != String.Empty ? Glass.Conversoes.StrParaInt(numSeq.ToString()) : 1;

            uint idSetor = base.Insert(objInsert);

            // Recarrega listagem de setores
            Utils.GetSetores = SetorDAO.Instance.GetOrdered();

            // Cadastra os beneficiamentos associados
            if (objInsert.Tipo == TipoSetor.PorBenef)
                foreach (uint b in objInsert.Beneficiamentos)
                {
                    SetorBenef novo = new SetorBenef();
                    novo.IdSetor = idSetor;
                    novo.IdBenefConfig = b;
                    SetorBenefDAO.Instance.Insert(novo);
                }

            return idSetor;*/
            throw new NotImplementedException();
        }

        public override int Update(Setor objUpdate)
        {
            /*LogAlteracaoDAO.Instance.LogSetor(objUpdate);
            int retorno = base.Update(objUpdate);

            // Recarrega listagem de setores
            Utils.GetSetores = SetorDAO.Instance.GetOrdered();

            // Apaga os beneficiamentos
            SetorBenefDAO.Instance.DeleteBySetor(objUpdate.IdSetor);

            // Cadastra os beneficiamentos associados
            if (objUpdate.Tipo == TipoSetor.PorBenef)
                foreach (uint b in objUpdate.Beneficiamentos)
                {
                    SetorBenef novo = new SetorBenef();
                    novo.IdSetor = objUpdate.IdSetor;
                    novo.IdBenefConfig = (int)b;
                    SetorBenefDAO.Instance.Insert(novo);
                }

            return retorno;*/
            throw new NotImplementedException();
        }

        #endregion

        #region Setores da classificação 

        /// <summary>
        /// Recucpera os setores da classificação de roteiro de produção passada
        /// </summary>
        public List<Setor> GetSetoresClassificacao(int idClassificacao)
        {
            var sqlSetores = string.Format(@"SELECT * FROM setor WHERE IdSetor IN 
                (SELECT IdSetor FROM roteiro_producao_setor WHERE IdRoteiroProducao IN 
                    (SELECT IdRoteiroProducao FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao = {0}))", idClassificacao);

            return objPersistence.LoadData(sqlSetores).ToList();
        }

        #endregion

        public bool ConsultaAntes(uint idSetor)
        {
            return ExecuteScalar<bool>("select consultarAntes from setor where idSetor=" + idSetor);
        }

        public bool IsCorte(uint idSetor)
        {
            return ObtemValorCampo<bool>("corte", "idSetor=" + idSetor);
        }

        /// <summary>
        /// Retorna a Descrição do setor onde o IdSetor é igual à idSetor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public string ObtemDescricaoSetor(int idSetor)
        {
            return ObtemDescricaoSetor(null, idSetor);
        }

        /// <summary>
        /// Retorna a Descrição do setor onde o IdSetor é igual à idSetor.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public string ObtemDescricaoSetor(GDASession sessao, int idSetor)
        {
            return ObtemValorCampo<string>(sessao, "Descricao", "idSetor=" + idSetor);
        }
    }
}
