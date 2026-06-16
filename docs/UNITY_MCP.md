# UNITY_MCP

Usamos a **MCP for Unity da CoplayDev** (open source, MIT, gratuita), e NÃO a
oficial `com.unity.ai.assistant`. A oficial, a partir da versão 2.7, passou a
exigir um assento de IA pago da Unity (sintoma: "Up to 0 direct connections
allowed" e "Connection revoked"). A da CoplayDev não tem essa cerca.

Repositório: https://github.com/CoplayDev/unity-mcp
Docs: https://coplaydev.github.io/unity-mcp/

## O que ela faz

Liga o Claude Code ao Editor da Unity via MCP: gerenciar assets, controlar cena,
editar scripts, rodar testes e ler o console. Roda um servidor local em Python
(via uv), sem custo e sem login da Unity.

## Setup (tarefa humana, uma vez)

Pré-requisitos: Python 3.10+ e uv.
- Python: instalar de python.org (no Windows, marcar "Add to PATH"). Conferir com
  `python --version`.
- uv (PowerShell): `irm https://astral.sh/uv/install.ps1 | iex`. Conferir com
  `uv --version`.

Evitar conflito com a oficial:
- Em Project Settings > AI > Unity MCP Server (oficial), clique Disable no Claude
  Code.
- No Claude Code, `claude mcp list` e remova qualquer servidor antigo que aponte
  para `.unity\relay\relay_win.exe` com `claude mcp remove <nome>`.

Instalar a CoplayDev:
1. Unity: Window > Package Manager > + > Add package from git URL:
   `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`
2. Window > MCP for Unity. Se faltar Python/uv, use o botão de instruções.
3. Clique "Configure All Detected Clients" para escrever a config do Claude Code.
4. Se o Unity Bridge estiver Stopped, clique Start Bridge.

Conectar e testar:
- Reabra `claude` na pasta do projeto, `/mcp` mostra a MCP for Unity conectada.
- Teste: "Create a red, blue, and yellow cube in the current scene". Três cubos
  na cena = funcionando.

## O que o Claude Code PASSA a fazer via MCP

- Ler o console (erros de compilação e runtime). Use como loop: mudou código,
  leia o console, corrija antes de dar por pronto.
- Criar e editar cena, posicionar objetos, montar a arena.
- Operações de asset, incluindo ScriptableObjects (BrawlerDefinition etc).
- Editar scripts (já fazia via filesystem; segue valendo).

## O que continua MANUAL ou externo

- Conta e App ID no painel do Photon. Site externo.
- Importar o Photon Fusion 2 e aceitar licença. GUI.
- Dar Play para jogar de fato e validar a sensação.

## Regra de uso para o agente

Mantenha a Unity aberta durante as sessões, senão a ponte cai. Prefira resolver
tarefas de Editor pela MCP e confirme lendo o console. Se uma ação não for
possível via MCP, ou se a MCP não estiver conectada, pare e instrua o humano.
Nunca finja ter feito algo no Editor que não foi confirmado.
