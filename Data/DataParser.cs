using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artis.Consts;
using NLog;

namespace Artis.Data
{
    /// <summary>
    /// Класс для запроса данных с БД
    /// </summary>
    public class DataParser
    {
        //TODO Сделать логирование через NLog!
        /// <summary>
        /// Имя файла логирования
        /// </summary>
        //private const string _logFilePath = "NHibernateError.log";

        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private AreaRepository _areaRepository;
        private ActionRepository _actionRepository;
        private ActionDateRepository _actionDateRepository;
        private ProducerRepository _producerRepository;
        private ActorRepository _actorRepository;
        private GenreRepository _genreRepository;
        private DataRepository _dataRepository;
        private MetroRepository _metroRepository;

        //public event DataLoaderActionLoaded ActionLoadedEvent;
        //public event DataLoaderActionNotLoaded ActionNotLoadedEvent;

        //public event FatalError FatalErrorEvent;

        public DataParser()
        {
            Init();
        }

        /// <summary>
        /// Запись в БД информации по мероприятию
        /// </summary>
        /// <param name="actionWeb">Информация о мероприятии</param>
        /// <returns></returns>
        public async Task<int> Parse(ActionWeb actionWeb)
        {
            return await ParseAction(actionWeb);
        }

        private void Init()
        {
            _areaRepository = new AreaRepository();
            _actionRepository = new ActionRepository();
            _actionDateRepository = new ActionDateRepository();
            _producerRepository = new ProducerRepository();
            _actorRepository = new ActorRepository();
            _genreRepository = new GenreRepository();
            _dataRepository = new DataRepository();
            _metroRepository = new MetroRepository();
        }

        //private void InvokeActionLoadedEvent(ActionWeb action)
        //{
        //    DataLoaderActionLoaded handler = ActionLoadedEvent;
        //    if (handler != null)
        //        handler(action);
        //}

        //private void InvokeActionNotLoadedEvent(ActionWeb action)
        //{
        //    DataLoaderActionNotLoaded handler = ActionNotLoadedEvent;
        //    if (handler != null)
        //        handler(action);
        //}

        //private void InvokeFatalErrorEvent(string message)
        //{
        //    FatalError handler = FatalErrorEvent;
        //    if (handler != null)
        //        handler(message);
        //}

        private async Task<int> ParseAction(ActionWeb actionWeb)
        {
            try
            {
                Metro metro = CreateMetro(actionWeb.AreaMetro);
                Area area = CreateArea(actionWeb.AreaName, actionWeb.AreaAddress, actionWeb.AreaDescription,
                    actionWeb.AreaSchemaImage, actionWeb.AreaImage, metro);

                List<Producer> producers = new List<Producer>();
                if (actionWeb.Producer != null && !string.IsNullOrEmpty(actionWeb.Producer))
                    foreach (string producerItem in actionWeb.Producer.Split(','))
                    {
                        Producer producer = CreateProducer(producerItem);
                        if (producer != null && producers.All(i => i.ID != producer.ID))
                            producers.Add(producer);
                    }

                List<Actor> actors = new List<Actor>();
                if (actionWeb.Actors != null && !string.IsNullOrEmpty(actionWeb.Actors))
                    foreach (string actorItem in actionWeb.Actors.Split(','))
                    {
                        Actor actor = CreateActor(actorItem);

                        if (actors.All(i => i.ID != actor.ID))
                            actors.Add(actor);
                    }

                Genre genre = CreateGenre(actionWeb.Genre);

                Action action = CreateAction(actionWeb.Name, actionWeb.AreaName, actionWeb.Date, actionWeb.Time,
                    actionWeb.PriceRange, actionWeb.Description, actionWeb.Duration, actionWeb.Image, area, genre, actors, producers);
                if (action != null)
                {
                    //InvokeActionLoadedEvent(actionWeb);
                    return 1;
                }
                else
                {
                    return 0;
                    //InvokeActionNotLoadedEvent(actionWeb);
                }
            }
            catch (DBAccessFailedException dbAccessExp)
            {
                _logger.Fatal("Не удалось подключение к базе данных.Проверьте параметры подключения.Работа приложения остановлена!");
                return -1;
                //InvokeFatalErrorEvent(dbAccessExp.Message);
                //throw new DBAccessFailedException("Ошибка доступа к Базе данных \"Артис\".Текущая директория:" + AppDomain.CurrentDomain.BaseDirectory);

            }
            catch (Exception ex)
            {
                //InvokeActionNotLoadedEvent(actionWeb);
                _logger.ErrorException("Ошибка записи мероприятия " + actionWeb.Name, ex);
                return -1;
            }
        }

        private Metro CreateMetro(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Metro metro = _metroRepository.GetByName(Name);
                if (metro == null)
                {
                    metro = new Metro() { Name = Name };
                    try
                    {
                        _metroRepository.Add(metro);
                        return metro;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Ошибка записи метро " + Name, ex);
                    }
                }
                else
                {
                    _logger.Info("Метро " + Name + " уже есть");
                    return metro;
                }

            }
            _logger.Info("Пустое метро!");
            return null;
        }
        

