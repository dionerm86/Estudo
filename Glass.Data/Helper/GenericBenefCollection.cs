using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Glass.Data.Helper
{
    [Serializable]
    public class GenericBenefCollection : IList<GenericBenef>
    {
        #region Constantes e campos somente-leitura públicos

        /// <summary>
        /// Coleção imutável e vazia de beneficiamentos.
        /// </summary>
        [NonSerialized]
        #pragma warning disable S2386 // Mutable fields should not be "public static"
        #pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
        // Warnings removidos porque o campo é imutável internamente
        public static readonly GenericBenefCollection Empty = new GenericBenefCollection()
        {
            lista = new ReadOnlyCollection<GenericBenef>(new List<GenericBenef>())
        };
        #pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"
        #pragma warning restore S2386 // Mutable fields should not be "public static"

        #endregion

        #region Campos Privados

        private IList<GenericBenef> lista = new List<GenericBenef>();
        private TipoProdutoBeneficiamento _tipo = TipoProdutoBeneficiamento.Nenhum;
        private int? countAreaMinima;
        private int? numeroBeneficiamentos;

        #endregion

        #region Métodos Estáticos

        /// <summary>
        /// Retorna a lista de tabelas de beneficiamento.
        /// </summary>
        /// <returns></returns>
        public static string[] GetTabelas()
        {
            List<string> retorno = new List<string>();

            foreach (int i in Enum.GetValues(typeof(TipoProdutoBeneficiamento)))
            {
                if (i == 0)
                    continue;

                retorno.Add(GenericBenef.GetTabela((TipoProdutoBeneficiamento)i));
            }

            return retorno.ToArray();
        }

        #endregion

        #region Construtores

        public GenericBenefCollection()
        {
        }

        public GenericBenefCollection(IEnumerable<ProdutoPedidoBenef> produtosPedido)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoPedido;
            foreach (ProdutoPedidoBenef p in produtosPedido)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutoOrcamentoBenef> produtosOrcamento)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoOrcamento;
            foreach (ProdutoOrcamentoBenef p in produtosOrcamento)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<MaterialProjetoBenef> materiaisProjeto)
        {
            _tipo = TipoProdutoBeneficiamento.MaterialProjeto;
            foreach (MaterialProjetoBenef m in materiaisProjeto)
                Add(new GenericBenef(m));
        }

        public GenericBenefCollection(IEnumerable<ProdutoPedidoEspelhoBenef> produtosPedidoEspelho)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoPedidoEspelho;
            foreach (ProdutoPedidoEspelhoBenef p in produtosPedidoEspelho)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutosCompraBenef> produtosCompra)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoCompra;
            foreach (ProdutosCompraBenef p in produtosCompra)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutoBenef> produtos)
        {
            _tipo = TipoProdutoBeneficiamento.Produto;
            foreach (ProdutoBenef p in produtos)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutoTrocaDevolucaoBenef> produtosTrocaDevolucao)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoTrocaDevolucao;
            foreach (ProdutoTrocaDevolucaoBenef p in produtosTrocaDevolucao)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<PecaModeloBenef> pecasProjetoModelo)
        {
            _tipo = TipoProdutoBeneficiamento.PecaModeloProjeto;
            foreach (PecaModeloBenef p in pecasProjetoModelo)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<PecaItemProjBenef> pecasItemProjeto)
        {
            _tipo = TipoProdutoBeneficiamento.PecaItemProjeto;
            foreach (PecaItemProjBenef p in pecasItemProjeto)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutoTrocadoBenef> produtosTrocado)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoTrocado;
            foreach (ProdutoTrocadoBenef p in produtosTrocado)
                Add(new GenericBenef(p));
        }

        public GenericBenefCollection(IEnumerable<ProdutoBaixaEstoqueBenef> produtosBaixaEst)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoBaixaEst;
            foreach (ProdutoBaixaEstoqueBenef p in produtosBaixaEst)
                Add(new GenericBenef(p));
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Retorna o número de beneficiamentos que cobram área mínima.
        /// </summary>
        public int CountAreaMinimaSession(GDA.GDASession sessao)
        {
            if (countAreaMinima == null)
            {
                string idsBenefConfig = string.Join(",", lista.Select(b => b.IdBenefConfig));
                countAreaMinima = BenefConfigDAO.Instance.CobrarAreaMinima(sessao, idsBenefConfig);
            }

            return countAreaMinima ?? 0;
        }

        /// <summary>
        /// Retorna o número de beneficiamentos que cobram área mínima.
        /// </summary>
        public int CountAreaMinima
        {
            get
            {
                return CountAreaMinimaSession(null);
            }
        }

        /// <summary>
        /// Retorna o número de beneficiamentos feitos para um produto.
        /// </summary>
        /// <param name="qtdeProduto"></param>
        /// <returns></returns>
        public int NumeroBeneficiamentos
        {
            get
            {
                if (Count == 0)
                    return 0;

                if (numeroBeneficiamentos == null)
                {
                    float qtdeProduto = 0;

                    switch (_tipo)
                    {
                        case TipoProdutoBeneficiamento.MaterialProjeto:
                            qtdeProduto = MaterialItemProjetoDAO.Instance.ObtemValorCampo<float>("qtde", "idMaterItemProj=" + this[0].IdMaterialItemProjeto);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoCompra:
                            qtdeProduto = ProdutosCompraDAO.Instance.ObtemValorCampo<float>("qtde", "idProdCompra=" + this[0].IdProdutoCompra);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoOrcamento:
                            qtdeProduto = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<float>("qtde", "idProd=" + this[0].IdProdutoOrcamento);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoPedido:
                            qtdeProduto = ProdutosPedidoDAO.Instance.ObtemQtde(this[0].IdProdutoPedido);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoPedidoEspelho:
                            qtdeProduto = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(this[0].IdProdutoPedidoEspelho);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoTrocaDevolucao:
                            qtdeProduto = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<float>("qtde", "idProdTrocaDev=" + this[0].IdProdutoTrocaDevolucao);
                            break;

                        case TipoProdutoBeneficiamento.PecaModeloProjeto:
                            qtdeProduto = PecaProjetoModeloDAO.Instance.ObtemValorCampo<float>("qtde", "idPecaProjMod=" + this[0].IdPecaProjetoModelo);
                            break;

                        case TipoProdutoBeneficiamento.ProdutoBaixaEst:
                            qtdeProduto = ProdutoBaixaEstoqueDAO.Instance.ObtemValorCampo<float>("qtde", "idProdBaixaEst=" + this[0].IdProdBaixaEst);
                            break;

                        default:
                            qtdeProduto = 1;
                            break;
                    }

                    numeroBeneficiamentos = (int)(qtdeProduto * Count);
                }

                return numeroBeneficiamentos ?? 0;
            }
        }

        /// <summary>
        /// Retorna uma string com a descrição dos beneficiamentos.
        /// </summary>
        public string DescricaoBeneficiamentos
        {
            get
            {
                if (Count == 0)
                    return "";

                string retorno = "";

                GenericBenef[] itens = this.ToArray();
                Array.Sort(itens, (x, y) => x.DescricaoBeneficiamento.CompareTo(y.DescricaoBeneficiamento));

                retorno = string.Join(", ", itens.Select(benef =>
                {
                    string textoQtd = benef.TipoCalculo == TipoCalculoBenef.Quantidade
                        ? benef.Qtd + " "
                        : string.Empty;

                    return textoQtd + benef.DescricaoBeneficiamento;
                }));

                return "(" + retorno + ")";
            }
        }

        internal bool GerarServicosInfo { get; set; }

        public string ServicosInfo
        {
            get
            {
                List<string> retorno = new List<string>(Count);

                for (int i = 0; i < Count; i++)
                {
                    if (this[i].IdBenefConfig == 0 && !GerarServicosInfo)
                        continue;

                    var idBenefConfig = GerarServicosInfo ? 0 : this[i].IdBenefConfig;

                    decimal percComissao = Math.Round(this[i].ValorComissao / (this[i].Valor > 0 ? this[i].Valor : 1), 2);

                    // [0]Id do beneficiamento [1]Qtd [2]Valor [3]Total [4]Custo [5]Perc. Comissão [6]Altura [7]Largura [8]Esp. Bisote
                    string info = idBenefConfig + ";" + this[i].Qtd + ";" + this[i].ValorUnit + ";" + this[i].Valor + ";" +
                        this[i].Custo + ";" + percComissao + ";" + (this[i].BisAlt + this[i].LapAlt) + ";" + (this[i].BisLarg + this[i].LapLarg) + ";" + this[i].EspBisote;

                    // [9]Descrição [10]Descrição Parent
                    info += ";" + (this[i].Descricao != null ? this[i].Descricao.Replace(";", "") : "");
                    info += ";" + (this[i].DescricaoParent != null ? this[i].DescricaoParent.Replace(";", "") : "");

                    retorno.Add(info);
                }

                return String.Join("|", retorno.ToArray());
            }
            set
            {
                Clear();
                AddBenefFromServicosInfo(value);
            }
        }

        #endregion

        #region Serviços Info

        public static bool IsLapidacao(uint idBenefConfig)
        {
            BenefConfig b = BenefConfigDAO.Instance.GetElement(idBenefConfig);
            var tipoControle = b.TipoControleParent > 0 ? b.TipoControleParent.Value : b.TipoControle;
            return tipoControle == TipoControleBenef.Lapidacao;
        }

        public static bool IsBisote(uint idBenefConfig)
        {
            BenefConfig b = BenefConfigDAO.Instance.GetElement(idBenefConfig);
            var tipoControle = b.TipoControleParent > 0 ? b.TipoControleParent.Value : b.TipoControle;
            return tipoControle == TipoControleBenef.Bisote;
        }

        /// <summary>
        /// Adiciona vários beneficiamentos de acordo com os serviços info.
        /// </summary>
        /// <param name="servicosInfo">A string com os serviços info do controle de beneficiamento.</param>
        public void AddBenefFromServicosInfo(string servicosInfo)
        {
            string[] servicoInfo = servicosInfo.TrimEnd('\r').TrimEnd('|').Split('|');

            // Para cada serviço executado no vidro
            foreach (string servico in servicoInfo)
            {
                if (String.IsNullOrEmpty(servico))
                    continue;

                AddBenefFromServicoInfo(servico);
            }
        }

        /// <summary>
        /// Adiciona um beneficiamento de acordo de um serviço info.
        /// </summary>
        /// <param name="servicosInfo">Um serviço info do controle de beneficiamento.</param>
        public void AddBenefFromServicoInfo(string servicoInfo)
        {
            // [0]Id do beneficiamento [1]Qtd [2]Valor [3]Total [4]Custo [5]Perc. Comissão [6]Altura [7]Largura [8]Esp. Bisote [9]Descrição [10]Descrição Parent
            string[] dadosServ = servicoInfo.Split(';');

            // Caso o vetor tenha 10 posições, inclui a posição 5, porque neste caso o vetor terá vindo de um pedido exportado 
            // de um sistema com a versão lite sendo que lá não vem o [5]Perc. Comissão
            if (dadosServ.Length == 10)
            {
                List<string> lst = new List<string>(dadosServ);
                lst.Insert(5, "0");
                dadosServ = lst.ToArray();
            }

            uint? idBenefConfig = Glass.Conversoes.StrParaUint(dadosServ[0]) > 0 ? Glass.Conversoes.StrParaUint(dadosServ[0]) :
                BenefConfigDAO.Instance.GetIdByDescricao(dadosServ[9], dadosServ[10]);

            if (idBenefConfig == null)
                throw new Exception("Beneficiamento '" + (!String.IsNullOrEmpty(dadosServ[10]) ? dadosServ[10] + " " : "") + dadosServ[9] + "' não encontrado.");

            GenericBenef benef = new GenericBenef();
            benef.IdBenefConfig = idBenefConfig.Value;
            benef.Qtd = Glass.Conversoes.StrParaInt(dadosServ[1]);
            benef.ValorUnit = Glass.Conversoes.StrParaDecimal(dadosServ[2]);
            benef.Valor = Glass.Conversoes.StrParaDecimal(dadosServ[3]);
            benef.Custo = Glass.Conversoes.StrParaDecimal(dadosServ[4]);
            benef.ValorComissao = Math.Round(benef.Valor * Glass.Conversoes.StrParaDecimal(dadosServ[5]) / 100, 2);

            if (IsLapidacao(benef.IdBenefConfig))
            {
                benef.LapAlt = Glass.Conversoes.StrParaInt(dadosServ[6]);
                benef.LapLarg = Glass.Conversoes.StrParaInt(dadosServ[7]);
            }
            else if (IsBisote(benef.IdBenefConfig))
            {
                benef.BisAlt = Glass.Conversoes.StrParaInt(dadosServ[6]);
                benef.BisLarg = Glass.Conversoes.StrParaInt(dadosServ[7]);
                benef.EspBisote = !String.IsNullOrEmpty(dadosServ[8]) ? Glass.Conversoes.StrParaFloat(dadosServ[8]) : 0;
            }

            Add(benef);
        }

        #endregion

        #region Conversão para vetores

        public ProdutoPedidoBenef[] ToProdutosPedido()
        {
            return ToProdutosPedido(0);
        }

        public ProdutoPedidoBenef[] ToProdutosPedido(uint idProdPed)
        {
            ProdutoPedidoBenef[] retorno = new ProdutoPedidoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoPedido(idProdPed);

            return retorno;
        }

        public ProdutoOrcamentoBenef[] ToProdutosOrcamento()
        {
            return ToProdutosOrcamento(0);
        }

        public ProdutoOrcamentoBenef[] ToProdutosOrcamento(uint idProdOrca)
        {
            ProdutoOrcamentoBenef[] retorno = new ProdutoOrcamentoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoOrcamento(idProdOrca);

            return retorno;
        }

        public MaterialProjetoBenef[] ToMateriaisProjeto()
        {
            return ToMateriaisProjeto(0);
        }

        public MaterialProjetoBenef[] ToMateriaisProjeto(uint idMaterialProj)
        {
            MaterialProjetoBenef[] retorno = new MaterialProjetoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToMaterialProjeto(idMaterialProj);

            return retorno;
        }

        public ProdutoPedidoEspelhoBenef[] ToProdutosPedidoEspelho()
        {
            return ToProdutosPedidoEspelho(0);
        }

        public ProdutoPedidoEspelhoBenef[] ToProdutosPedidoEspelho(uint idProdPedEsp)
        {
            ProdutoPedidoEspelhoBenef[] retorno = new ProdutoPedidoEspelhoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoPedidoEspelho(idProdPedEsp);

            return retorno;
        }

        public ProdutosCompraBenef[] ToProdutosCompra()
        {
            return ToProdutosCompra(0);
        }

        public ProdutosCompraBenef[] ToProdutosCompra(uint idProdCompra)
        {
            ProdutosCompraBenef[] retorno = new ProdutosCompraBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoCompra(idProdCompra);

            return retorno;
        }

        public ProdutoBenef[] ToProdutos()
        {
            return ToProdutos(0);
        }

        public ProdutoBenef[] ToProdutos(uint idProd)
        {
            ProdutoBenef[] retorno = new ProdutoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProduto(idProd);

            return retorno;
        }

        public ProdutoTrocaDevolucaoBenef[] ToProdutosTrocaDevolucao()
        {
            return ToProdutosTrocaDevolucao(0);
        }

        public ProdutoTrocaDevolucaoBenef[] ToProdutosTrocaDevolucao(uint idProdTrocaDev)
        {
            ProdutoTrocaDevolucaoBenef[] retorno = new ProdutoTrocaDevolucaoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoTrocaDevolucao(idProdTrocaDev);

            return retorno;
        }

        public PecaModeloBenef[] ToPecasProjetoModelo()
        {
            return ToPecasProjetoModelo(0);
        }

        public PecaModeloBenef[] ToPecasProjetoModelo(uint idPecaProjMod)
        {
            PecaModeloBenef[] retorno = new PecaModeloBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToPecaProjetoModelo(idPecaProjMod);

            return retorno;
        }

        public PecaItemProjBenef[] ToPecasItemProjeto()
        {
            return ToPecasItemProjeto(0);
        }

        public PecaItemProjBenef[] ToPecasItemProjeto(uint idPecaItemProj)
        {
            PecaItemProjBenef[] retorno = new PecaItemProjBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToPecaItemProjeto(idPecaItemProj);

            return retorno;
        }

        public ProdutoTrocadoBenef[] ToProdutosTrocado()
        {
            return ToProdutosTrocado(0);
        }

        public ProdutoTrocadoBenef[] ToProdutosTrocado(uint idProdTrocado)
        {
            ProdutoTrocadoBenef[] retorno = new ProdutoTrocadoBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoTrocado(idProdTrocado);

            return retorno;
        }

        public ProdutoBaixaEstoqueBenef[] ToProdutoBaixaEst()
        {
            return ToProdutoBaixaEst(0);
        }

        public ProdutoBaixaEstoqueBenef[] ToProdutoBaixaEst(uint idProdBaixaEst)
        {
            ProdutoBaixaEstoqueBenef[] retorno = new ProdutoBaixaEstoqueBenef[Count];
            int i = 0;
            foreach (GenericBenef b in this)
                retorno[i++] = b.ToProdutoBaixaEst(idProdBaixaEst);

            return retorno;
        }

        #endregion

        #region Cast Implícito

        #region Entrada

        public static implicit operator GenericBenefCollection(ProdutoPedidoBenef[] produtosPedido)
        {
            return new GenericBenefCollection(produtosPedido);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoPedidoBenef> produtosPedido)
        {
            return new GenericBenefCollection(produtosPedido);
        }

        public static implicit operator GenericBenefCollection(ProdutoOrcamentoBenef[] produtosOrcamento)
        {
            return new GenericBenefCollection(produtosOrcamento);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoOrcamentoBenef> produtosOrcamento)
        {
            return new GenericBenefCollection(produtosOrcamento);
        }

        public static implicit operator GenericBenefCollection(MaterialProjetoBenef[] materiaisProjeto)
        {
            return new GenericBenefCollection(materiaisProjeto);
        }

        public static implicit operator GenericBenefCollection(List<MaterialProjetoBenef> materiaisProjeto)
        {
            return new GenericBenefCollection(materiaisProjeto);
        }

        public static implicit operator GenericBenefCollection(ProdutoPedidoEspelhoBenef[] produtosPedidoEspelho)
        {
            return new GenericBenefCollection(produtosPedidoEspelho);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoPedidoEspelhoBenef> produtosPedidoEspelho)
        {
            return new GenericBenefCollection(produtosPedidoEspelho);
        }

        public static implicit operator GenericBenefCollection(ProdutosCompraBenef[] produtosCompra)
        {
            return new GenericBenefCollection(produtosCompra);
        }

        public static implicit operator GenericBenefCollection(List<ProdutosCompraBenef> produtosCompra)
        {
            return new GenericBenefCollection(produtosCompra);
        }

        public static implicit operator GenericBenefCollection(ProdutoBenef[] produtos)
        {
            return new GenericBenefCollection(produtos);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoBenef> produtos)
        {
            return new GenericBenefCollection(produtos);
        }

        public static implicit operator GenericBenefCollection(ProdutoTrocaDevolucaoBenef[] produtosTrocaDevolucao)
        {
            return new GenericBenefCollection(produtosTrocaDevolucao);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoTrocaDevolucaoBenef> produtosTrocaDevolucao)
        {
            return new GenericBenefCollection(produtosTrocaDevolucao);
        }

        public static implicit operator GenericBenefCollection(PecaModeloBenef[] pecasProjetoModelo)
        {
            return new GenericBenefCollection(pecasProjetoModelo);
        }

        public static implicit operator GenericBenefCollection(List<PecaModeloBenef> pecasProjetoModelo)
        {
            return new GenericBenefCollection(pecasProjetoModelo);
        }

        public static implicit operator GenericBenefCollection(PecaItemProjBenef[] pecasItemProjeto)
        {
            return new GenericBenefCollection(pecasItemProjeto);
        }

        public static implicit operator GenericBenefCollection(List<PecaItemProjBenef> pecasItemProjeto)
        {
            return new GenericBenefCollection(pecasItemProjeto);
        }

        public static implicit operator GenericBenefCollection(ProdutoTrocadoBenef[] produtosTrocado)
        {
            return new GenericBenefCollection(produtosTrocado);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoTrocadoBenef> produtosTrocado)
        {
            return new GenericBenefCollection(produtosTrocado);
        }

        public static implicit operator GenericBenefCollection(ProdutoBaixaEstoqueBenef[] produtosBaixaEstBenef)
        {
            return new GenericBenefCollection(produtosBaixaEstBenef);
        }

        public static implicit operator GenericBenefCollection(List<ProdutoBaixaEstoqueBenef> produtosBaixaEstBenef)
        {
            return new GenericBenefCollection(produtosBaixaEstBenef);
        }

        #endregion

        #region Saída

        public static implicit operator ProdutoPedidoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosPedido();
        }

        public static implicit operator List<ProdutoPedidoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoPedidoBenef>(colecao.ToProdutosPedido());
        }

        public static implicit operator ProdutoOrcamentoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosOrcamento();
        }

        public static implicit operator List<ProdutoOrcamentoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoOrcamentoBenef>(colecao.ToProdutosOrcamento());
        }

        public static implicit operator MaterialProjetoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToMateriaisProjeto();
        }

        public static implicit operator List<MaterialProjetoBenef>(GenericBenefCollection colecao)
        {
            return new List<MaterialProjetoBenef>(colecao.ToMateriaisProjeto());
        }

        public static implicit operator ProdutoPedidoEspelhoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosPedidoEspelho();
        }

        public static implicit operator List<ProdutoPedidoEspelhoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoPedidoEspelhoBenef>(colecao.ToProdutosPedidoEspelho());
        }

        public static implicit operator ProdutosCompraBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosCompra();
        }

        public static implicit operator List<ProdutosCompraBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutosCompraBenef>(colecao.ToProdutosCompra());
        }

        public static implicit operator ProdutoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutos();
        }

        public static implicit operator List<ProdutoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoBenef>(colecao.ToProdutos());
        }

        public static implicit operator ProdutoTrocaDevolucaoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosTrocaDevolucao();
        }

        public static implicit operator List<ProdutoTrocaDevolucaoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoTrocaDevolucaoBenef>(colecao.ToProdutosTrocaDevolucao());
        }

        public static implicit operator PecaModeloBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToPecasProjetoModelo();
        }

        public static implicit operator List<PecaModeloBenef>(GenericBenefCollection colecao)
        {
            return new List<PecaModeloBenef>(colecao.ToPecasProjetoModelo());
        }

        public static implicit operator PecaItemProjBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToPecasItemProjeto();
        }

        public static implicit operator List<PecaItemProjBenef>(GenericBenefCollection colecao)
        {
            return new List<PecaItemProjBenef>(colecao.ToPecasItemProjeto());
        }

        public static implicit operator ProdutoTrocadoBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutosTrocado();
        }

        public static implicit operator List<ProdutoTrocadoBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoTrocadoBenef>(colecao.ToProdutosTrocado());
        }

        public static implicit operator ProdutoBaixaEstoqueBenef[] (GenericBenefCollection colecao)
        {
            return colecao.ToProdutoBaixaEst();
        }

        public static implicit operator List<ProdutoBaixaEstoqueBenef>(GenericBenefCollection colecao)
        {
            return new List<ProdutoBaixaEstoqueBenef>(colecao.ToProdutoBaixaEst());
        }

        #endregion

        #endregion

        #region IList

        public GenericBenef this[int index]
        {
            get
            {
                return lista[index];
            }

            set
            {
                lista[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return lista.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return lista.IsReadOnly;
            }
        }

        public void Add(GenericBenef item)
        {
            lista.Add(item);
        }

        public void Clear()
        {
            lista.Clear();
        }

        public bool Contains(GenericBenef item)
        {
            return lista.Contains(item);
        }

        public void CopyTo(GenericBenef[] array, int arrayIndex)
        {
            lista.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GenericBenef> GetEnumerator()
        {
            return lista.GetEnumerator();
        }

        public int IndexOf(GenericBenef item)
        {
            return lista.IndexOf(item);
        }

        public void Insert(int index, GenericBenef item)
        {
            lista.Insert(index, item);
        }

        public bool Remove(GenericBenef item)
        {
            return lista.Remove(item);
        }

        public void RemoveAt(int index)
        {
            lista.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lista.GetEnumerator();
        }

        #endregion
    }
}