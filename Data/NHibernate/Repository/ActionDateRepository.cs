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

        private  ActionRepository _actionRepository;
        private  ActorRepository _actorRepository;
        private  ProducerRepository _producerRepository;
        private DataRepository _dataRepository;

        public ActionDateRepository()
        {
            _actionRepository=new ActionRepository();
            _actorRepository=new ActorRepository();
            _producerRepository=new ProducerRepository();
            _dataRepository=new DataRepository();
        }

        public async Task<bool> Save(ActionDate action, List<Data> addedImages, List<long> deletedImages, List<Data> smallAddedImages, List<Actor> actors, List<Producer> producers)
        {
            try
            {
                Action originalAction = _actionRepository.GetById(action.Action.ID);
                ActionDate originalActionDate = GetById(action.ID);

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
                if (smallAddedImages != null)
                    foreach (Data data in smallAddedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (originalAction.DataSmall == null)
                            originalAction.DataSmall = new Collection<Data>();
                        originalAction.DataSmall.Add(data);
                    }
                CompareAction(originalAction, action.Action,actors,producers);
                CompareActionDate(originalActionDate, action);

                Update(originalActionDate);
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
        private void CompareAction(Action originalAction, Action action, List<Actor> actors, List<Producer> producers)
        {
            if (originalAction.Name==null || !originalAction.Name.Equals(action.Name))
                originalAction.Name = action.Name;

            if (originalAction.EnglishName==null || !originalAction.EnglishName.Equals(action.EnglishName))
                originalAction.EnglishName = action.EnglishName;

            if (originalAction.Rating==null || originalAction.Rating != action.Rating)
                originalAction.Rating = action.Rating;

            if (originalAction.Duration==null || !originalAction.Duration.Equals(action.Duration))
                originalAction.Duration = action.Duration;

            if (originalAction.Description==null || !originalAction.Description.Equals(action.Description))
                originalAction.Description = action.Description;

            if (originalAction.EnglishDescription==null || !originalAction.EnglishDescription.Equals(action.EnglishDescription))
                originalAction.EnglishDescription = action.EnglishDescription;

            if (originalAction.Genre.ID!=action.Genre.ID)
                originalAction.Genre = action.Genre;

            UpdateActors(originalAction, actors);

            UpdateProducers(originalAction, producers);
        }

        private void UpdateProducers(Action originalAction, List<Producer> producers)
        {
            List<Producer> currentProducers = originalAction.Producer.ToList();
            List<Producer> deletedProducers = currentProducers.Except(producers, new ProducerComparer()).ToList();
            List<Producer> addedProducers = producers.Except(currentProducers, new ProducerComparer()).ToList();
            List<Producer> resProducers = currentProducers.Except(deletedProducers, new ProducerComparer()).ToList();
            foreach (Producer producer in addedProducers)
            {
                //_producerRepository.Add(producer);
                //originalAction.Producer.Add(producer);
                foreach (Data image in producer.Data)
                    _dataRepository.Add(image);
                _producerRepository.Add(producer);
                originalAction.Producer.Add(producer);
            }

            foreach (Producer producer in resProducers)
            {
                Producer changedActor = producers.First(i => i.ID == producer.ID);

                if (producer.FIO != changedActor.FIO)
                    producer.FIO = changedActor.FIO;
                if (producer.EnglishFIO != changedActor.EnglishFIO)
                    producer.EnglishFIO = changedActor.EnglishFIO;
                if (producer.Description != changedActor.Description)
                    producer.Description = changedActor.Description;
                if (producer.EnglishDescription != changedActor.EnglishDescription)
                    producer.EnglishDescription = changedActor.EnglishDescription;

                //_actorRepository.Update(actor);
                List<Data> currentImages = producer.Data.ToList();
                List<Data> deletedImages = currentImages.Except(changedActor.Data.ToList(), new DataComparer()).ToList();
                List<Data> addedImages = changedActor.Data.Where(i => i.ID <= 0).ToList();
                foreach (Data image in deletedImages)
                {
                    producer.Data.Remove(image);
                }
                foreach (Data image in addedImages)
                {
                    Data data = new Data() { Base64StringData = image.Base64StringData };
                    _dataRepository.Add(data);
                    producer.Data.Add(data);
                }
                _producerRepository.Update(producer);
            }

            foreach (Producer producer in deletedProducers)
                originalAction.Producer.Remove(producer);
        }

        private void UpdateActors(Action originalAction, List<Actor> actors)
        {
            List<Actor> currentActors = originalAction.Actor.ToList();
            List<Actor> deletedActors = currentActors.Except(actors, new ActorComparer()).ToList();
            List<Actor> addedActors = actors.Except(currentActors, new ActorComparer()).ToList();
            List<Actor> restActors = currentActors.Except(deletedActors, new ActorComparer()).ToList();

            foreach (Actor actor in addedActors)
            {
                foreach (Data image in actor.Data)
                    _dataRepository.Add(image);
                _actorRepository.Add(actor);
                originalAction.Actor.Add(actor);
            }

            foreach (Actor actor in restActors)
            {
                Actor changedActor = actors.First(i => i.ID == actor.ID);

                if (actor.FIO != changedActor.FIO)
                    actor.FIO = changedActor.FIO;
                if (actor.EnglishFIO != changedActor.EnglishFIO)
                    actor.EnglishFIO = changedActor.EnglishFIO;
                if (actor.Description != changedActor.Description)
                    actor.Description = changedActor.Description;
                if (actor.EnglishDescription != changedActor.EnglishDescription)
                    actor.EnglishDescription = changedActor.EnglishDescription;

                //_actorRepository.Update(actor);
                List<Data> currentImages = actor.Data.ToList();
                List<Data> deletedImages = currentImages.Except(changedActor.Data.ToList(), new DataComparer()).ToList();
                List<Data> addedImages = changedActor.Data.Where(i => i.ID <= 0).ToList();
                foreach (Data image in deletedImages)
                {
                    actor.Data.Remove(image);
                }
                foreach (Data image in addedImages)
                {
                    Data data = new Data() {Base64StringData = image.Base64StringData};
                    _dataRepository.Add(data);
                    actor.Data.Add(data);
                }
                _actorRepository.Update(actor);
            }


            foreach (Actor actor in deletedActors)
                originalAction.Actor.Remove(actor);
        }

        private void CompareActionDate(ActionDate originalActionDate, ActionDate actionDate)
        {
            if (originalActionDate.Date != actionDate.Date)
                originalActionDate.Date = actionDate.Date;
            if (!originalActionDate.Time.Equals(actionDate.Time))
                originalActionDate.Time = actionDate.Time;
            if (originalActionDate.PriceRange != actionDate.PriceRange)
                originalActionDate.PriceRange = actionDate.PriceRange;
            if (originalActionDate.EnglishPriceRange != actionDate.EnglishPriceRange)
                originalActionDate.EnglishPriceRange = actionDate.EnglishPriceRange;
            if (originalActionDate.MinPrice != actionDate.MinPrice)
                originalActionDate.MinPrice = actionDate.MinPrice;
            if (originalActionDate.MaxPrice != actionDate.MaxPrice)
                originalActionDate.MaxPrice = actionDate.MaxPrice;
        }

        public async Task<bool> Add(ActionDate actionDate, List<Data> images, List<Actor> actors, List<Producer> producers)
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
                Add(actionDateNew);

                
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

        public async Task<bool> AddActionDate(ActionDate actionDate)
        {
            try
            {
                Add(actionDate);
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
