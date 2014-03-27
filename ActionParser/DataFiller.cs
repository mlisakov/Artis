using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Data;
using Artis.DataLoader;

namespace Artis.ActionParser
{
    public class DataFiller
    {
        private WcfServiceCaller _wcfAdminService; 
        private int _actionLoadersCompletedCount;

        /// <summary>
        /// Список классов для загрузки данных
        /// </summary>
        private List<IUrlDataLoader> _urlDataLoaders;

        /// <summary>
        /// Класс для записи информации о мероприятия в БД
        /// </summary>
        //private DataParser _dataParser;

        public delegate void ActionLoaded(ActionWeb action);
        public delegate void WorkDone(UrlActionLoadingSource source);
        public delegate void FatalError(string message);

        public event ActionLoaded ActionLoadedEvent;
        public event ActionLoaded ActionNotLoadedEvent;
        public event ActionWebLoaded ActionWebLoadedEvent;
        public event WorkDone WorkDoneEvent;
        public event FatalError FatalErrorEvent;

        public DataFiller()
        {
            _wcfAdminService=new WcfServiceCaller();
            _actionLoadersCompletedCount = 0;
            _urlDataLoaders=new List<IUrlDataLoader>(){new UrlBileterDataLoader(),new UrlMariinskyDataLoader(),new UrlMikhailovskyDataLoader()};
            //_urlDataLoaders = new List<IUrlDataLoader>() { new UrlMariinskyDataLoader(),new UrlMikhailovskyDataLoader() };
            //_dataParser=new DataParser();
            HandleEvents();
        }


        //~ActionParser()
        //{
        //    UnHandleLoadersEvents();
        //}

        /// <summary>
        /// Загрузка данных по меропритяием за период
        /// </summary>
        /// <param name="start">Начальная дата фильтра по дате проведения мероприятия</param>
        /// <param name="finish">Конечная дата фильтра по дате проведения мероприятия</param>
        /// <returns></returns>
        public async Task ParseActionsAsync(DateTime start, DateTime finish)
        {
            foreach (IUrlDataLoader dataLoader in _urlDataLoaders)
            {
                await dataLoader.LoadData(start, finish);
            }
        }

        /// <summary>
        /// Отмена загрузки информации по мероприятиям
        /// </summary>
        public void CancelParseData()
        {
             foreach (IUrlDataLoader dataLoader in _urlDataLoaders)
            {
                dataLoader.CancelLoadData();
            }
        }

        private void HandleEvents()
        {
            foreach (IUrlDataLoader dataLoader in _urlDataLoaders)
            {
                dataLoader.ActionLoadedEvent += dataLoader_ActionLoadedEvent;
                dataLoader.WorkDoneEvent += dataLoader_WorkDoneEvent;
            }
            //_dataParser.ActionLoadedEvent += DataParserActionLoadedEvent;
            //_dataParser.ActionNotLoadedEvent += DataParserActionNotLoadedEvent;
            //_dataParser.FatalErrorEvent+=_dataParser_FatalErrorEvent;
        }

        private void dataLoader_WorkDoneEvent(UrlActionLoadingSource source)
        {
            _actionLoadersCompletedCount++;
            if (_actionLoadersCompletedCount == _urlDataLoaders.Count)
                InvokeWorkDone(source);
        }

        //void DataParserActionNotLoadedEvent(ActionWeb action)
        //{
        //    InvokeActionNotLoaded(action);
        //}

        //void DataParserActionLoadedEvent(ActionWeb action)
        //{
        //    InvokeActionLoaded(action);
        //}

        //private void _dataParser_FatalErrorEvent(string message)
        //{
        //    //Прекращаем загрузку при возникновении критической ошибки
        //    CancelParseData();
        //    InvokeFatalError(message);
        //}
        private void UnHandleLoadersEvents()
        {
            foreach (IUrlDataLoader dataLoader in _urlDataLoaders)
            {
                dataLoader.ActionLoadedEvent -= dataLoader_ActionLoadedEvent;
                dataLoader.WorkDoneEvent -= dataLoader_WorkDoneEvent;
            }
            //_dataParser.ActionLoadedEvent -= DataParserActionLoadedEvent;
            //_dataParser.ActionNotLoadedEvent -= DataParserActionNotLoadedEvent;
        }

