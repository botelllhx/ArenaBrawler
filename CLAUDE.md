# CLAUDE.md

Memória de projeto. Mantenha enxuto. Detalhe fica nos docs apontados aqui.

## O que é

Jogo de arena PvP top-down em tempo real estilo Brawl Stars, feito em Unity.
Objetivo: rodar localmente e jogar com amigos via sala por código. Não há deploy
nem servidor dedicado. Um jogador hospeda (Host Mode do Fusion) e o relay do
Photon Cloud conecta o resto.

## Stack

- Unity 6 (URP)
- Photon Fusion 2 (netcode, Host Mode autoritativo, relay Photon Cloud)
- Unity Input System (controle twin-stick)
- C# com Assembly Definitions por módulo
- Unity MCP (`com.unity.ai.assistant`) conectando o Claude Code ao Editor

## Regras de ouro (não negociáveis)

1. **Orientado a dados.** Brawler, ataque e modo são ScriptableObjects. NUNCA
   crie uma classe por brawler. Há uma classe genérica que lê os dados. Ver
   `docs/ARCHITECTURE.md`.
2. **Servidor é a verdade.** Toda mudança de estado de jogo (vida, dano, posse de
   gema, score) só acontece no State Authority. Cliente sugere input, nunca
   decide resultado. Ver `docs/NETWORKING.md`.
3. **Simulação no tick, render no frame.** Lógica de jogo roda em
   `FixedUpdateNetwork()`, nunca em `Update()`. Visual e interpolação em
   `Render()`. Input lido via `GetInput<NetworkInputData>()`, nunca `Input.Get*`
   dentro da simulação.
4. **A API do Fusion 2 é diferente do Fusion 1.** Seu conhecimento de treino
   provavelmente está desatualizado. Antes de escrever qualquer coisa de rede,
   confira a doc oficial: https://doc.photonengine.com/fusion/current/ . Não
   invente nomes de API.
5. **Uma fase por vez.** Siga `docs/ROADMAP.md` em ordem. Não comece a fase N+1
   antes da fase N passar no Definition of Done dela.

## Estrutura

```
Assets/_Project/Scripts/  -> código (Core, Networking, Gameplay, UI)
Assets/_Project/Data/     -> ScriptableObjects (Brawlers, Attacks, Modes)
Assets/_Project/Prefabs/  -> prefabs (eu monto no Editor, você só escreve scripts)
docs/                     -> especificações detalhadas (leia sob demanda)
```

## Divisão de trabalho (importante)

Com a Unity MCP conectada, você (Claude Code) faz muito mais que escrever código:
também monta cena, cria e configura assets, e LÊ O CONSOLE para ver erros de
compilação e runtime e corrigir sozinho. Use o console como seu loop de feedback:
depois de mudar código, verifique o console antes de declarar pronto. Ver
`docs/UNITY_MCP.md`.

Ainda assim, algumas tarefas são externas ou puramente humanas: criar conta e
App ID no painel do Photon (site externo), importar o Fusion via Package Manager
/ Asset Store (licença e GUI), e dar Play de fato para jogar. Para essas, PARE e
instrua o humano com passos numerados.

Se a Unity MCP NÃO estiver conectada na sessão, trate toda interação com o Editor
como tarefa humana e apenas instrua. Cada fase do roadmap separa o que é código,
o que dá para fazer via MCP e o que é externo/humano.

## Docs detalhados (leia quando o assunto aparecer)

- `docs/ROADMAP.md` -> as fases, em ordem, com critério de pronto de cada uma
- `docs/ARCHITECTURE.md` -> sistemas, responsabilidades, fluxo de dados, SOs
- `docs/GDD.md` -> design do jogo: modos, brawlers, números, mecânicas
- `docs/NETWORKING.md` -> padrões e armadilhas do Fusion 2 Host Mode
- `docs/CONVENTIONS.md` -> convenções de C#/Unity e organização de código
- `docs/UNITY_MCP.md` -> o que dá para fazer no Editor via MCP e o que é manual

## Definition of Done (qualquer tarefa)

- Compila sem erros nem warnings novos.
- Respeita as regras de ouro acima.
- Mudanças de estado de jogo estão atrás de `HasStateAuthority`.
- Você descreveu ao humano as tarefas de Editor necessárias para testar.
- Antes de declarar pronto, revise o próprio diff num subagente em contexto limpo.

## Git

Commit pequeno e frequente, um por unidade de trabalho coesa. Mensagem no
imperativo descrevendo o porquê. Não comite segredos (App ID do Photon vai em
arquivo ignorado, ver `docs/NETWORKING.md`). **NUNCA** adicione a linha
`Co-Authored-By` nos commits.
