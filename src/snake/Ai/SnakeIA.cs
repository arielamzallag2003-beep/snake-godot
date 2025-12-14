using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elysium.Foundation.Serpentis.Core.Domain;
using Snake.SaveData;

namespace Snake.AI
{
    public partial class SnakeIA : Node
    {
        private IntPtr _model = IntPtr.Zero;
        private bool _isTrained = false;
        private readonly int[] _modelArchi = { 10, 32, 32, 4 }; 
        private const double W = 30.0, H = 20.0;

        public override void _Ready() => TrainModel();

        public override void _ExitTree()
        {
            if (_model != IntPtr.Zero) 
            { 
                Dll.deletePMC(_model); 
                _model = IntPtr.Zero; 
            }
        }

        // préparation des entrées pour le modèle
        private double[] GetInputs(double dx, double dy, int wU, int wD, int wL, int wR, int bU, int bD, int bL, int bR)
        {
            return new double[] { dx / W, dy / H, wU, wD, wL, wR, bU, bD, bL, bR };
        }

        public void TrainModel(string pathArg = "")
        {
            string path;

            
            if (!string.IsNullOrEmpty(pathArg))
            {
                path = pathArg;
                GD.Print($"chargement du dataset depuis : {path}");
            }
            else
            {
                // "res://"
                path = "res://dataset.txt";
                GD.Print("chargement du dataset par défaut.");
            }

            
            string[] lines;

            
            if (path.StartsWith("res://"))
            {
                if (!Godot.FileAccess.FileExists(path))
                {
                    GD.PrintErr($"dataset introuvable {path}");
                    return;
                }

                using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
                string content = file.GetAsText();
               
                lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
            else
            {
                if (!System.IO.File.Exists(path))
                {
                    GD.PrintErr($"dataset introuvable sur le disque à : {path}");
                    return;
                }
                lines = System.IO.File.ReadAllLines(path);
            }

            
            var uniqueData = new Dictionary<string, (double[], int)>();

            foreach (var line in lines.Skip(1)) // skip le header
            {
                var p = line.Split(',');
                if (p.Length < 11) continue;

                // Parsing direct
                double dx = double.Parse(p[0], System.Globalization.CultureInfo.InvariantCulture);
                double dy = double.Parse(p[1], System.Globalization.CultureInfo.InvariantCulture);
                int action = int.Parse(p[10]);

                var inputs = GetInputs(dx, dy,
                    int.Parse(p[2]), int.Parse(p[3]), int.Parse(p[4]), int.Parse(p[5]),
                    int.Parse(p[6]), int.Parse(p[7]), int.Parse(p[8]), int.Parse(p[9]));

                string key = string.Join("|", inputs.Select(x => x.ToString("F2"))) + "|" + action;
                uniqueData.TryAdd(key, (inputs, action));
            }
            // Tableau de 4 listes
            var sortedData = new List<double[]>[4];
            for (int i = 0; i < 4; i++) sortedData[i] = new List<double[]>();

            foreach (var item in uniqueData.Values)
                if (item.Item2 >= 0 && item.Item2 <= 3) sortedData[item.Item2].Add(item.Item1);

            int minCount = sortedData.Min(l => l.Count);
            if (minCount == 0) { GD.PrintErr("pas assez de données."); return; }

            int targetCount = Math.Min(minCount, 1000);
            GD.Print($"entraînement : {uniqueData.Count} uniques  {targetCount} par direction.");

            var finalInputs = new List<double[]>();
            var finalOutputs = new List<double[]>();
            var rng = new Random(42);

            for (int dir = 0; dir < 4; dir++)
            {
                var shuffled = sortedData[dir].OrderBy(x => rng.Next()).Take(targetCount).ToList();
                foreach (var input in shuffled)
                {
                    finalInputs.Add(input);
                    var output = new double[4]; output[dir] = 1.0;
                    finalOutputs.Add(output);
                }
            }

            int n = finalInputs.Count;
            while (n > 1)
            {
                n--; int k = rng.Next(n + 1);
                (finalInputs[k], finalInputs[n]) = (finalInputs[n], finalInputs[k]);
                (finalOutputs[k], finalOutputs[n]) = (finalOutputs[n], finalOutputs[k]);
            }

            if (_model != IntPtr.Zero) Dll.deletePMC(_model);
            _model = Dll.createPMC(_modelArchi, _modelArchi.Length);

            Dll.trainPMC(_model,
                finalInputs.SelectMany(x => x).ToArray(),
                finalOutputs.SelectMany(x => x).ToArray(),
                finalInputs.Count, _modelArchi[0], _modelArchi.Last(),
                100000, 0.01, true);

            _isTrained = true;
            GD.Print("ia pret ");
        }

        public Direction PredictMove(Snapshot snapshot)
        {
            if (!_isTrained || _model == IntPtr.Zero)
            {
                return Direction.Right;
            }

            var head = snapshot.Snake[0];
            var food = snapshot.Food;

            // préparation des entrées
            double[] inputs = GetInputs(
                food.X - head.X,
                food.Y - head.Y,
                head.Y <= 1 ? 1 : 0, head.Y >= H - 2 ? 1 : 0, head.X <= 1 ? 1 : 0, head.X >= W - 2 ? 1 : 0, // murs
                snapshot.Snake.Any(p => p.X == head.X && p.Y == head.Y - 1) ? 1 : 0, // haut
                snapshot.Snake.Any(p => p.X == head.X && p.Y == head.Y + 1) ? 1 : 0,  // bas
                snapshot.Snake.Any(p => p.X == head.X - 1 && p.Y == head.Y) ? 1 : 0, // gauche
                snapshot.Snake.Any(p => p.X == head.X + 1 && p.Y == head.Y) ? 1 : 0// droite
            );

            double[] p = new double[4];
            Dll.predictPMC(_model, inputs, _modelArchi[0], true, p);

            
            int bestAction = 0;
            double maxVal = p[0];
            for (int i = 1; i < 4; i++) 
            { 
                if (p[i] > maxVal) 
                { 
                    maxVal = p[i]; bestAction = i; 
                } 
            }

            return bestAction switch { 0 => Direction.Up, 1 => Direction.Down, 2 => Direction.Left, _ => Direction.Right };
        }
    }
}