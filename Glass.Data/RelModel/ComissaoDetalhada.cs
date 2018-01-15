using System;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ComissaoDetalhadaDAO))]
    public class ComissaoDetalhada
    {
        #region Enumeradores

        public enum TipoFunc
        {
            Funcionario,
            Comissionado,
            Instalador
        }

        #endregion

        #region Propriedades

        public uint IdFuncionario { get; set; }

        public int TipoFuncionario { get; set; }

        public string NomeFuncionario { get; set; }

        public string Cpf { get; set; }

        public string Logradouro { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Cep { get; set; }

        public string TelefoneContato { get; set; }

        public string TelefoneResidencial { get; set; }

        public string TelefoneCelular { get; set; }

        public string Email { get; set; }

        public string Banco { get; set; }

        public string Agencia { get; set; }

        public string Conta { get; set; }

        public string Obs { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Endereco
        {
            get { return Logradouro + (!String.IsNullOrEmpty(Complemento) ? " " + Complemento : ""); }
        }

        public string Telefones
        {
            get
            {
                string telefones = "";

                if (!String.IsNullOrEmpty(TelefoneContato) && !telefones.ToLower().Contains(TelefoneContato.ToLower()))
                    telefones += " / " + TelefoneContato;

                if (!String.IsNullOrEmpty(TelefoneResidencial) && !telefones.ToLower().Contains(TelefoneResidencial.ToLower()))
                    telefones += " / " + TelefoneResidencial;

                if (!String.IsNullOrEmpty(TelefoneCelular) && !telefones.ToLower().Contains(TelefoneCelular.ToLower()))
                    telefones += " / " + TelefoneCelular;

                return telefones.TrimStart(' ', '/');
            }
        }

        #endregion
    }
}