namespace Minesweeper.Core.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;


    public class LikePattern : StringPattern
    {
        #region -- Constructores --

        public LikePattern(string pattern, bool ignoreCase)
        {
            Regex regex;

            pattern = "^" + pattern
                           .Replace(".", "\\.")
                           .Replace("%", ".*")
//                           .Replace("\\.*", "\\%")
                           + "$";

            if (ignoreCase)
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            else
                regex = new Regex(pattern);

            this.Pattern = regex;
        }

        #endregion
    }
}
