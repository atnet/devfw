﻿/* *
 * name     :服务管理类
 * author   :OPS newmin
 * date     :09/20 2010
 * note     :继承此类需在子类的静态构造函数中添加代码,如下:         
 *           serviceDict.Add(typeof(IP), ConfigurationManager.AppSettings["srv_ip"]);
 * */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace JR.DevFw.Framework.Service
{
    /// <summary>
    /// 服务管理
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class ServiceManager<T> : IService<T> where T : MarshalByRefObject
    {
        /// <summary>
        /// 服务配置字典
        /// </summary>
        protected static IDictionary<Type, string> serviceDict = new Dictionary<Type, string>();

        /// <summary>
        /// 服务配置链接
        /// </summary>
        protected static IDictionary<string, string> serviceUriDict = new Dictionary<string, string>();

        /// <summary>
        /// 根据配置文件初始化服务
        /// </summary>
        /// <param name="configFile"></param>
        protected static void Init(string configFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlNodeList xs = doc.SelectNodes("/serviceConfig/service");
            //服务的类型
            string[] ta; //TypeArr
            Assembly a;
            foreach (XmlNode xn in xs)
            {
                ta = xn.Attributes["type"].Value.Split(',');
                a = Assembly.Load(ta[1]);
                serviceDict.Add(a.GetType(ta[0], true, true), xn.Attributes["objectUri"].Value);
            }
            //获取Uri
            xs = doc.SelectNodes("/serviceConfig/serviceUri/add");
            foreach (XmlNode xn in xs)
            {
                serviceUriDict.Add(xn.Attributes["name"].Value, xn.Attributes["uri"].Value);
            }
        }

        #region IService<T> 成员

        public T GetInstance()
        {
            Type type = typeof (T);
            return (T) Activator.GetObject(typeof (T), serviceDict[typeof (T)]);
        }

        #endregion
    }
}