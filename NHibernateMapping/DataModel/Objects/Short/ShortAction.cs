using System.Linq;

namespace Artis.Data
{
    public class ShortAction:SmallAction
    {
        private string _smallImage;
        private bool _isVerticalSmallImage;

        /// <summary>
        /// Маленькое изображение для мероприятия
        /// </summary>
        public string SmallImage
        {
            get { return _smallImage; }
            set { _smallImage = value; }
        }

        /// <summary>
        /// Направление маленького изображение для мероприятия
        /// </summary>
        public bool IsVerticalSmallImage
        {
            get { return _isVerticalSmallImage; }
            set { _isVerticalSmallImage = value; }
        }

        public ShortAction(ActionDate actionDate)
            : base(actionDate)
        {
            if (actionDate.Action.DataSmall != null && actionDate.Action.DataSmall.Any())
                SmallImage = actionDate.Action.DataSmall.First().Base64StringData;
            IsVerticalSmallImage = actionDate.Action.IsVerticalSmallImage;
        }
    }
}
