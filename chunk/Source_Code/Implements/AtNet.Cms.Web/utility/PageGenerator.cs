//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGenerator.cs
// Author: newmin
// Date  : 2011/07/29
//

namespace AtNet.Cms.Web
{
    using System;

    public delegate void GenerateDelegate();
    public delegate string ParameterGenerateDelegate(object[] parameters);
    public delegate void ListPageGenerateDelegate(string arg1,int arg2);
    //public delegate string  CategoryListPageGenerateDelegate(Category category,int page);

    /// <summary>
    /// 页面生成器
    /// </summary>
    [Obsolete]
    public static class PageGenerator
    {
        public static void Generate(GenerateDelegate generator)
        {
            if (generator != null) generator();
        }
        /*
         * 
         * 
         */
        public static void Generate(ParameterGenerateDelegate generator, params object[] parameters)
        {
            if (generator != null) generator(parameters);        //注释
        }

        //public static void Generate(ArchiveHandler generator, Archive archive)
        //{
        //    if (generator != null) generator(archive);
        //}

        //public static string Generate(ArchiveBehavior generator, Category category, Archive archive)
        //{
        //    return generator == null ? String.Empty : generator(category, archive);
        //}

        public static void Generate(ListPageGenerateDelegate generator, string arg1, int arg2)
        {
            if (generator != null) generator(arg1, arg2);
        }
        //public static void Generate(CategoryListPageGenerateDelegate generator, Category category, int page)
        //{
        //    if (generator != null) generator(category, page);
        //}
        //public static string ReturnGenerate(CategoryListPageGenerateDelegate generator, Category category, int page)
        //{
        //    if (generator != null) return generator(category, page);
        //    return "not found!";
        //}

        public static string ReturnGenerate(ParameterGenerateDelegate generator, params object[] args)
        {
            if (generator != null) return generator(args);
            return "not found!";
        }

    }
}