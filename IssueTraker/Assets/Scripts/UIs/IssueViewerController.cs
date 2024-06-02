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
using UnityEngine.Rendering.VirtualTexturing;
using Tank;

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

    private string _state;
    private string _playerRole;

    private Coroutine initializing;

    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private TMP_InputField _newTitleIpF;
    [SerializeField] private Image _statusImg;
    [SerializeField] private TextMeshProUGUI _reporterTxt;
    [SerializeField] private TextMeshProUGUI _dateTxt;
    [SerializeField] private TextMeshProUGUI _issueContentTxt;
    [SerializeField] private TMP_InputField _newContentIpF;
    [SerializeField] private TextMeshProUGUI _priorityTxt;
    [SerializeField] private Button _prioritySelectBtn;
    [SerializeField] private TMP_Dropdown _prioritySelectDropdown;
    [SerializeField] private TextMeshProUGUI _assigneeTxt;
    [SerializeField] private Button _assigneeSelectBtn;
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
    [SerializeField] private Button _deleteIssueBtn;
    [SerializeField] private Button _reopenIssueBtn;
    [SerializeField] private Button _commentAssignBtn;


    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private GameObject suggestAssigneePrefab;

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

    public void UpdateIssueViewer()
    {
        StartCoroutine(ConnectionManager.Get<JSON.GetIssue>($"project/{projectId}/issue/{issueId}", IssueInitialize));
    }

    private void IssueInitialize(JSON.GetIssue issue)
    {
        Initialize();
        UpdateIssueData(issue);
        //Get Comments
        UpdateCommentList();
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", UpdateUserRoleData));
        SetInputField();
    }

    public void UpdateIssueData(JSON.GetIssue obj)
    {
        _titleTxt.text = obj.title;
        UpdateStateObj(obj.state);
        _issueContentTxt.text = obj.description;
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
                _playerRole = role.role;
                switch (role.role)
                {
                    case "TESTER":
                        _disposeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _issueFixedBtn.transform.parent.gameObject.SetActive(false);
                        if (_state == "FIXED") _issueResolvedBtn.transform.parent.gameObject.SetActive(true);
                        else _issueResolvedBtn.transform.parent.gameObject.SetActive(false);
                        _closeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _reopenIssueBtn.transform.parent.gameObject.SetActive(false);
                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(false);
                        _assigneeSelectDropdown.gameObject.SetActive(false);
                        _prioritySelectDropdown.gameObject.SetActive(false);
                        _assignIssueDataBtn.gameObject.SetActive(false);
                        _deleteIssueBtn.gameObject.SetActive(false);
                        break;
                    case "PL":
                        if(_state != "CLOSED" && _state != "DISPOSED") _disposeIssueBtn.transform.parent.gameObject.SetActive(true);
                        else _disposeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _issueFixedBtn.transform.parent.gameObject.SetActive(false);
                        _issueResolvedBtn.transform.parent.gameObject.SetActive(false);
                        if (_state == "RESOLVED") _closeIssueBtn.transform.parent.gameObject.SetActive(true);
                        else _closeIssueBtn.transform.parent.gameObject.SetActive(false);
                        if (_state == "CLOSED" || _state == "DISPOSED") _reopenIssueBtn.transform.parent.gameObject.SetActive(true);
                        else _reopenIssueBtn.transform.parent.gameObject.SetActive(false);

                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(true);
                        GetSuggestedDevelopers();
                        _assigneeSelectDropdown.gameObject.SetActive(false);
                        _prioritySelectDropdown.gameObject.SetActive(false);
                        _assignIssueDataBtn.gameObject.SetActive(true);
                        FillDeveloperList();

                        _deleteIssueBtn.gameObject.SetActive(true);
                        break;
                    case "DEV":
                        _disposeIssueBtn.transform.parent.gameObject.SetActive(false);
                        if (_state == "ASSIGNED" && role.user_id == _assigneeTxt.text) _issueFixedBtn.transform.parent.gameObject.SetActive(true);
                        else _issueFixedBtn.transform.parent.gameObject.SetActive(false);
                        _issueResolvedBtn.transform.parent.gameObject.SetActive(false);
                        _closeIssueBtn.transform.parent.gameObject.SetActive(false);
                        _reopenIssueBtn.transform.parent.gameObject.SetActive(false);
                        _assigneeSuggestionTrf.transform.parent.parent.parent.gameObject.SetActive(false);
                        _assigneeSelectDropdown.gameObject.SetActive(false);
                        _prioritySelectDropdown.gameObject.SetActive(false);
                        _assignIssueDataBtn.gameObject.SetActive(false);
                        _deleteIssueBtn.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    public void SetInputField()
    {
        _newTitleIpF.transform.parent.gameObject.SetActive(false);
        _newContentIpF.transform.parent.gameObject.SetActive(false);
        if(_state == "CLOSED" || _state == "DISPOSED")
        {
            _newCommentIpF.gameObject.SetActive(false) ;
            _commentAssignBtn.gameObject.SetActive(false) ;
        }
        else
        {
            _newCommentIpF.gameObject.SetActive(true);
            _commentAssignBtn.gameObject.SetActive(true) ;
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

    public void ShowAssigneeSelectDropdown()
    {
        if (_playerRole == "PL")
            _assigneeSelectDropdown.gameObject.SetActive(true);
    }

    public void ShowPrioritySelectDropdown()
    {
        if (_playerRole == "PL")
            _prioritySelectDropdown.gameObject.SetActive(true);
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
        StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/state", updateStatus, UpdateIssueViewer));
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
        UpdateIssueState("CLOSED");
    }
    public void ReopenIssue()
    {
        UpdateIssueState("REOPEN");
    }
    public void DeleteIssue()
    {
        StartCoroutine(ConnectionManager.Delete($"project/{projectId}/issue/{issueId}", GameManager.instance.ExitUserUI));
    }


    public void AssignIssueData()
    {
        JSON.AssignData assignData = new JSON.AssignData() { user_id = _assigneeSelectDropdown.captionText.text, priority = _prioritySelectDropdown.captionText.text };
        StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/assign", assignData, UpdateIssueViewer));
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
                _statusImg.color = Colors.IssueStateColor.NEW; break;
            case "FIXED":
                _statusImg.color = Colors.IssueStateColor.FIXED; break;
            case "RESOLVED":
                _statusImg.color = Colors.IssueStateColor.RESOLVED; break;
            case "CLOSED":
                _statusImg.color = Colors.IssueStateColor.CLOSED; break;
            case "REOPEN":
                _statusImg.color = Colors.IssueStateColor.REOPEN; break;
            case "DISPOSED":
                _statusImg.color = Colors.IssueStateColor.DISPOSED; break;
            case "ASSIGNED":
                _statusImg.color = Colors.IssueStateColor.ASSIGNED; break;
        }
        _statusImg.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = state;
        _state = state;
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

    public void ShowIssueContentEditor()
    {
        if (_playerRole == "TESTER" && _state != "CLOSED" && _state != "DISPOSED")
        {
            _newContentIpF.transform.parent.gameObject.SetActive(true);
            _newContentIpF.text = _issueContentTxt.text;
        }
    }

    public void ShowIssueTitleEditor()
    {
        if (_playerRole == "TESTER" && _state != "CLOSED" && _state != "DISPOSED")
        {
            _newTitleIpF.transform.parent.gameObject.SetActive(true);
            _newTitleIpF.text = _titleTxt.text;
        }
    }

    public void EditIssueContent()
    {
        if(_playerRole == "TESTER")
        {
            JSON.UpdateContents updateContents = new JSON.UpdateContents() { title = _titleTxt.text, description = _newContentIpF.text};
            StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/content", updateContents, UpdateIssueViewer));
            _newContentIpF.text = string.Empty;
        }
    }
    
    public void EditIssueTitle()
    {
        if(_playerRole == "TESTER")
        {
            JSON.UpdateContents updateContents = new JSON.UpdateContents() { title = _newTitleIpF.text, description = _issueContentTxt.text};
            StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/content", updateContents, UpdateIssueViewer));
            _newTitleIpF.text = string.Empty;
        }
    }

    public void CancelEditingTitle()
    {
        _newTitleIpF.text = string.Empty;
        _newTitleIpF.transform.parent.gameObject.SetActive(false);
    }

    public void CancelEditingContent()
    {
        _newContentIpF.text = string.Empty;
        _newContentIpF.transform.parent.gameObject.SetActive(false);
    }

    public void CreateNewComment()
    {
        JSON.NewComment newComment = new JSON.NewComment() { content = _newCommentIpF.text};
        StartCoroutine(ConnectionManager.Post($"project/{projectId}/issue/{issueId}/comment", newComment, UpdateIssueViewer));
        _newCommentIpF.text = "";
    }
}
