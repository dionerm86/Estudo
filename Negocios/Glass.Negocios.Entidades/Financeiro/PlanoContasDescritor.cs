using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa o descritor dos dados do plano de contas.
    /// </summary>
    public class PlanoContasDescritor : Colosoft.Business.BusinessEntityDescriptor
    {
        #region Variáveis Locais

        private string _grupo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Descrição do grupo.
        /// </summary>
        public string Grupo
        {
            get { return _grupo; }
            set
            {
                if (_grupo != value)
                {
                    _grupo = value;
                    RaisePropertyChanged("Grupo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        public PlanoContasDescritor(Colosoft.Business.CreateEntityDescriptorArgs args)
            : base(args)
        {
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera os valores adicionais.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override IEnumerable<string> Bind(Colosoft.Query.IRecord record, Colosoft.Query.BindStrategyMode mode)
        {
            if (record.Descriptor.Contains("Grupo"))
            {
                string grupo = record["Grupo"];

                if (mode == Colosoft.Query.BindStrategyMode.All ||
                    (grupo != Grupo))
                {
                    Grupo = grupo;
                    yield return "Grupo";
                }
            }

            foreach (var i in base.Bind(record, mode))
                yield return i;
        }

        #endregion

    }
}
