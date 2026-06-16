# ARCHITECTURE

Como o jogo é organizado. O princípio central é **separar dados de comportamento**:
o que um brawler é (números) vive em ScriptableObjects; como um brawler funciona
(lógica) vive em poucas classes genéricas. Adicionar conteúdo é criar dados, não
escrever classes.

## Camadas (Assembly Definitions)

- `Arena.Core` — bootstrap, máquina de estado de aplicação, utilidades. Não
  depende de nada do jogo.
- `Arena.Networking` — runner do Fusion, conexão, struct de input, callbacks.
- `Arena.Gameplay` — player, combate, modos, ambiente. Depende de Core e Networking.
- `Arena.UI` — HUD, menus, joysticks. Depende de Core, observa Gameplay por eventos.

Regra de dependência: setas só apontam para baixo (UI -> Gameplay -> Networking ->
Core). Nunca o contrário. Comunicação de volta é por eventos.

## Modelo orientado a dados

Três famílias de ScriptableObject. Elas são **dados puros**, sem lógica de rede.

### BrawlerDefinition
```
nome, ícone
vidaMax (int)
velocidadeMovimento (float)
municaoMax (int)
tempoRecargaPorCarga (float)
attack (AttackDefinition)
super (SuperDefinition)
cargaSuperNecessaria (float)   // dano acumulado para encher o super
```

### AttackDefinition
```
projetilPrefab (referência ao NetworkObject)
dano (int)
alcance (float)
velocidadeProjetil (float)
quantidadeProjeteis (int)      // 1 = tiro reto, 3+ = leque/shotgun
anguloDispersao (float)
```

### SuperDefinition
```
tipo (enum: TiroEspecial, Dash, Escudo, Invocacao...)
parâmetros específicos por tipo
```

Trocar o brawler de um jogador é trocar a `BrawlerDefinition` atribuída. Nenhuma
classe muda.

## Componentes de gameplay (lógica genérica)

Todos são `NetworkBehaviour` quando guardam estado de jogo.

- `PlayerController` — orquestra os componentes do player, segura a referência à
  `BrawlerDefinition` atual.
- `PlayerMotor` — movimento a partir do input, na simulação.
- `Health` — vida `[Networked]`, aplica dano só no State Authority, emite evento
  de morte. Lê `vidaMax` da definition.
- `AmmoSystem` — cargas de munição `[Networked]`, recarrega no tick.
- `Weapon` — consome input de ataque, gasta munição, spawna projétil(s) conforme
  `AttackDefinition`. Spawn só no State Authority.
- `Projectile` — move na simulação, acerto via lag compensation, aplica dano.
- `SuperMeter` — acumula dano causado, libera o super ao encher.
- `SuperExecutor` — lê `SuperDefinition` e executa o efeito (switch por tipo).

## Modos de jogo

`GameModeBase` é uma máquina de estados networkada (Warmup -> Playing -> Ended)
com timer, regra de vitória e respawn. Cada modo concreto (`GemGrabMode`,
`ShowdownMode`, `BrawlBallMode`) herda e implementa só suas regras. O modo é o
"dono" das regras da partida e roda no State Authority.

## Fluxo de dados de uma ação (ex: atirar)

1. Cliente lê mouse/joystick e preenche `NetworkInputData` (camada Networking).
2. Fusion entrega esse input no tick via `GetInput<NetworkInputData>()`.
3. `Weapon.FixedUpdateNetwork()` roda nos dois lados, mas o `Runner.Spawn` do
   projétil só acontece no State Authority.
4. `Projectile` simula e, no State Authority, faz o raycast lag-compensado.
5. No acerto, `Health` (State Authority) reduz a vida `[Networked]`.
6. Fusion replica a vida nova; o cliente vê em `Render()` e a UI reage ao evento.

## Eventos e UI

Gameplay expõe eventos C# (vida mudou, morreu, super pronto, gema coletada). A UI
assina esses eventos. A UI nunca escreve estado de jogo, só lê e mostra.

## O que NÃO fazer

- Não criar `class Shelly : Brawler`. Use dados.
- Não guardar estado de jogo em MonoBehaviour comum. Estado replicado é `[Networked]`.
- Não acessar `Input.Get*` dentro da simulação. Use o input do Fusion.
- Não decidir dano/score no cliente. Só no State Authority.
