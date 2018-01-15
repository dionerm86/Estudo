using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DescontoFormaPagamentoDadosProdutoDAO))]
    [PersistenceClass("desconto_forma_pagamento_dados_produto")]
    public class DescontoFormaPagamentoDadosProduto : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDDESCONTOFORMAPAGAMENTODADOSPRODUTO", PersistenceParameterType.IdentityKey)]
        public uint IdDescontoFormaPagamentoDadosProduto { get; set; }

        [PersistenceProperty("TIPOVENDA")]
        [PersistenceForeignKey(typeof(Pedido), "TipoVenda")]
        public uint TipoVenda { get; set; }

        [Log("Forma Pagto.", "Descricao", typeof(FormaPagtoDAO))]
        [PersistenceProperty("IDFORMAPAGTO")]
        [PersistenceForeignKey(typeof(Pedido), "IdFormaPagto")]
        public uint? IdFormaPagto { get; set; }

        [Log("Tipo Cartão", "Descrição", typeof(TipoCartaoCreditoDAO))]
        [PersistenceProperty("IDTIPOCARTAO")]
        [PersistenceForeignKey(typeof(Pedido), "IdTipoCartao")]
        public uint? IdTipoCartao { get; set; }

        [Log("Parcela", "Descricao", typeof(ParcelasDAO))]
        [PersistenceProperty("IDPARCELA")]
        [PersistenceForeignKey(typeof(Pedido), "IdParcela")]
        public uint? IdParcela { get; set; }

        [Log("Grupo", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        [PersistenceForeignKey(typeof(GrupoProd), "IdGrupoProd")]
        public uint? IdGrupoProd { get; set; }

        [Log("Subgrupo", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        [PersistenceForeignKey(typeof(SubgrupoProd), "IdSubgrupoProd")]
        public uint? IdSubgrupoProd { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("Situacao")]
        [PersistenceProperty("SITUACAO")]
        public Situacao Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescGrupoProd", DirectionParameter.InputOptional)]
        public string DescGrupoProd { get; set; }

        [PersistenceProperty("DescSubgrupoProd", DirectionParameter.InputOptional)]
        public string DescSubgrupoProd { get; set; }

        [PersistenceProperty("DescFormaPagto", DirectionParameter.InputOptional)]
        public string DescFormaPagto { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Tipo de venda")]
        public string DescrTipoVenda
        {
            get
            {
                return Glass.Data.Model.Pedido.GetDescrTipoVenda((int?)TipoVenda);
            }
        }

        public string DescricaoParcelas
        {
            get
            {
                if (IdParcela > 0)
                {
                    foreach (var p in ParcelasDAO.Instance.GetAll())
                        if (p.IdParcela == IdParcela)
                            return p.Descricao;
                }

                return "";
            }
        }

        public string DescTipoCartao
        {
            get
            {
                if (IdTipoCartao.GetValueOrDefault(0) == 0)
                    return string.Empty;

                return TipoCartaoCreditoDAO.Instance.ObterDescricao(null, (int)IdTipoCartao.Value);
            }
        }

        #endregion
    }
}
