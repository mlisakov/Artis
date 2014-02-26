using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Artis.Consts;
using HtmlAgilityPack;
using NLog;

namespace Artis.DataLoader
{
    public class UrlMikhailovskyDataLoader:IUrlDataLoader
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private string _areaDescription;
        private List<string> _areaImages;

        CancellationTokenSource _cancelToken;

        public event ActionWebLoaded ActionLoadedEvent;
        public event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;
        public event WorkDone WorkDoneEvent;

        public UrlMikhailovskyDataLoader()
        {
            _cancelToken = new CancellationTokenSource();
            _areaDescription = string.Empty;
            _areaImages = new List<string>();
        }

        public async Task LoadData(DateTime start, DateTime finish)
        {
            DateTime checkedFinish = finish;
            if (start.AddMonths(3) < finish)
                checkedFinish = start.AddMonths(3);

            await DownloadAreaInfo();

            DateTime currDate = start;
            for (int i = start.Month; i <= checkedFinish.Month; i++)
            {
                currDate = start;
                KeyValuePair<string, HtmlDocument> doc = await DownloadDataFromWebSite(i);

                if (doc.Value.DocumentNode.ChildNodes.Count == 0)
                {
                    _logger.Fatal("Ошибка загрузки данных с URL=" + doc.Key);
                    throw new UrlDataLoaderException("Не удалось загрузить данные!", "Загрузка данных");
                }
                bool isFinished = await ParseHtmlDoc(doc.Key, doc.Value, i, currDate.Year, checkedFinish);
                if (isFinished)
                    break;
                 currDate = start.AddMonths(i);
            }

            InvokeWorkDoneEvent();
        }

        public void CancelLoadData()
        {
             _cancelToken.Cancel();
        }

