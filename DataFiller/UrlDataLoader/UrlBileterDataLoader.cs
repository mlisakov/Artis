using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Artis.Consts;
using HtmlAgilityPack;
using NLog;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Artis.DataLoader
{
    public class UrlBileterDataLoader:IUrlDataLoader
    {
        //TODO Подключить логирование NLog!
        /// <summary>
        /// Путь к файлу логирования
        /// </summary>
        //private const string _logPath = "DataFiller.log";
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        CancellationTokenSource _cancelToken;

        /// <summary>
        /// Событие загрузки страницы
        /// </summary>
        public event ActionWebLoaded ActionLoadedEvent;

        /// <summary>
        /// Событие возникновения НЕ критической ошибки в процессе загрузки данных
        /// </summary>
        public event UrlDataLoaderExceptionThrown UrlDataLoaderExceptionThrownEvent;

        /// <summary>
        /// Собыьте окончания загрузки данных
        /// </summary>
        public event WorkDone WorkDoneEvent;

        public UrlBileterDataLoader()
        {
            _cancelToken=new CancellationTokenSource();
        }

        /// <summary>
        /// Загрузка мероприятий в указанном интервале
        /// </summary>
        /// <param name="start">Начальная дата фильтра по дате проведения мероприятия</param>
        /// <param name="finish">Конечная дата фильтра по дате проведения мероприятия</param>
        /// <returns></returns>
        public async Task LoadData(DateTime start, DateTime finish)
        {
            _cancelToken = new CancellationTokenSource();

            KeyValuePair<string,HtmlDocument> doc = await DownloadDataFromWebSite(start, finish);

            if (doc.Value.DocumentNode.ChildNodes.Count == 0)
            {
                _logger.Fatal("Bileter.Ошибка загрузки данных с URL=" + doc.Key);
                throw new UrlDataLoaderException("Bileter.Не удалось загрузить данный!", "Загрузка данных");
            }

            await ParseHtmlDoc(doc.Value);

            var pageCount = GetPageCount(doc.Value);

            for (var i = 2; i <= pageCount; i++)
            {
                _cancelToken.Token.ThrowIfCancellationRequested();
                 doc=await DownloadDataFromWebSite(start, finish, i);

                if (doc.Value.DocumentNode.ChildNodes.Count == 0)
                {
                    _logger.Fatal("Bileter.Ошибка загрузки данных с URL=" + doc.Key);
                    throw new UrlDataLoaderException("Bileter.Не удалось загрузить данный!", "Загрузка данных");
                }

                await ParseHtmlDoc(doc.Value);
            }

            InvokeWorkDoneEvent();
        }

        public void CancelLoadData()
        {
            _cancelToken.Cancel();
        }

        private void InvokeActionLoadedEvent(UrlActionLoadingSource source,ActionWeb action)
        {
            ActionWebLoaded handler = ActionLoadedEvent;
            if (handler != null)
                handler(source,action);
        }

        private void InvokeUrlDataLoaderExceptionThrownEvent(string text)
        {
            UrlDataLoaderExceptionThrown handler = UrlDataLoaderExceptionThrownEvent;
            if (handler != null)
                handler(text);
        }

        private void InvokeWorkDoneEvent()
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(UrlActionLoadingSource.Bileter);
        }


        private async Task<KeyValuePair<string,HtmlDocument>> DownloadDataFromWebSite(DateTime start, DateTime finish, int page = 1)
        {
            // Периодически проверяем, не запрошена ли отмена операции
            _cancelToken.Token.ThrowIfCancellationRequested();
            
            string url = CretaeUriToDownload(start, finish, page);
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

        private string CretaeUriToDownload(DateTime start, DateTime finish, int page)
        {
            string sourceUlr =
                "http://bileter.ru/afisha/" + start.ToString("yyyy-MM-dd") +
                "/" + finish.ToString("yyyy-MM-dd");
            if (page > 1)
                sourceUlr += "/list" + page + ".html";
            else
                sourceUlr += "/index.html";
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
                    throw new UrlDataLoaderException(query, "Bileter.Ошибка загрузки" + Environment.NewLine + ex.Message);
                }
            }
        }

        private int GetPageCount(HtmlDocument doc)
        {
            int pageCount = 0;
            HtmlNodeCollection pageNodeCollection =
                doc.DocumentNode.SelectNodes("//nav[@class='page_nav']/ul/li");
            if (pageNodeCollection != null)
            {
                foreach (
                    HtmlNode htmlNode in pageNodeCollection)
                {
                    int tempPageIndex;
                    if (int.TryParse(htmlNode.InnerText.Replace("...", ""), out tempPageIndex))
                    {
                        if (tempPageIndex > pageCount)
                            pageCount = tempPageIndex;
                    }
                }
            }
            return pageCount;
        }

        private async Task ParseHtmlDoc(HtmlDocument doc)
        {
            try
            {
                HtmlNodeCollection dateNode =
                    doc.DocumentNode.SelectNodes("//div[@class='afisha_events_item']");
                bool isSuccess = true;
                foreach (HtmlNode htmlNode in dateNode)
                {
                    // Периодически проверяем, не запрошена ли отмена операции
                    _cancelToken.Token.ThrowIfCancellationRequested();
                    bool success = await createNode(htmlNode);

                    if (!success)
                    {
                        _logger.Error("Bileter.Не удалось распознать мероприятие" + Environment.NewLine + htmlNode.InnerHtml);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("Bileter.Ошибка распознания данных для мероприятия. Загрузка данных будет продолжена...", ex);
            }
        }

        private async Task<bool> createNode(HtmlNode htmlNode)
        {
            try
            {
                HtmlNode placeNode = htmlNode.SelectSingleNode("div/p[@class='afisha_event_place']/a");
                string place = HttpUtility.HtmlDecode(placeNode.InnerText.Normalize());
                string placeURLPath = placeNode.Attributes["href"].Value;

                HtmlNode actionNode = htmlNode.SelectSingleNode("div/h4[@class='afisha_event_h']/a");
                string action = HttpUtility.HtmlDecode(actionNode.InnerText.Normalize());
                string actionURLPath = actionNode.Attributes["href"].Value;

                string dateText = HttpUtility.HtmlDecode(htmlNode.ParentNode.ParentNode.ChildNodes[1].InnerText).Trim();
                string date = dateText.Split(new char[] { ' ' })[0];

                string rating=string.Empty;
                HtmlNode ratingNode = htmlNode.SelectSingleNode("div/p[@class='afisha_kdmtp']");
                if(ratingNode!=null)
                 rating = ratingNode.InnerText;

                HtmlNode infoNode = htmlNode.SelectSingleNode("div[@class='afisha_item_info']");
                string time = infoNode.SelectSingleNode("p[@class='first']").ChildNodes[1].InnerText;

                string price = string.Empty;
                HtmlNode priceNode = infoNode.SelectSingleNode("p[@class='last']");
                if (priceNode != null && priceNode.ChildNodes.Count > 1 && priceNode.ChildNodes[1] != null)
                    price = priceNode.ChildNodes[1].InnerText;


                string actionURL = "http://bileter.ru/" + actionURLPath;
                string placeURL = "http://bileter.ru/" + placeURLPath;

                string areaGoogleURL = "https://www.google.ru/search?q=" + place.Trim().Replace(" ", "+") + "&tbm=isch";
                string actionGoogleURL = "https://www.google.ru/search?q=" + action.Trim().Replace(" ", "+") + "&tbm=isch";

                HtmlDocument actionInfo = await DownloadDataFromWebSite(actionURL);

                HtmlDocument areaInfo = await DownloadDataFromWebSite(placeURL);
                HtmlDocument areaGoogleImgaInfo = await DownloadDataFromWebSite(areaGoogleURL);
                HtmlDocument actionGoogleImgaInfo = await DownloadDataFromWebSite(actionGoogleURL);

                List<string> areaImage = GetImageBase64StringFromGoogle(areaGoogleImgaInfo);
                List<string> actionImage = GetImageBase64StringFromGoogle(actionGoogleImgaInfo);

                ActionWeb actionWeb = new ActionWeb()
                {
                    AreaName = place,
                    Date = date,
                    Name = action,
                    PriceRange = price,
                    Time = time
                };

                if (areaImage.Any())
                    actionWeb.AreaImage = areaImage;

                if (actionImage.Any())
                    actionWeb.Image = actionImage;

                await ParseActionInfo(actionInfo, areaInfo, actionWeb);

                InvokeActionLoadedEvent(UrlActionLoadingSource.Bileter, actionWeb);

                return true;
            }
            catch (Exception ex)
            {
                InvokeUrlDataLoaderExceptionThrownEvent("Bileter.Ошибка распознования HTML-документа для мероприятия");
                _logger.ErrorException(
                    "Bileter.Ошибка распознования HTML-документа для мероприятия. Загрузка данных будет продолжена...", ex);
                return false;
            }
        }

        private async Task ParseActionInfo(HtmlDocument actionInfo, HtmlDocument areaInfo, ActionWeb actionWeb)
        {
            HtmlNode descriptionNode = actionInfo.DocumentNode.SelectSingleNode("//div[@class='event_text']");
            if (descriptionNode != null)
                actionWeb.Description = HttpUtility.HtmlDecode(descriptionNode.InnerText).Trim();


            HtmlNodeCollection eventDescription = actionInfo.DocumentNode.SelectNodes("//div[@class='event_desc']/p");
            if (eventDescription != null)
                foreach (HtmlNode node in eventDescription)
                {
                    string Type = HttpUtility.HtmlDecode(node.ChildNodes[0].InnerText).Trim().Replace(":", "");
                    string Text = HttpUtility.HtmlDecode(node.ChildNodes[1].InnerText).Trim();
                    switch (Type)
                    {
                        case "Жанр":
                            actionWeb.Genre = Text;
                            break;
                        case "Режиссер":
                            actionWeb.Producer = Text;
                            break;
                        case "Автор":
                            actionWeb.Author = Text;
                            break;
                        case "Продолжительность":
                            actionWeb.Duration = Text;
                            break;
                    }
                }

            HtmlNode actorsNode = actionInfo.DocumentNode.SelectSingleNode("//div[@class='event_actors']");
            if (actorsNode != null && actorsNode.ChildNodes[3] != null)
                actionWeb.Actors = actorsNode.ChildNodes[3].InnerText;

            //Area
            HtmlNode areaNodeSchema =
                areaInfo.DocumentNode.SelectSingleNode(
                    "//article[@class='event']/h1[@class='afisha_event_h']/div[@class='platform_photo']/a");
            if (areaNodeSchema != null)
            {
                string shemaUrl = areaNodeSchema.Attributes["href"].Value;
                string base64String;
                using (WebClient client = new WebClient())
                {
                    byte[] image = client.DownloadData("http://www.bileter.ru" + shemaUrl);
                    base64String =
                        Convert.ToBase64String(image,
                            0,
                            image.Length);

                }
                if (!string.IsNullOrEmpty(base64String))
                    actionWeb.AreaSchemaImage = base64String;
            }

            HtmlNodeCollection areaDescription =
                areaInfo.DocumentNode.SelectNodes("//article[@class='event']/div[@class='fat_grey_bar']");

            if (areaDescription != null)
                foreach (HtmlNode node in areaDescription)
                {
                    HtmlNodeCollection areaDescriptionParagraphCollection = node.SelectNodes("p");
                    if (areaDescriptionParagraphCollection.Count == 3)
                    {
                        foreach (HtmlNode paragraph in areaDescriptionParagraphCollection)
                        {
                            string Type = HttpUtility.HtmlDecode(paragraph.ChildNodes[0].InnerText)
                                .Trim()
                                .Replace(":", "");
                            string Text = HttpUtility.HtmlDecode(paragraph.ChildNodes[1].InnerText)
                                .Trim()
                                .Replace(":", "");
                            switch (Type)
                            {
                                case "Адрес":
                                    actionWeb.AreaAddress = Text;
                                    break;
                                case "Станция метро":
                                    actionWeb.AreaMetro = Text;
                                    break;
                                case "Район города":
                                    actionWeb.AreaArea = Text;
                                    break;


                            }
                        }

                    }
                }

            if (string.IsNullOrEmpty(actionWeb.AreaAddress))
                _logger.Error("Bileter.Для площадки не удалось получить адрес" + Environment.NewLine +
                              areaInfo.DocumentNode.InnerHtml);

            HtmlNode areaDesc =
                areaInfo.DocumentNode.SelectSingleNode("//article[@class='event']/div[@class='event_text']");
            if (areaDesc != null)
                actionWeb.AreaDescription = HttpUtility.HtmlDecode(areaDesc.InnerText)
                    .Trim()
                    .Replace("ОПИСАНИЕ", "");
        }

        private List<string> GetImageBase64StringFromGoogle(HtmlDocument areaGoogleImageInfo)
        {
            try
            {
                if (areaGoogleImageInfo != null)
                {
                    long currentMaxSize = 0;
                    string currentImageUrl = string.Empty;
                    int index = 0;
                    Dictionary<string, int> imageUrlList = new Dictionary<string, int>();
                    List<string> stringImages = new List<string>();
                    HtmlNodeCollection images = areaGoogleImageInfo.DocumentNode.SelectNodes("//td/a/img");
                    if (images!=null)
                        foreach (HtmlNode img in images)
                        {
                            if (index >= 10) break;
                            string imageUrl = img.Attributes["src"].Value;
                            HtmlNode tdNode = img.ParentNode.ParentNode;
                            string imageSize = HttpUtility.HtmlDecode(tdNode.ChildNodes.Last().InnerText).Trim();
                            string[] splitedSize = imageSize.Split(';');
                            int imageActualSize = 0;
                            int.TryParse(splitedSize[1].Replace("КБ", "").Replace("-", "").Replace("jpg", "").Trim(),
                                out imageActualSize);
                            imageUrlList.Add(imageUrl, imageActualSize);
                            index++;
                        }

                    using (WebClient client = new WebClient())
                    {
                        foreach (string url in imageUrlList.OrderBy(i => i.Value).Take(5).Select(i => i.Key))
                        {
                            byte[] image = client.DownloadData(url);
                            string base64String = Convert.ToBase64String(image, 0, image.Length);
                            stringImages.Add(base64String);
                        }
                        return stringImages;
                    }
                }
                return new List<string>();

            }
            catch (Exception ex)
            {
                InvokeUrlDataLoaderExceptionThrownEvent("Bileter.Ошибка загрузки изображений для мероприятия");
                _logger.ErrorException("Bileter.Ошибка загрузки изображения...", ex);
                return new List<string>();
            }

        }
    }
}
