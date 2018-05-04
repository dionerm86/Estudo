using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade.Relatorios.ItemRentabilidade
{
    /// <summary>
    /// Representa uma fonte de dados para um item de rentabilidade.
    /// </summary>
    public class ItemRentabilidadeDataSource
    {
        #region Propriedades

        /// <summary>
        /// Item associado.
        /// </summary>
        public IItemRentabilidade Item { get; }

        /// <summary>
        /// Provedor dos descritores.
        /// </summary>
        protected IProvedorDescritorRegistroRentabilidade ProvedorDescritores { get; }

        /// <summary>
        /// Cultura que será usada pela instância.
        /// </summary>
        protected System.Globalization.CultureInfo Cultura { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorDescritores"></param>
        /// <param name="item"></param>
        /// <param name="culture"></param>
        public ItemRentabilidadeDataSource(
            IProvedorDescritorRegistroRentabilidade provedorDescritores, 
            IItemRentabilidade item,
            System.Globalization.CultureInfo culture = null)
        {
            provedorDescritores.Require(nameof(provedorDescritores)).NotNull();
            item.Require(nameof(item)).NotNull();
            ProvedorDescritores = provedorDescritores;
            Item = item;
            Cultura = culture ?? System.Globalization.CultureInfo.CurrentCulture;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera o Json dos descritores.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDictionary<string, object>> ObterJsonDescritores()
        {
            var tipos = new List<TipoRegistroRentabilidade>();

            foreach (TipoRegistroRentabilidade i in Enum.GetValues(typeof(TipoRegistroRentabilidade)))
                tipos.Add(i);


            // Carrega os descritores por tipo
            var descritoresPorTipo = tipos.Select(tipo =>
                ProvedorDescritores.ObterDescritores(tipo)
                    .Where(f => f.ExibirRelatorio)
                    .Select(f => new { Tipo = tipo, Nome = f.Nome, f.Descricao, Posicao = f.Posicao }));

            // Carrega os descritores de for única
            var descritores = descritoresPorTipo.First();
            foreach (var i in descritoresPorTipo.Skip(1))
                descritores = descritores.Concat(i);
                
            var resultado = new List<IDictionary<string, object>>();

            // Percorre os descritores de forma ordenada
            foreach(var descritor in descritores.OrderBy(f => f.Posicao))
            {
                var json = new Dictionary<string, object>
                {
                    { "Tipo", descritor.Tipo.ToString() },
                    { "Nome", descritor.Nome },
                    { "Descricao", descritor.Descricao },
                    { "Posicao", descritor.Posicao }
                };

                resultado.Add(json);
            }

            return resultado;
        }

        /// <summary>
        /// Converte o item informado para um dicionario que
        /// será serializa para JSON.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private IDictionary<string, object> ToJson(IItemRentabilidade item)
        {
            // Carrega os dados básicos do item
            var dados = new Dictionary<string, object>
            {
                { "Descricao", item.Descricao },
                { "PercentualRentabilidade", item.PercentualRentabilidade.ToString("P", Cultura) },
                { "RentabilidadeFinanceira", item.RentabilidadeFinanceira.ToString("R$#,##0.00;\\(R$#,##0.00\\)", Cultura) }
            };

            foreach (var registro in item.RegistrosRentabilidade)
            {
                var descritor = registro.Descritor;

                // Verifica se o registro deve ser exibido no relatório
                if (descritor != null && descritor.ExibirRelatorio)
                    dados.Add(descritor.Nome, descritor.FormatarValor(registro, Cultura));
            }

            var container = item as IItemRentabilidadeContainer;
            if (container != null)
            {
                var itens = new List<IDictionary<string, object>>();
                foreach (var item1 in container.Itens)
                    itens.Add(ToJson(item1));

                dados.Add("Itens", itens);
            }

            return dados;
        }

        #endregion

        #region Método Públicos

        /// <summary>
        /// Converte os dados do item para uma estrutura que seje possível gerar um JSON.
        /// </summary>
        public IDictionary<string, object> ToJson()
        {
            return new Dictionary<string, object>
            {
                { "Descritores", ObterJsonDescritores() },
                { "Item", ToJson(Item) }
            };
        }

        #endregion
    }
}
