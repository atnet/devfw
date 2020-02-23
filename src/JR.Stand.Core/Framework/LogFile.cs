﻿//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: OPS
// FileName : LogFile.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/11/1 20:23:38
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.IO;
using System.Text;

namespace JR.DevFw.Framework
{
    public class LogFile
    {
        private string filePath;
        private bool printPrefix;
        private Encoding _encoding;

        //种子，用于判断
        public int Seed { get; set; }

        //编码
        public Encoding FileEncoding
        {
            get { return this._encoding ?? (this._encoding = Encoding.UTF8); }
            set { this._encoding = value; }
        }

        public LogFile(string filePath) : this(filePath, false)
        {
        }

        public LogFile(string filePath, bool printPrefix)
        {
            this.printPrefix = printPrefix;
            this.filePath = filePath;
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
        }

        /// <summary>
        /// 填充内容
        /// </summary>
        /// <param name="text"></param>
        /* [Obsolete]
        public void Append(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            if (fs.CanWrite)
            {
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            fs.Dispose();

        }
        */
        public void Print(Byte[] bytes)
        {
            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            if (fs.CanWrite)
            {
                if (this.printPrefix)
                {
                    byte[] data = this.FileEncoding.GetBytes(string.Format("{0:yyyy-MM-dd HH:mm:ss ", DateTime.Now));
                    fs.Write(data, 0, data.Length);
                }
                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Dispose();
        }

        public void Println(string text)
        {
            this.Print(this.FileEncoding.GetBytes(text + System.Environment.NewLine));
        }

        public void Printf(string format, params object[] data)
        {
            this.Print(this.FileEncoding.GetBytes(string.Format(format, data)));
        }

        /// <summary>
        /// 清空内容
        /// </summary>
        /// <param name="text"></param>
        public void Truncate()
        {
            FileStream fs = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
            fs.Dispose();
        }
    }
}