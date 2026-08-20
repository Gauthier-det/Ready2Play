# Game design — mécaniques

Traduit la narration de [STORY.md](STORY.md) en mécaniques concrètes. Complète [DECISIONS.md](DECISIONS.md)
(journal de décisions) — les choix ici sont ceux qui définissent directement ce qui sera codé.

## Boucle de jeu principale

Explorer un parcours → rencontrer/interagir avec un PNJ ou un point d'intérêt → obtenir un indice ou un
souvenir → progresser dans la compréhension du secret du village.

Scope du premier vertical slice (voir [DECISIONS.md](DECISIONS.md)) : un seul parcours, un protagoniste,
un PNJ, un souvenir.

## Caméra et contrôle

- **Vue 3e personne** pendant les parcours (caméra derrière/à côté du personnage).
- Déplacement en marche uniquement pour le vertical slice (pas de course/stamina pour l'instant — lié aux
  consommables santé, hors scope).

## Dialogues et interactions

- **Plein écran, pause du monde** pendant une interaction (le joueur ne se déplace pas pendant qu'un
  dialogue est actif).
- Une interaction peut débloquer un souvenir ou faire avancer la compréhension du secret du village.
- Une interaction peut faire évoluer le comportement d'un PNJ par la suite (cf. STORY.md).

  > ⚠️ **Valable en solo uniquement.** Une pause globale (`Time.timeScale`) ne fonctionne pas telle
  > quelle si le mode coop à deux (cf. DECISIONS.md) se concrétise : ça figerait aussi le deuxième
  > joueur pendant que le premier parle à un PNJ. À revoir à ce moment-là (ex : pause locale du joueur
  > qui interagit plutôt que pause globale du monde).

## Souvenirs

- Se présentent sous forme de parchemins.
- Deux sources de collecte : points d'intérêt le long des parcours (coffres, crevasses...), ou obtenus
  directement après certaines interactions avec des PNJ.
- Servent à reconstituer l'histoire du village.
- Consultables à tout moment via un **journal dédié** (accessible pendant l'exploration, pas seulement
  au menu pause).

## Détection des interactions

- Un objet/PNJ interactif se **met en surbrillance** (outline/highlight) quand le joueur est à portée —
  pas de texte ni d'icône, juste un effet visuel sur l'objet lui-même.

## Révélation du premier PNJ

- **Découverte progressive** : le PNJ du vertical slice semble parfaitement normal au premier abord. Le
  joueur ne réalise la vérité qu'au fil de l'interaction/exploration — cohérent avec le twist de STORY.md
  (le village semble vivant). Impact direct sur l'écriture du dialogue : les premiers échanges doivent
  sonner "normaux", les indices arrivent progressivement.

## Sauvegarde

- **Points de passage (checkpoints)** : sauvegarde automatique à des moments clés du parcours (début/fin
  de zone, après une interaction importante) — pas de sauvegarde libre ni de slots pour le vertical slice.

## Hors scope pour l'instant

- Deuxième protagoniste / coopération à deux (piste multijoueur, cf. DECISIONS.md).
- Mine (2e partie de l'histoire).
- Consommables et économie (nourriture, boissons, boosts, monnaie).
- Course/stamina (liée aux consommables santé).
- Sauvegarde libre (checkpoints seulement pour l'instant).
