using Models.Enums;

namespace Models.Interfaces;

public interface IConditional
{
    public Condition Condition { get; set; }
}