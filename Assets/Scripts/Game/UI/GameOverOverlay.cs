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
        string coinsPrefix = "Most Competitive Elite: ";
        [SerializeField]
        string qualityPrefix = "Most Value Created (Elite Quality): ";
        [SerializeField]
        LineDiagram diagram = default;
        [SerializeField]
        string coinsDiagramTitle = "Accumulated coins (Wealth)";
        [SerializeField]
        string vcpDiagramTitle = "Average value creation percentage (Elite Quality)";

        TrackType current = TrackType.VCP;

        string winnerCoins;
        string winnerVCP;

        // Start is called before the first frame update
        void Start() {
            overlay.SetActive(false);
            RoundManager.instance.onGameEnd += StartGameOver;
        }

        private void StartGameOver() {
            RoundManager.instance.onGameEnd -= StartGameOver;
            overlay.SetActive(true);
            winnerCoins = InvestmentManager.instance.accounts.OrderByDescending(account => account.capital).First(account => account.isActive).player.playerName;
            winnerVCP = PlayerObserver.instance.playerTracks.OrderByDescending(track => track.valueCreationPercentile[RoundManager.instance.currentRound - 1]).First(track=>track.active).playerName;
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
                    winnerText.text = $"{coinsPrefix}{winnerCoins}";
                    diagram.ClearData();
                    diagram.maxMargin = 1.1f;
                    diagram.minMargin = .9f;
                    diagram.yScaleDecimals = 0;
                    foreach (var track in PlayerObserver.instance.playerTracks.OrderByDescending(trk => trk.capital[RoundManager.instance.currentRound])) {
                        diagram.AddLineData(track.capital, track.playerName);
                    }
                    diagram.labelX = "Round";
                    diagram.labelY = "Coins";
                    diagram.header = coinsDiagramTitle;
                    diagram.Redraw();

                    break;
                case TrackType.VCP:
                    winnerText.text = $"{qualityPrefix}{winnerVCP}";
                    diagram.ClearData();
                    diagram.maxMargin = 1;
                    diagram.minMargin = .9f;
                    Debug.Log($"Getting VCP for Round {RoundManager.instance.currentRound - 1}");
                    foreach (var track in PlayerObserver.instance.playerTracks.OrderByDescending(trk => trk.valueCreationPercentile[RoundManager.instance.currentRound-1])) {
                        diagram.AddLineData(track.valueCreationPercentile, track.playerName);
                    }
                    diagram.labelX = "Round";
                    diagram.labelY = "Value Creation %";
                    diagram.header = vcpDiagramTitle;
                    diagram.yScaleDecimals = 2;
                    diagram.Redraw();
                    break;
            }

        }

        public void PreviousDiagram() {
            NextDiagram();
        }   
    }
}
