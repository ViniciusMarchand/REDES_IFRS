using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string serverAddress = "127.0.0.1"; // Substitua pelo IP ou hostname do servidor
        //docker: host.docker.internal
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
                // Verifica se o jogo acabou
                if (pergunta.StartsWith("Jogo encerrado"))
                {
                    Console.WriteLine(pergunta);
                    break;
                }

                // Mostra a pergunta
                Console.WriteLine($"Servidor: {pergunta}");

                // Envia ao servidor a escolha (responder ou passar)
                Console.WriteLine("Digite 'responder' para responder ou 'passar' para passar a pergunta:");
                string escolha = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                // Envia a escolha para o servidor
                byte[] escolhaBytes = Encoding.UTF8.GetBytes(escolha);
                await stream.WriteAsync(escolhaBytes, 0, escolhaBytes.Length);

                // Se o jogador escolher responder, envia a resposta
                if (escolha == "responder")
                {
                    Console.Write("Sua resposta: ");
                    string resposta = Console.ReadLine()?.Trim() ?? string.Empty;
                    byte[] respostaBytes = Encoding.UTF8.GetBytes(resposta);
                    await stream.WriteAsync(respostaBytes, 0, respostaBytes.Length);
                    // Aguarda e exibe a pontuação
                    buffer = new byte[256];
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string pontuacao = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine(pontuacao + "AQUI");
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
