using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddMemberPageController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _userIdDrp;
    [SerializeField] private TMP_Dropdown _roleDrp;

    public void AddMember()
    {
        string role = "";
        switch (_roleDrp.captionText.text)
        {
            case "Tester":
                role = "TESTER";
                break;
            case "Project Leader":
                role = "PL";
                break;
            case "Developer":
                role = "DEV";
                break;
        }
        AdminUIManager.instance.AddMember(_userIdDrp.captionText.text, role);
        this.gameObject.SetActive(false);
    }

    public void AddMemberOption(string userId)
    {
        _userIdDrp.AddOptions(new List<string> { userId });
    }
}
