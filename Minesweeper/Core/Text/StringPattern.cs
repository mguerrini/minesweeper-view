namespace Minesweeper.Core.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public abstract class StringPattern
    {
        protected Regex Pattern { get; set; }

        public virtual bool IsLike(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return this.Pattern.IsMatch(input);
        }

        public override string ToString()
        {
            return "Pattern: " + this.Pattern.ToString();
        }
    }
}
