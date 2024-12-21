using UnityEngine;

// Cubeの各面にRayを飛ばし、色を取得するクラス
public class CubeRaycaster : MonoBehaviour
{
    public Transform cubeCenterRaySource;  // Cubeの中心からRayを発射するオブジェクト
    
    public Transform[] rayTargets;  // 各面にRayを飛ばすターゲット
    
    public Color GetColorFromRay(int index)
    {
        if (index < 0 || index >= rayTargets.Length)
        {
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

        Debug.LogWarning($"{target.name} にRayが衝突してない");
        return Color.black;  // デフォルトの黒色を返す
    }
}