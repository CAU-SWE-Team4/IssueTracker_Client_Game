using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddProjectPageController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _titleIpF;
    [SerializeField] private TMP_Dropdown _userIdDrp;
    [SerializeField] private TMP_Dropdown _roleDrp;
    [SerializeField] private GameObject _newMemberPrefab;
    [SerializeField] private Transform _newMemberContainer;

    JSON.PostProject _projectInfo = new JSON.PostProject() { title = "", members = new List<JSON.UserRole>() };
    public void Initialize()
    {
        ClearMemberList();
        _titleIpF.text = string.Empty;
    }
    private void ClearMemberList()
    {
        _userIdDrp.ClearOptions();
        if (_newMemberContainer == null || _newMemberContainer.childCount == 0) return;
        for(int i=0;i<_newMemberContainer.childCount;i++)
        {
            Destroy(_newMemberContainer.GetChild(i).gameObject);
        }
    }

    public void AddUser()
    {
        if (_userIdDrp.captionText.text == null || _userIdDrp.captionText.text.Length == 0) return;
        foreach (JSON.UserRole member in _projectInfo.members)
        {
            if (member.user_id == _userIdDrp.captionText.text)
                return;
        }
        string role = string.Empty;
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
        JSON.UserRole newMember = new JSON.UserRole { user_id = _userIdDrp.captionText.text, role = role };
        GameObject memberObj = Instantiate(_newMemberPrefab, _newMemberContainer);
        memberObj.transform.GetComponent<AddProjectMemberObjController>().userId = _userIdDrp.captionText.text;
        memberObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _userIdDrp.captionText.text;
        memberObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _roleDrp.captionText.text;
        _projectInfo.members.Add(newMember);
        _userIdDrp.options.Remove(new TMP_Dropdown.OptionData() { text = newMember.user_id });
    }

    public void AddUsertoList(string userId)
    {
        _userIdDrp.AddOptions(new List<string>() { userId });
        if (_projectInfo.members == null || _projectInfo.members.Count == 0) return;
        foreach(JSON.UserRole member in _projectInfo.members)
        {
            if (member.user_id == userId)
            {
                _projectInfo.members.Remove(member);
                break;
            }
        }
    }

    public void AddProject()
    {
        _projectInfo.title = _titleIpF.text;
        _projectInfo.members.Add(new JSON.UserRole() { user_id = "admin", role = "ADMIN" });
        if (_projectInfo.title == string.Empty) return;
        StartCoroutine(ConnectionManager.Post("project", _projectInfo));
        Initialize();
        AdminUIManager.instance.InitializeAdminPage();
        this.gameObject.SetActive(false);
    }

    public void CancelAddProjecT()
    {
        Initialize();
        this.gameObject.SetActive(false);
    }
}
