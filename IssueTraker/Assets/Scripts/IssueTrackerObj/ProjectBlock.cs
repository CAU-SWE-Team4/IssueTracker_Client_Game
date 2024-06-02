using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tank
{
    public class ProjectObj : MonoBehaviour
    {
        private string _title;
        public string title
        {
            set
            {
                _title = value;
                UpdateProjectTitle();
            }
            get
            {
                return _title;
            }
        }
        public TMPro.TextMeshPro mPro;
        private void UpdateProjectTitle()
        {
            mPro.text = title;
        }

        public string project_id;
        public string created_at;
    }
}