# Gradon Tower Defense Game

Bem-vindo ao README do projeto de Tower Defense! Este documento detalha a arquitetura, as mecânicas de jogo e os próximos passos para o desenvolvimento deste jogo de estratégia em pixel art.

![Placeholder para GIF do Jogo](https://via.placeholder.com/600x300.png?text=Adicione+um+GIF+do+seu+gameplay+aqui!)

## 📜 Visão Geral

Este é um jogo de Tower Defense com uma temática de mitologia japonesa, onde o jogador deve defender um Totem sagrado contra ondas de Yokai (demônios e espíritos). O jogo combina estratégia de posicionamento, gerenciamento de recursos e progressão de torres para criar uma experiência desafiadora e recompensadora.

## ✨ Funcionalidades Principais

O projeto atualmente conta com os seguintes sistemas totalmente funcionais:

*   **Sistema de Fases e Ondas Dinâmico:**
    *   Utiliza **Scriptable Objects (`LevelData`)** para definir fases e ondas, permitindo que novos níveis sejam criados e balanceados diretamente no editor da Unity, sem a necessidade de alterar o código.
    *   Suporta múltiplas ondas por fase, cada uma com múltiplos grupos de inimigos.

*   **Economia Baseada em Mana:**
    *   As torres têm um custo em mana para serem construídas.
    *   Os inimigos concedem mana automaticamente ao serem derrotados, criando um ciclo de gameplay de risco e recompensa.

*   **Sistema de Construção Intuitivo:**
    *   O jogador constrói torres arrastando "cartas" da UI para o campo de batalha (`ArrastavelUI`).
    *   Feedback visual claro indica se a posição é válida, se há mana suficiente ou se a área é proibida (próximo ao Totem).

*   **Variedade de Torres e Inimigos:**
    *   **Torres:** Sistema de herança (`TowerBase` -> `TowerWithBuffs`) que permite a fácil criação de novas torres.
        *   **Dragão:** Torre de longo alcance com dano de alvo único.
        *   **Samurai:** Torre de combate corpo a corpo (ou curto alcance).
        *   **Kirin:** Torre de suporte que aplica buffs de velocidade de ataque e dano em torres aliadas próximas.
    *   **Inimigos:** Sistema de herança (`EnemyController`) com comportamentos distintos.
        *   `NormalEnemy`: Inimigo básico.
        *   `RangedEnemy`: Ataca as torres à distância.
        *   `ExplosiveEnemy`: Causa dano em área ao morrer.

*   **Progressão e Estratégia:**
    *   **Sistema de Upgrade de Torres:** O jogador pode clicar em uma torre já construída para gastar mais mana e aprimorá-la, aumentando seu dano, alcance e cadência de tiro.
    *   **UI Interativa:** Um painel de informações aparece ao selecionar uma torre, mostrando seus status atuais e o custo do próximo upgrade.

*   **Gerenciamento Centralizado:**
    *   Um `GameManager` (Singleton) persiste entre as cenas, gerenciando a seleção e o carregamento das fases.
    *   Um `UIManager` controla todos os elementos do HUD (vida, mana, informações de onda), mantendo a lógica de UI separada do gameplay.

## 🔄 Ciclo de Gameplay (Gameplay Loop)

1.  O jogador inicia no **Menu Principal** e seleciona uma fase.
2.  O `GameManager` carrega a cena de jogo e os dados da fase selecionada (`LevelData`).
3.  O `WaveSpawner` lê os dados da fase e inicia a contagem para a primeira onda.
4.  O jogador usa sua **mana inicial** para construir as primeiras torres, arrastando-as da UI.
5.  As **ondas de inimigos** começam a aparecer, avançando em direção ao Totem.
6.  Ao derrotar inimigos, o jogador **ganha mana** automaticamente.
7.  Com a nova mana, o jogador deve decidir entre:
    *   Construir mais torres para aumentar a cobertura.
    *   Fazer o **upgrade** de torres existentes para fortalecê-las.
8.  O objetivo é sobreviver a todas as ondas da fase. Se o **Totem for destruído**, o jogador perde. Se **todas as ondas forem concluídas**, o jogador vence.

## 🛠️ Como Adicionar Novas Fases (Guia Rápido)

O sistema foi projetado para ser facilmente expansível:

1.  **Crie o Arquivo da Fase:** Na pasta `Project/Levels`, clique com o botão direito -> `Create > Tower Defense > Level`. Renomeie o arquivo (ex: `Level_06`).
2.  **Configure a Fase:** Selecione o novo arquivo `.asset`. No **Inspector**, preencha o nome, índice, mana inicial e, o mais importante, configure a lista de `Waves` e `Enemy Groups`, arrastando os prefabs de inimigos desejados.
3.  **Adicione ao Catálogo:** Vá para a cena do **Menu Principal**, selecione o objeto `GameManager`. No Inspector, adicione o novo arquivo `Level_06.asset` à lista `All Levels`.
4.  **Crie o Botão:** Adicione um novo botão na UI do menu e configure seu `On Click()` para chamar a função `GameManager.LoadLevel(6)`.

## 🚀 Próximos Passos e Roadmap

O projeto tem uma base sólida. As próximas etapas para o desenvolvimento incluem:

- [ ] **Implementar as Mecânicas do Boss (Tek Tek):** Adicionar as habilidades de "Pisada Tectônica" e "Invocação de Fragmentos".
- [ ] **Adicionar Mais Torres:** Criar torres com efeitos de status, como lentidão (slow) ou dano contínuo (veneno).
- [ ] **Adicionar Mais Inimigos:** Criar inimigos voadores (que exigem torres específicas) ou inimigos rápidos.
- [ ] **Desenvolver a UI do Menu:** Criar uma tela de seleção de fases que seja gerada dinamicamente a partir da lista no `GameManager`.
- [ ] **Adicionar Efeitos Sonoros e Música:** Implementar um `AudioManager` para dar vida ao jogo.
- [ ] **Polimento Visual:** Adicionar efeitos de partículas para ataques, mortes e upgrades.

---
**Desenvolvido por: DeadWhispers Studio**
