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

        Server server = new Server();
        server.Start();


        while(true)
        {
            (List<TcpClient> clientes, List<NetworkStream> streams) = await server.ConnectClients();
            var perguntas = Perguntas.GetPerguntas();


            //criar  um objeto do tipo Player

            List<Player> players = new();

            for(int i = 0; i < 2; i++)
            {
                //mandar mensagem para o cliente 1
                byte[] mensagemBytes = Encoding.UTF8.GetBytes("Digite seu nome: ");
                await streams[i].WriteAsync(mensagemBytes, 0, mensagemBytes.Length);

                //aguardar nome do cliente 
                byte[] nomeBytes = new byte[256];
                int bytesRead = await streams[i].ReadAsync(nomeBytes, 0, nomeBytes.Length);
                string nome = Encoding.UTF8.GetString(nomeBytes, 0, bytesRead).Trim();
                Player player = new(nome, streams[i]);

                players.Add(player);
            }

            Console.WriteLine($"Clientes {players[0].Name} e {players[1].Name} conectados. Iniciando jogo...\n");

            //sortear lista dos players
            Random random = new();
            players = players.OrderBy(x => random.Next()).ToList();
            
            int[] pontuacoes = new int[2] { 0, 0 }; // Pontuação dos jogadores
            int turno = 0; // Controla quem é o jogador ativo (0 ou 1)

            for (int i = 0; i < perguntas.Count; i++)
            {
                bool perguntaRespondida = false;
                while (!perguntaRespondida)
                {

                    string mensagem = perguntas[i].Enunciado;
                    if (perguntas[i].isPerguntaPassada)
                    {
                        mensagem = mensagem + " \nEssa mensagem foi passada pelo outro jogador, se você passar essa pergunta você perderá ponto.";
                    }


                    byte[] perguntaBytes = Encoding.UTF8.GetBytes(mensagem);
                    await players[turno].PlayerStream.WriteAsync(perguntaBytes, 0, perguntaBytes.Length);


                    Console.WriteLine($"Enviando pergunta {i + 1} para o {players[turno].Name}: {perguntas[i].Enunciado}\nResponder ou Passar?\n");

        

                    // Espera pela resposta ou pelo comando de passar
                    byte[] respostaBytes = new byte[256];
                    int bytesRead = await players[turno].PlayerStream.ReadAsync(respostaBytes, 0, respostaBytes.Length);
                    string resposta = Encoding.UTF8.GetString(respostaBytes, 0, bytesRead).Trim().ToLower();

                    Console.WriteLine($"Aguardando resposta do {players[turno].Name}...\n");
                    if (resposta == "responder")
                    {
                        // Recebe a resposta do cliente
                        byte[] respostaClienteBytes = new byte[256];
                        bytesRead = await players[turno].PlayerStream.ReadAsync(respostaClienteBytes, 0, respostaClienteBytes.Length);
                        string respostaCliente = Encoding.UTF8.GetString(respostaClienteBytes, 0, bytesRead).Trim();
                        Console.WriteLine("pergunta: " + perguntas[i].Enunciado);
                        Console.WriteLine("resposta: " + perguntas[i].Resposta);
                        Console.WriteLine("respostaCliente: " + respostaCliente);
                        // Verifica a resposta
                        if (respostaCliente.ToLower() == perguntas[i].Resposta.ToLower())
                        {
                            pontuacoes[turno]++;
                            Console.WriteLine($"{players[turno].Name} respondeu corretamente!\n");
                        }
                        else
                        {
                            Console.WriteLine($"{players[turno].Name} respondeu incorretamente.\n");
                            pontuacoes[turno]--;
                        }

                        perguntaRespondida = true; // Avança para a próxima pergunta

                    }
                    else if (resposta == "passar")
                    {
                        if (perguntas[i].isPerguntaPassada)
                        {
                            Console.WriteLine($"{players[turno].Name} desistiu da pergunta.\n");
                            perguntas[i] = (perguntas[i].Enunciado, perguntas[i].Resposta, false); // Marca a pergunta como passada
                            perguntaRespondida = true; // Passa para o próximo jogador
                            pontuacoes[turno]--;
                        }
                        else
                        {
                            Console.WriteLine($"{players[turno].Name} escolheu passar a pergunta.\n");
                            perguntaRespondida = false; // Passa para o próximo jogador
                            perguntas[i] = (perguntas[i].Enunciado, perguntas[i].Resposta, true); // Marca a pergunta como passada
                        }
                    }

                    players[turno].SendMessageToPlayer($"Sua pontuação: {pontuacoes[turno]}\n");
                    // Alterna o turno entre os jogadores se não for pergunta passada
                    if (!perguntas[i].isPerguntaPassada || !(resposta == "responder"))
                        turno = (turno + 1) % 2;
                }
            }

            // Ao final, mostra a pontuação final de cada jogador
            for (int i = 0; i < 2; i++)
            {
                players[i].SendMessageToPlayer($"Jogo encerrado! Sua pontuação final: {pontuacoes[i]}\n");
            }

            // mostra quem ganhou ou se houve empate
            if (pontuacoes[0] > pontuacoes[1])
            {
                players[0].SendMessageToPlayer($"Você ganhou!");
                players[1].SendMessageToPlayer($"Você perdeu!");
            }
            else if (pontuacoes[0] < pontuacoes[1])
            {
                players[1].SendMessageToPlayer($"Você ganhou!");
                players[0].SendMessageToPlayer($"Você perdeu!");
            }
            else
            {
                players[0].SendMessageToPlayer($"Empate!");
                players[1].SendMessageToPlayer($"Empate!");
            }

            // Fecha a conexão
            foreach (var cliente in clientes)
            {
                cliente.Close();
            }
        }
    }
}
