using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Artis.Consts;
using Artis.Logger;
using HtmlAgilityPack;

namespace Artis.DataLoader
{
    public class UrlMariinskyDataLoader:IUrlDataLoader
    {
        /// <summary>
        /// Путь к файлу логирования
        /// </summary>
        private const string _logPath = "DataFiller.log";

        private const string _mariinskyMainUrl = "http://www.mariinsky.ru/about/history_theatre/mariinsky_theatre/";
        private const string _mariinskySecondHallUrl = "http://www.mariinsky.ru/about/history_theatre/mariinsky_2/";
        private const string _mariinskyConcertHallUrl = "http://www.mariinsky.ru/about/history_theatre/concert_hall/";

        private const string rootUrl = "http://www.mariinsky.ru";

        CancellationTokenSource _cancelToken;

        private MariinskyAreaInfo _mainArea;
        private MariinskyAreaInfo _secondArea;
        private MariinskyAreaInfo _concertHall;

        public event ActionWebLoaded ActionLoadedEvent;
        public event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;
        public event WorkDone WorkDoneEvent;

        public UrlMariinskyDataLoader()
        {
            _cancelToken=new CancellationTokenSource();
        }

        public async Task LoadData(DateTime start, DateTime finish)
        {
            await DownloadAreasInfo();

            DateTime currentDate = start;
            while (currentDate <= finish)
            {
                // Периодически проверяем, не запрошена ли отмена операции
                _cancelToken.Token.ThrowIfCancellationRequested();
                HtmlDocument doc = await DownloadDataFromWebSite(currentDate);
                if (doc.DocumentNode.ChildNodes.Count == 0)
                {
                    throw new UrlDataLoaderException("Не удалось загрузить данный!", "Загрузка данных");
                }

                await ParseHtmlDoc(doc, currentDate);
                currentDate=currentDate.AddDays(1);
            }

            InvokeWorkDoneEvent();
        }

        public void CancelLoadData()
        {
            _cancelToken.Cancel();
        }


