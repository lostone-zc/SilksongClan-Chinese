using System;
using System.Collections.Generic;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Silk_Song_Clan.Plugin
{
    /// <summary>
    /// Status effect that adds attack damage per current Silk (DamagePerSilk) via a room modifier.
    /// Mirrors StatusEffectMagebladeState but scales with Silk instead of Magic Power.
    /// </summary>
    public class StatusEffectPinMasteryState : StatusEffectState
    {
        public const string StatusId = "pinmastery";

        private RoomStateSelfDamagePerSilkModifier? modifier;
        private CharacterState? affectedCharacter;

        protected override void CopyStateForPreviewInternal(StatusEffectState dest, List<IRoomStateModifier> characterRoomStateModifiers)
        {
            if (dest is not StatusEffectPinMasteryState destPinMastery)
            {
                return;
            }
            destPinMastery.modifier = modifier != null ? modifier.CopyForPreview() : null;
            destPinMastery.affectedCharacter = affectedCharacter;
            int idx = modifier != null ? characterRoomStateModifiers.IndexOf(modifier) : -1;
            if (idx >= 0 && destPinMastery.modifier != null)
            {
                characterRoomStateModifiers[idx] = destPinMastery.modifier;
            }
        }

        public override void OnStacksAdded(CharacterState character, int numStacksAdded, CharacterState.AddStatusEffectParams addStatusEffectParams, ICoreGameManagers coreGameManagers)
        {
            if (modifier == null)
            {
                affectedCharacter = character;
                var roomModifierData = new RoomModifierData();
                modifier = new RoomStateSelfDamagePerSilkModifier();
                modifier.Initialize(roomModifierData, coreGameManagers.GetSaveManager());
                modifier.IsPreviewModeCopy = IsPreviewModeCopy();
                character.AddNewCharacterRoomModifierState(modifier);
            }
            modifier.AdditionalDamagePerSilk = character.GetStatusEffectStacks(StatusId) * GetMagnitudePerStack();
        }

        public override void OnStacksRemoved(CharacterState character, int numStacksRemoved, ICoreGameManagers coreGameManagers)
        {
            if (modifier == null)
            {
                return;
            }
            modifier.AdditionalDamagePerSilk = character.GetStatusEffectStacks(StatusId) * GetMagnitudePerStack();
        }

        public override int GetEffectMagnitude(int stacks = 1)
        {
            if (affectedCharacter == null)
            {
                return 0;
            }
            if (affectedCharacter.IsDestroyed || affectedCharacter.IsDeadAndUnrevivable)
            {
                return 0;
            }
            var container = Railend.GetContainer();
            var silkManager = container.GetInstance<SilkManager>();
            if (silkManager == null)
            {
                return 0;
            }
            int silk = silkManager.GetCurrentSilk();
            return GetMagnitudePerStack() * stacks * silk;
        }

        public override int GetMagnitudePerStack()
        {
            return GetParamInt() + relicManager.GetModifiedStatusMagnitudePerStack(StatusId, GetAssociatedCharacter().GetTeamType());
        }
    }
}
