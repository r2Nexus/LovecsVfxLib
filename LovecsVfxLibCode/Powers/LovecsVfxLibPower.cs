using BaseLib.Abstracts;
using BaseLib.Extensions;
using LovecsVfxLib.LovecsVfxLibCode.Extensions;
using Godot;

namespace LovecsVfxLib.LovecsVfxLibCode.Powers;

public abstract class LovecsVfxLibPower : CustomPowerModel
{
    //Loads from LovecsVfxLib/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}