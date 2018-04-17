using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class BenefConfigDAO : BaseDAO<BenefConfig, BenefConfigDAO>
    {
        //private BenefConfigDAO() { }

        private string Sql(uint idBenefConfig, uint idParent, int espessuraExcluir, bool soPais, bool ordenar, bool soCalculaveis,
            bool apenasAtivos, int tipoBenef, bool selecionar)
        {
            var campos = selecionar ? @"b.*, coalesce(n.numChild, 0) as NumChild, ea.codInterno as codAplicacao, 
                ep.codInterno as codProcesso, p.descricao as descrParent, p.tipoControle as tipoControleParent, 
                p.tipoCalculo as tipoCalculoParent" : "Count(*)";

            var sql = "Select " + campos + @" From benef_config b
                Left Join (select Max(numSeq) as numSeqMax, Min(numSeq) as numSeqMin From benef_config) as ns On (1)
                Left Join (select idParent, count(*) as numChild from benef_config group by idParent) as n On (b.IdBenefConfig=n.IdParent) 
                Left Join benef_config p On (b.idParent=p.idBenefConfig)
                Left Join etiqueta_aplicacao ea On (b.idAplicacao=ea.idAplicacao)
                Left Join etiqueta_processo ep On (b.idProcesso=ep.idProcesso)
                Where 1";

            if (idBenefConfig > 0)
                sql += " And b.idBenefConfig=" + idBenefConfig;

            if (apenasAtivos)
                sql += " And b.situacao=" + (int)Situacao.Ativo;

            if (soCalculaveis)
                sql += " and coalesce(n.numChild,0)=0";
            else if (idParent > 0 && espessuraExcluir > 0)
                sql += " and b.idParent=" + idParent + " and b.TipoEspessura <> " + espessuraExcluir;
            else if (soPais)
                sql += " and b.IdParent is null";

            if (tipoBenef > 0)
                sql += " AND (COALESCE(b.tipoBenef, 0)=0 OR COALESCE(b.tipoBenef, 0)= " + tipoBenef + ")";

            if (ordenar)
                sql += " order by NumSeq asc, idBenefConfig asc";

            return sql;
        }

        /// <summary>
        /// Retorna os beneficiamentos filhos de um beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public IList<BenefConfig> GetByBenefConfig(uint idBenefConfig)
        {
            return GetByBenefConfig(idBenefConfig, false);
        }

        /// <summary>
        /// Retorna os beneficiamentos filhos de um beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public IList<BenefConfig> GetByBenefConfig(uint idBenefConfig, bool soEspessura)
        {
            return objPersistence.LoadData(Sql(0, idBenefConfig, !soEspessura ? (int)TipoEspessuraBenef.ItemEEspessura :
                (int)TipoEspessuraBenef.ItemPossui, false, true, false, true, 0, true)).ToList();
        }

        /// <summary>
        /// Busca o id de um beneficiamento pela sua descrição.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="descricao"></param>
        /// <param name="descricaoParent"></param>
        /// <returns></returns>
        public uint? GetIdByDescricao(string descricao, string descricaoParent)
        {
            var sql = @"select bc.* from benef_config bc 
                left join benef_config pai on (bc.idParent=pai.idBenefConfig) 
                where replace(bc.descricao, ';', '')=?descr";

            if (!String.IsNullOrEmpty(descricaoParent))
                sql += " and replace(pai.descricao, ';', '')=?descrParent";

            var item = objPersistence.LoadData(sql, new GDAParameter("?descr", descricao), new GDAParameter("?descrParent", descricaoParent)).ToList<BenefConfig>();
            return item.Count > 0 && item[0] != null ? (uint?)item[0].IdBenefConfig : null;
        }

        /// <summary>
        /// Busca o idProd de um beneficiamento pelo seu id.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public uint? ObtemIdProd(uint idBenefConfig)
        {
            return ObtemValorCampo<uint?>("idProd", "idBenefConfig=" + idBenefConfig);
        }

        private BenefConfig[] GetChildItens(BenefConfig b)
        {
            if (b.NumChild == 0)
                return new BenefConfig[] { b };
            else
            {
                var temp = new List<BenefConfig>();
                foreach (var bc in GetByBenefConfig((uint)b.IdBenefConfig, true))
                    temp.AddRange(GetChildItens(bc));

                return temp.ToArray();
            }
        }

        /// <summary>
        /// Retorna os beneficiamentos filhos que são realmente aplicados a um produto.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public BenefConfig[] GetByBenefConfigItens(uint idBenefConfig)
        {
            var retorno = new List<BenefConfig>();

            var temp = GetByBenefConfig(idBenefConfig);
            if (temp.Count > 0)
            {
                for (var i = 0; i < temp.Count; i++)
                    retorno.AddRange(GetChildItens(temp[i]));
            }
            else
                retorno = GetByBenefConfig(idBenefConfig, true).ToList<BenefConfig>();

            return retorno.ToArray();
        }

        /// <summary>
        /// Verifica se o beneficiamento é usado em alguma parte do sistema.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public bool BenefConfigUsado(uint idBenefConfig)
        {
            foreach (var t in GenericBenefCollection.GetTabelas())
                if (objPersistence.ExecuteSqlQueryCount("select count(*) from " + t + " t left join benef_config bc on (t.idBenefConfig=" +
                    "bc.idBenefConfig) where t.idBenefConfig=" + idBenefConfig + " or bc.idParent=" + idBenefConfig) > 0)
                    return true;

            return false;
        }

        #region Métodos para o controle de beneficiamentos

        public uint GetTypeID(uint idBenefConfig)
        {
            var sql = "select coalesce(IdParent, idBenefConfig) from benef_config where idBenefConfig=" + idBenefConfig;
            var idParent = ExecuteScalar<uint>(sql);

            var parentId = idBenefConfig;
            sql = "select IdParent from benef_config where idBenefConfig=?id";

            var id = new uint?();
            do
            {
                if (id > 0)
                    parentId = id.Value;

                id = ExecuteScalar<uint?>(sql, new GDAParameter("?id", parentId));
                if (id == null)
                    break;
            }
            while (id != idParent);

            return parentId;
        }

        /// <summary>
        /// Retorna os beneficiamentos que serão exibidos no controle.
        /// </summary>
        /// <returns></returns>
        public IList<BenefConfig> GetForControl(TipoBenef tipoBenef)
        {
            return objPersistence.LoadData(Sql(0, 0, 0, true, true, false, true, (int)tipoBenef, true)).ToList();
        }

        public IList<BenefConfig> GetForControl()
        {
            return objPersistence.LoadData(Sql(0, 0, 0, true, true, false, true, 0, true)).ToList();
        }

        private BenefConfig GetParent(List<BenefConfig> lista, BenefConfig item)
        {
            return lista.Find(new Predicate<BenefConfig>(delegate(BenefConfig b)
            {
                return b.IdBenefConfig == item.IdParent;

            }));
        }

        public IList<BenefConfig> GetForConfig()
        {
            return objPersistence.LoadData(Sql(0, 0, 0, false, false, true, true, 0, true)).ToList();
        }

        #endregion

        #region Busca para parte de criação de controle

        /// <summary>
        /// Verifica se o beneficiamento é cobrado por espessura do vidro
        /// </summary>
        public bool CobrarPorEspessura(uint idBenefConfig)
        {
            return CobrarPorEspessura(null, idBenefConfig);
        }

        /// <summary>
        /// Verifica se o beneficiamento é cobrado por espessura do vidro
        /// </summary>
        public bool CobrarPorEspessura(GDASession session, uint idBenefConfig)
        {
            var sql = "Select Count(*) From benef_config_preco Where espessura is not null and idBenefConfig in (" +
                "select idBenefConfig from benef_config where idBenefConfig=" + idBenefConfig + " or idParent=" + idBenefConfig + ")";
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se o beneficiamento é cobrado por cor do vidro
        /// </summary>
        public bool CobrarPorCor(uint idBenefConfig)
        {
            return CobrarPorCor(null, idBenefConfig);
        }

        /// <summary>
        /// Verifica se o beneficiamento é cobrado por cor do vidro
        /// </summary>
        public bool CobrarPorCor(GDASession session, uint idBenefConfig)
        {
            var sql = "Select Count(*) From benef_config_preco Where idCorVidro is not null and idBenefConfig in (" +
                "select idBenefConfig from benef_config where idBenefConfig=" + idBenefConfig + " or idParent=" + idBenefConfig + ")";
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se o beneficiamento é usado para cobrar área mínima.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public bool CobrarAreaMinima(uint idBenefConfig)
        {
            return CobrarAreaMinima(null, idBenefConfig.ToString()) > 0;
        }

        /// <summary>
        /// Verifica se o beneficiamento é usado para cobrar área mínima.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public int CobrarAreaMinima(GDASession sessao, string idsBenefConfig)
        {
            if (String.IsNullOrEmpty(idsBenefConfig))
                return 0;

            var sql = "Select Count(*) From benef_config Where cobrarAreaMinima=true and ((idBenefConfig in (" +
                idsBenefConfig + ") and idParent is null) or idBenefConfig in (select idParent from benef_config where idBenefConfig in (" + idsBenefConfig + ")))";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Recupera o subgrupo que o beneficiamento usa para cobrar separadamente.
        /// </summary>
        public uint? GetSubgrupoProd(uint idBenefConfig)
        {
            return GetSubgrupoProd(null, idBenefConfig);
        }

        /// <summary>
        /// Recupera o subgrupo que o beneficiamento usa para cobrar separadamente.
        /// </summary>
        public uint? GetSubgrupoProd(GDASession session, uint idBenefConfig)
        {
            var sql = "Select idSubgrupoProd From benef_config_preco Where idSubgrupoProd is not null and idBenefConfig in (" +
                "select idBenefConfig from benef_config where idBenefConfig=" + idBenefConfig + " or idParent=" + idBenefConfig + ")";

            return ExecuteScalar<uint?>(session, sql);
        }

        public BenefConfig GetElement(uint idBenefConfig)
        {
            return GetElement(null, idBenefConfig);
        }

        public BenefConfig GetElement(GDASession session, uint idBenefConfig)
        {
            var benef = objPersistence.LoadData(session, Sql(idBenefConfig, 0, 0, false, false, false, false, 0, true)).ToList<BenefConfig>();
            if (benef.Count == 0)
                return null;

            // Verifica se este beneficiamento é cobrado por espessura
            benef[0].CobrarPorEspessura = CobrarPorEspessura(session, idBenefConfig);

            // Verifica se este beneficiamento é cobrado por cor
            benef[0].CobrarPorCor = CobrarPorCor(session, idBenefConfig);

            // Recupera o subgrupo do produto
            benef[0].IdSubgrupoProd = GetSubgrupoProd(session, idBenefConfig);

            return benef[0];
        }

        public BenefConfig[] GetOpcoes(uint idParent)
        {
            return objPersistence.LoadData(Sql(0, idParent, 2, false, false, false, true, 0, true)).ToList().ToArray();
        }

        public IList<BenefConfig> GetList(string sortExpression, int startRow, int pageSize)
        {
            var sort = String.IsNullOrEmpty(sortExpression) ? "NumSeq" : sortExpression;
            return LoadDataWithSortExpression(Sql(0, 0, 0, true, false, false, false, 0, true), sort, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, 0, true, false, false, false, 0, false), null);
        }

        #endregion

        #region Busca para parte de atualização de valores

        public string SqlValor(string descricao, bool selecionar)
        {
            var campos = selecionar ? "bc.*, if(bc.idParent>0, if(bc.tipoEspessura=2, Concat(Coalesce(bcavo.Descricao, ''), ' ', bc.Descricao), " +
                "Concat(bcpai.Descricao, ' ', bc.Descricao)), bc.Descricao) as descrBenefValor" : "Count(*)";

            var sql = "Select " + campos + " From benef_config bc " +
                "Left Join benef_config bcpai On (bc.idParent=bcpai.idBenefConfig) " +
                "Left Join benef_config bcavo On (bcpai.idParent=bcavo.idBenefConfig) " +
                "Where (bc.tipoEspessura=2 Or (bc.tipoEspessura<>1 And bc.tipoControle in (3, 7)) Or " +
                "(bc.tipoEspessura=0 And bc.idParent is not null)) And Coalesce(bcavo.situacao, bcpai.situacao, bc.situacao)=" +
                (int)Situacao.Ativo;

            if (!String.IsNullOrEmpty(descricao))
                sql += " And Concat(Coalesce(bcavo.descricao, bcpai.descricao, ''), ' ', bc.descricao) like ?descricao";

            if (selecionar)
                sql += " Order By if(bc.idParent>0, if(bc.tipoEspessura=2, Concat(Coalesce(bcavo.Descricao, bcpai.descricao), ' ', bc.Descricao), " +
                    "Concat(bcpai.Descricao, ' ', bc.Descricao)), bc.Descricao)";

            return sql;
        }

        public IList<BenefConfig> GetListValor(string descricao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlValor(descricao, true), sortExpression, startRow, pageSize, GetParamValor(descricao));
        }

        public int GetCountValor(string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlValor(descricao, false), GetParamValor(descricao));
        }

        private GDAParameter[] GetParamValor(string descricao)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("descricao", "%" + descricao + "%"));

            return lstParam.ToArray();
        }

        #endregion

        #region Troca posição do beneficiamento

        /// <summary>
        /// Troca a posição de um beneficiamento
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="acima"></param>
        public void ChangePosition(uint idBenefConfig, bool acima)
        {
            var numSeq = ObtemValorCampo<int>("numSeq", "idBenefConfig=" + idBenefConfig);

            // Altera a posição do beneficiamento adjacente à este
            objPersistence.ExecuteCommand("Update benef_config Set numSeq=numSeq" + (acima ? "+1" : "-1") +
                " Where numSeq=" + (numSeq + (acima ? -1 : 1)));

            // Altera a posição deste beneficiamento
            objPersistence.ExecuteCommand("Update benef_config Set numSeq=numSeq" + (acima ? "-1" : "+1") +
                " Where idBenefConfig=" + idBenefConfig);
        }

        #endregion

        /// <summary>
        /// Retorna o próximo número de sequência a ser utilizado
        /// </summary>
        /// <returns></returns>
        private int GetNumSeq()
        {
            var sql = "Select Coalesce(Max(NumSeq) + 1, 1) From benef_config";
            return ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Verifica se o beneficiamento "Redondo" é cobrado.
        /// </summary>
        /// <returns></returns>
        public bool CobrarRedondo()
        {
            var sql = "select count(*) from benef_config where (lower(descricao)='redondo' or lower(nome)='redondo') " +
                "and tipoCalculo is not null and tipoCalculo>0";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se um dos beneficiamentos selecionados é o 'Redondo'.
        /// </summary>
        public bool TemBenefRedondo(string idsBenefConfig)
        {
            return TemBenefRedondo(idsBenefConfig?.Split(',')?.Select(f => f.StrParaInt()) ?? new List<int>());
        }

        /// <summary>
        /// Verifica se um dos beneficiamentos selecionados é o 'Redondo'.
        /// </summary>
        public bool TemBenefRedondo(IEnumerable<int> idsBenefConfig)
        {
            if (!(idsBenefConfig?.Any(f => f > 0)).GetValueOrDefault())
            {
                return false;
            }

            var sql = string.Format("SELECT COUNT(*) FROM benef_config WHERE (LOWER(Descricao)='redondo' OR LOWER(Nome)='redondo') AND IdBenefConfig IN ({0})", idsBenefConfig);

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Retorna uma string com a descrição de todos os beneficiamentos
        /// </summary>
        public string GetDescrBenef(string idsBenefConfig)
        {
            return GetDescrBenef(null, idsBenefConfig);
        }

        /// <summary>
        /// Retorna uma string com a descrição de todos os beneficiamentos
        /// </summary>
        public string GetDescrBenef(GDASession session, string idsBenefConfig)
        {
            var retorno = String.Empty;

            var ids = idsBenefConfig.Split(',');
            foreach (var s in ids)
            {
                try
                {
                    var id = new uint();
                    if (uint.TryParse(s.Trim(), out id))
                        retorno += GetElement(session, id).DescricaoCompleta + ", ";
                }
                catch { }
            }

            return retorno.TrimEnd(',', ' ');
        }

        /// <summary>
        /// Não exibir a descrição do beneficiamento na impressão da etiqueta
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public bool NaoExibirDescrImpEtiqueta(GDASession session, uint idBenefConfig)
        {
            // Verifica se este idBenefConfig possui um pai, se tiver deve verificar se ele está ou não com esta opção marcada
            // uma vez que não é possível marcar se os filhos do beneficiamento serão exibidos ou não   
            uint? idParent = ObtemValorCampo<uint?>(session, "idParent", "idBenefConfig=" + idBenefConfig);

            return ObtemValorCampo<bool>(session, "naoExibirEtiqueta", "idBenefConfig=" + (idParent > 0 ? idParent : idBenefConfig));
        }

        #region Relatório

        private string SqlPeriodoLoja(string dtIni, string dtFim, uint idLoja, uint idFunc, bool selecionar)
        {
            var campos = selecionar ?
                @"b.*, cast(sum(ppb.Valor) as decimal(12,2)) as SumValor, sum(pp.TotM) as SumTotM,
                /* Chamado 13216. A quantidade de beneficiamentos no relatório deve ser a quantidade no pedido.
                count(b.idBenefConfig)*/ SUM(pp.Qtde) as QtdBenef" :
                "count(distinct b.Descricao)";

            var sql = @"
                Select " + campos + @"
                From benef_config b 
                    Left join produto_pedido_benef ppb on (b.idBenefConfig=ppb.idBenefConfig) 
                    Left join produtos_pedido pp on (ppb.idProdPed=pp.idProdPed) 
                    Left join pedido p on (pp.idPedido=p.idPedido) 
                Where p.IdLoja=" + idLoja;

            var dateFormat = "str_to_date('{0} {1}', '%d/%m/%Y %H:%i')";

            if (idFunc > 0)
                sql += " And p.idFunc=" + idFunc;

            if (!PedidoConfig.LiberarPedido)
            {
                if (!String.IsNullOrEmpty(dtIni))
                    sql += " and p.DataConf >= " + String.Format(dateFormat, dtIni, "00:00");

                if (!String.IsNullOrEmpty(dtFim))
                    sql += " and p.DataConf <= " + String.Format(dateFormat, dtFim, "23:59");
            }
            else
            {
                if (!String.IsNullOrEmpty(dtIni))
                    sql += @" and p.idPedido in (select * from (
                        select plp.idPedido from produtos_liberar_pedido plp 
                        left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                        where lp.dataLiberacao >= " + String.Format(dateFormat, dtIni, "00:00") + @"
                        " + (!String.IsNullOrEmpty(dtFim) ? " and lp.dataLiberacao <= " + String.Format(dateFormat, dtFim, "23:59") : "") + ") as temp)";

                else if (!String.IsNullOrEmpty(dtFim))
                    sql += @" and p.idPedido in (select * from (
                        select plp.idPedido from produtos_liberar_pedido plp 
                        left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                        where lp.dataLiberacao <= " + String.Format(dateFormat, dtFim, "23:59") + ") as temp)";
            }

            if (selecionar)
                sql += " group by b.Descricao";

            return sql;
        }

        /// <summary>
        /// Retorna uma lista com os beneficiamentos feitos durante um período em uma loja.
        /// </summary>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<BenefConfig> GetBenefByPeriodoLoja(string dtIni, string dtFim, uint idLoja, uint idFunc, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlPeriodoLoja(dtIni, dtFim, idLoja, idFunc, true), sortExpression, startRow, pageSize, null);
        }

        /// <summary>
        /// Retorna o número de beneficiamentos feitos em uma loja durante um período.
        /// </summary>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public int GetBenefByPeriodoLojaCount(string dtIni, string dtFim, uint idLoja, uint idFunc)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlPeriodoLoja(dtIni, dtFim, idLoja, idFunc, false));
        }

        /// <summary>
        /// Retorna uma lista com os beneficiamentos feitos durante um período em uma loja para impressão.
        /// </summary>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<BenefConfig> GetForRpt(string dtIni, string dtFim, uint idLoja, uint idFunc)
        {
            return objPersistence.LoadData(SqlPeriodoLoja(dtIni, dtFim, idLoja, idFunc, true)).ToList();
        }

        #endregion

        #region Insere os preços dos beneficiamentos na tabela

        /// <summary>
        /// Verifica se o preço deve ser inserido.
        /// </summary>
        /// <param name="idBenefConfig">O id do beneficiamento.</param>
        /// <param name="espessura">A espessura referente ao preço.</param>
        /// <returns></returns>
        private bool InserirPreco(uint idBenefConfig, float espessura, uint? idSubgrupoProd, uint? idCorVidro)
        {
            var where = new Func<float?, string>(x => x > 0 ? "=" + x : " is null");

            var sql = "select count(*) from benef_config_preco where idBenefConfig=" + idBenefConfig + " and espessura" +
                where(espessura) + " and idSubgrupoProd" + where(idSubgrupoProd) + " and idCorVidro" + where(idCorVidro);

            return objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        /// <summary>
        /// Insere os preços dos beneficiamentos na tabela.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="cobrarCor"></param>
        /// <param name="cobrarEspessura"></param>
        /// <param name="idSubgrupoProd"></param>
        /// <param name="lstOpcoes"></param>
        private void InserirPrecos(uint idBenefConfig, bool cobrarCor, bool cobrarEspessura, uint? idSubgrupoProd, BenefConfig[] lstOpcoes)
        {
            var benefPreco = new BenefConfigPreco();

            var vetEsp = cobrarEspessura ? Glass.Medidas.ObtemEspessuras.Split(',') : new[] { "0" };
            var vetSubgrupo = idSubgrupoProd != null ? new[] { null, (int?)idSubgrupoProd } : new int?[] { null };
            var vetCor = (cobrarCor ? CorVidroDAO.Instance.GetAll().Select(x => (int?)x.IdCorVidro) : new int?[0]).Union(new int?[] { null }).ToArray();

            // Se não houver lista de opções, cria um item com o ID do beneficiamento para inserir
            var opcoes = lstOpcoes.Length > 0 ? lstOpcoes :
                new BenefConfig[] {
                    new BenefConfig()
                    {
                        IdBenefConfig = (int)idBenefConfig
                    }
                };

            // Para cada beneficiamento da lista de opções
            foreach (var bc in opcoes)
            {
                // Para cada espessura gera um novo registro deste beneficiamento
                foreach (var esp in vetEsp)
                {
                    float espessura = Glass.Conversoes.StrParaFloat(esp);

                    // Para cada subgrupo gera um novo registro deste beneficiamento
                    foreach (var sub in vetSubgrupo)
                    {
                        // Para cada cor gera um novo registro deste beneficiamento
                        foreach (var cor in vetCor)
                        {
                            if (InserirPreco((uint)bc.IdBenefConfig, espessura, (uint?)sub, (uint?)cor))
                            {
                                benefPreco = new BenefConfigPreco();
                                benefPreco.IdBenefConfig = bc.IdBenefConfig;

                                if (cobrarEspessura)
                                    benefPreco.Espessura = espessura;

                                benefPreco.IdCorVidro = cor;
                                benefPreco.IdSubgrupoProd = sub;

                                BenefConfigPrecoDAO.Instance.Insert(benefPreco);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Recupera o valor de tabela de um beneficiamento

        /// <summary>
        /// Recupera o valor de tabela de um beneficiamento.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal GetValorTabela(uint idBenefConfig, int? tipoEntrega, uint? idCliente)
        {
            return GetValorTabela(idBenefConfig, null, null, null, tipoEntrega, idCliente);
        }

        /// <summary>
        /// Recupera o valor de tabela de um beneficiamento.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal GetValorTabela(uint idBenefConfig, int? idSubgrupoProd, int? idCorVidro, int? espessura, int? tipoEntrega, uint? idCliente)
        {
            var benef = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(null, idBenefConfig, idSubgrupoProd, idCorVidro, espessura);
            return GetValorTabela(benef, tipoEntrega, idCliente);
        }

        /// <summary>
        /// Recupera o valor de tabela de um beneficiamento.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal GetValorTabela(BenefConfigPreco benefPreco, int? tipoEntrega, uint? idCliente)
        {
            if (ClienteDAO.Instance.IsRevenda(null, idCliente))
                return benefPreco.ValorAtacado;

            if (tipoEntrega == null)
                tipoEntrega = 1;

            switch (tipoEntrega)
            {
                case 1: // Balcão
                case 4: // Entrega
                    return benefPreco.ValorBalcao;

                default:
                    return benefPreco.ValorObra;
            }
        }

        #endregion

        #region Recupera o beneficiamento pelo nome e código do pai

        /// <summary>
        /// Recupera o beneficiamento pelo nome e código do pai.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="idParent"></param>
        /// <returns></returns>
        public BenefConfig GetByNomeParent(string nome, uint idParent)
        {
            var sql = "select * from benef_config where idParent=" + idParent + " and (nome=?nome or descricao=?nome)";
            var itens = objPersistence.LoadData(sql, new GDAParameter("?nome", nome)).ToList();
            return itens.Count > 0 ? itens[0] : null;
        }

        #endregion

        #region Recupera Id do pai pelo id do filho

        public uint GetParentId(uint idBenefConfig)
        {
            var sql = "select coalesce(IdParent, idBenefConfig) from benef_config where idBenefConfig=" + idBenefConfig;
            return ExecuteScalar<uint>(sql);
        }

        #endregion

        #region Métodos sobrescritos

        private BenefConfig[] GetOpcoes(uint idBenefConfig, BenefConfig benefConfig, bool procurarAntesDeAdicionar)
        {
            var benef = new BenefConfig();
            var lstOpcoes = new List<BenefConfig>();

            // Se o controle possuir lista de opções
            if (benefConfig.TipoControle != TipoControleBenef.SelecaoSimples && benefConfig.TipoControle != TipoControleBenef.Quantidade)
            {
                // Insere a lista de opções
                var vetOpcoes = benefConfig.ListaSelecao.TrimEnd('|').Split('|');
                var vetItens = benefConfig.ListaItens.TrimEnd('|').Split('|');

                for (var i = 0; i < vetOpcoes.Length; i++)
                {
                    benef = new BenefConfig();
                    benef.IdParent = (int)idBenefConfig;
                    benef.Nome = vetOpcoes[i];
                    benef.Descricao = benef.Nome;
                    benef.TipoEspessura = benefConfig.CobrarPorEspessura ? TipoEspessuraBenef.ItemPossui : TipoEspessuraBenef.ItemNaoPossui;
                    benef.Situacao = Glass.Situacao.Ativo;
                    benef.TipoCalculo = benefConfig.TipoCalculo;
                    benef.IdProcesso = vetItens[i].Split(';')[0].StrParaIntNullable();
                    benef.IdAplicacao = vetItens[i].Split(';')[1].StrParaIntNullable();
                    benef.IdProd = vetItens[i].Split(';')[2].StrParaIntNullable();
                    benef.AcrescimoAltura = Glass.Conversoes.StrParaInt(vetItens[i].Split(';')[5]);
                    benef.AcrescimoLargura = Glass.Conversoes.StrParaInt(vetItens[i].Split(';')[6]);

                    var id = new int();
                    if (procurarAntesDeAdicionar)
                    {
                        var item = GetByNomeParent(vetOpcoes[i], idBenefConfig) as BenefConfig;
                        id = item != null ? item.IdBenefConfig : 0;

                        if (item != null && (item.IdProcesso != benef.IdProcesso || item.IdAplicacao != benef.IdAplicacao ||
                            item.IdProd != benef.IdProd || item.AcrescimoAltura != benef.AcrescimoAltura ||
                            item.AcrescimoLargura != benef.AcrescimoLargura))
                        {
                            benef.IdBenefConfig = id;
                            base.Update(benef);
                        }
                    }

                    benef.IdBenefConfig = id == 0 ? (int)base.Insert(benef) : id;
                    lstOpcoes.Add(benef);
                }
            }

            return lstOpcoes.ToArray();
        }

        public override uint Insert(BenefConfig objInsert)
        {
            // Não permite que sejam inseridos beneficiamentos com o mesmo nome
            if (objPersistence.ExecuteSqlQueryCount(@"Select Count(*) From benef_config Where (nome=?nome Or descricao=?descricao) 
                and situacao=" + (int)Situacao.Ativo + " And idParent is null",
                new GDAParameter("?nome", objInsert.Nome), new GDAParameter("?descricao", objInsert.Descricao)) > 0)
                throw new Exception("Já foi inserido um beneficiamento com este nome/descrição.");

            // Não permite inserir beneficiamento que seja "Seleção simples" e que o cálculo seja "Qtd"
            if (objInsert.TipoControle == TipoControleBenef.SelecaoSimples && objInsert.TipoCalculo == TipoCalculoBenef.Quantidade)
                throw new Exception("Não é possível cadastrar beneficiamento que seja do tipo seleção simples e calculado por quantidade.");

            // Insere o beneficiamento
            objInsert.TipoEspessura = objInsert.CobrarPorEspessura ? ((objInsert.TipoControle !=  TipoControleBenef.SelecaoSimples && objInsert.TipoControle != TipoControleBenef.Quantidade) ? TipoEspessuraBenef.ItemNaoPossui : TipoEspessuraBenef.ItemPossui) : TipoEspessuraBenef.ItemNaoPossui;
            objInsert.Situacao = Glass.Situacao.Ativo;
            objInsert.NumSeq = GetNumSeq();
            var idBenefConfig = base.Insert(objInsert);

            var lstOpcoes = GetOpcoes(idBenefConfig, objInsert, false);

            try
            {
                BenefConfigPrecoDAO.Instance.DeleteByIdBenefConfig(idBenefConfig);
                InserirPrecos(idBenefConfig, objInsert.CobrarPorCor, objInsert.CobrarPorEspessura, objInsert.IdSubgrupoProd, lstOpcoes);
            }
            catch (Exception ex)
            {
                if (idBenefConfig > 0)
                    DeleteByPrimaryKey(idBenefConfig);

                foreach (var bc in lstOpcoes)
                    DeleteByPrimaryKey(bc.IdBenefConfig);

                BenefConfigPrecoDAO.Instance.DeleteByIdBenefConfig(idBenefConfig);

                throw ex;
            }

            return idBenefConfig;
        }

        public override int Update(BenefConfig objUpdate)
        {
            // Não permite que sejam inseridos beneficiamentos com o mesmo nome
            if (objPersistence.ExecuteSqlQueryCount(
                @"Select Count(*) From benef_config Where (nome=?nome Or descricao=?descricao) And idParent is null 
                and situacao=" + (int)Situacao.Ativo + " And idBenefConfig<>" + objUpdate.IdBenefConfig, 
                new GDAParameter("?nome", objUpdate.Nome), new GDAParameter("?descricao", objUpdate.Descricao)) > 0)
                throw new Exception("Já foi inserido um beneficiamento com este nome/descrição.");

            // Não permite inserir beneficiamento que seja "Seleção simples" e que o cálculo seja "Qtd"
            if (objUpdate.TipoControle == TipoControleBenef.SelecaoSimples && objUpdate.TipoCalculo == TipoCalculoBenef.Quantidade)
                throw new Exception("Não é possível cadastrar beneficiamento que seja do tipo seleção simples e calculado por quantidade.");

            // Pega o id de cada benef que estiver referenciado ao que está sendo atualizado
            var lstIds = objPersistence.LoadResult("Select idBenefConfig From benef_config Where idParent=" + objUpdate.IdBenefConfig, null).Select(f => f.GetUInt32(0)).ToList();

            if (lstIds.Count > 0)
            {
                var ids = String.Empty;
                foreach (var id in lstIds)
                    ids += id + ",";

                // Atualiza o tipo de cálculo de todos os beneficiamentos relacionados à este
                objPersistence.ExecuteCommand("Update benef_config set tipoCalculo=" + objUpdate.TipoCalculo +
                    " Where idParent=" + objUpdate.IdBenefConfig + (!String.IsNullOrEmpty(ids.TrimEnd(',')) ? " Or idParent In (" + ids.TrimEnd(',') + ")" : String.Empty));
            }

            LogAlteracaoDAO.Instance.LogBenefConfig(objUpdate);
            var retorno = base.Update(objUpdate);

            var originais = GetByBenefConfig((uint)objUpdate.IdBenefConfig).ToArray();
            var novos = GetOpcoes((uint)objUpdate.IdBenefConfig, objUpdate, true).ToArray();

            foreach (var o in originais)
            {
                var remover = true;

                foreach (var n in novos)
                    if (n.IdBenefConfig == o.IdBenefConfig)
                    {
                        remover = false;
                        break;
                    }

                if (remover)
                {
                    Delete(o);
                    BenefConfigPrecoDAO.Instance.DeleteByIdBenefConfig((uint)o.IdBenefConfig);
                }
            }

            // Verifica se há alteração de algum beneficiamento
            bool opcoesAlteradas = originais.Length != novos.Length;
            if (!opcoesAlteradas)
            {
                var idsOriginais = new List<uint>(Array.ConvertAll<BenefConfig, uint>(originais, new Converter<BenefConfig, uint>(
                    delegate(BenefConfig x)
                    {
                        return (uint)x.IdBenefConfig;
                    }
                )));

                foreach (var b in novos)
                {
                    if (!idsOriginais.Contains((uint)b.IdBenefConfig))
                    {
                        opcoesAlteradas = true;
                        break;
                    }
                }
            }

            // Verifica se houve mudança no tipo de cobrança do beneficiamento
            if (objUpdate.CobrarPorCor || objUpdate.CobrarPorEspessura || objUpdate.IdSubgrupoProd != null || opcoesAlteradas)
                InserirPrecos((uint)objUpdate.IdBenefConfig, objUpdate.CobrarPorCor, objUpdate.CobrarPorEspessura, objUpdate.IdSubgrupoProd, novos);

            return retorno;
        }

        public override int Delete(BenefConfig objDelete)
        {
            throw new NotSupportedException();
            /*// Verifica se este beneficiamento ou seus filhos estão sendo usados em alguma tabela, 
            // se estiverem, não permite que este beneficiamento seja excluído
            if (BenefConfigUsado((uint)objDelete.IdBenefConfig))
                objPersistence.ExecuteCommand("Update benef_config Set situacao=" + (int)Situacao.Inativo +
                    " Where idBenefConfig=" + objDelete.IdBenefConfig);
            else
            {
                // Pega os ids que serão excluídos
                var lstIds = objPersistence.LoadResult("Select idBenefConfig From benef_config Where idBenefConfig=" +
                    objDelete.IdBenefConfig + " Or idBenefConfig In (Select idBenefConfig From benef_config where idParent=" +
                    objDelete.IdBenefConfig + " Or idParent In (Select idBenefConfig From benef_config Where idParent=" + objDelete.IdBenefConfig + "))", null)
                    .Select(f => f.GetUInt32(0))
                       .ToList(); ;

                if (lstIds.Count > 0)
                {
                    var ids = String.Empty;
                    foreach (var id in lstIds)
                    {
                        LogAlteracaoDAO.Instance.ApagaLogBenefConfig(id);
                        ids += id + ",";
                    }

                    objPersistence.ExecuteCommand("Delete From benef_config Where idBenefConfig In (" + ids.TrimEnd(',') + ")");
                }
            }

            BenefConfigPrecoDAO.Instance.DeleteByIdBenefConfig((uint)objDelete.IdBenefConfig);
            return 1;*/
        }

        #endregion
    }
}