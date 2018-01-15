﻿using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FotosLiberacaoDAO))]
    [PersistenceClass("fotos_liberacao")]
    public class FotosLiberacao : IFoto
    {
        #region Propriedades

        [PersistenceProperty("IDFOTO", PersistenceParameterType.IdentityKey)]
        public override uint IdFoto { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint IdLiberarPedido { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public override string Descricao { get; set; }

        [PersistenceProperty("EXTENSAO")]
        public override string Extensao { get; set; }

        #endregion

        #region IFoto Members

        public override uint IdParent
        {
            get { return IdLiberarPedido; }
            set { IdLiberarPedido = value; }
        }

        public override IFoto.TipoFoto Tipo
        {
            get { return TipoFoto.Liberacao; }
        }

        public override string CodInterno
        {
            get { return null; }
            set { }
        }

        public override float AreaQuadrada
        {
            get { return 0; }
            set { }
        }

        public override float MetroLinear
        {
            get { return 0; }
            set { }
        }

        public override bool PermiteCalcularArea
        {
            get { return false; }
        }

        public override bool ApenasImagens
        {
            get { return false; }
        }

        #endregion
    }
}