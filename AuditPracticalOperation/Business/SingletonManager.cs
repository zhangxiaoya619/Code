using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business
{
    public static class SingletonManager
    {
        private static IDictionary<Type, object> singletons;

        public static T Get<T>()
        {
            if (!singletons.ContainsKey(typeof(T)))
                throw new NotSupportedException("只能获取带有SingletonAttribute特性的单例。");

            return (T)singletons[typeof(T)];
        }

        static SingletonManager()
        {
            singletons = new Dictionary<Type, object>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    object[] attrs = type.GetCustomAttributes(typeof(SingletonAttribute), true);
                    if (!type.IsAbstract && attrs != null && attrs.Length > 0)
                    {
                        if (type.GetConstructors().Length > 0)
                            throw new NotSupportedException(string.Format("带有SingletonAttribute特性的{0}不能含有公有构造函数。", type.Name));

                        if (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new Type[] { }, null) == null)
                            throw new NotSupportedException(string.Format("带有SingletonAttribute特性的{0}必须含有私有无参构造函数。", type.Name));

                        singletons.Add(type, Activator.CreateInstance(type, true));
                    }
                }
            }
        }
    }

    public class SingletonAttribute : Attribute { }
}
