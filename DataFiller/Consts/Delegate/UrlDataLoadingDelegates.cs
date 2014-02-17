using Artis.Consts;

namespace Artis.DataLoader
{

    public delegate void ActionWebLoaded(UrlActionLoadingSource source, ActionWeb action);

    public delegate void UrlDataLoaderExceptionThrown(string text);

    public delegate void WorkDone(UrlActionLoadingSource source);

}
