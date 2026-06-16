# GDD — Game Design

Referência de design. Quando uma fase pedir números ou regras, busque aqui.
Tudo é ajustável; estes valores são ponto de partida para o jogo "sentir" certo.

## Visão

Arena top-down, partidas curtas (2 a 3 minutos), times pequenos. Controle
twin-stick: um eixo move, outro mira. Munição limitada que recarrega, e um super
que carrega conforme você causa dano.

## Loop de combate

- Cada brawler tem 3 cargas de munição. Atacar gasta 1. Cada carga recarrega
  sozinha após alguns segundos.
- Causar dano enche o medidor de super. Super cheio libera uma habilidade forte
  de uso único (até encher de novo).
- Morrer respawna após um delay (varia por modo).

## Brawlers de exemplo (números iniciais)

### Tank de perto
```
vidaMax: 6000
velocidade: alta
ataque: shotgun, 5 projéteis em leque, 300 dano cada, alcance curto
recarga: 2.0s por carga
super: dash que atravessa e empurra
```

### Atirador médio
```
vidaMax: 3800
velocidade: média
ataque: 1 projétil reto, 1000 dano, alcance médio-longo
recarga: 1.5s por carga
super: rajada de vários tiros retos
```

### Suporte/área
```
vidaMax: 4200
velocidade: média
ataque: projétil que explode em área, 800 dano
recarga: 1.8s por carga
super: zona de cura ou escudo para aliados
```

Os números acima validam que a `BrawlerDefinition` cobre arquétipos diferentes
sem código novo.

## Modos

### Gem Grab (primeiro a implementar)
- 3v3 no original; comece com 1v1 ou 2v2 para teste.
- Gema nasce no centro a cada poucos segundos.
- Carregar gemas; ao morrer, derruba as que carregava.
- Vitória: um time segura 10 gemas no total por uma contagem regressiva (ex: 15s)
  sem perder a vantagem.
- Respawn rápido.

### Showdown (depois)
- Battle royale individual ou em duplas.
- Último em pé vence. Sem respawn.
- Caixas pelo mapa dão upgrades de poder durante a partida.

### Brawl Ball (depois)
- 3v3, marcar gol levando a bola ao gol adversário.
- Primeiro a 2 gols vence, ou maior placar no tempo.

## Mecânicas de ambiente

- **Arbustos**: dão stealth. Quem está dentro fica invisível para quem está fora,
  até atacar ou chegar muito perto. É marca registrada do gênero.
- **Paredes**: bloqueiam movimento e projéteis. Podem ser destrutíveis.

## Sensação (game feel)

- Resposta imediata ao input (predição do Fusion cuida disso).
- Feedback claro de acerto: flash no alvo, número de dano, leve knockback.
- Telegrafar supers com efeito visual antes do impacto.
- Câmera levemente afastada, leve shake em impactos grandes.