        private Data CreateData(string Data)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                Data data = new Data() { Base64StringData = Data };
                try
                {
                    _dataRepository.Add(data);
                    return data;
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Ошибка записи изображения ", ex);
                }
            }
            _logger.Info("Пустое изображение!");
            return null;
        }

        private Producer CreateProducer(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Producer producer = _producerRepository.GetByName(Name, "FIO");
                if (producer == null)
                {
                    producer = new Producer() { FIO = Name };
                    try
                    {
                        _producerRepository.Add(producer);
                        return producer;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Ошибка записи режиссера " + Name, ex);
                    }
                }
                else
                {
                    _logger.Info("Режиссер " + Name + " уже есть");
                    return producer;
                }
            }
            _logger.Info("Пустой режиссер!");
            return null;
        }

        private Actor CreateActor(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Actor actor = _actorRepository.GetByName(Name, "FIO");
                if (actor == null)
                {
                    actor = new Actor() { FIO = Name };
                    try
                    {
                        _actorRepository.Add(actor);
                        return actor;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Ошибка записи актера " + Name, ex);
                    }
                }
                else
                {
                    _logger.Info("Актер " + Name + " уже есть");
                    return actor;
                }
            }
            _logger.Info("Пустой актер!");
            return null;
        }

        private Genre CreateGenre(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Genre genre = _genreRepository.GetByName(Name);
                if (genre == null)
                {
                    genre = new Genre() { Name = Name };
                    try
                    {
                        _genreRepository.Add(genre);
                        return genre;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Ошибка записи жанра " + Name, ex);
                    }
                }
                else
                {
                    _logger.Info("Жанр " + Name + " уже есть");
                    return genre;
                }
            }
            _logger.Info("Пустой жанр!");
            return null;
        }

        private ActionDate CreateActionDate(Action action,Area area, DateTime date, string time, string priceRange, int minPrice = 0, int maxPrice = 0)
        {
            if (!string.IsNullOrEmpty(time) && date > DateTime.MinValue)
            {
                ActionDate data = new ActionDate() { Action = action, Area = area,Date = date, Time = time, PriceRange = priceRange, MinPrice = minPrice, MaxPrice = maxPrice };
                try
                {
                    _actionDateRepository.Add(data);
                    return data;
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Ошибка записи даты для мероприятия ", ex);
                }
            }
            _logger.Info("Пустая дата для мероприятия!");
            return null;
        }

        private Area CreateArea(string Name, string Address, string Description, string SchemaImage, IEnumerable<string> AreaImage, Metro metro)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Area area = _areaRepository.GetByName(Name);
                if (area == null)
                {
                    area = new Area()
                    {
                        Name = Name,
                        Addres = Address,
                        Description = Description
                    };
                    if (metro != null)
                        area.Metro = metro;

                    if (!string.IsNullOrEmpty(SchemaImage))
                        area.SchemaImage = SchemaImage;

                    if (AreaImage != null)
                    {
                        List<Data> dataList = AreaImage.Select(CreateData).ToList();
                        area.Data = dataList;
                    }
                    try
                    {
                        _areaRepository.Add(area);
                        return area;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Ошибка записи площадки " + Name, ex);
                    }
                }
                else
                {
                    _logger.Info("Площадка " + Name + " уже есть");
                    return area;
                }
            }
            _logger.Info("Пустая площадка!");
            return null;
        }

        private Action CreateAction(string Name, string AreaName, string Date, string Time, string PriceRange, string Description,string Duration, IEnumerable<string> Image, Area area, Genre Genre, List<Actor> Actors, List<Producer> Producers)
        {
            Action action = _actionRepository.GetByName(Name);
            if (action == null)
            {
                if (area == null || string.IsNullOrEmpty(Date) || string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(Time))
                {
                    string text = string.Empty;
                    if (area == null)
                        text = "Для мероприятия не задана площадка";
                    if (string.IsNullOrEmpty(Date))
                        text = "Для мероприятия не задана дата начала";
                    if (string.IsNullOrEmpty(Time))
                        text = "Для мероприятия не задано время начала";
                    if (string.IsNullOrEmpty(Name))
                        text = "Для мероприятия не задано имя";

                    _logger.Error("Ошибка записи мероприятия:" + text);
                    return null;
                }

                action = new Action()
                {
                    Name = Name,
                    Duration = Duration
                };

                if (action.Area==null)
                    action.Area=new Collection<Area>();

                action.Area.Add(area);

                //Отключил запись изображений
                //if (Image != null)
                //{
                //    List<Data> dataList = Image.Select(CreateData).Where(data => data != null).ToList();
                //    action.Data = dataList;
                //}

                if (!string.IsNullOrEmpty(Description))
                    action.Description = Description;



                if (Genre != null)
                    action.Genre = Genre;

                if (Actors.Any())
                    action.Actor = Actors;


                if (Producers.Any())
                    action.Producer = Producers;

                try
                {
                    _actionRepository.Add(action);

                    ActionDate actionDate = CreateActionDate(action,area, DateTime.Parse(Date), Time, PriceRange);
                    if (actionDate != null)
                        action.ActionDate = new List<ActionDate>() { actionDate };
                    return action;
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Ошибка записи мероприятия " + Name, ex);
                }

            }
            else
            {
                if (!action.ActionDate.Any(i => i.Date == DateTime.Parse(Date) && i.Time.Equals(Time) && i.Area.Name.Equals(AreaName)))
                {
                    ActionDate actionDate = CreateActionDate(action,area, DateTime.Parse(Date), Time, PriceRange);
                    action.ActionDate.Add(actionDate);
                    return action;
                }

                _logger.Error("Мероприятие "+Name+" на дату "+Date+";"+Time+" в "+AreaName+" уже записано!");
            }
            return null;
        }

    }
}