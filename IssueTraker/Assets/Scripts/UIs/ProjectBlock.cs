using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectBlock : MonoBehaviour
{
    public string projectId;
    public string title;
    public string createdAt;
    public TextMeshProUGUI _titleTxt;
    public void UpdateProjectObj()
    {
        _titleTxt.text = title;
    }
    public void ShowMembers()
    {
        AdminUIManager.instance.GetMembers(projectId, title, createdAt);
    }
    public void DeleteProject()
    {
        AdminUIManager.instance.DeleteProject(projectId);
    }
}
