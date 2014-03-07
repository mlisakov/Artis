using System;
using System.Threading.Tasks;
using NLog;

namespace Artis.Data
{
    public class ProducerRepository : BaseRepository<Producer>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static ProducerRepository _producerRepository;
        private static ActionRepository _actionRepository;

        static ProducerRepository()
        {
            _producerRepository = new ProducerRepository();
            _actionRepository=new ActionRepository();
        }

        public static async Task<bool> AddProducer(Producer producer, Action action)
        {
            try
            {
                _producerRepository.Add(producer);
                action.Producer.Add(producer);
                _actionRepository.Update(action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления актера для мероприятия ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> Remove(Producer producer, Action action)
        {
            try
            {
                //_producerRepository.Delete(producer);
                action.Producer.Remove(producer);
                _actionRepository.Update(action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка удаления продюссера из мероприятия ", ex);
                return false;
            }
            return true;
        }
    }
}