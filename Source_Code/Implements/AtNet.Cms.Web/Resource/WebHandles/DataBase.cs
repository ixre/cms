//
// Go.cs    跳转
// Copyright 2011 @ OPS Inc,All rights reseved !
// newmin @ 2011/03/18
//
namespace OPSite.WebHandler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Ops.Web;
    using System.Data;
    using Ops.Cms;
    using Ops.Data;
    using Ops.Cms.BLL;
    using Ops.Cms.DAL;
    using Ops.Cms.Models;

    [WebExecuteable]
    public class DataBase
    {
        private static readonly char[] jpArr = {'ァ','ア','ィ','イ','ゥ','ウ','ェ','エ','ォ','オ','カ','ガ','キ','ギ','ク','グ','ケ','ゲ',
                             'コ','ゴ','サ','ザ','シ','ジ','ス','ズ','セ','ゼ','ソ','ゾ','タ','ダ','チ','ヂ','ッ',
                             'ツ','ヅ','テ','デ','ト','ド','ナ','ニ','ヌ','ネ','ノ','ハ','バ','パ','ヒ','ビ','ピ',
                              'フ','ブ','プ','ヘ','ベ','ペ','ホ','ボ','ポ','マ','ミ','ム','メ','モ','ャ','ヤ','ュ',
                             'ユ','ョ','ヨ','ラ','リ','ル','レ','ロ','ヮ','ワ','ヰ','ヱ','ヲ','ン','ヴ','ヵ','ヶ',
                               'ー','ヽ','ヾ'};


        public void FixJpChar()
        {
            //
            //TODO : DbHelper.db问题
            //

            //IEnumerable<global::Ops.Cms.Models.Archive> ars = DbHelper.db.GetDataSet("select * from O_Archives").Tables[0].ToEntityList<global::Ops.Cms.Models.Archive>();
            //foreach (global::Ops.Cms.Models.Archive a in ars)
            //{
            //    if (a.Title.IndexOfAny(jpArr) != -1 || a.Content.IndexOfAny(jpArr) != -1 || a.Tags.IndexOfAny(jpArr) != -1)
            //        throw new Exception("archiveID:" + a.ID);
            //}
            //HttpContext.Current.Response.Write("没错误");
        }
    }
}