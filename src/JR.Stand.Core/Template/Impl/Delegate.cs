﻿//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:delegate.cs
// Author:newmin
// Create:2011/06/28
//

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板呈现处理
    /// </summary>
    /// <param name="templateContent">模板内容</param>
    /// <param name="obj">数据对象</param>
    public delegate void TemplateHandler<T>(T obj, ref string templateContent);

    public delegate void TemplateBehavior();
}