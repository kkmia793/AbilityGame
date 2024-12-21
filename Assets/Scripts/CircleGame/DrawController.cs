using System.Collections.Generic;
using UnityEngine;

public class DrawController : MonoBehaviour
{
    private CircleDrawingGame circleGame;  
    public LineRenderer lineRenderer;  // 描画に使用するLineRenderer
    private List<Vector3> points = new List<Vector3>();  // 描画する点のリスト
    public float distanceThreshold = 0.01f;  // 新しい点を追加する際の最小距離
    public float sePlayInterval = 0.1f;  // SEを再生する間隔
    private float seTimer = 0f;  // SE再生用のタイマー
    public bool IsReset;
    
    public void SetCircleGame(CircleDrawingGame game)
    {
        this.circleGame = game;
    }

    void Update()
    {
        // circleGameが存在し、かつ描画可能な場合のみ描画を許可
        if (circleGame != null && circleGame.CanDraw())
        {
            // マウスの左ボタンが押されている間、描画を実行する
            if (Input.GetMouseButton(0))
            {
                IsReset = false;
                // マウス位置をワールド座標に変換
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

                // 直前に追加した点と一定の距離が離れている場合、新しい点を追加
                if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePosition) > distanceThreshold)
                {
                    AddPoint(mousePosition);  // 新しい点を描画に追加
                }

                // 一定間隔でSEを再生
                seTimer -= Time.deltaTime;
                if (seTimer <= 0f)
                {
                    SoundManager.Instance.PlaySE(SESoundData.SE.Draw);
                    seTimer = sePlayInterval;  // タイマーをリセット
                }
            }
            // マウスボタンが離された時、描画完了と見なす
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsCircleTooSmall() || points.Count < 100)  // 条件を満たさない場合
                {
                    ResetDrawing();  // 描画をリセット
                    seTimer = 0f; //SE用のタイマーもリセット
                    SoundManager.Instance.PlaySE(SESoundData.SE.WrongAnswer);
                }
                else
                {
                    circleGame.SetHasDrawn(true);  // ゲームに描画完了を通知
                }
            }
        }
    }

    // 円が小さすぎるかどうかを判定する
    private bool IsCircleTooSmall()
    {
        float averageRadius = 0f;
        Vector3 center = Vector3.zero;

        foreach (var point in points)
        {
            center += point;
        }
        center /= points.Count;

        foreach (var point in points)
        {
            averageRadius += Vector3.Distance(center, point);
        }
        averageRadius /= points.Count;

        // ある基準の半径以下なら小さすぎると判断
        return averageRadius < 0.5f;  // 0.5fを閾値とする
    }


    // 点を追加して、LineRendererにその点を描画させる
    private void AddPoint(Vector3 point)
    {
        points.Add(point);  // 点のリストに新しい点を追加
        lineRenderer.positionCount = points.Count;  // LineRendererの点の数を更新
        lineRenderer.SetPosition(points.Count - 1, point);  // LineRendererに新しい点の位置を設定
    }

    // 描画をリセットするメソッド
    public void ResetDrawing()
    {
        points.Clear();  // 描画した点を全てクリア
        lineRenderer.positionCount = 0;  // LineRendererの点の数をリセット
        IsReset = true;
    }

    public int EvaluateCircle()
    {
        // 点の数が少なすぎる場合はスコアを計算できない
        if (points.Count < 40)
        {
            return 0;
        }

        // 全ての点の中心位置を計算する（円の中心を推定）
        Vector3 center = Vector3.zero;
        foreach (var point in points)
        {
            center += point;  // 点を合計していく
        }
        center /= points.Count;  // 点の合計を点の数で割り、中心を求める

        // 中心から各点までの距離の平均を計算（平均半径を推定）
        float averageRadius = 0f;
        foreach (var point in points)
        {
            averageRadius += Vector3.Distance(center, point);  // 中心からの距離を加算
        }
        averageRadius /= points.Count;  // 距離の合計を点の数で割る

        // 各点の距離が平均半径からどれだけばらついているかを計算（絶対値）
        float variance = 0f;
        foreach (var point in points)
        {
            float distance = Vector3.Distance(center, point);  // 各点の距離を計算
            variance += Mathf.Abs(distance - averageRadius);  // 距離のばらつき（絶対値）を加算
        }
        variance /= points.Count;  // 分散を点の数で割る

        // ばらつきが小さいほど高いスコアを計算する
        float maxVariance = Mathf.Max(averageRadius * 0.1f, 1.0f);  // 平均半径の 10% を最大分散とする
        float score = Mathf.Clamp(100 - (variance / maxVariance) * 100, 0, 100);  // スコアを計算し0〜100に制限

        return Mathf.RoundToInt(score);  
    }

    public bool GetIsReset()
    {
        return IsReset;
    }

}