        private async Task<HtmlDocument> DownloadDataFromWebSite(DateTime date)
        {
            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();

            string url = CretaeUriToDownload(date.Year, date.Month,date.Day);
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        private async Task<HtmlDocument> DownloadDataFromWebSite(string url)
        {
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        private string CretaeUriToDownload(int year, int month,int day=0)
        {
            string sourceUlr =
                "http://www.mariinsky.ru/ru/playbill/playbill/?year=" + year +
                "&month=" + month;
            if (day > 0)
                sourceUlr += "&day=" + day;
            return sourceUlr;
        }

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

        private async Task DownloadAreasInfo()
        {
            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();
            _mainArea = await DownloadAreaInfo(_mariinskyMainUrl);

            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();
            _secondArea = await DownloadAreaInfo(_mariinskySecondHallUrl);

            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();
            _concertHall = await DownloadAreaInfo(_mariinskyConcertHallUrl);
        }

        private async Task<MariinskyAreaInfo> DownloadAreaInfo(string areaUrl)
        {
            string result = await DownloadHtml(areaUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return await ParseAreaInfo(doc);
        }

        private async Task<MariinskyAreaInfo> ParseAreaInfo(HtmlDocument doc)
        {
            try
            {
                string description = string.Empty;
                List<string> images=new List<string>();

                HtmlNodeCollection descriptionNodeCollection = doc.DocumentNode.SelectNodes("//td[@class='site_content']/table[@class='w_100 h_100']/tr/td[@id='content']/table/tr/td")[1].SelectNodes("p");
                int i=0;
                foreach (HtmlNode descriptioNode in descriptionNodeCollection)
                {
                    if (i != 0)
                        description += Environment.NewLine;
                    description+=HttpUtility.HtmlDecode(descriptioNode.InnerText.Normalize());
                    i++;
                }

                HtmlNodeCollection imageNodeCollection =
                    doc.DocumentNode.SelectNodes(
                        "//table[@class='rounded_foto']/tbody/tr/td/div/a[@class='lightbox']");
                
                foreach (HtmlNode imageNdoe in imageNodeCollection)
                {
                    using (WebClient client = new WebClient())
                    {
                        byte[] image = client.DownloadData(rootUrl + imageNdoe.Attributes["href"].Value);
                        string base64String = Convert.ToBase64String(image, 0, image.Length);
                        images.Add(base64String);
                    }
                }

                return new MariinskyAreaInfo(description, images);
            }
            catch (Exception ex)
            {
                Log.WriteLog(_logPath, ex);
            }
            return null;
        }

        private async Task<bool> ParseHtmlDoc(HtmlDocument doc,DateTime date)
        {
            try
            {
                HtmlNodeCollection dateNode =
                    doc.DocumentNode.SelectNodes("//table[@id='afisha']/tr/td/table[@class='this_day w_100']/tr");
                bool isSuccess = true;
                foreach (HtmlNode htmlNode in dateNode)
                {
                    // Периодически проверяем, не запрошена ли отмена операции
                    _cancelToken.Token.ThrowIfCancellationRequested();
                    bool success = await createNode(htmlNode, date);

                    if (!success)
                        isSuccess = false;
                    else
                    {
                        Log.WriteLog(_logPath, "Node " + htmlNode.InnerHtml + " parse failed!");
                    }
                }
                if (isSuccess)
                    return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(_logPath, ex);
            }
            return false;
        }
        private async Task<bool> createNode(HtmlNode htmlNode,DateTime date)
        {
            try
            {
                ActionWeb actionWeb=new ActionWeb();

                HtmlNode placeNode = htmlNode.SelectSingleNode("td[@class='where_ico']");
                string place = HttpUtility.HtmlDecode(placeNode.InnerText.Normalize());
                List<string> actionImage;
                switch (place)
                {
                    case "Мариинский театр":
                        actionWeb.AreaImage = _mainArea.Images;
                        actionWeb.AreaDescription = _mainArea.Description;
                        actionWeb.AreaAddress = "Санкт-Петербург,Театральная площадь, д. 1";
                        break;
                    case "Мариинский-2 (Новая сцена)":
                        actionWeb.AreaImage = _secondArea.Images;
                        actionWeb.AreaDescription = _secondArea.Description;
                        actionWeb.AreaAddress = "Санкт-Петербург,ул. Декабристов, д. 34";
                        break;
                    case "Концертный зал":
                        actionWeb.AreaImage = _concertHall.Images;
                        actionWeb.AreaDescription = _concertHall.Description;
                        actionWeb.AreaAddress = "Санкт-Петербург,ул. Декабристов, д. 37";
                        break;
                }
                actionWeb.AreaName = place;
                actionWeb.AreaMetro = "Садовая";
                actionWeb.AreaArea = "Центральный";

                HtmlNode timeNode = htmlNode.SelectSingleNode("td[@class='time']");
                actionWeb.Time = HttpUtility.HtmlDecode(timeNode.InnerText.Normalize());

                HtmlNode actionNode = htmlNode.SelectSingleNode("td[@class='main']/p[@class='title']/a");
                actionWeb.Name = HttpUtility.HtmlDecode(actionNode.InnerText.Normalize());
                string actionUrl = actionNode.Attributes["href"].Value;

                actionWeb.Date = date.ToShortDateString();
                await UploadActionInfo(actionWeb, rootUrl + actionUrl);

                InvokeActionLoadedEvent(UrlActionLoadingSource.Mariinsky, actionWeb);

                return true;
            }
            catch (Exception ex)
            {
                InvokeUrlDataLoaderExceptionThrownEvent("Ошибка распознования HTML-документа для мероприятия");
                Log.WriteLog(_logPath, ex);
                return false;
            }
        }

        private async Task UploadActionInfo(ActionWeb actionWeb, string actionUrl)
        {
            string description = string.Empty;
           
            HtmlDocument actionInfo = await DownloadDataFromWebSite(actionUrl);
            HtmlNode rootNode = actionInfo.DocumentNode;
            HtmlNodeCollection shortInfoNodes = rootNode.SelectNodes("//div[@class='spec_info_short_spect']");
            if (shortInfoNodes != null)
            {
                int i = 0;
                foreach (HtmlNode shortInfoNode in shortInfoNodes)
                {
                    string text = HttpUtility.HtmlDecode(shortInfoNode.InnerText.Normalize());
                    if (string.IsNullOrEmpty(text)) continue;
                    if (i != 0)
                        description += Environment.NewLine;
                    description += text;
                    i++;
                }
            }

            HtmlNodeCollection infoNodeCollection = rootNode.SelectNodes("//td[@class='spec_info_img']/div");
            if (infoNodeCollection != null && infoNodeCollection.Count >= 3)
            {
                HtmlNode ageLimitNode = infoNodeCollection[2].SelectSingleNode("span");
                if (ageLimitNode != null)
                    actionWeb.Rating =
                        HttpUtility.HtmlDecode(
                            ageLimitNode.InnerText.Normalize().Replace("Возрастная категория", "").Trim());
            }

            HtmlNodeCollection descriptionNodeCollection = rootNode.SelectNodes("//div[@class='spec_description']");
            if (descriptionNodeCollection != null)
            {
                foreach (HtmlNode descriptionNode in descriptionNodeCollection)
                {
                    string text=HttpUtility.HtmlDecode(descriptionNode.InnerText.Normalize());
                    if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(text))
                        description += Environment.NewLine;
                    if (!string.IsNullOrEmpty(text))
                        description +=text;
                }
            }
            actionWeb.Description = description;

            HtmlNodeCollection actorsInfoNodeCollection = rootNode.SelectNodes("//td[@class='tab_content']/div");
            if (actorsInfoNodeCollection != null)
            {
                HtmlNodeCollection actorsNodeCollection = actorsInfoNodeCollection.First().SelectNodes("div");
                if (actorsNodeCollection != null && actorsNodeCollection.Count == 2)
                {
                    HtmlNodeCollection actorsCollection = actorsNodeCollection[1].SelectNodes("a");
                    int i = 0;
                    foreach (HtmlNode actorNode in actorsCollection)
                    {
                        if (i != 0)
                            actionWeb.Actors += ",";
                        actionWeb.Actors += HttpUtility.HtmlDecode(actorNode.InnerText.Normalize());
                        i++;
                    }
                }
            }

            //todo fill action image
        }

        private void InvokeWorkDoneEvent()
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(UrlActionLoadingSource.Mariinsky);
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
