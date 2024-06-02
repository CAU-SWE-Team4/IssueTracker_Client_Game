using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _memberPrefab;
    [SerializeField] private GameObject _projectPrefab;
    [SerializeField] private Transform _memberContainer;
    [SerializeField] private Transform _projectContainer;

    [SerializeField] private GameObject _addMemberPage;
    [SerializeField] private GameObject _addProjectPage;

    public static AdminUIManager instance;

    private JSON.Project _activatedProject;
    private List<JSON.UserRole> _userRoles;
    private void Awake()
    {
        if (instance && !instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitializeAdminPage();
    }

    private void InitializeProjectList()
    {
        if(_projectContainer.childCount != 0)
        {
            for(int i = 0; i < _projectContainer.childCount; i++)
            {
                Destroy(_projectContainer.GetChild(i).gameObject);
            }
        }
    }
    public void InitializeMemberList()
    {
        if (_memberContainer.childCount != 0)
        {
            for (int i = 0; i < _memberContainer.childCount; i++)
            {
                Destroy(_memberContainer.GetChild(i).gameObject);
            }
        }
    }

    public void InitializeAdminPage()
    {
        _activatedProject = new JSON.Project();
        _activatedProject.title = string.Empty;
        _activatedProject.project_id = string.Empty;
        _activatedProject.created_at = string.Empty;
        if (_userRoles != null) _userRoles.Clear();
        else _userRoles = new List<JSON.UserRole>();
        InitializeProjectList();
        InitializeMemberList();
        GetProject();
    }

    public void OpenAddMember()
    {
        if (_memberContainer.childCount != 0)
        {
            _addMemberPage.SetActive(true);
            StartCoroutine(ConnectionManager.Get<JSON.UserList>("user", AddUserOptionOnAddMember));
        }
    }
    private void AddUserOptionOnAddMember(JSON.UserList obj)
    {
        foreach (JSON.UserInfo user in obj.members)
        {
            bool willAdd = true;
            foreach(JSON.UserRole member in _userRoles)
            {
                if(user.user_id == member.user_id)
                {
                    willAdd = false;
                    break;
                }
            }
            if (willAdd) _addMemberPage.transform.GetComponent<AddMemberPageController>().AddMemberOption(user.user_id);
        }
    }

    public void OpenAddProject()
    {
        if (_memberContainer.childCount != 0) _addMemberPage.SetActive(true);
    }

    public void AddMember(string userId, string role)
    {
        _userRoles.Add(new JSON.UserRole() { user_id = userId, role = role });
        JSON.PostProject postProject = new JSON.PostProject() { title = _activatedProject.title, members = _userRoles };
        StartCoroutine(ConnectionManager.Put($"project/{_activatedProject.project_id}", postProject, InitializeAdminPage));
    }

    public void AddProject()
    {
        _addProjectPage.SetActive(true);
        _addProjectPage.transform.GetComponent<AddProjectPageController>().Initialize();
        StartCoroutine(ConnectionManager.Get<JSON.UserList>("user", AddUserOptionOnAddProject));

    }
    private void AddUserOptionOnAddProject(JSON.UserList obj)
    {
        foreach (JSON.UserInfo user in obj.members)
        {
            if (user.user_id == "admin") continue;
            AddUserOptionToAddProjectPageList(user.user_id);
        }
    }

    public void GetProject()
    {
        StartCoroutine(ConnectionManager.Get<JSON.GetProjects>($"project", LoadProject));
    }

    private void LoadProject(JSON.GetProjects obj)
    {
        foreach(JSON.Project project in obj.projects)
        {
            GameObject projectObj = Instantiate(_projectPrefab, _projectContainer);
            projectObj.GetComponent<ProjectBlock>().projectId = project.project_id;
            projectObj.GetComponent<ProjectBlock>().title = project.title;
            projectObj.GetComponent<ProjectBlock>().createdAt = project.created_at;
            projectObj.GetComponent<ProjectBlock>().UpdateProjectObj();
        }
    }

    public void GetMembers(string projectId, string title, string createdAt)
    {
        _activatedProject.project_id = projectId;
        _activatedProject.title = title;
        _activatedProject.created_at = createdAt;
        InitializeMemberList();
        StartCoroutine(ConnectionManager.Get<JSON.UserRoles>($"project/{projectId}/userRole", CreateMemberBlock));
    }

    private void CreateMemberBlock(JSON.UserRoles obj)
    {
        foreach(JSON.UserRole userRole in obj.members)
        {
            _userRoles.Add(userRole);
            if (userRole.role == "ADMIN") continue;
            GameObject memberObj = Instantiate(_memberPrefab, _memberContainer);
            memberObj.transform.GetComponent<MemberBlock>().role = ReturnRoleFullName(userRole.role);
            memberObj.transform.GetComponent<MemberBlock>().userId = userRole.user_id;
            memberObj.transform.GetComponent<MemberBlock>().UpdateProjectObj();
        }
    }

    public string ReturnRoleFullName(string fullName)
    {
        switch(fullName)
        {
            case "TESTER":
                return "Tester";
            case "PL":
                return "Project Leader";
            case "DEV":
                return "Developer";
            default:
                return string.Empty;
        }
    }

    public void ClearMembers()
    {
        for(int i=0; i<_memberContainer.childCount; i++)
        {
            Destroy(_memberContainer.GetChild(i));
        }
    }

    public void UpdateMember(string userId, string role)
    {
        foreach(JSON.UserRole userRole in _userRoles)
        {
            if(userRole.user_id == userId)
            {
                userRole.role = role;
                break;
            }
        }
        JSON.PostProject postProject = new JSON.PostProject() { title = _activatedProject.title, members = _userRoles };
        StartCoroutine(ConnectionManager.Put($"project/{_activatedProject.project_id}", postProject, InitializeAdminPage));
    }

    public void DeleteProject(string projectId)
    {
        StartCoroutine(ConnectionManager.Delete($"project/{projectId}", InitializeAdminPage));
    }

    public void DeleteMember(string userId)
    {
        foreach (JSON.UserRole userRole in _userRoles)
        {
            if (userRole.user_id == userId)
            {
                _userRoles.Remove(userRole);
                break;
            }
        }
        JSON.PostProject postProject = new JSON.PostProject() { title = _activatedProject.title, members = _userRoles };
        StartCoroutine(ConnectionManager.Put($"project/{_activatedProject.project_id}", postProject, InitializeAdminPage));

    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void AddUserOptionToAddProjectPageList(string userId)
    {
        _addProjectPage.transform.GetComponent<AddProjectPageController>().AddUsertoList(userId);
    }
}
