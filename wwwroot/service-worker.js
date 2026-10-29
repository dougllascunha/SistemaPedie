const CACHE_NAME = 'pedie-cache-v2';
const urlsToCache = [
    '/css/site.css',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/imagens/LogoPedie.png'
];

self.addEventListener('install', event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(urlsToCache))
    );
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames
                    .filter(name => name !== CACHE_NAME)
                    .map(name => caches.delete(name))
            );
        }).then(() => self.clients.claim())
    );
});

self.addEventListener('fetch', event => {
    // Nunca faz cache de requisições POST ou de páginas HTML dinâmicas (navegação/sessão/login/admin)
    if (event.request.method !== 'GET') {
        return;
    }

    const acceptHeader = event.request.headers.get('accept') || '';
    if (event.request.mode === 'navigate' || acceptHeader.includes('text/html')) {
        event.respondWith(
            fetch(event.request).catch(() => caches.match(event.request))
        );
        return;
    }

    // Para arquivos estáticos (imagens, CSS, JS), busca na rede ou no cache
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
