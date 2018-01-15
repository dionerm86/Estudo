using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcEngine;
using Colosoft;

namespace Glass.Projeto.Negocios.Componentes
{
    /// <summary>
    /// Representa o cache das ferragens do sistema.
    /// </summary>
    public class FerragemCache : CalcEngine.IReferenceValueProvider, ICalcEngineReferencesService
    {
        #region Variáveis Locais

        private readonly object _objLock = new object();
        private bool _inicializado = false;
        private List<Item> _itens = new List<Item>();

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Assegura a inicializaçao do cache.
        /// </summary>
        private void AsseguraInicializacao()
        {
            lock (_objLock)
                if (!_inicializado)
                {
                    ConstruirItems();
                    _inicializado = true;
                }
        }

        /// <summary>
        /// Constrói os itens do cache.
        /// </summary>
        private void ConstruirItems()
        {
            var itens = new Dictionary<int, Item>();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Ferragem>("f")
                    .InnerJoin<Data.Model.FabricanteFerragem>("f.IdFabricanteFerragem=ff.IdFabricanteFerragem", "ff")
                    .Where("f.Situacao=?situacao")
                    .Add("?situacao", Situacao.Ativo)
                    .Select("f.IdFerragem, f.Nome AS Ferragem, ff.IdFabricanteFerragem, ff.Nome AS Fabricante"),
                    (sender, query, result) =>
                    {
                        foreach (var i in result)
                        {
                            var item = new Item
                            {
                                IdFerragem = i["IdFerragem"],
                                Nome = i["Ferragem"],
                                IdFabricanteFerragem = i["IdFabricanteFerragem"],
                                Fabricante = i["Fabricante"]
                            };
                            itens.Add(item.IdFerragem, item);
                        }
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ConstanteFerragem>("cf")
                    .InnerJoin<Data.Model.Ferragem>("cf.IdFerragem=f.IdFerragem", "f")
                    .Where("f.Situacao=?situacao")
                    .Add("?situacao", Situacao.Ativo)
                    .Select("cf.IdFerragem, cf.Nome, cf.Valor"),
                    (sender, query, result) =>
                    {
                        // Carrega as constantes das ferragens
                        foreach (var constantes in result.Select(f => new
                        {
                            IdFerragem = f.GetInt32("IdFerragem"),
                            Constante = new Constante
                            {
                                Nome = f["Nome"],
                                Valor = f["Valor"]
                            }
                        }).GroupBy(f => f.IdFerragem))
                        {
                            var item = itens[constantes.Key];

                            item.Constantes = constantes.Select(f => f.Constante).ToArray();
                        }
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.CodigoFerragem>("c")
                    .InnerJoin<Data.Model.Ferragem>("c.IdFerragem=f.IdFerragem", "f")
                    .Where("f.Situacao=?situacao")
                    .Add("?situacao", Situacao.Ativo)
                    .Select("c.IdFerragem, c.Codigo"),
                    (sender, query, result) =>
                    {
                        foreach (var codigos in result.Select(f => new
                        {
                            IdFerragem = f.GetInt32("IdFerragem"),
                            Codigo = f.GetString("Codigo")
                        }).GroupBy(f => f.IdFerragem))
                        {
                            var item = itens[codigos.Key];
                            item.Codigos = codigos.Select(f => f.Codigo).ToArray();
                        }
                    })
                 .Execute();

            _itens = itens.Select(f => f.Value).ToList();
        }

        /// <summary>
        /// Converte a ferragem para um item do cache.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <returns></returns>
        private Item Converter(Entidades.Ferragem ferragem)
        {
            return new Item
            {
                IdFerragem = ferragem.IdFerragem,
                Nome = ferragem.Nome,
                IdFabricanteFerragem = ferragem.IdFabricanteFerragem,
                Fabricante = ferragem.Fabricante.Nome,
                Codigos = ferragem.Codigos.Select(f => f.Codigo).ToArray(),
                Constantes = ferragem.Constantes
                    .Select(f => new Constante
                    {
                        Nome = f.Nome,
                        Valor = f.Valor
                    }).ToArray()
            };
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Atualiza os dados da ferragem no cache.
        /// </summary>
        /// <param name="ferragem"></param>
        public void Atualizar(Entidades.Ferragem ferragem)
        {
            ferragem.Require("ferragem").NotNull();

            AsseguraInicializacao();

            lock (_itens)
            {
                var index = _itens.FindIndex(f => f.IdFerragem == ferragem.IdFerragem);

                if (index >= 0)
                    _itens[index] = Converter(ferragem);

                else
                    _itens.Add(Converter(ferragem));
            }
        }

        /// <summary>
        /// Apaga os dados da ferragem do cache.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <returns>True se os dados foram removidos.</returns>
        public bool Apagar(Entidades.Ferragem ferragem)
        {
            ferragem.Require("ferragem").NotNull();

            AsseguraInicializacao();

            lock (_itens)
            {
                var index = _itens.FindIndex(f => f.IdFerragem == ferragem.IdFerragem);

                if (index >= 0)
                {
                    _itens.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Atualiza os dados do fabricante no cache.
        /// </summary>
        /// <param name="fabricante"></param>
        public void Atualizar(Entidades.FabricanteFerragem fabricante)
        {
            fabricante.Require("ferragem").NotNull();

            AsseguraInicializacao();

            lock (_itens)
            {
                foreach (var item in _itens)
                    if (item.IdFabricanteFerragem == fabricante.IdFabricanteFerragem)
                        item.Nome = fabricante.Nome;
            }
        }

        #endregion

        #region IReferenceValueProvider

        /// <summary>
        /// Recupera o valor de referência.
        /// </summary>
        /// <param name="referenceValue">Dados da referência do valor.</param>
        /// <returns></returns>
        public double GetValue(ReferenceValue referenceValue)
        {
            if (string.IsNullOrEmpty(referenceValue.Path)) return 0.0;

            // Quebra o caminho da referencias em partes
            var parts = referenceValue.Path.Split('/');

            if (parts.Length < 3) return 0;

            AsseguraInicializacao();

            var fabricante = parts[0];
            var ferragem = parts[1];
            var nomeConstante = parts[2];

            Constante constante = null;

            lock(_itens)
            {
                var item = _itens.FirstOrDefault(f =>
                    StringComparer.InvariantCultureIgnoreCase.Equals(f.Fabricante, fabricante) &&
                    StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, ferragem));

                if (item != null)
                    constante = item.Constantes.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, nomeConstante));
            }

            if (constante != null)
                return constante.Valor;

            return referenceValue.DefaultValue;
        }

        #endregion

        #region ICalcEngineReferencesService Members

        /// <summary>
        /// Recupera o contexto de referência pelo nome informado.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        CalcEngineReferenceContext ICalcEngineReferencesService.GetContext(string name)
        {
            AsseguraInicializacao();
            lock (_itens)
            {
                IEnumerable<CalcEngineReferenceValue> referenceValues = new CalcEngineReferenceValue[0];

                foreach (var i in _itens)
                    referenceValues = referenceValues.Concat(i.ObterValoresReferencia());

                return new CalcEngineReferenceContext
                {
                    Name = "WebGlass",
                    Description = "WebGlass Repository",
                    ReferenceValues = referenceValues.ToArray()
                };
            }
        }

        /// <summary>
        /// Recupera os contextos.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CalcEngineReferenceContextInfo> ICalcEngineReferencesService.GetContexts()
        {
            yield return new CalcEngineReferenceContextInfo
            {
                Name = "WebGlass",
                Description = "WebGlass Repository"
            };
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa um item do cache.
        /// </summary>
        class Item
        {
            #region Propriedades

            /// <summary>
            /// Identificador da ferragem.
            /// </summary>
            public int IdFerragem { get; set; }

            /// <summary>
            /// Nome da ferrage.
            /// </summary>
            public string Nome { get; set; }

            /// <summary>
            /// Identificador do fabricante.
            /// </summary>
            public int IdFabricanteFerragem { get; set; }

            /// <summary>
            /// Fabricante.
            /// </summary>
            public string Fabricante { get; set; }

            /// <summary>
            /// Códigos das ferragens.
            /// </summary>
            public string[] Codigos { get; set; }

            /// <summary>
            /// Parametros.
            /// </summary>
            public Constante[] Constantes { get; set; }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Recupera os valores de referência.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<CalcEngineReferenceValue> ObterValoresReferencia()
            {
                var path = string.Format("{0}/{1}", Fabricante, Nome);
                var descricao = string.Join(",", Codigos);

                foreach (var constante in Constantes)
                    yield return new CalcEngineReferenceValue
                    {
                        Path = string.Format("{0}/{1}", path, constante.Nome),
                        Description = descricao,
                        Value = constante.Valor
                    };
            }

            #endregion

        }

        /// <summary>
        /// Represent ao parametro da ferragem.
        /// </summary>
        class Constante
        {
            #region Propriedades

            /// <summary>
            /// Nome do parametro.
            /// </summary>
            public string Nome { get; set; }

            /// <summary>
            /// Valor do parametro.
            /// </summary>
            public double Valor { get; set; }

            #endregion
        }

        #endregion
    }
}
