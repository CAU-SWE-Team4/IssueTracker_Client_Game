using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddProjectMemberObjController : MonoBehaviour
{
    public string userId;
    public void Delete()
    {
        AdminUIManager.instance.AddUserOptionToAddProjectPageList(userId);
        this.gameObject.SetActive(false);
    }
}
