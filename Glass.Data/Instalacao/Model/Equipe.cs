using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EquipeDAO))]
	[PersistenceClass("equipe")]
	public class Equipe
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativa = 1,
            Inativa
        }

        public enum TipoEquipeEnum
        {
            ColocacaoComum = 1,
            ColocacaoTemperado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDEQUIPE", PersistenceParameterType.IdentityKey)]
        public uint IdEquipe { get; set; }

        /// <summary>
        /// 1-Ativa
        /// 2-Inativa
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        private string _placa;

        [PersistenceProperty("PLACA")]
        public string Placa
        {
            get { return _placa != null ? _placa.ToUpper() : _placa; }
            set { _placa = value; }
        }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        /// <summary>
        /// 1-Colocação Comum
        /// 2-Colocação Temperado
        /// </summary>
        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("LOGIN")]
        public string Login { get; set; }

        [PersistenceProperty("SENHA")]
        public string Senha { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescrFunc", DirectionParameter.InputOptional)]
        public string DescrFunc { get; set; }

        [PersistenceProperty("DescrVeiculo", DirectionParameter.InputOptional)]
        public string DescrVeiculo { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        private string _nomeEstendido;

        public string NomeEstendido 
        {
            get { return _nomeEstendido ;}
            set { _nomeEstendido = value; }
        }

        public string DescrTipo
        {
            get 
            {
                switch (Tipo)
                {
                    case (int)TipoEquipeEnum.ColocacaoComum: return "Colocação Comum";
                    case (int)TipoEquipeEnum.ColocacaoTemperado: return "Colocação Temperado";
                    default: return "";
                }
            }
        }

        public string DescrSituacao
        {
            get 
            {
                switch (Situacao)
                {
                    case (int)SituacaoEnum.Ativa: return "Ativa";
                    case (int)SituacaoEnum.Inativa: return "Inativa";
                    default: return "";
                }
            }
        }

        #endregion
    }
}