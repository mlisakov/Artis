using System.Collections.Generic;
using Artis.Consts;
using NUnit.Framework;

namespace Artis.ActionParser
{
    [TestFixture]
    public class Test
    {
        private readonly DataFiller _parser=new DataFiller();
        [Test]
        public void CanCrateActions()
        {
            //List<ActionWeb> actionsWeb = new List<ActionWeb>()
            //{
            //    new ActionWeb()
            //    {
            //        Actors = "Н.Антюфеева, з.а.России А.Матвеев, з.а.России Е.Терновая, з.а.России Ю.Давиденко и др.",
            //        AreaAddress = "Санкт-Петербург, ул.Рубинштейна, д.13",
            //        AreaName = "Театр Зазеркалье",
            //        AreaDescription = "Театр «Зазеркалье» основан в 1987",
            //        AreaMetro = "Достоевская",
            //        AreaArea = "Центральный",
            //        Producer = "И.Черкасова",
            //        Author = "В.Рывкин",
            //        Name = "Мюзикл для детей «Людвиг и Тутта, или Лессайдская история»",
            //        Description = "«Зверьский мюзикл» по мотивам сказки ",
            //        Rating = "6+",
            //        Date = "31.01.2014",
            //        Time = "11:00",
            //        Genre = "детский спектакль с 6 лет",
            //        Duration = "01:40",
            //        PriceRange = "300 p. "
            //    },
            //     new ActionWeb()
            //    {
            //        AreaAddress = "Санкт-Петербург, Московский пр., д.121",
            //        AreaName = "Кукольный театр сказки",
            //        AreaDescription = "Санкт-Петербургский государственный кукольный театр сказки ",
            //        AreaMetro = "Московские ворота",
            //        AreaArea = "Московский",
            //        Actors = "Е.Стукалова, Ф.Игнатьев, И.Всеволодов, з.а.РФ Э.Куликова, Л.Иевлева, С.Пенкин",
            //        Producer = "Ф.Игнатьев",
            //        Author = "Ф.Игнатьев",
            //        Name = "Спектакль для детей «Дикие лебеди»",
            //        Description = "Волшебная сказка о любви и зависти ",
            //        Rating = "6+",
            //        Date = "31.01.2014",
            //        Time = "11:00",
            //        Genre = "ТЮЗ зрители от 7 лет",
            //        Duration = "01:30",
            //        PriceRange = "200-300 p. "
            //    },
            //};
            //_parser.ParseAtions(actionsWeb);
        }
    }
}
