using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// スキル名の取得
    /// </summary>
    /// <returns></returns>
    public string GetName();

    /// <summary>
    /// スキルの初期設定
    /// </summary>
    public void DataSetting();

    /// <summary>
    /// スキルを使用するために必要なキーバインド
    /// </summary>
    /// <returns></returns>
    public KeyCode InputKey();
    
    /// <summary>
    /// スキルが使用可能か確認
    /// </summary>
    /// <returns></returns>
    public bool SkillAvaible();

    /// <summary>
    /// スキルを使用した際の処理
    /// </summary>
    public void Activate();
}
