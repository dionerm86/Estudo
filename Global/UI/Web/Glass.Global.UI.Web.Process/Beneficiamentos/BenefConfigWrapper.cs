using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Global.UI.Web.Process.Beneficiamentos
{
    /// <summary>
    /// Adaptação da configuração do beneficiamento.
    /// </summary>
    public class BenefConfigWrapper
    {
        #region Tipos Aninhados

        /// <summary>
        /// Armazena as informações do item
        /// </summary>
        class ItemInfo
        {
            #region Propriedades

            /// <summary>
            /// Identificador do processo.
            /// </summary>
            public int? IdProcesso { get; set; }

            /// <summary>
            /// Código interno do processo.
            /// </summary>
            public string CodProcesso { get; set; }

            /// <summary>
            /// Identificador da aplicação.
            /// </summary>
            public int? IdAplicacao { get; set; }

            /// <summary>
            /// Código interno ada aplicação.
            /// </summary>
            public string CodAplicacao { get; set; }

            /// <summary>
            /// Identificador do produto.
            /// </summary>
            public int? IdProd { get; set; }

            /// <summary>
            /// Código interno do produto.
            /// </summary>
            public string CodProduto { get; set; }

            /// <summary>
            /// Descrição do produto.
            /// </summary>
            public string Produto { get; set; }

            /// <summary>
            /// Acréscimo de altura.
            /// </summary>
            public int AcrescimoAltura { get; set; }

            /// <summary>
            /// Acréscimo de largura.
            /// </summary>
            public int AcrescimoLargura { get; set; }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Recupera o parser dos dados na linha informada.
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>
            public static ItemInfo Parse(string line)
            {
                var parts = line.Split(';');

                // Verifica se as partes são válidas
                if (parts.Length != 7)
                    return null;

                var item = new ItemInfo();
                int id = 0;

                if (!string.IsNullOrEmpty(parts[0]) &&
                    int.TryParse(parts[0], out id))
                    item.IdProcesso = id;

                if (!string.IsNullOrEmpty(parts[1]) &&
                    int.TryParse(parts[1], out id))
                    item.IdAplicacao = id;

                if (!string.IsNullOrEmpty(parts[2]) &&
                    int.TryParse(parts[2], out id))
                    item.IdProd = id;
                
                if (!string.IsNullOrEmpty(parts[5]) &&
                    int.TryParse(parts[5], out id))
                    item.AcrescimoAltura = id;

                if (!string.IsNullOrEmpty(parts[6]) &&
                    int.TryParse(parts[6], out id))
                    item.AcrescimoLargura = id;

                return item;
            }

            #endregion
        }

        #endregion

        #region Variáveis Locais

        /// <summary>
        /// Espessuras padrão dos vidros.
        /// </summary>
        private static readonly float[] Espessuras = new float[] { 3, 4, 5, 6, 8, 10, 12, 15, 19 };
        private Global.Negocios.Entidades.BenefConfig _benefConfig;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da configuração do beneficiamento.
        /// </summary>
        public int IdBenefConfig
        {
            get { return _benefConfig.IdBenefConfig; }
            set { _benefConfig.IdBenefConfig = value; }
        }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome
        {
            get { return _benefConfig.Nome; }
            set { _benefConfig.Nome = value; }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return _benefConfig.Descricao; }
            set { _benefConfig.Descricao = value; }
        }

        /// <summary>
        /// Tipo de controle.
        /// </summary>
        public Glass.Data.Model.TipoControleBenef TipoControle
        {
            get { return _benefConfig.TipoControle; }
            set { _benefConfig.TipoControle = value; }
        }

        /// <summary>
        /// Tipo de cálculo.
        /// </summary>
        public Glass.Data.Model.TipoCalculoBenef TipoCalculo
        {
            get { return _benefConfig.TipoCalculo; }
            set { _benefConfig.TipoCalculo = value; }
        }

        /// <summary>
        /// Tipo de espessura.
        /// </summary>
        public Glass.Data.Model.TipoEspessuraBenef TipoEspessura
        {
            get { return _benefConfig.TipoEspessura; }
            set { _benefConfig.TipoEspessura = value; }
        }

        /// <summary>
        /// Cobrança opcional.
        /// </summary>
        public bool CobrancaOpcional
        {
            get { return _benefConfig.CobrancaOpcional; }
            set { _benefConfig.CobrancaOpcional = value; }
        }
        /// <summary>
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return _benefConfig.Situacao; }
            set { _benefConfig.Situacao = value; }
        }
        
        /// <summary>
        /// Cobrar área mínima.
        /// </summary>
        public bool CobrarAreaMinima
        {
            get { return _benefConfig.CobrarAreaMinima; }
            set { _benefConfig.CobrarAreaMinima = value; }
        }

        /// <summary>
        /// Identificador da aplicação associada.
        /// </summary>
        public int? IdAplicacao
        {
            get { return _benefConfig.IdAplicacao; }
            set { _benefConfig.IdAplicacao = value; }
        }

        /// <summary>
        /// Identificador da etiqueta de processo associada.
        /// </summary>
        public int? IdProcesso
        {
            get { return _benefConfig.IdProcesso; }
            set { _benefConfig.IdProcesso = value; }
        }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public int? IdProd
        {
            get { return _benefConfig.IdProd; }
            set { _benefConfig.IdProd = value; }
        }

        /// <summary>
        /// Produto associado.
        /// </summary>
        public Global.Negocios.Entidades.Produto Produto
        {
            get { return _benefConfig.Produto; }
        }

        /// <summary>
        /// Acréscimo altura.
        /// </summary>
        public int AcrescimoAltura
        {
            get { return _benefConfig.AcrescimoAltura; }
            set { _benefConfig.AcrescimoAltura = value; }
        }

        /// <summary>
        /// Acréscimo de largura.
        /// </summary>
        public int AcrescimoLargura
        {
            get { return _benefConfig.AcrescimoLargura; }
            set { _benefConfig.AcrescimoLargura = value; }
        }

        /// <summary>
        /// Identifica se não é para exibir descrição do beneficiamento na etiqueta.
        /// </summary>
        public bool NaoExibirEtiqueta
        {
            get { return _benefConfig.NaoExibirEtiqueta; }
            set { _benefConfig.NaoExibirEtiqueta = value; }
        }

        /// <summary>
        /// Tipo do beneficiamento.
        /// </summary>
        public Glass.Data.Model.TipoBenef TipoBenef
        {
            get { return _benefConfig.TipoBenef; }
            set { _benefConfig.TipoBenef = value; }
        }

        /// <summary>
        /// Indica se o beneficiamento é de preenchimento obrigatório.
        /// </summary>
        public string IdsSubGrupoPreenchimentoObrigatorio
        {
            get { return _benefConfig.IdsSubGrupoPreenchimentoObrigatorio; }
            set { _benefConfig.IdsSubGrupoPreenchimentoObrigatorio = value; }
        }

        /// <summary>
        /// Identifica se é para cobrar por espessura.
        /// </summary>
        public bool CobrarPorEspessura { get; set; }

        /// <summary>
        /// Identifica se é para cobrar por cor.
        /// </summary>
        public bool CobrarPorCor { get; set; }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int? IdSubgrupoProd { get; set; }

        /// <summary>
        /// Lista de seleção.
        /// </summary>
        public string ListaSelecao { get; set; }

        /// <summary>
        /// Lista dos itens.
        /// </summary>
        public string ListaItens { get; set; }

        /// <summary>
        /// Filhos do beneficiamento.
        /// </summary>
        public IList<Global.Negocios.Entidades.BenefConfig> Filhos
        {
            get { return _benefConfig.Filhos; }
        }

        /// <summary>
        /// Código interno da etiqueta de aplicação.
        /// </summary>
        public string CodAplicacao
        {
            get
            {
                if (_benefConfig.IdAplicacao.HasValue && _benefConfig.Aplicacao != null)
                    return _benefConfig.Aplicacao.CodInterno;

                return null;
            }
        }

        /// <summary>
        /// Código interno da etiqueta de processo.
        /// </summary>
        public string CodProcesso
        {
            get
            {
                if (_benefConfig.IdProcesso.HasValue && _benefConfig.Processo != null)
                    return _benefConfig.Processo.CodInterno;

                return null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public BenefConfigWrapper()
        {
            _benefConfig = new Negocios.Entidades.BenefConfig();
        }

        /// <summary>
        /// Cria a instancia com a entidade da configuração do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        public BenefConfigWrapper(Global.Negocios.Entidades.BenefConfig benefConfig)
        {
            _benefConfig = benefConfig;

            // Verifica se existe algum preço que identifica
            // se é para cobrar por espessura
            CobrarPorEspessura = NavegarPrecos(_benefConfig).Any(f => f.Espessura.HasValue);
            // Verifica se existe algum preço que identifica
            // se é para cobrar por cor
            CobrarPorCor = NavegarPrecos(_benefConfig).Any(f => f.IdCorVidro.HasValue);

            // Recupera o identificador do subgrupo de produto
            // associado com os preços
            IdSubgrupoProd = NavegarPrecos(_benefConfig).Where(f => f.IdSubgrupoProd.HasValue)
                .Select(f => f.IdSubgrupoProd).FirstOrDefault();
        }

        #endregion

        #region Conversões Explicitas

        /// <summary>
        /// Conversão implicita para o adaptador.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        public static implicit operator BenefConfigWrapper(Global.Negocios.Entidades.BenefConfig benefConfig)
        {
            if (benefConfig == null)
                return null;

            return new BenefConfigWrapper(benefConfig);
        }

        /// <summary>
        /// Conversão implicita para BenefConfig.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static implicit operator Global.Negocios.Entidades.BenefConfig(BenefConfigWrapper wrapper)
        {
            if (wrapper == null)
                return null;

            return wrapper._benefConfig;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Navega pelos preços.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        private static IEnumerable<Negocios.Entidades.BenefConfigPreco> NavegarPrecos
            (Negocios.Entidades.BenefConfig benefConfig)
        {
            foreach (var i in benefConfig.Precos)
                yield return i;

            foreach (var i in benefConfig.Filhos)
                foreach (var j in NavegarPrecos(i))
                    yield return j;
        }

        /// <summary>
        /// Executa o processamento das opções.
        /// </summary>
        private void ProcessarOpcoes()
        {
            var opcoes = new List<string>();
            var itens = new List<ItemInfo>();

            if (!string.IsNullOrEmpty(ListaSelecao))
                opcoes.AddRange(ListaSelecao.TrimEnd('|').Split('|'));

            if (!string.IsNullOrEmpty(ListaItens))
                itens.AddRange(ListaItens.TrimEnd('|').Split('|').Select(f => ItemInfo.Parse(f)).Where(f => f != null));


            // Se o controle possuir lista de opções
            if (TipoControle != Glass.Data.Model.TipoControleBenef.SelecaoSimples &&
                TipoControle != Glass.Data.Model.TipoControleBenef.Quantidade)
            {
                var atualizados = new List<Glass.Global.Negocios.Entidades.BenefConfig>();

                // Insere a lista de opções
                for (var i = 0; i < opcoes.Count; i++)
                {
                    // Tenta recupera o beneficiamento existente
                    var benef = Filhos.FirstOrDefault(f =>
                        StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, opcoes[i]));

                    if (benef == null)
                    {
                        benef = new Negocios.Entidades.BenefConfig();
                        benef.IdParent = IdBenefConfig;
                        // Recupera o nome da opção
                        benef.Nome = opcoes[i];

                        Filhos.Add(benef);
                    }

                    benef.Descricao = benef.Nome;
                    benef.CobrarAreaMinima = CobrarAreaMinima;

                    benef.TipoEspessura = CobrarPorEspessura ?
                        Glass.Data.Model.TipoEspessuraBenef.ItemPossui :
                        Glass.Data.Model.TipoEspessuraBenef.ItemNaoPossui;

                    benef.Situacao = Glass.Situacao.Ativo;
                    benef.TipoCalculo = TipoCalculo;

                    var item = itens[i];

                    benef.IdProcesso = item.IdProcesso;
                    benef.IdAplicacao = item.IdAplicacao;
                    benef.IdProd = item.IdProd;
                    benef.AcrescimoAltura = item.AcrescimoAltura;
                    benef.AcrescimoLargura = item.AcrescimoLargura;

                    atualizados.Add(benef);
                }

                // Recupera os filhos que devem ser apagados
                foreach (var i in Filhos.Where(f => !atualizados.Exists(x => f.Equals(x))).ToArray())
                    Filhos.Remove(i);
            }
        }

        /// <summary>
        /// Executa o processamento dos preços.
        /// </summary>
        private void ProcessarPrecos()
        {
            var vetEsp = CobrarPorEspessura ? Espessuras : new float[] { 0 };
            var vetSubgrupo = IdSubgrupoProd != null ? new[] { null, IdSubgrupoProd } : new int?[] { null };

            var vetCor = new int?[] { null };

            if (CobrarPorCor)
            {
                var coresFluxos = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ICoresFluxo>();
                // Recupera as cores dos vidros
                vetCor = coresFluxos.ObtemCoresVidro().Select(f => (int?)f.Id).Union(vetCor).ToArray();
            }

            // Se não houver lista de opções, cria um item com o ID do beneficiamento para inserir
            var opcoes = Filhos.Count > 0 ? Filhos :
                new Negocios.Entidades.BenefConfig[] {
                   _benefConfig
                };

            if (Filhos.Count > 0)
                // Limpa os preços da configuração principal
                _benefConfig.Precos.Clear();

            /* Chamado 15928.
             * O beneficiamento que não possui filho não estava sendo salvo com a opção de calcular por espessura,
             * pois, a variável estava sendo setada somente no método que organiza os filhos. Por isso, colocamos esta verificação abaixo,
             * para que os beneficiamentos que não possuem filhos sejam marcados com a opção de calcular espessura, normalmente. */
            _benefConfig.TipoEspessura = CobrarPorEspessura && _benefConfig.TipoEspessura == Data.Model.TipoEspessuraBenef.ItemNaoPossui ?
                Data.Model.TipoEspessuraBenef.ItemPossui : _benefConfig.TipoEspessura;

            // Para cada beneficiamento da lista de opções
            foreach (var bc in opcoes)
            {
                var atualizados = new List<Negocios.Entidades.BenefConfigPreco>();

                // Para cada espessura gera um novo registro deste beneficiamento
                foreach (var espessura in vetEsp)
                {
                    // Para cada subgrupo gera um novo registro deste beneficiamento
                    foreach (var idSubgrupoProd in vetSubgrupo)
                    {
                        // Para cada cor gera um novo registro deste beneficiamento
                        foreach (var idCorVidro in vetCor)
                        {
                            // Tenta recuperar o preço do beneficiamento associado
                            // as parametros informados
                            var preco = bc.Precos.FirstOrDefault
                                (f => (CobrarPorEspessura ? f.Espessura == espessura : true) &&
                                      f.IdSubgrupoProd == idSubgrupoProd &&
                                      f.IdCorVidro == idCorVidro);

                            if (preco == null)
                            {
                                preco = new Negocios.Entidades.BenefConfigPreco();
                                preco.IdBenefConfig = bc.IdBenefConfig;

                                if (CobrarPorEspessura)
                                    preco.Espessura = espessura;

                                preco.IdCorVidro = idCorVidro;
                                preco.IdSubgrupoProd = idSubgrupoProd;

                                bc.Precos.Add(preco);
                            }

                            atualizados.Add(preco);
                        }
                    }
                }

                // Recupera os preços que devem ser apagados
                foreach (var i in bc.Precos.Where(f => !atualizados.Exists(x => f.Equals(x))).ToArray())
                    bc.Precos.Remove(i);
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Gera o scripta para iniciar os itens.
        /// </summary>
        /// <param name="benefConfigs"></param>
        /// <returns></returns>
        public string GerarScriptIniciarItens()
        {
            var script = new System.Text.StringBuilder();

            foreach (var i in Filhos)
            {
                // Monta a scripta para iniciar os itens
                script.AppendLine(string.Format("iniciarItens('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
                    i.IdProcesso,
                    i.IdProcesso.HasValue ? i.Processo.CodInterno : null,
                    i.IdAplicacao,
                    i.IdAplicacao.HasValue ? i.Aplicacao.CodInterno : null,
                    i.IdProd,
                    i.Produto != null ? i.Produto.CodInterno : null,
                    i.Produto != null ? i.Produto.Descricao : null,
                    i.AcrescimoAltura > 0 ? i.AcrescimoAltura.ToString() : null,
                    i.AcrescimoLargura > 0 ? i.AcrescimoLargura.ToString() : null));
            }

            return script.ToString();
        }

        /// <summary>
        /// Fixa os dados da configuração do beneficiamento.
        /// </summary>
        public void Fixar()
        {
            ProcessarOpcoes();
            ProcessarPrecos();
        }

        #endregion
    }
}
