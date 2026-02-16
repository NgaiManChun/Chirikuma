using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのスキル機能を登録、使用できるクラスです。
/// </summary>
public class SkillHolder : MonoBehaviour
{
    // 使用できるスキルリスト
    [SerializeField] List<ISkill> m_skills = new List<ISkill>();

#if UNITY_EDITOR
    // Editor（Inspector）上で登録されたスキル（名前）を確認可能
    [SerializeField, ReadOnly] private List<string> m_checkRegisterSkill = new List<string>();
#endif

    private void Start()
    {
        // 登録されたスキルの設定関数を呼び出し
        foreach (var skill in m_skills)
        {
            skill.DataSetting();
        }

    }

    private void Update()
    {
        foreach (var skill in m_skills)
        {
            // skillに割り振られたキーバインドをチェック
            if (Input.GetKeyDown(skill.InputKey()))
            {
                Debug.Log("<color=green>[PlayerLog]</color>入力確認");

                // スキルが使用可能状態か確認
                if (skill.SkillAvaible() == true)
                {
                    Debug.Log("<color=green>[PlayerLog]</color>発動");

                    // スキルの使用関数を呼び出し
                    skill.Activate();
                }
                // スキルが使用できなかった場合の処理
                else
                {

                    Debug.Log("<color=green>[PlayerLog]</color>発動キャンセル");
                }
            }
        }
    }

    /// <summary>
    /// スキルの登録
    /// </summary>
    /// <param name="_skill"></param>
    public void Register(ISkill _skill)
    {
        m_skills.Add(_skill);

        Debug.Log($"<color=green>[PlayerLog]</color>スキル - {_skill.GetName()}を取得しました。");

#if UNITY_EDITOR
        m_checkRegisterSkill.Add(_skill.GetName());
#endif
    }
}
