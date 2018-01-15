using System;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que serve como tipo para o vértice do grafo.
    /// </summary>
    public class Vertice : ICloneable
    {
        // Variável que controla os identificadores dos vértices.
        public delegate object GetId(Vertice v);
        private GetId _id;
        
        /// <summary>
        /// Identificador do vértice.
        /// </summary>
        public object Id
        {
            get { return _id(this); }
        }

        /// <summary>
        /// Capacidade do vértice.
        /// (usado para vértices de destino)
        /// </summary>
        public float Capacidade { get; protected set; }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Vertice(GetId id) :
            this(id, 1)
        {
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Vertice(GetId id, float capacidade)
        {
            _id = id;
            Capacidade = capacidade;
        }

        /// <summary>
        /// Compara dois objetos do tipo Vertice.
        /// </summary>
        /// <param name="obj">O objeto que será comparado ao atual.</param>
        /// <returns>Verdadeiro, se os objetos tiverem o mesmo Id; caso contrário, falso.</returns>
        public override bool Equals(object obj)
        {
            // Só compara os Ids se o objeto for um Vertice
            if (!(obj is Vertice))
                return false;

            if (obj.GetType() != this.GetType())
                return false;

            return object.Equals((obj as Vertice).Id, Id);
        }

        /// <summary>
        /// Retorna o HashCode para a classe.
        /// Necessário para que o método Equals seja chamado na lista de adjacências.
        /// </summary>
        /// <returns>Um número inteiro.</returns>
        public override int GetHashCode()
        {
            return 493216232;
        }

        #region ICloneable Members

        /// <summary>
        /// Duplica o objeto atual.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return MetodosExtensao.Clonar(this, _id);
        }

        #endregion
    }
}
