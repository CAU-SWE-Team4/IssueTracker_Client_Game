using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using JSON;
using System.ComponentModel.Design;

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


    private Coroutine initializing;

    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private Image _statusImg;
    [SerializeField] private TextMeshProUGUI _reporterTxt;
    [SerializeField] private TextMeshProUGUI _dateTxt;
    [SerializeField] private TextMeshProUGUI _issueContentsTxt;
    [SerializeField] private TextMeshProUGUI _priorityTxt;
    [SerializeField] private TMP_Dropdown _prioritySelectDropdown;
    [SerializeField] private TextMeshProUGUI _assigneeTxt;
    [SerializeField] private TMP_Dropdown _assigneeSelectDropdown;
    [SerializeField] private Button _assignIssueDataBtn;
    [SerializeField] private TextMeshProUGUI _plTxt;
    [SerializeField] private Transform _assigneeSuggestionTrf;
    [SerializeField] private Transform _commentContentsTrf;
    [SerializeField] private TMP_InputField _newCommentIpF;
    [SerializeField] private Button _disposeIssueBtn;
    [SerializeField] private Button _issueFixedBtn;
    [SerializeField] private Button _issueResolvedBtn;
    [SerializeField] private Button _closeIssueBtn;
    [SerializeField] private Button _commentAssignBtn;


    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private GameObject suggestAssigneePrefab;

    [SerializeField] private Color _newColor;
    [SerializeField] private Color _assignedColor;
    [SerializeField] private Color _fixedColor;
    [SerializeField] private Color _closedColor;

    private void OnDisable()
    {

    }
    public void Initialize()
    {
        if (_commentContentsTrf.childCount == 0) return;
        for (int i = 0; i < _commentContentsTrf.childCount; i++)
        {
            Destroy(_commentContentsTrf.GetChild(i).gameObject);
        }
    }


    private void IssueInitialize(JSON.GetIssue issue)
    {
        Initialize();
        UpdateIssueData(issue);
        //Get Comments
        UpdateCommentList();
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", UpdateUserRoleData));
    }

    public void UpdateIssueData(JSON.GetIssue obj)
    {
        _titleTxt.text = obj.title;
        UpdateStateObj(obj.state);
        _priorityTxt.text = obj.priority;
        _assigneeTxt.text = obj.assignee_id;
        _reporterTxt.text = obj.reporter_id;
        _dateTxt.text = $"commented at {obj.created_date.Substring(0, 4)}-{obj.created_date.Substring(5, 2)}-{obj.created_date.Substring(8, 2)}";
    }

    public void UpdateCommentList()
    {
        StartCoroutine(ConnectionManager.Get<JSON.GetComments>($"project/{projectId}/issue/{issueId}/comment", CommentsInitialize));
    }

    public void UpdateUserRoleData(JSON.UserRoles roles)
    {
        foreach (JSON.UserRole role in roles.members)
        {
            if (role.role == "PL") _plTxt.text = role.user_id;
            if (role.user_id == ConnectionManager.id)
            {
                switch (role.role)
                {
                    case "TESTER":
                        _disposeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _issueFixedBtn.transform.parent.gameObject.SetActive(false);
                        _issueResolvedBtn.transform.parent.gameObject.SetActive(true);
                        _closeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(false);
                        _assigneeSelectDropdown.gameObject.SetActive(false);
                        _prioritySelectDropdown.gameObject.SetActive(false);
                        _assignIssueDataBtn.gameObject.SetActive(false);
                        break;
                    case "PL":
                        _disposeIssueBtn.transform.parent.gameObject.SetActive(true);
                        _issueFixedBtn.transform.parent.gameObject.SetActive(false);
                        _issueResolvedBtn.transform.parent.gameObject.SetActive(false);
                        _closeIssueBtn.transform.parent.gameObject.SetActive(true);
                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(true);
                        GetSuggestedDevelopers();
                        _assigneeSelectDropdown.gameObject.SetActive(true);
                        _prioritySelectDropdown.gameObject.SetActive(true);
                        _assignIssueDataBtn.gameObject.SetActive(true);
                        FillDeveloperList();

                        break;
                    case "DEV":
                        _disposeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _issueFixedBtn.transform.parent.gameObject.SetActive(true);
                        _issueResolvedBtn.transform.parent.gameObject.SetActive(false);
                        _closeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(false);
                        _assigneeSelectDropdown.gameObject.SetActive(false);
                        _prioritySelectDropdown.gameObject.SetActive(false);
                        _assignIssueDataBtn.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    public void GetSuggestedDevelopers()
    {
        StartCoroutine(ConnectionManager.Get<JSON.GetAssigneeSuggestion>($"project/{projectId}/issue/{issueId}/recommend", FillSuggestedDevelopers));
    }

    private void FillSuggestedDevelopers(GetAssigneeSuggestion obj)
    {
        for(int i=0;i<_assigneeSuggestionTrf.childCount;i++) 
            Destroy(_assigneeSuggestionTrf.GetChild(i).gameObject);
        foreach(string id in obj.dev_ids)
        {
            GameObject assigneeObj = Instantiate(suggestAssigneePrefab, _assigneeSuggestionTrf);
            assigneeObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = id;
        }
    }

    private void FillDeveloperList()
    {
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", UpdateDevList));
    }

    private void UpdateDevList(UserRoles obj)
    {
        _assigneeSelectDropdown.ClearOptions();
        foreach (JSON.UserRole role in obj.members)
        {
            if(role.role == "DEV")
                _assigneeSelectDropdown.AddOptions(new List<string> { role.user_id });
        }
    }

    private void UpdateIssueState(string _state)
    {
        JSON.UpdateStatus updateStatus = new JSON.UpdateStatus() { state = _state};
        StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/state", updateStatus));
    }
    public void DisposeIssue()
    {
        UpdateIssueState("DISPOSED");
    }
    public void IssueFixed()
    {
        UpdateIssueState("FIXED");
    }
    public void IssueResolved()
    {
        UpdateIssueState("RESOLVED");
    }
    public void CloseIssue()
    {
        UpdateIssueState("CLSOED");
    }
    public void AssignIssueData()
    {
        JSON.AssignData assignData = new JSON.AssignData() { user_id = _assigneeSelectDropdown.captionText.text, priority = _prioritySelectDropdown.captionText.text };
        StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/assign", assignData));
    }

    private void CommentsInitialize(JSON.GetComments issue)
    {
        if (issue.comments == null || issue.comments.Count == 0) return;
        List<JSON.Comment> commentslist = issue.comments;
        foreach (JSON.Comment comment in commentslist)
        {
            CreateComments(comment);
        }
    }


    private void UpdateStateObj(string state)
    {
        switch (state)
        {
            case "NEW":
                _statusImg.color = _newColor; break;
            case "ASSIGNED":
                _statusImg.color = _assignedColor; break;
            case "RESOLVED":
                _statusImg.color = _fixedColor; break;
            case "CLOSED":
                _statusImg.color = _closedColor; break;
            case "REOPENED":
                _statusImg.color = _newColor; break;
        }
        _statusImg.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = state;
    }

    private void CreateComments(JSON.Comment comment)
    {
        GameObject commentObj = Instantiate(commentPrefab, _commentContentsTrf);
        commentObj.transform.GetComponent<CommentController>().commentId = comment.comment_id;
        commentObj.transform.GetComponent<CommentController>().authorId = comment.author_id;
        commentObj.transform.GetComponent<CommentController>().authorName = comment.author_name;
        commentObj.transform.GetComponent<CommentController>().content = comment.content;
        commentObj.transform.GetComponent<CommentController>().createdDate=comment.created_date;
        commentObj.transform.GetComponent<CommentController>().projectId = projectId;
        commentObj.transform.GetComponent<CommentController>().issueId = issueId;
        commentObj.transform.GetComponent<CommentController>().UpdateCommentObj();
    }

    public void RequestIssueData(string project_id, string issue_id)
    {
        projectId = project_id;
        issueId = issue_id;
        initializing = StartCoroutine(ConnectionManager.Get<JSON.GetIssue>($"project/{projectId}/issue/{issueId}", IssueInitialize));
    }

    public void CreateComment()
    {
        JSON.NewComment newComment = new JSON.NewComment() { content = _newCommentIpF.text};
        StartCoroutine(ConnectionManager.Post($"project/{projectId}/issue/{issueId}/comment", newComment));
        _newCommentIpF.text = "";
        UpdateCommentList();
    }
}
