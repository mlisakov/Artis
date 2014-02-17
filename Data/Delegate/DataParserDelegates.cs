using Artis.Consts;

namespace Artis.Data
{
    public delegate void DataLoaderActionLoaded(ActionWeb action);
    public delegate void DataLoaderActionNotLoaded(ActionWeb action);

    public delegate void FatalError(string message);
}
