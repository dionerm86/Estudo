using Glass.Data.Model;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que representa uma consulta pretendida por
    /// um cliente.
    /// </summary>
    public class ProdutoPedidoEspelho : Vertice
    {
        private ProdutosPedidoEspelho produtoPedidoEspelho;

        /// <summary>
        /// Código do produto pedido espelho.
        /// </summary>
        public uint IdProdPed
        {
            get { return produtoPedidoEspelho.IdProdPed; }
        }

        /// <summary>
        /// Retorna a descrição do produto.
        /// </summary>
        public string Descricao
        {
            get { return produtoPedidoEspelho.DescrProduto; }
        }

        /// <summary>
        /// Código do produto.
        /// </summary>
        public uint IdProd
        {
            get { return produtoPedidoEspelho.IdProd; }
        }

        /// <summary>
        /// Número da peça do produto que será impressa.
        /// </summary>
        public int NumeroPeca { get; set; }

        /// <summary>
        /// Construtor da classe.
        /// Chama o construtor da superclasse.
        /// </summary>
        public ProdutoPedidoEspelho(ProdutosPedidoEspelho produtoPedidoEspelho, int numeroPeca)
            : base(x => (x as ProdutoPedidoEspelho).IdProdPed + "_" + (x as ProdutoPedidoEspelho).NumeroPeca)
        {
            this.produtoPedidoEspelho = produtoPedidoEspelho;
            this.NumeroPeca = numeroPeca;
            //this.Capacidade = produtoPedidoEspelho.TotM / produtoPedidoEspelho.Qtde;
        }

        public override object Clone()
        {
            return MetodosExtensao.Clonar(this, produtoPedidoEspelho, NumeroPeca);
        }
    }
}
