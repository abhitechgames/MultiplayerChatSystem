using System;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Auth;
using StreamChat.Core.Events;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEngine;
using Photon.Pun;

public class StreamChatBehaviour : MonoBehaviour
{
    public static StreamChatBehaviour instance;
    ChannelState channelState;

    public Transform content;
    public GameObject messagePurple;
    public GameObject messageGreen;


    protected void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this);
    }

    public void CreateClient(string userName)
    {
        try
        {
            string userId = StreamChatClient.SanitizeUserId(userName);
            //Replace with your credentials
            var authCredentials = new AuthCredentials("6zqxq5snrbyh", userId, StreamChatClient.CreateDeveloperAuthToken(userId));

            //Create IStreamChatClient
            _client = StreamChatClient.CreateDefaultClient(authCredentials);

            //Connect user
            _client.Connect();

            //Log when Stream Chat Client has successfully connected the user
            _client.Connected += OnStreamChatConnected;

            _client.MessageReceived += OnStreamChatMessageReceived;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    //Update client in order to send/receive data between the Stream Chat API
    protected void Update() => _client.Update(Time.deltaTime);

    //Dispose client in order to cleanup connection
    protected void OnDestroy() => _client.Dispose();

    private IStreamChatClient _client;

    private void OnStreamChatConnected()
    {
        Debug.Log("Stream Chat User connected!");
    }

    public void RoomJoinedCreateChannel(string roomName)
    {
        CreateAChannel(roomName);
    }

    private async void CreateAChannel(string roomName)
    {
        try
        {
            channelState = await GetOrCreateChannelAsync(roomName);

            await SendMessageAsync("Hello", channelState.Channel.Type, channelState.Channel.Id);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task<ChannelState> GetOrCreateChannelAsync(string roomName)
    {
        var getOrCreateRequest = new ChannelGetOrCreateRequest
        {
            State = true,
            //Tell server to watch channel and receive events when anything in channel changes
            Watch = true,
        };

        //Response contains channel state with messages, members and much more
        var channelStateResponse = await _client.ChannelApi.GetOrCreateChannelAsync(
            channelType: "livestream", channelId: roomName, getOrCreateRequest);

        Debug.Log($"Channel with Id: {channelStateResponse.Channel.Id} has been created");

        return channelStateResponse;
    }

    private async Task SendMessageAsync(string message, string channelType, string channelId)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Message = new MessageRequest
            {
                Text = message
            }
        };

        var messageResponse = await _client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);
    }

    private void OnStreamChatMessageReceived(EventMessageNew messageNew)
    {
        Debug.Log($"New message received for channel: `{messageNew.ChannelId}` and text: `{messageNew.Message.Text}`");
        Message message;
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
            message = Instantiate(messagePurple, content).GetComponent<Message>();
        else
            message = Instantiate(messageGreen, content).GetComponent<Message>();

        message.message.text = messageNew.Message.Text;
        message.sender.text = messageNew.User.Id;
    }

    public void Messaging(string message)
    {
        SendMessageChat(message);
    }

    private async void SendMessageChat(string message)
    {
        await SendMessageAsync(message, channelState.Channel.Type, channelState.Channel.Id);
    }
}