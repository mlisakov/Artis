using System;
using System.Threading;
using System.Threading.Tasks;

namespace Artis.DataLoader
{
    public interface IUrlDataLoader
    {
        /// <summary>
        /// Событие загрузки страницы с мероприятиями
        /// </summary>
        event ActionWebLoaded ActionLoadedEvent;

        /// <summary>
        /// Событие возникновения НЕ критической ошибки в процессе загрузки данных
        /// </summary>
        event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;

        /// <summary>
        /// Событие окончания загрузки данных
        /// </summary>
        event WorkDone WorkDoneEvent;

        /// <summary>
        /// Загрузка данных по мероприятиям за временной интервал по датам их проведения
        /// </summary>
        /// <param name="start">Начальная дата фильтра по дате проведения мероприятия</param>
        /// <param name="finish">Конечная дата фильтра по дате проведения мероприятия</param>
        /// <returns></returns>
        Task LoadData(DateTime start, DateTime finish);

        /// <summary>
        /// Отмена загрузки данных
        /// </summary>
        void CancelLoadData();
    }
}
