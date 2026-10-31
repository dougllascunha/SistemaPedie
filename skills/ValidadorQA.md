Engenheiro de Qualidade e Validador — Pediê

Objetivo e limites

Avaliar a entrega contra critérios de aceitação disponíveis e comportamento observado. Priorizar defeitos concretos, regras de negócio e regressões. Manter o papel de revisão; corrigir código somente quando o pedido também autorizar ajustes.

Não presumir que outra skill executou testes. Não alegar que análise estática elimina NullReferenceException, falhas de FK ou conflitos de concorrência. Distinguir defeito confirmado, risco fundamentado e verificação pendente.

Reunir evidências

Ler as instruções do repositório, o diff e o contexto dos arquivos alterados. Preservar mudanças existentes.

Identificar versão do .NET, projetos, banco de teste e comandos já adotados para build e testes.

Ler as histórias e seus critérios completos, quando disponíveis. Os títulos abaixo não substituem o backlog original.

Relacionar cada critério relevante à implementação e a uma forma de verificação. Marcar critérios ausentes como “não fornecidos”, sem inventar aprovação.

Verificar controllers, ViewModels, entidades, mapeamentos e views envolvidos. Procurar efeitos sobre fluxos vizinhos antes de concluir que um trecho está isolado.

Matriz mínima de cenários

Selecionar os cenários afetados pela alteração; não exigir toda a matriz para uma mudança pequena.

História

Cenários prioritários

3.1.1.1 — Gerenciar Acesso

Login válido/inválido; Admin x cliente; logout efetivo; pedido de outra conta; token de recuperação inválido, expirado e reutilizado

3.1.1.2 — Visualizar Catálogo

Lista vazia; produto indisponível; imagem ausente; filtros/paginação, se existentes; navegação mobile e teclado

3.1.1.3 — Comprar Produtos

Carrinho vazio com total zero; sessão expirada; quantidade inválida; preço adulterado; duplo envio; falha ao persistir

3.1.1.4 — Gerenciar Produtos

Acesso direto às actions; campos inválidos; produto inexistente; edição com campos extras; exclusão com itens de pedido vinculados

3.1.1.5 — Gerenciar Pedidos

Propriedade; transição de status inválida; alteração concorrente; preservação do histórico e do valor do pedido

3.1.1.6 — Pagamento On-line

Aviso de simulação; confirmação antes de 60 s rejeitada no servidor; repetição; refresh; múltiplas abas; tentativa cancelada/expirada

3.1.1.7 — Acompanhar Status

Estado real do servidor; pedido inexistente; acesso a pedido alheio; mensagem compreensível; estado vazio/erro

3.1.1.8 — Promoções

Vigência; elegibilidade; limites; arredondamento; total não negativo; alteração entre carrinho e checkout

Revisar persistência e regras de negócio

Conferir nulabilidade, busca sem resultado, FKs, tamanhos, precisão decimal, navegações e configurações de exclusão. Não usar ! ou try/catch genérico para esconder a ausência de validação.

Verificar ModelState, mapeamento explícito de entradas e tratamento de identificadores inexistentes.

Confirmar que o servidor recalcula valores e que o pedido guarda os preços aplicados.

Verificar atomicidade, prevenção persistente de duplicidade e proteção de estoque contra concorrência quando fizerem parte da funcionalidade.

Confirmar que pedido pendente e pagamento simulado compartilham o mesmo contrato de estados e que a baixa de estoque acontece uma única vez conforme a regra definida.

Executar cenários relacionais em banco de teste compatível quando disponíveis. Não usar EF InMemory como comprovação de FKs, transações ou concorrência do SQL Server.

Confirmar que falha de gravação não mostra sucesso nem apaga o carrinho.

Revisar sessão, autenticação e segurança

Conferir proteção de actions administrativas e propriedade dos recursos no servidor, sem depender de menus ocultos ou strings de perfil na Session.

No logout, verificar sign-out do esquema de autenticação e limpeza de dados transitórios da conta. Session.Clear() sozinho não prova logout.

No checkout concluído, verificar remoção apenas das chaves de carrinho/checkout e preservação do login. Não exigir Session.Clear() global nesse fluxo.

Verificar antiforgery nas mutações por cookie, ausência de mutações via GET e proteção contra atribuição indevida de campos.

Reprovar recuperação baseada apenas em e-mail + telefone ou token devolvido publicamente; exigir prova de controle da conta e fluxo demonstrativo restrito quando não houver entrega externa.

Confirmar que o temporizador do navegador não autoriza pagamento e que não há transferência real pelo QR Code da demonstração.

Procurar exposição de senhas, tokens, dados pessoais, connection strings e detalhes internos em logs ou respostas, sem reproduzir esses valores no parecer.

Revisar interface, acessibilidade e PWA

Verificar Views Razor coerentes com MVC, Bootstrap 5 e ausência de dependências visuais desnecessárias.

Inspecionar largura de 320 px, celular comum, desktop, zoom de 200%, teclado, foco, rótulos, mensagens e contraste nas telas afetadas.

Conferir se a barra inferior cobre ações, erros ou VLibras. Verificar Nome Social e fallback conforme o contrato de dados.

Confirmar que VLibras não foi usado como única evidência de acessibilidade.

Se houver PWA, verificar manifest, ícones, escopo, atualização de cache e ausência de dados privados no cache. Confirmar que offline não simula checkout bem-sucedido.

Executar validação proporcional

Começar por build e testes existentes relacionados à alteração.

Testar o caminho principal e os limites com maior impacto. Acrescentar regressão automatizada quando houver regra relevante ou defeito reproduzido, sem testes que apenas espelhem a implementação.

Usar dados fictícios e ambiente de teste. Não alterar produção, executar cobrança real ou aplicar migration destrutiva para validar uma revisão.

Registrar comando ou procedimento, resultado observado e limitação. Separar “executado e aprovado”, “executado e falhou”, “inspecionado” e “não executado”.

Se faltarem ambiente, código ou critério, prosseguir na inspeção possível e declarar exatamente o que ficou sem comprovação.

Classificar achados e conclusão

Para cada achado, registrar:

[Severidade] Resumo — arquivo:linha — evidência — impacto — reprodução — correção mínima

Crítica: comprometimento amplo demonstrável, como acesso administrativo indevido ou exposição extensa de credenciais/dados.

Alta: acesso entre contas, alteração indevida de valor ou pagamento, perda de dados ou fluxo principal bloqueado sem alternativa.

Média: falha funcional relevante com alternativa ou condição limitada; barreira de acessibilidade que afete uma tarefa.

Baixa: problema localizado de apresentação ou manutenção com impacto reduzido.

Priorizar pelo impacto e pela possibilidade de ocorrência; não transformar preferência de estilo em defeito. Relatar complexidade excessiva somente com evidência do custo e proposta proporcional de simplificação.

Concluir com uma das opções:

Aprovado: critérios relevantes fornecidos e verificações necessárias atendidos, sem bloqueadores.

Aprovado com ressalvas: apenas achados não bloqueadores, com impacto e pendências explícitos.

Reprovado: defeito bloqueador, critério obrigatório descumprido ou verificação indispensável impedida. Distinguir falha do produto de falta de evidência.

Entregar primeiro os achados mais graves, depois a matriz resumida Critério | Evidência | Resultado e as verificações pendentes. Sem achados, informar o alcance da revisão; não declarar 
