namespace Glass.Data.Model
{
    public interface IDadosGrupoSubgrupo
    {
        string DescricaoSubgrupo();
        bool ProdutoDeProducao();
        bool IsVidroTemperado();
        bool ProdutoEAluminio();
        bool ProdutoEVidro();
        TipoSubgrupoProd TipoSubgrupo();
        TipoCalculoGrupoProd TipoCalculo(bool fiscal = false);
    }
}
