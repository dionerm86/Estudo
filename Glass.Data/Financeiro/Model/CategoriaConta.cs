using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis tipos de caregoria da conta.
    /// </summary>
    [Colosoft.EmptyDescription("N/D")]
    public enum TipoCategoriaConta
    {
        [Description("Subtotal")]
        Subtotal = 0,
        [Description("Receita")]
        Receita = 1,
        [Description("Despesa Variável")]
        DespesaVariavel,
        [Description("Despesa Fixa")]
        DespesaFixa,
        [Description("Subtotal Agregado")]
        SubtotalAgregado
    }

    [PersistenceBaseDAO(typeof(CategoriaContaDAO))]
    [PersistenceClass("categoria_conta")]
    public class CategoriaConta : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCATEGORIACONTA", PersistenceParameterType.IdentityKey)]
        public int IdCategoriaConta { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        /// <summary>
        /// 0-Subtotal, 1-Crédito, 2-Débito, 3-Investimento
        /// </summary>
        [Log("Tipo")]
        [PersistenceProperty("TIPO")]
        public TipoCategoriaConta? Tipo { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumeroSequencia { get; set; }

        /// <summary>
        /// 1 - Ativo
        /// 2 - Inativo
        /// </summary>
        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        #endregion
    }
}