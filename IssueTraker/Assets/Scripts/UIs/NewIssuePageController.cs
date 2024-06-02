using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewIssuePageController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _title;
    [SerializeField] private TMP_InputField _description;
    public string projectId;
    
    public void AssignNewIssue()
    {
        JSON.NewIssue newIssue = new JSON.NewIssue() {title = _title.text, description = _description.text};
        StartCoroutine(ConnectionManager.Post($"project/{projectId}/issue", newIssue));
        Initialize();
        UserUIManager.instance.RefreshProjectIssueList();
        this.gameObject.SetActive(false);
    }
    public void Initialize()
    {
        _title.text = null;
        _description.text = null;
    }

}
