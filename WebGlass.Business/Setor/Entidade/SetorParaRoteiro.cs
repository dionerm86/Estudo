namespace WebGlass.Business.Setor.Entidade
{
    public class SetorParaRoteiro
    {
        private Glass.Data.Model.Setor _setor;

        #region Construtores

        internal SetorParaRoteiro(Glass.Data.Model.Setor model)
        {
            _setor = model;
        }

        #endregion

        public int Codigo
        {
            get { return _setor.IdSetor; }
        }

        public string Descricao
        {
            get { return _setor.Descricao; }
        }

        public bool Beneficiamento
        {
            get { return _setor.Tipo == Glass.Data.Model.TipoSetor.PorBenef; }
        }
    }
}
