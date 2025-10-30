=====================================
🐍 SNAKE ELYSIUM – GODOT INTEGRATION
=====================================

SnakeElysium.Core est un moteur de jeu Snake écrit en C#, totalement indépendant du moteur de rendu.
Ce projet montre comment l’intégrer dans Godot 4.5 à travers un simple adaptateur visuel.

-------------------------------------
🎥 APERÇU DU JEU
-------------------------------------




https://github.com/user-attachments/assets/2efa982c-49ec-4b38-b7c7-c1a52e72bd95



-------------------------------------
📂 STRUCTURE DU PROJET
-------------------------------------

* SnakeElysium/
    * SnakeElysium.Core/ : Moteur du jeu (logique pure)
        * Engine/ : Boucle de jeu, gestion des entités
        * Domain/ : Modèles de données (Cell, Direction, etc.)
        * Config/ : Paramètres de jeu
        * SnakeElysium.Core.csproj
    * SnakeElysium.Godot/ : Intégration dans Godot
        * Main.tscn : Scène principale
        * Snake.cs : Script d’adaptation du moteur au rendu Godot
        * Assets/ : Sprites, polices, sons
        * SnakeElysium.Godot.csproj
    * SnakeElysium.sln : Solution globale (.NET)

-------------------------------------
🛠️ PRÉREQUIS
-------------------------------------

* Godot 4.5 (C# / .NET 8)
* .NET SDK 8.0
* Visual Studio 2022 ou JetBrains Rider

-------------------------------------
🔗 LIAISON DU CORE À GODOT
-------------------------------------

Pour permettre à Godot d'accéder aux classes du moteur SnakeElysium.Core, ajoutez cette référence dans votre fichier SnakeElysium.Godot.csproj :

    <ItemGroup>
      <ProjectReference Include="..\..\SnakeElysium.Core\SnakeElysium.Core.csproj" />
    </ItemGroup>

-------------------------------------
🌲 STRUCTURE DE LA SCÈNE GODOT
-------------------------------------

Voici la hiérarchie recommandée pour la scène Main.tscn :

* SampleScene
    * Main (Node2D)  <- Attacher le script Snake.cs ici
        * Snake (Node)
            * Sprite2D (tête du serpent)
        * LabelStatus (Label - affichage du score)
    * MainCamera (Camera2D)
    * GlobalLight2D (optionnel)

-------------------------------------
🔄 CYCLE DE VIE DU JEU
-------------------------------------

1. _Ready() : Instancie SnakeGame et configure le jeu.
2. _Process(delta) : Met à jour la logique avec _engine.Update(delta).
3. Input : Les touches (flèches ou ZQSD) sont transmises à _engine.HandleInput().
4. LabelStatus : Affiche les informations du moteur (score, état, etc.).

-------------------------------------
🚀 TEST RAPIDE
-------------------------------------

1. Ouvrez la scène Main.tscn dans Godot.
2. Lancez le jeu (F5).
3. Le message "SnakeElysium.Core connected!" devrait s'afficher.
4. Utilisez les flèches ou ZQSD pour bouger le serpent.

-------------------------------------
🎯 OBJECTIF DU PROJET
-------------------------------------

* Séparation des responsabilités : Isoler la logique du jeu (Core) du rendu (Godot).
* Portabilité : Faciliter le portage entre différents moteurs (Godot, Unity, etc.).
* Architecture Modulaire :
    * Core = Logique pure du jeu.
    * Godot = Interface, affichage et gestion des inputs.
-------------------------------------
👤 AUTEUR
-------------------------------------
XAVIER Alfred
Amzallag Ariel
Développeur C++ / C# – passionné par les moteurs de jeu modulaires.
Projet : Elysium.Foundation.Serpentis

© 2025 Elysium Foundation. Tous droits réservés.
