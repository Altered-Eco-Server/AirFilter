namespace Eco.Mods.TechTree
{
    using System;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Simulation.WorldLayers;
    using Eco.Simulation.Time;
    using Eco.Gameplay.Systems.Messaging.Chat.Commands;
    using Eco.Gameplay.Systems.Messaging.Notifications;
    using Eco.Shared.Services;
    using System.Runtime.CompilerServices;
    using Eco.Gameplay.Housing.PropertyValues;
    using static Eco.Gameplay.Housing.PropertyValues.HomeFurnishingValue;
    using System.Collections.Generic;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Shared.Math;
    using Eco.Simulation.WorldLayers.Layers;
    using Eco.Simulation;
    using Eco.Gameplay.Interactions;
    using Eco.AlteredCore;
    using Eco.Simulation.Settings;
    using Eco.Shared.Utils;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    [RequireComponent(typeof(FuelSupplyComponent))]
    [RequireComponent(typeof(FuelConsumptionComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [RequireComponent(typeof(StatusCheckerComponent))]
    public partial class AirFilterObject : WorldObject, IRepresentsItem
    {
        public static bool isFiltering;
        private static double tickTime = TimeUtil.MinutesToSeconds(5);
        public override LocString DisplayName { get { return Localizer.DoStr("Air Filter"); } }
        public virtual Type RepresentedItemType { get { return typeof(AirFilterItem); } }
        public double timeSent = 0;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<FuelSupplyComponent>().Initialize(2, new[] { "Filter" });
            this.GetComponent<FuelConsumptionComponent>().Initialize(2);
            this.GetComponent<PowerConsumptionComponent>().Initialize(250);
            this.GetComponent<PowerGridComponent>().Initialize(10, new ElectricPower());
            this.GetComponent<StatusCheckerComponent>().SuccessStatusMessage = Localizer.DoStr("Air Filter is able to filter CO2.");
            this.GetComponent<StatusCheckerComponent>().FailStatusMessage = Localizer.DoStr("Not enough CO2 in the atmosphere.");
            this.GetComponent<StatusCheckerComponent>().EnabledTest = this.StatusTest;
            this.GetComponent<StatusCheckerComponent>().UpdateEnabled();
            this.ModsPostInitialize();
        }

        public override void Tick()
        {
            var totalCo2 = WorldLayerManager.Obj.Climate.TotalCO2;
            var layer = WorldLayerManager.Obj.GetLayer("AirPollutionSpread");
            base.Tick();
            if (totalCo2 > EcoDef.Obj.ClimateSettings.MinCO2ppm)
            {
                enabled = true;
                isFiltering = true;
                if (WorldTime.Seconds - timeSent >= tickTime && Operating)
                {
                    var removed = -0.25f * (totalCo2 / 400);
                    string message = " Filtered " + (removed * -1) + " CO2 in last 5 Minutes";
                    WorldLayerManager.Obj.ClimateSim.State.TotalCO2 += removed;
                    if (layer.EntryWorldPos((int)Position.X, (int)Position.Z) > 0)
                    {
                        layer.SetAtWorldPos(new Vector2i((int)Position.X, (int)Position.Z), -1f);
                    }
                    timeSent = WorldTime.Seconds;
                }
            }
            else
            {
                isFiltering = false;
                enabled = false;
            }
            StatusTest(this.Position3i);
            this.GetComponent<StatusCheckerComponent>().UpdateEnabled();
            this.GetComponent<StatusCheckerComponent>().UpdateStatus();
            UpdateEnabledAndOperating();
            SetAnimatedState("cleanAir", isFiltering && Operating);
        }

        static AirFilterObject()
        {
            AddOccupancy<AirFilterObject>(new List<BlockOccupancy>()
            {    // Front Row
            new BlockOccupancy(new Vector3i(-1, 0, 0)),
            new BlockOccupancy(new Vector3i(-1, 0, 1)),
            new BlockOccupancy(new Vector3i(-1, 0, 2)),
            new BlockOccupancy(new Vector3i(-1, 0, 3)),
            new BlockOccupancy(new Vector3i(-1, 0, -1)),
            new BlockOccupancy(new Vector3i(-1, 0, -2)),
            new BlockOccupancy(new Vector3i(-1, 0, -3)),
            new BlockOccupancy(new Vector3i(-1, 1, 0)),
            new BlockOccupancy(new Vector3i(-1, 1, 1)),
            new BlockOccupancy(new Vector3i(-1, 1, 2)),
            new BlockOccupancy(new Vector3i(-1, 1, 3)),
            new BlockOccupancy(new Vector3i(-1, 1, -1)),
            new BlockOccupancy(new Vector3i(-1, 1, -2)),
            new BlockOccupancy(new Vector3i(-1, 1, -3)),
            new BlockOccupancy(new Vector3i(-1, 2, 0)),
            new BlockOccupancy(new Vector3i(-1, 2, 1)),
            new BlockOccupancy(new Vector3i(-1, 2, 2)),
            new BlockOccupancy(new Vector3i(-1, 2, 3)),
            new BlockOccupancy(new Vector3i(-1, 2, -1)),
            new BlockOccupancy(new Vector3i(-1, 2, -2)),
            new BlockOccupancy(new Vector3i(-1, 2, -3)),
                // Middle Row
            new BlockOccupancy(new Vector3i(0, 0, 0)),
            new BlockOccupancy(new Vector3i(0, 0, 1)),
            new BlockOccupancy(new Vector3i(0, 0, 2)),
            new BlockOccupancy(new Vector3i(0, 0, 3)),
            new BlockOccupancy(new Vector3i(0, 0, -1)),
            new BlockOccupancy(new Vector3i(0, 0, -2)),
            new BlockOccupancy(new Vector3i(0, 0, -3)),
            new BlockOccupancy(new Vector3i(0, 1, 0)),
            new BlockOccupancy(new Vector3i(0, 1, 1)),
            new BlockOccupancy(new Vector3i(0, 1, 2)),
            new BlockOccupancy(new Vector3i(0, 1, 3)),
            new BlockOccupancy(new Vector3i(0, 1, -1)),
            new BlockOccupancy(new Vector3i(0, 1, -2)),
            new BlockOccupancy(new Vector3i(0, 1, -3)),
            new BlockOccupancy(new Vector3i(0, 2, 0)),
            new BlockOccupancy(new Vector3i(0, 2, 1)),
            new BlockOccupancy(new Vector3i(0, 2, 2)),
            new BlockOccupancy(new Vector3i(0, 2, 3)),
            new BlockOccupancy(new Vector3i(0, 2, -1)),
            new BlockOccupancy(new Vector3i(0, 2, -2)),
            new BlockOccupancy(new Vector3i(0, 2, -3)),
                // Back Row
            new BlockOccupancy(new Vector3i(1, 0, 0)),
            new BlockOccupancy(new Vector3i(1, 0, 1)),
            new BlockOccupancy(new Vector3i(1, 0, 2)),
            new BlockOccupancy(new Vector3i(1, 0, 3)),
            new BlockOccupancy(new Vector3i(1, 0, -1)),
            new BlockOccupancy(new Vector3i(1, 0, -2)),
            new BlockOccupancy(new Vector3i(1, 0, -3)),
            new BlockOccupancy(new Vector3i(1, 1, 0)),
            new BlockOccupancy(new Vector3i(1, 1, 1)),
            new BlockOccupancy(new Vector3i(1, 1, 2)),
            new BlockOccupancy(new Vector3i(1, 1, 3)),
            new BlockOccupancy(new Vector3i(1, 1, -1)),
            new BlockOccupancy(new Vector3i(1, 1, -2)),
            new BlockOccupancy(new Vector3i(1, 1, -3)),
            new BlockOccupancy(new Vector3i(1, 2, 0)),
            new BlockOccupancy(new Vector3i(1, 2, 1)),
            new BlockOccupancy(new Vector3i(1, 2, 2)),
            new BlockOccupancy(new Vector3i(1, 2, 3)),
            new BlockOccupancy(new Vector3i(1, 2, -1)),
            new BlockOccupancy(new Vector3i(1, 2, -2)),
            new BlockOccupancy(new Vector3i(1, 2, -3)),
            });
        }

        public bool StatusTest(Vector3i pos)
        {
            return isFiltering;
        }

        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Air Filter")]
    public partial class AirFilterItem : WorldObjectItem<AirFilterObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Removes Co2 from the atmosphere");
        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0
                    | DirectionAxisFlags.Down
                ;
        [TooltipChildren] public HomeFurnishingValue HousingTooltip { get { return HomeValue; } }
        public static readonly HomeFurnishingValue HomeValue = new HomeFurnishingValue()
        {
            Category = RoomCategory.Industrial,
            TypeForRoomLimit = Localizer.DoStr(""),
        };
    }

    [RequiresSkill(typeof(IndustrySkill), 6)]
    public partial class AirFilterRecipe : RecipeFamily
    {
        public AirFilterRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "AirFilter",  //noloc
                Localizer.DoStr("Air Filter"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(SteelBarItem), 24, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                    new IngredientElement(typeof(GearboxItem), 8, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                    new IngredientElement(typeof(AdvancedCircuitItem), 3, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<AirFilterItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 8;
            this.LaborInCalories = CreateLaborInCaloriesValue(700, typeof(IndustrySkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(AirFilterRecipe), 6, typeof(IndustrySkill), typeof(IndustryFocusedSpeedTalent), typeof(IndustryParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Air Filter"), typeof(AirFilterRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(RoboticAssemblyLineObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}