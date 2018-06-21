using GDA;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa a relação da faixa do percentual da rentabilidade 
    /// com o percentual de redução de comissão aplicado.
    /// </summary>
    [PersistenceClass("faixa_rentabilidade_comissao")]
    public class FaixaRentabilidadeComissao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador da relação.
        /// </summary>
        [PersistenceProperty("IDFAIXARENTABILIDADECOMISSAO", PersistenceParameterType.IdentityKey)]
        public int IdFaixaRentabilidadeComissao { get; set; }

        /// <summary>
        /// Identificador da loja no qual a faixa se aplica.
        /// </summary>
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        [Log("Loja", "NomeFantasia", typeof(DAL.LojaDAO))]
        public int IdLoja { get; set; }

        /// <summary>
        /// Identificador do funcionário ao qual se aplica a faixa.
        /// </summary>
        [PersistenceProperty("IdFunc")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        [Log("Funcionário", "Nome", typeof(DAL.FuncionarioDAO))]
        public int? IdFunc { get; set; }

        /// <summary>
        /// Faixa do percentual da rentabilidade. 
        /// </summary>
        [PersistenceProperty("PercentualRentabilidade")]
        [Log("Percentual Rentabilidade")]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Percentual da comissão.
        /// </summary>
        [PersistenceProperty("PercentualComissao")]
        [Log("Percentual Comissão")]
        public decimal PercentualComissao { get; set; }

        #endregion
    }
}
