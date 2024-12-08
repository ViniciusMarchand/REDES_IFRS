public class Perguntas
{
    public static List<(string Enunciado, string Resposta, bool isPerguntaPassada)> GetPerguntas()
    {
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

        //embaralhar perguntas
        Random random = new Random();
        var perguntasEmbaralhadas = perguntas.OrderBy(x => random).ToList();

        return perguntas.ToList();
    }
}