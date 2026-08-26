using me.cqp.luohuaming.iKun.Domain.PetAttributes;

namespace me.cqp.luohuaming.iKun.Domain.Results;

/// <summary>幻化结果</summary>
public sealed class TransmogrifyResult
{
    public bool Success { get; init; } = true;

    public double WeightLoss { get; init; }

    public double CurrentWeight { get; init; }

    public bool Died { get; init; }

    public PetAttribute CurrentMain { get; init; } = null!;

    public PetAttribute CurrentAffix1 { get; init; } = null!;

    public PetAttribute CurrentAffix2 { get; init; } = null!;

    public PetAttribute OriginalMain { get; init; } = null!;

    public PetAttribute OriginalAffix1 { get; init; } = null!;

    public PetAttribute OriginalAffix2 { get; init; } = null!;

    public override string ToString() =>
        $"执行成功={Success}，体重减量={WeightLoss}，当前体重={CurrentWeight}，死亡={Died}，" +
        $"新词条：{CurrentMain.Name}[{CurrentMain.Element}] {CurrentAffix1.Name}[{CurrentAffix1.AffixId}] {CurrentAffix2.Name}[{CurrentAffix2.AffixId}]，" +
        $"原始词条：{OriginalMain.Name}[{OriginalMain.Element}] {OriginalAffix1.Name}[{OriginalAffix1.AffixId}] {OriginalAffix2.Name}[{OriginalAffix2.AffixId}]";
}