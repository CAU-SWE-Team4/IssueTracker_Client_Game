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
        [SerializeField] private Transform _projectSpawn;
        [SerializeField] private TankShooting _tankShooting;
        [SerializeField] private UserUIManager _userUIManager;
        public static GameManager instance { get; private set; }
        private void Awake()
        {
            if (instance && !instance.Equals(this))
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void Exit()
        {
            SceneManager.LoadScene(0);
        }

        private void Start()
        {
            StartCoroutine(ConnectionManager.Get<JSON.GetProjects>($"project", "", "", CreateProject));
        }

        private void CreateProject(JSON.GetProjects response)
        {
            Debug.Log("GetProjectInfoSucceed");
            if (response.projects == null || response.projects.Count == 0) return;
            int pos = 0;
            foreach(JSON.Project project in response.projects)
            {
                GameObject Camp = InstantiateCamp(project);
                Camp.transform.position = _projectSpawn.position + Vector3.forward * pos;
                Camp.transform.Rotate(new Vector3(0, 225, 0));
                pos+=10;
            }
            Debug.Log(response.projects.Count);
        }
        private GameObject InstantiateCamp(JSON.Project project)
        {
            GameObject projectObj = Instantiate(_campPrefab, _projectSpawn);
            projectObj.transform.GetComponent<ProjectObj>().title = project.title;
            projectObj.transform.GetComponent<ProjectObj>().project_id = project.project_id;
            projectObj.transform.GetComponent<ProjectObj>().created_at = project.created_at;
            Debug.Log("project info : " + JsonUtility.ToJson(project));
            return projectObj;
        }

        public void ShootProject(string project_id, string project_title)
        {
            Time.timeScale = 0;
            _userUIManager.ShowProjectDashboard(project_id, project_title);
        }
        public void ExitUserUI()
        {
            Time.timeScale = 1;
            Debug.Log(Time.timeScale);
            _tankShooting.DestroyShell();
            _userUIManager.HideAllUI();
        }
    }
}