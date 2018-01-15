using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using Colosoft;

namespace Glass
{
    /// <summary>
    /// Armazena as informações do enumerador.
    /// </summary>
    public class InformacaoEnumerador
    {
        #region Propriedades

        /// <summary>
        /// Valor do enumerador.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Nome do enumerador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Descricao do enumerador.
        /// </summary>
        public string Descricao { get; set; }

        #endregion

        #region Métodos

        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}({2})", Id, Nome, Descricao);
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public InformacaoEnumerador()
        {
        }

        /// <summary>
        /// Construtor de inicialização.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nome"></param>
        /// <param name="descricao"></param>
        public InformacaoEnumerador(int? id, string nome, string descricao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
        }

        /// <summary>
        /// Cria a instancia com o valor do enumerador e a descricao.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="descricao"></param>
        public InformacaoEnumerador(object valor, string descricao)
        {
            valor.Require("valor").NotNull();

            var valorType = valor.GetType();

            if (valorType.IsEnum)
            {
                var tipoEnumerador = valorType.UnderlyingSystemType;
                Nome = Enum.GetName(tipoEnumerador, valor);
                Id = RepositorioEnumedores.ObterValor(Enum.GetUnderlyingType(tipoEnumerador).Name, valor);
                Descricao = descricao ?? Nome;
            }
            else
                throw new InvalidOperationException("O valor não é compatível com um Enum");
        }

