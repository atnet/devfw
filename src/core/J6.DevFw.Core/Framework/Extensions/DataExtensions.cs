﻿/*
 * Copyright 2010 OPS,All rights reseved !
 * name     : 泛型扩展
 * author   : newmin
 * date     : 2010/11/08 07:31
 * 
 * 2010/12/13 00:30 newmin [!]:添加数据表字段为DBNull赋值给对象异常时候忽略的处理
 */
namespace Ops.Framework.Extensions
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public static class DataTableExtensions
    {

        /// <summary>
        ///将当前DataTable中的行转换成实体集合(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static IList<T> ToEntityList<T>(this DataTable table) where T : new()
        {
            IList<T> list = new List<T>();
            T t;
            object rowData;

            //获取各自的属性
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            PropertyInfo _p;


            int propertiesCount = -1,              //对应的属性数量
                hasFoundPropertiesCount = 0;        //已经赋值的数量


            foreach (DataRow dr in table.Rows)
            {
                t = new T();
                foreach (DataColumn c in table.Columns)
                {
                    _p = Array.Find(props, a => string.Compare(a.Name, c.ColumnName, true) == 0);
                    if (_p == null) continue;

                    rowData = dr[c.ColumnName];
                    try
                    {
                        if (!(rowData is DBNull))
                        {
                            if (_p.PropertyType.IsEnum)
                            {
                                if (rowData.GetType() == typeof(Int32))
                                {
                                    _p.SetValue(t, Convert.ToInt32(rowData), null);
                                }
                                else
                                {
                                    _p.SetValue(t, Enum.Parse(_p.PropertyType, rowData.ToString()), null);
                                }
                            }
                            else
                            {
                                _p.SetValue(t, Convert.ChangeType(rowData, _p.PropertyType), null);
                            }
                        }

                    }
                    catch { continue; }

                    //计算是否已经赋值完成
                    if (++hasFoundPropertiesCount == propertiesCount)
                    {
                        hasFoundPropertiesCount = 0;
                        break;
                    }
                }
                list.Add(t);


                //对所有属性赋值后跳出循环
                if (propertiesCount == -1)
                {
                    propertiesCount = hasFoundPropertiesCount;
                }
            }
            return list;
        }

        /// <summary>
        ///将DataReader中的行转换成实体集合(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static IList<T> ToEntityList<T>(this DbDataReader reader) where T : new()
        {
            IList<T> list = new List<T>();
            T t;

            //获取表结构
            DataTable schemaTable = reader.GetSchemaTable();

            //表结构列名
            string columnName;

            //获取泛型的的所有属性
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            PropertyInfo pro;
            object rowData;

            int propertiesCount = -1,              //对应的属性数量
                hasFoundPropertiesCount = 0;        //已经赋值的数量

            while (reader.Read())
            {
                t = new T();

                foreach (DataRow dr in schemaTable.Rows)
                {
                    columnName = dr["ColumnName"].ToString();
                    pro = Array.Find(props, a => string.Compare(a.Name, columnName, true) == 0);
                    if (pro == null) continue;

                    //如果数据库中的值不为空，则赋值
                    if (!((rowData = reader[columnName]) is DBNull))
                    {
                        try
                        {
                            if (pro.PropertyType.IsEnum)
                            {
                                if (rowData.GetType() == typeof(Int32))
                                {
                                    pro.SetValue(t, Convert.ToInt32(rowData), null);
                                }
                                else
                                {
                                    pro.SetValue(t, Enum.Parse(pro.PropertyType, rowData.ToString()), null);
                                }
                            }
                            else
                            {
                                pro.SetValue(t, Convert.ChangeType(rowData, pro.PropertyType), null);
                            }
                        }

                        catch
                        {
                            continue;
                        }

                        //计算是否已经赋值完成
                        if (++hasFoundPropertiesCount == propertiesCount)
                        {
                            hasFoundPropertiesCount = 0;
                            break;
                        }
                    }
                }
                list.Add(t);


                //对所有属性赋值后跳出循环
                if (propertiesCount == -1)
                {
                    propertiesCount = hasFoundPropertiesCount;
                }
            }
            return list;
        }

        public static T ToEntity<T>(this DataRow row) where T : new()
        {
            //获取各自的属性
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            PropertyInfo _p;
            object rowData;

            T t = new T();

            foreach (DataColumn c in row.Table.Columns)
            {
                _p = Array.Find(props, a => string.Compare(a.Name, c.ColumnName, true) == 0);
                rowData = row[c.ColumnName];

                try
                {
                    if (_p != null && !(rowData is DBNull))
                    {
                        if (_p.PropertyType.IsEnum)
                        {
                            if (rowData.GetType() == typeof(Int32))
                            {
                                _p.SetValue(t, Convert.ToInt32(rowData), null);
                            }
                            else
                            {
                                _p.SetValue(t, Enum.Parse(_p.PropertyType, rowData.ToString()), null);
                            }
                        }
                        else
                        {
                            _p.SetValue(t, Convert.ChangeType(rowData, _p.PropertyType), null);
                        }
                    }
                }
                catch { continue; }
            }
            return t;
        }

        /// <summary>
        ///将DataReader转换成实体(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static T ToEntity<T>(this DbDataReader reader) where T : new()
        {

            T t = new T();

            //获取表结构
            DataTable schemaTable = reader.GetSchemaTable();

            //表结构列名
            string columnName;

            //获取泛型的的所有属性
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            PropertyInfo pro;
            object rowData;


            int propertiesCount = props.Length,     //属性数量
                hasFoundPropertiesCount = 0;        //已经赋值的数量

            if (reader.Read())
            {
                foreach (DataRow dr in schemaTable.Rows)
                {
                    columnName = dr["ColumnName"].ToString();
                    pro = Array.Find(props, a => string.Compare(a.Name, columnName, true) == 0);
                    if (pro == null) continue;


                    if (!((rowData = reader[columnName]) is DBNull))
                    {
                        try
                        {
                            if (pro.PropertyType.IsEnum)
                            {
                                if (rowData.GetType() == typeof(Int32))
                                {
                                    pro.SetValue(t, Convert.ToInt32(rowData), null);
                                }
                                else
                                {
                                    pro.SetValue(t, Enum.Parse(pro.PropertyType, rowData.ToString()), null);
                                }
                            }
                            else
                            {
                                pro.SetValue(t, Convert.ChangeType(rowData, pro.PropertyType), null);
                            }
                        }
                        catch { continue; }
                    }

                    if (++hasFoundPropertiesCount == propertiesCount)
                    {
                        hasFoundPropertiesCount = 0;
                        break;
                    }
                }
                return t;
            }
            return default(T);
        }


        /// <summary>
        /// 生成SQL语句参数对象数组
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="fields">字段，用空格隔开多个字段。参数名称需与字段名称一致！</param>
        /// <returns></returns>
        public static object[,] GetDbParameter<T>(this T obj, String fields)
        {
            String[] filedArray;
            Type type;
            PropertyInfo pro;
            object proValue;
            object[,] parameters;

            type = obj.GetType();

            bool autoPro = fields == "*" || String.IsNullOrEmpty(fields);


            if (autoPro)
            {
                PropertyInfo[] pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                parameters = new object[pros.Length, 2];

                int i = 0;
                foreach (PropertyInfo _pro in pros)
                {
                    proValue = _pro.GetValue(obj, null);
                    parameters[i, 0] = String.Format("@{0}", _pro.Name);
                    parameters[i, 1] = proValue;
                    ++i;
                }
            }
            else
            {
                filedArray = fields.Split(' ', ',');

                //初始化参数数组
                int fieldCount = filedArray.Length;

                //参数数组
                parameters = new object[fieldCount, 2];


                string fieldName;

                for (int i = 0; i < parameters.GetLength(0); i++)
                {
                    fieldName = filedArray[i];
                    pro = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pro == null)
                    {
                        throw new Exception(String.Format("对象不存在属性：{0}", fieldName));
                    }
                    //获取对象的值
                    proValue = pro.GetValue(obj, null);
                    if (proValue == null)
                    {
                        throw new Exception(String.Format("对象属性必须赋值，属性名称:{0}", fieldName));
                    }

                    parameters[i, 0] = String.Format("@{0}", fieldName);
                    parameters[i, 1] = proValue;

                }
            }
            return parameters;
        }

        /// <summary>
        /// 自动生成插入SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fields"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public static string AutoInsertSQL<T>(this T obj, string fields, bool setValue)
        {
            String[] filedArray;
            Type type;
            PropertyInfo pro;
            object proValue;
            bool autoPro = fields == "*" || String.IsNullOrEmpty(fields);

            StringBuilder sb = new StringBuilder();

            type = obj.GetType();


            sb.Append("INSERT INTO ").Append(type.Name).Append("(");


            String temp1 = "";
            String temp2 = "";

            if (autoPro)
            {
                PropertyInfo[] pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                int i = 0;
                foreach (PropertyInfo _pro in pros)
                {
                    if (i != 0)
                    {
                        temp1 += ",";
                        temp2 += ",";
                    }
                    temp1 += _pro.Name;

                    if (setValue)
                    {
                        //获取对象的值
                        proValue = _pro.GetValue(obj, null);
                        if (proValue == null)
                        {
                            throw new Exception(String.Format("对象属性必须赋值，属性名称:{0}", _pro.Name));
                        }
                        temp2 += proValue;
                    }
                    else
                    {
                        temp2 += "@" + _pro.Name;
                    }
                    ++i;
                }
            }
            else
            {
                filedArray = fields.Split(' ', ',');

                //初始化参数数组
                int fieldCount = filedArray.Length;


                string fieldName;

                for (int i = 0; i < fieldCount; i++)
                {
                    fieldName = filedArray[i];
                    pro = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pro == null)
                    {
                        throw new Exception(String.Format("对象不存在属性：{0}", fieldName));
                    }

                    if (i != 0)
                    {
                        temp1 += ",";
                        temp2 += ",";
                    }
                    temp1 += fieldName;

                    if (setValue)
                    {
                        //获取对象的值
                        proValue = pro.GetValue(obj, null);
                        if (proValue == null)
                        {
                            throw new Exception(String.Format("对象属性必须赋值，属性名称:{0}", fieldName));
                        }
                        temp2 += proValue;
                    }
                    else
                    {
                        temp2 += "@" + fieldName;
                    }

                }
            }


            sb.Append(temp1).Append(")VALUES(").Append(temp2).Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// 自动生成更新SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fields"></param>
        /// <param name="primaryKey"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public static string AutoUpdateSQL<T>(this T obj, string fields, string primaryKey, bool setValue)
        {
            String[] filedArray;
            Type type;
            PropertyInfo pro;
            object proValue;
            bool autoPro = fields == "*" || String.IsNullOrEmpty(fields);

            StringBuilder sb = new StringBuilder();

            type = obj.GetType();
            filedArray = fields.Split(' ', ',');

            //初始化参数数组
            int fieldCount = filedArray.Length;

            sb.Append("UPDATE ").Append(type.Name).Append(" SET ");


            string fieldName;

            if (autoPro)
            {
                string primaryValue = String.Empty;

                PropertyInfo[] pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                int i = 0;
                foreach (PropertyInfo _pro in pros)
                {
                    //主键值
                    if (String.Compare(_pro.Name, primaryKey, true) == 0)
                    {
                        proValue = _pro.GetValue(obj, null);
                        primaryValue = proValue.ToString();
                        continue;
                    }

                    if (i != 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(_pro.Name).Append("=");

                    if (setValue)
                    {
                        //获取对象的值
                        proValue = _pro.GetValue(obj, null);
                        if (proValue == null)
                        {
                            throw new Exception(String.Format("对象属性必须赋值，属性名称:{0}", _pro.Name));

                        }

                        sb.Append(proValue.ToString());
                    }
                    else
                    {
                        sb.Append("@" + _pro.Name);
                    }

                    ++i;
                }
                sb.Append(" WHERE ").Append(primaryKey).Append("=").Append(setValue ? primaryValue : "@" + primaryKey);
            }
            else
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    fieldName = filedArray[i];
                    pro = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pro == null)
                    {
                        throw new Exception(String.Format("对象不存在属性：{0}", fieldName));
                    }

                    if (i != 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(fieldName).Append("=");

                    if (setValue)
                    {
                        //获取对象的值
                        proValue = pro.GetValue(obj, null);
                        if (proValue == null)
                        {
                            throw new Exception(String.Format("对象属性必须赋值，属性名称:{0}", fieldName));
                        }
                        sb.Append(proValue);
                    }
                    else
                    {
                        sb.Append("@" + fieldName);
                    }
                }

            }
            return sb.ToString();
        }
    }
}
