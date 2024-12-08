using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {

        try
        {   
            while(true)
            {
                Client client = new();

                client.Connect();

                Console.WriteLine("Conectado ao servidor!");

                NetworkStream stream = client.Tcp.GetStream();
        
                client.EnviarNome(stream);

                while (true)
                {
                    // Aguarda a pergunta do servidor
                    string pergunta = await Client.ReceberMensagem(stream);
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
                        await Client.EnviarMensagem(stream);

                    }

                    string pontos = await Client.ReceberMensagem(stream);

                    Console.WriteLine($"Pontuação: {pontos}");

                }

                string resultado = await Client.ReceberMensagem(stream);
                Console.WriteLine(resultado);

                Console.WriteLine("Deseja jogar novamente? (s/n)");

                string jogarNovamente = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                if (jogarNovamente == "n")
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
