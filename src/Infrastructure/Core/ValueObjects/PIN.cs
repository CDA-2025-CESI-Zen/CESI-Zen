namespace CesiZen.Infrastructure.Core.ValueObjects;
public readonly record struct PIN(uint Code) {
    public PIN() : this((uint)new Random().Next(0, 1_0000_0000)) {}
    public static implicit operator uint(PIN from) => from.Code;
    public static implicit operator PIN(uint from) => new (from);
    public override string ToString() => this.Code.ToString().PadLeft(8, '0').Insert(4, " ");
}
