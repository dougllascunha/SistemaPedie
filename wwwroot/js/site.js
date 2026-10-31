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
                    atualizarBadgesCarrinho(dados.totalItens);

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

    function atualizarBadgesCarrinho(totalItens) {
        if (typeof totalItens !== "number") return;
        const contadores = document.querySelectorAll(".contador-carrinho");
        contadores.forEach(badge => {
            badge.textContent = totalItens;
            if (totalItens > 0) {
                badge.classList.remove("d-none");
            } else {
                badge.classList.add("d-none");
            }
        });
    }

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
                if (e.target.id === "cep") {
                    e.preventDefault();
                    buscarCep();
                    const numeroEl = document.getElementById("numero");
                    if (numeroEl) numeroEl.focus();
                    return;
                }

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

    // =========================================================================
    // 3. SINCRONIZAÇÃO AUTOMÁTICA EM TEMPO REAL COM MOBILE / PWA
    // =========================================================================
    const STORAGE_KEY_PEDIDOS = "pedie_status_pedidos_sync";

    function obterClasseBadgeStatus(idSituacao) {
        switch (Number(idSituacao)) {
            case 1: return "bg-warning text-dark";
            case 2: return "bg-info text-dark";
            case 3: return "bg-primary";
            case 4: return "bg-success";
            case 5: return "bg-primary";
            case 6: return "bg-danger";
            default: return "bg-secondary";
        }
    }

    function obterHtmlDetalheStatus(idSituacao) {
        switch (Number(idSituacao)) {
            case 1:
            case 2:
                return '<div class="text-muted mt-1 fw-semibold bi bi-hourglass-split" style="font-size: 0.85rem;"> Previsão: 1h a 3h30 min</div>';
            case 5:
                return '<div class="text-primary mt-1 fw-semibold bi bi-scooter" style="font-size: 0.85rem;"> Seu pedido saiu para entrega!</div>';
            case 3:
                return '<div class="text-primary mt-1 fw-semibold bi bi-shop" style="font-size: 0.85rem;"> Pronto! Disponível para retirada na loja.</div>';
            case 4:
                return '<div class="text-success mt-1 fw-semibold bi bi-check-circle-fill" style="font-size: 0.85rem;"> Pedido entregue</div>';
            case 6:
                return '<div class="text-danger mt-1 fw-semibold bi bi-x-circle-fill" style="font-size: 0.85rem;"> Pedido cancelado</div>';
            default:
                return "";
        }
    }

    function exibirToastSincronizacao(mensagem, exibirBotaoPedido = true) {
        const toastEl = document.getElementById("toastSincronizacao");
        const toastTexto = document.getElementById("toastSincronizacaoTexto");
        const toastLink = document.getElementById("toastSincronizacaoLink");

        if (toastTexto) toastTexto.textContent = mensagem;
        if (toastLink) {
            if (exibirBotaoPedido && !window.location.pathname.toLowerCase().includes("/pedido/meuspedidos")) {
                toastLink.classList.remove("d-none");
            } else {
                toastLink.classList.add("d-none");
            }
        }

        if (toastEl && window.bootstrap) {
            const toast = bootstrap.Toast.getOrCreateInstance(toastEl, { delay: 5000 });
            toast.show();
        }

        // Vibração tátil em dispositivos móveis quando há atualização de status
        if ("vibrate" in navigator) {
            try { navigator.vibrate([180, 80, 180]); } catch (_) { }
        }
    }

    // 3.1. Salvar Status no Painel Admin via AJAX sem recarregar a página
    const formsAdminStatus = document.querySelectorAll(".form-atualizar-status-pedido");
    formsAdminStatus.forEach(form => {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            const btn = form.querySelector(".btn-salvar-status");
            const textoOriginal = btn ? btn.innerHTML : "";
            const formData = new FormData(form);

            if (btn) {
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
            }

            try {
                const resp = await fetch(form.action, {
                    method: "POST",
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                        "Accept": "application/json"
                    },
                    body: formData
                });

                if (resp.ok) {
                    const dados = await resp.json();
                    if (dados.sucesso) {
                        const tr = form.closest("tr");
                        if (tr) {
                            tr.dataset.situacaoId = dados.idSituacao;
                            const badge = tr.querySelector(".badge-status-admin");
                            if (badge) {
                                badge.className = `badge ${obterClasseBadgeStatus(dados.idSituacao)} px-3 py-2 badge-status-admin`;
                                badge.textContent = dados.descricao;
                            }
                        }
                        exibirToastSincronizacao(dados.mensagem, false);
                    }
                } else {
                    form.submit();
                }
            } catch (err) {
                console.error("Erro ao atualizar status via AJAX:", err);
                form.submit();
            } finally {
                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = textoOriginal;
                }
            }
        });
    });

    // 3.2. Sincronização em Tempo Real dos Pedidos do Cliente (Mobile e Web)
    const usuarioLogado = document.body.dataset.usuarioLogado === "true";
    const perfilUsuario = document.body.dataset.perfilUsuario || "";

    function carregarMapaStatusSalvo() {
        try {
            const raw = sessionStorage.getItem(STORAGE_KEY_PEDIDOS);
            return raw ? JSON.parse(raw) : {};
        } catch (_) {
            return {};
        }
    }

    function salvarMapaStatus(mapa) {
        try {
            sessionStorage.setItem(STORAGE_KEY_PEDIDOS, JSON.stringify(mapa));
        } catch (_) { }
    }

    // Inicializa o mapa com os pedidos já renderizados na tela MeusPedidos (se estiver nela)
    const linhasMeusPedidos = document.querySelectorAll("#tabelaMeusPedidosBody tr[data-pedido-id]");
    if (linhasMeusPedidos.length > 0) {
        const mapaInicial = carregarMapaStatusSalvo();
        linhasMeusPedidos.forEach(tr => {
            mapaInicial[tr.dataset.pedidoId] = Number(tr.dataset.situacaoId);
        });
        salvarMapaStatus(mapaInicial);
    }

    async function sincronizarPedidosCliente() {
        if (!usuarioLogado || !navigator.onLine) return;

        const indicador = document.getElementById("indicadorSyncCliente");
        const textoSync = document.getElementById("textoSyncCliente");

        try {
            const response = await fetch("/Pedido/SincronizarMeusPedidos", {
                headers: { "Accept": "application/json" },
                cache: "no-store"
            });

            if (!response.ok) return;
            const dados = await response.json();
            if (!dados.autenticado || !Array.isArray(dados.pedidos)) return;

            const mapaAnterior = carregarMapaStatusSalvo();
            const tinhaEstadoAnterior = Object.keys(mapaAnterior).length > 0;
            const novoMapa = {};

            dados.pedidos.forEach(p => {
                novoMapa[p.idPedido] = Number(p.idSituacao);

                // Verifica se o status mudou em relação ao que o celular conhecia
                if (tinhaEstadoAnterior &&
                    mapaAnterior[p.idPedido] !== undefined &&
                    mapaAnterior[p.idPedido] !== Number(p.idSituacao)) {

                    exibirToastSincronizacao(
                        `Pedido #${p.idPedido} atualizado para: ${p.descricaoSituacao}`,
                        true
                    );
                }

                // Se o cliente estiver com a tela MeusPedidos aberta, atualiza o DOM sem recarregar
                const tr = document.querySelector(`#tabelaMeusPedidosBody tr[data-pedido-id="${p.idPedido}"]`);
                if (tr && Number(tr.dataset.situacaoId) !== Number(p.idSituacao)) {
                    tr.dataset.situacaoId = p.idSituacao;

                    const badge = tr.querySelector(".badge-status-pedido");
                    if (badge) {
                        badge.className = `badge ${obterClasseBadgeStatus(p.idSituacao)} px-3 py-2 badge-status-pedido`;
                        badge.textContent = p.descricaoSituacao;
                    }

                    const detalhe = tr.querySelector(".detalhe-status-pedido");
                    if (detalhe) {
                        detalhe.innerHTML = obterHtmlDetalheStatus(p.idSituacao);
                    }

                    tr.classList.add("table-active");
                    setTimeout(() => tr.classList.remove("table-active"), 2500);
                }
            });

            salvarMapaStatus(novoMapa);

            if (indicador) {
                indicador.className = "d-inline-block rounded-circle bg-success";
            }
            if (textoSync && dados.timestamp) {
                textoSync.textContent = `Sincronizado às ${dados.timestamp}`;
            }
        } catch (err) {
            if (indicador) {
                indicador.className = "d-inline-block rounded-circle bg-warning";
            }
        }
    }

    // 3.3. Sincronização em Tempo Real do Painel Admin (GerenciarPedidos)
    async function sincronizarPainelAdmin() {
        const containerAdmin = document.getElementById("containerGerenciarPedidos");
        if (!containerAdmin || perfilUsuario !== "Admin" || !navigator.onLine) return;

        const indicadorAdmin = document.getElementById("indicadorSyncAdmin");
        const textoAdmin = document.getElementById("textoSyncAdmin");
        const contadorAdmin = document.getElementById("contadorTotalPedidosAdmin");

        try {
            const response = await fetch("/Pedido/SincronizarPainelAdmin", {
                headers: { "Accept": "application/json" },
                cache: "no-store"
            });

            if (!response.ok) return;
            const dados = await response.json();

            if (contadorAdmin && typeof dados.totalPedidos === "number") {
                contadorAdmin.textContent = `${dados.totalPedidos} pedido(s)`;
            }

            if (Array.isArray(dados.pedidos)) {
                const linhasAtuais = document.querySelectorAll("#tabelaGerenciarPedidosBody tr[data-pedido-id]");
                // Se entrou um pedido novo que ainda não está na tabela, recarrega suavemente ou avisa
                if (linhasAtuais.length > 0 && dados.pedidos.length > linhasAtuais.length) {
                    exibirToastSincronizacao("Novo pedido recebido! Atualizando painel...", false);
                    setTimeout(() => window.location.reload(), 1200);
                    return;
                }

                dados.pedidos.forEach(p => {
                    const tr = document.querySelector(`#tabelaGerenciarPedidosBody tr[data-pedido-id="${p.idPedido}"]`);
                    if (tr && Number(tr.dataset.situacaoId) !== Number(p.idSituacao)) {
                        tr.dataset.situacaoId = p.idSituacao;
                        const badge = tr.querySelector(".badge-status-admin");
                        if (badge) {
                            badge.className = `badge ${obterClasseBadgeStatus(p.idSituacao)} px-3 py-2 badge-status-admin`;
                            badge.textContent = p.descricaoSituacao;
                        }
                        const select = tr.querySelector(".select-status-pedido");
                        if (select) {
                            select.value = String(p.idSituacao);
                        }
                    }
                });
            }

            if (indicadorAdmin) indicadorAdmin.className = "d-inline-block rounded-circle bg-success";
            if (textoAdmin && dados.timestamp) textoAdmin.textContent = `Sincronizado às ${dados.timestamp}`;
        } catch (_) {
            if (indicadorAdmin) indicadorAdmin.className = "d-inline-block rounded-circle bg-warning";
        }
    }

    // 3.4. Sincronização do Catálogo de Produtos e Contador do Carrinho no Mobile
    async function sincronizarCatalogoECarrinho() {
        if (!navigator.onLine) return;

        try {
            const response = await fetch("/Home/SincronizarDadosMobile", {
                headers: { "Accept": "application/json" },
                cache: "no-store"
            });

            if (!response.ok) return;
            const dados = await response.json();

            // Sincroniza o contador do carrinho no topo e na barra inferior mobile
            if (typeof dados.totalItensCarrinho === "number") {
                atualizarBadgesCarrinho(dados.totalItensCarrinho);
            }

            // Se estiver na tela de catálogo, sincroniza preços e disponibilidade dos produtos
            const cardsProdutos = document.querySelectorAll(".coluna-produto-catalogo[data-produto-card]");
            if (cardsProdutos.length > 0 && Array.isArray(dados.produtos)) {
                dados.produtos.forEach(prod => {
                    const cardCol = document.querySelector(`.coluna-produto-catalogo[data-produto-card="${prod.idProduto}"]`);
                    if (!cardCol) return;

                    if (!prod.disponivel) {
                        cardCol.classList.add("d-none");
                    } else {
                        cardCol.classList.remove("d-none");
                        const precoEl = cardCol.querySelector(".preco-produto-card");
                        if (precoEl && prod.precoFormatado) {
                            precoEl.textContent = `R$ ${prod.precoFormatado}`;
                        }
                        const nomeEl = cardCol.querySelector(".nomeProduto");
                        if (nomeEl && prod.nome) {
                            nomeEl.textContent = prod.nome;
                        }
                    }
                });
            }
        } catch (_) { }
    }

    // Executa ciclo de sincronização completo
    function executarCicloSincronizacao() {
        if (document.visibilityState === "hidden") return;
        sincronizarPedidosCliente();
        sincronizarPainelAdmin();
        sincronizarCatalogoECarrinho();
    }

    // Dispara a primeira sincronização logo após carregar e mantém ciclo a cada 6 segundos
    executarCicloSincronizacao();
    setInterval(executarCicloSincronizacao, 6000);

    // Sincroniza imediatamente quando o usuário volta para o aplicativo no celular (PWA / aba ativa)
    document.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "visible") {
            executarCicloSincronizacao();
        }
    });

    // 3.5. Monitoramento de Conexão Mobile (Offline / Online) com Ressincronização Automática
    const alertaConexao = document.getElementById("alertaConexaoMobile");

    window.addEventListener("offline", function () {
        if (alertaConexao) {
            alertaConexao.classList.remove("d-none");
            alertaConexao.classList.add("d-flex");
        }
    });

    window.addEventListener("online", function () {
        if (alertaConexao) {
            alertaConexao.classList.add("d-none");
            alertaConexao.classList.remove("d-flex");
        }
        executarCicloSincronizacao();
        exibirToastSincronizacao("Conexão restabelecida! Dados sincronizados.", false);
    });
});
