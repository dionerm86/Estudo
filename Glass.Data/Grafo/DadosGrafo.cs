using System.Collections.Generic;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que encapsula os dados do grafo.
    /// </summary>
    public class DadosGrafo<Origem, Destino>
        where Origem : Vertice
        where Destino : Vertice
    {
        // Listas de vértices do grafo, uma para
        // cliente e uma para clínicas
        private List<Origem> origens = new List<Origem>();
        private List<Destino> destinos = new List<Destino>();

        // Lista de arestas do grafo
        private List<Aresta<Origem, Destino>> _arestas = new List<Aresta<Origem, Destino>>();

        /// <summary>
        /// Retorna a lista de clientes.
        /// </summary>
        public List<Origem> Origens
        {
            get { return origens; }
        }
        
        /// <summary>
        /// Retorna a lista de clínicas.
        /// </summary>
        public List<Destino> Destinos
        {
            get { return destinos; }
        }

        /// <summary>
        /// Retorna a lista de vértices do grafo.
        /// </summary>
        public IEnumerable<Vertice> Vertices
        {
            get
            {
                List<Vertice> retorno = new List<Vertice>();
                
                foreach (Vertice v in origens)
                    retorno.Add(v);

                foreach (Vertice v in destinos)
                    retorno.Add(v);

                return retorno as IEnumerable<Vertice>;
            }
        }

        /// <summary>
        /// Retorna a lista de arestas do grafo.
        /// </summary>
        public List<Aresta<Origem, Destino>> Arestas
        {
            get { return _arestas; }
        }
    }
}
