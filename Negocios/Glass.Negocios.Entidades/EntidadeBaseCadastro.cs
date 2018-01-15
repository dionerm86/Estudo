using System;

namespace Glass.Negocios.Entidades
{
    /// <summary>
    /// Implementação base do cadastro das entidades.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class EntidadeBaseCadastro<TModel> : Colosoft.Business.Entity<TModel> where TModel : Glass.Data.Model.ModelBaseCadastro, new()
    {
        #region Propriedades

        /// <summary>
        /// Data de cadastro da entidade.
        /// </summary>
        public DateTime DataCadastro
        {
            get { return DataModel.DataCad; }
            set
            {
                if (DataModel.DataCad != value &&
                    RaisePropertyChanging("DataCadastro", value))
                {
                    DataModel.DataCad = value;
                    RaisePropertyChanged("DataCadastro", "DatCad");
                }
            }
        }

        /// <summary>
        /// Identificador do usuário que cadastrou a entidade..
        /// </summary>
        public int IdUsuarioCadastro
        {
            get { return (int)DataModel.Usucad; }
            set
            {
                if (DataModel.Usucad != (uint)value &&
                    RaisePropertyChanging("IdUsuarioCadastro", value))
                {
                    DataModel.Usucad = (uint)value;
                    RaisePropertyChanged("IdUsuarioCadastro", "Usucad");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Cria a instancia com os dados da model.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public EntidadeBaseCadastro(TModel dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager = null)
            : base(dataModel, uiContext, entityTypeManager)
        {
            if (!DataModel.ExistsInStorage)
            {
                var usuario = Glass.Data.Helper.UserInfo.GetUserInfo;
                DataModel.DataCad = DateTime.Now;
                if (usuario != null)
                    DataModel.Usucad = usuario.CodUser;
            }
        }

        /// <summary>
        /// Construtor interno.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="initialize"></param>
        /// <param name="entityTypeManager"></param>
        protected EntidadeBaseCadastro(TModel dataModel, string uiContext, bool initialize, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, initialize, entityTypeManager)
        {
            if (!DataModel.ExistsInStorage)
            {
                var usuario = Glass.Data.Helper.UserInfo.GetUserInfo;
                DataModel.DataCad = DateTime.Now;
                if (usuario != null)
                    DataModel.Usucad = usuario.CodUser;
            }
        }

        #endregion
    }
}
