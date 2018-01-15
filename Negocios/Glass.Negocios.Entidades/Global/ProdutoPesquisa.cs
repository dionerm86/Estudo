using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório dos beneficiamentos dos produtos.
    /// </summary>
    public interface IProdutoBeneficiamentosRepositorio
    {
        /// <summary>
        /// Recupera as descriçõa dos beneficiamentos associados com o produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        IEnumerable<string> ObtemDescricoes(int idProd);
    }

    /// <summary>
    /// Armazena os dados da pesquisa dos produtos.
    /// </summary>
    public class ProdutoPesquisa
    {
        #region Variáveis Locais

        private string _descricaoBeneficiamentos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd { get; set; }

        /// <summary>
        /// Identificador do grupo de produtos.
        /// </summary>
        public int IdGrupoProd { get; set; }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int? IdSubgrupoProd { get; set; }

        /// <summary>
        /// Nome grupo associado.
        /// </summary>
        public string Grupo { get; set; }

        /// <summary>
        /// Nome do subgrupo.
        /// </summary>
        public string Subgrupo { get; set; }

        /// <summary>
        /// Tipo de calculo associado.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculo { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Quantidade de beneficiamentos associados.
        /// </summary>
        public int QtdeBeneficiamentos { get; set; }

        /// <summary>
        /// Descrição dos beneficiamentos
        /// </summary>
        public string DescricaoBeneficiamentos
        {
            get
            {
                if (IdProd <= 0 &&
                    !Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos &&
                    (int)Glass.Data.Model.NomeGrupoProd.Vidro != IdGrupoProd)
                    return null;
            
                if (QtdeBeneficiamentos > 0)
                {
                    if (_descricaoBeneficiamentos == null)
                    {
                        _descricaoBeneficiamentos = 
                            string.Join(" - ",
                                Microsoft.Practices.ServiceLocation.ServiceLocator
                                .Current.GetInstance<IProdutoBeneficiamentosRepositorio>()
                                .ObtemDescricoes(IdProd)
                                .ToArray());
                    }
                }

                return _descricaoBeneficiamentos;;
            }
        }

        /// <summary>
        /// Descrição do produto beneficiamento.
        /// </summary>
        public string DescricaoProdutoBeneficiamento
        {
            get
            {
                return !string.IsNullOrEmpty(Descricao) ?
                    (Descricao.ToUpper() + (!string.IsNullOrEmpty(DescricaoBeneficiamentos) ? " ( " + DescricaoBeneficiamentos + " )" : "")) : Descricao.ToUpper();
            }
        }

        /// <summary>
        /// Descrição do tipo de produto.
        /// </summary>
        public string TipoProduto { get; set; }

        /// <summary>
        /// Altura.
        /// </summary>
        public int? Altura { get; set; }

        /// <summary>
        /// Largura.
        /// </summary>
        public int? Largura { get; set; }

        /// <summary>
        /// Custom de fabricação base.
        /// </summary>
        public decimal Custofabbase { get; set; }

        /// <summary>
        /// Custo de compra.
        /// </summary>
        public decimal CustoCompra { get; set; }

        /// <summary>
        /// Valor do produto no atacado.
        /// </summary>
        public decimal ValorAtacado { get; set; }

        /// <summary>
        /// Valor do produto no balcão.
        /// </summary>
        public decimal ValorBalcao { get; set; }

        /// <summary>
        /// Valor obra.
        /// </summary>
        public decimal ValorObra { get; set; }

        /// <summary>
        /// Valor reposição.
        /// </summary>
        public decimal ValorReposicao { get; set; }

        /// <summary>
        /// Recupera o valor de atacado ou reposicao.
        /// </summary>
        public decimal ValorAtacadoRepos
        {
            get { return ValorAtacado; }
        }

        /// <summary>
        /// Valor mínimo.
        /// </summary>
        public decimal ValorMinimo { get; set; }

        /// <summary>
        /// Quantidade no estoque.
        /// </summary>
        public double QtdeEstoque { get; set; }

        /// <summary>
        /// Reserva.
        /// </summary>
        public double Reserva { get; set; }

        /// <summary>
        /// Liberação.
        /// </summary>
        public double Liberacao { get; set; }

        /// <summary>
        /// Quantidade disponível.
        /// </summary>
        public double Disponivel
        {
            get
            {
                return Math.Round(QtdeEstoque - Reserva - (Glass.Configuracoes.PedidoConfig.LiberarPedido ? Liberacao : 0), 2);
            }
        }

        /// <summary>
        /// Estoque.
        /// </summary>
        public string Estoque
        {
            get
            {
                return QtdeEstoque + Colosoft.Translator.Translate(TipoCalculo, true).Format();
            }
        }

        /// <summary>
        /// Estoque disponível.
        /// </summary>
        public string EstoqueDisponivel
        {
            get
            {
                return Disponivel + Colosoft.Translator.Translate(TipoCalculo, true).Format();
            }
        }

        /// <summary>
        /// Data de cadastro.
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Nome do usuário que cadastrou o produto.
        /// </summary>
        public string NomeUsuarioCad { get; set; }

        /// <summary>
        /// Data de alteração.
        /// </summary>
        public DateTime? DataAlt { get; set; }

        /// <summary>
        /// Nome do usuário que alterou.
        /// </summary>
        public string NomeUsuarioAlt { get; set; }

        #endregion
    }
}
