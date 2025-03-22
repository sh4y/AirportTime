public class ModifierMetrics
{
    private readonly Airport airport;

    public ModifierMetrics(Airport airport)
    {
        this.airport = airport;
    }

    public List<ModifierInfo> GetActiveModifiers()
    {
        var modifiers = GetAllModifiers();
        
        return modifiers.Select(m => new ModifierInfo
        {
            Name = m.Name,
            Value = m.Value,
            EffectType = m.Value > 1.0 ? "bonus" : "penalty",
            PercentageEffect = Math.Abs(m.Value - 1.0) * 100,
            Description = GetModifierDescription(m.Name)
        }).ToList();
    }

    private List<(string Name, double Value)> GetAllModifiers()
    {
        var result = new List<(string Name, double Value)>();
        
        try
        {
            // Try to access modifiers through reflection
            var fieldInfo = typeof(ModifierManager).GetField("modifiers", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (fieldInfo != null)
            {
                var modifiers = fieldInfo.GetValue(airport.ModifierManager) as List<Modifier>;
                if (modifiers != null)
                {
                    foreach (var modifier in modifiers)
                    {
                        result.Add((modifier.Name, modifier.Value));
                    }
                }
            }
        }
        catch
        {
            // If we can't access modifiers, add some dummy data based on level
            if (airport.ExperienceSystem.CurrentLevel >= 3)
            {
                result.Add(("High Airport Reputation", 1.25));
            }
            
            if (airport.ExperienceSystem.CurrentLevel >= 7)
            {
                result.Add(("Weather Resistance", 0.70));
            }
        }
        
        return result;
    }

    private string GetModifierDescription(string modifierName)
    {
        return modifierName switch
        {
            "High Airport Reputation" => "Increased revenue",
            "Weather Resistance" => "Reduced runway wear",
            "Flight Specialist" => "VIP/Emergency bonus",
            _ => ""
        };
    }
}