﻿using Assets.Scripts.APIObjs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assets.Scripts.JSON
{
    [Serializable]
    public class Response<T>
    {
        public T data;
        public string error;
        public string message;
    }

}
namespace Assets.Scripts.JSON.Post
{

    [Serializable]
    public class login
    {
        public string user_id;
        public string password;
    }
    // todo mail vs email
    [Serializable]
    public class SignUp
    {
        public string user_id;
        public string password;
        public string name;
        public string email;
    }

    [Serializable]
    public class UserInfo
    {
        public string user_id;
        public string name;
        public string email;
    }

    [Serializable]
    public class PostProject
    {
        public string title;
        public List<UserRole> members;
    }

    [Serializable]
    public class GetProjects
    {
        public List<Project> projects;
    }
    [Serializable]
    public class Project
    {
        public string project_id;
        public string title;
        public string create_at;
    }

    [Serializable]
    public class ProjectStatistic
    {
        public int day_issues;
        public int month_issue;
        public int total_issue;
        public int closed_issues;

    }

    [Serializable]
    public class UserRoles
    {
        public List<UserRole> members;
    }

    [Serializable]
    public class UserRole
    {
        public string user_id;
        public string role;
    }

    [Serializable]
    public class PutMembers
    {
        public string title;
        public List<UserRole> members;
    }

    [Serializable]
    public class GetComments
    {
        public List<Comment> comments;
    }

    [Serializable]
    public class Comment
    {
        public string commentId;
        public string authorId;
        public string authorName;
        public string content;
        public string createdDate;
    }
}