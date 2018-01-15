using System;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que representa uma aresta do grafo.
    /// </summary>
    public class Aresta<Origem, Destino> : ICloneable
        where Origem : Vertice
        where Destino : Vertice
    {
        /// <summary>
        /// Vértice de origem da aresta.
        /// </summary>
        public Origem VerticeOrigem { get; set; }

        /// <summary>
        /// Vértice de destino da aresta.
        /// </summary>
        public Destino VerticeDestino { get; set; }

        #region ICloneable Members

        /// <summary>
        /// Duplica o objeto atual.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Aresta<Origem, Destino> clone = new Aresta<Origem, Destino>();

            clone.VerticeOrigem = (Origem)this.VerticeOrigem.Clone();
            clone.VerticeDestino = (Destino)this.VerticeDestino.Clone();

            return clone;
        }

        #endregion
    }
}
