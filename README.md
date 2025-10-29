=====================================
🐍 SNAKE – Core INTEGRATION
=====================================

SnakeElysium.Core est un moteur de jeu Snake écrit en C#, totalement indépendant du moteur de rendu.
Ce projet montre comment l’intégrer dans Godot 4.5 à travers un simple adaptateur visuel.

-------------------------------------
STRUCTURE DU PROJET
-------------------------------------
SnakeElysium/
│
├─ SnakeElysium.Core/        → Moteur du jeu (logique pure)
│   ├─ Engine/               → Boucle de jeu, gestion des entités
│   ├─ Domain/               → Modèles de données (Cell, Direction, etc.)
│   ├─ Config/               → Paramètres de jeu
│   └─ SnakeElysium.Core.csproj
│
├─ SnakeElysium.Godot/       → Intégration dans Godot
│   ├─ Main.tscn             → Scène principale
│   ├─ Snake.cs              → Script d’adaptation du moteur au rendu Godot
│   ├─ Assets/               → Sprites, polices, sons
│   └─ SnakeElysium.Godot.csproj
│
└─ SnakeElysium.sln          → Solution globale (.NET)

-------------------------------------
PRÉREQUIS
-------------------------------------
- Godot 4.5 (C# / .NET 8)
- .NET SDK 8.0
- Visual Studio 2022 ou JetBrains Rider

-------------------------------------
STRUCTURE DE LA SCÈNE DANS GODOT
-------------------------------------
SampleScene
├── Main (Node2D)
│    ├── Snake (Node)
│    │    └── Sprite2D (tête du serpent)
│    └── LabelStatus (Label - affichage du score)
├── MainCamera (Camera2D)
└── GlobalLight2D (optionnel)


-------------------------------------
OBJECTIF DU PROJET
-------------------------------------
- Séparer la logique du rendu
- Faciliter le portage entre Godot et Unity
- Illustrer une architecture modulaire
  → Core = logique du jeu
  → Godot = interface et affichage

-------------------------------------
AUTEUR
-------------------------------------
XAVIER Alfred
Ariel Amzallag
Développeur C++
Projet : Elysium.Foundation.Serpentis

© 2025  Tous droits réservés.
