using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class BenefConfigPrecoDAO : BaseDAO<BenefConfigPreco, BenefConfigPrecoDAO>
    {
        //private BenefConfigPrecoDAO() { }

        private enum TipoCompararEspesura
        {
            Igual,
            MaiorIgual
        }

        private enum TipoPadrao
        {
            Nenhum,
            SoPadrao,
            SoNaoPadrao
        }

        private string Sql(uint idBenefConfig, int? idSubgrupoProd, int? idCorVidro, float? espessura, string descricao,
            TipoCompararEspesura tipoCompararEspessura, TipoPadrao tipoPadrao, bool apenasAtivos, bool selecionar)
        {
            string sql = @"
                Select bcp.*, concat(if(length(bc1.Descricao)>0, concat(bc1.Descricao, ' '), ''), bc.Descricao,
                    if(bcp.espessura is not null, concat(' ', if(bcp.espessura<10, '0', ''), cast(bcp.espessura as char), 'mm'), ''))
                    as Descricao, s.Descricao as DescrSubgrupoProd, cv.Descricao as DescrCorVidro,
                    bc.TipoCalculo
                From benef_config_preco bcp
                    left join benef_config bc on (bcp.idBenefConfig=bc.idBenefConfig)
                    left join benef_config bc1 on (bc.idParent=bc1.idBenefConfig)
                    left join subgrupo_prod s on (bcp.idSubgrupoProd=s.idSubgrupoProd)
                    left join cor_vidro cv on (bcp.idCorVidro=cv.idCorVidro)
                Where 1";

            if (apenasAtivos)
                sql += " And coalesce(bc1.situacao, bc.situacao)=" + (int)Situacao.Ativo;

            switch (tipoPadrao)
            {
                case TipoPadrao.SoNaoPadrao:
                    sql += " and (bcp.idSubgrupoProd is not null or bcp.idCorVidro is not null)";
                    break;

                case TipoPadrao.SoPadrao:
                    sql += " and bcp.idSubgrupoProd is null and bcp.idCorVidro is null";
                    break;
            }

            if (idBenefConfig > 0)
                sql += " and bcp.idBenefConfig=" + idBenefConfig;

            if (idSubgrupoProd > 0)
                sql += " and (bcp.idSubgrupoProd=" + idSubgrupoProd.Value + " or bcp.idSubgrupoProd is null)";

            if (idCorVidro > 0)
                sql += " and (bcp.idCorVidro=" + idCorVidro.Value + " or bcp.idCorVidro is null)";

            if (espessura.HasValue && espessura > 0)
                switch (tipoCompararEspessura)
                {
                    case TipoCompararEspesura.Igual:
                        sql += " and bcp.espessura=?esp";
                        break;

                    case TipoCompararEspesura.MaiorIgual:
                        sql += " and (bcp.espessura>=?esp or bcp.espessura is null)";
                        break;
                }

            sql += " order by if(bcp.espessura is null, 999, bcp.espessura) asc, if(bcp.idSubgrupoProd is null, 999, bcp.idSubgrupoProd) asc, " +
                "if(bcp.idCorVidro is null, 999, bcp.idCorVidro) asc, concat(bc1.Descricao, ' ', bc.Descricao)";

            sql = "select " + (selecionar ? "*" : "count(*)") + " from(" + sql + ") as temp where 1";
            if (!String.IsNullOrEmpty(descricao))
                sql += " and Descricao like ?descricao";

            return sql;
        }

        private GDAParameter[] GetParam(string descricao, float? espessura)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            if (espessura > 0)
                lstParam.Add(new GDAParameter("?esp", espessura));

            return lstParam.ToArray();
        }

        public IList<BenefConfigPreco> GetList(string descricao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;
            return LoadDataWithSortExpression(Sql(0, null, null, null, descricao, TipoCompararEspesura.MaiorIgual, TipoPadrao.SoPadrao, true, true), sortExpression, startRow, pageSize, GetParam(descricao, null));
        }

        public int GetListCount(string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, null, null, null, descricao, TipoCompararEspesura.MaiorIgual, TipoPadrao.SoPadrao, true, false), GetParam(descricao, null));
        }

        public IList<BenefConfigPreco> GetByIdBenefConfig(uint idBenefConfig)
        {
            return this.GetByIdBenefConfig(null, idBenefConfig);
        }

        public IList<BenefConfigPreco> GetByIdBenefConfig(GDASession sessao, uint idBenefConfig)
        {
            return objPersistence.LoadData(sessao, Sql(idBenefConfig, null, null, null, null, TipoCompararEspesura.MaiorIgual, TipoPadrao.Nenhum, false, true)).ToList();
        }

        public BenefConfigPreco GetByIdBenefConfig(GDASession session, uint idBenefConfig, uint idProd)
        {
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)idProd);
            var idCorVidro = ProdutoDAO.Instance.ObtemValorCampo<int?>(session, "idCorVidro", "idProd=" + idProd);
            float? espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)idProd);

            return GetByIdBenefConfig(session, idBenefConfig, idSubgrupoProd, idCorVidro, espessura);
        }

        public BenefConfigPreco GetByIdBenefConfig(uint idBenefConfig, int? idSubgrupoProd, int? idCorVidro, float? espessura)
        {
            return GetByIdBenefConfig(null, idBenefConfig, idSubgrupoProd, idCorVidro, espessura);
        }

        public BenefConfigPreco GetByIdBenefConfig(GDASession session, uint idBenefConfig, int? idSubgrupoProd, int? idCorVidro, float? espessura)
        {
            List<BenefConfigPreco> lstBenefPreco = objPersistence.LoadData(session, Sql(idBenefConfig, idSubgrupoProd, idCorVidro, espessura, null,
                TipoCompararEspesura.MaiorIgual, TipoPadrao.Nenhum, false, true), GetParam(null, espessura));

            return lstBenefPreco.Count > 0 ? lstBenefPreco[0] : new BenefConfigPreco();
        }

        public IList<BenefConfigPreco> GetForBenefConfigEdit(uint idBenefConfig, float? espessura)
        {
            return objPersistence.LoadData(Sql(idBenefConfig, null, null, espessura, null, TipoCompararEspesura.Igual,
                TipoPadrao.SoNaoPadrao, true, true), GetParam(null, espessura)).ToList();
        }

        public int GetForBenefConfigEditCount(uint idBenefConfig, float? espessura)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idBenefConfig, null, null, espessura, null,
                TipoCompararEspesura.Igual, TipoPadrao.SoNaoPadrao, true, false), GetParam(null, espessura));
        }

        private uint? GetIdBenefConfigPreco(BenefConfigPreco obj)
        {
            string sql = "select idBenefConfigPreco from benef_config_preco Where idBenefConfig=" + obj.IdBenefConfig +
                " and idSubgrupoProd" + (obj.IdSubgrupoProd != null ? "=" + obj.IdSubgrupoProd.Value : " is null") +
                " and idCorVidro" + (obj.IdCorVidro != null ? "=" + obj.IdCorVidro.Value : " is null") +
                " and espessura" + (obj.Espessura != null ? "=?esp" : " is null");

            return ExecuteScalar<uint?>(sql, GetParam(null, obj.Espessura));
        }

        public int UpdateValor(BenefConfigPreco objUpdate)
        {
            objUpdate.IdBenefConfigPreco = (int)GetIdBenefConfigPreco(objUpdate).GetValueOrDefault();

            if (objUpdate.IdBenefConfigPreco > 0)
                LogAlteracaoDAO.Instance.LogBenefConfigPreco(objUpdate);

            string sql = "Update benef_config_preco Set valorAtacado=" + objUpdate.ValorAtacado.ToString().Replace(',', '.') +
                ", valorBalcao=" + objUpdate.ValorBalcao.ToString().Replace(',', '.') + ", valorObra=" +
                objUpdate.ValorObra.ToString().Replace(',', '.') + ", custo=" + objUpdate.Custo.ToString().Replace(',', '.') +
                " Where idBenefConfig=" + objUpdate.IdBenefConfig + " and idSubgrupoProd" +
                (objUpdate.IdSubgrupoProd != null ? "=" + objUpdate.IdSubgrupoProd.Value : " is null") +
                " and idCorVidro" + (objUpdate.IdCorVidro != null ? "=" + objUpdate.IdCorVidro.Value : " is null") +
                " and espessura" + (objUpdate.Espessura != null ? "=?esp" : " is null");

            return objPersistence.ExecuteCommand(sql, GetParam(null, objUpdate.Espessura));
        }

        public void DeleteByIdBenefConfig(uint idBenefConfig)
        {
            List<uint> ids = objPersistence.LoadResult(@"select idBenefConfigPreco from benef_config_preco
                where idBenefConfig in (select idBenefConfig from benef_config where idBenefConfig=" +
                idBenefConfig + " or idParent=" + idBenefConfig + ")", null).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            if (ids.Count > 0)
            {
                string apagar = String.Empty;
                foreach (uint id in ids)
                {
                    LogAlteracaoDAO.Instance.ApagaLogBenefConfigPreco(id);
                    apagar += id + ",";
                }

                objPersistence.ExecuteCommand("delete from benef_config_preco where idBenefConfigPreco in (" + apagar.TrimEnd(',') + ")");
            }
        }

        /// <summary>
        /// Retorna o custo do beneficiamento.
        /// </summary>
        public decimal ObtemCustoBenef(GDASession session, uint idBenefConfig, float espessura)
        {
            return ObtemValorCampo<decimal>(session, "Custo", "IdBenefConfig=" + idBenefConfig +
                (espessura > 0 ? " And (espessura is null Or espessura=" + espessura.ToString().Replace(",", ".") + ")" : String.Empty));
        }

        public uint ObtemIdBenefConfig(uint idBenefConfigPreco)
        {
            return ObtemIdBenefConfig(null, idBenefConfigPreco);
        }

        public uint ObtemIdBenefConfig(GDASession session, uint idBenefConfigPreco)
        {
            return ObtemValorCampo<uint>(session, "idBenefConfig", "idBenefConfigPreco=" + idBenefConfigPreco);
        }

        public uint AtualizarTelaConfig(BenefConfigPreco objUpdate)
        {
            // Aplica o ajuste de preço com base na porcentagem escolhida pelo usuário
            float valorAtacado = (100f + objUpdate.AjusteAtacado) / 100f;
            float valorBalcao = (100f + objUpdate.AjusteBalcao) / 100f;
            float valorObra = (100f + objUpdate.AjusteObra) / 100f;
            float valorCustoCompra = (100 + objUpdate.AjusteCustoCompra) / 100;

            if (valorAtacado < 1 && valorBalcao < 1 && valorObra < 1 && valorCustoCompra < 1)
                throw new Exception("Selecione pelo menos 1 tipo de preço para reajustar.");

            // Define o campo que contém o preço base
            string campoBase = "Custo";

            // Cria o filtro que será usado pelo SQL
            string filtroSubgrupo = "IdBenefConfig in (select idBenefConfig from benef_config where idParent=" + objUpdate.IdSubgrupoProd + " or idBenefConfig=" + objUpdate.IdSubgrupoProd + ")";

            // Define as colunas que serão alteradas
            string mudarAtacado = valorAtacado >= 1 ? "ValorAtacado=(" + campoBase + " * " + valorAtacado.ToString().Replace(',', '.') + ")" : String.Empty;
            string mudarBalcao = valorBalcao >= 1 ? "ValorBalcao=(" + campoBase + " * " + valorBalcao.ToString().Replace(',', '.') + ")" : String.Empty;
            string mudarObra = valorObra >= 1 ? "ValorObra=(" + campoBase + " * " + valorObra.ToString().Replace(',', '.') + ")" : String.Empty;

            string filtro = " Where 1";
            string filtroCampos = String.Empty;

            if (!String.IsNullOrEmpty(filtroSubgrupo))
            {
                filtro += !String.IsNullOrEmpty(filtroSubgrupo) ? " and " + filtroSubgrupo : String.Empty;

                filtroCampos += " or coalesce(Custo ,0) <> 0";

                if (!String.IsNullOrEmpty(filtroCampos))
                    filtroCampos = " and (" + filtroCampos.Substring(4) + ")";
            }

            // Verifica se a alteração pode ser feita
            string nomeTabela = "benef_config_preco";
            string sql;

            // Define o SQL de alteração
            string mudar = String.Empty;
            mudar += !String.IsNullOrEmpty(mudarAtacado) ? mudarAtacado : String.Empty;
            mudar += !String.IsNullOrEmpty(mudar) && !String.IsNullOrEmpty(mudarBalcao) ? ", " : String.Empty;
            mudar += !String.IsNullOrEmpty(mudarBalcao) ? mudarBalcao : String.Empty;
            mudar += !String.IsNullOrEmpty(mudar) && !String.IsNullOrEmpty(mudarObra) ? ", " : String.Empty;
            mudar += !String.IsNullOrEmpty(mudarObra) ? mudarObra : String.Empty;

            sql = "update " + nomeTabela + " set " + mudar + filtro + " and coalesce(" + campoBase + ",0)<>0";

            objPersistence.ExecuteCommand(sql);

            LogProduto log = new LogProduto();
            log.DataAjuste = DateTime.Now;

            log.IdGrupoProd = 0;
            log.IdSubgrupoProd = (uint?)objUpdate.IdSubgrupoProd;
            log.TipoPrecoBase = 1;
            log.AjusteAtacado = objUpdate.AjusteAtacado;
            log.AjusteBalcao = objUpdate.AjusteBalcao;
            log.AjusteCustoCompra = objUpdate.AjusteCustoCompra;
            log.AjusteObra = objUpdate.AjusteObra;
            log.IdFunc = UserInfo.GetUserInfo.CodUser;
            return LogProdutoDAO.Instance.InsertBase(log);
        }

    }
}
