using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;
using System.ComponentModel.Composition;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.IO;
using GDA;

namespace Glass.Financeiro.UI.Web.Process.CartaoNaoIdentificado
{
    /// <summary>
    /// Fluxo de cadastro para CNI
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CadastroCartaoNaoIdentificadoFluxo
    {
        #region Variáveis Locais

        private int? _idCNI;
        private Negocios.ICartaoNaoIdentificadoFluxo _fluxoCNI;
        private Negocios.IArquivoCartaoNaoIdentificadoFluxo _fluxoACNI;
        private Negocios.Entidades.CartaoNaoIdentificado _CNI;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor Padrão
        /// </summary>
        [ImportingConstructor]
        public CadastroCartaoNaoIdentificadoFluxo(Negocios.ICartaoNaoIdentificadoFluxo fluxoCNI, Negocios.IArquivoCartaoNaoIdentificadoFluxo fluxoACNI)
        {
            _fluxoCNI = fluxoCNI;
            _fluxoACNI = fluxoACNI;
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id do Cartão não identificado
        /// </summary>
        public int? IdCNI
        {
            get { return _idCNI; }
            set
            {
                if (_idCNI != value)
                {
                    _idCNI = value;
                    _CNI = null;
                }
            }
        }

        /// <summary>
        /// Pedido.
        /// </summary>
        public Negocios.Entidades.CartaoNaoIdentificado CNI
        {
            get
            {
                if (_CNI == null && IdCNI.HasValue)
                    _CNI = _fluxoCNI.ObterCartaoNaoIdentificado(IdCNI.Value);

                else if (_CNI == null && !IdCNI.HasValue)
                    _CNI = _fluxoCNI.CriarCartaoNaoIdentificado();

                return _CNI;
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Insere as parcelas do cartão
        /// </summary>
        private UtilsFinanceiro.DadosRecebimento InserirParcelas(GDASession transaction, Negocios.Entidades.CartaoNaoIdentificado cni)
        {
            return UtilsFinanceiro.Receber(transaction, UserInfo.GetUserInfo.IdLoja, null, null, null, null,
                null, null, null, null, null, null, null, 0, 0, (uint)cni.IdCartaoNaoIdentificado, cni.DataVenda.ToString(), cni.Valor, cni.Valor,
                new[] { cni.Valor }, new[] { (uint)Data.Model.Pagto.FormaPagto.Cartao }, new[] { (uint)cni.IdContaBanco }, null, null,
                new[] { (uint)cni.TipoCartao }, null, null, 0, false, false, 0, null, cni.CxDiario,
                new[] { (uint)cni.NumeroParcelas }, null, false, UtilsFinanceiro.TipoReceb.CartaoNaoIdentificado);
        }

        /// <summary>
        /// Insere um novo cartão não identificado
        /// </summary>
        public Colosoft.Business.SaveResult InserirCartaoNaoIdentificado(Negocios.Entidades.CartaoNaoIdentificado cni)
        {
            var resultado = _fluxoCNI.SalvarCartaoNaoIdentificado(cni);

            if (!resultado)
                return resultado;

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    InserirParcelas(transaction, cni);
                    transaction.Commit();
                    transaction.Close();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    _fluxoCNI.ApagarCartaoNaoIdentificado(cni);
                    ErroDAO.Instance.InserirFromException("Falha ao inserir cartão não identificado.", ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber valor cartão não identificado", ex));
                }
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Salvar alterações no Cartão não identificado
        /// </summary>
        public Colosoft.Business.SaveResult AlterarCartaoNaoIdentificado(Negocios.Entidades.CartaoNaoIdentificado cni)
        {
            if(cni.ChangedProperties.Contains("Valor") || cni.ChangedProperties.Contains("DataRecebimento"))
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();
                        var parcelas = _fluxoCNI.PesquisarIdsParcelasCNI(cni.IdCartaoNaoIdentificado);

                        foreach (var item in parcelas)
                        {
                            ContasReceberDAO.Instance.DeleteByPrimaryKey(transaction, item);
                        }

                        InserirParcelas(transaction, cni);
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("Falha ao alterar cartão não identificado.", ex);
                        return new Colosoft.Business.SaveResult(false, ("Falha ao receber valor cartão não identificado" + ex.Message.ToString()).GetFormatter());
                    }
                }                                
            }

             return _fluxoCNI.SalvarCartaoNaoIdentificado(cni);
        }

        /// <summary>
        /// Importa um xlsx para criação de novos CNI
        /// </summary>
        public Colosoft.Business.SaveResult Importar(Stream stream, string extensao, bool cxDiario)
        {
            var mensagem = string.Empty;
            var resultadoImportacao = _fluxoACNI.Importar(stream, extensao);

            if (!resultadoImportacao)
                return new Colosoft.Business.SaveResult(false, resultadoImportacao.Message);            

            foreach (var item in resultadoImportacao.CartoesNaoIdentificados)
            {
                item.CxDiario = cxDiario;
                var resultado = InserirCartaoNaoIdentificado(item);

                if (!resultado)
                    mensagem += resultado.Message.ToString() + 
                        string.Format(" Cartão: (Numero de Autorização: {0}, Valor: {1}, Numero de Parcelas: {2});", item.NumAutCartao, item.Valor, item.NumeroParcelas);
            }

            return new Colosoft.Business.SaveResult(true, mensagem.GetFormatter());
        }

        #endregion

        //Apagar isso após feito o processo
        #region Método para ExecScript

        /// <summary>
        /// Cria movimentações bancárias para os CNIs que ficaram incorretos anteriormente.
        /// </summary>
        public string AjustarCNI()
        {
            var retorno = string.Empty;
            var cnis = _fluxoCNI.ObterCartoesNaoIdentificadosDebitoSemMovimentacao();

            foreach(var cni in cnis)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        InserirParcelas(transaction, cni);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        retorno += "As movimentações bancárias do CNI " + cni.IdCartaoNaoIdentificado + " não puderam ser criadas. " + ex.InnerException.ToString();
                    }
                }
            }

            return retorno;
        }

        #endregion
    }
}
