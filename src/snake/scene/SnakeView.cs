using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using Godot;
using Snake.InputHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake.Views
{
    public partial class SnakeView : Node2D
    {
        private SnakeGame _engine;
        private GameConfig _config;
        private Sprite2D _snakeHeadSprite;
        private readonly Node2D _snakeBodyNode = new();
        private Sprite2D _backgroundSprite;
        private readonly Dictionary<Direction, Texture2D> _snakeHeadTexture = new()
        {
            [Direction.Up] = GD.Load<Texture2D>("res://assets/snake/head_up.png"),
            [Direction.Down] = GD.Load<Texture2D>("res://assets/snake/head_down.png"),
            [Direction.Left] = GD.Load<Texture2D>("res://assets/snake/head_left.png"),
            [Direction.Right] = GD.Load<Texture2D>("res://assets/snake/head_right.png"),
        };
        private readonly Dictionary<string, Texture2D> _snakeBodyTextures = new()
        {

            ["horizontal"] = GD.Load<Texture2D>("res://assets/snake/body_horizontal.png"),
            ["vertical"] = GD.Load<Texture2D>("res://assets/snake/body_vertical.png"),
            ["body_topleft"] = GD.Load<Texture2D>("res://assets/snake/body_topleft.png"),
            ["body_topright"] = GD.Load<Texture2D>("res://assets/snake/body_topright.png"),
            ["body_bottomleft"] = GD.Load<Texture2D>("res://assets/snake/body_bottomleft.png"),
            ["body_bottomright"] = GD.Load<Texture2D>("res://assets/snake/body_bottomright.png"),
        };
        private Direction _currentDirection = Direction.Right;

        public void Init(GameConfig config, Sprite2D background, SnakeGame engine)
        {

            _config = config;
            _backgroundSprite = background;
            _engine = engine;


        }

        public override void _Ready()
        {
            _snakeHeadSprite = GetNode<Sprite2D>("head_right");
            AddChild(_snakeBodyNode);
        }

        public void UpdateGraphics(Snapshot snapshot)
        {

            var snakeHead = snapshot.Snake[0];
            _snakeHeadSprite.Position = GridUtils.CellToWorld(snakeHead, _backgroundSprite, _config);

            foreach (Node child in _snakeBodyNode.GetChildren())
            {
                child.QueueFree();
            }

            for (int i = 1; i < snapshot.Snake.Length; i++)
            {
                var bodySnake = snapshot.Snake[i];
                var bodySprite = new Sprite2D();
                bodySprite.Position = GridUtils.CellToWorld(bodySnake, _backgroundSprite, _config);


                if (i < snapshot.Snake.Length - 1)
                {
                    var previousSegment = snapshot.Snake[i - 1];
                    var nextSegment = snapshot.Snake[i + 1];

                    var right_up = previousSegment.X > bodySnake.X && nextSegment.Y < bodySnake.Y;
                    var up_right = previousSegment.Y < bodySnake.Y && nextSegment.X > bodySnake.X;
                    var left_up = previousSegment.X < bodySnake.X && nextSegment.Y < bodySnake.Y;
                    var up_left = previousSegment.Y < bodySnake.Y && nextSegment.X < bodySnake.X;
                    var bottom_right = previousSegment.X > bodySnake.X && nextSegment.Y > bodySnake.Y;
                    var right_bottom = previousSegment.Y > bodySnake.Y && nextSegment.X > bodySnake.X;

                    if (previousSegment.X == nextSegment.X)
                    {
                        bodySprite.Texture = _snakeBodyTextures["vertical"];
                    }
                    else if (previousSegment.Y == nextSegment.Y)
                    {
                        bodySprite.Texture = _snakeBodyTextures["horizontal"];
                    }
                    else
                    {
                        if (right_up || up_right)
                        {
                            bodySprite.Texture = _snakeBodyTextures["body_topright"];
                        }
                        else if (left_up || up_left)
                        {
                            bodySprite.Texture = _snakeBodyTextures["body_topleft"];
                        }
                        else if (bottom_right || right_bottom)
                        {
                            bodySprite.Texture = _snakeBodyTextures["body_bottomright"];
                        }
                        else
                        {
                            bodySprite.Texture = _snakeBodyTextures["body_bottomleft"];
                        }
                    }
                }
                else
                {
                    var previousSegment = snapshot.Snake[i - 1];

                    if (previousSegment.X < bodySnake.X)
                    {
                        bodySprite.Texture = GD.Load<Texture2D>("res://assets/snake/tail_right.png");
                    }
                    else if (previousSegment.X > bodySnake.X)
                    {
                        bodySprite.Texture = GD.Load<Texture2D>("res://assets/snake/tail_left.png");
                    }
                    else if (previousSegment.Y > bodySnake.Y)
                    {
                        bodySprite.Texture = GD.Load<Texture2D>("res://assets/snake/tail_up.png");
                    }
                    else
                    {
                        bodySprite.Texture = GD.Load<Texture2D>("res://assets/snake/tail_down.png");
                    }

                }

                _snakeBodyNode.AddChild(bodySprite);
            }

        }

        public override void _Input(InputEvent @event)
        {
            _currentDirection = SnakeInput.Handle(@event, _engine, _currentDirection);

            if (_snakeHeadTexture.TryGetValue(_currentDirection, out var texture) && texture != null)
            {
                _snakeHeadSprite.Texture = texture;
            }

        }
    }
}

