using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace Artis.Data
{
    public class ActionDateRepository:BaseRepository<ActionDate>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static ActionDateRepository _actionDateRepository;
        private static ActionRepository _actionRepository;
        private static ActorRepository _actorRepository;
        private static ProducerRepository _producerRepository;

        static ActionDateRepository()
        {
            _actionDateRepository=new ActionDateRepository();
            _actionRepository=new ActionRepository();
            _actorRepository=new ActorRepository();
            _producerRepository=new ProducerRepository();
        }

        public static async Task<bool> Save(ActionDate action, List<Data> addedImages, List<long> deletedImages, List<Actor> actors, List<Producer> producers)
        {
            try
            {
                Action originalAction = _actionRepository.GetById(action.Action.ID);
                ActionDate originalActionDate = _actionDateRepository.GetById(action.ID);

                if (deletedImages != null)
                    foreach (long idImage in deletedImages)
                    {
                        Data item = originalAction.Data.First(i => i.ID == idImage);
                        originalAction.Data.Remove(item);
                    }

                if (addedImages != null)
                    foreach (Data data in addedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (originalAction.Data == null)
                            originalAction.Data = new Collection<Data>();
                        originalAction.Data.Add(data);
                    }
                CompareAction(originalAction, action.Action,actors,producers);
                CompareActionDate(originalActionDate, action);

                _actionDateRepository.Update(originalActionDate);
                _actionRepository.Update(originalAction);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка обновления мероприятия ", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Сравнивает исходное мероприятие и мероприятия, полученное в ходу изменений от клиента
        /// </summary>
        /// <param name="originalAction">Текущее мероприятие</param>
        /// <param name="action">Измененное мероприятие</param>
        private static void CompareAction(Action originalAction, Action action, List<Actor> actors, List<Producer> producers)
        {
            if (!originalAction.Name.Equals(action.Name))
                originalAction.Name = action.Name;
            if (originalAction.Rating != action.Rating)
                originalAction.Rating = action.Rating;
            if (!originalAction.Duration.Equals(action.Duration))
                originalAction.Duration = action.Duration;
            if (!originalAction.Description.Equals(action.Description))
                originalAction.Description = action.Description;
            if (originalAction.Genre.ID!=action.Genre.ID)
                originalAction.Genre = action.Genre;

            List<Actor> currentActors = originalAction.Actor.ToList();
            List<Actor> deletedActors = currentActors.Except(actors, new ActorComparer()).ToList();
            List<Actor> addedActors = actors.Except(currentActors, new ActorComparer()).ToList();
            foreach (Actor actor in addedActors)
            {
                _actorRepository.Add(actor);
                originalAction.Actor.Add(actor);
            }


            foreach (Actor actor in deletedActors)
                originalAction.Actor.Remove(actor);

            List<Producer> currentProducers = originalAction.Producer.ToList();
            List<Producer> deletedProducers = currentProducers.Except(producers, new ProducerComparer()).ToList();
            List<Producer> addedProducers = producers.Except(currentProducers, new ProducerComparer()).ToList();
            foreach (Producer producer in addedProducers)
            {
                _producerRepository.Add(producer);
                originalAction.Producer.Add(producer);
            }

            foreach (Producer producer in deletedProducers)
                originalAction.Producer.Remove(producer);

            //_actionRepository.Update(originalAction);

            //foreach (Actor actor in deletedActors)
            //     _actorRepository.Delete(actor);

            //foreach (Producer producer in deletedProducers)
            //    _producerRepository.Delete(producer);


        }

        private static void CompareActionDate(ActionDate originalActionDate, ActionDate actionDate)
        {
            if (originalActionDate.Date != actionDate.Date)
                originalActionDate.Date = actionDate.Date;
            if (!originalActionDate.Time.Equals(actionDate.Time))
                originalActionDate.Time = actionDate.Time;
            if (originalActionDate.PriceRange != actionDate.PriceRange)
                originalActionDate.PriceRange = actionDate.PriceRange;
            if (originalActionDate.MinPrice != actionDate.MinPrice)
                originalActionDate.MinPrice = actionDate.MinPrice;
            if (originalActionDate.MaxPrice != actionDate.MaxPrice)
                originalActionDate.MaxPrice = actionDate.MaxPrice;
        }

        public static async Task<bool> Add(ActionDate actionDate, List<Data> images, List<Actor> actors, List<Producer> producers)
        {
            try
            {
                Action action = new Action()
                {
                    Name = actionDate.Action.Name,
                    Rating = actionDate.Action.Rating,
                    Duration = actionDate.Action.Duration,
                    Description = actionDate.Action.Description,
                    Genre = actionDate.Action.Genre,
                    
                };

                if (images != null)
                {
                    DataRepository _dataRepository = new DataRepository();
                    foreach (Data data in images)
                    {
                        _dataRepository.Add(data);
                        if (action.Data == null)
                            action.Data = new Collection<Data>();
                        action.Data.Add(data);
                    }
                }
                if (actors != null)
                {
                    foreach (Actor actor in actors)
                    {

                        _actorRepository.Add(actor);
                        if (action.Actor == null)
                            action.Actor = new Collection<Actor>();
                        action.Actor.Add(actor);
                    }
                }

                if (producers != null)
                {
                    foreach (Producer producer in producers)
                    {

                        _producerRepository.Add(producer);
                        if (action.Producer == null)
                            action.Producer = new Collection<Producer>();
                        action.Producer.Add(producer);
                    }
                }

                _actionRepository.Add(action);

                ActionDate actionDateNew=new ActionDate()
                {
                    PriceRange = actionDate.PriceRange,
                    Time=actionDate.Time,
                    Action = action,
                    Area =  actionDate.Area,
                    Date = actionDate.Date,
                };
                _actionDateRepository.Add(actionDateNew);

                
                //action.ActionDate=new Collection<ActionDate>(){actionDateNew};
                //_actionRepository.Update(action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления мероприятия ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> AddActionDate(ActionDate actionDate)
        {
            try
            {
                _actionDateRepository.Add(actionDate);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления даты проведения мероприятия ", ex);
                return false;
            }
            return true;
        }
    }
}
