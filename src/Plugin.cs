using BepInEx;
using System.Security.Permissions;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace Deadass;

[BepInPlugin("com.dual.deadass", "Deadass", "1.0.0")]
sealed class Plugin : BaseUnityPlugin
{
    FAtlas atlas;

    public void OnEnable()
    {
        On.RainWorld.OnModsInit += Init;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
    }

    private void Init(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        try {
            atlas ??= Futile.atlasManager.LoadAtlas("sprites/NixFace");

            if (atlas == null) {
                Logger.LogWarning("nixface atlas not found! Reinstall the mod.");
            }
        }
        catch (System.Exception e) {
            Logger.LogError("nixface atlas failed to load: " + e);
        }
    }

    private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (atlas == null) {
            return;
        }

        var sprite = sLeaser.sprites[9];
        var name = sprite?.element?.name;
        if (name != null) {
            string blink = self.blink > 0 ? "B" : "";

            if (!self.player.Consious && atlas._elementsByName.TryGetValue("NixFaceB0", out var element)) {
                sprite.element = element;
            }
            else if (int.TryParse(name.Substring(name.Length - 1), out int spriteIndex) && atlas._elementsByName.TryGetValue($"NixFace{blink}{spriteIndex}", out var element2)) {
                sprite.element = element2;

                if (self.blink > 0) {
                    sprite.y -= 1;
                }
            }
        }
    }
}
