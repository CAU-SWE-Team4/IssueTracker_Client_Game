using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CommentController : MonoBehaviour
{
    public string commentId;
    public string authorId;
    public string authorName;
    public string content;
    public string createdDate;
    public string projectId;
    public string issueId;

    [SerializeField] private TextMeshProUGUI _authorTxt;
    [SerializeField] private TextMeshProUGUI _dateTxt;
    [SerializeField] private TextMeshProUGUI _contentTxt;
    [SerializeField] private GameObject _optionPage;
    [SerializeField] private TMP_InputField _inputField;

    public void ChangeOptionPage()
    {
        _optionPage.SetActive(!_optionPage.active);
    }

    public void UpdateCommentObj()
    {
        _authorTxt.text = authorName;
        _dateTxt.text = $"commented at {createdDate.Substring(0, 4)}-{createdDate.Substring(5, 2)}-{createdDate.Substring(8, 2)}";
        _contentTxt.text = content;
    }

    public void OpenEditor()
    {
        if(authorId == ConnectionManager.id)
        _inputField.gameObject.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void DeleteComment()
    {
        StartCoroutine(ConnectionManager.Delete($"project/{projectId}/issue/{issueId}/comment/{commentId}"));
    }

    public void EditComment()
    {
        JSON.EditCommentContent editCommentContent = new JSON.EditCommentContent { content = _inputField.text };
        Debug.Log(JsonUtility.ToJson(editCommentContent));
        StartCoroutine(ConnectionManager.Put($"project/{projectId}/issue/{issueId}/comment/{commentId}", editCommentContent));
        CancelEdit();
        UserUIManager.instance.CommentListRefresh();
    }

    public void CancelEdit()
    {
        _inputField.text = "";
        _inputField.gameObject.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
