using System;
using System.Collections;
using System.Collections.Generic;
using Tank;
using UnityEngine;
using UnityEngine.UI;

public class UserUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _blocker;
    [SerializeField] private GameObject _projectDashboard;
    [SerializeField] private GameObject _issueViewer;
    [SerializeField] private GameObject _newIssuePage;
    public static UserUIManager instance;
    private void Awake()
    {
        if (instance && !instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ShowProjectDashboard(string project_id, string project_title)
    {
        ActivateBlocker();
        _projectDashboard.SetActive(true);
        _issueViewer.SetActive(false);
        _projectDashboard.transform.GetComponent<ProjectDashboardController>().InitializeProjectDashboard(project_id, project_title);
    }
    public void RefreshProjectIssueList()
    {
        _projectDashboard.transform.GetComponent<ProjectDashboardController>().UpdateIssueList();

    }
    public void ShowIssueViewer(string project_id, string issue_id)
    {
        ActivateBlocker();
        _projectDashboard.SetActive(false);
        _newIssuePage.SetActive(false);
        _issueViewer.SetActive(true);
        _issueViewer.transform.GetComponent<IssueViewerController>().RequestIssueData(project_id, issue_id);
    }

    internal void HideAllUI()
    {
        _projectDashboard.transform.GetComponent<ProjectDashboardController>().Initialize();
        _issueViewer.transform.GetComponent<IssueViewerController>().Initialize();
        _newIssuePage.GetComponent<NewIssuePageController>().Initialize();

        DeactivateBlocker();
        _projectDashboard.SetActive(false);
        _issueViewer.SetActive(false);
        _newIssuePage.SetActive(false);
    }

    private void ActivateBlocker()
    {
        _blocker.SetActive(true);
    }

    private void DeactivateBlocker()
    {
        _blocker.SetActive(false);
    }

    public void CommentListRefresh()
    {
        _issueViewer.transform.GetComponent<IssueViewerController>().Initialize();
        _issueViewer.transform.GetComponent<IssueViewerController>().UpdateCommentList();
    }

    public void UpdateIssueViewer()
    {
        _issueViewer.transform.GetComponent<IssueViewerController>().UpdateIssueViewer();
    }
}
