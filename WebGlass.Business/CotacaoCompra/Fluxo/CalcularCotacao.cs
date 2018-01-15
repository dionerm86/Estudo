using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class CalcularCotacao : BaseFluxo<CalcularCotacao>
    {
        private CalcularCotacao() { }

        #region Classe auxiliar

        public class DadosFornecedor
        {
            internal DadosFornecedor(uint codigoFornecedor, long prazoEntregaDias, DateTime[] datasParcelas)
            {
                CodigoFornecedor = codigoFornecedor;
                PrazoEntregaDias = prazoEntregaDias;
                DatasParcelas = datasParcelas;
            }

            public uint CodigoFornecedor { get; private set; }
            public long PrazoEntregaDias { get; private set; }
            public DateTime[] DatasParcelas { get; private set; }

            public override bool Equals(object obj)
            {
                if (!(obj is DadosFornecedor))
                    return false;

                DadosFornecedor d = obj as DadosFornecedor;
                return d.CodigoFornecedor == CodigoFornecedor && d.PrazoEntregaDias == PrazoEntregaDias &&
                    d.DatasParcelas.Length == DatasParcelas.Length && d.DatasParcelas.All(x => DatasParcelas.Contains(x));
            }

            public override int GetHashCode()
            {
                return 1324698321;
            }
        }

        #endregion

        /// <summary>
        /// Obtém os tipos de cálculo usados na cotação de compra.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] ObtemTiposCalculoCotacao()
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel((int)Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao.MenorCusto, "Menor custo"));
            lst.Add(new GenericModel((int)Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao.MenorPrazo, "Menor prazo"));

            return lst.ToArray();
        }

        /// <summary>
        /// Calcula uma cotação de compras, separando os melhores produtos por fornecedor
        /// de acordo com a prioridade de cálculo selecionada.
        /// </summary>
        public Entidade.CotacaoCompraCalculada[] Calcular(GDA.GDASession session, uint codigoCotacaoCompra, 
            Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao tipoCalculo, bool isRpt)
        {
            // Valida a cotação de compra selecionada
            if (!CotacaoCompraDAO.Instance.Exists(session, codigoCotacaoCompra))
                throw new Exception("Cotação de compra não existe.");

            var cotacao = CotacaoCompraDAO.Instance.GetElementByPrimaryKey(session, codigoCotacaoCompra);

            switch (cotacao.Situacao)
            {
                case Glass.Data.Model.CotacaoCompra.SituacaoEnum.Cancelada:
                    throw new Exception("Cotação de compra está cancelada.");

                case Glass.Data.Model.CotacaoCompra.SituacaoEnum.Finalizada:
                    if (!isRpt)
                        throw new Exception("Cotação de compra está finalizada.");
                    break;
            }

            List<Entidade.CotacaoCompraCalculada> retorno = new List<Entidade.CotacaoCompraCalculada>();

            // Recupera os dados dos fornecedores cadastrados para a cotação de compras
            var fornecedores = ProdutoFornecedorCotacaoCompraDAO.Instance.ObtemProdutosFornecedorCotacao(session,
                codigoCotacaoCompra, 0, 0, true);
            var produtosCotacaoCompra = ProdutoCotacaoCompraDAO.Instance.ObtemProdutos(session, codigoCotacaoCompra);

            if (produtosCotacaoCompra == null || produtosCotacaoCompra.Length == 0)
                throw new Exception("A Cotação de compra não possui produtos.");

            // Busca os produtos cadastrados na cotação de compras
            foreach (var produto in produtosCotacaoCompra)
            {
                // Busca os fornecedores do produto atual, ordenando em ordem crescente
                // pela proridade de cálculo
                var fp = from f in fornecedores
                         where f.IdProd == produto.IdProd
                         orderby (tipoCalculo == Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao.MenorCusto ?
                             f.CustoUnit : f.PrazoEntregaDias), (tipoCalculo == Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao.MenorCusto ?
                             f.PrazoEntregaDias : f.CustoUnit)
                         select f;

                // Ignora o produto se não houver fornecedor
                if (fp.Count() == 0)
                    continue;

                // Adiciona o item calculado à lista
                Entidade.CotacaoCompraCalculada item = new Entidade.CotacaoCompraCalculada(cotacao, produto, fp.ToArray()[0]);
                retorno.Add(item);
            }

            if (retorno.Count() == 0)
                throw new Exception("Nenhum produto informado está associado a fornecedor.");

            // Retorna os itens calculados
            return retorno.ToArray();
        }

        /// <summary>
        /// Calcula uma cotação de compras, separando os melhores produtos por fornecedor
        /// de acordo com a prioridade de cálculo selecionada. Retorna os produtos por fornecedor.
        /// </summary>
        public Dictionary<DadosFornecedor, Entidade.CotacaoCompraCalculada[]> CalcularPorFornecedor(GDA.GDASession session,
            uint codigoCotacaoCompra, Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao tipoCalculo)
        {
            // Dicionário que contém os dados de retorno
            Dictionary<DadosFornecedor, Entidade.CotacaoCompraCalculada[]> retorno = 
                new Dictionary<DadosFornecedor, Entidade.CotacaoCompraCalculada[]>();

            // Recupera os dados calculados
            var calculados = Calcular(session, codigoCotacaoCompra, tipoCalculo, false);

            // Agrupa os itens por fornecedor e os adiciona ao dicionário
            foreach (uint f in calculados.Select(x => x.CodigoFornecedor).Distinct())
            {
                // Busca os itens do fornecedor
                var itensBase = (from c in calculados
                                 where c.CodigoFornecedor == f
                                 select c).ToArray();

                // Separa as formas de pagamento definidas para o fornecedor
                var formasPagto = (from i in itensBase
                                   select new {
                                       i.CodigoParcela,
                                       i.DatasParcelasConfiguradas
                                   }).Distinct();

                // Agrupa os itens por fornecedor e por forma de pagamento
                foreach (var fp in formasPagto)
                {
                    // Recupera os itens que possuem a forma de pagamento indicada
                    var itens = from i in itensBase
                                where i.CodigoParcela == fp.CodigoParcela &&
                                   i.DatasParcelasConfiguradas.Length == fp.DatasParcelasConfiguradas.Length
                                select i;

                    // Recupera o prazo de entrega da compra
                    long prazoEntrega = itens.Max(x => x.PrazoEntregaDiasFornecedor);

                    // Busca as datas das parcelas
                    List<DateTime> datas = new List<DateTime>();

                    if (fp.CodigoParcela == -1)
                        datas.AddRange(fp.DatasParcelasConfiguradas);

                    else if (fp.CodigoParcela > 0)
                    {
                        DateTime dataBase = DateTime.Now;
                        var p = ParcelasDAO.Instance.GetElement(session, (uint)fp.CodigoParcela);

                        foreach (int dias in p.NumeroDias)
                            datas.Add(dataBase.AddDays(dias));
                    }
                    else
                        throw new Exception("Selecione a condição de pagamento do fornecedor para continuar.");

                    var dadosFornecedor = new DadosFornecedor(f, prazoEntrega, datas.ToArray());

                    /* Chamado 35091. */
                    // Adiciona o item ao dicionário
                    if (!retorno.ContainsKey(dadosFornecedor))
                        retorno.Add(dadosFornecedor, itens.ToArray());
                    else
                        retorno[dadosFornecedor].ToList().AddRange(itens.ToArray());
                }
            }

            // Retorna o dicionário
            return retorno;
        }
    }
}
