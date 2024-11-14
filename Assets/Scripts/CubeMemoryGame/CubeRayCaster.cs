using UnityEngine;

/// <summary>
/// Cubeの各面にRayを飛ばし、色を取得するクラス
/// </summary>
public class CubeRaycaster : MonoBehaviour
{
    [Tooltip("Cubeの中心からRayを発射するオブジェクト")]
    public Transform cubeCenterRaySource;  // Cubeの中心からRayを発射するオブジェクト

    [Tooltip("各面のターゲット（Front, BackなどのRayの着弾点）")]
    public Transform[] rayTargets;  // 各面にRayを飛ばすターゲット

    /// <summary>
    /// 指定されたインデックスの面の色を取得
    /// </summary>
    /// <param name="index">取得する面のインデックス</param>
    /// <returns>取得した色。失敗した場合は黒色を返す</returns>
    public Color GetColorFromRay(int index)
    {
        if (index < 0 || index >= rayTargets.Length)
        {
            Debug.LogError($"インデックス {index} が範囲外です。");
            return Color.black;
        }

        Transform target = rayTargets[index];
        Vector3 rayDirection = (target.position - cubeCenterRaySource.position).normalized;  // Cube中心からターゲットへの正規化された方向ベクトル
        Ray ray = new Ray(cubeCenterRaySource.position, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Rayが当たったオブジェクトの色を返す
                return renderer.material.color;
            }
        }

        Debug.LogWarning($"{target.name} にRayが衝突しませんでした。");
        return Color.black;  // デフォルトの黒色を返す
    }
}