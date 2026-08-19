# Journal de décisions

Décisions de design/architecture actées au fil du projet. Complète [STORY.md](STORY.md) (narration) et
[GAMEPLAY.md](GAMEPLAY.md) (mécaniques), ne les remplace pas.

## 2026-08-19 — Scope du premier vertical slice

On ne construit pas la vision complète de STORY.md d'un coup. Première étape : un seul petit parcours,
un seul protagoniste jouable, un seul PNJ, un seul souvenir à collecter. Objectif : valider que la boucle
"marcher → parler → découvrir l'indice → collecter le souvenir" est intéressante avant d'investir dans le
reste (deuxième protagoniste, mine, économie...).

**Pourquoi** : projet solo, ambitieux, en cours d'apprentissage Unity — le risque principal est de vouloir
poser tous les systèmes avant d'avoir vérifié que le cœur du jeu fonctionne.

## 2026-08-19 — Deux protagonistes : piste multijoueur

Le choix de deux protagonistes dans l'histoire n'est pas qu'un choix narratif : c'est pour garder la porte
ouverte à un mode multijoueur à deux plus tard.

**Impact sur l'architecture** : ne pas implémenter de réseau maintenant, mais éviter les raccourcis qui
supposent "un seul joueur" de façon rigide dans les futurs scripts `Gameplay`/`Systems` (ex: penser en
liste de joueurs plutôt qu'une référence unique en dur), pour ne pas avoir à tout refactoriser si le
multijoueur est ajouté plus tard.

## 2026-08-19 — Consommables : besoin réel, mais secondaire

Le système de consommables (nourriture/boisson, boosts, indices, monnaie) reste dans la vision du jeu,
mais volontairement hors scope du vertical slice et des prochaines étapes proches. À reprendre une fois
la boucle cœur du jeu validée.

## Prochaine étape

Ouvrir [GAMEPLAY.md](GAMEPLAY.md) pour traduire la narration de STORY.md en mécaniques concrètes
(perspective caméra, type de contrôle, forme des interactions/dialogues) avant d'écrire le code du
premier parcours.
