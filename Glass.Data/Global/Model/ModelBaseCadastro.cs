using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    public abstract class ModelBaseCadastro : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public virtual DateTime DataCad { get; set; }

        [PersistenceProperty("USUCAD", DirectionParameter.OutputOnlyInsert)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public uint Usucad { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _DescrUsuCad;

        [PersistenceProperty("DescrUsuCad", DirectionParameter.InputOptional)]
        public virtual string DescrUsuCad
        {
            get 
            {
                try
                {
                    return !String.IsNullOrEmpty(_DescrUsuCad) ? BibliotecaTexto.GetTwoFirstNames(_DescrUsuCad) :
                        BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(Usucad));
                }
                catch
                {
                    return _DescrUsuCad;
                }
            }
            set { _DescrUsuCad = value; }
        }

        #endregion
    }
}