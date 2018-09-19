using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncionarioDAO))]
	[PersistenceClass("funcionario")]
	public class Funcionario : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDFUNC", PersistenceParameterType.IdentityKey)]
        public int IdFunc { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdLoja { get; set; }

        [Log("Tipo de Funcionário", "Descricao", typeof(TipoFuncDAO))]
        [PersistenceProperty("IDTIPOFUNC")]
        [PersistenceForeignKey(typeof(TipoFuncionario), "IdTipoFuncionario")]
        public int IdTipoFunc { get; set; }

        [Log("Nome")]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [Log("Função")]
        [PersistenceProperty("FUNCAO")]
        public string Funcao { get; set; }

        [Log("Endereço")]
        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [Log("Complemento")]
        [PersistenceProperty("COMPL")]
        public string Compl { get; set; }

        [Log("Bairro")]
        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        [Log("Cidade")]
        [PersistenceProperty("CIDADE")]
        public string Cidade { get; set; }

        [Log("UF")]
        [PersistenceProperty("UF")]
        public string Uf { get; set; }

        [Log("CEP")]
        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [Log("CPF")]
        [PersistenceProperty("CPF")]
        public string Cpf { get; set; }

        [Log("RG")]
        [PersistenceProperty("RG")]
        public string Rg { get; set; }

        [Log("Data de Nascimento")]
        [PersistenceProperty("DATANASC")]
        public DateTime? DataNasc { get; set; }

        [Log("Estado Civil")]
        [PersistenceProperty("ESTCIVIL")]
        public string EstCivil { get; set; }

        [Log("Telefone Residencial")]
        [PersistenceProperty("TELRES")]
        public string TelRes { get; set; }

        [Log("Telefone de Contato")]
        [PersistenceProperty("TELCONT")]
        public string TelCont { get; set; }

        [Log("Telefone Celular")]
        [PersistenceProperty("TELCEL")]
        public string TelCel { get; set; }

        [Log("Ramal")]
        [PersistenceProperty("RAMAL")]
        public string Ramal { get; set; }

        [Log("E-mail")]
        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        [PersistenceProperty("LOGIN")]
        public string Login { get; set; }

        [PersistenceProperty("SENHA")]
        public string Senha { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [Log("Data de Entrada")]
        [PersistenceProperty("DATAENT")]
        public DateTime? DataEnt { get; set; }

        [Log("Data de Saída")]
        [PersistenceProperty("DATASAIDA")]
        public DateTime? DataSaida { get; set; }

        [Log("Salário")]
        [PersistenceProperty("SALARIO")]
        public decimal Salario { get; set; }

        [Log("Gratificação")]
        [PersistenceProperty("GRATIFICACAO")]
        public decimal Gratificacao { get; set; }

        [Log("Auxílio Alimentação")]
        [PersistenceProperty("AUXALIMENTACAO")]
        public decimal AuxAlimentacao { get; set; }

        [Log("Comissão")]
        [PersistenceProperty("COMISSAO")]
        public decimal Comissao { get; set; }

        [Log("Registrado")]
        [PersistenceProperty("REGISTRADO")]
        public bool Registrado { get; set; }

        [Log("Núm. Carteira Trabalho")]
        [PersistenceProperty("NUMCARTEIRATRABALHO")]
        public string NumCarteiraTrabalho { get; set; }

        [Log("Núm. PIS")]
        [PersistenceProperty("NUMPIS")]
        public string NumPis { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Núm. Dias Atrasar Pedido")]
        [PersistenceProperty("NUMDIASATRASARPEDIDO")]
        public int NumDiasAtrasarPedido { get; set; }

        [PersistenceProperty("ADMINSYNC", DirectionParameter.Input)]
        public bool AdminSync { get; set; }

        [PersistenceProperty("TipoPedido")]
        public string TipoPedido { get; set; }

        [PersistenceProperty("ENVIAREMAIL")]
        public bool EnviarEmail { get; set; }

        [PersistenceProperty("NumeroPdv")]
        public int NumeroPdv { get; set; }

        [PersistenceProperty("HabilitarChat")]
        public bool HabilitarChat { get; set; }

        [PersistenceProperty("HABILITARCONTROLEUSUARIOS")]
        public bool HabilitarControleUsuarios { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DescrTipoFunc", DirectionParameter.InputOptional)]
        public string DescrTipoFunc { get; set; }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("NovasMensagens", DirectionParameter.InputOptional)]
        public long NovasMensagens { get; set; }

        [PersistenceProperty("SETORESFUNC", DirectionParameter.InputOptional)]
        public string SetoresFunc { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                return Colosoft.Translator.Translate(Situacao).Format();
            }
        }

        public string DescrEndereco
        {
            get
            {
                return Endereco + ", " + Bairro + "(" + Compl + ") - " + Cidade + "/" + Uf + " " + Cep;
            }
        }

        public byte[] Imagem
        {
            get { return Utils.GetImageFromFile(Utils.GetFuncionariosPath + IdFunc + ".jpg"); }
            set { ManipulacaoImagem.SalvarImagem(Utils.GetFuncionariosPath + IdFunc + ".jpg", value); }
        }

        public string ImagemUrl
        {
            get
            {
                string aux = Utils.GetFuncionariosVirtualPath + IdFunc + ".jpg";
                return Utils.GetFuncionariosVirtualPath + IdFunc + ".jpg";
            }
        }

        public bool TemImagem
        {
            get { return Imagem.Length > 0; }
        }

        public bool DeleteVisible
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Administrador; }
        }

        #endregion
    }
}
