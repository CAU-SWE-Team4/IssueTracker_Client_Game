using Assets.Scripts.JSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tank
{
    public class GameManager : MonoBehaviour
    {

        private void Awake()
        {
            StartCoroutine(ConnectionManager.Get<Assets.Scripts.JSON.GetProjects>($"/project", CreateProject));
        }

        private void CreateProject(Response<Assets.Scripts.JSON.GetProjects> response)
        {
            throw new NotImplementedException();
        }
    }
}