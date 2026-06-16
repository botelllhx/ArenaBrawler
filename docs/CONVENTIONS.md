# CONVENTIONS

Convenções de código. Curtas e obrigatórias.

## C# / Unity

- Namespaces seguem os assemblies: `Arena.Gameplay.Combat`, `Arena.UI`, etc.
- `PascalCase` para tipos, métodos e propriedades públicas. `camelCase` para
  campos privados, com `_` na frente: `_health`. Constantes em `PascalCase`.
- Um tipo por arquivo, nome do arquivo = nome do tipo.
- Prefira composição a herança. A herança permitida é a dos modos de jogo
  (`GameModeBase`) e nada além sem justificativa.
- `[SerializeField] private` em vez de campos públicos para expor no Inspector.
- Evite `GameObject.Find` e `FindObjectOfType` em hot path. Resolva referências
  no spawn ou via injeção simples.
- Cache componentes em `Awake`/`Spawned`, não busque por frame.
- Comentário explica o porquê, não o quê. Código óbvio não precisa de comentário.
- Sem `async void` exceto em handlers do próprio Unity. Use `async Task`.

## Unity específico

- Lógica de jogo replicada herda `NetworkBehaviour`, não `MonoBehaviour`.
- Inicialização de objeto de rede vai em `Spawned()`, não em `Start()`.
- Nada de `Update()` para simulação. Ver `docs/NETWORKING.md`.
- ScriptableObjects são dados, sem lógica de rede dentro.

## Organização

- Scripts por feature, não por tipo: a pasta `Combat/` tem `Weapon`, `Projectile`,
  `Health` juntos, não uma pasta `Scripts/` gigante.
- Prefabs e assets correspondentes ficam perto da feature quando possível.

## Idioma

- Código e identificadores em inglês (padrão do ecossistema e da API do Fusion).
- Comentários e docs podem ser em português.

## Antes de declarar uma tarefa pronta

- Compila limpo, sem warning novo.
- Sem `Debug.Log` esquecido em caminho de produção (logs de diagnóstico ok se
  intencionais e marcados).
- Diff revisado em subagente de contexto limpo contra o critério da fase.
