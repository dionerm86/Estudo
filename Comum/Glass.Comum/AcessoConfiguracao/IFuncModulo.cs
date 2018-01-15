namespace Glass.AcessoConfiguracao
{
    public interface IFuncModulo
    {
        uint[] ObtemIdsModulosPorFuncionario(uint idFunc);

        bool Permissao(uint idFunc, Glass.Seguranca.ModuloIndividual modulo);
    }
}
