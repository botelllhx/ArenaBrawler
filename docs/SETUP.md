# SETUP — Montando o ambiente do zero

Guia completo, na ordem certa. Faça uma parte por vez e confira o resultado antes
de seguir. Onde Windows e Mac diferem, está marcado. O resto é igual nos dois.

## Mapa mental (entenda antes de começar)

Três programas trabalham juntos:

- **Unity (Editor)**: onde o jogo vive. Fica aberto o tempo todo durante o trabalho.
- **VS Code**: onde você vê e edita o código, e de onde você roda o Claude Code
  (terminal integrado). Você abre a PASTA do projeto Unity dentro dele.
- **Claude Code**: o agente, roda no terminal. Ele mexe nos arquivos do projeto e,
  via Unity MCP, também conversa com o Editor (cria cena, lê erros do console).

Não existe "colocar arquivos no VS Code". Você abre a pasta do projeto no VS Code
e os arquivos (CLAUDE.md, docs/, scripts) ficam lá dentro, no disco.

---

# Parte 1 — Ferramentas base

## 1.1 Git
1. Baixe em https://git-scm.com/downloads e instale com as opções padrão.
   (No Windows isso também instala o Git Bash, que o Claude Code usa.)
2. Confirme: abra o terminal e rode `git --version`.

## 1.2 VS Code + extensão de Unity
1. Baixe o VS Code em https://code.visualstudio.com e instale.
2. Abra o VS Code, vá na aba de Extensões (ícone de blocos na lateral).
3. Procure e instale a extensão **Unity** (publicada pela Microsoft). Ela já
   traz o C# Dev Kit e o suporte a C#.

## 1.3 Claude Code
Método recomendado (instalador nativo, não precisa de Node.js).
- **Windows (PowerShell)**: `irm https://claude.ai/install.ps1 | iex`
- **Mac/Linux (terminal)**: `curl -fsSL https://claude.ai/install.sh | bash`

Depois:
1. Feche e reabra o terminal (pra ele pegar o PATH novo).
2. Rode `claude --version` para confirmar.
3. Rode `claude` numa pasta qualquer. Na primeira vez ele abre o navegador para
   login. Entre com a sua conta Anthropic do plano Max. Não use `sudo`.
4. Opcional de diagnóstico: `claude doctor` checa o ambiente.

---

# Parte 2 — Unity

## 2.1 Unity Hub
1. Baixe o Unity Hub em https://unity.com/download e instale.
2. Abra e faça login com uma conta Unity (crie se não tiver; o plano Personal
   gratuito serve).

## 2.2 Instalar o Unity 6
1. No Hub, aba **Installs** > **Install Editor**.
2. Escolha a versão **Unity 6 LTS** (a 6000.x recomendada).
3. Em módulos, marque:
   - **Windows Build Support (IL2CPP)** no Windows, ou **Mac Build Support** no
     Mac. Isso permite gerar um executável pra mandar pros amigos depois.
   - Pode deixar o Visual Studio desmarcado, já que vamos usar o VS Code.
4. Conclua a instalação (demora, são vários GB).

## 2.3 Criar o projeto
1. No Hub, aba **Projects** > **New project**.
2. Template **Universal 3D** (URP). Top-down com câmera ortográfica rende bem em 3D.
3. Nome: `ArenaBrawl`. Escolha a pasta. **Anote esse caminho**, é a raiz do projeto.
4. Create. Espere abrir.

## 2.4 Apontar o VS Code como editor
1. Na Unity: **Edit > Preferences > External Tools** (no Mac, **Unity > Settings**).
2. Em **External Script Editor**, selecione **Visual Studio Code**.

---

# Parte 3 — Photon Fusion 2

## 3.1 Conta e App ID
1. Crie conta em https://dashboard.photonengine.com
2. **Create a New App**. No dropdown **Select Photon SDK** escolha **Fusion**, e
   no dropdown de versão que aparece escolha **Fusion 2**. Dê um nome e crie.
3. Abra o app criado e **copie o App ID** (uma string longa). Guarde. O plano
   gratuito (20 CCU) é suficiente para jogar com amigos.

## 3.2 Importar o Fusion 2 no projeto
Atenção: o pacote NÃO se chama "Photon Fusion 2". Na Asset Store ele se chama só
**Photon Fusion** (o card "Tools / Verified Solution" da Photon Engine, gratuito).
A versão atual dele já é a Fusion 2. NÃO confunda com "Photon Fusion Starter", que
é um projeto de exemplo, não o SDK.

Caminho A, pela Asset Store:
1. Em https://assetstore.unity.com procure **Photon Fusion**, abra o card Verified
   Solution e "Add to My Assets" (gratuito).
2. Na Unity: **Window > Package Manager** > aba **My Assets** > Photon Fusion >
   **Download** > **Import**. Importe tudo.

