# NETWORKING

Tudo sobre a camada de rede. Antes de escrever qualquer coisa aqui, **confira a
API contra a doc oficial do Fusion 2**: https://doc.photonengine.com/fusion/current/
O Fusion 2 mudou bastante em relação ao Fusion 1, e modelos costumam alucinar a
API antiga. Não confie na memória, confirme os nomes.

## Topologia: Host Mode

Um jogador roda como **Host** (é servidor e cliente ao mesmo tempo, é o State
Authority de tudo). Os demais são **Clients**. O relay do Photon Cloud conecta
todos por um `SessionName` (o código da sala), sem port forwarding. É o que
permite jogar com amigos pela internet sem infraestrutura.

Iniciar (confirmar assinatura exata na doc):
```csharp
await runner.StartGame(new StartGameArgs {
    GameMode    = GameMode.Host,   // ou GameMode.Client para quem entra
    SessionName = codigoDaSala,
    Scene       = ...,             // cena da arena
    SceneManager= ...,
});
```

## Conceitos que governam todo o código de rede

### State Authority vs Input Authority
- **State Authority**: quem pode escrever o estado replicado. No Host Mode é
  sempre o host. TODA mudança de vida, score, posse, spawn passa por aqui.
- **Input Authority**: o cliente dono daquele objeto, que fornece o input dele.
- Cheque com `Object.HasStateAuthority` e `Object.HasInputAuthority`.

Padrão obrigatório para mudar estado:
```csharp
if (Object.HasStateAuthority) {
    Health -= dano;   // só o host decide
}
```

### Propriedades replicadas
Estado que precisa sincronizar usa `[Networked]`:
```csharp
[Networked] public int Health { get; private set; }
```
Para reagir a mudanças no visual, use `ChangeDetector` dentro de `Render()`
(padrão do Fusion 2). Confirmar o uso exato na doc.

### Onde roda o quê
- `FixedUpdateNetwork()` — simulação determinística por tick. TODA a lógica de
  jogo vai aqui (movimento, combate, regras). Nunca em `Update()`.
- `Render()` — interpolação e apresentação visual. Roda por frame, não por tick.
- Leitura de input na simulação:
```csharp
public override void FixedUpdateNetwork() {
    if (GetInput(out NetworkInputData input)) {
        // mover, atirar etc. usando input
    }
}
```

### Input
Defina a struct e implemente `OnInput`:
```csharp
public struct NetworkInputData : INetworkInput {
    public Vector2 Move;
    public Vector2 Aim;
    public NetworkButtons Buttons;   // atacar, super
}
```
O coletor preenche isso no callback. Dentro da simulação só se lê via `GetInput`,
nunca `Input.GetAxis`.

### Spawn e despawn
Só o State Authority spawna/despawna objetos de rede:
```csharp
if (Object.HasStateAuthority) {
    Runner.Spawn(projetilPrefab, pos, rot, inputAuthority: Object.InputAuthority);
}
```
Prefabs de rede precisam estar registrados como Network Object e na config do
Fusion. Esse registro é **tarefa de Editor**, instrua o humano.

### Lag compensation (hit detection justo)
Para tiro parecer justo apesar do ping, o acerto usa o sistema de lag compensation
do Fusion, que reconstrói as posições passadas dos alvos:
```csharp
if (Runner.LagCompensation.Raycast(origem, direcao, alcance, player, out var hit)) {
    // aplicar dano no alvo, no State Authority
}
```
Alvos precisam de hitboxes do Fusion (componentes Hitbox/HitboxRoot), atribuídos
no prefab. Tarefa de Editor.

## Armadilhas comuns (evite ativamente)

- Simular em `Update()` em vez de `FixedUpdateNetwork()`. Quebra a sincronia.
- Ler `Input.Get*` na simulação. Use o input do Fusion ou desincroniza.
- Mudar `[Networked]` sem checar `HasStateAuthority`. Vira inconsistência.
- Spawnar em todos os peers. Só o State Authority spawna.
- Usar API do Fusion 1 (ex: `[Networked(OnChanged=...)]` no estilo antigo). Confirme
  o equivalente atual.
- Misturar lógica visual com lógica de simulação no mesmo método.

## Segredos

O App ID do Photon não vai para o git. Mantenha a config do Fusion fora do
versionamento ou use um arquivo local ignorado. Adicione ao `.gitignore`.
