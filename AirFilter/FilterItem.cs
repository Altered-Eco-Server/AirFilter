namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Core.Items;
    using Eco.World;
    using Eco.World.Blocks;
    using Eco.Gameplay.Pipes;

    [Serialized]
    [LocDisplayName("Graphene Filter")]
    [Weight(100)]
    [Fuel(16000)]
    [Tag("Filter")]
    public partial class FilterItem : Item
    {
        public override LocString DisplayDescription { get { return Localizer.DoStr("Replacement filter for the Air Filter."); } }
    }

    [RequiresSkill(typeof(IndustrySkill), 5)]
    public partial class FilterRecipe : RecipeFamily
    {
        public FilterRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "GrapheneFilter",
                Localizer.DoStr("Graphene Filter"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(GrapheneItem), 4, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                    new IngredientElement(typeof(NylonFabricItem), 2, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                    new IngredientElement(typeof(SteelBarItem), 4, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<FilterItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5;
            this.LaborInCalories = CreateLaborInCaloriesValue(250, typeof(IndustrySkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(FilterRecipe), 3, typeof(IndustrySkill), typeof(IndustryFocusedSpeedTalent), typeof(IndustryParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Graphene Filter"), typeof(FilterRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(RoboticAssemblyLineObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}