self.addEventListener("install", () => self.skipWaiting());

self.addEventListener("activate", event => {
    event.waitUntil(self.clients.claim());
});
//== É UM WORKER QUE NÃO ATIVA O MODO OFFILINE NEM GUARDA DADOS==//TESTE PWA
//Para testar o PWA primeiro precisa criar um certificado local e confiar nele com esse comando no terminal do projeto 
//dotnet dev-certs https --trust
//depois pode rodar esse de baixo que ele vai abrir normalmente 
//dotnet run --launch-profile https