using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProjectEye.Core.Service
{
    public class ServiceCollection
    {
        /// <summary>
        /// 已创建的实例
        /// </summary>
        private IList<object> instanceList;
        public ServiceCollection()
        {
            instanceList = new List<object>();
        }
        public void Add<T>() where T : IService
        {
            var type = typeof(T);
            CreateInstance(type);
        }
        public void AddInstance(object obj)
        {
            instanceList.Add(obj);
        }
        public void Initialize()
        {
            foreach (var instance in instanceList)
            {
                MethodInfo method = instance.GetType().GetMethod("Init");
                if (method != null)
                {
                    method.Invoke(instance, null);
                }
            }
        }
        //public void AddViews()
        //{
        //    string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        //    string viewsNameSpace = assemblyName + ".Views";
        //    var viewTypeList = from t in Assembly.GetExecutingAssembly().GetTypes()
        //                       where t.IsClass && t.Namespace == viewsNameSpace
        //                       select t;
        //    foreach (var viewType in viewTypeList)
        //    {

        //    }
        //    //q.ToList().ForEach(t => Debug.WriteLine(t.Name));
        //}
        /// <summary>
        /// 创建实例
        /// </summary>
        private void CreateInstance(Type type)
        {
            var constructorInfoObj = type.GetConstructors()[0];
            var constructorParameters = constructorInfoObj.GetParameters();
            int constructorParametersLength = constructorParameters.Length;
            Type[] types = new Type[constructorParametersLength];
            object[] objs = new object[constructorParametersLength];
            for (int i = 0; i < constructorParametersLength; i++)
            {
                string typeFullName = constructorParameters[i].ParameterType.FullName;
                Type t = Type.GetType(typeFullName);
                types[i] = t;

                objs[i] = GetInstance(typeFullName);

            }
            ConstructorInfo ctor = type.GetConstructor(types);
            object instance = ctor.Invoke(objs);
            if (instance != null)
            {
                instanceList.Add(instance);
                
            }
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public object GetInstance(string typeFullName)
        {
            var result = instanceList.Where(m => m.GetType().FullName == typeFullName);
            if (result.Count() > 0)
            {
                return result.Single();
            }
            return null;
        }
    }
}
