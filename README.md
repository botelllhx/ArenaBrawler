# Arena Brawl

Jogo de arena PvP top-down em tempo real, estilo Brawl Stars, feito em Unity com
Photon Fusion 2. Pensado para rodar localmente e jogar com amigos por código de
sala, sem servidor dedicado.

Este repositório é construído majoritariamente com o Claude Code. A pasta `docs/`
e o `CLAUDE.md` são o "cérebro" que mantém o agente consistente ao longo do projeto.

## Pré-requisitos

- Unity 6 (com URP)
- Conta no Photon e um App ID do tipo Fusion (dashboard.photonengine.com)
- Photon Fusion 2 importado no projeto
- Pacote Input System do Unity
- Unity MCP (`com.unity.ai.assistant`) para o Claude Code operar o Editor (ver `docs/UNITY_MCP.md`)

## Como rodar

1. Abra o projeto no Unity 6.
2. Cole seu App ID do Photon na config do Fusion.
3. Abra a cena `Menu`.
4. Play. Um jogador cria a sala (Host) com um código; os outros entram (Join) com
   o mesmo código.

## Jogar com amigos

Quem hospeda escolhe um código de sala e compartilha. Os amigos abrem o jogo,
digitam o mesmo código e entram. O relay do Photon cuida da conexão; não é preciso
abrir portas no roteador.

## Estrutura de docs

- `CLAUDE.md` — memória de projeto para o Claude Code (regras de ouro, índice)
- `docs/ROADMAP.md` — fases de construção, em ordem
- `docs/ARCHITECTURE.md` — desenho dos sistemas e modelo orientado a dados
- `docs/GDD.md` — design do jogo, modos, brawlers, números
- `docs/NETWORKING.md` — padrões e armadilhas do Fusion 2
- `docs/CONVENTIONS.md` — convenções de código
- `docs/UNITY_MCP.md` — setup da Unity MCP e divisão de trabalho com o Editor

## Estado

Em construção, fase a fase, conforme `docs/ROADMAP.md`.
