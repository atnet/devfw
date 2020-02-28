﻿using System;
using System.Collections.Generic;
using System.Web;

namespace JR.DevFw.Framework.Xml.AutoObject
{
    public class AutoXmlTest
    {
        public static void Test()
        {
            Xml.AutoObject.AutoObjectXml ax =
                new Xml.AutoObject.AutoObjectXml(AppDomain.CurrentDomain.BaseDirectory + "templet/template_dict.xml");

            ax.RemoveAllObjects();

            //ax.RemoveObjects("Pro");

            ax.InsertObjectNode("Temp", "客户", "", new XmlObjectProperty("ID", "客户编号", "1234\nretyt\nfdsf"),
                new XmlObjectProperty("RealName", "真实姓名"));
            ax.InsertFromDLL(AppDomain.CurrentDomain.BaseDirectory + "bin/spc.dll", null);
            ax.Flush();

            HttpResponse response = HttpContext.Current.Response;

            response.Write(XmlObjectDoc.DocStyleSheet);

            //显示单个对象文档
            // XmlObject obj = ax.GetObject("Temp");
            // response.Write(XmlObjectDoc.GetGrid(obj,-1));


            IList<XmlObject> objects = new List<XmlObject>(ax.GetObjects());

            response.Write("<h1>Objects</h1><ul>");
            for (int i = 0; i < objects.Count; i++)
            {
                response.Write("<li><a href=\"#object_");
                response.Write(objects[i].Key);
                response.Write("\">");
                response.Write(objects[i].Name);
                response.Write("(");
                response.Write(objects[i].Key);
                response.Write(")");
                response.Write("</a></li>");
            }

            response.Write("</ul>");

            for (int i = 0; i < objects.Count; i++)
            {
                response.Write(XmlObjectDoc.GetGrid(objects[i], i + 1));
            }
        }
    }
}