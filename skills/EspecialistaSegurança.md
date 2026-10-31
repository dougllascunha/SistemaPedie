Especialista em Segurança, Autenticação e Negócios — Pediê

Objetivo e limites

Proteger identidades, permissões e dados mantendo uma implementação acadêmica compreensível. Priorizar 3.1.1.1 — Gerenciar Acesso e 3.1.1.6 — Pagamento On-line. Distinguir demonstração local de funcionalidade adequada a um ambiente real.

Não prometer um sistema “blindado”. Descrever proteção implementada, evidência observada e limitações restantes. Não executar testes intrusivos contra serviços externos ou ambientes fora do escopo autorizado.

Inspecionar antes de editar

Ler as instruções locais, versão do .NET, configuração do pipeline, autenticação, cookies, sessão e autorização.

Ler entidades de usuário, ViewModels, controllers, serviços de autenticação e pagamento, views e esquema relacionado.

Identificar se existe ASP.NET Core Identity ou autenticação por cookies. Reutilizar mecanismos do framework; não criar criptografia ou protocolo de autenticação próprios.

Localizar critérios do backlog. Sinalizar requisitos que dependam de regra de negócio ausente e continuar nas partes independentes.

Usar dados fictícios em testes. Não exibir segredos, tokens, senhas, telefones ou dados de clientes nos relatórios.

Autenticação e ciclo da sessão

Armazenar senhas usando o mecanismo de hashing do Identity ou PasswordHasher<TUser> compatível com o projeto. Não usar texto puro, criptografia reversível ou hash simples sem política adequada.

Validar credenciais no servidor. Produzir identidade autenticada usando o mecanismo de cookies/Identity e conferir a ordem dos middlewares.

Aplicar cookies HttpOnly, transporte seguro em HTTPS e política SameSite compatível com os fluxos reais. Definir expiração e impedir redirecionamento para URL externa não validada.

Reduzir enumeração de contas com mensagens genéricas e limitar tentativas de login e recuperação usando recursos compatíveis com a versão e a implantação.

No login ou troca de conta, não reaproveitar carrinho nem estado de checkout de outro usuário; adotar uma política explícita para carrinho anônimo, se o projeto permitir.

No logout por POST protegido contra CSRF, executar o sign-out do esquema real e limpar dados transitórios da conta na sessão. Session.Clear() isolado não invalida o cookie de autenticação.

Ao concluir um pedido, remover apenas os dados de carrinho/checkout concluídos. Não chamar Session.Clear() indiscriminadamente nem encerrar a autenticação.

Autorização no servidor

Proteger actions de produtos, promoções e alteração de pedidos com [Authorize(Roles = "Admin")] ou política equivalente, usando o nome de perfil efetivamente cadastrado.

Derivar identidade e privilégios de fonte confiável no servidor. Não aceitar Perfil, UsuarioId, proprietário ou privilégio administrativo enviados pelo formulário.

Não usar apenas uma string Admin na Session como mecanismo de autorização. Se houver legado assim, identificar a lacuna e preparar uma adaptação pequena para autenticação e autorização do framework.

Conferir propriedade de cada pedido consultado ou alterado, inclusive por URL direta ou requisição JavaScript. Cliente autenticado não ganha acesso aos dados de outros clientes.

Manter ações públicas explicitamente delimitadas e testar acesso anônimo, cliente, administrador e tentativa de acesso a recurso de outra conta.

Recuperação de senha sem SMTP externo

Não redefinir senha usando apenas correspondência de e-mail + telefone. Esses dados cadastrais não demonstram controle da conta.

Implementar o fluxo mínimo:

Receber a solicitação e responder de forma genérica, exista ou não a conta. Limitar tentativas sem bloquear arbitrariamente contas de terceiros.

Gerar token imprevisível, vinculado ao usuário e à finalidade, com validade curta e uso único, preferencialmente pelo provedor do Identity já configurado. Não devolver o token na resposta pública de solicitação.

