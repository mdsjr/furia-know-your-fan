FURIA Know Your Fan
Descrição
FURIA Know Your Fan é uma aplicação web desenvolvida para o Challenge #2 - Know Your Fan da Trybe. A solução coleta e analisa dados de fãs de esports, com foco na organização FURIA, permitindo:

Coleta de dados pessoais (nome, endereço, CPF, interesses, eventos, compras).
Upload e validação de documentos (PNG, JPEG, PDF) com simulação de IA.
Análise de interações no X com #FURIACS, incluindo top palavras, sentimento e posts por dia.
Validação de perfis do X com simulação de IA.

Funcionalidades

Análise de Tweets: Exibe gráficos de top palavras, posts por dia, sentimento e tweets associados.
Perfil do Usuário: Formulário para coleta de dados pessoais e upload de documentos.
Validação de Perfil: Verifica interações do usuário no X com a FURIA.
Exportação de Dados: Baixa a análise de tweets como JSON.
Frontend: Interface com logotipo da FURIA, gráficos interativos e design responsivo.
Backend: API RESTful com cache de 5 minutos.

Tecnologias

Backend: ASP.NET Core, C#
Frontend: React, Chart.js, Tailwind CSS
API Externa: X API v2
Outros: Git, JSON

Configuração

Pré-requisitos:

.NET 8 SDK
Chave de API do X (Bearer Token)
Navegador moderno (Chrome, Firefox)

Instalação:
git clone https://github.com/seu_usuario/furia-know-your-fan.git
cd furia-know-your-fan

Configurar API do X:

Crie um arquivo appsettings.json em D:\FuriaKnowYourFan:{
"XApiBearerToken": "SUA_CHAVE_AQUI"
}

Substitua SUA_CHAVE_AQUI pela sua chave de API do X.

Executar:
dotnet run

Acesse http://localhost:5001 no navegador.

Uso

Análise de Tweets:

Na página inicial (/index.html), clique em "Atualizar Dados" para carregar a análise de tweets.
Veja os gráficos, sentimento e tweets associados.
Clique em "Exportar Dados" para baixar o JSON.

Editar Perfil:

Clique em "Editar Perfil" para acessar /profile.html.
Preencha os dados pessoais, faça upload de um documento e insira seu handle do X.
Clique em "Validar" para verificar o perfil e "Salvar Perfil" para simular o salvamento.

Estrutura do Projeto
FuriaKnowYourFan/
├── Controllers/
│ ├── FanController.cs
│ ├── ProfileController.cs
├── Services/
│ ├── FanAnalysisService.cs
│ ├── XApiService.cs
├── wwwroot/
│ ├── images/
│ │ ├── furia-logo.png
│ ├── index.html
│ ├── profile.html
├── Properties/
│ ├── launchSettings.json
├── appsettings.json
├── README.md

Vídeo de Demonstração

Link para o vídeo de 3 minutos (substituir pelo link após gravação)

Licença
MIT License
Contato

Moacir
Email: seu_email@example.com
GitHub: github.com/seu_usuario
