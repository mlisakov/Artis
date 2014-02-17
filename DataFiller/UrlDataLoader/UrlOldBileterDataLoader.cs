using System;
using System.Threading.Tasks;
using Artis.Consts;

namespace Artis.DataLoader
{
    public class UrlOldBileterDataLoader:IUrlDataLoader
    {
        public event ActionWebLoaded ActionLoadedEvent;
        public event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;
        public event WorkDone WorkDoneEvent;

        public Task LoadData(DateTime start, DateTime finish)
        {
            InvokeWorkDoneEvent();
            return null;
        }

        public void CancelLoadData()
        {
        }

        private void InvokeWorkDoneEvent()
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(UrlActionLoadingSource.OldBileter);
        }
    }
}
