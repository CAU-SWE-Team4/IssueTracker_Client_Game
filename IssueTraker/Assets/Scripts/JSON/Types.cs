using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JSON
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
    public class UserList
    {
        public List<UserInfo> members;
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
        public string created_at;
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
    public class NewIssue  
    {
        public string title;
        public string description;
    }

    [Serializable]
    public class GetIssueList  
    {
        public List<GetIssue> issues;
    }

    [Serializable]
    public class Issue  
    {
        public string issue_id;
        public string issue_title;
        public string created_date;
        public string state;
    }

    [Serializable]
    public class GetIssue  
    {
        public string id;
        public string project_id;
        public string title;
        public string description;
        public string reporter_id;
        public string assignee_id;
        public string fixer_id;
        public string priority;
        public string state;
        public string created_date;
        public string modified_date;
    }

    [Serializable]
    public class GetAssigneeSuggestion  
    {
        public List<string> dev_ids;
    }

    [Serializable]
    public class AssignData  
    {
        public string user_id;
        public string priority;
    }

    [Serializable]
    public class UpdateContents  
    {
        public string title;
        public string description;
    }


    [Serializable]
    public class UpdateStatus  
    {
        public string state;
    }

    [Serializable]
    public class NewComment  
    {
        public string content;
    }


    [Serializable]
    public class GetComments  
    {
        public List<Comment> comments;
    }

    [Serializable]
    public class Comment  
    {
        public string comment_id;
        public string author_id;
        public string author_name;
        public string content;
        public string created_date;
    }

    [Serializable]
    public class EditCommentContent
    {
        public string content;
    }
}