namespace Minesweeper.Core.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class WildcardPattern : StringPattern
    {
        #region -- Constructores --

        public WildcardPattern(string pattern, bool ignoreCase)
        {
            Regex regex;

            pattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

            if (ignoreCase)
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            else
                regex = new Regex(pattern);

            this.Pattern = regex;
        }

        #endregion
    }
}
