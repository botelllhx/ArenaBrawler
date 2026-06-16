Revise o diff atual com olhar crítico e independente, como se você não tivesse
escrito o código.

Verifique especificamente:

1. Regras de ouro do `CLAUDE.md` respeitadas (orientado a dados, servidor
   autoritativo, simulação em FixedUpdateNetwork, input via Fusion).
2. Toda mudança de estado de jogo está atrás de `HasStateAuthority`.
3. Nenhum uso de API do Fusion 1 desatualizada; nomes batem com a doc oficial.
4. Sem `Input.Get*` dentro da simulação, sem lógica de jogo em `Update()`.
5. Convenções de `docs/CONVENTIONS.md`.
6. O resultado satisfaz o Definition of Done da fase em questão (me diga qual
   fase considerar, ou infira pelo diff).

Reporte os problemas encontrados como uma lista objetiva, com arquivo e linha,
separando "bloqueante" de "melhoria opcional". Não conserte ainda; só reporte.
