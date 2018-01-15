namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Classe que armazena o resultado da pesquisa do subgrupo de produtos.
    /// </summary>
    public class SubgrupoProdPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int IdSubgrupoProd { get; set; }

        /// <summary>
        /// Identificador do grupo.
        /// </summary>
        public int IdGrupoProd { get; set; }

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        public uint? IdLoja { get; set; }

        /// <summary>
        /// Descrição do subgrupo.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Tipo de calculo.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculo { get; set; }

        /// <summary>
        /// Tipo do calculo para nota fiscal
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculoNf { get; set; }

        /// <summary>
        /// Identifica se é para bloquear o estoqu.
        /// </summary>
        public bool BloquearEstoque { get; set; }

        /// <summary>
        /// Identifica que não é para alterar o estoque.
        /// </summary>
        public bool NaoAlterarEstoque { get; set; }

        /// <summary>
        /// Identifica se é para alterar o estoque.
        /// </summary>
        public bool AlterarEstoque { get { return !NaoAlterarEstoque; } }

        /// <summary>
        /// Identifica se não é para alterar o estoque fiscal.
        /// </summary>
        public bool NaoAlterarEstoqueFiscal { get; set; }

        /// <summary>
        /// Identifica se é para altera o estoque fiscal.
        /// </summary>
        public bool AlterarEstoqueFiscal { get { return !NaoAlterarEstoqueFiscal; } }

        /// <summary>
        /// Produtos para estoque.
        /// </summary>
        public bool ProdutosEstoque { get; set; }

        /// <summary>
        /// Identifica se é vidro temperado.
        /// </summary>
        public bool IsVidroTemperado { get; set; }

        /// <summary>
        /// Exibir mensagem estoque.
        /// </summary>
        public bool ExibirMensagemEstoque { get; set; }

        /// <summary>
        /// Número Mínimo Dias Entrega
        /// </summary>
        public int? NumeroDiasMinimoEntrega { get; set; }

        /// <summary>
        /// Dia da semana para entrega.
        /// </summary>
        public int? DiaSemanaEntrega { get; set; }

        /// <summary>
        /// Identifica se é para gerar volume.
        /// </summary>
        public bool GeraVolume { get; set; }

        /// <summary>
        /// Tipo do subgrupo.
        /// </summary>
        public Data.Model.TipoSubgrupoProd TipoSubgrupo { get; set; }

        /// <summary>
        /// Tipo de calculo associado com o grupo.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculoGrupo { get; set; }

        /// <summary>
        /// Tipo de calculo de nota fiscal associado com o grupo.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculoNfGrupo { get; set; }

        /// <summary>
        /// Identifica se é um subgrupo do sistema.
        /// </summary>
        public bool SubgrupoSistema
        {
            get { return IdSubgrupoProd > 0 && IdSubgrupoProd <= 8; }
        }

        /// <summary>
        /// Verifica se é para exibir os produtos no estoque
        /// </summary>
        public bool ExibirProdutosEstoque
        {
            get
            {
                return IdGrupoProd == (int)Data.Model.NomeGrupoProd.Vidro;
            }
        }

        /// <summary>
        /// Identifica o Id do Cliente
        /// </summary>
        public int? IdCli { get; set; }

        /// <summary>
        /// Identifica o Nome do Cliente
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Identifica se os produtos desse subgrupo podem ser liberados com produção pendente
        /// </summary>
        public bool LiberarPendenteProducao { get; set; }

        /// <summary>
        /// Loja associada ao subgrupo
        /// </summary>
        public string Loja { get; set; }

        /// <summary>
        /// Indica se é permitida a revenda de produtos do tipo venda (solução para inclusão de embalagem no pedido de venda)
        /// </summary>
        public bool PermitirItemRevendaNaVenda { get; set; }

        #endregion
    }
}
