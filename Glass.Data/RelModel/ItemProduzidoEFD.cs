using GDA;
using Glass.Data.RelDAL;
using Sync.Fiscal.EFD.Entidade;
using System;
using System.Collections.Generic;

namespace Glass.Data.RelModel
{
    /// <summary>
    /// Classe criada para preencher os dados dos itens produzidos, para a geração do EFD.
    /// </summary>
    [PersistenceBaseDAO(typeof(ItemProduzidoEFDDAO))]
    public class ItemProduzidoEFD : IItemProduzido
    {
        #region Membros de IItemProduzido

        /// <summary>
        /// Obtém ou define data de início da produção.
        /// </summary>
        public DateTime InicioProducao { get; set; }

        /// <summary>
        /// Obtém ou define data final da produção.
        /// </summary>
        public DateTime? FinalProducao { get; set; }

        /// <summary>
        /// Obtém ou define o número da etiqueta.
        /// </summary>
        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [PersistenceProperty("IDPROD")]
        public int CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade produzida, em M2.
        /// </summary>
        public decimal QtdeProduzida { get; set; }

        /// <summary>
        /// Obtém ou define os insumos do item produzido.
        /// </summary>
        public IEnumerable<IInsumoConsumido> Insumos { get; set; }

        #endregion

        /// <summary>
        /// Obtém ou define o ID do produto de produção.
        /// </summary>
        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public int IdProdPedProducao { get; set; }

        /// <summary>
        /// Obtém ou define o ID do produto do pedido espelho.
        /// </summary>
        [PersistenceProperty("IDPRODPED")]
        public int IdProdPed { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do produto.
        /// </summary>
        [PersistenceProperty("QUANTIDADE")]
        public float Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o total de M2.
        /// </summary>
        [PersistenceProperty("TOTM2CALC")]
        public float TotM2Calc { get; set; }

        /// <summary>
        /// Obtém ou define o ID do setor.
        /// </summary>
        [PersistenceProperty("IDSETOR")]
        public int IdSetor { get; set; }

        /// <summary>
        /// Obtém ou define a data de leitura.
        /// </summary>
        [PersistenceProperty("DATALEITURA")]
        public DateTime DataLeitura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça foi marcada como pronta pelo roteiro.
        /// </summary>
        [PersistenceProperty("PRONTOROTEIRO")]
        public bool ProntoRoteiro { get; set; }

        /// <summary>
        /// Obtém ou define o ID do grupo do produto.
        /// </summary>
        [PersistenceProperty("IDGRUPOPROD")]
        public int IdGrupoProd { get; set; }

        /// <summary>
        /// Obtém ou define o ID do subgrupo do produto.
        /// </summary>
        [PersistenceProperty("IDSUBGRUPOPROD")]
        public int? IdSubgrupoProd { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo do grupo.
        /// </summary>
        [PersistenceProperty("TIPOCALCULOGRUPO")]
        public int? TipoCalculoGrupo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo fiscal do grupo.
        /// </summary>
        [PersistenceProperty("TIPOCALCULONFGRUPO")]
        public int? TipoCalculoNfGrupo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo do subgrupo.
        /// </summary>
        [PersistenceProperty("TIPOCALCULOSUBGRUPO")]
        public int? TipoCalculoSubgrupo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo fiscal do subgrupo.
        /// </summary>
        [PersistenceProperty("TIPOCALCULONFSUBGRUPO")]
        public int? TipoCalculoNfSubgrupo { get; set; }
    }
}
