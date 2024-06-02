using JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IssueBlock : MonoBehaviour
{
    public string projectId { get; private set; }
    public string issueId { get; private set; }
    [SerializeField] private Button _showIssue;
    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private TextMeshProUGUI _statusTxt;
    [SerializeField] private TextMeshProUGUI _assigneeTxt;
    [SerializeField] private TextMeshProUGUI _reporterTxt;
    [SerializeField] private TextMeshProUGUI _dateTxt;
    [SerializeField] private Color _newColor;
    [SerializeField] private Color _disposedColor;
    [SerializeField] private Color _fixedColor;
    [SerializeField] private Color _closedColor;

    public void UpdateIssueBlock(string project_id, string issue_Id)
    {
        projectId = project_id;
        issueId = issue_Id;
        StartCoroutine(ConnectionManager.Get<JSON.GetIssue>($"project/{projectId}/issue/{issueId}", UpdateBlockData));
    }

    private void UpdateBlockData(GetIssue obj)
    {
        _titleTxt.text = obj.title;
        UpdateState(obj.state);
        _assigneeTxt.text = obj.assignee_id;
        _reporterTxt.text = obj.reporter_id;
        _dateTxt.text = $"{obj.created_date.Substring(0,4)}-{obj.created_date.Substring(5, 2)}-{obj.created_date.Substring(8, 2)}";
    }


    private void UpdateState(string state)
    {
        switch (state)
        {
            case "NEW":
                _statusTxt.color = _newColor; break;
            case "ASSIGNED":
                _statusTxt.color = _newColor; break;
            case "RESOLVED":
                _statusTxt.color = _fixedColor; break;
            case "CLOSED":
                _statusTxt.color = _closedColor; break;
            case "REOPENED":
                _statusTxt.color = _newColor; break;
        }
        _statusTxt.text = state;
    }

    public void ShowIssueVier()
    {
        UserUIManager.instance.ShowIssueViewer(projectId, issueId);
    }
}
