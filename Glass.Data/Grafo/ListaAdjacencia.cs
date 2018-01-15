using System;
using System.Collections.Generic;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe com a lista de adjacências para o grafo.
    /// </summary>
    /// <typeparam name="Vertice">O tipo do vértice.</typeparam>
    /// <typeparam name="Aresta">O tipo da aresta.</typeparam>
    internal class ListaAdjacencia<Vertice, Aresta> : IEnumerable<Vertice>, ICloneable
        where Vertice : ICloneable
        where Aresta : struct
    {
        /// <summary>
        /// O grafo é direcionado?
        /// </summary>
        public bool Direcionado { get; private set; }

        /// <summary>
        /// Lista com os dados de arestas para cada vértice.
        /// </summary>
        private Dictionary<Vertice, Dictionary<Vertice, Aresta>> lista;

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="listaVertices">A lista de vértices que serão adicionados à lista.</param>
        public ListaAdjacencia(IEnumerable<Vertice> listaVertices)
            : this(listaVertices, false)
        {
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="listaVertices">A lista de vértices que serão adicionados à lista.</param>
        /// <param name="direcionado">O grafo é direcionado?</param>
        public ListaAdjacencia(IEnumerable<Vertice> listaVertices, bool direcionado)
        {
            // Indica se o grafo é direcionado
            Direcionado = direcionado;

            // Cria a lista interna
            lista = new Dictionary<Vertice, Dictionary<Vertice, Aresta>>();

            // Adiciona os vértices à lista, se houver
            if (listaVertices != null)
                foreach (Vertice v in listaVertices)
                    AdicionarVertice(v);
        }

        /// <summary>
        /// Limpa a lista de adjacências.
        /// </summary>
        public void Clear()
        {
            lista.Clear();
        }

        /// <summary>
        /// Adiciona um vértice à lista de adjacências.
        /// </summary>
        /// <param name="item">O vértice que será adicionado.</param>
        public void AdicionarVertice(Vertice item)
        {
            // Só adiciona o vértice à lista se ele ainda não estiver adicionado
            if (!ContemVertice(item))
                lista.Add(item, new Dictionary<Vertice, Aresta>());
        }

        /// <summary>
        /// Adiciona uma aresta à lista de adjacências.
        /// No caso do grafo ser não direcionado, será gerada uma aresta na ordem contrária com o mesmo peso.
        /// </summary>
        /// <param name="inicial">O vértice inicial da aresta.</param>
        /// <param name="final">O vértice final da aresta.</param>
        /// <param name="peso">O peso da aresta.</param>
        public void AdicionarAresta(Vertice inicial, Vertice final, Aresta peso)
        {
            // Adiciona o vértice inicial, se necessário
            if (!ContemVertice(inicial))
                AdicionarVertice(inicial);

            // Adiciona o vértice final, se necessário
            if (!ContemVertice(final))
                AdicionarVertice(final);

            // Adiciona a aresta à lista de adjacências do vértice inicial
            lista[inicial].Add(final, peso);

            // Verifica se o grafo é não-direcionado
            if (!Direcionado)
            {
                // Adiciona a aresta à lista de adjacências do vértice final
                lista[final].Add(inicial, peso);
            }
        }

        /// <summary>
        /// Verifica se a lista de adjacências contém um vértice.
        /// </summary>
        /// <param name="item">O vértice que será buscado.</param>
        /// <returns>Verdadeiro, se o vértice for encontrado; caso contrário, falso.</returns>
        public bool ContemVertice(Vertice item)
        {
            return lista.ContainsKey(item);
        }

        /// <summary>
        /// Retorna o número de vértices na lista de adjacências.
        /// </summary>
        public int Count
        {
            get { return lista.Count; }
        }

        /// <summary>
        /// Remove um vértice da lista.
        /// </summary>
        /// <param name="item">O item que será removido.</param>
        /// <returns>Verdadeiro, se o item for removido; senão, falso.</returns>
        public bool RemoverVertice(Vertice item)
        {
            // Percorre a lista de adjacências para encontrar as arestas
            // que fazem referência a esse vértice que será removido
            foreach (Vertice vertice in lista.Keys)
            {
                // Percorre todas as arestas de cada vértice
                // verificando se o destino da aresta é o
                // vértice sendo removido. Caso seja, remove a aresta
                foreach (KeyValuePair<Vertice, Aresta> aresta in lista[vertice])
                    if (object.Equals(aresta.Key, item))
                        lista[vertice].Remove(aresta.Key);
            }

            // Remove a lista de adjacências do vértice
            // e removendo-o da lista
            return lista.Remove(item);
        }

        /// <summary>
        /// Remove uma aresta do grafo.
        /// Se o grafo não for direcionado, remove também a aresta
        /// que é cadastrada no sentido contrário à aresta sendo removida.
        /// </summary>
        /// <param name="inicial">O vértice inicial da aresta.</param>
        /// <param name="final">O vértice final da aresta.</param>
        /// <returns>Verdadeiro, se a aresta for removida; caso contrário, falso.</returns>
        public bool RemoverAresta(Vertice inicial, Vertice final)
        {
            // Verifica se um dos dois vértices da aresta não existe na lista
            if (!ContemVertice(inicial) || !ContemVertice(final))
                return false;

            // Remove as arestas que forem do vértice inicial ao final
            while (lista[inicial].ContainsKey(final))
                lista[inicial].Remove(final);

            // Verifica se o grafo é não-direcionado
            if (!Direcionado)
            {
                // Remove as arestas que forem do vértice final ao inicial
                while (lista[final].ContainsKey(inicial))
                    lista[final].Remove(inicial);
            }

            return true;
        }

        /// <summary>
        /// Retorna a lista de adjacências de um vértice.
        /// </summary>
        /// <param name="item">O vértice que será buscado.</param>
        /// <returns>Uma lista com pares contendo o vértice de destino e o peso da aresta.</returns>
        public Dictionary<Vertice, Aresta> this[Vertice item]
        {
            get
            {
                // Garante que o vértice exista na lista
                if (!ContemVertice(item))
                    throw new ArgumentOutOfRangeException();

                // Retorna a lista de adjacências
                return lista[item];
            }
        }

        /// <summary>
        /// Retorna/altera a aresta entre dois vértices.
        /// </summary>
        /// <param name="inicial">O vértice inicial.</param>
        /// <param name="final">O vértice final.</param>
        /// <returns>O peso da aresta entre os dois vértices.</returns>
        public Aresta this[Vertice inicial, Vertice final]
        {
            get
            {
                // Garante que os vértices existam
                if (!ContemVertice(inicial))
                    throw new ArgumentException("Vértice inicial não encontrado.");

                if (!ContemVertice(final))
                    throw new ArgumentException("Vértice final não encontrado.");

                // Garante que a aresta exista
                if (!lista[inicial].ContainsKey(final))
                    throw new Exception("Aresta não encontrada.");

                // Retorna o peso da aresta
                return lista[inicial][final];
            }
            set
            {
                // Remove e adiciona a aresta novamente, para que
                // seu peso seja atualizado
                RemoverAresta(inicial, final);
                AdicionarAresta(inicial, final, value);
            }
        }

        /// <summary>
        /// Retorna o enumerador para a lista de adjacências.
        /// </summary>
        /// <returns>Um objeto IEnumerator&lt;Vertice&gt;.</returns>
        public IEnumerator<Vertice> GetEnumerator()
        {
            return lista.Keys.GetEnumerator();
        }

        /// <summary>
        /// Retorna o enumerador para a lista de adjacências.
        /// </summary>
        /// <returns>Um objeto IEnumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return lista.Keys.GetEnumerator();
        }

        #region ICloneable Members

        /// <summary>
        /// Duplica o objeto atual, criando novas referências.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ListaAdjacencia<Vertice, Aresta> clone = new ListaAdjacencia<Vertice, Aresta>(null, Direcionado);

            foreach (Vertice v in lista.Keys)
            {
                Vertice v1 = (Vertice)v.Clone();
                clone.lista.Add(v1, new Dictionary<Vertice, Aresta>());

                foreach (Vertice u in lista[v].Keys)
                {
                    Vertice u1 = (Vertice)u.Clone();
                    clone.lista[v].Add(u1, lista[v][u]);
                }
            }

            return clone;
        }

        #endregion
    }
}
