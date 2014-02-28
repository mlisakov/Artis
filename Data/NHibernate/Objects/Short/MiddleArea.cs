namespace Artis.Data
{
    public class MiddleArea:ShortArea
    {
        private string _metro;
        private string _description;

        /// <summary>
        /// Индекс
        /// </summary>
        public virtual string Metro
        {
            get { return _metro; }
            set { _metro = value; }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public MiddleArea(Area area) : base(area)
        {
            Description = area.Description;
            if (area.Metro != null)
                Metro = area.Metro.Name;
        }
    }
}
