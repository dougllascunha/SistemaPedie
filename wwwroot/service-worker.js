const CACHE_NAME = 'pedie-cache-v1';
const urlsToCache = [
    '/',
    '/css/site.css',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/imagens/LogoPedie.png'
];


self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
    );
});


self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                
                return response || fetch(event.request);
            })
    );
});
