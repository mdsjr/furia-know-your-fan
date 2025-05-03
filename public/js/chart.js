async function validateHandle(handle) {
    const endpoint = handle.includes('FuriaFan') ? '/api/profile/mock' : '/api/profile/validate';
    const response = await fetch(`${endpoint}/${handle.replace('@', '')}`);
    if (!response.ok) throw new Error('Erro ao validar');
    return response.json();
}