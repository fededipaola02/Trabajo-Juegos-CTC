using System;
using System.Collections.Generic;
using System.Linq;

namespace WordGame.Models
{
    public class GameManager
    {
        public enum GameState
        {
            Setup,
            Playing,
            Finished
        }

        public GameState State { get; private set; } = GameState.Setup;
        public List<Player> Players { get; private set; } = new List<Player>();
        public Player? CurrentPlayer { get; private set; }
        public List<string> UsedWords { get; private set; } = new List<string>();
        public string Theme { get; private set; } = string.Empty;
        public int TimeLimit { get; private set; }
        public int TimeRemaining { get; private set; }
        public bool IsVictory { get; private set; }
        public string EndReason { get; private set; } = string.Empty;

        private Random _random = new Random();
        private System.Timers.Timer? _timer;

        public event Action? OnStateChanged;

        public void StartGame(List<string> playerNames, int timeLimit, string theme)
        {
            Players.Clear();
            for (int i = 0; i < playerNames.Count; i++)
            {
                Players.Add(new Player { Id = i, Name = playerNames[i] });
            }

            TimeLimit = timeLimit;
            TimeRemaining = timeLimit;
            Theme = theme;
            UsedWords.Clear();
            
            State = GameState.Playing;

            NextTurn();
            StartTimer();
        }

        private void StartTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (sender, e) =>
            {
                TimeRemaining--;
                if (TimeRemaining <= 0)
                {
                    EndGame(true, "¡Tiempo agotado sin repetir palabras!");
                }
                NotifyStateChanged();
            };
            _timer.Start();
        }

        public bool SubmitWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return false;

            string normalizedWord = word.Trim().ToLower();

            if (UsedWords.Contains(normalizedWord))
            {
                EndGame(false, $"¡Palabra repetida ({normalizedWord})!");
                return false;
            }

            UsedWords.Add(normalizedWord);
            if (CurrentPlayer != null)
            {
                CurrentPlayer.WordsGuessed++;
            }
            NextTurn();
            return true;
        }

        public void NextTurn()
        {
            if (Players.Count == 0) return;
            
            if (Players.Count == 1)
            {
                CurrentPlayer = Players[0];
                NotifyStateChanged();
                return;
            }

            Player nextPlayer;
            do
            {
                int nextIndex = _random.Next(Players.Count);
                nextPlayer = Players[nextIndex];
            } while (CurrentPlayer != null && nextPlayer.Id == CurrentPlayer.Id);

            CurrentPlayer = nextPlayer;
            NotifyStateChanged();
        }

        private void EndGame(bool victory, string reason)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            IsVictory = victory;
            EndReason = reason;
            State = GameState.Finished;
            NotifyStateChanged();
        }

        public void Restart()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            State = GameState.Setup;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();
    }
}
