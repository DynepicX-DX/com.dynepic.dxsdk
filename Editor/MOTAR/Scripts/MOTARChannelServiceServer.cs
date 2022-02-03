using System;
using System.Text;
using UnityEditor.MPE;
using UnityEditor;
using UnityEngine;

public static class ChannelCommunicationDocExample
{
    [MenuItem("ChannelDoc/Step 1")]
    static void StartChannelService()
    {
        if (!ChannelService.IsRunning())
        {
            ChannelService.Start();
        }
        Debug.Log($"[Step1] ChannelService Running: {ChannelService.GetAddress()}:{ChannelService.GetPort()}");
    }

    static int s_BinaryChannelId;
    static int s_StringChannelId;
    static Action s_DisconnectBinaryChannel;
    static Action s_DisconnectStringChannel;
    static int savedID = 0;

    [MenuItem("ChannelDoc/Step 2")]
    static void SetupChannelService()
    {
        if (s_DisconnectBinaryChannel == null)
        {
            s_DisconnectBinaryChannel = ChannelService.GetOrCreateChannel("custom_binary_ping_pong", HandleChannelBinaryMessage);
            s_BinaryChannelId = ChannelService.ChannelNameToId("custom_binary_ping_pong");
        }
        Debug.Log($"[Step2] Setup channel_custom_binary id: {s_BinaryChannelId}");

        if (s_DisconnectStringChannel == null)
        {
            s_DisconnectStringChannel = ChannelService.GetOrCreateChannel("custom_ascii_ping_pong", HandleChannelStringMessage);
            s_StringChannelId = ChannelService.ChannelNameToId("custom_ascii_ping_pong");
        }
        Debug.Log($"[Step2] Setup channel_custom_ascii id: {s_StringChannelId}");
    }

    static void HandleChannelBinaryMessage(int connectionId, byte[] data)
    {
        var msg = "";
        for (var i = 0; i < Math.Min(10, data.Length); ++i)
        {
            msg += data[i].ToString();
        }
        Debug.Log($"Channel Handling binary from connection {connectionId} - {data.Length} bytes - {msg}");

        // Client has sent a message (this is a ping)
        // Lets send back the same message (as a pong)
        ChannelService.Send(connectionId, data);
    }
    public static void OutputSelection()
    {
        // Client has sent a message (this is a ping)
        // Client expects string data. Encode the data and send it back as a string:

       
        if(Selection.activeObject != null)
            ChannelService.Send(savedID, Selection.activeObject.name);
    }
    static void HandleChannelStringMessage(int connectionId, byte[] data)
    {
        // Client has sent a message (this is a ping)
        // Client expects string data. Encode the data and send it back as a string:

        var msgStr = Encoding.UTF8.GetString(data);
        Debug.Log($"Channel Handling string from connection {connectionId} - {msgStr}");

        // Send back the same message (as a pong)
        ChannelService.Send(connectionId, msgStr);
        savedID = connectionId;
    }

    static ChannelClient s_BinaryClient;
    static Action s_DisconnectBinaryClient;
    static ChannelClient s_StringClient;
    static Action s_DisconnectStringClient;
    [MenuItem("ChannelDoc/Step 3")]
    static void SetupChannelClient()
    {
        const bool autoTick = true;

        if (s_BinaryClient == null)
        {
            s_BinaryClient = ChannelClient.GetOrCreateClient("custom_binary_ping_pong");
            s_BinaryClient.Start(autoTick);
            s_DisconnectBinaryClient = s_BinaryClient.RegisterMessageHandler(HandleClientBinaryMessage);
        }
        Debug.Log($"[Step3] Setup client for channel custom_binary_ping_pong. ClientId: {s_BinaryClient.clientId}");

        if (s_StringClient == null)
        {
            s_StringClient = ChannelClient.GetOrCreateClient("custom_ascii_ping_pong");
            s_StringClient.Start(autoTick);
            s_DisconnectStringClient = s_StringClient.RegisterMessageHandler(HandleClientStringMessage);
        }
        Debug.Log($"[Step3] Setup client for channel custom_ascii_ping_pong. ClientId: {s_StringClient.clientId}");
    }

    static void HandleClientBinaryMessage(byte[] data)
    {
        Debug.Log($"Receiving pong binary data: {data} for clientId: {s_BinaryClient.clientId} with channelName: {s_BinaryClient.channelName}");
    }

    static void HandleClientStringMessage(string data)
    {
        Debug.Log($"Receiving pong data: {data} for clientId: {s_StringClient.clientId} with channelName: {s_StringClient.channelName}");
    }

    [MenuItem("ChannelDoc/Step 4")]
    static void ClientSendMessageToServer()
    {
        Debug.Log("[Step 4]: Clients are sending data!");
        s_BinaryClient.Send(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        s_StringClient.Send("Hello world!");
    }

    [MenuItem("ChannelDoc/Step 5")]
    static void CloseClients()
    {
        Debug.Log("[Step 5]: Closing clients");
        s_DisconnectBinaryClient();
        s_BinaryClient.Close();

        s_DisconnectStringClient();
        s_StringClient.Close();
    }

    [MenuItem("ChannelDoc/Step 6")]
    static void CloseService()
    {
        Debug.Log("[Step 6]: Closing clients");

        if(s_DisconnectBinaryChannel != null)
            s_DisconnectBinaryChannel();
        if(s_DisconnectStringChannel != null)
            s_DisconnectStringChannel();

        ChannelService.Stop();
    }
}
