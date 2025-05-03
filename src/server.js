require('dotenv').config();
const express = require('express');
const axios = require('axios');
const OpenAI = require('openai');
const cors = require('cors');
const fs = require('fs').promises;
const path = require('path');
const app = express();

// Configuração de variáveis de ambiente
const PORT = process.env.PORT || 5001;
const X_API_KEY = process.env.X_API_KEY;
const BEARER_TOKEN = process.env.BEARER_TOKEN;
const OPENAI_API_KEY = process.env.API_KEY;

// Verificar variáveis de ambiente
if (!X_API_KEY || !BEARER_TOKEN) {
    console.error('Erro: X_API_KEY ou BEARER_TOKEN não configurados.');
    process.exit(1);
}

// Inicializar OpenAI (se API_KEY estiver presente)
const openai = OPENAI_API_KEY ? new OpenAI({
    apiKey: OPENAI_API_KEY,
    timeout: 10000
}) : null;

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.static('public'));

// Caminho para cache
const CACHE_DIR = path.join(__dirname, 'cache');
const CACHE_FILE = path.join(CACHE_DIR, 'profiles.json');

// Inicializar cache
async function initCache() {
    try {
        await fs.mkdir(CACHE_DIR, { recursive: true });
        try {
            await fs.access(CACHE_FILE);
        } catch {
            await fs.writeFile(CACHE_FILE, JSON.stringify({}));
        }
    } catch (error) {
        console.error('Erro ao inicializar cache:', error);
    }
}
initCache();

// Endpoint: Validar perfil com cache
app.get('/api/profile/validate/:handle', async (req, res) => {
    const handle = req.params.handle.replace('@', '');
    try {
        // Verificar cache
        const cacheData = JSON.parse(await fs.readFile(CACHE_FILE, 'utf-8'));
        if (cacheData[handle]) {
            console.log(`Cache hit para ${handle}`);
            return res.json(cacheData[handle]);
        }

        // Consultar API do X
        const profileResponse = await axios.get(`https://api.x.com/2/users/by/username/${handle}`, {
            headers: { Authorization: `Bearer ${BEARER_TOKEN}` }
        });
        const userId = profileResponse.data.data.id;
        const tweetsResponse = await axios.get(`https://api.x.com/2/users/${userId}/tweets`, {
            headers: { Authorization: `Bearer ${BEARER_TOKEN}` },
            params: { max_results: 10 }
        });

        const result = {
            data: profileResponse.data.data,
            tweets: tweetsResponse.data.data || []
        };

        // Salvar no cache
        cacheData[handle] = result;
        await fs.writeFile(CACHE_FILE, JSON.stringify(cacheData, null, 2));
        console.log(`Cache miss para ${handle}, dados salvos`);

        res.json(result);
    } catch (error) {
        console.error('Erro ao validar perfil:', {
            message: error.message,
            status: error.response?.status
        });
        res.status(500).json({ error: 'Erro ao validar perfil. Tente novamente!' });
    }
});

// Endpoint: Mock de perfil com OpenAI
app.get('/api/profile/mock/:handle', async (req, res) => {
    const handle = req.params.handle.replace('@', '');
    if (!openai) {
        return res.status(503).json({ error: 'OpenAI não configurado.' });
    }

    try {
        const prompt = `
Gere um JSON simulando uma resposta da API do X para um perfil de fã da FURIA.
- Handle: "${handle}"
- Estrutura: { "data": { "id": string, "name": string, "username": string }, "tweets": [{ "id": string, "text": string, "created_at": string }] }
- Inclua 3 tweets fictícios com tom de torcida (ex.: "VAMOS FURIA! 🖤🧡").
- Datas em formato ISO (ex.: "2025-04-30T12:00:00Z"), entre 27/04/2025 e 30/04/2025.
Exemplo:
{
  "data": { "id": "mock123", "name": "Furia Fan", "username": "${handle}" },
  "tweets": [{ "id": "1", "text": "FURIA é foda!", "created_at": "2025-04-29T10:00:00Z" }, ...]
}
`;

        const completion = await openai.chat.completions.create({
            model: 'gpt-4o-mini',
            messages: [{ role: 'user', content: prompt }],
            max_tokens: 300,
            temperature: 0.7
        });

        const mockData = JSON.parse(completion.choices[0].message.content.trim());
        res.json(mockData);
    } catch (error) {
        console.error('Erro ao gerar mock:', error);
        res.status(500).json({ error: 'Erro ao gerar perfil mockado. Tente novamente!' });
    }
});

// Iniciar servidor
app.listen(PORT, () => {
    console.log(`Servidor rodando em http://localhost:${PORT}`);
});