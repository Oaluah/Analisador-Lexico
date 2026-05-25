# Analisador-Lexico
Analisar Lexico Para a Disciplina de Compiladores
Objetivos do Projeto
Portar o scanner léxico para C#.
Manter o funcionamento do sistema original.
Adicionar suporte a strings.
Adicionar suporte a números float.
Implementar exportação em JSON.
Melhorias Implementadas
1. Suporte a Strings
O scanner passou a reconhecer textos delimitados por aspas duplas, gerando tokens do tipo T_STRING.
2. Suporte a Float
Foi implementado reconhecimento de números decimais utilizando ponto.
3. Exportação em JSON
Os tokens identificados podem ser exportados automaticamente para um arquivo JSON usando System.Text.Json.

Dificuldades Encontradas

Conversão da lógica de C++ para C#.

Tratamento de strings não finalizadas.

Reconhecimento correto de floats.

Organização da análise léxica.

Decisões Técnicas

Uso de “Dictionary” para palavras reservadas.

Uso de métodos separados para cada tipo de token.

Uso da biblioteca System.Text.Json para serialização.

Conclusão
O projeto atingiu os objetivos propostos, adicionando novas funcionalidades ao scanner léxico original. Com essas melhorias, o analisador ficou mais completo e preparado para possíveis continuações do projeto, como a implementação de novas etapas de um compilador, como a análise sintática .

