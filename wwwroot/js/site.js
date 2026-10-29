document.addEventListener("DOMContentLoaded", function () {

    // =========================================================================
    // 1. ADICIONAR AO CARRINHO VIA AJAX (SEM RECARREGAR A PÁGINA NEM VOLTAR AO TOPO)
    // =========================================================================
    const formsAdicionar = document.querySelectorAll(".form-adicionar-carrinho");

    formsAdicionar.forEach(form => {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            const botao = form.querySelector(".btn-adicionar");
            const textoOriginal = botao ? botao.innerHTML : "";
            const formData = new FormData(form);

            if (botao) {
                botao.disabled = true;
                botao.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Adicionando...';
            }

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                        "Accept": "application/json"
                    },
                    body: formData
                });

                if (response.ok) {
                    const dados = await response.json();

                    // Atualiza todos os badges de contador do carrinho (Desktop e Mobile)
                    const contadores = document.querySelectorAll(".contador-carrinho");
                    contadores.forEach(badge => {
                        badge.textContent = dados.totalItens;
                        if (dados.totalItens > 0) {
                            badge.classList.remove("d-none");
                        }
                    });

                    // Exibe o Toast de confirmação no canto da tela
                    const toastEl = document.getElementById("toastCarrinho");
                    const toastTexto = document.getElementById("toastCarrinhoTexto");
                    if (toastTexto && dados.mensagem) {
                        toastTexto.textContent = dados.mensagem;
                    }
                    if (toastEl && window.bootstrap) {
                        const toast = bootstrap.Toast.getOrCreateInstance(toastEl, { delay: 3500 });
                        toast.show();
                    }

                    // Feedback visual rápido no próprio botão
                    if (botao) {
                        botao.classList.remove("btn-primary");
                        botao.classList.add("btn-success");
                        botao.innerHTML = '<i class="bi bi-check-lg me-1"></i> Adicionado!';
                        setTimeout(() => {
                            botao.classList.remove("btn-success");
                            botao.classList.add("btn-primary");
                            botao.innerHTML = textoOriginal;
                            botao.disabled = false;
                        }, 1200);
                    }
                } else {
                    form.submit(); // Fallback caso ocorra algum erro inesperado
                }
            } catch (erro) {
                console.error("Erro ao adicionar item ao carrinho:", erro);
                form.submit();
            }
        });
    });

    // =========================================================================
    // 2. TELA DE CHECKOUT (ChecarPedido): ENTREGA VS RETIRADA, VIACEP E VALIDAÇÃO DO ENTER
    // =========================================================================
    const formChecarPedido = document.getElementById("formChecarPedido");
    const opcaoEntrega = document.getElementById("opcaoEntrega");
    const opcaoRetirada = document.getElementById("opcaoRetirada");
    const dadosEndereco = document.getElementById("dadosEndereco");
    const camposObrigatorios = document.querySelectorAll(".campo-endereco-obrigatorio");
    const resumoCard = document.getElementById("resumoCompraCard");
    const valorFreteTexto = document.getElementById("valorFreteTexto");
    const valorTotalFinalTexto = document.getElementById("valorTotalFinalTexto");
    const alertaEndereco = document.getElementById("alertaEnderecoIncompleto");

    function formatarMoeda(valor) {
        return valor.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function atualizarModoEntrega() {
        if (!opcaoEntrega || !opcaoRetirada || !dadosEndereco) return;

        const ehEntrega = opcaoEntrega.checked;
        dadosEndereco.style.display = ehEntrega ? "block" : "none";

        camposObrigatorios.forEach(campo => {
            if (ehEntrega) {
                campo.setAttribute("required", "required");
            } else {
                campo.removeAttribute("required");
                campo.classList.remove("is-invalid");
            }
        });

        if (!ehEntrega && alertaEndereco) {
            alertaEndereco.classList.add("d-none");
        }

        if (resumoCard && valorFreteTexto && valorTotalFinalTexto) {
            const subtotal = parseFloat(resumoCard.dataset.subtotal || "0");
            const fretePadrao = parseFloat(resumoCard.dataset.frete || "10");
            const freteAtual = ehEntrega ? fretePadrao : 0;
            const totalFinal = subtotal + freteAtual;

            valorFreteTexto.textContent = ehEntrega ? `R$ ${formatarMoeda(freteAtual)}` : "Grátis (R$ 0,00)";
            valorTotalFinalTexto.textContent = `R$ ${formatarMoeda(totalFinal)}`;
        }
    }

    if (opcaoEntrega && opcaoRetirada && dadosEndereco) {
        opcaoEntrega.addEventListener("change", atualizarModoEntrega);
        opcaoRetirada.addEventListener("change", atualizarModoEntrega);
        atualizarModoEntrega();
    }

    // Busca automática de CEP via ViaCEP
    const cepInput = document.getElementById("cep");

    async function buscarCep() {
        if (!cepInput) return;
        let cep = cepInput.value.replace(/\D/g, "");
        if (cep.length !== 8) return;

        try {
            const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
            const dados = await response.json();

            if (!dados.erro) {
                const ruaEl = document.getElementById("rua");
                const bairroEl = document.getElementById("bairro");
                const cidadeEl = document.getElementById("cidade");
                const estadoEl = document.getElementById("estado");
                const numeroEl = document.getElementById("numero");

                if (ruaEl && dados.logradouro) {
                    ruaEl.value = dados.logradouro;
                    ruaEl.classList.remove("is-invalid");
                }
                if (bairroEl && dados.bairro) {
                    bairroEl.value = dados.bairro;
                    bairroEl.classList.remove("is-invalid");
                }
                if (cidadeEl && dados.localidade) {
                    cidadeEl.value = dados.localidade;
                    cidadeEl.classList.remove("is-invalid");
                }
                if (estadoEl && dados.uf) {
                    estadoEl.value = dados.uf;
                    estadoEl.classList.remove("is-invalid");
                }
                if (numeroEl && !numeroEl.value.trim()) {
                    numeroEl.focus();
                }
            }
        } catch (err) {
            console.error("Erro ao consultar ViaCEP:", err);
        }
    }

    if (cepInput) {
        cepInput.addEventListener("change", buscarCep);
        cepInput.addEventListener("blur", buscarCep);
    }

    // Validação do formulário ChecarPedido e bloqueio de Enter prematuro
    if (formChecarPedido) {
        const listaCamposObrigatorios = Array.from(camposObrigatorios);

        listaCamposObrigatorios.forEach(campo => {
            campo.addEventListener("input", function () {
                if (this.value.trim()) {
                    this.classList.remove("is-invalid");
                }
            });
        });

        formChecarPedido.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && e.target.tagName === "INPUT") {
                // Se o usuário apertar Enter no campo de CEP, consulta o CEP e foca no número sem submeter o form
                if (e.target.id === "cep") {
                    e.preventDefault();
                    buscarCep();
                    const numeroEl = document.getElementById("numero");
                    if (numeroEl) numeroEl.focus();
                    return;
                }

                // Se for Entrega em domicílio e ainda houver campos obrigatórios vazios,
                // impede que o Enter finalize a compra sem preencher tudo
                if (opcaoEntrega && opcaoEntrega.checked) {
                    const primeiroVazio = listaCamposObrigatorios.find(c => !c.value.trim());
                    if (primeiroVazio) {
                        e.preventDefault();
                        if (alertaEndereco) alertaEndereco.classList.remove("d-none");
                        primeiroVazio.classList.add("is-invalid");
                        primeiroVazio.focus();
                    }
                }
            }
        });

        formChecarPedido.addEventListener("submit", function (e) {
            if (opcaoEntrega && opcaoEntrega.checked) {
                let todosPreenchidos = true;
                let primeiroVazio = null;

                listaCamposObrigatorios.forEach(campo => {
                    if (!campo.value.trim()) {
                        todosPreenchidos = false;
                        campo.classList.add("is-invalid");
                        if (!primeiroVazio) primeiroVazio = campo;
                    } else {
                        campo.classList.remove("is-invalid");
                    }
                });

                if (!todosPreenchidos) {
                    e.preventDefault();
                    if (alertaEndereco) alertaEndereco.classList.remove("d-none");
                    if (primeiroVazio) primeiroVazio.focus();
                }
            }
        });
    }
});