Caminho B, recomendado pela Photon (mais explícito sobre versão):
1. Em https://doc.photonengine.com/fusion , vá em Getting Started > SDK & Release
   Notes e baixe o `.unitypackage` da Fusion 2 estável (linha 2.0.x, NÃO a 2.1
   preview).
2. Na Unity: **Assets > Import Package > Custom Package** e selecione o arquivo.

Use a versão **2.0.x estável**. Evite a 2.1, que é preview e pode quebrar.

## 3.3 Colar o App ID
1. Após importar, abre um assistente do Fusion (Fusion Hub). Se não abrir, vá em
   **Tools > Fusion > Fusion Hub**.
2. Cole o **App ID** do Fusion no campo indicado e salve.

---

# Parte 4 — Pacotes da Unity

## 4.1 Input System
1. **Window > Package Manager** > **Unity Registry** > procure **Input System** >
   **Install**.
2. A Unity vai perguntar se quer ativar o backend novo e reiniciar. Aceite. (Se
   quiser manter os dois, em **Project Settings > Player > Active Input Handling**
   escolha **Both**.)

## 4.2 Unity MCP (CoplayDev, open source)
NÃO use a MCP oficial (`com.unity.ai.assistant`): a partir da 2.7 ela exige
assento de IA pago da Unity e dá "Connection revoked". Use a da CoplayDev.

Pré-requisitos: Python 3.10+ (python.org, marcar "Add to PATH") e uv
(PowerShell: `irm https://astral.sh/uv/install.ps1 | iex`).

1. **Window > Package Manager** > **+** > **Add package from git URL**:
   `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`
2. Detalhes em docs/UNITY_MCP.md.

---

# Parte 5 — Conectar o Claude Code à MCP for Unity (CoplayDev)

## 5.1 Configurar o cliente
1. Abra **Window > MCP for Unity**. Confira Server Status e Unity Bridge.
2. Se faltar Python ou uv, use o botão de instruções de instalação dele.
3. Clique **Configure All Detected Clients** (escreve a config do Claude Code).
4. Se o Unity Bridge estiver **Stopped**, clique **Start Bridge**.

## 5.2 Evitar conflito com a oficial
Se você chegou a configurar a MCP oficial antes:
1. Em **Project Settings > AI > Unity MCP Server**, clique **Disable** no Claude Code.
2. No Claude Code, `claude mcp list` e remova o servidor antigo que aponta para
   `.unity\relay\relay_win.exe` com `claude mcp remove <nome>`.

## 5.3 Conectar e testar
1. Com a Unity aberta, inicie o Claude Code na pasta do projeto: `claude`.
2. `/mcp` deve listar a MCP for Unity conectada.
3. Teste: peça "Create a red, blue, and yellow cube in the current scene". Se
   três cubos aparecerem na cena, está funcionando.

---

# Parte 6 — Colocar o "cérebro" do projeto

1. Copie para a RAIZ do projeto (a pasta `ArenaBrawl` do passo 2.3):
   - `CLAUDE.md`
   - a pasta `docs/` inteira
   - a pasta `.claude/` inteira (com os comandos)
   - o `README.md`
   - o `.gitignore` (já pronto neste pacote)
2. Inicie o versionamento. No terminal, dentro da pasta do projeto:
   ```
   git init
   git add .
   git commit -m "Ambiente e contexto inicial do projeto"
   ```
3. Abra a pasta do projeto no VS Code: **File > Open Folder** > selecione a pasta
   `ArenaBrawl`.

---

# Parte 7 — Primeira sessão e uso diário

## Abrir o Claude Code
- No VS Code: **Terminal > New Terminal**, e rode `claude`. (Ou instale a extensão
  do Claude Code para uma integração mais visual; o terminal já funciona.)
- Deixe a **Unity aberta em paralelo**, senão a ponte MCP cai e ele fica cego.

## Primeiros comandos
- `/init` uma vez: ele varre o projeto e se situa. Seu CLAUDE.md já está pronto,
  então é mais reconhecimento.
- `/model` para escolher o **Opus** como modelo principal (você está no Max).
- Use o **plan mode**: aperte **Shift+Tab** para alternar. Deixe ele planejar
  antes de codar.

## O loop de cada fase
1. Peça `/fase 0` (depois `/fase 1`, etc). Ele lê o roadmap e propõe um plano.
2. Revise o plano. Aprove.
3. Ele implementa. Com a MCP, também monta cena/assets e lê o console.
4. Faça as tarefas externas que ele listar (painel do Photon, importações).
5. Dê **Play** na Unity para testar o Definition of Done da fase.
6. Rode `/revisar` para ele auditar o próprio diff antes do commit.
7. Commit. Próxima fase.

## Regra de ouro do dia a dia
Uma fase por vez. Só avance quando a atual passar no teste dela. E sempre que ele
mexer em código, confirme pelo console (via MCP) que compilou antes de comemorar.
