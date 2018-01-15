using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DescontoAcrescimoClienteDAO))]
    [PersistenceClass("desconto_acrescimo_cliente")]
    public class DescontoAcrescimoCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDDESCONTO", PersistenceParameterType.IdentityKey)]
        public int IdDesconto { get; set; }

        //[Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int? IdCliente { get; set; }

        //[Log("Grupo", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        [PersistenceForeignKey(typeof(GrupoProd), "IdGrupoProd")]
        public int IdGrupoProd { get; set; }

        //[Log("Subgrupo", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        [PersistenceForeignKey(typeof(SubgrupoProd), "IdSubgrupoProd")]
        public int? IdSubgrupoProd { get; set; }

        //[Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int? IdProduto { get; set; }

        //[Log("Tabela", "Descricao", typeof(TabelaDescontoAcrescimoClienteDAO))]
        [PersistenceProperty("IDTABELADESCONTO")]
        [PersistenceForeignKey(typeof(TabelaDescontoAcrescimoCliente), "IdTabelaDesconto")]
        public int? IdTabelaDesconto { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public float Desconto { get; set; }

        [Log("Acrescimo")]
        [PersistenceProperty("ACRESCIMO")]
        public float Acrescimo { get; set; }

        [Log("DescontoAVista")]
        [PersistenceProperty("DESCONTOAVISTA")]
        public float DescontoAVista { get; set; }

        [Log("Aplicar Beneficiamentos")]
        [PersistenceProperty("APLICARBENEFICIAMENTOS")]
        public bool AplicarBeneficiamentos { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DescrGrupo", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        [PersistenceProperty("DescrSubgrupo", DirectionParameter.InputOptional)]
        public string DescrSubgrupo { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string DescrGrupoSubgrupo
        {
            get { return DescrGrupo + (IdSubgrupoProd > 0 ? " " + DescrSubgrupo : String.Empty); }
        }

        public string DescricaoCompleta
        {
            get { return string.Format("Cliente: {0} IdTabelaDesconto: {1} Grupo: {2}{3}",
                NomeCliente, IdTabelaDesconto, DescrGrupoSubgrupo, (IdProduto > 0 ? " Produto: " + DescrProduto : string.Empty)); }
        }

        /// <summary>
        /// Percentual para multiplicação no valor do produto, já considerando acréscimo e desconto.
        /// </summary>
        public decimal PercMultiplicar
        {
            get 
            { 
                return (decimal)(Acrescimo > Desconto ? 1 + ((Acrescimo - Desconto) / 100) : 
                    1 - ((Desconto - Acrescimo) / 100)); 
            }
        }

        /// <summary>
        /// Percentual para multiplicação no valor do produto, já considerando acréscimo e desconto.
        /// </summary>
        public decimal PercMultiplicarAVista
        {
            get
            {
                return (decimal)(Acrescimo > DescontoAVista ? 1 + ((Acrescimo - DescontoAVista) / 100) :
                    1 - ((DescontoAVista - Acrescimo) / 100));
            }
        }

        public bool TemOcorrenciasProdutos { get; set; }

        #endregion
    }
}