        private async Task<bool> ParseHtmlDoc(string url,HtmlDocument doc,int month,int year,DateTime finishDate)
        {
            try
            {
                HtmlNodeCollection itemNodeCollection =
                    doc.DocumentNode.SelectNodes("//div[@class='afisha-list']/div[@class='item']");

                foreach (HtmlNode htmlNode in itemNodeCollection)
                {
                    HtmlNode actionDateNode = htmlNode.SelectSingleNode("div[@class='date']/div[@class='day f-ap']");
                    if (actionDateNode == null)
                    {
                        _logger.Error("Mikhailovsky. Не удалось получить дату проведения мероприятия " + Environment.NewLine + htmlNode.InnerHtml);
                        continue;
                    }

                    DateTime actionDate=new DateTime(year,month,int.Parse(actionDateNode.InnerText));
                    if (actionDate > finishDate)
                        return true;

                    // Периодически проверяем, не запрошена ли отмена операции
                    _cancelToken.Token.ThrowIfCancellationRequested();
                    KeyValuePair<bool, ActionWeb> result = await createNode(htmlNode, actionDate);
                    if (result.Key)
                    {
                        if (!string.IsNullOrEmpty(result.Value.AreaName)
                            && !string.IsNullOrEmpty(result.Value.Date)
                            && !string.IsNullOrEmpty(result.Value.Name)
                            && !string.IsNullOrEmpty(result.Value.Time))
                            InvokeActionLoadedEvent(UrlActionLoadingSource.Mikhailovsky, result.Value);
                        else
                        {
                            if (string.IsNullOrEmpty(result.Value.AreaName))
                                _logger.Error("Не удалось распознать площадку для мероприятия");
                            if (string.IsNullOrEmpty(result.Value.Name))
                                _logger.Error("Не удалось распознать имя мероприятия");
                            if (string.IsNullOrEmpty(result.Value.Date))
                                _logger.Error("Не удалось распознать дату проведения мероприятия");
                            if (string.IsNullOrEmpty(result.Value.Time))
                                _logger.Error("Не удалось распознать время проведения мероприятия");
                        }
                    }
                    else
                    {
                        _logger.Error("Mikhailovsky.Не удалось распознать мероприятия со страницы " +
                                      url);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Mikhailovsky.Ошибка распознания данных для мероприятия. Загрузка данных будет продолжена...", ex);
            }
            return false;
        }

        private async Task<KeyValuePair<bool, ActionWeb>> createNode(HtmlNode htmlNode,DateTime date)
        {
            ActionWeb actionWeb = new ActionWeb();
            try
            {
                actionWeb.AreaAddress = "Санкт-Петербург, пл. Искусств, д.1";
                actionWeb.AreaName = "Михайловский театр";
                actionWeb.AreaDescription = _areaDescription;
                actionWeb.AreaImage = _areaImages;
                actionWeb.AreaMetro = "Невский проспект";
                actionWeb.Date = date.ToShortDateString();

                string actionName = string.Empty;

                HtmlNode actionDetailNode = htmlNode.SelectSingleNode("div[@class='detail']");
                if (actionDetailNode != null)
                {
                    HtmlNode actionNameNode = actionDetailNode.SelectSingleNode("h2/a");
                    if (actionNameNode != null)
                    {
                        actionName = HttpUtility.HtmlDecode(actionNameNode.InnerText.Normalize()).Trim();
                    }
                    else
                    {
                        _logger.Error("Mikhailovsky.Не удалось получить название мероприятия."+Environment.NewLine+htmlNode);
                        return new KeyValuePair<bool, ActionWeb>(false, actionWeb);
                    }
                }
                else
                {
                    _logger.Error("Mikhailovsky.Не удалось получить название мероприятия." + Environment.NewLine + htmlNode);
                    return new KeyValuePair<bool, ActionWeb>(false, actionWeb);
                }

                actionWeb.Name = actionName;

                HtmlNode actionTimeNode = htmlNode.SelectSingleNode("div[@class='date']/div[@class='time f-ap']/span");
                if (actionTimeNode != null)
                {
                    actionWeb.Time = HttpUtility.HtmlDecode(actionTimeNode.InnerText.Normalize()).Trim();
                }
                else
                {
                    string warnText = "Mikhailovsky.Не удалось получить время проведения для мероприятия" + actionName + " в Михайловском театре";
                    _logger.Warn(warnText);
                    actionWeb.Time = "00:00";
                }

                string actionLink = actionDetailNode.SelectSingleNode("h2/a").Attributes["href"].Value;
                await UploadActionInfo(actionWeb, actionLink);
                return new KeyValuePair<bool, ActionWeb>(true, actionWeb);
            }
            catch (Exception ex)
            {
                InvokeUrlDataLoaderExceptionThrownEvent("Mikhailovsky.Ошибка распознования HTML-документа для мероприятия");
                _logger.ErrorException(
                    "Mikhailovsky.Ошибка распознования HTML-документа для мероприятия. Загрузка данных будет продолжена...", ex);
                return new KeyValuePair<bool, ActionWeb>(false, actionWeb);
            }
        }

        private async Task UploadActionInfo(ActionWeb actionWeb, string actionUrl)
        {
            HtmlDocument actionInfo = await DownloadDataFromWebSite("http://mikhailovsky.ru"+actionUrl);
            HtmlNode rootNode = actionInfo.DocumentNode;
            HtmlNode infoNode = rootNode.SelectSingleNode("//div[@id='s-description']");
            if (infoNode != null)
            {
                string actionDesc = HttpUtility.HtmlDecode(infoNode.InnerText.Normalize()).Trim();
                if (!string.IsNullOrEmpty(actionDesc))
                    actionWeb.Description += actionDesc;
            }
            else
            {
                _logger.Warn("Mikhailovsky.Михайловский театр. Не удалось получить описание для " + actionWeb.Name);
            }
            HtmlNode contentNode = rootNode.SelectSingleNode("//div[@id='s-content']");
            if (contentNode != null)
            {
                string actionDesc = HttpUtility.HtmlDecode(contentNode.InnerText.Normalize()).Trim();
                if (!string.IsNullOrEmpty(actionDesc))
                    actionWeb.Description += actionDesc;
            }
            else
            {
                _logger.Warn("Mikhailovsky.Михайловский театр. Не удалось получить содержание для " + actionWeb.Name);
            }

            HtmlNode legendNode = rootNode.SelectSingleNode("//div[@class='legend']");
            if (legendNode != null)
            {
                HtmlNode durationNode = legendNode.SelectSingleNode("div[@class='time']");
                if (durationNode != null)
                {
                    string text = HttpUtility.HtmlDecode(durationNode.InnerText.Normalize()).Trim();
                    int Hours = 0;
                    int Minute = 0;
                    string[] splitedString = text.Split(' ');
                    foreach (string item in splitedString)
                    {
                        int intItem;
                        if (int.TryParse(item.Trim(), out intItem))
                        {
                            if (Hours == 0)
                                Hours = intItem;
                            else
                                Minute = intItem;
                        }
                    }
                    if (Hours != 0 && Minute != 0)
                        actionWeb.Duration = Hours + ":" + Minute;
                    else if (Hours != 0)
                        actionWeb.Duration = Hours + ":00";
                }
                else
                {
                    _logger.Warn("Mikhailovsky.Михайловский театр. Не удалось получить продолжительность для " + actionWeb.Name);
                }

                HtmlNode ratingNode = legendNode.SelectSingleNode("div[@class='age']/span");
                if (ratingNode != null)
                {
                    actionWeb.Rating = HttpUtility.HtmlDecode(ratingNode.InnerText.Normalize()).Trim();
                }
                else
                {
                    _logger.Warn("Mikhailovsky.Михайловский театр. Не удалось получить рейтинг для " + actionWeb.Name);
                }
            }

            HtmlNodeCollection imgNode = rootNode.SelectNodes("//div[@class='square-gallery']/div[@id='s-spectacle']/ul/li/a");
            if (imgNode != null)
            {
                List<string> actionImages =new List<string>();
                foreach (HtmlNode aNode in imgNode)
                {
                    string url = aNode.Attributes["href"].Value;
                    if (url.Contains("www.mikhailovsky.ru"))
                        url = "http://" + url.Remove(0, 2);
                    else
                        url = "http://www.mikhailovsky.ru.images.1c-bitrix-cdn.ru" + url;
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            byte[] image = client.DownloadData(url);
                            string base64String = Convert.ToBase64String(image, 0, image.Length);
                            actionImages.Add(base64String);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorException(
                                "Mikhailovsky.Михайловский театр. Не удалось получить изображение для " + actionWeb.Name + Environment.NewLine + aNode.InnerHtml,
                                ex);
                        }

                    }
                }
            }
            else
            {
                _logger.Warn("Mikhailovsky.Михайловский театр. Не удалось получить изображения для " + actionWeb.Name);
            }
        }

