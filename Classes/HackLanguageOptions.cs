public class HackLanguageOptions
{
    public int UserVariableStartingAddress { get; set; }
    public int MinAddress { get; set; }
    public int MaxAddress { get; set; }
    public List<Symbol> SymbolMap { get; set; }
    public List<Comp> CompMap { get; set; }
    public List<Dest> DestMap { get; set; }
    public List<Jump> JumpMap { get; set; }
}