using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace IocExample
{
    public class DependencyResolver
    {
        private readonly Dictionary<Type, object> bindedObjects;
        private readonly Dictionary<Type, Func<object>> bindedConstructors;
        private readonly Dictionary<Type, Type> bindedTypes;
        
        public DependencyResolver()
        {
            bindedObjects = new Dictionary<Type, object>();
            bindedConstructors = new Dictionary<Type, Func<object>>();
            bindedTypes = new Dictionary<Type, Type>();
        }

        public void Bind<T1, T2>(Func<T2> func, bool isSingletonScope = false) where T2: T1 where T1: class 
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            
            Unbind<T1>();
            
            var type = typeof(T1);
            var objFunc = new Func<object>(() => (object) func());
            
            if (isSingletonScope)
                bindedObjects[type] = objFunc.Invoke();
            else
                bindedConstructors[type] = objFunc;
        }

        public void Bind<T1, T2>() where T2: T1 where T1: class
        {
            Unbind<T1>();
            
            bindedTypes[typeof(T1)] = typeof(T2);
        }

        public void Unbind<T>()
        {
            var unbindingType = typeof(T);
            
            if (bindedTypes.ContainsKey(unbindingType)) bindedTypes.Remove(unbindingType);
            if (bindedObjects.ContainsKey(unbindingType)) bindedObjects.Remove(unbindingType);
            if (bindedConstructors.ContainsKey(unbindingType)) bindedConstructors.Remove(unbindingType);
        }

        private object Get(Type type)
        {
            if (bindedObjects.ContainsKey(type)) return bindedObjects[type];
            if (bindedConstructors.ContainsKey(type)) return bindedConstructors[type].Invoke();
            
            if (!bindedTypes.ContainsKey(type)) throw new ArgumentException("Type is not binded");

            var bindedType = bindedTypes[type];
            var parameters = Utils.GetSingleConstructor(bindedType)
                .GetParameters()
                .Select(x => Get(x.ParameterType))
                .ToList();
            
            return Utils.CreateInstance(bindedType, parameters);
        }

        public T Get<T>() where T: class
        {
            return (T) Get(typeof(T));
        }
    }
}