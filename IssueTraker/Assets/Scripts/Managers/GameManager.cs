using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tank
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject _campPrefab;

        private void Awake()
        {
            StartCoroutine(ConnectionManager.Get<Assets.Scripts.JSON.GetProjects>($"/project", CreateProject));
        }

        private void CreateProject(Assets.Scripts.JSON.GetProjects response)
        {
            if (response.projects == null || response.projects.Count == 0) return;
            foreach(Assets.Scripts.JSON.Project project in response.projects)
            {
                InstantiateCamp(project);
            }
            Debug.Log(response.projects.Count);
        }
        private void InstantiateCamp(Assets.Scripts.JSON.Project project)
        {
            Instantiate(_campPrefab);
        }
    }
}