        private void dataLoader_ActionLoadedEvent(UrlActionLoadingSource source,ActionWeb action)
        {
            InvokeActionWebLoaded(source,action);
            ParseDownloadedAction(action);
            //_dataParser.Parse(action);
        }

        private async void ParseDownloadedAction(ActionWeb action)
        {
            int result = await _wcfAdminService.ParseActionAsync(action);
            if (result == 1)
                InvokeActionLoaded(action);
            else if (result == 0)
                InvokeActionNotLoaded(action);
            else
            {
                //Прекращаем загрузку при возникновении критической ошибки
                CancelParseData();
                InvokeFatalError("Ошибка записи загруженного мероприятия!");
            }
        }

        private void InvokeWorkDone(UrlActionLoadingSource source)
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(source);
        }

        private void InvokeActionLoaded(ActionWeb action)
        {
            ActionLoaded handler = ActionLoadedEvent;
            if (handler != null)
                handler(action);
        }
        private void InvokeActionNotLoaded(ActionWeb action)
        {
            ActionLoaded handler = ActionNotLoadedEvent;
            if (handler != null)
                handler(action);
        }
        private void InvokeActionWebLoaded(UrlActionLoadingSource source,ActionWeb action)
        {
            ActionWebLoaded handler = ActionWebLoadedEvent;
            if (handler != null)
                handler(source,action);
        }
        private void InvokeFatalError(string message)
        {
            FatalError handler = FatalErrorEvent;
            if (handler != null)
                handler(message);
        }

        //public delegate void PageLoaded(string url);

        //public delegate void WorkDone(int actionsCount);

        //public delegate void ActionWorkInProgress(string actionName);


        //public event PageLoaded PageLoadedEvent;
        //public event WorkDone WorkDoneEvent;
        //public event ActionWorkInProgress ActionWorkInProgressEvent;

        //private readonly DataLoader.DataLoader _dataLoader;
        //private readonly DataParser _data;

        //public List<ActionWeb> Actions 
        //{
        //    get { return new List<ActionWeb>(_data.Actions); } 
        //}

        //public List<ActionWeb> ActionsNotLoaded
        //{
        //    get { return new List<ActionWeb>(_data.ActionsNotLoaded); }
        //}

        //public int PageCount
        //{
        //    get { return _dataLoader.PageCount; }
        //}

        //public int ActionsNewCount
        //{
        //    get { return _data.ActionNewCount; }
        //}

        //public int AreaNewCount
        //{
        //    get { return _data.AreaNewCount; }
        //}

        //public ActionParser()
        //{
        //    _dataLoader = new DataLoader.DataLoader();
        //    _dataLoader.WorkDoneEvent += DataLoaderWorkDoneEvent;
        //    _dataLoader.PageLoadedEvent += DataLoaderPageLoadedEvent;
        //    _dataLoader.ActionWorkInProgressEvent += DataLoaderActionWorkInProgressEvent;
        //    _data = new DataParser();
        //}

        //private void DataLoaderActionWorkInProgressEvent(string actionname)
        //{
        //    InvokeActionWorkInProgress(actionname);
        //}

        //private void DataLoaderPageLoadedEvent(string pagenumber)
        //{
        //    InvokePageLoaded(pagenumber);
        //}

        //private void DataLoaderWorkDoneEvent(int actionsCount)
        //{
        //    ParseAtions(new List<ActionWeb>(_dataLoader.Actions));
        //}

        //public async Task ParseAtions(IEnumerable<ActionWeb> actions)
        //{
        //    foreach (ActionWeb action in actions)
        //    {
        //        await _data.Parse(action);
        //    }

        //    InvokeWorkDone(actions.Count());
        //}

        //public void ParseActions(DateTime start, DateTime finish)
        //{
        //    _dataLoader.FillData(start, finish);
        //}

        //private void InvokePageLoaded(string pageNumber)
        //{
        //    PageLoaded handler = PageLoadedEvent;
        //    if (handler != null)
        //        handler(pageNumber);
        //}

        //private void InvokeActionWorkInProgress(string actionName)
        //{
        //    ActionWorkInProgress handler = ActionWorkInProgressEvent;
        //    if (handler != null)
        //        handler(actionName);
        //}

        //private void InvokeWorkDone(int eventsCount)
        //{
        //    WorkDone handler = WorkDoneEvent;
        //    if (handler != null)
        //        handler(eventsCount);
        //}
    }
}