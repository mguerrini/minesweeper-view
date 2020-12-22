namespace Minesweeper.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Linq.Expressions;
    using System.Reflection;


    public class ExpressionHelper
    {
        #region -- GetValues --

        public static object GetValue<T>(T valueOwner, Expression<Func<T, object>> exp)
        {
            try
            {
                return exp.Compile().Invoke(valueOwner);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static TOutput GetValue<T, TOutput>(T valueOwner, Expression<Func<T, TOutput>> exp)
        {
            try
            {
                return exp.Compile().Invoke(valueOwner);
            }
            catch 
            {
                throw;
            }
        }


        public static string GetString<T>(T valueOwner, Expression<Func<T, string>> exp)
        {
            return GetValue<T, string>(valueOwner, exp);
        }

        public static int GetInt<T>(T valueOwner, Expression<Func<T, int>> exp)
        {
            return GetValue<T, int>(valueOwner, exp);
        }

        public static bool GetBool<T>(T valueOwner, Expression<Func<T, bool>> exp)
        {
            return GetValue<T, bool>(valueOwner, exp);
        }

        public static double GetDouble<T>(T valueOwner, Expression<Func<T, double>> exp)
        {
            return GetValue<T, double>(valueOwner, exp);
        }

        #endregion


        #region -- Get Path --

        public static PropertyInfo GetFirstProperty(Expression exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp, out memberExp))
                return null;

            do
            {
                PropertyInfo info = memberExp.Member as PropertyInfo;
                if (info != null)
                    return info;
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return null; 
        }

        public static List<string> GetPropertyNamePath(Expression exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp, out memberExp))
                return new List<string>();

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return new List<string>(memberNames);
        }


        public static List<PropertyInfo> GetPropertyInfoPath(Expression exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp, out memberExp))
                return new List<PropertyInfo>();

            var members = new Stack<PropertyInfo>();
            do
            {
                PropertyInfo info = memberExp.Member as PropertyInfo;

                if (info == null)
                    throw new Exception("The path is not formed only from PropertyInfos");

                members.Push(info);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return new List<PropertyInfo>(members);
        }


        public static string GetFullPropertyName(Expression exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        public static string GetFullPropertyName<T, TOutput>(Expression<Func<T, TOutput>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            while(exp.NodeType == ExpressionType.Lambda)
                exp = ((LambdaExpression)exp).Body;

            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                    return true;
            }
            else if (exp.NodeType == ExpressionType.MemberAccess)
            {
                memberExp = exp as MemberExpression;
                return true;
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked 
            );
        }

        #endregion
    }
}
