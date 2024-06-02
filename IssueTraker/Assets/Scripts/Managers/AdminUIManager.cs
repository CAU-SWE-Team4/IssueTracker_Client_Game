using JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _memberPrefab;
    [SerializeField] private GameObject _projectPrefab;
    [SerializeField] private Transform _memberContainer;
    [SerializeField] private Transform _projectContainer;

    public static AdminUIManager instance;

    private void Awake()
    {
        if (instance && !instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        GetProject();
    }

    public void AddProject()
    {

    }

    public void GetProject()
    {
        StartCoroutine(ConnectionManager.Get<JSON.GetProjects>($"project", "", "", CreateProject));
    }

    private void CreateProject(GetProjects obj)
    {
        foreach(JSON.Project project in obj.projects)
        {
            GameObject projectObj = Instantiate(_projectPrefab, _projectContainer);

        }
    }

    public void GetMembers()
    {

    }

    public void ClearMembers()
    {
        for(int i=0; i<_memberContainer.childCount; i++)
        {
            Destroy(_memberContainer.GetChild(i));
        }
    }


    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
