﻿//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:RegexUtility.cs
// Author:newmin
// Create:2011/06/05
//

using System;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    public sealed class TemplateRegexUtility
    {
        private static readonly Regex tagRegex = new Regex("\\${([a-zA-Z\u4e00-\u9fa5\\._]+)}");

        //部分页匹配模式
        internal static Regex partialRegex = new Regex("\\${(partial|include):\"(.+?)\"}");
        // 部分面ID匹配模式
        private static Regex partialIdRegexp = new Regex("^[a-z0-9]+$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 替换模板数据
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        internal static string ReplaceTemplate(string templateID, MatchEvaluator eval)
        {
            string html = TemplateUtility.Read(templateID);
            return TemplateRegexUtility.Replace(html, eval);
        }

        /// <summary>
        /// 替换标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static string Replace(string html, MatchEvaluator eval)
        {
            //如果包含部分视图，则替换成部分视图的内容
            //ReplacePartial(html);

            //替换匹配
            return tagRegex.Replace(html, eval);
        }

        internal static string ReplaceHtml(string html, string tagKey, string tagValue)
        {
            return Regex.Replace(html, "\\${" + tagKey + "}", tagValue, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 替换模板的部分视图
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        internal static string ReplacePartial(string html)
        {
            //如果包含部分视图，则替换成部分视图的内容
            if (partialRegex.IsMatch(html))
            {
                //替换模板里的部分视图，并将内容添加到模板内容中
                html = partialRegex.Replace(html, match =>
                {
                    // 匹配的部分视图编号
                    string matchValue = match.Groups[2].Value;
                    string[] arr = matchValue.Split('@');
                    //Console.WriteLine("---" + arr[0]);
                    if (partialIdRegexp.IsMatch(arr[0]))
                    {
                        //Console.WriteLine("---" + arr[1]);
                        string content = TemplateUtility.ReadPartial(arr[0]);
                        if (content != null) return content;
                    }
                    return String.Format("No such partial file \"{0}\"", matchValue);
                });
            }
            //返回替换部分视图后的内容
            return html;
        }
    }
}