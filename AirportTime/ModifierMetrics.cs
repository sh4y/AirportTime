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
    
    public Dictionary<FlightType, double> GetFlightTypeMultipliers() 
    {
        var result = new Dictionary<FlightType, double>();
        
        foreach (FlightType type in Enum.GetValues(typeof(FlightType)))
        {
            // In a real implementation, we'd get this from ModifierManager
            result[type] = type switch 
            {
                FlightType.VIP => 1.5,
                FlightType.Emergency => 1.5,
                FlightType.Cargo => 1.2,
                _ => 1.0
            };
        }
        
        return result;
    }
    
    public double GetEffectiveWeatherResistance() 
    {
        // In a real implementation, we'd query the actual weather resistance
        return 0.3; // Placeholder
    }
    
    public double GetEffectiveXpMultiplier() 
    {
        // In a real implementation, we'd query the XP system
        return 1.0 + (airport.ExperienceSystem.CurrentLevel * 0.1);
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