using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using JSON;

public class ProjectDashboardController : MonoBehaviour
{
    public string projectId;
    public string issueId;


    public List<CommentController> Comments;

    private Coroutine updating;


    [SerializeField] private Transform _issueContainerTrf;
    //[SerializeField] private StatisticsViewer _statisticsViewer;
    [SerializeField] private TextMeshProUGUI _titleTxt;

    [SerializeField] private TextMeshProUGUI _dayIssueTxt;
    [SerializeField] private TextMeshProUGUI _monthIssueTxt;
    [SerializeField] private TextMeshProUGUI _totalIssueTxt;
    [SerializeField] private TextMeshProUGUI _closedIssueTxt;

    [SerializeField] private Button _newIssueBtn;
    [SerializeField] private TMP_Dropdown _filterOption;
    [SerializeField] private TMP_InputField _searchIpF;

    [SerializeField] private GameObject _issuePrefab;

    [SerializeField] private GameObject _newIssuePage;

    public void Initialize()
    {
        if (_issueContainerTrf.childCount == 0) return;
        for(int i=0; i< _issueContainerTrf.childCount; i++)
        {
            Destroy(_issueContainerTrf.GetChild(i).gameObject);
        }
    }

    public void UpdateProjectDashboard()
    {
        InitializeProjectDashboard(projectId, issueId);
    }

    public void InitializeProjectDashboard(string project_id, string projectTitle)
    {
        projectId = project_id;
        _titleTxt.text = projectTitle;
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", ActivateNewIssue));
        UpdateIssueList();
        StartCoroutine(ConnectionManager.Get<JSON.ProjectStatistic>($"project/{projectId}/issue/statistic", UpdateStatistic));
    }

    private void UpdateStatistic(ProjectStatistic obj)
    {
        _dayIssueTxt.text = obj.day_issues.ToString();
        _monthIssueTxt.text = obj.month_issue.ToString();
        _totalIssueTxt.text = obj.total_issue.ToString();
        _closedIssueTxt.text = obj.closed_issues.ToString();
    }

    public void UpdateIssueList(string filterBy = "", string filterValue = "")
    {
        Initialize();
        StartCoroutine(ConnectionManager.Get<JSON.GetIssueList>($"project/{projectId}/issue", filterBy, filterValue, CreateIssueBlock));
    }

    private void ActivateNewIssue(JSON.UserRoles roles)
    {
        foreach(JSON.UserRole role in roles.members)
        {
            if (role.user_id == ConnectionManager.id && role.role == "TESTER")
            {
                Debug.Log(JsonUtility.ToJson(role));

                _newIssueBtn.gameObject.SetActive(true);
                break;
            }
            else _newIssueBtn.gameObject.SetActive(false);
        }
    }

    private void CreateIssueBlock(GetIssueList obj)
    {
        if (obj.issues == null || obj.issues.Count == 0) return;
        foreach (JSON.GetIssue issueData in obj.issues)
        {
            GameObject issueObj = Instantiate(_issuePrefab, _issueContainerTrf);
            issueObj.GetComponent<IssueBlock>().UpdateIssueBlock(projectId, issueData.id);

        }
    }
    public void NewIssuePageActivate()
    {
        _newIssuePage.transform.GetComponent<NewIssuePageController>().projectId = projectId;
        _newIssuePage.SetActive(true);
    }

    public void SearchIssues()
    {
        if (_searchIpF.text.Length == 0) 
            UpdateIssueList();
        else
            UpdateIssueList(_filterOption.captionText.text, _searchIpF.text);
    }
}
