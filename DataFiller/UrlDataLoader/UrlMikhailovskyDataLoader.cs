using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Artis.Consts;

namespace Artis.DataLoader
{
    public class UrlMikhailovskyDataLoader:IUrlDataLoader
    {
        CancellationTokenSource _cancelToken;
        public event ActionWebLoaded ActionLoadedEvent;
        public event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;
        public event WorkDone WorkDoneEvent;

        public async Task LoadData(DateTime start, DateTime finish)
        {
            InvokeWorkDoneEvent();
        }

        public void CancelLoadData()
        {
             _cancelToken.Cancel();
        }

         private void InvokeWorkDoneEvent()
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(UrlActionLoadingSource.Mikhailovsky);
        }

        private void InvokeUrlDataLoaderExceptionThrownEvent(string text)
        {
            UrlDataLoaderExceptionThrown handler = UrlDataLoaderExceptionThrownEvent;
            if (handler != null)
                handler(text);
        }

        private void InvokeActionLoadedEvent(UrlActionLoadingSource source, ActionWeb action)
        {
            ActionWebLoaded handler = ActionLoadedEvent;
            if (handler != null)
                handler(source, action);
        }
    }
}
