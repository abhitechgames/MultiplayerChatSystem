using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField]
    private GameObject chatBox;

    [SerializeField]
    private GameObject enableButton;

    [SerializeField]
    private TMP_InputField message;

    private void Start()
    {
        StreamChatBehaviour.instance.content = GameObject.FindGameObjectWithTag("content").transform; ;
    }

    UIManager()
    {
        instance = this;
    }

    public void EnableChat()
    {
        _EnableChatSystem();
        enableButton.SetActive(false);
    }

    public void DisableChat()
    {
        _DisableChatSystem();
        enableButton.SetActive(true);
    }

    private void _EnableChatSystem()
    {
        chatBox.SetActive(true);
    }

    private void _DisableChatSystem()
    {
        chatBox.SetActive(false);
    }

    public void SendMessage() { if (message.text.Length >= 1) StreamChatBehaviour.instance.Messaging(message.text); }
}