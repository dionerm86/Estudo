namespace Glass.Global.Negocios.Entidades
{
    public class EtiquetaProcessoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da etiqueta de processo.
        /// </summary>
        public int IdProcesso { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Identificador da aplicação da etiqueta.
        /// </summary>
        public int? IdAplicacao { get; set; }

        /// <summary>
        /// Descricao.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Identifica se é para destacar a etiqueta.
        /// </summary>
        public bool DestacarEtiqueta { get; set; }

        /// <summary>
        /// Define se a peça com este processo irá gerar uma forma inexistente na exportação para o Optyway 
        /// para o conferente saber que precisa criar uma forma para a mesma, desde que a mesma não possua forma.
        /// </summary>
        public bool GerarFormaInexistente { get; set; }

        /// <summary>
        /// Define se a peça com esse processo irá gerar Arquivo de Mesa
        /// </summary>
        public bool GerarArquivoDeMesa { get; set; }

        /// <summary>
        /// Número mínimo de dias úteis para a data de entrega.
        /// </summary>
        public int NumeroDiasUteisDataEntrega { get; set; }

        /// <summary>
        /// Tipo de processo.
        /// </summary>
        public Data.Model.EtiquetaTipoProcesso? TipoProcesso { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Código interno da aplicação.
        /// </summary>
        public string CodInternoAplicacao { get; set; }

        /// <summary>
        /// Descrição da aplicação.
        /// </summary>
        public string DescricaoAplicacao { get; set; }

        /// <summary>
        /// Tipos de pedido
        /// </summary>
        public string TipoPedido { get; set; }

        /// <summary>
        /// Descrição dos tipos de pedido
        /// </summary>
        public string DescricaoTipoPedido { get; set; }

        /// <summary>
        /// Define se a peça com esse processo irá gerar Arquivo de Mesa
        /// </summary>
        public bool ForcarGerarSAG { get; set; }

        #endregion
    }
}
