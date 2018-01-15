using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ProdutoObraDAO : BaseDAO<ProdutoObra, ProdutoObraDAO>
    {
        //private ProdutoObraDAO() { }

        private string Sql(string idObra, string codInterno, bool selecionar)
        {
            string campos = selecionar ? "po.*, o.idCliente, p.codInterno, p.descricao As descrProduto, " +
                (!String.IsNullOrEmpty(idObra) ? @"Cast((Select SUM(temp.totM2Calc) From
                    (Select pp.idprod, Coalesce(pp.totM2Calc, 0) As totM2Calc
                        From produtos_pedido pp
                            Inner Join produto p1 On (pp.idProd = p1.idProd)
                        Where pp.idPedido In (Select idPedido From pedido
                            Where situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + " And idObra In (" + idObra + @")) And If((Select  Count(*) From pedido_espelho
                                Where idPedido=pp.idPedido)>0, !Coalesce(pp.invisivelFluxo, False), !Coalesce(pp.invisivelPedido, False))) As temp
                Where p.idProd=temp.idProd) As Decimal(12,2)) As totM2Utilizado,
                Cast((Select SUM(temp.totalCalc) From
                    (Select pp.idprod, Coalesce(pp.total, 0) As totalCalc 
                        From produtos_pedido pp
                            Inner Join produto p1 On (pp.idProd = p1.idProd)
                        Where pp.idPedido In (Select idPedido From pedido
                            Where situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + " And idObra In (" + idObra + @")) And If((Select  Count(*) From pedido_espelho
                                Where idPedido=pp.idPedido)>0, !Coalesce(pp.invisivelFluxo, False), !Coalesce(pp.invisivelPedido, False))) As temp
                Where p.idProd=temp.idProd) As Decimal(12,2)) As totalCalc" : "Null As totM2Utilizado, Null As totalCalc") : "Count(*)";
            
            string sql = "Select " + campos + @"
                From produto_obra po
                    Left Join obra o On (po.idObra=o.idObra)
                    Left Join produto p On (po.idProd=p.idProd)
                Where 1";

            if (idObra != "0" && !String.IsNullOrEmpty(idObra))
                sql += " And po.idObra In (" + idObra + ")";

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And p.codInterno=?codInterno";

            return sql;
        }

        private GDAParameter[] GetParams(string codInterno)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lst.Add(new GDAParameter("?codInterno", codInterno));

            return lst.ToArray();
        }

        public IList<ProdutoObra> GetList(uint idObra, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idObra) == 0)
                return new ProdutoObra[] { new ProdutoObra() };

            return LoadDataWithSortExpression(Sql(idObra.ToString(), null, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idObra)
        {
            int retorno = GetCountReal(idObra);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idObra)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idObra.ToString(), null, false));
        }

        public IList<ProdutoObra> GetForRpt(uint idObra)
        {
            return objPersistence.LoadData(Sql(idObra.ToString(), null, true)).ToList();
        }

        public IList<ProdutoObra> GetForRpt(Obra[] obras)
        {
            string idsObras = String.Join(",", Array.ConvertAll(obras, x => x.IdObra.ToString()));
            return objPersistence.LoadData(Sql(idsObras, null, true)).ToList();
        }

        public ProdutoObra GetByCodInterno(uint idObra, string codInterno)
        {
            return GetByCodInterno(null, idObra, codInterno);
        }

        public ProdutoObra GetByCodInterno(GDASession sessao, uint idObra, string codInterno)
        {
            List<ProdutoObra> itens = objPersistence.LoadData(sessao, Sql(idObra.ToString(), codInterno, true), GetParams(codInterno));
            return itens.Count > 0 ? itens[0] : null;
        }
 
        public IList<ProdutoObra> GetByObra(int idObra)
        {
            return GetByObra(null, idObra);
        }

        public IList<ProdutoObra> GetByObra(GDASession session, int idObra)
        {
            return objPersistence.LoadData(session, Sql(idObra.ToString(), null, true)).ToList();
        }

        #region Verifica se o produto pertence a uma obra e se pode ser inserido nela

        public class DadosProdutoObra
        {
            public bool ProdutoValido, AlterarValorUnitario;
            public float M2Produto;
            public decimal ValorUnitProduto;
            public string MensagemErro;

            public DadosProdutoObra(string mensagemErro)
            {
                ProdutoValido = false;
                AlterarValorUnitario = false;
                ValorUnitProduto = 0;
                M2Produto = 0;
                MensagemErro = mensagemErro;
            }

            public DadosProdutoObra(decimal valorUnitProduto, float m2Produto, bool alterarValorUnitario)
            {
                ProdutoValido = true;
                AlterarValorUnitario = alterarValorUnitario;
                ValorUnitProduto = valorUnitProduto;
                M2Produto = m2Produto;
                MensagemErro = null;
            }
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(GDASession sessao, uint idObra, uint idProd)
        {
            string codInterno = ProdutoDAO.Instance.GetCodInterno(sessao, (int)idProd);
            return IsProdutoObra(sessao, idObra, codInterno);
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(uint idObra, uint idProd)
        {
            string codInterno = ProdutoDAO.Instance.GetCodInterno((int)idProd);
            return IsProdutoObra(idObra, codInterno);
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(uint idObra, string codInterno)
        {
            return IsProdutoObra(null, idObra, codInterno);
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(GDASession sessao, uint idObra, string codInterno)
        {
            return IsProdutoObra(sessao, idObra, codInterno, null);
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="codInterno"></param>
        /// <param name="idPedidoPcp"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(uint idObra, string codInterno, uint? idPedidoPcp)
        {
            return IsProdutoObra(null, idObra, codInterno, idPedidoPcp);
        }

        /// <summary>
        /// Verifica se um produto pode ser inserido na obra.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <param name="codInterno"></param>
        /// <param name="idPedidoPcp"></param>
        /// <returns></returns>
        public DadosProdutoObra IsProdutoObra(GDASession sessao, uint idObra, string codInterno, uint? idPedidoPcp)
        {
            if (PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                ProdutoObra prod = GetByCodInterno(sessao, idObra, codInterno);
                if (prod == null)
                    return new DadosProdutoObra("Esse produto não está cadastrado no pagamento antecipado.");

                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(sessao, idObra, codInterno, idPedidoPcp);

                // Verifica se o tamanho máximo do produto configurado na obra é excedido pelos produtos inseridos no pedido
                if (prod.TamanhoMaximo < tamanhoProdutos)
                {
                    string unidadeMedida = UnidadeMedidaDAO.Instance.ObtemValorCampo<string>(sessao, "codigo",
                        "idUnidadeMedida in (select idUnidadeMedida from produto where codInterno=?cod)",
                        new GDAParameter("?cod", codInterno));

                    if (!String.IsNullOrEmpty(unidadeMedida))
                        unidadeMedida = " " + unidadeMedida.Trim();

                    return new DadosProdutoObra("Esse produto já foi utilizado totalmente para a obra, com o tamanho sendo excedido em " +
                        Math.Round(tamanhoProdutos - prod.TamanhoMaximo, 3) + unidadeMedida + ".");
                }

                // Calcula a metragem máxima restante
                float tamanhoMaximoRestante = prod.TamanhoMaximo - tamanhoProdutos;

                // Se a metragem máxima restante for 0 e o produto obra tiver tamanho máximo, retorna 0.1, para que entre no método
                // de validação no javascript
                if (prod.TamanhoMaximo > 0 && tamanhoMaximoRestante == 0)
                    tamanhoMaximoRestante = 0.01f;

                return new DadosProdutoObra(prod.ValorUnitario, tamanhoMaximoRestante, PedidoConfig.DadosPedido.AlterarValorUnitarioProduto);
            }

            return new DadosProdutoObra(0, 0, PedidoConfig.DadosPedido.AlterarValorUnitarioProduto);
        }

        #endregion

        #region Métodos sobrescritos
        public void VerificaProdutoComposicao(uint idProd)
        {
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);
            if (tipoSubgrupo == TipoSubgrupoProd.VidroLaminado || tipoSubgrupo == TipoSubgrupoProd.VidroDuplo)
            {
                if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(idProd))
                    throw new Exception("Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.");
            }
        }

        public override uint Insert(ProdutoObra objInsert)
        {
            uint idProdObra = base.Insert(objInsert);
            ObraDAO.Instance.UpdateValorObra(objInsert.IdObra);

            return idProdObra;
        }

        public override int Update(ProdutoObra objUpdate)
        {
            int retorno = base.Update(objUpdate);
            ObraDAO.Instance.UpdateValorObra(objUpdate.IdObra);

            return retorno;
        }

        public override int Delete(ProdutoObra objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProdObra);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            uint idObra = ObtemValorCampo<uint>("idObra", "idProdObra=" + Key);

            int retorno = GDAOperations.Delete(new ProdutoObra { IdProdObra = Key });
            ObraDAO.Instance.UpdateValorObra(idObra);

            return retorno;
        }

        #endregion
    }
}