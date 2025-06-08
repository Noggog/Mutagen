namespace Mutagen.Bethesda.Fallout3;

public partial class Model
{
    [Flags]
    public enum FaceGenFlag
    {
        Head = 0x0001,
        Torso = 0x0002,
        RightHand = 0x0004,
        LeftHand = 0x0008,
    }
}