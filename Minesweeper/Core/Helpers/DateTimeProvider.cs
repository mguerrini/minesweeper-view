using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core.Helpers
{
    public class DateTimeProvider
    {
        private static DateTimeProvider _instance = new DateTimeProvider();

        public static DateTimeProvider Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static DateTime GetNow()
        {
            return Instance.Now;
        }

        public static DateTime GetZero()
        {
            return DateTime.MinValue;
        }

        public virtual DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public virtual DateTime Zero
        {
            get
            {
                return DateTime.MinValue;
            }
        }
    }
}
