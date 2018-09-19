using Colosoft;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados do funcionário.
    /// </summary>
    public interface IProvedorFuncionario
    {
        /// <summary>
        /// Valida a atualização dos dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Funcionario funcionario);

        /// <summary>
        /// Valida a existencia do dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Funcionario funcionario);

        /// <summary>
        /// Obtém os menus que esta empresa tem acesso
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        IList<Menu> ObterMenusEmpresa(int idLoja);
    }

    /// <summary>
    /// Representa a entidade de negócio do funcionário.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FuncionarioLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Funcionario, false, false)]
    public class Funcionario : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Funcionario>
    {
        #region Tipos Aninhados

        class FuncionarioLoader : Colosoft.Business.EntityLoader<Funcionario, Data.Model.Funcionario>
        {
            public FuncionarioLoader()
            {
                Configure()
                    .Uid(f => f.IdFunc)
                    .FindName(f => f.Nome)
                    .Child<FuncionarioSetor, Glass.Data.Model.FuncionarioSetor>("Setores", f => f.FuncionarioSetores, f => f.IdFunc)
                    .Link<PCP.Negocios.Entidades.Setor, Glass.Data.Model.Setor, Glass.Data.Model.FuncionarioSetor>
                        ("Setores", "Setores", f => f.Setores, f => f.IdSetor, f => f.IdSetor)
                    .Reference<TipoFuncionario, Glass.Data.Model.TipoFuncionario>("TipoFuncionario", f => f.TipoFuncionario, f => f.IdTipoFunc)
                    .Child<Global.Negocios.Entidades.ConfigMenuFunc, Glass.Data.Model.ConfigMenuFunc>("ConfigsMenuFunc", f => f.ConfigsMenuFunc, f => f.IdFunc)
                    .Log("ConfigsMenuFunc", "Menus")
                    .Child<Global.Negocios.Entidades.ConfigFuncaoFunc, Glass.Data.Model.ConfigFuncaoFunc>("ConfigsFuncaoFunc", f => f.ConfigsFuncaoFunc, f => f.IdFunc)
                    .Log("ConfigsFuncaoFunc", "Funções")
                    .Creator(f => new Funcionario(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigMenuFunc> _configsMenuFunc;
        private Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigFuncaoFunc> _configsFuncaoFunc;
        private Colosoft.Business.IEntityChildrenList<FuncionarioSetor> _funcionarioSetores;
        private Colosoft.Business.IEntityLinksList<PCP.Negocios.Entidades.Setor> _setores;

        #endregion

        #region Propriedades

        public Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigMenuFunc> ConfigsMenuFunc
        {
            get { return _configsMenuFunc; }
        }

        public Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigFuncaoFunc> ConfigsFuncaoFunc
        {
            get { return _configsFuncaoFunc; }
        }

        /// <summary>
        /// Relação dos setores do funcionários.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<FuncionarioSetor> FuncionarioSetores
        {
            get { return _funcionarioSetores; }
        }

        /// <summary>
        /// Setores associados.
        /// </summary>
        public Colosoft.Business.IEntityLinksList<PCP.Negocios.Entidades.Setor> Setores
        {
            get { return _setores; }
        }

        /// <summary>
        /// Código do funcionário no sistema.
        /// </summary>
        public int IdFunc
        {
            get { return DataModel.IdFunc; }
            set
            {
                if (DataModel.IdFunc != value &&
                    RaisePropertyChanging("IdFunc", value))
                {
                    DataModel.IdFunc = value;
                    RaisePropertyChanged("IdFunc");
                }
            }
        }

        /// <summary>
        /// Código da loja a que pertence o funcionário.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de funcionario.
        /// </summary>
        public int IdTipoFunc
        {
            get { return DataModel.IdTipoFunc; }
            set
            {
                if (DataModel.IdTipoFunc != value &&
                    RaisePropertyChanging("IdTipoFunc", value))
                {
                    // Se qualquer um que não seja Administrador tentar mudar o cargo de um administrador para qualquer outro, impede.
                    if (Glass.Data.Helper.UserInfo.GetUserInfo.TipoUsuario != (uint)Glass.Data.Helper.Utils.TipoFuncionario.Administrador &&
                        !Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync &&
                        DataModel.IdTipoFunc == (int)Glass.Data.Helper.Utils.TipoFuncionario.Administrador)
                        return;

                    DataModel.IdTipoFunc = value;
                    RaisePropertyChanged("IdTipoFunc");
                }
            }
        }

        /// <summary>
        /// Nome do funcionário.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        /// <summary>
        /// Função exercida pelo funcionário.
        /// </summary>
        public string Funcao
        {
            get { return DataModel.Funcao; }
            set
            {
                if (DataModel.Funcao != value &&
                    RaisePropertyChanging("Funcao", value))
                {
                    DataModel.Funcao = value;
                    RaisePropertyChanged("Funcao");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço do funcionário.
        /// </summary>
        public string Endereco
        {
            get { return DataModel.Endereco; }
            set
            {
                if (DataModel.Endereco != value &&
                    RaisePropertyChanging("Endereco", value))
                {
                    DataModel.Endereco = value;
                    RaisePropertyChanged("Endereco");
                }
            }
        }

        /// <summary>
        /// Complemento no endereço do funcionário.
        /// </summary>
        public string Compl
        {
            get { return DataModel.Compl; }
            set
            {
                if (DataModel.Compl != value &&
                    RaisePropertyChanging("Compl", value))
                {
                    DataModel.Compl = value;
                    RaisePropertyChanged("Compl");
                }
            }
        }

        /// <summary>
        /// Bairro no endereço do funcionário.
        /// </summary>
        public string Bairro
        {
            get { return DataModel.Bairro; }
            set
            {
                if (DataModel.Bairro != value &&
                    RaisePropertyChanging("Bairro", value))
                {
                    DataModel.Bairro = value;
                    RaisePropertyChanged("Bairro");
                }
            }
        }

        /// <summary>
        /// Nome da cidade no endereço do funcionário.
        /// </summary>
        public string Cidade
        {
            get { return DataModel.Cidade; }
            set
            {
                if (DataModel.Cidade != value &&
                    RaisePropertyChanging("Cidade", value))
                {
                    DataModel.Cidade = value;
                    RaisePropertyChanged("Cidade");
                }
            }
        }

        /// <summary>
        /// Sigla da UF no endereço do funcionário.
        /// </summary>
        public string Uf
        {
            get { return DataModel.Uf; }
            set
            {
                if (DataModel.Uf != value &&
                    RaisePropertyChanging("Uf", value))
                {
                    DataModel.Uf = value;
                    RaisePropertyChanged("Uf");
                }
            }
        }

        /// <summary>
        /// CEP no endereço do funcionário.
        /// </summary>
        public string Cep
        {
            get { return DataModel.Cep; }
            set
            {
                if (DataModel.Cep != value &&
                    RaisePropertyChanging("Cep", value))
                {
                    DataModel.Cep = value;
                    RaisePropertyChanged("Cep");
                }
            }
        }

        /// <summary>
        /// CPF do funcionário.
        /// </summary>
        public string Cpf
        {
            get { return DataModel.Cpf; }
            set
            {
                if (DataModel.Cpf != value &&
                    RaisePropertyChanging("Cpf", value))
                {
                    DataModel.Cpf = value;
                    RaisePropertyChanged("Cpf");
                }
            }
        }

        /// <summary>
        /// RG do funcionário.
        /// </summary>
        public string Rg
        {
            get { return DataModel.Rg; }
            set
            {
                if (DataModel.Rg != value &&
                    RaisePropertyChanging("Rg", value))
                {
                    DataModel.Rg = value;
                    RaisePropertyChanged("Rg");
                }
            }
        }

        /// <summary>
        /// Data de nascimento do funcionário.
        /// </summary>
        public DateTime? DataNasc
        {
            get { return DataModel.DataNasc; }
            set
            {
                if (DataModel.DataNasc != value &&
                    RaisePropertyChanging("DataNasc", value))
                {
                    DataModel.DataNasc = value;
                    RaisePropertyChanged("DataNasc");
                }
            }
        }

        /// <summary>
        /// Estado civil do funcionário.
        /// </summary>
        public string EstCivil
        {
            get { return DataModel.EstCivil; }
            set
            {
                if (DataModel.EstCivil != value &&
                    RaisePropertyChanging("EstCivil", value))
                {
                    DataModel.EstCivil = value;
                    RaisePropertyChanged("EstCivil");
                }
            }
        }

        /// <summary>
        /// Telefone residencial do funcionário.
        /// </summary>
        public string TelRes
        {
            get { return DataModel.TelRes; }
            set
            {
                if (DataModel.TelRes != value &&
                    RaisePropertyChanging("TelRes", value))
                {
                    DataModel.TelRes = value;
                    RaisePropertyChanged("TelRes");
                }
            }
        }

        /// <summary>
        /// Telefone de contato do funcionário.
        /// </summary>
        public string TelCont
        {
            get { return DataModel.TelCont; }
            set
            {
                if (DataModel.TelCont != value &&
                    RaisePropertyChanging("TelCont", value))
                {
                    DataModel.TelCont = value;
                    RaisePropertyChanged("TelCont");
                }
            }
        }

        /// <summary>
        /// Telefone celular do funcionário.
        /// </summary>
        public string TelCel
        {
            get { return DataModel.TelCel; }
            set
            {
                if (DataModel.TelCel != value &&
                    RaisePropertyChanging("TelCel", value))
                {
                    DataModel.TelCel = value;
                    RaisePropertyChanged("TelCel");
                }
            }
        }

        /// <summary>
        /// Ramal do funcionário.
        /// </summary>
        public string Ramal
        {
            get { return DataModel.Ramal; }
            set
            {
                if (DataModel.Ramal != value &&
                    RaisePropertyChanging("Ramal", value))
                {
                    DataModel.Ramal = value;
                    RaisePropertyChanged("Ramal");
                }
            }
        }

        /// <summary>
        /// E-mail do funcionário.
        /// </summary>
        public string Email
        {
            get { return DataModel.Email; }
            set
            {
                if (DataModel.Email != value &&
                    RaisePropertyChanging("Email", value))
                {
                    DataModel.Email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// Login de acesso ao sistema do funcionário.
        /// </summary>
        public string Login
        {
            get { return DataModel.Login; }
            set
            {
                if (DataModel.Login != value &&
                    RaisePropertyChanging("Login", value))
                {
                    DataModel.Login = value;
                    RaisePropertyChanged("Login");
                }
            }
        }

        /// <summary>
        /// Senha de acesso ao sistema do funcionário.
        /// </summary>
        public string Senha
        {
            get { return DataModel.Senha; }
            set
            {
                if (DataModel.Senha != value &&
                    RaisePropertyChanging("Senha", value))
                {
                    DataModel.Senha = value;
                    RaisePropertyChanged("Senha");
                }
            }
        }

        /// <summary>
        /// Situação do funcionário.
        /// </summary>
        public Situacao Situacao
        {
            get { return (Situacao)DataModel.Situacao; }
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
        /// Data de entrada na empresa do funcionário.
        /// </summary>
        public DateTime? DataEnt
        {
            get { return DataModel.DataEnt; }
            set
            {
                if (DataModel.DataEnt != value &&
                    RaisePropertyChanging("DataEnt", value))
                {
                    DataModel.DataEnt = value;
                    RaisePropertyChanged("DataEnt");
                }
            }
        }

        /// <summary>
        /// Data de saída da empresa do funcionário.
        /// </summary>
        public DateTime? DataSaida
        {
            get { return DataModel.DataSaida; }
            set
            {
                if (DataModel.DataSaida != value &&
                    RaisePropertyChanging("DataSaida", value))
                {
                    DataModel.DataSaida = value;
                    RaisePropertyChanged("DataSaida");
                }
            }
        }

        /// <summary>
        /// Valor do salário do funcionário.
        /// </summary>
        public decimal Salario
        {
            get { return DataModel.Salario; }
            set
            {
                if (DataModel.Salario != value &&
                    RaisePropertyChanging("Salario", value))
                {
                    DataModel.Salario = value;
                    RaisePropertyChanged("Salario");
                }
            }
        }

        /// <summary>
        /// Valor da gratificação ao funcionário.
        /// </summary>
        public decimal Gratificacao
        {
            get { return DataModel.Gratificacao; }
            set
            {
                if (DataModel.Gratificacao != value &&
                    RaisePropertyChanging("Gratificacao", value))
                {
                    DataModel.Gratificacao = value;
                    RaisePropertyChanged("Gratificacao");
                }
            }
        }

        /// <summary>
        /// Valor do auxílio alimentação ao funcionário.
        /// </summary>
        public decimal AuxAlimentacao
        {
            get { return DataModel.AuxAlimentacao; }
            set
            {
                if (DataModel.AuxAlimentacao != value &&
                    RaisePropertyChanging("AuxAlimentacao", value))
                {
                    DataModel.AuxAlimentacao = value;
                    RaisePropertyChanged("AuxAlimentacao");
                }
            }
        }

        /// <summary>
        /// Valor da comissão ao funcionário.
        /// </summary>
        public decimal Comissao
        {
            get { return DataModel.Comissao; }
            set
            {
                if (DataModel.Comissao != value &&
                    RaisePropertyChanging("Comissao", value))
                {
                    DataModel.Comissao = value;
                    RaisePropertyChanged("Comissao");
                }
            }
        }

        /// <summary>
        /// Funcionário está registrado?.
        /// </summary>
        public bool Registrado
        {
            get { return DataModel.Registrado; }
            set
            {
                if (DataModel.Registrado != value &&
                    RaisePropertyChanging("Registrado", value))
                {
                    DataModel.Registrado = value;
                    RaisePropertyChanged("Registrado");
                }
            }
        }

        /// <summary>
        /// Número da CTPS do funcionário.
        /// </summary>
        public string NumCarteiraTrabalho
        {
            get { return DataModel.NumCarteiraTrabalho; }
            set
            {
                if (DataModel.NumCarteiraTrabalho != value &&
                    RaisePropertyChanging("NumCarteiraTrabalho", value))
                {
                    DataModel.NumCarteiraTrabalho = value;
                    RaisePropertyChanged("NumCarteiraTrabalho");
                }
            }
        }

        /// <summary>
        /// Número de PIS do funcionário.
        /// </summary>
        public string NumPis
        {
            get { return DataModel.NumPis; }
            set
            {
                if (DataModel.NumPis != value &&
                    RaisePropertyChanging("NumPis", value))
                {
                    DataModel.NumPis = value;
                    RaisePropertyChanged("NumPis");
                }
            }
        }

        /// <summary>
        /// Observações sobre o funcionário.
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
        /// Número de dias que o pedido deve ser atrasado.
        /// (Utilizado para uma função no sistema que permite que
        /// os pedidos sejam expedidos hoje para uma data retroativa)
        /// </summary>
        public int NumDiasAtrasarPedido
        {
            get { return DataModel.NumDiasAtrasarPedido; }
            set
            {
                if (DataModel.NumDiasAtrasarPedido != value &&
                    RaisePropertyChanging("NumDiasAtrasarPedido", value))
                {
                    DataModel.NumDiasAtrasarPedido = value;
                    RaisePropertyChanged("NumDiasAtrasarPedido");
                }
            }
        }

        /// <summary>
        /// Funcionário é Adminstrador do Sistema (Sync)?
        /// </summary>
        public bool AdminSync
        {
            get { return DataModel.AdminSync; }
            set
            {
                if (DataModel.AdminSync != value &&
                    RaisePropertyChanging("AdminSync", value))
                {
                    DataModel.AdminSync = value;
                    RaisePropertyChanged("AdminSync");
                }
            }
        }

        /// <summary>
        /// Tipos de Pedido que o funcionário pode emitir.
        /// </summary>
        public string TipoPedido
        {
            get { return DataModel.TipoPedido; }
            set
            {
                if (DataModel.TipoPedido != value &&
                    RaisePropertyChanging("TipoPedido", value))
                {
                    DataModel.TipoPedido = value;
                    RaisePropertyChanged("TipoPedido");
                }
            }
        }

        /// <summary>
        /// Enviar e-mail ao finalizar pedido PCP?
        /// </summary>
        public bool EnviarEmail
        {
            get { return DataModel.EnviarEmail; }
            set
            {
                if (DataModel.EnviarEmail != value &&
                    RaisePropertyChanging("EnviarEmail", value))
                {
                    DataModel.EnviarEmail = value;
                    RaisePropertyChanged("EnviarEmail");
                }
            }
        }

        /// <summary>
        /// Numero do PDV.
        /// </summary>
        public int NumeroPdv
        {
            get { return DataModel.NumeroPdv; }
            set
            {
                if (DataModel.NumeroPdv != value &&
                    RaisePropertyChanging("NumeroPdv", value))
                {
                    DataModel.NumeroPdv = value;
                    RaisePropertyChanged("NumeroPdv");
                }
            }
        }

        /// <summary>
        /// Habilitar chat.
        /// </summary>
        public bool HabilitarChat
        {
            get { return DataModel.HabilitarChat; }
            set
            {
                if (DataModel.HabilitarChat != value &&
                    RaisePropertyChanging("HabilitarChat", value))
                {
                    DataModel.HabilitarChat = value;
                    RaisePropertyChanged("HabilitarChat");
                }
            }
        }

        /// <summary>
        /// Habilitar Controle de usuários.
        /// </summary>
        public bool HabilitarControleUsuarios
        {
            get { return DataModel.HabilitarControleUsuarios; }
            set
            {
                if (DataModel.HabilitarControleUsuarios != value &&
                    RaisePropertyChanging("HabilitarControleUsuarios", value))
                {
                    DataModel.HabilitarControleUsuarios = value;
                    RaisePropertyChanged("HabilitarControleUsuarios");
                }
            }
        }

        /// <summary>
        /// Identifica se o funcionário é vendedor.
        /// </summary>
        public bool Vendedor
        {
            get
            {
                return ConfigsFuncaoFunc.Any(f => f.IdFuncaoMenu == Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido));
            }
        }

        /// <summary>
        /// Tipo de Funcionário
        /// </summary>
        public TipoFuncionario TipoFuncionario
        {
            get { return GetReference<TipoFuncionario>("TipoFuncionario", true); }
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Inicializa os listeners da classe.
        /// </summary>
        protected void ConfigureListeners()
        {
            this.PropertyChanged += FuncionarioPropertyChanged;
        }

        private void FuncionarioPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IdTipoFunc":
                    // Atualiza os menus e funções que o funcionário tem acesso
                    AtualizarMenuseFuncoes();
                    break;
            }
        }

        /// <summary>
        /// Recupera os menus e funções do idTipoFunc modificado e salva neste funcioário
        /// </summary>
        private void AtualizarMenuseFuncoes()
        {
            // Pega os idsMenu e idsFuncaoMenu do novo tipo de funcionário
            var idsMenu = TipoFuncionario.ConfigsMenuTipoFunc.OrderBy(f => f.IdMenu).Select(f => f.IdMenu).ToList();
            var idsFuncaoMenu = TipoFuncionario.ConfigsFuncaoTipoFunc.Select(f => f.IdFuncaoMenu).ToList();

            // Pega os menus que a empresa tem acesso, para que altere apenas estes (evita adicionar e remover menus que o tipo funcionário tenha mas a empresa não tenha)
            var menusEmpresa = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IProvedorFuncionario>().ObterMenusEmpresa(IdLoja);

            // Limpa os menus e funções que o mesmo tem acesso (Tem que fazer desta forma ao invés de usar o .Clear() para que salve o log corretamente)
            foreach (var configMenu in ConfigsMenuFunc.ToList().OrderBy(f => f.IdMenu))
                if (!idsMenu.Contains(configMenu.IdMenu) && menusEmpresa.Any(f => f.IdMenu == configMenu.IdMenu))
                    ConfigsMenuFunc.Remove(configMenu);

            foreach (var configFuncao in ConfigsFuncaoFunc.ToList().OrderBy(f => f.IdFuncaoMenu))
                if (!idsFuncaoMenu.Contains(configFuncao.IdFuncaoMenu) && menusEmpresa.Any(f => configFuncao.FuncaoMenu != null && f.IdMenu == configFuncao.FuncaoMenu.IdMenu))
                    ConfigsFuncaoFunc.Remove(configFuncao);

            // Adiciona menus e funções associados
            foreach (var id in idsMenu)
                if (!ConfigsMenuFunc.Any(f => f.IdMenu == id) && menusEmpresa.Any(f => f.IdMenu == id))
                    ConfigsMenuFunc.Add(new ConfigMenuFunc() { IdFunc = IdFunc, IdMenu = id });

            foreach (var id in idsFuncaoMenu)
                if (!ConfigsFuncaoFunc.Any(f => f.IdFuncaoMenu == id))
                    ConfigsFuncaoFunc.Add(new ConfigFuncaoFunc() { IdFunc = IdFunc, IdFuncaoMenu = id });
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Funcionario()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Funcionario(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Funcionario> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _funcionarioSetores = GetChild<FuncionarioSetor>(args.Children, "Setores");
            _setores = GetLink<PCP.Negocios.Entidades.Setor>(args.Links, "Setores");
            _configsMenuFunc = GetChild<ConfigMenuFunc>(args.Children, "ConfigsMenuFunc");
            _configsFuncaoFunc = GetChild<ConfigFuncaoFunc>(args.Children, "ConfigsFuncaoFunc");
            ConfigureListeners();
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Funcionario(Data.Model.Funcionario dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _funcionarioSetores = CreateChild<Colosoft.Business.IEntityChildrenList<FuncionarioSetor>>("Setores");
            _setores = CreateLink<Colosoft.Business.IEntityLinksList<PCP.Negocios.Entidades.Setor>>("Setores");
            _configsMenuFunc = CreateChild<Colosoft.Business.IEntityChildrenList<ConfigMenuFunc>>("ConfigsMenuFunc");
            _configsFuncaoFunc = CreateChild<Colosoft.Business.IEntityChildrenList<ConfigFuncaoFunc>>("ConfigsFuncaoFunc");
            ConfigureListeners();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados do funcionário.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (!String.IsNullOrEmpty(EstCivil))
            {
                // Faz com que o estado civil seja salvo sempre com a primeira letra maiúscula e todas as outras minúsculas.
                var estadoCivil = EstCivil.Substring(1, EstCivil.Length - 1);
                EstCivil = EstCivil.Substring(0, 1).ToUpper() + estadoCivil.ToLower();
            }

            // Se qualquer um que não seja Administrador tentar alterar qualquer funcionário para administrador, impede
            if (Glass.Data.Helper.UserInfo.GetUserInfo.TipoUsuario != (uint)Glass.Data.Helper.Utils.TipoFuncionario.Administrador &&
                !Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync &&
                IdTipoFunc == (int)Glass.Data.Helper.Utils.TipoFuncionario.Administrador)
                    return new Colosoft.Business.SaveResult(false,
                        "O usuário logado não possui permissão para alterar o tipo de funcionário para administrador".GetFormatter());

            if (IdLoja == 0)
                return new Colosoft.Business.SaveResult(false, "Selecione a loja do funcionário.".GetFormatter());

            // Verifica se o funcionário não é mais um marcador de produção
            if (IdTipoFunc != (uint)Glass.Data.Helper.Utils.TipoFuncionario.MarcadorProducao)
                // Limpa os setores associados
                FuncionarioSetores.Clear();

            if (!ExistsInStorage)
            {
                AtualizarMenuseFuncoes();
                TipoPedido = "1,2";
            }

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IProvedorFuncionario>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        /// <summary>
        /// Apaga os dados do funcionário.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IProvedorFuncionario>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
