using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class DescontoFormaPagamentoDadosProdutoDAO : BaseCadastroDAO<DescontoFormaPagamentoDadosProduto, DescontoFormaPagamentoDadosProdutoDAO>
    {
        #region Busca Padrão

        private string Sql(bool selecionar)
        {
            string campos = selecionar ? "dfpdp.*, gp.descricao as DescGrupoProd, sgp.descricao as DescSubgrupoProd, fp.descricao as DescFormaPagto, tcc.descricao as DescTipoCartao" : "Count(*)";

            string sql = "Select " + campos + @" From desconto_forma_pagamento_dados_produto dfpdp
                Left Join grupo_prod gp On (dfpdp.idGrupoProd = gp.idGrupoProd)
                Left Join subgrupo_prod sgp On (dfpdp.idSubgrupoProd = sgp.idSubgrupoProd)
                Left Join formapagto fp On (dfpdp.idFormaPagto = fp.idFormaPagto)
                Left Join tipo_cartao_credito tcc On (dfpdp.idTipoCartao = tcc.idTipoCartao)";

            return sql;
        }

        public IList<DescontoFormaPagamentoDadosProduto> PesquisarDescontoFormaPagamentoDadosProduto(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize);
        }

        public int CountDescontoFormaPagamentoDadosProduto()
        {
            return GetCountWithInfoPaging(Sql(true));
        }

        #endregion

        public decimal ObterDesconto(uint? tipoVenda, uint? idFormaPagto, uint? idTipoCartao, uint? idParcela, uint? idGrupoProd, uint? idSubgrupoProd)
        {
            return ObterDesconto(null, tipoVenda, idFormaPagto, idTipoCartao, idParcela, idGrupoProd, idSubgrupoProd);
        }

        #region Obtém valor dos campos

        public decimal ObterDesconto(GDASession sessao, uint? tipoVenda, uint? idFormaPagto, uint? idTipoCartao, uint? idParcela, uint? idGrupoProd, uint? idSubgrupoProd)
        {
            var sql = "select desconto from desconto_forma_pagamento_dados_produto WHERE situacao=1 ";

            sql += " And tipoVenda" + (tipoVenda != null ? "=" + tipoVenda.ToString() : " is null");
            sql += " And (idFormaPagto" + (idFormaPagto != null ? "=" + idFormaPagto.ToString() + " or idFormaPagto is null)" : " is null)");
            sql += " And (idTipoCartao" + (idTipoCartao != null ? "=" + idTipoCartao.ToString() + " or idTipoCartao is null)" : " is null)");
            sql += " And (idParcela" + (idParcela != null ? "=" + idParcela.ToString() + " or idParcela is null)" : " is null)");
            sql += " And (idGrupoProd" + (idGrupoProd != null ? "=" + idGrupoProd.ToString() + " or idGrupoProd is null)" : " is null)");
            sql += " And (idSubgrupoProd" + (idSubgrupoProd != null ? "=" + idSubgrupoProd.ToString() + " or idSubgrupoProd is null)" : " is null)");
            sql += " order by idFormaPagto desc, idTipoCartao desc, idparcela desc, idgrupoprod desc, idsubgrupoprod desc";

            return ExecuteMultipleScalar<decimal>(sql).FirstOrDefault();
        }

        public uint? ObtemIdGrupoProd(uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemIdGrupoProd(null, idDescontoFormaPagamentoDadosProduto);
        }

        public uint? ObtemIdGrupoProd(GDASession sessao, uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemValorCampo<uint>(sessao, "IdGrupoProd", "IdDescontoFormaPagamentoDadosProduto=" + idDescontoFormaPagamentoDadosProduto);
        }

        public uint? ObtemIdSubgrupoProd(uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemIdSubgrupoProd(null, idDescontoFormaPagamentoDadosProduto);
        }

        public uint? ObtemIdSubgrupoProd(GDASession sessao, uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemValorCampo<uint>(sessao, "IdSubgrupoProd", "IdDescontoFormaPagamentoDadosProduto=" + idDescontoFormaPagamentoDadosProduto);
        }

        public uint? ObtemIdTipoCartao(uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemIdTipoCartao(null, idDescontoFormaPagamentoDadosProduto);
        }

        public uint? ObtemIdTipoCartao(GDASession sessao, uint idDescontoFormaPagamentoDadosProduto)
        {
            return ObtemValorCampo<uint?>(sessao, "idTipoCartao", "idDescontoFormaPagamentoDadosProduto=" + idDescontoFormaPagamentoDadosProduto);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(DescontoFormaPagamentoDadosProduto objInsert)
        {
            if (JaCadastrado(objInsert.TipoVenda, objInsert.IdFormaPagto, objInsert.IdTipoCartao, objInsert.IdParcela, objInsert.IdGrupoProd, objInsert.IdSubgrupoProd))
                throw new Exception("Já existe um desconto cadastrado com os dados informados.");

            return base.Insert(objInsert);
        }

        public override int Update(DescontoFormaPagamentoDadosProduto objUpdate)
        {
            LogAlteracaoDAO.Instance.LogDescontoFormaPagamentoDadosProduto(null, objUpdate);

            var retorno = base.Update(objUpdate);

            return retorno;
        }

        #endregion

        public DescontoFormaPagamentoDadosProduto GetElement(uint idDescontoFormaPagamentoDadosProduto)
        {
            return GetElementByPrimaryKey(idDescontoFormaPagamentoDadosProduto);
        }

        public bool JaCadastrado(uint? tipoVenda, uint? idFormaPagto, uint? idTipoCartao, uint? idParcela, uint? idGrupoProd, uint? idSubgrupoProd)
        {
            return JaCadastrado(null, tipoVenda, idFormaPagto, idTipoCartao, idParcela, idGrupoProd, idSubgrupoProd);
        }

        public bool JaCadastrado(GDASession sessao, uint? tipoVenda, uint? idFormaPagto, uint? idTipoCartao, uint? idParcela, uint? idGrupoProd, uint? idSubgrupoProd)
        {
            var sql = "select * from desconto_forma_pagamento_dados_produto WHERE situacao=1 ";

            sql += " And tipoVenda" + (tipoVenda != null ? "=" + tipoVenda.ToString() : " is null");
            sql += " And idFormaPagto" + (idFormaPagto != null ? "=" + idFormaPagto.ToString() : " is null");
            sql += " And idTipoCartao" + (idTipoCartao != null ? "=" + idTipoCartao.ToString() : " is null");
            sql += " And idParcela" + (idParcela != null ? "=" + idParcela.ToString() : " is null");
            sql += " And idGrupoProd" + (idGrupoProd != null ? "=" + idGrupoProd.ToString() : " is null");
            sql += " And idSubgrupoProd" + (idSubgrupoProd != null ? "=" + idSubgrupoProd.ToString() : " is null");
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        public void AtivarInativar(uint IdDescontoFormaPagamentoDadosProduto)
        {
            var DescontoFormaPagamentoDadosProduto = GetElementByPrimaryKey(IdDescontoFormaPagamentoDadosProduto);
            if (DescontoFormaPagamentoDadosProduto.Situacao == Situacao.Inativo)
                DescontoFormaPagamentoDadosProduto.Situacao = Situacao.Ativo;
            else
                DescontoFormaPagamentoDadosProduto.Situacao = Situacao.Inativo;

            Update(DescontoFormaPagamentoDadosProduto);
        }
    }
}
