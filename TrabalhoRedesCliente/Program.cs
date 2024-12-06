using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string serverAddress = "host.docker.internal";
        int port = 8080;

        try
        {
            using TcpClient client = new TcpClient(serverAddress, port);
            Console.WriteLine("Conectado ao servidor!");

            using NetworkStream stream = client.GetStream();

            while (true)
            {
                // Aguarda a pergunta do servidor

                byte[] buffer = new byte[256];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string pergunta = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine("Pergunta recebida  com sucesso");

                if (pergunta.StartsWith("Jogo encerrado"))
                {
                    Console.WriteLine(pergunta);
                    break;
                }


                Console.WriteLine($"Servidor: {pergunta}");

                Console.WriteLine("Digite 'responder' para responder ou 'passar' para passar a pergunta:");
                string escolha = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                byte[] escolhaBytes = Encoding.UTF8.GetBytes(escolha);
                await stream.WriteAsync(escolhaBytes, 0, escolhaBytes.Length);

                if (escolha == "responder")
                {
                    Console.Write("Sua resposta: ");
                    string resposta = Console.ReadLine()?.Trim() ?? string.Empty;
                    byte[] respostaBytes = Encoding.UTF8.GetBytes(resposta);
                    await stream.WriteAsync(respostaBytes, 0, respostaBytes.Length);
                    // Aguarda e exibe a pontuação
                    // buffer = new byte[256];
                    // bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    // string pontuacao = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    // Console.WriteLine(pontuacao);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
