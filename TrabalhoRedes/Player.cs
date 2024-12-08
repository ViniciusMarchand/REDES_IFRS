using System.Net.Sockets;

public class Player
{

    public Player(string name, NetworkStream playerStream)
    {
        Name = name;
        PlayerStream = playerStream;
    }

    public string Name { get; set; } = "";
    public NetworkStream PlayerStream { get; set; }


    public void SendMessageToPlayer(string message)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        PlayerStream.Write(messageBytes, 0, messageBytes.Length);
    }

}