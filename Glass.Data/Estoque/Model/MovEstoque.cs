using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovEstoqueDAO))]
    [PersistenceClass("mov_estoque")]
    public class MovEstoque
    {
        #region Enumeradores

        public enum TipoMovEnum
        {
            Entrada = 1,
            Saida
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDMOVESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdMovEstoque { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }
        
        [PersistenceProperty("IDCOMPRA")]
        public uint? IdCompra { get; set; }

        [PersistenceProperty("IDPRODCOMPRA")]
        public uint? IdProdCompra { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDPRODLIBERARPEDIDO")]
        public uint? IdProdLiberarPedido { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint? IdProdPedProducao { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IDPRODTROCADEV")]
        public uint? IdProdTrocaDev { get; set; }

        [PersistenceProperty("IDPRODTROCADO")]
        public uint? IdProdTrocado { get; set; }

        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("IDPEDIDOINTERNO")]
        public uint? IdPedidoInterno { get; set; }

        [PersistenceProperty("IDPRODPEDINTERNO")]
        public uint? IdProdPedInterno { get; set; }

        [PersistenceProperty("IDRETALHOPRODUCAO")]
        public uint? IdRetalhoProducao { get; set; }

        [PersistenceProperty("IDPERDACHAPAVIDRO")]
        public uint? IdPerdaChapaVidro { get; set; }

        [PersistenceProperty("IDCARREGAMENTO")]
        public uint? IdCarregamento { get; set; }

        [PersistenceProperty("IDVOLUME")]
        public uint? IdVolume { get; set; }

        [PersistenceProperty("IDINVENTARIOESTOQUE")]
        public uint? IdInventarioEstoque { get; set; }

        [PersistenceProperty("IdProdImpressaoChapa")]
        public uint? IdProdImpressaoChapa { get; set; }

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

        [PersistenceProperty("LANCMANUAL")]
        public bool LancManual { get; set; }

        [Log("Data Cadastro")]
        [PersistenceProperty("DATACAD")]
        public DateTime? DataCad { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CODUNIDADE", DirectionParameter.InputOptional)]
        public string CodUnidade { get; set; }

        [PersistenceProperty("DESCRGRUPO", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        [PersistenceProperty("DESCRSUBGRUPO", DirectionParameter.InputOptional)]
        public string DescrSubgrupo { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                string referencia = string.Empty;

                if (IdCarregamento > 0)
                    referencia += " Carregamento: " + IdCarregamento;

                if (IdPedido > 0) 
                    referencia += " Pedido: " + IdPedido;

                if (IdCompra > 0) 
                    referencia += " Compra: " + IdCompra;

                if (IdLiberarPedido > 0) 
                    referencia += " Liberação: " + IdLiberarPedido;

                if (IdProdPedProducao > 0)
                {
                    var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd);
                    var etiquetaChapa = string.Empty;

                    if (tipoSubgrupoProd == TipoSubgrupoProd.ChapasVidro)
                        etiquetaChapa = ProdutoPedidoProducaoDAO.Instance.ObtemEtiquetaChapa(null, (int)IdProdPedProducao.Value);

                    if (string.IsNullOrWhiteSpace(etiquetaChapa))
                    {
                        var etiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(IdProdPedProducao.Value);

                        if (!string.IsNullOrWhiteSpace(etiqueta))
                            referencia += string.Format(" Etiq. Produção: {0}", etiqueta);
                    }
                    else
                        referencia += string.Format(" Etiq. Chapa: {0}", etiquetaChapa);
                }

                if (IdTrocaDevolucao > 0)
                    referencia += " Troca/Dev.: " + IdTrocaDevolucao;

                if (IdNf > 0)
                    referencia += " Nota Fiscal: " + NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(IdNf.ToString());

                if (IdPedidoInterno > 0) 
                    referencia += " Pedido Interno: " + IdPedidoInterno;

                if (IdRetalhoProducao > 0)
                    referencia += " Etiqueta: " + RetalhoProducaoDAO.Instance.Obter(IdRetalhoProducao.Value).NumeroEtiqueta;

                if (IdPerdaChapaVidro > 0)
                    referencia += " Perda de Chapa: " + PerdaChapaVidroDAO.Instance.ObtemNumEtiqueta(IdPerdaChapaVidro.Value);

                if (LancManual)
                    referencia += " Lanc. Manual";

                if (IdVolume > 0)
                    referencia += " Etiq. Volume: " + "V" + IdVolume.GetValueOrDefault(0).ToString("D9");

                if (IdInventarioEstoque > 0)
                    referencia += " Inventário de Estoque: " + IdInventarioEstoque;

                if (IdProdImpressaoChapa > 0)
                    referencia += " Etiqueta: " + ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(IdProdImpressaoChapa.Value);

                return referencia.TrimStart(' ');
            }
        }

        [Log("Entrada/Saída")]
        public string DescrTipoMov
        {
            get { return ObtemDescricaoTipoMov(TipoMov); }
        }

        public string NomeFuncAbrev
        {
            get { return BibliotecaTexto.GetTwoFirstNames(NomeFunc); }
        }

        public bool DeleteVisible
        {
            get { return LancManual; }
        }

        #endregion

        #region Métodos internos

        internal static string ObtemDescricaoTipoMov(int tipoMov)
        {
            return tipoMov == (int)TipoMovEnum.Entrada ? "E" : 
                tipoMov == (int)TipoMovEnum.Saida ? "S" : "N/D";
        }

        #endregion
    }
}
