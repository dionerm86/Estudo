using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio da rentabilidade.
    /// </summary>
    public class RentabilidadeFluxo : 
        IRentabilidadeFluxo, 
        IProvedorCalculadoraRentabilidade, 
        IProvedorIndicadorFinanceiro, 
        IProvedorDescritorRegistroRentabilidade, 
        Entidades.IProvedorExpressaoRentabilidade,
        Entidades.IProvedorConfigRegistroRentabilidade
    {
        #region Variáveis Locais

        private Rentabilidade.CalculadoraRentabilidade _calculadora;
        private IDictionary<string, decimal> _indicadoresFinanceiros;
        private IList<DescritorExpressaoRentabilidade> _descritoresExpressaoRentabilidade;
        private IList<DescritorIndicadorFinanceiro> _descritoresIndicadoreFinanceiro;
        private IList<DescritorVariavelItem> _descritoresVariavelItem;

        #endregion

        #region Propriedades

        /// <summary>
        /// Relação dos indicadores financeiros.
        /// </summary>
        private IDictionary<string, decimal> IndicadoresFinanceiros
        {
            get
            {
                AsseguraCargaIndicadoresFinanceiros();
                return _indicadoresFinanceiros;
            }
        }

        /// <summary>
        /// Descritores dos indicadores financeiros.
        /// </summary>
        private IList<DescritorIndicadorFinanceiro> DescritoresIndicadorFinanceiro
        {
            get
            {
                AsseguraCargaIndicadoresFinanceiros();
                return _descritoresIndicadoreFinanceiro;
            }
        }

        /// <summary>
        /// Descritores das expressões.
        /// </summary>
        private IList<DescritorExpressaoRentabilidade> DescritoresExpressaoRentabilidade
        {
            get
            {
                AsseguraCalcularadoCriada();
                return _descritoresExpressaoRentabilidade;
            }
        }

        /// <summary>
        /// Descritores das variáveis dos itens.
        /// </summary>
        private IList<DescritorVariavelItem> DescritoresVariavelItem
        {
            get
            {
                AsseguraCargaVariaveisItens();
                return _descritoresVariavelItem;
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Assegura a carga das variáveis dos itens.
        /// </summary>
        private void AsseguraCargaVariaveisItens()
        {
            if (_descritoresVariavelItem == null)
            {
                var configs = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ConfigRegistroRentabilidade>()
                    .Where("Tipo=?tipo")
                    .Add("?tipo", Rentabilidade.TipoRegistroRentabilidade.VariavelItem)
                    .Select("IdRegistro, Posicao, ExibirRelatorio")
                    .Execute()
                    .Select(f => new
                    {
                        IdRegistro = f.GetInt32("IdRegistro"),
                        Posicao = f.GetInt32("Posicao"),
                        ExibirRelatorio = f.GetBoolean("ExibirRelatorio")
                    }).ToList();

                var descritores = Colosoft.Translator.GetTranslates<TipoVariavelItemRentabilidade>()
                    .Select(tranlate =>
                    {
                        var config = configs.FirstOrDefault(f => f.IdRegistro == (int)tranlate.Key);

                        return new DescritorVariavelItem((TipoVariavelItemRentabilidade)tranlate.Key,
                            tranlate.Key.ToString(), tranlate.Translation,
                            config?.Posicao ?? 999, config?.ExibirRelatorio ?? false);
                    }).ToList();

                _descritoresVariavelItem = descritores;
            }
        }

        /// <summary>
        /// Assegura a carga dos indicadores financeiros.
        /// </summary>
        private void AsseguraCargaIndicadoresFinanceiros()
        {
            if (_indicadoresFinanceiros == null)
            {
                var indicadoresFinanceiros = new Dictionary<string, decimal>();
                var descritores = new List<DescritorIndicadorFinanceiro>();

                // Carrega a relação completa dos indicadores financeiros
                foreach (var indicador in SourceContext.Instance.CreateQuery()
                    .From<Data.Model.IndicadorFinanceiro>("i")
                    .LeftJoin<Data.Model.ConfigRegistroRentabilidade>("c.Tipo=?tipo AND c.IdRegistro=i.IdIndicadorFinanceiro", "c")
                    .Add("?tipo", TipoRegistroRentabilidade.IndicadorFinaceiro)
                    .Select("i.IdIndicadorFinanceiro, i.Nome, i.Descricao, i.Valor, " +
                            "ISNULL(c.Posicao, 999) AS Posicao, ISNULL(c.ExibirRelatorio, 0) AS ExibirRelatorio, i.Formatacao")
                    .Execute()
                    .Select(f => new
                    {
                        IdIndicadorFinanceiro = f.GetInt32("IdIndicadorFinanceiro"),
                        Nome = f.GetString("Nome"),
                        Descricao = f.GetString("Descricao"),
                        Formatacao = f.GetString("Formatacao"),
                        Posicao = f.GetInt32("Posicao"),
                        ExibirRelatorio = f.GetBoolean("ExibirRelatorio"),
                        Valor = f.GetDecimal("Valor")
                    }))
                {
                    if (!indicadoresFinanceiros.ContainsKey(indicador.Nome))
                    {
                        indicadoresFinanceiros.Add(indicador.Nome, indicador.Valor);
                        descritores.Add(new DescritorIndicadorFinanceiro(
                            indicador.IdIndicadorFinanceiro, indicador.Nome, indicador.Descricao, 
                            indicador.Posicao, indicador.ExibirRelatorio, indicador.Formatacao));
                    }
                }

                _indicadoresFinanceiros = indicadoresFinanceiros;
                _descritoresIndicadoreFinanceiro = descritores;
            }
        }

        /// <summary>
        /// Assegura que a instancia da calculadora foi criada.
        /// </summary>
        private void AsseguraCalcularadoCriada()
        {
            if (_calculadora == null)
                _calculadora = CriarCalculadora();
        }

        /// <summary>
        /// Cria o calculo da rentabilidade.
        /// </summary>
        /// <returns></returns>
        private Rentabilidade.CalculoRentabilidade CriarCalculoRentabilidade()
        {
            var calculo = new CalculoRentabilidade();

            var expressoes = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ExpressaoRentabilidade>("e")
                .LeftJoin<Data.Model.ConfigRegistroRentabilidade>("c.Tipo=?tipo AND c.IdRegistro=e.IdExpressaoRentabilidade", "c")
                .Add("?tipo", TipoRegistroRentabilidade.Expressao)
                .Select("e.IdExpressaoRentabilidade, e.Nome, e.Descricao, e.Expressao, e.Formatacao, " + 
                        "ISNULL(c.Posicao, 9999) AS Posicao, ISNULL(c.ExibirRelatorio, 0) AS ExibirRelatorio, e.SomaFormulaRentabilidade")
                .Execute()
                .Select(f => new
                {
                    IdExpressaoRentabilidade = f.GetInt32("IdExpressaoRentabilidade"),
                    Nome = f.GetString("Nome"),
                    Descricao = f.GetString("Descricao"),
                    Expressao = f.GetString("Expressao"),
                    Formatacao = f.GetString("Formatacao"),
                    Posicao = f.GetInt32("Posicao"),
                    ExibirRelatorio = f.GetBoolean("ExibirRelatorio"),
                    SomaFormulaRentabilidade = f.GetBoolean("SomaFormulaRentabilidade")
                }).ToList();

            foreach (var i in expressoes)
                calculo.Add(i.Nome, i.Expressao);

            _descritoresExpressaoRentabilidade = expressoes
                .Select(f => new DescritorExpressaoRentabilidade(
                    f.IdExpressaoRentabilidade, f.Nome, f.Descricao, 
                    f.Posicao, f.ExibirRelatorio, f.Formatacao)).ToList();

            calculo.Formula = string.Join(" + ", expressoes.Where(f => f.SomaFormulaRentabilidade).Select(f => f.Nome));
            return calculo;
        }

        /// <summary>
        /// Cria uma instancia da calculadora de rentabilidade carregando as configurações do banco de dados.
        /// </summary>
        /// <returns></returns>
        private Rentabilidade.CalculadoraRentabilidade CriarCalculadora()
        {
            var calculo = CriarCalculoRentabilidade();

            var calculadora = new Rentabilidade.CalculadoraRentabilidade(this, calculo);
            calculadora.Preparar();

            return calculadora;
        }

        /// <summary>
        /// Redefine a calculadora de rentabilidade.
        /// </summary>
        private void RedefinirCalculadora()
        {
            _calculadora = null;
        }

        /// <summary>
        /// Atualiza as expressões da rentabilidade.
        /// </summary>
        private void AtualizarExpressoesRentabilidade()
        {
            RedefinirCalculadora();
        }

        /// <summary>
        /// Atualiza os indicadores financeiros na calcularado ativa.
        /// </summary>
        private void AtualizarIndicadoresFinanceiros()
        {
            _indicadoresFinanceiros = null;
        }

        /// <summary>
        /// Atualiza os descritores das variaveis dos itens.
        /// </summary>
        private void AtualizarDescritoresVariaveisItens()
        {
            _descritoresVariavelItem = null;
        }

        #endregion

        #region ExpressaoRentabilidade

        /// <summary>
        /// Cria uma instancia do calculo.
        /// </summary>
        /// <returns></returns>
        public Entidades.ExpressaoRentabilidade CriarExpressaoRentabilidade()
        {
            return SourceContext.Instance.Create<Entidades.ExpressaoRentabilidade>();
        }

        /// <summary>
        /// Recupera o calculo pelo identificador informado.
        /// </summary>
        /// <param name="idExpressaoRentabilidade"></param>
        /// <returns></returns>
        public Entidades.ExpressaoRentabilidade ObterExpressaoRentabilidade(int idExpressaoRentabilidade)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ExpressaoRentabilidade>()
                .Where($"IdExpressaoRentabilidade=?id")
                .Add("?id", idExpressaoRentabilidade)
                .ProcessLazyResult<Entidades.ExpressaoRentabilidade>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Pesquisa os calculos.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IList<Entidades.ExpressaoRentabilidade> PesquisarExpressoesRentabilidade(string nome)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ExpressaoRentabilidade>()
                .OrderBy("Posicao");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("Nome LIKE ?nome")
                    .Add("?nome", $"%{nome}$");

            return consulta.ToVirtualResult<Entidades.ExpressaoRentabilidade>();
        }

        /// <summary>
        /// Salva os dados do calculo de rentabilidade.
        /// </summary>
        /// <param name="expressaoRentabilidade"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarExpressaoRentabilidade(Entidades.ExpressaoRentabilidade expressaoRentabilidade)
        {
            var resultado = SourceContext.Instance.ExecuteSave(expressaoRentabilidade);

            if (resultado)
                RedefinirCalculadora();

            return resultado;
        }

        /// <summary>
        /// Apaga os dados do calculo de rentabilidade.
        /// </summary>
        /// <param name="expressaoRentabilidade"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarExpressaoRentabilidade(Entidades.ExpressaoRentabilidade expressaoRentabilidade)
        {
            var resultado = SourceContext.Instance.ExecuteDelete(expressaoRentabilidade);

            if (resultado)
                RedefinirCalculadora();

            return resultado;
        }

        #endregion

        #region IndicadorFinanceiro

        /// <summary>
        /// Cria uma instancia do indicador financeiro.
        /// </summary>
        /// <returns></returns>
        public Entidades.IndicadorFinanceiro CriarIndicadorFinanceiro()
        {
            return SourceContext.Instance.Create<Entidades.IndicadorFinanceiro>();
        }

        /// <summary>
        /// Recupera o indicador financeiro pelo identificador informado.
        /// </summary>
        /// <param name="idIndicadorFinanceiro"></param>
        /// <returns></returns>
        public Entidades.IndicadorFinanceiro ObterIndicadorFinanceiro(int idIndicadorFinanceiro)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.IndicadorFinanceiro>()
                .Where("IdIndicadorFinanceiro=?id")
                .Add("?id", idIndicadorFinanceiro)
                .ProcessLazyResult<Entidades.IndicadorFinanceiro>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Pesquisa os indicadores financeiros.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IList<Entidades.IndicadorFinanceiro> PesquisaIndicadoresFinanceiros(string nome)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.IndicadorFinanceiro>()
                .OrderBy("Nome");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("Nome LIKE ?nome")
                    .Add("?nome", $"%{nome}%");

            return consulta.ToVirtualResult<Entidades.IndicadorFinanceiro>();
        }

        /// <summary>
        /// Salva os dados do indicador financeiro.
        /// </summary>
        /// <param name="indicadorFinanceiro"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarIndicadorFinanceiro(Entidades.IndicadorFinanceiro indicadorFinanceiro)
        {
            var resultado = SourceContext.Instance.ExecuteSave(indicadorFinanceiro);

            if (resultado)
                AtualizarIndicadoresFinanceiros();

            return resultado;
        }

        /// <summary>
        /// Apaga os dados do indicador financeiro.
        /// </summary>
        /// <param name="indicadorFinanceiro"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarIndicadorFinanceiro(Entidades.IndicadorFinanceiro indicadorFinanceiro)
        {
            var resultado = SourceContext.Instance.ExecuteDelete(indicadorFinanceiro);

            if (resultado)
            {
                AtualizarIndicadoresFinanceiros();
                RedefinirCalculadora();
            }

            return resultado;
        }

        #endregion

        #region ConfigRegistroRentabilidade

        /// <summary>
        /// Recupera a configuração do registro de rentabilidade.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="idRegistro">Identificador do registro.</param>
        /// <returns></returns>
        public Entidades.ConfigRegistroRentabilidade ObterConfigRegistroRentabilidade(int tipo, int idRegistro)
        {
            var config = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ConfigRegistroRentabilidade>()
                .Where("Tipo=?tipo AND IdRegistro=?idRegistro")
                .Add("?tipo", tipo)
                .Add("?idRegistro", idRegistro)
                .ProcessResult<Entidades.ConfigRegistroRentabilidade>()
                .FirstOrDefault();

            if (config == null)
            {
                config = SourceContext.Instance.Create<Entidades.ConfigRegistroRentabilidade>();
                config.Tipo = tipo;
                config.IdRegistro = idRegistro;
            }

            return config;
        }

        /// <summary>
        /// Recupera as configurações dos registros de rentabilidade.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.ConfigRegistroRentabilidadePesquisa> ObterConfigsRegistroRentabilidade()
        {
            var consultaExpressoes = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ExpressaoRentabilidade>("e")
                .LeftJoin<Data.Model.ConfigRegistroRentabilidade>("c1.Tipo=?tipo1 AND c1.IdRegistro=e.IdExpressaoRentabilidade", "c1")
                .Add("?tipo1", (int)TipoRegistroRentabilidade.Expressao)
                .Select("?tipo1 AS Tipo, e.IdExpressaoRentabilidade AS IdRegistro, ISNULL(c1.Posicao, 9999) AS Posicao, ISNULL(c1.ExibirRelatorio, 0) AS ExibirRelatorio, e.Nome, e.Descricao");

            var consultaIndicadores = SourceContext.Instance.CreateQuery()
                .From<Data.Model.IndicadorFinanceiro>("i")
                .LeftJoin<Data.Model.ConfigRegistroRentabilidade>("c2.Tipo=?tipo2 AND c2.IdRegistro=i.IdIndicadorFinanceiro", "c2")
                .Add("?tipo2", (int)TipoRegistroRentabilidade.IndicadorFinaceiro)
                .Select("?tipo2 AS Tipo, i.IdIndicadorFinanceiro AS IdRegistro, ISNULL(c2.Posicao, 9999) AS Posicao, ISNULL(c2.ExibirRelatorio, 0) AS ExibirRelatorio, i.Nome, i.Descricao");

            var consultaVariaveisItem = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ConfigRegistroRentabilidade>()
                .Where("Tipo=?tipo3")
                .Add("?tipo3", (int)TipoRegistroRentabilidade.VariavelItem)
                .Select("?tipo3 AS Tipo, IdRegistro, Posicao, ExibirRelatorio, NULL AS Nome, NULL AS Descricao")
                .UnionAll(consultaExpressoes)
                .UnionAll(consultaIndicadores);

            var configs = consultaVariaveisItem
                .Execute<Entidades.ConfigRegistroRentabilidadePesquisa>()
                .OrderBy(f => f.Posicao)
                .ToList();

            if (configs.Any())
            {
                configs.First().PodeMoverParaCima = false;
                configs.Last().PodeMoverParaBaixo = false;
            }

            var ultimaPosicao = 0;

            foreach (var config in configs)
            {
                if (config.Posicao == 9999)
                    config.Posicao = ultimaPosicao + 1;

                if (config.Tipo == (byte)TipoRegistroRentabilidade.VariavelItem)
                {
                    var tipoVariavel = (Rentabilidade.TipoVariavelItemRentabilidade)config.IdRegistro;
                    config.Nome = tipoVariavel.ToString();
                    config.Descricao = tipoVariavel.Translate().FormatOrNull();
                }

                ultimaPosicao = config.Posicao;
            }

            return configs;
        }

        /// <summary>
        /// Salva os dados da configuração.
        /// </summary>
        /// <param name="configRegistroRentabilidade"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarConfigRegistroRentabilidade(Entidades.ConfigRegistroRentabilidade configRegistroRentabilidade)
        {
            var resultado = SourceContext.Instance.ExecuteSave(configRegistroRentabilidade);

            if (resultado)
            {
                switch ((Rentabilidade.TipoRegistroRentabilidade)configRegistroRentabilidade.Tipo)
                {
                    case Rentabilidade.TipoRegistroRentabilidade.VariavelItem:
                        AtualizarDescritoresVariaveisItens();
                        break;

                    case TipoRegistroRentabilidade.Expressao:
                        AtualizarExpressoesRentabilidade();
                        break;

                    case TipoRegistroRentabilidade.IndicadorFinaceiro:
                        AtualizarIndicadoresFinanceiros();
                        break;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Move a configuração para cima.
        /// </summary>
        /// <param name="configRegistroRentabilidade"></param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult MoverConfigRegistroRentabilidade(Entidades.ConfigRegistroRentabilidade configRegistroRentabilidade, bool paraCima)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ConfigRegistroRentabilidade>()
                .Add("?posicao", configRegistroRentabilidade.Posicao)
                .Take(1);

            if (paraCima)
                consulta.Where("Posicao<?posicao")
                    .OrderBy("Posicao DESC");
            else
                consulta.Where("Posicao>?posicao")
                   .OrderBy("Posicao");

            var config2 = consulta
                .ProcessResult<Entidades.ConfigRegistroRentabilidade>()
                .FirstOrDefault();

            // Verifica se encontra a configuração que será trocada
            if (config2 != null)
            {
                var posicao = configRegistroRentabilidade.Posicao;
                configRegistroRentabilidade.Posicao = config2.Posicao;
                config2.Posicao = posicao;
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = configRegistroRentabilidade.Save(session);
                if (!resultado)
                    return resultado;

                if (config2 != null)
                {
                    resultado = config2.Save(session);
                    if (!resultado)
                        return resultado;
                }

                resultado = session.Execute(false).ToSaveResult();

                if (resultado)
                {
                    if (configRegistroRentabilidade.Tipo == (byte)TipoRegistroRentabilidade.Expressao ||
                        config2?.Tipo == (byte)TipoRegistroRentabilidade.Expressao)
                        AtualizarExpressoesRentabilidade();

                    if (configRegistroRentabilidade.Tipo == (byte)TipoRegistroRentabilidade.VariavelItem ||
                        config2?.Tipo == (byte)TipoRegistroRentabilidade.VariavelItem)
                        AtualizarDescritoresVariaveisItens();

                    if (configRegistroRentabilidade.Tipo == (byte)TipoRegistroRentabilidade.IndicadorFinaceiro ||
                       config2?.Tipo == (byte)TipoRegistroRentabilidade.IndicadorFinaceiro)
                        AtualizarIndicadoresFinanceiros();
                }

                return resultado;
            }
        }

        #endregion

        #region Membros de IProvedorCalculadoraRentabilidade

        /// <summary>
        /// Instancia da calculadora associada.
        /// </summary>
        public Rentabilidade.CalculadoraRentabilidade Calculadora
        {
            get
            {
                AsseguraCalcularadoCriada();
                return _calculadora;
            }
        }

        #endregion

        #region Membros de IProvedorIndicadorFinanceiro

        /// <summary>
        /// Nome dos indicadores disponíveis. 
        /// </summary>
        IEnumerable<string> IProvedorIndicadorFinanceiro.Nomes => IndicadoresFinanceiros.Keys;

        /// <summary>
        /// Verifica se existe um indicador com o nome informado.
        /// </summary>
        /// <param name="nome">Nome do indicador.</param>
        /// <returns></returns>
        bool IProvedorIndicadorFinanceiro.Contains(string nome) => IndicadoresFinanceiros.ContainsKey(nome);

        /// <summary>
        /// Recupera o valor do indicador financiero pelo nome informado.
        /// </summary>
        /// <param name="nome">Nome do indicador.</param>
        /// <returns></returns>
        decimal IProvedorIndicadorFinanceiro.this[string nome] => IndicadoresFinanceiros[nome];

        #endregion

        #region Membros de IProvedorDescritorRegistroRentabilidade

        /// <summary>
        /// Recupera os descritores com base no tipo de registro informado.
        /// </summary>
        /// <param name="tipo">Tipo de registro que será usado para filtrar os descritores.</param>
        /// <returns></returns>
        IEnumerable<DescritorRegistroRentabilidade> IProvedorDescritorRegistroRentabilidade.ObterDescritores(TipoRegistroRentabilidade tipo)
        {
            switch(tipo)
            {
                case TipoRegistroRentabilidade.Expressao:
                    return DescritoresExpressaoRentabilidade;

                case TipoRegistroRentabilidade.IndicadorFinaceiro:
                    return DescritoresIndicadorFinanceiro;

                case TipoRegistroRentabilidade.VariavelItem:
                    return DescritoresVariavelItem;
            }

            return new DescritorRegistroRentabilidade[0];
        }

        /// <summary>
        /// Recupera o descritor.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="id">Identificador do registor.</param>
        /// <returns></returns>
        DescritorRegistroRentabilidade IProvedorDescritorRegistroRentabilidade.ObterDescritor(TipoRegistroRentabilidade tipo, int id)
        {
            switch(tipo)
            {
                case TipoRegistroRentabilidade.Expressao:
                    return DescritoresExpressaoRentabilidade.FirstOrDefault(f => f.IdExpressaoRentabilidade == id);

                case TipoRegistroRentabilidade.IndicadorFinaceiro:
                    return DescritoresIndicadorFinanceiro.FirstOrDefault(f => f.IdIndicadorFinanceiro == id);

                case TipoRegistroRentabilidade.VariavelItem:
                    return DescritoresVariavelItem.FirstOrDefault(f => f.TipoVariavel == (TipoVariavelItemRentabilidade)id);
            }

            return null;
        }

        /// <summary>
        /// Recupera o identificador do registro pelo tipo e pelo nome informado.
        /// </summary>
        /// <param name="tipo">Tipo de registro.</param>
        /// <param name="nome">Nome do registro.</param>
        /// <returns>Identificador do registro.</returns>
        int IProvedorDescritorRegistroRentabilidade.ObterRegistro(TipoRegistroRentabilidade tipo, string nome)
        {
            switch (tipo)
            {
                case TipoRegistroRentabilidade.Expressao:
                    return DescritoresExpressaoRentabilidade.FirstOrDefault(f => f.Nome == nome)?.IdExpressaoRentabilidade ?? -1;

                case TipoRegistroRentabilidade.IndicadorFinaceiro:
                    return DescritoresIndicadorFinanceiro.FirstOrDefault(f => f.Nome == nome)?.IdIndicadorFinanceiro ?? -1;

                case TipoRegistroRentabilidade.VariavelItem:
                    return ((byte?)DescritoresVariavelItem.FirstOrDefault(f => f.Nome == nome)?.TipoVariavel) ?? -1;
            }

            return -1;
        }

        #endregion

        #region Membros de IProvedorExpressaoRentabilidade

        /// <summary>
        /// Assinatura do provedor da expressões de rentabilidade.
        /// </summary>
        int Entidades.IProvedorExpressaoRentabilidade.ObterUltimaPosicao()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ExpressaoRentabilidade>()
                .Select("MAX(Posicao)")
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault();
        }

        #endregion

        #region Membros de IProvedorConfigRegistroRentabilidade

        /// <summary>
        /// Recupera o última posição das confgiurações do registro de rentabilidade.
        /// </summary>
        /// <returns></returns>
        int Entidades.IProvedorConfigRegistroRentabilidade.ObterUltimaPosicao()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ConfigRegistroRentabilidade>()
                .Select("MAX(Posicao)")
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault();
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação base do descritor do registor de rentabilidade com suporte a formatação.
        /// </summary>
        abstract class DescritorRegistroRentabilidadeFormatado : DescritorRegistroRentabilidade
        {
            #region Propriedades

            /// <summary>
            /// Formatação.
            /// </summary>
            public string Formatacao { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="nome"></param>
            /// <param name="descricao"></param>
            /// <param name="posicao"></param>
            /// <param name="exibirRelatorio"></param>
            /// <param name="formatacao"></param>
            protected DescritorRegistroRentabilidadeFormatado(
                string nome, string descricao, int posicao, bool exibirRelatorio, string formatacao)
                : base(nome, descricao, posicao, exibirRelatorio)
            {
                Formatacao = formatacao;
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Formata o valor do registro.
            /// </summary>
            /// <param name="registro"></param>
            /// <param name="cultura"></param>
            /// <returns></returns>
            public override string FormatarValor(IRegistroRentabilidade registro, CultureInfo cultura)
            {
                if (!string.IsNullOrEmpty(Formatacao))
                    return string.Format(cultura, Formatacao, registro.Valor);

                return base.FormatarValor(registro, cultura);
            }

            #endregion
        }

        /// <summary>
        /// Representa um descritor da expressão de rentabilidade.
        /// </summary>
        class DescritorExpressaoRentabilidade : DescritorRegistroRentabilidadeFormatado
        {
            #region Propriedades

            /// <summary>
            /// Identificador da expressão.
            /// </summary>
            public int IdExpressaoRentabilidade { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="idExpressaoRentabilidade"></param>
            /// <param name="nome"></param>
            /// <param name="descricao"></param>
            /// <param name="posicao"></param>
            /// <param name="exibirRelatorio"></param>
            /// <param name="formatacao"></param>
            public DescritorExpressaoRentabilidade(
                int idExpressaoRentabilidade, 
                string nome, string descricao, int posicao, bool exibirRelatorio, string formatacao)
                : base(nome, descricao, posicao, exibirRelatorio, formatacao)
            {
                IdExpressaoRentabilidade = idExpressaoRentabilidade;
            }

            #endregion
        }

        /// <summary>
        /// Descritor do indicador financeiro.
        /// </summary>
        class DescritorIndicadorFinanceiro : DescritorRegistroRentabilidadeFormatado
        {
            #region Propriedades

            /// <summary>
            /// Identificador do indicador financeiro.
            /// </summary>
            public int IdIndicadorFinanceiro { get; set; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="idIndicadorFinanceiro"></param>
            /// <param name="nome"></param>
            /// <param name="descricao"></param>
            /// <param name="posicao"></param>
            /// <param name="exibirRelatorio"></param>
            /// <param name="formatacao"></param>
            public DescritorIndicadorFinanceiro(
                int idIndicadorFinanceiro, 
                string nome, string descricao, int posicao, 
                bool exibirRelatorio, string formatacao)
                : base(nome, descricao, posicao, exibirRelatorio, formatacao)
            {
                IdIndicadorFinanceiro = idIndicadorFinanceiro;
            }

            #endregion
        }

        /// <summary>
        /// Descritor da variável do item da rentabilidade.
        /// </summary>
        class DescritorVariavelItem : DescritorRegistroRentabilidade
        {
            #region Propriedades

            /// <summary>
            /// Tipo de variável associada.
            /// </summary>
            public TipoVariavelItemRentabilidade TipoVariavel { get; }

            /// <summary>
            /// Formatação.
            /// </summary>
            public string Formatacao
            {
                get
                {
                    switch(TipoVariavel)
                    {
                        case TipoVariavelItemRentabilidade.PrecoVenda:
                        case TipoVariavelItemRentabilidade.PrecoCusto:
                        case TipoVariavelItemRentabilidade.CustosExtras:
                            return "{0:R$#,##0.00;\\(R$#,##0.00\\)}";
                        case TipoVariavelItemRentabilidade.PrazoMedio:
                            return "{0:#0}";
                        case TipoVariavelItemRentabilidade.PComissao:
                        case TipoVariavelItemRentabilidade.PICMSCompra:
                        case TipoVariavelItemRentabilidade.PICMSVenda:
                        case TipoVariavelItemRentabilidade.PIPICompra:
                        case TipoVariavelItemRentabilidade.PIPIVenda:
                        case TipoVariavelItemRentabilidade.FatorICMSSubstituicao:
                            return "{0:P}";
                        
                    }

                    return null;
                }
            }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="tipoVariavel"></param>
            /// <param name="nome"></param>
            /// <param name="descricao"></param>
            /// <param name="posicao"></param>
            /// <param name="exibirRelatorio"></param>
            public DescritorVariavelItem(
                TipoVariavelItemRentabilidade tipoVariavel, 
                string nome, string descricao, int posicao, bool exibirRelatorio)
                : base(nome, descricao, posicao, exibirRelatorio)
            {
                TipoVariavel = tipoVariavel;
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Formata o valor do registro.
            /// </summary>
            /// <param name="registro"></param>
            /// <param name="cultura"></param>
            /// <returns></returns>
            public override string FormatarValor(IRegistroRentabilidade registro, CultureInfo cultura)
            {
                if (!string.IsNullOrEmpty(Formatacao))
                    return string.Format(cultura, Formatacao, registro.Valor);

                return base.FormatarValor(registro, cultura);
            }

            #endregion
        }

        #endregion
    }
}
