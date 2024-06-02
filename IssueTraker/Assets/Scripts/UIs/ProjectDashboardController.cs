using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using JSON;
using UnityEditor.SearchService;

public class ProjectDashboardController : MonoBehaviour
{
    public string projectId;
    public string issueId;


    public List<CommentController> Comments;

    private Coroutine updating;


    [SerializeField] private Transform _issueContainerTrf;
    //[SerializeField] private StatisticsViewer _statisticsViewer;
    [SerializeField] private TextMeshProUGUI _titleTxt;

    [SerializeField] private Button _searchBtn;
    [SerializeField] private Button _newIssueBtn;
    [SerializeField] private TMP_Dropdown _filterOption;
    [SerializeField] private TMP_InputField _searchIpF;

    [SerializeField] private GameObject _issuePrefab;

    [SerializeField] private GameObject _newIssuePage;

    public void Initialize()
    {
        for(int i=0; i< _issueContainerTrf.childCount; i++)
        {
            Destroy(_issueContainerTrf.GetChild(i).gameObject);
        }
    }

    public void UpdateProjectDashboard(string project_id, string projectTitle)
    {
        projectId = project_id;
        _titleTxt.text = projectTitle;
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", ActivateNewIssue));
        StartCoroutine(ConnectionManager.Get<JSON.GetIssueList>($"project/{projectId}/issue", CreateIssueBlock));
    }

    public void UpdateIssueList()
    {
        Initialize();
        StartCoroutine(ConnectionManager.Get<JSON.GetIssueList>($"project/{projectId}/issue", CreateIssueBlock));
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
}
