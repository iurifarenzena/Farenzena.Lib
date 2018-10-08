using System.Runtime.CompilerServices;

namespace Farenzena.Lib.Diagnostic
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string GetCurrentMethodName([CallerMemberName]string caller = "")
        {
            return caller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Must be the type of the class calling this method</typeparam>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string GetCurrentMethodFullName<T>([CallerMemberName]string caller = "")
        {
            var className = typeof(T).FullName;
            return $"{className}.{caller}";
        }
    }
}
