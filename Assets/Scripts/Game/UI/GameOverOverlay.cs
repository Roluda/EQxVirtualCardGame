using EQx.Game.Investing;
using EQx.Game.Statistics;
using EQx.Game.Table;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace EQx.Game.UI {
    public class GameOverOverlay : MonoBehaviour {
        enum TrackType {
            Capital,
            VCP
        }

        [SerializeField]
        GameObject overlay = default;
        [SerializeField]
        TMP_Text winnerText = default;
        [SerializeField]
        string winnerAffix = " is the most valuable Elite.";
        [SerializeField]
        LineDiagram diagram = default;

        TrackType current = TrackType.VCP;

        // Start is called before the first frame update
        void Start() {
            overlay.SetActive(false);
            RoundManager.instance.onGameEnd += StartGameOver;
        }

        private void StartGameOver() {
            RoundManager.instance.onGameEnd -= StartGameOver;
            overlay.SetActive(true);
            var winner = InvestmentManager.instance.accounts.OrderByDescending(account => account.capital).First().player.playerName;
            winnerText.text = $"{winner}{winnerAffix}";
            StartCoroutine(ShowDiagram());
        }

        IEnumerator ShowDiagram() {
            yield return new WaitForSeconds(.1f);
            NextDiagram();
        }

        public void NextDiagram() {
            if (current == TrackType.Capital) {
                current = TrackType.VCP;
            } else {
                current = TrackType.Capital;
            }

            switch (current) {
                case TrackType.Capital:
                    diagram.ClearData();
                    foreach (var player in RoundManager.instance.registeredPlayers) {
                        diagram.AddLineData(PlayerObserver.instance.GetTrack(player).capital, player.playerName);
                    }
                    diagram.xScaleSegments = RoundManager.instance.maxRounds + 1;
                    diagram.labelX = "Round";
                    diagram.labelY = "Coins";
                    diagram.header = "Coins over Game";
                    diagram.Redraw();

                    break;
                case TrackType.VCP:
                    diagram.ClearData();
                    foreach (var player in RoundManager.instance.registeredPlayers) {
                        diagram.AddLineData(PlayerObserver.instance.GetTrack(player).valueCreationPercentile, player.playerName);
                    }
                    diagram.xScaleSegments = RoundManager.instance.maxRounds + 1;
                    diagram.labelX = "Round";
                    diagram.labelY = "VCP";
                    diagram.header = "Value Creation Percentile over Game";
                    diagram.Redraw();
                    break;
            }

        }

        public void PreviousDiagram() {
            NextDiagram();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
