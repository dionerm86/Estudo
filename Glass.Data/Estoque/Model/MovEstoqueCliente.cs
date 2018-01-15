using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceClass("mov_estoque_cliente")]
    [PersistenceBaseDAO(typeof(MovEstoqueClienteDAO))]
    public class MovEstoqueCliente
    {
        #region Propriedades

        [PersistenceProperty("IDMOVESTOQUECLIENTE", PersistenceParameterType.IdentityKey)]
        public uint IdMovEstoqueCliente { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDPRODLIBERARPEDIDO")]
        public uint? IdProdLiberarPedido { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint? IdProdPedProducao { get; set; }

        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [Log("Qtde.")]
        [PersistenceProperty("QTDEMOV")]
        public decimal QtdeMov { get; set; }

        [Log("Saldo")]
        [PersistenceProperty("SALDOQTDEMOV")]
        public decimal SaldoQtdeMov { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALORMOV")]
        public decimal ValorMov { get; set; }

        [Log("Valor Acumulado")]
        [PersistenceProperty("SALDOVALORMOV")]
        public decimal SaldoValorMov { get; set; }

        [Log("Lançamento Manual")]
        [PersistenceProperty("LANCMANUAL")]
        public bool LancManual { get; set; }

        [Log("Data Cadastro")]
        [PersistenceProperty("DATACAD")]
        public DateTime? DataCad { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        #endregion

        #region Propriedades estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("CODUNIDADE", DirectionParameter.InputOptional)]
        public string CodUnidade { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion

        #region Propriedades de suporte

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                string referencia = String.Empty;

                if (IdPedido > 0)
                    referencia += " Pedido: " + IdPedido;

                if (IdLiberarPedido > 0)
                    referencia += " Liberação: " + IdLiberarPedido;

                if (IdNf > 0)
                    referencia += " Nota Fiscal: " + Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(IdNf.ToString());

                if (IdProdPedProducao> 0)
                    referencia += " Etiq. Produção: " + Glass.Data.DAL.ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(IdProdPedProducao.Value);

                if (LancManual)
                    referencia += " Lanc. Manual";

                return referencia.TrimStart(' ');
            }
        }

        [Log("Entrada/Saída")]
        public string DescrTipoMov
        {
            get { return MovEstoque.ObtemDescricaoTipoMov(TipoMov); }
        }
        
        public string NomeFuncAbrev
        {
            get { return BibliotecaTexto.GetTwoFirstNames(NomeFunc); }
        }

        #endregion
    }
}
