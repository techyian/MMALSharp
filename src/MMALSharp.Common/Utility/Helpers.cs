using System;
using System.Linq.Expressions;

namespace MMALSharp
{
    public class Helpers
    {
        public static string ConvertBytesToMegabytes(long bytes)
        {
            return ((bytes / 1024f) / 1024f).ToString("0.00mb");
        }

        public static void PrintWarning(string msg)
        {
            Console.WriteLine("Warning: " + msg);
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
}
