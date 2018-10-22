using Colosoft;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do produto.
    /// </summary>
    public interface IValidadorProduto
    {
        /// <summary>
        /// Valida a atualização dos dados do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Produto produto);

        /// <summary>
        /// Valida a existencia do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Produto produto);
    }

    /// <summary>
    /// Representa a entidade de negócio do produto.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutoLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Produto)]
    public class Produto : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Produto>
    {
        #region Tipos Aninhados

        class ProdutoLoader : Colosoft.Business.EntityLoader<Produto, Data.Model.Produto>
        {
            public ProdutoLoader()
            {
                Configure()
                    .Uid(f => f.IdProd)
                    .FindName(f => f.CodInterno)
                    .Description(f => f.Descricao)
                    .Child<Fiscal.Negocios.Entidades.MvaProdutoUf, Data.Model.MvaProdutoUf>("Mva", f => f.Mva, f => f.IdProd)
                    .Child<Fiscal.Negocios.Entidades.IcmsProdutoUf, Data.Model.IcmsProdutoUf>("AliquotasIcms", f => f.AliquotasIcms, f => f.IdProd)
                    .Child<ProdutoBenef, Data.Model.ProdutoBenef>("ProdutoBeneficiamentos", f => f.ProdutoBeneficiamentos, f => f.IdProd)
                    .Child<Estoque.Negocios.Entidades.ProdutoBaixaEstoque, Data.Model.ProdutoBaixaEstoque>
                        ("BaixasEstoque", f => f.BaixasEstoque, f => f.IdProd)
                    .Log("BaixasEstoque", "Matéria Prima")
                    .Child<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal, Data.Model.ProdutoBaixaEstoqueFiscal>
                        ("BaixasEstoqueFiscal", f => f.BaixasEstoqueFiscal, f => f.IdProd)
                    .Log("BaixasEstoqueFiscal", "Baixas Estoque Fiscal")
                    .Reference<Fornecedor, Data.Model.Fornecedor>("Fornecedor", f => f.Fornecedor, f => f.IdFornec, true, Colosoft.Business.LoadOptions.Lazy)
                    .Reference<EtiquetaAplicacao, Data.Model.EtiquetaAplicacao>
                        ("Aplicacao", f => f.Aplicacao, f => f.IdAplicacao, true, Colosoft.Business.LoadOptions.Lazy)
                    .Reference<EtiquetaProcesso, Data.Model.EtiquetaProcesso>
                        ("Processo", f => f.Processo, f => f.IdProcesso, true, Colosoft.Business.LoadOptions.Lazy)
                    .Reference<GrupoProd, Data.Model.GrupoProd>("GrupoProd", f => f.GrupoProd, f => f.IdGrupoProd, true, Colosoft.Business.LoadOptions.Lazy)
                    .Reference<SubgrupoProd, Data.Model.SubgrupoProd>("Subgrupo", f => f.Subgrupo, f => f.IdSubgrupoProd)
                    .Reference<Fiscal.Negocios.Entidades.Cest, Data.Model.Cest>("Cest", f => f.Cest, f => f.IdCest)
                    .Child<ProdutoNCM, Data.Model.ProdutoNCM>("NCMs", f => f.NCMs, f => f.IdProd)
                    .Child<FlagArqMesaProduto, Data.Model.FlagArqMesaProduto>("FlagArqMesaProduto", f => f.FlagArqMesaProduto, f => f.IdProduto)
                    .Reference<UnidadeMedida, Data.Model.UnidadeMedida>("UnidadeMedida", f => f.UnidadeMedida, f => f.IdUnidadeMedida, true, Colosoft.Business.LoadOptions.Lazy)
                    .Creator(f => new Produto(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.MvaProdutoUf> _mva;
        private Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.IcmsProdutoUf> _aliquotasIcms;
        private Colosoft.Business.IEntityChildrenList<ProdutoBenef> _produtoBeneficiamentos;
        private Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoque> _baixasEstoque;
        private Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal> _baixasEstoqueFiscal;
        private Colosoft.Business.IEntityChildrenList<ProdutoNCM> _NCMs;
        private Colosoft.Business.IEntityChildrenList<FlagArqMesaProduto> _flagArqMesaProduto;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do fornecedor associado.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public Fornecedor Fornecedor
        {
            get { return GetReference<Fornecedor>("Fornecedor", true); }
        }

        /// <summary>
        /// Aplicação associada.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public EtiquetaAplicacao Aplicacao
        {
            get { return GetReference<EtiquetaAplicacao>("Aplicacao", true); }
        }

        /// <summary>
        /// Processo associado.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public EtiquetaProcesso Processo
        {
            get { return GetReference<EtiquetaProcesso>("Processo", true); }
        }

        /// <summary>
        /// Obtém o grupo de produtos.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public GrupoProd GrupoProd
        {
            get
            {
                return this.GetReference<GrupoProd>("GrupoProd", true);
            }
        }

        /// <summary>
        /// Subgrupo ao qual o produto pertence.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public SubgrupoProd Subgrupo
        {
            get { return GetReference<SubgrupoProd>("Subgrupo", true); }
        }

        /// <summary>
        /// Cest associado.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public Fiscal.Negocios.Entidades.Cest Cest
        {
            get { return GetReference<Fiscal.Negocios.Entidades.Cest>("Cest", true); }
        }

        /// <summary>
        /// Obtém a unidade de medida associada.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public UnidadeMedida UnidadeMedida
        {
            get
            {
                return this.GetReference<UnidadeMedida>("UnidadeMedida", true);
            }
        }

        /// <summary>
        /// Relação dos beneficiamentos do produto.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<ProdutoBenef> ProdutoBeneficiamentos
        {
            get { return _produtoBeneficiamentos; }
        }

        /// <summary>
        /// Relação das baixas de estoque do produto.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoque> BaixasEstoque
        {
            get { return _baixasEstoque; }
        }

        /// <summary>
        /// Relação das baixas do estoque fiscal.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal> BaixasEstoqueFiscal
        {
            get { return _baixasEstoqueFiscal; }
        }

        /// <summary>
        /// Mva's associados.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.MvaProdutoUf> Mva
        {
            get { return _mva; }
        }

        /// <summary>
        /// Aliquotas ICMS associadas.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.IcmsProdutoUf> AliquotasIcms
        {
            get { return _aliquotasIcms; }
        }

        /// <summary>
        /// NCM por loja
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<ProdutoNCM> NCMs
        {
            get { return _NCMs; }
        }

        /// <summary>
        /// NCM por loja
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<FlagArqMesaProduto> FlagArqMesaProduto
        {
            get { return _flagArqMesaProduto; }
        }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd
        {
            get { return DataModel.IdProd; }
            set
            {
                if (DataModel.IdProd != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProd = value;
                    RaisePropertyChanged("IdProd");
                }
            }
        }

        /// <summary>
        /// Identificador do fornecedor associado.
        /// </summary>
        public int IdFornec
        {
            get { return DataModel.IdFornec; }
            set
            {
                if (DataModel.IdFornec != value &&
                    RaisePropertyChanging("IdFornec", value))
                {
                    DataModel.IdFornec = value;
                    RaisePropertyChanged("IdFornec");
                }
            }
        }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int? IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador do grupo de produtos.
        /// </summary>
        public int IdGrupoProd
        {
            get { return DataModel.IdGrupoProd; }
            set
            {
                if (DataModel.IdGrupoProd != value &&
                    RaisePropertyChanging("IdGrupoProd", value))
                {
                    DataModel.IdGrupoProd = value;
                    RaisePropertyChanged("IdGrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro
        {
            get { return DataModel.IdCorVidro; }
            set
            {
                if (DataModel.IdCorVidro != value &&
                    RaisePropertyChanging("IdCorVidro", value))
                {
                    DataModel.IdCorVidro = value;
                    RaisePropertyChanged("IdCorVidro");
                }
            }
        }

        /// <summary>
        /// Identificador da cor da ferragem.
        /// </summary>
        public int? IdCorFerragem
        {
            get { return DataModel.IdCorFerragem; }
            set
            {
                if (DataModel.IdCorFerragem != value &&
                    RaisePropertyChanging("IdCorFerragem", value))
                {
                    DataModel.IdCorFerragem = value;
                    RaisePropertyChanged("IdCorFerragem");
                }
            }
        }

        /// <summary>
        /// Identificador da cor do Aluminio.
        /// </summary>
        public int? IdCorAluminio
        {
            get { return DataModel.IdCorAluminio; }
            set
            {
                if (DataModel.IdCorAluminio != value &&
                    RaisePropertyChanging("IdCorAluminio", value))
                {
                    DataModel.IdCorAluminio = value;
                    RaisePropertyChanged("IdCorAluminio");
                }
            }
        }

        /// <summary>
        /// Identificador da unidade de medida.
        /// </summary>
        public int IdUnidadeMedida
        {
            get { return DataModel.IdUnidadeMedida; }
            set
            {
                if (DataModel.IdUnidadeMedida != value &&
                    RaisePropertyChanging("IdUnidadeMedida", value))
                {
                    DataModel.IdUnidadeMedida = value;
                    RaisePropertyChanged("IdUnidadeMedida");
                }
            }
        }

        /// <summary>
        /// Identificador da unidade de medida Tributária.
        /// </summary>
        public int IdUnidadeMedidaTrib
        {
            get { return DataModel.IdUnidadeMedidaTrib; }
            set
            {
                if (DataModel.IdUnidadeMedidaTrib != value &&
                    RaisePropertyChanging("IdUnidadeMedidaTrib", value))
                {
                    DataModel.IdUnidadeMedidaTrib = value;
                    RaisePropertyChanged("IdUnidadeMedidaTrib");
                }
            }
        }

        /// <summary>
        /// Identificador do Genero do produto.
        /// </summary>
        public int? IdGeneroProduto
        {
            get { return DataModel.IdGeneroProduto; }
            set
            {
                if (DataModel.IdGeneroProduto != value &&
                    RaisePropertyChanging("IdGeneroProduto", value))
                {
                    DataModel.IdGeneroProduto = value;
                    RaisePropertyChanged("IdGeneroProduto");
                }
            }
        }

        /// <summary>
        /// Identificador do arquivo da mesa de corte.
        /// </summary>
        public int? IdArquivoMesaCorte
        {
            get { return DataModel.IdArquivoMesaCorte; }
            set
            {
                if (DataModel.IdArquivoMesaCorte != value &&
                    RaisePropertyChanging("IdArquivoMesaCorte", value))
                {
                    DataModel.IdArquivoMesaCorte = value;
                    RaisePropertyChanged("IdArquivoMesaCorte");
                }
            }
        }
 
        /// <summary>
        /// Tipo do arquivo de mesa de corte.
        /// </summary>
        public Data.Model.TipoArquivoMesaCorte? TipoArquivo
        {
            get { return DataModel.TipoArquivo; }
            set
            {
                if (DataModel.TipoArquivo != value &&
                    RaisePropertyChanging("TipoArquivo", value))
                {
                    DataModel.TipoArquivo = value;
                    RaisePropertyChanged("TipoArquivo");
                }
            }
        }

        /// <summary>
        /// Tipo de mercadoria.
        /// </summary>
        public Glass.Data.Model.TipoMercadoria? TipoMercadoria
        {
            get { return DataModel.TipoMercadoria; }
            set
            {
                if (DataModel.TipoMercadoria != value &&
                    RaisePropertyChanging("TipoMercadoria", value))
                {
                    DataModel.TipoMercadoria = value;
                    RaisePropertyChanged("TipoMercadoria");
                }
            }
        }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno
        {
            get { return DataModel.CodInterno; }
            set
            {
                if (DataModel.CodInterno != value &&
                    RaisePropertyChanging("CodInterno", value))
                {
                    if (value != null)
                        value = value.ToUpper();

                    DataModel.CodInterno = value;
                    RaisePropertyChanged("CodInterno");
                }
            }
        }

        /// <summary>
        /// Código EX.
        /// </summary>
        public string CodigoEX
        {
            get { return DataModel.CodigoEX; }
            set
            {
                if (DataModel.CodigoEX != value &&
                    RaisePropertyChanging("CodigoEX", value))
                {
                    DataModel.CodigoEX = value;
                    RaisePropertyChanged("CodigoEX");
                }
            }
        }

        /// <summary>
        /// GTIN Produto.
        /// </summary>
        public string GTINProduto
        {
            get { return DataModel.GTINProduto; }
            set
            {
                if (DataModel.GTINProduto != value &&
                    RaisePropertyChanging("GTINProduto", value))
                {
                    DataModel.GTINProduto = value;
                    RaisePropertyChanged("GTINProduto");
                }
            }
        }

        /// <summary>
        /// GTIN Unid. Trib.
        /// </summary>
        public string GTINUnidTrib
        {
            get { return DataModel.GTINUnidTrib; }
            set
            {
                if (DataModel.GTINUnidTrib != value &&
                    RaisePropertyChanging("GTINUnidTrib", value))
                {
                    DataModel.GTINUnidTrib = value;
                    RaisePropertyChanged("GTINUnidTrib");
                }
            }
        }

        /// <summary>
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Custo de fabricação base.
        /// </summary>
        public decimal Custofabbase
        {
            get { return DataModel.Custofabbase; }
            set
            {
                if (DataModel.Custofabbase != value &&
                    RaisePropertyChanging("Custofabbase", value))
                {
                    DataModel.Custofabbase = value;
                    RaisePropertyChanged("Custofabbase");
                }
            }
        }

        /// <summary>
        /// Custom de compra.
        /// </summary>
        public decimal CustoCompra
        {
            get { return DataModel.CustoCompra; }
            set
            {
                if (DataModel.CustoCompra != value &&
                    RaisePropertyChanging("CustoCompra", value))
                {
                    DataModel.CustoCompra = value;
                    RaisePropertyChanged("CustoCompra");
                }
            }
        }

        /// <summary>
        /// Valor atacado.
        /// </summary>
        public decimal ValorAtacado
        {
            get { return DataModel.ValorAtacado; }
            set
            {
                if (DataModel.ValorAtacado != value &&
                    RaisePropertyChanging("ValorAtacado", value))
                {
                    DataModel.ValorAtacado = value;
                    RaisePropertyChanged("ValorAtacado");
                }
            }
        }

        /// <summary>
        /// Valor balcão.
        /// </summary>
        public decimal ValorBalcao
        {
            get { return DataModel.ValorBalcao; }
            set
            {
                if (DataModel.ValorBalcao != value &&
                    RaisePropertyChanging("ValorBalcao", value))
                {
                    DataModel.ValorBalcao = value;
                    RaisePropertyChanged("ValorBalcao");
                }
            }
        }

        /// <summary>
        /// Valor Obra.
        /// </summary>
        public decimal ValorObra
        {
            get { return DataModel.ValorObra; }
            set
            {
                if (DataModel.ValorObra != value &&
                    RaisePropertyChanging("ValorObra", value))
                {
                    DataModel.ValorObra = value;
                    RaisePropertyChanged("ValorObra");
                }
            }
        }

        /// <summary>
        /// Valor reposição.
        /// </summary>
        public decimal ValorReposicao
        {
            get { return DataModel.ValorReposicao; }
            set
            {
                if (DataModel.ValorReposicao != value &&
                    RaisePropertyChanging("ValorReposicao", value))
                {
                    DataModel.ValorReposicao = value;
                    RaisePropertyChanged("ValorReposicao");
                }
            }
        }

        /// <summary>
        /// Valor mínimo.
        /// </summary>
        public decimal ValorMinimo
        {
            get { return DataModel.ValorMinimo; }
            set
            {
                if (DataModel.ValorMinimo != value &&
                    RaisePropertyChanging("ValorMinimo", value))
                {
                    DataModel.ValorMinimo = value;
                    RaisePropertyChanged("ValorMinimo");
                }
            }
        }

        /// <summary>
        /// Valor transferencia.
        /// </summary>
        public decimal ValorTransferencia
        {
            get { return DataModel.ValorTransferencia; }
            set
            {
                if (DataModel.ValorTransferencia != value &&
                    RaisePropertyChanging("ValorTransferencia", value))
                {
                    DataModel.ValorTransferencia = value;
                    RaisePropertyChanged("ValorTransferencia");
                }
            }
        }

        /// <summary>
        /// Cst.
        /// </summary>
        public string Cst
        {
            get { return DataModel.Cst; }
            set
            {
                if (DataModel.Cst != value &&
                    RaisePropertyChanging("Cst", value))
                {
                    DataModel.Cst = value;
                    RaisePropertyChanged("Cst");
                }
            }
        }

        /// <summary>
        /// CSOSN.
        /// </summary>
        public string Csosn
        {
            get { return DataModel.Csosn; }
            set
            {
                if (DataModel.Csosn != value &&
                    RaisePropertyChanging("Csosn", value))
                {
                    DataModel.Csosn = value;
                    RaisePropertyChanged("Csosn");
                }
            }
        }

        /// <summary>
        /// NCM.
        /// </summary>
        public string Ncm
        {
            get { return DataModel.Ncm; }
            set
            {
                if (DataModel.Ncm != value &&
                    RaisePropertyChanging("Ncm", value))
                {
                    if (value != null)
                        value = value.Replace("\t", "");

                    DataModel.Ncm = value;
                    RaisePropertyChanged("Ncm");
                }
            }
        }

        /// <summary>
        /// Alíquota IPI.
        /// </summary>
        public float AliqIPI
        {
            get { return DataModel.AliqIPI; }
            set
            {
                if (DataModel.AliqIPI != value &&
                    RaisePropertyChanging("AliqIPI", value))
                {
                    DataModel.AliqIPI = value;
                    RaisePropertyChanged("AliqIPI");
                }
            }
        }

        /// <summary>
        /// Código de Oitmização.
        /// </summary>
        public string CodOtimizacao
        {
            get { return DataModel.CodOtimizacao; }
            set
            {
                if (DataModel.CodOtimizacao != value &&
                    RaisePropertyChanging("CodOtimizacao", value))
                {
                    if (value != null)
                        value = value.ToUpper();

                    DataModel.CodOtimizacao = value;
                    RaisePropertyChanged("CodOtimizacao");
                }
            }
        }

        /// <summary>
        /// Ativar Mínimo.
        /// </summary>
        public bool AtivarMin
        {
            get { return DataModel.AtivarMin; }
            set
            {
                if (DataModel.AtivarMin != value &&
                    RaisePropertyChanging("AtivarMin", value))
                {
                    DataModel.AtivarMin = value;
                    RaisePropertyChanged("AtivarMin");
                }
            }
        }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float Espessura
        {
            get { return DataModel.Espessura; }
            set
            {
                if (DataModel.Espessura != value &&
                    RaisePropertyChanging("Espessura", value))
                {
                    DataModel.Espessura = value;
                    RaisePropertyChanged("Espessura");
                }
            }
        }

        /// <summary>
        /// Peso.
        /// </summary>
        public float Peso
        {
            get { return DataModel.Peso; }
            set
            {
                if (DataModel.Peso != value &&
                    RaisePropertyChanging("Peso", value))
                {
                    DataModel.Peso = value;
                    RaisePropertyChanged("Peso");
                }
            }
        }

        /// <summary>
        /// Área mínima.
        /// </summary>
        public float AreaMinima
        {
            get { return DataModel.AreaMinima; }
            set
            {
                if (DataModel.AreaMinima != value &&
                    RaisePropertyChanging("AreaMinima", value))
                {
                    DataModel.AreaMinima = value;
                    RaisePropertyChanged("AreaMinima");
                }
            }
        }

        /// <summary>
        /// Ativar Área Mínima.
        /// </summary>
        public bool AtivarAreaMinima
        {
            get { return DataModel.AtivarAreaMinima; }
            set
            {
                if (DataModel.AtivarAreaMinima != value &&
                    RaisePropertyChanging("AtivarAreaMinima", value))
                {
                    DataModel.AtivarAreaMinima = value;
                    RaisePropertyChanged("AtivarAreaMinima");
                }
            }
        }

        /// <summary>
        /// Compra.
        /// </summary>
        public bool Compra
        {
            get { return DataModel.Compra; }
            set
            {
                if (DataModel.Compra != value &&
                    RaisePropertyChanging("Compra", value))
                {
                    DataModel.Compra = value;
                    RaisePropertyChanged("Compra");
                }
            }
        }

        /// <summary>
        /// Item genérico.
        /// </summary>
        public bool ItemGenerico
        {
            get { return DataModel.ItemGenerico; }
            set
            {
                if (DataModel.ItemGenerico != value &&
                    RaisePropertyChanging("ItemGenerico", value))
                {
                    DataModel.ItemGenerico = value;
                    RaisePropertyChanged("ItemGenerico");
                }
            }
        }

        /// <summary>
        /// Data de Alteração.
        /// </summary>
        public DateTime? DataAlt
        {
            get { return DataModel.DataAlt; }
            set
            {
                if (DataModel.DataAlt != value &&
                    RaisePropertyChanging("DataAlt", value))
                {
                    DataModel.DataAlt = value;
                    RaisePropertyChanged("DataAlt");
                }
            }
        }

        /// <summary>
        /// Usuário alteração.
        /// </summary>
        public int? UsuAlt
        {
            get { return DataModel.UsuAlt; }
            set
            {
                if (DataModel.UsuAlt != value &&
                    RaisePropertyChanging("UsuAlt", value))
                {
                    DataModel.UsuAlt = value;
                    RaisePropertyChanged("UsuAlt");
                }
            }
        }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs
        {
            get { return DataModel.Obs; }
            set
            {
                if (DataModel.Obs != value &&
                    RaisePropertyChanging("Obs", value))
                {
                    DataModel.Obs = value;
                    RaisePropertyChanged("Obs");
                }
            }
        }

        /// <summary>
        /// Altura.
        /// </summary>
        public int? Altura
        {
            get { return DataModel.Altura; }
            set
            {
                if (DataModel.Altura != value &&
                    RaisePropertyChanging("Altura", value))
                {
                    DataModel.Altura = value;
                    RaisePropertyChanged("Altura");
                }
            }
        }

        /// <summary>
        /// Largura.
        /// </summary>
        public int? Largura
        {
            get { return DataModel.Largura; }
            set
            {
                if (DataModel.Largura != value &&
                    RaisePropertyChanging("Largura", value))
                {
                    DataModel.Largura = value;
                    RaisePropertyChanged("Largura");
                }
            }
        }

        /// <summary>
        /// Redondo.
        /// </summary>
        public bool Redondo
        {
            get { return DataModel.Redondo; }
            set
            {
                if (DataModel.Redondo != value &&
                    RaisePropertyChanging("Redondo", value))
                {
                    DataModel.Redondo = value;
                    RaisePropertyChanged("Redondo");
                }
            }
        }

        /// <summary>
        /// Forma.
        /// </summary>
        public string Forma
        {
            get { return DataModel.Forma; }
            set
            {
                if (DataModel.Forma != value &&
                    RaisePropertyChanging("Forma", value))
                {
                    DataModel.Forma = value;
                    RaisePropertyChanged("Forma");
                }
            }
        }

        /// <summary>
        /// CST IPI.
        /// </summary>
        public Data.Model.ProdutoCstIpi? CstIpi
        {
            get { return DataModel.CstIpi; }
            set
            {
                if (DataModel.CstIpi != value &&
                    RaisePropertyChanging("CstIpi", value))
                {
                    DataModel.CstIpi = value;
                    RaisePropertyChanged("CstIpi");
                }
            }
        }

        /// <summary>
        /// Plano de conta Contábil.
        /// </summary>
        public int? IdContaContabil
        {
            get { return DataModel.IdContaContabil; }
            set
            {
                if (DataModel.IdContaContabil != value &&
                    RaisePropertyChanging("IdContaContabil", value))
                {
                    DataModel.IdContaContabil = value;
                    RaisePropertyChanged("IdContaContabil");
                }
            }
        }

        /// <summary>
        /// Local de Armazenagem.
        /// </summary>
        public string LocalArmazenagem
        {
            get { return DataModel.LocalArmazenagem; }
            set
            {
                if (DataModel.LocalArmazenagem != value &&
                    RaisePropertyChanging("LocalArmazenagem", value))
                {
                    DataModel.LocalArmazenagem = value;
                    RaisePropertyChanged("LocalArmazenagem");
                }
            }
        }

        /// <summary>
        /// Identificador do processo de etiqueta.
        /// </summary>
        public int? IdProcesso
        {
            get { return DataModel.IdProcesso; }
            set
            {
                if (DataModel.IdProcesso != value &&
                    RaisePropertyChanging("IdProcesso", value))
                {
                    DataModel.IdProcesso = value;
                    RaisePropertyChanged("IdProcesso");
                }
            }
        }

        /// <summary>
        /// Identificador da aplicação de etiqueta.
        /// </summary>
        public int? IdAplicacao
        {
            get { return DataModel.IdAplicacao; }
            set
            {
                if (DataModel.IdAplicacao != value &&
                    RaisePropertyChanging("IdAplicacao", value))
                {
                    DataModel.IdAplicacao = value;
                    RaisePropertyChanged("IdAplicacao");
                }
            }
        }

        /// <summary>
        /// Valor fiscal.
        /// </summary>
        public decimal ValorFiscal
        {
            get { return DataModel.ValorFiscal; }
            set
            {
                if (DataModel.ValorFiscal != value &&
                    RaisePropertyChanging("ValorFiscal", value))
                {
                    DataModel.ValorFiscal = value;
                    RaisePropertyChanged("ValorFiscal");
                }
            }
        }

        /// <summary>
        /// Identificador do produto de origem.
        /// </summary>
        public int? IdProdOrig
        {
            get { return DataModel.IdProdOrig; }
            set
            {
                if (DataModel.IdProdOrig != value &&
                    RaisePropertyChanging("IdProdOrig", value))
                {
                    DataModel.IdProdOrig = value;
                    RaisePropertyChanged("IdProdOrig");
                }
            }
        }

        /// <summary>
        /// Identificador do produto base.
        /// </summary>
        public int? IdProdBase
        {
            get { return DataModel.IdProdBase; }
            set
            {
                if (DataModel.IdProdBase != value &&
                    RaisePropertyChanging("IdProdBase", value))
                {
                    DataModel.IdProdBase = value;
                    RaisePropertyChanged("IdProdBase");
                }
            }
        }

        /// <summary>
        /// MarkUp.
        /// </summary>
        public decimal MarkUp
        {
            get { return DataModel.MarkUp; }
            set
            {
                if (DataModel.MarkUp != value &&
                    RaisePropertyChanging("MarkUp", value))
                {
                    DataModel.MarkUp = value;
                    RaisePropertyChanged("MarkUp");
                }
            }
        }

        /// <summary>
        /// Identificador do CEST.
        /// </summary>
        public int? IdCest
        {
            get { return DataModel.IdCest; }
            set
            {
                if (DataModel.IdCest != value &&
                    RaisePropertyChanging("IdCest", value))
                {
                    DataModel.IdCest = value;
                    RaisePropertyChanged("IdCest");
                }
            }
        }

        /// <summary>
        /// Flags do arquivo de mesa
        /// </summary>
        public int[] FlagsArqMesa
        {
            get { return FlagArqMesaProduto.Select(f=> f.IdFlagArqMesa).ToArray(); }
            set
            {
                var flagsRemover = new Queue<FlagArqMesaProduto>();


                foreach (var flag in FlagArqMesaProduto)
                {
                    if (!value.Contains(flag.IdFlagArqMesa))
                        flagsRemover.Enqueue(flag);
                }

                while (flagsRemover.Count > 0)
                    FlagArqMesaProduto.Remove(flagsRemover.Dequeue());

                foreach (var flag in value)
                {
                    if (!FlagArqMesaProduto.Any(f => f.IdFlagArqMesa == flag))
                        FlagArqMesaProduto.Add(new Entidades.FlagArqMesaProduto { IdFlagArqMesa = flag });
                }
            }
        }

        /// <summary>
        /// descrição das flags do arquivo de mesa
        /// </summary>
        public string FlagsArqMesaDescricao
        {
            get { return DataModel.FlagsArqMesaDescricao; }
            set
            {
                if (DataModel.FlagsArqMesaDescricao != value &&
                    RaisePropertyChanging("FlagsArqMesaDescricao", value))
                {
                    DataModel.FlagsArqMesaDescricao = value;
                    RaisePropertyChanged("FlagsArqMesaDescricao");
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância da margem inferior da chapa.
        /// </summary>
        public double RecorteX1
        {
            get { return DataModel.RecorteX1; }
            set
            {
                if (DataModel.RecorteX1 != value &&
                    RaisePropertyChanging(nameof(RecorteX1), value))
                {
                    DataModel.RecorteX1 = value;
                    RaisePropertyChanged(nameof(RecorteX1));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância da margem esquerda da chapa.
        /// </summary>
        public double RecorteY1
        {
            get { return DataModel.RecorteY1; }
            set
            {
                if (DataModel.RecorteY1 != value &&
                    RaisePropertyChanging(nameof(RecorteY1), value))
                {
                    DataModel.RecorteY1 = value;
                    RaisePropertyChanged(nameof(RecorteY1));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância da margem superior da chapa.
        /// </summary>
        public double RecorteX2
        {
            get { return DataModel.RecorteX2; }
            set
            {
                if (DataModel.RecorteX2 != value &&
                    RaisePropertyChanging(nameof(RecorteX2), value))
                {
                    DataModel.RecorteX2 = value;
                    RaisePropertyChanged(nameof(RecorteX2));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância da margem direita da chapa.
        /// </summary>
        public double RecorteY2
        {
            get { return DataModel.RecorteY2; }
            set
            {
                if (DataModel.RecorteY2 != value &&
                    RaisePropertyChanging(nameof(RecorteY2), value))
                {
                    DataModel.RecorteY2 = value;
                    RaisePropertyChanged(nameof(RecorteY2));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância máxima de lado X da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        public double TransversalMaxX
        {
            get { return DataModel.TransversalMaxX; }
            set
            {
                if (DataModel.TransversalMaxX != value &&
                    RaisePropertyChanging(nameof(TransversalMaxX), value))
                {
                    DataModel.TransversalMaxX = value;
                    RaisePropertyChanged(nameof(TransversalMaxX));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância máxima de lado Y da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        public double TransversalMaxY
        {
            get { return DataModel.TransversalMaxY; }
            set
            {
                if (DataModel.TransversalMaxY != value &&
                    RaisePropertyChanging(nameof(TransversalMaxY), value))
                {
                    DataModel.TransversalMaxY = value;
                    RaisePropertyChanged(nameof(TransversalMaxY));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a dimensão mínima em X da superfície de desperdício
        /// geradas pelo programa de otimização que, ao serem suficientemente 
        /// grandes, se podem considerar reutilizáveis e é desejável introduzi-las
        /// de novo no estoque para otimizações posteriores.
        /// </summary>
        public double DesperdicioMinX
        {
            get { return DataModel.DesperdicioMinX; }
            set
            {
                if (DataModel.DesperdicioMinX != value &&
                    RaisePropertyChanging(nameof(DesperdicioMinX), value))
                {
                    DataModel.DesperdicioMinX = value;
                    RaisePropertyChanged(nameof(DesperdicioMinX));
                }
            }
        }

        /// <summary>
        ///  Obtém ou define a dimensão mínima em Y da superfície de desperdício
        /// geradas pelo programa de otimização que, ao serem suficientemente 
        /// grandes, se podem considerar reutilizáveis e é desejável introduzi-las
        /// de novo no estoque para otimizações posteriores.
        /// </summary>
        public double DesperdicioMinY
        {
            get { return DataModel.DesperdicioMinY; }
            set
            {
                if (DataModel.DesperdicioMinY != value &&
                    RaisePropertyChanging(nameof(DesperdicioMinY), value))
                {
                    DataModel.DesperdicioMinY = value;
                    RaisePropertyChanged(nameof(DesperdicioMinY));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a distância minima aceitavel durante a otimização 
        /// entre dois cortes paralelos, com o intuito de facilitar ou tornar 
        /// possível a abertura dos cortes.
        /// </summary>
        /// <example>
        /// Ao configurar o valor em 20mm, será impossível encontrar no interior
        /// de um plano de corte duas peças ou dois cortes próximos um do outro,
        /// de distância inferior à anteriormente introduzida (20mm).
        /// </example>
        /// <remarks>
        /// Evidentemente, esta distância não é tida em conta nos casos em que 
        /// 2 peças compartilham o mesmo corte.
        /// </remarks>
        public double DistanciaMin
        {
            get { return DataModel.DistanciaMin; }
            set
            {
                if (DataModel.DistanciaMin != value &&
                    RaisePropertyChanging(nameof(DistanciaMin), value))
                {
                    DataModel.DistanciaMin = value;
                    RaisePropertyChanged(nameof(DistanciaMin));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a configuração do valor de recorte que deve
        /// introduzir-se nas formas no caso de esta conter ângulos 
        /// inferiores ao configurado no campo "AnguloRecorteAutomatico".
        /// </summary>
        public double RecorteAutomaticoForma
        {
            get { return DataModel.RecorteAutomaticoForma; }
            set
            {
                if (DataModel.RecorteAutomaticoForma != value &&
                    RaisePropertyChanging(nameof(RecorteAutomaticoForma), value))
                {
                    DataModel.RecorteAutomaticoForma = value;
                    RaisePropertyChanged(nameof(RecorteAutomaticoForma));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o valor do ângulo ao qual o recorte deve
        /// ser introduzido de forma automática.
        /// </summary>
        public double AnguloRecorteAutomatico
        {
            get { return DataModel.AnguloRecorteAutomatico; }
            set
            {
                if (DataModel.AnguloRecorteAutomatico != value &&
                    RaisePropertyChanging(nameof(AnguloRecorteAutomatico), value))
                {
                    DataModel.AnguloRecorteAutomatico = value;
                    RaisePropertyChanged(nameof(AnguloRecorteAutomatico));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Produto()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Produto(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Produto> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _mva = GetChild<Fiscal.Negocios.Entidades.MvaProdutoUf>(args.Children, "Mva");
            _aliquotasIcms = GetChild<Fiscal.Negocios.Entidades.IcmsProdutoUf>(args.Children, "AliquotasIcms");
            _produtoBeneficiamentos = GetChild<ProdutoBenef>(args.Children, "ProdutoBeneficiamentos");
            _baixasEstoque = GetChild<Estoque.Negocios.Entidades.ProdutoBaixaEstoque>(args.Children, "BaixasEstoque");
            _baixasEstoqueFiscal = GetChild<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal>(args.Children, "BaixasEstoqueFiscal");
            _NCMs = GetChild<Global.Negocios.Entidades.ProdutoNCM>(args.Children, "NCMs");
            _flagArqMesaProduto = GetChild<Global.Negocios.Entidades.FlagArqMesaProduto>(args.Children, "FlagArqMesaProduto");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Produto(Data.Model.Produto dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _mva = CreateChild<Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.MvaProdutoUf>>("Mva");
            _aliquotasIcms = CreateChild<Colosoft.Business.IEntityChildrenList<Fiscal.Negocios.Entidades.IcmsProdutoUf>>("AliquotasIcms");
            _produtoBeneficiamentos = CreateChild<Colosoft.Business.IEntityChildrenList<Entidades.ProdutoBenef>>("ProdutoBeneficiamentos");
            _baixasEstoque = CreateChild<Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoque>>("BaixasEstoque");
            _baixasEstoqueFiscal = CreateChild<Colosoft.Business.IEntityChildrenList<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal>>("BaixasEstoqueFiscal");
            _NCMs = CreateChild<Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ProdutoNCM>>("NCMs");
            _flagArqMesaProduto = CreateChild<Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.FlagArqMesaProduto>>("FlagArqMesaProduto");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (IdSubgrupoProd.HasValue)
            {
                //Se o tipo do subgrupo do produto for chapa de vidro, não pode ter materia-prima associada, apenas produto base.
                if (Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.ChapasVidro && BaixasEstoque.Count > 0)
                    return new Colosoft.Business.SaveResult(false, "Produtos de subgrupo do tipo Chapas de Vidro, não podem ter matéria-prima associada.".GetFormatter());

                //Se o tipo do subgrupo do produto for chapa de vidro ou chapas de vidro laminado, obriga a informar produto base.
                if ((Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.ChapasVidro || Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.ChapasVidroLaminado) && !IdProdBase.HasValue)
                    return new Colosoft.Business.SaveResult(false, "Produtos de subgrupo do tipo Chapas de Vidro e Chapas de Vidro Laminado, devem ser informados o Produto Base.".GetFormatter());
            }

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorProduto>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        /// <summary>
        /// Apaga os dados do produto.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorProduto>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            // Apaga os produtos/loja
            session.Delete<Data.Model.ProdutoLoja>(Colosoft.Query.ConditionalContainer
                .Parse("IdProd=?idProd").Add("?idProd", this.IdProd));

            return base.Delete(session);
        }

        /// <summary>
        /// Obtem o NCM de acordo com a loja informada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string ObtemNCM(int idLoja)
        {
            var _ncm = NCMs.Where(f => f.IdLoja == idLoja).Select(f => f.NCM).FirstOrDefault();

            if (string.IsNullOrEmpty(_ncm))
                _ncm = Ncm;

            return _ncm;
        }

        #endregion
    }
}
