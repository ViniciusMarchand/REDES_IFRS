
using System.Net.Sockets;
using System.Text;

public class Client
{

    TcpClient client;

    internal void Connect()
    {
        //ler ip pelo input
        Console.WriteLine("Digite o IP do servidor:");
        string? serverAddress = Console.ReadLine();
        if (serverAddress == null)
        {
            throw new ArgumentException("Server address cannot be null.");
        }

        //ler porta pelo input
        Console.WriteLine("Digite a porta do servidor:");
        string? portInput = Console.ReadLine();
        if (string.IsNullOrEmpty(portInput))
        {
            throw new ArgumentException("Port cannot be null or empty.");
        }
        int port = int.Parse(portInput);

        if (string.IsNullOrEmpty(serverAddress))
        {
            throw new ArgumentException("Server address cannot be null or empty.");
        }
        client = new(serverAddress, port);
    }

    public async void EnviarNome(NetworkStream stream)
    {
        stream = client.GetStream();

        // aguardar o servidor enviar a mensagem
        byte[] buffer = new byte[256];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string mensagem = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

        Console.WriteLine($"Servidor: {mensagem}"); // mostra "Digite seu nome:"

        // enviar o nome de volta ao servidor
        Console.Write("Digite seu nome: ");
        string nome = Console.ReadLine()?.Trim() ?? string.Empty;
        byte[] respostaBytes = Encoding.UTF8.GetBytes(nome);
        await stream.WriteAsync(respostaBytes, 0, respostaBytes.Length);
        Console.WriteLine("Nome digitado: " + nome);
    }

    public async static Task<string> ReceberMensagem(NetworkStream stream)
    {
        byte[] buffer = new byte[256];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string mensagem = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        return mensagem;
    }

    public async static Task EnviarMensagem (NetworkStream stream)
    {
        string resposta = Console.ReadLine()?.Trim() ?? string.Empty;
        byte[] respostaBytes = Encoding.UTF8.GetBytes(resposta);
        await stream.WriteAsync(respostaBytes, 0, respostaBytes.Length);
        Console.WriteLine("Mensagem enviada: " + resposta);
    }

    public TcpClient Tcp
    {
        get
        {
            return client;
        }
        set
        {
            client = value;
        }
    }
}