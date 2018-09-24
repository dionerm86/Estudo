using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    #region Classe retorno aplicativo cálculo projeto

    /// <summary>
    /// Classe de retorno da criação de peças e materiais do projeto para o Aplicativo de cálculo de projetos.
    /// </summary>
    public class PecasMateriaisProjeto
    {
        #region Construtores

        public PecasMateriaisProjeto()
        {
        }

        public PecasMateriaisProjeto(IItemProjeto itemProjeto, List<PecaProjetoModelo> pecasProjetoModelo, IEnumerable<IPecaItemProjeto> pecasItemProjeto,
            IEnumerable<IMaterialItemProjeto> materiaisItemProjeto)
        {
            ItemProjeto = itemProjeto;
            PecasProjetoModelo = pecasProjetoModelo;
            PecasItemProjeto = pecasItemProjeto;
            MateriaisItemProjeto = materiaisItemProjeto;
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Item de projeto.
        /// </summary>
        public IItemProjeto ItemProjeto { get; set; }

        /// <summary>
        /// Peças do modelo de projeto.
        /// </summary>
        public List<PecaProjetoModelo> PecasProjetoModelo { get; set; }

        /// <summary>
        /// Peças do item de projeto.
        /// </summary>
        public IEnumerable<IPecaItemProjeto> PecasItemProjeto { get; set; }

        /// <summary>
        /// Materiais do item de projeto.
        /// </summary>
        public IEnumerable<IMaterialItemProjeto> MateriaisItemProjeto { get; set; }

        #endregion
    }

    #endregion

    public sealed class ItemProjetoDAO : BaseDAO<ItemProjeto, ItemProjetoDAO>
    {
        //private ItemProjetoDAO() { }

        #region Listagem Padrão

        private string Sql(uint idItemProjeto, string idsItensProjetos, uint idProjeto, uint idOrcamento, uint idPedido, uint idPedidoEspelho,
            bool selecionar, bool buscarProjetoVazio, bool apenasVidros, bool apenasCalculadosPorVao, out string filtroAdicional)
        {
            filtroAdicional = "";

            string campos = selecionar ? @"ip.*, pm.codigo as codigoModelo, pm.descricao as DescrModelo, pm.TextoOrcamento,
                cv.Descricao as DescrCorVidro, ca.Descricao as DescrCorAluminio, cf.Descricao as DescrCorFerragem,
                ped.dataEntrega as dataEntregaPedido, pedEsp.dataFabrica as dataFabricaPedido" : "Count(*)";

            StringBuilder str = new StringBuilder();

            str.Append("Select " + campos + @" From item_projeto ip
                Left Join pedido ped on (coalesce(ip.idPedido, ip.idPedidoEspelho)=ped.idPedido)
                Left Join pedido_espelho pedEsp on (ped.idPedido=pedEsp.idPedido)
                Left Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                Left Join cor_vidro cv On (ip.idCorVidro=cv.idCorVidro)
                Left Join cor_aluminio ca On (ip.idCorAluminio=ca.idCorAluminio)
                Left Join cor_ferragem cf On (ip.idCorFerragem=cf.idCorFerragem)
                Where 1 ?filtroAdicional?");

            if (idItemProjeto > 0)
                filtroAdicional += " And ip.idItemProjeto=" + idItemProjeto;
            else if (!String.IsNullOrEmpty(idsItensProjetos))
                filtroAdicional += " And ip.idItemProjeto in (" + idsItensProjetos + ")";

            if (idProjeto > 0 || buscarProjetoVazio)
                filtroAdicional += " And ip.idProjeto=" + idProjeto;
            else if (idOrcamento > 0)
                filtroAdicional += " And ip.idOrcamento=" + idOrcamento;
            else if (idPedido > 0)
                filtroAdicional += " And ip.idPedido=" + idPedido;
            else if (idPedidoEspelho > 0)
                filtroAdicional += " And ip.idPedidoEspelho=" + idPedidoEspelho;

            if (apenasVidros)
                filtroAdicional += " And apenasVidros=true";

            if (apenasCalculadosPorVao)
                filtroAdicional += " And coalesce(medidaExata, false)=false";

            return str.ToString();
        }

        private void AtualizaPodeEditar(ref ItemProjeto ip)
        {
            AtualizaPodeEditar(null, ref ip);
        }

        private void AtualizaPodeEditar(GDASession sessao, ref ItemProjeto ip)
        {
            ItemProjeto[] temp = new ItemProjeto[] { ip };
            AtualizaPodeEditar(sessao, ref temp);
            ip = temp[0];
        }

        private void AtualizaPodeEditar(ref ItemProjeto[] ip)
        {
            AtualizaPodeEditar(null, ref ip);
        }

        private void AtualizaPodeEditar(GDASession sessao, ref ItemProjeto[] ip)
        {
            if (ip == null || ip.Length == 0)
                return;

            string ids = String.Empty;
            foreach (ItemProjeto i in ip)
                ids += i != null ? i.IdItemProjeto + "," : "";

            if (String.IsNullOrEmpty(ids))
                return;

            // Recupera os idsProdPed dos materiais
            string sql = @"
                select ppe.idProdPed
	            from produtos_pedido_espelho ppe
		            inner join material_item_projeto mip on (ppe.idMaterItemProj=mip.idMaterItemProj)
	            where coalesce(ppe.invisivelFluxo,false)=false and mip.idItemProjeto in ({0})";

            ids = GetValoresCampo(sessao, String.Format(sql, ids.TrimEnd(',')), "idProdPed");

            if (!String.IsNullOrEmpty(ids))
            {
                // Recupera os idsProdPed que estão impressos
                sql = @"
                    select pi.idProdPed
                    from produto_impressao pi
                        left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                    where coalesce(ie.situacao, 0)={0} and !coalesce(pi.cancelado,false)
                        and pi.idProdPed in ({1})";

                ids = GetValoresCampo(sessao, String.Format(sql, (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa,
                    ids.TrimEnd(',')), "idProdPed");

                if (!String.IsNullOrEmpty(ids))
                {
                    // Recupera os idsItemProjeto que possuem produtos impressos
                    sql = @"
                        select mip.idItemProjeto
                        from produtos_pedido_espelho ppe
                            inner join material_item_projeto mip on (ppe.idMaterItemProj=mip.idMaterItemProj)
                        where coalesce(ppe.invisivelFluxo,false)=false and ppe.idProdPed in ({0})";

                    ids = GetValoresCampo(sessao, String.Format(sql, ids.TrimEnd(',')), "idItemProjeto");
                }
            }

            List<string> id = new List<string>(!String.IsNullOrEmpty(ids) ? ids.Split(',') : new string[] { });
            foreach (ItemProjeto i in ip)
                i.EditDeleteVisible = !id.Contains(i.IdItemProjeto.ToString());
        }

        public ItemProjeto GetElement(uint idItemProjeto)
        {
            return GetElement(null, idItemProjeto);
        }

        public ItemProjeto GetElement(GDASession sessao, uint idItemProjeto)
        {
            if (!Exists(sessao, idItemProjeto))
                return null;

            string filtroAdicional;
            string sql = Sql(idItemProjeto, null, 0, 0, 0, 0, true, false, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            ItemProjeto ip = objPersistence.LoadOneData(sessao, sql);
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        public ItemProjeto[] GetByString(string idsItensProjetos)
        {
            return GetByString(null, idsItensProjetos);
        }

        public ItemProjeto[] GetByString(GDASession sessao, string idsItensProjetos)
        {
            string filtroAdicional;
            string sql = Sql(0, idsItensProjetos, 0, 0, 0, 0, true, false, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(sessao, sql).ToList().ToArray();
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        public IList<ItemProjeto> GetByOrcamento(uint idOrcamento)
        {
            return GetByOrcamento(null, idOrcamento);
        }

        public IList<ItemProjeto> GetByOrcamento(GDASession sessao, uint idOrcamento)
        {
            string filtroAdicional;
            string sql = Sql(0, null, 0, idOrcamento, 0, 0, true, false, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(sessao, sql).ToList().ToArray();
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        public IList<ItemProjeto> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        public IList<ItemProjeto> GetByPedido(GDASession session, uint idPedido)
        {
            string filtroAdicional;
            var sql = Sql(0, null, 0, 0, idPedido, 0, true, false, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(session, sql).ToList().ToArray();
            AtualizaPodeEditar(session, ref ip);

            return ip;
        }

        public IList<ItemProjeto> GetByPedidoEspelho(uint idPedidoEspelho)
        {
            return GetByPedidoEspelho(null, idPedidoEspelho);
        }

        public IList<ItemProjeto> GetByPedidoEspelho(GDASession sessao, uint idPedidoEspelho)
        {
            string filtroAdicional;
            string sql = Sql(0, null, 0, 0, 0, idPedidoEspelho, true, false, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(sessao, sql).ToList().ToArray();
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        public IList<ItemProjeto> GetForGerarOrcamento(uint idPedidoEspelho)
        {
            return GetForGerarOrcamento(null, idPedidoEspelho);
        }

        public IList<ItemProjeto> GetForGerarOrcamento(GDASession sessao, uint idPedidoEspelho)
        {
            string filtroAdicional;
            string sql = Sql(0, null, 0, 0, 0, idPedidoEspelho, true, false, true, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(sessao, sql).ToList().ToArray();
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        public IList<ItemProjeto> GetByProjeto(uint idProjeto)
        {
            return GetByProjeto(null, idProjeto);
        }

        public IList<ItemProjeto> GetByProjeto(GDASession sessao, uint idProjeto)
        {
            return GetByProjeto(sessao, idProjeto, false);
        }

        public IList<ItemProjeto> GetByProjeto(uint idProjeto, bool buscarProjetoVazio)
        {
            return GetByProjeto(null, idProjeto, buscarProjetoVazio);
        }

        public IList<ItemProjeto> GetByProjeto(GDASession sessao, uint idProjeto, bool buscarProjetoVazio)
        {
            string filtroAdicional;
            string sql = Sql(0, null, idProjeto, 0, 0, 0, true, buscarProjetoVazio, false, false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var ip = objPersistence.LoadData(sessao, sql).ToList().ToArray();
            AtualizaPodeEditar(sessao, ref ip);

            return ip;
        }

        /// <summary>
        /// Listagem na tela CadProjeto.aspx
        /// </summary>
        public IList<ItemProjeto> GetList(uint idProjeto, string sortExpression, int startRow, int pageSize)
        {
            if (idProjeto == 0)
                return new ItemProjeto[0];

            string filtroAdicional;
            string sql = Sql(0, null, idProjeto, 0, 0, 0, true, false, false, false, out filtroAdicional);

            var ip = ((List<ItemProjeto>)LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, null)).ToArray();
            AtualizaPodeEditar(ref ip);

            return ip;
        }

        public int GetCount(uint idProjeto)
        {
            if (idProjeto == 0)
                return 0;

            string filtroAdicional;
            string sql = Sql(0, null, idProjeto, 0, 0, 0, true, false, false, false, out filtroAdicional);

            return GetCountWithInfoPaging(sql, false, filtroAdicional, null);
        }

        /// <summary>
        /// Listagem na tela de cadastro de projeto avulso
        /// </summary>
        public ItemProjeto[] GetListAvulso(uint idOrcamento, uint idPedido, uint idPedidoEspelho, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(0, null, 0, idOrcamento, idPedido, idPedidoEspelho, true, false, false, false, out filtroAdicional);

            var ip = ((List<ItemProjeto>)LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, null)).ToArray();
            AtualizaPodeEditar(ref ip);

            return ip;
        }

        public int GetCountAvulso(uint idOrcamento, uint idPedido, uint idPedidoEspelho)
        {
            string filtroAdicional;
            string sql = Sql(0, null, 0, idOrcamento, idPedido, idPedidoEspelho, true, false, false, false, out filtroAdicional);

            return GetCountWithInfoPaging(sql, false, filtroAdicional, null);
        }

        #endregion

        #region Busca para gerar pedido

        public List<ItemProjeto> GetForPedido(uint idProjeto)
        {
            return GetForPedido(null, idProjeto);
        }

        public List<ItemProjeto> GetForPedido(GDASession session, uint idProjeto)
        {
            string sql = @"Select ip.*, pm.codigo as codigoModelo, pm.textoOrcamento, cv.Descricao as DescrCorVidro,
                ca.Descricao as DescrCorAluminio, cf.Descricao as DescrCorFerragem
                From item_projeto ip Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                Left Join cor_vidro cv On (ip.idCorVidro=cv.idCorVidro)
                Left Join cor_aluminio ca On (ip.idCorAluminio=ca.idCorAluminio)
                Left Join cor_ferragem cf On (ip.idCorFerragem=cf.idCorFerragem)
                Where ip.idProjeto=" + idProjeto + " Order By idItemProjeto";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca para gerar orçamento

        public List<ItemProjeto> GetForOrcamento(uint idProjeto)
        {
            return GetForOrcamento(null, idProjeto);
        }

        public List<ItemProjeto> GetForOrcamento(GDASession session, uint idProjeto)
        {
            string sql = "Select ip.*, pm.codigo as codigoModelo, pm.textoOrcamento, cv.Descricao as DescrCorVidro, " +
                "ca.Descricao as DescrCorAluminio, cf.Descricao as DescrCorFerragem " +
                "From item_projeto ip Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo) " +
                "Left Join cor_vidro cv On (ip.idCorVidro=cv.idCorVidro) " +
                "Left Join cor_aluminio ca On (ip.idCorAluminio=ca.idCorAluminio) " +
                "Left Join cor_ferragem cf On (ip.idCorFerragem=cf.idCorFerragem) " +
                "Where ip.idProjeto=" + idProjeto;

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca para impressão

        /// <summary>
        /// Busca itens de projeto para serem selecionados para impressão
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="selecionar"></param>
        /// <returns></returns>
        private string SqlImpr(uint idProjeto, uint idPedido, uint idOrcamento, string pcp, bool selecionar)
        {
            string campos = selecionar ? @"ip.*, pm.codigo as codigoModelo, cv.Descricao as descrCorVidro,
                ca.Descricao as descrCorAluminio, cf.Descricao as descrCorFerragem, ape.idAmbientePedido as idAmbientePedidoEspelho" : "Count(*)";

            string sql = "Select " + campos + @" From item_projeto ip
                Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                Left Join cor_vidro cv On (ip.idCorVidro=cv.idCorVidro)
                Left Join cor_aluminio ca On (ip.idCorAluminio=ca.idCorAluminio)
                Left Join cor_ferragem cf On (ip.idCorFerragem=cf.idCorFerragem)
                Left Join ambiente_pedido_espelho ape On (ip.idItemProjeto=ape.idItemProjeto)
                Where 1 ";

            if (idProjeto > 0)
                sql += " And ip.idProjeto=" + idProjeto;

            if (idPedido > 0)
                sql += " And ip.idPedido" + (pcp == "1" ? "Espelho" : "") + "=" + idPedido;

            if (idOrcamento > 0)
                sql += " and ip.idOrcamento=" + idOrcamento;

            return sql;
        }

        public IList<ItemProjeto> GetListImpr(uint idProjeto, uint idPedido, uint idOrcamento, string pcp, string sortExpression,
            int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "Ambiente";
            return LoadDataWithSortExpression(SqlImpr(idProjeto, idPedido, idOrcamento, pcp, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountImpr(uint idProjeto, uint idPedido, uint idOrcamento, string pcp)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlImpr(idProjeto, idPedido, idOrcamento, pcp, false), null);
        }

        #endregion

        #region Busca GrupoModelo do ProjetoModelo do ItemProjeto

        /// <summary>
        /// Busca GrupoModelo do ProjetoModelo do item passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdGrupoModelo(uint idItemProjeto)
        {
            string sql = "Select Coalesce(pm.idGrupoModelo, 0) From item_projeto ip " +
                "Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo) " +
                "Where ip.idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sql).ToString());
        }

        #endregion

        #region Busca idProjetoModelo do ProjetoModelo do ItemProjeto

        /// <summary>
        /// Busca o idProjetoModelo do ProjetoModelo do item passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdProjetoModelo(uint idItemProjeto)
        {
            return GetIdProjetoModelo(null, idItemProjeto);
        }

        /// <summary>
        /// Busca o idProjetoModelo do ProjetoModelo do item passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdProjetoModelo(GDASession session, uint idItemProjeto)
        {
            string sql = "Select idProjetoModelo From item_projeto Where idItemProjeto=" + idItemProjeto;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sql).ToString()) : 0;
        }

        #endregion

        #region Cria as peças e materiais do projeto

        /// <summary>
        /// Método utilizado para preencher o produto vidro.
        /// </summary>
        public void PreencherProdutoVidro(GDASession session, IItemProjeto itemProjeto, ProjetoModelo projetoModelo, List<PecaProjetoModelo> pecasProjetoModelo)
        {
            if (pecasProjetoModelo == null || pecasProjetoModelo.Count == 0)
                return;

            var isBoxPadrao = ProjetoModeloDAO.Instance.IsBoxPadrao(session, projetoModelo.IdProjetoModelo);

            // Percorre cada peça do modelo de projeto e, com base na cor do vidro selecionada no item de projeto, recupera o produto.
            foreach (var pecaProjetoModelo in pecasProjetoModelo.Where(f => f.IdProd == 0))
                pecaProjetoModelo.IdProd = ProdutoProjetoConfigDAO.Instance.GetIdProdVidro(session, pecaProjetoModelo.Tipo, isBoxPadrao, itemProjeto.EspessuraVidro,
                    itemProjeto.IdCorVidro).GetValueOrDefault();
        }

        /// <summary>
        /// Calcula as peças do projeto modelo, cria as peças do item de projeto, cria os materiais do item de projeto, calcula o valor e M2 do item de projeto.
        /// </summary>
        public PecasMateriaisProjeto CriarPecasMateriaisProjeto(IItemProjeto itemProjeto, List<PecaProjetoModelo> pecasProjetoModelo, IEnumerable<IPecaItemProjeto> pecasItemProjeto,
            IEnumerable<IMedidaItemProjeto> medidasItemProjeto, int? tipoEntrega, uint? idCliente)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retornoValidacao = string.Empty;

                    var projetoModelo = ProjetoModeloDAO.Instance.GetElement(transaction, itemProjeto.IdProjetoModelo);
                    var medidasProjetoModelo = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(transaction, projetoModelo.IdProjetoModelo, false);

                    // Recupera as peças de vidro com base no item de projeto.
                    PreencherProdutoVidro(transaction, itemProjeto, projetoModelo, pecasProjetoModelo);

                    // Calcula as medidas das peças, retornando a listagem de peças.
                    pecasProjetoModelo = UtilsProjeto.CalcularMedidasPecas(transaction, itemProjeto, projetoModelo, pecasProjetoModelo, medidasItemProjeto, true, false, out retornoValidacao);

                    // Cria as peças do item de projeto com base nas peças do projeto modelo.
                    pecasItemProjeto = PecaItemProjetoDAO.Instance.CriarPelaPecaProjetoModelo(transaction, itemProjeto, pecasProjetoModelo, medidasItemProjeto, medidasProjetoModelo);

                    // Cria os materiais do item de projeto com base nas peças criadas.
                    var materiaisItemProjeto = MaterialItemProjetoDAO.Instance.CriarMateriais(transaction, itemProjeto, projetoModelo, pecasProjetoModelo, pecasItemProjeto, medidasProjetoModelo,
                        medidasItemProjeto, tipoEntrega, idCliente);

                    // Calcula o total e custo do item de projeto.
                    itemProjeto.Total = materiaisItemProjeto.Sum(f => f.Total);
                    itemProjeto.CustoTotal = materiaisItemProjeto.Sum(f => f.Custo);

                    transaction.Commit();
                    transaction.Close();

                    return new PecasMateriaisProjeto(itemProjeto, pecasProjetoModelo, pecasItemProjeto, materiaisItemProjeto);
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

        #region Confirma Projeto

        /// <summary>
        /// Confirma o projeto.
        /// </summary>
        public void Confirmar(ItemProjeto itemProjeto, uint idOrcamento, uint idPedido, uint idPedidoEsp, uint idAmbienteOrca,
            uint idAmbientePedido, uint idAmbientePedidoEsp, string ambiente, bool pecasAlteradas, bool alterarMedidasPecas,
            bool visualizar, ref System.Web.UI.WebControls.Table tbPecaModelo, ref System.Web.UI.WebControls.Table tbMedInst,
            out string retornoValidacao, ref bool medidasAlteradas, bool ecommerce)
        {
            // Aguarda na fila.
            FilaOperacoes.ConfirmarProjeto.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    retornoValidacao = string.Empty;

                    /* Chamado 48676. */
                    if (itemProjeto == null)
                        throw new Exception("Não foi possível recuperar o projeto. Atualize a tela e confirme o projeto novamente.");

                    // Chamado 49342 - O ambiente do orçamento estava sendo excluido durante a inserção e depois não era possível editar os produtos.
                    if (idAmbienteOrca > 0 && !AmbienteOrcamentoDAO.Instance.AmbienteOrcamentoExiste(transaction, idAmbienteOrca))
                        throw new Exception("O ambiente do orçamento foi excluído durante a inserção desse projeto, volte a tela do orçamento, insira um novo ambiente e então insira o projeto novamente.");

                    //Busca o modelo de Projeto.
                    var modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(transaction, itemProjeto.IdProjetoModelo);
                    uint idAmbienteNovo;

                    // Atualiza o campo ambiente no itemProjeto
                    AtualizaAmbiente(transaction, itemProjeto.IdItemProjeto, ambiente);
                    itemProjeto.Ambiente = ambiente;

                    var medidasAlteradasAreaInstalacao = false;

                    if (!pecasAlteradas && !itemProjeto.MedidaExata && !alterarMedidasPecas)
                        medidasAlteradasAreaInstalacao = UtilsProjeto.VerificaMedidasAreaInstalacaoAlteradas(transaction, itemProjeto, modelo, tbMedInst, tbPecaModelo);

                    // Se for cálculo de projeto com medidas exatas da peça, ou seja, sem medida de vão
                    if (pecasAlteradas || itemProjeto.MedidaExata || alterarMedidasPecas || medidasAlteradasAreaInstalacao)
                    {
                        uint? idObra = null, idCliente = null;
                        int? tipoEntrega = null;

                        if (idOrcamento > 0)
                        {
                            idCliente = OrcamentoDAO.Instance.ObtemIdCliente(transaction, idOrcamento);
                            tipoEntrega = OrcamentoDAO.Instance.ObtemTipoEntrega(transaction, idOrcamento);
                        }

                        if (idPedido > 0)
                        {
                            var acrescimoPedido = PedidoDAO.Instance.ObterAcrescimo(transaction, (int)idPedido);

                            /* Chamado 57877. */
                            if (acrescimoPedido > 0)
                                throw new Exception("Retire o acréscimo do pedido para confirmar os projetos.");

                            idCliente = PedidoDAO.Instance.ObtemIdCliente(transaction, idPedido);
                            tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(transaction, idPedido);
                            idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(transaction, idPedido) : null;
                        }

                        if (idPedidoEsp > 0)
                        {
                            idCliente = PedidoDAO.Instance.ObtemIdCliente(transaction, idPedidoEsp);
                            tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(transaction, idPedidoEsp);
                            idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(transaction, idPedidoEsp) : null;
                        }

                        // Calcula as medidas das peças, retornando lista
                        var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTela(transaction, modelo, itemProjeto, tbMedInst, tbPecaModelo, false, alterarMedidasPecas,
                            medidasAlteradasAreaInstalacao, out retornoValidacao);

                        // Insere Peças na tabela peca_item_projeto
                        PecaItemProjetoDAO.Instance.InsertFromPecaModelo(transaction, itemProjeto, ref lstPecaModelo, ecommerce);

                        // Insere Peças na tabela material_item_projeto
                        MaterialItemProjetoDAO.Instance.InserePecasVidro(transaction, idObra, idCliente, tipoEntrega, itemProjeto, modelo, lstPecaModelo);

                        #region Update Total Item Projeto

                        UpdateTotalItemProjeto(transaction, itemProjeto.IdItemProjeto);

                        uint? idProjeto = GetIdProjeto(transaction, itemProjeto.IdItemProjeto);
                        uint? _idOrcamento = GetIdOrcamento(transaction, itemProjeto.IdItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(transaction, idProjeto.Value);
                        else if (_idOrcamento > 0)
                        {
                            uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(transaction, itemProjeto.IdItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, idProd);

                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(transaction, _idOrcamento.Value);
                        }

                        #endregion

                        // Monta tabela de peças
                        UtilsProjeto.CreateTablePecasItemProjeto(transaction, ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, alterarMedidasPecas, visualizar, ecommerce);

                        // Recarrega o item projeto após calcular os totais, para que no método "InsereAtualizaProdProj" abaixo
                        // considere o valor correto do item projeto ao inserir produto no orçamento
                        // Deve ser GetElement para não dar problema ao criar a descrição das cores dos vidros/ferragens/alumínios dentro de
                        // InsereAtualizaProdProj ao chamar a função UtilsProjeto.FormataTextoOrcamento
                        itemProjeto = GetElement(transaction, itemProjeto.IdItemProjeto);
                    }
                    else
                    {
                        #region Update Total Item Projeto

                        /* Chamado 53665. */
                        UpdateTotalItemProjeto(transaction, itemProjeto.IdItemProjeto);

                        var idProjeto = GetIdProjeto(transaction, itemProjeto.IdItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(transaction, idProjeto);
                        else if (idOrcamento > 0)
                        {
                            var idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(transaction, itemProjeto.IdItemProjeto);

                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, idProd);

                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(transaction, idOrcamento);
                        }

                        #endregion
                    }

                    var lstPecas = PecaItemProjetoDAO.Instance.GetByItemProjeto(transaction, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                    var idGrupoModelo = ProjetoModeloDAO.Instance.ObtemGrupoModelo(transaction, itemProjeto.IdProjetoModelo);
                    var codigoGrupoModelo = GrupoModeloDAO.Instance.ObtemDescricao(transaction, idGrupoModelo);

                    if (lstPecas.Count == 0 &&
                        ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01" &&
                        /* Chamado 22588. */
                        !codigoGrupoModelo.ToLower().Contains("esquadria"))
                        throw new Exception("Informe as peças de vidro.");

                    // Verifica se as peças do item projeto estão de acordo com os materiais do mesmo. Chamado 9673.
                    foreach (var peca in lstPecas)
                    {
                        /* Chamado 63058. */
                        if (peca.Qtde == 0 || peca.IdProd == 0)
                            continue;

                        var material = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(transaction, peca.IdPecaItemProj);

                        // Se a Peça Item Projeto não tiver IdProd, é porque não foi calculado os vidros.
                        if (peca.IdProd > 0 && (material == null || material.Qtde != peca.Qtde))
                        {
                            var ex = new Exception("Calcule as medidas novamente. Os materias do projeto não conferem com as peças do mesmo.");
                            ErroDAO.Instance.InserirFromException("CadProjetoAvulso.aspx", ex);
                            throw ex;
                        }
                        else if (peca.Altura == 0 || peca.Largura == 0)
                            throw new Exception(
                                string.Format("A {0} da peça {1} está zerada. Informe o valor desta medida e confirme o projeto novamente.",
                                    peca.Altura == 0 ? "Altura" : "Largura", peca.CodInterno));

                        /* Chamado 24308. */
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE peca_item_projeto SET ImagemEditada=0 WHERE IdPecaItemProj={0};", peca.IdPecaItemProj));
                    }

                    if (idOrcamento > 0)
                        idAmbienteNovo = ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(transaction, idOrcamento, idAmbienteOrca, itemProjeto);

                    if (idPedido > 0)
                    {
                        /* Chamado 52637.
                         * Remove e aplica acréscimo/desconto/comissão no pedido somente uma vez.
                         * Antes essa atualização estava demorando muito porque era feita para cada ambiente. */
                        #region Remove acréscimo/desconto/comissão do pedido

                        var idsAmbientePedido = new List<uint>();
                        var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, idPedido);

                        // Remove acréscimo, desconto e comissão.
                        objPersistence.ExecuteCommand(transaction, "UPDATE pedido SET IdComissionado=NULL WHERE IdPedido=" + idPedido);
                        PedidoDAO.Instance.RemoveComissaoDescontoAcrescimo(transaction, pedido);

                        #endregion

                        idAmbienteNovo = ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(transaction, pedido, idAmbientePedido, itemProjeto, pecasAlteradas || medidasAlteradas, false, true);

                        #region Aplica acréscimo/desconto/comissão do pedido

                        // Aplica acréscimo, desconto e comissão.
                        PedidoDAO.Instance.AplicaComissaoDescontoAcrescimo(transaction, pedido, Geral.ManterDescontoAdministrador);

                        // Aplica acréscimo e desconto no ambiente.
                        if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento && idsAmbientePedido.Count > 0)
                            foreach (var idAmbPed in idsAmbientePedido)
                            {
                                var acrescimoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);
                                var descontoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);

                                if (acrescimoAmbiente == 0 && descontoAmbiente == 0)
                                    continue;

                                var produtosPedido = ProdutosPedidoDAO.Instance.GetByAmbiente(transaction, idAmbPed);

                                if (acrescimoAmbiente > 0)
                                {
                                    AmbientePedidoDAO.Instance.AplicarAcrescimo(
                                        transaction,
                                        pedido,
                                        idAmbPed,
                                        AmbientePedidoDAO.Instance.ObterTipoAcrescimo(transaction, idAmbPed),
                                        acrescimoAmbiente,
                                        produtosPedido
                                    );
                                }

                                if (descontoAmbiente > 0)
                                {
                                    AmbientePedidoDAO.Instance.AplicarDesconto(
                                        transaction,
                                        pedido,
                                        idAmbPed,
                                        AmbientePedidoDAO.Instance.ObterTipoDesconto(transaction, idAmbPed),
                                        descontoAmbiente,
                                        produtosPedido
                                    );
                                }

                                AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(transaction, pedido, produtosPedido, true);
                            }

                        // Atualiza o total do pedido.
                        PedidoDAO.Instance.UpdateTotalPedido(transaction, pedido);

                        #endregion
                    }

                    if (idPedidoEsp > 0)
                        idAmbienteNovo = ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(transaction, idPedidoEsp, idAmbientePedidoEsp, itemProjeto, pecasAlteradas || medidasAlteradas);

                    // Marca que cálculo de projeto foi conferido
                    if (idPedido > 0 || idPedidoEsp > 0 || idOrcamento > 0)
                    {
                        // Verifica se todas as medidas de instalação foram inseridas
                        if (!itemProjeto.MedidaExata && itemProjeto.IdCorVidro > 0 && MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(transaction, itemProjeto.IdProjetoModelo, false).Count >
                            MedidaItemProjetoDAO.Instance.GetListByItemProjeto(transaction, itemProjeto.IdItemProjeto).Count && ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01")
                            throw new Exception("Falha ao inserir medidas, confirme o projeto novamente.");

                        CalculoConferido(transaction, itemProjeto.IdItemProjeto);
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    /* Chamado 16164.
                     * Log criado para identificar a causa do problema, caso ocorra novamente. */
                    ErroDAO.Instance.InserirFromException(string.Format("Confirmar Projeto - IdItemProjeto: {0} IdOrcamento: {1} " +
                        "IdPedido: {2} IdPedidoEsp: {3} IdAmbienteOrca: {4} IdAmbientePedido: {5} IdAmbientePedidoEsp: {6} " +
                        "Ambiente: {7} PecasAlteradas: {8} AlterarMedidasPecas: {9} Visualizar: {10}",
                        itemProjeto != null ? itemProjeto.IdItemProjeto.ToString() : "null", idOrcamento, idPedido, idPedidoEsp,
                        idAmbienteOrca, idAmbientePedido, idAmbientePedidoEsp, ambiente != null ? ambiente : "null",
                        pecasAlteradas.ToString(), alterarMedidasPecas.ToString(), visualizar.ToString()), ex);

                    throw ex;
                }
                finally
                {
                    FilaOperacoes.ConfirmarProjeto.ProximoFila();
                }
            }
        }

        #endregion

        #region Verifica se ItemProjeto existe

        /// <summary>
        /// Verifica se ItemProjeto existe
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public new bool Exists(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select count(*) From item_projeto ip Where idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) > 0;
        }

        #endregion

        #region Verifica se todos os projetos do pedido foram confirmados

        /// <summary>
        /// Verifica se todos os projetos do pedido foram confirmados
        /// </summary>
        public bool ProjetosConfirmadosPedido(uint idPedido, ref string projetosNaoConfirmados)
        {
            return ProjetosConfirmadosPedido(null, idPedido, ref projetosNaoConfirmados);
        }

        /// <summary>
        /// Verifica se todos os projetos do pedido foram confirmados
        /// </summary>
        public bool ProjetosConfirmadosPedido(GDASession session, uint idPedido, ref string projetosNaoConfirmados)
        {
            var sql = @"
                Select concat(pm.codigo, if(length(ip.ambiente)>0, concat(' (', ip.ambiente, ')'), '')) as codigo
                From item_projeto ip
                    Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                Where idPedido=" + idPedido + @"
                    And Coalesce(conferido, false)=false";

            var retorno = GetValoresCampo(session, sql, "codigo", ", ");

            if (string.IsNullOrEmpty(retorno))
            {
                // Verifica se algum item projeto está sem material
                sql = @"
                    Select concat(pm.codigo, if(length(ip.ambiente)>0, concat(' (', ip.ambiente, ')'), '')) as codigo
                    From item_projeto ip
                        Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                    Where idPedido=" + idPedido + @"
                        And ip.idItemProjeto Not In (Select iditemprojeto from material_item_projeto)";

                retorno = GetValoresCampo(session, sql, "codigo", ", ");

                if (string.IsNullOrEmpty(retorno))
                    return true;
            }

            projetosNaoConfirmados = retorno;
            return false;
        }

        #endregion

        #region Retorna o IdProjeto do ItemProjeto

        /// <summary>
        /// Retorna o IdProjeto do ItemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdProjeto(uint idItemProjeto)
        {
            return GetIdProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o IdProjeto do ItemProjeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select idProjeto From item_projeto Where idItemProjeto=" + idItemProjeto;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        #endregion

        #region Retorna o IdOrcamento do ItemProjeto

        /// <summary>
        /// Retorna o IdProjeto do ItemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdOrcamento(uint idItemProjeto)
        {
            return GetIdOrcamento(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o IdProjeto do ItemProjeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public uint GetIdOrcamento(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select idOrcamento From item_projeto Where idItemProjeto=" + idItemProjeto;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        #endregion

        #region Cria um novo item de projeto

        private static object _novoItemProjetoLock = new object();

        /// <summary>
        /// Cria um novo item de projeto
        /// </summary>
        public ItemProjeto NovoItemProjetoVazioComTransacao(uint? idProjeto, uint? idOrcamento, uint? idAmbienteOrca, uint? idPedido,
            uint? idAmbientePedido, uint? idPedidoEsp, uint? idAmbientePedidoEsp, uint idProjetoModelo, int? espessuraVidro,
            uint idCorVidro, uint idCorAluminio, uint idCorFerragem, bool apenasVidros, bool medidaExata, bool inserirProdutos)
        {
            lock (_novoItemProjetoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var itemProjeto = NovoItemProjetoVazio(transaction, idProjeto, idOrcamento, idAmbienteOrca, idPedido, idAmbientePedido, idPedidoEsp,
                            idAmbientePedidoEsp, idProjetoModelo, espessuraVidro, idCorVidro, idCorAluminio, idCorFerragem, apenasVidros, medidaExata, inserirProdutos);

                        transaction.Commit();
                        transaction.Close();

                        return itemProjeto;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Cria um novo item de projeto
        /// </summary>
        public ItemProjeto NovoItemProjetoVazio(GDASession session, uint? idProjeto, uint? idOrcamento, uint? idAmbienteOrca, uint? idPedido,
            uint? idAmbientePedido, uint? idPedidoEsp, uint? idAmbientePedidoEsp, uint idProjetoModelo, int? espessuraVidro,
            uint idCorVidro, uint idCorAluminio, uint idCorFerragem, bool apenasVidros, bool medidaExata, bool inserirProdutos)
        {
            // Cria o projeto
            ItemProjeto itemProj = new ItemProjeto();
            itemProj.IdProjeto = idProjeto;
            itemProj.IdOrcamento = idOrcamento;
            itemProj.IdPedido = idPedido;
            itemProj.IdPedidoEspelho = idPedidoEsp;
            itemProj.IdProjetoModelo = idProjetoModelo;
            itemProj.Qtde = 1;
            itemProj.IdCorVidro = idCorVidro;
            itemProj.IdCorAluminio = idCorAluminio;
            itemProj.IdCorFerragem = idCorFerragem;
            itemProj.EspessuraVidro =
                espessuraVidro.GetValueOrDefault() == 0 ?
                    ProjetoModeloDAO.Instance.ObtemEspessura(session, idProjetoModelo) : espessuraVidro.Value;
            itemProj.ApenasVidros = apenasVidros;
            itemProj.MedidaExata = medidaExata && apenasVidros;
            itemProj.IdItemProjeto = Insert(session, itemProj);

            // Busca o item projeto do banco para carregar alguns campos tais como o total
            itemProj = GetElementByPrimaryKey(session, itemProj.IdItemProjeto);

            uint? idCliente;
            int? tipoEntrega;
            bool reposicao;
            GetTipoEntregaCliente(session, itemProj.IdItemProjeto, out tipoEntrega, out idCliente, out reposicao);

            bool isBoxPadrao = ProjetoModeloDAO.Instance.IsBoxPadrao(session, idProjetoModelo);
            //bool isBoxPadrao = GrupoModeloDAO.Instance.ObtemDescricao(
            //    ProjetoModeloDAO.Instance.ObtemGrupoModelo(idProjetoModelo)).ToLower().Replace("ã", "a").Contains("box padrao");

            #region Insere peças de vidro

            foreach (PecaProjetoModelo ppmo in PecaProjetoModeloDAO.Instance.GetByModelo(session, idProjetoModelo))
            {
                //Recupera peça personalizadas do cliente
                var ppm = PecaProjetoModeloDAO.Instance.GetByCliente(session, ppmo.IdPecaProjMod, itemProj.IdCliente.GetValueOrDefault());

                uint? idProd = ProdutoProjetoConfigDAO.Instance.GetIdProdVidro(session, ppm.Tipo, isBoxPadrao,
                    (float)itemProj.EspessuraVidro, idCorVidro);

                /* Chamado 55137. */
                if (idProd != null)
                {
                    var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(session, (int)idProd);

                    if (idsLojaSubgrupoProd.Any())
                    {
                        var idOrcamentoValidacaoLoja = idOrcamento ?? itemProj.IdOrcamento;
                        var idPedidoValidacaoLoja = idPedido ?? itemProj.IdPedido;
                        var idPedidoEspelhoValidacaoLoja = idPedidoEsp ?? itemProj.IdPedidoEspelho;
                        var idProjetoValidacaoLoja = idProjeto ?? itemProj.IdProjeto;

                        var idLoja = idOrcamentoValidacaoLoja > 0 ? OrcamentoDAO.Instance.GetIdLoja(session, idOrcamentoValidacaoLoja.Value) :
                            idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                PedidoDAO.Instance.ObtemIdLoja(session, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                            idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(session, (int)idProjetoValidacaoLoja.Value) : 0;

                        if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                            continue;
                    }
                }

                var idsSubgrupoProdCliente = ClienteDAO.Instance.ObtemIdsSubgrupoArr(idCliente.GetValueOrDefault());
                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)idProd.GetValueOrDefault(0));
                if (idsSubgrupoProdCliente.Count > 0 && !idsSubgrupoProdCliente.Contains((uint)idSubgrupoProd.GetValueOrDefault(0)))
                    continue;

                PecaItemProjeto pip = new PecaItemProjeto();
                pip.IdItemProjeto = itemProj.IdItemProjeto;
                pip.IdProd = idProd;

                pip.Altura =
                    !itemProj.MedidaExata ?
                        (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 3 ? ppm.Altura03MM :
                            itemProj.EspessuraVidro == 4 ? ppm.Altura04MM :
                            itemProj.EspessuraVidro == 5 ? ppm.Altura05MM :
                            itemProj.EspessuraVidro == 6 ? ppm.Altura06MM :
                            itemProj.EspessuraVidro == 8 ? ppm.Altura08MM :
                            itemProj.EspessuraVidro == 10 ? ppm.Altura10MM :
                            itemProj.EspessuraVidro == 12 ? ppm.Altura12MM : 0) : ppm.Altura) : 0;

                pip.Largura =
                    !itemProj.MedidaExata ?
                        (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 3 ? ppm.Largura03MM :
                            itemProj.EspessuraVidro == 4 ? ppm.Largura04MM :
                            itemProj.EspessuraVidro == 5 ? ppm.Largura05MM :
                            itemProj.EspessuraVidro == 6 ? ppm.Largura06MM :
                            itemProj.EspessuraVidro == 8 ? ppm.Largura08MM :
                            itemProj.EspessuraVidro == 10 ? ppm.Largura10MM :
                            itemProj.EspessuraVidro == 12 ? ppm.Largura12MM : 0) : ppm.Largura) : 0;

                pip.Qtde = ppm.Qtde;
                pip.Tipo = ppm.Tipo;
                pip.Obs = ppm.Obs;
                pip.Beneficiamentos = ppm.Beneficiamentos;
                ppm.IdPecaItemProj = PecaItemProjetoDAO.Instance.Insert(session, pip);
            }

            #endregion

            // Se o projeto/item projeto for cálculo apenas de vidros, não precissa inserir os materias deste itemProjeto
            if ((idProjeto == null || !ProjetoDAO.Instance.ObtemValorCampo<bool>(session, "apenasVidro", "idProjeto=" + idProjeto)) && !apenasVidros)
            {
                // Calcula os materiais utilizados neste modelo de projeto
                foreach (MaterialItemProjeto mip in CalculaMateriais(session, itemProj, idCliente, tipoEntrega, false))
                    if (mip.IdPecaItemProj == null || ExecuteScalar<bool>(session, "Select Count(*)=0 From material_item_projeto Where idPecaItemProj=" + mip.IdPecaItemProj))
                        MaterialItemProjetoDAO.Instance.InsertFromNovoItemProjeto(session, mip);
            }

            if (inserirProdutos)
            {
                // Insere/Atualiza produto no orçamento/pedido/pedido espelho
                if (idOrcamento > 0)
                    ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(session, idOrcamento.Value, idAmbienteOrca, itemProj);

                // Insere/Atualiza produto no pedido, caso esteja sendo inserido projeto no pedido
                if (idPedido > 0)
                {
                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(session, (int)idPedido);
                    ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(session, pedido, idAmbientePedido, itemProj, false, false);
                }

                // Insere/Atualiza produto no pedido, caso esteja sendo inserido projeto no pedido
                if (idPedidoEsp > 0)
                    ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(session, idPedidoEsp.Value, idAmbientePedidoEsp, itemProj, false);
            }

            return itemProj;
        }

        /// <summary>
        /// Calcula os materiais a serem utilizados no cálculo de projeto passado
        /// </summary>
        /// <param name="itemProj"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public List<MaterialItemProjeto> CalculaMateriais(ItemProjeto itemProj, uint? idCliente, int? tipoEntrega, bool calcularMateriais)
        {
            return CalculaMateriais(null, itemProj, idCliente, tipoEntrega, calcularMateriais);
        }

        /// <summary>
        /// Calcula os materiais a serem utilizados no cálculo de projeto passado
        /// </summary>
        /// <param name="itemProj"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public List<MaterialItemProjeto> CalculaMateriais(GDASession sessao, ItemProjeto itemProj, uint? idCliente, int? tipoEntrega, bool calcularMateriais)
        {
            // Importa os materiais do projeto da tabela materiais do modelo
            List<MaterialProjetoModelo> lstMaterialModelo = MaterialProjetoModeloDAO.Instance.GetByProjetoModelo(sessao, itemProj.IdProjetoModelo, itemProj.EspessuraVidro);
            List<MaterialItemProjeto> lstMaterialItemProj = new List<MaterialItemProjeto>();
            ProjetoModelo projMod = calcularMateriais ? ProjetoModeloDAO.Instance.GetElementByPrimaryKey(sessao, itemProj.IdProjetoModelo) : new ProjetoModelo();

            // Pega o id do produto referente ao material "escova"
            uint idProdEscova = ProdutoProjetoDAO.Instance.GetEscovaId(sessao);

            // Pega o id do produto referente ao material "mão de obra"
            uint idProdMaoDeObra = ProdutoProjetoDAO.Instance.GetMaoDeObraId(sessao);

            foreach (MaterialProjetoModelo mpm in lstMaterialModelo)
            {
                uint idProd = mpm.IdProd;

                #region Configuração produto x cor

                if (mpm.TipoProd == (int)ProdutoProjeto.TipoProduto.Aluminio)
                {
                    uint? retIdProd = ProdutoProjetoConfigDAO.Instance.GetIdProd(sessao, mpm.IdProdProj, ProdutoProjeto.TipoProduto.Aluminio, itemProj.IdCorAluminio);

                    if (retIdProd > 0)
                    {
                        var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(sessao, (int)idProd);

                        if (idsLojaSubgrupoProd.Any())
                        {
                            var idOrcamentoValidacaoLoja = itemProj.IdOrcamento;
                            var idPedidoValidacaoLoja = itemProj.IdOrcamento;
                            var idPedidoEspelhoValidacaoLoja = itemProj.IdOrcamento;
                            var idProjetoValidacaoLoja = itemProj.IdProjeto;

                            var idLoja = idOrcamentoValidacaoLoja > 0 ? OrcamentoDAO.Instance.GetIdLoja(sessao, idOrcamentoValidacaoLoja.Value) :
                                idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                    PedidoDAO.Instance.ObtemIdLoja(sessao, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                                idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(sessao, (int)idProjetoValidacaoLoja.Value) : 0;

                            if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                                continue;
                            /* Chamado 48322. */
                            else if (idLoja == 0 && idProjetoValidacaoLoja > 0)
                                ProjetoDAO.Instance.AtualizarIdLojaProjeto(sessao, (int)idProjetoValidacaoLoja, (int)idsLojaSubgrupoProd.First());
                        }

                        var idsSubgrupoProdCliente = ClienteDAO.Instance.ObtemIdsSubgrupoArr(idCliente.GetValueOrDefault());
                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd);
                        if (idsSubgrupoProdCliente.Count > 0 && !idsSubgrupoProdCliente.Contains((uint)idSubgrupoProd.GetValueOrDefault(0)))
                            continue;

                        idProd = retIdProd.Value;
                    }
                    /* Chamado 51169. */
                    else
                        idProd = 0;
                }
                else if (mpm.TipoProd == (int)ProdutoProjeto.TipoProduto.Ferragem)
                {
                    uint? retIdProd = ProdutoProjetoConfigDAO.Instance.GetIdProd(sessao, mpm.IdProdProj, ProdutoProjeto.TipoProduto.Ferragem, itemProj.IdCorFerragem);

                    if (retIdProd > 0)
                    {
                        var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(sessao, (int)idProd);

                        if (idsLojaSubgrupoProd.Any())
                        {
                            var idOrcamentoValidacaoLoja = itemProj.IdOrcamento;
                            var idPedidoValidacaoLoja = itemProj.IdOrcamento;
                            var idPedidoEspelhoValidacaoLoja = itemProj.IdOrcamento;
                            var idProjetoValidacaoLoja = itemProj.IdProjeto;

                            var idLoja = idOrcamentoValidacaoLoja > 0 ? OrcamentoDAO.Instance.GetIdLoja(sessao, idOrcamentoValidacaoLoja.Value) :
                                idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                    PedidoDAO.Instance.ObtemIdLoja(sessao, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                                idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(sessao, (int)idProjetoValidacaoLoja.Value) : 0;

                            if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                                continue;
                            /* Chamado 48322. */
                            else if (idLoja == 0 && idProjetoValidacaoLoja > 0)
                                ProjetoDAO.Instance.AtualizarIdLojaProjeto(sessao, (int)idProjetoValidacaoLoja, (int)idsLojaSubgrupoProd.First());
                        }

                        var idsSubgrupoProdCliente = ClienteDAO.Instance.ObtemIdsSubgrupoArr(idCliente.GetValueOrDefault());
                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd);
                        if (idsSubgrupoProdCliente.Count > 0 && !idsSubgrupoProdCliente.Contains((uint)idSubgrupoProd.GetValueOrDefault(0)))
                            continue;

                        idProd = retIdProd.Value;
                    }
                    /* Chamado 51169. */
                    else
                        idProd = 0;
                }

                #endregion

                if (idProd == 0 || !ProdutoDAO.Instance.Exists(sessao, idProd))
                    continue;

                var idPedido = (int?)itemProj.IdPedido ?? (int?)itemProj.IdPedidoEspelho ?? null;

                MaterialItemProjeto materItem = new MaterialItemProjeto();
                materItem.IdItemProjeto = itemProj.IdItemProjeto;
                materItem.IdMaterProjMod = mpm.IdMaterProjMod;
                materItem.IdProd = idProd;
                materItem.Qtde = mpm.Qtde;
                materItem.Altura = mpm.Altura;
                materItem.AlturaCalc = mpm.Altura;
                materItem.Largura = mpm.Largura;
                materItem.TotM = mpm.TotM;
                materItem.Espessura = ProdutoDAO.Instance.ObtemEspessura(sessao, (int)idProd);
                materItem.Valor = ProdutoDAO.Instance.GetValorTabela(sessao, (int)idProd, tipoEntrega, idCliente, false, itemProj.Reposicao, 0, idPedido, null, (int?)itemProj.IdOrcamento, mpm.Altura);
                materItem.GrauCorte = mpm.GrauCorte;

                if (calcularMateriais)
                {
                    materItem.QtdModelo = mpm.Qtde;
                    materItem.IdCliente = idCliente.Value;
                    materItem.IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd);
                    materItem.IdSubgrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd);
                    materItem.CalculoQtde = mpm.CalculoQtde;
                    materItem.CalculoAltura = mpm.CalculoAltura;

                    UtilsProjeto.CalcMaterial(sessao, ref materItem, itemProj, projMod, idProdEscova, idProdMaoDeObra);
                }

                lstMaterialItemProj.Add(materItem);
            }

            return lstMaterialItemProj;
        }

        #endregion

        #region Atualiza valor do campo "Ambiente"

        /// <summary>
        /// Atualiza valor do campo "Ambiente" do item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="ambiente"></param>
        public void AtualizaAmbiente(uint idItemProjeto, string ambiente)
        {
            AtualizaAmbiente(null, idItemProjeto, ambiente);
        }

        /// <summary>
        /// Atualiza valor do campo "Ambiente" do item_projeto passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="ambiente"></param>
        public void AtualizaAmbiente(GDASession sessao, uint idItemProjeto, string ambiente)
        {
            string sql = "Update item_projeto set ambiente=?ambiente Where idItemProjeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter[] { new GDAParameter("?ambiente", ambiente) });
        }

        #endregion

        #region Marca que cálculo foi conferido

        /// <summary>
        /// Marca que cálculo foi conferido
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void CalculoConferido(uint idItemProjeto)
        {
            CalculoConferido(null, idItemProjeto);
        }

        /// <summary>
        /// Marca que cálculo foi conferido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        public void CalculoConferido(GDASession sessao, uint idItemProjeto)
        {
            if (ProjetoPossuiMaterialSemValor(sessao, (int)idItemProjeto))
                throw new Exception("Para confirmar o projeto informe o valor de cada produto.");

            string sql = "Update item_projeto set conferido=true where iditemprojeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Marca que cálculo precisa ser conferido
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void CalculoNaoConferido(uint idItemProjeto)
        {
            CalculoNaoConferido(null, idItemProjeto);
        }

        /// <summary>
        /// Marca que cálculo precisa ser conferido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        public void CalculoNaoConferido(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Update item_projeto set conferido=false where iditemprojeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Atualiza total do item do projeto

        /// <summary>
        /// Atualiza o valor total do item do projeto, somando os totais dos produtos relacionados à ele
        /// </summary>
        public void UpdateTotalItemProjeto(uint idItemProjeto)
        {
            UpdateTotalItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Atualiza o valor total do item do projeto, somando os totais dos produtos relacionados à ele
        /// </summary>
        public void UpdateTotalItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = @"
                update item_projeto ip set ip.total=Round((
                        Select Sum(Total+Coalesce(valorBenef, 0))
                        From material_item_projeto mip
                        Where mip.idItemProjeto=ip.idItemProjeto
                    ), 2), ip.CustoTotal=Round((
                        select sum(Coalesce(Custo,0))
                        from material_item_projeto mip
                        Where mip.idItemProjeto=ip.idItemProjeto
                    ), 2)
                Where idItemProjeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Retorna o valor total do item do projeto

        /// <summary>
        /// Retorna o valor total do item do projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public decimal GetTotalItemProjeto(uint idItemProjeto)
        {
            return GetTotalItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o valor total do item do projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public decimal GetTotalItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            return ExecuteScalar<decimal>(sessao, "Select coalesce(total, 0) From item_projeto ip Where ip.idItemProjeto=" + idItemProjeto);
        }

        /// <summary>
        /// Retorna o valor total do item do projeto com a otimização das barras de alumínio.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public decimal GetTotalItemProjetoAluminio(uint idItemProjeto)
        {
            return GetTotalItemProjetoAluminio(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o valor total do item do projeto com a otimização das barras de alumínio.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public decimal GetTotalItemProjetoAluminio(GDASession sessao, uint idItemProjeto)
        {
            return GetTotalItemProjeto(sessao, idItemProjeto);
        }

        #endregion

        #region Retorna o m² do vão do item do projeto

        /// <summary>
        /// Retorna o valor total do item do projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public Single GetM2VaoItemProjeto(uint idItemProjeto)
        {
            string sql = "Select coalesce(m2Vao, 0) From item_projeto ip Where ip.idItemProjeto=" + idItemProjeto;

            return Single.Parse(objPersistence.ExecuteScalar(sql).ToString().Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
        }

        #endregion

        #region Verifica se há algum produtosPedido relacionado ao itemProjeto

        /// <summary>
        /// Verifica se há algum produtosPedido relacionado ao itemProjeto
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public bool ExistsItemProjInProdPed(uint idItemProjeto)
        {
            return ExistsItemProjInProdPed(null, idItemProjeto);
        }

        /// <summary>
        /// Verifica se há algum produtosPedido relacionado ao itemProjeto
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public bool ExistsItemProjInProdPed(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select Count(*) From produtos_pedido Where idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) > 0;
        }

        #endregion

        #region Verifica se item é apenas vidro

        /// <summary>
        /// Verifica se item é apenas vidro
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool ApenasVidros(uint idItemProjeto)
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From item_projeto Where apenasVidros=true And idItemProjeto=" + idItemProjeto) > 0;
        }

        #endregion

        #region Retorna um elemento para usar na tela de projeto avulso

        /// <summary>
        /// Retorna um elemento para usar na tela de projeto avulso.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public ItemProjeto GetElementForProjetoAvulso(uint idItemProjeto)
        {
            return Exists(idItemProjeto) ? GetElementByPrimaryKey(idItemProjeto) : null;
        }

        #endregion

        #region Verifica se o item projeto pode ser editado

        /// <summary>
        /// Verifica se o item projeto pode ser editado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool PodeSerEditado(uint idItemProjeto)
        {
            uint? idAmbiente = AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(idItemProjeto);

            if (idAmbiente == null || idAmbiente == 0)
                return true;

            var idsProdPed = String.Join(",",
                ExecuteMultipleScalar<string>("Select idProdPed From produtos_pedido_espelho Where idAmbientePedido=" + idAmbiente.Value).ToArray());

            if (String.IsNullOrEmpty(idsProdPed))
                idsProdPed = "0";

            // SQL otimizado, dessa forma fica mais rápido do que a outra forma que estava.
            string sql = @"
                Select count(*)
                From impressao_etiqueta
                Where idImpressao In (Select idimpressao From produto_impressao pi Where pi.idprodped in (" + idsProdPed + @"))
                    And situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa;

            return objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        #endregion

        #region Recupera o tipo de entrega e o ID do cliente do item do projeto

        /// <summary>
        /// Recupera o tipo de entrega e o cliente de um item de projeto.
        /// </summary>
        public void GetTipoEntregaCliente(uint idItemProjeto, out int? tipoEntrega, out uint? idCliente, out bool isReposicao)
        {
            GetTipoEntregaCliente(null, idItemProjeto, out tipoEntrega, out idCliente, out isReposicao);
        }

        /// <summary>
        /// Recupera o tipo de entrega e o cliente de um item de projeto.
        /// </summary>
        public void GetTipoEntregaCliente(GDASession session, uint idItemProjeto, out int? tipoEntrega, out uint? idCliente, out bool isReposicao)
        {
            uint? idProjeto, idOrcamento, idPedido, idPedidoEspelho = null;

            if ((idProjeto = ObtemValorCampo<uint?>(session, "idProjeto", "idItemProjeto=" + idItemProjeto)) > 0)
            {
                tipoEntrega = ProjetoDAO.Instance.ObtemValorCampo<int?>(session, "tipoEntrega", "idProjeto=" + idProjeto);
                idCliente = ProjetoDAO.Instance.ObtemValorCampo<uint?>(session, "idCliente", "idProjeto=" + idProjeto);
                isReposicao = false;
            }
            else if ((idOrcamento = ObtemValorCampo<uint?>(session, "idOrcamento", "idItemProjeto=" + idItemProjeto)) > 0)
            {
                tipoEntrega = OrcamentoDAO.Instance.ObtemValorCampo<int?>(session, "tipoEntrega", "idOrcamento=" + idOrcamento);
                idCliente = OrcamentoDAO.Instance.ObtemIdCliente(session, idOrcamento.Value);
                isReposicao = false;
            }
            else if ((idPedido = ObtemValorCampo<uint?>(session, "idPedido", "idItemProjeto=" + idItemProjeto)) > 0 ||
                (idPedidoEspelho = ObtemValorCampo<uint?>(session, "idPedidoEspelho", "idItemProjeto=" + idItemProjeto)) > 0)
            {
                uint id = idPedido > 0 ? idPedido.Value : idPedidoEspelho.Value;
                tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(session, id);
                idCliente = PedidoDAO.Instance.ObtemIdCliente(session, id);
                isReposicao = PedidoDAO.Instance.IsPedidoReposicao(session, id.ToString());
            }
            else
            {
                tipoEntrega = null;
                idCliente = null;
                isReposicao = false;
            }
        }

        #endregion

        #region Obtem dados de um item projeto

        public uint ObtemIdCliente(uint idItemProjeto)
        {
            return ObtemIdCliente(null, idItemProjeto);
        }

        public uint ObtemIdCliente(GDASession sessao, uint idItemProjeto)
        {
            string sql = @"select coalesce(proj.idCliente, coalesce(ped.idCli, coalesce(pedEsp.idCli, orca.idCliente)))
                from item_projeto ip
                    left join projeto proj on (ip.idProjeto=proj.idProjeto)
                    left join pedido ped on (ip.idPedido=ped.idPedido)
                    left join pedido pedEsp on (ip.idPedidoEspelho=pedEsp.idPedido)
                    left join orcamento orca on (ip.idOrcamento=orca.idOrcamento)
                where idItemProjeto=" + idItemProjeto;

            return ExecuteScalar<uint>(sessao, sql);
        }

        public uint ObtemIdProjetoModelo(GDASession sessao, uint idItemProjeto)
        {
            return ObtemValorCampo<uint>(sessao, "idProjetoModelo", "idItemProjeto=" + idItemProjeto);
        }

        public uint? ObtemIdProjeto(uint idItemProjeto)
        {
            return ObtemValorCampo<uint?>("idProjeto", "idItemProjeto=" + idItemProjeto);
        }

        public uint? ObtemIdPedido(uint idItemProjeto)
        {
            return ObtemIdPedido(null, idItemProjeto);
        }

        public uint? ObtemIdPedido(GDASession session, uint idItemProjeto)
        {
            return ObtemValorCampo<uint?>(session, "idPedido", "idItemProjeto=" + idItemProjeto);
        }

        public uint? ObtemIdPedidoEspelho(uint idItemProjeto)
        {
            return ObtemIdPedidoEspelho(null, idItemProjeto);
        }

        public uint? ObtemIdPedidoEspelho(GDASession session, uint idItemProjeto)
        {
            return ObtemValorCampo<uint?>(session, "idPedidoEspelho", "idItemProjeto=" + idItemProjeto);
        }

        public string ObtemAmbiente(uint idItemProjeto)
        {
            return ObtemValorCampo<string>("ambiente", "idItemProjeto=" + idItemProjeto);
        }

        public string ObtemObs(uint idItemProjeto)
        {
            return ObtemValorCampo<string>("obs", "idItemProjeto=" + idItemProjeto);
        }

        public bool EstaConferido(uint idItemProjeto)
        {
            return ObtemValorCampo<bool>("conferido", "idItemProjeto=" + idItemProjeto);
        }

        /// <summary>
        /// Retorna os Ids dos projetos modelos utilizados no item projeto para o pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<uint> ObterIdsProjetoModeloPorPedido(GDASession sessao, uint idPedido)
        {
            var sql = string.Format("SELECT DISTINCT(IdProjetoModelo) FROM item_projeto WHERE IdPedido={0}", idPedido);
            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        #endregion

        #region Exclui cálculo de projeto

        private static readonly object _excluirProjetoLock = new object();

        /// <summary>
        /// Exclui o item projeto a partir de sua fonte (ProdutosOrcamento, AmbientePedido, AmbientePedidoEspelho)
        /// </summary>
        public void ExcluiProjeto(uint idItemProjeto, uint? idOrcamento, uint? idPedido, uint? idPedidoEspelho)
        {
            lock (_excluirProjetoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var itemProjeto = Instance.GetElementByPrimaryKey(transaction, (int)idItemProjeto);

                        if (idOrcamento > 0 || itemProjeto.IdOrcamento > 0)
                        {
                            uint? idProd = ProdutosOrcamentoDAO.Instance.GetIdByIdItemProjeto(transaction, idItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.DeleteByPrimaryKeyExcluirProjeto(transaction, idProd.Value);
                            else
                                DeleteByPrimaryKey(transaction, idItemProjeto);
                        }

                        if (idPedido > 0 || itemProjeto.IdPedido > 0)
                        {
                            uint idAmbientePedido = AmbientePedidoDAO.Instance.ObtemIdAmbiente(transaction, idItemProjeto);
                            if (idAmbientePedido > 0)
                                AmbientePedidoDAO.Instance.DeleteByPrimaryKey(transaction, idAmbientePedido);
                            else
                                DeleteByPrimaryKey(transaction, idItemProjeto);
                        }

                        if (idPedidoEspelho > 0 || itemProjeto.IdPedidoEspelho > 0)
                        {
                            uint idAmbientePedidoEspelho = AmbientePedidoEspelhoDAO.Instance.ObtemIdAmbiente(transaction, idItemProjeto);
                            if (idAmbientePedidoEspelho > 0)
                                AmbientePedidoEspelhoDAO.Instance.DeleteByPrimaryKey(transaction, idAmbientePedidoEspelho);
                            else
                                DeleteByPrimaryKey(transaction, idItemProjeto);
                        }

                        if (idOrcamento.GetValueOrDefault() == 0 &&
                            idPedido.GetValueOrDefault() == 0 &&
                            idPedidoEspelho.GetValueOrDefault() == 0 &&
                            itemProjeto.IdOrcamento.GetValueOrDefault() == 0 &&
                            itemProjeto.IdPedido.GetValueOrDefault() == 0 &&
                            itemProjeto.IdPedidoEspelho.GetValueOrDefault() == 0)
                            DeleteByPrimaryKey(transaction, idItemProjeto);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("ItemProjetoDAO - ExcluiProjeto - IdItemProjeto: {0}", idItemProjeto), ex);

                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Atualiza dados do item projeto

        /// <summary>
        /// Atualiza dados do item projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="qtd"></param>
        /// <param name="m2Vao"></param>
        public void AtualizaQtdM2(uint idItemProjeto, int qtd, float m2Vao)
        {
            AtualizaQtdM2(null, idItemProjeto, qtd, m2Vao);
        }

        /// <summary>
        /// Atualiza dados do item projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="qtd"></param>
        /// <param name="m2Vao"></param>
        public void AtualizaQtdM2(GDASession sessao, uint idItemProjeto, int qtd, float m2Vao)
        {
            string sql = "Update item_projeto Set qtde=" + qtd + ", m2Vao=" + m2Vao.ToString().Replace(',', '.') +
                " Where idItemProjeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Atualiza observação do item projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="obs"></param>
        public void AtualizaObs(uint idItemProjeto, string obs)
        {
            string sql = "Update item_projeto Set obs=?obs Where idItemProjeto=" + idItemProjeto;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?obs", obs));
        }

        #endregion

        #region Verifica se cálculo é de box padrão

        /// <summary>
        /// Verifica se cálculo é de box padrão
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public bool IsBoxPadrao(uint idItemProjeto)
        {
            string sql = @"
                Select Count(*) From item_projeto ip
                    Inner Join projeto_modelo pm On (ip.idProjetoModelo=pm.idProjetoModelo)
                    Left Join grupo_modelo g On (pm.idGrupoModelo=g.idGrupoModelo)
                Where idItemProjeto=" + idItemProjeto + @"
                    And (pm.idGrupoModelo=" + (int)UtilsProjeto.GrupoModelo.BoxPadrao + " Or g.descricao Like 'box padrão%')";

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Verifica se há algum material sem valor

        /// <summary>
        /// Verifica se há algum material sem valor.
        /// </summary>
        public bool ProjetoPossuiMaterialSemValor(GDASession session, int idItemProjeto)
        {
            var idPedido = ObtemIdPedido(session, (uint)idItemProjeto) ?? ObtemIdPedidoEspelho(session, (uint)idItemProjeto);

            if (idPedido > 0 && PedidoDAO.Instance.IsPedidoGarantia(session, idPedido.ToString()) ||
                PedidoDAO.Instance.IsPedidoReposicao(session, idPedido.ToString()))
                return false;

            var sql = string.Format("SELECT COUNT(*) FROM material_item_projeto WHERE (Total IS NULL OR Total=0) AND IdItemProjeto={0}", idItemProjeto);
            return objPersistence.ExecuteScalar(session, sql).ToString().StrParaInt() > 0;
        }

        #endregion

        #region Verifica se existe algum Item Projeto do Projeto Modelo

        public bool TemItemProjeto(uint IdProjetoModelo)
        {
            return ObtemValorCampo<int>("COUNT(IdItemProjeto)", "IdProjetoModelo=" + IdProjetoModelo) > 0;
        }

        #endregion

        #region

        /// <summary>
        /// Verifica se o projeto possui itens de projeto não conferidos.
        /// </summary>
        public bool VerificarProjetoPossuiItensNaoConferidos(GDASession session, int idProjeto)
        {
            if (idProjeto == 0)
                return false;

            var sql = string.Format("SELECT IdItemProjeto FROM item_projeto WHERE (Conferido IS NULL OR Conferido=0) AND IdProjeto={0}", idProjeto);
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Obtém os ambientes do projeto que não foram conferidos.
        /// </summary>
        public IList<string> ObterAmbientesProjetoItensProjetoNaoConferidos(GDASession session, int idProjeto)
        {
            if (idProjeto == 0)
                return new List<string>();

            var sql = string.Format("SELECT Ambiente FROM item_projeto WHERE (Conferido IS NULL OR Conferido=0) AND IdProjeto={0}", idProjeto);
            return ExecuteMultipleScalar<string>(session, sql);
        }

        #endregion

        #region Métodos Sobrescritos

        public override int Update(GDASession session, ItemProjeto objUpdate)
        {
            int ret = base.Update(session, objUpdate);

            if (objUpdate.IdProjeto != null)
                ProjetoDAO.Instance.UpdateTotalProjeto(session, objUpdate.IdProjeto.Value);
            else if (objUpdate.IdOrcamento != null)
            {
                uint idProd = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idItemProjeto=" + objUpdate.IdItemProjeto);
                if (idProd > 0)
                    ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, idProd);

                OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, objUpdate.IdOrcamento.Value);
            }

            return ret;
        }

        public override int Update(ItemProjeto objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Delete(ItemProjeto objDelete)
        {
            return Delete(null, objDelete);
        }

        public override int Delete(GDASession sessao, ItemProjeto objDelete)
        {
            return DeleteByPrimaryKey(sessao, objDelete.IdItemProjeto);
        }


        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            uint? idProjeto = ObtemValorCampo<uint?>(sessao, "idProjeto", "idItemProjeto=" + Key);

            // Verifica se este cálculo está relacionado à algum pedido e projeto ao mesmo tempo, se estiver, não poderá ser excluído
            if (idProjeto > 0 && ExistsItemProjInProdPed(sessao, Key))
                throw new Exception("Este cálculo não pode ser excluído por haver um projeto relacionado ao mesmo. " +
                    "Cancele o pedido, edite o projeto e gere o pedido novamente.");

            // Veririca se há algum produto de orçamento que esteja referenciado à este itemProjeto
            if (idProjeto > 0 && objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produtos_orcamento Where idItemProjeto=" + Key) > 0)
                throw new Exception("Este cálculo não pode ser excluído por haver um projeto relacionado ao mesmo. " +
                    "Cancele o orçamento, edite o projeto e gere o orçamento novamente.");

            // Remove a referência do item projeto nos ambientes para não ocorrer erro de referência
            objPersistence.ExecuteCommand(sessao, @"
                Update ambiente_pedido Set idItemProjeto=null Where idItemProjeto=" + Key + @";
                Update ambiente_pedido_espelho Set idItemProjeto=null Where idItemProjeto=" + Key + ";");

            var listaIds = ExecuteMultipleScalar<string>(sessao, "Select idMaterItemProj From material_item_projeto Where idItemProjeto=" + Key);

            if (listaIds != null && listaIds.Count > 0)
            {
                var idsMaterItemProj = String.Join(",", listaIds);

                if (!String.IsNullOrEmpty(idsMaterItemProj))
                {
                    // Exclui beneficiamentos deste item projeto
                    objPersistence.ExecuteCommand(sessao,
                        @"Delete From material_projeto_benef where idMaterItemProj In (" + idsMaterItemProj + ")");

                    // Apaga a referência do material nos produtos antes de excluí-los
                    objPersistence.ExecuteCommand(sessao, @"
                        update produtos_pedido set idmateritemproj=null where idmateritemproj in (" + idsMaterItemProj + @");
                        update produtos_pedido_espelho set idmateritemproj=null where idmateritemproj in (" + idsMaterItemProj + ")");
                }
            }

            // Exclui materiais/peças/medidas deste item_projeto
            objPersistence.ExecuteCommand(sessao, @"
                Delete from material_item_projeto where idItemProjeto=" + Key + @";
                Delete from medida_item_projeto where idItemProjeto=" + Key + @";
                Delete from peca_item_projeto where idItemProjeto=" + Key);

            int ret = base.DeleteByPrimaryKey(sessao, Key);

            if (idProjeto != null)
                ProjetoDAO.Instance.UpdateTotalProjeto(sessao, idProjeto.Value);

            return ret;
        }

        #endregion

        #region Retorna Cor e Espessura do ItemProjeto

        /// <summary>
        /// Retorna a espessura do item do projeto.
        /// </summary>
        public int GetEspessuraItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            return ExecuteScalar<int>(sessao, "Select ESPESSURAVIDRO From item_projeto ip Where ip.idItemProjeto=" + idItemProjeto);
        }

        /// <summary>
        /// Retorna a cor do item do projeto.
        /// </summary>
        public uint GetCorItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            return ExecuteScalar<uint>(sessao, "Select IDCORVIDRO From item_projeto ip Where ip.idItemProjeto=" + idItemProjeto);
        }

        #endregion
    }
}
