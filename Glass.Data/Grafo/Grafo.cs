using System;
using System.Collections.Generic;

namespace Glass.Data.Grafo
{
    /// <summary>
    /// Classe que representa um grafo.
    /// </summary>
    public class Grafo<Origem, Destino>
        where Origem : Vertice
        where Destino : Vertice
    {
        /// <summary>
        /// Lista de adjacência para o grafo.
        /// </summary>
        private ListaAdjacencia<Vertice, float> listaAdj;

        // Os dados do grafo.
        private DadosGrafo<Origem, Destino> _dadosGrafo;

        /// <summary>
        /// Indica se o grafo é direcionado.
        /// </summary>
        public bool Direcionado
        {
            get { return listaAdj.Direcionado; }
        }

        /// <summary>
        /// Retorna/altera os dados do grafo.
        /// </summary>
        public DadosGrafo<Origem, Destino> DadosGrafo
        {
            get { return _dadosGrafo; }
            set
            {
                // Limpa o grafo e salva o caso de teste em uma variável
                Clear();
                _dadosGrafo = value;

                // Adiciona os vértices ao grafo
                foreach (Vertice vertice in _dadosGrafo.Vertices)
                    listaAdj.AdicionarVertice(vertice);

                foreach (Aresta<Origem, Destino> aresta in _dadosGrafo.Arestas)
                    listaAdj.AdicionarAresta(aresta.VerticeOrigem, aresta.VerticeDestino, aresta.VerticeOrigem.Capacidade);
            }
        }

        /// <summary>
        /// Construtor do grafo.
        /// </summary>
        /// <param name="direcionado">O grafo é direcionado?</param>
        public Grafo(bool direcionado)
        {
            listaAdj = new ListaAdjacencia<Vertice, float>(null, direcionado);
        }

        /// <summary>
        /// Limpa o grafo.
        /// </summary>
        public void Clear()
        {
            // Reinicia os dados do grafo e limpa a lista de adjacências.
            _dadosGrafo = null;
            listaAdj.Clear();
        }

        /// <summary>
        /// Executa o algoritmo de fluxo máximo para o grafo atual.
        /// </summary>
        /// <param name="permitirFluxoParcialCapacidade"></param>
        /// <returns>A relação de entre origens e destinos.</returns>
        public Dictionary<Origem, Destino> FluxoMaximo(bool permitirFluxoParcialCapacidade)
        {
            if (!Direcionado)
                throw new Exception("Apenas para grafos direcionados.");

            // Lista de adjacências que representa o grafo
            // em cada iteração: começa com o grafo original
            ListaAdjacencia<Vertice, float> grafo = listaAdj.Clone() as ListaAdjacencia<Vertice, float>;

            #region Prepara o grafo para o algoritmo de fluxo máximo

            // Vértices de origem e destino do grafo
            Vertice s = new Vertice(x => -1, 0);
            Vertice t = new Vertice(x => -2, 0);

            // Inclui os vértices de origem e destino
            // usados no algoritmo de Ford-Fulkerson
            grafo.AdicionarVertice(s);
            grafo.AdicionarVertice(t);

            // Adiciona as arestas de suporte, que ligam os vértices s e t
            // às origens e destinos, respectivamente
            foreach (Vertice vertice in _dadosGrafo.Vertices)
            {   
                if (vertice is Origem)
                    grafo.AdicionarAresta(s, vertice, vertice.Capacidade);

                else if (vertice is Destino)
                    grafo.AdicionarAresta(vertice, t, vertice.Capacidade);
            }

            #endregion

            // Recupera o primeiro caminho aumentante
            // do grafo sem nenhum fluxo
            ListaAdjacencia<Vertice, float> caminho = BuscaLargura(grafo, s, t, permitirFluxoParcialCapacidade);

            // Executa enquanto houver um caminho aumentante
            while (caminho != null && caminho.Count > 0)
            {
                // Calcula o menor fluxo do caminho
                float fluxo = float.MaxValue;
                foreach (Vertice v in caminho)
                    foreach (Vertice u in caminho[v].Keys)
                    {
                        float fluxoCaminho = caminho[v, u];

                        if (fluxo > fluxoCaminho && fluxoCaminho > 0)
                            fluxo = fluxoCaminho;
                    }

                // Percorre todos os vértices do caminho
                foreach (Vertice v in caminho)
                    foreach (KeyValuePair<Vertice, float> a in caminho[v])
                    {
                        // Atualiza a capacidade da aresta, reduzindo em (fluxo),
                        // indicando a passagem de fluxo
                        grafo[v, a.Key] -= fluxo;

                        // Aumenta a capacidade da aresta residual
                        if (grafo[a.Key].ContainsKey(v))
                            grafo[a.Key, v] += fluxo;
                        else
                            grafo.AdicionarAresta(a.Key, v, fluxo);
                    }

                // Busca um novo caminho aumentante
                caminho = BuscaLargura(grafo, s, t, permitirFluxoParcialCapacidade);
            }

            // Recupera as ligações entre as arestas
            // de clientes e clínicas para definir as consultas
            Dictionary<Origem, Destino> retorno = new Dictionary<Origem, Destino>();

            // Inclui todos os clientes na variável de retorno
            foreach (Origem o in _dadosGrafo.Origens)
                retorno.Add(o, null);

            // Percorre todo o grafo
            foreach (Vertice v in grafo)
            {
                // Só pesquisa os vértices de origem
                if (!(v is Origem))
                    continue;

                // Verifica entre as arestas de origem
                // qual tem a capacidade zerada, indicando
                // que o vértice destino será usado para a origem
                foreach (KeyValuePair<Vertice, float> a in grafo[v])
                    if (a.Key is Destino && a.Value < v.Capacidade)
                    {
                        retorno[v as Origem] = a.Key as Destino;
                        break;
                    }
            }

            // Retorna a variável com as consultas
            return retorno;
        }

        /// <summary>
        /// Executa uma busca em largura no grafo.
        /// Encontra um caminho aumentante.
        /// </summary>
        /// <param name="grafo">A lista de adjacências do grafo.</param>
        /// <param name="s">O vértice de origem do caminho.</param>
        /// <param name="t">O vértice de destino do caminho.</param>
        /// <param name="permitirFluxoParcialCapacidade"></param>
        /// <returns>Uma lista de adjacências com um caminho aumentante.</returns>
        private static ListaAdjacencia<Vertice, float> BuscaLargura(ListaAdjacencia<Vertice, float> grafo,
            Vertice s, Vertice t, bool permitirFluxoParcialCapacidade)
        {
            // Fila dos vértices a pesquisar
            Queue<Vertice> fila = new Queue<Vertice>();

            // Dicionário que contém os vértices descobertos e seus predecessores
            Dictionary<Vertice, Vertice> p = new Dictionary<Vertice, Vertice>();

            // Coloca o vértice de origem na fila
            fila.Enqueue(s);

            // Indica que a origem já foi descoberta
            p.Add(s, null);

            // Procura enquanto houver vértices na fila
            while (fila.Count > 0)
            {
                // Recupera o vértice atual
                Vertice atual = fila.Dequeue();

                // Verifica se o vértice atual é o vértice de destino
                if (object.Equals(atual, t))
                {
                    // Cria a lista de adjacências de retorno
                    ListaAdjacencia<Vertice, float> retorno = new ListaAdjacencia<Vertice, float>(null, true);

                    // Cria arestas na lista de adjacências para indicar o caminho
                    // (o método já adiciona os vértices à lista)
                    while (!object.Equals(atual, s))
                    {
                        retorno.AdicionarAresta(p[atual], atual, grafo[p[atual], atual]);
                        atual = p[atual];
                    }

                    // Retorna a lista de adjacências
                    return retorno;
                }

                // Busca todos os vértices que são ligados por arestas
                // pelo vértice atual que tenham capacidade > 0
                foreach (KeyValuePair<Vertice, float> a in grafo[atual])
                {
                    bool permiteFluxo = permitirFluxoParcialCapacidade || atual is Destino ? a.Value > 0 :
                        a.Value > 0 && (a.Value == atual.Capacidade || object.Equals(s, atual));

                    if (permiteFluxo && !p.ContainsKey(a.Key))
                    {
                        p.Add(a.Key, atual);
                        fila.Enqueue(a.Key);
                    }
                }
            }

            // Retorna caso não seja encontrado um caminho aumentante
            return null;
        }
    }
}
