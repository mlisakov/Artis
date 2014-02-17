using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Artis.Consts;
using Artis.Logger;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Artis.DataLoader
{
    class DataLoader
    {
        private const string _logPath = "DataFiller.log";
        private const string _pagesCountClassName = "reglink2";
        private const string _dayTDClassName = "headerred14";
        private int _eventsCount;

        private List<ActionWeb> _actions;

        public delegate void EventLoaded(ActionWeb action);

        public delegate void WorkDone(int actionsCount);

        public delegate void ActionWorkInProgress(string actionName);

        /// <summary>
        /// Событие загрузки мероприятия
        /// </summary>
        public event EventLoaded EventLoadedEvent;
        /// <summary>
        /// Событие окончания загрузки
        /// </summary>
        public event WorkDone WorkDoneEvent;
        /// <summary>
        /// Событие загрузки нового мероприятия
        /// </summary>
        public event ActionWorkInProgress ActionWorkInProgressEvent;

        private int _pageCount;


        public List<ActionWeb> Actions
        {
            get { return new List<ActionWeb>(_actions); }
        }

        public DataLoader()
        {
            _actions = new List<ActionWeb>();
        }

        public async void FillData(DateTime start, DateTime finish)
        {
            _eventsCount = 0;
            _actions.Clear();

            HtmlDocument doc = await DownloadDataFromWesSite(start, finish);

            if (doc.DocumentNode.ChildNodes.Count == 0)
            {
                MessageBox.Show("Не удалось загрузить данный!", "Загрузка данных");
                return;
            }

            await ParseHtmlDoc(doc);

            _pageCount = GetPageCount(doc);

            for (int i = 2; i <= _pageCount; i++)
            {
                doc = await DownloadDataFromWesSite(start, finish, i);
                await ParseHtmlDoc(doc);
            }
            InvokeWorkDone(_eventsCount);
        }

        //public async void FillData(DateTime start, DateTime finish)
        //{
        //    _eventsCount = 0;
        //    _actions.Clear();

        //    HtmlDocument doc = await DownloadDataFromWesSite(start, finish);

        //    if (doc.DocumentNode.ChildNodes.Count == 0)
        //    {
        //        MessageBox.Show("Не удалось загрузить данный!", "Загрузка данных");
        //        return;
        //    }

        //    var t = await ParseHtmlDoc(doc);
        //    InvokePageLoaded(1);
        //    int pages = GetPageCount(doc);

        //    for (int i = 2; i <= pages; i++)
        //    {
        //        doc = await DownloadDataFromWesSite(start, finish, i);
        //        var m = await ParseHtmlDoc(doc);
        //        InvokePageLoaded(i);
        //    }
        //    InvokeWorkDone(_eventsCount);
        //}

        private void InvokeEventLoaded(ActionWeb action)
        {
            EventLoaded handler = EventLoadedEvent;
            if (handler != null)
                handler(action);
        }

        private void InvokeActionWorkInProgress(string actionName)
        {
            ActionWorkInProgress handler = ActionWorkInProgressEvent;
            if (handler != null)
                handler(actionName);
        }

        private void InvokeWorkDone(int eventsCount)
        {
            WorkDone handler = WorkDoneEvent;
            if (handler != null)
                handler(eventsCount);
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
                    if (int.TryParse(htmlNode.InnerText.Replace("...",""), out tempPageIndex))
                    {
                        if (tempPageIndex > pageCount)
                            pageCount = tempPageIndex;
                    }
                }
            }
            return pageCount;
        }

        private async Task<HtmlDocument> DownloadDataFromWesSite(DateTime start, DateTime finish, int page = 1)
        {
            string url = CretaeUriToDownload(start, finish, page);
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        private async Task<HtmlDocument> DownloadDataFromWesSite(string url)
        {
            string result = await DownloadHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return doc;
        }

        private async Task<string> DownloadHtml(string query)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(query);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(_logPath, ex);
                }
            }
            return string.Empty;
        }

        //private string CretaeUriToDownload(DateTime start, DateTime finish, int page)
        //{
        //    string sourceULR =
        //        "http://old.bileter.ru/poster.phtml?&JS=98d46a7433ab6ed98b904da3140a5262&cala=d&calm=d&actex_d=" + start +
        //        "&actex_e=" + finish;
        //    if (page > 1)
        //        sourceULR += "&csp=" + page;
        //    return sourceULR;
        //}

        private string CretaeUriToDownload(DateTime start, DateTime finish, int page)
        {
            string sourceULR =
                "http://bileter.ru/afisha/" + start.ToString("yyyy-MM-dd") +
                "/" + finish.ToString("yyyy-MM-dd");
            if (page > 1)
                sourceULR += "/list" + page + ".html";
            else
                sourceULR += "/index.html";
            return sourceULR;
        }

        private async Task<bool> ParseHtmlDoc(HtmlDocument doc)
        {
            try
            {
                HtmlNodeCollection dateNode =
                    doc.DocumentNode.SelectNodes("//div[@class='afisha_events_item']");
                bool isSuccess = true;
                foreach (HtmlNode htmlNode in dateNode)
                {
                    bool success = await createNode(htmlNode);

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

                string dateText =  HttpUtility.HtmlDecode(htmlNode.ParentNode.ParentNode.ChildNodes[1].InnerText).Trim();
                string date = dateText.Split(new char[] { ' ' })[0];

                HtmlNode ratingNode = htmlNode.SelectSingleNode("div/p[@class='afisha_kdmtp']");
                string rating = ratingNode.InnerText;

                HtmlNode infoNode = htmlNode.SelectSingleNode("div[@class='afisha_item_info']");
                string time = infoNode.SelectSingleNode("p[@class='first']").ChildNodes[1].InnerText;
                string price = infoNode.SelectSingleNode("p[@class='last']").ChildNodes[1].InnerText;
                //string price =
                //    GetPriceRange(node.ParentNode.ParentNode.SelectSingleNode("./td/table/tr/td/div/nobr"));
                //string time = GetTime(node.ParentNode.ParentNode.SelectNodes("./td/table/tr/td/b"));

                InvokeActionWorkInProgress(date+" "+action);

                string actionURL = "http://bileter.ru/" + actionURLPath;
                string placeURL = "http://bileter.ru/" + placeURLPath;

                string areaGoogleURL = "https://www.google.ru/search?q=" + place.Trim().Replace(" ", "+") + "&tbm=isch";
                string actionGoogleURL = "https://www.google.ru/search?q=" + action.Trim().Replace(" ", "+") + "&tbm=isch";
                
                HtmlDocument actionInfo = await DownloadDataFromWesSite(actionURL);

                HtmlDocument areaInfo = await DownloadDataFromWesSite(placeURL);
                HtmlDocument areaGoogleImgaInfo = await DownloadDataFromWesSite(areaGoogleURL);
                HtmlDocument actionGoogleImgaInfo = await DownloadDataFromWesSite(actionGoogleURL);

                List<string> areaImage = DownloadImageBase64String(areaGoogleImgaInfo);
                List<string> actionImage = DownloadImageBase64String(actionGoogleImgaInfo);

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
                _actions.Add(actionWeb);
                _eventsCount++;
                InvokeEventLoaded(actionWeb);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(_logPath, ex);
                return false;
            }
        }

        //private async Task<bool> createNodes(HtmlNode htmlNode, string dateN)
        //{
        //    try
        //    {
        //        foreach (HtmlNode node in htmlNode.SelectNodes("//tr/td/a[@class='reglink10']"))
        //        {
        //            if (node.ChildNodes[0].InnerText != "")
        //            {
        //                string t = node.InnerText.Normalize();
        //                string actionUrlPath = node.Attributes[0].Value;

        //                HtmlNode areaNode = node.ParentNode.ParentNode.SelectSingleNode("./td/a[@class='reglink8']");
        //                string place = areaNode.InnerText;
        //                string areaURLPath = areaNode.Attributes[0].Value;

        //                string date = String.Format("{0:yyyy-MM-dd}", dateN);
        //                string eventText = node.InnerText.Normalize();
        //                string price =
        //                    GetPriceRange(node.ParentNode.ParentNode.SelectSingleNode("./td/table/tr/td/div/nobr"));
        //                string time = GetTime(node.ParentNode.ParentNode.SelectNodes("./td/table/tr/td/b"));

        //                InvokeActionWorkInProgress(eventText);

        //                string actionURL = "http://old.bileter.ru/poster.phtml" + actionUrlPath;
        //                string areanURL = "http://old.bileter.ru/" + areaURLPath;
        //                string areaGoogleURL = "https://www.google.ru/search?q=" + place.Trim().Replace(" ","+") + "&tbm=isch";
        //                string actionGoogleURL = "https://www.google.ru/search?q=" + eventText.Trim().Replace(" ", "+") + "&tbm=isch";
        //                HtmlDocument actionInfo = await DownloadDataFromWesSite(actionURL);

        //                HtmlDocument areaInfo = await DownloadDataFromWesSite(areanURL);
        //                HtmlDocument areaGoogleImgaInfo = await DownloadDataFromWesSite(areaGoogleURL);
        //                HtmlDocument actionGoogleImgaInfo = await DownloadDataFromWesSite(actionGoogleURL);

        //                string areaImage=DownloadImageBase64String(areaGoogleImgaInfo);
        //                string actionImage = DownloadImageBase64String(actionGoogleImgaInfo);

        //                ActionWeb actionWeb = new ActionWeb()
        //                {
        //                    AreaName = place,
        //                    Date = date,
        //                    Name = eventText,
        //                    PriceRange = price,
        //                    Time = time
        //                };

        //                if (!string.IsNullOrEmpty(areaImage))
        //                    actionWeb.AreaImage = areaImage;

        //                if (!string.IsNullOrEmpty(actionImage))
        //                    actionWeb.Image = actionImage;

        //                ParseActionInfo(actionInfo, areaInfo, actionWeb);
        //                _actions.Add(actionWeb);
        //                _eventsCount++;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteLog(_logPath, ex);
        //        return false;
        //    }
        //}

        private List<string> DownloadImageBase64String(HtmlDocument areaGoogleImgaInfo)
        {
            try
            {
                if (areaGoogleImgaInfo != null)
                {
                    long currentMaxSize = 0;
                    string currentImageUrl = string.Empty;
                    int index = 0;
                    Dictionary<string, int> imageUrlList = new Dictionary<string, int>();
                    List<string> stringImages = new List<string>();
                    foreach (HtmlNode img in areaGoogleImgaInfo.DocumentNode.SelectNodes("//td/a/img"))
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
                        //if (currentMaxSize < imageActualSize)
                        //{
                        //    currentMaxSize = imageActualSize;
                        //    currentImageUrl = imageUrl;
                        //    index++;
                        //}
                        index++;
                    }

                    using (WebClient client = new WebClient())
                    {
                        foreach (string url in imageUrlList.OrderBy(i => i.Value).Take(5).Select(i => i.Key))
                        {
                            byte[] image = client.DownloadData(url);
                            string base64String =Convert.ToBase64String(image,0,image.Length);
                            stringImages.Add(base64String);
                        }
                        return stringImages;
                    }
                }
                return new List<string>();

            }
            catch (Exception ex)
            {
                Log.WriteLog(_logPath, ex);
                return new List<string>();
            }
            
        }

        private async Task ParseActionInfo(HtmlDocument actionInfo, HtmlDocument areaInfo, ActionWeb actionWeb)
        {

            HtmlNode descriptionNode = actionInfo.DocumentNode.SelectSingleNode("//div[@class='event_text']");
            if (descriptionNode != null)
                actionWeb.Description = HttpUtility.HtmlDecode(descriptionNode.InnerText).Trim();


            HtmlNodeCollection eventDescription = actionInfo.DocumentNode.SelectNodes("//div[@class='event_desc']/p");
            if (eventDescription!=null)
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
            HtmlNode areaNodeSchema = areaInfo.DocumentNode.SelectSingleNode("//article[@class='event']/h1[@class='afisha_event_h']/div[@class='platform_photo']/a");
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

            HtmlNodeCollection areaDescription = areaInfo.DocumentNode.SelectNodes("//article[@class='event']/div[@class='fat_grey_bar']/p");
            if (areaDescription!=null)
            foreach (HtmlNode node in areaDescription)
            {
                string Type = HttpUtility.HtmlDecode(node.ChildNodes[0].InnerText).Trim().Replace(":", "");
                string Text = HttpUtility.HtmlDecode(node.ChildNodes[1].InnerText).Trim().Replace(":", "");
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

            HtmlNode areaDesc = areaInfo.DocumentNode.SelectSingleNode("//article[@class='event']/div[@class='event_text']");
            if (areaDesc != null)
                actionWeb.AreaDescription = HttpUtility.HtmlDecode(areaDesc.InnerText)
                    .Trim()
                    .Replace("ОПИСАНИЕ", "");
        }

        //private string GetPriceRange(HtmlNode htmlNode)
        //{
        //    string strReturn = "";
        //    if (htmlNode.InnerText != "")
        //    {
        //        strReturn = " от " + htmlNode.InnerText.Replace("&nbsp;", " ").Replace("-", " до ");
        //    }
        //    return strReturn;
        //}

        //private string GetTime(HtmlNodeCollection htmlNode)
        //{
        //    string strReturn = "";
        //    foreach (HtmlNode node in htmlNode)
        //    {
        //        if ((node.InnerText != "") && (node.InnerText != "Цена&nbsp;"))
        //        {
        //            strReturn = node.InnerText.Remove(5);
        //        }
        //    }
        //    return strReturn;
        //}
    }
}