using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.APIObjs
{
    public class IssueClass
    {
        public string Title;
        public string Description;
        public string ReporterId;
        public string ReporterDate;
        public string EditedDate;
        public string AssigneeId;
        public string FixerId;
        public Priority Priority;
        public State State;
        public List<CommentClass> Comments;
    }

    public enum State
    {
        NEW,
        ASSIGNED,
        RESOLVED,
        CLOSED,
        REOPENED
    }

    public enum Priority
    {
        BLOCKER,
        CRITICAL,
        MAJOR,
        MINOR,
        TRIVIAL
    }


}
