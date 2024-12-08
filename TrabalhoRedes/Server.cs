using System.Net;
using System.Net.Sockets;

public class Server
{

    private TcpListener tcpListener = new TcpListener(IPAddress.Any, 5000);

    public void Start()
    {
        // espera o usuadio digitar o IP
        Console.WriteLine("Digite o IP que deseja ouvir: ");
        string? ip = Console.ReadLine();

        if (string.IsNullOrEmpty(ip))
        {
            throw new ArgumentException("IP não pode ser nulo ou vazio.");
        }

        //espera o usuario digitar uma porta
        Console.WriteLine("Digite a porta que deseja ouvir: ");
        int port = Convert.ToInt32(Console.ReadLine());

        //cria um objeto do tipo TcpListener
        tcpListener = new TcpListener(IPAddress.Parse(ip), port);
        tcpListener.Start();
        Console.WriteLine($"Servidor ouvindo na porta {port}...");

    }

    public async Task<(List<TcpClient> clientes, List<NetworkStream> streams)> ConnectClients()
    {
        List<TcpClient> clientes = new(); // Lista de clientes conectados
        List<NetworkStream> streams = new(); // Streams para comunicação com os clientes

        while (clientes.Count < 2) // Aguarda até 2 clientes se conectarem
        {
            TcpClient cliente = await tcpListener.AcceptTcpClientAsync();
            clientes.Add(cliente);
            streams.Add(cliente.GetStream());
            Console.WriteLine("Novo cliente conectado.");
        }

        return (clientes, streams);
    }
}