using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MemberBlock : MonoBehaviour
{
    public string userId;
    public string role;
    public TextMeshProUGUI _userId;
    public TextMeshProUGUI _role;
    public TMP_Dropdown _roleOption;
    public void UpdateProjectObj()
    {
        _userId.text = userId;
        _role.text = role;
    }
    public void ShowMembers()
    {
        string roleCode = "";
        switch (_roleOption.captionText.text)
        {
            case "Tester":
                roleCode = "TESTER";
                break;
            case "Project Leader":
                roleCode = "PL";
                break;
            case "Developer":
                roleCode = "DEV";
                break;
        }
        AdminUIManager.instance.UpdateMember(userId, roleCode);
    }
    public void DeleteMember()
    {
        AdminUIManager.instance.DeleteMember(userId);
    }
}
