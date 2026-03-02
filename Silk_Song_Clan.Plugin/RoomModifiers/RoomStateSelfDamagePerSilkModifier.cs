using System;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Silk_Song_Clan.Plugin
{
    /// <summary>
    /// Room state modifier that adds attack damage based on current Silk (DamagePerSilk).
    /// Mirrors the game's RoomStateSelfDamagePerMagicPowerModifier but scales with Silk.
    /// </summary>
    public sealed class RoomStateSelfDamagePerSilkModifier : RoomStateModifierBase, IRoomStateDamageModifier, IRoomStateModifier, ILocalizationParamInt, ILocalizationParameterContext
    {
        public int AdditionalDamagePerSilk { get; set; }

        public bool IsPreviewModeCopy { get; set; }

        public override void Initialize(RoomModifierData roomModifierData, SaveManager saveManager)
        {
            base.Initialize(roomModifierData, saveManager);
            AdditionalDamagePerSilk = roomModifierData.GetParamInt();
        }

        public RoomStateSelfDamagePerSilkModifier CopyForPreview()
        {
            var copy = new RoomStateSelfDamagePerSilkModifier();
            base.CopyBaseStateForPreview(copy);
            copy.AdditionalDamagePerSilk = AdditionalDamagePerSilk;
            copy.IsPreviewModeCopy = true;
            return copy;
        }

        public int GetModifiedMagicPowerDamage(ICoreGameManagers coreGameManagers)
        {
            return 0;
        }

        public int GetModifiedAttackDamage(Damage.Type damageType, CharacterState attackerState, bool requestingForCharacterStats, ICoreGameManagers coreGameManagers)
        {
            if (requestingForCharacterStats)
            {
                return GetDynamicInt(attackerState);
            }
            return 0;
        }

        public override int GetDynamicInt(CharacterState characterContext)
        {
            int silk = 0;
            if (characterContext.GetRoomStateModifiers().Contains(this) && characterContext.GetSpawnPoint(false) != null)
            {
                var container = Railend.GetContainer();
                var silkManager = container.GetInstance<SilkManager>();
                if (silkManager != null)
                {
                    silk = silkManager.GetCurrentSilk();
                }
            }
            return silk * AdditionalDamagePerSilk;
        }

        public override bool GetShowTooltip()
        {
            return false;
        }
    }
}
