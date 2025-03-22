using UnityEngine;

public class EnemyCanvasToCamera : MonoBehaviour
{
    // Ссылка на камеру игрока (если камера всегда Main Camera, можно использовать Camera.main)
    private Transform playerCamera;

    void Start()
    {
        // Инициализируем ссылку на основную камеру
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera не найдена! Убедитесь, что у камеры установлен тег 'MainCamera'.");
        }
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            Vector3 direction = transform.position - playerCamera.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Фиксируем вращение по оси X (например, оставляем Y-поворот)
            targetRotation.x = 0;
            targetRotation.z = 0;

            transform.rotation = targetRotation;
        }
    }
}