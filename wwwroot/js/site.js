document.addEventListener("DOMContentLoaded", function(){

    
    const opcaoEntrega = document.getElementById("opcaoEntrega");
    const opcaoRetirada = document.getElementById("opcaoRetirada");
    const dadosEndereco = document.getElementById("dadosEndereco");

    if (!opcaoEntrega || !opcaoRetirada || !dadosEndereco)
        {
        console.error("Não encontrei os elementos de entrega no HTML");
        return;
        }
        
        function atualizarEndereco() {
            dadosEndereco.style.display = 
            opcaoEntrega.checked ? "block" : "none";
        }
        
        opcaoEntrega.addEventListener("change", atualizarEndereco);
        opcaoRetirada.addEventListener("change", atualizarEndereco);
        
        atualizarEndereco();
        
    }
)
document.addEventListener("DOMContentLoaded", function () {

    const cepInput = document.getElementById("cep");

    cepInput.addEventListener("change", function () {

        let cep = cepInput.value.replace(/\D/g, "");

        if (cep.length !== 8) {
            return;
        }

        fetch(`https://viacep.com.br/ws/${cep}/json/`)
            .then(response => response.json())
            .then(dados => {

                document.getElementById("rua").value =
                    dados.logradouro || "";

                document.getElementById("bairro").value =
                    dados.bairro || "";

                document.getElementById("cidade").value =
                    dados.localidade || "";

                document.getElementById("estado").value =
                    dados.uf || "";
            });
    });

});