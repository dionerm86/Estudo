using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AdministradoraCartaoDAO))]
    [PersistenceClass("administradora_cartao")]
    public class AdministradoraCartao : Sync.Fiscal.EFD.Entidade.IAdministradoraCartao
    {
        #region Propriedades

        [PersistenceProperty("IDADMINCARTAO", PersistenceParameterType.IdentityKey)]
        public uint IdAdminCartao { get; set; }

        [PersistenceProperty("IDCIDADE")]
        public uint IdCidade { get; set; }

        [Log("Nome", true)]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [Log("CNPJ", true)]
        [PersistenceProperty("CNPJ")]
        public string Cnpj { get; set; }

        [Log("Inscrição Estadual", true)]
        [PersistenceProperty("INSCREST")]
        public string InscrEst { get; set; }

        [Log("Endereço", true)]
        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [Log("Número", true)]
        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [Log("Complemento", true)]
        [PersistenceProperty("COMPLEMENTO")]
        public string Compl { get; set; }

        [Log("Bairro", true)]
        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _cidade;

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string Cidade
        {
            get { return _cidade == null ? String.Empty : _cidade.ToUpper(); }
            set { _cidade = value; }
        }

        [PersistenceProperty("NOMEUF", DirectionParameter.InputOptional)]
        public string Uf { get; set; }

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodUf { get; set; }

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Cód. IBGE Cidade", true)]
        public string CodIBGECompleto
        {
            get { return CodUf + CodIbgeCidade; }
        }

        public string DescrEndereco
        {
            get
            {
                return Endereco + ", " + Numero + (!String.IsNullOrEmpty(Compl) ? "(" + Compl + ")" : String.Empty) + 
                    " - " + Bairro + " - " + _cidade + "/" + Uf;
            }
        }

        #endregion

        #region IParticipanteNFe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Codigo
        {
            get { return (int)IdAdminCartao; }
        }

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CodigoCidade
        {
            get { return (int)IdCidade; }
        }

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IParticipanteNFe.TipoPessoa
        {
            get { return Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CpfCnpj
        {
            get { return Cnpj; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.InscricaoEstadual
        {
            get { return InscrEst; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Suframa
        {
            get { return null; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Complemento
        {
            get { return Compl; }
        }

        #endregion
    }
}