using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        int port = 8080;
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        Console.WriteLine($"Servidor ouvindo na porta {port}...");

        List<TcpClient> clientes = new List<TcpClient>(); // Lista de clientes conectados
        List<NetworkStream> streams = new List<NetworkStream>(); // Streams para comunicação com os clientes

        while (clientes.Count < 2) // Aguarda até 2 clientes se conectarem
        {
            TcpClient cliente = await tcpListener.AcceptTcpClientAsync();
            clientes.Add(cliente);
            streams.Add(cliente.GetStream());
            Console.WriteLine("Novo cliente conectado.");
        }

    var perguntas = new (string Enunciado, string Resposta, bool isPerguntaPassada)[]
    {
        ("Qual é o maior planeta do sistema solar?", "Júpiter", false),
        ("Qual é o elemento químico representado pelo símbolo 'Au'?", "Ouro", false),
        ("Qual país é conhecido como 'a terra do sol nascente'?", "Japão", false),
        ("Qual é o idioma mais falado no mundo?", "Mandarim", false),
        ("Em que continente fica o Egito?", "África", false),
        // ("Qual é o nome do maior oceano da Terra?", "Oceano Pacífico", false),
        // ("Qual é o menor país do mundo?", "Vaticano", false),
        // ("Em que ano o homem pisou na Lua pela primeira vez?", "1969", false),
        // ("Qual é a fórmula química da água?", "H2O", false),
        // ("Em que país está localizada a Torre Eiffel?", "França", false),
        // ("Qual é o animal mais rápido do mundo?", "Guepardo", false),
        // ("Quantos ossos tem o corpo humano adulto?", "206", false),
        // ("Qual é a montanha mais alta do mundo?", "Monte Everest", false),
        // ("Quem foi o primeiro presidente dos Estados Unidos?", "George Washington", false),
        // ("Em que ano começou a Primeira Guerra Mundial?", "1914", false),
        // ("Quem é considerado o pai da computação?", "Alan Turing", false),
        // ("Em que ano foi lançada a primeira versão do sistema operacional Windows?", "1985", false),
        // ("Qual é a unidade básica da vida?", "Célula", false)
    };


        int[] pontuacoes = new int[2] { 0, 0 }; // Pontuação dos jogadores
        int turno = 0; // Controla quem é o jogador ativo (0 ou 1)

        for (int i = 0; i < perguntas.Length; i++)
        {
            bool perguntaRespondida = false;
            while (!perguntaRespondida)
            {

                string mensagem = perguntas[i].Enunciado;
                if(perguntas[i].isPerguntaPassada)
                {
                    mensagem = mensagem + " \nEssa mensagem foi passada pelo outro jogador, se você passar essa pergunta você perderá ponto.";
                }


                byte[] perguntaBytes = Encoding.UTF8.GetBytes(mensagem);
                await streams[turno].WriteAsync(perguntaBytes, 0, perguntaBytes.Length);


                Console.WriteLine($"Enviando pergunta {i + 1} para o cliente {turno + 1}: {perguntas[i].Enunciado}\nResponder ou Passar?\n");

                // Pergunta se ele quer responder ou passar
                // byte[] escolhaBytes = Encoding.UTF8.GetBytes("Responder ou Passar?");
                // await streams[turno].WriteAsync(escolhaBytes, 0, escolhaBytes.Length);

                // Espera pela resposta ou pelo comando de passar
                byte[] respostaBytes = new byte[256];
                int bytesRead = await streams[turno].ReadAsync(respostaBytes, 0, respostaBytes.Length);
                string resposta = Encoding.UTF8.GetString(respostaBytes, 0, bytesRead).Trim().ToLower();

                Console.WriteLine($"Aguardando resposta do cliente {turno + 1}...\n");
                if (resposta == "responder")
                {
                    // Recebe a resposta do cliente
                    byte[] respostaClienteBytes = new byte[256];
                    bytesRead = await streams[turno].ReadAsync(respostaClienteBytes, 0, respostaClienteBytes.Length);
                    string respostaCliente = Encoding.UTF8.GetString(respostaClienteBytes, 0, bytesRead).Trim();

                    // Verifica a resposta
                    if (respostaCliente.ToLower() == perguntas[i].Resposta.ToLower())
                    {
                        pontuacoes[turno]++;
                        Console.WriteLine($"Cliente {turno + 1} respondeu corretamente!\n");
                    }
                    else
                    {
                        Console.WriteLine($"Cliente {turno + 1} respondeu incorretamente.\n");
                        pontuacoes[turno]--;
                    }

                    perguntaRespondida = true; // Avança para a próxima pergunta

                }
                else if (resposta == "passar")
                {
                    if(perguntas[i].isPerguntaPassada) 
                    {
                        Console.WriteLine($"Cliente {turno + 1} desistiu da pergunta.\n");
                        perguntas[i] = (perguntas[i].Enunciado, perguntas[i].Resposta, false); // Marca a pergunta como passada
                        perguntaRespondida = true; // Passa para o próximo jogador
                    }
                    else
                    {
                        Console.WriteLine($"Cliente {turno + 1} escolheu passar a pergunta.\n");
                        perguntaRespondida = false; // Passa para o próximo jogador
                        perguntas[i] = (perguntas[i].Enunciado, perguntas[i].Resposta, true); // Marca a pergunta como passada
                    }
                }

                // Envia a pontuação após cada pergunta
                // byte[] pontuacaoBytes = Encoding.UTF8.GetBytes($"Pontuação atual: {pontuacoes[turno]}\n");
                // await streams[turno].WriteAsync(pontuacaoBytes, 0, pontuacaoBytes.Length);

                // Alterna o turno entre os jogadores se não for pergunta passada
                if (!perguntas[i].isPerguntaPassada || !(resposta == "responder"))
                    turno = (turno + 1) % 2;
            }
        }

        // Ao final, mostra a pontuação final de cada jogador
        for (int i = 0; i < 2; i++)
        {
            byte[] resultadoFinalBytes = Encoding.UTF8.GetBytes($"Jogo encerrado! Sua pontuação final: {pontuacoes[i]}\n");
            await streams[i].WriteAsync(resultadoFinalBytes, 0, resultadoFinalBytes.Length);
        }

        // mostra quem ganhou ou se houve empate
        if (pontuacoes[0] > pontuacoes[1])
        {
            byte[] resultadoFinalBytes = Encoding.UTF8.GetBytes("Você ganhou!");
            await streams[0].WriteAsync(resultadoFinalBytes, 0, resultadoFinalBytes.Length);
            byte[] resultadoFinalBytes2 = Encoding.UTF8.GetBytes("Você perdeu!");
            await streams[1].WriteAsync(resultadoFinalBytes2, 0, resultadoFinalBytes2.Length);
        }
        else if (pontuacoes[0] < pontuacoes[1])
        {
            byte[] resultadoFinalBytes = Encoding.UTF8.GetBytes("Você perdeu!");
            await streams[0].WriteAsync(resultadoFinalBytes, 0, resultadoFinalBytes.Length);
            byte[] resultadoFinalBytes2 = Encoding.UTF8.GetBytes("Você ganhou!");
            await streams[1].WriteAsync(resultadoFinalBytes2, 0, resultadoFinalBytes2.Length);
        }
        else
        {
            byte[] resultadoFinalBytes = Encoding.UTF8.GetBytes("Empate!");
            await streams[0].WriteAsync(resultadoFinalBytes, 0, resultadoFinalBytes.Length);
            await streams[1].WriteAsync(resultadoFinalBytes, 0, resultadoFinalBytes.Length);
        }

        // Fecha a conexão
        foreach (var cliente in clientes)
        {
            cliente.Close();
        }
    }
}
