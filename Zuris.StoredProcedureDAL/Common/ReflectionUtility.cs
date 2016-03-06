using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Zuris
{
    public static class ReflectionUtility
    {
        public static PropertyInfo GetPropertyFromExpression(Expression expression)
        {
            PropertyInfo prop = null;
            if (expression is MemberExpression)
            {
                prop = (expression as MemberExpression).Member as PropertyInfo;
            }
            else if (expression is UnaryExpression)
            {
                var unaryExp = (expression as UnaryExpression);
                prop = (unaryExp.Operand as MemberExpression).Member as PropertyInfo;
            }
            return prop;
        }

        /// <summary>
        /// Returns the string version of a strongly-typed class property. Example: ReflectionUtiliy.GetProperty((PagerGroup p) => p.Code) returns "Code". This gives us
        /// the ability to do strongly-typed data lookups on things like data-bound controls that are usually done weakly. Assuming an object called "dataBoundObject" that
        /// we know has been bound to a specific object type, we previously would lookup a databound property like this:
        ///
        /// <example>string data = dataBoundObject.GetColumnValue("Code")</example>
        ///
        /// We can add strong typing by using this utility:
        ///
        /// <p>string columnName = ReflectionUtility.GetProperty((BoundClass b) => b.PropertyName));
        /// string data = dataBoundObject.GetColumnValue(columnName);</p>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="expression">A lambda expression (e.g. (Class c) => c.PropertyName)</param>
        /// <returns></returns>
        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression me = expression.Body as MemberExpression;
            if (me == null)
            {
                throw new ArgumentException("The expression does not reference a class property.");
            }

            return me.Member.Name;
        }

        public static string GetMethodName<T>(Expression<Action<T>> expression)
        {
            MethodCallExpression mce = expression.Body as MethodCallExpression;
            if (mce == null)
            {
                throw new ArgumentException("The expression does not reference a class method.");
            }
            return mce.Method.Name;
        }
    }
}