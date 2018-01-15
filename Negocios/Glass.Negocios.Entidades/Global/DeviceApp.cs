using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(DeviceAppLoader))]
    public class DeviceApp : Colosoft.Business.Entity<Data.Model.DeviceApp>
    {
        #region Tipos Aninhados

        class DeviceAppLoader : Colosoft.Business.EntityLoader<DeviceApp, Data.Model.DeviceApp>
        {
            public DeviceAppLoader()
            {
                Configure()
                    .Keys(f => f.IdCliente, f => f.Uuid)
                    .Creator(f => new DeviceApp(f));
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public int IdCliente
        {
            get { return DataModel.IdCliente; }
            set
            {
                if (DataModel.IdCliente != value &&
                    RaisePropertyChanging("IdCliente", value))
                {
                    DataModel.IdCliente = value;
                    RaisePropertyChanged("IdCliente");
                }
            }
        }

        /// <summary>
        /// Identificador do aparelho.
        /// </summary>
        public string Uuid
        {
            get { return DataModel.Uuid; }
            set
            {
                if (DataModel.Uuid != value &&
                    RaisePropertyChanging("Uuid", value))
                {
                    DataModel.Uuid = value;
                    RaisePropertyChanged("Uuid");
                }
            }
        }

        /// <summary>
        /// Token.
        /// </summary>
        public string Token
        {
            get { return DataModel.Token; }
            set
            {
                if (DataModel.Token != value &&
                    RaisePropertyChanging("Token", value))
                {
                    DataModel.Token = value;
                    RaisePropertyChanged("Token");
                }
            }
        }

        #endregion

        #region Contrutores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public DeviceApp()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected DeviceApp(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.DeviceApp> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public DeviceApp(Glass.Data.Model.DeviceApp dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
