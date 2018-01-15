using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OrcamentoSiteDAO))]
    [PersistenceClass("orcamento_site")]
    public class OrcamentoSite
    {
        #region Propriedades

        [PersistenceProperty("CodOrcamento", PersistenceParameterType.IdentityKey)]
        public uint CodOrcamento { get; set; }

        [PersistenceProperty("Nome")]
        public string Nome { get; set; }

        [PersistenceProperty("Endereco")]
        public string Endereco { get; set; }

        [PersistenceProperty("Telefone")]
        public string Telefone { get; set; }

        [PersistenceProperty("Email")]
        public string Email { get; set; }

        [PersistenceProperty("DescrProduto")]
        public string DescrProduto { get; set; }

        [PersistenceProperty("Medidas")]
        public string Medidas { get; set; }

        [PersistenceProperty("Cor")]
        public string Cor { get; set; }

        [PersistenceProperty("Servico")]
        public string Servico { get; set; }

        [PersistenceProperty("FixacaoVidro")]
        public string FixacaoVidro { get; set; }

        [PersistenceProperty("DataPedido")]
        public DateTime DataPedido { get; set; }

        [PersistenceProperty("Emitido")]
        public bool Emitido { get; set; }

        [PersistenceProperty("Observacoes")]
        public string Observacoes { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Altura
        {
            get { return String.Empty; }
            set { Medidas = value; }
        }

        public string Largura
        {
            get { return String.Empty; }
            set { Medidas += " x " + value; }
        }

        public string FlagEmitido
        {
            get
            {
                if (Emitido)
                    return "Sim";
                else
                    return "Não";
            }
        }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion
    }
}