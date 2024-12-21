using UnityEngine;
using UnityEngine.UI;
using unityroom.Api;
using DG.Tweening;

// ゲームの結果画面を管理し、スコアとランクの表示を制御するクラス
public class ResultManager : MonoBehaviour
{
    public Text rankText;  
    public Text Score1Text;  
    public Text Score2Text;  
    public Text Score3Text;  

    private void Start()
    {
        // スコアをロード
        int game1Score = ES3.Load<int>("game1Score", 0);
        int game2Score = ES3.Load<int>("game2Score", 0);
        int game3Score = ES3.Load<int>("game3Score", 0);

        // スコア表示アニメーションを実行し、完了後にランクを表示
        DisplayScoreAnimation(
            new[] { Score1Text, Score2Text, Score3Text },
            new[] { game2Score, (game1Score / 2) + (game3Score / 2), game3Score },
            () => DisplayRankAnimation(game1Score, game2Score, game3Score)
        );
    }
    
    // ランクをアニメーション付きで表示
    private void DisplayRankAnimation(int game1Score, int game2Score, int game3Score)
    {
        float totalScore = game1Score + game2Score + game3Score;
        float rankScore = game2Score + game1Score / 2 + game3Score / 2 + game3Score;
        string rank = CalculateRank(totalScore);

        UnityroomApiClient.Instance.SendScore(1, rankScore, ScoreboardWriteMode.HighScoreDesc);

        // ランク表示の初期設定
        rankText.text = rank;
        rankText.transform.localScale = Vector3.one * 1.5f;  // 初期サイズは大きめ
        rankText.color = new Color(rankText.color.r, rankText.color.g, rankText.color.b, 0);  // 完全に透明にする

        SoundManager.Instance.PlaySE(SESoundData.SE.RankDisplay);  // ランク表示時のサウンド

        // フェードインとスケールアニメーション
        rankText.DOFade(1, 1f);  // 1秒かけてフェードイン
        rankText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);  // スケールを元に戻す
    }
    
    // スコアをアニメーション付きで順次表示
    private void DisplayScoreAnimation(Text[] scoreTexts, int[] scores, TweenCallback onComplete, int index = 0)
    {
        if (index >= scoreTexts.Length)
        {
            onComplete?.Invoke();  // 全スコアの表示が完了したらコールバックを実行
            return;
        }

        SoundManager.Instance.PlaySE(SESoundData.SE.ScoreDisplay);  // スコア表示時のサウンド

        DOTween.To(() => 0, x => scoreTexts[index].text = x.ToString().PadLeft(3), scores[index], 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => DisplayScoreAnimation(scoreTexts, scores, onComplete, index + 1));
    }
    
    // 合計スコアに基づいてランクを計算
    private string CalculateRank(float totalScore)
    {
        if (totalScore >= 300)
            return "X";
        else if (totalScore >= 270)
            return "S";
        else if (totalScore >= 240)
            return "A";
        else if (totalScore >= 210)
            return "B";
        else if (totalScore >= 180)
            return "C";
        else
            return "D";
    }
}
