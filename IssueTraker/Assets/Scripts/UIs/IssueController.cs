using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IssueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private Image _statusImg;
    [SerializeField] private TextMeshProUGUI _reporterTxt;
    [SerializeField] private TextMeshProUGUI _issueContentsTxt;
    [SerializeField] private TextMeshProUGUI _assigneeTxt;
    [SerializeField] private TMP_Dropdown _assigneeSuggestionDrp;
    [SerializeField] private Transform _commentContentsTrf;
    [SerializeField] private TMP_InputField _newCommentIpF;
    [SerializeField] private Button _disposeIssueBtn;
    [SerializeField] private Button _issueFixedBtn;
    [SerializeField] private Button _closeIssueBtn;
    [SerializeField] private Button _commentAssignBtn;


    [SerializeField] private GameObject commnetPrefab;
    private void OnEnable()
    {
        
    }
}
