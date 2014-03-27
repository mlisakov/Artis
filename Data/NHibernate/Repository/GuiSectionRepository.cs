using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Artis.Data
{
    public class GuiSectionRepository : BaseRepository<GuiSection>
    {
         /// <summary>
        /// Логгер
        /// </summary>
        private  NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<bool> Save(long currentGuiSection, List<Genre> usedGenres)
        {
            try
            {
                GuiSection section = GetById(currentGuiSection);
                ObservableCollection<Genre> currentGenres =
                    await DataRequestFactory.GetGuiSectionGenres(currentGuiSection);
                IEnumerable<Genre> deletedGenres = currentGenres.Except(usedGenres, new GenreComparer());
                IEnumerable<Genre> addedGenres = usedGenres.Except(currentGenres, new GenreComparer());
                foreach (Genre genre in addedGenres)
                    section.Genre.Add(genre);
                foreach (Genre genre in deletedGenres)
                    section.Genre.Remove(genre);
                Update(section);
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка жанров для секции интерфейса ", ex);
            }
            return false;
        }
    }
}
