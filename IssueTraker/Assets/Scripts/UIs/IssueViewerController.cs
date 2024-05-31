using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.APIObjs;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;

public class IssueData
{
    public string Title;
    public string Description;
    public string ReporterId;
    public string ReporterDate;
    public string EditedDate;
    public string AssigneeId;
    public string FixerId;
    public string Priority;
    public string State;
}


public class IssueViewerController : MonoBehaviour
{
    public string projectId;
    public string issueId;

    private IssueData _issueData;
    public IssueData issueData
    {
        get => _issueData;
        set
        {
            UpdateIssueViewer(value);
        }
    }

    public List<CommentData> Comments;

    private Coroutine initializing;

    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private Image _statusImg;
    [SerializeField] private TextMeshProUGUI _reporterTxt;
    [SerializeField] private TextMeshProUGUI _issueContentsTxt;
    [SerializeField] private TextMeshProUGUI _assigneeTxt;
    [SerializeField] private TMP_Dropdown _assigneeSuggestionDrp;
    [SerializeField] private Transform _commentContentsTrf;
    [SerializeField] private TMP_InputField _newCommentIpF;
    [SerializeField] private Button _disposeIssueBtn;
    [SerializeField] private Button _issueFixedBtn;
    [SerializeField] private Button _closeIssueBtn;
    [SerializeField] private Button _commentAssignBtn;


    [SerializeField] private GameObject commentPrefab;

    [SerializeField] private Color _newColor;
    [SerializeField] private Color _assignedColor;
    [SerializeField] private Color _resolvedColor;
    [SerializeField] private Color _closedColor;
    [SerializeField] private Color _reopenedColor;
    private void OnEnable()         
    {                               
        initializing = StartCoroutine(ConnectionManager.Get<Assets.Scripts.JSON.GetIssue>($"/project{projectId}/issue/{issueId}", IssueInitialize));
    }

    private void OnDisable()
    {
        
    }

    private void IssueInitialize(Assets.Scripts.JSON.Response<Assets.Scripts.JSON.GetIssue> issue)
    {
        UpdateIssueData(issue.data);
        //Get Comments
        StartCoroutine(ConnectionManager.Get<Assets.Scripts.JSON.GetComments>($"/project{projectId}/issue/{issueId}/comment", CommentsInitialize));
    }

    public void UpdateIssueData(Assets.Scripts.JSON.GetIssue value)
    {
        IssueData issueData = new IssueData() { Title = value.Title, AssigneeId = value.AssigneeId, Description = value.Description, EditedDate = value.EditedDate, FixerId = value.FixerId, Priority = value.Priority, State = value.State, ReporterDate = value.ReporterDate, ReporterId = value.ReporterId};
    }

    private void CommentsInitialize(Assets.Scripts.JSON.Response<Assets.Scripts.JSON.GetComments> issue)
    {
        List<Assets.Scripts.JSON.Comment> commentslist = issue.data.comments;
        foreach (Assets.Scripts.JSON.Comment comment in commentslist)
        {
            CreateComments(comment);
        }
    }

    private void UpdateIssueViewer(IssueData value)
    {
        _titleTxt.text = value.Title;
        UpdateState(value.State);
        _reporterTxt.text = value.ReporterId;
        _issueContentsTxt.text = value.Description;
        _assigneeTxt.text = value.AssigneeId;
        //_assigneeSuggestionDrp;
    }

    private void UpdateState(string state)
    {
        switch (state)
        {
            case "NEW":
                _statusImg.color = _newColor; break;
            case "ASSIGNED":
                _statusImg.color = _assignedColor; break;
            case "RESOLVED":
                _statusImg.color = _resolvedColor; break;
            case "CLOSED":
                _statusImg.color = _closedColor; break;
            case "REOPENED":
                _statusImg.color = _reopenedColor; break;
        }
        _statusImg.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = state;
    }

    private void CreateComments(Assets.Scripts.JSON.Comment comment)
    {
        CommentData commentData = ToComment(comment);
        Instantiate(commentPrefab, _commentContentsTrf);
    }

    private CommentData ToComment(Assets.Scripts.JSON.Comment comment)
    {
        return new CommentData() { authorId = comment.authorId, authorName = comment.authorName, commentId = comment.commentId, content = comment.content, createdDate = comment.createdDate };
    }

}
