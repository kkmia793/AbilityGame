# AbilityGame

## 作品概要
AbilityGame は、プレイヤーが様々なミニゲームを連続してプレイし、総合スコアを競うゲーム。
初めてリリースしたゲームであり、Unityの基礎を学んだ。
小学生の頃に正確な円を描きスコア化するゲームを遊び、昔遊んでいたゲームがどのようなプログラムで動いているのか知りたいという思いから制作した。

公開先: [AbilityGame on Unityroom](https://unityroom.com/games/humanapex)

---

## 使用言語・環境
- Unity (C#)
- 外部アセット
  - Easy Save (データ保存用)
  - UI Samples (ボタンデザイン)
- 使用ライブラリ
  - DOTween (アニメーション)
  - UniTask (非同期処理)

---

## 工夫点

### 1. 描画時の点追加の最適化

- プレイヤーが円を描く際、点を追加する処理では、前の点から一定距離以上離れた場合にのみ新しい点を追加しています。
- これにより、無駄な点追加を防ぎ、描画精度を保ちながらパフォーマンスを最適化しています。

```csharp
if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePosition) > distanceThreshold)
{
    AddPoint(mousePosition);
}
```

- `distanceThreshold` を小さくすることで、描画が滑らかに見えるよう調整しています。

### 2. 円の評価アルゴリズム

- 描いた円が適切かどうかを評価するために、中心点からの平均半径とその分散を計算し、スコア化しています。
- 分散が小さい（＝均一な円形）ほど高いスコアとなるように設計しています。

```csharp
float variance = 0f;
foreach (var point in points)
{
    float distance = Vector3.Distance(center, point);
    variance += Mathf.Abs(distance - averageRadius);
}
variance /= points.Count;

float score = Mathf.Clamp(100 - (variance / maxVariance) * 100, 0, 100);
```

- `maxVariance` を設定し、スコアが 0〜100 の範囲に収まるように正規化しています。

### 3. 動的なゲームフロー制御と非同期処理

- `GameFlowManager` クラスでは、`UniTask` を活用してゲームフローを非同期に制御しています。
- シーン遷移やカウントダウン、UI操作がスムーズに行えるようになり、ユーザー体験（UX）の向上に寄与しています。


### 4. デバッグモードの実装

- `debugMode` では特定のゲームのみをプレイすることが可能で、開発中のテスト効率を大幅に向上。
- テストサイクルの短縮や、特定機能のデバッグを容易にする工夫を行っています。

```csharp
if (debugMode)
{
    DebugModeStart().Forget();
}
else
{
    StartNextGame().Forget();
}
```

### 5. 柔軟なゲーム追加が可能な設計

- `BaseGame` 抽象クラスを基底に、新しいミニゲームを簡単に追加可能な設計になっています。
- ゲームをモジュール化することで、後からの機能追加や、別のプロジェクトへの流用が容易です。

```csharp
[SerializeField] private BaseGame[] games;
private int currentGameIndex = 0;

// 次のゲームを開始
private async UniTaskVoid StartNextGame()
{
    BaseGame currentGame = games[currentGameIndex];
    int score = await currentGame.PlayGame(uiManager);
}
```

### 6. シングルトンパターンの活用

- 比較的規模の小さいゲームのため、`GameFlowManager` はシングルトンとして実装し、シーンを跨いだデータ保持や操作を一元化。
- メモリリークを防ぎ、ゲーム全体の状態管理を効率化しています。

```csharp
private static GameFlowManager instance;  
public static GameFlowManager Instance => instance;

private void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);  
    }
    else
    {
        Destroy(gameObject);  
    }
}
```

---

## 今後の改善点
- 描画アルゴリズムの最適化（ベジェ曲線などの導入を検討）
- シーン遷移時のロード時間短縮
- UI/UX 改善（視覚効果やフィードバックの強化）

---

## 特記事項
- 本プロジェクトでは、外部アセットとして、データ保存に Easy Save、UI デザインに Unity の UI Samples を利用しました。
- アニメーションは DOTween を活用し、ゲーム中の非同期処理は UniTask により実装しています。
