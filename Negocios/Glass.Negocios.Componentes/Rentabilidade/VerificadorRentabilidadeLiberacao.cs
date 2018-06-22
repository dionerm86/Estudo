using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Representa o verificador de rentabilidade para liberação.
    /// </summary>
    public class VerificadorRentabilidadeLiberacao : IVerificadorRentabilidadeLiberacao
    {
        #region Variáveis Locais

        private readonly IDictionary<int, IEnumerable<Faixa>> _faixasLoja = new Dictionary<int, IEnumerable<Faixa>>();

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recuperea as faixas com base na loja de
        /// forma direta no banco de dados.
        /// </summary>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <returns></returns>
        private IEnumerable<Faixa> ObterFaixasDireto(int idLoja)
        {
            var idsFuncionario = new Dictionary<int, IEnumerable<int>>();
            var idsTipoFuncionario = new Dictionary<int, IEnumerable<int>>();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FuncionarioFaixaRentabilidadeLiberacao>("ff")
                    .InnerJoin<Data.Model.FaixaRentabilidadeLiberacao>("ff.IdFaixaRentabilidadeLiberacao=f.IdFaixaRentabilidadeLiberacao", "f")
                    .Where("f.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja)
                    .Select("ff.IdFaixaRentabilidadeLiberacao, ff.IdFunc"),
                    (sender, query, result) =>
                    {
                        // Recupera os registros agrupados pela faixa
                        foreach (var item in result.Select(f => new
                        {
                            IdFaixaRentabilidadeLiberacao = f.GetInt32("IdFaixaRentabilidadeLiberacao"),
                            IdFunc = f.GetInt32("IdFunc")
                        }).GroupBy(f => f.IdFaixaRentabilidadeLiberacao))
                        {
                            idsFuncionario.Add(item.Key, item.Select(f => f.IdFunc).ToArray());
                        }
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao>("tf")
                    .InnerJoin<Data.Model.FaixaRentabilidadeLiberacao>("tf.IdFaixaRentabilidadeLiberacao=f.IdFaixaRentabilidadeLiberacao", "f")
                    .Where("f.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja)
                    .Select("tf.IdFaixaRentabilidadeLiberacao, tf.IdTipoFuncionario"),
                    (sender, query, result) =>
                    {
                        // Recupera os registros agrupados pela faixa
                        foreach (var item in result.Select(f => new
                        {
                            IdFaixaRentabilidadeLiberacao = f.GetInt32("IdFaixaRentabilidadeLiberacao"),
                            IdTipoFuncionario = f.GetInt32("IdTipoFuncionario")
                        }).GroupBy(f => f.IdFaixaRentabilidadeLiberacao))
                        {
                            idsTipoFuncionario.Add(item.Key, item.Select(f => f.IdTipoFuncionario).ToArray());
                        }
                    })
                 .Execute();



            var registros = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FaixaRentabilidadeLiberacao>()
                    .Where("IdLoja=?idLoja")
                    .Add("?idLoja", idLoja)
                    .Execute()
                    .Select(f =>
                    {
                        var idFaixa = f.GetInt32(nameof(Data.Model.FaixaRentabilidadeLiberacao.IdFaixaRentabilidadeLiberacao));
                        IEnumerable<int> idsFuncionario1;
                        IEnumerable<int> idsTipoFuncionario1;

                        if (!idsFuncionario.TryGetValue(idFaixa, out idsFuncionario1))
                            idsFuncionario1 = new int[0];

                        if (!idsTipoFuncionario.TryGetValue(idFaixa, out idsTipoFuncionario1))
                            idsTipoFuncionario1 = new int[0];

                        return new
                        {
                            Id = idFaixa,
                            PercentualRentabilidade = f.GetDecimal(nameof(Data.Model.FaixaRentabilidadeLiberacao.PercentualRentabilidade)) / 100m,
                            RequerLiberacao = f.GetBoolean(nameof(Data.Model.FaixaRentabilidadeLiberacao.RequerLiberacao)),
                            IdsFuncionario = idsFuncionario1,
                            IdsTipoFuncionario = idsTipoFuncionario1
                        };
                    })
                    .OrderBy(f => f.PercentualRentabilidade)
                    .ToList();

            if (!registros.Any()) yield break;

            // Verifica se a faixa começa negativa para trabalhar 
            // com a lógica inicial invertida
            if (registros[0].PercentualRentabilidade < 0m)
            {
                var registro = registros[0];
                registros.RemoveAt(0);

                yield return new Faixa(
                    decimal.MinValue, registro.PercentualRentabilidade, 
                    registro.RequerLiberacao, registro.IdsFuncionario, registro.IdsTipoFuncionario);

                if (registros.Any())
                    foreach (var registro2 in registros)
                    {
                        yield return new Faixa(
                            registro.PercentualRentabilidade,
                            registro2.PercentualRentabilidade,
                            registro2.RequerLiberacao,
                            registro2.IdsFuncionario,
                            registro2.IdsTipoFuncionario);

                        registro = registro2;
                    }
            }
            else
            {
                var inicio = 0m;

                foreach (var registro in registros)
                {
                    yield return new Faixa(
                            inicio,
                            registro.PercentualRentabilidade,
                            registro.RequerLiberacao,
                            registro.IdsFuncionario,
                            registro.IdsTipoFuncionario);

                    inicio = registro.PercentualRentabilidade;
                }
            }
        }

        /// <summary>
        /// Recupera as faixas com base no identificador do funcionário.
        /// </summary>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        private IEnumerable<Faixa> ObterFaixas(int idLoja)
        {
            lock (_faixasLoja)
            {
                IEnumerable<Faixa> faixas;
                if (!_faixasLoja.TryGetValue(idLoja, out faixas))
                {
                    faixas = ObterFaixasDireto(idLoja).ToArray();
                    _faixasLoja.Add(idLoja, faixas);
                }

                return faixas;
            }
        }

        /// <summary>
        /// Obtém a faixa com base no item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Faixa ObterFaixa(IItemRentabilidade item)
        {
            var itemPedido = item as IItemRentabilidade<Data.Model.Pedido>;

            if (itemPedido != null)
            {
                var faixas = ObterFaixas((int)itemPedido.Proprietario.IdLoja);

                var faixa = faixas.FirstOrDefault(f =>
                    item.PercentualRentabilidade > f.Inicio &&
                    item.PercentualRentabilidade <= f.Fim);

                return faixa;
            }

            return null;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se é necessário liberação para o item informado.
        /// </summary>
        /// <param name="item">Item que será verificado.</param>
        /// <returns></returns>
        public bool VerificarRequerLiberacao(IItemRentabilidade item)
        {
            var faixa = ObterFaixa(item);

            return faixa?.RequerLiberacao ?? false;
        }

        /// <summary>
        /// Verifica se pode liberar o item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool PodeLiberar(IItemRentabilidade item)
        {
            var faixa = ObterFaixa(item);

            if (faixa?.RequerLiberacao ?? false)
            {
                if (!faixa.IdsFuncionario.Any() &&
                    !faixa.IdsTipoFuncionario.Any())
                    return Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
                

                if (Data.Helper.UserInfo.GetUserInfo != null)
                {
                    var usuario = Data.Helper.UserInfo.GetUserInfo;

                    // Verifica se o funcionário logado ou o tipo de funcionário 
                    // tem permissão para a liberação
                    return faixa.IdsFuncionario.Contains((int)usuario.CodUser) ||
                           faixa.IdsTipoFuncionario.Contains((int)usuario.TipoUsuario);
                }
            }

            return true;
        }

        /// <summary>
        /// Atualiza os dados do verificador.
        /// </summary>
        public void AtualizarDados()
        {
            lock (_faixasLoja)
                _faixasLoja.Clear();
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa uma faixa usada pela calculadora.
        /// </summary>
        class Faixa
        {
            /// <summary>
            /// Valor de início da faixa da rentabilidade.
            /// </summary>
            public decimal Inicio { get; }

            /// <summary>
            /// Valor de fim da faixa da rentabilidade.
            /// </summary>
            public decimal Fim { get; }

            /// <summary>
            /// Obtém se requer liberação.
            /// </summary>
            public bool RequerLiberacao { get; }

            /// <summary>
            /// Obtém os identificadores de funcionários associados com a faixa.
            /// </summary>
            public IEnumerable<int> IdsFuncionario { get; }

            /// <summary>
            /// Obtém os identificadores dos tipos de funcionários associados com a faixa.
            /// </summary>
            public IEnumerable<int> IdsTipoFuncionario { get; }

            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="inicio"></param>
            /// <param name="fim"></param>
            /// <param name="requerLiberacao"></param>
            /// <param name="idsFuncionario"></param>
            /// <param name="idsTipoFuncionario"></param>
            public Faixa(
                decimal inicio, decimal fim, bool requerLiberacao, 
                IEnumerable<int> idsFuncionario, IEnumerable<int> idsTipoFuncionario)
            {
                Inicio = inicio;
                Fim = fim;
                RequerLiberacao = requerLiberacao;
                IdsFuncionario = idsFuncionario;
                IdsTipoFuncionario = idsTipoFuncionario;
            }

            #endregion
        }

        #endregion
    }
}
