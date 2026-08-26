namespace me.cqp.luohuaming.iKun.Domain.Enums;

/// <summary>鲲主词缀（五行 + 风雷阴阳 + 无）</summary>
public enum Element
{
    None = 0,
    Metal,  // 金
    Wood,   // 木
    Water,  // 水
    Fire,   // 火
    Earth,  // 土
    Wind,   // 风
    Thunder, // 雷
    Yin,
    Yang,

    /// <summary>副词缀占位</summary>
    Affix = 10,
}