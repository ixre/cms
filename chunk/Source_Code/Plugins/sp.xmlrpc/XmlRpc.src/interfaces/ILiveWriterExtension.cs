//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: OPSite.XmlRpc.interface
// FileName : ILiveWriterExtension.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/25 10:46:23
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
//

namespace J6.Cms.XmlRpc
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CookComputing.XmlRpc;
    using CookComputing.MetaWeblog;
    using CookComputing.Blogger;


    public interface ILiveWriterExtension
    {

        [XmlRpcMethod("blogger.getUsersBlogs",
              Description = "Returns information on all the blogs a given user "
              + "is a member.")]
        BlogInfo[] getUsersBlogs(
           string appKey,
           string username,
           string password);


        [XmlRpcMethod("blogger.deletePost",
            Description = "Deletes a post.")]
        [return: XmlRpcReturnValue(Description = "Always returns true.")]
        bool deletePost(
          string appKey,
          string postid,
          string username,
          string password,
          [XmlRpcParameter(
             Description = "Where applicable, this specifies whether the blog "
             + "should be republished after the post has been deleted.")]
      bool publish);

    }
}
