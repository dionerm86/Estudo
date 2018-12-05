using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using System.Linq;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class AmbientePedidoEspelhoDAO : BaseDAO<AmbientePedidoEspelho, AmbientePedidoEspelhoDAO>
    {
        //private AmbientePedidoEspelhoDAO() { }

        private string Sql(uint idPedido, string idsPedidos, uint idAmbientePedido, uint idAmbientePedidoOrig, uint idProdPed, string idsAmbientes,
            bool selecionar, bool agrupar, bool forPcp)
        {
            string campos = selecionar ? @"a.*, p.codOtimizacao, p.codInterno, ip.obs as obsProj, ea.codInterno as codAplicacao,
                cast(sum(pp.total+coalesce(pp.valorBenef,0)) as decimal(12,2)) as totalProdutos,
                cast(sum(coalesce(pp.valorAcrescimoProd,0)) as decimal(12,2)) as valorAcrescimo,
                cast(sum(coalesce(pp.valorDescontoProd,0)) as decimal(12,2)) as valorDesconto,
                ep.codInterno as codProcesso" : "a.idAmbientePedido";

            string sql = "Select " + campos + @" From ambiente_pedido_espelho a
                Left Join produtos_pedido_espelho pp On (pp.idAmbientePedido=a.idAmbientePedido)
                Left Join produto p On (a.idProd=p.idProd)
                Left Join item_projeto ip On (a.iditemProjeto=ip.idItemProjeto)
                Left Join etiqueta_aplicacao ea On (a.idAplicacao=ea.idAplicacao)
                Left Join etiqueta_processo ep On (a.idProcesso=ep.idProcesso)
                Where pp.IdProdPedParent IS NULL ";

            if (idPedido > 0)
                sql += " And a.idPedido=" + idPedido;

            if (!string.IsNullOrEmpty(idsPedidos))
                sql += " And a.idPedido IN (" + idsPedidos + ")";

            if (idAmbientePedido > 0)
                sql += " and a.idAmbientePedido=" + idAmbientePedido;

            if (idAmbientePedidoOrig > 0)
                sql += " and a.idAmbientePedidoOrig=" + idAmbientePedidoOrig;

            if (idProdPed > 0)
                sql += " and pp.idProdPed=" + idProdPed;

            if (!String.IsNullOrEmpty(idsAmbientes))
                sql += " and a.idAmbientePedido in (" + idsAmbientes + ")";

            if (agrupar)
                sql += " group by a.idAmbientePedido";

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql;
        }

        private void AtualizaPodeEditar(ref IList<AmbientePedidoEspelho> ambientes)
        {
            if (ambientes.Count == 0)
                return;

            string ids = String.Empty;
            foreach (AmbientePedidoEspelho a in ambientes)
                ids += a.IdAmbientePedido + ",";

            // Recupera os idsProdPed dos materiais
            string sql = @"
                select idProdPed
                from produtos_pedido_espelho
                where coalesce(invisivelFluxo,false)=false and idAmbientePedido in ({0})";

            ids = GetValoresCampo(String.Format(sql, ids.TrimEnd(',')), "idProdPed");

            if (!String.IsNullOrEmpty(ids))
            {
                // Recupera os idsProdPed que estão impressos
                sql = @"
                    select pi.idProdPed
                    from produto_impressao pi
                        left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                    where coalesce(ie.situacao, 0)={0} and !coalesce(pi.cancelado,false)
                        and pi.idProdPed in ({1})";

                ids = GetValoresCampo(String.Format(sql, (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa,
                    ids.TrimEnd(',')), "idProdPed");

                if (!String.IsNullOrEmpty(ids))
                {
                    // Recupera os idsItemProjeto que possuem produtos impressos
                    sql = @"
                        select idAmbientePedido
                        from produtos_pedido_espelho
                        where coalesce(invisivelFluxo,false)=false and idProdPed in ({0})";

                    ids = GetValoresCampo(String.Format(sql, ids.TrimEnd(',')), "idAmbientePedido");
                }
            }

            List<string> i = new List<string>(!String.IsNullOrEmpty(ids) ? ids.Split(',') : new string[] { });
            foreach (AmbientePedidoEspelho a in ambientes)
                a.EditDeleteVisible = !i.Contains(a.IdAmbientePedido.ToString());
        }

        public IList<AmbientePedidoEspelho> GetList(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            if (CountInPedido(idPedido) == 0)
            {
                List<AmbientePedidoEspelho> lst = new List<AmbientePedidoEspelho>();
                lst.Add(new AmbientePedidoEspelho());
                return lst.ToArray();
            }

            string filtro = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "a.idAmbientePedido";

            var amb = LoadDataWithSortExpression(Sql(idPedido, null, 0, 0, 0, null, true, true, false), filtro, startRow, pageSize, null);
            AtualizaPodeEditar(ref amb);

            return amb;
        }

        public int GetCount(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            int count = CountInPedido(idPedido);
            return count == 0 ? 1 : count;
        }

        public int GetCount(uint idPedido)
        {
            int count = CountInPedido(idPedido);
            return count == 0 ? 1 : count;
        }

        public AmbientePedidoEspelho GetByIdProdPed(uint idProdPed)
        {
            List<AmbientePedidoEspelho> item = objPersistence.LoadData(Sql(0, null, 0, 0, idProdPed, null, true, true, false));
            return item.Count > 0 ? item[0] : null;
        }

        public IList<AmbientePedidoEspelho> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        public IList<AmbientePedidoEspelho> GetByPedido(GDASession sessao, uint idPedido)
        {
            return GetByPedido(sessao, idPedido, false);
        }

        public IList<AmbientePedidoEspelho> GetByPedido(uint idPedido, bool forPcp)
        {
            return GetByPedido(null, idPedido, forPcp);
        }

        public IList<AmbientePedidoEspelho> GetByPedido(GDASession sessao, uint idPedido, bool forPcp)
        {
            return objPersistence.LoadData(sessao, Sql(idPedido, null, 0, 0, 0, null, true, true, forPcp)).ToList();
        }

        public IList<AmbientePedidoEspelho> GetByString(string idsAmbientes)
        {
            return objPersistence.LoadData(Sql(0, null, 0, 0, 0, idsAmbientes, true, true, false)).ToList();
        }

        public IList<AmbientePedidoEspelho> GetForCompraPcp(uint idPedido, string idsAmbientes)
        {
            return objPersistence.LoadData(Sql(idPedido, null, 0, 0, 0, idsAmbientes, true, true, true)).ToList();
        }

        public bool PossuiProdutos(uint idAmbientePedido)
        {
            return PossuiProdutos(null, idAmbientePedido);
        }

        public bool PossuiProdutos(GDASession session, uint idAmbientePedido)
        {
            string sql = "select count(*) from produtos_pedido_espelho where idAmbientePedido=" + idAmbientePedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public IList<AmbientePedidoEspelho> GetForEtiquetas(uint idPedido, uint idProcesso, uint idAplicacao,
            uint idCorVidro, float espessura, uint idSubgrupoProd, float alturaMin, float alturaMax, int larguraMin, int larguraMax)
        {
            string sql = Sql(idPedido, null, 0, 0, 0, null, true, false, false);
            sql += " and a.qtde>a.qtdeImpresso and (select count(*) from produtos_pedido_espelho where idAmbientePedido=a.idAmbientePedido)>0 group by a.idAmbientePedido";

            if (idProcesso > 0)
                sql += " and pp.idProcesso=" + idProcesso;

            if (idAplicacao > 0)
                sql += " and pp.idAplicacao=" + idAplicacao;

            if (idCorVidro > 0)
                sql += " and p.idCorVidro=" + idCorVidro;

            if (espessura > 0)
                sql += " and p.espessura=" + espessura.ToString().Replace(",", ".");

            if (idSubgrupoProd > 0)
                sql += " and p.idSubgrupoProd=" + idSubgrupoProd;

            if (alturaMin > 0)
                sql += " and a.altura>=" + alturaMin.ToString().Replace(",", ".");

            if (alturaMax > 0)
                sql += " and a.altura<=" + alturaMax.ToString().Replace(",", ".");

            if (larguraMin > 0)
                sql += " and a.largura>=" + larguraMin;

            if (larguraMax > 0)
                sql += " and a.largura<=" + larguraMax;

            return objPersistence.LoadData(sql).ToList();
        }

        public string GetDescrMaoObra(uint idAmbientePedido)
        {
            int qtdeAmbiente = AmbientePedidoEspelhoDAO.Instance.GetQtde(idAmbientePedido);

            ProdutosPedidoEspelho[] prod = qtdeAmbiente > 0 ? ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(idAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.GetAllByAmbiente(idAmbientePedido);

            string retorno = String.Empty;
            foreach (ProdutosPedidoEspelho maoObra in prod)
                retorno += maoObra.DescricaoProdutoComBenef + "; ";

            return retorno;
        }

        /// <summary>
        /// Retorna a quantidade de ambientes relacionados ao pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int CountInPedido(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idPedido, null, 0, 0, 0, null, false, true, false), null);
        }

        public AmbientePedidoEspelho GetElement(uint idAmbientePedido)
        {
            return objPersistence.LoadOneData(Sql(0, null, idAmbientePedido, 0, 0, null, true, true, false));
        }

        public AmbientePedidoEspelho GetElementOrig(uint idAmbientePedidoOrig)
        {
            return objPersistence.LoadOneData(Sql(0, null, 0, idAmbientePedidoOrig, 0, null, true, true, false));
        }

        public uint? GetKeyByPedidoDescr(uint idPedido, string descricao)
        {
            string sql = "select coalesce(idAmbientePedido, 0) from ambiente_pedido_espelho where idPedido=" +
                idPedido + " and descricao LIKE ?descr";

            return ExecuteScalar<uint?>(sql, new GDAParameter("?descr", "%" + descricao + "%"));
        }

        public int GetQtde(uint? idAmbientePedido)
        {
            return GetQtde(null, idAmbientePedido);
        }

        public int GetQtde(GDASession sessao, uint? idAmbientePedido)
        {
            if (idAmbientePedido == null || idAmbientePedido <= 0)
                return 1;

            string sql = "select coalesce(qtde, 1) from ambiente_pedido_espelho where idAmbientePedido=" + idAmbientePedido;
            return ExecuteScalar<int>(sessao, sql);
        }

        public uint InsertComTransacao(AmbientePedidoEspelho objInsert)
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

        #region CompraPcp

        public IList<AmbientePedidoEspelho> ObterParaCompraPcpPorPedidos(string idsPedidos, string idsAmbientes)
        {
            return objPersistence.LoadData(Sql(0, idsPedidos.Substring(0, idsPedidos.LastIndexOf(',')), 0, 0, 0, idsAmbientes, true, true, true)).ToList();
        }

        public IList<AmbientePedidoEspelho> ObterPorPedidos(string idsPedidos)
        {
            return objPersistence.LoadData(Sql(0, idsPedidos, 0, 0, 0, null, true, true, true)).ToList();
        }

        #endregion

        #region Marca a quantidade de determinado item que foi impresso

        /// <summary>
        /// Marca a quantidade de determinado produto que foi impresso
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="qtdImpresso"></param>
        /// <param name="obs"></param>
        public void MarcarImpressao(GDASession session, uint idAmbientePedido, int qtdImpresso, string obs)
        {
            string sql = "Update ambiente_pedido_espelho set qtdeImpresso=coalesce(qtdeImpresso,0)+" + qtdImpresso +
                ", obs=?obs Where idAmbientePedido=" + idAmbientePedido;

            objPersistence.ExecuteCommand(session, sql, new GDAParameter[] { new GDAParameter("?obs", obs) });
        }

        #endregion

        #region Retorna a posição de um prodPed ou prodPed de uma posição

        /// <summary>
        /// Retorna a posição de um produto no pedido, do grupo vidro ou de pedido mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int GetAmbientePosition(uint idPedido, uint idAmbientePedido)
        {
            return GetAmbientePosition(null, idPedido, idAmbientePedido);
        }

        /// <summary>
        /// Retorna a posição de um produto no pedido, do grupo vidro ou de pedido mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public int GetAmbientePosition(GDASession session, uint idPedido, uint idAmbientePedido)
        {
            string sql = "Select count(*) From ambiente_pedido_espelho a " +
                "Where a.idAmbientePedido<=" + idAmbientePedido + " And a.idPedido=" + idPedido;

            return ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint GetIdAmbienteByEtiqueta(string codEtiqueta)
        {
            return GetIdAmbienteByEtiqueta(null, codEtiqueta);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint GetIdAmbienteByEtiqueta(GDASession sessao, string codEtiqueta)
        {
            // Pega o idPedido pelo código da etiqueta
            uint idPedido = Glass.Conversoes.StrParaUint(codEtiqueta.Substring(0, codEtiqueta.IndexOf('-')));

            // Pega a posição do produto no pedido pelo código da etiqueta
            int posicao = Glass.Conversoes.StrParaInt(codEtiqueta.Substring(codEtiqueta.IndexOf('-') + 1, codEtiqueta.IndexOf('.') - codEtiqueta.IndexOf('-') - 1));

            string sql = "Select a.idAmbientePedido From ambiente_pedido_espelho a " +
                "Where a.idPedido=" + idPedido + " Order by a.IdAmbientePedido Asc";

            List<uint> lstProd = objPersistence.LoadResult(sessao, sql).Select(f => f.GetUInt32(0))
                       .ToList();;

            if (lstProd.Count < posicao)
                throw new Exception("Ambiente da etiqueta não encontrado.");

            return lstProd[posicao - 1];
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public AmbientePedidoEspelho GetAmbienteByEtiqueta(string codEtiqueta)
        {
            return GetAmbienteByEtiqueta(null, codEtiqueta);
        }

        /// <summary>
        /// Retorna um produto pedido a partir da posição do mesmo no pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public AmbientePedidoEspelho GetAmbienteByEtiqueta(GDASession sessao, string codEtiqueta)
        {
            return GetElementByPrimaryKey(sessao, GetIdAmbienteByEtiqueta(sessao, codEtiqueta));
        }

        #endregion

        #region Exclui os ambientes de um pedido

        /// <summary>
        /// Exclui os ambientes de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(uint idPedido)
        {
            objPersistence.ExecuteCommand("delete from ambiente_pedido_espelho where idPedido=" + idPedido);
        }

        #endregion

        #region Retorna o IdItemProjeto do ambiente do pedido espelho

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido espelho
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public uint ObtemItemProjeto(uint idAmbientePedidoEspelho)
        {
            return ObtemItemProjeto(null, idAmbientePedidoEspelho);
        }

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido espelho
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public uint ObtemItemProjeto(GDASession session, uint idAmbientePedidoEspelho)
        {
            string sql = "Select Coalesce(idItemProjeto, 0) From ambiente_pedido_espelho Where idAmbientePedido=" +
                idAmbientePedidoEspelho;

            return ExecuteScalar<uint>(session, sql);
        }

        #endregion

        #region Retorna o IdAmbiente a partir do item projeto

        /// <summary>
        /// Retorna o idAmbiente do ambiente do pedido espelho que possui o idItemProjeto passado
        /// </summary>
        public uint ObtemIdAmbiente(uint idItemProjeto)
        {
            return ObtemIdAmbiente(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o idAmbiente do ambiente do pedido espelho que possui o idItemProjeto passado
        /// </summary>
        public uint ObtemIdAmbiente(GDASession session, uint idItemProjeto)
        {
            string sql = "Select Coalesce(idAmbientePedido, 0) From ambiente_pedido_espelho Where idItemProjeto=" +
                idItemProjeto;

            return ExecuteScalar<uint>(session, sql);
        }

        #endregion

        #region Acréscimo e Desconto

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarAcrescimo(GDASession sessao, PedidoEspelho pedidoEspelho, uint idAmbientePedido, int tipoAcrescimo, decimal acrescimo,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.AplicarAcrescimoAmbiente(
                sessao,
                pedidoEspelho,
                tipoAcrescimo,
                acrescimo,
                produtosPedidoEspelho
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverAcrescimo(GDASession sessao, PedidoEspelho pedidoEspelho, uint idAmbientePedido,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimoAmbiente(sessao, pedidoEspelho, produtosPedidoEspelho);
        }

        #endregion

        #endregion

        #region Desconto

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarDesconto(GDASession sessao, PedidoEspelho pedidoEspelho, uint idAmbientePedido, int tipoDesconto, decimal desconto,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.AplicarDescontoAmbiente(
                sessao,
                pedidoEspelho,
                tipoDesconto,
                desconto,
                produtosPedidoEspelho
            );
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverDesconto(GDASession sessao, PedidoEspelho pedidoEspelho, uint idAmbientePedido,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.RemoverDescontoAmbiente(
                sessao,
                pedidoEspelho,
                produtosPedidoEspelho
            );
        }

        #endregion

        #endregion

        #region Finalização

        internal void FinalizarAplicacaoAcrescimoDesconto(GDASession sessao, PedidoEspelho pedidoEspelho,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho, bool atualizar)
        {
            if (atualizar)
            {
                foreach (var produto in produtosPedidoEspelho)
                    ProdutosPedidoEspelhoDAO.Instance.Update(sessao, produto, pedidoEspelho);
            }
        }

        #endregion

        #endregion

        #region Retorna o id do ambiente relacionado a um item projeto

        public uint? GetIdByItemProjeto(uint idItemProjeto)
        {
            return GetIdByItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o id do ambiente relacionado a um item projeto.
        /// </summary>
        public uint? GetIdByItemProjeto(GDASession session, uint idItemProjeto)
        {
            return ObtemValorCampo<uint?>(session, "IdAmbientePedido", string.Format("IdItemProjeto={0}", idItemProjeto));
        }

        #endregion

        #region Verifica se o ambiente é redondo

        /// <summary>
        /// Verifica se o ambiente é redondo
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public bool IsRedondo(GDASession session, uint idAmbientePedido)
        {
            string sql = "Select Count(*) From ambiente_pedido_espelho Where redondo=true And idAmbientePedido=" + idAmbientePedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Obtem informações do ambiente

        /// <summary>
        /// Obtem o id do ambiente do pedido original.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public uint? ObtemIdAmbientePedidoOriginal(GDASession sessao, uint idAmbientePedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idAmbientePedidoOrig", "idAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Obtem o id do pedido do ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public uint ObtemIdPedido(uint idAmbientePedido)
        {
            return ObtemIdPedido(null, idAmbientePedido);
        }

        /// <summary>
        /// Obtem o id do pedido do ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public uint ObtemIdPedido(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<uint>(session, "idPedido", "idAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Obtém o tipo de acréscimo do ambiente.
        /// </summary>
        public int ObterTipoAcrescimo(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<int>(session, "TipoAcrescimo", string.Format("IdAmbientePedido={0}", idAmbientePedido));
        }

        /// <summary>
        /// Obtém o acréscimo do ambiente.
        /// </summary>
        public decimal ObterAcrescimo(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<decimal>(session, "Acrescimo", string.Format("IdAmbientePedido={0}", idAmbientePedido));
        }

        /// <summary>
        /// Obtem o id do ambiente pelo ambiente original.
        /// </summary>
        /// <param name="idAmbientePedidoOrig"></param>
        /// <returns></returns>
        public uint? ObtemIdAmbienteByOrig(uint? idAmbientePedidoOrig)
        {
            if (idAmbientePedidoOrig.GetValueOrDefault() == default(int))
                return null;

            return ObtemValorCampo<uint?>("idAmbientePedido", "idAmbientePedidoOrig=" + idAmbientePedidoOrig);
        }

        public string ObtemPecaVidro(GDASession session, uint idAmbientePedido)
        {
            string where = "idAmbientePedido=" + idAmbientePedido;
            AmbientePedidoEspelho amb = new AmbientePedidoEspelho();

            amb.Ambiente = ObtemValorCampo<string>(session, "ambiente", where);
            amb.Redondo = ObtemValorCampo<bool>(session, "redondo", where);
            amb.Altura = ObtemValorCampo<int?>(session, "altura", where);
            amb.Largura = ObtemValorCampo<int?>(session, "largura", where);

            return amb.PecaVidro;
        }

        public string ObtemPecaVidroQtd(uint idAmbientePedido)
        {
            string where = "idAmbientePedido=" + idAmbientePedido;
            AmbientePedidoEspelho amb = new AmbientePedidoEspelho();

            amb.Ambiente = ObtemValorCampo<string>("ambiente", where);
            amb.Redondo = ObtemValorCampo<bool>("redondo", where);
            amb.Altura = ObtemValorCampo<int?>("altura", where);
            amb.Largura = ObtemValorCampo<int?>("largura", where);
            amb.Qtde = ObtemValorCampo<int?>("qtde", where);
            amb.IdProd = ObtemValorCampo<uint?>("idProd", where);

            return amb.PecaVidroQtd;
        }

        internal decimal GetTotalBruto(uint idAmbientePedido)
        {
            string sql = String.Format(@"
                select sum(valor) from (
                    select sum(totalBruto) as valor
                    from produtos_pedido_espelho
                    where {0}
                    union all select sum(valor - valorAcrescimo - valorAcrescimoProd -
                        valorComissao + valorDesconto + valorDescontoProd) as valor
                    from produto_pedido_espelho_benef
                    where idProdPed in (select * from (
                        select idProdPed from produtos_pedido_espelho where {0}
                    ) as temp1)
                ) as temp", "coalesce(invisivelFluxo, false)=false and idAmbientePedido=" + idAmbientePedido);

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Remove a etiqueta do pedido - Mão de obra

        /// <summary>
        /// Diminui a quantidade do ambiente (se quantidade for maior que 1) ou remove o ambiente.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        public void PerdaEtiquetaMaoObra(GDASession sessao, uint idAmbientePedido)
        {
            uint idPedido = ObtemIdPedido(sessao, idAmbientePedido);
            if (!PedidoDAO.Instance.IsMaoDeObra(sessao, idPedido))
                throw new Exception("Pedido não é mão-de-obra.");

            // Atualiza a quantidade de ambientes
            objPersistence.ExecuteCommand(sessao, "update ambiente_pedido_espelho set qtde=qtde-1 where idAmbientePedido=" + idAmbientePedido);

            // Recupera a quantidade atualizada do ambiente
            int qtde = GetQtde(sessao, idAmbientePedido);

            if (qtde > 0)
            {
                // Atualiza o total dos produtos com base na nova quantidade do ambiente
                // atualiza o total do pedido (dentro do Update)
                foreach (ProdutosPedidoEspelho p in ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(sessao, 0, idAmbientePedido))
                    ProdutosPedidoEspelhoDAO.Instance.Update(sessao, p);
            }
            else
            {
                // Marca os produtos como invisível (Não deve ser excluído, pois desta forma a peça não seria localizada nem como cancelada
                // na produção). Atualiza o total do pedido (dentro do Update)
                foreach (ProdutosPedidoEspelho p in ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(sessao, 0, idAmbientePedido))
                {
                    p.QtdeInvisivel = p.Qtde;
                    p.Qtde = 0;
                    p.ValorVendido = 0;
                    p.Total = 0;
                    p.TotM = 0;
                    p.TotM2Calc = 0;
                    ProdutosPedidoEspelhoDAO.Instance.Update(sessao, p);
                }
            }
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Recupera os ambientes do pedido para a rentabilidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<AmbientePedidoEspelho> ObterAmbientesParaRentabilidade(GDA.GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM ambiente_pedido_espelho WHERE IdPedido=?id", new GDAParameter("?id", idPedido)).ToList();
        }

        /// <summary>
        /// Atualiza a rentabilidade do ambiente do pedido.
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            uint idAmbientePedido, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE ambiente_pedido_espelho SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdAmbientePedido=?id",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?id", idAmbientePedido));
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(AmbientePedidoEspelho objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession sessao, AmbientePedidoEspelho objUpdate)
        {
            var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElement(sessao, objUpdate.IdPedido);
            return Update(sessao, objUpdate, pedidoEspelho);
        }

        internal int Update(GDASession sessao, AmbientePedidoEspelho objUpdate, PedidoEspelho pedidoEspelho)
        {
            /* Chamado 23293. */
            if ((pedidoEspelho as IContainerCalculo).MaoDeObra && objUpdate.Qtde == 0)
                throw new Exception("Informe a quantidade do ambiente.");

            objUpdate.IdItemProjeto = ObtemValorCampo<uint?>(sessao, "idItemProjeto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            objUpdate.IdAmbientePedidoOriginal = ObtemValorCampo<uint?>(sessao, "idAmbientePedidoOrig", "idAmbientePedido=" + objUpdate.IdAmbientePedido);

            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(sessao, objUpdate.IdAmbientePedido);

            int tipoAcrescimo = ObtemValorCampo<int>(sessao, "tipoAcrescimo", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            decimal acrescimo = ObtemValorCampo<decimal>(sessao, "acrescimo", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            bool alterarAcrescimo = objUpdate.TipoAcrescimo != tipoAcrescimo || objUpdate.Acrescimo != acrescimo;

            if (alterarAcrescimo)
            {
                RemoverAcrescimo(sessao, pedidoEspelho, objUpdate.IdAmbientePedido, produtosPedidoEspelho);
                AplicarAcrescimo(sessao, pedidoEspelho, objUpdate.IdAmbientePedido, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, produtosPedidoEspelho);
            }

            int tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            decimal desconto = ObtemValorCampo<decimal>(sessao, "desconto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            var alterarDesconto = objUpdate.TipoDesconto != tipoDesconto || objUpdate.Desconto != desconto;

            if (alterarDesconto)
            {
                RemoverDesconto(sessao, pedidoEspelho, objUpdate.IdAmbientePedido, produtosPedidoEspelho);
                AplicarDesconto(sessao, pedidoEspelho, objUpdate.IdAmbientePedido, objUpdate.TipoDesconto, objUpdate.Desconto, produtosPedidoEspelho);
            }

            FinalizarAplicacaoAcrescimoDesconto(sessao, pedidoEspelho, produtosPedidoEspelho, alterarAcrescimo || alterarDesconto);

            int retorno = base.Update(sessao, objUpdate);

            if ((pedidoEspelho as IContainerCalculo).MaoDeObra)
            {
                objPersistence.ExecuteCommand(sessao, "update produtos_pedido_espelho set altura=" + objUpdate.Altura.Value +
                    ", largura=" + objUpdate.Largura.Value + " where idAmbientePedido=" + objUpdate.IdAmbientePedido +
                    " and (altura<>0 or largura<>0)");

                // Atualiza todos os produtos deste ambiente, caso a quantidade/m² do ambiente tenha sido alterada é necessário recalcular
                // os valores dos produtos associados ao mesmo
                foreach (ProdutosPedidoEspelho ppe in ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(sessao, objUpdate.IdAmbientePedido))
                    ProdutosPedidoEspelhoDAO.Instance.Update(sessao, ppe);

                // Após atualizar a altura e largura dos produtos deste ambiente, a mesma alteração deve ser aplicada nos clones
                foreach (ProdutosPedidoEspelho ppe in ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(sessao, 0, objUpdate.IdAmbientePedido))
                {
                    ProdutosPedidoEspelhoDAO.Instance.RemoverClone(sessao, pedidoEspelho, ppe.IdProdPed);
                    ProdutosPedidoEspelhoDAO.Instance.CriarClone(sessao, pedidoEspelho, ppe, true, false);
                }
            }

            PedidoEspelhoDAO.Instance.UpdateTotalPedido(sessao, objUpdate.IdPedido);

            return retorno;
        }

        public override int Delete(AmbientePedidoEspelho objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdAmbientePedido);
        }

        public override int DeleteByPrimaryKey(uint key)
        {
            return DeleteByPrimaryKey(null, key);
        }

        public override int DeleteByPrimaryKey(GDASession session, uint key)
        {
            uint idItemProjeto = ObtemItemProjeto(session, key);
            if (idItemProjeto > 0)
            {
                // Exclui os dados relacionados com projeto deste ambiente
                ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, idItemProjeto);
            }
            else if (PossuiProdutos(session, key))
                throw new Exception("Esse ambiente possui alguns produtos. Exclua-os antes de excluir o ambiente.");

            // Apaga os produtos desse ambiente
            foreach (ProdutosPedidoEspelho p in ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(session, 0, key))
                ProdutosPedidoEspelhoDAO.Instance.Delete(session, p);

            // Recupera o identificador do pedido associado
            var idPedido = objPersistence.LoadResult(session,
                "SELECT IdPedido FROM ambiente_pedido_espelho WHERE IdAmbientePedido=?id",
                new GDAParameter("?id", key))
                .Select(f => f.GetUInt32(0))
                .First();

            var retorno = base.DeleteByPrimaryKey(session, key);

            PedidoEspelhoDAO.Instance.UpdateTotalPedido(session, idPedido);

            return retorno;
        }

        #endregion
    }
}
