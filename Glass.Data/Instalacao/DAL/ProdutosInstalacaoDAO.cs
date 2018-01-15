using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ProdutosInstalacaoDAO : BaseDAO<ProdutosInstalacao, ProdutosInstalacaoDAO>
    {
        //private ProdutosInstalacaoDAO() { }

        #region Busca padrão

        private string Sql(uint idProdInst, uint idInstalacao)
        {
            string sql = @"
                select pi.*, p.idProd, p.codInterno, pp.qtde, pp.valorVendido, pp.altura, pp.largura, pp.totM, pp.total, pp.valorBenef,
                    p.descricao as descrProduto, p.idGrupoProd, p.idSubgrupoProd, pp.idAmbientePedido, Coalesce(ae.ambiente, a.ambiente) as ambiente, 
                    Coalesce(ae.descricao, a.descricao) as descrAmbiente
                from produtos_instalacao pi
                    inner join instalacao i on (pi.idInstalacao=i.idInstalacao)
                    left join produtos_pedido pp on (pi.idProdPed=pp.idProdPed)
                    left join produto p on (pp.idProd=p.idProd)
                    left join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    left join ambiente_pedido_espelho ae on (ppe.idAmbientePedido=ae.idAmbientePedido)
                where 1";

            if (idProdInst > 0)
                sql += " and pi.idProdInst=" + idProdInst;

            if (idInstalacao > 0)
                sql += " and pi.idInstalacao=" + idInstalacao;

            return sql;
        }

        public IList<ProdutosInstalacao> GetByInstalacao(uint idInstalacao)
        {
            var lst = objPersistence.LoadData(Sql(0, idInstalacao)).ToList();

            CalculaValor(null, ref lst);

            return lst;
        }

        public ProdutosInstalacao GetElement(uint idProdInst)
        {
            return GetElement(null, idProdInst);
        }

        public ProdutosInstalacao GetElement(GDASession session, uint idProdInst)
        {
            var prodInst = objPersistence.LoadData(session, Sql(idProdInst, 0)).ToList();

            CalculaValor(session, ref prodInst);

            return prodInst.Count > 0 ? prodInst[0] : null;
        }

        /// <summary>
        /// Calcula o valor de cada produto da instalação, já rateando o desconto do pedido e a quantidade de peças instaladas
        /// </summary>
        private void CalculaValor(GDASession session, ref List<ProdutosInstalacao> lst)
        {
            if (lst == null || lst.Count == 0)
                return;

            var situacaoInst = InstalacaoDAO.Instance.ObtemValorCampo<int>(session, "situacao", "idInstalacao=" + lst[0].IdInstalacao);
            
            var idPedido = InstalacaoDAO.Instance.ObtemIdPedido(session, lst[0].IdPedido);
            
            var percDesc = (decimal)PedidoDAO.Instance.GetPercDesc(session, idPedido);

            if (percDesc == 0)
                return;

            foreach (ProdutosInstalacao pi in lst)
            {
                var valorProd = pi.Total + pi.ValorBenef;

                if (!PedidoConfig.RatearDescontoProdutos)
                    valorProd = valorProd - (valorProd * percDesc);

                pi.ValorProdutos = valorProd / (decimal)pi.Qtde *
                    (decimal)(situacaoInst == (int)Instalacao.SituacaoInst.EmAndamento ? pi.Qtde - pi.QtdeInstalada : pi.QtdeInstalada);
            }
        }

        #endregion

        public void DeleteByInstalacoes(GDASession sessao, string idsInstalacao)
        {
            objPersistence.ExecuteCommand(sessao, "delete from produtos_instalacao where idInstalacao in (" + idsInstalacao + ")");
        }

        /// <summary>
        /// Insere um produto instalado
        /// </summary>
        public void InsereProdutoInstalado(GDASession session, int idInstalacao, List<ProdutosInstalacao> lstProdInst)
        {
            // Insere produto com suas respectivas quantidades instaladas
            foreach (var pi in lstProdInst.Where(f => f.QtdeInstalada > 0).ToList())
            {
                string sql = "select idProdInst from produtos_instalacao where idInstalacao=" + pi.IdInstalacao + " and idProdPed=" + pi.IdProdPed;
                object id = objPersistence.ExecuteScalar(session, sql);
                pi.IdProdInst = id != null && id != DBNull.Value && id.ToString() != "" ? id.ToString().StrParaUint() : 0;

                if (pi.IdProdInst > 0)
                {
                    ProdutosInstalacao original = GetElement(session, pi.IdProdInst);
                    pi.QtdeInstalada = Math.Min(pi.QtdeInstalada + original.QtdeInstalada, (int)original.Qtde);

                    Update(session, pi);
                }
                else
                    Insert(session, pi);
            }
        }

        /// <summary>
        /// Verifica se a instalação deve ser finalizada (caso contrário será continuada).
        /// </summary>
        public bool VerificaFinalizarInst(GDASession session, uint idPedido)
        {
            string sql = @"
                select count(*) from (
                    select pp.qtde, sum(pi.qtdeInstalada) as qtdeInstalada
                    from produtos_pedido pp
                        left join produtos_instalacao pi on (pp.idProdPed=pi.idProdPed)
                    where pp.idPedido=" + idPedido + @" and coalesce(pp.invisivelFluxo,false)=false
                    group by pp.idProdPed having qtde>coalesce(qtdeInstalada,0)
                ) as temp";

            return objPersistence.ExecuteSqlQueryCount(session, sql) == 0;
        }
    }
}
