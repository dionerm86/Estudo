using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovEstoqueFiscalDAO))]
    [PersistenceClass("mov_estoque_fiscal")]
    public class MovEstoqueFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDMOVESTOQUEFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdMovEstoqueFiscal { get; set; }

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

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("NCM", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        [PersistenceProperty("CODUNIDADE", DirectionParameter.InputOptional)]
        public string CodUnidade { get; set; }

        [PersistenceProperty("DESCRGRUPO", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        [PersistenceProperty("DESCRSUBGRUPO", DirectionParameter.InputOptional)]
        public string DescrSubgrupo { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("IDSPEDIDO", DirectionParameter.InputOptional)]
        public string IdPedido { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                string referencia = String.Empty;

                if (IdNf > 0)
                    referencia += " Nota Fiscal: " + Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(IdNf.ToString());

                if (!String.IsNullOrEmpty(IdPedido))
                    referencia += "Ped.: " + IdPedido;

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

        public bool DeleteVisible
        {
            get { return LancManual; }
        }

        #endregion
    }
}