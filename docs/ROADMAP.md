# ROADMAP

Construção em fases. Regra inviolável: **cada fase termina jogável**, mesmo que
feia. Nunca acumular sistemas sem testar em rede. Só avance para a fase seguinte
quando o Definition of Done da atual estiver verde.

Legenda:
- **[Código]** = Claude Code escreve código
- **[Editor]** = trabalho no Editor da Unity. Com a Unity MCP conectada, o Claude
  Code faz boa parte (cena, assets, ScriptableObjects, ler console). O que for
  externo (painel do Photon, importar Fusion, dar Play) continua humano. Ver
  `docs/UNITY_MCP.md`.
- **DoD** = Definition of Done, o teste que prova que a fase acabou

---

## Fase 0 — Fundação do projeto

**[Editor]**
1. Criar projeto Unity 6 com template URP (2D ou 3D, sugiro 3D com câmera
   ortográfica top-down para ganhar iluminação).
2. Criar conta no painel do Photon (dashboard.photonengine.com), criar um app do
   tipo Fusion, copiar o App ID.
3. Importar o Photon Fusion 2 (Asset Store ou SDK do site do Photon) e colar o
   App ID na config do Fusion.
4. Instalar o pacote Input System (Window > Package Manager) e ativar o backend
   novo (Edit > Project Settings > Player > Active Input Handling = Input System
   ou Both).
5. Criar a estrutura de pastas conforme `CLAUDE.md`.
6. Instalar e conectar a Unity MCP para o Claude Code poder operar o Editor e ler
   o console. Passos em `docs/UNITY_MCP.md`. Recomendado fazer já na Fase 0, pois
   acelera todas as fases seguintes.

**[Código]**
- Criar os Assembly Definitions: `Arena.Core`, `Arena.Networking`,
  `Arena.Gameplay`, `Arena.UI` com as dependências corretas.
- Criar `Bootstrap.cs` que apenas loga "boot ok" no Awake, só para validar a
  estrutura de assemblies.

**DoD:** projeto abre sem erro de compilação, Fusion reconhecido, log "boot ok"
aparece ao dar Play.

---

## Fase 1 — Conexão e movimento em rede

A fase mais importante. Se isso funciona suave, o resto é construção.

**[Código]**
- `NetworkBootstrap.cs`: inicia o `NetworkRunner` em Host Mode com `SessionName`
  vindo de um campo de texto (código da sala). Implementa
  `INetworkRunnerCallbacks`.
- `NetworkInputData` (struct `INetworkInput`): direção de movimento, direção de
  mira, flags de ataque/super.
- `PlayerInputCollector.cs`: lê o Input System e preenche `NetworkInputData` no
  callback `OnInput`.
- `PlayerMotor.cs` (NetworkBehaviour): lê o input em `FixedUpdateNetwork()` e
  move o personagem. Sem combate ainda, só andar.
- Spawn do player em `OnPlayerJoined` via `Runner.Spawn`, com input authority
  para o jogador que entrou.

**[Editor]**
- Criar prefab de player (cápsula + script) e registrar como Network Object.
- Cena `Menu` com um campo de texto para o código da sala e botões Host/Join.
- Cena `Arena` vazia com chão e câmera top-down.

**DoD:** dois clientes (duas instâncias do jogo, ou dois PCs) entram na mesma
sala por código, cada um controla sua cápsula, e os dois veem o movimento um do
outro suave. Testar com latência simulada pela ferramenta de network do Fusion.

---

## Fase 2 — Combate de um brawler

**[Código]**
- `Health.cs` (NetworkBehaviour): vida como `[Networked]`, dano só aplicado no
  State Authority, morte e evento de morte.
- `AmmoSystem.cs`: 3 cargas de munição que recarregam com o tempo, tudo `[Networked]`.
- `Weapon.cs`: ao receber input de ataque com munição disponível, gasta carga e
  dispara projétil via `Runner.Spawn` (só no State Authority).
- `Projectile.cs` (NetworkBehaviour): move em `FixedUpdateNetwork`, detecta
  colisão usando **lag compensation** (`Runner.LagCompensation`), aplica dano,
  se despawna.
- Câmera top-down que segue o player local (em `Render`/`LateUpdate`, é visual).

