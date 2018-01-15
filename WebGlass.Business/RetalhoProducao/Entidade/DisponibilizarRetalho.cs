namespace WebGlass.Business.RetalhoProducao.Entidade
{
    public class DisponibilizarRetalho
    {
        private Glass.Data.Model.RetalhoProducao _retalho;

        #region Construtores

        public DisponibilizarRetalho()
            : this(new Glass.Data.Model.RetalhoProducao())
        {
        }

        internal DisponibilizarRetalho(Glass.Data.Model.RetalhoProducao model)
        {
            _retalho = model;
        }

        #endregion

        public uint CodigoRetalho
        {
            get { return _retalho.IdRetalhoProducao; }
        }

        public string CodigoProduto
        {
            get { return _retalho.CodInterno; }
        }

        public string DescricaoProduto
        {
            get { return _retalho.Descricao; }
        }

        public string Situacao
        {
            get { return _retalho.DescricaoRetalho; }
        }

        public int Altura
        {
            get { return _retalho.Altura; }
        }

        public int Largura
        {
            get { return _retalho.Largura; }
        }

        public float TotalM2
        {
            get { return _retalho.TotM; }
        }

        public string NumeroEtiqueta
        {
            get { return _retalho.NumeroEtiqueta; }
        }
    }
}
