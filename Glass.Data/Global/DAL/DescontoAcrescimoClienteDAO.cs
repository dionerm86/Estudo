using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class DescontoAcrescimoClienteDAO : BaseDAO<DescontoAcrescimoCliente, DescontoAcrescimoClienteDAO>
    {
        //private DescontoAcrescimoClienteDAO() { }

        #region Busca padrão

        private string GetWhere(uint idDesconto, uint idGrupo, uint idSubgrupo, string produto, int situacao, bool usarTabelaProduto,
            string aliasGrupoProd, string aliasSubgrupoProd, string aliasProduto, string aliasIdProd)
        {
            string where = "";

            if (idDesconto > 0)
                where += " and dc.idDesconto=" + idDesconto;
            else
            {
                if (idGrupo > 0 && !String.IsNullOrEmpty(aliasGrupoProd))
                    where += " and {0}.idGrupoProd=" + idGrupo;

                if (idSubgrupo > 0 && !String.IsNullOrEmpty(aliasSubgrupoProd))
                    where += " and {1}.idSubgrupoProd=" + idSubgrupo;

                if (!String.IsNullOrEmpty(aliasProduto) && situacao > 0)
                    where += " and {2}.situacao=" + situacao;

                if (!String.IsNullOrEmpty(produto) && !String.IsNullOrEmpty(aliasProduto))
                    where += " and {2}.Descricao LIKE ?produto";
                else if (!usarTabelaProduto && !String.IsNullOrEmpty(aliasIdProd))
                    where += " and {3}.idProd is null";
            }

            return String.Format(where, aliasGrupoProd, aliasSubgrupoProd, aliasProduto, aliasIdProd);
        }

        private string Sql(uint idDesconto, uint idCliente, uint idTabelaDesconto, uint idGrupo, uint idSubgrupo, string produto, int situacao, bool usarTabelaProduto)
        {
            string tabela = !usarTabelaProduto ? @"
                grupo_prod g
                    Left Join subgrupo_prod s On (g.idGrupoProd=s.idGrupoProd) " : @"
                produto p
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) ";

            // Inclui o filtro pela tabela que o cliente está usando atualmente
            string filtroCliente = idDesconto == 0 && idTabelaDesconto == 0 ? " and idCliente=" + idCliente : "";
            string filtroTabela = idDesconto == 0 && idTabelaDesconto > 0 ? " and {0}.idTabelaDesconto=" + idTabelaDesconto : "";

            string tabelaDesconto = @"(
                select idDesconto, idCliente, idTabelaDesconto, idGrupoProd, idSubgrupoProd, idProd, desconto, DescontoAVista, acrescimo, aplicarBeneficiamentos
                from desconto_acrescimo_cliente dc
                where 1" + filtroCliente + String.Format(filtroTabela, "dc") +
                    GetWhere(idDesconto, idGrupo, idSubgrupo, produto, situacao, usarTabelaProduto, "dc", "dc", null, "dc") + @"
                
                union all select null, c.id_Cli, null, p.idGrupoProd, p.idSubgrupoProd, p.idProd, null, null, null, false
                from produto p, cliente c
                where c.id_Cli=" + idCliente + @"
                    and not exists (
                        select dc.idProd from desconto_acrescimo_cliente dc
                        where dc.idCliente=c.id_Cli " + String.Format(filtroTabela, "dc") + @" and dc.idGrupoProd=p.idGrupoProd and 
                            dc.idSubgrupoProd=p.idSubgrupoProd and dc.idProd=p.idProd
                    )
                    " + GetWhere(0, idGrupo, idSubgrupo, produto, situacao, usarTabelaProduto, "p", "p", "p", "p") + @"
                
                union all select null, c.id_Cli, null, g.idGrupoProd, s.idSubgrupoProd, null, null, null, null, false
                from grupo_prod g, subgrupo_prod s, cliente c
                where c.id_Cli=" + idCliente + @"
        	        and g.idGrupoProd=s.idGrupoProd
        	        and (g.idGrupoProd, s.idSubgrupoProd) not in (
            	        select dc.idGrupoProd, dc.idSubgrupoProd
                        from desconto_acrescimo_cliente dc
                        where idCliente=c.id_Cli and dc.idProd is null " + String.Format(filtroTabela, "dc") +
                            GetWhere(0, idGrupo, idSubgrupo, null, situacao, usarTabelaProduto, "dc", "dc", null, "dc") + @"
                    )
                    " + GetWhere(0, idGrupo, idSubgrupo, produto, situacao, usarTabelaProduto, "g", "s", null, null) + @"
            )";

            string camposProduto = usarTabelaProduto ? ", p.Descricao as DescrProduto" : "";

            string sql = @"
                Select dc.IdDesconto, dc.idCliente, dc.idTabelaDesconto, dc.desconto, dc.DescontoAVista, dc.acrescimo, g.idGrupoProd, g.Descricao as DescrGrupo, 
                    s.idSubgrupoProd, s.Descricao as DescrSubgrupo, " + (usarTabelaProduto ? "p.idProd" : "dc.idProd") + ", c.nome as nomeCliente, dc.AplicarBeneficiamentos, dc.descontoavista AS DescontoAvista" + camposProduto + @"
                From " + tabela + @"
                    Left Join " + tabelaDesconto + " dc On (dc.idGrupoProd=g.idGrupoProd and (dc.idSubgrupoProd=s.idSubgrupoProd Or " +
                        "dc.idSubgrupoProd is null)" + (usarTabelaProduto ? " and (dc.idProd=p.idProd" + (idDesconto > 0 ? " or dc.idProd is null)" : ")") : "") + @")
                    Left Join cliente c on (dc.idCliente=c.id_Cli)
                where " + (idDesconto == 0 ? "(dc.idCliente=" + idCliente + " or dc.idCliente is null)" : "1") + @"
                    " + GetWhere(idDesconto, idGrupo, idSubgrupo, produto, situacao, usarTabelaProduto, "g", "s", "p", "dc");

            sql += " order by g.descricao, s.descricao" + (usarTabelaProduto ? ", p.descricao" : "");

            return sql;
        }

        /// <summary>
        /// Busca todos os grupos de produtos com os descontos que o cliente passado possui
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public IList<DescontoAcrescimoCliente> GetByCliente(uint idCliente, uint idTabelaDesconto)
        {
            return objPersistence.LoadData(Sql(0, idCliente, idTabelaDesconto, 0, 0, null, 0, false)).ToList();
        }

        public IList<DescontoAcrescimoCliente> GetByCliente(uint idCliente, uint idTabelaDesconto, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, idCliente, idTabelaDesconto, 0, 0, null, 0, false), sortExpression, startRow, pageSize);
        }

        public IList<DescontoAcrescimoCliente> GetOcorrenciasByCliente(uint idCliente)
        {
            var descontosCliente = GetByCliente(idCliente, 0);

            var descCliOcorrencias = new List<DescontoAcrescimoCliente>();

            foreach (DescontoAcrescimoCliente dac in descontosCliente)
            {
                if (dac.Desconto > 0 || dac.DescontoAVista > 0 || dac.Acrescimo > 0)
                {
                    descCliOcorrencias.Add(dac);

                    if (GetOcorrenciasByClienteGrupoSubgrupo(idCliente, (uint)dac.IdGrupoProd, dac.IdSubgrupoProd == null ? 0 : (uint)dac.IdSubgrupoProd).Count > 0)
                    {
                        dac.TemOcorrenciasProdutos = true;
                    }
                }
                else if (dac.IdSubgrupoProd != null)
                {
                    var descGrupoSub = GetOcorrenciasByClienteGrupoSubgrupo(idCliente, (uint)dac.IdGrupoProd, (uint)dac.IdSubgrupoProd);
                    if (descGrupoSub.Count > 0)
                    {
                        dac.TemOcorrenciasProdutos = true;

                        descCliOcorrencias.Add(dac);
                    }
                }
            }

            return descCliOcorrencias;
        }

        public IList<DescontoAcrescimoCliente> GetByClienteGrupoSubgrupo(uint idCliente, uint idTabelaDesconto, uint idGrupo, uint idSubgrupo, 
            string produto, int situacao, string sortExpression, int startRow, int pageSize)
        {
            GDAParameter p = new GDAParameter("?produto", "%" + produto + "%");
            return LoadDataWithSortExpression(Sql(0, idCliente, idTabelaDesconto, idGrupo, idSubgrupo, produto, situacao, true), sortExpression, startRow, pageSize, p);
        }

        public IList<DescontoAcrescimoCliente> GetOcorrenciasByClienteGrupoSubgrupo(uint idCliente, uint idGrupo, uint idSubgrupo)
        {
            if (idGrupo == 0 && idSubgrupo == 0)
            {
                return new DescontoAcrescimoCliente[0];
            }
            var sql = string.Format("SELECT * FROM ({0}) sub WHERE (sub.Desconto IS NOT NULL AND (sub.Desconto > 0 OR sub.DescontoAVista > 0)) OR (sub.Acrescimo IS NOT NULL AND sub.Acrescimo > 0)",
                Sql(0, idCliente, 0, idGrupo, idSubgrupo, null, 0, true));

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<DescontoAcrescimoCliente> GetForList(uint idCliente, uint idTabelaDesconto, uint idGrupo, uint idSubgrupo, string produto, int situacao, 
            string sortExpression, int startRow, int pageSize)
        {
            if (idGrupo == 0 && idSubgrupo == 0)
                return GetByCliente(idCliente, idTabelaDesconto, sortExpression, startRow, pageSize);
            else
                return GetByClienteGrupoSubgrupo(idCliente, idTabelaDesconto, idGrupo, idSubgrupo, produto, situacao, sortExpression, startRow, pageSize);
        }

        public int GetForListCount(uint idCliente, uint idTabelaDesconto, uint idGrupo, uint idSubgrupo, string produto, int situacao)
        {
            return GetForList(idCliente, idTabelaDesconto, idGrupo, idSubgrupo, produto, situacao, null, 0, int.MaxValue).Count;
        }

        public DescontoAcrescimoCliente GetElement(uint idDesconto)
        {
            return GetElement(null, idDesconto);
        }

        public DescontoAcrescimoCliente GetElement(GDASession session, uint idDesconto)
        {
            List<DescontoAcrescimoCliente> itens = objPersistence.LoadData(session, Sql(idDesconto, 0, 0, 0, 0, null, 0, true));
            return itens.Count > 0 ? itens[0] : new DescontoAcrescimoCliente();
        }

        #endregion

        #region Recupera o desconto/acréscimo por cliente, grupo, subgrupo e produto

        internal DescontoAcrescimoCliente GetDescontoAcrescimo(GDASession sessao, IContainerCalculo container, Produto produto)
        {
            var idCliente = container?.Cliente?.Id ?? default(uint);

            return GetDescontoAcrescimo(
                sessao,
                idCliente,
                produto.IdGrupoProd,
                produto.IdSubgrupoProd,
                produto.IdProd,
                container.IdPedido(),
                container.IdProjeto()
            );
        }

        /// <summary>
        /// Busca o desconto que o cliente passado possui no grupo passado
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupoProd"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public DescontoAcrescimoCliente GetDescontoAcrescimo(uint idCliente, int idGrupo, int? idSubgrupo, int? idProd, int? idPedido, int? idProjeto)
        {
            return GetDescontoAcrescimo(null, idCliente, idGrupo, idSubgrupo, idProd, idPedido, idProjeto);
        }

        /// <summary>
        /// Busca o desconto que o cliente passado possui no grupo passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupoProd"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public DescontoAcrescimoCliente GetDescontoAcrescimo(GDASession sessao, uint idCliente, int idGrupo, int? idSubgrupo, int? idProd, int? idPedido, int? idProjeto)
        {
            // Chamado 16021: Se não passar o cliente, não deve tentar buscar desconto/acréscimo
            if (idCliente == 0)
                return new DescontoAcrescimoCliente();

            uint? idTabelaDesconto = ClienteDAO.Instance.ObtemTabelaDescontoAcrescimo(sessao, idCliente);
            string filtro = idTabelaDesconto > 0 ? "idTabelaDesconto=" + idTabelaDesconto : "idCliente=" + idCliente;

            // Busca apenas itens que tenham desconto ou acréscimo, para que, caso a busca seja por produto, 
            // retorne o desconto/acréscimo do subgrupo ou grupo se o produto não possuir desconto/acréscimo
            string sql = @"select count(*) from desconto_acrescimo_cliente where (desconto>0 or acrescimo>0 or DescontoAVista>0) and " + filtro;

            if (idProd > 0)
                sql += " And idProd=" + idProd.Value;
            /* Chamado 17578.
             * O desconto foi aplicado no produto quando ele pertencia a outro subgrupo,
             * sendo assim, o sql executado abaixo retornava um registro e, logo abaixo,
             * o desconto não era recuperado, pois, o produto no subgrupo atual não possuía desconto. */
            /*else
            {
                sql += " and idGrupoProd=" + idGrupo;

                if (idSubgrupo > 0)
                    sql += " And idSubgrupoProd=" + idSubgrupo.Value;
            }*/
            sql += " and idGrupoProd=" + idGrupo;
 
            if (idSubgrupo > 0)
                sql += " And idSubgrupoProd=" + idSubgrupo.Value;
            /* Chamado 17578. */
            else
                sql += " And idSubgrupoProd IS NULL";

            if (idSubgrupo > 0)
                sql += " And idSubgrupoProd=" + idSubgrupo.Value;
            /* Chamado 17578. */
            else
                sql += " And idSubgrupoProd IS NULL";

            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) == 0)
            {
                if (idProd > 0)
                    return GetDescontoAcrescimo(sessao, idCliente, idGrupo, idSubgrupo, null, idPedido, idProjeto);
                else if (idSubgrupo > 0)
                    return GetDescontoAcrescimo(sessao, idCliente, idGrupo, null, null, idPedido, idProjeto);
                else
                    return new DescontoAcrescimoCliente();
            }

            // O Order By foi colocado para o caso do desconto já estar aplicado em um grupo sem subgrupos e o usuário cadastra um subgrupo,
            // ficando um registro apenas com o grupo e outro com o grupo/subgrupo, fazendo com que a subquery retorne mais de um registro.

            // 21/11/2012 André: O filtro por grupo e subgrupo foi colocado abaixo ao invés de filtrar por g.idGrupoProd e s.idSubgrupoProd
            // devido a um erro ocorrido na Vidro Metro que fazia com que buscasse o desconto de outro grupo ou subgrupo.
            string descontoCliente = @"
                Select {0} From desconto_acrescimo_cliente dc 
                Where " + filtro + @" 
                    and (dc.idGrupoProd=" + idGrupo + @" 
                        and ((dc.idSubgrupoProd=" + (idSubgrupo > 0 ? idSubgrupo.ToString() : (idProd == null || idProd == 0) ? "0" : "s.idSubgrupoProd") + @" Or dc.idSubgrupoProd is null) 
                        and (dc.idProd{1})))
                Order by dc.idsubgrupoprod desc limit 1
                ";

            sql = "Select null as IdDesconto, null as idCliente, null as idTabelaDesconto, (" + 
                String.Format(descontoCliente, "dc.desconto", "{0}") + ") as Desconto, (" +
                String.Format(descontoCliente, "dc.acrescimo", "{0}") + ") as Acrescimo, (" +
                String.Format(descontoCliente, "dc.DescontoAVista", "{0}") + ") as DescontoAVista, (" +
                String.Format(descontoCliente, "dc.AplicarBeneficiamentos", "{0}") + 
                @") as AplicarBeneficiamentos, g.idGrupoProd, s.idSubgrupoProd, g.Descricao as DescrGrupo, p.idProd as IdProd From produto p 
                left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) Left Join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd) 
                Where g.idGrupoProd=" + idGrupo;

            if (idSubgrupo > 0)
                sql += " And s.idSubgrupoProd=" + idSubgrupo.Value;
            // Esta alteração foi necessária para que ao pesquisar descontos por grupo depois da pesquisa de
            // produto e subgrupo terem retornado vazio, estava retornando desconto de qualquer subgrupo dentro do grupo filtrado,
            // sendo que deveria filtrar apenas pelo produto ou subgrupo informados, caso um dos dois tenha sido informado
            else if (idProd == null || idProd == 0) 
                sql += " And s.idSubgrupoProd is null";

            if (idProd > 0)
                sql += " And p.idProd=" + idProd.Value;

            sql = String.Format(sql, idProd > 0 ? "=" + idProd.Value : " is null");

            List<DescontoAcrescimoCliente> lstDesc = objPersistence.LoadData(sessao, sql).ToList();
            return lstDesc.Count > 0 ? lstDesc[0] : new DescontoAcrescimoCliente();
        }

        #endregion

        #region Insere/atualiza o desconto/acréscimo por cliente

        /// <summary>
        /// Insere ou atualiza desconto dado sobre determinado grupo de produto a determinado cliente
        /// </summary>
        public void InsertOrUpdate(GDASession session, uint? idCliente, uint? idTabelaDesconto, uint idGrupo, uint? idSubgrupo,
            uint? idProd, Single desconto, Single acrescimo, bool aplicarBeneficiamentos)
        {
            if (desconto > 100)
                throw new Exception("Não é permitido lançar descontos acima de 100%.");

            string where = idCliente > 0 ? "idCliente=" + idCliente : "idTabelaDesconto=" + idTabelaDesconto;
            where += " And idGrupoProd=" + idGrupo;

            if (idSubgrupo > 0)
                where += " And idSubgrupoProd=" + idSubgrupo.Value;

            if (idProd > 0)
                where += " and idProd=" + idProd.Value;
            else // Se o produto não tiver sido informado, deve retornar apenas registros que idProd seja=null
                where += " and idProd is null";

            string sql = "Select Count(*) From desconto_acrescimo_cliente Where " + where;

            // Se não houver algum registro no banco deste cliente com este grupo, inclui, senão, atualiza
            if (objPersistence.ExecuteSqlQueryCount(session, sql) <= 0)
            {
                if (desconto <= 0 && acrescimo <= 0)
                    return;

                // Não permite inserir se estiver tentando inserir um desconto/acréscimo de subgrupo sendo que já existe um 
                // lançamento com o grupo do mesmo e o subgrupo nulo, isso deve ser feito para que não tenha registro de grupo
                // que nunca será utilizado e para não exibir duplicado na tela de desconto/acréscimo.
                if (idSubgrupo > 0 && idCliente > 0 && idProd.GetValueOrDefault(0) == 0 &&
                    objPersistence.ExecuteSqlQueryCount(session, @"Select Count(*) From desconto_acrescimo_cliente Where idsubgrupoprod is null 
                        and idprod is null and idgrupoprod=" + idGrupo + " and idCliente=" + idCliente) > 0)
                    throw new Exception("Não é possível incluir um desconto para esse subgrupo pois existe um desconto para todo o grupo.");

                DescontoAcrescimoCliente descCli = new DescontoAcrescimoCliente();
                descCli.IdCliente = (int?)idCliente;
                descCli.IdTabelaDesconto = (int?)idTabelaDesconto;
                descCli.IdGrupoProd = (int)idGrupo;
                descCli.Desconto = desconto;
                descCli.Acrescimo = acrescimo;
                descCli.AplicarBeneficiamentos = aplicarBeneficiamentos;

                if (idSubgrupo > 0)
                    descCli.IdSubgrupoProd = (int?)idSubgrupo;

                if (idProd > 0)
                    descCli.IdProduto = (int?)idProd;

                DescontoAcrescimoCliente vazio = new DescontoAcrescimoCliente();
                vazio.IdDesconto = (int)Insert(session, descCli);
                LogAlteracaoDAO.Instance.LogDescontoAcrescimoCliente(session, vazio);
            }
            else
            {
                var itens = objPersistence.LoadData(session, "select * from desconto_acrescimo_cliente where " + where).ToList().ToArray();

                // Gera log de desconto apenas se não for excluir o desconto
                objPersistence.ExecuteCommand(session, "Update desconto_acrescimo_cliente Set desconto=" +
                    desconto.ToString().Replace(",", ".") + ", acrescimo=" + acrescimo.ToString().Replace(",", ".") +
                    ", aplicarBeneficiamentos=" + aplicarBeneficiamentos + " Where " + where);

                foreach (DescontoAcrescimoCliente d in itens)
                    LogAlteracaoDAO.Instance.LogDescontoAcrescimoCliente(session, d);
            }
        }

        #endregion

        #region Verifica se o cliente possui algum desconto

        /// <summary>
        /// Verifica se o cliente possui algum desconto
        /// </summary>
        /// <returns></returns>
        public bool ClientePossuiDesconto(uint idCliente, uint idOrcamento, uint? idProdOrca, uint idPedido, uint? idAmbientePedido)
        {
            return ClientePossuiDesconto(null, idCliente, idOrcamento, idProdOrca, idPedido, idAmbientePedido);
        }

        /// <summary>
        /// Verifica se o cliente possui algum desconto
        /// </summary>
        /// <returns></returns>
        public bool ClientePossuiDesconto(GDASession sessao, uint idCliente, uint idOrcamento, uint? idProdOrca, uint idPedido, uint? idAmbientePedido)
        {
            var filtroCliente = "c.id_cli=" + idCliente;
            var filtroClienteTabela = "d.idCliente=" + idCliente;

            var sql = @"
                SELECT COUNT(*) 
                FROM desconto_acrescimo_cliente d 
                    LEFT JOIN cliente c ON (d.IdTabelaDesconto=c.IdTabelaDesconto)
                WHERE (d.Desconto > 0 OR d.DescontoAVista > 0) AND {0}";

            if (idPedido > 0)
                sql += @" 
                    And idGrupoProd In (Select idGrupoProd From produto Where idProd In 
                        (Select idProd From produtos_pedido Where idPedido=" + idPedido +
                        (idAmbientePedido > 0 ? " And idAmbientePedido=" + idAmbientePedido : "") + @"))
                    And (idSubgrupoProd is null Or 
                        (idSubgrupoProd In 
                            (Select idSubgrupoProd From produto Where idProd In 
                            (Select idProd From produtos_pedido Where idPedido=" + idPedido +
                            (idAmbientePedido > 0 ? " And idAmbientePedido=" + idAmbientePedido : "") + @")) and idProd is null) Or
                        (idProd In 
                            (Select idProd From produtos_pedido Where idPedido=" + idPedido +
                            (idAmbientePedido > 0 ? " And idAmbientePedido=" + idAmbientePedido : "") + @"))
                    )";

            if (idOrcamento > 0)
            {
                // Buscas os ids dos produtos do orçamento (para simplificar o sql)
                string idsProd = String.Join(",", ExecuteMultipleScalar<string>(sessao, @"
                    Select distinct idProduto From produtos_orcamento 
                    Where idOrcamento=" + idOrcamento + @" and idProduto is not null
                        Union
                    Select distinct idProd From material_item_projeto mip
                        Inner Join item_projeto ip On (mip.idItemProjeto=ip.idItemProjeto)
                    Where idOrcamento=" + idOrcamento).ToArray());

                if (String.IsNullOrEmpty(idsProd))
                    return false;

                sql += @" 
                    And idGrupoProd In (Select idGrupoProd From produto Where idProd In 
                        (" + idsProd + @"))
                    And (idSubgrupoProd is null Or 
                        (idSubgrupoProd In 
                            (Select idSubgrupoProd From produto Where idProd In (" + idsProd + @")) and idProd is null) Or 
                        (idProd In (" + idsProd + @"))
                    )";
            }

            if (ExecuteScalar<int>(sessao, String.Format(sql, filtroCliente) + " limit 1") > 0)
                return true;
            else if (ExecuteScalar<int>(sessao, String.Format(sql, filtroClienteTabela) + " limit 1") > 0)
                return true;

            return false;
        }

        #endregion

        #region Verifica se o produto possui algum desconto

        /// <summary>
        /// Verifica se o produto informado possui algum desconto, associado a ele mesmo, ao subgrupo ou ao grupo caso não tenha subgrupo.
        /// </summary>
        public bool ProdutoPossuiDesconto(GDASession session, int idCliente, int idProd)
        {
            var idTabelaDescontoCliente = ClienteDAO.Instance.ObtemTabelaDescontoAcrescimo(session, (uint)idCliente);

            return objPersistence.ExecuteSqlQueryCount(session,
                string.Format(
                    @"SELECT COUNT(*)
                    FROM produto p
                        INNER JOIN desconto_acrescimo_cliente dac ON ({0} AND dac.IdGrupoProd = p.IdGrupoProd
                            AND (dac.IdSubgrupoProd IS NULL OR (dac.IdSubgrupoProd = p.IdSubgrupoProd And dac.IdProd = p.IdProd) OR 
							    (dac.IdSubgrupoProd = p.IdSubgrupoProd And dac.IdProd IS NULL)))
                    WHERE (dac.Desconto > 0{1}) AND p.IdProd={2}
                    GROUP BY p.IdProd",
                    idTabelaDescontoCliente > 0 ?
                        string.Format("dac.IdTabelaDesconto={0}", idTabelaDescontoCliente) :
                        string.Format("dac.IdCliente={0}", idCliente),
                    Configuracoes.PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista ? " || dac.DescontoAVista > 0" : string.Empty, idProd)) > 0;
        }

        #endregion

        #region Relatório de clientes com desconto/acréscimo

        private string SqlRpt(uint idCliente, string nomeCliente, uint idGrupoProd, uint idSubgrupoProd, string codInternoProd,
            string descrProd, uint idRota, uint idVendedor, uint idLoja, SituacaoCliente situacao, bool selecionar)
        {
            string campos = selecionar ? "dac.*, c.nome as nomeCliente, g.descricao as descrGrupo, s.descricao as descrSubgrupo, " +
                "p.descricao as descrProduto, '$$$' as criterio" : "count(*)";

            var sql = string.Format(@"SELECT {0}
                FROM desconto_acrescimo_cliente dac
                    INNER JOIN cliente c ON (dac.IdCliente=c.Id_Cli)
                    {1}
                    {2}
                    INNER JOIN grupo_prod g ON (dac.IdGrupoProd=g.IdGrupoProd)
                    LEFT JOIN subgrupo_prod s ON (dac.IdSubgrupoProd=s.IdSubgrupoProd)
                    LEFT JOIN produto p On (dac.IdProd=p.IdProd)
                WHERE (Desconto>0 OR DescontoAVista>0 OR Acrescimo>0)", campos,
                idVendedor > 0 ? "INNER JOIN funcionario func ON (c.IdFunc=func.IdFunc)" : string.Empty,
                idRota > 0 ? "INNER JOIN rota_cliente rotaCli ON (dac.IdCliente=rotaCli.IdCliente)" : string.Empty);

            string criterio = "";

            if (idCliente > 0)
            {
                sql += " and dac.idCliente=" + idCliente;
                criterio += "   Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "   Cliente: " + nomeCliente + "    ";
            }

            if (idGrupoProd > 0)
            {
                sql += " and dac.idGrupoProd=" + idGrupoProd;
                criterio += "   Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (idSubgrupoProd > 0)
            {
                sql += " and dac.idSubgrupoProd=" + idSubgrupoProd;
                criterio += "   Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd) + "    ";
            }

            if (!String.IsNullOrEmpty(codInternoProd))
            {
                sql += " and p.codInterno=?codInternoProd";
                criterio += "   Produto: " + ProdutoDAO.Instance.ObtemDescricao(codInternoProd) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                sql += " and p.idProd In (" + ids + ")";
                criterio += "   Produto: " + descrProd;
            }

            if (idRota > 0)
            {
                sql += " and rotaCli.idRota=" + idRota;
                criterio += "   Rota: " + RotaDAO.Instance.GetElement(idRota).Descricao;
            }

            if (idVendedor > 0)
            {
                sql += " and func.idfunc=" + idVendedor;
                criterio += "   Vendedor: " + FuncionarioDAO.Instance.GetNome(idVendedor);
            }

            if (idLoja > 0)
            {
                sql += " and c.Id_loja=" + idLoja;
                criterio += "   Loja: " + LojaDAO.Instance.GetNome(idLoja);
            }

            if (situacao > 0)
            {
                sql += " and c.Situacao=" + (int)situacao;
                criterio += "   Situação: " + new Cliente() { Situacao = (int)situacao }.DescrSituacao;
            }

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParamsRpt(string nomeCliente, string codInternoProd, string descrProd)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(codInternoProd))
                lst.Add(new GDAParameter("?codInternoProd", codInternoProd));

            if (!String.IsNullOrEmpty(descrProd))
                lst.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            return lst.ToArray();
        }

        public IList<DescontoAcrescimoCliente> GetListRpt(uint idCliente, string nomeCliente, uint idGrupoProd, uint idSubgrupoProd, string codInternoProd,
            string descrProd, uint idRota, uint idVendedor, uint idLoja, SituacaoCliente situacao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlRpt(idCliente, nomeCliente, idGrupoProd, idSubgrupoProd, codInternoProd, descrProd, 
                idRota, idVendedor, idLoja, situacao, true), 
                sortExpression, startRow, pageSize, GetParamsRpt(nomeCliente, codInternoProd, descrProd));
        }

        public int GetCountRpt(uint idCliente, string nomeCliente, uint idGrupoProd, uint idSubgrupoProd, string codInternoProd,
            string descrProd, uint idRota, uint idVendedor, uint idLoja, SituacaoCliente situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlRpt(idCliente, nomeCliente, idGrupoProd, idSubgrupoProd, codInternoProd, descrProd,
                idRota, idVendedor, idLoja, situacao, false), GetParamsRpt(nomeCliente, codInternoProd, descrProd));
        }

        public IList<DescontoAcrescimoCliente> GetForRpt(uint idCliente, string nomeCliente, uint idGrupoProd, uint idSubgrupoProd, string codInternoProd,
            string descrProd, uint idRota, uint idVendedor, uint idLoja, SituacaoCliente situacao)
        {
            return objPersistence.LoadData(SqlRpt(idCliente, nomeCliente, idGrupoProd, idSubgrupoProd, codInternoProd, descrProd,
                idRota, idVendedor, idLoja, situacao, true), GetParamsRpt(nomeCliente, codInternoProd, descrProd)).ToList();
        }

        #endregion

        #region Copiar desconto/acréscimo entre clientes

        private static readonly object _copiarLock = new object();

        /// <summary>
        /// Copiar desconto/acréscimo entre clientes.
        /// </summary>
        public void Copiar(uint idClienteAtual, uint idClienteNovo)
        {
            lock(_copiarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (ClienteDAO.Instance.ObtemTabelaDescontoAcrescimo(transaction, idClienteAtual) > 0 ||
                            ClienteDAO.Instance.ObtemTabelaDescontoAcrescimo(transaction, idClienteNovo) > 0)
                            throw new Exception("Não é possível copiar descontos/acréscimos entre clientes, caso pelo menos um possua referência de tabela de desconto/acréscimo.");

                        // Clona os descontos do cliente atual para o novo
                        var atuais = objPersistence.LoadData(transaction, "select * from desconto_acrescimo_cliente where idCliente=" + idClienteAtual).ToList();

                        foreach (DescontoAcrescimoCliente d in atuais)
                        {
                            InsertOrUpdate(transaction, idClienteNovo, null, (uint)d.IdGrupoProd, (uint?)d.IdSubgrupoProd, (uint?)d.IdProduto, (float)d.Desconto,
                                (float)d.Acrescimo, d.AplicarBeneficiamentos);
                        }

                        // Apaga os descontos do cliente novo que não existem no atual
                        var naoExistem = objPersistence.LoadData(transaction, @"
                            select * from desconto_acrescimo_cliente
                            where concat(coalesce(idGrupoProd, 0), ',', coalesce(idSubgrupoProd, 0), ',', coalesce(idProd, 0)) not in (
                                select concat(coalesce(idGrupoProd, 0), ',', coalesce(idSubgrupoProd, 0), ',', coalesce(idProd, 0))
                                from desconto_acrescimo_cliente where idCliente=" + idClienteAtual + @"
                            ) and idCliente=" + idClienteNovo).ToList();

                        foreach (DescontoAcrescimoCliente d in naoExistem)
                        {
                            InsertOrUpdate(transaction, idClienteNovo, null, (uint)d.IdGrupoProd, (uint?)d.IdSubgrupoProd, (uint?)d.IdProduto, 0, 0, false);
                        }

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
        }

        #endregion

        #region Métodos sobescritos

        /// <summary>
        /// Insere desconto dado sobre determinado grupo de produto a determinado cliente
        /// </summary>
        public override uint Insert(DescontoAcrescimoCliente objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Insere desconto dado sobre determinado grupo de produto a determinado cliente
        /// </summary>
        public override uint Insert(GDASession session, DescontoAcrescimoCliente objInsert)
        {
            if (objInsert.IdTabelaDesconto > 0 && objInsert.IdCliente > 0)
            {
                ErroDAO.Instance.InserirFromException(string.Format("SalvarDescontoAcrescimo - IdDesconto: {0} | IdTabelaDesconto: {1} | IdCliente: {2}",
                    objInsert.IdDesconto, objInsert.IdTabelaDesconto, objInsert.IdCliente), new Exception());

                throw new Exception("Não é possível salvar um acréscimo/desconto com cliente e tabela associados.");
            }

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Atualiza desconto dado sobre determinado grupo de produto a determinado cliente
        /// </summary>
        public override int Update(DescontoAcrescimoCliente objUpdate)
        {
            return Update(null, objUpdate);
        }

        /// <summary>
        /// Atualiza desconto dado sobre determinado grupo de produto a determinado cliente
        /// </summary>
        public override int Update(GDASession session, DescontoAcrescimoCliente objUpdate)
        {
            if (objUpdate.IdTabelaDesconto > 0 && objUpdate.IdCliente > 0)
            {
                ErroDAO.Instance.InserirFromException(string.Format("SalvarDescontoAcrescimo - IdDesconto: {0} | IdTabelaDesconto: {1} | IdCliente: {2}",
                    objUpdate.IdDesconto, objUpdate.IdTabelaDesconto, objUpdate.IdCliente), new Exception());

                throw new Exception("Não é possível salvar um acréscimo/desconto com cliente e tabela associados.");
            }

            return base.Update(session, objUpdate);
        }

        #endregion
    }
}
