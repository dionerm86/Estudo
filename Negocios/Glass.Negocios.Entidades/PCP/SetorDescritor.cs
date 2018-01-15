using System.Collections.Generic;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Representa o descritor dos dados do setor.
    /// </summary>
    public class SetorDescritor : Colosoft.Business.BusinessEntityDescriptor
    {
        #region Variáveis Locais

        private Glass.Data.Model.TipoSetor _tipoSetor;

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo do setor associado.
        /// </summary>
        public Glass.Data.Model.TipoSetor TipoSetor
        {
            get { return _tipoSetor; }
            set
            {
                if (_tipoSetor != value)
                {
                    _tipoSetor = value;
                    RaisePropertyChanged("TipoSetor");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        public SetorDescritor(Colosoft.Business.CreateEntityDescriptorArgs args)
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
            if (record.Descriptor.Contains("Tipo"))
            {
                var tipoSetor = (Glass.Data.Model.TipoSetor)(int)record["Tipo"];

                if (mode == Colosoft.Query.BindStrategyMode.All ||
                    (tipoSetor != TipoSetor))
                {
                    TipoSetor = tipoSetor;
                    yield return "TipoSetor";
                }
            }

            foreach (var i in base.Bind(record, mode))
                yield return i;
        }

        #endregion
    }
}
