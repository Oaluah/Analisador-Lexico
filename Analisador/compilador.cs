using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CompiladorLexico
{
    // =========================================
    // ENUMERAÇÃO DOS TOKENS
    // =========================================
    [JsonConverter(typeof(JsonStringEnumConverter))]
    enum TokenType
    {
        // Palavras reservadas
        T_INT,
        T_FLOAT,
        T_IF,
        T_ELSE,
        T_WHILE,
        T_PRINT,

        // Identificadores e valores
        T_ID,
        T_NUM,
        T_STRING,

        // Operadores
        T_ASSIGN,
        T_EQ,

        T_PLUS,
        T_MINUS,
        T_MULT,
        T_DIV,

        T_LT,
        T_GT,

        // Agrupamento
        T_LPAREN,
        T_RPAREN,

        T_LBRACE,
        T_RBRACE,

        // Delimitador
        T_SEMICOLON,

        // Fim do arquivo
        T_EOF
    }

    // =========================================
    // CLASSE TOKEN
    // =========================================
    class Token
    {
        public TokenType Type { get; set; }
        public string Lexeme { get; set; }
        public int Line { get; set; }

        public Token(TokenType type, string lexeme, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
        }
    }

    // =========================================
    // SCANNER LÉXICO
    // =========================================
    class Scanner
    {
        private string input;
        private int pos;
        private int line;

        private Dictionary<string, TokenType> keywords;

        public Scanner(string source)
        {
            input = source;
            pos = 0;
            line = 1;

            keywords = new Dictionary<string, TokenType>()
            {
                {"int", TokenType.T_INT},
                {"float", TokenType.T_FLOAT},
                {"if", TokenType.T_IF},
                {"else", TokenType.T_ELSE},
                {"while", TokenType.T_WHILE},
                {"print", TokenType.T_PRINT}
            };
        }

        // =========================================
        // RETORNA O CARACTERE ATUAL
        // =========================================
        private char Peek()
        {
            if (pos >= input.Length)
                return '\0';

            return input[pos];
        }

        // =========================================
        // AVANÇA LEITURA
        // =========================================
        private char Next()
        {
            char c = Peek();

            if (c != '\0')
                pos++;

            return c;
        }

        // =========================================
        // IGNORA ESPAÇOS
        // =========================================
        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(Peek()))
            {
                if (Next() == '\n')
                    line++;
            }
        }

        // =========================================
        // IGNORA COMENTÁRIOS
        // =========================================
        private void SkipComment()
        {
            while (Peek() != '\n' && Peek() != '\0')
            {
                Next();
            }
        }

        // =========================================
        // LÊ NÚMEROS (INT E FLOAT)
        // =========================================
        private Token ScanNumber(char start)
        {
            string buffer = "";
            bool hasDot = false;

            buffer += start;

            while (char.IsDigit(Peek()) || Peek() == '.')
            {
                if (Peek() == '.')
                {
                    if (hasDot)
                        break;

                    hasDot = true;
                }

                buffer += Next();
            }

            return new Token(TokenType.T_NUM, buffer, line);
        }

        // =========================================
        // LÊ IDENTIFICADORES
        // =========================================
        private Token ScanIdentifier(char start)
        {
            string buffer = "";
            buffer += start;

            while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                buffer += Next();
            }

            // Palavra reservada
            if (keywords.ContainsKey(buffer))
            {
                return new Token(keywords[buffer], buffer, line);
            }

            // Identificador comum
            return new Token(TokenType.T_ID, buffer, line);
        }

        // =========================================
        // LÊ STRINGS
        // =========================================
        private Token ScanString()
        {
            string buffer = "";

            while (Peek() != '"' && Peek() != '\0')
            {
                if (Peek() == '\n')
                    line++;

                buffer += Next();
            }

            // Verifica string não finalizada
            if (Peek() == '\0')
            {
                throw new Exception(
                    $"Erro Léxico: string não finalizada na linha {line}"
                );
            }

            // Consome aspas finais
            Next();

            return new Token(TokenType.T_STRING, buffer, line);
        }

        // =========================================
        // RETORNA PRÓXIMO TOKEN
        // =========================================
        public Token NextToken()
        {
            SkipWhitespace();

            if (pos >= input.Length)
            {
                return new Token(TokenType.T_EOF, "", line);
            }

            char c = Next();

            // Número
            if (char.IsDigit(c))
            {
                return ScanNumber(c);
            }

            // Identificador
            if (char.IsLetter(c) || c == '_')
            {
                return ScanIdentifier(c);
            }

            switch (c)
            {
                case '+':
                    return new Token(TokenType.T_PLUS, "+", line);

                case '-':
                    return new Token(TokenType.T_MINUS, "-", line);

                case '*':
                    return new Token(TokenType.T_MULT, "*", line);

                case '/':

                    // Comentário de linha
                    if (Peek() == '/')
                    {
                        Next();
                        SkipComment();
                        return NextToken();
                    }

                    return new Token(TokenType.T_DIV, "/", line);

                case '=':

                    // Operador ==
                    if (Peek() == '=')
                    {
                        Next();
                        return new Token(TokenType.T_EQ, "==", line);
                    }

                    return new Token(TokenType.T_ASSIGN, "=", line);

                case '<':
                    return new Token(TokenType.T_LT, "<", line);

                case '>':
                    return new Token(TokenType.T_GT, ">", line);

                case '(':
                    return new Token(TokenType.T_LPAREN, "(", line);

                case ')':
                    return new Token(TokenType.T_RPAREN, ")", line);

                case '{':
                    return new Token(TokenType.T_LBRACE, "{", line);

                case '}':
                    return new Token(TokenType.T_RBRACE, "}", line);

                case ';':
                    return new Token(TokenType.T_SEMICOLON, ";", line);

                // Strings
                case '"':
                    return ScanString();

                default:
                    throw new Exception(
                        $"Erro Léxico: caractere inválido '{c}' na linha {line}"
                    );
            }
        }
    }

    // =========================================
    // CONTEXTO JSON (CORREÇÃO DA SERIALIZAÇÃO) 
    // =========================================
    [JsonSerializable(typeof(List<Token>))]
    internal partial class TokenJsonContext : JsonSerializerContext
    {
    }

    // =========================================
    // PROGRAMA PRINCIPAL
    // =========================================
    class Program
    {
        static void Main(string[] args)
        {
            // =========================================
            // CÓDIGO DE TESTE
            // =========================================
            string code = @"

int idade = 20;
float altura = 1.75;

print(""Ola mundo"");

if (idade == 20) {
    print(""Maior de idade"");
}

// comentario ignorado

";

            Scanner scanner = new Scanner(code);

            // Lista de tokens
            List<Token> tokens = new List<Token>();

            try
            {
                Token token = scanner.NextToken();

                while (token.Type != TokenType.T_EOF)
                {
                    // Exibe token
                    Console.WriteLine(
                        $"{token.Type} -> {token.Lexeme} (linha {token.Line})"
                    );

                    // Salva token
                    tokens.Add(token);

                    // Próximo token
                    token = scanner.NextToken();
                }

                Console.WriteLine();
                Console.WriteLine("Fim da análise léxica.");

                // =========================================
                // EXPORTAÇÃO JSON
                // =========================================
                string json = JsonSerializer.Serialize(
    tokens,
    TokenJsonContext.Default.ListToken
);

                json = JsonSerializer.Serialize(
                    tokens,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        TypeInfoResolver = TokenJsonContext.Default
                    }
                );

                // Salva arquivo
                File.WriteAllText("tokens.json", json);

                Console.WriteLine();
                Console.WriteLine("Arquivo tokens.json exportado com sucesso.");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
            }
        }
    }
}