**[Editor]**
- Prefab de projétil registrado como Network Object e referenciado na arma.
- Aim por mouse (PC) mapeado no Input System.

**DoD:** um jogador atira no outro, a vida cai no servidor e replica para os dois,
munição recarrega, quem zera a vida "morre". Tiro parece justo mesmo com ping.

---

## Fase 3 — Brawlers orientados a dados + super

**[Código]**
- `BrawlerDefinition` (ScriptableObject): vida, velocidade, dano, munição máxima,
  tempo de recarga, alcance, carga de super necessária. Ver `docs/ARCHITECTURE.md`.
- `AttackDefinition` e `SuperDefinition` (ScriptableObjects).
- Refatorar `Health`, `AmmoSystem`, `Weapon` para lerem stats da `BrawlerDefinition`
  em vez de valores fixos.
- `SuperMeter.cs`: carrega ao causar dano, descarrega ao usar o super, dispara um
  efeito de super (ex: tiro em leque, dash, escudo).

**[Editor]**
- Criar 2 ou 3 assets de BrawlerDefinition com números diferentes para testar
  que trocar dados muda o brawler sem mexer em código.

**DoD:** dá para jogar com brawlers diferentes só trocando o ScriptableObject
atribuído, e o super carrega e dispara corretamente, replicado em rede.

---

## Fase 4 — Um modo completo: Gem Grab

**[Código]**
- `GameModeBase.cs` (NetworkBehaviour): máquina de estados (aquecimento, jogando,
  fim), timer, condição de vitória, controle de respawn.
- `GemGrabMode.cs`: spawner central de gema, contagem de gemas por time, lógica
  de "segurar 10 por X segundos para vencer", derrubar gemas ao morrer.
- `Gem.cs` e `GemSpawner.cs` (NetworkBehaviours).
- Respawn de jogadores com delay, tudo autoritativo.
- Times (atribuir time no join).

**[Editor]**
- Cena `Arena_GemGrab` com o spawner posicionado e pontos de spawn dos times.

**DoD:** partida 2v2 (ou 1v1 para teste) de Gem Grab começa, jogadores coletam e
seguram gemas, morte derruba gemas, um time vence e a partida reinicia. Score e
posse corretos em todos os clientes.

---

## Fase 5 — Mapa e ambiente

**[Código]**
- `Wall.cs`: bloqueia movimento e projétil (colisão networkada coerente).
- `Bush.cs`: dá stealth, esconde quem está dentro dos olhos de quem está fora,
  com a regra de revelar ao atacar ou ao aproximar. Visibilidade calculada de
  forma autoritativa mas renderizada por cliente.
- Destrutíveis opcionais (paredes que quebram com dano).

**[Editor]**
- Montar um layout de mapa com paredes e arbustos.

**DoD:** dá para se esconder em arbusto e sumir do oponente, paredes bloqueiam
tiro e passagem, comportamento idêntico em todos os clientes.

---

## Fase 6 — Game feel

**[Código]**
- Feedback de hit (flash, knockback leve, números de dano).
- Hooks de VFX/SFX nos eventos (tiro, dano, morte, super, coleta de gema).
- Animação/tween de spawn, morte, recarga (DOTween).
- Indicadores de HUD: barra de vida, munição, medidor de super.

**[Editor]**
- Importar/atribuir partículas, sons e sprites (placeholder de Kenney serve).

**DoD:** o jogo "sente" como jogo, não protótipo. Feedbacks claros em cada ação.

---

## Fase 7 — Menu, lobby e seleção

**[Código]**
- Fluxo: menu -> escolher brawler -> criar/entrar sala por código -> aguardar
  jogadores -> começar -> partida -> tela de resultado -> voltar ao menu.
- Persistência local simples da última escolha de brawler.

**[Editor]**
- Telas de UI montadas.

**DoD:** um amigo consegue, do zero, abrir o jogo, escolher brawler, entrar na
sua sala por um código e jogar uma partida inteira.

---

## Depois

Brawlers e modos novos viram conteúdo, não engenharia: criar ScriptableObjects e,
no máximo, um comportamento de super novo. É o pagamento da arquitetura de dados.