        #endregion
    }

    /// <summary>
    /// Representa um repositório que gerencia
    /// os enumeradores do sistema.
    /// </summary>
    public static class RepositorioEnumedores
    {
        #region Variáveis Locais

        private static readonly InformacaoEnumerador[][] _colecaoVazia = new InformacaoEnumerador[0][];
        private static Dictionary<string, IEnumerable<IEnumerable<InformacaoEnumerador>>> _cache =
            new Dictionary<string, IEnumerable<IEnumerable<InformacaoEnumerador>>>();

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera as informados dos valores do enumerador.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<InformacaoEnumerador>> CarregarInformacoes(Type tipoEnumerador)
        {
            var resultado = new List<InformacaoEnumerador>();

            // Verifica se o tipo e um enumerador
            if (tipoEnumerador.IsEnum)
            {
                object provedor = null;

                try
                {
                    // Recupera o provedor de traducao
                    provedor = tipoEnumerador
                        .GetCustomAttributes(typeof(ProvedorTraducaoAttribute), false)
                        .Select(f => Activator.CreateInstance(((ProvedorTraducaoAttribute)f).Tipo))
                        .FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format("Não foi possível carregar o provedor de tradução para o tipo '{0}'", tipoEnumerador.FullName), ex);
                }

                // Verifica se carregou um provedor de tradução
                if (provedor is IProvedorTraducao)
                    return new InformacaoEnumerador[][]
                        {
                            ((IProvedorTraducao)provedor).ObterTraducoes().ToArray()
                        };

                // Verifica se carregou um provedor de tradução multipla
                else if (provedor is IProvedorMultiplaTraducao)
                    return ((IProvedorMultiplaTraducao)provedor).ObterTraducoes();

                var tipoBase = Enum.GetUnderlyingType(tipoEnumerador).Name;

                // Percorre os campos do enumerador
                foreach (var campo in tipoEnumerador.GetFields().Where(f => f.FieldType.IsEnum))
                {
                    // Tenta recuperar a descrica do campo, se não encontrar usa o nome mesmo
                    var descricao = campo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                        .Select(f => ((System.ComponentModel.DescriptionAttribute)f).Description)
                        .FirstOrDefault() ?? campo.Name;

                    // Recupera o valor do campo
                    var valor = campo.GetValue(null);
                    var id = ObterValor(tipoBase, valor);

                    resultado.Add(new InformacaoEnumerador
                    {
                        Id = id,
                        Nome = campo.Name,
                        Descricao = descricao
                    });
                }
            }

            if (resultado.Count == 0)
                return _colecaoVazia;

            return new InformacaoEnumerador[][]
                        {
                            resultado.ToArray()
                        };
        }

        /// <summary>
        /// Recupera o valor UInt associado.
        /// </summary>
        /// <param name="tipoBase"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        internal static int ObterValor(string tipoBase, object valor)
        {
            switch (tipoBase)
            {
                case "Int32":
                    return (int)valor;
                case "Int64":
                    return (int)(long)valor;
                case "Int16":
                    return (int)(short)valor;
                case "UInt32":
                    return (int)(uint)valor;
                case "UInt16":
                    return (int)(ushort)valor;
                case "UInt64":
                    return (int)(ulong)valor;
                case "Byte":
                    return (int)(byte)valor;
            }
            return 0;
        }

        #endregion

        #region Métodos Públicos

        ///// <summary>
        ///// Traduz o valor informado.
        ///// </summary>
        ///// <param name="valor"></param>
        ///// <param name="posicaoTraducao"></param>
        ///// <returns></returns>
        //public static string Traduz(object valor)
        //{
        //    return Traduz(valor, 0);
        //}

        ///// <summary>
        ///// Traduz o valor informado.
        ///// </summary>
        ///// <param name="valor"></param>
        ///// <param name="posicaoTraducao"></param>
        ///// <returns></returns>
        //public static string Traduz(object valor, int posicaoTraducao)
        //{
        //    valor.Requerido("valor").NaoNulo();

        //    var valorType = valor.GetType();

        //    if (valorType.IsEnum)
        //    {
        //        var tipoEnumerador = valorType.UnderlyingSystemType;
        //        return Traduz(tipoEnumerador, valor, posicaoTraducao);
        //    }
        //    else
        //        throw new InvalidOperationException("O valor não é compatível com um Enum");
        //}

        /// <summary>
        /// Traduz o valor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string Traduz<T>(T valor)
        {
            return Traduz(typeof(T), valor);
        }

        /// <summary>
        /// Traduz o valor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valor"></param>
        /// <param name="posicaoTraducao">Posição da tradução que será recuperada.</param>
        /// <returns></returns>
        public static string Traduz<T>(T valor, int posicaoTraducao)
        {
            return Traduz(typeof(T), valor, posicaoTraducao);
        }

        /// <summary>
        /// Traduz o valor do tipo.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string Traduz(Type tipoEnumerador, object valor)
        {
            return Traduz(tipoEnumerador, valor, 0);
        }

        /// <summary>
        /// Traduz o valor do tipo.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <param name="valor"></param>
        /// <param name="posicaoTraducao">Posição da tradução que será recuperada.</param>
        /// <returns></returns>
        public static string Traduz(Type tipoEnumerador, object valor, int posicaoTraducao)
        {
            if (valor == null)
                return string.Empty;

            var id = ObterValor(Enum.GetUnderlyingType(tipoEnumerador).Name, valor);

            return ObtemInformacoes(tipoEnumerador, posicaoTraducao)
                    .Where(f => f.Id == id)
                    .Select(f => f.Descricao)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Recupera as informados dos valores do enumerador.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<InformacaoEnumerador> ObtemInformacoes<T>()
        {
            return ObtemInformacoes(typeof(T));
        }

        /// <summary>
        /// Recupera as informados dos valores do enumerador.
        /// </summary>
        /// <param name="posicaoTraducao">Posição da tradução que será recuperada.</param>
        /// <returns></returns>
        public static IEnumerable<InformacaoEnumerador> ObtemInformacoes<T>(int posicaoTraducao)
        {
            return ObtemInformacoes(typeof(T), posicaoTraducao);
        }

        /// <summary>
        /// Recupera as informados dos valores do enumerador.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <param name="posicaoTraducao">Posição da tradução que será recuperada.</param>
        /// <returns></returns>
        public static IEnumerable<InformacaoEnumerador> ObtemInformacoes(Type tipoEnumerador)
        {
            return ObtemInformacoes(tipoEnumerador, 0);
        }

        /// <summary>
        /// Recupera as informações do enumerador.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <returns></returns>
        public static IEnumerable<InformacaoEnumerador> ObtemInformacoes(string tipoEnumerador)
        {
            return ObtemInformacoes(Type.GetType(tipoEnumerador));
        }

        /// <summary>
        /// Recupera as informados dos valores do enumerador.
        /// </summary>
        /// <param name="tipoEnumerador"></param>
        /// <param name="posicaoTraducao">Posição da tradução que será recuperada.</param>
        /// <returns></returns>
        public static IEnumerable<InformacaoEnumerador> ObtemInformacoes(Type tipoEnumerador, int posicaoTraducao)
        {
            tipoEnumerador.Require("tipoEnumerador").NotNull();

            IEnumerable<IEnumerable<InformacaoEnumerador>> result = null;

            var nomeTipo = tipoEnumerador.FullName;
            lock (_cache)
                // Tenta recupera os valores do cache
                if (!_cache.TryGetValue(nomeTipo, out result))
                {
                    result = CarregarInformacoes(tipoEnumerador);
                    _cache.Add(nomeTipo, result);
                }

            if (result is InformacaoEnumerador[][])
                try
                {
                    return ((InformacaoEnumerador[][])result)[posicaoTraducao];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new InvalidOperationException("Não foi encontrada a tradução na posição informada.", ex);
                }

            throw new InvalidOperationException("A relação das informações possui o tipo errado.");
        }

        #endregion

        /// <summary>
        /// Recupera a descrição do enumerador que possui o atributo using System.ComponentModel.Description.
        /// </summary>
        /// <param name="value">Enumerador</param>
        /// <returns>string</returns>
        public static string GetEnumDescricao(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }
            return value.ToString();
        }
    }
}