Para demonstração local, entregar a mensagem com o link a um capturador de e-mail local controlado pelo desenvolvedor. Usar dados fictícios; restringir esse mecanismo ao ambiente Development, sem endpoint público de leitura e sem registrar o token em logs comuns.

Fora desse ambiente, exigir canal de entrega verificado. Na ausência dele, desabilitar a redefinição automática e informar a dependência, sem abrir um atalho por dados cadastrais.

Validar o token no servidor, aplicar a política de senha e invalidá-lo após sucesso. Se houver armazenamento próprio de tokens aleatórios, guardar seu hash e consumir atomicamente, evitando reutilização concorrente.

Invalidar credenciais de recuperação anteriores e sessões conforme o mecanismo do projeto; no Identity, integrar a atualização de security stamp e sua validação. Não afirmar revogação imediata sem verificar o comportamento configurado.

Exigir novo login após a redefinição. Não autenticar automaticamente o solicitante.

Se o backlog exigir e-mail e telefone, tratá-los apenas como conferência adicional. Explicar o conflito caso o requisito proíba uma prova efetiva de controle da conta.

Pagamento PIX simulado

Mostrar “Simulação acadêmica — nenhum pagamento real será realizado” antes da confirmação. Usar QR Code demonstrativo que não gere cobrança ou transferência real.

Criar o pedido e os itens no servidor com estado pendente antes de iniciar a simulação. Usar os nomes reais do esquema; não inventar colunas silenciosamente.

Registrar no servidor a tentativa, vínculo com o pedido e usuário, horário de início e estado. Fixar a duração demonstrativa de 60 segundos no servidor.

Usar JavaScript apenas para exibir a contagem e consultar o estado. Não aceitar pago=true, total, horário ou sucesso enviados pelo cliente como prova de confirmação.

Após o prazo, permitir que o simulador no servidor faça a transição por POST autenticado e protegido contra CSRF, respeitando propriedade, estado atual, valor calculado e validações do pedido. Reservar GET para consultar estado, sem mutação. Não introduzir scheduler só para a demonstração.

Tornar a confirmação idempotente e segura contra concorrência: várias abas e requisições repetidas não podem duplicar pagamento, pedido ou baixa de estoque. Aplicar a garantia no banco, não só no botão ou na sessão.

Rejeitar confirmação antecipada ou de tentativa cancelada/expirada. Preservar o pedido pendente em erro e permitir recuperar seu estado após recarregar a página.

Manter pedido, registro da simulação e estoque consistentes na unidade transacional necessária. Coordenar o momento da reserva/baixa com as regras do back-end, sem contabilizar duas vezes.

Manter o simulador restrito ao ambiente de demonstração configurado; não habilitá-lo como gateway real em produção. Uma integração real deverá validar confirmações do provedor no servidor e está fora desse mock.

Proteções transversais

Aplicar antiforgery às mutações autenticadas por cookie, inclusive logout e confirmação; não alterar estado via GET.

Usar ViewModels e mapeamento explícito para impedir atribuição de privilégios ou estados internos por campos extras.

Preservar escape de saída e consultas parametrizadas. Tratar uploads apenas se fizerem parte da tarefa, validando tipo, tamanho e local de armazenamento.

Guardar segredos em configuração segura e registrar eventos úteis sem dados sensíveis. Não expor stack trace, SQL ou tokens em erros ao cliente.

Verificar e entregar

Testar as fronteiras alteradas: rota administrativa direta, pedido de outra conta, logout, token inválido/expirado/reutilizado, tentativas excessivas e confirmação antecipada/repetida de pagamento. Usar ambiente de teste autorizado.

Entregar comportamento implementado, arquivos alterados, verificações executadas, achados remanescentes e dependências. Para cada achado, indicar evidência, impacto e correção mínima. Não declarar ausência de vulnerabilidades sem evidência suficiente.

Referências técnicas

Autorização por perfil no ASP.NET Core MVC.

OWASP — recuperação de senha.
