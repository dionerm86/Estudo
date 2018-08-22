using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class AmbientePedidoDAO : BaseDAO<AmbientePedido, AmbientePedidoDAO>
    {
        //private AmbientePedidoDAO() { }

        private string Sql(uint idAmbientePedido, uint idPedido, bool isRelatorioPcp, bool apenasAmbientesComProdutos, bool selecionar)
        {
            string campos = selecionar ? @"a.*, p.codInterno, ip.obs as obsProj, pp.totalProdutos, pp.valorAcrescimo,
                pp.valorDesconto, pp.totM, ea.codInterno as codAplicacao, ep.codInterno as codProcesso, pp.totalProdutos" : "count(*)";

            string sql = "Select " + campos + " From ambiente_pedido" + (isRelatorioPcp ? "_espelho" : String.Empty) + @" a
                " + (selecionar ? (isRelatorioPcp ? @"Left Join ambiente_pedido a1 On (a.idAmbientePedidoOrig=a1.idAmbientePedido)" : String.Empty) + @"
                Left Join item_projeto ip On (a.idItemProjeto=ip.idItemProjeto)
                Left Join produto p on (a.idProd=p.idProd)
                Left Join etiqueta_aplicacao ea on (a.idAplicacao=ea.idAplicacao)
                Left Join etiqueta_processo ep on (a.idProcesso=ep.idProcesso)
                Left Join pedido ped on (a.idPedido=ped.idPedido)" : String.Empty) + @"
                Left Join (
                    select " + (isRelatorioPcp ? "coalesce(a.idAmbientePedidoOrig, " : "") + "pp.idAmbientePedido" + (isRelatorioPcp ? ") as idAmbientePedido" : "") + @",
                        count(*) as qtdeProd, cast(sum(pp.total+coalesce(pp.valorBenef,0)) as decimal(12,2)) as totalProdutos,
                        cast(sum(coalesce(pp.valorAcrescimoProd,0)) as decimal(12,2)) as valorAcrescimo, cast(sum(coalesce(pp.valorDescontoProd,0))
                        as decimal(12,2)) as valorDesconto, sum(pp.totM) as totM
                    from produtos_pedido" + (!isRelatorioPcp ? String.Empty : "_espelho") + @" pp
                        left join ambiente_pedido" + (!isRelatorioPcp ? String.Empty : "_espelho") + @" a on (pp.idAmbientePedido=a.idAmbientePedido and pp.idPedido=a.idPedido)
                    where coalesce(pp.invisivel" + (!isRelatorioPcp ? "Pedido" : "Fluxo") + @", false)=false AND pp.IdProdPedParent IS NULL {0}
                    group by pp.idAmbientePedido
                ) pp on (" + (isRelatorioPcp ? "coalesce(a.idAmbientePedidoOrig," : String.Empty) + "a.idAmbientePedido" + (isRelatorioPcp ? ")" : String.Empty) + @"=pp.idAmbientePedido)
                Where 1 {0}";

            string where = String.Empty;
            if (idAmbientePedido > 0)
            {
                if (!isRelatorioPcp)
                    where += " And a.idAmbientePedido=" + idAmbientePedido;
                else
                    where += " And (a.idAmbientePedidoOrig=" + idAmbientePedido + " or (a.idAmbientePedidoOrig is null and a.idAmbientePedido=" + idAmbientePedido + "))";
            }

            if (idPedido > 0)
                where += " And a.idPedido=" + idPedido;

            if (apenasAmbientesComProdutos)
                sql += " and pp.qtdeProd > 0";

            return String.Format(sql, where);
        }

        public AmbientePedido GetElement(uint idAmbientePedido)
        {
            return GetElement(null, idAmbientePedido);
        }

        public AmbientePedido GetElement(GDASession sessao, uint idAmbientePedido)
        {
            return GetElement(sessao, idAmbientePedido, 0, false);
        }

        public AmbientePedido GetElement(uint idAmbientePedido, uint idPedido, bool isRelatorioPcp)
        {
            return GetElement(null, idAmbientePedido, idPedido, isRelatorioPcp);
        }

        public AmbientePedido GetElement(GDASession sessao, uint idAmbientePedido, uint idPedido, bool isRelatorioPcp)
        {
            var item = objPersistence.LoadData(sessao, Sql(idAmbientePedido, idPedido, isRelatorioPcp, false, true)).ToList();
            return item.Count > 0 ? item[0] : null;
        }

        public IList<AmbientePedido> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        public IList<AmbientePedido> GetByPedido(GDASession sessao, uint idPedido)
        {
            return GetByPedido(sessao, idPedido, false);
        }

        public IList<AmbientePedido> GetForRecibo(uint idPedido)
        {
            return GetByPedido(null, idPedido, false);
        }

        public IList<AmbientePedido> GetForExportacao(uint idPedido, uint[] idsProdutosPedido, bool usarEspelho)
        {
            List<AmbientePedido> a = new List<AmbientePedido>(GetByPedido(null, idPedido, usarEspelho));

            if (idsProdutosPedido.Length > 0)
            {
                var idsProd = String.Empty;
                foreach (uint idProd in idsProdutosPedido)
                    idsProd += idProd + ",";

                // Busca os ids dos ambientes do pedido (buscando do PCP, se necessário)
                var idsAmbientesPedido = objPersistence.LoadResult(String.Format(
                    @"select distinct coalesce(pp{0}.idAmbientePedido,0) from produtos_pedido pp
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    where pp.idProdPed in (" + idsProd.TrimEnd(',') + ")", usarEspelho ? "e" : String.Empty), null).Select(f => f.GetUInt32(0))
                       .ToList();

                for (int i = a.Count - 1; i >= 0; i--)
                    if (!idsAmbientesPedido.Contains(a[i].IdAmbientePedido))
                        a.RemoveAt(i);
            }

            return a.ToArray();
        }

        public IList<AmbientePedido> GetByPedido(uint idPedido, bool isRelatorioPcp)
        {
            return GetByPedido(null, idPedido, isRelatorioPcp);
        }

        public IList<AmbientePedido> GetByPedido(GDASession sessao, uint idPedido, bool isRelatorioPcp)
        {
            var retorno = objPersistence.LoadData(sessao, Sql(0, idPedido, isRelatorioPcp, true, true)).ToList();

            if (!PedidoConfig.RelatorioPedido.ExibirItensProdutosPedido)
            {
                foreach (AmbientePedido r in retorno)
                    if (r.IdItemProjeto > 0)
                    {
                        string nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(sessao, r.IdItemProjeto.Value);

                        if (!String.IsNullOrEmpty(nomeFigura))
                            r.ImagemProjModPath = "file:///" + Utils.GetModelosProjetoPath.Replace("\\", "/") + nomeFigura;
                    }
            }

            return retorno;
        }

        public IList<AmbientePedido> GetForDescontoPedido(uint idPedido)
        {
            return objPersistence.LoadData(Sql(0, idPedido, false, false, true)).ToList();
        }

        public AmbientePedido GetForReposicaoPedido(uint idPedido)
        {
            string sql = "select idAmbientePedido from ambiente_pedido where idPedido=" + idPedido;
            uint idAmbientePedido = ExecuteScalar<uint>(sql);

            return idAmbientePedido > 0 ? GetElement(idAmbientePedido) : null;
        }

        public bool PossuiProdutos(uint idAmbientePedido)
        {
            return PossuiProdutos(null, idAmbientePedido);
        }

        public bool PossuiProdutos(GDASession session, uint idAmbientePedido)
        {
            string sql = "select count(*) from produtos_pedido where idAmbientePedido=" + idAmbientePedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public IList<AmbientePedido> GetList(uint idPedido, string sortExpression, int startRow, int pageSize)
        {
            if (CountInPedido(idPedido) == 0)
            {
                List<AmbientePedido> lst = new List<AmbientePedido>();
                lst.Add(new AmbientePedido());
                return lst.ToArray();
            }

            string filtro = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "a.idAmbientePedido";

            return LoadDataWithSortExpression(Sql(0, idPedido, false, false, true), filtro, startRow, pageSize, null);
        }

        public int GetCount(uint idPedido)
        {
            int count = CountInPedido(idPedido);
            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Retorna a quantidade de ambientes relacionados ao pedido passado
        /// </summary>
        public int CountInPedido(uint idPedido)
        {
            return idPedido == 0 ? 0 : objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido, false, false, false), null);
        }

        public uint? GetKeyByPedidoDescr(uint idPedido, string descricao)
        {
            string sql = "select coalesce(idAmbientePedido, 0) from ambiente_pedido where idPedido=" + idPedido +
                " and descricao LIKE ?descr";

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

            string sql = "select coalesce(qtde, 1) from ambiente_pedido where idAmbientePedido=" + idAmbientePedido;
            return ExecuteScalar<int>(sessao, sql);
        }

        public List<uint> GetIdsByPedido(uint idPedido)
        {
            return GetIdsByPedido(null, idPedido);
        }

        public List<uint> GetIdsByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadResult(
                sessao,
                "select idAmbientePedido from ambiente_pedido where idPedido=" + idPedido, null)
                .Select(f => f.GetUInt32(0))
                .ToList();
        }

        #region Retorna o IdAmbiente a partir do item projeto

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido espelho
        /// </summary>
        public uint ObtemIdAmbiente(uint idItemProjeto)
        {
            return ObtemIdAmbiente(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido espelho
        /// </summary>
        public uint ObtemIdAmbiente(GDASession session, uint idItemProjeto)
        {
            string sql = "Select Coalesce(idAmbientePedido, 0) From ambiente_pedido Where idItemProjeto=" +
                idItemProjeto;

            return ExecuteScalar<uint>(session, sql);
        }

        #endregion

        #region Retorna o IdItemProjeto do ambiente do pedido

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido
        /// </summary>
        public uint ObtemItemProjeto(uint idAmbientePedido)
        {
            return ObtemItemProjeto(null, idAmbientePedido);
        }

        /// <summary>
        /// Retorna o IdItemProjeto do ambiente do pedido
        /// </summary>
        public uint ObtemItemProjeto(GDASession session, uint idAmbientePedido)
        {
            string sql = "Select Coalesce(idItemProjeto, 0) From ambiente_pedido Where idAmbientePedido=" + idAmbientePedido;
            return ExecuteScalar<uint>(session, sql);
        }

        #endregion

        #region Exclui ambientes do pedido

        /// <summary>
        /// Exclui todos os ambientes de um pedido
        /// </summary>
        public void DeleteByPedido(uint idPedido)
        {
            using (var sessao = new GDASession())
            {
                objPersistence.ExecuteCommand(sessao,
                    "DELETE FROM ambiente_pedido_rentabilidade WHERE IdAmbientePedido IN (SELECT IdAmbientePedido FROM ambiente_pedido WHERE IdPedido=" + idPedido + ")");
                objPersistence.ExecuteCommand(sessao, "Delete From ambiente_pedido Where idPedido=" + idPedido);
            }
        }

        #endregion

        #region Acréscimo e Desconto

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarAcrescimo(GDASession sessao, Pedido pedido, uint idAmbientePedido, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.AplicarAcrescimoAmbiente(
                sessao,
                pedido,
                tipoAcrescimo,
                acrescimo,
                produtosPedido
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverAcrescimo(GDASession sessao, Pedido pedido, uint idAmbientePedido,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimoAmbiente(sessao, pedido, produtosPedido);
        }

        #endregion

        #endregion

        #region Desconto

        /// <summary>
        /// Verifica se algum ambiente do pedido passado possui desconto.
        /// </summary>
        public bool PossuiDesconto(uint idPedido)
        {
            string sql = "Select Count(*) From ambiente_pedido Where idPedido=" + idPedido + " And desconto>0";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se o desconto dado é permitido
        /// </summary>
        public bool ValidaDesconto(AmbientePedido ambientePedido, out string msg)
        {
            return ValidaDesconto(null, ambientePedido, out msg);
        }

        /// <summary>
        /// Verifica se o desconto dado é permitido
        /// </summary>
        public bool ValidaDesconto(GDASession sessao, AmbientePedido ambientePedido, out string msg)
        {
            var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(sessao, ambientePedido.IdPedido);

            //Verificar se o pedido possui o tipo Venda antes de validar o desconto
            if (tipoVenda == 0 && ambientePedido.Desconto > 0)
            {
                msg = "Informe o tipo venda do pedido antes de aplicar desconto";
                return false;
            }

            // Se permitir desconto apenas em pedidos à vista e o pedido deste ambiente não for à vista, bloqueia
            if (PedidoConfig.Desconto.DescontoPedidoApenasAVista && PedidoDAO.Instance.ObtemTipoVenda(sessao, ambientePedido.IdPedido) != (int)Pedido.TipoVendaPedido.AVista
                && ambientePedido.Desconto > 0)
            {
                msg = "Não é permitido lançar desconto em pedidos que não sejam à vista.";
                return false;
            }

            // Obtém o percentual de desconto aplicado no ambiente
            decimal percDescAmbiente = 0;
            decimal totalBrutoAmbiente = GetTotalBruto(sessao, ambientePedido.IdAmbientePedido);

            if (totalBrutoAmbiente == 0)
            {
                msg = String.Empty;
                return true;
            }

            if (ambientePedido.TipoDesconto == 1)
                percDescAmbiente = ambientePedido.Desconto;
            else
                percDescAmbiente = (ambientePedido.Desconto / totalBrutoAmbiente) * 100;

            var descontoMaximoPedido = Conversoes.StrParaDecimal(PedidoConfig.Desconto.GetDescontoMaximoPedido(UserInfo.GetUserInfo.CodUser,
                tipoVenda, (int?)PedidoDAO.Instance.ObtemIdParcela(null, ambientePedido.IdPedido)).ToString());

            // Verifica se o desconto lançado é maior que o máximo configurado no pedido
            if (percDescAmbiente > descontoMaximoPedido)
            {
                msg = "Não é permitido lançar desconto maior que " + descontoMaximoPedido + "% no pedido.";
                return false;
            }

            // Se houver bloqueio de desconto somativo, verifica se o cliente já possui desconto por grupo/subgrupo, se houver,
            // verifica se o ambiente é de projeto, apenas vidros e só possui um tipo de vidro nesta situação verifica quanto de desconto
            // o cliente possui neste produto e verifica qual é o máximo que pode ser aplicado para este produto de desconto.
            if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                ambientePedido.Desconto > 0)
            {
                if (PedidoDAO.Instance.ObterDesconto(sessao, (int)ambientePedido.IdPedido) > 0)
                {
                    msg = "O pedido já possui desconto, não é permitido lançar mais um desconto por ambiente.";
                    return false;
                }

                if (DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(sessao, PedidoDAO.Instance.ObtemIdCliente(sessao, ambientePedido.IdPedido), 0, null,
                    ambientePedido.IdPedido, ambientePedido.IdAmbientePedido))
                {
                    // Mesmo que tenha desconto por grupo/subgrupo, permite lançar desconto no ambiente considerando que
                    // o desconto por grupo/subgrupo + o desconto dado no vidro presente no projeto seja menor ou igual
                    // ao desconto máximo configurado para este produto
                    //uint idItemProjeto = ObtemItemProjeto(ambientePedido.IdAmbientePedido);
                    //if (idItemProjeto > 0 && ItemProjetoDAO.Instance.ApenasVidros(idItemProjeto))
                    //{
                    //    List<MaterialItemProjeto> lstMaterial = MaterialItemProjetoDAO.Instance.GetByItemProjeto(idItemProjeto);
                    //    uint idProd = lstMaterial[0].IdProd;

                    //    // Verifica se todas as peças de vidro é do mesmo vidro, se não for não permite lançar desconto por ambiente
                    //    foreach (MaterialItemProjeto mip in lstMaterial)
                    //        if (mip.IdProd != idProd)
                    //        {
                    //            msg = "O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.";
                    //            return false;
                    //        }

                    //    // Obtém o percentual de desconto que o cliente possui por ambiente, por grupo/subgrupo e o máximo por produto,
                    //    // para verificar se o desconto é permitido
                    //    decimal percDescGrupoSubgrupo = (decimal)DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                    //        PedidoDAO.Instance.ObtemIdCliente(ambientePedido.IdPedido), ProdutoDAO.Instance.ObtemIdGrupoProd(idProd),
                    //        ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd), idProd).Desconto;
                    //    decimal percDescMaxProduto = (decimal)DescontoQtdeDAO.Instance.GetPercDescontoByProd(idProd, 1);

                    //    // Recalcula o percentual lançado no ambiente considerando o desconto por grupo/subgrupo
                    //    if (ambientePedido.TipoDesconto != 1)
                    //        percDescAmbiente = (ambientePedido.Desconto / (totalBrutoAmbiente - (totalBrutoAmbiente * (percDescGrupoSubgrupo / 100)))) * 100;

                    //    if (percDescMaxProduto - percDescGrupoSubgrupo <= 0)
                    //    {
                    //        msg = "O percentual de desconto máximo permitido neste ambiente foi excedido com o desconto por" +
                    //            " grupo/subgrupo configurado para este cliente juntamente com o desconto aplicado no ambiente.";
                    //        return false;
                    //    }

                    //    if (percDescGrupoSubgrupo + percDescAmbiente > percDescMaxProduto)
                    //    {
                    //        msg = "O percentual de desconto máximo permitido neste ambiente é de " +
                    //            (percDescMaxProduto - percDescGrupoSubgrupo) + "%, este valor foi excedido com o desconto por grupo/subgrupo " +
                    //            "configurado para este cliente juntamente com o desconto aplicado no ambiente.";
                    //        return false;
                    //    }
                    //}
                    //else
                    {
                        msg = "O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.";
                        return false;
                    }
                }
            }

            msg = String.Empty;

            return true;
        }

        #region Aplica o desconto

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarDesconto(GDASession sessao, Pedido pedido, uint idAmbientePedido, int tipoDesconto, decimal desconto,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.AplicarDescontoAmbiente(
                sessao,
                pedido,
                tipoDesconto,
                desconto,
                produtosPedido
            );
        }

        #endregion

        #region Remove o desconto

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverDesconto(GDASession sessao, Pedido pedido, uint idAmbientePedido,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.RemoverDescontoAmbiente(sessao, pedido, produtosPedido);
        }

        #endregion

        #endregion

        #region Finalização

        internal void FinalizarAplicacaoAcrescimoDesconto(GDASession sessao, Pedido pedido,
            IEnumerable<ProdutosPedido> produtosPedido, bool atualizar)
        {
            if (atualizar)
            {
                foreach (var prod in produtosPedido)
                {
                    ProdutosPedidoDAO.Instance.UpdateBase(sessao, prod, pedido);
                    ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, prod.IdProdPed, prod.Beneficiamentos, pedido);
                }
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
        public uint? GetIdByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "select idAmbientePedido from ambiente_pedido where idItemProjeto=" + idItemProjeto;
            return ExecuteScalar<uint?>(sessao, sql);
        }

        #endregion

        #region Verifica se o ambiente é redondo

        /// <summary>
        /// Verifica se o ambiente é redondo
        /// </summary>
        public bool IsRedondo(uint idAmbientePedido)
        {
            return IsRedondo(null, idAmbientePedido);
        }

        /// <summary>
        /// Verifica se o ambiente é redondo
        /// </summary>
        public bool IsRedondo(GDASession sessao, uint idAmbientePedido)
        {
            string sql = "Select Count(*) From ambiente_pedido Where redondo=true And idAmbientePedido=" + idAmbientePedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Obtem dados do ambiente

        public string ObtemPecaVidro(uint idAmbientePedido)
        {
            string where = "idAmbientePedido=" + idAmbientePedido;
            AmbientePedido amb = new AmbientePedido();

            amb.Ambiente = ObtemValorCampo<string>("ambiente", where);
            amb.Redondo = ObtemValorCampo<bool>("redondo", where);
            amb.Altura = ObtemValorCampo<int?>("altura", where);
            amb.Largura = ObtemValorCampo<int?>("largura", where);

            return amb.PecaVidro;
        }

        public string ObtemPecaVidroQtd(uint idAmbientePedido)
        {
            string where = "idAmbientePedido=" + idAmbientePedido;
            AmbientePedido amb = new AmbientePedido();

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
            return GetTotalBruto(null, idAmbientePedido);
        }

        internal decimal GetTotalBruto(GDASession sessao, uint idAmbientePedido)
        {
            string sql = String.Format(@"
                select sum(valor) from (
                    select sum(totalBruto) as valor
                    from produtos_pedido
                    where {0}
                    union all select sum(valor - valorAcrescimo - valorAcrescimoProd -
                        valorComissao + valorDesconto + valorDescontoProd) as valor
                    from produto_pedido_benef
                    where idProdPed in (select * from (
                        select idProdPed from produtos_pedido where {0}
                    ) as temp1)
                ) as temp", "coalesce(invisivelPedido, false)=false and idAmbientePedido=" + idAmbientePedido);

            return ExecuteScalar<decimal>(sessao, sql);
        }

        /// <summary>
        /// Obtém o acréscimo do ambiente do pedido.
        /// </summary>
        public decimal ObterAcrescimo(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<decimal>(session, "Acrescimo", "IdAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Obtém o tipo de acréscimo do ambiente do pedido.
        /// </summary>
        public int ObterTipoAcrescimo(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<int>(session, "TipoAcrescimo", "IdAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Obtém o desconto do ambiente do pedido.
        /// </summary>
        public decimal ObterDesconto(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<decimal>(session, "Desconto", "IdAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Obtém o tipo de desconto do ambiente do pedido.
        /// </summary>
        public int ObterTipoDesconto(GDASession session, uint idAmbientePedido)
        {
            return ObtemValorCampo<int>(session, "TipoDesconto", "IdAmbientePedido=" + idAmbientePedido);
        }

        /// <summary>
        /// Retorna a altura do ambiente do pedido espelho
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public float ObtemAltura(uint idAmbientePedido)
        {
            var sql = "Select Coalesce(altura, 0) From ambiente_pedido Where idAmbientePedido=" +
                idAmbientePedido;

            return ExecuteScalar<float>(sql);
        }

        /// <summary>
        /// Retorna a largura do ambiente do pedido espelho
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public float ObtemLargura(uint idAmbientePedido)
        {
            var sql = "Select Coalesce(largura, 0) From ambiente_pedido Where idAmbientePedido=" +
                idAmbientePedido;

            return ExecuteScalar<float>(sql);
        }

        /// <summary>
        /// Recupera a descrição do ambiente.
        /// </summary>
        public string ObterDescricaoAmbiente(GDASession sessao, uint idAmbientePedido)
        {
            return ObtemValorCampo<string>(sessao, "ambiente", "idAmbientePedido=" + idAmbientePedido);
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Recupera os ambientes do pedido para a rentabilidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<AmbientePedido> ObterAmbientesParaRentabilidade(GDA.GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM ambiente_pedido WHERE IdPedido=?id", new GDAParameter("?id", idPedido)).ToList();
        }

        /// <summary>
        /// Atualiza a rentabilidade do ambiente do pedido..
        /// </summary>
        /// <param name="idAmbientePedido"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            uint idAmbientePedido, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE ambiente_pedido SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdAmbientePedido=?id",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?id", idAmbientePedido));
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(AmbientePedido objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession sessao, AmbientePedido objUpdate)
        {
            string msg;
            if (!ValidaDesconto(sessao, objUpdate, out msg))
                throw new Exception(msg);

            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, objUpdate.IdPedido);

            objUpdate.IdItemProjeto = ObtemValorCampo<uint?>(sessao, "idItemProjeto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            int? qtdAmbienteAntiga = ObtemValorCampo<int?>(sessao, "qtde", "idAmbientePedido=" + objUpdate.IdAmbientePedido);

            var tipoAcrescimo = ObtemValorCampo<int>(sessao, "tipoAcrescimo", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            var acrescimo = ObtemValorCampo<decimal>(sessao, "acrescimo", "idAmbientePedido=" + objUpdate.IdAmbientePedido);

            var tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);
            var desconto = ObtemValorCampo<decimal>(sessao, "desconto", "idAmbientePedido=" + objUpdate.IdAmbientePedido);

            LogAlteracaoDAO.Instance.LogAmbientePedido(sessao, objUpdate);

            if (objUpdate.Qtde == null)
                objUpdate.Qtde = qtdAmbienteAntiga;

            int retorno = base.Update(sessao, objUpdate);

            try
            {
                var produtosPedido = ProdutosPedidoDAO.Instance.GetByAmbiente(sessao, objUpdate.IdAmbientePedido);

                foreach (ProdutosPedido pp in produtosPedido)
                {
                    var valorUnitario = ValorUnitario.Instance.RecalcularValor(sessao, pedido, pp);
                    pp.ValorVendido = valorUnitario ?? pp.ValorVendido;

                    ValorTotal.Instance.Calcular(
                        sessao,
                        pedido,
                        pp,
                        Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.NaoArredondar,
                        pp.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !(pedido as IContainerCalculo).IsPedidoProducaoCorte,
                        pp.Beneficiamentos.CountAreaMinimaSession(sessao)
                    );
                }

                RemoverAcrescimo(sessao, pedido, objUpdate.IdAmbientePedido, produtosPedido);
                RemoverDesconto(sessao, pedido, objUpdate.IdAmbientePedido, produtosPedido);

                if (pedido.MaoDeObra)
                {
                    // Mantém a quantidade salva apenas se for mão de obra, porque se for projeto, deve manter a quantidade preenchida
                    // no método ProdutosPedidoDAO.InsereAtualizaProdProj()
                    if (objUpdate.Qtde.GetValueOrDefault(0) == 0)
                        objUpdate.Qtde = qtdAmbienteAntiga;

                    objPersistence.ExecuteCommand(sessao, "update produtos_pedido set altura=" + objUpdate.Altura.Value +
                        ", largura=" + objUpdate.Largura.Value + " where idAmbientePedido=" + objUpdate.IdAmbientePedido +
                        " and (altura<>0 or largura<>0)");

                    // Atualiza todos os produtos deste ambiente, caso a quantidade/m² do ambiente tenha sido alterada é necessário recalcular
                    // os valores dos produtos associados ao mesmo
                    foreach (ProdutosPedido pp in produtosPedido)
                        ProdutosPedidoDAO.Instance.Update(sessao, pp, pedido);
                }

                bool aplicado = false;

                if (AplicarAcrescimo(sessao, pedido, objUpdate.IdAmbientePedido, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, produtosPedido))
                    aplicado = true;

                if (AplicarDesconto(sessao, pedido, objUpdate.IdAmbientePedido, objUpdate.TipoDesconto, objUpdate.Desconto, produtosPedido))
                    aplicado = true;

                FinalizarAplicacaoAcrescimoDesconto(sessao, pedido, produtosPedido, aplicado);

                PedidoDAO.Instance.UpdateTotalPedido(sessao, pedido);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("UpdateAmbientePedido. Pedido: " + objUpdate.IdPedido +
                    " Ambiente: " + objUpdate.IdAmbientePedido, ex);

                throw ex;
            }

            return retorno;
        }

        public int DeleteComTransacao(AmbientePedido objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = DeleteByPrimaryKey(transaction, objDelete.IdAmbientePedido);

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

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        private static object ApagarAmbientePedidoLock = new object();

        public override int DeleteByPrimaryKey(GDASession session, uint Key)
        {
            lock (ApagarAmbientePedidoLock)
            {
                var idItemProjeto = ObtemItemProjeto(session, Key);

                var idsProdPed = string.Join(",",
                    ExecuteMultipleScalar<string>(session, "Select idProdPed From produtos_pedido Where idAmbientePedido=" + Key).ToArray());

                if (string.IsNullOrEmpty(idsProdPed))
                    idsProdPed = "0";

                if (idItemProjeto > 0)
                {
                    // Exclui os dados relacionados com projeto deste ambiente
                    ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, idItemProjeto);

                    // Exclui os produtos deste ambiente
                    objPersistence.ExecuteCommand(session, @"
                        Delete From produto_pedido_benef Where idProdPed In (" + idsProdPed + @");
                        DELETE FROM produto_pedido_rentabilidade WHERE IdProdPed IN (" + idsProdPed + @");
                        Delete From produtos_pedido Where idAmbientePedido=" + Key);
                }
                else
                {
                    if (PossuiProdutos(session, Key))
                        throw new Exception("Esse ambiente possui produtos. Exclua-os antes de excluir o ambiente.");

                    // Exclui os produtos deste ambiente
                    objPersistence.ExecuteCommand(session, @"
                        Delete From produto_pedido_benef Where idProdPed In (" + idsProdPed + @");
                        DELETE FROM produto_pedido_rentabilidade WHERE IdProdPed IN (" + idsProdPed + @");
                        Delete From produtos_pedido Where idAmbientePedido=" + Key);
                }

                var idPedido = ObtemValorCampo<uint>(session, "idPedido", "idAmbientePedido=" + Key);

                var retorno = base.DeleteByPrimaryKey(session, Key);

                PedidoDAO.Instance.UpdateTotalPedido(session, idPedido);

                // Atualiza a data de entrega do pedido para considerar o número de dias mínimo de entrega do subgrupo ao informar o produto.
                bool enviarMensagem;
                PedidoDAO.Instance.RecalcularEAtualizarDataEntregaPedido(session, idPedido, null, out enviarMensagem, false);

                return retorno;
            }
        }

        #endregion
    }
}
