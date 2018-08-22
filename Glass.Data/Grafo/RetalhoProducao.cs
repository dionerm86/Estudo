namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que representa as possibilidades
    /// de consulta para um retalho de produção.
    /// </summary>
    public class RetalhoProducao : Vertice
    {
        // Variável que contém a model do retalho de produção
        private Model.RetalhoProducao retalho;

        /// <summary>
        /// Código do retalho no banco de dados.
        /// </summary>
        public uint IdRetalhoProducao
        {
            get { return (uint)retalho.IdRetalhoProducao; }
        }

        /// <summary>
        /// Descrição do retalho.
        /// </summary>
        public string Descricao
        {
            get { return retalho.DescricaoRetalho; }
        }

        /// <summary>
        /// Código do produto associado ao retalho.
        /// </summary>
        public uint IdProd
        {
            get { return retalho.IdProd; }
        }

        /// <summary>
        /// Altura do retalho.
        /// </summary>
        public int Altura
        {
            get { return retalho.Altura; }
        }

        /// <summary>
        /// Largura do retalho.
        /// </summary>
        public int Largura
        {
            get { return retalho.Largura; }
        }

        /// <summary>
        /// Construtor da classe.
        /// Chama o construtor da superclasse.
        /// </summary>
        public RetalhoProducao(Model.RetalhoProducao retalho)
            : base(x => (x as RetalhoProducao).IdRetalhoProducao)
        {
            this.retalho = retalho;
            //this.Capacidade = retalho.TotM;
        }

        public override object Clone()
        {
            return MetodosExtensao.Clonar(this, retalho);
        }
    }
}