        private async Task DownloadAreaInfo()
        {
            string photoResult = await DownloadHtml("http://www.mikhailovsky.ru/media/photo/theatre_today/");
            string descResult = await DownloadHtml("http://www.mikhailovsky.ru/theatre/history/");

            HtmlDocument photoDoc = new HtmlDocument();
            photoDoc.LoadHtml(photoResult);

            HtmlDocument descDoc = new HtmlDocument();
            descDoc.LoadHtml(descResult);

            await ParseAreaInfo(photoDoc, descDoc);
        }

        private async Task ParseAreaInfo(HtmlDocument photoDoc, HtmlDocument descDoc)
        {
            HtmlNodeCollection itemNodeCollection = photoDoc.DocumentNode.SelectNodes("//div[@class='square-gallery']/ul/li/a");
            if (itemNodeCollection != null)
            {
                foreach (HtmlNode imgNode in itemNodeCollection)
                {
                    using (WebClient client = new WebClient())
                    {
                        string url = imgNode.Attributes["href"].Value.Remove(0, 2);
                        int i = url.IndexOf('?');
                        url="http://"+url.Remove(i, url.Length - i);
                        byte[] image = client.DownloadData(url);
                        string base64String = Convert.ToBase64String(image, 0, image.Length);
                        _areaImages.Add(base64String);
                    }
                }
            }
            else
            {
                _logger.Warn("Mikhailovsky.Не удалось загрузить изображения для Михайловского театра");
            }

            HtmlNode descNode = descDoc.DocumentNode.SelectSingleNode("//div[@id='content']/div[@class='i-wrapper']");
            if (descNode != null)
            {
                string desc = HttpUtility.HtmlDecode(descNode.InnerText.Normalize()).Trim();
                if (!string.IsNullOrEmpty(desc))
                    _areaDescription = desc;
            }
            else
            {
                _logger.Warn("Mikhailovsky.Не удалось загрузить описание для Михайловского театра");
            }

        }

        private async Task<KeyValuePair<string, HtmlDocument>> DownloadDataFromWebSite(int month)
        {
            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();

            string url = CretaeUriToDownload(month);
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return new KeyValuePair<string, HtmlDocument>(url, doc);
        }

        private async Task<HtmlDocument> DownloadDataFromWebSite(string url)
        {
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        /// <summary>
        /// Загрузка данных с Url
        /// </summary>
        /// <param name="query">url</param>
        /// <returns>Ответ в виде строки</returns>
        private async Task<string> DownloadHtml(string query)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    //HttpResponseMessage response = await client.GetAsync(query);
                    //string data = await response.Content.ReadAsStringAsync();
                    return await client.GetStringAsync(query);
                }
                catch (Exception ex)
                {
                    throw new UrlDataLoaderException(query, "Ошибка загрузки" + Environment.NewLine + ex.Message);
                }
            }
        }

        private string CretaeUriToDownload(int month)
        {
            if (month > DateTime.Today.Month + 3 || month < DateTime.Today.Month)
                _logger.Warn("Mikhailovsky.Попытка загрузить информацию за " + month +
                             " месяц.Сайт Михайловского сайта позволяет грузить только за три месяца от текущего!");
            string baseUrl = "http://www.mikhailovsky.ru/afisha/performances/" + DateTime.Today.Year + "/" + month +
                             "/#list";
            return baseUrl;
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
