using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Artis.Consts;
using Artis.Logger;
using HtmlAgilityPack;
using NLog;

namespace Artis.DataLoader
{
    public class UrlMariinskyDataLoader:IUrlDataLoader
    {
        /// <summary>
        /// Путь к файлу логирования
        /// </summary>
        //private const string _logPath = "DataFiller.log";

        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

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
                
                KeyValuePair<string,HtmlDocument> doc = await DownloadDataFromWebSite(currentDate);
                if (doc.Value.DocumentNode.ChildNodes.Count == 0)
                {
                    _logger.Fatal("Ошибка загрузки данных с URL=" + doc.Key);
                    throw new UrlDataLoaderException("Не удалось загрузить данный!", "Загрузка данных");
                }
                _logger.Debug("Парсинг данных с URL=" + doc.Key);
                await ParseHtmlDoc(doc.Value, currentDate);
                currentDate=currentDate.AddDays(1);
            }

            InvokeWorkDoneEvent();
        }

        public void CancelLoadData()
        {
            _cancelToken.Cancel();
        }


        private async Task<KeyValuePair<string,HtmlDocument>> DownloadDataFromWebSite(DateTime date)
        {
            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();

            string url = CretaeUriToDownload(date.Year, date.Month,date.Day);
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return new KeyValuePair<string, HtmlDocument>(url,doc);
        }

        private async Task<HtmlDocument> DownloadDataFromWebSite(string url)
        {
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        private async Task<XDocument> DownloadXmlFromWebSite(string url)
        {
            string result = await DownloadHtml(url);
            return XDocument.Parse(result);
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
                    _logger.ErrorException("Ошибка загрузки данных с URL=" + query+"Загрузка данных будет продолжена.",ex);
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
            return await ParseAreaInfo(doc, areaUrl);
        }

        private async Task<MariinskyAreaInfo> ParseAreaInfo(HtmlDocument doc,string areaUrl)
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
                _logger.ErrorException(
                    "Ошибка распознания данных для площадки(" + areaUrl +
                    "). Загрузка данных будет продолжена, однако для мероприятия, проводимые на данной площадке не будут сохранены!",
                    ex);
            }
            return null;
        }

        private async Task ParseHtmlDoc(HtmlDocument doc,DateTime date)
        {
            try
            {
                HtmlNodeCollection dateNode =
                    doc.DocumentNode.SelectNodes("//table[@id='afisha']/tr/td/table[@class='this_day w_100']/tr");

                foreach (HtmlNode htmlNode in dateNode)
                {
                    // Периодически проверяем, не запрошена ли отмена операции
                    _cancelToken.Token.ThrowIfCancellationRequested();
                    KeyValuePair<bool,ActionWeb> result = await createNode(htmlNode, date);
                    if (result.Key)
                    {
                        if (!string.IsNullOrEmpty(result.Value.AreaName)
                            && !string.IsNullOrEmpty(result.Value.Date)
                            && !string.IsNullOrEmpty(result.Value.Name)
                            && !string.IsNullOrEmpty(result.Value.Time))
                            InvokeActionLoadedEvent(UrlActionLoadingSource.Mariinsky, result.Value);
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
                        _logger.Error("Не удалось распознать мероприятия со страницы для даты " +
                                      date.ToShortDateString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка распознания данных для мероприятия. Загрузка данных будет продолжена...", ex);
                //Log.WriteLog(_logPath, ex);
            }
        }

        private async Task<KeyValuePair<bool,ActionWeb>>  createNode(HtmlNode htmlNode, DateTime date)
        {
            ActionWeb actionWeb = new ActionWeb();
            try
            {


                HtmlNode placeNode = htmlNode.SelectSingleNode("td[@class='where_ico']");
                if (placeNode != null)
                {
                    string place = HttpUtility.HtmlDecode(placeNode.InnerText.Normalize()).Trim();
                    switch (place)
                    {
                        case "Мариинский театр":
                            actionWeb.AreaImage = _mainArea.Images;
                            actionWeb.AreaDescription = _mainArea.Description;
                            actionWeb.AreaAddress = "Санкт-Петербург,Театральная площадь, д. 1";
                            break;
                        case "Мариинский-2 (Новая сцена)":
                            actionWeb.AreaImage = _secondArea.Images;
                            actionWeb.AreaDescription = _secondArea.Description;
                            actionWeb.AreaAddress = "Санкт-Петербург,ул. Декабристов, д. 34";
                            break;
                        case "Концертный зал":
                            actionWeb.AreaImage = _concertHall.Images;
                            actionWeb.AreaDescription = _concertHall.Description;
                            actionWeb.AreaAddress = "Санкт-Петербург,ул. Декабристов, д. 37";
                            break;
                        default:
                            actionWeb.AreaImage = _mainArea.Images;
                            actionWeb.AreaDescription = _mainArea.Description;
                            actionWeb.AreaAddress = "Санкт-Петербург,Театральная площадь, д. 1";
                            break;
                    }
                    actionWeb.AreaName = place;
                }
                else
                {
                    _logger.Warn("Не удалось найти площадку для мероприятия. Будет проставлена площадка по-умолчанию-Мариинский театр");
                    actionWeb.AreaImage = _mainArea.Images;
                    actionWeb.AreaDescription = _mainArea.Description;
                    actionWeb.AreaAddress = "Санкт-Петербург,Театральная площадь, д. 1";
                    actionWeb.AreaName = "Мариинский театр";
                }

                
                actionWeb.AreaMetro = "Садовая";
                actionWeb.AreaArea = "Центральный";

                HtmlNode timeNode = htmlNode.SelectSingleNode("td[@class='time']");
                if (timeNode != null)
                {
                    actionWeb.Time = HttpUtility.HtmlDecode(timeNode.InnerText.Normalize());
                }
                else
                {
                    _logger.Warn("Не удалось найти время проведения мероприятия. Будет проставлено время по-умолчанию - 00:00");
                    actionWeb.Time = "00:00";

                }

                HtmlNode actionNode = htmlNode.SelectSingleNode("td[@class='main']/p[@class='title']/a");
                if (actionNode==null)
                {
                    _logger.Error("Не удалось найти наименование мероприятия.");
                    return new KeyValuePair<bool, ActionWeb>(false, actionWeb);
                }
                actionWeb.Name = HttpUtility.HtmlDecode(actionNode.InnerText.Normalize());
                string actionUrl = actionNode.Attributes["href"].Value;
                HtmlNode actionDescriptionNode = htmlNode.SelectSingleNode("td[@class='main']/p[@class='descr']");
                if (actionDescriptionNode != null)
                {
                    string shortDescription = HttpUtility.HtmlDecode(actionDescriptionNode.InnerText.Normalize()).Trim();
                    if (shortDescription.IndexOf("балет", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        actionWeb.Genre = "Балет";
                    }
                    else if (shortDescription.IndexOf("спектакль", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        actionWeb.Genre = "Спектакль";
                    }
                    else if (shortDescription.IndexOf("концерт", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        actionWeb.Genre = "Концерт";
                    }
                    else if (shortDescription.IndexOf("опера", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        actionWeb.Genre = "Опера";
                    }
                    else if (shortDescription.IndexOf("поэма ", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        actionWeb.Genre = "Поэма ";
                    }

                }
                else
                {
                    _logger.Warn("Не удалось найти описание мероприятия.Проставлено значение по умолчанию-Отсутствует");
                    actionWeb.Genre = "Отсутствует";
                }


                actionWeb.Date = date.ToShortDateString();
                await UploadActionInfo(actionWeb, rootUrl + actionUrl);
                return new KeyValuePair<bool, ActionWeb>(true, actionWeb);
            }
            catch (Exception ex)
            {
                InvokeUrlDataLoaderExceptionThrownEvent("Ошибка распознования HTML-документа для мероприятия");
                _logger.ErrorException(
                    "Ошибка распознования HTML-документа для мероприятия. Загрузка данных будет продолжена...", ex);
                return new KeyValuePair<bool, ActionWeb>(false, actionWeb);
                ;
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
                    string text = HttpUtility.HtmlDecode(descriptionNode.InnerText.Normalize());
                    if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(text))
                        description += Environment.NewLine;
                    if (!string.IsNullOrEmpty(text))
                        description += text;
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

            List<string> actionImages = new List<string>();
            HtmlNodeCollection actionImageNodeCollection =
                rootNode.SelectNodes(
                    "//table[@id='spectacle_photo_gallery']/tr/td/a/img");
            if (actionImageNodeCollection != null)
            {
                foreach (HtmlNode imageNode in actionImageNodeCollection)
                {
                    using (WebClient client = new WebClient())
                    {
                        byte[] image = client.DownloadData(rootUrl + imageNode.Attributes["src"].Value);
                        string base64String = Convert.ToBase64String(image, 0, image.Length);
                        actionImages.Add(base64String);
                    }
                }
                actionWeb.Image = actionImages;
            }
            else
            {
                _logger.Warn("Не удалось загрузить изображения для мероприятия");
            }

            HtmlNodeCollection actionDescNodes = rootNode.SelectNodes("//div[@class='spec_description']/p");
            if (actionDescNodes != null)
            {
                foreach (HtmlNode node in actionDescNodes)
                {
                    string text = HttpUtility.HtmlDecode(node.InnerText.Normalize()).Trim();
                    if (text.IndexOf("Продолжительность спектакля", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        int Hours=0;
                        int Minute=0;
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
                }
            }

            HtmlNode buyTicketNodes = rootNode.SelectSingleNode("//td[@class='ticket']/a");
            if (buyTicketNodes != null)
                await UploadPriceInfo(actionWeb, buyTicketNodes.Attributes["href"].Value);

        }

        private async Task UploadPriceInfo(ActionWeb actionWeb, string actionUrl)
        {
            try
            {
                XDocument actionInfo = await DownloadXmlFromWebSite(actionUrl);
                XElement node = actionInfo.Root.Element("content").Element("spec_free_place_pict");
                IEnumerable<XElement> tickets = node.Elements("ticket").ToList();

                int min = tickets.Select(str =>
                {
                    int value;
                    if (int.TryParse(str.Element("tprice1").Value, out value))
                        return value;
                    return int.MaxValue;
                }).Min();

                int max = tickets.Select(str =>
                {
                    int value;
                    if (int.TryParse(str.Element("tprice2").Value, out value))
                        return value;
                    return int.MinValue;
                }).Max();


                if (min != 0 && max != 0)
                    actionWeb.PriceRange = "от " + min + " до " + max;
                else if (min != 0)
                    actionWeb.PriceRange = min.ToString();
                else if (max != 0)
                    actionWeb.PriceRange = max.ToString();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка загрузки минимальной и максимальной стоимости билетов для мероприятия",ex);
            }

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
