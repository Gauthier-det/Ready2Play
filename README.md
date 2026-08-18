# Ready2Play

Projet Unity 6 (6000.5.8f1), Universal Render Pipeline (URP).

## Arborescence du projet

```
Assets/
  _Project/                  # Tout le contenu custom du projet (préfixé "_" pour rester en haut de la liste)
    Art/
      Models/                # Meshes 3D, imports FBX/OBJ
      Materials/             # Materials (.mat)
      Textures/              # Textures sources (albedo, normal, etc.)
      Animations/             # Clips d'animation, Animator Controllers
    Audio/
      Music/                 # Musiques
      SFX/                   # Effets sonores
    Prefabs/                 # Prefabs de gameplay, UI, environnement
    Scenes/                  # Scènes du jeu
    Scripts/
      Runtime/
        Core/                # Bootstrap, game state, point d'entrée du jeu
        Gameplay/            # Mécaniques spécifiques au jeu (joueur, ennemis, items...)
        UI/                  # Code d'interface, découplé du gameplay
        Systems/             # Services transverses (audio manager, save, scene loader...)
        Utils/               # Helpers génériques, extensions, sans dépendance au gameplay
      Editor/                # Scripts qui tournent UNIQUEMENT dans l'éditeur (custom inspectors, tools).
                              # Unity exclut ce dossier des builds runtime automatiquement.
      Tests/                 # Tests EditMode / PlayMode
    Settings/                # Assets URP, Input Actions, configs ScriptableObject

Packages/                    # Dépendances du projet (manifest.json)
ProjectSettings/             # Réglages du projet, versionnés en git
```

`Library/`, `Temp/`, `Logs/`, `UserSettings/` sont générés par l'éditeur Unity et ignorés par git
(voir `.gitignore`) — ils peuvent être supprimés sans risque, Unity les régénère à l'ouverture.

## Principes d'architecture

- **Core** ne connaît rien du gameplay : il initialise les `Systems` au démarrage et rien d'autre.
- **Systems** sont des services transverses (audio, save, scènes...), consommés par `Gameplay` et `UI`,
  mais qui ne dépendent jamais du gameplay en retour.
- **Gameplay** dépend des `Systems`, jamais de `UI` directement.
- **UI** communique avec le gameplay via events / ScriptableObjects, pas de référence directe à la logique de jeu.
- Un dossier de `Scripts/Runtime/*` correspond, à terme, à une assembly definition (`.asmdef`) dédiée si le
  projet grossit, pour accélérer la compilation et forcer des dépendances propres entre modules.

## Configuration éditeur recommandée

Dans `Edit > Project Settings > Editor` :
- **Version Control > Mode** : `Visible Meta Files`
- **Asset Serialization > Mode** : `Force Text`

Ces deux réglages sont nécessaires pour que les fichiers `.meta` et les scènes restent diffables et
mergeables avec git.
