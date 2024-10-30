using UnityEngine;
using UnityEngine.UIElements;

public class ModifyColliderAndMaintainScale : MonoBehaviour
{
    public BoxCollider2D colliderA; // ��һ����ײ��
    public BoxCollider2D colliderB; // �ڶ�����ײ��

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ������������
        {
            // ��ȡ colliderA �ı߽�
            Bounds boundsA = colliderA.bounds;
            if (colliderB.bounds.Intersects(boundsA)) // �ж�������ײ���Ƿ��ཻ
            {

                // �����ཻ����
                //Bounds boundsA = colliderA.bounds;
                Bounds boundsB = colliderB.bounds;

                // �����ص�����
                Vector3 overlapMin = new Vector3(
                    Mathf.Max(boundsA.min.x, boundsB.min.x),
                    Mathf.Max(boundsA.min.y, boundsB.min.y),
                    0
                );

                Vector3 overlapMax = new Vector3(
                    Mathf.Min(boundsA.max.x, boundsB.max.x),
                    Mathf.Min(boundsA.max.y, boundsB.max.y),
                    0
                );

                // �����µ� ColliderB �ߴ�
                if (overlapMin.x < overlapMax.x && overlapMin.y < overlapMax.y) // ���ص�
                {
                    Debug.Log("����Collider2D���ཻ��");

                    // �����µĴ�С
                    Vector2 newSize = colliderB.size; // ��ȡ��ǰ��С
                    newSize.x -= (overlapMax.x - overlapMin.x); // ��ȥ�ص��������
                    newSize.y -= (overlapMax.y - overlapMin.y); // ��ȥ�ص�����߶�

                    // ȷ���µĴ�С����С��0
                    newSize.x = Mathf.Max(newSize.x, 0.1f);
                    newSize.y = 1;

                    // ��¼��Сǰ��λ��
                    Vector3 originalPosition = colliderB.transform.position;

                    // �ֶ����� GameObject �Ĵ�С���Ա������Ӿ�Ч����
                    // ͨ���ı� transform.localScale ��Ӱ�촫ͳ��ʾ
                    // ��������ı���
                    Vector3 scaleChange = new Vector3(newSize.x / colliderB.size.x, newSize.y / colliderB.size.y, 1);

                    // ���� colliderB �Ĵ�С
                    //colliderB.size = newSize;
                    // ������� GameObject �����ţ���Ϊ colliderB ֱ�Ӷ�Ӧ������� GameObject ������
                    //��ѡ������ colliderB ��ƫ������
                    colliderB.offset = new Vector2(colliderB.offset.x, 0);
                    float offsetX = (1 - scaleChange.x) / 2;
                    // 更新物体位置，确保左对齐
                    colliderB.transform.localScale = new Vector3(scaleChange.x,
                                                        1,
                                                        1);

                    var newPosx = originalPosition.x + offsetX;


                    // 确定对齐方式
                    if (boundsA.min.x < boundsB.min.x) // 判断左对齐
                    {
                        // 左对齐时，保持左侧位置不变
                        colliderB.transform.position += new Vector3((colliderB.size.x - scaleChange.x) / 2, 0, 0);
                    }
                    else // 右对齐
                    {
                        // 右对齐时，保持右侧位置不变
                        colliderB.transform.position -= new Vector3((colliderB.size.x - scaleChange.x) / 2, 0, 0);
                    }
                    //var newPosx = originalPosition.x + offsetX;
                    //Debug.Log("newPosx " + newPosx);
                    //colliderB.transform.localPosition = new Vector3(newPosx, originalPosition.y, originalPosition.z);

                }
            }
        }
    }
}
